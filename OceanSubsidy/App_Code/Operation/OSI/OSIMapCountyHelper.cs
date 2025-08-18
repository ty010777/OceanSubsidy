using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;

/// <summary>
/// OSIMapCountyHelper 的摘要描述
/// </summary>
public class OSIMapCountyHelper
{
    public OSIMapCountyHelper()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }

    public static GisTable QueryAll()
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [qgs_fid]
      ,[geom]
      ,[ogr_fid]
      ,[county_id]
      ,[c_name]
      ,[c_desc]
      ,[add_date]
      ,[add_accept]
      ,[remark]
      ,[orderBy]
FROM [OCA_OceanSubsidy].[dbo].[OSI_MapCounty]
ORDER BY [orderBy]
";
        db.Parameters.Clear();

        return db.GetTable();
    }


}