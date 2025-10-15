using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;

/// <summary>
/// OSIExecutorCategories 的摘要描述
/// </summary>
public class OSIExecutorCategoriesHelper
{
    public OSIExecutorCategoriesHelper()
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
SELECT [CategoryID]
    ,[CategoryName]
FROM [OCA_OceanSubsidy].[dbo].[OSI_ExecutorCategories]
";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 判斷該名稱是否存在
    /// </summary>
    /// <returns></returns>
    public static bool IsExistByCategoryName(string categoryName)
    {
        bool rtVal = false;
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT 1
FROM [OCA_OceanSubsidy].[dbo].[OSI_ExecutorCategories]
WHERE CategoryName = @CategoryName
";
        db.Parameters.Clear();
        db.Parameters.Add("@CategoryName", categoryName);

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
SELECT [CategoryID]
FROM [OCA_OceanSubsidy].[dbo].[OSI_ExecutorCategories]
WHERE CategoryName = @CategoryName
";
        db.Parameters.Clear();
        db.Parameters.Add("@CategoryName", name);

        GisTable tbl = db.GetTable();
        if (tbl != null && tbl.Rows.Count > 0)
        {
            return Convert.ToInt32(tbl.Rows[0]["CategoryID"]);
        }

        return 0;
    }



}