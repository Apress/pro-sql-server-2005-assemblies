using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.SqlServer.Server;

namespace Apress.SqlAssemblies.Chapter03
{
   public class SendResultsTest
   {
      // Define read-only variables to refer to the fields' ordinal indexes by
      readonly static int FIELD_ID = 0;
      readonly static int FIELD_LASTCHANGE = 1;
      readonly static int FIELD_DUECHANGE = 2;

      public static void GetPayRiseDates()
      {
         // Open the context connection
         using (SqlConnection cn = new SqlConnection("context connection=true"))
         {
            cn.Open();

            // Get a new SqlDataReader with the details of the
            // employees who are due a pay rise
            string sql = @"SELECT EmployeeID,
MAX(RateChangeDate) AS LastRateChange,
DATEADD(year, 2, MAX(RateChangeDate)) AS DueRateChange
FROM HumanResources.EmployeePayHistory
GROUP BY EmployeeID
HAVING MAX(RateChangeDate) < DATEADD(year, -2, GETDATE())";
            SqlCommand cmd = new SqlCommand(sql, cn);
            SqlDataReader reader = cmd.ExecuteReader();

            // Get the SqlPipe
            SqlPipe pipe = SqlContext.Pipe;

            // Create the SqlMetaData objects for the rowset
            SqlMetaData idMeta = new SqlMetaData("Id", SqlDbType.Int);
            SqlMetaData lastRaiseMeta = new SqlMetaData("Last Rate Change", SqlDbType.DateTime);
            SqlMetaData dueRaiseMeta = new SqlMetaData("Due Rate Change", SqlDbType.DateTime);
            SqlMetaData[] rowMetaData = new SqlMetaData[] { idMeta, lastRaiseMeta, dueRaiseMeta };

            // Keep track of whether it's the first row or not
            bool firstRow = true;

            // Iterate through the rows, update if necessary,
            // and send them back to the caller
            while (reader.Read())
            {
               // Create a new SqlDataRecord for each row
               SqlDataRecord row = new SqlDataRecord(rowMetaData);

               // Add the ID and Last Rate Change values to the row
               row.SetInt32(FIELD_ID, (int)reader[FIELD_ID]);
               row.SetDateTime(FIELD_LASTCHANGE, (DateTime)reader[FIELD_LASTCHANGE]);

               // Store the change due date in a local variable
               DateTime dueDate = (DateTime)reader[FIELD_DUECHANGE];

               // If it's over six months overdue, set pay rise for
               // a month's time; otherwise, put it back seven months
               if (dueDate < DateTime.Now.AddMonths(-6))
                  row.SetDateTime(FIELD_DUECHANGE, DateTime.Now.AddMonths(1));
               else
                  row.SetDateTime(FIELD_DUECHANGE, dueDate.AddMonths(7));

               // If it's the first row, we need to call
               // SendResultsStart; otherwise, we call SendResultsRow
               if (firstRow == true)
               {
                  pipe.SendResultsStart(row);
                  firstRow = false;
               }
               else
               {
                  pipe.SendResultsRow(row);
               }
            }

            // Close the SqlDataReader once we've finished iterating through it
            reader.Close();

            // Call SendResultsEnd after the loop has finished, to
            // say we're done
            pipe.SendResultsEnd();
         }
      }
   }
}
