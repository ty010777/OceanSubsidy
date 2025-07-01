using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;

/// <summary>
/// OSIResearchItemsHelper 的摘要描述
/// </summary>
public class OSIResearchItemsHelper
{
    public OSIResearchItemsHelper()
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
SELECT [ItemID]
    ,[ItemName]
FROM [OCA_OceanSubsidy].[dbo].[OSI_ResearchItems]
";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 判斷該名稱是否存在
    /// </summary>
    /// <returns></returns>
    public static bool IsExistByItemName(string itemName)
    {
        bool rtVal = false;
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT 1
FROM [OCA_OceanSubsidy].[dbo].[OSI_ResearchItems]
WHERE ItemName = @ItemName
";
        db.Parameters.Clear();
        db.Parameters.Add("@ItemName", itemName);

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
SELECT [ItemID]
FROM [OCA_OceanSubsidy].[dbo].[OSI_ResearchItems]
WHERE ItemName = @ItemName
";
        db.Parameters.Clear();
        db.Parameters.Add("@ItemName", name);

        GisTable tbl = db.GetTable();
        if (tbl != null && tbl.Rows.Count > 0)
        {
            return Convert.ToInt32(tbl.Rows[0]["ItemID"]);
        }

        return 0;
    }




}