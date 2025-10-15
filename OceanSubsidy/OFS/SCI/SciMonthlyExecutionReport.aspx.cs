using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.App;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Operation.OFS;

public partial class OFS_SCI_SciMonthlyExecutionReport : System.Web.UI.Page
{
    /// <summary>
    /// 目前處理的計畫ID
    /// </summary>
    protected string ProjectID => Request.QueryString["ProjectID"];

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // 檢查是否有計畫ID
            if (string.IsNullOrEmpty(ProjectID))
            {
                Response.Redirect("~/OFS/inprogressList.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // 初始化頁面
                InitializePage();
            }
        }
        catch (Exception ex)
        {
            // 例外處理
            System.Diagnostics.Debug.WriteLine($"頁面載入時發生錯誤: {ex.Message}");
        }
    }

    /// <summary>
    /// 初始化頁面
    /// </summary>
    private void InitializePage()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return;
            }
            
            // 載入基本資料
            LoadBasicData();
            
            // 產生動態月份時間軸
            GenerateTimeline();
            
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"初始化頁面時發生錯誤: {ex.Message}");
        }
    }
    
    
    /// <summary>
    /// 載入基本資料
    /// </summary>
    private void LoadBasicData()
    {
        try
        {
            // 從 OFS_SCI_Application_Main 取得計畫資料
            var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(ProjectID);
            
            if (applicationMain != null)
            {
                // 設定當前月份 (預設為第一個月)
                var monthList = GenerateMonthList(applicationMain.StartTime, applicationMain.EndTime);
                if (monthList.Count > 0)
                {
                    currentMonth.Text = monthList[0];
                    currentMonth2.Text = monthList[0];
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"載入基本資料時發生錯誤: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 根據開始和結束時間產生月份列表
    /// </summary>
    private List<string> GenerateMonthList(DateTime? startTime, DateTime? endTime)
    {
        var monthList = new List<string>();
        
        try
        {
            if (!startTime.HasValue || !endTime.HasValue)
            {
                return monthList;
            }
            
            var current = new DateTime(startTime.Value.Year, startTime.Value.Month, 1);
            var end = new DateTime(endTime.Value.Year, endTime.Value.Month, 1);
            
            while (current <= end)
            {
                // 轉換為民國年月格式
                string monthDisplay = DateTimeHelper.ToMinguoDate(current);
                // 只取年月部分 (例如：114年4月)
                string[] parts = monthDisplay.Split('/');
                if (parts.Length >= 2)
                {
                    monthDisplay = $"{parts[0]}年{int.Parse(parts[1])}月";
                }
                
                monthList.Add(monthDisplay);
                
                // 移動到下個月
                current = current.AddMonths(1);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"產生月份列表時發生錯誤: {ex.Message}");
        }
        
        return monthList;
    }
    
    /// <summary>
    /// 產生動態時間軸
    /// </summary>
    private void GenerateTimeline()
    {
        try
        {
            // 從 OFS_SCI_Application_Main 取得計畫資料
            var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(ProjectID);
            
            if (applicationMain == null || !applicationMain.StartTime.HasValue || !applicationMain.EndTime.HasValue)
            {
                GenerateEmptyTimeline();
                return;
            }
            
            // 產生月份列表
            var monthList = GenerateMonthList(applicationMain.StartTime, applicationMain.EndTime);
            
            if (monthList.Count == 0)
            {
                GenerateEmptyTimeline();
                return;
            }
            
            // 依年份分組
            var monthsByYear = monthList
                .GroupBy(m => m.Split('年')[0] + "年")
                .ToList();
            
            // 取得當前民國年月
            DateTime currentDate = DateTime.Now;
            string currentMinguoMonth = DateTimeHelper.ToMinguoDate(currentDate);
            string[] currentParts = currentMinguoMonth.Split('/');
            string currentMonthDisplay = "";
            if (currentParts.Length >= 2)
            {
                currentMonthDisplay = $"{currentParts[0]}年{int.Parse(currentParts[1])}月";
            }
            
            // 產生時間軸HTML (使用現有的CSS類別)
            string timelineHtml = @"
                <div class='horizontal-scrollable'>
                    <button class='btn-control btn-prev' role='button' disabled=''><i class='fas fa-angle-left'></i></button>
                    <ul class='timeline'>";
            
            foreach (var yearGroup in monthsByYear)
            {
                string year = yearGroup.Key;
                var months = yearGroup.ToList();
                
                timelineHtml += $@"
                    <li>
                        <span class='year'>{System.Web.HttpUtility.HtmlEncode(year)}</span>
                        <ul class='month'>";
                
                for (int i = 0; i < months.Count; i++)
                {
                    string month = months[i];
                    string monthOnly = month.Split('年')[1]; // 取得「5月」部分
                    
                    // 判斷月份狀態
                    string cssClass = "";
                    bool isFirstMonth = (monthsByYear.First() == yearGroup && i == 0);
                    bool isDisabled = IsMonthDisabled(month, currentMonthDisplay);
                    
                    if (isDisabled)
                    {
                        cssClass = "disabled";
                    }
                    else if (isFirstMonth)
                    {
                        cssClass = "active";
                    }
                    
                    timelineHtml += string.Format(@"
                        <li class='{0}'><a href='#' onclick='switchMonth(""{1}"")'>{2}</a></li>", 
                        cssClass, 
                        System.Web.HttpUtility.HtmlAttributeEncode(month), 
                        System.Web.HttpUtility.HtmlEncode(monthOnly));
                }
                
                timelineHtml += @"
                        </ul>
                    </li>";
            }
            
            timelineHtml += @"
                    </ul>
                    <button class='btn-control btn-next' role='button'><i class='fas fa-angle-right'></i></button>
                </div>";
            
            // 設定到容器
            timelineContainer.Controls.Clear();
            timelineContainer.Controls.Add(new System.Web.UI.LiteralControl(timelineHtml));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"產生時間軸時發生錯誤: {ex.Message}");
            GenerateEmptyTimeline();
        }
    }
    
    /// <summary>
    /// 判斷月份是否應該設為 disabled (未到期)
    /// </summary>
    private bool IsMonthDisabled(string targetMonth, string currentMonth)
    {
        try
        {
            if (string.IsNullOrEmpty(currentMonth))
            {
                return false;
            }
            
            // 解析目標月份 (例如：114年5月)
            string[] targetParts = targetMonth.Replace("年", "/").Replace("月", "").Split('/');
            if (targetParts.Length < 2) return false;
            
            int targetYear = int.Parse(targetParts[0]);
            int targetMonthNum = int.Parse(targetParts[1]);
            
            // 解析當前月份
            string[] currentParts = currentMonth.Replace("年", "/").Replace("月", "").Split('/');
            if (currentParts.Length < 2) return false;
            
            int currentYear = int.Parse(currentParts[0]);
            int currentMonthNum = int.Parse(currentParts[1]);
            
            // 比較年月，如果目標月份大於當前月份則為 disabled
            if (targetYear > currentYear)
            {
                return true;
            }
            else if (targetYear == currentYear && targetMonthNum > currentMonthNum)
            {
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"判斷月份是否 disabled 時發生錯誤: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 產生空白時間軸
    /// </summary>
    private void GenerateEmptyTimeline()
    {
        string emptyTimelineHtml = @"
            <div class='horizontal-scrollable'>
                <div class='text-center text-muted py-4'>尚未設定計畫執行期程</div>
            </div>";
        
        timelineContainer.Controls.Clear();
        timelineContainer.Controls.Add(new System.Web.UI.LiteralControl(emptyTimelineHtml));
    }
    
    /// <summary>
    /// AJAX 方法：載入指定月份的資料
    /// </summary>
    [System.Web.Services.WebMethod]
    public static object LoadMonthData(string projectID, string month)
    {
        try
        {
            if (string.IsNullOrEmpty(projectID) || string.IsNullOrEmpty(month))
            {
                return new { success = false, message = "參數不完整" };
            }

            // 從 OFS_SCI_PreMonthProgress 載入該月份的資料
            var progressData = OFS_PreMonthProgressHelper.GetPreMonthProgressByProjectIdAndMonth(projectID, month);
            
            // 計算該月份應顯示的累計執行經費
            var (displaySubsidy, displayCoop, displayTotal) = OFS_PreMonthProgressHelper.GetDisplayBudgetForMonth(projectID, month);
            
            // 從 OFS_SCI_WorkSch_CheckStandard 載入該月份的查核點資料
            var checkStandards = OFS_SciWorkSchHelper.GetCheckStandardsByProjectIdAndMonth(projectID, month);
            
            // 計算整個專案的年度目標達成率
            var achievementRate = OFS_SciWorkSchHelper.GetProjectAchievementRate(projectID);
            
            // 將查核標準轉換為前端需要的格式
            var checkPoints = checkStandards.Select(cs => {
                System.Diagnostics.Debug.WriteLine($"載入查核點: Id={cs.Id}, SerialNumber={cs.SerialNumber}, IsFinish={cs.IsFinish}, DelayReason={cs.DelayReason}, ImprovedWay={cs.ImprovedWay}, ActFinishTime={cs.ActFinishTime}");
                return new
                {
                    id = cs.Id, // 資料庫主鍵ID，用於更新操作
                    month = month,
                    checkDescription = (cs.SerialNumber ?? "") + "：" + (cs.CheckDescription ?? ""), // SerialNumber + ： + CheckDescription
                    workItem = cs.WorkItem ?? "",
                    serialNumber = cs.SerialNumber ?? "",
                    plannedFinishDate = cs.PlannedFinishDate?.ToString("yyyy-MM-dd") ?? "",
                    isFinish = cs.IsFinish ?? 0, // 從資料庫載入 IsFinish 狀態
                    delayReason = cs.DelayReason ?? "",
                    improvement = cs.ImprovedWay ?? "",
                    actFinishTime = cs.ActFinishTime?.ToString("yyyy-MM-dd") ?? "", // 實際完成時間
                    plannedProgress = progressData?.PreProgress?.ToString() ?? ""
                };
            }).ToList();
            
            // 如果沒有查核點，至少回傳一個空的項目
            if (checkPoints.Count == 0)
            {
                checkPoints.Add(new
                {
                    id=0, // 假設一個虛擬ID
                    month = month,
                    checkDescription = "本月無查核點",
                    workItem = "",
                    serialNumber = "",
                    plannedFinishDate = "",
                    isFinish = 3, // 無查核點視為完成
                    delayReason = "",
                    improvement = "",
                    actFinishTime = "", // 實際完成時間為空
                    plannedProgress = progressData?.PreProgress?.ToString() ?? "",
                });
            }
            
            var result = new
            {
                success = true,
                progressData = new
                {
                    month = month,
                    PreWorkAbstract = progressData?.PreWorkAbstract ?? "",
                    ActWorkAbstract = progressData?.ActWorkAbstract ?? "",
                    PreProgress = progressData?.PreProgress?.ToString() ?? "",
                    ActProgress = progressData?.ActProgress?.ToString() ?? "",
                    MonthlySubsidy = displaySubsidy.ToString("N0"), // 使用累計邏輯計算的補助金額
                    MonthlyCoop = displayCoop.ToString("N0"),       // 使用累計邏輯計算的配合款
                    MonthlyTotal = displayTotal.ToString("N0")     // 使用累計邏輯計算的總額
                },
                checkPoints = checkPoints,
                achievementRate = achievementRate.ToString("F2") + "%" // 年度目標達成率，格式化為百分比
            };

            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadMonthData 發生錯誤: {ex.Message}");
            return new { success = false, message = "載入資料時發生錯誤：" + ex.Message };
        }
    }
    
    /// <summary>
    /// 取得預定進度資料（工作摘要、預定進度百分比）
    /// </summary>
    private static dynamic GetPlannedProgressData(string projectID, string month)
    {
        try
        {
            // 從 OFS_SCI_PreMonthProgress 取得預定工作摘要和預定進度
            var plannedProgress = OFS_PreMonthProgressHelper.GetPreMonthProgressByProjectIdAndMonth(projectID, month);
            
            if (plannedProgress != null)
            {
                return new
                {
                    PreWorkAbstract = plannedProgress.PreWorkAbstract ?? "",
                    PreProgress = plannedProgress.PreProgress
                };
            }
            
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetPlannedProgressData 發生錯誤: {ex.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 取得查核點資料
    /// </summary>
    private static List<object> GetCheckPointData(string projectID, string month)
    {
        var checkPoints = new List<object>();
        
        try
        {
            // 從 OFS_SCI_PreMonthProgress 取得查核點描述
            var progressData = OFS_PreMonthProgressHelper.GetPreMonthProgressByProjectIdAndMonth(projectID, month);
            
            if (progressData != null && !string.IsNullOrEmpty(progressData.CheckDescription))
            {
                // 將查核點描述分割成多個項目（假設用換行或分號分割）
                var checkDescriptions = progressData.CheckDescription
                    .Split(new char[] { '\n', '\r', ';' }, StringSplitOptions.RemoveEmptyEntries);
                
                for (int i = 0; i < checkDescriptions.Length; i++)
                {
                    checkPoints.Add(new
                    {
                        month = month,
                        checkDescription = checkDescriptions[i].Trim(),
                        completionStatus = "incomplete", // 預設為未完成
                        delayReason = "",
                        improvement = "",
                        plannedProgress = progressData.PreProgress?.ToString() ?? ""
                    });
                }
            }
            
            // 如果沒有查核點，至少回傳一個空的項目
            if (checkPoints.Count == 0)
            {
                checkPoints.Add(new
                {
                    month = month,
                    checkDescription = "本月無查核點",
                    completionStatus = "complete",
                    delayReason = "",
                    improvement = "",
                    plannedProgress = progressData?.PreProgress?.ToString() ?? ""
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetCheckPointData 發生錯誤: {ex.Message}");
            
            // 發生錯誤時回傳預設項目
            checkPoints.Add(new
            {
                month = month,
                checkDescription = "載入查核點時發生錯誤",
                completionStatus = "incomplete",
                delayReason = "",
                improvement = "",
                plannedProgress = ""
            });
        }
        
        return checkPoints;
    }
    
    /// <summary>
    /// AJAX 方法：儲存月份報告資料 (暫存)
    /// </summary>
    [System.Web.Services.WebMethod]
    public static object SaveMonthlyReport(string projectID, string month, object reportData)
    {
        try
        {
            if (string.IsNullOrEmpty(projectID) || string.IsNullOrEmpty(month) || reportData == null)
            {
                return new { success = false, message = "參數不完整" };
            }

            // 解析報告資料
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(reportData.ToString());
            
            // 儲存實際進度資料（不影響預定進度欄位）
            if (data.actualProgress != null)
            {
                string actWorkAbstract = data.actualProgress.actWorkAbstract?.ToString() ?? "";
                decimal? actProgress = decimal.TryParse(data.actualProgress.actProgress?.ToString() ?? "", out decimal parsedActProgress) ? (decimal?)parsedActProgress : null;
                
                // 從月份預算資料中取得補助款和配合款
                decimal? monthlySubsidy = null;
                decimal? monthlyCoop = null;
                
                if (data.monthlyBudget != null)
                {
                    if (decimal.TryParse(data.monthlyBudget.monthlySubsidy?.ToString() ?? "", out decimal subsidy))
                    {
                        monthlySubsidy = subsidy;
                    }
                    if (decimal.TryParse(data.monthlyBudget.monthlyCoop?.ToString() ?? "", out decimal coop))
                    {
                        monthlyCoop = coop;
                    }
                }
                
                // 使用專門更新實際進度的方法
                OFS_PreMonthProgressHelper.UpdateActualProgress(projectID, month, actWorkAbstract, actProgress, monthlySubsidy, monthlyCoop);
            }
            
            // 儲存查核點資料
            if (data.checkPoints != null)
            {
                var checkPointUpdates = new List<OFS_SCI_WorkSch_CheckStandard>();
                
                foreach (var checkPoint in data.checkPoints)
                {
                    if (checkPoint.id != null && checkPoint.checkDescription?.ToString() != "本月無查核點")
                    {
                        DateTime? actFinishTime = null;
                        if (!string.IsNullOrEmpty(checkPoint.actFinishTime?.ToString()))
                        {
                            DateTime.TryParse(checkPoint.actFinishTime.ToString(), out DateTime parsedDate);
                            actFinishTime = parsedDate;
                        }
                        
                        var updateEntity = new OFS_SCI_WorkSch_CheckStandard
                        {
                            Id = Convert.ToInt32(checkPoint.id),
                            IsFinish = Convert.ToInt32(checkPoint.isFinish ?? 0),
                            DelayReason = checkPoint.delayReason?.ToString() ?? "",
                            ImprovedWay = checkPoint.improvement?.ToString() ?? "",
                            ActFinishTime = actFinishTime,
                            UpdatedAt = DateTime.Now
                        };
                        
                        checkPointUpdates.Add(updateEntity);
                    }
                }
                
                if (checkPointUpdates.Count > 0)
                {
                    OFS_SciWorkSchHelper.BatchUpdateCheckStandardStatus(checkPointUpdates);
                }
            }

            return new { 
                success = true, 
                message = "資料暫存成功",
                shouldReload = true  // 告訴前端需要重新載入頁面
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"SaveMonthlyReport 發生錯誤: {ex.Message}");
            return new { success = false, message = "儲存資料時發生錯誤：" + ex.Message };
        }
    }
    
    /// <summary>
    /// AJAX 方法：提送月份報告資料
    /// </summary>
    [System.Web.Services.WebMethod]
    public static object SubmitMonthlyReport(string projectID, string month, object reportData)
    {
        try
        {
            // 先執行暫存邏輯
            var saveResult = SaveMonthlyReport(projectID, month, reportData);
            
            // 檢查暫存是否成功 - 直接使用匿名物件
            
            if (saveResult.GetType().GetProperty("success")?.GetValue(saveResult, null)?.ToString() != "True")
            {
                return saveResult; // 暫存失敗，直接回傳
            }
            InprogressListHelper.UpdateLastOperation(projectID, $"已完成{month}月進度回報");

            // 檢查是否所有月份都已完成，如果是則將 MonthlyReport 任務標記為完成
            if (!OFS_ScienceTaskHelper.CheckMonthlyReportDeadline(projectID))
            {
                OFS_ScienceTaskHelper.CompleteMonthlyReportTask(projectID);
            }

            // 取得計畫資料並寄信
            try
            {
                // 取得計畫名稱
                var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);
                var projectMain = OFS_SciApplicationHelper.getVersionByProjectID(projectID);

                if (applicationMain != null && projectMain != null)
                {
                    string projectName = applicationMain.ProjectNameTw;
                    string supervisoryAccount = projectMain.SupervisoryPersonAccount;

                    // 根據承辦人帳號取得 UserID
                    int? organizer = SysUserHelper.GetUserIDByAccount(supervisoryAccount);

                    // 寄送通知信
                    NotificationHelper.G1("科專", projectName, $"{month}月進度回報", organizer);
                }
            }
            catch (Exception emailEx)
            {
                System.Diagnostics.Debug.WriteLine($"寄送通知信時發生錯誤: {emailEx.Message}");
                // 寄信失敗不影響主要流程
            }

            return new {
                success = true,
                message = "資料提送成功",
                emailNotification = "系統將自動寄送通知信給相關人員",
                shouldReload = true  // 告訴前端需要重新載入頁面
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"SubmitMonthlyReport 發生錯誤: {ex.Message}");
            return new { success = false, message = "提送資料時發生錯誤：" + ex.Message };
        }
    }
}