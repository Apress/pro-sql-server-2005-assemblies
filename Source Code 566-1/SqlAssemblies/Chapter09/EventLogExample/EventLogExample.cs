using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using Microsoft.SqlServer.Server;

namespace Apress.SqlAssemblies.Chapter09
{
   public class EventLogExample
   {
      [SqlProcedure]
      public static void GetDataForNamedEmployee(string name)
      {
         // Open the context connection
         using (SqlConnection cn = new SqlConnection("context connection=true"))
         {
            cn.Open();

            // Define our SQL statement
            string sql = @"SELECT * FROM HumanResources.Employee e
                          INNER JOIN Person.Contact c
                          ON c.ContactID = e.ContactID
                          WHERE c.FirstName + ' ' + c.LastName = @name";
            SqlCommand cmd = new SqlCommand(sql, cn);

            // Add the @name parameter and set its value
            cmd.Parameters.AddWithValue("@name", name);

            // Get the SqlPipe to send data/messages back to the user
            SqlPipe pipe = SqlContext.Pipe;

            try
            {
               SqlDataReader reader = cmd.ExecuteReader();
               bool hasRows = reader.HasRows;
               pipe.Send(reader);

               if (!hasRows)
               {
                  WriteLogEntry(name, EventLogEntryType.Warning, 2000, 1);
                  pipe.Send("No matching rows found.");
               }
               else
               {
                  WriteLogEntry(name, EventLogEntryType.Information, 1000, 1);
                  pipe.Send("Command executed successfully.");
               }
            }
            catch (SqlException e)
            {
               // Build the log entry from the SqlErrors
               StringBuilder sb = new StringBuilder();
               foreach (SqlError err in e.Errors)
                  sb.AppendFormat("Error {0}\nSeverity {1}\nState {2}\n{3}\n\n",
                                  err.Number, err.Class, err.State, err.Message);

               // Write the entry and send a message to the caller
               WriteLogEntry(sb.ToString(), EventLogEntryType.Error, 3000, 2);
               pipe.Send("SQL errors occurred executing the stored procedure.");
               pipe.Send(sb.ToString());
            }
            catch (Exception e)
            {
               WriteLogEntry(e.Message, EventLogEntryType.Error, 4000, 2);
               pipe.Send("An unknown error occurred executing the stored procedure.");
               pipe.Send(e.Message);
            }
         }
      }

      private static void WriteLogEntry(string message, EventLogEntryType entryType,
                                        int msgId, short categoryId)
      {
         if (!EventLog.SourceExists("EventLogExample"))
            EventLog.CreateEventSource("EventLogExample", "SQL Assemblies Log");

         EventLog log = new EventLog("SQL Assemblies Log");
         log.Source = "EventLogExample";
         log.WriteEntry(message, entryType, msgId, categoryId);
         log.Close();
      }
   }
}
