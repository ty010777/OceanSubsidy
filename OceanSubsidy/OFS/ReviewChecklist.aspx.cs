using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
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
                    // TODO: 載入結果公告清單
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
    /// 取得目前登入使用者資訊
    /// </summary>
    private SessionHelper.UserInfoClass GetCurrentUserInfo()
    {
        try
        {
            return SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo);
        }
        catch (Exception ex)
        {
            HandleException(ex, "取得使用者資訊時發生錯誤");
            return null;
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
            ddlYear_Type2.Items.Add(new ListItem("113", "113年"));
            ddlYear_Type2.Items.Add(new ListItem("114", "114年"));            
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

    #endregion
}