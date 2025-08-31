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

    #region 資料轉換方法

    /// <summary>
    /// 將英文類別代碼轉換為中文類別名稱
    /// </summary>
    /// <param name="categoryCode">英文類別代碼</param>
    /// <returns>中文類別名稱</returns>
    public static string ConvertCategoryCodeToName(string categoryCode)
    {
        if (string.IsNullOrEmpty(categoryCode)) return "";

        switch (categoryCode.ToUpper())
        {
            case "SCI":
                return "科專";
            case "CUL":
                return "文化";
            case "EDC":
                return "學校民間";
            case "CLB":
                return "學校社團";
            case "MUL":
                return "多元";
            case "LIT":
                return "素養";
            case "ACC":
                return "無障礙";
            default:
                return categoryCode; // 如果找不到對應，返回原始值
        }
    }

    /// <summary>
    /// 將中文類別名稱轉換為英文類別代碼
    /// </summary>
    /// <param name="categoryName">中文類別名稱</param>
    /// <returns>英文類別代碼</returns>
    public static string ConvertCategoryNameToCode(string categoryName)
    {
        if (string.IsNullOrEmpty(categoryName)) return "";

        switch (categoryName)
        {
            case "科專":
                return "SCI";
            case "文化":
                return "CUL";
            case "學校民間":
                return "EDC";
            case "學校社團":
                return "CLB";
            case "多元":
                return "MUL";
            case "素養":
                return "LIT";
            case "無障礙":
                return "ACC";
            default:
                return categoryName; // 如果找不到對應，返回原始值
        }
    }

    #endregion

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
    /// 取得指定資格審查的申請單位選項
    /// </summary>
    /// <returns>申請單位清單</returns>
    public static List<DropdownItem> GetType1OrgOptions()
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT　distinct OrgName
              FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type1]
              WHERE OrgName != '' AND OrgName IS NOT NULL
         ";


        try
        {
            db.Parameters.Clear();

            DataTable dt = db.GetTable();
            List<DropdownItem> result = new List<DropdownItem>();

            // 加入「全部申請單位」選項
            result.Add(new DropdownItem { Value = "", Text = "全部申請單位" });

            foreach (DataRow row in dt.Rows)
            {
                if (row["OrgName"] != DBNull.Value)
                {
                    string org = row["OrgName"].ToString();
                    result.Add(new DropdownItem { Value = org, Text = org });
                }
            }
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
    /// 取得指定資格審查的申請單位選項
    /// </summary>
    /// <returns>申請單位清單</returns>
    public static List<DropdownItem> GetType4OrgOptions()
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT　distinct OrgName
              FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type4]
              WHERE OrgName != '' AND OrgName IS NOT NULL
         ";


        try
        {
            db.Parameters.Clear();

            DataTable dt = db.GetTable();
            List<DropdownItem> result = new List<DropdownItem>();

            // 加入「全部申請單位」選項
            result.Add(new DropdownItem { Value = "", Text = "全部申請單位" });

            foreach (DataRow row in dt.Rows)
            {
                if (row["OrgName"] != DBNull.Value)
                {
                    string org = row["OrgName"].ToString();
                    result.Add(new DropdownItem { Value = org, Text = org });
                }
            }
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
    /// 取得指定審查階段的申請單位選項
    /// </summary>
    /// <param name="status">審查階段狀態</param>
    /// <returns>申請單位清單</returns>
    public static List<DropdownItem> GetReviewOrgOptions(string status = "")
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
         SELECT DISTINCT OrgName
            FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main] PM
	        LEFT JOIN OFS_SCI_Application_Main AM
	        on AM.ProjectID = PM.ProjectID
            WHERE OrgName != '' AND OrgName IS NOT NULL
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
                if (row["OrgName"] != DBNull.Value)
                {
                    string org = row["OrgName"].ToString();
                    result.Add(new DropdownItem { Value = org, Text = org });
                }
            }
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
    /// 取得 Type6 執行計畫審核的申請單位選項
    /// </summary>
    /// <returns>申請單位清單</returns>
    public static List<DropdownItem> GetType6OrgOptions()
    {
        DbHelper db = new DbHelper();
        List<DropdownItem> result = new List<DropdownItem>();

        try
        {
            db.CommandText = @"
                SELECT distinct [OrgName]
                FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type6]
                WHERE [OrgName] IS NOT NULL AND [OrgName] != ''
                ORDER BY [OrgName]
            ";

            DataTable dt = db.GetTable();

            // 加入「全部申請單位」選項
            result.Add(new DropdownItem { Value = "", Text = "全部申請單位" });

            foreach (DataRow row in dt.Rows)
            {
                if (row["OrgName"] != DBNull.Value)
                {
                    string orgName = row["OrgName"].ToString().Trim();
                    if (!string.IsNullOrEmpty(orgName))
                    {
                        result.Add(new DropdownItem { Value = orgName, Text = orgName });
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Type6 申請單位選項載入完成，共 {result.Count - 1} 個單位");
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得 Type6 申請單位選項時發生錯誤：{ex.Message}");
            throw new Exception($"取得 Type6 申請單位選項時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 取得 Type6 執行計畫審核的主管單位選項
    /// </summary>
    /// <returns>主管單位清單</returns>
    public static List<DropdownItem> GetType6SupervisoryUnitOptions()
    {
        DbHelper db = new DbHelper();
        List<DropdownItem> result = new List<DropdownItem>();

        try
        {
            db.CommandText = @"
                SELECT distinct [SupervisoryUnit]
                FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type6]
                WHERE [SupervisoryUnit] IS NOT NULL AND [SupervisoryUnit] != ''
                ORDER BY [SupervisoryUnit]
            ";

            DataTable dt = db.GetTable();

            // 加入「全部主管單位」選項
            result.Add(new DropdownItem { Value = "", Text = "全部主管單位" });

            foreach (DataRow row in dt.Rows)
            {
                if (row["SupervisoryUnit"] != DBNull.Value)
                {
                    string supervisoryUnit = row["SupervisoryUnit"].ToString().Trim();
                    if (!string.IsNullOrEmpty(supervisoryUnit))
                    {
                        result.Add(new DropdownItem { Value = supervisoryUnit, Text = supervisoryUnit });
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Type6 主管單位選項載入完成，共 {result.Count - 1} 個單位");
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得 Type6 主管單位選項時發生錯誤：{ex.Message}");
            throw new Exception($"取得 Type6 主管單位選項時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 取得 Type5 計畫變更審核的申請單位選項
    /// </summary>
    /// <returns>申請單位清單</returns>
    public static List<DropdownItem> GetType5OrgOptions()
    {
        DbHelper db = new DbHelper();
        List<DropdownItem> result = new List<DropdownItem>();

        try
        {
            db.CommandText = @"
                SELECT distinct [OrgName]
                FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type5]
                WHERE [OrgName] IS NOT NULL AND [OrgName] != ''
                ORDER BY [OrgName]
            ";

            DataTable dt = db.GetTable();

            // 加入「全部申請單位」選項
            result.Add(new DropdownItem { Value = "", Text = "全部申請單位" });

            foreach (DataRow row in dt.Rows)
            {
                if (row["OrgName"] != DBNull.Value)
                {
                    string orgName = row["OrgName"].ToString().Trim();
                    if (!string.IsNullOrEmpty(orgName))
                    {
                        result.Add(new DropdownItem { Value = orgName, Text = orgName });
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Type5 申請單位選項載入完成，共 {result.Count - 1} 個單位");
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得 Type5 申請單位選項時發生錯誤：{ex.Message}");
            throw new Exception($"取得 Type5 申請單位選項時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 取得 Type5 計畫變更審核的主管單位選項
    /// </summary>
    /// <returns>主管單位清單</returns>
    public static List<DropdownItem> GetType5SupervisoryUnitOptions()
    {
        DbHelper db = new DbHelper();
        List<DropdownItem> result = new List<DropdownItem>();

        try
        {
            db.CommandText = @"
                SELECT distinct [SupervisoryUnit]
                FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type5]
                WHERE [SupervisoryUnit] IS NOT NULL AND [SupervisoryUnit] != ''
                ORDER BY [SupervisoryUnit]
            ";

            DataTable dt = db.GetTable();

            // 加入「全部主管單位」選項
            result.Add(new DropdownItem { Value = "", Text = "全部主管單位" });

            foreach (DataRow row in dt.Rows)
            {
                if (row["SupervisoryUnit"] != DBNull.Value)
                {
                    string supervisoryUnit = row["SupervisoryUnit"].ToString().Trim();
                    if (!string.IsNullOrEmpty(supervisoryUnit))
                    {
                        result.Add(new DropdownItem { Value = supervisoryUnit, Text = supervisoryUnit });
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Type5 主管單位選項載入完成，共 {result.Count - 1} 個單位");
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得 Type5 主管單位選項時發生錯誤：{ex.Message}");
            throw new Exception($"取得 Type5 主管單位選項時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 取得 Type1 資格審查的承辦人員選項
    /// </summary>
    /// <returns>承辦人員清單</returns>
    public static List<DropdownItem> GetType1SupervisorOptions()
    {
        DbHelper db = new DbHelper();
        List<DropdownItem> result = new List<DropdownItem>();

        try
        {
            db.CommandText = @"
                SELECT DISTINCT SupervisoryPersonAccount, SupervisoryPersonName
                FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type1]
                WHERE SupervisoryPersonAccount != ''
                  AND SupervisoryPersonName != ''
                  AND SupervisoryPersonAccount IS NOT NULL
                  AND SupervisoryPersonName IS NOT NULL
                ORDER BY SupervisoryPersonName
            ";

            DataTable dt = db.GetTable();

            // 加入「全部承辦人員」選項
            result.Add(new DropdownItem { Value = "", Text = "全部承辦人員" });

            foreach (DataRow row in dt.Rows)
            {
                if (row["SupervisoryPersonAccount"] != DBNull.Value && row["SupervisoryPersonName"] != DBNull.Value)
                {
                    string account = row["SupervisoryPersonAccount"].ToString().Trim();
                    string name = row["SupervisoryPersonName"].ToString().Trim();

                    if (!string.IsNullOrEmpty(account) && !string.IsNullOrEmpty(name))
                    {
                        result.Add(new DropdownItem { Value = account, Text = name });
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Type1 承辦人員選項載入完成，共 {result.Count - 1} 個人員");
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得 Type1 承辦人員選項時發生錯誤：{ex.Message}");
            throw new Exception($"取得 Type1 承辦人員選項時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 取得 Type4 決審核定的承辦人員選項
    /// </summary>
    /// <returns>承辦人員清單</returns>
    public static List<DropdownItem> GetType4SupervisorOptions()
    {
        DbHelper db = new DbHelper();
        List<DropdownItem> result = new List<DropdownItem>();

        try
        {
            db.CommandText = @"
                SELECT DISTINCT SupervisoryPersonAccount, SupervisoryPersonName
                FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type4]
                WHERE SupervisoryPersonAccount != ''
                  AND SupervisoryPersonName != ''
                  AND SupervisoryPersonAccount IS NOT NULL
                  AND SupervisoryPersonName IS NOT NULL
                ORDER BY SupervisoryPersonName
            ";

            DataTable dt = db.GetTable();

            // 加入「全部承辦人員」選項
            result.Add(new DropdownItem { Value = "", Text = "全部承辦人員" });

            foreach (DataRow row in dt.Rows)
            {
                if (row["SupervisoryPersonAccount"] != DBNull.Value && row["SupervisoryPersonName"] != DBNull.Value)
                {
                    string account = row["SupervisoryPersonAccount"].ToString().Trim();
                    string name = row["SupervisoryPersonName"].ToString().Trim();

                    if (!string.IsNullOrEmpty(account) && !string.IsNullOrEmpty(name))
                    {
                        result.Add(new DropdownItem { Value = account, Text = name });
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Type4 承辦人員選項載入完成，共 {result.Count - 1} 個人員");
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得 Type4 承辦人員選項時發生錯誤：{ex.Message}");
            throw new Exception($"取得 Type4 承辦人員選項時發生錯誤：{ex.Message}", ex);
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
            new DropdownItem { Value = "未完成", Text = "未完成" },
            new DropdownItem { Value = "完成", Text = "完成" }
        };

        return result;
    }

    /// <summary>
    /// 取得回覆狀態選項（固定選項）
    /// </summary>
    /// <returns>回覆狀態清單</returns>
    public static List<DropdownItem> GetReviewReplyStatusOptions()
    {
        // 固定的審查進度選項
        List<DropdownItem> result = new List<DropdownItem>
        {
            new DropdownItem { Value = "", Text = "全部" },
            new DropdownItem { Value = "未完成", Text = "未完成" },
            new DropdownItem { Value = "完成", Text = "完成" }
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

    // /// <summary>
    // /// 查詢科專決審核定清單（支援分頁）
    // /// </summary>
    // /// <param name="year">年度</param>
    // /// <param name="orgName">申請單位</param>
    // /// <param name="supervisor">承辦人員</param>
    // /// <param name="keyword">關鍵字</param>
    // /// <param name="reviewGroupCode">審查組別代碼</param>
    // /// <param name="pageNumber">頁碼</param>
    // /// <param name="pageSize">每頁筆數</param>
    // /// <param name="totalRecords">總記錄數（輸出參數）</param>
    // /// <returns>分頁資料</returns>
    // public static PaginatedResult<ReviewChecklistItem> Search_SCI_Type4_Paged(
    //     out int totalRecords
    //     , string year = "",
    //     string orgName = "",
    //     string supervisor = "",
    //     string keyword = "",
    //     string reviewGroupCode = "",
    //     int pageNumber = 1,
    //     int pageSize = 10)
    // {
    //     var allData = Search_SCI_Type4(year, orgName, supervisor, keyword, reviewGroupCode);
    //     totalRecords = allData.Count;
    //
    //     var pagedData = allData
    //         .Skip((pageNumber - 1) * pageSize)
    //         .Take(pageSize)
    //         .ToList();
    //
    //     return new PaginatedResult<ReviewChecklistItem>
    //     {
    //         Data = pagedData,
    //         TotalRecords = totalRecords,
    //         PageNumber = pageNumber,
    //         PageSize = pageSize,
    //         TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
    //     };
    // }

    public static List<ReviewChecklistItem> Search_Type4(
        string year = "",
        string orgName = "",
        string supervisor = "",
        string keyword = "",
        string category="",
        string reviewGroupCode = "")
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
SELECT TOP (1000) [ProjectID]
      ,[Year]
      ,[ProjectNameTw]
      ,[OrgName]
      ,[Field]
      ,[SupervisoryPersonAccount]
      ,[TotalSubsidyPrice]
      ,[StatusesName]
      ,[ApprovedSubsidy]
      ,[FinalReviewNotes]
      ,[FinalReviewOrder]
      ,[TotalScore]
  FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type4]

";

        try
        {
            List<ReviewChecklistItem> checklist = new List<ReviewChecklistItem>();
            db.Parameters.Clear();

            // 添加篩選條件參數
            db.CommandText += "Where StatusesName != '不通過'";
            if (!string.IsNullOrEmpty(year))
            {
                db.CommandText += " AND Year = @year";
                db.Parameters.Add("@year", year);
            }

            if (!string.IsNullOrEmpty(orgName))
            {
                db.CommandText += " AND OrgName LIKE @orgName";
                db.Parameters.Add("@orgName", $"%{orgName}%");
            }

            if (!string.IsNullOrEmpty(supervisor))
            {
                db.CommandText += " AND SupervisoryPersonAccount = @supervisor";
                db.Parameters.Add("@supervisor", supervisor);
            }

            if (!string.IsNullOrEmpty(category))
            {
                db.CommandText += " AND ProjectID LIKE @category";
                db.Parameters.Add("@category", $"%{category}%");
            }

            if (!string.IsNullOrEmpty(reviewGroupCode))
            {
                db.CommandText += " AND Field = @reviewGroupCode";
                db.Parameters.Add("@reviewGroupCode", reviewGroupCode);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                db.CommandText += " AND (ProjectID LIKE @keyword OR ProjectNameTw LIKE @keyword)";
                db.Parameters.Add("@keyword", $"%{keyword}%");
            }

            db.CommandText += " ORDER BY FinalReviewOrder ASC, TotalScore DESC";

            DataTable dt = db.GetTable();

            foreach (DataRow row in dt.Rows)
            {
                var item = new ReviewChecklistItem
                {
                    ProjectID = row["ProjectID"]?.ToString(),
                    ProjectNameTw = row["ProjectNameTw"]?.ToString(),
                    Year = row["Year"]?.ToString(),
                    UserOrg = row["OrgName"]?.ToString(),
                    OrgName = row["OrgName"]?.ToString(),
                    StatusesName = row["StatusesName"]?.ToString(),
                    SupervisoryPersonAccount = row["SupervisoryPersonAccount"]?.ToString(),
                    TopicField = row["Field"]?.ToString(),
                    Req_SubsidyAmount = row["TotalSubsidyPrice"]?.ToString() ?? "0",
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
    S.TotalSubsidyPrice AS Req_SubsidyAmount,
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
                    Req_SubsidyAmount = row["Req_SubsidyAmount"] != DBNull.Value ? row["Req_SubsidyAmount"].ToString() : "0",
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
    //TODO 想刪除
    // /// <summary>
    // /// 查詢科專資格審查清單（支援分頁）
    // /// </summary>
    // /// <param name="year">年度</param>
    // /// <param name="status">狀態</param>
    // /// <param name="orgName">申請單位</param>
    // /// <param name="supervisor">承辦人員</param>
    // /// <param name="keyword">關鍵字</param>
    // /// <param name="pageNumber">頁碼</param>
    // /// <param name="pageSize">每頁筆數</param>
    // /// <param name="totalRecords">總記錄數（輸出參數）</param>
    // /// <returns>分頁資料</returns>
    // public static PaginatedResult<ReviewChecklistItem> Search_SCI_Type1_Paged(out int totalRecords,
    //     string year = "",
    //     string status = "",
    //     string orgName = "",
    //     string supervisor = "",
    //     string keyword = "",
    //     int pageNumber = 1,
    //     int pageSize = 10
    //     )
    // {
    //     var allData = Search_SCI_Type1(year, status, orgName, supervisor, keyword);
    //     totalRecords = allData.Count;
    //
    //     var pagedData = allData
    //         .Skip((pageNumber - 1) * pageSize)
    //         .Take(pageSize)
    //         .ToList();
    //
    //     return new PaginatedResult<ReviewChecklistItem>
    //     {
    //         Data = pagedData,
    //         TotalRecords = totalRecords,
    //         PageNumber = pageNumber,
    //         PageSize = pageSize,
    //         TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
    //     };
    // }

    public static List<ReviewChecklistItem> Search_Type1(
        string year = "",
        string status = "",
        string orgName = "",
        string supervisor = "",
        string keyword = "")
    {
        DbHelper db = new DbHelper();
        db.CommandText = $@"
         SELECT TOP (1000) [ProjectID]
              ,[ProjectNameTw]
              ,[OrgName]
              ,[StatusesName]
              ,[ExpirationDate]
              ,[SupervisoryPersonAccount]
              ,[SupervisoryPersonName]
              ,[SupervisoryUnit]
              ,[created_at]
              ,[Req_SubsidyAmount]
              ,[Year]
              ,[Category]
          FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type1]


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
                    db.CommandText += " AND OrgName LIKE @orgName";
                else
                {
                    db.CommandText += " WHERE OrgName LIKE @orgName";
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
                    OrgName = row["OrgName"]?.ToString(),
                    created_at = row["created_at"] != DBNull.Value ? (DateTime?)row["created_at"] : null,

                    // 從 SQL 結果設定的額外欄位
                    ProjectNameTw = row["ProjectNameTw"]?.ToString(),
                    Year = row["Year"]?.ToString(),
                    Req_SubsidyAmount = row["Req_SubsidyAmount"]?.ToString() ?? "0"
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
    // TODO 想刪除
    // /// <summary>
    // /// 查詢科專領域審查清單（支援分頁）
    // /// </summary>
    // /// <param name="year">年度</param>
    // /// <param name="orgName">申請單位</param>
    // /// <param name="supervisor">承辦人員</param>
    // /// <param name="keyword">關鍵字</param>
    // /// <param name="reviewProgress">審查進度</param>
    // /// <param name="replyProgress">回覆進度</param>
    // /// <param name="pageNumber">頁碼</param>
    // /// <param name="pageSize">每頁筆數</param>
    // /// <param name="totalRecords">總記錄數（輸出參數）</param>
    // /// <returns>分頁資料</returns>
    // public static PaginatedResult<ReviewChecklistItem> Search_SCI_Type2_Paged(out int totalRecords,
    //     string year = "",
    //     string orgName = "",
    //     string supervisor = "",
    //     string keyword = "",
    //     string reviewProgress = "",
    //     string replyProgress = "",
    //     int pageNumber = 1,
    //     int pageSize = 10
    //     )
    // {
    //     var allData = Search_SCI_Type2(year, orgName, supervisor, keyword, reviewProgress, replyProgress);
    //     totalRecords = allData.Count;
    //
    //     var pagedData = allData
    //         .Skip((pageNumber - 1) * pageSize)
    //         .Take(pageSize)
    //         .ToList();
    //
    //     return new PaginatedResult<ReviewChecklistItem>
    //     {
    //         Data = pagedData,
    //         TotalRecords = totalRecords,
    //         PageNumber = pageNumber,
    //         PageSize = pageSize,
    //         TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
    //     };
    // }


    /// <summary>
    /// 取得科專專案資料
    /// </summary>


    /// <summary>
    /// 取得科專基本資料
    /// </summary>
    public static List<ReviewChecklistItem> GetSciBasicData(

        string year,
        string orgName,
        string supervisor,
        string keyword,
        string status )
    {
        DbHelper db = new DbHelper();

        db.CommandText = $@"
            SELECT p.*,
                   m.ProjectNameTw,
                   m.OrgName,
                   YEAR(p.created_at) - 1911 AS 'Year',
                   m.SubsidyPlanType,
                   '科專' AS 'Category',
                   ISNULL((SELECT SUM(SubsidyAmount)
                           FROM OFS_SCI_PersonnelCost_TotalFee tf
                           WHERE tf.ProjectID = p.ProjectID), 0) AS 'Req_SubsidyAmount'
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
                Req_SubsidyAmount = row["Req_SubsidyAmount"]?.ToString() ?? "0"
            };

            result.Add(item);
        }

        return result;
    }

    /// <summary>
    /// 取得科專進度資料
    /// </summary>
    public static List<ProgressData> GetSciProgressData(List<string> projectIds, string status)
    {
        if (projectIds.Count == 0) return new List<ProgressData>();

        // 建立 IN 子句的參數
        var projectIdParams = projectIds.Select((id, index) => $"@projectId{index}").ToList();
        string inClause = "(" + string.Join(",", projectIdParams) + ")";
        DbHelper db = new DbHelper();

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
                    WHEN COUNT(RR.ReviewComment) > 0 AND COUNT(RR.ProjectID) = COUNT(RR.ReplyComment) THEN '完成'
                    ELSE '未完成'
                END AS ReplyProgress
            FROM OFS_ReviewRecords RR
            WHERE RR.ReviewStage = '領域審查'
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
    public static List<ReviewGroupData> GetSciReviewGroupData(List<string> projectIds)
    {
        DbHelper db = new DbHelper();
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
    //TODO 想刪除
    // /// <summary>
    // /// 查詢科專技術審查清單（支援分頁）
    // /// </summary>
    // /// <param name="year">年度</param>
    // /// <param name="orgName">申請單位</param>
    // /// <param name="supervisor">承辦人員</param>
    // /// <param name="keyword">關鍵字</param>
    // /// <param name="reviewProgress">審查進度</param>
    // /// <param name="replyProgress">回覆進度</param>
    // /// <param name="pageNumber">頁碼</param>
    // /// <param name="pageSize">每頁筆數</param>
    // /// <param name="totalRecords">總記錄數（輸出參數）</param>
    // /// <returns>分頁資料</returns>
    // public static PaginatedResult<ReviewChecklistItem> Search_SCI_Type3_Paged(out int totalRecords,
    //     string year = "",
    //     string orgName = "",
    //     string supervisor = "",
    //     string keyword = "",
    //     string reviewProgress = "",
    //     string replyProgress = "",
    //     int pageNumber = 1,
    //     int pageSize = 10
    //     )
    // {
    //     var allData = Search_SCI_Type3(year, orgName, supervisor, keyword, reviewProgress, replyProgress);
    //     totalRecords = allData.Count;
    //
    //     var pagedData = allData
    //         .Skip((pageNumber - 1) * pageSize)
    //         .Take(pageSize)
    //         .ToList();
    //
    //     return new PaginatedResult<ReviewChecklistItem>
    //     {
    //         Data = pagedData,
    //         TotalRecords = totalRecords,
    //         PageNumber = pageNumber,
    //         PageSize = pageSize,
    //         TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
    //     };
    // }



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
                    LastOperation = '計畫已核定',
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
                        if (projectId.Contains("SCI"))
                        {
                            // 更新專案狀態為不通過
                            UpdateProjectRejectStatus(db, projectId);
                            // 記錄審查歷程
                            InsertRejectHistory(db, projectId, actionType, userAccount);
                            successCount++;
                            successIds.Add(projectId);
                        }
                        else if (projectId.Contains("CUL"))
                        {
                            OFS_CulProjectHelper.updateStatus(projectId, 4);
                            InsertRejectHistory(db, projectId, actionType, userAccount);
                            successCount++;
                            successIds.Add(projectId);
                        }
                        else if (projectId.Contains("EDC"))
                        {
                            OFS_EdcProjectHelper.updateStatus(projectId, 4);
                            InsertRejectHistory(db, projectId, actionType, userAccount);
                            successCount++;
                            successIds.Add(projectId);
                        }
                        else if (projectId.Contains("CLB"))
                        {
                            // 學校社團
                        }
                        else if (projectId.Contains("MUL"))
                        {
                            OFS_MulProjectHelper.updateStatus(projectId, 4);
                            InsertRejectHistory(db, projectId, actionType, userAccount);
                            successCount++;
                            successIds.Add(projectId);
                        }
                        else if (projectId.Contains("LIT"))
                        {
                            OFS_LitProjectHelper.updateStatus(projectId, 4);
                            InsertRejectHistory(db, projectId, actionType, userAccount);
                            successCount++;
                            successIds.Add(projectId);
                        }
                        else if (projectId.Contains("ACC"))
                        {
                            OFS_AccProjectHelper.updateStatus(projectId, 4);
                            InsertRejectHistory(db, projectId, actionType, userAccount);
                            successCount++;
                            successIds.Add(projectId);
                        }
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

    #region Type=5 計畫變更審核查詢方法

    /// <summary>
    /// 查詢計畫變更審核清單 (Type=5)
    /// </summary>
    /// <param name="year">年度</param>
    /// <param name="category">類別</param>
    /// <param name="orgName">申請單位</param>
    /// <param name="supervisoryUnit">主管單位</param>
    /// <param name="keyword">計畫編號或名稱關鍵字</param>
    /// <returns>計畫變更審核項目清單</returns>
    public static List<PlanChangeReviewItem> Search_Type5_PlanChangeReview(
        string year = "",
        string category = "",
        string orgName = "",
        string supervisoryUnit = "",
        string keyword = "")
    {
        List<PlanChangeReviewItem> results = new List<PlanChangeReviewItem>();
        var db = new DbHelper();

        try
        {
            // 建立基礎查詢語句
            db.CommandText = @"
                SELECT
                    [Year],
                    [ProjectID],
                    [Category],
                    [ProjectNameTw],
                    [OrgName],
                    [SupervisoryUnit]
                FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type5]
                WHERE 1=1";

            // 清除參數
            db.Parameters.Clear();

            // 動態新增查詢條件
            if (!string.IsNullOrEmpty(year))
            {
                db.CommandText += " AND [Year] = @Year";
                db.Parameters.Add("@Year", year);
            }

            if (!string.IsNullOrEmpty(category))
            {
                db.CommandText += " AND [Category] = @Category";
                db.Parameters.Add("@Category", category);
            }

            if (!string.IsNullOrEmpty(orgName))
            {
                db.CommandText += " AND [OrgName] LIKE @OrgName";
                db.Parameters.Add("@OrgName", $"%{orgName}%");
            }

            if (!string.IsNullOrEmpty(supervisoryUnit))
            {
                db.CommandText += " AND [SupervisoryUnit] LIKE @SupervisoryUnit";
                db.Parameters.Add("@SupervisoryUnit", $"%{supervisoryUnit}%");
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                db.CommandText += " AND ([ProjectID] LIKE @Keyword OR [ProjectNameTw] LIKE @Keyword)";
                db.Parameters.Add("@Keyword", $"%{keyword}%");
            }

            // 排序
            db.CommandText += " ORDER BY [Year] DESC, [ProjectID] ASC";

            // 執行查詢
            DataTable dt = db.GetTable();

            // 處理查詢結果
            foreach (DataRow row in dt.Rows)
            {
                var item = new PlanChangeReviewItem
                {
                    Year = row["Year"]?.ToString() ?? "",
                    ProjectID = row["ProjectID"]?.ToString() ?? "",
                    Category = row["Category"]?.ToString() ?? "",
                    ProjectNameTw = row["ProjectNameTw"]?.ToString() ?? "",
                    OrgName = row["OrgName"]?.ToString() ?? "",
                    SupervisoryUnit = row["SupervisoryUnit"]?.ToString() ?? ""
                };

                results.Add(item);
            }

            System.Diagnostics.Debug.WriteLine($"Type5 查詢完成，共找到 {results.Count} 筆資料");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Search_Type5_PlanChangeReview 查詢時發生錯誤：{ex.Message}");
            throw new Exception($"Search_Type5_PlanChangeReview 查詢時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }

        return results;
    }

    /// <summary>
    /// 搜尋 Type=5 計畫變更審核資料（支援分頁）
    /// </summary>
    /// <param name="year">年度</param>
    /// <param name="category">類別</param>
    /// <param name="orgName">申請單位</param>
    /// <param name="supervisoryUnit">主管單位</param>
    /// <param name="keyword">關鍵字</param>
    /// <param name="pageNumber">頁碼</param>
    /// <param name="pageSize">每頁筆數</param>
    /// <returns>分頁結果</returns>
    public static PaginatedResult<PlanChangeReviewItem> Search_Type5_PlanChangeReview_Paged(
        string year = "",
        string category = "",
        string orgName = "",
        string supervisoryUnit = "",
        string keyword = "",
        int pageNumber = 1,
        int pageSize = 10)
    {
        // 取得所有資料
        var allData = Search_Type5_PlanChangeReview(year, category, orgName, supervisoryUnit, keyword);

        // 計算分頁資料
        int totalRecords = allData.Count;
        int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

        var pagedData = allData
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PaginatedResult<PlanChangeReviewItem>
        {
            Data = pagedData,
            TotalRecords = totalRecords,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }

    /// <summary>
    /// 搜尋 Type=6 執行計畫審核資料
    /// </summary>
    /// <param name="year">年度</param>
    /// <param name="category">類別</param>
    /// <param name="orgName">申請單位</param>
    /// <param name="supervisoryUnit">主管單位</param>
    /// <param name="keyword">關鍵字</param>
    /// <returns>執行計畫審核項目清單</returns>
    public static List<ExecutionPlanReviewItem> Search_Type6_ExecutionPlanReview(
        string year = "",
        string category = "",
        string orgName = "",
        string supervisoryUnit = "",
        string keyword = "")
    {
        List<ExecutionPlanReviewItem> results = new List<ExecutionPlanReviewItem>();
        var db = new DbHelper();

        try
        {
            // 建立基礎查詢語句
            db.CommandText = @"
                SELECT TOP (1000)
                    [Year],
                    [ProjectID],
                    [Category],
                    [Stage],
                    [ReviewTodo],
                    [SupervisoryPersonAccount],
                    [SupervisoryUnit],
                    [OrgName],
                    [ProjectNameTw]
                FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type6]
                WHERE 1=1";

            // 清除參數
            db.Parameters.Clear();

            // 動態新增查詢條件
            if (!string.IsNullOrEmpty(year))
            {
                db.CommandText += " AND [Year] = @Year";
                db.Parameters.Add("@Year", year);
            }

            if (!string.IsNullOrEmpty(category))
            {
                db.CommandText += " AND [Category] = @Category";
                db.Parameters.Add("@Category", category);
            }

            if (!string.IsNullOrEmpty(orgName))
            {
                db.CommandText += " AND [OrgName] LIKE @OrgName";
                db.Parameters.Add("@OrgName", "%" + orgName + "%");
            }

            if (!string.IsNullOrEmpty(supervisoryUnit))
            {
                db.CommandText += " AND [SupervisoryUnit] LIKE @SupervisoryUnit";
                db.Parameters.Add("@SupervisoryUnit", "%" + supervisoryUnit + "%");
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                db.CommandText += " AND ([ProjectID] LIKE @Keyword OR [ProjectNameTw] LIKE @Keyword OR [OrgName] LIKE @Keyword)";
                db.Parameters.Add("@Keyword", "%" + keyword + "%");
            }

            // 排序
            db.CommandText += " ORDER BY [Year] DESC, [ProjectID] ASC";

            // 執行查詢
            DataTable dt = db.GetTable();

            // 轉換資料
            foreach (DataRow row in dt.Rows)
            {
                var projectId = row["ProjectID"]?.ToString() ?? "";
                var rowCategory = row["Category"]?.ToString() ?? "";

                var item = new ExecutionPlanReviewItem
                {
                    Year = row["Year"]?.ToString() ?? "",
                    ProjectID = projectId,
                    Category = rowCategory,
                    Stage = row["Stage"]?.ToString() ?? "",
                    ReviewTodo = row["ReviewTodo"]?.ToString() ?? "",
                    SupervisoryPersonAccount = row["SupervisoryPersonAccount"]?.ToString() ?? "",
                    SupervisoryUnit = row["SupervisoryUnit"]?.ToString() ?? "",
                    OrgName = row["OrgName"]?.ToString() ?? "",
                    ProjectNameTw = row["ProjectNameTw"]?.ToString() ?? ""
                };

                // 僅針對科專專案加入審查委員進度
                if (rowCategory == "SCI")
                {
                    item.ReviewProgress = GetSciReviewProgress(projectId);
                }

                results.Add(item);
            }

            System.Diagnostics.Debug.WriteLine($"Search_Type6_ExecutionPlanReview 共查詢到 {results.Count} 筆資料");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Search_Type6_ExecutionPlanReview 查詢時發生錯誤：{ex.Message}");
            throw new Exception($"Search_Type6_ExecutionPlanReview 查詢時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }

        return results;
    }

    // /// <summary>
    // /// 搜尋 Type=6 執行計畫審核資料（支援分頁）
    // /// </summary>
    // /// <param name="year">年度</param>
    // /// <param name="category">類別</param>
    // /// <param name="orgName">申請單位</param>
    // /// <param name="supervisoryUnit">主管單位</param>
    // /// <param name="keyword">關鍵字</param>
    // /// <param name="pageNumber">頁碼</param>
    // /// <param name="pageSize">每頁筆數</param>
    // /// <returns>分頁結果</returns>
    // public static PaginatedResult<ExecutionPlanReviewItem> Search_Type6_ExecutionPlanReview_Paged(
    //     string year = "",
    //     string category = "",
    //     string orgName = "",
    //     string supervisoryUnit = "",
    //     string keyword = "",
    //     int pageNumber = 1,
    //     int pageSize = 10)
    // {
    //     // 取得所有資料
    //     var allData = Search_Type6_ExecutionPlanReview(year, category, orgName, supervisoryUnit, keyword);
    //
    //     // 計算分頁資料
    //     int totalRecords = allData.Count;
    //     int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
    //
    //     var pagedData = allData
    //         .Skip((pageNumber - 1) * pageSize)
    //         .Take(pageSize)
    //         .ToList();
    //
    //     return new PaginatedResult<ExecutionPlanReviewItem>
    //     {
    //         Data = pagedData,
    //         TotalRecords = totalRecords,
    //         PageNumber = pageNumber,
    //         PageSize = pageSize,
    //         TotalPages = totalPages
    //     };
    // }

    /// <summary>
    /// 取得科專專案的審查委員進度
    /// </summary>
    /// <param name="projectId">專案編號</param>
    /// <returns>審查委員進度字串，格式：已繳交人數/總人數 狀態</returns>
    public static string GetSciReviewProgress(string projectId)
    {
        if (string.IsNullOrEmpty(projectId) || !projectId.Contains("SCI"))
        {
            return null;
        }

        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
                SELECT
                    CAST(SUM(CASE WHEN SR.isSubmit = 1 THEN 1 ELSE 0 END) AS VARCHAR(10)) + '/' +
                    CAST(COUNT(*) AS VARCHAR(10)) + ' ' +
                    CASE
                        WHEN SUM(CASE WHEN SR.isSubmit = 1 THEN 1 ELSE 0 END) < COUNT(*)
                            THEN N'未完成'
                        ELSE N'完成'
                    END AS ReviewProgress
                FROM dbo.OFS_SCI_StageExam SE
                INNER JOIN dbo.OFS_SCI_StageExam_ReviewerList SR
                    ON SE.id = SR.ExamID
                WHERE SE.Status = '審核中'
                  AND SE.ProjectID = @ProjectID
                GROUP BY SE.ProjectID";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectId);

            DataTable dt = db.GetTable();

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["ReviewProgress"]?.ToString();
            }

            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得科專審查委員進度時發生錯誤：{ex.Message}");
            return null;
        }
        finally
        {
            db.Dispose();
        }
    }

    #endregion

    #region 待辦事項管理

    /// <summary>
    /// 為專案建立預設的待辦事項模板
    /// </summary>
    /// <param name="projectId">專案編號</param>
    /// <returns>是否成功建立</returns>
    public static void CreateTaskQueueTemplate(string projectId)
    {
        try
        {

            // 定義待辦事項模板
            var taskTemplates = new List<(string TaskNameEn, string TaskName, int PriorityLevel, bool IsTodo, bool IsCompleted)>
            {
                ("Contract", "簽訂契約資料", 1, true, false),
                ("Payment1", "第一次請款(預撥)", 2, true, false),
                ("Change", "計畫變更", 3, false, false),
                ("Schedule", "填寫預定進度", 4, true, false),
                ("MidReport", "填寫期中報告", 5, false, false),
                ("FinalReport", "填寫期末報告", 6, false, false),
                ("MonthlyReport", "填寫每月進度報告", 7, false, false),
                ("Payment2", "第二期請款", 8, false, false)
            };

            // 批次插入待辦事項
            BatchInsertTaskQueue(projectId, taskTemplates);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"建立待辦事項模板時發生錯誤：{ex.Message}");
        }
    }



    /// <summary>
    /// 批次插入待辦事項
    /// </summary>
    /// <param name="projectId">專案編號</param>
    /// <param name="taskTemplates">待辦事項模板列表</param>
    /// <returns>是否成功插入</returns>
    private static bool BatchInsertTaskQueue(string projectId, List<(string TaskNameEn, string TaskName, int PriorityLevel, bool IsTodo, bool IsCompleted)> taskTemplates)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                foreach (var template in taskTemplates)
                {
                    db.CommandText = $@"
                        INSERT INTO OFS_TaskQueue (ProjectID, TaskNameEn, TaskName, PriorityLevel, IsTodo, IsCompleted)
                        VALUES ('{projectId}', '{template.TaskNameEn}', '{template.TaskName}', {template.PriorityLevel}, {(template.IsTodo ? 1 : 0)}, {(template.IsCompleted ? 1 : 0)})";

                    db.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"批次插入待辦事項時發生錯誤：{ex.Message}");
                return false;
            }
        }
    }

    /// <summary>
    /// 取得專案的待辦事項列表
    /// </summary>
    /// <param name="projectId">專案編號</param>
    /// <returns>待辦事項列表</returns>
    public static List<OFS_TaskQueue> GetTaskQueueByProjectId(string projectId)
    {
        var taskList = new List<OFS_TaskQueue>();

        using (DbHelper db = new DbHelper())
        {
            try
            {
                db.CommandText = $@"
                    SELECT ProjectID, TaskName, PriorityLevel, IsTodo, IsCompleted
                    FROM OFS_TaskQueue
                    WHERE ProjectID = '{projectId}'
                    ORDER BY PriorityLevel";

                DataTable dt = db.GetTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        var task = new OFS_TaskQueue
                        {
                            ProjectID = row["ProjectID"]?.ToString(),
                            TaskName = row["TaskName"]?.ToString(),
                            PriorityLevel = row["PriorityLevel"] as int?,
                            IsTodo = Convert.ToBoolean(row["IsTodo"]),
                            IsCompleted = Convert.ToBoolean(row["IsCompleted"])
                        };
                        taskList.Add(task);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"取得待辦事項時發生錯誤：{ex.Message}");
            }
        }

        return taskList;
    }

    /// <summary>
    /// 更新待辦事項狀態
    /// </summary>
    /// <param name="projectId">專案編號</param>
    /// <param name="taskName">任務名稱</param>
    /// <param name="isCompleted">是否完成</param>
    /// <returns>是否成功更新</returns>
    public static void UpdateTaskQueueStatus(string projectId, string taskName, bool isCompleted)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                db.CommandText = $@"
                    UPDATE OFS_TaskQueue
                    SET IsCompleted = {(isCompleted ? 1 : 0)}
                    WHERE ProjectID = '{projectId}' AND TaskName = '{taskName}'";

               db.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"更新待辦事項狀態時發生錯誤：{ex.Message}");
            }
        }
    }

    #endregion
}
