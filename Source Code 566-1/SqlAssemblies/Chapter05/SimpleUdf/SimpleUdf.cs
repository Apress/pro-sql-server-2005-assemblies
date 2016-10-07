using System;
using System.DirectoryServices;
using System.Text;
using Microsoft.SqlServer.Server;

namespace Apress.SqlAssemblies.Chapter05
{
   public class SimpleUdf
   {
      [SqlFunction(DataAccess=DataAccessKind.None, IsDeterministic=false)]
      public static string GetProxyAddresses(string email)
      {
         DirectoryEntry root = new DirectoryEntry("LDAP://daufiltri.julianskinner.local/CN=Users,DC=JulianSkinner,DC=local");
         if (root == null)
            throw new NullReferenceException("The directory service cannot be referenced.");

         DirectorySearcher searcher = new DirectorySearcher(root, "(&(objectClass=user)(mail=" + email + "))");
         searcher.SearchScope = SearchScope.OneLevel;
         SearchResult result = searcher.FindOne();

         if (result == null)
         {
            return null;
         }
         else
         {
            ResultPropertyValueCollection propvals = result.Properties["proxyAddresses"];
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < propvals.Count; i++)
            {
               sb.Append(propvals[i]);
               if (i < propvals.Count - 1)
                  sb.Append(",");
            }
            return sb.ToString();
         }
      }
   }
}