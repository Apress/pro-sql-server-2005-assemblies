using System;
using System.Diagnostics;
using System.IO;
using Microsoft.SqlServer.Server;

namespace Apress.SqlAssemblies.Chapter04
{
   public class OSCmdExample
   {
      [SqlProcedure]
      public static void ExecuteOSCmd(string filename)
      {
         try
         {
            Process p = new Process();
            p.StartInfo.FileName = filename;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();

            StreamReader sr = p.StandardOutput;
            SqlPipe pipe = SqlContext.Pipe;
            char[] buffer = new char[4000];
            int length = 0;
            do
            {
               length = sr.ReadBlock(buffer, 0, 4000);
               string msg = new string(buffer, 0, length);
               pipe.Send(msg);
            } while (length == 4000);
            sr.Close();
         }
         catch (Exception e)
         {
            SqlContext.Pipe.Send(e.Message);
         }
      }
   }
}