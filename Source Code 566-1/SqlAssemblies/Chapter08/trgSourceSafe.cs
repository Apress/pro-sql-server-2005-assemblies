using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Xml;
using System.Collections;
using Microsoft.SqlServer.Server;

namespace Apress.SqlAssemblies.Chapter08
{
  public partial class Triggers
  {
    public static void clsSS()
    {
      XmlDocument xmlEventData;
      ArrayList auditActions = ReadMonitoringActions();
      String[] arrActions = (String[])auditActions.ToArray(typeof(string));
      SqlTriggerContext sqlTrg = SqlContext.TriggerContext;
      for (int i = 0; i < arrActions.Length - 1; i += 3)
      {
        if (arrActions[i] == sqlTrg.TriggerAction.ToString())
        {
           xmlEventData = new XmlDocument();
           xmlEventData.LoadXml(sqlTrg.EventData.Value);
           WriteOldVersion(xmlEventData);
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

    private static void WriteOldVersion(XmlNode objectXML)
    {
      string objectName = "";
      string objectType = "";
      string objectDetails = "";
      string loginName = "";
      string databaseName = "";
      string postTime = "";
      XmlNodeReader xmlDetails = new XmlNodeReader(objectXML);

      while (xmlDetails.Read())
      {
        if (xmlDetails.NodeType == XmlNodeType.Element && 
              xmlDetails.Name == "ObjectName")
        {
          xmlDetails.Read();
          objectName = xmlDetails.Value;
        }
        if (xmlDetails.NodeType == XmlNodeType.Element && 
              xmlDetails.Name == "ObjectType")
        {
          xmlDetails.Read();
          objectType = xmlDetails.Value;
        }
        if (xmlDetails.NodeType == XmlNodeType.Element && xmlDetails.Name == "LoginName")
        {
          xmlDetails.Read();
          loginName = xmlDetails.Value;
        }
        if (xmlDetails.NodeType == XmlNodeType.Element && 
               xmlDetails.Name == "DatabaseName")
        {
          xmlDetails.Read();
          databaseName = xmlDetails.Value;
        }
        if (xmlDetails.NodeType == XmlNodeType.Element && 
              xmlDetails.Name == "PostTime")
        {
          xmlDetails.Read();
          postTime = xmlDetails.Value;
        }
        if (xmlDetails.NodeType == XmlNodeType.Element && 
              xmlDetails.Name == "CommandText")
        {
          xmlDetails.Read();
           objectDetails = xmlDetails.Value;
        }
      }
      xmlDetails.Close();

      SqlPipe pipeSql = SqlContext.Pipe;
      using (SqlConnection cn = new SqlConnection("context connection=true"))
      {
        cn.Open();
        string sql = "INSERT INTO VSS " +
                     "(ObjectName,ObjectType,LoginName,DatabaseName," +
                     " PostTime,ObjectDetails)  " +
                     "VALUES('" + objectName.Trim() + "','" + objectType.Trim() +
                     "','" + loginName.Trim() + "','" + databaseName.Trim() +
                     "','" + postTime.Trim() + "','" + objectDetails.Trim() + "')";
        SqlCommand sqlComm = new SqlCommand(sql, cn);
        pipeSql.Send(sqlComm.CommandText);
        pipeSql.ExecuteAndSend(sqlComm);
      }
    }
  }
}
