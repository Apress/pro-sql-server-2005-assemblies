using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using Microsoft.SqlServer.Server;

namespace FatBelly.Images
{
  public partial class StoredProcedures
  {
    [SqlProcedure]
    public static void LoadAnyImage(string imageLocation)
    {
      FileInfo imgFile = new FileInfo(imageLocation);
      Stream fileStream = imgFile.OpenRead();
      Byte[]byteImage = new Byte [fileStream.Length];
      fileStream.Read(byteImage,0 ,(int)  imgFile.Length);
      fileStream.Close();

      using (SqlConnection cn = new SqlConnection("context connection=true"))
      {
        cn.Open();
        SqlCommand sqlCmd = new SqlCommand("InsertImage", cn);
        sqlCmd.CommandType = CommandType.StoredProcedure;

        sqlCmd.Parameters.Add("@ImageName", SqlDbType.NVarChar,255).Value = imgFile.Name;
        sqlCmd.Parameters.Add("@ImageToLoad", SqlDbType.Image).Value = byteImage;

        SqlContext.Pipe.ExecuteAndSend(sqlCmd);
      }
    }

    public static void LoadAllImages(string imagesLocation,  string fileSuffix)
    {
      DirectoryInfo dirFiles = new DirectoryInfo(imagesLocation);
      FileInfo[]listOfFiles = dirFiles.GetFiles("*."+fileSuffix);

      foreach (FileInfo aFile in listOfFiles)
      {
        LoadAnyImage(aFile.FullName);
      }
    }

    public static void ExtractImage(Int32 photoID, string location)
    {
      SqlDataReader sqlReader = null;
      using (SqlConnection cn = new SqlConnection("context connection=true"))
      {
        cn.Open();
        SqlCommand sqlCmd = new SqlCommand("RetrieveImage", cn);
        sqlCmd.CommandType = CommandType.StoredProcedure;
        sqlCmd.Parameters.Add("@Id", SqlDbType.Int);
        sqlCmd.Parameters[0].Value = photoID;
        sqlReader = sqlCmd.ExecuteReader();
        sqlReader.Read();
        string fileName = (string) sqlReader.GetSqlString(0);

        // Create a file to hold the output.
        if (!Directory.Exists(location))
        {
          Directory.CreateDirectory(location);
        }

        string fileNameWithPath = location + "\\" + fileName;
        if (File.Exists(fileNameWithPath))
        {
          File.Delete(fileNameWithPath);
        }

        using (FileStream outputStream = new FileStream(fileNameWithPath,  FileMode.Create,  FileAccess.Write))
        {
          using (BinaryWriter binOutput = new BinaryWriter(outputStream))
          {
            int length = 4096;
            byte[]pictBlob = new byte[length];
            int startPoint = 0;
            long retval = sqlReader.GetBytes(1,  startPoint,  pictBlob,  0,  length);
            binOutput.Write(pictBlob,0 ,(int)  retval);
            while (retval == length)
            {
              startPoint += length;

              retval = sqlReader.GetBytes(1, startPoint,pictBlob, 0,length);
              binOutput.Write(pictBlob,0 ,(int) retval);
            }
            binOutput.Flush();
            binOutput.Close();
          }
        }
        sqlReader.Close();
      }
    }
  }
}
