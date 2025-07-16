using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Operation.OFS;
using Newtonsoft.Json;

/// <summary>
/// 科專計畫工作排程頁面
/// 提供期程/工作項目/查核標準的填寫、儲存功能
/// </summary>
public partial class OFS_SciWorkSch : System.Web.UI.Page
{
    private string ProjectID => Request.QueryString["ProjectID"];
    
    #region 頁面生命週期
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // 設定顯示模式
            SetDisplayMode();

            if (!IsPostBack)
            {
                InitializePage();
                
            }
        }
        catch (Exception ex)
        {
            Response.Write($"<script>alert('頁面載入錯誤：{ex.Message}');</script>");
        }
    }

    /// <summary>
    /// 初始化頁面
    /// </summary>
    private void InitializePage()
    {
        try
        {
            // 載入現有資料
            LoadExistingData();
            
            // 檢查表單狀態並控制暫存按鈕顯示
            CheckFormStatusAndHideTempSaveButton();
        }
        catch (Exception ex)
        {
            ShowMessage($"載入資料時發生錯誤：{ex.Message}", "error");
        }
    }
    
    /// <summary>
    /// 載入現有資料到前端
    /// </summary>
    private void LoadExistingData()
    {
        try
        {
            // 取得 ProjectID
            var ProjectID = Request.QueryString["ProjectID"];
            if (string.IsNullOrEmpty(ProjectID))
            {
                Console.WriteLine("無法取得 ProjectID，跳過資料載入");
                return;
            }
            
            // 載入計畫期程
            LoadProjectSchedule(ProjectID);
            
            // 載入工作項目
            LoadWorkItems(ProjectID);
            
            // 載入查核標準
            LoadCheckStandards(ProjectID);
            
            // 載入計畫架構圖
            LoadDiagramFile(ProjectID);
            
        }
        catch (Exception ex)
        {
            
            throw new Exception($"載入現有資料失敗：{ex.Message}");
        }
    }
    
    /// <summary>
    /// 載入計畫期程到前端
    /// </summary>
    private void LoadProjectSchedule(string ProjectID)
    {
        var (startTime, endTime) = OFS_SciWorkSchHelper.GetProjectScheduleByProjectID(ProjectID);
        
        var script = "";
        if (startTime.HasValue)
        {
            script += $"document.getElementById('startDate').value = '{startTime.Value:yyyy-MM-dd}';";
        }
        if (endTime.HasValue)
        {
            script += $"document.getElementById('endDate').value = '{endTime.Value:yyyy-MM-dd}';";
        }
        
        if (!string.IsNullOrEmpty(script))
        {
            ClientScript.RegisterStartupScript(this.GetType(), "LoadProjectSchedule", script, true);
        }
    }
    
    /// <summary>
    /// 載入工作項目到前端
    /// </summary>
    private void LoadWorkItems(string ProjectID)
    {
        var workItems = OFS_SciWorkSchHelper.GetWorkItemsByProjectID(ProjectID);
        
        if (workItems.Any())
        {
            var workItemsJson = JsonConvert.SerializeObject(workItems.Select(w => new
            {
                projectId = w.ProjectID,
                workItemId = w.WorkItem_id,
                itemCode = OFS_SciWorkSchHelper.ExtractItemCodeFromWorkItemId(w.WorkItem_id),
                itemName = w.WorkName,
                startMonth = w.StartMonth,
                endMonth = w.EndMonth,
                weight = w.Weighting,
                personMonth = w.InvestMonth,
                isOutsourced = w.IsOutsourced ?? false
            }));
            
            var script = $@"
                console.log('=== 後端準備傳送工作項目資料 ===');
                console.log('工作項目 JSON:', {workItemsJson});
                
                function loadWorkItemsData() {{
                    if (window.sciWorkSchManager) {{
                        window.sciWorkSchManager.loadWorkItems({workItemsJson});
                        console.log('已載入工作項目資料：', {workItemsJson});
                    }} else {{
                        console.log('sciWorkSchManager 未初始化，延遲重試...');
                        setTimeout(loadWorkItemsData, 500);
                    }}
                }}
                
                // 確保 DOM 完全載入後再執行
                if (document.readyState === 'loading') {{
                    document.addEventListener('DOMContentLoaded', loadWorkItemsData);
                }} else {{
                    loadWorkItemsData();
                }}
            ";
            
            ClientScript.RegisterStartupScript(this.GetType(), "LoadWorkItems", script, true);
        }
    }
    
    /// <summary>
    /// 載入查核標準到前端
    /// </summary>
    private void LoadCheckStandards(string ProjectID)
    {
        var checkStandards = OFS_SciWorkSchHelper.GetCheckStandardsByProjectID(ProjectID);
        
        if (checkStandards.Any())
        {
            var checkStandardsJson = JsonConvert.SerializeObject(checkStandards.Select(c => new
            {
                id = c.Id,
                projectId = c.ProjectID,
                workItem = c.WorkItem,
                serialNumber = c.SerialNumber,
                plannedFinishDate = c.PlannedFinishDate?.ToString("yyyy-MM-dd"),
                description = c.CheckDescription
            }));
            
            var script = $@"
                if (window.sciWorkSchManager) {{
                    window.sciWorkSchManager.loadCheckStandards({checkStandardsJson});
                    console.log('已載入查核標準資料：', {checkStandardsJson});
                }} else {{
                    // 如果 sciWorkSchManager 還沒初始化，延遲執行
                    setTimeout(function() {{
                        if (window.sciWorkSchManager) {{
                            window.sciWorkSchManager.loadCheckStandards({checkStandardsJson});
                            console.log('已載入查核標準資料（延遲）：', {checkStandardsJson});
                        }}
                    }}, 1000);
                }}
            ";
            
            ClientScript.RegisterStartupScript(this.GetType(), "LoadCheckStandards", script, true);
        }
    }
    
    /// <summary>
    /// 載入計畫架構圖到前端
    /// </summary>
    private void LoadDiagramFile(string ProjectID)
    {
        var uploadFiles = OFS_SciWorkSchHelper.GetUploadFilesByProjectIDAndFileCode(ProjectID, "WorkSchStructure");
        
        if (uploadFiles.Any())
        {
            var file = uploadFiles.First();
            var script = $@"
                if (window.sciWorkSchManager) {{
                    window.sciWorkSchManager.loadDiagramFile('{file.TemplatePath}', '{file.FileName}');
                    console.log('已載入計畫架構圖：', '{file.TemplatePath}');
                }} else {{
                    // 如果 sciWorkSchManager 還沒初始化，延遲執行
                    setTimeout(function() {{
                        if (window.sciWorkSchManager) {{
                            window.sciWorkSchManager.loadDiagramFile('{file.TemplatePath}', '{file.FileName}');
                            console.log('已載入計畫架構圖（延遲）：', '{file.TemplatePath}');
                        }}
                    }}, 1000);
                }}
            ";
            
            ClientScript.RegisterStartupScript(this.GetType(), "LoadDiagramFile", script, true);
        }
    }
    #endregion

    #region 按鈕事件處理
    /// <summary>
    /// 暫存按鈕點擊事件
    /// </summary>
    protected void btnTempSave_Click(object sender, EventArgs e)
    {
        try
        {
            // 處理檔案上傳（如果有的話）
            ProcessDiagramUpload();
            
            SaveWorkScheduleData(false);
            
            // 暫存成功後重新載入資料
            LoadExistingData();
            
            ShowMessage("資料暫存成功！", "success");
        }
        catch (Exception ex)
        {
            ShowMessage($"暫存失敗：{ex.Message}", "error");
        }
    }

    /// <summary>
    /// 儲存並下一步按鈕點擊事件
    /// </summary>
    protected void btnSaveAndNext_Click(object sender, EventArgs e)
    {
        try
        {
            // 驗證資料
            if (ValidateFormData())
            {
                // 處理檔案上傳（如果有的話）
                ProcessDiagramUpload();
                SaveWorkScheduleData(true);
                
                // 跳轉到下一頁
                var projectId = Request.QueryString["ProjectID"];
                if (!string.IsNullOrEmpty(projectId))
                {
                    Response.Redirect($"SciFunding.aspx?ProjectID={projectId}");
                }
                else
                {
                    Response.Redirect("SciFunding.aspx");
                }
            }
        }
        catch (Exception ex)
        {
            ShowMessage($"儲存失敗：{ex.Message}", "error");
        }
    }

    
    /// <summary>
    /// 處理計畫架構圖上傳（在暫存/完成時呼叫）
    /// </summary>
    private void ProcessDiagramUpload()
    {
        try
        {
            // 檢查是否有檔案上傳 - 使用正確的檔案 input name
            var uploadedFile = Request.Files["fileUploadDiagram"];
            if (uploadedFile != null && uploadedFile.ContentLength > 0)
            {
                // 驗證檔案類型
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png" };
                if (!allowedTypes.Contains(uploadedFile.ContentType.ToLower()))
                {
                    throw new Exception("只支援JPG和PNG格式的圖片");
                }

                // 驗證檔案大小（10MB）
                if (uploadedFile.ContentLength > 10 * 1024 * 1024)
                {
                    throw new Exception("檔案大小不能超過10MB");
                }

          
                var ProjectID = Request.QueryString["ProjectID"];
                // 建立檔案名稱
                var fileExtension = Path.GetExtension(uploadedFile.FileName).ToLower();
                var fileName = $"WorkSchStructure{fileExtension}";
                
                // 建立完整檔案路徑（加上 ProjectID 資料夾）
                var uploadDir = Server.MapPath($"~/UploadFiles/OFS/SCI/{ProjectID}/");
                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }
                
                var fullFilePath = Path.Combine(uploadDir, fileName);
                
                // 如果檔案已存在，先刪除舊檔案
                if (File.Exists(fullFilePath))
                {
                    File.Delete(fullFilePath);
                }
                
                // 儲存檔案
                uploadedFile.SaveAs(fullFilePath);
                
                // 相對路徑（用於資料庫儲存和顯示）
                var relativePath = $"UploadFiles/OFS/SCI/{ProjectID}/{fileName}";
                
                // 儲存到資料庫
                SaveUploadFileRecord(ProjectID, fileName, relativePath);
                
            }
            else
            {
                // 檢查是否有預覽圖片但沒有檔案上傳
                var diagramDataJson = Request.Form["diagramData"];
                if (!string.IsNullOrEmpty(diagramDataJson))
                {
                    var diagramData = JsonConvert.DeserializeObject<dynamic>(diagramDataJson);
                    if (diagramData.hasImage == true)
                    {
                        Console.WriteLine("有預覽圖片但沒有檔案上傳");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // 記錄錯誤
            var errorScript = $"console.error('計畫架構圖上傳失敗：', '{ex.Message.Replace("'", "\\'")}');";
            ClientScript.RegisterStartupScript(this.GetType(), "LogDiagramError", errorScript, true);
            
            // 不拋出例外，讓其他資料繼續儲存
            Console.WriteLine($"計畫架構圖上傳失敗: {ex.Message}");
        }
    }
    #endregion

    #region 資料處理 (新架構)
    /// <summary>
    /// 儲存工作排程資料 (新架構 - 單表設計)
    /// </summary>
    /// <param name="isComplete">是否為完成狀態</param>
    private void SaveWorkScheduleData(bool isComplete)
    {
        var ProjectID = Request.QueryString["ProjectID"];
        // 1. 處理計畫期程 - 儲存到 OFS_SCI_Application_Main
        var projectSchedule = ReadProjectScheduleData();
        OFS_SciWorkSchHelper.UpdateProjectSchedule(ProjectID, projectSchedule.startDate, projectSchedule.endDate);
        
        // 2. 讀取工作項目資料 (包含主項和子項)
        var allWorkItems = ReadAllWorkItemsData(ProjectID);
        
        // 3. 儲存所有工作項目到單表
        int savedCount = OFS_SciWorkSchHelper.SaveAllWorkItems(ProjectID, allWorkItems);
        
        // 4. 讀取查核標準資料
        var checkStandards = ReadCheckStandardsData(ProjectID);
        
        // 5. 儲存查核標準
        int savedCheckStandardsCount = OFS_SciWorkSchHelper.SaveCheckStandards(checkStandards);
        
        // 6. 更新版本狀態
        UpdateVersionStatusBasedOnAction(ProjectID, isComplete);
    }

    /// <summary>
    /// 根據動作類型更新版本狀態
    /// </summary>
    /// <param name="ProjectID">ProjectID</param>
    /// <param name="isComplete">是否為完成動作（下一步）</param>
    private void UpdateVersionStatusBasedOnAction(string ProjectID, bool isComplete)
    {
        try
        {
            if (isComplete)
            {
                // 點擊「完成本頁，下一步」按鈕
                // 1. Form2Status 設為 "完成" 
                // 2. 檢查 CurrentStep，如果 < 3則改成 3
                
                string currentStep = OFS_SciWorkSchHelper.GetCurrentStepByProjectID(ProjectID);
                int currentStepNum = 1;
                int.TryParse(currentStep, out currentStepNum);
                
                bool shouldUpdateCurrentStep = currentStepNum < 3;
                string newCurrentStep = shouldUpdateCurrentStep ? "3" : currentStep;
                
                OFS_SciWorkSchHelper.UpdateVersionStatus(ProjectID, "完成", shouldUpdateCurrentStep, newCurrentStep);
            }
            else
            {
                // 點擊「暫存」按鈕
                // 只更新 Form2Status 為 "暫存"，CurrentStep 不變
                
                OFS_SciWorkSchHelper.UpdateVersionStatus(ProjectID, "暫存");
                
            }
        }
        catch (Exception ex)
        {
            // 記錄錯誤但不中斷流程
            Console.WriteLine($"更新版本狀態失敗: {ex.Message}");
        }
    }

    /// <summary>
    /// 讀取計畫期程資料
    /// </summary>
    private (DateTime? startDate, DateTime? endDate) ReadProjectScheduleData()
    {
        var startDateInput = Request.Form["startDate"];
        var endDateInput = Request.Form["endDate"];
        
        DateTime? startDate = null;
        DateTime? endDate = null;
        
        if (DateTime.TryParse(startDateInput, out DateTime start))
        {
            startDate = start;
        }
        
        if (DateTime.TryParse(endDateInput, out DateTime end))
        {
            endDate = end;
        }
        
        return (startDate, endDate);
    }

    /// <summary>
    /// 讀取所有工作項目資料 (主項+子項)
    /// </summary>
    /// <param name="ProjectID">ProjectID</param>
    private List<OFS_SCI_WorkSch_Main> ReadAllWorkItemsData(string ProjectID)
    {
        var workItems = new List<OFS_SCI_WorkSch_Main>();
        
        // 從前端JSON資料讀取工作項目
        var workItemsJson = Request.Form["workItemsData"];
        
        if (!string.IsNullOrEmpty(workItemsJson))
        {
            try
            {
                // 解析前端傳來的工作項目JSON資料
                var frontendData = JsonConvert.DeserializeObject<List<dynamic>>(workItemsJson);
                
                foreach (var item in frontendData)
                {
                    var workItem = new OFS_SCI_WorkSch_Main();
                    
                    // 基本資訊
                    workItem.ProjectID = ProjectID;
                    var itemCode = item.code?.ToString() ?? "";
                    workItem.WorkItem_id = OFS_SciWorkSchHelper.GenerateWorkItemId(ProjectID, itemCode);
                    workItem.WorkName = item.itemName?.ToString() ?? "";
                    
                    // 起訖月份 (新架構使用 int 類型)
                    if (int.TryParse(item.startMonth?.ToString(), out int startMonth) && startMonth > 0)
                    {
                        workItem.StartMonth = startMonth;
                    }
                    
                    if (int.TryParse(item.endMonth?.ToString(), out int endMonth) && endMonth > 0)
                    {
                        workItem.EndMonth = endMonth;
                    }
                    
                    // 權重
                    if (decimal.TryParse(item.weight?.ToString()?.Replace("%", ""), out decimal weight))
                    {
                        workItem.Weighting = weight;
                    }
                    
                    // 人月數
                    if (decimal.TryParse(item.personMonth?.ToString(), out decimal personMonth))
                    {
                        workItem.InvestMonth = personMonth;
                    }
                    
                    // 委外狀態
                    workItem.IsOutsourced = bool.TryParse(item.isOutsourced?.ToString(), out bool isOutsourced) && isOutsourced;
                    
                    workItems.Add(workItem);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"解析工作項目資料時發生錯誤：{ex.Message}");
            }
        }
        
        return workItems;
    }

    /// <summary>
    /// 讀取查核標準資料
    /// </summary>
    /// <param name="ProjectID">ProjectID</param>
    private List<OFS_SCI_WorkSch_CheckStandard> ReadCheckStandardsData(string ProjectID)
    {
        var checkStandards = new List<OFS_SCI_WorkSch_CheckStandard>();
        
        // 從前端JSON資料讀取查核標準
        var checkStandardsJson = Request.Form["checkStandardsData"];
        
        if (!string.IsNullOrEmpty(checkStandardsJson))
        {
            try
            {
                // 解析前端傳來的查核標準JSON資料
                var frontendData = JsonConvert.DeserializeObject<List<dynamic>>(checkStandardsJson);
                
                foreach (var item in frontendData)
                {
                    var checkStandard = new OFS_SCI_WorkSch_CheckStandard();
                    
                    checkStandard.ProjectID = ProjectID;
                    checkStandard.WorkItem = item.workItem?.ToString() ?? "";
                    checkStandard.SerialNumber = item.serialNumber?.ToString() ?? "";
                    checkStandard.CheckDescription = item.description?.ToString() ?? "";
                    
                    // 解析預定完成日期
                    if (DateTime.TryParse(item.plannedFinishDate?.ToString(), out DateTime plannedDate))
                    {
                        checkStandard.PlannedFinishDate = plannedDate;
                    }
                    
                    checkStandards.Add(checkStandard);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"解析查核標準資料時發生錯誤：{ex.Message}");
            }
        }
        
        return checkStandards;
    }

    /// <summary>
    /// 驗證表單資料
    /// </summary>
    private bool ValidateFormData()
    {
        var errors = new List<string>();
        
        // 驗證期程
        var startDateInput = Request.Form["startDate"];
        var endDateInput = Request.Form["endDate"];
        
        if (string.IsNullOrEmpty(startDateInput))
        {
            errors.Add("請選擇計畫開始日期");
        }
        
        if (string.IsNullOrEmpty(endDateInput))
        {
            errors.Add("請選擇計畫結束日期");
        }
        
        if (DateTime.TryParse(startDateInput, out DateTime startDate) && 
            DateTime.TryParse(endDateInput, out DateTime endDate))
        {
            if (startDate >= endDate)
            {
                errors.Add("計畫結束日期必須晚於開始日期");
            }
        }
        
        // 驗證工作項目
        var workItemsJson = Request.Form["workItemsData"];
        if (string.IsNullOrEmpty(workItemsJson))
        {
            errors.Add("請至少新增一個工作項目");
        }
        
        // 驗證查核標準
        var checkStandardsJson = Request.Form["checkStandardsData"];
        if (string.IsNullOrEmpty(checkStandardsJson))
        {
            errors.Add("請至少新增一個查核標準");
        }
        
        if (errors.Any())
        {
            var errorMessage = string.Join("\\n", errors);
            ShowMessage($"資料驗證失敗：\\n{errorMessage}", "error");
            return false;
        }
        
        return true;
    }

    /// <summary>
    /// 輸出資料到瀏覽器Console（用於除錯）
    /// </summary>
    private void LogDataToConsole((DateTime? startDate, DateTime? endDate) projectSchedule, 
                                  List<OFS_SCI_WorkSch_Main> workItems, 
                                  List<OFS_SCI_WorkSch_CheckStandard> checkStandards, 
                                  int savedCount,
                                  int savedCheckStandardsCount)
    {
        var logData = new
        {
            ProjectSchedule = new
            {
                StartDate = projectSchedule.startDate,
                EndDate = projectSchedule.endDate
            },
            WorkItems = workItems.Select(w => new
            {
                w.ProjectID,
                w.WorkItem_id,
                w.WorkName,
                w.StartMonth,
                w.EndMonth,
                w.Weighting,
                w.InvestMonth,
                w.IsOutsourced,
                ItemCode = OFS_SciWorkSchHelper.ExtractItemCodeFromWorkItemId(w.WorkItem_id)
            }),
            CheckStandards = checkStandards.Select(c => new
            {
                c.ProjectID,
                c.WorkItem,
                c.SerialNumber,
                c.PlannedFinishDate,
                c.CheckDescription
            }),
            Summary = new
            {
                TotalWorkItems = workItems.Count,
                SavedWorkItems = savedCount,
                TotalCheckStandards = checkStandards.Count,
                SavedCheckStandards = savedCheckStandardsCount,
                WorkItemsSuccessRate = workItems.Count > 0 ? $"{(savedCount * 100.0 / workItems.Count):F1}%" : "0%",
                CheckStandardsSuccessRate = checkStandards.Count > 0 ? $"{(savedCheckStandardsCount * 100.0 / checkStandards.Count):F1}%" : "0%"
            }
        };
        
        var script = $@"
            console.log('=== 工作排程資料儲存結果 (新架構) ===');
            console.log('WorkItem_id 格式：{{ProjectID}}_{{項目代碼}} (例如：v001_SCI_2024_A1)');
            console.log('儲存結果：工作項目 {savedCount}/{workItems.Count} 筆，查核標準 {savedCheckStandardsCount}/{checkStandards.Count} 筆');
            console.log('詳細資料：', {JsonConvert.SerializeObject(logData)});
        ";
        ClientScript.RegisterStartupScript(this.GetType(), "LogData", script, true);
    }

    /// <summary>
    /// 顯示訊息給使用者
    /// </summary>
    private void ShowMessage(string message, string type = "info")
    {
        var script = $"alert('{message.Replace("'", "\\'")}');";
        ClientScript.RegisterStartupScript(this.GetType(), "ShowMessage", script, true);
    }
    
    #endregion

    #region 工具方法
    /// <summary>
    /// 取得目前的 ProjectID
    /// </summary>
   
    
    /// <summary>
    /// 檢查表單狀態並控制暫存按鈕顯示
    /// </summary>
    private void CheckFormStatusAndHideTempSaveButton()
    {
        try
        {
            var ProjectID = Request.QueryString["ProjectID"];
            var formStatus = OFS_SciWorkSchHelper.GetFormStatusByProjectID(ProjectID, "Form2Status");
            
            if (formStatus == "完成")
            {
                // 隱藏暫存按鈕
                btnTempSave.Style["display"] = "none";
                
                // 也可以用 Visible 屬性
                // btnTempSave.Visible = false;
            }
        }
        catch (Exception ex)
        {
            // 發生錯誤時不隱藏按鈕，讓用戶正常使用
            System.Diagnostics.Debug.WriteLine($"檢查表單狀態失敗: {ex.Message}");
        }
    }
    #endregion

    #region 檔案上傳處理 (ASP.NET Button Click 方式)

    /// <summary>
    /// 儲存上傳檔案記錄到資料庫
    /// </summary>
    private void SaveUploadFileRecord(string ProjectID, string fileName, string relativePath)
    {
        try
        {
            // 先檢查是否已有該版本的工作排程架構圖記錄
            var existingFiles = OFS_SciWorkSchHelper.GetUploadFilesByProjectIDAndFileCode(ProjectID, "WorkSchStructure");
            
            // 如果有舊記錄，先刪除
            if (existingFiles != null && existingFiles.Any())
            {
                foreach (var existingFile in existingFiles)
                {
                    // 從資料庫刪除記錄
                    OFS_SciWorkSchHelper.DeleteUploadFile(existingFile.ProjectID, existingFile.FileCode);
                }
            }
            
            // 建立新的上傳檔案記錄
            var uploadFile = new OFS_SCI_UploadFile
            {
                ProjectID = ProjectID,
                FileCode = "WorkSchStructure",
                FileName = fileName,
                TemplatePath = relativePath,
                Statuses = "Active"
            };
            
            // 儲存新記錄到資料庫
            OFS_SciWorkSchHelper.InsertUploadFile(uploadFile);
        }
        catch (Exception ex)
        {
            // 記錄錯誤但不中斷流程
            var errorScript = $"console.error('檔案記錄儲存失敗：', '{ex.Message.Replace("'", "\\'")}');";
            ClientScript.RegisterStartupScript(this.GetType(), "LogUploadFileError", errorScript, true);
            
            Console.WriteLine($"儲存檔案記錄失敗：{ex.Message}");
        }
    }
    #endregion

    #region 顯示模式控制

    /// <summary>
    /// 設定顯示模式
    /// </summary>
    private void SetDisplayMode()
    {
        var master = (OFSApplicationMaster)this.Master;
        
        try
        {
            // 根據申請狀態決定模式
            if (ShouldShowInEditMode())
            {
                master.SetModeTo("編輯");
            }
            else
            {
                master.SetModeTo("檢視");
            }
        }
        catch (Exception ex)
        {
            // 發生錯誤時預設為檢視模式（安全考量）
            master.SetModeTo("檢視");
            System.Diagnostics.Debug.WriteLine($"設定顯示模式時發生錯誤：{ex.Message}");
        }
    }
    
    /// <summary>
    /// 判斷是否應該顯示為編輯模式
    /// </summary>
    /// <returns>true: 編輯模式, false: 檢視模式</returns>
    private bool ShouldShowInEditMode()
    {
        // 如果沒有 ProjectID，是新申請案件，可以編輯
        if (string.IsNullOrEmpty(ProjectID))
        {
            return true;
        }
        
        try
        {
            // 取得最新版本的狀態
            var ApplicationData = OFS_SciApplicationHelper.getVersionByProjectID(ProjectID);
            if (ApplicationData == null)
            {
                return true; // 沒有資料時允許編輯
            }
            
            // 只有這兩種狀態可以編輯
            string statuses = ApplicationData.Statuses ?? "";
            string statusesName = ApplicationData.StatusesName ?? "";
            
            return statuses == "尚未提送" || statusesName == "補正補件";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得申請狀態時發生錯誤：{ex.Message}");
            return false; // 發生錯誤時預設為檢視模式
        }
    }

    #endregion
}