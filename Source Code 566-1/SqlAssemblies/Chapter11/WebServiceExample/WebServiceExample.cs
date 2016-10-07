using System;
using System.Collections;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using Microsoft.SqlServer.Server;

namespace Apress.SqlAssemblies.Chapter11
{
   public class WebServiceExample
   {
      [SqlProcedure]
      public static void GetCityData(int addressID)
      {
         try
         {

            using (SqlConnection cn = new SqlConnection("context connection=true"))
            {
               cn.Open();
               string selectQuery = @"SELECT a.City, s.Name As State, c.Name As Country
                                      FROM Person.Address a
                                      INNER JOIN Person.StateProvince s
                                      ON a.StateProvinceID = s.StateProvinceID
                                         INNER JOIN Person.CountryRegion c
                                         ON s.CountryRegionCode = c.CountryRegionCode
                                      WHERE a.AddressID = @addressID";
               SqlCommand selectCmd = new SqlCommand(selectQuery, cn);
               selectCmd.Parameters.AddWithValue("@addressID", addressID);
               SqlDataReader reader = selectCmd.ExecuteReader();
               if (reader.HasRows)
               {
                  reader.Read();
                  string city = (string)reader[0];
                  string state = (string)reader[1];
                  string country = (string)reader[2];
                  reader.Close();
                  string placeName = city + ", " + state + ", " + country;

                  string insertQuery = "INSERT INTO Person.CityDetails VALUES (@addressID, @name, @longitude, @latitude, @population, @image)";
                  SqlCommand insertCmd = new SqlCommand(insertQuery, cn);
                  SqlParameter addressIDParam = new SqlParameter("@addressID", SqlDbType.Int);
                  SqlParameter nameParam = new SqlParameter("@name", SqlDbType.NVarChar, 256);
                  SqlParameter longParam = new SqlParameter("@longitude", SqlDbType.Float);
                  SqlParameter latParam = new SqlParameter("@latitude", SqlDbType.Float);
                  SqlParameter popParam = new SqlParameter("@population", SqlDbType.Int);
                  SqlParameter imgParam = new SqlParameter("@image", SqlDbType.Image);
                  insertCmd.Parameters.AddRange(new SqlParameter[] { addressIDParam, nameParam, longParam, latParam, popParam, imgParam });
                  addressIDParam.Value = addressID;

                  TerraService terraService = new TerraService();
                  PlaceFacts[] places = terraService.GetPlaceList(placeName, 100, false);
                  foreach (PlaceFacts facts in places)
                  {
                     LonLatPt coords = facts.Center;
                     TileMeta metadata = terraService.GetTileMetaFromLonLatPt(coords, 1, Scale.Scale8m);
                     byte[] image = terraService.GetTile(metadata.Id);
                     nameParam.Value = facts.Place.City;
                     longParam.Value = coords.Lon;
                     latParam.Value = coords.Lat;
                     popParam.Value = facts.Population;
                     imgParam.Value = image;
                     try
                     {
                        insertCmd.ExecuteNonQuery();
                     }
                     catch (Exception e)
                     {
                        SqlContext.Pipe.Send("Cannot insert row for place " + facts.Place.City);
                        SqlContext.Pipe.Send(e.Message);
                     }
                  }
                  cn.Close();
      
                  SqlContext.Pipe.Send("Command executed successfully.");
                  terraService.Dispose();
               }
               else
               {
                  reader.Close();
                  cn.Close();
                  SqlContext.Pipe.Send("No addresses in the database match the specified ID.");
               }
            }
         }
         catch (Exception e)
         {
            SqlContext.Pipe.Send("An error occurred executing the GetCityData stored procedure:");
            SqlContext.Pipe.Send(e.Message); 
         }
 

     }
   }
}