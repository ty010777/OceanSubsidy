using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.App;
using GS.OCA_OceanSubsidy.Operation.OFS;
using System.Data;

public partial class OFS_SCI_SciInprogress : System.Web.UI.MasterPage
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
        string approvedClass = currentPage.Contains("SciInprogress_Approved.aspx") ? "step-line-item active" : "step-line-item";
        string contractClass = currentPage.Contains("SciInprogress_Contract.aspx") ? "step-line-item active" : "step-line-item";
        
        string approvedStatus = currentPage.Contains("SciInprogress_Approved.aspx") ? 
            "<div class=\"tag tag-white\">編輯中</div>" : "";
        string contractStatus = currentPage.Contains("SciInprogress_Contract.aspx") ? 
            "<div class=\"tag tag-white\">編輯中</div>" : "";
            
        return $@"
            <div class=""step-line"" style=""gap:24px"">
                <div class=""{approvedClass}"" onclick=""navigateWithProjectID('{ResolveUrl("~/OFS/SCI/SciInprogress_Approved.aspx")}')"">
                    <div class=""step-line-item-title"">核定計畫</div>
                    {approvedStatus}
                </div>
                <div class=""{contractClass}"" onclick=""navigateWithProjectID('{ResolveUrl("~/OFS/SCI/SciInprogress_Contract.aspx")}')"">
                    <div class=""step-line-item-title"">契約資料</div>
                    {contractStatus}
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
            // 從 OFS_SCI_Project_Main 取得計畫基本資訊
            var projectMain = OFS_SciApplicationHelper.getVersionByProjectID(ProjectID);
            
            // 從 OFS_SCI_Application_Main 取得計畫名稱
            var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(ProjectID);
            
            if (projectMain != null)
            {
                // 從 Application_Main 取得計畫名稱
                string projectName = applicationMain?.ProjectNameTw ?? "計畫名稱";
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
    private void GenerateTodoControls(GS.OCA_OceanSubsidy.Entity.OFS_SCI_Project_Main projectMain)
    {
        // 清空現有內容
        var placeholder = phTodoItems;
        if (placeholder == null) return;

        placeholder.Controls.Clear();

        // 檢查計畫狀態
        if (projectMain.StatusesName == "已終止")
        {
            // 顯示「計畫已終止」標籤
            var statusTag = new LiteralControl("<span class=\"tag tag-light-pink\">計畫已終止</span>");
            placeholder.Controls.Add(statusTag);
        }
        else if (projectMain.StatusesName == "已結案")
        {
            // 顯示「計畫已結案」標籤
            var statusTag = new LiteralControl("<span class=\"tag tag-light-gray\">計畫已結案</span>");
            placeholder.Controls.Add(statusTag);
        }
        else
        {
            // 顯示待辦事項
            try
            {
                // 從 OFS_TaskQueue 取得待辦事項（只顯示第一筆）
                var dt = OFS_TaskQueueHelper.GetProjectTodoTasks(ProjectID);
                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0]; // 只取第一筆
                    string taskNameEn = row["TaskNameEn"]?.ToString();
                    string taskName = row["TaskName"]?.ToString();

                    string todoText = "";
                    if (taskNameEn == "MonthlyReport")
                    {
                        // 每月進度報告需要額外判斷具體月份（取最早的月份）
                        var monthlyTodoItems = new List<string>();
                        CheckMonthlyProgressTodo(ProjectID, monthlyTodoItems);

                        // 只顯示第一筆月份（最早的）
                        if (monthlyTodoItems.Count > 0)
                        {
                            todoText = monthlyTodoItems[0];
                        }
                    }
                    else
                    {
                        todoText = $"待辦事項:{taskName}";
                    }

                    // 建立待辦事項標籤
                    if (!string.IsNullOrEmpty(todoText))
                    {
                        var span = new LiteralControl("<span class=\"tag tag-pale-green\">");
                        var literal = new Literal();
                        literal.Text = todoText;
                        var closeSpan = new LiteralControl("</span>");

                        placeholder.Controls.Add(span);
                        placeholder.Controls.Add(literal);
                        placeholder.Controls.Add(closeSpan);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"取得待辦事項時發生錯誤: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// 檢查每月進度填寫的代辦事項
    /// </summary>
    /// <param name="projectID">計畫ID</param>
    /// <param name="todoItems">代辦事項列表</param>
    private void CheckMonthlyProgressTodo(string projectID, List<string> todoItems)
    {
        try
        {
            // 取得申請表主檔資料，獲取計畫期間
            var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);
            
            if (applicationMain == null || !applicationMain.StartTime.HasValue || !applicationMain.EndTime.HasValue)
            {
                return; // 沒有計畫期間資料，無法檢查
            }
            
            DateTime startTime = applicationMain.StartTime.Value;
            DateTime endTime = applicationMain.EndTime.Value;
            DateTime today = DateTime.Today;
            
            // 計算檢查範圍：從 StartMonth 到 min(當前年月, EndMonth)
            DateTime startMonth = new DateTime(startTime.Year, startTime.Month, 1);
            DateTime endMonth = new DateTime(endTime.Year, endTime.Month, 1);
            DateTime currentMonth = new DateTime(today.Year, today.Month, 1);
            
            // 取最小的月份作為檢查結束點
            DateTime checkEndMonth = currentMonth < endMonth ? currentMonth : endMonth;
            
            // 迴圈檢查每個月份
            DateTime checkMonth = startMonth;
            while (checkMonth <= checkEndMonth)
            {
                // 將月份轉換為民國年月格式 (例如: 114年8月)
                int minguoYear = DateTimeHelper.GregorianYearToMinguo(checkMonth.Year);
                string monthString = $"{minguoYear}年{checkMonth.Month}月";
                
                bool shouldCheck = true;
                
                // 如果檢查的月份是當月，需要額外判斷是否已過20號
                if (checkMonth.Year == today.Year && checkMonth.Month == today.Month)
                {
                    if (today.Day <= 20)
                    {
                        shouldCheck = false; // 當月但還沒到20號，不檢查
                    }
                }
                
                if (shouldCheck)
                {
                    // 查詢該月份的進度資料
                    var monthProgress = OFS_PreMonthProgressHelper.GetPreMonthProgressByProjectIdAndMonth(projectID, monthString);
                    
                    // 檢查 ActProgress 是否為 null 或空值
                    if (monthProgress == null || !monthProgress.ActProgress.HasValue)
                    {
                        // 新增代辦事項
                        todoItems.Add($"待辦事項:請填寫{monthString}進度");
                        break;
                    }
                }
                
                // 移到下一個月
                checkMonth = checkMonth.AddMonths(1);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"檢查每月進度代辦事項時發生錯誤: {ex.Message}");
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
            // 如果沒有 projectID，導向回清單頁
            if (string.IsNullOrEmpty(projectID))
            {
                RedirectToInprogressList("缺少專案編號");
                return false;
            }

            // 取得當前使用者資訊
            var currentUser = GetCurrentUserInfo();
            if (currentUser == null || string.IsNullOrEmpty(currentUser.Account))
            {
                // 沒有登入資訊，導向回清單頁
                RedirectToInprogressList("未找到使用者登入資訊");
                return false;
            }

            // 取得專案資訊
            var projectData = OFS_SciApplicationHelper.getVersionByProjectID(projectID);
            if (projectData == null)
            {
                // 專案不存在，導向回清單頁
                RedirectToInprogressList("專案不存在");
                return false;
            }

            // 比較使用者帳號，或檢查是否為主管單位人員
            if (projectData.UserAccount != currentUser.Account && !IsSupervisorRole(currentUser))
            {
                // 不是專案擁有者且不是主管單位人員，導向回清單頁
                RedirectToInprogressList("無法檢視不是自己的專案");
                return false;
            }

            return true; // 驗證通過
        }
        catch (Exception ex)
        {
            // 發生錯誤時記錄並導向回清單頁
            System.Diagnostics.Debug.WriteLine($"驗證專案擁有權時發生錯誤: {ex.Message}");
            RedirectToInprogressList("系統錯誤，請稍後再試");
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
            System.Diagnostics.Debug.WriteLine($"取得使用者資訊時發生錯誤: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"檢查主管單位角色時發生錯誤: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 直接導向到執行中清單頁面（不顯示訊息）
    /// </summary>
    /// <param name="message">錯誤訊息（記錄用，不顯示給使用者）</param>
    private void RedirectToInprogressList(string message)
    {
        // 記錄錯誤訊息到 Debug
        System.Diagnostics.Debug.WriteLine($"RedirectToInprogressList: {message}");

        // 直接跳轉到執行中清單頁面
        string redirectUrl = Page.ResolveUrl("~/OFS/inprogressList.aspx");
        Response.Redirect(redirectUrl, false);
        Context.ApplicationInstance.CompleteRequest();
    }

}