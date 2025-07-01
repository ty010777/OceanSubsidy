using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;

/// <summary>
/// OSICarrierTypesHelper 的摘要描述
/// </summary>
public class OSICarrierTypesHelper
{
    public OSICarrierTypesHelper()
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
SELECT [CarrierTypeID]
    ,[CarrierTypeName]
FROM [OCA_OceanSubsidy].[dbo].[OSI_CarrierTypes]
";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 判斷該名稱是否存在
    /// </summary>
    /// <returns></returns>
    public static bool IsExistByCarrierTypeName(string carrierTypeName)
    {
        bool rtVal = false;
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT 1
FROM [OCA_OceanSubsidy].[dbo].[OSI_CarrierTypes]
WHERE CarrierTypeName = @CarrierTypeName
";
        db.Parameters.Clear();
        db.Parameters.Add("@CarrierTypeName", carrierTypeName);

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
SELECT [CarrierTypeID]
FROM [OCA_OceanSubsidy].[dbo].[OSI_CarrierTypes]
WHERE CarrierTypeName = @CarrierTypeName
";
        db.Parameters.Clear();
        db.Parameters.Add("@CarrierTypeName", name);

        GisTable tbl = db.GetTable();
        if (tbl != null && tbl.Rows.Count > 0)
        {
            return Convert.ToInt32(tbl.Rows[0]["CarrierTypeID"]);
        }

        return 0;
    }



}