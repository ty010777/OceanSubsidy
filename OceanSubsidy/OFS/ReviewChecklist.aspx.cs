using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Operation.OFS;
using GS.OCA_OceanSubsidy.Model.OFS;
using GS.App;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

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
                ShowErrorAndRedirect("您沒有存取此頁面的權限");
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
    private void ShowErrorAndRedirect(string message)
    {
        string script = $@"
            Swal.fire({{
                title: '存取權限',
                text: '{message}',
                icon: 'error',
                confirmButtonText: '確定'
            }}).then((result) => {{
                if (result.isConfirmed) {{
                    window.location.href = '~/Default.aspx';
                }}
            }});
        ";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowErrorAndRedirect", script, true);
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
            // 載入年度選項
            ddlYear_Type1.Items.Add(new ListItem("全部", ""));
            ddlYear_Type1.Items.Add(new ListItem("113年", "113"));
            ddlYear_Type1.Items.Add(new ListItem("114年", "114")); 
            ddlYear_Type1.DataTextField = "Text";
            ddlYear_Type1.DataValueField = "Value";
            ddlYear_Type1.DataBind();

            // 載入類別選項
            ddlCategory_Type1.Items.Add(new ListItem("全部", ""));            
            ddlCategory_Type1.Items.Add(new ListItem("科專", "SCI"));            
            ddlCategory_Type1.Items.Add(new ListItem("文化", "CUL"));            
            ddlCategory_Type1.Items.Add(new ListItem("學校/民間", "EDC"));            
            ddlCategory_Type1.Items.Add(new ListItem("學校/社團", "CLB"));   
            ddlCategory_Type1.DataTextField = "Text";
            ddlCategory_Type1.DataValueField = "Value";
            ddlCategory_Type1.DataBind();

            // 載入階段狀態選項
            var stageOptions = ReviewCheckListHelper.GetReviewStageStatusOptions();
            ddlStage_Type1.DataSource = stageOptions;
            ddlStage_Type1.DataTextField = "Text";
            ddlStage_Type1.DataValueField = "Value";
            ddlStage_Type1.DataBind();

            
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
    /// 載入科專領域審查的下拉選單資料
    /// </summary>
    private void LoadDomainReviewDropdowns()
    {
        try
        {
            // 載入科專年度選項
            ddlYear_Type2.Items.Add(new ListItem("全部", ""));
            ddlYear_Type2.Items.Add(new ListItem("113年", "113"));
            ddlYear_Type2.Items.Add(new ListItem("114年", "114"));            
            ddlYear_Type2.DataTextField = "Text";
            ddlYear_Type2.DataValueField = "Value";
            ddlYear_Type2.DataBind();

            // 載入類別選項
            ddlCategory_Type2.Items.Add(new ListItem("全部", ""));            
            ddlCategory_Type2.Items.Add(new ListItem("科專", "SCI"));            
            ddlCategory_Type2.Items.Add(new ListItem("文化", "CUL"));            
            ddlCategory_Type2.Items.Add(new ListItem("學校/民間", "EDC"));            
            ddlCategory_Type2.Items.Add(new ListItem("學校/社團", "CLB"));            
            ddlCategory_Type2.DataTextField = "Text";
            ddlCategory_Type2.DataValueField = "Value";
            ddlCategory_Type2.DataBind();

            // 載入申請單位選項
            // TODO 正文 載入申請單位選項
            // TODO 正文 這裡是 領域審查的申請單位 下拉選單，但因為type2 3 沒有view 所以請搜尋文化 目前project的 領域審查 的申請單位

            var orgOptions = ReviewCheckListHelper.GetReviewOrgOptions("領域審查");
            ddlOrg_Type2.DataSource = orgOptions;
            ddlOrg_Type2.DataTextField = "Text";
            ddlOrg_Type2.DataValueField = "Value";
            ddlOrg_Type2.DataBind();

            // 
            // TODO 正文 載入承辦人員選項
            // TODO 正文 這裡是 領域審查的承辦人 下拉選單，但因為type2 3 沒有view 所以請搜尋文化 目前project的 領域審查 的承辦人
            
            var supervisorOptions = ReviewCheckListHelper.GetReviewSupervisorOptions("領域審查");
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
            HandleException(ex, "載入領域審查下拉選單時發生錯誤");
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
            // 載入年度選項
            ddlYear_Type3.Items.Add(new ListItem("全部", ""));
            ddlYear_Type3.Items.Add(new ListItem("113", "113年"));
            ddlYear_Type3.Items.Add(new ListItem("114", "114年"));            
            ddlYear_Type3.DataTextField = "Text";
            ddlYear_Type3.DataValueField = "Value";
            ddlYear_Type3.DataBind();

            // 載入類別選項
            ddlCategory_Type3.Items.Add(new ListItem("全部", ""));            
            ddlCategory_Type3.Items.Add(new ListItem("科專", "SCI"));            
            ddlCategory_Type3.Items.Add(new ListItem("文化", "CUL"));            
            ddlCategory_Type3.Items.Add(new ListItem("學校/民間", "EDC"));            
            ddlCategory_Type3.Items.Add(new ListItem("學校/社團", "CLB"));            
            ddlCategory_Type3.DataTextField = "Text";
            ddlCategory_Type3.DataValueField = "Value";
            ddlCategory_Type3.DataBind();

            // 載入申請單位選項
            // TODO 正文 載入申請單位選項
            // TODO 正文 這裡是 技術審查的申請單位 下拉選單，但因為type2 3 沒有view 所以請搜尋文化 目前project的 技術審查 的申請單位

            var orgOptions = ReviewCheckListHelper.GetReviewOrgOptions("技術審查");
            ddlOrg_Type3.DataSource = orgOptions;
            ddlOrg_Type3.DataTextField = "Text";
            ddlOrg_Type3.DataValueField = "Value";
            ddlOrg_Type3.DataBind();
            
            
            // TODO 正文 載入承辦人員選項
            // TODO 正文 這裡是 技術審查的承辦人 下拉選單，但因為type2 3 沒有view 所以請搜尋文化 目前project的 技術審查 的承辦人

            // 載入承辦人員選項
            var supervisorOptions = ReviewCheckListHelper.GetReviewSupervisorOptions("技術審查");
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
            // 載入年度選項（移除「全部」選項）
            ddlYear_Type4.Items.Add(new ListItem("113年", "113"));
            ddlYear_Type4.Items.Add(new ListItem("114年", "114"));            
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
            ddlCategory_Type4.Items.Add(new ListItem("科專", "SCI"));            
            ddlCategory_Type4.Items.Add(new ListItem("文化", "CUL"));            
            ddlCategory_Type4.Items.Add(new ListItem("學校/民間", "EDC"));            
            ddlCategory_Type4.Items.Add(new ListItem("學校/社團", "CLB"));            
            ddlCategory_Type4.DataTextField = "Text";
            ddlCategory_Type4.DataValueField = "Value";
            ddlCategory_Type4.DataBind();
            
            // 初始化審查組別選項 (預設載入科專的審查組別)
            ddlReviewGroup_Type4.Items.Clear();
            var reviewGroupOptions = ReviewCheckListHelper.GetSciReviewGroupOptions();
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
            // 載入年度選項
            ddlYear_Type5.Items.Add(new ListItem("全部", ""));
            ddlYear_Type5.Items.Add(new ListItem("113年", "113"));
            ddlYear_Type5.Items.Add(new ListItem("114年", "114"));

            // 載入類別選項
            ddlCategory_Type5.Items.Add(new ListItem("全部", ""));
            ddlCategory_Type5.Items.Add(new ListItem("科專", "SCI"));
            ddlCategory_Type5.Items.Add(new ListItem("文化", "CUL"));
            ddlCategory_Type5.Items.Add(new ListItem("學校民間", "EDC"));
            ddlCategory_Type5.Items.Add(new ListItem("學校社團", "CLB"));
            ddlCategory_Type5.Items.Add(new ListItem("多元", "MUL"));
            ddlCategory_Type5.Items.Add(new ListItem("素養", "LIT"));
            ddlCategory_Type5.Items.Add(new ListItem("無障礙", "ACC"));

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
            // 載入年度選項
            ddlYear_Type6.Items.Add(new ListItem("全部", ""));
            ddlYear_Type6.Items.Add(new ListItem("113年", "113"));
            ddlYear_Type6.Items.Add(new ListItem("114年", "114"));

            // 載入類別選項
            ddlCategory_Type6.Items.Add(new ListItem("全部", ""));
            ddlCategory_Type6.Items.Add(new ListItem("科專", "SCI"));
            ddlCategory_Type6.Items.Add(new ListItem("文化", "CUL"));
            ddlCategory_Type6.Items.Add(new ListItem("學校民間", "EDC"));
            ddlCategory_Type6.Items.Add(new ListItem("學校社團", "CLB"));
            ddlCategory_Type6.Items.Add(new ListItem("多元", "MUL"));
            ddlCategory_Type6.Items.Add(new ListItem("素養", "LIT"));
            ddlCategory_Type6.Items.Add(new ListItem("無障礙", "ACC"));

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
    /// Type4 決定審核清單查詢按鈕點擊事件
    /// </summary>
    protected void btnSearch_Type4_Click(object sender, EventArgs e)
    {
        try
        {
            // 取得查詢條件
            string year = ddlYear_Type4.SelectedValue;
            string category = ddlCategory_Type4.SelectedValue;
            string reviewGroup = ddlReviewGroup_Type4.SelectedValue;
            string orgName = ddlOrg_Type4.SelectedValue;
            string supervisor = ddlSupervisor_Type4.SelectedValue;
            string keyword = Request.Form["txtKeyword_Type4"] ?? "";
            List<ReviewChecklistItem> Results = new List<ReviewChecklistItem>();
            
            // 執行決定審核清單查詢
            if(category == "SCI" || string.IsNullOrEmpty(category)){
                Results = ReviewCheckListHelper.Search_Type4(
                    year, 
                    orgName, 
                    supervisor, 
                    keyword,
                    category,
                    reviewGroup);     // 審查組別代碼
            }
            
            
            // 將查詢結果轉換為 JSON 並傳遞給前端
            string jsonResults = Newtonsoft.Json.JsonConvert.SerializeObject(Results);
            
            string script = $@"
                $(function() {{
                    // 將查詢結果傳遞給前端 JavaScript
                    if (typeof window.ReviewChecklistManager !== 'undefined') {{
                        window.ReviewChecklistManager.renderSearchResults({jsonResults}, 4);
                    }} else {{
                        console.log('Type4 搜尋結果:', {jsonResults});
                    }}
                }});
            ";
            
            Page.ClientScript.RegisterStartupScript(this.GetType(), "SearchResults_Type4", script, true);
        }
        catch (Exception ex)
        {
            HandleException(ex, "執行 Type4 查詢時發生錯誤");
            
            // 顯示錯誤訊息給使用者
            string errorScript = @"
                Swal.fire({
                    title: '查詢錯誤',
                    text: '查詢時發生錯誤，請稍後再試',
                    icon: 'error',
                    confirmButtonText: '確定'
                });
            ";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "SearchError_Type4", errorScript, true);
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
                // 根據計畫類別決定要更新的資料表
                string tableName = GetTableNameByCategory(item.Category);
                
                // 準備參數
                var parameters = new Dictionary<string, object>
                {
                    ["@approvedSubsidy"] = item.ApprovedSubsidy ?? "0",
                    ["@finalReviewNotes"] = item.FinalReviewNotes ?? "",
                    ["@projectId"] = item.ProjectID
                };

                // 調用 Helper 執行資料庫更新
                ReviewCheckListHelper.UpdateApprovalData(tableName, parameters);
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
    /// <param name="reviewGroupCode">審查組別代碼</param>
    /// <returns>排序模式查詢結果</returns>
    [WebMethod]
    public static string SearchSortingMode(string year, string category, string reviewGroupCode)
    {
        try
        {
            List<SortingModeItem> results = ReviewCheckListHelper.Search_SCI_ForSorting(
                year: year,
                category: category,
                reviewGroupCode: reviewGroupCode
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

    /// <summary>
    /// 根據計畫類別取得對應的資料表名稱
    /// </summary>
    /// <param name="category">計畫類別</param>
    /// <returns>資料表名稱</returns>
    private static string GetTableNameByCategory(string category)
    {
        switch (category?.ToUpper())
        {
            case "SCI":
            case "科專":
                return "OFS_SCI_Project_Main";
            case "CUL":  
            case "文化":
                return "OFS_CUL_Project_Main";
            case "EDC":
            case "學校民間":
                return "OFS_EDC_Project_Main";
            case "CLB":
            case "學校社團":
                return "OFS_CLB_Project_Main";
            default:
                return "";
        }
    }

    #endregion

    #region 批次處理 WebMethods

    /// <summary>
    /// 批次審核 Type1 (資格審查/內容審查)
    /// </summary>
    /// <param name="projectIds">專案編號列表</param>
    /// <param name="actionType">操作類型</param>
    /// <param name="reviewType">審查類型</param>
    /// <returns>批次處理結果</returns>
    [WebMethod]
    public static BatchApprovalResult BatchApproveType(List<string> projectIds, string actionType, string reviewType)
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
            // 僅初審 且 文化 可以使用進入決審功能。
            if (result.ActionType == "進入決審" && result.ReviewType == "2")
            {
                //TODO 正文 進入決審 功能
            }
            else
            {
                return ProcessBatchToNextStage(projectIds, actionType, reviewType);
            }

           

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
    /// 處理批次轉下一階段功能
    /// </summary>
    /// <param name="projectIds">專案編號列表</param>
    /// <param name="actionType">操作類型</param>
    /// <param name="reviewType">審查類型</param>
    /// <returns>批次處理結果</returns>
    private static BatchApprovalResult ProcessBatchToNextStage(List<string> projectIds, string actionType, string reviewType)
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
                if (!GetStatusTransitionBySubsidyType(subsidyType, reviewType,out fromStatus, out toStatus, out StatusesName))
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
                    // 科專批次審核後的特殊處理（只在資格審查和領域審查階段執行）
                    if (reviewType == "1" || reviewType == "2")
                    {
                        switch (subsidyType)
                        {
                            case "SCI":
                                ReviewCheckListHelper.ProcessSciPostApproval(groupResult.SuccessProjectIds, toStatus, actionType, currentUser.Account);
                                break;
                            case "CUL":
                                // TODO 正文 如果是要進入初審、複審，請新增審核委員的基本審核資訊，並產生隨機token。
                                // 文化補助案的特殊處理（未來實作）
                                break;
                            default:
                                break;
                        }
                        // TODO 寄信 正文 (先至少有這個) 可以讓審查人員可以藉由寄信到審查頁面  
                        // TODO http://localhost:50929/OFS/SCI/SciDomainReview.aspx?ProjectID=114SCI0005&Token=c420d7d6-d045-4ced-b71e-6cd60a5ebf66
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
                actionType
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
    private static bool GetStatusTransitionBySubsidyType(string subsidyType, string currentReviewType, out string fromStatus, out string toStatus , out string StatusesName)
    {
        fromStatus = "";
        toStatus = "";
        StatusesName = "";
        try
        {
            switch (subsidyType)
            {
                case "SCI": // 科專: 資格審查 --> 領域審查 --> 技術審查 --> 決審
                    switch (currentReviewType)
                    {   
                        case "1": // 資格審查 → 領域審查
                            fromStatus = "資格審查";
                            toStatus = "領域審查";
                            return true;
                        case "2": // 領域審查 → 技術審查
                            fromStatus = "領域審查";
                            toStatus = "技術審查";
                            return true;
                        case "3": // 技術審查 → 決審
                            fromStatus = "技術審查";
                            StatusesName = "核定中";
                            toStatus = "決審核定";
                            return true;
                        case "4": // 決審 → 計畫執行
                            fromStatus = "決審核定";
                            StatusesName = "";
                            toStatus = "計畫執行";
                            return true;
                    }
                    break;
                // TODO: 正文 這裡是 實作 轉入下一階段時，各補助案 會從什麼階段到什麼階段。
                case "CUL": // 文化 (尚未實作)
                case "EDC": // 學校民間 (尚未實作)
                case "CLB": // 學校社團 (尚未實作)
                case "MUL":
                case "LIT": 
                case "ACC": 
                    
                return false;
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
    /// <param name="reviewType">審查類型 (2:領域審查, 3:技術審查)</param>
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
            // 權限檢查：只有 Type2(領域審查) 和 Type3(技術審查) 可以使用
            if (reviewType != "2" && reviewType != "3")
            {
                result.Success = false;
                result.Message = "提送至申請者功能只適用於領域審查(Type2)和技術審查(Type3)";
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

            // 2. 組成ID,帳號的清單
            var projectAccountInfo = new List<string>();
            foreach (var item in projectAccountList)
            {
                projectAccountInfo.Add($"ProjectID: {item.Key}, Account: {item.Value}");
            }

            // 3. TODO: 正文 根據email寄信
            // TODO: 實作寄信功能
            // - 取得申請者的 email 地址
            // - 產生寄信內容（告知審查結果需要補件或修正）
            // - 寄送通知信件給申請者
            foreach (var item in projectAccountList)
            {
                string projectId = item.Key;
                string account = item.Value;
                System.Diagnostics.Debug.WriteLine($"TODO: 寄信給申請者 - ProjectID: {projectId}, Account: {account}");
            }

            // 設定處理結果
            result.Success = true;
            result.SuccessCount = projectAccountList.Count;
            result.SuccessProjectIds = projectAccountList.Keys.ToList();
            result.Message = $"成功提送 {projectAccountList.Count} 筆計畫給申請者";

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
                    // TODO: 正文 處理各補助類型：搜尋 該補助案的Account (Email) 以利寄信
                    case "CUL":
                        break;
                        
                    case "EDC":
                        break;
                        
                    case "CLB":
                        break;
                        
                    case "MUL":
                        break;
                        
                    case "LIT":
                        break;
                        
                    case "ACC":
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
    /// <param name="reviewType">審查類型 (2:領域審查, 3:技術審查)</param>
    /// <returns>計畫詳細資料</returns>
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
                if (reviewType == "2")
                {
                    reviewStage = "領域審查";
                }
                else if (reviewType == "3")
                {
                    reviewStage = "技術審查";
                }
                planData = ReviewCheckListHelper.GetSciPlanDetail(projectId);
                reviewData = ReviewCheckListHelper.GetSciReviewComments(projectId, reviewStage);
            }
            else if (projectId.Contains("CUL"))
            {
                // TODO 正文 : 實作文化類別的計畫詳細資料和審查意見 
                if (reviewType == "2")
                {
                    reviewStage = "初審"; //自行設定
                }
                else if (reviewType == "3")
                {
                    reviewStage = "複審"; //自行設定
                }
                planData = ReviewCheckListHelper.GetCulturalPlanDetail(projectId); //基本資訊
                reviewData = ReviewCheckListHelper.GetCulturalReviewComments(projectId, reviewStage); //審查意見
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

    // GetReviewGroupOptions WebMethod 已移除，改用頁面載入時預載所有選項

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
            
            // 如果是科專類別，取得科專審查組別選項
            if (category == "SCI")
            {
                options = ReviewCheckListHelper.GetSciReviewGroupOptions();
            }
            //TODO 正文 加入 CUL 的審查組別選項
            else
            {
                // 其他類別只顯示「全部」
                options.Add(new DropdownItem { Text = "全部", Value = "" });
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
                    // TODO 正文: 實作轉入下一階段時，要update資料庫。
                    else if (projectId.Contains("CUL"))
                    {
                        // 文化
                    }
                    else if (projectId.Contains("EDC"))
                    {
                        // 學校民間
                    }
                    else if (projectId.Contains("CLB"))
                    {
                        // 學校社團
                    }else if (projectId.Contains("MUL"))
                    {
                        // 多元
                    }else if (projectId.Contains("LIT"))
                    {
                        // 素養
                    }else if (projectId.Contains("ACC"))
                    {
                        // 無障礙
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
            if ((fromStatus == "領域審查" && toStatus == "技術審查") || 
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
            var errorMessages = new List<string>();

            foreach (string projectId in projectIds)
            {
                try
                {
                    if(projectId.Contains("SCI")){
                        // 更新專案狀態名稱為「計畫書修正中 」
                        ReviewCheckListHelper.UpdateProjectStatusName(projectId, "計畫書修正中", currentUser.Account);
                        // 記錄歷程：核定中 → 計畫書修正中 
                        ReviewCheckListHelper.InsertReviewHistory(projectId, "核定中", "計畫書修正中", "提送至申請者", currentUser.Account);
                    }// TODO 正文 將專案狀態改為 計畫書修正中 ，且寄信給申請者，請他來修正計畫書。
                    else if (projectId.Contains("CUL"))
                    {
                        // 文化
                    }
                    else if (projectId.Contains("EDC"))
                    {
                        // 學校民間
                    }
                    else if (projectId.Contains("CLB"))
                    {
                        // 學校社團
                    }else if (projectId.Contains("MUL"))
                    {
                        // 多元
                    }else if (projectId.Contains("LIT"))
                    {
                        // 素養
                    }else if (projectId.Contains("ACC"))
                    {
                        // 無障礙
                    }
                    // TODO: 寄信 實作寄信給申請者功能
                    // SendNotificationEmail(projectId);
                    
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
                if (errorMessages.Count > 0)
                {
                    message += $"，{errorMessages.Count} 件失敗";
                }
                ShowMessage(message, "success");
                
                // 重新載入頁面資料
                RefreshPageData();
            }
            else
            {
                ShowMessage("所有案件處理失敗", "error");
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

    /// <summary>
    /// 重新載入頁面資料
    /// </summary>
    private void RefreshPageData()
    {
        // 觸發前端重新查詢
        string script = @"
            $(function() {
                // 觸發 Type4 重新查詢
                if ($('#type4').is(':checked')) {
                    $('#btnSearch_Type4').click();
                }
            });
        ";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "RefreshData", script, true);
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
            
            // 執行領域審查查詢
            if (category == "SCI" || string.IsNullOrEmpty(category))
            {
                results = GetSciProjectData(
                    year,
                    orgName,
                    supervisor,
                    keyword,
                    progress,
                    replyStatus,"領域審查"
                    );
            }
            // TODO 正文 這裡是 文化初審的搜尋 ， 因為只有文化有初審功能(及牽扯到多張表單，就不做成view了 請仿照 科專的搜尋 add到 results裡面)
            else if (category == "CUL")
            {
                // results = ReviewCheckListHelper.Search_Cultural_Type2(
                //     year,
                //     orgName,
                //     supervisor,
                //     keyword,
                //     progress,
                //     replyStatus,
                //     "初審");
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
                results = GetSciProjectData(
                    year,
                    orgName,
                    supervisor,
                    keyword,
                    progress,
                    replyStatus,"技術審查");
            }
            // TODO 正文 這裡是 文化複審的搜尋 ， 因為也只還有文化會有複審(及牽扯到多張表單，就不做成view了 請仿照 科專的搜尋 add到 results裡面)
            // GetSciProjectData 初審、複審是共用的 OFS_ReviewRecords也是科專、文化共用
            
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

}