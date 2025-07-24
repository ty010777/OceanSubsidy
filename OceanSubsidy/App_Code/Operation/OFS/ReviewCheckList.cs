using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Model.OFS;

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
    
    #region 下拉選單資料載入方法
    

    /// <summary>
    /// 取得審查階段的狀態選項（固定選項）
    /// </summary>
    /// <returns>階段狀態清單</returns>
    public static List<DropdownItem> GetReviewStageStatusOptions()
    {
        // 固定的階段狀態選項
        List<DropdownItem> result = new List<DropdownItem>
        {
            new DropdownItem { Value = "", Text = "全部" },
            new DropdownItem { Value = "審查中", Text = "審查中" },
            new DropdownItem { Value = "通過", Text = "通過" },
            new DropdownItem { Value = "未通過", Text = "未通過" },
            new DropdownItem { Value = "補正補件", Text = "補正補件" },
            new DropdownItem { Value = "逾期未補", Text = "逾期未補" }
        };

        return result;
    }

    /// <summary>
    /// 取得指定審查階段的申請單位選項
    /// </summary>
    /// <param name="status">審查階段狀態</param>
    /// <returns>申請單位清單</returns>
    public static List<DropdownItem> GetReviewOrgOptions(string status = "")
    {
        DbHelper db = new DbHelper();
        
        db.CommandText = @"
            SELECT DISTINCT UserOrg
            FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main]
            WHERE UserOrg != '' AND UserOrg IS NOT NULL
        ";
        
        // 如果有指定審查階段，加入篩選條件
        if (!string.IsNullOrEmpty(status))
        {
            db.CommandText += " AND Statuses LIKE @status";
        }
        
        try
        {
            db.Parameters.Clear();
            if (!string.IsNullOrEmpty(status))
            {
                db.Parameters.Add("@status", $"%{status}%");
            }
            
            DataTable dt = db.GetTable();
            List<DropdownItem> result = new List<DropdownItem>();

            // 加入「全部申請單位」選項
            result.Add(new DropdownItem { Value = "", Text = "全部申請單位" });

            foreach (DataRow row in dt.Rows)
            {
                if (row["UserOrg"] != DBNull.Value)
                {
                    string org = row["UserOrg"].ToString();
                    result.Add(new DropdownItem { Value = org, Text = org });
                }
            }
            
            // TODO  目前只有科專，之後還要有文化、學校民間、學校社團等等的申請單位 和併入到result。
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"取得申請單位選項時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 取得指定審查階段的承辦人員選項
    /// </summary>
    /// <param name="status">審查階段狀態</param>
    /// <returns>承辦人員清單</returns>
    public static List<DropdownItem> GetReviewSupervisorOptions(string status = "")
    {
        DbHelper db = new DbHelper();
        
        db.CommandText = @"
            SELECT DISTINCT SupervisoryPersonAccount, SupervisoryPersonName
            FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main]
            WHERE SupervisoryPersonAccount != '' AND SupervisoryPersonAccount IS NOT NULL
              AND SupervisoryPersonName != '' AND SupervisoryPersonName IS NOT NULL
        ";
        
        // 如果有指定審查階段，加入篩選條件
        if (!string.IsNullOrEmpty(status))
        {
            db.CommandText += " AND Statuses LIKE @status";
        }

        try
        {
            db.Parameters.Clear();
            if (!string.IsNullOrEmpty(status))
            {
                db.Parameters.Add("@status", $"%{status}%");
            }
            
            DataTable dt = db.GetTable();
            List<DropdownItem> result = new List<DropdownItem>();

            // 加入「全部」選項
            result.Add(new DropdownItem { Value = "", Text = "全部" });

            foreach (DataRow row in dt.Rows)
            {
                if (row["SupervisoryPersonName"] != DBNull.Value)
                {
                    string account = row["SupervisoryPersonAccount"].ToString();
                    string name = row["SupervisoryPersonName"].ToString();
                    result.Add(new DropdownItem { Value = account, Text = name });
                }
            }
            
            // TODO  目前只有科專，之後還要有文化、學校民間、學校社團等等的承辦人員 和併入到result。

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"取得承辦人員選項時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }
    

    /// <summary>
    /// 取得審查進度選項（固定選項）
    /// </summary>
    /// <returns>審查進度清單</returns>
    public static List<DropdownItem> GetReviewProgressOptions()
    {
        // 固定的審查進度選項
        List<DropdownItem> result = new List<DropdownItem>
        {
            new DropdownItem { Value = "", Text = "全部" },
            new DropdownItem { Value = "進行中", Text = "進行中" },
            new DropdownItem { Value = "已完成", Text = "已完成" }
        };

        return result;
    }

    /// <summary>
    /// 取得回覆狀態選項（固定選項）
    /// </summary>
    /// <returns>回覆狀態清單</returns>
    public static List<DropdownItem> GetReviewReplyStatusOptions()
    {
        // 固定的回覆狀態選項
        List<DropdownItem> result = new List<DropdownItem>
        {
            new DropdownItem { Value = "", Text = "全部" },
            new DropdownItem { Value = "進行中", Text = "進行中" },
            new DropdownItem { Value = "已完成", Text = "已完成" }
        };

        return result;
    }

    #endregion

    #region type-1 Search 科專

    public static List<ReviewChecklistItem> Search_SCI_Type1(
        string year = "", 
        string status = "", 
        string orgName = "", 
        string supervisor = "", 
        string keyword = "")
    { 
        
        DbHelper db = new DbHelper();
        db.CommandText = $@"
WITH ProjectSubsidySummary AS (
    SELECT 
        PM.[ProjectID],
        AM.ProjectNameTw,
        [UserOrg],
        [StatusesName],
        [ExpirationDate],
        [SupervisoryPersonAccount],
        [SupervisoryPersonName],
        [SupervisoryUnit],
        PM.created_at,
        SUM(SubsidyAmount) AS SubsidyAmount
    FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main] PM
    LEFT JOIN OFS_SCI_Application_Main AM
        ON AM.ProjectID = PM.ProjectID 
    LEFT JOIN OFS_SCI_PersonnelCost_TotalFee PT
        ON PM.ProjectID = PT.ProjectID 
    WHERE PM.isExist = 1 
      AND Statuses = '資格審查'
    GROUP BY 
        PM.[ProjectID],
        AM.ProjectNameTw,
        [UserOrg],
        [StatusesName],
        [ExpirationDate],
        [SupervisoryPersonAccount],
        [SupervisoryPersonName],
        [SupervisoryUnit],
        PM.created_at
)

SELECT *,
    '科專' AS Category,
    YEAR(created_at) - 1911 AS [Year]
FROM ProjectSubsidySummary
";
        try
        {
            List<ReviewChecklistItem> checklist = new List<ReviewChecklistItem>();
            db.Parameters.Clear();
            
            // 添加篩選條件參數
            if (!string.IsNullOrEmpty(year))
            {
                db.CommandText += " WHERE [Year] = @year";
                db.Parameters.Add("@year", year);
            }
            
            if (!string.IsNullOrEmpty(status))
            {
                if (db.CommandText.Contains("WHERE"))
                    db.CommandText += " AND StatusesName LIKE @status";
                else
                    db.CommandText += " WHERE StatusesName LIKE @status";
                db.Parameters.Add("@status", $"%{status}%");
            }
            
            if (!string.IsNullOrEmpty(orgName))
            {
                if (db.CommandText.Contains("WHERE"))
                    db.CommandText += " AND UserOrg LIKE @orgName";
                else
                    db.CommandText += " WHERE UserOrg LIKE @orgName";
                db.Parameters.Add("@orgName", $"%{orgName}%");
            }
            
            if (!string.IsNullOrEmpty(supervisor))
            {
                if (db.CommandText.Contains("WHERE"))
                    db.CommandText += " AND SupervisoryPersonAccount = @supervisor";
                else
                    db.CommandText += " WHERE SupervisoryPersonAccount = @supervisor";
                db.Parameters.Add("@supervisor", supervisor);
            }
            
            if (!string.IsNullOrEmpty(keyword))
            {
                if (db.CommandText.Contains("WHERE"))
                    db.CommandText += " AND (ProjectID LIKE @keyword OR ProjectNameTw LIKE @keyword)";
                else
                    db.CommandText += " WHERE (ProjectID LIKE @keyword OR ProjectNameTw LIKE @keyword)";
                db.Parameters.Add("@keyword", $"%{keyword}%");
            }
            
            // 執行查詢
            DataTable dt = db.GetTable();
            
            // 將查詢結果轉換為 ReviewChecklistItem 物件
            foreach (DataRow row in dt.Rows)
            {
                var item = new ReviewChecklistItem
                {
                    ProjectID = row["ProjectID"]?.ToString(),
                    Statuses = "資格審查",
                    StatusesName = row["StatusesName"]?.ToString(),
                    ExpirationDate = row["ExpirationDate"] != DBNull.Value ? (DateTime?)row["ExpirationDate"] : null,
                    SupervisoryUnit = row["SupervisoryUnit"]?.ToString(),
                    SupervisoryPersonName = row["SupervisoryPersonName"]?.ToString(),
                    SupervisoryPersonAccount = row["SupervisoryPersonAccount"]?.ToString(),
                    UserOrg = row["UserOrg"]?.ToString(),
                    created_at = row["created_at"] != DBNull.Value ? (DateTime?)row["created_at"] : null,
                    
                    // 從 SQL 結果設定的額外欄位
                    ProjectNameTw = row["ProjectNameTw"]?.ToString(),
                    Year = row["Year"]?.ToString(),
                    ApplicationAmount = row["SubsidyAmount"]?.ToString() ?? "0"
                };
                
                checklist.Add(item);
            }
            
            return checklist;
        }
        catch (Exception ex)
        {
            throw new Exception($"查詢審查清單時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
        
    }


    #endregion
    #region type-2 Search 科專
    /// <summary>
    /// 查詢科專領域審查清單
    /// </summary>
    /// <param name="year">年度</param>
    /// <param name="status">狀態</param>
    /// <param name="orgName">申請單位</param>
    /// <param name="supervisor">承辦人員</param>
    /// <param name="keyword">關鍵字</param>
    /// <param name="reviewProgress">審查進度</param>
    /// <param name="replyProgress">回覆進度</param>
    /// <returns>科專審查清單</returns>
    
    public static List<ReviewChecklistItem> Search_SCI_Type2(
        string  year = "",
        string orgName = "",
        string supervisor = "",
        string keyword = "",
        string reviewProgress = "",
        string replyProgress = "")
    {
        DbHelper db = new DbHelper();
        
        try
        {
            // 直接查詢科專領域審查資料
            return GetSciProjectData(db, year,  orgName, supervisor, keyword, reviewProgress, replyProgress);
        }
        catch (Exception ex)
        {
            throw new Exception($"查詢審查清單時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 取得科專專案資料
    /// </summary>
    private static List<ReviewChecklistItem> GetSciProjectData(
        DbHelper db,
        string year,
        string orgName,
        string supervisor,
        string keyword,
        string reviewProgress,
        string replyProgress,
        string status = "領域審查")
    {
        // 1. 先取得科專的基本專案資料
        var basicProjects = GetSciBasicData(db, year, orgName, supervisor, keyword, status);
        
        if (basicProjects.Count == 0)
        {
            return new List<ReviewChecklistItem>();
        }
        
        // 2. 取得進度資料
        var projectIds = basicProjects.Select(p => p.ProjectID).ToList();
        var progressData = GetSciProgressData(db, projectIds, reviewProgress, replyProgress);
        
        // 3. 取得審查組別資料
        var reviewGroupData = GetSciReviewGroupData(db, projectIds);
        
        // 4. 組合結果
        List<ReviewChecklistItem> result = new List<ReviewChecklistItem>();
        
        foreach (var project in basicProjects)
        {
            var progress = progressData.FirstOrDefault(p => p.ProjectID == project.ProjectID);
            var reviewGroup = reviewGroupData.FirstOrDefault(r => r.ProjectID == project.ProjectID);
            
            var item = new ReviewChecklistItem
            {
                ProjectID = project.ProjectID,
                Statuses = project.Statuses,
                StatusesName = project.StatusesName,
                ExpirationDate = project.ExpirationDate,
                SeqPoint = project.SeqPoint,
                SupervisoryUnit = project.SupervisoryUnit,
                SupervisoryPersonName = project.SupervisoryPersonName,
                SupervisoryPersonAccount = project.SupervisoryPersonAccount,
                UserAccount = project.UserAccount,
                UserOrg = project.UserOrg,
                UserName = project.UserName,
                Form1Status = project.Form1Status,
                Form2Status = project.Form2Status,
                Form3Status = project.Form3Status,
                Form4Status = project.Form4Status,
                Form5Status = project.Form5Status,
                CurrentStep = project.CurrentStep,
                created_at = project.created_at,
                updated_at = project.updated_at,
                
                // Additional fields from Application Main
                ProjectNameTw = project.ProjectNameTw,
                OrgName = project.OrgName,
                Year = project.Year,
                SubsidyPlanType = project.SubsidyPlanType,
                ApplicationAmount = project.ApplicationAmount,
                
                // Progress fields
                ReviewProgress = progress?.ReviewProgress,
                ReplyProgress = progress?.ReplyProgress,
                ReviewProgressDisplay = progress?.ReviewProgressDisplay,
                ReplyProgressDisplay = progress?.ReplyProgressDisplay,
                Field_Descname = reviewGroup?.Field_Descname // 審查組別
            };

            result.Add(item);
        }
        
        // 統一進行進度篩選
        if (!string.IsNullOrEmpty(reviewProgress) || !string.IsNullOrEmpty(replyProgress))
        {
            result = result.Where(item => 
            {
                bool includeItem = true;
                
                // 審查進度篩選
                if (!string.IsNullOrEmpty(reviewProgress))
                {
                    if (reviewProgress == "已完成" && item.ReviewProgress != "完成")
                        includeItem = false;
                    else if (reviewProgress == "進行中" && item.ReviewProgress != "未完成")
                        includeItem = false;
                }
                
                // 回覆進度篩選
                if (!string.IsNullOrEmpty(replyProgress) && includeItem)
                {
                    if (replyProgress == "已完成" && item.ReplyProgress != "完成")
                        includeItem = false;
                    else if (replyProgress == "進行中" && item.ReplyProgress != "未完成")
                        includeItem = false;
                }
                
                return includeItem;
            }).ToList();
        }
        
        // 按照更新時間和專案編號排序
        return result.OrderByDescending(r => r.updated_at).ThenByDescending(r => r.ProjectID).ToList();
    }

    /// <summary>
    /// 取得科專基本資料
    /// </summary>
    private static List<ReviewChecklistItem> GetSciBasicData(
        DbHelper db,
        string year,
        string orgName,
        string supervisor,
        string keyword,
        string status = "領域審查")
    {
        db.CommandText = $@"
            SELECT p.*, 
                   m.ProjectNameTw, 
                   m.OrgName, 
                   YEAR(p.created_at) - 1911 AS 'Year',
                   m.SubsidyPlanType,
                   '科專' AS 'Category',
                   ISNULL((SELECT SUM(SubsidyAmount) 
                           FROM OFS_SCI_PersonnelCost_TotalFee tf 
                           WHERE tf.ProjectID = p.ProjectID), 0) AS 'ApplicationAmount'
            FROM OFS_SCI_Project_Main p
            LEFT JOIN OFS_SCI_Application_Main m ON p.ProjectID = m.ProjectID
            WHERE p.Statuses LIKE '%{status}%'
              AND (p.isExist = 1 OR p.isExist IS NULL)
        ";

        // 添加篩選條件
        db.Parameters.Clear();
        
        if (!string.IsNullOrEmpty(year))
        {
            db.CommandText += " AND YEAR(p.created_at) - 1911 = @year";
            db.Parameters.Add("@year", year);
        }
        
        
        if (!string.IsNullOrEmpty(orgName))
        {
            db.CommandText += " AND p.UserOrg LIKE @orgName";
            db.Parameters.Add("@orgName", $"%{orgName}%");
        }
        
        if (!string.IsNullOrEmpty(supervisor))
        {
            db.CommandText += " AND p.SupervisoryPersonAccount = @supervisor";
            db.Parameters.Add("@supervisor", supervisor);
        }
        
        if (!string.IsNullOrEmpty(keyword))
        {
            db.CommandText += " AND (p.ProjectID LIKE @keyword OR m.ProjectNameTw LIKE @keyword)";
            db.Parameters.Add("@keyword", $"%{keyword}%");
        }

        db.CommandText += " ORDER BY p.updated_at DESC, p.ProjectID DESC";

        DataTable dt = db.GetTable();
        List<ReviewChecklistItem> result = new List<ReviewChecklistItem>();

        foreach (DataRow row in dt.Rows)
        {
            var item = new ReviewChecklistItem
            {
                ProjectID = row["ProjectID"]?.ToString(),
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
                CurrentStep = row["CurrentStep"]?.ToString(),
                created_at = row["created_at"] != DBNull.Value ? (DateTime?)row["created_at"] : null,
                updated_at = row["updated_at"] != DBNull.Value ? (DateTime?)row["updated_at"] : null,
                
                // Additional fields from Application Main
                ProjectNameTw = row["ProjectNameTw"]?.ToString(),
                OrgName = row["OrgName"]?.ToString(),
                Year = row["Year"]?.ToString(),
                SubsidyPlanType = row["SubsidyPlanType"]?.ToString(),
                ApplicationAmount = row["ApplicationAmount"]?.ToString() ?? "0"
            };

            result.Add(item);
        }

        return result;
    }

    /// <summary>
    /// 取得科專進度資料
    /// </summary>
    private static List<ProgressData> GetSciProgressData(DbHelper db, List<string> projectIds, string reviewProgress, string replyProgress)
    {
        if (projectIds.Count == 0) return new List<ProgressData>();

        // 建立 IN 子句的參數
        var projectIdParams = projectIds.Select((id, index) => $"@projectId{index}").ToList();
        string inClause = "(" + string.Join(",", projectIdParams) + ")";

        db.CommandText = $@"
            SELECT 
                RR.ProjectID,
                COUNT(RR.ProjectID) AS TotalCount,
                COUNT(RR.ReviewComment) AS CommentCount,
                COUNT(RR.ReplyComment) AS ReplyCount,
                CASE 
                    WHEN COUNT(RR.ProjectID) > 0 AND COUNT(RR.ProjectID) = COUNT(RR.ReviewComment) THEN '完成'
                    ELSE '未完成'
                END AS ReviewProgress,
                CASE 
                    WHEN COUNT(RR.ReviewComment) > 0 AND COUNT(RR.ReviewComment) = COUNT(RR.ReplyComment) THEN '完成'
                    ELSE '未完成'
                END AS ReplyProgress
            FROM OFS_ReviewRecords RR
            WHERE RR.ProjectID IN {inClause}
            GROUP BY RR.ProjectID
        ";

        // 添加 ProjectID 參數
        db.Parameters.Clear();
        for (int i = 0; i < projectIds.Count; i++)
        {
            db.Parameters.Add($"@projectId{i}", projectIds[i]);
        }

        DataTable dt = db.GetTable();
        List<ProgressData> result = new List<ProgressData>();

        foreach (DataRow row in dt.Rows)
        {
            var totalCount = Convert.ToInt32(row["TotalCount"]);
            var commentCount = Convert.ToInt32(row["CommentCount"]);
            var replyCount = Convert.ToInt32(row["ReplyCount"]);
            var reviewProgressValue = row["ReviewProgress"]?.ToString();
            var replyProgressValue = row["ReplyProgress"]?.ToString();

            var item = new ProgressData
            {
                ProjectID = row["ProjectID"]?.ToString(),
                TotalCount = totalCount,
                CommentCount = commentCount,
                ReplyCount = replyCount,
                ReviewProgress = reviewProgressValue,
                ReplyProgress = replyProgressValue,
                ReviewProgressDisplay = reviewProgressValue == "完成" 
                    ? $"完成({commentCount}/{totalCount})" 
                    : $"未完成({commentCount}/{totalCount})",
                ReplyProgressDisplay = replyProgressValue == "完成" 
                    ? $"完成({replyCount}/{commentCount})" 
                    : $"未完成({replyCount}/{commentCount})"
            };

            result.Add(item);
        }

        return result;
    }

    /// <summary>
    /// 取得科專審查組別資料
    /// </summary>
    private static List<ReviewGroupData> GetSciReviewGroupData(DbHelper db, List<string> projectIds)
    {
        if (projectIds.Count == 0) return new List<ReviewGroupData>();

        // 建立 IN 子句的參數
        var projectIdParams = projectIds.Select((id, index) => $"@projectId{index}").ToList();
        string inClause = "(" + string.Join(",", projectIdParams) + ")";

        db.CommandText = $@"
            SELECT 
                m.ProjectID,
                zg.Descname AS Field_Descname
            FROM OFS_SCI_Application_Main m
            LEFT JOIN Sys_ZgsCode zg ON m.Field = zg.Code
            WHERE m.ProjectID IN {inClause}
        ";

        // 添加 ProjectID 參數
        db.Parameters.Clear();
        for (int i = 0; i < projectIds.Count; i++)
        {
            db.Parameters.Add($"@projectId{i}", projectIds[i]);
        }

        DataTable dt = db.GetTable();
        List<ReviewGroupData> result = new List<ReviewGroupData>();

        foreach (DataRow row in dt.Rows)
        {
            var item = new ReviewGroupData
            {
                ProjectID = row["ProjectID"]?.ToString(),
                Field_Descname = row["Field_Descname"]?.ToString()
            };

            result.Add(item);
        }

        return result;
    }

    #endregion
    
    #region type-3 Search 科專
    /// <summary>
    /// 查詢科專技術審查清單
    /// </summary>
    /// <param name="year">年度</param>
    /// <param name="orgName">申請單位</param>
    /// <param name="supervisor">承辦人員</param>
    /// <param name="keyword">關鍵字</param>
    /// <param name="reviewProgress">審查進度</param>
    /// <param name="replyProgress">回覆進度</param>
    /// <returns>科專審查清單</returns>
    
    public static List<ReviewChecklistItem> Search_SCI_Type3(
        string year = "",
        string orgName = "",
        string supervisor = "",
        string keyword = "",
        string reviewProgress = "",
        string replyProgress = "")
    {
        DbHelper db = new DbHelper();
        
        try
        {
            // 直接查詢科專技術審查資料
            return GetSciProjectData(db, year, orgName, supervisor, keyword, reviewProgress, replyProgress, "技術審查");
        }
        catch (Exception ex)
        {
            throw new Exception($"查詢審查清單時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    #endregion

   
}
