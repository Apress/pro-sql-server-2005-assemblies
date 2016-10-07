using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;


[Serializable]
[Microsoft.SqlServer.Server.SqlUserDefinedAggregate(Format.Native)]
public struct CountNulls
{
    // The current count of NULLs
    private int theCount;

    public void Init()
    {
        this.theCount = 0;
    }

    public void Accumulate(SqlString Value)
    {
        if (Value.IsNull)
            this.theCount++;
    }

    public void Merge(CountNulls Group)
    {
        this.theCount += Group.theCount;
    }

    public SqlInt32 Terminate()
    {
        return ((SqlInt32)this.theCount);
    }
}
