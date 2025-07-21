using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;

/// <summary>
/// OSIActivityNaturesHelper 的摘要描述
/// </summary>
public class OSIActivityNaturesHelper
{
    public OSIActivityNaturesHelper()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
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
SELECT [NatureID]
    ,[NatureName]
FROM [OCA_OceanSubsidy].[dbo].[OSI_ActivityNatures]
";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 判斷該名稱是否存在
    /// </summary>
    /// <returns></returns>
    public static bool IsExistByNatureName(string natureName)
    {
        bool rtVal = false;
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT 1
FROM [OCA_OceanSubsidy].[dbo].[OSI_ActivityNatures]
WHERE NatureName = @NatureName
";
        db.Parameters.Clear();
        db.Parameters.Add("@NatureName", natureName);

        GisTable tbl = db.GetTable();
        if (tbl != null && tbl.Rows.Count > 0)
        {
            rtVal = true;
        }

        return rtVal;
    }

    /// <summary>
    /// 查詢ID By Name
    /// </summary>
    /// <param name="Name"></param>
    /// <returns></returns>
    public static int QueryIDByName(string name)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [NatureID]
FROM [OCA_OceanSubsidy].[dbo].[OSI_ActivityNatures]
WHERE NatureName = @NatureName
";
        db.Parameters.Clear();
        db.Parameters.Add("@NatureName", name);

        GisTable tbl = db.GetTable();
        if (tbl != null && tbl.Rows.Count > 0)
        {
            return Convert.ToInt32(tbl.Rows[0]["NatureID"]);
        }

        return 0;
    }

    /// <summary>
    /// 根據ID查詢單筆活動性質
    /// </summary>
    /// <param name="natureID">活動性質ID</param>
    /// <returns></returns>
    public static GS.OCA_OceanSubsidy.Entity.OSI_ActivityNatures GetNatureByID(string natureID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [NatureID]
    ,[NatureName]
FROM [OCA_OceanSubsidy].[dbo].[OSI_ActivityNatures]
WHERE NatureID = @NatureID
";
        db.Parameters.Clear();
        db.Parameters.Add("@NatureID", natureID);

        GisTable tbl = db.GetTable();
        if (tbl != null && tbl.Rows.Count > 0)
        {
            var nature = new GS.OCA_OceanSubsidy.Entity.OSI_ActivityNatures();
            nature.NatureID = Convert.ToInt32(tbl.Rows[0]["NatureID"]);
            nature.NatureName = tbl.Rows[0]["NatureName"].ToString();
            return nature;
        }

        return null;
    }



}