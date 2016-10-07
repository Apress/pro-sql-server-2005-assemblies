using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Xml;
using System.IO;
using Microsoft.SqlServer.Server;

public partial class Triggers
{
   public static void EventDataXMLDump()
   {
      SqlTriggerContext sqlTrg = SqlContext.TriggerContext;
      string evData = sqlTrg.EventData.Value;
      XmlDocument xmlDoc = new XmlDocument();
      xmlDoc.LoadXml(evData);

      XmlNode xmlNd = xmlDoc.SelectSingleNode("//EVENT_INSTANCE/EventType/text()");
      string eventType =xmlNd.Value;
      using (SqlConnection cn = new SqlConnection("context connection=true"))
      {
         cn.Open();
         string sql = "INSERT INTO AssembliesTesting..EventDataDumpData " +
                      "(TriggerName, XMLData)  VALUES('" + eventType + "',' " +
                      evData + "')";
         SqlCommand sqlComm = new SqlCommand(sql, cn);
         sqlComm.ExecuteNonQuery();
         sqlComm.Dispose();
      }
   }
}
