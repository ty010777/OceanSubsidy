using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.App;

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
}