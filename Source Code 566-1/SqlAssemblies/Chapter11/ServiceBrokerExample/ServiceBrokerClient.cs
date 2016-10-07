using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Text;
using Microsoft.SqlServer.Server;
using Microsoft.Samples.SqlServer;

namespace Apress.SqlAssemblies.Chapter11
{
   public class ServiceBrokerClient
   {
      public static void WriteStatsToQueue()
      {
         using (SqlConnection cn = new SqlConnection("context connection=true"))
         {
            string sql = @"EXEC uspGetSystemStats @connections OUTPUT,
                             @cpuBusy OUTPUT, @idle OUTPUT, @ioBusy OUTPUT,
                             @packErrors OUTPUT, @packRecd OUTPUT, @packSent OUTPUT,
                             @timeticks OUTPUT, @totErrors OUTPUT, @totRead OUTPUT,
                             @totWrite OUTPUT";
            SqlCommand cmd = new SqlCommand(sql, cn);
            cn.Open();

            SqlParameter connParm = new SqlParameter("@connections", SqlDbType.Int);
            connParm.Direction = ParameterDirection.Output;
            SqlParameter cpuParm = new SqlParameter("@cpuBusy", SqlDbType.Int);
            cpuParm.Direction = ParameterDirection.Output;
            SqlParameter idleParm = new SqlParameter("@idle", SqlDbType.Int);
            idleParm.Direction = ParameterDirection.Output;
            SqlParameter ioParm = new SqlParameter("@ioBusy", SqlDbType.Int);
            ioParm.Direction = ParameterDirection.Output;
            SqlParameter packErrsParm = new SqlParameter("@packErrors", SqlDbType.Int);
            packErrsParm.Direction = ParameterDirection.Output;
            SqlParameter packRecdParm = new SqlParameter("@packRecd", SqlDbType.Int);
            packRecdParm.Direction = ParameterDirection.Output;
            SqlParameter packSentParm = new SqlParameter("@packSent", SqlDbType.Int);
            packSentParm.Direction = ParameterDirection.Output;
            SqlParameter ticksParm = new SqlParameter("@timeticks", SqlDbType.Int);
            ticksParm.Direction = ParameterDirection.Output;
            SqlParameter totErrsParm = new SqlParameter("@totErrors", SqlDbType.Int);
            totErrsParm.Direction = ParameterDirection.Output;
            SqlParameter totReadParm = new SqlParameter("@totRead", SqlDbType.Int);
            totReadParm.Direction = ParameterDirection.Output;
            SqlParameter totWriteParm = new SqlParameter("@totWrite", SqlDbType.Int);
            totWriteParm.Direction = ParameterDirection.Output;

            cmd.Parameters.AddRange(new SqlParameter[] { connParm, cpuParm,
                     idleParm, ioParm, packErrsParm, packRecdParm, packSentParm,
                     ticksParm, totErrsParm, totReadParm, totWriteParm });
            cmd.ExecuteNonQuery();

            long timeticks = (int)ticksParm.Value;
            int connections = (int)connParm.Value;
            long cpuBusy = timeticks * (int)cpuParm.Value;
            long idle = timeticks * (int)idleParm.Value;
            long ioBusy = timeticks * (int)ioParm.Value;
            int packErrors = (int)packErrsParm.Value;
            int packRecd = (int)packRecdParm.Value;
            int packSent = (int)packSentParm.Value;
            int totalErrors = (int)totErrsParm.Value;
            int totalRead = (int)totReadParm.Value;
            int totalWrite = (int)totWriteParm.Value;

            string time = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.ff+00:00");
SqlContext.Pipe.Send(time);
            string msgBody = string.Format(@"<?xml version='1.0'?>
<sysStatsMessage>
   <connections>{0}</connections>
   <cpuBusy>{1}</cpuBusy>
   <idle>{2}</idle>
   <ioBusy>{3}</ioBusy>
   <packErrors>{4}</packErrors>
   <packRecd>{5}</packRecd>
   <packSent>{6}</packSent>
   <totalErrors>{7}</totalErrors>
   <totalRead>{8}</totalRead>
   <totalWrite>{9}</totalWrite>
   <time>{10}</time>
</sysStatsMessage>", connections, cpuBusy, idle, ioBusy, packErrors, packRecd, 
packSent, totalErrors, totalRead, totalWrite, time);

            Service client = new Service("http://schemas.apress.com/sqlassemblies/StatsRequestService", cn, null);
            Conversation conv = client.BeginDialog(
                   "http://schemas.apress.com/sqlassemblies/StatsProcessorService", null,
                   "http://schemas.apress.com/sqlassemblies/StatsServiceContract",
                   TimeSpan.FromMinutes(1), false, cn, null);

            MemoryStream msgStm = new MemoryStream(Encoding.ASCII.GetBytes(msgBody));
            Message msg = new Message("http://schemas.apress.com/sqlassemblies/StatsRequestMessage", msgStm);

            conv.Send(msg, cn, null);
            conv.End(cn, null);
            msgStm.Close();
         }
      }
   }
}
