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
public class ReviewCheckListHelper
{
    public ReviewCheckListHelper()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }

    public static List<ReviewChecklistItem> GetLatestVersionPerSource()
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
        WITH RankedVersions AS (
            SELECT *,
                   ROW_NUMBER() OVER (PARTITION BY ProjectID ORDER BY VersionNum DESC) AS rn
            FROM OFS_SCI_Version
        )
        SELECT v.*, 
               m.ProjectNameTw, 
               m.OrgName, 
               m.Year, 
               m.SubsidyPlanType
        FROM RankedVersions v
        LEFT JOIN OFS_SCI_Application_Main m ON v.Version_ID = m.Version_ID
        WHERE v.rn = 1 AND (v.isExist = 1 OR v.isExist IS NULL);";

        try
        {
            DataTable dt = db.GetTable();
            List<ReviewChecklistItem> resultList = new List<ReviewChecklistItem>();

            foreach (DataRow row in dt.Rows)
            {
                var item = new ReviewChecklistItem
                {
                    ProjectID = row["ProjectID"]?.ToString(),
                    Version_ID = row["Version_ID"]?.ToString(),
                    VersionNum = row["VersionNum"] != DBNull.Value ? Convert.ToInt32(row["VersionNum"]) : 0,
                    Statuses = row["Statuses"]?.ToString(),
                    StatusesName = row["StatusesName"]?.ToString(),
                    ExpirationDate = row["ExpirationDate"] != DBNull.Value ? (DateTime?)row["ExpirationDate"] : null,
                    SeqPoint = row["SeqPoint"] != DBNull.Value ? Convert.ToDecimal(row["SeqPoint"]) : 0,
                    SupervisoryUnit = row["SupervisoryUnit"]?.ToString(),
                    SupervisoryPersonName = row["SupervisoryPersonName"]?.ToString(),
                    SupervisoryPersonAccount = row["SupervisoryPersonAccount"]?.ToString(),
                    UserAccount = row["UserAccount"]?.ToString(),
                    UserOrg = row["UserOrg"]?.ToString(),
                    UserName = row["UserName"]?.ToString(),
                    Form1Status = row["Form1Status"]?.ToString(),
                    Form2Status = row["Form2Status"]?.ToString(),
                    Form3Status = row["Form3Status"]?.ToString(),
                    Form4Status = row["Form4Status"]?.ToString(),
                    Form5Status = row["Form5Status"]?.ToString(),
                    Form6Status = row["Form6Status"]?.ToString(),
                    CurrentStep = row["CurrentStep"]?.ToString(),
                    created_at = row["created_at"] != DBNull.Value ? (DateTime?)row["created_at"] : null,
                    updated_at = row["updated_at"] != DBNull.Value ? (DateTime?)row["updated_at"] : null,
                    
                    // Additional fields from Application Main
                    ProjectNameTw = row["ProjectNameTw"]?.ToString(),
                    OrgName = row["OrgName"]?.ToString(),
                    Year = row["Year"]?.ToString(),
                    SubsidyPlanType = row["SubsidyPlanType"]?.ToString(),
                    ApplicationAmount = "0" // TODO: Calculate from funding tables
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
    
    /// <summary>
    /// 根據 Version_ID 列表取得案件資料
    /// </summary>
    /// <param name="Version_ID">Version_ID 列表</param>
    /// <returns>案件列表</returns>
    public static List<ReviewChecklistItem> GetCasesByProjectIds(List<string> projectIds)
    {
        if (projectIds == null || projectIds.Count == 0)
        {
            return new List<ReviewChecklistItem>();
        }
        
        DbHelper db = new DbHelper();
        
        // 建立 IN 條件的參數
        var inClause = string.Join(",", projectIds.Select((id, index) => $"@projectId{index}"));
        
        db.CommandText = $@"
        SELECT v.*, 
               m.ProjectNameTw, 
               m.OrgName, 
               m.Year, 
               m.SubsidyPlanType
        FROM OFS_SCI_Version v
        LEFT JOIN OFS_SCI_Application_Main m ON v.Version_ID = m.Version_ID
        WHERE v.Version_ID IN ({inClause}) AND (v.isExist = 1 OR v.isExist IS NULL)";
        
        // 添加參數
        db.Parameters.Clear();
        for (int i = 0; i < projectIds.Count; i++)
        {
            db.Parameters.Add($"@projectId{i}", projectIds[i]);
        }
        
        try
        {
            DataTable dt = db.GetTable();
            List<ReviewChecklistItem> resultList = new List<ReviewChecklistItem>();

            foreach (DataRow row in dt.Rows)
            {
                var item = new ReviewChecklistItem
                {
                    ProjectID = row["ProjectID"]?.ToString(),
                    Version_ID = row["Version_ID"]?.ToString(),
                    VersionNum = row["VersionNum"] != DBNull.Value ? Convert.ToInt32(row["VersionNum"]) : 0,
                    Statuses = row["Statuses"]?.ToString(),
                    StatusesName = row["StatusesName"]?.ToString(),
                    ExpirationDate = row["ExpirationDate"] != DBNull.Value ? (DateTime?)row["ExpirationDate"] : null,
                    SeqPoint = row["SeqPoint"] != DBNull.Value ? Convert.ToDecimal(row["SeqPoint"]) : 0,
                    SupervisoryUnit = row["SupervisoryUnit"]?.ToString(),
                    SupervisoryPersonName = row["SupervisoryPersonName"]?.ToString(),
                    SupervisoryPersonAccount = row["SupervisoryPersonAccount"]?.ToString(),
                    UserAccount = row["UserAccount"]?.ToString(),
                    UserOrg = row["UserOrg"]?.ToString(),
                    UserName = row["UserName"]?.ToString(),
                    Form1Status = row["Form1Status"]?.ToString(),
                    Form2Status = row["Form2Status"]?.ToString(),
                    Form3Status = row["Form3Status"]?.ToString(),
                    Form4Status = row["Form4Status"]?.ToString(),
                    Form5Status = row["Form5Status"]?.ToString(),
                    Form6Status = row["Form6Status"]?.ToString(),
                    CurrentStep = row["CurrentStep"]?.ToString(),
                    created_at = row["created_at"] != DBNull.Value ? (DateTime?)row["created_at"] : null,
                    updated_at = row["updated_at"] != DBNull.Value ? (DateTime?)row["updated_at"] : null,
                    
                    // Additional fields from Application Main
                    ProjectNameTw = row["ProjectNameTw"]?.ToString(),
                    OrgName = row["OrgName"]?.ToString(),
                    Year = row["Year"]?.ToString(),
                    SubsidyPlanType = row["SubsidyPlanType"]?.ToString(),
                    ApplicationAmount = "0" // TODO: Calculate from funding tables
                };

                resultList.Add(item);
            }

            return resultList;
        }
        catch (Exception ex)
        {
            throw new Exception($"根據ProjectID查詢案件時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }
    
    /// <summary>
    /// 取得指定補助案類型的允許批次通過階段
    /// </summary>
    /// <param name="projectId">計畫編號</param>
    /// <returns>允許批次通過的階段列表</returns>
    public static List<string> GetAllowedBatchStages(string projectId)
    {
        // 根據 ProjectID 判斷補助案類型
        if (string.IsNullOrEmpty(projectId))
        {
            return new List<string>();
        }
        
        if (projectId.Contains("SCI"))  // 科專
        {
            return new List<string> 
            { 
                "資格審查", 
                "領域審查/初審", 
                "技術審查/複審" 
            };
        }
        // 未來可以添加其他補助案類型的階段定義
        // else if (projectId.Contains("CUL"))  // 文化
        // {
        //     return new List<string> { ... };
        // }
        // else if (projectId.Contains("SCH"))  // 學校民間
        // {
        //     return new List<string> { ... };
        // }
        // else if (projectId.Contains("CLB"))  // 學校社團
        // {
        //     return new List<string> { ... };
        // }
        
        // 預設返回科專的階段
        return new List<string> 
        { 
            "資格審查", 
            "領域審查/初審", 
            "技術審查/複審" 
        };
    }
    
    /// <summary>
    /// 驗證指定的案件是否可以進行批次通過
    /// </summary>
    /// <param name="versionIds">Version_ID 列表</param>
    /// <returns>是否全部都可以批次通過</returns>
    public static bool ValidateBatchPassEligibility(List<string> versionIds)
    {
        if (versionIds == null || versionIds.Count == 0)
        {
            return false;
        }
        
        try
        {
            var cases = GetCasesByProjectIds(versionIds);
            
            foreach (var c in cases)
            {
                // 檢查狀態名稱是否為「通過」
                if (c.StatusesName != "通過")
                {
                    return false;
                }
                
                // 檢查是否在允許的階段中
                var allowedStages = GetAllowedBatchStages(c.ProjectID);
                if (!allowedStages.Contains(c.Statuses))
                {
                    return false;
                }
            }
            
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 將指定的案件更新到下一階段（根據當前狀態自動判斷下一階段）
    /// </summary>
    /// <param name="Version_ID">要更新的 Version_ID 列表</param>
    public static void UpdateCasesToNextStage(List<string> Version_ID)
    {
        if (Version_ID == null || Version_ID.Count == 0)
        {
            return;
        }

        using (DbHelper db = new DbHelper())
        {
            try
            {
                // 建立 IN 子句參數
                var paramNames = Version_ID.Select((id, index) => $"@Version_ID{index}").ToList();
                string inClause = string.Join(",", paramNames);

                // 使用 CASE WHEN 根據當前狀態決定下一階段
                // 所有案件的 StatusesName 都從「通過」變成「審查中」
                db.CommandText = $@"
                UPDATE OFS_SCI_Version 
                SET Statuses = CASE Statuses
                                 WHEN '資格審查' THEN '領域審查/初審'
                                 WHEN '領域審查/初審' THEN '技術審查/複審'  
                                 WHEN '技術審查/複審' THEN '決審核定'
                                 ELSE Statuses  -- 如果不在預期階段則保持原狀
                               END,
                    StatusesName = '審查中',
                    updated_at = GETDATE()
                WHERE Version_ID IN ({inClause})
                  AND StatusesName = '通過'
                  AND Statuses IN ('資格審查', '領域審查/初審', '技術審查/複審')
            ";

                db.Parameters.Clear();
                for (int i = 0; i < Version_ID.Count; i++)
                {
                    db.Parameters.Add($"@Version_ID{i}", Version_ID[i]);
                }
                
                db.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"更新案件狀態時發生錯誤：{ex.Message}", ex);
            }
        }
    }
    
    /// <summary>
    /// 驗證指定的案件是否可以進行批次不通過
    /// </summary>
    /// <param name="versionIds">Version_ID 列表</param>
    /// <returns>是否全部都可以批次不通過</returns>
    public static bool ValidateBatchRejectEligibility(List<string> versionIds)
    {
        if (versionIds == null || versionIds.Count == 0)
        {
            return false;
        }
        
        try
        {
            var cases = GetCasesByProjectIds(versionIds);
            
            foreach (var c in cases)
            {
                // 檢查狀態名稱是否為「不通過」或「逾期未補」
                if (c.StatusesName != "不通過" && c.StatusesName != "逾期未補")
                {
                    return false;
                }
            }
            
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    /// <summary>
    /// 將指定的案件批次不通過並結案
    /// </summary>
    /// <param name="Version_ID">要更新的 Version_ID 列表</param>
    public static void BatchRejectCases(List<string> Version_ID)
    {
        if (Version_ID == null || Version_ID.Count == 0)
        {
            return;
        }

        using (DbHelper db = new DbHelper())
        {
            try
            {
                // 建立 IN 子句參數
                var paramNames = Version_ID.Select((id, index) => $"@Version_ID{index}").ToList();
                string inClause = string.Join(",", paramNames);

                // 將案件設為結案(未通過)狀態，並設定 isExist = 0
                db.CommandText = $@"
                UPDATE OFS_SCI_Version 
                SET Statuses = '結案(未通過)',
                    StatusesName = '結案(未通過)',
                    isExist = 0,
                    updated_at = GETDATE()
                WHERE Version_ID IN ({inClause})
                  AND StatusesName IN ('不通過', '逾期未補')
            ";

                db.Parameters.Clear();
                for (int i = 0; i < Version_ID.Count; i++)
                {
                    db.Parameters.Add($"@Version_ID{i}", Version_ID[i]);
                }
                
                db.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"批次不通過案件時發生錯誤：{ex.Message}", ex);
            }
        }
    }
}
