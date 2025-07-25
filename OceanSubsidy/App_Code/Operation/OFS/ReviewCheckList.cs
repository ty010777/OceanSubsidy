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
            // 直接查詢科專領域審查資料
            return GetSciProjectData(db, year, orgName, supervisor, keyword, reviewProgress, replyProgress);
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
    private static List<ProgressData> GetSciProgressData(DbHelper db, List<string> projectIds, string reviewProgress,
        string replyProgress)
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
                    ? $"完成({replyCount}/{totalCount})"
                    : $"未完成({replyCount}/{totalCount})"
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

    #region 批次處理方法

    /// <summary>
    /// 批次更新專案狀態
    /// </summary>
    /// <param name="projectIds">專案編號列表</param>
    /// <param name="fromStatus">原始狀態</param>
    /// <param name="toStatus">目標狀態</param>
    /// <param name="userAccount">操作者帳號</param>
    /// <param name="actionType">操作類型</param>
    /// <returns>批次處理結果</returns>
    public static BatchApprovalResult BatchUpdateProjectStatus(
        List<string> projectIds,
        string fromStatus,
        string toStatus,
        string userAccount,
        string actionType)
    {
        var result = new BatchApprovalResult
        {
            ActionType = actionType,
            ProcessedAt = DateTime.Now
        };

        if (projectIds == null || projectIds.Count == 0)
        {
            result.Success = false;
            result.Message = "專案編號列表為空";
            return result;
        }

        DbHelper db = new DbHelper();

        try
        {
            int successCount = 0;
            var successIds = new List<string>();
            var errorMessages = new List<string>();

            foreach (string projectId in projectIds)
            {
                try
                {
                    // 更新專案狀態
                    UpdateProjectStatusInDatabase(db, projectId, toStatus, userAccount);
                    // 記錄審查歷程
                    InsertReviewHistory(db, projectId, fromStatus, toStatus, actionType, userAccount);
                    successCount++;
                    successIds.Add(projectId);
                }
                catch (Exception ex)
                {
                    errorMessages.Add($"處理專案 {projectId} 時發生錯誤: {ex.Message}");
                }
            }

            // 如果有成功的項目，提交事務
            if (successCount > 0)
            {
                result.Success = true;
                result.SuccessCount = successCount;
                result.SuccessProjectIds = successIds;
                result.Message = $"成功處理 {successCount} 件計畫";

                if (errorMessages.Count > 0)
                {
                    result.ErrorMessages = errorMessages;
                }
            }
            else
            {
                result.Success = false;
                result.Message = "所有專案處理失敗";
                result.ErrorMessages = errorMessages;
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = "批次處理時發生系統錯誤";
            result.ErrorMessages = new List<string> { ex.Message };
        }
        finally
        {
            db.Dispose();
        }

        return result;
    }

    /// <summary>
    /// 更新專案狀態到資料庫
    /// </summary>
    /// <param name="db">資料庫連線</param>
    /// <param name="projectId">專案編號</param>
    /// <param name="newStatus">新狀態</param>
    /// <param name="userAccount">操作者帳號</param>
    /// <returns>是否更新成功</returns>
    private static void UpdateProjectStatusInDatabase(DbHelper db, string projectId, string newStatus,
        string userAccount)
    {
        db.CommandText = $@"
            UPDATE OFS_SCI_Project_Main 
            SET Statuses = '{newStatus}',
                StatusesName = '審查中',
                updated_at = GETDATE()
            WHERE ProjectID = '{projectId}'";

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 插入審查歷程記錄
    /// </summary>
    /// <param name="db">資料庫連線</param>
    /// <param name="projectId">專案編號</param>
    /// <param name="fromStatus">原始狀態</param>
    /// <param name="toStatus">目標狀態</param>
    /// <param name="actionType">操作類型</param>
    /// <param name="userAccount">操作者帳號</param>
    private static void InsertReviewHistory(DbHelper db, string projectId, string fromStatus, string toStatus,
        string actionType, string userAccount)
    {
        db.CommandText = $@"
            INSERT INTO OFS_CaseHistoryLog (
                ProjectID, 
                ChangeTime, 
                UserName, 
                StageStatusBefore, 
                StageStatusAfter, 
                Description
            ) VALUES (
                '{projectId}', 
                GETDATE(), 
                '{userAccount}', 
                '{fromStatus}通過', 
                '{toStatus}審核中', 
                '批次{actionType}: {fromStatus}通過 → {toStatus}審核中'
            )";

        db.ExecuteNonQuery();
    }

    #endregion

    #region 批次不通過處理

    /// <summary>
    /// 批次設定專案狀態為不通過
    /// </summary>
    /// <param name="projectIds">專案編號列表</param>
    /// <param name="userAccount">操作者帳號</param>
    /// <param name="actionType">操作類型</param>
    /// <returns>批次處理結果</returns>
    public static BatchApprovalResult BatchRejectProjectStatus(
        List<string> projectIds,
        string userAccount,
        string actionType)
    {
        var result = new BatchApprovalResult
        {
            ActionType = actionType,
            ProcessedAt = DateTime.Now
        };
        

        using (DbHelper db = new DbHelper())
        {
            try
            {
                int successCount = 0;
                var successIds = new List<string>();
                var errorMessages = new List<string>();

                foreach (string projectId in projectIds)
                {
                    try
                    {
                        // 更新專案狀態為不通過
                        UpdateProjectRejectStatus(db, projectId, userAccount);
                        // 記錄審查歷程
                        InsertRejectHistory(db, projectId, actionType, userAccount);
                        successCount++;
                        successIds.Add(projectId);
                        System.Diagnostics.Debug.WriteLine($"成功設定專案 {projectId} 為不通過");
                    }
                    catch (Exception ex)
                    {
                        errorMessages.Add($"處理專案 {projectId} 時發生錯誤: {ex.Message}");
                    }
                }

                // 設定結果
                if (successCount > 0)
                {
                    result.Success = true;
                    result.SuccessCount = successCount;
                    result.SuccessProjectIds = successIds;
                    result.Message = $"成功處理 {successCount} 件計畫";
                    
                    if (errorMessages.Count > 0)
                    {
                        result.ErrorMessages = errorMessages;
                    }
                }
                else
                {
                    result.Success = false;
                    result.Message = "所有專案處理失敗";
                    result.ErrorMessages = errorMessages;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "批次處理時發生系統錯誤";
                result.ErrorMessages = new List<string> { ex.Message };
            }
        }

        return result;
    }

    /// <summary>
    /// 更新專案狀態為不通過
    /// </summary>
    /// <param name="db">資料庫連線</param>
    /// <param name="projectId">專案編號</param>
    /// <param name="userAccount">操作者帳號</param>
    private static void UpdateProjectRejectStatus(DbHelper db, string projectId, string userAccount)
    {
        db.CommandText = $@"
            UPDATE OFS_SCI_Project_Main 
            SET StatusesName = '不通過',
                updated_at = GETDATE(),
                updated_by = '{userAccount}'
            WHERE ProjectID = '{projectId}'";

        db.ExecuteNonQuery();
    }

    /// <summary>
    /// 插入不通過審查歷程記錄
    /// </summary>
    /// <param name="db">資料庫連線</param>
    /// <param name="projectId">專案編號</param>
    /// <param name="actionType">操作類型</param>
    /// <param name="userAccount">操作者帳號</param>
    private static void InsertRejectHistory(DbHelper db, string projectId, string actionType, string userAccount)
    {
        db.CommandText = $@"
            INSERT INTO OFS_CaseHistoryLog (
                ProjectID, 
                ChangeTime, 
                UserName, 
                StageStatusBefore, 
                StageStatusAfter, 
                Description
            ) VALUES (
                '{projectId}', 
                GETDATE(), 
                '{userAccount}', 
                '審查中', 
                '不通過', 
                '批次{actionType}: 審查中 → 不通過'
            )";

        db.ExecuteNonQuery();
    }

    #endregion

    #region 科專批次審核後處理

    /// <summary>
    /// 處理科專批次審核後的特殊流程
    /// </summary>
    /// <param name="projectIds">專案編號列表</param>
    /// <param name="reviewStage">審查階段</param>
    /// <param name="actionType">操作類型</param>
    /// <param name="userAccount">操作者帳號</param>
    public static void ProcessSciPostApproval(List<string> projectIds, string reviewStage, string actionType, string userAccount)
    {
        DbHelper db = new DbHelper();

        foreach (string projectId in projectIds)
        {
            // 1. 從 OFS_SCI_Application_Main 取得 Field
            db.CommandText = $"SELECT Field FROM OFS_SCI_Application_Main WHERE ProjectID = '{projectId}'";
            string field = db.GetTable().Rows[0]["Field"]?.ToString();

            if (!string.IsNullOrEmpty(field))
            {
                // 2. 從 OFS_ReviewCommitteeList 取得對應審查組別的所有人員
                db.CommandText = $@"
                        SELECT CommitteeUser, Email 
                        FROM OFS_ReviewCommitteeList 
                        WHERE SubjectTypeID = '{field}'";

                DataTable reviewers = db.GetTable();

                // 3. 取得科專的審查範本
                db.CommandText = $@"
                    SELECT TemplateName, TemplateWeight 
                    FROM OFS_ReviewTemplate 
                    WHERE SubsidyProjects = 'SCI'";
                
                DataTable templates = db.GetTable();

                // 4. 為每位審查委員建立完整的審核記錄
                foreach (DataRow reviewer in reviewers.Rows)
                {
                    string reviewerEmail = reviewer["Email"]?.ToString();
                    string reviewerName = reviewer["CommitteeUser"]?.ToString();
                    
                    if (!string.IsNullOrEmpty(reviewerEmail) && !string.IsNullOrEmpty(reviewerName))
                    {
                        // 產生隨機 Token
                        string token = Guid.NewGuid().ToString();
                        
                        // 4.1 新增記錄到 OFS_ReviewRecords
                        db.CommandText = $@"
                            INSERT INTO OFS_ReviewRecords (
                                ProjectID, 
                                ReviewStage, 
                                Email, 
                                ReviewerName,
                                Token
                            ) VALUES (
                                '{projectId}', 
                                '{reviewStage}', 
                                '{reviewerEmail}', 
                                '{reviewerName}',
                                '{token}'
                            );
                            SELECT SCOPE_IDENTITY() AS ReviewID;";
                        
                        int reviewId = Convert.ToInt32(db.GetTable().Rows[0]["ReviewID"]);
                        
                        // 4.2 為此審查委員建立所有評審項目記錄
                        foreach (DataRow template in templates.Rows)
                        {
                            string templateName = template["TemplateName"]?.ToString();
                            decimal templateWeight = template["TemplateWeight"] != DBNull.Value 
                                ? Convert.ToDecimal(template["TemplateWeight"]) 
                                : 0;
                            
                            if (!string.IsNullOrEmpty(templateName))
                            {
                                db.CommandText = $@"
                                    INSERT INTO OFS_ReviewScores (
                                        ReviewID,
                                        ItemName,
                                        Weight,
                                        Score
                                    ) VALUES (
                                        {reviewId},
                                        '{templateName}',
                                        {templateWeight},
                                        NULL
                                    )";
                                
                                db.ExecuteNonQuery();
                            }
                        }
                        
                        System.Diagnostics.Debug.WriteLine($"已為專案 {projectId} 審查委員 {reviewerName} 建立完整審核記錄，包含 {templates.Rows.Count} 個評審項目");
                    }
                }
                
                System.Diagnostics.Debug.WriteLine($"專案 {projectId} 領域 {field} 找到 {reviewers.Rows.Count} 位審查委員，已完成記錄新增");
            }
        }
    }

    #endregion
}