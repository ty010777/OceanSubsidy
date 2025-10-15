using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// OSINSTCDataHelper 的摘要描述
/// </summary>
public class OSINSTCDataHelper
{
    public OSINSTCDataHelper()
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
SELECT [ID]
    ,[Year]
    ,[Unit]
    ,[tName]
    ,[TotalApprovedAmount]
    ,[ExecutionStart]
    ,[ExecutionEnd]
    ,[CreateDate]
    ,[UpdateDate]
    ,[IsValid]
FROM [OCA_OceanSubsidy].[dbo].[OSI_NSTCData]
WHERE IsValid = 1";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 查詢國科會補助研究計畫資料（支援條件查詢）
    /// </summary>
    /// <param name="year">年度</param>
    /// <param name="amountFrom">總核定金額最小值</param>
    /// <param name="amountTo">總核定金額最大值</param>
    /// <param name="projectName">計畫名稱關鍵字</param>
    /// <returns></returns>
    public static GisTable QueryNSTCData(string year, decimal? amountFrom, decimal? amountTo, string projectName)
    {
        DbHelper db = new DbHelper();
        string whereClause = " WHERE IsValid = 1 ";
        db.Parameters.Clear();

        // 年度條件
        if (!string.IsNullOrWhiteSpace(year))
        {
            whereClause += " AND Year = @Year ";
            db.Parameters.Add("@Year", year);
        }

        // 總核定金額條件（需要處理字串轉數字）
        if (amountFrom.HasValue || amountTo.HasValue)
        {
            whereClause += @" AND TRY_CAST(REPLACE(REPLACE(TotalApprovedAmount, ',', ''), '元', '') AS DECIMAL(18,2)) IS NOT NULL ";
            
            if (amountFrom.HasValue)
            {
                whereClause += @" AND TRY_CAST(REPLACE(REPLACE(TotalApprovedAmount, ',', ''), '元', '') AS DECIMAL(18,2)) >= @AmountFrom ";
                db.Parameters.Add("@AmountFrom", amountFrom.Value);
            }
            if (amountTo.HasValue)
            {
                whereClause += @" AND TRY_CAST(REPLACE(REPLACE(TotalApprovedAmount, ',', ''), '元', '') AS DECIMAL(18,2)) <= @AmountTo ";
                db.Parameters.Add("@AmountTo", amountTo.Value);
            }
        }

        // 計畫名稱關鍵字
        if (!string.IsNullOrWhiteSpace(projectName))
        {
            whereClause += " AND tName LIKE @ProjectName ";
            db.Parameters.Add("@ProjectName", "%" + projectName.Trim() + "%");
        }

        // 查詢資料
        db.CommandText = @"
SELECT [ID]
    ,[Year]
    ,[Unit]
    ,[tName]
    ,[TotalApprovedAmount]
    ,[ExecutionStart]
    ,[ExecutionEnd]
    ,[CreateDate]
    ,[UpdateDate]
    ,[IsValid]
FROM [OCA_OceanSubsidy].[dbo].[OSI_NSTCData]" + whereClause + @"
ORDER BY ID DESC";

        return db.GetTable();
    }

    /// <summary>
    /// 獲取所有不重複的年度
    /// </summary>
    /// <returns>年度列表</returns>
    public static List<string> GetDistinctYears()
    {
        var rtVal = new List<string>();

        DbHelper db = new DbHelper();
        db.CommandText = @"
SELECT DISTINCT Year
FROM [OCA_OceanSubsidy].[dbo].[OSI_NSTCData]
WHERE IsValid = 1 AND Year IS NOT NULL AND Year <> ''
ORDER BY Year DESC";
        
        db.Parameters.Clear();
        
        GisTable tbl = db.GetTable();
        if (tbl != null && tbl.Rows.Count > 0)
        {
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                rtVal.Add(tbl.Rows[i]["Year"].ToString());
            }
        }

        return rtVal;
    }

    /// <summary>
    /// 獲取最新的更新日期
    /// </summary>
    /// <returns>最新的更新日期，如果沒有資料則返回 null</returns>
    public static DateTime? GetLatestUpdateDate()
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
SELECT TOP 1 UpdateDate
FROM [OCA_OceanSubsidy].[dbo].[OSI_NSTCData]
WHERE IsValid = 1 AND UpdateDate IS NOT NULL
ORDER BY UpdateDate DESC";
        
        db.Parameters.Clear();
        
        var table = db.GetTable();
        if (table != null && table.Rows.Count > 0)
        {
            var dateValue = table.Rows[0]["UpdateDate"];
            if (dateValue != DBNull.Value)
            {
                return Convert.ToDateTime(dateValue);
            }
        }
        
        // 如果沒有更新日期，則嘗試獲取最新的建立日期
        db.CommandText = @"
SELECT TOP 1 CreateDate
FROM [OCA_OceanSubsidy].[dbo].[OSI_NSTCData]
WHERE IsValid = 1 AND CreateDate IS NOT NULL
ORDER BY CreateDate DESC";
        
        table = db.GetTable();
        if (table != null && table.Rows.Count > 0)
        {
            var dateValue = table.Rows[0]["CreateDate"];
            if (dateValue != DBNull.Value)
            {
                return Convert.ToDateTime(dateValue);
            }
        }
        
        return null;
    }

    /// <summary>
    /// 查詢所有計畫名稱 (用於重複檢查)
    /// </summary>
    /// <returns></returns>
    public static List<string> QueryAllProjectNames()
    {
        var rtVal = new List<string>();

        DbHelper db = new DbHelper();
        db.CommandText = @"
SELECT [tName]
FROM [OCA_OceanSubsidy].[dbo].[OSI_NSTCData]
WHERE IsValid = 1 AND tName IS NOT NULL AND tName <> ''";
        db.Parameters.Clear();
        
        GisTable tbl = db.GetTable();
        if (tbl != null && tbl.Rows.Count > 0)
        {
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                rtVal.Add(tbl.Rows[i]["tName"].ToString());
            }
        }

        return rtVal;
    }

    /// <summary>
    /// Insert List資料
    /// </summary>
    /// <param name="datas"></param>
    /// <returns></returns>
    public static bool InsertList(List<OSI_NSTCData> datas)
    {
        bool rtVal = false;

        if (datas == null || datas.Count == 0)
            return rtVal;

        DbHelper db = new DbHelper();
        
        try
        {
            // 開始交易
            db.BeginTrans();

            foreach (var data in datas)
            {
                db.CommandText = @"
INSERT INTO [OCA_OceanSubsidy].[dbo].[OSI_NSTCData]
    ([Year]
    ,[Unit]
    ,[tName]
    ,[TotalApprovedAmount]
    ,[ExecutionStart]
    ,[ExecutionEnd]
    ,[CreateDate]
    ,[UpdateDate]
    ,[IsValid])
VALUES
    (@Year
    ,@Unit
    ,@tName
    ,@TotalApprovedAmount
    ,@ExecutionStart
    ,@ExecutionEnd
    ,GETDATE()
    ,GETDATE()
    ,1)";

                db.Parameters.Clear();
                db.Parameters.Add("@Year", data.Year ?? string.Empty);
                db.Parameters.Add("@Unit", data.Unit ?? string.Empty);
                db.Parameters.Add("@tName", data.tName ?? string.Empty);
                db.Parameters.Add("@TotalApprovedAmount", data.TotalApprovedAmount ?? string.Empty);
                
                if (data.ExecutionStart.HasValue)
                    db.Parameters.Add("@ExecutionStart", data.ExecutionStart.Value);
                else
                    db.Parameters.Add("@ExecutionStart", DBNull.Value);
                
                if (data.ExecutionEnd.HasValue)
                    db.Parameters.Add("@ExecutionEnd", data.ExecutionEnd.Value);
                else
                    db.Parameters.Add("@ExecutionEnd", DBNull.Value);

                db.ExecuteNonQuery();
            }

            // 提交交易
            db.Commit();
            rtVal = true;
        }
        catch (Exception ex)
        {
            // 發生錯誤時回滾交易
            db.Rollback();
            throw ex;
        }
        finally
        {
            db.Dispose();
        }

        return rtVal;
    }

}