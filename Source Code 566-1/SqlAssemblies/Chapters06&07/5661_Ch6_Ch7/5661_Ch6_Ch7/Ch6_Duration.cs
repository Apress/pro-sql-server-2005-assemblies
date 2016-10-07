using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

[Serializable]
[Microsoft.SqlServer.Server.SqlUserDefinedType(
    Format.Native,
    IsByteOrdered = true)]
public struct Duration : INullable
{
    // number of milliseconds
    private int milliseconds;

    // Private member
    private bool m_Null;

    public static Duration Parse(SqlString s)
    {
        if (s.IsNull)
            return Null;

        Duration u = new Duration();
        TimeSpan ts = TimeSpan.Parse((string)s);
        u.milliseconds = (int)ts.TotalMilliseconds;

        return u;
    }

    public override string ToString()
    {
        //Convert to ts and return ToString()
        return TimeSpan.FromMilliseconds(this.milliseconds).ToString();
    }

    public Duration AddDuration(Duration other)
    {
        if (other.IsNull)
            return Null;

        else
        {
            this.milliseconds += other.milliseconds;
            return this;
        }
    }

    public SqlDouble TotalHours
    {
        [SqlMethod(IsDeterministic = true)]
        get
        {
            return TimeSpan.FromMilliseconds(this.milliseconds).TotalHours;
        }
    }

    public bool IsNull
    {
        get
        {
            // Put your code here
            return m_Null;
        }
    }

    public static Duration Null
    {
        get
        {
            Duration h = new Duration();
            h.m_Null = true;
            return h;
        }
    } 
}


