using System;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Net;
using System.Security;
using System.Security.Permissions;
using Microsoft.SqlServer.Server;

namespace Apress.SqlAssemblies.Chapter10
{
   public class CasExample
   {
      [SqlProcedure]
      public static void InsertLatestStockPrice(string symbol)
      {
         try
         {
            PermissionSet perms = new PermissionSet(PermissionState.None);
            string url = "http://finance.yahoo.com/d/quotes.csv?s=" + symbol +
                      "&f=sl1d1t1c1ov";
            WebPermission webPerm = new WebPermission(NetworkAccess.Connect, url);
            perms.AddPermission(webPerm);

            SqlClientPermission sqlPerm = new SqlClientPermission(
                                                 PermissionState.None);
            sqlPerm.Add("context connection=true", "",
                        KeyRestrictionBehavior.AllowOnly);
            perms.AddPermission(sqlPerm);
            perms.PermitOnly();
            string[] data = HttpFileReader.ReadFile(url);
            string[] cols = data[0].Split(new char[] { ',' });

            string date = cols[2].Substring(1, cols[2].Length - 2);
            string time = cols[3].Substring(1, cols[3].Length - 2);
            DateTime tradetime = DateTime.Parse(date + " " + time);

            double price = Double.Parse(cols[1]);
            double change = Double.Parse(cols[4]);
            SqlDouble openprice = cols[5] == "N/A" ? SqlDouble.Null :
                                                     SqlDouble.Parse(cols[5]);
            int volume = Int32.Parse(cols[6]);

            using (SqlConnection cn = new SqlConnection("context connection=true"))
            {
               cn.Open();
               string cmdStr = "INSERT INTO StockPrices VALUES (@symbol, @price, @tradetime, @change, @openprice, @volume)";
               SqlCommand cmd = new SqlCommand(cmdStr, cn);
               cmd.Parameters.AddWithValue("@symbol", symbol);
               cmd.Parameters.AddWithValue("@price", price);
               cmd.Parameters.AddWithValue("@tradetime", tradetime);
               cmd.Parameters.AddWithValue("@change", change);
               cmd.Parameters.AddWithValue("@openprice", openprice);
               cmd.Parameters.AddWithValue("@volume", volume);
               cmd.ExecuteNonQuery();
            } 
         }
         catch (Exception e)
         {
            SqlPipe pipe = SqlContext.Pipe;
            pipe.Send(e.Message);
         }
      }
   }
}