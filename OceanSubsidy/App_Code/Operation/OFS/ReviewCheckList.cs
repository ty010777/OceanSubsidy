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
            new DropdownItem { Value = "審核中", Text = "審核中" },
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

    /// <summary>
    /// 取得科專審查組別選項
    /// </summary>
    /// <returns>科專審查組別清單</returns>
    public static List<DropdownItem> GetSciReviewGroupOptions()
    {
        DbHelper db = new DbHelper();
        
        db.CommandText = @"
            SELECT [Code], [Descname]
            FROM [Sys_ZgsCode] 
            WHERE CodeGroup = 'SCIField' 
            AND IsValid = 1
            ORDER BY OrderNo, Code
        ";

        List<DropdownItem> result = new List<DropdownItem>();
        
        try
        {
            db.Parameters.Clear();
            DataTable dt = db.GetTable();
            
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    result.Add(new DropdownItem
                    {
                        Value = row["Code"].ToString(),
                        Text = row["Descname"].ToString()
                    });
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得科專審查組別選項時發生錯誤: {ex.Message}");
        }
        finally
        {
            db.Dispose();
        }

        return result;
    }

    #endregion

    #region type-4 Search 決審

    /// <summary>
    /// 查詢科專決審核定清單（支援分頁）
    /// </summary>
    /// <param name="year">年度</param>
    /// <param name="orgName">申請單位</param>
    /// <param name="supervisor">承辦人員</param>
    /// <param name="keyword">關鍵字</param>
    /// <param name="reviewGroupCode">審查組別代碼</param>
    /// <param name="pageNumber">頁碼</param>
    /// <param name="pageSize">每頁筆數</param>
    /// <param name="totalRecords">總記錄數（輸出參數）</param>
    /// <returns>分頁資料</returns>
    public static PaginatedResult<ReviewChecklistItem> Search_SCI_Type4_Paged(
        out int totalRecords
        , string year = "",
        string orgName = "",
        string supervisor = "",
        string keyword = "",
        string reviewGroupCode = "",
        int pageNumber = 1,
        int pageSize = 10)
    {
        var allData = Search_SCI_Type4(year, orgName, supervisor, keyword, reviewGroupCode);
        totalRecords = allData.Count;
        
        var pagedData = allData
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
            
        return new PaginatedResult<ReviewChecklistItem>
        {
            Data = pagedData,
            TotalRecords = totalRecords,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
        };
    }

    public static List<ReviewChecklistItem> Search_SCI_Type4(
        string year = "",
        string orgName = "",
        string supervisor = "",
        string keyword = "",
        string reviewGroupCode = "")
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
WITH SubsidySummary AS (
    SELECT 
        PM.ProjectID,
        AM.Year,
        AM.ProjectNameTw,
        AM.OrgName,
        AM.Field,
        PM.SupervisoryPersonAccount,
        PM.ApprovedSubsidy,
        PM.FinalReviewNotes,
        PM.StatusesName,
        PM.FinalReviewOrder,
        SUM(PT.SubsidyAmount) AS TotalSubsidyPrice
        
    FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main] PM
    LEFT JOIN OFS_SCI_Application_Main AM ON PM.ProjectID = AM.ProjectID
    LEFT JOIN OFS_SCI_PersonnelCost_TotalFee PT ON PM.ProjectID = PT.ProjectID
    WHERE Statuses = '決審核定'
    GROUP BY PM.ProjectID, AM.Year, AM.ProjectNameTw, AM.OrgName, AM.Field, PM.SupervisoryPersonAccount, PM.ApprovedSubsidy, PM.FinalReviewNotes, PM.StatusesName, PM.FinalReviewOrder
),
ScoreSummary AS (
    SELECT 
        PM.ProjectID,
        SUM(ORR.TotalScore) AS TotalScore
    FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main] PM
    LEFT JOIN OFS_ReviewRecords ORR ON PM.ProjectID = ORR.ProjectID
    WHERE Statuses = '決審核定'
    GROUP BY PM.ProjectID
)
SELECT 
    S.ProjectID,
    S.Year,
    S.ProjectNameTw,
    S.OrgName,
    S.Field,
    S.SupervisoryPersonAccount,
    S.TotalSubsidyPrice,
    S.StatusesName,
    S.ApprovedSubsidy,
    S.FinalReviewNotes,
    S.FinalReviewOrder,
    SC.TotalScore,
    '科專' AS Category
FROM SubsidySummary S
LEFT JOIN ScoreSummary SC ON S.ProjectID = SC.ProjectID
Where StatusesName != '不通過'
";

        try
        {
            List<ReviewChecklistItem> checklist = new List<ReviewChecklistItem>();
            db.Parameters.Clear();

            // 添加篩選條件參數
            
            if (!string.IsNullOrEmpty(year))
            {
                db.CommandText += " AND S.Year = @year";
                db.Parameters.Add("@year", year);
            }

            if (!string.IsNullOrEmpty(orgName))
            {
                
                db.CommandText += " AND S.OrgName LIKE @orgName";
                db.Parameters.Add("@orgName", $"%{orgName}%");
            }

            if (!string.IsNullOrEmpty(supervisor))
            {
                
                db.CommandText += " AND S.SupervisoryPersonAccount = @supervisor";
                db.Parameters.Add("@supervisor", supervisor);
            }

            if (!string.IsNullOrEmpty(reviewGroupCode))
            {
                db.CommandText += " AND S.Field = @reviewGroupCode";
                db.Parameters.Add("@reviewGroupCode", reviewGroupCode);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                db.CommandText += " AND (S.ProjectID LIKE @keyword OR S.ProjectNameTw LIKE @keyword)";
                db.Parameters.Add("@keyword", $"%{keyword}%");
            }

            db.CommandText += " ORDER BY S.FinalReviewOrder ASC, SC.TotalScore DESC";

            DataTable dt = db.GetTable();

            foreach (DataRow row in dt.Rows)
            {
                var item = new ReviewChecklistItem
                {
                    ProjectID = row["ProjectID"]?.ToString(),
                    ProjectNameTw = row["ProjectNameTw"]?.ToString(),
                    Year = row["Year"]?.ToString(),
                    Category = row["Category"]?.ToString(),
                    UserOrg = row["OrgName"]?.ToString(),
                    OrgName = row["OrgName"]?.ToString(),
                    StatusesName = row["StatusesName"]?.ToString(),
                    SupervisoryPersonAccount = row["SupervisoryPersonAccount"]?.ToString(),
                    TopicField = row["Field"]?.ToString(),
                    ApplicationAmount = row["TotalSubsidyPrice"]?.ToString() ?? "0",
                    ApprovedSubsidy = row["ApprovedSubsidy"]?.ToString() ?? "0",
                    FinalReviewNotes = row["FinalReviewNotes"]?.ToString(),
                    TotalScore = row["TotalScore"]?.ToString() ?? "0",
                    FinalReviewOrder = row["FinalReviewOrder"]?.ToString() ?? "0"
                };

                checklist.Add(item);
            }

            return checklist;
        }
        catch (Exception ex)
        {
            throw new Exception($"Search_SCI_Type4 查詢時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 排序模式專用查詢 - 取得決審核定階段的案件用於排序
    /// </summary>
    /// <param name="year">年度</param>
    /// <param name="category">計畫類別</param>
    /// <param name="reviewGroupCode">審查組別代碼</param>
    /// <returns>排序模式案件清單</returns>
    public static List<SortingModeItem> Search_SCI_ForSorting(
        string year = "",
        string category = "",
        string reviewGroupCode = "")
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
WITH SubsidySummary AS (
    SELECT 
        PM.ProjectID,
        AM.Year,
        AM.ProjectNameTw,
        AM.OrgName,
        AM.Field,
        PM.SupervisoryPersonName,
        PM.ApprovedSubsidy,
        PM.FinalReviewNotes,
        PM.FinalReviewOrder,
        SUM(PT.SubsidyAmount) AS TotalSubsidyPrice
        
    FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main] PM
    LEFT JOIN OFS_SCI_Application_Main AM ON PM.ProjectID = AM.ProjectID
    LEFT JOIN OFS_SCI_PersonnelCost_TotalFee PT ON PM.ProjectID = PT.ProjectID
    WHERE Statuses = '決審核定'
    GROUP BY PM.ProjectID, AM.Year, AM.ProjectNameTw, AM.OrgName, AM.Field, PM.SupervisoryPersonName, PM.ApprovedSubsidy, PM.FinalReviewNotes, PM.FinalReviewOrder
),
ScoreSummary AS (
    SELECT 
        PM.ProjectID,
        SUM(ORR.TotalScore) AS TotalScore
    FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main] PM
    LEFT JOIN OFS_ReviewRecords ORR ON PM.ProjectID = ORR.ProjectID
    WHERE Statuses = '決審核定'
    GROUP BY PM.ProjectID
),
FieldData AS (
    SELECT 
        PM.ProjectID,
        ZGS.DescName AS Field_Descname
    FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main] PM  
    LEFT JOIN OFS_SCI_Application_Main AM ON PM.ProjectID = AM.ProjectID
    LEFT JOIN Sys_ZgsCode ZGS ON AM.Field = ZGS.Code AND ZGS.CodeGroup = 'SCIField'
    WHERE Statuses = '決審核定'
)
SELECT 
    S.ProjectID,
    S.Year,
    S.ProjectNameTw,
    S.OrgName,
    S.SupervisoryPersonName,
    S.TotalSubsidyPrice AS ApplicationAmount,
    S.ApprovedSubsidy,
    S.FinalReviewNotes,
    S.FinalReviewOrder,
    SC.TotalScore,
    FD.Field_Descname
FROM SubsidySummary S
LEFT JOIN ScoreSummary SC ON S.ProjectID = SC.ProjectID
LEFT JOIN FieldData FD ON S.ProjectID = FD.ProjectID
WHERE 1=1
";

        // 動態加入篩選條件
        if (!string.IsNullOrEmpty(year))
        {
            db.CommandText += " AND S.Year = @year";
        }

        if (!string.IsNullOrEmpty(reviewGroupCode))
        {
            db.CommandText += " AND S.Field = @reviewGroupCode";
        }

        // 排序：優先依決審排序，再依總分降序
        db.CommandText += " ORDER BY S.FinalReviewOrder ASC, SC.TotalScore DESC";

        try
        {
            db.Parameters.Clear();
            
            if (!string.IsNullOrEmpty(year))
            {
                db.Parameters.Add("@year", year);
            }

            if (!string.IsNullOrEmpty(reviewGroupCode))
            {
                db.Parameters.Add("@reviewGroupCode", reviewGroupCode);
            }

            DataTable dt = db.GetTable();
            List<SortingModeItem> result = new List<SortingModeItem>();

            foreach (DataRow row in dt.Rows)
            {
                SortingModeItem item = new SortingModeItem
                {
                    ProjectID = row["ProjectID"].ToString(),
                    ProjectNameTw = row["ProjectNameTw"].ToString(),
                    OrgName = row["OrgName"].ToString(),
                    SupervisoryPersonName = row["SupervisoryPersonName"].ToString(),
                    ApplicationAmount = row["ApplicationAmount"] != DBNull.Value ? row["ApplicationAmount"].ToString() : "0",
                    ApprovedSubsidy = row["ApprovedSubsidy"] != DBNull.Value ? row["ApprovedSubsidy"].ToString() : "0",
                    FinalReviewNotes = row["FinalReviewNotes"] != DBNull.Value ? row["FinalReviewNotes"].ToString() : "",
                    FinalReviewOrder = row["FinalReviewOrder"] != DBNull.Value ? row["FinalReviewOrder"].ToString() : "999",
                    TotalScore = row["TotalScore"] != DBNull.Value ? row["TotalScore"].ToString() : "0",
                    Field_Descname = row["Field_Descname"] != DBNull.Value ? row["Field_Descname"].ToString() : "",
                    Category = GetProjectCategoryFromId(row["ProjectID"].ToString())
                };

                result.Add(item);
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"Search_SCI_ForSorting 查詢時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 更新排序順序 - 純資料庫操作
    /// </summary>
    /// <param name="sortingItems">要更新的排序項目清單</param>
    public static void UpdateSortingOrder(List<SortingSaveItem> sortingItems)
    {
        DbHelper db = new DbHelper();

        try
        {

            foreach (var item in sortingItems)
            {
                string tableName = GetTableNameByCategory(item.Category);
                
                db.CommandText = $@"
                    UPDATE {tableName} 
                    SET FinalReviewOrder = @finalReviewOrder,
                        FinalReviewNotes = @finalReviewNotes,
                        updated_at = GETDATE()
                    WHERE ProjectID = @projectId
                ";

                db.Parameters.Clear();
                db.Parameters.Add("@finalReviewOrder", item.FinalReviewOrder);
                db.Parameters.Add("@finalReviewNotes", item.FinalReviewNotes ?? "");
                db.Parameters.Add("@projectId", item.ProjectID);

                db.ExecuteNonQuery();
            }

        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 更新核定資料 - 純資料庫操作
    /// </summary>
    /// <param name="tableName">資料表名稱</param>
    /// <param name="parameters">更新參數</param>
    public static void UpdateApprovalData(string tableName, Dictionary<string, object> parameters)
    {
        DbHelper db = new DbHelper();

        try
        {
            // 建立更新 SQL - 只更新核定經費和備註，不更新 FinalReviewOrder
            db.CommandText = $@"
                UPDATE {tableName} 
                SET ApprovedSubsidy = @approvedSubsidy,
                    FinalReviewNotes = @finalReviewNotes,
                    updated_at = GETDATE()
                WHERE ProjectID = @projectId
            ";

            db.Parameters.Clear();
            foreach (var param in parameters)
            {
                db.Parameters.Add(param.Key, param.Value);
            }

            db.ExecuteNonQuery();
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 根據專案ID取得計畫類別
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <returns>計畫類別</returns>
    private static string GetProjectCategoryFromId(string projectId)
    {
        if (string.IsNullOrEmpty(projectId)) return "其他";
        
        if (projectId.Contains("SCI"))
            return "科專";
        else if (projectId.Contains("CUL"))
            return "文化";
        else if (projectId.Contains("EDC"))
            return "學校民間";
        else if (projectId.Contains("CLB"))
            return "學校社團";
        else
            return "其他";
    }

    /// <summary>
    /// 根據計畫類別取得對應的資料表名稱
    /// </summary>
    /// <param name="category">計畫類別</param>
    /// <returns>資料表名稱</returns>
    private static string GetTableNameByCategory(string category)
    {
        switch (category)
        {
            case "科專":
                return "OFS_SCI_Project_Main";
            case "文化":
                return "OFS_CUL_Project_Main";
            case "學校民間":
                return "OFS_EDC_Project_Main";
            case "學校社團":
                return "OFS_CLB_Project_Main";
            default:
                return "OFS_SCI_Project_Main";
        }
    }

    #endregion

    #region type-1 Search 科專

    /// <summary>
    /// 查詢科專資格審查清單（支援分頁）
    /// </summary>
    /// <param name="year">年度</param>
    /// <param name="status">狀態</param>
    /// <param name="orgName">申請單位</param>
    /// <param name="supervisor">承辦人員</param>
    /// <param name="keyword">關鍵字</param>
    /// <param name="pageNumber">頁碼</param>
    /// <param name="pageSize">每頁筆數</param>
    /// <param name="totalRecords">總記錄數（輸出參數）</param>
    /// <returns>分頁資料</returns>
    public static PaginatedResult<ReviewChecklistItem> Search_SCI_Type1_Paged(out int totalRecords,
        string year = "",
        string status = "",
        string orgName = "",
        string supervisor = "",
        string keyword = "",
        int pageNumber = 1,
        int pageSize = 10
        )
    {
        var allData = Search_SCI_Type1(year, status, orgName, supervisor, keyword);
        totalRecords = allData.Count;
        
        var pagedData = allData
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
            
        return new PaginatedResult<ReviewChecklistItem>
        {
            Data = pagedData,
            TotalRecords = totalRecords,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
        };
    }

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
        SUM(SubsidyAmount) AS SubsidyAmount,
        YEAR(PM.created_at) - 1911 AS [Year]  -- 民國年欄位整合進來
    FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main] PM
    LEFT JOIN OFS_SCI_Application_Main AM
        ON AM.ProjectID = PM.ProjectID 
    LEFT JOIN OFS_SCI_PersonnelCost_TotalFee PT
        ON PM.ProjectID = PT.ProjectID 
    WHERE PM.isExist = 1 
      AND Statuses = '資格審查' 
      AND PM.StatusesName != '不通過' 
      AND isWithdrawal != 1
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
    '科專' AS Category
FROM ProjectSubsidySummary
";
        try
        {
            List<ReviewChecklistItem> checklist = new List<ReviewChecklistItem>();
            db.Parameters.Clear();

            // 添加篩選條件參數
            bool hasWhereClause = false;
            
            if (!string.IsNullOrEmpty(year))
            {
                db.CommandText += " WHERE Year = @year";
                db.Parameters.Add("@year", year);
                hasWhereClause = true;
            }

            if (!string.IsNullOrEmpty(status))
            {
                if (hasWhereClause)
                    db.CommandText += " AND StatusesName LIKE @status";
                else
                {
                    db.CommandText += " WHERE StatusesName LIKE @status";
                    hasWhereClause = true;
                }
                db.Parameters.Add("@status", $"%{status}%");
            }

            if (!string.IsNullOrEmpty(orgName))
            {
                if (hasWhereClause)
                    db.CommandText += " AND UserOrg LIKE @orgName";
                else
                {
                    db.CommandText += " WHERE UserOrg LIKE @orgName";
                    hasWhereClause = true;
                }
                db.Parameters.Add("@orgName", $"%{orgName}%");
            }

            if (!string.IsNullOrEmpty(supervisor))
            {
                if (hasWhereClause)
                    db.CommandText += " AND SupervisoryPersonAccount = @supervisor";
                else
                {
                    db.CommandText += " WHERE SupervisoryPersonAccount = @supervisor";
                    hasWhereClause = true;
                }
                db.Parameters.Add("@supervisor", supervisor);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                if (hasWhereClause)
                    db.CommandText += " AND (ProjectID LIKE @keyword OR ProjectNameTw LIKE @keyword)";
                else
                {
                    db.CommandText += " WHERE (ProjectID LIKE @keyword OR ProjectNameTw LIKE @keyword)";
                    hasWhereClause = true;
                }
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
    /// 查詢科專領域審查清單（支援分頁）
    /// </summary>
    /// <param name="year">年度</param>
    /// <param name="orgName">申請單位</param>
    /// <param name="supervisor">承辦人員</param>
    /// <param name="keyword">關鍵字</param>
    /// <param name="reviewProgress">審查進度</param>
    /// <param name="replyProgress">回覆進度</param>
    /// <param name="pageNumber">頁碼</param>
    /// <param name="pageSize">每頁筆數</param>
    /// <param name="totalRecords">總記錄數（輸出參數）</param>
    /// <returns>分頁資料</returns>
    public static PaginatedResult<ReviewChecklistItem> Search_SCI_Type2_Paged(out int totalRecords,
        string year = "",
        string orgName = "",
        string supervisor = "",
        string keyword = "",
        string reviewProgress = "",
        string replyProgress = "",
        int pageNumber = 1,
        int pageSize = 10
        )
    {
        var allData = Search_SCI_Type2(year, orgName, supervisor, keyword, reviewProgress, replyProgress);
        totalRecords = allData.Count;
        
        var pagedData = allData
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
            
        return new PaginatedResult<ReviewChecklistItem>
        {
            Data = pagedData,
            TotalRecords = totalRecords,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
        };
    }

    /// <summary>
    /// 查詢科專領域審查清單
    /// </summary>
    /// <param name="year">年度</param>
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
        var progressData = GetSciProgressData(db, projectIds, status);

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
              AND (p.isExist = 1 OR p.isExist IS NULL) AND p.StatusesName != '不通過' AND isWithdrawal != 1
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
    private static List<ProgressData> GetSciProgressData(DbHelper db, List<string> projectIds, string status)
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
            WHERE RR.ProjectID IN {inClause} and RR.ReviewStage = @status
            GROUP BY RR.ProjectID
        ";

        // 添加 ProjectID 參數
        db.Parameters.Clear();
        for (int i = 0; i < projectIds.Count; i++)
        {
            db.Parameters.Add($"@projectId{i}", projectIds[i]);
        }
        
        // 添加 status 參數
        db.Parameters.Add("@status", status);

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
    /// 查詢科專技術審查清單（支援分頁）
    /// </summary>
    /// <param name="year">年度</param>
    /// <param name="orgName">申請單位</param>
    /// <param name="supervisor">承辦人員</param>
    /// <param name="keyword">關鍵字</param>
    /// <param name="reviewProgress">審查進度</param>
    /// <param name="replyProgress">回覆進度</param>
    /// <param name="pageNumber">頁碼</param>
    /// <param name="pageSize">每頁筆數</param>
    /// <param name="totalRecords">總記錄數（輸出參數）</param>
    /// <returns>分頁資料</returns>
    public static PaginatedResult<ReviewChecklistItem> Search_SCI_Type3_Paged(out int totalRecords,
        string year = "",
        string orgName = "",
        string supervisor = "",
        string keyword = "",
        string reviewProgress = "",
        string replyProgress = "",
        int pageNumber = 1,
        int pageSize = 10
        )
    {
        var allData = Search_SCI_Type3(year, orgName, supervisor, keyword, reviewProgress, replyProgress);
        totalRecords = allData.Count;
        
        var pagedData = allData
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
            
        return new PaginatedResult<ReviewChecklistItem>
        {
            Data = pagedData,
            TotalRecords = totalRecords,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
        };
    }

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
    /// 更新專案狀態到資料庫
    /// </summary>
    /// <param name="db">資料庫連線</param>
    /// <param name="projectId">專案編號</param>
    /// <param name="newStatus">新狀態</param>
    /// <param name="userAccount">操作者帳號</param>
    /// <returns>是否更新成功</returns>
    public static void UpdateProjectStatusInDatabase(string projectId, string newStatus,
        string userAccount, string StatusesName)
    {
        string finalStatusesName = string.IsNullOrEmpty(StatusesName) ? "審核中" : StatusesName;

        using (DbHelper db = new DbHelper())
        {
            db.CommandText = $@"
                UPDATE OFS_SCI_Project_Main 
                SET Statuses = '{newStatus}',
                    StatusesName = N'{finalStatusesName}',
                    updated_at = GETDATE()
                WHERE ProjectID = '{projectId}'";

            db.ExecuteNonQuery();
        }
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
    public static void InsertReviewHistory(string projectId, string fromStatus, string toStatus,
        string actionType, string userName)
    {
        using (DbHelper db = new DbHelper())
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
                    '{userName}', 
                    '{fromStatus}通過', 
                    '{toStatus}審核中', 
                    '批次{actionType}: {fromStatus}通過 → {toStatus}審核中'
                )";

            db.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// 更新專案狀態名稱（只更新StatusesName）
    /// </summary>
    /// <param name="projectId">專案編號</param>
    /// <param name="statusName">狀態名稱</param>
    /// <param name="userAccount">操作者帳號</param>
    public static void UpdateProjectStatusName(string projectId, string statusName, string userAccount)
    {
        using (DbHelper db = new DbHelper())
        {
            db.CommandText = $@"
                UPDATE OFS_SCI_Project_Main 
                SET StatusesName = N'{statusName}',
                    updated_at = GETDATE()
                WHERE ProjectID = '{projectId}'";

            db.ExecuteNonQuery();
        }
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
                        UpdateProjectRejectStatus(db, projectId);
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
    private static void UpdateProjectRejectStatus(DbHelper db, string projectId)
    {
        db.CommandText = $@"
            UPDATE OFS_SCI_Project_Main 
            SET StatusesName = '不通過',
                updated_at = GETDATE()
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
                '審核中', 
                '不通過', 
                '批次{actionType}: 審核中 → 不通過'
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

    #region 計畫詳情查詢

    /// <summary>
    /// 取得科專計畫詳細資料
    /// </summary>
    /// <param name="projectId">專案編號</param>
    /// <returns>計畫詳細資料</returns>
    public static DataTable GetSciPlanDetail(string projectId)
    {
        using (var db = new DbHelper())
        {
            string sql = @"
                SELECT TOP 1 
                    [ProjectID],
                    [Year],
                    [SubsidyPlanType],
                    [ProjectNameTw],
                    (
                        SELECT Descname 
                        FROM Sys_ZgsCode 
                        WHERE Code = AM.[Topic]
                    ) + ' 、 ' + 
                    (
                        SELECT Descname 
                        FROM Sys_ZgsCode 
                        WHERE Code = AM.[Field]
                    ) AS [TopicField],
                    [OrgName]
                FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Application_Main] AM
                WHERE [ProjectID] = '{0}'
            ";

            db.CommandText = string.Format(sql, projectId);
            return db.GetTable();
        }
    }

    /// <summary>
    /// 取得文化計畫詳細資料
    /// </summary>
    /// <param name="projectId">專案編號</param>
    /// <returns>計畫詳細資料</returns>
    public static DataTable GetCulturalPlanDetail(string projectId)
    {
        // TODO: 文化補助案查詢 - 之後實作
        return new DataTable();
    }

    /// <summary>
    /// 取得科專計畫評審意見
    /// </summary>
    /// <param name="projectId">專案編號</param>
    /// <param name="reviewStage">審查階段</param>
    /// <returns>評審意見資料</returns>
    public static DataTable GetSciReviewComments(string projectId, string reviewStage)
    {
        using (var db = new DbHelper())
        {
            string sql = @"
                SELECT TOP (1000) 
                    [ReviewID],
                    [ProjectID],
                    [ReviewStage],
                    [Email],
                    [ReviewerName],
                    [ReviewComment],
                    [ReplyComment],
                    [TotalScore],
                    [Token],
                    [CreateTime],
                    [IsSubmit]
                FROM [OCA_OceanSubsidy].[dbo].[OFS_ReviewRecords]
                WHERE ProjectID = '{0}' AND ReviewStage = '{1}' AND IsSubmit = 1
                ORDER BY ProjectID
            ";

            db.CommandText = string.Format(sql, projectId, reviewStage);
            return db.GetTable();
        }
    }

    /// <summary>
    /// 取得文化計畫評審意見
    /// </summary>
    /// <param name="projectId">專案編號</param>
    /// <param name="reviewStage">審查階段</param>
    /// <returns>評審意見資料</returns>
    public static DataTable GetCulturalReviewComments(string projectId, string reviewStage)
    {
        // TODO: 文化補助案評審意見查詢 - 之後實作
        return new DataTable();
    }

    #endregion
    
    /// <summary>
    /// 取得待回覆的案件清單
    /// </summary>
    public static List<string> GetWaitReplyList()
    {
        List<string> projectIDs = new List<string>();

        DbHelper db = new DbHelper();
        
        try
        {
            db.CommandText = @"
                SELECT DISTINCT ProjectID
                FROM [OCA_OceanSubsidy].[dbo].[OFS_ReviewRecords]
                WHERE IsSubmit = 1
                AND (ReplyComment IS NULL OR ReplyComment = '')";
            
            DataTable dt = db.GetTable();
            
            foreach (DataRow row in dt.Rows)
            {
                string ProjectID = row["ProjectID"]?.ToString();
                if (!string.IsNullOrEmpty(ProjectID))
                {
                    projectIDs.Add(ProjectID);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"搜尋關鍵字時發生錯誤：{ex.Message}");
        }
        finally
        {
            db.Dispose();
        }
        
        return projectIDs;
    }
}