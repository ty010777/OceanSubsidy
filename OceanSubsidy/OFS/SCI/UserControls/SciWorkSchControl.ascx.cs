using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Model.OFS;
using GS.OCA_OceanSubsidy.Operation.OFS;
using Newtonsoft.Json;

/// <summary>
/// 科專計畫工作排程 UserControl
/// 可重複使用的工作排程內容元件
/// </summary>
public partial class OFS_SCI_UserControls_SciWorkSchControl : System.Web.UI.UserControl
{
    #region 屬性

    /// <summary>
    /// 目前的計畫ID
    /// </summary>
    public string ProjectID { get; set; }

    /// <summary>
    /// 是否為檢視模式
    /// </summary>
    public bool IsViewMode { get; set; } = false;

    /// <summary>
    /// 工作排程資料
    /// </summary>
    public List<OFS_SCI_WorkSch_Main> WorkItemsData { get; private set; }

    /// <summary>
    /// 查核標準資料
    /// </summary>
    public List<OFS_SCI_WorkSch_CheckStandard> CheckStandardsData { get; private set; }

    /// <summary>
    /// 計畫架構圖檔案
    /// </summary>
    public OFS_SCI_UploadFile DiagramFile { get; private set; }

    #endregion

    #region 頁面事件

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                InitializeControl();
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "UserControl 載入時發生錯誤");
        }
    }

    #endregion

    #region 公開方法

    /// <summary>
    /// 載入資料到控制項
    /// </summary>
    /// <param name="projectID">計畫ID</param>
    /// <param name="isViewMode">是否為檢視模式</param>
    public void LoadData(string projectID, bool isViewMode = false)
    {
        try
        {
            this.ProjectID = projectID;
            this.IsViewMode = isViewMode;

            if (!string.IsNullOrEmpty(projectID))
            {
                LoadExistingData(projectID);
            }

            // 套用檢視模式
            if (isViewMode)
            {
                ApplyViewMode();
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入資料時發生錯誤");
        }
    }

    /// <summary>
    /// 驗證表單資料
    /// </summary>
    /// <returns>驗證結果</returns>
    public ValidationResult ValidateForm()
    {
        var result = new ValidationResult();

        try
        {
            // 驗證期程
            if (string.IsNullOrEmpty(startDate.Value))
            {
                result.AddError("請選擇計畫開始日期");
            }

            if (string.IsNullOrEmpty(endDate.Value))
            {
                result.AddError("請選擇計畫結束日期");
            }

            if (DateTime.TryParse(startDate.Value, out DateTime startDateTime) && 
                DateTime.TryParse(endDate.Value, out DateTime endDateTime))
            {
                if (startDateTime >= endDateTime)
                {
                    result.AddError("計畫結束日期必須晚於開始日期");
                }
            }

            // 驗證工作項目
            var workItems = GetWorkItemsFromForm();
            if (workItems.Count == 0)
            {
                result.AddError("請至少新增一個工作項目");
            }

            // 驗證查核標準
            var checkStandards = GetCheckStandardsFromForm();
            if (checkStandards.Count == 0)
            {
                result.AddError("請至少新增一個查核標準");
            }

            // 檢查計畫架構圖
            var hasExistingDiagram = DiagramFile != null;
            var hasNewUpload = fileUploadDiagram.HasFile;
            
            if (!hasExistingDiagram && !hasNewUpload)
            {
                result.AddError("請上傳計畫架構圖");
            }
        }
        catch (Exception ex)
        {
            result.AddError($"驗證過程發生錯誤：{ex.Message}");
        }

        return result;
    }

    /// <summary>
    /// 儲存表單資料
    /// </summary>
    /// <returns>儲存是否成功</returns>
    public bool SaveData()
    {
        try
        {
            // 處理檔案上傳
            ProcessDiagramUpload();

            // 取得表單資料
            var projectSchedule = GetProjectScheduleFromForm();
            var workItems = GetWorkItemsFromForm();
            var checkStandards = GetCheckStandardsFromForm();

            // 儲存計畫期程
            OFS_SciWorkSchHelper.UpdateProjectSchedule(ProjectID, projectSchedule.startDate, projectSchedule.endDate);

            // 儲存工作項目
            OFS_SciWorkSchHelper.SaveAllWorkItems(ProjectID, workItems);

            // 儲存查核標準
            OFS_SciWorkSchHelper.SaveCheckStandards(checkStandards);

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"儲存資料時發生錯誤：{ex.Message}", ex);
        }
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 初始化控制項
    /// </summary>
    private void InitializeControl()
    {
        // 初始化隱藏欄位
        hiddenWorkItemsData.Value = "[]";
        hiddenCheckStandardsData.Value = "[]";
    }

    /// <summary>
    /// 載入現有資料
    /// </summary>
    private void LoadExistingData(string projectID)
    {
        try
        {
            // 載入計畫期程
            LoadProjectSchedule(projectID);

            // 載入工作項目
            LoadWorkItems(projectID);

            // 載入查核標準
            LoadCheckStandards(projectID);

            // 載入計畫架構圖
            LoadDiagramFile(projectID);
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入現有資料時發生錯誤");
        }
    }

    /// <summary>
    /// 載入計畫期程
    /// </summary>
    private void LoadProjectSchedule(string projectID)
    {
        var (startTime, endTime) = OFS_SciWorkSchHelper.GetProjectScheduleByProjectID(projectID);

        if (startTime.HasValue)
        {
            startDate.Value = startTime.Value.ToString("yyyy-MM-dd");
        }

        if (endTime.HasValue)
        {
            endDate.Value = endTime.Value.ToString("yyyy-MM-dd");
        }
    }

    /// <summary>
    /// 載入工作項目到前端
    /// </summary>
    private void LoadWorkItems(string projectID)
    {
        WorkItemsData = OFS_SciWorkSchHelper.GetWorkItemsByProjectID(projectID);

        if (WorkItemsData != null && WorkItemsData.Any())
        {
            var workItemsJson = JsonConvert.SerializeObject(WorkItemsData.Select(w => new
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

            hiddenWorkItemsData.Value = workItemsJson;

            string script = $@"
                function loadWorkItemsData() {{
                    if (window.sciWorkSchManager) {{
                        window.sciWorkSchManager.loadWorkItems({workItemsJson});
                        console.log('已載入工作項目資料：', {workItemsJson});
                    }} else {{
                        console.log('sciWorkSchManager 未初始化，延遲重試...');
                        setTimeout(loadWorkItemsData, 500);
                    }}
                }}
                
                if (document.readyState === 'loading') {{
                    document.addEventListener('DOMContentLoaded', loadWorkItemsData);
                }} else {{
                    loadWorkItemsData();
                }}
            ";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "LoadWorkItems", script, true);
        }
    }

    /// <summary>
    /// 載入查核標準到前端
    /// </summary>
    private void LoadCheckStandards(string projectID)
    {
        CheckStandardsData = OFS_SciWorkSchHelper.GetCheckStandardsByProjectID(projectID);

        if (CheckStandardsData != null && CheckStandardsData.Any())
        {
            var checkStandardsJson = JsonConvert.SerializeObject(CheckStandardsData.Select(c => new
            {
                id = c.Id,
                projectId = c.ProjectID,
                workItem = c.WorkItem,
                serialNumber = c.SerialNumber,
                plannedFinishDate = c.PlannedFinishDate?.ToString("yyyy-MM-dd"),
                description = c.CheckDescription
            }));

            hiddenCheckStandardsData.Value = checkStandardsJson;

            var script = $@"
                if (window.sciWorkSchManager) {{
                    window.sciWorkSchManager.loadCheckStandards({checkStandardsJson});
                    console.log('已載入查核標準資料：', {checkStandardsJson});
                }} else {{
                    setTimeout(function() {{
                        if (window.sciWorkSchManager) {{
                            window.sciWorkSchManager.loadCheckStandards({checkStandardsJson});
                            console.log('已載入查核標準資料（延遲）：', {checkStandardsJson});
                        }}
                    }}, 1000);
                }}
            ";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "LoadCheckStandards", script, true);
        }
    }

    /// <summary>
    /// 載入計畫架構圖
    /// </summary>
    private void LoadDiagramFile(string projectID)
    {
        var uploadFiles = OFS_SciWorkSchHelper.GetUploadFilesByProjectIDAndFileCode(projectID, "WorkSchStructure");

        if (uploadFiles != null && uploadFiles.Any())
        {
            DiagramFile = uploadFiles.First();
            
            // 顯示預覽
            diagramPreview.ImageUrl = ResolveUrl($"~/{DiagramFile.TemplatePath}");
            diagramPreviewContainer.Style["display"] = "block";

            var script = $@"
                if (window.sciWorkSchManager) {{
                    window.sciWorkSchManager.loadDiagramFile('{DiagramFile.TemplatePath}', '{DiagramFile.FileName}');
                    console.log('已載入計畫架構圖：', '{DiagramFile.TemplatePath}');
                }} else {{
                    setTimeout(function() {{
                        if (window.sciWorkSchManager) {{
                            window.sciWorkSchManager.loadDiagramFile('{DiagramFile.TemplatePath}', '{DiagramFile.FileName}');
                            console.log('已載入計畫架構圖（延遲）：', '{DiagramFile.TemplatePath}');
                        }}
                    }}, 1000);
                }}
            ";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "LoadDiagramFile", script, true);
        }
        else
        {
            diagramPreviewContainer.Style["display"] = "none";
        }
    }

    /// <summary>
    /// 從表單取得計畫期程資料
    /// </summary>
    private (DateTime? startDate, DateTime? endDate) GetProjectScheduleFromForm()
    {
        DateTime? startDateTime = null;
        DateTime? endDateTime = null;

        if (DateTime.TryParse(startDate.Value, out DateTime start))
        {
            startDateTime = start;
        }

        if (DateTime.TryParse(endDate.Value, out DateTime end))
        {
            endDateTime = end;
        }

        return (startDateTime, endDateTime);
    }

    /// <summary>
    /// 從表單取得工作項目資料
    /// </summary>
    private List<OFS_SCI_WorkSch_Main> GetWorkItemsFromForm()
    {
        var workItems = new List<OFS_SCI_WorkSch_Main>();

        try
        {
            if (!string.IsNullOrEmpty(hiddenWorkItemsData.Value))
            {
                var frontendData = JsonConvert.DeserializeObject<List<dynamic>>(hiddenWorkItemsData.Value);

                foreach (var item in frontendData)
                {
                    var workItem = new OFS_SCI_WorkSch_Main();

                    workItem.ProjectID = ProjectID;
                    var code = item.code?.ToString() ?? "";
                    workItem.WorkItem_id = OFS_SciWorkSchHelper.GenerateWorkItemId(ProjectID, code);
                    workItem.WorkName = item.itemName?.ToString() ?? "";

                    if (int.TryParse(item.startMonth?.ToString(), out int startMonth) && startMonth > 0)
                    {
                        workItem.StartMonth = startMonth;
                    }

                    if (int.TryParse(item.endMonth?.ToString(), out int endMonth) && endMonth > 0)
                    {
                        workItem.EndMonth = endMonth;
                    }

                    if (decimal.TryParse(item.weight?.ToString()?.Replace("%", ""), out decimal weight))
                    {
                        workItem.Weighting = weight;
                    }

                    if (decimal.TryParse(item.personMonth?.ToString(), out decimal personMonth))
                    {
                        workItem.InvestMonth = personMonth;
                    }

                    workItem.IsOutsourced = bool.TryParse(item.isOutsourced?.ToString(), out bool isOutsourced) && isOutsourced;

                    workItems.Add(workItem);
                }
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "解析工作項目資料時發生錯誤");
        }

        return workItems;
    }

    /// <summary>
    /// 從表單取得查核標準資料
    /// </summary>
    private List<OFS_SCI_WorkSch_CheckStandard> GetCheckStandardsFromForm()
    {
        var checkStandards = new List<OFS_SCI_WorkSch_CheckStandard>();

        try
        {
            if (!string.IsNullOrEmpty(hiddenCheckStandardsData.Value))
            {
                var frontendData = JsonConvert.DeserializeObject<List<dynamic>>(hiddenCheckStandardsData.Value);

                foreach (var item in frontendData)
                {
                    var checkStandard = new OFS_SCI_WorkSch_CheckStandard();

                    checkStandard.ProjectID = ProjectID;
                    checkStandard.WorkItem = item.workItem?.ToString() ?? "";
                    checkStandard.SerialNumber = item.serialNumber?.ToString() ?? "";
                    checkStandard.CheckDescription = item.description?.ToString() ?? "";

                    if (DateTime.TryParse(item.plannedFinishDate?.ToString(), out DateTime plannedDate))
                    {
                        checkStandard.PlannedFinishDate = plannedDate;
                    }

                    checkStandards.Add(checkStandard);
                }
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "解析查核標準資料時發生錯誤");
        }

        return checkStandards;
    }

    /// <summary>
    /// 處理計畫架構圖上傳
    /// </summary>
    private void ProcessDiagramUpload()
    {
        try
        {
            if (fileUploadDiagram.HasFile)
            {
                var uploadedFile = fileUploadDiagram.PostedFile;

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

                // 建立檔案名稱
                var fileExtension = Path.GetExtension(uploadedFile.FileName).ToLower();
                var fileName = $"WorkSchStructure{fileExtension}";

                // 建立完整檔案路徑
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

                // 相對路徑
                var relativePath = $"UploadFiles/OFS/SCI/{ProjectID}/{fileName}";

                // 儲存到資料庫
                SaveUploadFileRecord(ProjectID, fileName, relativePath);
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "計畫架構圖上傳失敗");
            throw;
        }
    }

    /// <summary>
    /// 儲存上傳檔案記錄到資料庫
    /// </summary>
    private void SaveUploadFileRecord(string projectID, string fileName, string relativePath)
    {
        try
        {
            // 先檢查是否已有該版本的工作排程架構圖記錄
            var existingFiles = OFS_SciWorkSchHelper.GetUploadFilesByProjectIDAndFileCode(projectID, "WorkSchStructure");

            // 如果有舊記錄，先刪除
            if (existingFiles != null && existingFiles.Any())
            {
                foreach (var existingFile in existingFiles)
                {
                    OFS_SciWorkSchHelper.DeleteUploadFile(existingFile.ProjectID, existingFile.FileCode);
                }
            }

            // 建立新的上傳檔案記錄
            var uploadFile = new OFS_SCI_UploadFile
            {
                ProjectID = projectID,
                FileCode = "WorkSchStructure",
                FileName = fileName,
                TemplatePath = relativePath,
                Statuses = "Active"
            };

            // 儲存新記錄到資料庫
            OFS_SciWorkSchHelper.InsertUploadFile(uploadFile);
            DiagramFile = uploadFile;
        }
        catch (Exception ex)
        {
            HandleException(ex, "儲存檔案記錄失敗");
            throw;
        }
    }

    /// <summary>
    /// 套用檢視模式
    /// </summary>
    private void ApplyViewMode()
    {
        if (IsViewMode)
        {
            DisableAllControls(this);
        }
    }

    /// <summary>
    /// 禁用所有控制項
    /// </summary>
    private void DisableAllControls(Control parent)
    {
        foreach (Control control in parent.Controls)
        {
            if (control is TextBox textBox)
                textBox.ReadOnly = true;
            else if (control is DropDownList dropDown)
                dropDown.Enabled = false;
            else if (control is CheckBox checkBox)
                checkBox.Enabled = false;
            else if (control is Button button)
                button.Enabled = false;
            else if (control is FileUpload fileUpload)
                fileUpload.Enabled = false;
            else if (control is HtmlInputGenericControl htmlInput)
                htmlInput.Disabled = true;

            if (control.HasControls())
                DisableAllControls(control);
        }
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
}
