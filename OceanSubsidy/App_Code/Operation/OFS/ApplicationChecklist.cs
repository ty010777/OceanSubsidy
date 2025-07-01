using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// OFSRoleHelper 的摘要描述
/// </summary>
public class ApplicationChecklistHelper
{
    public ApplicationChecklistHelper()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }
    public static List<ReviewChecklistItem> GetLatestApplicationChecklist()
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
        WITH SubsidySummary AS (
    SELECT 
        Version_ID, 
        SUM(SubsidyAmount) AS TotalSubsidyAmount
    FROM 
        [OCA_OceanSubsidy].[dbo].[OFS_SCI_PersonnelCost_TotalFee]
    GROUP BY 
        Version_ID
), RankedVersions AS (
            SELECT *,
                   ROW_NUMBER() OVER (PARTITION BY ProjectID ORDER BY VersionNum DESC) AS rn
            FROM OFS_SCI_Version
        )
        SELECT 
            v.Version_ID,
            v.ProjectID,
            v.VersionNum,
            v.Statuses,
            v.StatusesName,
            v.ExpirationDate,
            v.SupervisoryUnit,	
            v.SupervisoryPersonName,
            v.SupervisoryPersonAccount,
            v.UserAccount,
            v.UserOrg,
            v.UserName,
            m.SubsidyPlanType,
            m.ProjectNameTw,
            m.OrgName,
            m.Year,
            ISNULL(s.TotalSubsidyAmount, 0) AS TotalSubsidyAmount
        FROM RankedVersions v
        LEFT JOIN OFS_SCI_Application_Main m ON v.Version_ID = m.Version_ID
		LEFT JOIN SubsidySummary s on v.Version_ID = s.Version_ID
        WHERE v.rn = 1 AND (v.isExist = 1)
        ORDER BY v.ProjectID DESC
";

        try
        {
            DataTable dt = db.GetTable();
            List<ReviewChecklistItem> resultList = new List<ReviewChecklistItem>();

            foreach (DataRow row in dt.Rows)
            {
                var item = new ReviewChecklistItem
                {
                    // 基本欄位
                    ProjectID = row["ProjectID"]?.ToString(),
                    Version_ID = row["Version_ID"]?.ToString(),
                    VersionNum = row["VersionNum"] != DBNull.Value ? Convert.ToInt32(row["VersionNum"]) : 0,
                    
                    // 申請計畫列表主要欄位
                    Year = row["Year"]?.ToString(),
                    ProjectNameTw = row["ProjectNameTw"]?.ToString(),
                    OrgName = row["OrgName"]?.ToString(),
                    ApplicationAmount = row["TotalSubsidyAmount"] != DBNull.Value ? 
                        Convert.ToDecimal(row["TotalSubsidyAmount"]).ToString("N0") : "0",
                    
                    // 狀態相關
                    Statuses = row["Statuses"]?.ToString(),
                    StatusesName = row["StatusesName"]?.ToString(),
                    ExpirationDate = row["ExpirationDate"] != DBNull.Value ? (DateTime?)row["ExpirationDate"] : null,
                    
                    // 審查相關
                    SupervisoryUnit = row["SupervisoryUnit"]?.ToString(),
                    SupervisoryPersonName = row["SupervisoryPersonName"]?.ToString(),
                    SupervisoryPersonAccount = row["SupervisoryPersonAccount"]?.ToString(),
                    UserAccount = row["UserAccount"]?.ToString(),
                    UserOrg = row["UserOrg"]?.ToString(),
                    UserName = row["UserName"]?.ToString(),
                    SubsidyPlanType = row["SubsidyPlanType"]?.ToString()
                };

                resultList.Add(item);
            }

            return resultList;
        }
        catch (Exception ex)
        {
            throw new Exception($"查詢版本清單時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }
}
