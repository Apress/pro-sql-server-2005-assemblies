using System;
using System.Collections;
using System.Data;
using System.Data.Sql;
using System.Data.SqlTypes;
using System.IO;
using System.Security;
using System.Security.Principal;
using Microsoft.SqlServer.Server;

namespace Apress.SqlAssemblies.Chapter10
{
   public class RbsExample
   {
      [SqlFunction(FillRowMethodName="GetMemo")]
      public static IEnumerable ReadMemos(string directoryName)
      {
         try
         {
            if (Directory.Exists(directoryName))
            {
               WindowsIdentity winId = SqlContext.WindowsIdentity;
               if (winId == null)
                  throw new SecurityException("Won't work with SQL Server authentication");
               return MemoReader.ReadAllMemos(directoryName, winId);
            }
            else
            {
               throw new IOException("Directory doesn't exist");
            }
         }
         catch (Exception e)
         {
            throw new IOException("Can't read files from the directory", e);
         }
      }

      public static void GetMemo(object o, out SqlChars sender,
               out SqlChars recipient, out SqlChars subject, out SqlChars body)
      {
         Memo memo = (Memo)o;
         sender = new SqlChars(memo.Sender);
         recipient = new SqlChars(memo.Recipient);
         subject = new SqlChars(memo.Subject);
         body = new SqlChars(memo.Body);
      }
   }
}