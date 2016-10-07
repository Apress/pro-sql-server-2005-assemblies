using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.DirectoryServices;
using System.Text;
using Microsoft.SqlServer.Server;

namespace Apress.SqlAssemblies.Chapter05
{
   public class TableUdfExample
   {
      public struct TableRow
      {
         public int Id;
         public string Name;
         public string Email;
         public string ProxyAddresses;
         public string ADsPath;
         public string UserName;
         public SqlGuid Guid;

         public TableRow(int id, string name, string email, string proxyAddresses, string adsPath, string userName, SqlGuid guid)
         {
            Id = id;
            Name = name;
            Email = email;
            ProxyAddresses = proxyAddresses;
            ADsPath = adsPath;
            UserName = userName;
            Guid = guid;
         }
      }

      [SqlFunction(DataAccess=DataAccessKind.Read, FillRowMethodName="GetRowData")]
      public static IEnumerable ReadDirectoryData()
      {
         ArrayList entries = new ArrayList();

         DirectoryEntry root = new DirectoryEntry("LDAP://daufiltri.julianskinner.local/CN=Users,DC=JulianSkinner,DC=local");
         if (root == null)
            throw new NullReferenceException("The directory service cannot be referenced.");

         using (SqlConnection cn = new SqlConnection("context connection=true"))
         {
            string sql = "SELECT id, name, email FROM employees";
            cn.Open();
            SqlCommand cmd = new SqlCommand(sql, cn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
               string email = (string)reader[2];
               DirectorySearcher searcher = new DirectorySearcher(root, "(&(objectClass=user)(mail=" + email + "))");
               searcher.SearchScope = SearchScope.OneLevel;
               SearchResult result = searcher.FindOne();

               TableRow row;
               if (result == null)
               {
                  row = new TableRow((int)reader[0], (string)reader[1], email, null, null, null, SqlGuid.Null);
               }
               else
               {
                  DirectoryEntry de = result.GetDirectoryEntry();
                  PropertyValueCollection proxyAddresses = de.Properties["proxyAddresses"];
                  StringBuilder sb = new StringBuilder();
                  for (int i = 0; i < proxyAddresses.Count; i++)
                  {
                     sb.Append(proxyAddresses[i]);
                     if (i < proxyAddresses.Count - 1)
                        sb.Append(",");
                  }
                  PropertyValueCollection userNameVals = de.Properties["userPrincipalName"];
                  string userName = userNameVals.Count > 0 ? userNameVals.Value.ToString() : null;
                  row = new TableRow((int)reader[0], (string)reader[1], email, sb.ToString(), result.Path, userName, new SqlGuid(de.Guid));
               }
               entries.Add(row);
            }
            reader.Close();
            cn.Close();
         }
         return entries;
      }

      public static void GetRowData(object o, out SqlInt32 id, out SqlChars name, out SqlChars email, out SqlChars proxyAddresses, out SqlChars adsPath, out SqlChars userName, out SqlGuid guid)
      {
         TableRow row = (TableRow)o;
         id = new SqlInt32(row.Id);
         name = new SqlChars(row.Name);
         email = new SqlChars(row.Email);
         proxyAddresses = new SqlChars(row.ProxyAddresses);
         adsPath = new SqlChars(row.ADsPath);
         userName = new SqlChars(row.UserName);
         guid = row.Guid;
      }
   }
}
