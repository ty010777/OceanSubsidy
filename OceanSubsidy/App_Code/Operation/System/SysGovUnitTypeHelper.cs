using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// SysGovUnitTypeHelper 的摘要描述
/// </summary>
public class SysGovUnitTypeHelper
{
    public SysGovUnitTypeHelper()
    {
      
    }

    /// <summary>
    /// 查詢全部
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryAll()
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT 
    [TypeID]
    ,[TypeName]
FROM [OCA_OceanSubsidy].[dbo].[Sys_GovUnitType]";
        db.Parameters.Clear();

        return db.GetTable();
    }


}