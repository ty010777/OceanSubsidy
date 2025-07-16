using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.Extension;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// OSICarrierHelper 的摘要描述
/// </summary>
public class OSICarrierHelper
{
    public OSICarrierHelper()
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
SELECT [CarrierID]
      ,[ReportID]
      ,[CarrierTypeID]
      ,[CarrierDetail]
      ,[CarrierNo]
      ,[IsValid]
      ,[CreatedAt]
      ,[DeletedAt]
      ,[DeletedBy]
  FROM [OCA_OceanSubsidy].[dbo].[OSI_Carrier]
";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 查詢 BY ReportID
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryByReportID(string reportID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [CarrierID]
      ,[ReportID]
      ,[CarrierTypeID]
      ,[CarrierDetail]
      ,[CarrierNo]
      ,[IsValid]
      ,[CreatedAt]
      ,[DeletedAt]
      ,[DeletedBy]
FROM [OCA_OceanSubsidy].[dbo].[OSI_Carrier]
WHERE IsValid = 1 
AND ReportID = @ReportID
";
        db.Parameters.Clear();
        db.Parameters.Add("@ReportID", reportID);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢 BY ReportID 並關聯載具類型名稱
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryNameByReportID(string reportID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT c.[CarrierID]
      ,c.[ReportID]
      ,c.[CarrierTypeID]
      ,ct.[CarrierTypeName]
      ,c.[CarrierDetail]
      ,c.[CarrierNo]
      ,c.[IsValid]
      ,c.[CreatedAt]
      ,c.[DeletedAt]
      ,c.[DeletedBy]
FROM [OCA_OceanSubsidy].[dbo].[OSI_Carrier] c
LEFT JOIN [OCA_OceanSubsidy].[dbo].[OSI_CarrierTypes] ct ON c.CarrierTypeID = ct.CarrierTypeID
WHERE c.IsValid = 1 
AND c.ReportID = @ReportID
";
        db.Parameters.Clear();
        db.Parameters.Add("@ReportID", reportID);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢 BY ReportID 回傳 List<OSI_Carrier>
    /// </summary>
    /// <returns></returns>
    public static List<OSI_Carrier> QueryByReportIDWithClass(string reportID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [CarrierID]
      ,[ReportID]
      ,[CarrierTypeID]
      ,[CarrierDetail]
      ,[CarrierNo]
      ,[IsValid]
      ,[CreatedAt]
      ,[DeletedAt]
      ,[DeletedBy]
FROM [OCA_OceanSubsidy].[dbo].[OSI_Carrier]
WHERE IsValid = 1 
AND ReportID = @ReportID
";
        db.Parameters.Clear();
        db.Parameters.Add("@ReportID", reportID);

        var tbl = db.GetTable();
        var list = new List<OSI_Carrier>();
        
        foreach (DataRow row in tbl.Rows)
        {
            var carrier = new OSI_Carrier
            {
                CarrierID = row["CarrierID"].ToString().toInt(),
                ReportID = row["ReportID"].ToString().toInt(),
                CarrierTypeID = row["CarrierTypeID"] == DBNull.Value ? null : (int?)row["CarrierTypeID"].ToString().toInt(),
                CarrierDetail = row["CarrierDetail"].ToString(),
                CarrierNo = row["CarrierNo"].ToString(),
                IsValid = row["IsValid"].ToString() == "1" || row["IsValid"].ToString().ToLower() == "true",
                CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                DeletedAt = row["DeletedAt"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["DeletedAt"]),
                DeletedBy = row["DeletedBy"].ToString()
            };
            list.Add(carrier);
        }

        return list;
    }

}