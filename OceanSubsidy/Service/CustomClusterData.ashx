<%@ WebHandler Language="C#" Class="CustomClusterData" %>

using System;
using System.Web;
using System.Data;
using GISFCU.Data.Sql;
using GISFCU.Data;
using System.Configuration;
using System.Linq;

public class CustomClusterData : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "applicatoin/json";
        context.Response.ContentEncoding = System.Text.Encoding.UTF8;
        switch (context.Request["data"].ToLower())
        {
            case "tree":
                writeTreeData(context);
                break;
        }
    }

    private void writeTreeData(HttpContext context)
    {
        var db = new DbHelper(ConfigurationManager.ConnectionStrings["Tccg_Tree"].ConnectionString);
        double res = Math.Floor(double.Parse(context.Request["res"]));
        DataTable dt;
        var isCluster = true;
        if (res >= 1)
        {
            db.CommandText = @"
SELECT DISTRICT_ID
     , DISTRICT_NAME
     , COUNT(*)
       AS [Count]
     , SUM(COR_X) / COUNT(*)
       AS COR_X
     , SUM(COR_Y) / COUNT(*)
       AS COR_Y
  FROM DISTRICT d
  JOIN TREE_DATA t
    ON d.DISTRICT_ID = t.DISTRICT
 GROUP BY DISTRICT_ID
        , DISTRICT_NAME
";
            dt = db.GetTable();

        }
        else
        {
            isCluster = false;
            db.CommandText = @"
SELECT UID, TREE_NO, TREE_TYPE_ID, COR_X, COR_Y
  FROM TREE_DATA
";
            bool bWhereUsed = false;
            if (!String.IsNullOrEmpty(context.Request["bbox"]))
            {
                double[] arBBox = context.Request["bbox"].Split(',').Select(r => double.Parse(r)).ToArray();
                db.CommandText += (bWhereUsed ? " AND " : " WHERE ") + @"
(
    COR_X BETWEEN " + arBBox[0] + " AND " + arBBox[2] + @"
    AND COR_Y BETWEEN " + arBBox[1] + " AND " + arBBox[3] + @"
)
";
                bWhereUsed = true;
            }
            dt = db.GetTable();
        }

        dt.Columns.Add("isCluster", typeof(bool));
        foreach (DataRow row in dt.Rows)
        {
            row["isCluster"] = isCluster;
        }
        context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(dt));
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}