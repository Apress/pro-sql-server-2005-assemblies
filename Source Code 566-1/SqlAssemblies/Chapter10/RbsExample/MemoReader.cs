using System;
using System.Collections;
using System.IO;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace Apress.SqlAssemblies.Chapter10
{
   public class Memo
   {
      private string recipient;
      private string sender;
      private string subject;
      private string body;

      public Memo(string recipient, string sender, string subject, string body)
      {
         this.recipient = recipient;
         this.sender = sender;
         this.subject = subject;
         this.body = body;
      }

      public string Recipient
      {
         get { return recipient; }
      }

      public string Sender
      {
         get { return sender; }
      }

      public string Subject
      {
         get { return subject; }
      }

      public string Body
      {
         get { return body; }
      }
   }

   public class MemoReader
   {
      private static Memo ReadRtfFile(string filename)
      {
         if (!filename.EndsWith(".rtf"))
            throw new IOException("Wrong file type: not an RTF file.");
         StreamReader sr = new StreamReader(filename);
         string memoContents = sr.ReadToEnd();
         sr.Close();

         Regex re = new Regex(@"\\par\s*From:\s*(?<sender>.+)\s*\\par\s*To:\s*(?<recipient>.+)\s*\\par\s*Subject:\s*(?<subject>.+)\s*\\par\s*\\par\s*\\par\s*(?<body>.+)\\par\r\n", RegexOptions.Compiled);
         Match match = re.Match(memoContents);
         if (!match.Success)
            return null;
         return new Memo(match.Result("${recipient}"), match.Result("${sender}"),
            match.Result("${subject}"), match.Result("${body}"));
      }

      public static ArrayList ReadAllMemos(string folder, WindowsIdentity winId)
      {
         WindowsImpersonationContext impersCtxt = null;

         try
         {
            impersCtxt = winId.Impersonate();

            ArrayList memos = new ArrayList();
            DirectoryInfo dir = new DirectoryInfo(folder);
            FileInfo[] files = dir.GetFiles("*.rtf");

            for (int i = 0; i < files.Length; i++)
            {
               // Ignore any files we can't read
               try
               {
                  Memo memo = ReadRtfFile(files[i].FullName);
                  if (memo != null) memos.Add(memo);
               }
               catch {}
            }

           return memos;
         }
         catch (Exception e)
         {
            IOException ex = new IOException("Cannot read directory", e);
            throw ex;
         }
         finally
         {
            if (impersCtxt != null)
               impersCtxt.Undo();
         }
      }
   }
}

