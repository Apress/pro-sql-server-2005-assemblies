using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;


[Serializable]
[Microsoft.SqlServer.Server.SqlUserDefinedAggregate(Format.Native)]
public struct AvgDuration
{
    private int totalTime;
    private int totalCount;

    public void Init()
    {
        this.totalTime = 0;
        this.totalCount = 0;
    }

    public void Accumulate(Duration Value)
    {
        if (!Value.IsNull)
        {
            TimeSpan ts = TimeSpan.Parse(Value.ToString());
            this.totalTime += (int)ts.TotalMilliseconds; 
            this.totalCount++;
        }
    }

    public void Merge(AvgDuration Group)
    {
        this.totalTime += Group.totalTime;
        this.totalCount += Group.totalCount;
    }

    public Duration Terminate()
    {
        double avg = this.totalTime / this.totalCount;
        TimeSpan ts = TimeSpan.FromMilliseconds(avg); 
        return(Duration.Parse(ts.ToString()));
    }
}
