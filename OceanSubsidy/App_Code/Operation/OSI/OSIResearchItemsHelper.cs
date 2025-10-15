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

    /// <summary>
    /// 根據ID查詢單筆研究項目
    /// </summary>
    /// <param name="itemID">研究項目ID</param>
    /// <returns></returns>
    public static GS.OCA_OceanSubsidy.Entity.OSI_ResearchItems GetItemByID(string itemID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [ItemID]
    ,[ItemName]
FROM [OCA_OceanSubsidy].[dbo].[OSI_ResearchItems]
WHERE ItemID = @ItemID
";
        db.Parameters.Clear();
        db.Parameters.Add("@ItemID", itemID);

        GisTable tbl = db.GetTable();
        if (tbl != null && tbl.Rows.Count > 0)
        {
            var item = new GS.OCA_OceanSubsidy.Entity.OSI_ResearchItems();
            item.ItemID = Convert.ToInt32(tbl.Rows[0]["ItemID"]);
            item.ItemName = tbl.Rows[0]["ItemName"].ToString();
            return item;
        }

        return null;
    }




}