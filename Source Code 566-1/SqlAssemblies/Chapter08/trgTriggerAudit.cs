using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Collections;
using Microsoft.SqlServer.Server;

public partial class Triggers
{
  public static void AuditTrigger()
  {
    ArrayList auditActions = ReadMonitoringActions();
    String[] arrActions = (String[])auditActions.ToArray(typeof(string));
    SqlTriggerContext trgContext = SqlContext.TriggerContext;

      for (int i = 0; i < arrActions.Length; i += 3)
      {
         DateTime fromDate = Convert.ToDateTime(arrActions[i + 1]);
         DateTime toDate = Convert.ToDateTime(arrActions[i + 2]);
         if (arrActions[i] == trgContext.TriggerAction.ToString() &&
             fromDate.ToShortTimeString().CompareTo(DateTime.Now.
                                               ToShortTimeString()) < 0 &&
             toDate.ToShortTimeString().CompareTo(DateTime.Now.
                                               ToShortTimeString()) > 0)
         {
            string evData = trgContext.EventData.Value;
            SqlPipe pipeSql = SqlContext.Pipe;
            using (SqlConnection cn = new SqlConnection("context connection=true"))
            {
               cn.Open();
               string sql = "msdb.dbo.sp_send_dbmail " + 
                 "@profile_name = 'default profile',"  +
                 "@recipients = 'Julian@JulianSkinner.com',"  +
                 "@body = '" + trgContext.TriggerAction.ToString() +
                 " is happening during core hours.' ,"  +
                 "@subject = 'Trigger Action occurring'" ;
               SqlCommand sqlComm = new SqlCommand(sql, cn);
               pipeSql.Send(sqlComm.CommandText);
               pipeSql.ExecuteAndSend(sqlComm);
            }
         }
      }
   }

   private static ArrayList ReadMonitoringActions()
   {
      ArrayList actions = new ArrayList();
      using (SqlConnection cn = new SqlConnection("context connection=true"))
      {
         cn.Open();
         string sql = "SELECT TriggerName,  StartTime,  EndTime " +
                      "FROM AuditActions ";
         SqlCommand sqlComm = new SqlCommand(sql, cn);
         SqlDataReader sqlTriggers = sqlComm.ExecuteReader();
         while (sqlTriggers.Read())
         {
            actions.Add(sqlTriggers.GetString(0)) ;
            actions.Add(sqlTriggers.GetString(1)) ;
            actions.Add(sqlTriggers.GetString(2)) ;
         }
         sqlTriggers.Close();
         return actions;
      }
   }
}

