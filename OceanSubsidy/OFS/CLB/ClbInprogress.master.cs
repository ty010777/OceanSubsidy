using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.App;
using System.Data;

public partial class OFS_CLB_ClbInprogress : System.Web.UI.MasterPage
{   
    /// <summary>
    /// 目前處理的計畫ID
    /// </summary>
    protected string ProjectID => Request.QueryString["ProjectID"];

    /// <summary>
    /// 設定計畫資訊文字
    /// </summary>
    public string ProjectInfoText
    {
        get { return litProjectInfo.Text; }
        set { litProjectInfo.Text = value; }
    }

    /// <summary>
    /// 設定待辦狀態文字 (已棄用，改用動態控制項)
    /// </summary>
    public string TodoStatusText
    {
        get { return ""; }
        set { /* 不再使用 */ }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        // 驗證專案擁有權
        if (!ValidateProjectOwnership(ProjectID))
        {
            return; // 如果不是擁有者，已經導向到清單頁
        }

        // 設定階段狀態的 active class
        SetActiveStep();
        SetProjectInfoToMaster();

        // 檢查並顯示計畫狀態提示
        CheckAndShowStatusAlert();
    }
    
    /// <summary>
    /// 根據當前頁面設定 active 狀態
    /// </summary>
    private void SetActiveStep()
    {
        string currentPage = System.IO.Path.GetFileName(Request.PhysicalPath);
        
        // 移除預設的 active class
        string stepLineHtml = GetStepLineHtml(currentPage);
        
        // 用 Literal 控制項來輸出動態 HTML
        var stepLinePlaceholder = FindControl("stepLinePlaceholder") as PlaceHolder;
        if (stepLinePlaceholder != null)
        {
            stepLinePlaceholder.Controls.Clear();
            stepLinePlaceholder.Controls.Add(new LiteralControl(stepLineHtml));
        }
    }
    
    /// <summary>
    /// 產生階段狀態的 HTML
    /// </summary>
    private string GetStepLineHtml(string currentPage)
    {
        string approvedClass = currentPage.Contains("ClbApproved.aspx") ? "step-line-item active" : "step-line-item";
        string stageReportClass = currentPage.Contains("ClbStageReport.aspx") ? "step-line-item active" : "step-line-item";
        string paymentClass = currentPage.Contains("ClbPayment.aspx") ? "step-line-item active" : "step-line-item";
        
        string approvedStatus = currentPage.Contains("ClbApproved.aspx") ? 
            "<div class=\"tag tag-white\">編輯中</div>" : "";
        string stageReportStatus = currentPage.Contains("ClbStageReport.aspx") ? 
            "<div class=\"tag tag-white\">編輯中</div>" : "";
        string paymentStatus = currentPage.Contains("ClbPayment.aspx") ? 
            "<div class=\"tag tag-white\">編輯中</div>" : "";
            
        return $@"
            <div class=""step-line"" style=""gap:24px"">
                <div class=""{approvedClass}"" onclick=""navigateWithProjectID('{ResolveUrl("~/OFS/CLB/ClbApproved.aspx")}')"">
                    <div class=""step-line-item-title"">核定計畫</div>
                    {approvedStatus}
                </div>
                <div class=""{stageReportClass}"" onclick=""navigateWithProjectID('{ResolveUrl("~/OFS/CLB/ClbStageReport.aspx")}')"">
                    <div class=""step-line-item-title"">階段報告</div>
                    {stageReportStatus}
                </div>
                <div class=""{paymentClass}"" onclick=""navigateWithProjectID('{ResolveUrl("~/OFS/CLB/ClbPayment.aspx")}')"">
                    <div class=""step-line-item-title"">請款核銷</div>
                    {paymentStatus}
                </div>
            </div>";
    }
     
    /// <summary>
    /// 設定計畫資訊到 Master Page
    /// </summary>
    private void SetProjectInfoToMaster()
    {
        try
        {
            // 從 OFS_CLB_Project_Main 取得計畫基本資訊
            var projectMain = OFS_ClbApplicationHelper.GetProjectMainData(ProjectID);
            
            // 從 OFS_CLB_Application_Basic 取得計畫名稱
            var applicationBasic = OFS_ClbApplicationHelper.GetBasicData(ProjectID);
            
            if (projectMain != null)
            {
                // 從 Application_Basic 取得計畫名稱
                string projectName = applicationBasic?.ProjectNameTw ?? "計畫名稱";
                litProjectInfo.Text = $"{ProjectID}/{projectName}";
                
                // 產生代辦事項控制項
                GenerateTodoControls(projectMain);
            }
            
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"設定計畫資訊時發生錯誤: {ex.Message}");
        }
    }

    /// <summary>
    /// 產生代辦事項控制項 (基於 OFS_TaskQueue 資料表)
    /// </summary>
    /// <param name="projectMain">專案主檔資料</param>
    private void GenerateTodoControls(GS.OCA_OceanSubsidy.Entity.OFS_CLB_Project_Main projectMain)
    {
        // 清空現有內容
        var placeholder = phTodoItems;
        if (placeholder == null) return;
        
        placeholder.Controls.Clear();

        List<string> todoItems = new List<string>();
        
        try
        {
            // 從 OFS_TaskQueue 取得待辦事項（只顯示第一筆）
            var dt = OFS_TaskQueueHelper.GetProjectTodoTasks(ProjectID);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0]; // 只取第一筆
                string taskNameEn = row["TaskNameEn"]?.ToString();
                string taskName = row["TaskName"]?.ToString();
                
                // CLB 類型的待辦事項處理
                todoItems.Add($"待辦事項:{taskName}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得待辦事項時發生錯誤: {ex.Message}");
        }

        // 為每個代辦事項建立獨立的 span 控制項
        foreach (string item in todoItems)
        {
            var span = new LiteralControl("<span class=\"tag tag-pale-green\">");
            var literal = new Literal();
            literal.Text = item;
            var closeSpan = new LiteralControl("</span>");
            
            placeholder.Controls.Add(span);
            placeholder.Controls.Add(literal);
            placeholder.Controls.Add(closeSpan);
        }
    }

    /// <summary>
    /// 檢查並顯示計畫狀態提示
    /// </summary>
    private void CheckAndShowStatusAlert()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return;
            }

            // 從資料庫取得計畫主資料
            var projectMain = OFS_ClbApplicationHelper.GetProjectMainData(ProjectID);

            if (projectMain == null)
            {
                return;
            }

            // 檢查 StatusesName 並顯示對應提示
            if (!string.IsNullOrEmpty(projectMain.StatusesName))
            {
                if (projectMain.StatusesName == "已終止")
                {
                    pnlStatusAlert.Visible = true;
                    lblStatusMessage.Text = "計畫已終止";
                }
                else if (projectMain.StatusesName == "已結案")
                {
                    pnlStatusAlert.Visible = true;
                    lblStatusMessage.Text = "計畫已結案";
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"檢查計畫狀態提示時發生錯誤: {ex.Message}");
        }
    }
    /// <summary>
    /// 驗證專案擁有權 - 確認使用者是否為專案的擁有者
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <returns>true: 是擁有者或驗證通過, false: 不是擁有者</returns>
    private bool ValidateProjectOwnership(string projectID)
    {
        try
        {
            // 如果沒有 projectID，允許新建專案
            if (string.IsNullOrEmpty(projectID))
            {
                return true; // ClbApplication.aspx 允許新建專案
            }

            // 取得當前使用者資訊
            var currentUser = GetCurrentUserInfo();
            if (currentUser == null || string.IsNullOrEmpty(currentUser.Account))
            {
                // 沒有登入資訊，導向回清單頁
                RedirectToApplicationChecklist("未找到使用者登入資訊");
                return false;
            }

            // 取得專案資訊
            var projectData = OFS_ClbApplicationHelper.GetProjectMainData(projectID);
            if (projectData == null)
            {
                // 專案不存在，導向回清單頁
                RedirectToApplicationChecklist("專案不存在");
                return false;
            }

            // 比較使用者帳號，或檢查是否為主管單位人員
            if (projectData.UserAccount != currentUser.Account && !IsSupervisorRole(currentUser))
            {
                // 不是專案擁有者且不是主管單位人員，導向回清單頁
                RedirectToApplicationChecklist("無法檢視不是自己的專案");
                return false;
            }

            return true; // 驗證通過
        }
        catch (Exception ex)
        {
            // 發生錯誤時記錄並導向回清單頁
            HandleException(ex, "驗證專案擁有權時發生錯誤");
            RedirectToApplicationChecklist("系統錯誤，請稍後再試");
            return false;
        }
    }
    /// <summary>
    /// 直接導向到申請清單頁面（不顯示訊息）
    /// </summary>
    /// <param name="message">錯誤訊息（記錄用，不顯示給使用者）</param>
    private void RedirectToApplicationChecklist(string message)
    {
        // 記錄錯誤訊息到 Debug
        System.Diagnostics.Debug.WriteLine($"RedirectToApplicationChecklist: {message}");

        // 直接跳轉到申請清單頁面
        string redirectUrl = Page.ResolveUrl("~/OFS/inprogressList.aspx");
        Response.Redirect(redirectUrl, false);
        Context.ApplicationInstance.CompleteRequest();
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
    /// 檢查使用者是否為主管單位人員（主管單位人員、主管單位窗口、系統管理者）
    /// </summary>
    /// <param name="userInfo">使用者資訊</param>
    /// <returns>true: 是主管單位人員, false: 不是主管單位人員</returns>
    private bool IsSupervisorRole(SessionHelper.UserInfoClass userInfo)
    {
        if (userInfo == null || userInfo.OFS_RoleName == null)
        {
            return false;
        }

        try
        {
            // 定義主管單位相關角色名稱
            var supervisorRoles = new[] { "主管單位人員", "主管單位窗口", "系統管理者" };

            // 檢查使用者的 OFS_RoleName 陣列中是否包含任一主管單位角色
            foreach (string roleName in userInfo.OFS_RoleName)
            {
                if (!string.IsNullOrEmpty(roleName) && supervisorRoles.Contains(roleName))
                {
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            HandleException(ex, "檢查主管單位角色時發生錯誤");
            return false;
        }
    }
    /// <summary>
    /// 例外處理
    /// </summary>
    private void HandleException(Exception ex, string context)
    {
        System.Diagnostics.Debug.WriteLine($"{context}: {ex.Message}");
        System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
    }

}