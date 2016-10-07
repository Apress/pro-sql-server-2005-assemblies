using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Runtime.InteropServices;
using System.Collections.Generic;


[Serializable]
[SqlUserDefinedAggregate(Format.UserDefined, MaxByteSize=8000)]
[StructLayout(LayoutKind.Sequential)]
public class VarP : IBinarySerialize
{
    List<double> aValues; 

    public void Write(System.IO.BinaryWriter w)
    {
        // Write out how many
        w.Write(aValues.Count); 

        // Write out each element
        foreach (double obj in aValues)
        {

            w.Write(obj);

        }
    } 

    public void Read(System.IO.BinaryReader r)
    {
        aValues = new List<double>(); 
        Int32 ii =  r.ReadInt32(); 

        for (int i = 1; i <= ii; i++)
        {
            double dblConv = r.ReadDouble();
            aValues.Add(dblConv);
        }
    } 

    public void Init()
    {
      aValues = new List<double>();
    } 

    public void Accumulate(SqlDouble lValue)
    {
        aValues.Add((double)lValue);
    } 


    public void Merge(VarP Group)
    {
        foreach (double dbl in Group.aValues)
            this.aValues.Add(dbl);
    }

    public SqlDouble Terminate()
    {
        double lSum = 0;

        foreach (double obj in aValues)
        {
            lSum = lSum + obj;
        }

        double lSumMeanDev = 0;
        double lMean = lSum / aValues.Count;
        double lTotMean = 0;

        foreach (double obj in aValues)
        {
            lTotMean = obj - lMean;
            lSumMeanDev = lSumMeanDev +
              (Math.Pow(Math.Abs(lTotMean), 2));
        }

        lSumMeanDev /= aValues.Count;
        return (SqlDouble)lSumMeanDev;
    }
}