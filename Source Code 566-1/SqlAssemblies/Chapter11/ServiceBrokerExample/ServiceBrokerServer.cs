using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Xml;
using Microsoft.SqlServer.Server;
using Microsoft.Samples.SqlServer;

namespace Apress.SqlAssemblies.Chapter11
{
   public class StatsProcessorService : Service
   {
      public StatsProcessorService(SqlConnection cn) : base(
                    "http://schemas.apress.com/sqlassemblies/StatsProcessorService",
                    cn, null)
      {
      }

      public override void Run(bool autoCommit, SqlConnection cn, SqlTransaction tx)
      {
         Message message = null;
         ArrayList statsArray = new ArrayList();

         int  lastConnections = 0;
         long lastCpuBusy = 0;
         long lastIdle = 0;
         long lastIoBusy = 0;
         int  lastPackErrors = 0;
         int  lastPackRecd = 0;
         int  lastPackSent = 0;
         int  lastTotalErrors = 0;
         int  lastTotalRead = 0;
         int  lastTotalWrite = 0;

         Conversation conv;
         while ((conv = GetConversation(cn, tx)) != null)
         {
            while ((message = conv.Receive()) != null)
            {
               if (message.Type == "http://schemas.apress.com/sqlassemblies/StatsRequestMessage")
               {
                  XmlDocument msgDoc = new XmlDocument();
                  msgDoc.Load(message.Body);
                  SystemStats stats = new SystemStats();
                  stats.Connections = XmlConvert.ToInt32(msgDoc.SelectSingleNode("//connections").FirstChild.Value);
                  stats.CpuBusy = XmlConvert.ToInt64(msgDoc.SelectSingleNode("//cpuBusy").FirstChild.Value);
                  stats.Idle = XmlConvert.ToInt64(msgDoc.SelectSingleNode("//idle").FirstChild.Value);
                  stats.IoBusy = XmlConvert.ToInt64(msgDoc.SelectSingleNode("//ioBusy").FirstChild.Value);
                  stats.PackErrs = XmlConvert.ToInt32(msgDoc.SelectSingleNode("//packErrors").FirstChild.Value);
                  stats.PackRecd = XmlConvert.ToInt32(msgDoc.SelectSingleNode("//packRecd").FirstChild.Value);
                  stats.PackSent = XmlConvert.ToInt32(msgDoc.SelectSingleNode("//packSent").FirstChild.Value);
                  stats.TotalErrs = XmlConvert.ToInt32(msgDoc.SelectSingleNode("//totalErrors").FirstChild.Value);
                  stats.TotalRead = XmlConvert.ToInt32(msgDoc.SelectSingleNode("//totalRead").FirstChild.Value);
                  stats.TotalWrite = XmlConvert.ToInt32(msgDoc.SelectSingleNode("//totalWrite").FirstChild.Value);
                  stats.Time = XmlConvert.ToDateTime(msgDoc.SelectSingleNode("//time").FirstChild.Value, XmlDateTimeSerializationMode.Local);
                  statsArray.Add(stats);
               }
            }
         }

         string sql = @"SELECT TOP 1 TotalConnections, TotalCpuBusy, TotalIdle,
                                     TotalIoBusy, TotalPackErrors, TotalPackRecd,
                                     TotalPackSent, TotalErrors, TotalRead,
                                     TotalWrite
                        FROM SystemStats
                        ORDER BY Time DESC;";
         SqlCommand cmd = new SqlCommand(sql, cn);

         using (SqlDataReader reader = cmd.ExecuteReader())
         {
            if (reader.HasRows)
            {
               reader.Read();
               lastConnections = (int)reader[0];
               lastCpuBusy = (long)reader[1];
               lastIdle = (long)reader[2];
               lastIoBusy = (long)reader[3];
               lastPackErrors = (int)reader[4];
               lastPackRecd = (int)reader[5];
               lastPackSent = (int)reader[6];
               lastTotalErrors = (int)reader[7];
               lastTotalRead = (int)reader[8];
               lastTotalWrite = (int)reader[9];
            }
         }

         string insertQuery = @"INSERT INTO SystemStats VALUES (@time, @connections,
                  @totalConnections, @cpuBusy, @totalCpuBusy, @idle, @totalIdle,
                  @ioBusy, @totalIoBusy, @packErrors, @totalPackErrors, @packRecd,
                  @totalPackRecd, @packSent, @totalPackSent, @numErrors,
                  @totalErrors, @numReads, @totalRead, @numWrites, @totalWrite)";
         SqlCommand insertCmd = new SqlCommand(insertQuery, cn);

         SqlParameter timeParam = new SqlParameter("@time", SqlDbType.DateTime);
         SqlParameter connParam = new SqlParameter("@connections", SqlDbType.Int);
         SqlParameter totalConnParam = new SqlParameter("@totalConnections", SqlDbType.Int);
         SqlParameter cpuParam = new SqlParameter("@cpuBusy", SqlDbType.BigInt);
         SqlParameter totalCpuParam = new SqlParameter("@totalCpuBusy",  SqlDbType.BigInt);
         SqlParameter idleParam = new SqlParameter("@idle", SqlDbType.BigInt);
         SqlParameter totalIdleParam = new SqlParameter("@totalIdle", SqlDbType.BigInt);
         SqlParameter ioParam = new SqlParameter("@ioBusy", SqlDbType.BigInt);
         SqlParameter totalIoParam = new SqlParameter("@totalIoBusy", SqlDbType.BigInt);
         SqlParameter packErrsParam = new SqlParameter("@packErrors", SqlDbType.Int);
         SqlParameter totalPackErrsParam = new SqlParameter("@totalPackErrors", SqlDbType.Int);
         SqlParameter packRecdParam = new SqlParameter("@packRecd", SqlDbType.Int);
         SqlParameter totalPackRecdParam = new SqlParameter("@totalPackRecd", SqlDbType.Int);
         SqlParameter packSentParam = new SqlParameter("@packSent", SqlDbType.Int);
         SqlParameter totalPackSentParam = new SqlParameter("@totalPackSent", SqlDbType.Int);
         SqlParameter numErrsParam = new SqlParameter("@numErrors", SqlDbType.Int);
         SqlParameter totErrsParam = new SqlParameter("@totalErrors", SqlDbType.Int);
         SqlParameter numReadsParam = new SqlParameter("@numReads", SqlDbType.Int);
         SqlParameter totReadParam = new SqlParameter("@totalRead", SqlDbType.Int);
         SqlParameter numWritesParam = new SqlParameter("@numWrites", SqlDbType.Int);
         SqlParameter totWriteParam = new SqlParameter("@totalWrite", SqlDbType.Int);

         insertCmd.Parameters.AddRange(new SqlParameter[] { timeParam, connParam,
               totalConnParam, cpuParam, totalCpuParam, idleParam, totalIdleParam,
               ioParam, totalIoParam, packErrsParam, totalPackErrsParam,
               packRecdParam, totalPackRecdParam, packSentParam, totalPackSentParam,
               numErrsParam, totErrsParam, numReadsParam, totReadParam,
               numWritesParam, totWriteParam });
         insertCmd.Prepare();

         foreach (SystemStats statsElement in statsArray)
         {
            timeParam.Value = statsElement.Time;
            connParam.Value = statsElement.Connections - lastConnections;
            totalConnParam.Value = statsElement.Connections;
            cpuParam.Value = statsElement.CpuBusy - lastCpuBusy;
            totalCpuParam.Value = statsElement.CpuBusy;
            idleParam.Value = statsElement.Idle - lastIdle;
            totalIdleParam.Value = statsElement.Idle;
            ioParam.Value = statsElement.IoBusy - lastIoBusy;
            totalIoParam.Value = statsElement.IoBusy;
            packErrsParam.Value = statsElement.PackErrs - lastPackErrors;
            totalPackErrsParam.Value = statsElement.PackErrs;
            packRecdParam.Value = statsElement.PackRecd - lastPackRecd;
            totalPackRecdParam.Value = statsElement.PackRecd;
            packSentParam.Value = statsElement.PackSent - lastPackSent;
            totalPackSentParam.Value = statsElement.PackSent;
            numErrsParam.Value = statsElement.TotalErrs - lastTotalErrors;
            totErrsParam.Value = statsElement.TotalErrs;
            numReadsParam.Value = statsElement.TotalRead - lastTotalRead;
            totReadParam.Value = statsElement.TotalRead;
            numWritesParam.Value = statsElement.TotalWrite - lastTotalWrite;
            totWriteParam.Value = statsElement.TotalWrite;

            lastConnections = statsElement.Connections;
            lastCpuBusy = statsElement.CpuBusy;
            lastIdle = statsElement.Idle;
            lastIoBusy = statsElement.IoBusy;
            lastPackErrors = statsElement.PackErrs;
            lastPackRecd = statsElement.PackRecd;
            lastPackSent = statsElement.PackSent;
            lastTotalErrors = statsElement.TotalErrs;
            lastTotalRead = statsElement.TotalRead;
            lastTotalWrite = statsElement.TotalWrite;
            insertCmd.ExecuteNonQuery();
         }
      }

      public static void ProcessMessages()
      {
         using (SqlConnection cn = new SqlConnection("context connection=true"))
         {
            cn.Open();
            StatsProcessorService svc = new StatsProcessorService(cn);
            svc.Run(false, cn, null);
         }
      }

      private struct SystemStats
      {
         public int Connections;
         public long CpuBusy;
         public long Idle;
         public long IoBusy;
         public int PackErrs;
         public int PackRecd;
         public int PackSent;
         public int TotalErrs;
         public int TotalRead;
         public int TotalWrite;
         public DateTime Time;
      }
   }
}