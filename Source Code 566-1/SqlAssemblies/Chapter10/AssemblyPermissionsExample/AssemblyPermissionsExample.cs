using System;
using System.IO;
using System.Net;
using Microsoft.SqlServer.Server;

namespace Apress.SqlAssemblies.Chapter10
{
   public class AssemblyPermissionsExample
   {
      [SqlProcedure]
      public static void ReadWebPage(string url)
      {
         SqlPipe pipe = SqlContext.Pipe;
         try
         {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stm = resp.GetResponseStream();

            StreamReader sr = new StreamReader(stm);
            int bufferSize = 4000;
            int charsRead = 0;
            char[] buffer = new char[bufferSize];
            do {
               charsRead = sr.Read(buffer, 0, bufferSize);
               pipe.Send(new string(buffer, 0, charsRead));
            } while (charsRead > 0);

            sr.Close();
            stm.Close();
            resp.Close();
         }
         catch (Exception e)
         {
            pipe.Send(e.Message);
         }
      }
   }
}
