using System;
using System.Data.SqlClient;
using System.Diagnostics;

#if !DEBUG
using Microsoft.SqlServer.Server;
#endif

namespace Apress.SqlAssemblies.Chapter09
{
   public class CmdLineDebugExample
   {
      #if !DEBUG
      [SqlProcedure]
      #endif
      public static void GetContactData()
      {
         try
         {
            string connStr;
            #if !DEBUG
            connStr = "context connection=true";
            #else
            connStr = "Database=(local);Initial Catalog=AdventureWorks;" +
                      "Integrated Security=true";
            #endif

            using (SqlConnection cn = new SqlConnection(connStr))
            {
               cn.Open();
               string sql = @"SELECT TOP 10 FirstName, LastName
                              FROM Person.Contact";
               SqlCommand cmd = new SqlCommand(sql, cn);
               SqlDataReader reader = cmd.ExecuteReader();

               #if DEBUG
               WriteData(reader);
               #else
               SqlContext.Pipe.Send(reader);
               #endif
            }
         }
         catch (Exception e)
         {
            #if DEBUG
            Console.WriteLine(e.Message);
            #else
            SqlContext.Pipe.Send(e.Message);
            #endif
         }
      }

      #if DEBUG
      private static void WriteData(SqlDataReader reader)
      {
         while (reader.Read())
         {
            #line hidden
            for (int i = 0; i < reader.FieldCount; i++)
               Console.Write("{0}\t", reader[i].ToString());
            Console.WriteLine();
            #line default
         }
      }

      public static void Main()
      {
         bool success = Debugger.Launch();
         Debug.Assert(success, "Unable to launch debugger. Continue running?");
         GetContactData();
      }
      #endif
   }
}