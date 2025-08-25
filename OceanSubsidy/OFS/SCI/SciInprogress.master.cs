using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.App;

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
    /// 產生代辦事項控制項
    /// </summary>
    /// <param name="projectMain">專案主檔資料</param>
    private void GenerateTodoControls(GS.OCA_OceanSubsidy.Entity.OFS_SCI_Project_Main projectMain)
    {
        // 清空現有內容
        var placeholder = phTodoItems;
        if (placeholder == null) return;
        
        placeholder.Controls.Clear();

        List<string> todoItems = new List<string>();
        bool hasToDoitems = false;
        
        // 1. 檢查契約資料
        if ((string.IsNullOrWhiteSpace(projectMain.PubNumber) || projectMain.ContractDate == null) && hasToDoitems == false )
        {
            todoItems.Add("待辦事項:填寫契約資料");
            hasToDoitems = true;
        }

        // 2. 檢查第一期請款
        if (!OFS_SciApplicationHelper.IsFirstPaymentPending(ProjectID) && hasToDoitems == false)
        {
            todoItems.Add("待辦事項:第一期請款");
            hasToDoitems = true;

        }

        // 3. 檢查預定進度
        if ((projectMain.MidtermExamDate == null || projectMain.FinalExamDate == null)&& hasToDoitems == false)
        {
            todoItems.Add("待辦事項:填寫預定進度");
            hasToDoitems = true;

        }
        
        if(hasToDoitems == false)
        {
            // 4. 檢查每月進度 - 如果今天已過當月20號，且當月進度資料的ActProgress沒有值
            CheckMonthlyProgressTodo(ProjectID, todoItems);
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

}