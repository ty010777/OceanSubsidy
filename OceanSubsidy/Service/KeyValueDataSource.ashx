<%@ WebHandler Language="C#" Class="KeyValueDataSource" %>

using System;
using System.IO;
using System.Web;
using System.Collections.Generic;
using Newtonsoft.Json;
using GS.Data;
using GS.Data.Sql;

public class KeyValueDataSource : IHttpHandler
{
    DbHelper db;
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "applicatoin/json";
        context.Response.ContentEncoding = System.Text.Encoding.UTF8;
        List<KeyValuePair> ltKVP = null;
        db = new DbHelper();
        try
        {
            switch (context.Request["data"].ToLower())
            {
                case "county":
                    ltKVP = GetCountyData();
                    break;
                case "town":
                    ltKVP = GetTownData(context.Request["county"]);
                    break;
                case "village":
                    ltKVP = GetVillageData(context.Request["county"], context.Request["town"]);
                    break;
                case "road_km_branch":
                    ltKVP = GetRoadKMBranch(context.Request["roadnum"]);
                    break;
                case "road_km_stake":
                    ltKVP = GetRoadKMStake(context.Request["roadname"]);
                    break;
                case "year":
                    ltKVP = GetOSIPeriodYears();
                    break;
                case "unit":
                    ltKVP = GetUnits();
                    break;
            }
        }
        catch (Exception e)
        {
        }

        if (ltKVP != null)
        {
            StreamWriter writer = new StreamWriter(context.Response.OutputStream, System.Text.Encoding.UTF8);
            writer.WriteLine(JsonConvert.SerializeObject(ltKVP, Formatting.Indented));
            writer.Flush();
            writer.Dispose();
        }

    }

    private List<KeyValuePair> GetCountyData()
    {
        db.Parameters.Clear();
        db.CommandText =
@"
SELECT DISTINCT county_id as [key], c_name as value 
  FROM MAP_COUNTY";
        return genKVPList(db.GetData());
    }

    private List<KeyValuePair> GetTownData(string county)
    {
        db.Parameters.Clear();
        db.CommandText =
@"
SELECT DISTINCT town_id as [key], t_name as value 
  FROM MAP_TOWN
 WHERE county_id = @county";
        db.Parameters.Add("@county", System.Data.SqlDbType.VarChar, county);
        return genKVPList(db.GetData());
    }

    private List<KeyValuePair> GetVillageData(string county, string town)
    {
        db.Parameters.Clear();
        db.CommandText =
@"
SELECT DISTINCT village_id as [key], v_name as value 
  FROM MAP_VILLAGE
 WHERE town_id = @town
";
        db.Parameters.Add("@county", System.Data.SqlDbType.VarChar, county);
        db.Parameters.Add("@town", System.Data.SqlDbType.VarChar, town);
        return genKVPList(db.GetData());
    }

    private List<KeyValuePair> GetRoadKMBranch(string roadnum)
    {
        db.Parameters.Clear();
        db.CommandText =
@"
SELECT roadname as [key], replace(replace(roadname, '台'+@roadnum, ''),'線', '') as value
  FROM (SELECT DISTINCT roadname FROM map_road_km) as map_road_km
 WHERE substring(roadname, PATINDEX('%[0-9]%',roadname), PATINDEX('%[^0-9]%', substring(roadname,PATINDEX('%[0-9]%',roadname),len(roadname)-1))-1) = @roadnum
 ORDER BY replace(replace(roadname, '台'+@roadnum, ''),'線', '')
";
        db.Parameters.Add("@roadnum", System.Data.SqlDbType.VarChar, roadnum);
        return genKVPList(db.GetData());
    }

    private List<KeyValuePair> GetRoadKMStake(string roadname)
    {
        db.Parameters.Clear();
        db.CommandText =
@"
SELECT ogr_fid as [key], stake as value
  FROM map_road_km
 WHERE roadname = @roadname
   AND (stake LIKE '%+000' OR stake LIKE '%+500')
";
        db.Parameters.Add("@roadname", System.Data.SqlDbType.VarChar, roadname);
        return genKVPList(db.GetData());
    }

    private List<KeyValuePair> genKVPList(GisTable dt)
    {
        List<KeyValuePair> ltKVP = new List<KeyValuePair>();

        for (var i = 0; i < dt.Rows.Count; i++)
        {
            KeyValuePair kvp = new KeyValuePair();
            ltKVP.Add(new KeyValuePair()
            {
                key = dt[i]["key"].ToString(),
                value = dt[i]["value"].ToString()
            });
        }
        return ltKVP;
    }

    private List<KeyValuePair> GetOSIPeriodYears()
    {
        db.Parameters.Clear();
        db.CommandText =
@"
SELECT DISTINCT
PeriodYear AS [key],
PeriodYear AS [value]
FROM OSI_DataPeriods
";
        return genKVPList(db.GetData());
    }

    private List<KeyValuePair> GetUnits()
    {
        db.Parameters.Clear();
        db.CommandText =
@"
SELECT 
UnitID AS [key],
UnitName AS [value]
FROM Sys_Unit
WHERE IsValid = 1
";
        return genKVPList(db.GetData());
    }

    public class KeyValuePair
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}