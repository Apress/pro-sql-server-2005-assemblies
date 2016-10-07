using System;
using System.Data.SqlClient;
using System.Text;
using System.Net.Mail;
using Microsoft.SqlServer.Server;

namespace Apress.SqlAssemblies.Chapter09
{
   public class EmailExample
   {
      [SqlProcedure]
      public static void GetEmployeeData(string columnList, int id)
      {
         StringBuilder sb = new StringBuilder();
         SqlPipe pipe = SqlContext.Pipe;

         try
         {
            columnList = columnList.Replace("[", "");
            columnList = columnList.Replace("]", "");
            sb.Append("SELECT ");

            string[] columnNames = columnList.Split(new char[] { ',' });
            foreach (string column in columnNames)
            {
               sb.Append("[");
               sb.Append(column.Trim());
               sb.Append("], ");
            }

            sb.Remove(sb.Length - 2, 2);
            sb.Append(" FROM HumanResources.Employee WHERE EmployeeID=@id");
int i = 0;
int j = 2/i;
            using (SqlConnection cn = new SqlConnection("context connection=true"))
            {
               cn.Open();
               SqlCommand cmd = new SqlCommand(sb.ToString(), cn);
               cmd.Parameters.AddWithValue("@id", id);
               SqlDataReader reader = cmd.ExecuteReader();
               pipe.Send(reader);
               pipe.Send("Command executed successfully.");
            }
         }
         catch (SqlException e)
         {
            pipe.Send("Invalid SQL statement: " + sb.ToString());
            foreach (SqlError err in e.Errors)
               pipe.Send(err.Message);
         }
         catch (Exception e)
         {
            EmailExceptionInfo(e.Message, "Exception in stored procedure GetEmployeeData", "Julian@JulianSkinner.com");
            pipe.Send(e.Message);
         }
      }

      private static void EmailExceptionInfo(string body, string subject, string recipients)
      {
         // Change the string in the following line to refer to a valid and
         // accessible SMTP server
         SmtpClient client = new SmtpClient("DAUFILTRI");
         MailMessage msg = new MailMessage("Julian@JulianSkinner.com", recipients, subject, body);
         client.Send(msg);
      }
   }
}