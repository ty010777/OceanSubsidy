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
               new[] { "1", "2", "3", "4", "5", "6", "7" }.Contains(type);
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
                    // TODO: 載入計畫變更審核清單
                    break;
                case "6":
                    // TODO: 載入執行計畫審核清單
                    break;
                case "7":
                    // TODO: 載入階段報告審核清單
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
            var orgOptions = ReviewCheckListHelper.GetReviewOrgOptions("資格審查");
            ddlOrg_Type1.DataSource = orgOptions;
            ddlOrg_Type1.DataTextField = "Text";
            ddlOrg_Type1.DataValueField = "Value";
            ddlOrg_Type1.DataBind();

            // 載入承辦人員選項
            var supervisorOptions = ReviewCheckListHelper.GetReviewSupervisorOptions("資格審查");
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
            var orgOptions = ReviewCheckListHelper.GetReviewOrgOptions("領域審查");
            ddlOrg_Type2.DataSource = orgOptions;
            ddlOrg_Type2.DataTextField = "Text";
            ddlOrg_Type2.DataValueField = "Value";
            ddlOrg_Type2.DataBind();

            // 載入承辦人員選項
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
            var orgOptions = ReviewCheckListHelper.GetReviewOrgOptions("技術審查");
            ddlOrg_Type3.DataSource = orgOptions;
            ddlOrg_Type3.DataTextField = "Text";
            ddlOrg_Type3.DataValueField = "Value";
            ddlOrg_Type3.DataBind();

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

            // 載入類別選項（移除「全部」選項）
            ddlCategory_Type4.Items.Add(new ListItem("科專", "SCI"));            
            ddlCategory_Type4.Items.Add(new ListItem("文化", "CUL"));            
            ddlCategory_Type4.Items.Add(new ListItem("學校/民間", "EDC"));            
            ddlCategory_Type4.Items.Add(new ListItem("學校/社團", "CLB"));            
            ddlCategory_Type4.DataTextField = "Text";
            ddlCategory_Type4.DataValueField = "Value";
            ddlCategory_Type4.DataBind();

            // 載入申請單位選項
            var orgOptions = ReviewCheckListHelper.GetReviewOrgOptions("決審核定");
            ddlOrg_Type4.DataSource = orgOptions;
            ddlOrg_Type4.DataTextField = "Text";
            ddlOrg_Type4.DataValueField = "Value";
            ddlOrg_Type4.DataBind();

            // 載入承辦人員選項
            var supervisorOptions = ReviewCheckListHelper.GetReviewSupervisorOptions("決審核定");
            ddlSupervisor_Type4.DataSource = supervisorOptions;
            ddlSupervisor_Type4.DataTextField = "Text";
            ddlSupervisor_Type4.DataValueField = "Value";
            ddlSupervisor_Type4.DataBind();
            
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

    #region 查詢事件處理

    /// <summary>
    /// Type1 資格審查/內容審查查詢按鈕點擊事件
    /// </summary>
    protected void btnSearch_Type1_Click(object sender, EventArgs e)
    {
        try
        {
            // 定義資格審查和內容審查的階段
            // 取得查詢條件
            string year = ddlYear_Type1.SelectedValue;
            string category = ddlCategory_Type1.SelectedValue;
            string status = ddlStage_Type1.SelectedValue;
            string orgName = ddlOrg_Type1.SelectedValue;
            string supervisor = ddlSupervisor_Type1.SelectedValue;
            string keyword = Request.Form["txtKeyword_Type1"] ?? "";
            List<ReviewChecklistItem> Results = new List<ReviewChecklistItem>();
            // 執行查詢
            if(category == "SCI" || category == "" ){
                Results = ReviewCheckListHelper.Search_SCI_Type1(year, status, orgName, supervisor, keyword);
            }
            
            //TODO 之後要再新增除了科專以外的搜尋 並且將這些Result 合併在一起
            // 將查詢結果轉換為 JSON 並傳遞給前端
            string jsonResults = Newtonsoft.Json.JsonConvert.SerializeObject(Results);
            
            string script = $@"
                $(function() {{
                    // 將查詢結果傳遞給前端 JavaScript
                    if (typeof window.ReviewChecklistManager !== 'undefined') {{
                        window.ReviewChecklistManager.renderSearchResults({jsonResults}, 1);
                    }} else {{
                        console.log('搜尋結果:', {jsonResults});
                    }}
                }});
            ";
            
            Page.ClientScript.RegisterStartupScript(this.GetType(), "SearchResults_Type1", script, true);
        }
        catch (Exception ex)
        {
            HandleException(ex, "執行 Type1 查詢時發生錯誤");
            
            // 顯示錯誤訊息給使用者
            string errorScript = @"
                Swal.fire({
                    title: '查詢錯誤',
                    text: '查詢時發生錯誤，請稍後再試',
                    icon: 'error',
                    confirmButtonText: '確定'
                });
            ";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "SearchError_Type1", errorScript, true);
        }
    }

    /// <summary>
    /// Type2 科專領域審查查詢按鈕點擊事件
    /// </summary>
    protected void btnSearch_Type2_Click(object sender, EventArgs e)
    {
        try
        {
            // 取得查詢條件
            string year = ddlYear_Type2.SelectedValue;
            string category = ddlCategory_Type2.SelectedValue;
            string progress = ddlProgress_Type2.SelectedValue;
            string replyStatus = ddlReplyStatus_Type2.SelectedValue;
            string orgName = ddlOrg_Type2.SelectedValue;
            string supervisor = ddlSupervisor_Type2.SelectedValue;
            string keyword = Request.Form["txtKeyword_Type2"] ?? "";
            List<ReviewChecklistItem> Results = new List<ReviewChecklistItem>();
            // 執行科專領域審查查詢
            if(category == "SCI" || string.IsNullOrEmpty(category)){
                Results = ReviewCheckListHelper.Search_SCI_Type2(
                year, 
                orgName, 
                supervisor, 
                keyword,
                progress,      // 審查進度
                replyStatus);  // 回覆狀態
            }
            // TODO : 之後要在這邊新增 文化、學校社團、學校民間 的搜尋function
            // TODO : 然後把它們合併起來 轉乘JSON檔案。
            // 將查詢結果轉換為 JSON 並傳遞給前端
            string jsonResults = Newtonsoft.Json.JsonConvert.SerializeObject(Results);
            
            string script = $@"
                $(function() {{
                    // 將查詢結果傳遞給前端 JavaScript
                    if (typeof window.ReviewChecklistManager !== 'undefined') {{
                        window.ReviewChecklistManager.renderSearchResults({jsonResults}, 2);
                    }} else {{
                        console.log('Type2 搜尋結果:', {jsonResults});
                    }}
                }});
            ";
            
            Page.ClientScript.RegisterStartupScript(this.GetType(), "SearchResults_Type2", script, true);
        }
        catch (Exception ex)
        {
            HandleException(ex, "執行 Type2 查詢時發生錯誤");
            
            // 顯示錯誤訊息給使用者
            string errorScript = @"
                Swal.fire({
                    title: '查詢錯誤',
                    text: '查詢時發生錯誤，請稍後再試',
                    icon: 'error',
                    confirmButtonText: '確定'
                });
            ";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "SearchError_Type2", errorScript, true);
        }
    }

    /// <summary>
    /// Type3 技術審查/複審查詢按鈕點擊事件
    /// </summary>
    protected void btnSearch_Type3_Click(object sender, EventArgs e)
    {
        try
        {
            // 取得查詢條件
            string year = ddlYear_Type3.SelectedValue;
            string category = ddlCategory_Type3.SelectedValue;
            string progress = ddlProgress_Type3.SelectedValue;
            string replyStatus = ddlReplyStatus_Type3.SelectedValue;
            string orgName = ddlOrg_Type3.SelectedValue;
            string supervisor = ddlSupervisor_Type3.SelectedValue;
            string keyword = Request.Form["txtKeyword_Type3"] ?? "";
            List<ReviewChecklistItem> Results = new List<ReviewChecklistItem>();
            
            // 執行技術審查查詢
            if(category == "SCI" || string.IsNullOrEmpty(category)){
                Results = ReviewCheckListHelper.Search_SCI_Type3(
                    year, 
                    orgName, 
                    supervisor, 
                    keyword,
                    progress,      // 審查進度
                    replyStatus);  // 回覆狀態
            }
            
            // TODO : 之後要在這邊新增 文化、學校社團、學校民間 的搜尋function
            // TODO : 然後把它們合併起來 轉乘JSON檔案。
            
            // 將查詢結果轉換為 JSON 並傳遞給前端
            string jsonResults = Newtonsoft.Json.JsonConvert.SerializeObject(Results);
            
            string script = $@"
                $(function() {{
                    // 將查詢結果傳遞給前端 JavaScript
                    if (typeof window.ReviewChecklistManager !== 'undefined') {{
                        window.ReviewChecklistManager.renderSearchResults({jsonResults}, 3);
                    }} else {{
                        console.log('Type3 搜尋結果:', {jsonResults});
                    }}
                }});
            ";
            
            Page.ClientScript.RegisterStartupScript(this.GetType(), "SearchResults_Type3", script, true);
        }
        catch (Exception ex)
        {
            HandleException(ex, "執行 Type3 查詢時發生錯誤");
            
            // 顯示錯誤訊息給使用者
            string errorScript = @"
                Swal.fire({
                    title: '查詢錯誤',
                    text: '查詢時發生錯誤，請稍後再試',
                    icon: 'error',
                    confirmButtonText: '確定'
                });
            ";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "SearchError_Type3", errorScript, true);
        }
    }

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
                Results = ReviewCheckListHelper.Search_SCI_Type4(
                    year, 
                    orgName, 
                    supervisor, 
                    keyword,
                    reviewGroup);     // 審查組別代碼
            }
            
            // TODO : 之後要在這邊新增 文化、學校社團、學校民間 的搜尋function
            // TODO : 然後把它們合併起來 轉乘JSON檔案。
            
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
    public static BatchApprovalResult BatchApproveType1(List<string> projectIds, string actionType, string reviewType)
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
                string fromStatus, toStatus,StatusesName;
                if (!GetStatusTransitionBySubsidyType(subsidyType, reviewType, out fromStatus, out toStatus,out StatusesName))
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
                                // 文化補助案的特殊處理（未來實作）
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
        if (projectId.Contains("GOV")) return "GOV";      // 政府機關
        if (projectId.Contains("NGO")) return "NGO";      // 非政府組織
        if (projectId.Contains("INT")) return "INT";      // 國際合作

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

                case "CUL": // 文化 (尚未實作)
                case "EDC": // 學校民間 (尚未實作)
                case "CLB": // 學校社團 (尚未實作)
                case "GOV": // 政府機關 (尚未實作)
                case "NGO": // 非政府組織 (尚未實作)
                case "INT": // 國際合作 (尚未實作)
                    // TODO: 實作其他補助案類型的狀態轉換
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
            if (reviewType == "2")
            {
                reviewStage = "領域審查";
            }
            else if (reviewType == "3")
            {
                reviewStage = "技術審查";
            }

            DataTable planData = null;
            DataTable reviewData = null;

            if (projectId.Contains("SCI"))
            {
                planData = ReviewCheckListHelper.GetSciPlanDetail(projectId);
                reviewData = ReviewCheckListHelper.GetSciReviewComments(projectId, reviewStage);
            }
            else if (projectId.Contains("CUL"))
            {
                planData = ReviewCheckListHelper.GetCulturalPlanDetail(projectId);
                reviewData = ReviewCheckListHelper.GetCulturalReviewComments(projectId, reviewStage);
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
    /// 取得審查組別選項 (AJAX 方法)
    /// </summary>
    /// <param name="category">類別</param>
    /// <returns>審查組別選項</returns>
    [WebMethod]
    public static object GetReviewGroupOptions(string category)
    {
        try
        {
            List<DropdownItem> options = new List<DropdownItem>();
            
            switch (category)
            {
                case "SCI":
                    // 載入科專審查組別選項
                    options = ReviewCheckListHelper.GetSciReviewGroupOptions();
                    break;
                    
                case "CUL":
                    // 文化類別，暫時只顯示全部 (未來可擴充)
                    options.Add(new DropdownItem { Value = "", Text = "全部" });
                    // TODO: 未來實作文化審查組別選項
                    break;
                    
                default:
                    // 其他類別 (學校民間、學校社團等) 只顯示全部
                    options.Add(new DropdownItem { Value = "", Text = "全部" });
                    break;
            }

            return new { success = true, options = options };
        }
        catch (Exception ex)
        {
            return new { success = false, message = ex.Message };
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
                    // 更新專案狀態
                    ReviewCheckListHelper.UpdateProjectStatusInDatabase(projectId, toStatus, userName, StatusesName);
                    
                    // 根據補助案類型決定歷程記錄方式
                    if (projectId.Contains("SCI"))
                    {
                        // 科專
                        RecordSciReviewHistory(projectId, fromStatus, toStatus, actionType, userName);
                    }
                    else if (projectId.Contains("CUL"))
                    {
                        // 文化
                        RecordCulturalReviewHistory(projectId, fromStatus, toStatus, actionType, userName);
                    }
                    else if (projectId.Contains("EDC"))
                    {
                        // 學校民間
                        RecordEducationReviewHistory(projectId, fromStatus, toStatus, actionType, userName);
                    }
                    else if (projectId.Contains("CLB"))
                    {
                        // 學校社團
                        RecordClubReviewHistory(projectId, fromStatus, toStatus, actionType, userName);
                    }
                    else
                    {
                        // 其他類型或未知類型
                        RecordGeneralReviewHistory(projectId, fromStatus, toStatus, actionType, userName);
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
                // 一般跨階段轉換：只記錄階段轉換
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
    /// 文化補助案的審查歷程記錄
    /// </summary>
    private static void RecordCulturalReviewHistory(string projectId, string fromStatus, string toStatus, 
        string actionType, string userAccount)
    {
        // TODO: 實作文化補助案特殊的歷程記錄邏輯
        // 目前先使用一般記錄方式
        RecordGeneralReviewHistory(projectId, fromStatus, toStatus, actionType, userAccount);
    }

    /// <summary>
    /// 學校民間補助案的審查歷程記錄
    /// </summary>
    private static void RecordEducationReviewHistory(string projectId, string fromStatus, string toStatus, 
        string actionType, string userAccount)
    {
        // TODO: 實作學校民間補助案特殊的歷程記錄邏輯
        // 目前先使用一般記錄方式
        RecordGeneralReviewHistory(projectId, fromStatus, toStatus, actionType, userAccount);
    }

    /// <summary>
    /// 學校社團補助案的審查歷程記錄
    /// </summary>
    private static void RecordClubReviewHistory(string projectId, string fromStatus, string toStatus, 
        string actionType, string userAccount)
    {
        // TODO: 實作學校社團補助案特殊的歷程記錄邏輯
        // 目前先使用一般記錄方式
        RecordGeneralReviewHistory(projectId, fromStatus, toStatus, actionType, userAccount);
    }

    /// <summary>
    /// 一般補助案的審查歷程記錄
    /// </summary>
    private static void RecordGeneralReviewHistory(string projectId, string fromStatus, string toStatus, 
        string actionType, string userName)
    {
        if (fromStatus != toStatus)
        {
            // 跨階段轉換：只記錄階段轉換
            ReviewCheckListHelper.InsertReviewHistory(projectId, fromStatus, toStatus, actionType, userName);
        }
        else
        {
            // 同階段內的狀態變更
            ReviewCheckListHelper.InsertReviewHistory(projectId, fromStatus, toStatus, actionType, userName);
        }
    }

    /// <summary>
    /// 提送至申請者按鈕點擊事件
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
                    // 更新專案狀態名稱為「計畫書修正中 」
                    ReviewCheckListHelper.UpdateProjectStatusName(projectId, "計畫書修正中", currentUser.Account);
                    
                    // 記錄歷程：核定中 → 計畫書修正中 
                    ReviewCheckListHelper.InsertReviewHistory(projectId, "核定中", "計畫書修正中", "提送至申請者", currentUser.Account);
                    
                    // TODO: 實作寄信功能
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
}