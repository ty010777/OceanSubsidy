using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using GS.App;
using Newtonsoft.Json;

public partial class OFS_SCI_SciInprogress_PreProgress : System.Web.UI.Page
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
                // 基本資料區域
                lblProjectID.Text = applicationMain.ProjectID;
                lblProjectName.Text = applicationMain.ProjectNameTw;
                lblExecutingUnit.Text = applicationMain.OrgName;
                
                // 計畫主持人 - 從人員表單取得
                LoadProjectManager(applicationMain.ProjectID);
                
                // 共同執行單位 (可編輯)
                txtCoExecutingUnit.Text = applicationMain.OrgPartner;
                
                // 執行期程 (顯示用，不可編輯)
                LoadExecutionPeriod(applicationMain);
                
                // 計畫聯絡人資訊
                LoadContactPersonnel(applicationMain.ProjectID);
                
                // 預定分月進度
                LoadMonthlyProgress(applicationMain);
                
                // 載入期中期末審查日期
                LoadExamDates();
            }
            else
            {
                // 如果找不到資料，顯示計畫ID
                lblProjectID.Text = ProjectID;
                lblProjectName.Text = "找不到計畫資料";
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"載入基本資料時發生錯誤: {ex.Message}");
            lblProjectID.Text = ProjectID;
            lblProjectName.Text = "載入資料時發生錯誤";
        }
    }
    
    /// <summary>
    /// 載入計畫主持人資料
    /// </summary>
    private void LoadProjectManager(string projectID)
    {
        try
        {
            // PersonID 格式為 P + ProjectID
            string personID = "P" + projectID;
            
            // 從人員表單取得計畫主持人
            var personnelList = OFS_SciApplicationHelper.GetPersonnelByPersonID(personID);
            
            if (personnelList != null && personnelList.Count > 0)
            {
                // 找到 Role = '計畫主持人' 的人員
                var projectManager = personnelList.FirstOrDefault(p => p.Role == "計畫主持人");
                
                if (projectManager != null)
                {
                    lblProjectManager.Text = projectManager.Name;
                }
                else
                {
                    lblProjectManager.Text = "未設定計畫主持人";
                }
            }
            else
            {
                lblProjectManager.Text = "查無人員資料";
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"載入計畫主持人時發生錯誤: {ex.Message}");
            lblProjectManager.Text = "載入失敗";
        }
    }
    
    /// <summary>
    /// 載入執行期程 (使用民國年格式)
    /// </summary>
    private void LoadExecutionPeriod(GS.OCA_OceanSubsidy.Entity.OFS_SCI_Application_Main applicationMain)
    {
        try
        {
            string startDateStr = "";
            string endDateStr = "";
            
            // 開始日期轉換為民國年
            if (applicationMain.StartTime.HasValue)
            {
                startDateStr = DateTimeHelper.ToMinguoDate(applicationMain.StartTime.Value);
            }
            
            // 結束日期轉換為民國年
            if (applicationMain.EndTime.HasValue)
            {
                endDateStr = DateTimeHelper.ToMinguoDate(applicationMain.EndTime.Value);
            }
            
            // 組合顯示執行期程
            if (!string.IsNullOrEmpty(startDateStr) && !string.IsNullOrEmpty(endDateStr))
            {
                lblExecutionPeriod.Text = $"{startDateStr} ~ {endDateStr}";
            }
            else if (!string.IsNullOrEmpty(startDateStr))
            {
                lblExecutionPeriod.Text = $"{startDateStr} ~ (未設定)";
            }
            else if (!string.IsNullOrEmpty(endDateStr))
            {
                lblExecutionPeriod.Text = $"(未設定) ~ {endDateStr}";
            }
            else
            {
                lblExecutionPeriod.Text = "未設定執行期程";
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"載入執行期程時發生錯誤: {ex.Message}");
            lblExecutionPeriod.Text = "載入失敗";
        }
    }
    
    /// <summary>
    /// 載入計畫聯絡人資訊
    /// </summary>
    private void LoadContactPersonnel(string projectID)
    {
        try
        {
            // PersonID 格式為 P + ProjectID
            string personID = "P" + projectID;
            
            // 從人員表單取得所有人員資料
            var personnelList = OFS_SciApplicationHelper.GetPersonnelByPersonID(personID);
            
            if (personnelList != null && personnelList.Count > 0)
            {
                // 產生聯絡人資訊的HTML表格
                GenerateContactPersonnelTable(personnelList);
            }
          
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"載入計畫聯絡人資訊時發生錯誤: {ex.Message}");
            GenerateEmptyContactPersonnelTable();
        }
    }
    
    /// <summary>
    /// 產生聯絡人資訊表格
    /// </summary>
    private void GenerateContactPersonnelTable(List<GS.OCA_OceanSubsidy.Entity.OFS_SCI_Application_Personnel> personnelList)
    {
        try
        {
            string tableHtml = @"
                <div class='table-responsive mt-4'>
                    <table class='table align-middle gray-table'>
                        <thead>
                            <tr>
                                <th></th>
                                <th>姓名</th>
                                <th>角色</th>
                                <th>職稱</th>
                                <th>手機號碼</th>
                                <th>電話</th>
                                <th>分機</th>
                            </tr>
                        </thead>
                        <tbody>";

            int index = 1;
            foreach (var person in personnelList)
            {
                tableHtml += $@"
                    <tr>
                        <td>{index}</td>
                        <td>{System.Web.HttpUtility.HtmlEncode(person.Name ?? "")}</td>
                        <td>{System.Web.HttpUtility.HtmlEncode(person.Role ?? "")}</td>
                        <td>{System.Web.HttpUtility.HtmlEncode(person.JobTitle ?? "")}</td>
                        <td>{System.Web.HttpUtility.HtmlEncode(person.MobilePhone ?? "")}</td>
                        <td>{System.Web.HttpUtility.HtmlEncode(person.Phone ?? "")}</td>
                        <td>{System.Web.HttpUtility.HtmlEncode(person.PhoneExt ?? "")}</td>
                    </tr>";
                index++;
            }

            tableHtml += @"
                        </tbody>
                    </table>
                </div>";

            // 直接存取聯絡人資訊的容器並設定HTML
            contactPersonnelContainer.Controls.Clear();
            contactPersonnelContainer.Controls.Add(new System.Web.UI.LiteralControl(tableHtml));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"產生聯絡人資訊表格時發生錯誤: {ex.Message}");
            GenerateEmptyContactPersonnelTable();
        }
    }
    
    /// <summary>
    /// 產生空白聯絡人資訊表格
    /// </summary>
    private void GenerateEmptyContactPersonnelTable()
    {
        string emptyTableHtml = @"
            <div class='table-responsive mt-4'>
                <table class='table align-middle gray-table'>
                    <thead>
                        <tr>
                            <th></th>
                            <th>姓名</th>
                            <th>角色</th>
                            <th>職稱</th>
                            <th>手機號碼</th>
                            <th>電話</th>
                            <th>分機</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td colspan='7' class='text-center text-muted py-4'>查無聯絡人資料</td>
                        </tr>
                    </tbody>
                </table>
            </div>";

        contactPersonnelContainer.Controls.Clear();
        contactPersonnelContainer.Controls.Add(new System.Web.UI.LiteralControl(emptyTableHtml));
    }
    
    /// <summary>
    /// 載入預定分月進度
    /// </summary>
    private void LoadMonthlyProgress(GS.OCA_OceanSubsidy.Entity.OFS_SCI_Application_Main applicationMain)
    {
        try
        {
            if (applicationMain.StartTime.HasValue && applicationMain.EndTime.HasValue)
            {
                // 產生月份列表
                var monthList = GenerateMonthList(applicationMain.StartTime.Value, applicationMain.EndTime.Value);
                
                // 取得查核標準資料
                var checkStandardByMonth = OFS_SciApplicationHelper.GetCheckStandardByMonth(applicationMain.ProjectID);
                
                // 產生預定分月進度表格
                GenerateMonthlyProgressTable(monthList, checkStandardByMonth);
            }
            else
            {
                // 如果沒有開始或結束時間，顯示空白表格
                GenerateEmptyMonthlyProgressTable();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"載入預定分月進度時發生錯誤: {ex.Message}");
            GenerateEmptyMonthlyProgressTable();
        }
    }
    
    /// <summary>
    /// 根據開始和結束時間產生月份列表
    /// </summary>
    private List<string> GenerateMonthList(DateTime startTime, DateTime endTime)
    {
        var monthList = new List<string>();
        
        try
        {
            var current = new DateTime(startTime.Year, startTime.Month, 1);
            var end = new DateTime(endTime.Year, endTime.Month, 1);
            
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
    /// 產生預定分月進度表格
    /// </summary>
    private void GenerateMonthlyProgressTable(List<string> monthList, Dictionary<string, List<string>> checkStandardByMonth)
    {
        try
        {
            // 載入既有的預定分月進度資料
            var existingData = OFS_PreMonthProgressHelper.GetPreMonthProgressByProjectId(ProjectID);
            var existingDataDict = existingData.ToDictionary(x => x.Month, x => x);

            string tableHtml = @"
                <div class='table-responsive'>
                    <table class='table align-middle gray-table'>
                        <thead>
                            <tr>
                                <th>月份</th>
                                <th width='350'><span class='text-pink'>*</span>工作摘要</th>
                                <th>查核點</th>
                                <th width='150'>累計預定進度%</th>
                            </tr>
                        </thead>
                        <tbody>";

            int rowIndex = 0;
            foreach (var month in monthList)
            {
                // 取得該月份的查核點資料
                string checkPointsHtml = "";
                if (checkStandardByMonth.ContainsKey(month) && checkStandardByMonth[month].Count > 0)
                {
                    // 將查核點以條列式方式顯示
                    var checkPoints = checkStandardByMonth[month];
                    for (int i = 0; i < checkPoints.Count; i++)
                    {
                        checkPointsHtml += System.Web.HttpUtility.HtmlEncode(checkPoints[i]);
                        if (i < checkPoints.Count - 1)
                        {
                            checkPointsHtml += "<br>";
                        }
                    }
                }
                else
                {
                    checkPointsHtml = "<span class='text-muted'>無查核點</span>";
                }

                // 取得既有資料
                string workAbstract = "";
                int? preProgress = null;
                
                if (existingDataDict.ContainsKey(month))
                {
                    workAbstract = existingDataDict[month].PreWorkAbstract ?? "";
                    preProgress = (int?)existingDataDict[month].PreProgress;
                }

                tableHtml += $@"
                    <tr>
                        <th>{System.Web.HttpUtility.HtmlEncode(month)}</th>
                        <td>
                            <div class='d-flex gap-2'>
                                <textarea name='workAbstract_{rowIndex}' class='form-control work-abstract' rows='3' placeholder='請輸入' data-month='{System.Web.HttpUtility.HtmlAttributeEncode(month)}'>{System.Web.HttpUtility.HtmlEncode(workAbstract)}</textarea>
                            </div>
                        </td>
                        <td>
                            {checkPointsHtml}
                        </td>
                        <td>
                            <input name='preProgress_{rowIndex}' type='number' class='form-control pre-progress' placeholder='請輸入' min='0' max='100' data-month='{System.Web.HttpUtility.HtmlAttributeEncode(month)}' value='{(preProgress?.ToString() ?? "")}'>
                        </td>
                    </tr>";
                
                rowIndex++;
            }

            tableHtml += @"
                        </tbody>
                    </table>
                </div>";

            // 直接存取預定分月進度的容器並設定HTML
            monthlyProgressContainer.Controls.Clear();
            monthlyProgressContainer.Controls.Add(new System.Web.UI.LiteralControl(tableHtml));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"產生預定分月進度表格時發生錯誤: {ex.Message}");
            GenerateEmptyMonthlyProgressTable();
        }
    }
    
    /// <summary>
    /// 產生空白預定分月進度表格
    /// </summary>
    private void GenerateEmptyMonthlyProgressTable()
    {
        string emptyTableHtml = @"
            <div class='table-responsive'>
                <table class='table align-middle gray-table'>
                    <thead>
                        <tr>
                            <th>月份</th>
                            <th width='350'><span class='text-pink'>*</span>工作摘要</th>
                            <th>查核點</th>
                            <th width='150'>累計預定進度%</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td colspan='4' class='text-center text-muted py-4'>請先設定計畫執行期程</td>
                        </tr>
                    </tbody>
                </table>
            </div>";

        monthlyProgressContainer.Controls.Clear();
        monthlyProgressContainer.Controls.Add(new System.Web.UI.LiteralControl(emptyTableHtml));
    }
    
    /// <summary>
    /// 暫存按鈕事件
    /// </summary>
    /// TODO 可能刪掉
    // protected void btnTempSave_Click(object sender, EventArgs e)
    // {
    //     try
    //     {
    //         // 收集表單資料
    //         var progressData = CollectMonthlyProgressData();
    //         
    //         // 暫存功能：無論有無資料都允許儲存
    //         bool success = OFS_PreMonthProgressHelper.SavePreMonthProgress(ProjectID, progressData);
    //         
    //         if (success)
    //         {
    //             // 同時儲存其他欄位資料（包含共同執行單位）
    //             SaveOtherFields();
    //             
    //             // 顯示成功訊息
    //             string message = "預定分月進度已暫存";
    //             ScriptManager.RegisterStartupScript(this, GetType(), "TempSaveSuccess",
    //                 $"Swal.fire({{title: '暫存成功', text: '{message}', icon: 'success'}});", true);
    //             InitializePage();
    //         }
    //         else
    //         {
    //             ScriptManager.RegisterStartupScript(this, GetType(), "TempSaveError",
    //                 "Swal.fire({title: '暫存失敗', text: '系統發生錯誤，請稍後再試', icon: 'error'});", true);
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         System.Diagnostics.Debug.WriteLine($"暫存時發生錯誤: {ex.Message}");
    //         ScriptManager.RegisterStartupScript(this, GetType(), "TempSaveException",
    //             $"Swal.fire({{title: '系統錯誤', text: '暫存時發生錯誤：{ex.Message}', icon: 'error'}});", true);
    //     }
    // }
    
    
    /// <summary>
    /// 收集預定分月進度資料
    /// </summary>
    private List<GS.OCA_OceanSubsidy.Entity.OFS_SCI_PreMonthProgress> CollectMonthlyProgressData()
    {
        var progressList = new List<GS.OCA_OceanSubsidy.Entity.OFS_SCI_PreMonthProgress>();
        
        try
        {
            // 取得查核標準資料以填入 CheckDescription
            var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(ProjectID);
            Dictionary<string, List<string>> checkStandardByMonth = new Dictionary<string, List<string>>();
            
            if (applicationMain != null)
            {
                checkStandardByMonth = OFS_SciApplicationHelper.GetCheckStandardByMonth(applicationMain.ProjectID);
            }
            
            // 從 Request.Form 收集資料
            var workAbstractKeys = Request.Form.AllKeys.Where(k => k.StartsWith("workAbstract_")).ToList();
            var preProgressKeys = Request.Form.AllKeys.Where(k => k.StartsWith("preProgress_")).ToList();
            
            for (int i = 0; i < workAbstractKeys.Count; i++)
            {
                string workAbstractKey = $"workAbstract_{i}";
                string preProgressKey = $"preProgress_{i}";
                
                if (Request.Form[workAbstractKey] != null)
                {
                    string workAbstract = Request.Form[workAbstractKey]?.Trim();
                    string preProgressStr = Request.Form[preProgressKey]?.Trim();
                    
                    // 透過 JavaScript 或 Hidden Field 取得對應的月份
                    string month = GetMonthFromFormIndex(i);
                    
                    if (!string.IsNullOrEmpty(month))
                    {
                        // 取得該月份的查核點資料
                        string checkDescription = "";
                        if (checkStandardByMonth.ContainsKey(month) && checkStandardByMonth[month].Count > 0)
                        {
                            // 將查核點以換行符號連接
                            checkDescription = string.Join("\n", checkStandardByMonth[month]);
                        }
                        
                        var progress = new GS.OCA_OceanSubsidy.Entity.OFS_SCI_PreMonthProgress
                        {
                            ProjectID = ProjectID,
                            Month = month,
                            PreWorkAbstract = workAbstract,
                            CheckDescription = checkDescription,
                            PreProgress = null
                        };
                        
                        // 解析預定進度
                        if (!string.IsNullOrEmpty(preProgressStr) && decimal.TryParse(preProgressStr, out decimal preProgress))
                        {
                            progress.PreProgress = preProgress;
                        }
                        
                        progressList.Add(progress);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"收集預定分月進度資料時發生錯誤: {ex.Message}");
        }
        
        return progressList;
    }
    
    /// <summary>
    /// 根據表單索引取得對應的月份
    /// </summary>
    private string GetMonthFromFormIndex(int index)
    {
        try
        {
            // 重新產生月份列表以取得對應的月份
            var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(ProjectID);
            if (applicationMain != null && applicationMain.StartTime.HasValue && applicationMain.EndTime.HasValue)
            {
                var monthList = GenerateMonthList(applicationMain.StartTime.Value, applicationMain.EndTime.Value);
                
                if (index >= 0 && index < monthList.Count)
                {
                    return monthList[index];
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得月份時發生錯誤: {ex.Message}");
        }
        
        return "";
    }
    
    /// <summary>
    /// 驗證其他必填欄位
    /// </summary>
    private bool ValidateOtherRequiredFields()
    {
        var errors = new List<string>();
        
        // 驗證期中審查預定日期
        if (string.IsNullOrEmpty(txtMidReviewDate.Text))
        {
            errors.Add("期中審查預定日期為必填項目");
        }
        
        // 驗證期末審查預定日期
        if (string.IsNullOrEmpty(txtFinalReviewDate.Text))
        {
            errors.Add("期末審查預定日期為必填項目");
        }
        
        // 驗證日期邏輯
        if (!string.IsNullOrEmpty(txtMidReviewDate.Text) && !string.IsNullOrEmpty(txtFinalReviewDate.Text))
        {
            if (DateTime.TryParse(txtMidReviewDate.Text, out DateTime midDate) &&
                DateTime.TryParse(txtFinalReviewDate.Text, out DateTime finalDate))
            {
                if (finalDate <= midDate)
                {
                    errors.Add("期末審查預定日期必須晚於期中審查預定日期");
                }
            }
        }
        
        if (errors.Count > 0)
        {
            string errorMessages = string.Join("\\n", errors);
            ScriptManager.RegisterStartupScript(this, GetType(), "ValidationError",
                $"Swal.fire({{title: '資料驗證失敗', text: '{errorMessages}', icon: 'warning'}});", true);
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// 儲存其他欄位資料
    /// </summary>
    private void SaveOtherFields()
    {
        try
        {
            // 更新 OFS_SCI_Application_Main 的共同執行單位
            string coExecutingUnit = txtCoExecutingUnit.Text?.Trim() ?? "";
            OFS_SciApplicationHelper.UpdateCoExecutingUnit(ProjectID, coExecutingUnit);
            
            // 更新 OFS_SCI_Project_Main 的期中期末審查日期
            DateTime? midtermDate = ParseDateFromTextBox(txtMidReviewDate.Text);
            DateTime? finalDate = ParseDateFromTextBox(txtFinalReviewDate.Text);
            OFS_SciApplicationHelper.UpdateExamDates(ProjectID, midtermDate, finalDate);
            
            System.Diagnostics.Debug.WriteLine($"儲存其他欄位資料: 共同執行單位已更新, 期中審查日期={midtermDate?.ToString("yyyy-MM-dd")}, 期末審查日期={finalDate?.ToString("yyyy-MM-dd")}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"儲存其他欄位資料時發生錯誤: {ex.Message}");
            throw; // 重新拋出例外讓上層處理
        }
    }
    
    /// <summary>
    /// 從 TextBox 文字解析日期
    /// </summary>
    /// <param name="dateText">日期文字</param>
    /// <returns>解析後的日期，失敗則返回 null</returns>
    private DateTime? ParseDateFromTextBox(string dateText)
    {
        if (string.IsNullOrWhiteSpace(dateText))
            return null;
            
        if (DateTime.TryParse(dateText, out DateTime result))
            return result;
            
        return null;
    }
    
    /// <summary>
    /// 載入期中期末審查日期
    /// </summary>
    private void LoadExamDates()
    {
        try
        {
            var examDates = OFS_SciApplicationHelper.GetExamDates(ProjectID);
            
            // 設定期中審查日期
            if (examDates.MidtermExamDate.HasValue)
            {
                txtMidReviewDate.Text = examDates.MidtermExamDate.Value.ToString("yyyy-MM-dd");
            }
            
            // 設定期末審查日期
            if (examDates.FinalExamDate.HasValue)
            {
                txtFinalReviewDate.Text = examDates.FinalExamDate.Value.ToString("yyyy-MM-dd");
            }
            
            System.Diagnostics.Debug.WriteLine($"已載入審查日期: 期中={examDates.MidtermExamDate?.ToString("yyyy-MM-dd")}, 期末={examDates.FinalExamDate?.ToString("yyyy-MM-dd")}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"載入審查日期時發生錯誤: {ex.Message}");
            // 不拋出例外，讓頁面可以正常載入，日期欄位保持空白
        }
    }
    
    /// <summary>
    /// AJAX 提送預定分月進度方法
    /// </summary>
    [WebMethod]
    public static object SubmitPreProgress(string jsonData)
    {
        try
        {
            var data = JsonConvert.DeserializeObject<dynamic>(jsonData);
            
            string projectID = data.projectID?.ToString();
            string coExecutingUnit = data.coExecutingUnit?.ToString();
            string midReviewDate = data.midReviewDate?.ToString();
            string finalReviewDate = data.finalReviewDate?.ToString();
            var monthlyProgressArray = data.monthlyProgress;
            
            if (string.IsNullOrEmpty(projectID))
            {
                return new { success = false, message = "計畫ID不能為空" };
            }
            
            // 收集預定分月進度資料
            var progressList = new List<GS.OCA_OceanSubsidy.Entity.OFS_SCI_PreMonthProgress>();
            
            if (monthlyProgressArray != null)
            {
                foreach (var item in monthlyProgressArray)
                {
                    string month = item.month?.ToString();
                    string workAbstract = item.workAbstract?.ToString();
                    string preProgressStr = item.preProgress?.ToString();
                    
                    if (!string.IsNullOrEmpty(month))
                    {
                        // 取得查核標準資料
                        var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);
                        string checkDescription = "";
                        
                        if (applicationMain != null)
                        {
                            var checkStandardByMonth = OFS_SciApplicationHelper.GetCheckStandardByMonth(applicationMain.ProjectID);
                            if (checkStandardByMonth.ContainsKey(month) && checkStandardByMonth[month].Count > 0)
                            {
                                checkDescription = string.Join("\n", checkStandardByMonth[month]);
                            }
                        }
                        
                        var progress = new GS.OCA_OceanSubsidy.Entity.OFS_SCI_PreMonthProgress
                        {
                            ProjectID = projectID,
                            Month = month,
                            PreWorkAbstract = workAbstract,
                            CheckDescription = checkDescription,
                            PreProgress = null
                        };
                        
                        // 解析預定進度
                        if (!string.IsNullOrEmpty(preProgressStr) && decimal.TryParse(preProgressStr, out decimal preProgress))
                        {
                            progress.PreProgress = preProgress;
                        }
                        
                        progressList.Add(progress);
                    }
                }
            }
            
            // 驗證資料
            var validationResult = OFS_PreMonthProgressHelper.ValidatePreMonthProgress(progressList);
            
            if (!validationResult.IsValid)
            {
                string errorMessages = string.Join(", ", validationResult.Errors);
                return new { success = false, message = errorMessages };
            }
            
            // 驗證日期欄位
            if (string.IsNullOrEmpty(midReviewDate))
            {
                return new { success = false, message = "期中審查預定日期為必填項目" };
            }
            
            if (string.IsNullOrEmpty(finalReviewDate))
            {
                return new { success = false, message = "期末審查預定日期為必填項目" };
            }
            
            // 驗證日期邏輯
            if (DateTime.TryParse(midReviewDate, out DateTime midDate) && 
                DateTime.TryParse(finalReviewDate, out DateTime finalDate))
            {
                if (finalDate <= midDate)
                {
                    return new { success = false, message = "期末審查預定日期必須晚於期中審查預定日期" };
                }
            }
            
            // 儲存資料
            bool success = OFS_PreMonthProgressHelper.SavePreMonthProgress(projectID, progressList);
            
            if (success)
            {
                // 儲存其他欄位資料
                // 更新共同執行單位
                OFS_SciApplicationHelper.UpdateCoExecutingUnit(projectID, coExecutingUnit ?? "");
                
                // 更新期中期末審查日期
                DateTime? midtermDate = DateTime.TryParse(midReviewDate, out DateTime mid) ? mid : (DateTime?)null;
                DateTime? finalDateParsed = DateTime.TryParse(finalReviewDate, out DateTime final) ? final : (DateTime?)null;
                OFS_SciApplicationHelper.UpdateExamDates(projectID, midtermDate, finalDateParsed);
                
                // 更新任務狀態
                InprogressListHelper.UpdateLastOperation(projectID, "已完成預定進度規劃");
                InprogressListHelper.UpdateTaskCompleted(projectID, "Schedule", true);
                
                return new { success = true, message = "預定分月進度已成功提送" };
            }
            else
            {
                return new { success = false, message = "系統發生錯誤，請稍後再試" };
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"SubmitPreProgress 發生錯誤: {ex.Message}");
            return new { success = false, message = $"系統錯誤：{ex.Message}" };
        }
    }
}