using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Text.RegularExpressions;

[Serializable]
[Microsoft.SqlServer.Server.SqlUserDefinedType(
    Format.UserDefined,
    IsByteOrdered=true,
    MaxByteSize=256)]
public struct EMail : INullable, IBinarySerialize
{
    private string userName;
    private string domainAddress;
    private bool m_Null;

    public bool IsNull

    {
        get
        {
            // Put your code here
            return m_Null;
        }
    } 

    public static EMail Null
    {
        get
        {
            EMail h = new EMail();
            h.m_Null = true;
            return h;
        }
    }

    public static EMail Parse(SqlString s)
    {
        if (s.IsNull)
            return Null;

        EMail u = new EMail(); 

        //Validate the e-mail address
        Regex r =
            new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"); 

        if (!r.IsMatch((string)s, 0))
            throw new ApplicationException("Not a valid address."); 

        //Find the @ symbol
        Regex r_at = new Regex("@");
        Match atSymb = r_at.Match((string)s); 

        //Populate the user name and address
        u.userName = ((string)s).Substring(0, atSymb.Index);
        u.domainAddress = ((string)s).Substring(atSymb.Index + 1); 
        return u;
    }

    public override string ToString()
    {
        return userName + "@" + domainAddress;
    }

    public SqlString UserName
    {
        get
        {
            return ((SqlString)userName);
        } 

        set
        {
            //Validate the user name
            Regex r =
                new Regex(@"\w+([-+.]\w+)*"); 

            if (!r.IsMatch((string)value, 0))
                throw new ApplicationException("Not a valid user name."); 

            this.userName = (string)value;
        }
    } 

    public SqlString DomainAddress
    {
        get
        {
            return ((SqlString)domainAddress);
        } 

        set
        {
            //Validate the domain address
            Regex r =
                new Regex(@"\w+([-.]\w+)*\.\w+([-.]\w+)*"); 

            if (!r.IsMatch((string)value, 0))
                throw new ApplicationException("Not a valid address."); 

            this.domainAddress = (string)value;
        }
    }

    public void Read(System.IO.BinaryReader r)
    {
        m_Null = r.ReadBoolean(); 
        if (!m_Null)
        {
            userName = r.ReadString();
            domainAddress = r.ReadString();
        }
    }

    public void Write(System.IO.BinaryWriter w)
    {
        w.Write(m_Null);
        if (!m_Null)
        {
            w.Write(userName);
            w.Write(domainAddress);
        }
    }
}