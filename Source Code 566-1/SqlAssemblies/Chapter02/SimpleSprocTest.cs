using System;
using System.Data.SqlClient;
using Microsoft.SqlServer.Server;

namespace Apress.SqlAssemblies.Chapter02
{
   public class SimpleSprocTest
   {
      public static void GetEmployeesHiredAfter(DateTime date)
      {
         try
         {
            using (SqlConnection cn = new SqlConnection("context connection=true"))
            {
               cn.Open();
               string sql = @"SELECT * FROM HumanResources.Employee 
                              WHERE HireDate > @hireDate";
               SqlCommand cmd = new SqlCommand(sql, cn);
               cmd.Parameters.AddWithValue("@hireDate", date);
               SqlDataReader reader = cmd.ExecuteReader();
               SqlPipe pipe = SqlContext.Pipe;
               pipe.Send(reader);
               pipe.Send("Command completed successfully");
            }
         }
         catch (Exception e)
         {
            SqlPipe pipe = SqlContext.Pipe;
            pipe.Send("Error occurred executing command:");
            pipe.Send(e.Message);
            pipe.Send(e.StackTrace);
         }
      }
   }
}