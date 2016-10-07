using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace Apress.SqlAssemblies.Chapter10
{
   public class HttpFileReader
   {
      public static string[] ReadFile(string url)
      {
         WebPermission webPerm = new WebPermission(NetworkAccess.Connect, url);
         webPerm.Demand();

         HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
         HttpWebResponse response = (HttpWebResponse)request.GetResponse();
         Stream stream = response.GetResponseStream();
         StreamReader sr = new StreamReader(stream);
         StringCollection strings = new StringCollection();
         string line = sr.ReadLine();
         while (line != null)
         {
            strings.Add(line);
            line = sr.ReadLine();
         }
         sr.Close();
         stream.Close();
         response.Close();
         string[] lines = new string[strings.Count];
         strings.CopyTo(lines, 0);
         return lines;
      }
   }
}