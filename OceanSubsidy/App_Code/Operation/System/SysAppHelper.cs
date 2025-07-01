using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;

/// <summary>
/// SysAppHelper 的摘要描述
/// </summary>
public class SysAppHelper
{
    public SysAppHelper()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }

    /// <summary>
    /// 查詢所有
    /// </summary>
    /// <returns></returns>
    public static GisTable QuerySysApp()
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT [SystemID]
            ,[SystemName]
            FROM Sys_App";
        db.Parameters.Clear();

        return db.GetTable();
    }
    /// <summary>
    /// 查詢
    /// </summary>
    /// <returns></returns>
    public static int QueryIDBySystemName(string systemName)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [SystemID]
    ,[SystemName]
FROM Sys_App
WHERE SystemName = @SystemName";
        db.Parameters.Clear();
        db.Parameters.Add("@SystemName", systemName);
        var tbl = db.GetTable();

        if (tbl.Rows.Count == 0)
        {
            return -1;
        }
        else
        {
            return Convert.ToInt32(tbl.Rows[0]["SystemID"]);
        }
    }


}