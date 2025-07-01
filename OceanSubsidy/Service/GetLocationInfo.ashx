<%@ WebHandler Language="C#" Class="GetLocationInfo" %>

using System;
using System.IO;
using System.Web;
using System.Collections.Generic;
using Newtonsoft.Json;
using GISFCU.Data.Sql;
using GISFCU.Data;

public class GetLocationInfo : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        DbHelper db;
        context.Response.ContentType = "applicatoin/json";
        context.Response.ContentEncoding = System.Text.Encoding.UTF8;
        List<String> ret = new List<string>();
        String table_name = null;
        db = new DbHelper();
        try
        {
            String whereCond = "";
            switch (context.Request["data"].ToLower())
            {
                case "county":
                    table_name = "map_county";
                    whereCond = " WHERE county_id = @county";
                    db.Parameters.Add("@county", System.Data.SqlDbType.VarChar, context.Request["county"]);
                    break;
                case "town":
                    table_name = "map_town";
                    whereCond = " WHERE town_id = @town";
                    db.Parameters.Add("@town", System.Data.SqlDbType.VarChar, context.Request["town"]);
                    break;
                case "village":
                    table_name = "map_village";
                    whereCond = " WHERE village_id = @village";
                    db.Parameters.Add("@village", System.Data.SqlDbType.VarChar, context.Request["village"]);
                    break;
                case "road_km":
                    table_name = "map_road_km";
                    whereCond = " WHERE ogr_fid = @fid";
                    db.Parameters.Add("@fid", System.Data.SqlDbType.Int, int.Parse(context.Request["fid"]));
                    break;
                    
            }
            db.CommandText = "SELECT geometry.STAsText() as [geometry] FROM " + table_name + " " + whereCond;
            var tb = db.GetData();
            for (var i = 0; i < tb.Rows.Count; i++)
            {
                ret.Add(tb[i]["geometry"].ToString());
            }
            StreamWriter writer = new StreamWriter(context.Response.OutputStream, System.Text.Encoding.UTF8);
            writer.WriteLine(JsonConvert.SerializeObject(ret, Formatting.Indented));
            writer.Flush();
            writer.Dispose();
        }
        catch (Exception e)
        {
        }
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}