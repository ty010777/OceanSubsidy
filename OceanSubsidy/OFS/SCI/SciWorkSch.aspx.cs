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
            // 取得 Version_ID
            var versionId = GetCurrentVersionId();
            if (string.IsNullOrEmpty(versionId))
            {
                Console.WriteLine("無法取得 Version_ID，跳過資料載入");
                return;
            }
            
            // 載入計畫期程
            LoadProjectSchedule(versionId);
            
            // 載入工作項目
            LoadWorkItems(versionId);
            
            // 載入查核標準
            LoadCheckStandards(versionId);
            
            // 載入計畫架構圖
            LoadDiagramFile(versionId);
            
        }
        catch (Exception ex)
        {
            
            throw new Exception($"載入現有資料失敗：{ex.Message}");
        }
    }
    
    /// <summary>
    /// 載入計畫期程到前端
    /// </summary>
    private void LoadProjectSchedule(string versionId)
    {
        var (startTime, endTime) = OFS_SciWorkSchHelper.GetProjectScheduleByVersionId(versionId);
        
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
    private void LoadWorkItems(string versionId)
    {
        var workItems = OFS_SciWorkSchHelper.GetWorkItemsByVersionId(versionId);
        
        if (workItems.Any())
        {
            var workItemsJson = JsonConvert.SerializeObject(workItems.Select(w => new
            {
                versionId = w.Version_ID,
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
    private void LoadCheckStandards(string versionId)
    {
        var checkStandards = OFS_SciWorkSchHelper.GetCheckStandardsByVersionId(versionId);
        
        if (checkStandards.Any())
        {
            var checkStandardsJson = JsonConvert.SerializeObject(checkStandards.Select(c => new
            {
                id = c.Id,
                versionId = c.Version_ID,
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
    private void LoadDiagramFile(string versionId)
    {
        var uploadFiles = OFS_SciWorkSchHelper.GetUploadFilesByVersionIdAndFileCode(versionId, "WorkSchStructure");
        
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
                ShowMessage("資料儲存成功！即將跳轉到下一頁...", "success");
                // 可以在這裡重導向到下一頁
                // Response.Redirect("NextPage.aspx");
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
            // 除錯：檢查所有收到的檔案
            var allFileKeys = new List<string>();
            var fileInfos = new List<object>();
            
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var key = Request.Files.AllKeys[i];
                var file = Request.Files[i];
                allFileKeys.Add(key);
                fileInfos.Add(new { 
                    Key = key, 
                    FileName = file?.FileName ?? "null", 
                    ContentLength = file?.ContentLength ?? 0,
                    ContentType = file?.ContentType ?? "null"
                });
            }
            
            // 檢查是否有檔案上傳 - 先嘗試不同的 key
            var uploadedFile = Request.Files["diagramFile"];
            
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

                // 取得 Version_ID
                var versionId = GetCurrentVersionId();
                if (string.IsNullOrEmpty(versionId))
                {
                    throw new Exception("無法取得計畫版本資訊");
                }

                // 建立檔案名稱
                var fileExtension = Path.GetExtension(uploadedFile.FileName).ToLower();
                var fileName = $"{versionId}_WorkSchStructure{fileExtension}";
                
                // 建立完整檔案路徑
                var uploadDir = Server.MapPath("~/UploadFiles/OFS/SCI/");
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
                var relativePath = $"UploadFiles/OFS/SCI/{fileName}";
                
                // 儲存到資料庫
                SaveUploadFileRecord(versionId, fileName, relativePath);
                
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
        // 取得Version_ID
        var versionId = GetCurrentVersionId();
        
        // 1. 處理計畫期程 - 儲存到 OFS_SCI_Application_Main
        var projectSchedule = ReadProjectScheduleData();
        OFS_SciWorkSchHelper.UpdateProjectSchedule(versionId, projectSchedule.startDate, projectSchedule.endDate);
        
        // 2. 讀取工作項目資料 (包含主項和子項)
        var allWorkItems = ReadAllWorkItemsData(versionId);
        
        // 3. 儲存所有工作項目到單表
        int savedCount = OFS_SciWorkSchHelper.SaveAllWorkItems(versionId, allWorkItems);
        
        // 4. 讀取查核標準資料
        var checkStandards = ReadCheckStandardsData(versionId);
        
        // 5. 儲存查核標準
        int savedCheckStandardsCount = OFS_SciWorkSchHelper.SaveCheckStandards(checkStandards);
        
        // 6. 更新版本狀態
        UpdateVersionStatusBasedOnAction(versionId, isComplete);
    }

    /// <summary>
    /// 根據動作類型更新版本狀態
    /// </summary>
    /// <param name="versionId">版本ID</param>
    /// <param name="isComplete">是否為完成動作（下一步）</param>
    private void UpdateVersionStatusBasedOnAction(string versionId, bool isComplete)
    {
        try
        {
            if (isComplete)
            {
                // 點擊「完成本頁，下一步」按鈕
                // 1. Form2Status 設為 "完成" 
                // 2. 檢查 CurrentStep，如果 < 3則改成 3
                
                string currentStep = OFS_SciWorkSchHelper.GetCurrentStepByVersionId(versionId);
                int currentStepNum = 1;
                int.TryParse(currentStep, out currentStepNum);
                
                bool shouldUpdateCurrentStep = currentStepNum < 3;
                string newCurrentStep = shouldUpdateCurrentStep ? "3" : currentStep;
                
                OFS_SciWorkSchHelper.UpdateVersionStatus(versionId, "完成", shouldUpdateCurrentStep, newCurrentStep);
            }
            else
            {
                // 點擊「暫存」按鈕
                // 只更新 Form2Status 為 "暫存"，CurrentStep 不變
                
                OFS_SciWorkSchHelper.UpdateVersionStatus(versionId, "暫存");
                
            }
        }
        catch (Exception ex)
        {
            var errorScript = $"console.error('版本狀態更新失敗：', '{ex.Message.Replace("'", "\\'")}');";
            ClientScript.RegisterStartupScript(this.GetType(), "LogVersionUpdateError", errorScript, true);
            
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
    /// <param name="versionId">版本ID</param>
    private List<OFS_SCI_WorkSch_Main> ReadAllWorkItemsData(string versionId)
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
                    workItem.Version_ID = versionId;
                    var itemCode = item.code?.ToString() ?? "";
                    workItem.WorkItem_id = OFS_SciWorkSchHelper.GenerateWorkItemId(versionId, itemCode);
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
    /// <param name="versionId">版本ID</param>
    private List<OFS_SCI_WorkSch_CheckStandard> ReadCheckStandardsData(string versionId)
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
                    
                    checkStandard.Version_ID = versionId;
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
                w.Version_ID,
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
                c.Version_ID,
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
            console.log('WorkItem_id 格式：{{Version_ID}}_{{項目代碼}} (例如：v001_SCI_2024_A1)');
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
    /// 取得目前的 Version_ID
    /// </summary>
    private string GetCurrentVersionId()
    {
        try
        {
            // 從 URL 參數取得 ProjectID
            var projectId = Request.QueryString["ProjectID"];
            if (string.IsNullOrEmpty(projectId))
            {
                projectId = Request.QueryString["projectId"]; // 嘗試小寫
            }
            
            if (string.IsNullOrEmpty(projectId))
            {
                throw new Exception("無法取得ProjectID參數");
            }
            
            // 呼叫 Helper 方法取得最新的 version_id
            var lastVersion = OFS_SciApplicationHelper.getVersionLatestProjectID(projectId);
            
            return lastVersion.Version_ID;
        }
        catch (Exception ex)
        {
            throw new Exception($"取得Version_ID失敗：{ex.Message}");
        }
    }
    #endregion

    #region 檔案上傳處理 (ASP.NET Button Click 方式)

    /// <summary>
    /// 儲存上傳檔案記錄到資料庫
    /// </summary>
    private void SaveUploadFileRecord(string versionId, string fileName, string relativePath)
    {
        try
        {
            // 先檢查是否已有該版本的工作排程架構圖記錄
            var existingFiles = OFS_SciWorkSchHelper.GetUploadFilesByVersionIdAndFileCode(versionId, "WorkSchStructure");
            
            // 如果有舊記錄，先刪除
            if (existingFiles != null && existingFiles.Any())
            {
                foreach (var existingFile in existingFiles)
                {
                    // 從資料庫刪除記錄
                    OFS_SciWorkSchHelper.DeleteUploadFile(existingFile.Version_ID, existingFile.FileCode);
                }
            }
            
            // 建立新的上傳檔案記錄
            var uploadFile = new OFS_SCI_UploadFile
            {
                Version_ID = versionId,
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
}