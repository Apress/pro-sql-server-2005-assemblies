using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CityImageViewer
{
   public partial class CityImageViewer : Form
   {
      string connString;
      
      public CityImageViewer()
      {
         InitializeComponent();
         PopulateComboBox();
      }

      private void PopulateComboBox()
      {
         try
         {
            AppSettingsReader configReader = new AppSettingsReader();
            connString = (string)configReader.GetValue("connString", typeof(string));
            SqlConnection cn = new SqlConnection(connString);
            cn.Open();
            SqlCommand cmd = new SqlCommand("SELECT Name FROM Person.CityDetails ORDER BY Name", cn);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
               comboBox1.Items.Add(dr[0].ToString());
            }
            cn.Close();
         }
         catch (Exception e)
         {
            MessageBox.Show("Cannot read the CityDetails table from SQL Server.\n\nPlease ensure that SQL Server is running, the Person.CityDetails table exists,\nand the CityImageViewer.exe.config file is correctly configured.\n\n\nDebug info:\n\n" + e.Message, "Cannot initialize CityImageViewer", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            this.Close();
         }
      }

      private void button1_Click(object sender, EventArgs e)
      {
         try
         {
            if (comboBox1.SelectedItem != null)
            {
               SqlConnection cn = new SqlConnection(connString);
               cn.Open();
               SqlCommand cmd = new SqlCommand("SELECT Image FROM Person.CityDetails WHERE Name = @name", cn);
               cmd.Parameters.AddWithValue("@name", comboBox1.SelectedItem.ToString());
               byte[] imgData = (byte[])cmd.ExecuteScalar();
               MemoryStream stm = new MemoryStream(imgData);
               ImageFormat jpgFmt = ImageFormat.Jpeg;
               Image img = Image.FromStream(stm);
               stm.Close();
               pictureBox1.Image = img;
            }
         }
         catch (Exception ex)
         {
            MessageBox.Show("Cannot display the image for that entry.\n\n\nDebug info:\n\n" + ex.Message, "Cannot display image", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
         }
      }
   }
}