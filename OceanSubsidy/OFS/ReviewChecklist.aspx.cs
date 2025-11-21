using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Services;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Operation.OFS;
using GS.OCA_OceanSubsidy.Model.OFS;
using GS.App;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using DocumentFormat.OpenXml.Wordprocessing;
using ListItem = System.Web.UI.WebControls.ListItem;

/// <summary>
/// 統一審查清單頁面
/// 整合原本的 information-plan-review-list1 ~ list4 功能
/// </summary>
public partial class OFS_ReviewChecklist : System.Web.UI.Page
{
    #region 屬性

    /// <summary>
    /// 審查類型 (1:資格審查, 2:書面審查, 3:會議審查, 4:結果公告)
    /// </summary>
    protected string ReviewType => Request.QueryString["type"];

    #endregion

    #region 頁面事件

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // 檢查使用者權限
            if (!CheckUserPermission())
            {
                ShowErrorAndRedirect();
                return;
            }

            if (!IsPostBack)
            {
                InitializePage();
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "頁面載入時發生錯誤");
        }
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 載入補助類型選項從 OFS_GrantType 資料表
    /// </summary>
    /// <param name="includeAll">是否包含「全部」選項</param>
    /// <returns>補助類型選項清單</returns>
    private List<ListItem> LoadGrantTypeOptions(bool includeAll = true)
    {
        var options = new List<ListItem>();

        try
        {
            // 如果需要，先加入「全部」選項
            if (includeAll)
            {
                options.Add(new ListItem("全部", ""));
            }

            // 從資料庫讀取補助類型
            using (var db = new GS.Data.Sql.DbHelper())
            {
                db.CommandText = @"
                    SELECT TypeCode,ShortName
                    FROM [OCA_OceanSubsidy].[dbo].[OFS_GrantType]
                    where TypeID in(1,2,3,4,5,6,7)
                    ORDER BY [TypeID]
                ";

                var table = db.GetTable();

                foreach (DataRow row in table.Rows)
                {
                    string shortName = row["ShortName"]?.ToString() ?? "";
                    string typeCode = row["TypeCode"]?.ToString() ?? "";

                    if (!string.IsNullOrEmpty(shortName) && !string.IsNullOrEmpty(typeCode))
                    {
                        options.Add(new ListItem(shortName, typeCode));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入補助類型選項時發生錯誤");

            // 發生錯誤時，提供預設選項
            if (includeAll && options.Count == 0)
            {
                options.Add(new ListItem("全部", ""));
            }
        }

        return options;
    }

    /// <summary>
    /// 初始化頁面
    /// </summary>
    private void InitializePage()
    {
        try
        {
            // 驗證審查類型參數
            if (!IsValidReviewType(ReviewType))
            {
                // 如果沒有提供 type 參數或參數無效，預設為類型 1
                Response.Redirect("~/OFS/ReviewChecklist.aspx?type=1");
                return;
            }

            // 根據審查類型載入對應內容
            LoadReviewContent(ReviewType);

            // 設定前端選中狀態
            SetActiveReviewType(ReviewType);

        }
        catch (Exception ex)
        {
            HandleException(ex, "初始化頁面時發生錯誤");
        }
    }

    /// <summary>
    /// 驗證審查類型是否有效
    /// </summary>
    /// <param name="type">審查類型</param>
    /// <returns>是否有效</returns>
    private bool IsValidReviewType(string type)
    {
        return !string.IsNullOrEmpty(type) &&
               new[] { "1", "2", "3", "4", "5", "6" }.Contains(type);
    }

    /// <summary>
    /// 根據審查類型載入對應內容
    /// </summary>
    /// <param name="type">審查類型</param>
    private void LoadReviewContent(string type)
    {
        try
        {
            switch (type)
            {
                case "1":
                    LoadQualificationReviewDropdowns();
                    break;
                case "2":
                    LoadDomainReviewDropdowns();
                    break;
                case "3":
                    LoadTechnicalReviewDropdowns();
                    break;
                case "4":
                    LoadDecisionReviewDropdowns();
                    break;
                case "5":
                    LoadPlanChangeReviewDropdowns();
                    break;
                case "6":
                    LoadExecutionPlanReviewDropdowns();
                    break;
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, $"載入審查類型 {type} 內容時發生錯誤");
        }
    }

    /// <summary>
    /// 設定前端選中的審查類型
    /// </summary>
    /// <param name="type">審查類型</param>
    private void SetActiveReviewType(string type)
    {
        string script = $@"
            $(function() {{
                // 設定選中的審查類型
                $('#type{type}').prop('checked', true);

                // 顯示對應的內容區塊
                $('.review-content').hide();
                $('#content-type-{type}').show();
            }});
        ";

        Page.ClientScript.RegisterStartupScript(this.GetType(), "SetActiveType", script, true);
    }



    /// <summary>
    /// 檢查使用者權限
    /// </summary>
    /// <returns>是否有權限</returns>
    private bool CheckUserPermission()
    {
        try
        {
            // 取得當前使用者資訊
            var currentUser = GetCurrentUserInfo();
            if (currentUser == null || currentUser.OFS_RoleName == null)
            {
                return false;
            }

            // 檢查是否為審核相關角色
            var reviewRoles = new[] { "主管單位人員", "主管單位窗口", "系統管理者", "審查委員" };

            foreach (string roleName in currentUser.OFS_RoleName)
            {
                if (!string.IsNullOrEmpty(roleName) && reviewRoles.Contains(roleName))
                {
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            HandleException(ex, "檢查使用者權限時發生錯誤");
            return false;
        }
    }

    /// <summary>
    /// 顯示錯誤訊息並跳轉
    /// </summary>
    private void ShowErrorAndRedirect()
    {
        Response.Redirect("~/OFS/Home.aspx");
    }

    /// <summary>
    /// 例外處理
    /// </summary>
    private void HandleException(Exception ex, string context)
    {
        System.Diagnostics.Debug.WriteLine($"{context}: {ex.Message}");
        // 可以在這裡加入記錄或通知邏輯
    }
    #endregion
    #region  type-1 的下拉式選單
    /// <summary>
    /// 載入資格審查/內容審查的下拉選單資料
    /// </summary>
    private void LoadQualificationReviewDropdowns()
    {
        try
        {
            // 載入年度選項（動態）
            ddlYear_Type1.Items.Add(new ListItem("全部", ""));
            var years = ReviewCheckListHelper.GetType1YearOptions();
            foreach (var year in years)
            {
                ddlYear_Type1.Items.Add(new ListItem($"{year}年", year));
            }

            // 載入類別選項
            var categoryOptions = LoadGrantTypeOptions(true);
            ddlCategory_Type1.DataSource = categoryOptions;
            ddlCategory_Type1.DataTextField = "Text";
            ddlCategory_Type1.DataValueField = "Value";
            ddlCategory_Type1.DataBind();

            // 載入階段狀態選項
            var stageOptions = ReviewCheckListHelper.GetReviewStageStatusOptions();
            ddlStatus_Type1.DataSource = stageOptions;
            ddlStatus_Type1.DataTextField = "Text";
            ddlStatus_Type1.DataValueField = "Value";
            ddlStatus_Type1.DataBind();


            // 載入申請單位選項
            var orgOptions = ReviewCheckListHelper.GetType1OrgOptions();
            ddlOrg_Type1.DataSource = orgOptions;
            ddlOrg_Type1.DataTextField = "Text";
            ddlOrg_Type1.DataValueField = "Value";
            ddlOrg_Type1.DataBind();

            // 載入承辦人員選項 - 從 V_OFS_ReviewChecklist_type1 動態載入
            var supervisorOptions = ReviewCheckListHelper.GetType1SupervisorOptions();
            ddlSupervisor_Type1.DataSource = supervisorOptions;
            ddlSupervisor_Type1.DataTextField = "Text";
            ddlSupervisor_Type1.DataValueField = "Value";
            ddlSupervisor_Type1.DataBind();
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入資格審查下拉選單時發生錯誤");
        }
    }
    #endregion
    # region type-2 的下拉式選單
    /// <summary>
    /// 載入科專實質審查的下拉選單資料
    /// </summary>
    private void LoadDomainReviewDropdowns()
    {
        try
        {
            // 載入年度選項（動態）
            ddlYear_Type2.Items.Add(new ListItem("全部", ""));
            var years = ReviewCheckListHelper.GetType2YearOptions();
            foreach (var year in years)
            {
                ddlYear_Type2.Items.Add(new ListItem($"{year}年", year));
            }

            // 載入類別選項
            var categoryOptions = LoadGrantTypeOptions(true);
            ddlCategory_Type2.DataSource = categoryOptions;
            ddlCategory_Type2.DataTextField = "Text";
            ddlCategory_Type2.DataValueField = "Value";
            ddlCategory_Type2.DataBind();

            // 載入申請單位選項
            var orgOptions = ReviewCheckListHelper.GetReviewOrgOptions("實質審查", 21);
            ddlOrg_Type2.DataSource = orgOptions;
            ddlOrg_Type2.DataTextField = "Text";
            ddlOrg_Type2.DataValueField = "Value";
            ddlOrg_Type2.DataBind();

            // 載入承辦人員選項
            var supervisorOptions = ReviewCheckListHelper.GetReviewSupervisorOptions("實質審查", 21);
            ddlSupervisor_Type2.DataSource = supervisorOptions;
            ddlSupervisor_Type2.DataTextField = "Text";
            ddlSupervisor_Type2.DataValueField = "Value";
            ddlSupervisor_Type2.DataBind();

            // 載入審查進度選項
            var progressOptions = ReviewCheckListHelper.GetReviewProgressOptions();
            ddlProgress_Type2.DataSource = progressOptions;
            ddlProgress_Type2.DataTextField = "Text";
            ddlProgress_Type2.DataValueField = "Value";
            ddlProgress_Type2.DataBind();

            // 載入回覆狀態選項
            var replyStatusOptions = ReviewCheckListHelper.GetReviewReplyStatusOptions();
            ddlReplyStatus_Type2.DataSource = replyStatusOptions;
            ddlReplyStatus_Type2.DataTextField = "Text";
            ddlReplyStatus_Type2.DataValueField = "Value";
            ddlReplyStatus_Type2.DataBind();
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入實質審查下拉選單時發生錯誤");
        }
    }
    #endregion
    # region type-3 的下拉式選單
    /// <summary>
    /// 載入技術審查/複審的下拉選單資料
    /// </summary>
    private void LoadTechnicalReviewDropdowns()
    {
        try
        {
            // 載入年度選項（動態）
            ddlYear_Type3.Items.Add(new ListItem("全部", ""));
            var years = ReviewCheckListHelper.GetType3YearOptions();
            foreach (var year in years)
            {
                ddlYear_Type3.Items.Add(new ListItem($"{year}年", year));
            }

            // 載入類別選項
            var categoryOptions = LoadGrantTypeOptions(true);
            ddlCategory_Type3.DataSource = categoryOptions;
            ddlCategory_Type3.DataTextField = "Text";
            ddlCategory_Type3.DataValueField = "Value";
            ddlCategory_Type3.DataBind();

            // 載入申請單位選項
            var orgOptions = ReviewCheckListHelper.GetReviewOrgOptions("技術審查", 31);
            ddlOrg_Type3.DataSource = orgOptions;
            ddlOrg_Type3.DataTextField = "Text";
            ddlOrg_Type3.DataValueField = "Value";
            ddlOrg_Type3.DataBind();

            // 載入承辦人員選項
            var supervisorOptions = ReviewCheckListHelper.GetReviewSupervisorOptions("技術審查", 31);
            ddlSupervisor_Type3.DataSource = supervisorOptions;
            ddlSupervisor_Type3.DataTextField = "Text";
            ddlSupervisor_Type3.DataValueField = "Value";
            ddlSupervisor_Type3.DataBind();

            // 載入審查進度選項
            var progressOptions = ReviewCheckListHelper.GetReviewProgressOptions();
            ddlProgress_Type3.DataSource = progressOptions;
            ddlProgress_Type3.DataTextField = "Text";
            ddlProgress_Type3.DataValueField = "Value";
            ddlProgress_Type3.DataBind();

            // 載入回覆狀態選項
            var replyStatusOptions = ReviewCheckListHelper.GetReviewReplyStatusOptions();
            ddlReplyStatus_Type3.DataSource = replyStatusOptions;
            ddlReplyStatus_Type3.DataTextField = "Text";
            ddlReplyStatus_Type3.DataValueField = "Value";
            ddlReplyStatus_Type3.DataBind();
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入技術審查下拉選單時發生錯誤");
        }
    }

    #endregion
    # region type-4 的下拉式選單
    /// <summary>
    /// 載入決定審核清單的下拉選單資料
    /// </summary>
    private void LoadDecisionReviewDropdowns()
    {
        try
        {
            // 載入年度選項（動態，不含「全部」選項）
            var years = ReviewCheckListHelper.GetType4YearOptions();
            var yearList = years.Select(y => new { Text = $"{y}年", Value = y }).ToList();
            ddlYear_Type4.DataSource = yearList;
            ddlYear_Type4.DataTextField = "Text";
            ddlYear_Type4.DataValueField = "Value";
            ddlYear_Type4.DataBind();



            // 載入申請單位選項
            var orgOptions = ReviewCheckListHelper.GetType4OrgOptions();
            ddlOrg_Type4.DataSource = orgOptions;
            ddlOrg_Type4.DataTextField = "Text";
            ddlOrg_Type4.DataValueField = "Value";
            ddlOrg_Type4.DataBind();

            // 載入承辦人員選項 - 從 V_OFS_ReviewChecklist_type4 動態載入
            var supervisorOptions = ReviewCheckListHelper.GetType4SupervisorOptions();
            ddlSupervisor_Type4.DataSource = supervisorOptions;
            ddlSupervisor_Type4.DataTextField = "Text";
            ddlSupervisor_Type4.DataValueField = "Value";
            ddlSupervisor_Type4.DataBind();

            // 載入類別選項（移除「全部」選項）
            var categoryOptions = LoadGrantTypeOptions(false);
            ddlCategory_Type4.DataSource = categoryOptions;
            ddlCategory_Type4.DataTextField = "Text";
            ddlCategory_Type4.DataValueField = "Value";
            ddlCategory_Type4.DataBind();

            // // 初始化審查組別選項 (預設載入科專的審查組別，並加入「全部」選項)
            ddlReviewGroup_Type4.Items.Clear();
            var reviewGroupOptions = new List<DropdownItem>();
            reviewGroupOptions.Add(new DropdownItem { Text = "全部", Value = "" });
            reviewGroupOptions.AddRange(ReviewCheckListHelper.GetSciReviewGroupOptions());
            ddlReviewGroup_Type4.DataSource = reviewGroupOptions;
            ddlReviewGroup_Type4.DataTextField = "Text";
            ddlReviewGroup_Type4.DataValueField = "Value";
            ddlReviewGroup_Type4.DataBind();

        }
        catch (Exception ex)
        {
            HandleException(ex, "載入決定審核清單下拉選單時發生錯誤");
        }
    }
    #endregion
    #region type-5 的下拉式選單
    /// <summary>
    /// 載入計畫變更審核的下拉選單資料
    /// </summary>
    private void LoadPlanChangeReviewDropdowns()
    {
        try
        {
            // 載入年度選項（動態）
            ddlYear_Type5.Items.Add(new ListItem("全部", ""));
            var years = ReviewCheckListHelper.GetType5YearOptions();
            foreach (var year in years)
            {
                ddlYear_Type5.Items.Add(new ListItem($"{year}年", year));
            }

            // 載入類別選項
            var categoryOptions = LoadGrantTypeOptions(true);
            ddlCategory_Type5.DataSource = categoryOptions;
            ddlCategory_Type5.DataTextField = "Text";
            ddlCategory_Type5.DataValueField = "Value";
            ddlCategory_Type5.DataBind();

            // 載入主管單位選項 - 從 V_OFS_ReviewChecklist_type5 動態載入
            var supervisoryOptions = ReviewCheckListHelper.GetType5SupervisoryUnitOptions();
            ddlDepartment_Type5.DataSource = supervisoryOptions;
            ddlDepartment_Type5.DataTextField = "Text";
            ddlDepartment_Type5.DataValueField = "Value";
            ddlDepartment_Type5.DataBind();

            // 載入申請單位選項 - 從 V_OFS_ReviewChecklist_type5 動態載入
            var orgOptions = ReviewCheckListHelper.GetType5OrgOptions();
            ddlOrg_Type5.DataSource = orgOptions;
            ddlOrg_Type5.DataTextField = "Text";
            ddlOrg_Type5.DataValueField = "Value";
            ddlOrg_Type5.DataBind();
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入計畫變更審核下拉選單時發生錯誤");
        }
    }
    #endregion
    #region type-6 的下拉式選單
    /// <summary>
    /// 載入執行計畫審核的下拉選單資料
    /// </summary>
    private void LoadExecutionPlanReviewDropdowns()
    {
        try
        {
            // 載入年度選項（動態）
            ddlYear_Type6.Items.Add(new ListItem("全部", ""));
            var years = ReviewCheckListHelper.GetType6YearOptions();
            foreach (var year in years)
            {
                ddlYear_Type6.Items.Add(new ListItem($"{year}年", year));
            }

            // 載入類別選項
            var categoryOptions = LoadGrantTypeOptions(true);
            ddlCategory_Type6.DataSource = categoryOptions;
            ddlCategory_Type6.DataTextField = "Text";
            ddlCategory_Type6.DataValueField = "Value";
            ddlCategory_Type6.DataBind();

            // 載入主管單位選項 - 從 V_OFS_ReviewChecklist_type6 動態載入
            var supervisoryOptions = ReviewCheckListHelper.GetType6SupervisoryUnitOptions();
            ddlDepartment_Type6.DataSource = supervisoryOptions;
            ddlDepartment_Type6.DataTextField = "Text";
            ddlDepartment_Type6.DataValueField = "Value";
            ddlDepartment_Type6.DataBind();

            // 載入申請單位選項 - 從 V_OFS_ReviewChecklist_type6 動態載入
            var orgOptions = ReviewCheckListHelper.GetType6OrgOptions();
            ddlOrg_Type6.DataSource = orgOptions;
            ddlOrg_Type6.DataTextField = "Text";
            ddlOrg_Type6.DataValueField = "Value";
            ddlOrg_Type6.DataBind();
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入執行計畫審核下拉選單時發生錯誤");
        }
    }
    #endregion

    #region 查詢事件處理





    /// <summary>
    /// Type4 AJAX 搜尋方法
    /// </summary>
    [WebMethod]
    public static string AjaxSearch_Type4(string year, string category, string reviewGroup, string orgName, string supervisor, string keyword)
    {
        try
        {

            List<ReviewChecklistItem> results = ReviewCheckListHelper.Search_Type4(
                year,
                orgName,
                supervisor,
                keyword,
                category,
                reviewGroup);     // 審查組別代碼

            return JsonConvert.SerializeObject(new
            {
                success = true,
                data = results,
                count = results.Count,
                message = $"成功搜尋到 {results.Count} 筆資料"
            });
        }
        catch (Exception ex)
        {
            return JsonConvert.SerializeObject(new
            {
                success = false,
                message = "搜尋時發生錯誤: " + ex.Message,
                data = new List<ReviewChecklistItem>(),
                count = 0
            });
        }
    }

    /// <summary>
    /// Type4 核定模式儲存按鈕點擊事件
    /// </summary>
    [WebMethod]
    public static string SaveApprovalMode_Type4(List<ApprovalItem> approvalItems)
    {
        try
        {
            // 處理每一筆資料
            foreach (var item in approvalItems)
            {
                string tableName = "";
                var parameters = new Dictionary<string, object>
                {
                    ["@approvedSubsidy"] = item.ApprovedSubsidy ?? "0",
                    ["@finalReviewNotes"] = item.FinalReviewNotes ?? "",
                    ["@projectId"] = item.ProjectID
                };
                int.TryParse(item.ApprovedSubsidy, out int amount);

                switch (item.Category)
                {
                    case "SCI":
                        tableName = "OFS_SCI_Project_Main";
                        // 調用 Helper 執行資料庫更新
                        ReviewCheckListHelper.UpdateApprovalData(tableName, parameters);
                        break;
                    case "CUL":
                        OFS_CulProjectHelper.updateApprovedAmount(item.ProjectID, amount, item.FinalReviewNotes);
                        break;
                    case "EDC":
                        OFS_EdcProjectHelper.updateApprovedAmount(item.ProjectID, amount, item.FinalReviewNotes);
                        break;
                    case "CLB":
                        tableName = "OFS_CLB_Project_Main";
                        // 調用 Helper 執行資料庫更新
                        ReviewCheckListHelper.UpdateApprovalData(tableName, parameters);
                        break;
                    case "MUL":
                        OFS_MulProjectHelper.updateApprovedAmount(item.ProjectID, amount, item.FinalReviewNotes);
                        break;
                    case "LIT":
                        OFS_LitProjectHelper.updateApprovedAmount(item.ProjectID, amount, item.FinalReviewNotes);
                        break;
                    case "ACC":
                        OFS_AccProjectHelper.updateApprovedAmount(item.ProjectID, amount, item.FinalReviewNotes);
                        break;
                    default:
                        break;
                }
            }

            return JsonConvert.SerializeObject(new {
                success = true,
                message = "儲存成功",
                count = approvalItems.Count
            });
        }
        catch (Exception ex)
        {
            return JsonConvert.SerializeObject(new {
                success = false,
                message = $"儲存時發生錯誤：{ex.Message}"
            });
        }
    }

    /// <summary>
    /// 排序模式查詢 WebMethod
    /// </summary>
    /// <param name="year">年度</param>
    /// <param name="category">計畫類別</param>
    /// <returns>排序模式查詢結果</returns>
    [WebMethod]
    public static string SearchSortingMode(string year, string category)
    {
        try
        {
            List<SortingModeItem> results = ReviewCheckListHelper.Search_ForSorting(
                year: year,
                category: category
            );

            return JsonConvert.SerializeObject(new {
                success = true,
                data = results,
                count = results.Count
            });
        }
        catch (Exception ex)
        {
            return JsonConvert.SerializeObject(new {
                success = false,
                message = $"查詢時發生錯誤：{ex.Message}",
                data = new List<SortingModeItem>(),
                count = 0
            });
        }
    }

    /// <summary>
    /// 排序模式儲存 WebMethod
    /// </summary>
    /// <param name="sortingItems">排序項目清單</param>
    /// <returns>儲存結果</returns>
    [WebMethod]
    public static string SaveSortingMode(List<SortingSaveItem> sortingItems)
    {
        try
        {
            // 調用 Helper 執行資料庫更新
            ReviewCheckListHelper.UpdateSortingOrder(sortingItems);

            return JsonConvert.SerializeObject(new {
                success = true,
                message = "排序儲存成功",
                count = sortingItems.Count
            });
        }
        catch (Exception ex)
        {
            return JsonConvert.SerializeObject(new {
                success = false,
                message = $"儲存時發生錯誤：{ex.Message}"
            });
        }
    }

    #endregion

    #region 批次處理 WebMethods

    /// <summary>
    /// 處理批次轉下一階段功能
    /// </summary>
    /// <param name="projectIds">專案編號列表</param>
    /// <param name="actionType">操作類型</param>
    /// <param name="reviewType">審查類型</param>
    /// <param name="reviewerList">審查人員清單（僅 Type1、Type2 使用）</param>
    /// <returns>批次處理結果</returns>
    [WebMethod]
    public static BatchApprovalResult BatchApproveType(List<string> projectIds, string actionType, string reviewType, List<ReviewerInfo> reviewerList = null)
    {
        var result = new BatchApprovalResult
        {
            ActionType = actionType,
            ReviewType = reviewType
        };

        try
        {
            // 驗證參數
            if (projectIds == null || projectIds.Count == 0)
            {
                result.Success = false;
                result.Message = "未提供要處理的專案編號";
                result.ErrorMessages.Add("專案編號列表為空");
                return result;
            }

            if (string.IsNullOrEmpty(actionType))
            {
                result.Success = false;
                result.Message = "未指定操作類型";
                result.ErrorMessages.Add("actionType 參數為空");
                return result;
            }

            // 取得當前使用者資訊
            var currentUser = GetCurrentUserInfo();
            if (currentUser == null)
            {
                result.Success = false;
                result.Message = "無法取得使用者資訊";
                result.ErrorMessages.Add("使用者未登入或 Session 已過期");
                return result;
            }

            // 記錄操作開始
            System.Diagnostics.Debug.WriteLine($"開始批次處理: 類型={actionType}, 專案數量={projectIds.Count}");

            // 依補助案類型分組處理
            var projectGroups = GroupProjectsByType(projectIds);

            int totalSuccess = 0;
            var allSuccessIds = new List<string>();
            var allErrorMessages = new List<string>();

            foreach (var group in projectGroups)
            {
                string subsidyType = group.Key;
                var groupProjectIds = group.Value;

                // 根據補助案類型和操作類型決定狀態轉換
                string fromStatus, toStatus, StatusesName;
                if (!GetStatusTransitionBySubsidyType(subsidyType, actionType, reviewType, out fromStatus, out toStatus, out StatusesName))
                {
                    // 如果不支援此補助案類型，直接跳過
                    continue;
                }

                // 執行該組的批次更新
                var groupResult = BatchUpdateProjectStatus(
                    groupProjectIds,
                    fromStatus,
                    toStatus,
                    StatusesName,
                    currentUser.UserName,
                    actionType
                );

                // 只對成功更新的專案執行後續處理
                if (groupResult != null && groupResult.SuccessProjectIds != null && groupResult.SuccessProjectIds.Count > 0)
                {
                    // 科專批次審核後的特殊處理（只在資格審查和實質審查階段執行）
                    if ((reviewType == "1" || reviewType == "2") && actionType != "進入決審")
                    {
                        switch (subsidyType)
                        {
                            case "SCI":
                                ReviewCheckListHelper.ProcessSciPostApproval(groupResult.SuccessProjectIds, toStatus, reviewerList);

                                break;
                            case "CUL":
                                ReviewCheckListHelper.ProcessCulPostApproval(groupResult.SuccessProjectIds, toStatus, reviewerList);

                                break;
                            default:
                                break;
                        }
                    }
                }

                if (groupResult != null)
                {
                    totalSuccess += groupResult.SuccessCount;
                    allSuccessIds.AddRange(groupResult.SuccessProjectIds);
                    if (groupResult.ErrorMessages != null)
                    {
                        allErrorMessages.AddRange(groupResult.ErrorMessages);
                    }
                }
            }

            // 寄送審查通知給審查人員（只在資格審→領域審、領域審→技術審時寄信）
            if (totalSuccess > 0 && allSuccessIds.Count > 0 && actionType != "進入決審")
            {
                //統計哪些地方會寄信
                //1.只有資格審(1)→領域審(2) 會寄信給 審查委員(沒有寄給使用者)
                //2.領域審(2)→技術審(3) 會寄信給 審查委員 也會寄信給 使用者 因為技術審核需要PPT上傳
                //2.1 文化 初審可以直接進入決審，會寄信給使用者
                //3.技術審(3)→決審核定(4) 會寄信給 使用者 告知結果
                //4.決審核定(4)→執行階段 寄信 告知核定金額
                
                if (reviewType == "1" || reviewType == "2")
                {
                    SendReviewNotifications(allSuccessIds, reviewType);
                }
                else if (reviewType == "3")
                {
                    SendFinalReviewNotifications(allSuccessIds);
                }
                else if (reviewType == "4")
                {
                    // 決審核定，使用 E5 通知申請者
                    SendApprovalNotifications(allSuccessIds);
                }
            }

            // 如果是「進入決審」，寄送 E31 通知給申請者
            if (totalSuccess > 0 && allSuccessIds.Count > 0 && reviewType == "2" && actionType == "進入決審")
            {
                SendEnterFinalReviewNotifications(allSuccessIds);
            }

            // 設定最終結果
            result.Success = totalSuccess > 0;
            result.SuccessCount = totalSuccess;
            result.SuccessProjectIds = allSuccessIds;
            result.ErrorMessages = allErrorMessages;

            if (totalSuccess > 0)
            {
                result.Message = $"成功處理 {totalSuccess} 件計畫";
            }
            else
            {
                result.Message = "沒有符合條件的計畫可以處理";
            }

            // 記錄操作結果
            System.Diagnostics.Debug.WriteLine($"批次處理完成: 成功={totalSuccess}");

        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = "批次處理時發生系統錯誤";
            result.ErrorMessages.Add($"例外錯誤: {ex.Message}");

            // 記錄完整錯誤
            System.Diagnostics.Debug.WriteLine($"批次處理例外: {ex}");
        }

        return result;
    }

    /// <summary>
    /// 批次不通過處理
    /// </summary>
    /// <param name="projectIds">專案編號列表</param>
    /// <param name="actionType">操作類型</param>
    /// <param name="reviewType">審查類型</param>
    /// <returns>批次處理結果</returns>
    [WebMethod]
    public static BatchApprovalResult BatchRejectProjects(List<string> projectIds, string actionType, string reviewType)
    {
        var result = new BatchApprovalResult
        {
            ActionType = actionType,
            ProcessedAt = DateTime.Now
        };

        try
        {
            // 驗證參數
            if (projectIds == null || projectIds.Count == 0)
            {
                result.Success = false;
                result.Message = "未提供要處理的專案編號";
                result.ErrorMessages.Add("專案編號列表為空");
                return result;
            }

            // 取得當前使用者資訊
            var currentUser = GetCurrentUserInfo();
            if (currentUser == null)
            {
                result.Success = false;
                result.Message = "無法取得使用者資訊";
                result.ErrorMessages.Add("使用者未登入或 Session 已過期");
                return result;
            }

            // 執行批次不通過處理
            var batchResult = ReviewCheckListHelper.BatchRejectProjectStatus(
                projectIds,
                currentUser.Account,
                actionType ,reviewType
            );

            if (batchResult != null)
            {
                result.Success = batchResult.Success;
                result.SuccessCount = batchResult.SuccessCount;
                result.SuccessProjectIds = batchResult.SuccessProjectIds;
                result.ErrorMessages = batchResult.ErrorMessages;

                if (batchResult.Success)
                {
                    result.Message = $"成功處理 {batchResult.SuccessCount} 件計畫";

                    // 發送 E2 審查不通過通知給申請者
                    if (batchResult.SuccessProjectIds != null && batchResult.SuccessProjectIds.Count > 0)
                    {
                        SendRejectNotifications(batchResult.SuccessProjectIds);
                    }
                }
                else
                {
                    result.Message = "批次不通過處理失敗";
                }
            }

        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = "批次處理時發生系統錯誤";
            result.ErrorMessages.Add($"例外錯誤: {ex.Message}");

            System.Diagnostics.Debug.WriteLine($"批次不通過例外: {ex}");
        }

        return result;
    }

    /// <summary>
    /// 依補助案類型分組專案編號
    /// </summary>
    private static Dictionary<string, List<string>> GroupProjectsByType(List<string> projectIds)
    {
        var groups = new Dictionary<string, List<string>>();

        foreach (string projectId in projectIds)
        {
            if (string.IsNullOrEmpty(projectId)) continue;

            string subsidyType = GetSubsidyTypeFromProjectId(projectId);

            if (!groups.ContainsKey(subsidyType))
            {
                groups[subsidyType] = new List<string>();
            }

            groups[subsidyType].Add(projectId);
        }

        return groups;
    }

    /// <summary>
    /// 從專案編號判斷補助案類型
    /// </summary>
    private static string GetSubsidyTypeFromProjectId(string projectId)
    {
        if (string.IsNullOrEmpty(projectId)) return "UNKNOWN";

        if (projectId.Contains("SCI")) return "SCI";      // 科專
        if (projectId.Contains("CUL")) return "CUL";      // 文化
        if (projectId.Contains("EDC")) return "EDC";      // 學校民間
        if (projectId.Contains("CLB")) return "CLB";      // 學校社團
        if (projectId.Contains("MUL")) return "MUL";
        if (projectId.Contains("LIT")) return "LIT";
        if (projectId.Contains("ACC")) return "ACC";

        return "UNKNOWN";
    }

    /// <summary>
    /// 根據補助案類型和操作類型取得狀態轉換
    /// </summary>
    private static bool GetStatusTransitionBySubsidyType(string subsidyType, string currentActionType, string currentReviewType, out string fromStatus, out string toStatus , out string StatusesName)
    {
        fromStatus = "";
        toStatus = "";
        StatusesName = "";
        try
        {
            switch (subsidyType)
            {
                case "SCI": // 科專: 資格審查 --> 實質審查 --> 技術審查 --> 決審
                    switch (currentReviewType)
                    {
                        case "1": // 資格審查 → 實質審查
                            fromStatus = "資格審查";
                            toStatus = "實質審查";
                            StatusesName = "審核中";
                            return true;
                        case "2": // 實質審查 → 技術審查
                            fromStatus = "實質審查";
                            toStatus = (currentActionType == "進入決審") ? "決審核定" : "技術審查";;
                            StatusesName = "審核中";
                            return true;
                        case "3": // 技術審查 → 決審
                            fromStatus = "技術審查";
                            toStatus = "決審核定";
                            StatusesName = "核定中";
                            return true;
                        case "4": // 決審 → 計畫執行
                            fromStatus = "決審核定";
                            toStatus = "計畫執行";
                            StatusesName = "";
                            return true;
                    }
                    break;
                case "CUL": // 文化
                    switch (currentReviewType)
                    {
                        case "1": // 資格審查 → 初審
                            fromStatus = "資格審查";
                            toStatus = "初審";
                            return true;
                        case "2": // 初審 → 複審
                            fromStatus = "初審";
                            toStatus = (currentActionType == "進入決審") ? "決審核定" : "複審";
                            return true;
                        case "3": // 複審 → 決審
                            fromStatus = "複審";
                            toStatus = "決審核定";
                            return true;
                        case "4": // 決審核定 → 計畫執行
                            fromStatus = "決審核定";
                            toStatus = "計畫執行";
                            return true;
                    }
                    break;
                case "CLB": // 學校社團
                    switch (currentReviewType)
                    {
                        case "1": // 資格審查 → 實質審查
                            fromStatus = "資格審查";
                            toStatus = "決審核定";
                            StatusesName = "核定中";
                            return true;
                        case "4": // 決審 → 計畫執行
                            fromStatus = "決審核定";
                            toStatus = "計畫執行";
                            StatusesName = "";
                            return true;
                    }
                    break;
                case "EDC": // 學校民間
                case "MUL": // 多元
                case "LIT": // 素養
                case "ACC": // 無障礙
                    switch (currentReviewType)
                    {
                        case "1": // 資格審查 → 決審核定
                            fromStatus = "資格審查";
                            toStatus = "決審核定";
                            return true;
                        case "4": // 決審核定 → 計畫執行
                            fromStatus = "決審核定";
                            toStatus = "計畫執行";
                            return true;
                    }
                    break;
            }

            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得狀態轉換時發生錯誤: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 取得目前登入使用者資訊 (靜態版本)
    /// </summary>
    private static SessionHelper.UserInfoClass GetCurrentUserInfo()
    {
        try
        {
            return SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得使用者資訊時發生錯誤: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 提送至申請者功能（Type2 和 Type3）
    /// </summary>
    /// <param name="projectIds">專案編號列表</param>
    /// <param name="reviewType">審查類型 (2:實質審查, 3:技術審查)</param>
    /// <returns>處理結果</returns>
    [WebMethod]
    public static BatchApprovalResult SendToApplicant(List<string> projectIds, string reviewType)
    {
        var result = new BatchApprovalResult
        {
            ActionType = "提送至申請者",
            ReviewType = reviewType
        };

        try
        {
            // 權限檢查：只有 Type2(實質審查)、Type3(技術審查) 和 Type4(決審) 可以使用
            if (reviewType != "2" && reviewType != "3" && reviewType != "4")
            {
                result.Success = false;
                result.Message = "提送至申請者功能只適用於實質審查(Type2)、技術審查(Type3)和決審(Type4)";
                return result;
            }

            // 驗證參數
            if (projectIds == null || projectIds.Count == 0)
            {
                result.Success = false;
                result.Message = "未提供要處理的專案編號";
                result.ErrorMessages.Add("專案編號列表為空");
                return result;
            }

            // 取得當前使用者資訊
            var currentUser = GetCurrentUserInfo();
            if (currentUser == null)
            {
                result.Success = false;
                result.Message = "無法取得使用者資訊";
                result.ErrorMessages.Add("使用者未登入或 Session 已過期");
                return result;
            }

            // 記錄操作開始
            System.Diagnostics.Debug.WriteLine($"開始提送至申請者處理: reviewType={reviewType}, 專案數量={projectIds.Count}");

            // 1. 先將 ProjectID 根據補助類型分類
            var categorizedProjects = CategorizeProjectsByType(projectIds);

            // 2. 根據不同補助類型執行對應的查詢並組成 ProjectID + 帳號
            var projectAccountList = GetProjectAccountsByCategory(categorizedProjects);

            if (projectAccountList == null || projectAccountList.Count == 0)
            {
                result.Success = false;
                result.Message = "無法找到對應的計畫資料或申請者帳號";
                return result;
            }

            // 3. 使用 NotificationHelper.C2 寄信給申請者
            int successEmailCount = 0;
            var failedProjects = new List<string>();

            foreach (var item in projectAccountList)
            {
                string projectId = item.Key;
                string account = item.Value;

                try
                {
                    // 從 ProjectID 中提取補助類型
                    string subsidyType = ExtractSubsidyTypeFromProjectId(projectId);

                    // 取得計畫資訊（計畫名稱、類別名稱）
                    DataRow projectInfoRow = null;

                    if (subsidyType == "SCI")
                    {
                        projectInfoRow = ReviewCheckListHelper.GetSciProjectInfo(projectId);
                    }
                    else if (subsidyType == "CUL")
                    {
                        projectInfoRow = ReviewCheckListHelper.GetCulProjectInfo(projectId);
                    }
                    else
                    {
                        failedProjects.Add($"{projectId} (不支援的補助類型: {subsidyType})");
                        System.Diagnostics.Debug.WriteLine($"不支援的補助類型 - ProjectID: {projectId}, Type: {subsidyType}");
                        continue;
                    }

                    if (projectInfoRow != null)
                    {
                        string projectName = projectInfoRow["ProjectName"]?.ToString() ?? "";

                        if (!string.IsNullOrEmpty(projectName))
                        {
                            // 根據補助類型決定 categoryName
                            string categoryName = subsidyType == "SCI" ? "科專" : "文化";

                            // 使用 NotificationHelper.C2 寄信
                            NotificationHelper.C2(
                                categoryName,
                                projectName,
                                account
                            );

                            successEmailCount++;
                            System.Diagnostics.Debug.WriteLine($"已寄信給申請者 - ProjectID: {projectId}, Account: {account}, ProjectName: {projectName}");
                        }
                        else
                        {
                            failedProjects.Add($"{projectId} (計畫名稱為空)");
                            System.Diagnostics.Debug.WriteLine($"計畫名稱為空 - ProjectID: {projectId}");
                        }
                    }
                    else
                    {
                        failedProjects.Add($"{projectId} (無法取得計畫資訊)");
                        System.Diagnostics.Debug.WriteLine($"無法取得計畫資訊 - ProjectID: {projectId}");
                    }
                }
                catch (Exception ex)
                {
                    failedProjects.Add($"{projectId} (寄信失敗: {ex.Message})");
                    System.Diagnostics.Debug.WriteLine($"寄信給申請者時發生錯誤 - ProjectID: {projectId}, Error: {ex.Message}");
                }
            }

            // 設定處理結果
            result.Success = successEmailCount > 0;
            result.SuccessCount = successEmailCount;
            result.SuccessProjectIds = projectAccountList.Keys.Where(pid => !failedProjects.Any(f => f.StartsWith(pid))).ToList();

            if (failedProjects.Count > 0)
            {
                result.Message = $"成功提送 {successEmailCount} 筆計畫給申請者，{failedProjects.Count} 筆失敗";
                result.ErrorMessages.AddRange(failedProjects);
            }
            else
            {
                result.Message = $"成功提送 {successEmailCount} 筆計畫給申請者";
            }

            // 不再將詳細清單加入 ErrorMessages，前端只顯示筆數即可

            // 記錄操作結果
            System.Diagnostics.Debug.WriteLine($"提送至申請者處理完成: 成功={projectAccountList.Count}");

        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = "提送至申請者處理時發生系統錯誤";
            result.ErrorMessages.Add($"例外錯誤: {ex.Message}");

            // 記錄完整錯誤
            System.Diagnostics.Debug.WriteLine($"提送至申請者處理例外: {ex}");
        }

        return result;
    }


    /// <summary>
    /// 根據補助類型分類 ProjectID
    /// </summary>
    /// <param name="projectIds">專案編號列表</param>
    /// <returns>分類後的專案編號字典，Key為補助類型，Value為該類型的專案編號列表</returns>
    private static Dictionary<string, List<string>> CategorizeProjectsByType(List<string> projectIds)
    {
        var categorizedProjects = new Dictionary<string, List<string>>();

        try
        {
            foreach (string projectId in projectIds)
            {
                if (string.IsNullOrEmpty(projectId)) continue;

                // 從 ProjectID 中提取補助類型 (例如: 114SCI0005 -> SCI)
                string subsidyType = ExtractSubsidyTypeFromProjectId(projectId);

                if (!string.IsNullOrEmpty(subsidyType))
                {
                    if (!categorizedProjects.ContainsKey(subsidyType))
                    {
                        categorizedProjects[subsidyType] = new List<string>();
                    }
                    categorizedProjects[subsidyType].Add(projectId);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"無法從 ProjectID {projectId} 中識別補助類型");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"分類 ProjectID 時發生錯誤: {ex.Message}");
        }

        return categorizedProjects;
    }

    /// <summary>
    /// 從 ProjectID 中提取補助類型
    /// </summary>
    /// <param name="projectId">專案編號 (例如: 114SCI0005)</param>
    /// <returns>補助類型 (例如: SCI)</returns>
    private static string ExtractSubsidyTypeFromProjectId(string projectId)
    {
        if (string.IsNullOrEmpty(projectId)) return string.Empty;

        // 定義支援的補助類型
        string[] supportedTypes = { "SCI", "CUL", "EDC", "CLB", "MUL", "LIT", "ACC" };

        // 檢查 ProjectID 中是否包含任何支援的補助類型
        foreach (string type in supportedTypes)
        {
            if (projectId.ToUpper().Contains(type))
            {
                return type;
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// 根據分類後的專案編號取得對應的申請者帳號
    /// </summary>
    /// <param name="categorizedProjects">分類後的專案編號</param>
    /// <returns>專案編號與申請者帳號的對應字典</returns>
    private static Dictionary<string, string> GetProjectAccountsByCategory(Dictionary<string, List<string>> categorizedProjects)
    {
        var result = new Dictionary<string, string>();

        try
        {
            foreach (var category in categorizedProjects)
            {
                string subsidyType = category.Key;
                List<string> projectIds = category.Value;

                System.Diagnostics.Debug.WriteLine($"處理補助類型 {subsidyType}，專案數量: {projectIds.Count}");

                switch (subsidyType.ToUpper())
                {
                    case "SCI":
                        // 處理 SCI 補助類型：搜尋 OFS_SCI_Project_Main
                        var sciResults = GetSciProjectAccounts(projectIds);
                        foreach (var item in sciResults)
                        {
                            result[item.Key] = item.Value;
                        }
                        break;

                    case "CUL":
                        foreach (string projectId in projectIds)
                        {
                            result[projectId] = OFS_CulProjectHelper.getUserAccount(projectId);
                        }
                        break;

                    case "EDC":
                        foreach (string projectId in projectIds)
                        {
                            result[projectId] = OFS_EdcProjectHelper.getUserAccount(projectId);
                        }
                        break;

                    case "CLB":
                        break;

                    case "MUL":
                        foreach (string projectId in projectIds)
                        {
                            result[projectId] = OFS_MulProjectHelper.getUserAccount(projectId);
                        }
                        break;

                    case "LIT":
                        foreach (string projectId in projectIds)
                        {
                            result[projectId] = OFS_LitProjectHelper.getUserAccount(projectId);
                        }
                        break;

                    case "ACC":
                        foreach (string projectId in projectIds)
                        {
                            result[projectId] = OFS_AccProjectHelper.getUserAccount(projectId);
                        }
                        break;

                    default:
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"根據分類取得專案帳號時發生錯誤: {ex.Message}");
        }

        return result;
    }

    /// <summary>
    /// 取得 SCI 補助類型的專案申請者帳號
    /// </summary>
    /// <param name="sciProjectIds">SCI 專案編號列表</param>
    /// <returns>專案編號與申請者帳號的對應字典</returns>
    private static Dictionary<string, string> GetSciProjectAccounts(List<string> sciProjectIds)
    {
        var result = new Dictionary<string, string>();

        try
        {
            foreach (string projectId in sciProjectIds)
            {
                if (string.IsNullOrEmpty(projectId)) continue;

                try
                {
                    // 直接調用 helper 中的方法來取得申請者帳號
                    string userAccount = OFS_SciApplicationHelper.GetApplicantAccountByProjectId(projectId);

                    if (!string.IsNullOrEmpty(userAccount))
                    {
                        result[projectId] = userAccount;
                        System.Diagnostics.Debug.WriteLine($"SCI 專案 {projectId} 對應申請者帳號: {userAccount}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"SCI 專案 {projectId} 在 OFS_SCI_Project_Main 中沒有找到 UserAccount");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"取得 SCI 專案 {projectId} 申請者帳號時發生錯誤: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得 SCI 專案帳號時發生錯誤: {ex.Message}");
        }

        return result;
    }



    #endregion

    #region WebMethods

    /// <summary>
    /// 取得計畫詳細資料
    /// </summary>
    /// <param name="projectId">專案編號</param>
    /// <param name="reviewType">審查類型 (2:實質審查, 3:技術審查)</param>
    /// <returns>計畫詳細資料</returns>
    /// <summary>
    /// 匯出審查結果與意見回覆對照表
    /// </summary>
    /// <param name="projectId">專案編號</param>
    /// <param name="reviewType">審查類型</param>
    /// <returns>匯出結果</returns>
    [WebMethod]
    public static object ExportReviewCommentsComparison(string projectId, string reviewType)
    {
        try
        {
            string fileName = ReviewCheckListHelper.ExportReviewCommentsComparison(projectId, reviewType);
            return new { Success = true, FileName = fileName };
        }
        catch (Exception ex)
        {
            return new { Success = false, Message = ex.Message };
        }
    }

    [WebMethod]
    public static object GetPlanDetail(string projectId, string reviewType)
    {
        try
        {
            // 根據 reviewType 決定 ReviewStage
            string reviewStage = "";


            DataTable planData = null;
            DataTable reviewData = null;

            if (projectId.Contains("SCI"))
            {
                reviewStage = reviewType =="2"? "2" : "3";
               
                planData = ReviewCheckListHelper.GetSciPlanDetail(projectId);
                reviewData = ReviewCheckListHelper.GetSciReviewComments(projectId, reviewStage);
            }
            else if (projectId.Contains("CUL"))
            {
                planData = ReviewCheckListHelper.GetCulturalPlanDetail(projectId); //基本資訊
                reviewData = ReviewCheckListHelper.GetCulturalReviewComments(projectId, reviewType); //審查意見
            }

            if (planData != null && planData.Rows.Count > 0)
            {
                var planRow = planData.Rows[0];

                // 處理評審意見資料
                var reviewComments = new List<object>();
                if (reviewData != null && reviewData.Rows.Count > 0)
                {
                    foreach (DataRow reviewRow in reviewData.Rows)
                    {
                        reviewComments.Add(new
                        {
                            ReviewerName = reviewRow["ReviewerName"]?.ToString(),
                            TotalScore = reviewRow["TotalScore"]?.ToString(),
                            ReviewComment = reviewRow["ReviewComment"]?.ToString(),
                            ReplyComment = reviewRow["ReplyComment"]?.ToString()
                        });
                    }
                }

                return new
                {
                    Success = true,
                    Data = new
                    {
                        ProjectID = planRow["ProjectID"]?.ToString(),
                        Year = planRow["Year"]?.ToString(),
                        SubsidyPlanType = planRow["SubsidyPlanType"]?.ToString(),
                        ProjectNameTw = planRow["ProjectNameTw"]?.ToString(),
                        TopicField = planRow["TopicField"]?.ToString(),
                        OrgName = planRow["OrgName"]?.ToString(),
                        ReviewComments = reviewComments
                    }
                };
            }

            return new { Success = false, Message = "查無資料" };
        }
        catch (Exception ex)
        {
            return new { Success = false, Message = ex.Message };
        }
    }

    /// <summary>
    /// 根據類別取得審查組別選項 WebMethod for Type4
    /// </summary>
    /// <param name="category">類別代碼</param>
    /// <returns>JSON格式的審查組別選項清單</returns>
    [WebMethod]
    public static string GetReviewGroupOptionsByCategory(string category)
    {
        try
        {
            List<DropdownItem> options = new List<DropdownItem>();

            // 先加入「全部」選項
            options.Add(new DropdownItem { Text = "全部", Value = "" });

            // 如果是科專類別，取得科專審查組別選項
            if (category == "SCI")
            {
                var sciOptions = ReviewCheckListHelper.GetSciReviewGroupOptions();
                options.AddRange(sciOptions);
            }
            else if (category == "CUL")
            {
                var culOptions = ReviewCheckListHelper.GetCulReviewGroupOptions();
                options.AddRange(culOptions);
            }

            return JsonConvert.SerializeObject(new {
                Success = true,
                Options = options
            });
        }
        catch (Exception ex)
        {
            return JsonConvert.SerializeObject(new {
                Success = false,
                Message = ex.Message
            });
        }
    }

    #endregion

    #region 批次處理邏輯

    /// <summary>
    /// 批次更新專案狀態
    /// </summary>
    /// <param name="projectIds">專案編號列表</param>
    /// <param name="fromStatus">原始狀態</param>
    /// <param name="toStatus">目標狀態</param>
    /// <param name="StatusesName">狀態名稱</param>
    /// <param name="userAccount">操作者帳號</param>
    /// <param name="actionType">操作類型</param>
    /// <returns>批次處理結果</returns>
    public static BatchApprovalResult BatchUpdateProjectStatus(
        List<string> projectIds,
        string fromStatus,
        string toStatus,
        string StatusesName,
        string userName,
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

        try
        {
            int successCount = 0;
            var successIds = new List<string>();
            var errorMessages = new List<string>();

            foreach (string projectId in projectIds)
            {
                try
                {

                    // 根據補助案類型決定歷程記錄方式
                    if (projectId.Contains("SCI"))
                    {
                        // 科專
                        // 更新科專專案狀態
                        ReviewCheckListHelper.UpdateProjectStatusInDatabase(projectId, toStatus, userName, StatusesName);

                        // 如果是進入計畫執行階段，建立待辦事項模板
                        if (toStatus == "計畫執行")
                        {
                            ReviewCheckListHelper.CreateTaskQueueTemplate(projectId);
                        }

                        RecordSciReviewHistory(projectId, fromStatus, toStatus, actionType, userName);
                    }
                    else if (projectId.Contains("CUL"))
                    {
                        // 文化
                        int status = 5;
                        switch (toStatus)
                        {
                            case "初審":
                                status = 2;
                                break;
                            case "複審":
                                status = 3;
                                break;
                            case "決審核定":
                                status = 4;
                                break;
                        }

                        OFS_CulProjectHelper.updateProgressStatus(projectId, status);

                        if (toStatus == "計畫執行")
                        {
                            ReviewCheckListHelper.CreateTaskQueueTemplate(projectId);
                        }

                        RecordReviewHistory(projectId, fromStatus, toStatus, actionType, userName);
                    }
                    else if (projectId.Contains("EDC"))
                    {
                        // 學校民間
                        OFS_EdcProjectHelper.updateProgressStatus(projectId, (toStatus == "計畫執行") ? 5 : 4);

                        if (toStatus == "計畫執行")
                        {
                            ReviewCheckListHelper.CreateTaskQueueTemplate(projectId);
                        }

                        RecordReviewHistory(projectId, fromStatus, toStatus, actionType, userName);
                    }
                    else if (projectId.Contains("CLB"))
                    {
                        ReviewCheckListHelper.CLB_UpdateProjectStatusInDatabase(projectId, toStatus, userName, StatusesName);

                        // 如果是進入計畫執行階段，建立待辦事項模板
                        if (toStatus == "計畫執行")
                        {
                             ReviewCheckListHelper.CreateTaskQueueTemplate(projectId);
                        }
                    }
                    else if (projectId.Contains("MUL"))
                    {
                        // 多元
                        OFS_MulProjectHelper.updateProgressStatus(projectId, (toStatus == "計畫執行") ? 5 : 4);

                        if (toStatus == "計畫執行")
                        {
                            ReviewCheckListHelper.CreateTaskQueueTemplate(projectId);
                        }

                        RecordReviewHistory(projectId, fromStatus, toStatus, actionType, userName);
                    }
                    else if (projectId.Contains("LIT"))
                    {
                        // 素養
                        OFS_LitProjectHelper.updateProgressStatus(projectId, (toStatus == "計畫執行") ? 5 : 4);

                        if (toStatus == "計畫執行")
                        {
                            ReviewCheckListHelper.CreateTaskQueueTemplate(projectId);
                        }

                        RecordReviewHistory(projectId, fromStatus, toStatus, actionType, userName);
                    }
                    else if (projectId.Contains("ACC"))
                    {
                        // 無障礙
                        OFS_AccProjectHelper.updateProgressStatus(projectId, (toStatus == "計畫執行") ? 5 : 4);

                        if (toStatus == "計畫執行")
                        {
                            ReviewCheckListHelper.CreateTaskQueueTemplate(projectId);
                        }

                        RecordReviewHistory(projectId, fromStatus, toStatus, actionType, userName);
                    }

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

        return result;
    }

    /// <summary>
    /// 科專專用的審查歷程記錄
    /// </summary>
    private static void RecordSciReviewHistory(string projectId, string fromStatus, string toStatus,
        string actionType, string userAccount)
    {
        if (fromStatus != toStatus)
        {
            // 檢查是否為需要記錄兩筆歷程的特定轉換
            //目的:為了 先變成 已核定/通過 在進入
            if ((fromStatus == "實質審查" && toStatus == "技術審查") ||
                (fromStatus == "技術審查" && toStatus == "決審核定") ||
                (fromStatus == "決審核定" && toStatus == "計畫執行"))
            {
                // 科專特定轉換：記錄兩筆歷程
                // 第一筆：當前階段變為完成狀態
                string firstStageEndStatus = fromStatus == "決審核定" ? "已核定" : "通過";
                ReviewCheckListHelper.InsertReviewHistory(projectId, fromStatus, fromStatus, $"{fromStatus}{firstStageEndStatus}", userAccount);

                // 延遲1毫秒確保時間戳記不同
                System.Threading.Thread.Sleep(1);

                // 第二筆：轉為下一階段初始狀態
                string nextStageInitialStatus = toStatus == "計畫執行" ? "簽訂契約" : "審核中";
                ReviewCheckListHelper.InsertReviewHistory(projectId, fromStatus, toStatus, $"{toStatus}{nextStageInitialStatus}", userAccount);
            }
            else
            {
                // 僅記錄一次狀態變更歷程
                ReviewCheckListHelper.InsertReviewHistory(projectId, fromStatus, toStatus, actionType, userAccount);
            }
        }
        else
        {
            // 同階段內的狀態變更
            ReviewCheckListHelper.InsertReviewHistory(projectId, fromStatus, toStatus, actionType, userAccount);
        }
    }

    private static void RecordReviewHistory(string projectId, string fromStatus, string toStatus, string actionType, string userAccount)
    {
        if (fromStatus != "資格審查")
        {
            var statusName = fromStatus == "決審核定" ? "已核定" : "通過";

            ReviewCheckListHelper.InsertReviewHistory(projectId, fromStatus, $"{fromStatus}-{statusName}", actionType, userAccount);

            System.Threading.Thread.Sleep(1);
        }

        ReviewCheckListHelper.InsertReviewHistory(projectId, fromStatus, toStatus, actionType, userAccount);
    }



    /// <summary>
    /// 提送至申請者按鈕點擊事件
    /// 此為進入決審的寄信功能
    /// </summary>
    protected void btnSendToApplicant_Click(object sender, EventArgs e)
    {
        try
        {
            // 從前端取得選中的專案ID列表 (假設透過 HiddenField 或 Request.Form 傳遞)
            string selectedProjectIds = hdnSelectedProjectIds.Value;

            if (string.IsNullOrEmpty(selectedProjectIds))
            {
                ShowMessage("請先選擇要處理的案件", "warning");
                return;
            }

            // 將字串分割成專案ID列表
            var projectIds = selectedProjectIds.Split(',').Where(id => !string.IsNullOrEmpty(id)).ToList();

            if (projectIds.Count == 0)
            {
                ShowMessage("未選擇有效的案件", "warning");
                return;
            }

            // 取得當前使用者資訊
            var currentUser = GetCurrentUserInfo();
            if (currentUser == null)
            {
                ShowMessage("無法取得使用者資訊", "error");
                return;
            }

            int successCount = 0;
            int skippedCount = 0;
            var errorMessages = new List<string>();
            var skippedProjects = new List<string>();
            string Reason = "提送至申請者修正計畫書";
            string fromStatus = "核定中";
            string toStatus = "計畫書修正中";
            foreach (string projectId in projectIds)
            {
                // 取得專案類型
                string projectType = null;
                if (projectId.Contains("SCI")) projectType = "SCI";
                else if (projectId.Contains("CUL")) projectType = "CUL";
                else if (projectId.Contains("EDC")) projectType = "EDC";
                else if (projectId.Contains("CLB")) projectType = "CLB";
                else if (projectId.Contains("MUL")) projectType = "MUL";
                else if (projectId.Contains("LIT")) projectType = "LIT";
                else if (projectId.Contains("ACC")) projectType = "ACC";

                // 檢查是否為不支援的專案類型
                if (projectType == "EDC" || projectType == "CLB" || projectType == "LIT")
                {
                    skippedCount++;
                    string typeName = projectType == "EDC" ? "學校民間" :
                                     projectType == "CLB" ? "學校社團" : "素養";
                    skippedProjects.Add($"{projectId} ({typeName})");
                    continue;
                }

                // 準備計畫變更記錄 model
                var changeRecord = new ProjectChangeRecord
                {
                    Type = projectType,
                    Method = 2, // 2: 修正計畫書
                    DataID = projectId,
                    Reason = Reason,
                    Status = 1
                };

                try
                {
                    // 使用 switch 處理不同專案類型
                    switch (projectType)
                    {
                        case "SCI":
                            // 更新專案狀態名稱為「計畫書修正中」
                            ReviewCheckListHelper.SCI_UpdateProjectStatusName(projectId, toStatus);
                            // 記錄歷程：核定中 → 計畫書修正中
                            ReviewCheckListHelper.InsertReviewHistory(projectId, fromStatus, toStatus, Reason, currentUser.UserName);
                            // 插入計畫變更記錄
                            OFSProjectChangeRecordHelper.insert(changeRecord);
                            break;

                        case "CUL":
                            // 文化
                            OFS_CulProjectHelper.updateStatus(projectId, 42); //計畫書修正中
                            ReviewCheckListHelper.InsertReviewHistory(projectId, "核定中", "計畫書修正中", "提送至申請者", currentUser.Account);
                            OFSProjectChangeRecordHelper.insert(changeRecord);
                            break;

                        case "MUL":
                            // 多元
                            OFS_MulProjectHelper.updateStatus(projectId, 42); //計畫書修正中
                            ReviewCheckListHelper.InsertReviewHistory(projectId, "核定中", "計畫書修正中", "提送至申請者", currentUser.Account);
                            OFSProjectChangeRecordHelper.insert(changeRecord);
                            break;

                        case "ACC":
                            OFS_AccProjectHelper.updateStatus(projectId, 42); //計畫書修正中
                            ReviewCheckListHelper.InsertReviewHistory(projectId, "核定中", "計畫書修正中", "提送至申請者", currentUser.Account);
                            OFSProjectChangeRecordHelper.insert(changeRecord);
                            break;
                    }

                    // 使用 NotificationHelper.E1 寄信給申請者
                    try
                    {
                        string applicantEmail = "";
                        string projectName = "";
                        string categoryName = "";

                        // 根據專案類型轉換類別名稱
                        switch (projectType)
                        {
                            case "SCI":
                                categoryName = "科專";
                                applicantEmail = OFS_SciApplicationHelper.GetApplicantAccountByProjectId(projectId);
                                var sciProjectInfo = ReviewCheckListHelper.GetSciProjectInfo(projectId);
                                if (sciProjectInfo != null)
                                {
                                    projectName = sciProjectInfo["ProjectName"]?.ToString() ?? "";
                                }
                                break;

                            case "CUL":
                                categoryName = "文化";
                                applicantEmail = OFS_CulProjectHelper.getUserAccount(projectId);
                                var culProjectInfo = ReviewCheckListHelper.GetCulProjectInfo(projectId);
                                if (culProjectInfo != null)
                                {
                                    projectName = culProjectInfo["ProjectName"]?.ToString() ?? "";
                                }
                                break;

                            case "MUL":
                                categoryName = "多元";
                                applicantEmail = OFS_MulProjectHelper.getUserAccount(projectId);
                                var mulProject = OFS_MulProjectHelper.query(new OFS_MulProject { ProjectID = projectId }, false).FirstOrDefault();
                                if (mulProject != null)
                                {
                                    projectName = mulProject.ProjectName ?? "";
                                }
                                break;

                            case "ACC":
                                categoryName = "無障礙";
                                applicantEmail = OFS_AccProjectHelper.getUserAccount(projectId);
                                var accProject = OFS_AccProjectHelper.query(new OFS_AccProject { ProjectID = projectId }, false).FirstOrDefault();
                                if (accProject != null)
                                {
                                    projectName = accProject.ProjectName ?? "";
                                }
                                break;
                        }

                        // 如果有取得申請者Email，則使用 E1 發送郵件
                        if (!string.IsNullOrEmpty(applicantEmail) && !string.IsNullOrEmpty(projectName))
                        {
                            NotificationHelper.E1(categoryName, projectName, applicantEmail);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"專案 {projectId} 無法取得申請者Email或計畫名稱");
                        }
                    }
                    catch (Exception emailEx)
                    {
                        // 郵件發送失敗不影響主流程，僅記錄錯誤
                        System.Diagnostics.Debug.WriteLine($"專案 {projectId} 寄信時發生錯誤: {emailEx.Message}");
                    }

                    successCount++;
                }
                catch (Exception ex)
                {
                    errorMessages.Add($"處理專案 {projectId} 時發生錯誤: {ex.Message}");
                }
            }

            // 顯示處理結果
            if (successCount > 0)
            {
                string message = $"成功處理 {successCount} 件計畫";
                if (skippedCount > 0)
                {
                    message += $"\\n已跳過 {skippedCount} 件不支援的案件類型";
                }
                if (errorMessages.Count > 0)
                {
                    message += $"\\n處理失敗 {errorMessages.Count} 件";
                }
                ShowMessage(message, "success");
            }
            else if (skippedCount > 0 && errorMessages.Count == 0)
            {
                // 全部都是跳過的案件，沒有錯誤
                string message = "所選案件類型目前不支援修正計畫書（學校民間類、學校社團類、素養類）";
                ShowMessage(message, "warning");
            }
            else
            {
                // 有錯誤但沒有成功的案件
                string message = "所有案件處理失敗";
                if (skippedCount > 0)
                {
                    message += $"\\n其中 {skippedCount} 件為不支援的案件類型";
                }
                ShowMessage(message, "error");
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "提送至申請者時發生錯誤");
            ShowMessage("系統發生錯誤，請稍後再試", "error");
        }
    }

    /// <summary>
    /// 顯示訊息給使用者
    /// </summary>
    /// <param name="message">訊息內容</param>
    /// <param name="type">訊息類型 (success, warning, error)</param>
    private void ShowMessage(string message, string type)
    {
        string icon = type == "success" ? "success" : type == "warning" ? "warning" : "error";

        string script = $@"
            Swal.fire({{
                title: '提送至申請者',
                text: '{message}',
                icon: '{icon}',
                confirmButtonText: '確定'
            }});
        ";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowMessage", script, true);
    }


    #endregion

    #region Type1 搜尋方法

    /// <summary>
    /// AJAX Type1 搜尋方法
    /// </summary>
    [WebMethod]
    public static string AjaxSearch_Type1(string year, string category, string status, string orgName, string supervisor, string keyword)
    {
        try
        {
            List<ReviewChecklistItem> results = new List<ReviewChecklistItem>();
            results = ReviewCheckListHelper.Search_Type1(
                year,
                status,
                orgName,
                supervisor,
                keyword,
                category
                );



            return JsonConvert.SerializeObject(new
            {
                success = true,
                data = results,
                count = results.Count,
                message = $"成功搜尋到 {results.Count} 筆資料"
            });
        }
        catch (Exception ex)
        {
            return JsonConvert.SerializeObject(new
            {
                success = false,
                message = "搜尋時發生錯誤: " + ex.Message,
                data = new List<ReviewChecklistItem>(),
                count = 0
            });
        }
    }
    #endregion
    #region Type2 搜尋方法



    /// <summary>
    /// AJAX Type2 搜尋方法
    /// </summary>
    [WebMethod]
    public static string AjaxSearch_Type2(string year, string category, string progress, string replyStatus, string orgName, string supervisor, string keyword)
    {
        try
        {
            List<ReviewChecklistItem> results = new List<ReviewChecklistItem>();

            // 執行實質審查查詢
            if (category == "SCI" || string.IsNullOrEmpty(category))
            {
                results.AddRange(GetSciProjectData(
                    year,
                    orgName,
                    supervisor,
                    keyword,
                    progress,
                    replyStatus,
                    "實質審查"
                ));
            }
            if (category == "CUL" || string.IsNullOrEmpty(category))
            {
                results.AddRange(GetCulProjectData(
                    year,
                    orgName,
                    supervisor,
                    keyword,
                    progress,
                    replyStatus,
                    "初審"
                ));
            }
            return JsonConvert.SerializeObject(new
            {
                success = true,
                data = results,
                count = results.Count,
                message = $"成功搜尋到 {results.Count} 筆資料"
            });
        }
        catch (Exception ex)
        {
            return JsonConvert.SerializeObject(new
            {
                success = false,
                message = "搜尋時發生錯誤: " + ex.Message,
                data = new List<ReviewChecklistItem>(),
                count = 0
            });
        }
    }
 public static List<ReviewChecklistItem> GetSciProjectData(
        string year,
        string orgName,
        string supervisor,
        string keyword,
        string reviewProgress,
        string replyProgress,
        string status )
    {
        // 1. 先取得科專的基本專案資料
        var basicProjects = ReviewCheckListHelper.GetSciBasicData( year, orgName, supervisor, keyword, status);

        if (basicProjects.Count == 0)
        {
            return new List<ReviewChecklistItem>();
        }

        // 2. 取得進度資料
        var projectIds = basicProjects.Select(p => p.ProjectID).ToList();
        var progressData = ReviewCheckListHelper.GetSciProgressData(projectIds, status);

        // 3. 取得審查組別資料
        var reviewGroupData = ReviewCheckListHelper.GetSciReviewGroupData(projectIds);

        // 4. 組合結果
        List<ReviewChecklistItem> result = new List<ReviewChecklistItem>();

        foreach (var project in basicProjects)
        {
            var progress = progressData.FirstOrDefault(p => p.ProjectID == project.ProjectID);
            var reviewGroup = reviewGroupData.FirstOrDefault(r => r.ProjectID == project.ProjectID);

            var item = new ReviewChecklistItem
            {
                // 基本資訊
                ProjectID = project.ProjectID,
                ProjectNameTw = project.ProjectNameTw,
                UserOrg = project.UserOrg,
                OrgName = project.OrgName,
                // 經費與狀態
                Req_SubsidyAmount = project.Req_SubsidyAmount,
                StatusesName = project.StatusesName,
                ExpirationDate = project.ExpirationDate,

                // 承辦人
                SupervisoryPersonName = project.SupervisoryPersonName,

                // 年度與補助類別
                Year = project.Year,
                SubsidyPlanType = project.SubsidyPlanType,
                //ReviewProgress,ReplyProgress

                ReviewProgress = progress?.ReviewProgress,
                ReplyProgress = progress?.ReplyProgress,

                // 前端顯示進度用
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
                    if (reviewProgress == "完成" && item.ReviewProgress != "完成")
                        includeItem = false;
                    else if (reviewProgress == "未完成" && item.ReviewProgress != "未完成")
                        includeItem = false;
                }

                // 回覆進度篩選
                if (!string.IsNullOrEmpty(replyProgress) && includeItem)
                {
                    if (replyProgress == "完成" && item.ReplyProgress != "完成")
                        includeItem = false;
                    else if (replyProgress == "未完成" && item.ReplyProgress != "未完成")
                        includeItem = false;
                }

                return includeItem;
            }).ToList();
        }

        // 按照更新時間和專案編號排序
        return result.OrderByDescending(r => r.updated_at).ThenByDescending(r => r.ProjectID).ToList();
    }
 public static List<ReviewChecklistItem> GetCulProjectData(
        string year,
        string orgName,
        string supervisor,
        string keyword,
        string reviewProgress,
        string replyProgress,
        string status)
    {
        // 1. 先取得文化的基本專案資料
        var basicProjects = ReviewCheckListHelper.GetCulBasicData(year, orgName, supervisor, keyword, status);

        if (basicProjects.Count == 0)
        {
            return basicProjects;
        }

        // 2. 取得進度資料
        var projectIds = basicProjects.Select(p => p.ProjectID).ToList();
        var progressData = ReviewCheckListHelper.GetCulProgressData(projectIds, status);

        // 3. 取得審查組別資料
        var reviewGroupData = ReviewCheckListHelper.GetCulReviewGroupData(projectIds);

        // 4. 組合結果
        var result = new List<ReviewChecklistItem>();

        foreach (var project in basicProjects)
        {
            var progress = progressData.FirstOrDefault(p => p.ProjectID == project.ProjectID);
            var reviewGroup = reviewGroupData.FirstOrDefault(r => r.ProjectID == project.ProjectID);

            result.Add(new ReviewChecklistItem
            {
                // 基本資訊
                ProjectID = project.ProjectID,
                ProjectNameTw = project.ProjectNameTw,
                UserOrg = project.UserOrg,

                // 經費與狀態
                Req_SubsidyAmount = project.Req_SubsidyAmount,
                StatusesName = project.StatusesName,
                ExpirationDate = project.ExpirationDate,

                // 承辦人
                SupervisoryPersonName = project.SupervisoryPersonName,

                // 年度與補助類別
                Year = project.Year,
                SubsidyPlanType = project.SubsidyPlanType,
                //ReviewProgress,ReplyProgress

                ReviewProgress = progress?.ReviewProgress,
                ReplyProgress = progress?.ReplyProgress,

                // 前端顯示進度用
                ReviewProgressDisplay = progress?.ReviewProgressDisplay,
                ReplyProgressDisplay = progress?.ReplyProgressDisplay,
                Field_Descname = reviewGroup?.Field_Descname // 審查組別
            });
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
                    if (reviewProgress == "完成" && item.ReviewProgress != "完成")
                        includeItem = false;
                    else if (reviewProgress == "未完成" && item.ReviewProgress != "未完成")
                        includeItem = false;
                }

                // 回覆進度篩選
                if (!string.IsNullOrEmpty(replyProgress) && includeItem)
                {
                    if (replyProgress == "完成" && item.ReplyProgress != "完成")
                        includeItem = false;
                    else if (replyProgress == "未完成" && item.ReplyProgress != "未完成")
                        includeItem = false;
                }

                return includeItem;
            }).ToList();
        }

        // 按照更新時間和專案編號排序
        return result.OrderByDescending(r => r.updated_at).ThenByDescending(r => r.ProjectID).ToList();
    }
    #endregion
    #region Type3 搜尋方法
    /// <summary>
    /// AJAX Type3 搜尋方法
    /// </summary>
    [WebMethod]
    public static string AjaxSearch_Type3(string year, string category, string progress, string replyStatus, string orgName, string supervisor, string keyword)
    {
        try
        {
            List<ReviewChecklistItem> results = new List<ReviewChecklistItem>();

            // 執行技術審查查詢
            if (category == "SCI" || string.IsNullOrEmpty(category))
            {
                results.AddRange( GetSciProjectData(
                    year,
                    orgName,
                    supervisor,
                    keyword,
                    progress,
                    replyStatus,"技術審查"));
            }
            if (category == "CUL"|| string.IsNullOrEmpty(category))
            {
                results.AddRange( GetCulProjectData(
                    year,
                    orgName,
                    supervisor,
                    keyword,
                    progress,
                    replyStatus,
                    "複審"));
            }

            return JsonConvert.SerializeObject(new
            {
                success = true,
                data = results,
                count = results.Count,
                message = $"成功搜尋到 {results.Count} 筆資料"
            });
        }
        catch (Exception ex)
        {
            return JsonConvert.SerializeObject(new
            {
                success = false,
                message = "搜尋時發生錯誤: " + ex.Message,
                data = new List<ReviewChecklistItem>(),
                count = 0
            });
        }
    }

    #endregion
    #region Type5 搜尋方法

    /// <summary>
    /// AJAX Type5 搜尋方法 - 計畫變更審核
    /// </summary>
    [WebMethod]
    public static string AjaxSearch_Type5(string year, string category, string orgName, string supervisoryUnit, string keyword)
    {
        try
        {
            // 執行計畫變更審核查詢（前端分頁）
            List<PlanChangeReviewItem> results = ReviewCheckListHelper.Search_Type5_PlanChangeReview(
                year,
                category,
                orgName,
                supervisoryUnit,
                keyword
            );

            return JsonConvert.SerializeObject(new
            {
                success = true,
                data = results,
                count = results.Count,
                message = $"成功搜尋到 {results.Count} 筆資料"
            });
        }
        catch (Exception ex)
        {
            return JsonConvert.SerializeObject(new
            {
                success = false,
                message = "搜尋時發生錯誤: " + ex.Message,
                data = new List<PlanChangeReviewItem>(),
                count = 0
            });
        }
    }

    /// <summary>
    /// AJAX 搜尋 Type=6 執行計畫審核資料
    /// </summary>
    /// <param name="year">年度</param>
    /// <param name="category">類別</param>
    /// <param name="orgName">申請單位</param>
    /// <param name="supervisoryUnit">主管單位</param>
    /// <param name="keyword">關鍵字</param>
    /// <returns>JSON格式的搜尋結果</returns>
    [WebMethod]
    public static string AjaxSearch_Type6(string year, string category, string orgName, string supervisoryUnit, string keyword)
    {
        try
        {
            // 執行計畫審核查詢（前端分頁）
            List<ExecutionPlanReviewItem> results = ReviewCheckListHelper.Search_Type6_ExecutionPlanReview(
                year,
                category,
                orgName,
                supervisoryUnit,
                keyword
            );

            return JsonConvert.SerializeObject(new
            {
                success = true,
                data = results,
                count = results.Count,
                message = $"成功搜尋到 {results.Count} 筆資料"
            });
        }
        catch (Exception ex)
        {
            return JsonConvert.SerializeObject(new
            {
                success = false,
                message = "搜尋時發生錯誤: " + ex.Message,
                data = new List<ExecutionPlanReviewItem>(),
                count = 0
            });
        }
    }

    #endregion

    #region 審查排名功能

    /// <summary>
    /// 取得審查結果排名
    /// </summary>
    /// <param name="reviewType">審查類型 (2: 實質審查, 3: 技術審查)</param>
    /// <param name="reviewGroup">審查組別 (如: Information, Environment 等)</param>
    /// <returns>JSON格式的排名資料</returns>
    [WebMethod]
    public static string GetReviewRanking(string reviewType, string reviewGroup = null)
    {
        try
        {
            // 使用Helper方法取得排名資料
            var results = ReviewCheckListHelper.GetReviewRanking(reviewType, reviewGroup);

            return JsonConvert.SerializeObject(new
            {
                success = true,
                data = results,
                count = results.Count,
                message = $"成功載入 {results.Count} 個專案的排名資料"
            });
        }
        catch (Exception ex)
        {
            return JsonConvert.SerializeObject(new
            {
                success = false,
                message = ex.Message,
                data = new List<ReviewRankingItem>()
            });
        }
    }

    /// <summary>
    /// 匯出所有審查結果到 XLSX 檔案
    /// </summary>
    /// <param name="exportType">匯出類型 (Type2 或 Type3)</param>
    /// <returns>包含檔案資料的 JSON 回應</returns>
    [WebMethod]
    public static string ExportAllReviewResults(string exportType = "Type2")
    {
        try
        {
            // 根據 exportType 設定審查階段
            string sciType, culType;
            if (exportType == "Type3")
            {
                sciType = "3";
                culType = "3";//複審
            }
            else // Type2 或預設值
            {
                sciType = "2";
                culType = "2";//初審
            }

            // 匯出所有審查資料的請求列表
            var exportRequests = new List<ReviewExportRequest>();

            // 科專 (SCI)
            exportRequests.Add(new ReviewExportRequest
            {
                GrantType = "SCI",
                ReviewStage = sciType,
                Fields = new List<string>
                {
                    "Information",
                    "Environment",
                    "Material",
                    "Mechanical"
                }
            });

            // 文化 (CUL)
            exportRequests.Add(new ReviewExportRequest
            {
                GrantType = "CUL",
                ReviewStage = culType,
                Fields = new List<string>
                {
                    "11", "12", "13",
                    "21","22",
                    "31","32"
                }
            });

            // 執行匯出 - 合併所有類型的資料
            byte[] fileBytes = OFS_ReviewResultExportHelper.ExportAllReviewResultsToXlsx(exportRequests);

            if (fileBytes == null || fileBytes.Length == 0)
            {
                return JsonConvert.SerializeObject(new
                {
                    success = false,
                    message = "匯出檔案為空或產生失敗"
                });
            }

            // 生成檔案名稱
            string fileName = $"審查結果排名_{DateTime.Now:yyyyMMdd}.xlsx";

            // 將檔案轉為 Base64 字符串以便傳輸
            string base64File = Convert.ToBase64String(fileBytes);

            return JsonConvert.SerializeObject(new
            {
                success = true,
                message = "審查結果匯出成功",
                fileName = fileName,
                fileData = base64File,
                fileSize = fileBytes.Length
            });
        }
        catch (Exception ex)
        {
            return JsonConvert.SerializeObject(new
            {
                success = false,
                message = $"匯出審查結果時發生錯誤: {ex.Message}"
            });
        }
    }

    #endregion

    #region Type4 匯出功能

    /// <summary>
    /// 匯出Type4列表資料到Excel
    /// </summary>
    /// <param name="year">年度</param>
    /// <param name="category">類別</param>
    /// <param name="reviewGroup">審查組別</param>
    /// <param name="orgName">申請單位</param>
    /// <param name="supervisor">承辦人員</param>
    /// <param name="keyword">關鍵字</param>
    /// <returns>包含檔案資料的 JSON 回應</returns>
    [WebMethod]
    public static string ExportType4ListData(string year, string category, string reviewGroup, string orgName, string supervisor, string keyword)
    {
        try
        {
            // 執行資料查詢 - 使用與搜尋相同的邏輯但查詢所有欄位
            var results = ReviewCheckListHelper.SearchForExport_Type4(
                year,
                orgName,
                supervisor,
                keyword,
                category,
                reviewGroup);

            if (results == null || results.Count == 0)
            {
                return JsonConvert.SerializeObject(new
                {
                    success = false,
                    message = "查無符合條件的資料可供匯出"
                });
            }

            // 生成Excel檔案
            byte[] fileBytes = ReviewCheckListHelper.GenerateType4ExcelFile(results);

            if (fileBytes == null || fileBytes.Length == 0)
            {
                return JsonConvert.SerializeObject(new
                {
                    success = false,
                    message = "Excel檔案生成失敗"
                });
            }

            // 生成檔案名稱
            string fileName = $"決審核定列表_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            // 將檔案轉為 Base64 字符串以便傳輸
            string base64File = Convert.ToBase64String(fileBytes);

            return JsonConvert.SerializeObject(new
            {
                success = true,
                message = $"成功匯出 {results.Count} 筆資料",
                fileName = fileName,
                fileData = base64File,
                fileSize = fileBytes.Length,
                recordCount = results.Count
            });
        }
        catch (Exception ex)
        {
            return JsonConvert.SerializeObject(new
            {
                success = false,
                message = $"匯出時發生錯誤: {ex.Message}"
            });
        }
    }

    #endregion

    #region 審查通知功能

    /// <summary>
    /// 發送審查通知給審查人員
    /// </summary>
    /// <param name="projectIds">專案ID清單</param>
    /// <param name="reviewType">審查類型 (1:資格審查, 2:實質審查)</param>
    private static void SendReviewNotifications(List<string> projectIds, string reviewType)
    {
        try
        {
            if (projectIds == null || projectIds.Count == 0)
                return;

            // 依補助類型分組專案
            //科專、文化 --> 領域審核、初審 寄信
            var sciProjects = projectIds.Where(p => p.Contains("SCI")).ToList();
            var culProjects = projectIds.Where(p => p.Contains("CUL")).ToList();
          
            // 處理科專審查通知
            if (sciProjects.Count > 0)
            {
                SendSciReviewNotifications(sciProjects, reviewType);
            }

            // 處理文化審查通知
            if (culProjects.Count > 0)
            {
                SendCulReviewNotifications(culProjects, reviewType);
            }

         

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"發送審查通知時發生錯誤：{ex.Message}");
        }
    }

    /// <summary>
    /// 發送科專審查通知
    /// </summary>
    private static void SendSciReviewNotifications(List<string> projectIds, string reviewType)
    {
        try
        {
            // 取得審查階段與事件名稱
            string reviewStage = GetReviewStageByType(reviewType);
            string eventName = GetSCIEventNameByReviewType(reviewType);

            // 從資料庫取得審查記錄
            var reviewRecords = GetReviewRecordsByProjects(projectIds, reviewStage);
            if (reviewRecords.Count == 0)
                return;

            // ===== 1. 發送 C1 通知給審查委員 =====
            var reviewerGroups = reviewRecords
                .Where(r => !string.IsNullOrEmpty(r.Email))
                .GroupBy(r => r.Email);

            foreach (var reviewerGroup in reviewerGroups)
            {
                var projectList = new Dictionary<string, string>();

                foreach (var record in reviewerGroup)
                {
                    string url = BuildReviewUrl(record.ProjectID, record.Token);
                    projectList[record.ProjectName] = url;
                }

                NotificationHelper.C1("科專", projectList, eventName, reviewerGroup.Key);
            }

            System.Diagnostics.Debug.WriteLine($"已發送 C1 審查通知給 {reviewerGroups.Count()} 位審查委員");

            // ===== 2. 發送 E32 通知給申請者（實質審查→技術審查）=====
            if (reviewType == "2")
            {
                int SuccessCount = 0;

                foreach (string projectId in projectIds)
                {
                    try
                    {
                        string applicantEmail = OFS_SciApplicationHelper.GetApplicantAccountByProjectId(projectId);
                        var projectInfo = ReviewCheckListHelper.GetSciProjectInfo(projectId);
                        string projectName = projectInfo?["ProjectName"]?.ToString() ?? "";

                        if (!string.IsNullOrEmpty(applicantEmail) && !string.IsNullOrEmpty(projectName))
                        {
                            NotificationHelper.E32("科專", projectName, "實質審查", "技術審查", applicantEmail);
                            SuccessCount++;
                        }
                    }
                    catch (Exception c3Ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"專案 {projectId} 發送 E32 通知失敗：{c3Ex.Message}");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"已發送 E32 通知給 {SuccessCount} 位申請者");
            }
            else //資格審查-->實質審查
            {
                int SuccessCount = 0;

                foreach (string projectId in projectIds)
                {
                    try
                    {
                        string applicantEmail = OFS_SciApplicationHelper.GetApplicantAccountByProjectId(projectId);
                        var projectInfo = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectId);
                        string projectName = projectInfo.ProjectNameTw;
                        int year = projectInfo.Year??0;
                        if (!string.IsNullOrEmpty(applicantEmail) && !string.IsNullOrEmpty(projectName))
                        {
                            NotificationHelper.E31("科專", year, projectName, "資格審查", applicantEmail);
                            SuccessCount++;
                        }
                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"專案 {projectId} 發送 E32 通知失敗：{Ex.Message}");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"已發送 E32 通知給 {SuccessCount} 位申請者");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"發送科專審查通知時發生錯誤：{ex.Message}");
        }
    }

    /// <summary>
    /// 發送文化審查通知
    /// </summary>
    private static void SendCulReviewNotifications(List<string> projectIds, string reviewType)
    {
        try
        {
            // 取得審查階段
            string reviewStage = GetReviewStageByType(reviewType);
            if (string.IsNullOrEmpty(reviewStage))
                return;

            // 取得事件名稱
            string eventName = GetCULEventNameByReviewType(reviewType);

            // 從資料庫取得審查記錄
            var reviewRecords = GetReviewRecordsByProjects(projectIds, reviewStage);

            if (reviewRecords.Count == 0)
                return;

            // 依照審查人員的 Email 分組
            var reviewerGroups = reviewRecords
                .Where(r => !string.IsNullOrEmpty(r.Email))
                .GroupBy(r => r.Email);

            // 對每個審查人員發送通知
            foreach (var reviewerGroup in reviewerGroups)
            {
                string reviewerEmail = reviewerGroup.Key;

                // 組成該審查人員的專案清單 (ProjectName -> URL)
                var projectList = new Dictionary<string, string>();

                foreach (var record in reviewerGroup)
                {
                    string url = BuildReviewUrl(record.ProjectID, record.Token);
                    projectList[record.ProjectName] = url;
                }

                // 發送 C1 通知
                NotificationHelper.C1("文化", projectList, eventName, reviewerEmail);
            }

            System.Diagnostics.Debug.WriteLine($"已發送文化審查通知給 {reviewerGroups.Count()} 位審查人員");

            // 當 reviewType == "2" 時（初審進入複審），發送 E32 通知給申請者
            //當 reviewType == "3" 時（複審進入決審），發送 E31 通知給申請者
            if (reviewType == "2")
            {
                foreach (string projectId in projectIds)
                {
                    try
                    {
                        string applicantEmail = OFS_CulProjectHelper.getUserAccount(projectId);
                        var culProjectInfo = ReviewCheckListHelper.GetCulProjectInfo(projectId);
                        string projectName = culProjectInfo != null ? culProjectInfo["ProjectName"]?.ToString() ?? "" : "";

                        if (!string.IsNullOrEmpty(applicantEmail) && !string.IsNullOrEmpty(projectName))
                        {
                            NotificationHelper.E32("文化", projectName, "初審", "複審", applicantEmail);
                        }
                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"發送 E32 通知給專案 {projectId} 時發生錯誤：{Ex.Message}");
                    }
                }
            }
            else
            {
                foreach (string projectId in projectIds)
                {
                    try
                    {
                        string applicantEmail = OFS_CulProjectHelper.getUserAccount(projectId);
                        var culProjectInfo = ReviewCheckListHelper.GetCulProjectInfo(projectId);
                        string projectName = culProjectInfo != null ? culProjectInfo["ProjectName"]?.ToString() ?? "" : "";
                        int year = culProjectInfo != null ? Convert.ToInt32(culProjectInfo["Year"]) : 0;

                        if (!string.IsNullOrEmpty(applicantEmail) && !string.IsNullOrEmpty(projectName))
                        {
                            NotificationHelper.E31("文化", year, projectName,  "複審", applicantEmail);
                        }
                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"發送 E32 通知給專案 {projectId} 時發生錯誤：{Ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"發送文化審查通知時發生錯誤：{ex.Message}");
        }
    }

  

    /// <summary>
    /// 發送進入決審通知（SCI 和 CUL）
    /// 技術審查通過後進入決審，寄信通知申請者，使用 G5 通知
    /// </summary>
    private static void SendFinalReviewNotifications(List<string> projectIds)
    {
        try
        {
            if (projectIds == null || projectIds.Count == 0)
                return;

            // 事件名稱固定為「技術審查」
            string eventName = "技術審查";

            // 依補助類型分組專案（只處理 SCI 和 CUL）
            var sciProjects = projectIds.Where(p => p.Contains("SCI")).ToList();
            var culProjects = projectIds.Where(p => p.Contains("CUL")).ToList();

            // 處理科專專案
            if (sciProjects.Count > 0)
            {
                SendFinalReviewNotificationsForGrantType(sciProjects, "SCI", "科專", eventName);
            }

            // 處理文化專案
            if (culProjects.Count > 0)
            {
                SendFinalReviewNotificationsForGrantType(culProjects, "CUL", "文化", eventName);
            }

            System.Diagnostics.Debug.WriteLine($"已完成發送進入決審通知，共 {projectIds.Count} 個專案");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"發送進入決審通知時發生錯誤：{ex.Message}");
        }
    }

    /// <summary>
    /// 發送特定補助類型的進入決審通知
    /// </summary>
    private static void SendFinalReviewNotificationsForGrantType(List<string> projectIds, string subsidyType, string grantTypeName, string eventName)
    {
        foreach (string projectId in projectIds)
        {
            try
            {
                // 取得專案名稱和使用者帳號
                var projectInfo = GetProjectInfoForFinalReview(projectId, subsidyType);

                if (projectInfo != null &&
                    !string.IsNullOrEmpty(projectInfo.ProjectName) &&
                    !string.IsNullOrEmpty(projectInfo.UserAccount))
                {
                    // 使用 G5 寄信通知申請者
                    NotificationHelper.G5(grantTypeName, projectInfo.ProjectName, eventName, projectInfo.UserAccount);

                    System.Diagnostics.Debug.WriteLine(
                        $"已發送 {grantTypeName} 進入決審通知 - ProjectID: {projectId}, " +
                        $"ProjectName: {projectInfo.ProjectName}, UserAccount: {projectInfo.UserAccount}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"無法取得專案資訊或使用者帳號 - ProjectID: {projectId}, SubsidyType: {subsidyType}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"發送 {grantTypeName} 進入決審通知時發生錯誤 - ProjectID: {projectId}, 錯誤: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// 取得進入決審專案資訊
    /// </summary>
    private static ProjectNotificationInfo GetProjectInfoForFinalReview(string projectId, string subsidyType)
    {
        try
        {
            using (var db = new GS.Data.Sql.DbHelper())
            {
                if (subsidyType?.ToUpper() == "SCI")
                {
                    db.CommandText = @"
                        SELECT
                            AM.ProjectNameTw AS ProjectName,
                            PM.UserAccount
                        FROM OFS_SCI_Application_Main AM
                        INNER JOIN OFS_SCI_Project_Main PM ON AM.ProjectID = PM.ProjectID
                        WHERE AM.ProjectID = @ProjectID
                    ";
                }
                else if (subsidyType?.ToUpper() == "CUL")
                {
                    db.CommandText = @"
                        SELECT
                            ProjectName,
                            UserAccount
                        FROM OFS_CUL_Project
                        WHERE ProjectID = @ProjectID
                    ";
                }
                else
                {
                    return null;
                }

                db.Parameters.Add("@ProjectID", projectId);

                var table = db.GetTable();

                if (table.Rows.Count > 0)
                {
                    var row = table.Rows[0];
                    return new ProjectNotificationInfo
                    {
                        ProjectID = projectId,
                        ProjectName = row["ProjectName"]?.ToString() ?? "",
                        UserAccount = row["UserAccount"]?.ToString() ?? ""
                    };
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得進入決審專案資訊時發生錯誤 - ProjectID: {projectId}, 錯誤: {ex.Message}");
        }

        return null;
    }

    /// <summary>
    /// 取得補助類型的中文名稱
    /// </summary>
    private static string GetGrantTypeChineseName(string typeCode)
    {
        switch (typeCode?.ToUpper())
        {
            case "ACC":
                return "無障礙";
            case "LIT":
                return "素養";
            case "MUL":
                return "多元";
            case "EDC":
                return "學校民間";
            case "CLB":
                return "學校社團";
            default:
                return typeCode;
        }
    }

    /// <summary>
    /// 取得專案資訊用於發送通知
    /// </summary>
    private static ProjectNotificationInfo GetProjectInfoForNotification(string projectId, string subsidyType)
    {
        try
        {
            using (var db = new GS.Data.Sql.DbHelper())
            {
                // CLB 需要 JOIN 兩個表
                if (subsidyType?.ToUpper() == "CLB")
                {
                    db.CommandText = @"
                        SELECT
                            AB.ProjectNameTw AS ProjectName,
                            PM.UserAccount
                        FROM OFS_CLB_Application_Basic AB
                        INNER JOIN OFS_CLB_Project_Main PM ON AB.ProjectID = PM.ProjectID
                        WHERE AB.ProjectID = @ProjectID
                    ";
                }
                else
                {
                    string tableName = GetProjectTableName(subsidyType);
                    string projectNameColumn = GetProjectNameColumn(subsidyType);

                    if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(projectNameColumn))
                        return null;

                    db.CommandText = $@"
                        SELECT {projectNameColumn} AS ProjectName, UserAccount
                        FROM {tableName}
                        WHERE ProjectID = @ProjectID
                    ";
                }

                db.Parameters.Add("@ProjectID", projectId);

                var table = db.GetTable();

                if (table.Rows.Count > 0)
                {
                    var row = table.Rows[0];
                    return new ProjectNotificationInfo
                    {
                        ProjectID = projectId,
                        ProjectName = row["ProjectName"]?.ToString() ?? "",
                        UserAccount = row["UserAccount"]?.ToString() ?? ""
                    };
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得專案資訊時發生錯誤 - ProjectID: {projectId}, 錯誤: {ex.Message}");
        }

        return null;
    }

    /// <summary>
    /// 取得專案表名稱
    /// </summary>
    private static string GetProjectTableName(string subsidyType)
    {
        switch (subsidyType?.ToUpper())
        {
            case "ACC":
                return "OFS_ACC_Project";
            case "LIT":
                return "OFS_LIT_Project";
            case "MUL":
                return "OFS_MUL_Project";
            case "EDC":
                return "OFS_EDC_Project";
            case "CLB":
                return "OFS_CLB_Project_Main";
            default:
                return "";
        }
    }

    /// <summary>
    /// 取得專案名稱欄位名稱
    /// </summary>
    private static string GetProjectNameColumn(string subsidyType)
    {
        switch (subsidyType?.ToUpper())
        {
            case "ACC":
            case "LIT":
            case "MUL":
            case "EDC":
                return "ProjectName";
            case "CLB":
                return "ProjectNameTw";
            default:
                return "";
        }
    }

    /// <summary>
    /// 專案通知資訊
    /// </summary>
    private class ProjectNotificationInfo
    {
        public string ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string UserAccount { get; set; }
    }

    /// <summary>
    /// 從資料庫取得審查記錄
    /// </summary>
    private static List<ReviewRecordInfo> GetReviewRecordsByProjects(List<string> projectIds, string reviewStage)
    {
        var records = new List<ReviewRecordInfo>();

        try
        {
            using (var db = new GS.Data.Sql.DbHelper())
            {
                // 建立 ProjectID 的 IN 查詢條件
                var projectIdParams = string.Join(",", projectIds.Select((p, i) => $"@ProjectID{i}"));

                db.CommandText = $@"
                    SELECT
                        rr.ProjectID,
                        rr.Email,
                        rr.ReviewerName,
                        rr.Token,
                        CASE
                            WHEN rr.ProjectID LIKE '%SCI%' THEN am.ProjectNameTw
                            WHEN rr.ProjectID LIKE '%CUL%' THEN cp.ProjectName
                            ELSE rr.ProjectID
                        END AS ProjectName
                    FROM OFS_ReviewRecords rr
                    LEFT JOIN OFS_SCI_Application_Main am ON rr.ProjectID = am.ProjectID
                    LEFT JOIN OFS_SCI_Project_Main pm ON am.ProjectID = pm.ProjectID
                    LEFT JOIN OFS_CUL_Project cp ON rr.ProjectID = cp.ProjectID
                    WHERE rr.ProjectID IN ({projectIdParams})
                      AND rr.ReviewStage = @ReviewStage
                      AND rr.IsSubmit = 0
                ";

                // 加入 ProjectID 參數
                for (int i = 0; i < projectIds.Count; i++)
                {
                    db.Parameters.Add($"@ProjectID{i}", projectIds[i]);
                }

                db.Parameters.Add("@ReviewStage", reviewStage);

                var table = db.GetTable();

                foreach (DataRow row in table.Rows)
                {
                    records.Add(new ReviewRecordInfo
                    {
                        ProjectID = row["ProjectID"]?.ToString() ?? "",
                        Email = row["Email"]?.ToString() ?? "",
                        ReviewerName = row["ReviewerName"]?.ToString() ?? "",
                        Token = row["Token"]?.ToString() ?? "",
                        ProjectName = row["ProjectName"]?.ToString() ?? ""
                    });
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得審查記錄時發生錯誤：{ex.Message}");
        }

        return records;
    }

    /// <summary>
    /// 組成審查 URL（統一使用 SciDomainReview.aspx）
    /// </summary>
    private static string BuildReviewUrl(string projectId, string token)
    {
        string host = HttpContext.Current?.Request?.Url?.GetLeftPart(UriPartial.Authority)
                      ?? "http://localhost:50929";

        string appRootPath = System.Configuration.ConfigurationManager.AppSettings["AppRootPath"] ?? "";
        string baseUrl = host + appRootPath;

        // 所有補助案類型、所有審查階段都導向 SciDomainReview.aspx
        string reviewPage = "/OFS/SCI/SciDomainReview.aspx";

        return $"{baseUrl}{reviewPage}?ProjectID={projectId}&Token={token}";
    }

    /// <summary>
    /// 根據審查類型取得審查階段代碼
    /// </summary>
    private static string GetReviewStageByType(string reviewType)
    {
        switch (reviewType)
        {
            case "1":
                return "2"; // 資格審查 --> 找 領域審
            case "2":
                return "3"; // 實質審查/初審 -->找技術審
            default:
                return "";
        }
    }

    /// <summary>
    /// 根據審查階段取得事件名稱
    /// </summary>
    private static string GetSCIEventNameByReviewType(string reviewStage)
    {
        switch (reviewStage)
        {
            case "1":
                return "實質審查";
            case "2":
                return "技術審查";
            default:
                return "審查";
        }
    }
    /// <summary>
    /// 根據審查階段取得事件名稱
    /// </summary>
    private static string GetCULEventNameByReviewType(string reviewStage)
    {
        switch (reviewStage)
        {
            case "1":
                return "初審";
            case "2":
                return "複審";
            default:
                return "審查";
        }
    }
    /// <summary>
    /// 審查記錄資訊類別
    /// </summary>
    private class ReviewRecordInfo
    {
        public string ProjectID { get; set; }
        public string Email { get; set; }
        public string ReviewerName { get; set; }
        public string Token { get; set; }
        public string ProjectName { get; set; }
    }

    #endregion

    #region 統計功能

    /// <summary>
    /// 取得各審查類型的統計數量
    /// </summary>
    /// <returns>包含各類型統計數量的 JSON 字串</returns>
    [WebMethod]
    public static string GetReviewTypeStatistics()
    {
        try
        {
            // 呼叫 Helper 取得統計資料
            var statistics = ReviewCheckListHelper.GetReviewTypeStatistics();

            return JsonConvert.SerializeObject(new
            {
                success = true,
                data = statistics,
                message = "成功取得統計資料"
            });
        }
        catch (Exception ex)
        {
            return JsonConvert.SerializeObject(new
            {
                success = false,
                message = $"取得統計資料時發生錯誤: {ex.Message}",
                error = ex.ToString()
            });
        }
    }

    /// <summary>
    /// 發送「進入決審」E31 通知給申請者
    /// 科專：實質審查通過後進入決審，寄信通知申請者
    /// 文化：複審通過後進入決審，寄信通知申請者
    /// </summary>
    private static void SendEnterFinalReviewNotifications(List<string> projectIds)
    {
        try
        {
            if (projectIds == null || projectIds.Count == 0)
                return;

            // 處理科專專案
            var sciProjects = projectIds.Where(p => p.Contains("SCI")).ToList();
            int sciSuccessCount = 0;
            foreach (string projectId in sciProjects)
            {
                try
                {
                    string applicantEmail = OFS_SciApplicationHelper.GetApplicantAccountByProjectId(projectId);
                    var sciProjectInfo = ReviewCheckListHelper.GetSciProjectInfo(projectId);
                    string projectName = sciProjectInfo != null ? sciProjectInfo["ProjectName"]?.ToString() ?? "" : "";
                    int year = sciProjectInfo != null ? Convert.ToInt32(sciProjectInfo["Year"]) : 0;

                    if (!string.IsNullOrEmpty(applicantEmail) && !string.IsNullOrEmpty(projectName) && year > 0)
                    {
                        NotificationHelper.E31("科專", year, projectName, "實質審查", applicantEmail);
                        sciSuccessCount++;
                        System.Diagnostics.Debug.WriteLine($"已發送進入決審 E31 通知 - ProjectID: {projectId}, Year: {year}, ProjectName: {projectName}, Email: {applicantEmail}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"進入決審通知資料不完整 - ProjectID: {projectId}, Email: {applicantEmail}, Name: {projectName}, Year: {year}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"發送進入決審 E31 通知給專案 {projectId} 時發生錯誤：{ex.Message}");
                }
            }

            // 處理文化專案
            var culProjects = projectIds.Where(p => p.Contains("CUL")).ToList();
            int culSuccessCount = 0;
            foreach (string projectId in culProjects)
            {
                try
                {
                    string applicantEmail = OFS_CulProjectHelper.getUserAccount(projectId);
                    var culProjectInfo = ReviewCheckListHelper.GetCulProjectInfo(projectId);
                    string projectName = culProjectInfo != null ? culProjectInfo["ProjectName"]?.ToString() ?? "" : "";
                    int year = culProjectInfo != null ? Convert.ToInt32(culProjectInfo["Year"]) : 0;

                    if (!string.IsNullOrEmpty(applicantEmail) && !string.IsNullOrEmpty(projectName) && year > 0)
                    {
                        NotificationHelper.E31("文化", year, projectName, "複審", applicantEmail);
                        culSuccessCount++;
                        System.Diagnostics.Debug.WriteLine($"已發送進入決審 E31 通知 - ProjectID: {projectId}, Year: {year}, ProjectName: {projectName}, Email: {applicantEmail}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"進入決審通知資料不完整 - ProjectID: {projectId}, Email: {applicantEmail}, Name: {projectName}, Year: {year}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"發送進入決審 E31 通知給專案 {projectId} 時發生錯誤：{ex.Message}");
                }
            }

            System.Diagnostics.Debug.WriteLine($"已完成發送進入決審 E31 通知，科專成功 {sciSuccessCount}/{sciProjects.Count} 件，文化成功 {culSuccessCount}/{culProjects.Count} 件");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"發送進入決審 E31 通知時發生錯誤：{ex.Message}");
        }
    }

    /// <summary>
    /// 發送「決審核定」E5 通知給申請者
    /// 決審核定後，寄信通知申請者核定金額
    /// </summary>
    private static void SendApprovalNotifications(List<string> projectIds)
    {
        try
        {
            if (projectIds == null || projectIds.Count == 0)
                return;

            // 依補助類型分組專案
            var sciProjects = projectIds.Where(p => p.Contains("SCI")).ToList();
            var clbProjects = projectIds.Where(p => p.Contains("CLB")).ToList();
            var culProjects = projectIds.Where(p => p.Contains("CUL")).ToList();
            var edcProjects = projectIds.Where(p => p.Contains("EDC")).ToList();
            var litProjects = projectIds.Where(p => p.Contains("LIT")).ToList();
            var mulProjects = projectIds.Where(p => p.Contains("MUL")).ToList();
            var accProjects = projectIds.Where(p => p.Contains("ACC")).ToList();

            int totalSuccess = 0;

            // 處理科專 (SCI)
            if (sciProjects.Count > 0)
            {
                totalSuccess += SendApprovalNotificationsForSCI(sciProjects);
            }

            // 處理社團 (CLB)
            if (clbProjects.Count > 0)
            {
                totalSuccess += SendApprovalNotificationsForCLB(clbProjects);
            }

            // 處理其他 5 類 (CUL, EDC, LIT, MUL, ACC)
            if (culProjects.Count > 0)
            {
                totalSuccess += SendApprovalNotificationsForOtherTypes(culProjects, "CUL", "文化");
            }
            if (edcProjects.Count > 0)
            {
                totalSuccess += SendApprovalNotificationsForOtherTypes(edcProjects, "EDC", "學校民間");
            }
            if (litProjects.Count > 0)
            {
                totalSuccess += SendApprovalNotificationsForOtherTypes(litProjects, "LIT", "素養");
            }
            if (mulProjects.Count > 0)
            {
                totalSuccess += SendApprovalNotificationsForOtherTypes(mulProjects, "MUL", "多元");
            }
            if (accProjects.Count > 0)
            {
                totalSuccess += SendApprovalNotificationsForOtherTypes(accProjects, "ACC", "無障礙");
            }

            System.Diagnostics.Debug.WriteLine($"已完成發送決審核定 E5 通知，成功 {totalSuccess}/{projectIds.Count} 件");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"發送決審核定 E5 通知時發生錯誤：{ex.Message}");
        }
    }

    /// <summary>
    /// 發送科專 (SCI) 決審核定 E5 通知
    /// </summary>
    private static int SendApprovalNotificationsForSCI(List<string> projectIds)
    {
        int successCount = 0;
        foreach (string projectId in projectIds)
        {
            try
            {
                var projectInfo = ReviewCheckListHelper.GetSciApprovalInfo(projectId);
                if (projectInfo != null)
                {
                    string applicantEmail = projectInfo["UserAccount"]?.ToString() ?? "";
                    string projectName = projectInfo["ProjectName"]?.ToString() ?? "";
                    int year = projectInfo["Year"] != null ? Convert.ToInt32(projectInfo["Year"]) : 0;
                    decimal approvedAmount = projectInfo["ApprovedSubsidy"] != null ? Convert.ToDecimal(projectInfo["ApprovedSubsidy"]) : 0;

                    if (!string.IsNullOrEmpty(applicantEmail) && !string.IsNullOrEmpty(projectName) && year > 0 && approvedAmount > 0)
                    {
                        NotificationHelper.E5( year, projectName, approvedAmount.ToString("N0"), applicantEmail);
                        NotificationHelper.F12( "科專", projectName, "契約資料", applicantEmail);
                        NotificationHelper.F12( "科專", projectName, "第一次請款", applicantEmail);
                        successCount++;
                        System.Diagnostics.Debug.WriteLine($"已發送科專決審核定 E5 通知 - ProjectID: {projectId}, Amount: {approvedAmount}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"發送科專決審核定 E5 通知給專案 {projectId} 時發生錯誤：{ex.Message}");
            }
        }
        return successCount;
    }

    /// <summary>
    /// 發送社團 (CLB) 決審核定 E5 通知
    /// </summary>
    private static int SendApprovalNotificationsForCLB(List<string> projectIds)
    {
        int successCount = 0;
        foreach (string projectId in projectIds)
        {
            try
            {
                var projectInfo = ReviewCheckListHelper.GetClbApprovalInfo(projectId);
                if (projectInfo != null)
                {
                    string applicantEmail = projectInfo["UserAccount"]?.ToString() ?? "";
                    string projectName = projectInfo["ProjectName"]?.ToString() ?? "";
                    int year = projectInfo["Year"] != null ? Convert.ToInt32(projectInfo["Year"]) : 0;
                    decimal approvedAmount = projectInfo["ApprovedSubsidy"] != null ? Convert.ToDecimal(projectInfo["ApprovedSubsidy"]) : 0;

                    if (!string.IsNullOrEmpty(applicantEmail) && !string.IsNullOrEmpty(projectName) && year > 0 && approvedAmount > 0)
                    {
                        NotificationHelper.E5(year, projectName, approvedAmount.ToString("N0"), applicantEmail);
                        successCount++;
                        System.Diagnostics.Debug.WriteLine($"已發送社團決審核定 E5 通知 - ProjectID: {projectId}, Amount: {approvedAmount}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"發送社團決審核定 E5 通知給專案 {projectId} 時發生錯誤：{ex.Message}");
            }
        }
        return successCount;
    }

    /// <summary>
    /// 發送其他類型 (CUL, EDC, LIT, MUL, ACC) 決審核定 E5 通知
    /// </summary>
    private static int SendApprovalNotificationsForOtherTypes(List<string> projectIds, string typeCode, string typeName)
    {
        int successCount = 0;
        foreach (string projectId in projectIds)
        {
            try
            {
                var projectInfo = ReviewCheckListHelper.GetOtherTypeApprovalInfo(projectId, typeCode);
                if (projectInfo != null)
                {
                    string applicantEmail = projectInfo["UserAccount"]?.ToString() ?? "";
                    string projectName = projectInfo["ProjectName"]?.ToString() ?? "";
                    int year = projectInfo["Year"] != null ? Convert.ToInt32(projectInfo["Year"]) : 0;
                    decimal approvedAmount = projectInfo["ApprovedAmount"] != null ? Convert.ToDecimal(projectInfo["ApprovedAmount"]) : 0;

                    if (!string.IsNullOrEmpty(applicantEmail) && !string.IsNullOrEmpty(projectName) && year > 0 && approvedAmount > 0)
                    {
                        NotificationHelper.E5(year, projectName, approvedAmount.ToString("N0"), applicantEmail);
                        
                        if(typeCode == "LIT" || typeCode == "MUL"){
                            NotificationHelper.F12(typeName, projectName, "第一期請款", applicantEmail);
                        }
                        successCount++;
                        System.Diagnostics.Debug.WriteLine($"已發送{typeName}決審核定 E5 通知 - ProjectID: {projectId}, Amount: {approvedAmount}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"發送{typeName}決審核定 E5 通知給專案 {projectId} 時發生錯誤：{ex.Message}");
            }
        }
        return successCount;
    }

    /// <summary>
    /// 發送「審查不通過」E2 通知給申請者
    /// </summary>
    private static void SendRejectNotifications(List<string> projectIds)
    {
        try
        {
            if (projectIds == null || projectIds.Count == 0)
                return;

            // 依補助類型分組專案
            var sciProjects = projectIds.Where(p => p.Contains("SCI")).ToList();
            var clbProjects = projectIds.Where(p => p.Contains("CLB")).ToList();
            var culProjects = projectIds.Where(p => p.Contains("CUL")).ToList();
            var edcProjects = projectIds.Where(p => p.Contains("EDC")).ToList();
            var litProjects = projectIds.Where(p => p.Contains("LIT")).ToList();
            var mulProjects = projectIds.Where(p => p.Contains("MUL")).ToList();
            var accProjects = projectIds.Where(p => p.Contains("ACC")).ToList();

            int totalSuccess = 0;

            // 處理科專 (SCI)
            if (sciProjects.Count > 0)
            {
                totalSuccess += SendRejectNotificationsForSCI(sciProjects);
            }

            // 處理社團 (CLB)
            if (clbProjects.Count > 0)
            {
                totalSuccess += SendRejectNotificationsForCLB(clbProjects);
            }

            // 處理其他 5 類 (CUL, EDC, LIT, MUL, ACC)
            if (culProjects.Count > 0)
            {
                totalSuccess += SendRejectNotificationsForOtherTypes(culProjects, "CUL", "文化");
            }
            if (edcProjects.Count > 0)
            {
                totalSuccess += SendRejectNotificationsForOtherTypes(edcProjects, "EDC", "學校民間");
            }
            if (litProjects.Count > 0)
            {
                totalSuccess += SendRejectNotificationsForOtherTypes(litProjects, "LIT", "素養");
            }
            if (mulProjects.Count > 0)
            {
                totalSuccess += SendRejectNotificationsForOtherTypes(mulProjects, "MUL", "多元");
            }
            if (accProjects.Count > 0)
            {
                totalSuccess += SendRejectNotificationsForOtherTypes(accProjects, "ACC", "無障礙");
            }

            System.Diagnostics.Debug.WriteLine($"已完成發送審查不通過 E2 通知，成功 {totalSuccess}/{projectIds.Count} 件");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"發送審查不通過 E2 通知時發生錯誤：{ex.Message}");
        }
    }

    /// <summary>
    /// 發送科專 (SCI) 審查不通過 E2 通知
    /// </summary>
    private static int SendRejectNotificationsForSCI(List<string> projectIds)
    {
        int successCount = 0;
        foreach (string projectId in projectIds)
        {
            try
            {
                var projectInfo = ReviewCheckListHelper.GetSciApprovalInfo(projectId);
                if (projectInfo != null)
                {
                    string applicantEmail = projectInfo["UserAccount"]?.ToString() ?? "";
                    string projectName = projectInfo["ProjectName"]?.ToString() ?? "";
                    int year = projectInfo["Year"] != null ? Convert.ToInt32(projectInfo["Year"]) : 0;

                    if (!string.IsNullOrEmpty(applicantEmail) && !string.IsNullOrEmpty(projectName) && year > 0)
                    {
                        NotificationHelper.E2("科專", year, projectName, applicantEmail);
                        successCount++;
                        System.Diagnostics.Debug.WriteLine($"已發送科專審查不通過 E2 通知 - ProjectID: {projectId}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"發送科專審查不通過 E2 通知給專案 {projectId} 時發生錯誤：{ex.Message}");
            }
        }
        return successCount;
    }

    /// <summary>
    /// 發送社團 (CLB) 審查不通過 E2 通知
    /// </summary>
    private static int SendRejectNotificationsForCLB(List<string> projectIds)
    {
        int successCount = 0;
        foreach (string projectId in projectIds)
        {
            try
            {
                var projectInfo = ReviewCheckListHelper.GetClbApprovalInfo(projectId);
                if (projectInfo != null)
                {
                    string applicantEmail = projectInfo["UserAccount"]?.ToString() ?? "";
                    string projectName = projectInfo["ProjectName"]?.ToString() ?? "";
                    int year = projectInfo["Year"] != null ? Convert.ToInt32(projectInfo["Year"]) : 0;

                    if (!string.IsNullOrEmpty(applicantEmail) && !string.IsNullOrEmpty(projectName) && year > 0)
                    {
                        NotificationHelper.E2("學校社團", year, projectName, applicantEmail);
                        successCount++;
                        System.Diagnostics.Debug.WriteLine($"已發送社團審查不通過 E2 通知 - ProjectID: {projectId}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"發送社團審查不通過 E2 通知給專案 {projectId} 時發生錯誤：{ex.Message}");
            }
        }
        return successCount;
    }

    /// <summary>
    /// 發送其他類型 (CUL, EDC, LIT, MUL, ACC) 審查不通過 E2 通知
    /// </summary>
    private static int SendRejectNotificationsForOtherTypes(List<string> projectIds, string typeCode, string typeName)
    {
        int successCount = 0;
        foreach (string projectId in projectIds)
        {
            try
            {
                var projectInfo = ReviewCheckListHelper.GetOtherTypeApprovalInfo(projectId, typeCode);
                if (projectInfo != null)
                {
                    string applicantEmail = projectInfo["UserAccount"]?.ToString() ?? "";
                    string projectName = projectInfo["ProjectName"]?.ToString() ?? "";
                    int year = projectInfo["Year"] != null ? Convert.ToInt32(projectInfo["Year"]) : 0;

                    if (!string.IsNullOrEmpty(applicantEmail) && !string.IsNullOrEmpty(projectName) && year > 0)
                    {
                        NotificationHelper.E2(typeName, year, projectName, applicantEmail);
                        successCount++;
                        System.Diagnostics.Debug.WriteLine($"已發送{typeName}審查不通過 E2 通知 - ProjectID: {projectId}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"發送{typeName}審查不通過 E2 通知給專案 {projectId} 時發生錯誤：{ex.Message}");
            }
        }
        return successCount;
    }

    #endregion

    #region 審查人員設置相關 WebMethod

    /// <summary>
    /// 取得年度清單
    /// </summary>
    [WebMethod]
    public static string GetYearList()
    {
        try
        {
            using (var db = new GS.Data.Sql.DbHelper())
            {
                db.CommandText = @"
                    SELECT Year
                    FROM [OCA_OceanSubsidy].[dbo].[OFS_GrantType]
                    GROUP BY Year
                    ORDER BY Year DESC
                ";

                var table = db.GetTable();
                var results = new List<object>();

                foreach (DataRow row in table.Rows)
                {
                    results.Add(new
                    {
                        year = row["Year"]?.ToString() ?? ""
                    });
                }

                return JsonConvert.SerializeObject(new
                {
                    success = true,
                    data = results,
                    message = "成功取得年度清單"
                });
            }
        }
        catch (Exception ex)
        {
            return JsonConvert.SerializeObject(new
            {
                success = false,
                message = "取得年度清單時發生錯誤: " + ex.Message,
                data = new List<object>()
            });
        }
    }

    /// <summary>
    /// 取得領域清單（主題、領域）
    /// </summary>
    [WebMethod]
    public static string GetSubjectTypes()
    {
        try
        {
            using (var db = new GS.Data.Sql.DbHelper())
            {
                db.CommandText = @"
                    SELECT Code, Descname
                    FROM [OCA_OceanSubsidy].[dbo].[Sys_ZgsCode]
                    WHERE (CodeGroup = 'CULField' AND ParentCode IN (10, 20, 30))
                       OR CodeGroup = 'SCIField'
                    ORDER BY CodeGroup
                ";

                var table = db.GetTable();
                var results = new List<object>();

                foreach (DataRow row in table.Rows)
                {
                    results.Add(new
                    {
                        code = row["Code"]?.ToString() ?? "",
                        name = row["Descname"]?.ToString() ?? ""
                    });
                }

                return JsonConvert.SerializeObject(new
                {
                    success = true,
                    data = results,
                    message = "成功取得領域清單"
                });
            }
        }
        catch (Exception ex)
        {
            return JsonConvert.SerializeObject(new
            {
                success = false,
                message = "取得領域清單時發生錯誤: " + ex.Message,
                data = new List<object>()
            });
        }
    }

    /// <summary>
    /// 根據領域代碼取得審查委員清單
    /// </summary>
    [WebMethod]
    public static string GetReviewersBySubject(string subjectCode)
    {
        try
        {
            using (var db = new GS.Data.Sql.DbHelper())
            {
                db.CommandText = @"
                    SELECT CommitteeUser, Email
                    FROM OFS_ReviewCommitteeList
                    WHERE SubjectTypeID = @SubjectCode
                ";

                db.Parameters.Add("@SubjectCode", subjectCode);

                var table = db.GetTable();
                var results = new List<object>();

                foreach (DataRow row in table.Rows)
                {
                    string name = row["CommitteeUser"]?.ToString() ?? "";  // 中文名稱
                    string account = row["Email"]?.ToString() ?? "";       // 帳號

                    results.Add(new
                    {
                        account = account,
                        name = name,
                        displayName = $"{account} {name}"
                    });
                }

                db.Parameters.Clear();

                return JsonConvert.SerializeObject(new
                {
                    success = true,
                    data = results,
                    message = $"成功取得 {results.Count} 位審查委員"
                });
            }
        }
        catch (Exception ex)
        {
            return JsonConvert.SerializeObject(new
            {
                success = false,
                message = "取得審查委員清單時發生錯誤: " + ex.Message,
                data = new List<object>()
            });
        }
    }

    #endregion

}
