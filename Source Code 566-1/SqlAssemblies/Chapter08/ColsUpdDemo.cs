using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using Microsoft.SqlServer.Server;

  public partial class Triggers
  {
    public static void ColsUpdDemo()
    {
      SqlTriggerContext sqlTrg = SqlContext.TriggerContext;

      EventLog ev = new EventLog("Application", ".", "ColsUpdated");
      ev.WriteEntry("Starting");

      for (int i = 0; i < sqlTrg.ColumnCount - 1; i++)
      {
        ev.WriteEntry(string.Format("Column {0}, updated: {1}",i, 
          sqlTrg.IsUpdatedColumn(i).ToString()));
      }

      SqlPipe pipeSql = SqlContext.Pipe;
      using (SqlConnection cn = new SqlConnection("context connection=true"))
      {
        cn.Open();
        string sql = "SELECT * FROM inserted";
        SqlCommand sqlComm = new SqlCommand(sql, cn);
        SqlDataReader dr = sqlComm.ExecuteReader();
        dr.Read();
        string col1 = dr.GetString(1);
        ev.WriteEntry(string.Format("Inserted {0}, {1}", dr.FieldCount, col1));
        dr.Close();
      }
    }
  }
