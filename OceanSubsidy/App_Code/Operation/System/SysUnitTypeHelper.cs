using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;

/// <summary>
/// SysUnitTypeHelper 的摘要描述
/// </summary>
public class SysUnitTypeHelper
{
    public SysUnitTypeHelper()
    {
        
    }

    /// <summary>
    /// 查詢所有
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryAll()
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [TypeID]
    ,[TypeName]
FROM [OCA_OceanSubsidy].[dbo].[Sys_UnitType]";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 查詢
    /// </summary>
    /// <returns></returns>
    public static int QueryIDByTypeName(string typeName)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [TypeID]
    ,[TypeName]
FROM [OCA_OceanSubsidy].[dbo].[Sys_UnitType]
WHERE TypeName = @TypeName";
        db.Parameters.Clear();
        db.Parameters.Add("@TypeName", typeName);
        var tbl = db.GetTable();

        if (tbl.Rows.Count == 0)
        {
            return -1;
        }
        else
        {
            return Convert.ToInt32(tbl.Rows[0]["TypeID"]);
        }
    }

}