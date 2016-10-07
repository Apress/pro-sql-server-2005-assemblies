using System;
using System.Collections;
using System.IO;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace Apress.SqlAssemblies.Chapter05
{
   public class DirectoryBrowserExample
   {
      [SqlFunction(FillRowMethodName="GetFolderInfo")]
      public static IEnumerable BrowseDirectory(string directoryName)
      {
         DirectoryInfo dir = new DirectoryInfo(directoryName);
         return dir.GetFileSystemInfos();
      }

      public static void GetFolderInfo(object o, out SqlChars name, out SqlChars type, out SqlChars extension, out SqlDateTime timeCreated, out SqlDateTime timeAccessed)
      {
         FileSystemInfo fileInfo = (FileSystemInfo)o;
         name = new SqlChars(fileInfo.Name);
         type = fileInfo is DirectoryInfo ? new SqlChars("FOLDER") : new SqlChars("FILE");
         extension = fileInfo is FileInfo ? new SqlChars(fileInfo.Extension) : SqlChars.Null;
         timeCreated = new SqlDateTime(fileInfo.CreationTime);
         timeAccessed = new SqlDateTime(fileInfo.LastWriteTime);
      }
   }
}