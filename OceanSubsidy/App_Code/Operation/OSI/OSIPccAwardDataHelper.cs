using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;
/// <summary>
/// OSIPccAwardDataHelper 的摘要描述
/// </summary>
public class OSIPccAwardDataHelper
{
    public OSIPccAwardDataHelper()
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
SELECT [Id]
    ,[OrgName]
    ,[AwardNo]
    ,[AwardName]
    ,[AwardNameUrl]
    ,[AwardWay]
    ,[ProctrgCate]
    ,[AwardDate]
    ,[AwardPrice]
    ,[AwardNoticeNo]
    ,[Url]
    ,[QueryParams]
    ,[CreateDate]
    ,[UpdateDate]
    ,[IsValid]
FROM [OCA_OceanSubsidy].[dbo].[OSI_PccAwardData]
WHERE IsValid = 1";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 查詢政府標案資料（支援條件查詢）
    /// </summary>
    /// <param name="awardDateFrom">決標公告開始日期</param>
    /// <param name="awardDateTo">決標公告結束日期</param>
    /// <param name="priceFrom">決標金額最小值</param>
    /// <param name="priceTo">決標金額最大值</param>
    /// <param name="keyword">標案名稱關鍵字</param>
    /// <returns></returns>
    public static GisTable QueryPccAwardData(DateTime? awardDateFrom, DateTime? awardDateTo, 
        decimal? priceFrom, decimal? priceTo, string keyword)
    {
        DbHelper db = new DbHelper();
        string whereClause = " WHERE IsValid = 1 ";
        db.Parameters.Clear();

        // 決標公告日期條件
        if (awardDateFrom.HasValue)
        {
            whereClause += " AND AwardDate >= @AwardDateFrom ";
            db.Parameters.Add("@AwardDateFrom", awardDateFrom.Value);
        }
        if (awardDateTo.HasValue)
        {
            whereClause += " AND AwardDate <= @AwardDateTo ";
            db.Parameters.Add("@AwardDateTo", awardDateTo.Value.AddDays(1).AddSeconds(-1));
        }

        // 決標金額條件（需要處理字串轉數字）
        if (priceFrom.HasValue || priceTo.HasValue)
        {
            whereClause += @" AND TRY_CAST(REPLACE(REPLACE(AwardPrice, ',', ''), '元', '') AS DECIMAL(18,2)) IS NOT NULL ";
            
            if (priceFrom.HasValue)
            {
                whereClause += @" AND TRY_CAST(REPLACE(REPLACE(AwardPrice, ',', ''), '元', '') AS DECIMAL(18,2)) >= @PriceFrom ";
                db.Parameters.Add("@PriceFrom", priceFrom.Value);
            }
            if (priceTo.HasValue)
            {
                whereClause += @" AND TRY_CAST(REPLACE(REPLACE(AwardPrice, ',', ''), '元', '') AS DECIMAL(18,2)) <= @PriceTo ";
                db.Parameters.Add("@PriceTo", priceTo.Value);
            }
        }

        // 標案名稱關鍵字
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            whereClause += " AND AwardName LIKE @Keyword ";
            db.Parameters.Add("@Keyword", "%" + keyword.Trim() + "%");
        }

        // 查詢資料
        db.CommandText = @"
SELECT [Id]
    ,[OrgName]
    ,[AwardNo]
    ,[AwardName]
    ,[AwardNameUrl]
    ,[AwardWay]
    ,[ProctrgCate]
    ,[AwardDate]
    ,[AwardPrice]
    ,[AwardNoticeNo]
    ,[Url]
    ,[QueryParams]
    ,[CreateDate]
    ,[UpdateDate]
    ,[IsValid]
FROM [OCA_OceanSubsidy].[dbo].[OSI_PccAwardData]" + whereClause + @"
ORDER BY AwardDate DESC";

        return db.GetTable();
    }

    /// <summary>
    /// 查詢所有Url
    /// </summary>
    /// <returns></returns>
    public static List<string> QueryAllUrl()
    {
        var rtVal = new List<string>();

        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [Url]
FROM [OCA_OceanSubsidy].[dbo].[OSI_PccAwardData]
WHERE IsValid = 1";
        db.Parameters.Clear();
        db.GetTable();

        GisTable tbl = db.GetTable();
        if (tbl != null && tbl.Rows.Count > 0)
        {
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                rtVal.Add(tbl.Rows[i]["Url"].ToString());
            }
        }


        return rtVal;
    }

    /// <summary>
    /// 獲取最新的決標日期
    /// </summary>
    /// <returns>最新的決標日期，如果沒有資料則返回 null</returns>
    public static DateTime? GetLatestAwardDate()
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
SELECT TOP 1 AwardDate
FROM [OCA_OceanSubsidy].[dbo].[OSI_PccAwardData]
WHERE IsValid = 1 AND AwardDate IS NOT NULL
ORDER BY AwardDate DESC";
        
        db.Parameters.Clear();
        
        var table = db.GetTable();
        if (table != null && table.Rows.Count > 0)
        {
            var dateValue = table.Rows[0]["AwardDate"];
            if (dateValue != DBNull.Value)
            {
                return Convert.ToDateTime(dateValue);
            }
        }
        
        return null;
    }

    /// <summary>
    /// Inert List資料
    /// </summary>
    /// <param name="datas"></param>
    /// <returns></returns>
    public static bool InsertList(List<OSI_PccAwardData> datas)
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
INSERT INTO [OCA_OceanSubsidy].[dbo].[OSI_PccAwardData]
    ([OrgName]
    ,[AwardNo]
    ,[AwardName]
    ,[AwardNameUrl]
    ,[AwardWay]
    ,[ProctrgCate]
    ,[AwardDate]
    ,[AwardPrice]
    ,[AwardNoticeNo]
    ,[Url]
    ,[QueryParams]
    ,[CreateDate]
    ,[UpdateDate]
    ,[IsValid])
VALUES
    (@OrgName
    ,@AwardNo
    ,@AwardName
    ,@AwardNameUrl
    ,@AwardWay
    ,@ProctrgCate
    ,@AwardDate
    ,@AwardPrice
    ,@AwardNoticeNo
    ,@Url
    ,@QueryParams
    ,GETDATE()
    ,GETDATE()
    ,1)";

                db.Parameters.Clear();
                db.Parameters.Add("@OrgName", data.OrgName ?? string.Empty);
                db.Parameters.Add("@AwardNo", data.AwardNo ?? string.Empty);
                db.Parameters.Add("@AwardName", data.AwardName ?? string.Empty);
                db.Parameters.Add("@AwardNameUrl", data.AwardNameUrl ?? string.Empty);
                db.Parameters.Add("@AwardWay", data.AwardWay ?? string.Empty);
                db.Parameters.Add("@ProctrgCate", data.ProctrgCate ?? string.Empty);
                
                if (data.AwardDate.HasValue)
                    db.Parameters.Add("@AwardDate", data.AwardDate.Value);
                else
                    db.Parameters.Add("@AwardDate", DBNull.Value);
                
                db.Parameters.Add("@AwardPrice", data.AwardPrice ?? string.Empty);
                db.Parameters.Add("@AwardNoticeNo", data.AwardNoticeNo ?? string.Empty);
                db.Parameters.Add("@Url", data.Url ?? string.Empty);
                db.Parameters.Add("@QueryParams", data.QueryParams ?? string.Empty);

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