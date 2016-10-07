using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;
using System.Xml;
using Microsoft.SqlServer.Server;

namespace Apress.XML
{
  public partial class StoredProcedures
  {
    [SqlProcedure]
    public static void OutputXML(string sprocName, string rootName, string fileName)
    {
      string parentNodeValue = "";
      XmlDocument xmlDoc = new XmlDocument();

      XmlDeclaration decl = xmlDoc.CreateXmlDeclaration("1.0", "", "");
      xmlDoc.AppendChild(decl);

      XmlElement rootElem = xmlDoc.CreateElement("", rootName, "");
      xmlDoc.AppendChild(rootElem);

      using (SqlConnection cn = new SqlConnection("context connection=true"))
      {
        cn.Open();
        SqlCommand sqlComm = new SqlCommand(sprocName, cn);
        sqlComm.CommandType = CommandType.StoredProcedure;
        SqlDataReader rdr = sqlComm.ExecuteReader();

        XmlElement parentElement = null;
        XmlElement itemNode = null;
        while (rdr.Read())
        {
          string name = (string)rdr.GetSqlString(0);
          if (parentNodeValue != name)
          {
            parentElement = xmlDoc.CreateElement("", name.Replace(" ", ""), "");
            rootElem.AppendChild(parentElement);
            parentNodeValue = name;
          }

          itemNode = xmlDoc.CreateElement("", "item", "");
          parentElement.AppendChild(itemNode);

          for (int i = 1; i < rdr.FieldCount; i++)
          {
            XmlElement xmlElem = xmlDoc.CreateElement("", rdr.GetName(i). Replace(" ", ""), "");
            xmlElem.InnerText = rdr.GetSqlValue(i).ToString();
            itemNode.AppendChild(xmlElem);
          }
        }

        xmlDoc.Save(fileName);
        rdr.Close();
        sqlComm.Dispose();
      }
    }

    [SqlProcedure]
    public static void OutputRSS(string sprocName, string fileName, string title, string link, string description)
    {
      string parentNodeValue = "";
      XmlDocument xmlDoc = new XmlDocument();

      XmlDeclaration decl = xmlDoc.CreateXmlDeclaration("1.0", "", "");
      xmlDoc.AppendChild(decl);

      XmlElement root = xmlDoc.CreateElement("rss");
      XmlAttribute attr = xmlDoc.CreateAttribute("version");
      attr.Value = "2.0";
      root.Attributes.Append(attr);
      xmlDoc.AppendChild(root);

      XmlElement channel = xmlDoc.CreateElement("channel");
      root.AppendChild(channel);

      XmlElement titleElem = xmlDoc.CreateElement("", "title", "");
      titleElem.InnerText = title;
      channel.AppendChild(titleElem);

      XmlElement linkElem = xmlDoc.CreateElement("", "link", "");
      linkElem.InnerText = link;
      channel.AppendChild(linkElem);

      XmlElement descElem = xmlDoc.CreateElement("", "description", "");
      descElem.InnerText = description;
      channel.AppendChild(descElem);

      using (SqlConnection cn = new SqlConnection("context connection=true"))
      {
        cn.Open();
        SqlCommand sqlComm = new SqlCommand(sprocName, cn);
        sqlComm.CommandType = CommandType.StoredProcedure;
        SqlDataReader rdr = sqlComm.ExecuteReader();

        XmlElement descNode = null;
        StringBuilder sb = null;
        while (rdr.Read())
        {
          string name = (string)rdr.GetSqlString(0);
          if (parentNodeValue != name)
          {
            XmlElement parentElement = xmlDoc.CreateElement("", "item", "");
            channel.AppendChild(parentElement);
            parentNodeValue = name;

            if (descNode != null)
            {
              sb.Append("</tbody></table>");
              descNode.InnerText = sb.ToString();
            }

            sb = new StringBuilder();
            XmlElement titleNode = xmlDoc.CreateElement("", "title", "");
            titleNode.InnerText = name;
            parentElement.AppendChild(titleNode);
            descNode = xmlDoc.CreateElement("", "description", "");
            parentElement.AppendChild(descNode);
            sb.Append("<table border='1'><thead>");
            for (int i=1; i<rdr.FieldCount; i++)
              sb.AppendFormat("<th>{0}</th>", rdr.GetName(i));
            sb.Append("</thead><tbody>");
          }

          sb.Append("<tr>");

          for (int i = 1; i < rdr.FieldCount; i++)
          {
            sb.AppendFormat("<td>{0}</td>", rdr.GetSqlValue(i).ToString());
          }
          sb.Append("</tr>");
        }

        sb.Append("</tbody></table>");
        descNode.InnerText = sb.ToString();

        xmlDoc.Save(fileName);
        rdr.Close();
        sqlComm.Dispose();
      }
    }
  }
};