using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.Services;
using GS.App;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Model.OFS;
using GS.OCA_OceanSubsidy.Operation.OFS;
using Newtonsoft.Json;

/// <summary>
/// Control 擴展方法
/// </summary>
public static class ControlExtensions
{
    /// <summary>
    /// 遞歸搜尋控制項
    /// </summary>
    public static Control FindControlRecursive(this Control root, string id)
    {
        if (root.ID == id) return root;

        foreach (Control control in root.Controls)
        {
            Control foundControl = control.FindControlRecursive(id);
            if (foundControl != null) return foundControl;
        }

        return null;
    }
}

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
    protected string ProjectID => Request.QueryString["ProjectID"];

    /// <summary>
    /// 是否為檢視模式
    /// </summary>
    public bool IsViewMode { get; set; }

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

    /// <summary>
    /// 計畫結束日期提示文字
    /// </summary>
    public string PlanEndDateHint { get; private set; }

    /// <summary>
    /// 是否顯示計畫結束日期提示
    /// </summary>
    public bool ShouldShowPlanEndDateHint { get; private set; }

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

    private void SetViewMode()
    {
        if (IsViewMode)
        {
         // 使用 JavaScript 添加檢視模式的 CSS 類別並禁用輸入控制項
        string script = @"
            document.addEventListener('DOMContentLoaded', function() {
                // 只針對此 UserControl 內的元素進行鎖定
                // 找到 tab2 容器（期程及工作項目）
                var userControl = document.querySelector('#tab2');

                if (!userControl) {
                    console.warn('找不到 UserControl 容器: #tab2');
                    return;
                }

                // 禁用此 UserControl 內的所有表單元素
                var formElements = userControl.querySelectorAll('input, textarea, select, button');
                formElements.forEach(function(element) {
                    element.disabled = true;
                    element.readOnly = true;
                });

                // 將此 UserControl 內有 view-mode class 的元件加上 d-none class
                var viewModeElements = userControl.querySelectorAll('.view-mode');
                viewModeElements.forEach(function(element) {
                    element.classList.add('d-none');
                });

                // 特別處理一些可能動態生成的元素
                setTimeout(function() {
                    var dynamicElements = userControl.querySelectorAll('input, textarea, select, button');
                    dynamicElements.forEach(function(element) {
                        if (!element.disabled) {
                            element.disabled = true;
                            element.readOnly = true;
                        }
                    });

                    // 再次處理可能動態生成的 view-mode 元素
                    var dynamicViewModeElements = userControl.querySelectorAll('.view-mode');
                    dynamicViewModeElements.forEach(function(element) {
                        if (!element.classList.contains('d-none')) {
                            element.classList.add('d-none');
                        }
                    });
                }, 1000);

                // 處理表格欄位隱藏
                var table = userControl.querySelector('#checkStandards');
                var table2 = userControl.querySelector('#workItemsTable');

                if (table) {
                    table.classList.add('hide-col-last');
                }
                if (table2) {
                    table2.classList.add('hide-col-last');
                }
            });
        ";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "AddViewModeStyles", script, true);
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
            // 儲存資料
            string message = SaveDraft();

            // 顯示成功訊息
            ShowSuccessMessage(message);
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"暫存失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 儲存並下一步按鈕點擊事件
    /// </summary>
    protected void btnSaveAndNext_Click(object sender, EventArgs e)
    {
        try
        {
            // 儲存並完成下一步
            string message = SaveAndNext();

            // 儲存變更說明 - 透過頁面找到變更說明控制項
            tab2_ucChangeDescription.SaveChangeDescription(ProjectID);

            // 判斷當前頁面是否為 SciInprogress_Approved.aspx
            string currentPage = System.IO.Path.GetFileName(Request.Url.AbsolutePath);
            string redirectUrl = currentPage != "SciInprogress_Approved.aspx"
                ? $"SciFunding.aspx?ProjectID={ProjectID}"
                : "";

            // 顯示成功訊息（如果有 URL 則 1 秒後跳轉）
            ShowSuccessMessage(message, redirectUrl);
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"{ex.Message}");
        }
    }

    /// <summary>
    /// 計畫架構圖上傳按鈕點擊事件
    /// </summary>
    protected void btnUploadDiagram_Click(object sender, EventArgs e)
    {
        try
        {
            // 立即處理檔案上傳
            ProcessDiagramUpload();

            // 重新載入計畫架構圖顯示
            LoadDiagramFile(ProjectID);
            //會觸發postback 所以要先存檔並還原
            BackupFormDataToSession();
            RestoreFormDataFromSession();
            // 顯示成功訊息
            ShowSuccessMessage("計畫架構圖上傳成功！");
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"上傳失敗：{ex.Message}");
        }
    }


    #endregion

    #region 公開方法

    /// <summary>
    /// 載入資料到控制項
    /// </summary>
    /// <param name="projectID">計畫ID</param>
    public void LoadData(string ProjectID)
    {
        try
        {
            if (!string.IsNullOrEmpty(ProjectID))
            {
                LoadExistingData(ProjectID);
                // 載入資料到 UserControl
                SetViewMode();
                // 載入變更說明控制項
                tab2_ucChangeDescription.LoadData(ProjectID, IsViewMode);
            }




        }
        catch (Exception ex)
        {
            HandleException(ex, "載入資料時發生錯誤");
        }
    }

    /// <summary>
    /// 從快照資料載入（用於快照檢視頁面）
    /// </summary>
    /// <param name="snapshotData">快照的 JSON 資料物件</param>
    public void LoadFromSnapshot(dynamic snapshotData)
    {
        try
        {
            // 載入計畫期程（從 ApplicationMain）
            if (snapshotData.ApplicationMain != null)
            {
                OFS_SCI_Application_Main applicationMain = Newtonsoft.Json.JsonConvert.DeserializeObject<OFS_SCI_Application_Main>(
                    snapshotData.ApplicationMain.ToString()
                );
                // 起日欄位顯示純文字
                if (applicationMain != null && applicationMain.StartTime.HasValue)
                {
                    startDateText.InnerText = applicationMain.StartTime.Value.ToMinguoDate();
                }
                else
                {
                    startDateText.InnerText = "自計畫核定日起";
                }
                if (applicationMain != null && applicationMain.EndTime.HasValue)
                {
                    endDate.Value = applicationMain.EndTime.Value.ToMinguoDate();
                }
            }

            // 載入工作項目（WorkSchMain）- 參考 LoadWorkItems 方法
            if (snapshotData.WorkSchMain != null)
            {
                var workSchMain = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OFS_SCI_WorkSch_Main>>(
                    snapshotData.WorkSchMain.ToString()
                );
                if (workSchMain != null && workSchMain.Count > 0)
                {
                    WorkItemsData = workSchMain;

                    // 轉換成前端需要的格式（參考 LoadWorkItems 方法的邏輯）
                    var workItemsJson = JsonConvert.SerializeObject(WorkItemsData.Select(w => new
                    {
                        projectId = w.ProjectID,
                        workItemId = w.WorkItem_id,
                        code = OFS_SciWorkSchHelper.ExtractItemCodeFromWorkItemId(w.WorkItem_id),
                        itemName = w.WorkName,
                        // 將西元年轉換為民國年供前端顯示
                        startYear = w.StartYear.HasValue ? DateTimeHelper.GregorianYearToMinguo(w.StartYear.Value) : (int?)null,
                        startMonth = w.StartMonth,
                        // 將西元年轉換為民國年供前端顯示
                        endYear = w.EndYear.HasValue ? DateTimeHelper.GregorianYearToMinguo(w.EndYear.Value) : (int?)null,
                        endMonth = w.EndMonth,
                        weight = w.Weighting,
                        personMonth = w.InvestMonth,
                        isOutsourced = w.IsOutsourced ?? false
                    }));

                    hiddenWorkItemsData.Value = workItemsJson;

                    // 註冊 JavaScript 腳本來載入資料到前端
                    string script = $@"
                        function loadWorkItemsDataFromSnapshot() {{
                            if (window.sciWorkSchManager) {{
                                window.sciWorkSchManager.loadWorkItems({workItemsJson});
                                console.log('已載入工作項目資料（從快照）：', {workItemsJson});
                            }} else {{
                                console.log('sciWorkSchManager 未初始化，延遲重試...');
                                setTimeout(loadWorkItemsDataFromSnapshot, 500);
                            }}
                        }}

                        if (document.readyState === 'loading') {{
                            document.addEventListener('DOMContentLoaded', loadWorkItemsDataFromSnapshot);
                        }} else {{
                            loadWorkItemsDataFromSnapshot();
                        }}
                    ";

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "LoadWorkItemsFromSnapshot", script, true);
                }
            }

            // 載入查核標準（WorkSchCheckStandard）- 參考 LoadCheckStandards 方法
            if (snapshotData.WorkSchCheckStandard != null)
            {
                var checkStandards = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OFS_SCI_WorkSch_CheckStandard>>(
                    snapshotData.WorkSchCheckStandard.ToString()
                );
                if (checkStandards != null && checkStandards.Count > 0)
                {
                    CheckStandardsData = checkStandards;

                    // HiddenField 儲存西元年格式（供後端解析使用）
                    var checkStandardsForHidden = JsonConvert.SerializeObject(CheckStandardsData.Select(c => new
                    {
                        id = c.Id,
                        projectId = c.ProjectID,
                        workItem = c.WorkItem,
                        serialNumber = c.SerialNumber,
                        plannedFinishDate = c.PlannedFinishDate?.ToString("yyyy-MM-dd") ?? "",  // 西元年
                        description = c.CheckDescription
                    }));

                    hiddenCheckStandardsData.Value = checkStandardsForHidden;

                    // JavaScript 顯示用的資料（民國年格式）
                    var checkStandardsJson = JsonConvert.SerializeObject(CheckStandardsData.Select(c => new
                    {
                        id = c.Id,
                        projectId = c.ProjectID,
                        workItem = c.WorkItem,
                        serialNumber = c.SerialNumber,
                        plannedFinishDate = c.PlannedFinishDate?.ToMinguoDate() ?? "",  // 民國年（僅供前端顯示）
                        description = c.CheckDescription
                    }));

                    // 註冊 JavaScript 腳本來載入資料到前端
                    var script = $@"
                        function loadCheckStandardsDataFromSnapshot() {{
                            if (window.sciWorkSchManager) {{
                                window.sciWorkSchManager.loadCheckStandards({checkStandardsJson});
                                console.log('已載入查核標準資料（從快照）：', {checkStandardsJson});
                            }} else {{
                                console.log('sciWorkSchManager 未初始化，延遲重試...');
                                setTimeout(loadCheckStandardsDataFromSnapshot, 500);
                            }}
                        }}

                        // 延遲 1500ms 確保工作項目已完全載入
                        setTimeout(loadCheckStandardsDataFromSnapshot, 1500);
                    ";

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "LoadCheckStandardsFromSnapshot", script, true);
                }
            }

            // 載入計畫架構圖（UploadFile）- 設定為控制項屬性
            if (snapshotData.UploadFile != null)
            {
                List<OFS_SCI_UploadFile> uploadFiles = JsonConvert.DeserializeObject<List<OFS_SCI_UploadFile>>(
                    snapshotData.UploadFile.ToString()
                );
                if (uploadFiles != null && uploadFiles.Count > 0)
                {
                    OFS_SCI_UploadFile diagramFile = uploadFiles.FirstOrDefault(f => f.FileCode == "WorkSchStructure");
                    if (diagramFile != null && !string.IsNullOrEmpty(diagramFile.TemplatePath))
                    {
                        DiagramFile = diagramFile;

                        // 設定 ImageUrl
                        string resolvedPath = ResolveUrl($"~/{diagramFile.TemplatePath}");
                        DiagramFile.TemplatePath = resolvedPath;
                        diagramPreview.ImageUrl = resolvedPath;
                        System.Diagnostics.Debug.WriteLine($"快照 TemplatePath: {diagramFile.TemplatePath}");

                        // 顯示容器
                        diagramPreviewContainer.Style["display"] = "block";

                        // 註冊 JavaScript 腳本以在前端載入檔案（參考 LoadDiagramFile 方法）
                        var script = $@"
                            if (window.sciWorkSchManager) {{
                                window.sciWorkSchManager.loadDiagramFile('{resolvedPath}', '{System.Web.HttpUtility.JavaScriptStringEncode(diagramFile.FileName)}');
                                console.log('已載入計畫架構圖（從快照）：', '{resolvedPath}');
                            }} else {{
                                setTimeout(function() {{
                                    if (window.sciWorkSchManager) {{
                                        window.sciWorkSchManager.loadDiagramFile('{resolvedPath}', '{System.Web.HttpUtility.JavaScriptStringEncode(diagramFile.FileName)}');
                                        console.log('已載入計畫架構圖（從快照，延遲）：', '{resolvedPath}');
                                    }}
                                }}, 1000);
                            }}
                        ";

                        Page.ClientScript.RegisterStartupScript(this.GetType(), "LoadDiagramFileFromSnapshot", script, true);
                    }
                }
            }

            // 設定為檢視模式
            IsViewMode = true;
            SetViewMode();

            // 隱藏變更說明控制項（快照檢視不需要）
            if (tab2_ucChangeDescription != null)
            {
                tab2_ucChangeDescription.Visible = false;
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "從快照載入資料時發生錯誤");
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
            // 註解：起日欄位改為純文字顯示，不再驗證
            //if (string.IsNullOrEmpty(startDate.Value))
            //{
            //    result.AddError("請選擇計畫開始日期");
            //}

            if (string.IsNullOrEmpty(endDate.Value))
            {
                result.AddError("請選擇計畫結束日期");
            }

            // 註解：起日欄位改為純文字顯示，不再驗證日期區間
            //if (DateTime.TryParse(startDate.Value, out DateTime startDateTime) &&
            //    DateTime.TryParse(endDate.Value, out DateTime endDateTime))
            //{
            //    if (startDateTime >= endDateTime)
            //    {
            //        result.AddError("計畫結束日期必須晚於開始日期");
            //    }
            //}

            // 驗證工作項目
            var workItems = GetWorkItemsFromForm();
            if (workItems.Count == 0)
            {
                result.AddError("請至少新增一個工作項目");
            }
            else
            {
                // 檢查工作項目內容是否有效
                var validWorkItems = workItems.Where(w =>
                    !string.IsNullOrWhiteSpace(w.WorkName) &&
                    w.StartMonth.HasValue && w.StartMonth > 0 &&
                    w.EndMonth.HasValue && w.EndMonth > 0).ToList();

                if (validWorkItems.Count == 0)
                {
                    result.AddError("請至少新增一個有效的工作項目");
                }
            }

            // 驗證查核標準
            var checkStandards = GetCheckStandardsFromForm();
            if (checkStandards.Count == 0)
            {
                result.AddError("請至少新增一個查核標準");
            }
            else
            {
                // 檢查查核標準內容是否有效
                var validCheckStandards = checkStandards.Where(c =>
                    !string.IsNullOrWhiteSpace(c.WorkItem) &&
                    !string.IsNullOrWhiteSpace(c.SerialNumber) &&
                    !string.IsNullOrWhiteSpace(c.CheckDescription) &&
                    c.PlannedFinishDate.HasValue).ToList();

                if (validCheckStandards.Count == 0)
                {
                    result.AddError("請至少新增一個有效的查核標準");
                }
            }

            // 檢查計畫架構圖
            var hasNewUpload = fileUploadDiagram.HasFile;
            var hasExistingDiagram = false;

            // 直接從資料庫查詢是否已有上傳檔案
            if (!string.IsNullOrEmpty(ProjectID))
            {
                var existingFiles = OFS_SciWorkSchHelper.GetUploadFilesByProjectIDAndFileCode(ProjectID, "WorkSchStructure");
                hasExistingDiagram = existingFiles != null && existingFiles.Any();
            }

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

    /// <summary>
    /// 暫存資料（不更新版本狀態為完成）
    /// </summary>
    /// <returns>操作結果訊息</returns>
    public string SaveDraft()
    {
        try
        {
            // 儲存表單資料
            SaveData();

            // 更新版本狀態為暫存
            UpdateVersionStatusBasedOnAction(ProjectID, false);

            // 重新載入資料到 UserControl 並自動渲染到前端
            LoadData(ProjectID);

            return "資料暫存成功！";
        }
        catch (Exception ex)
        {
            throw new Exception($"暫存失敗：{ex.Message}", ex);
        }
    }

    /// <summary>
    /// 儲存並完成下一步
    /// </summary>
    /// <returns>操作結果訊息</returns>
    public string SaveAndNext()
    {
        try
        {
            // 先備份表單資料到 Session
            BackupFormDataToSession();
    
            // 驗證資料
            var validationResult = ValidateForm();
            RestoreFormDataFromSession();
            if (!validationResult.IsValid)
            {
                throw new Exception($"資料驗證失敗：{validationResult.GetErrorsAsString()}");
            }
            // 儲存表單資料
            SaveData();

            // 更新版本狀態為完成並進入下一步
            UpdateVersionStatusBasedOnAction(ProjectID, true);

            // 成功後清除備份的 Session 資料
            ClearBackupSession();

            return "資料儲存成功";
        }
        catch (Exception ex)
        {
            throw new Exception($"{ex.Message}", ex);
        }
    }

   
    /// <summary>
    /// 根據動作類型更新版本狀態
    /// </summary>
    /// <param name="projectID">ProjectID</param>
    /// <param name="isComplete">是否為完成動作（下一步）</param>
    private void UpdateVersionStatusBasedOnAction(string projectID, bool isComplete)
    {
        try
        {
            if (isComplete)
            {
                // 點擊「完成本頁，下一步」按鈕
                // 1. Form2Status 設為 "完成"
                // 2. 檢查 CurrentStep，如果 < 3則改成 3

                string currentStep = OFS_SciWorkSchHelper.GetCurrentStepByProjectID(projectID);
                int currentStepNum = 1;
                int.TryParse(currentStep, out currentStepNum);

                bool shouldUpdateCurrentStep = currentStepNum < 3;
                string newCurrentStep = shouldUpdateCurrentStep ? "3" : currentStep;

                OFS_SciWorkSchHelper.UpdateVersionStatus(projectID, "完成", shouldUpdateCurrentStep, newCurrentStep);
            }
            else
            {
                // 點擊「暫存」按鈕
                // 只更新 Form2Status 為 "暫存"，CurrentStep 不變

                OFS_SciWorkSchHelper.UpdateVersionStatus(projectID, "暫存");
            }
        }
        catch (Exception ex)
        {
            // 記錄錯誤但不中斷流程
            System.Diagnostics.Debug.WriteLine($"更新版本狀態失敗: {ex.Message}");
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
        CheckFormStatusAndHideTempSaveButton();

        // 處理計畫結束日期提示
        ProcessPlanEndDateHint();
    }
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
                tab2_btnTempSave.Style["display"] = "none";

                // 也可以用 Visible 屬性
            }
        }
        catch (Exception ex)
        {
            // 發生錯誤時不隱藏按鈕，讓用戶正常使用
            System.Diagnostics.Debug.WriteLine($"檢查表單狀態失敗: {ex.Message}");
        }
    }

    /// <summary>
    /// 處理計畫結束日期提示
    /// </summary>
    private void ProcessPlanEndDateHint()
    {
        try
        {
            // 預設不顯示提示
            ShouldShowPlanEndDateHint = false;
            PlanEndDateHint = "";

            // 判斷是否應該顯示提示
            // 條件1: 沒有 ProjectID，或
            // 條件2: 專案狀態為 "尚未提送"
            bool shouldShow = false;

            if (string.IsNullOrEmpty(ProjectID))
            {
                // 沒有 ProjectID，顯示提示
                shouldShow = true;
            }
            else
            {
                // 有 ProjectID，檢查狀態
                string projectStatus = OFS_SciWorkSchHelper.GetProjectStatus(ProjectID);
                if (projectStatus == "尚未提送")
                {
                    shouldShow = true;
                }
            }

            // 如果需要顯示提示，從資料庫取得計畫結束日期
            if (shouldShow)
            {
                DateTime? planEndDate = OFS_SciWorkSchHelper.GetCurrentSCIGrantTypePlanEndDate();
                if (planEndDate.HasValue)
                {
                    // 使用 ToMinguoDate 將西元年轉換為民國年字串
                    string planEndDateStr = planEndDate.Value.ToMinguoDate();
                    PlanEndDateHint = $"(期程不可超過 {planEndDateStr})";
                    ShouldShowPlanEndDateHint = true;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"處理計畫結束日期提示失敗: {ex.Message}");
            // 發生錯誤時不顯示提示
            ShouldShowPlanEndDateHint = false;
            PlanEndDateHint = "";
        }
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

        // 起日欄位顯示純文字
        if (startTime.HasValue)
        {
            startDateText.InnerText = startTime.Value.ToMinguoDate();
        }
        else
        {
            startDateText.InnerText = "自計畫核定日起";
        }

        if (endTime.HasValue)
        {
            endDate.Value = endTime.Value.ToMinguoDate();
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
                code = OFS_SciWorkSchHelper.ExtractItemCodeFromWorkItemId(w.WorkItem_id),
                itemName = w.WorkName,
                // 將西元年轉換為民國年供前端顯示
                startYear = w.StartYear.HasValue ? DateTimeHelper.GregorianYearToMinguo(w.StartYear.Value) : (int?)null,
                startMonth = w.StartMonth,
                // 將西元年轉換為民國年供前端顯示
                endYear = w.EndYear.HasValue ? DateTimeHelper.GregorianYearToMinguo(w.EndYear.Value) : (int?)null,
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
            // HiddenField 儲存西元年格式（供後端解析使用）
            var checkStandardsForHidden = JsonConvert.SerializeObject(CheckStandardsData.Select(c => new
            {
                id = c.Id,
                projectId = c.ProjectID,
                workItem = c.WorkItem,
                serialNumber = c.SerialNumber,
                plannedFinishDate = c.PlannedFinishDate?.ToString("yyyy-MM-dd") ?? "",  // 西元年
                description = c.CheckDescription
            }));

            hiddenCheckStandardsData.Value = checkStandardsForHidden;

            // JavaScript 顯示用的資料（民國年格式）
            var checkStandardsJson = JsonConvert.SerializeObject(CheckStandardsData.Select(c => new
            {
                id = c.Id,
                projectId = c.ProjectID,
                workItem = c.WorkItem,
                serialNumber = c.SerialNumber,
                plannedFinishDate = c.PlannedFinishDate?.ToMinguoDate() ?? "",  // 民國年（僅供前端顯示）
                description = c.CheckDescription
            }));

            var script = $@"
                function loadCheckStandardsData() {{
                    if (window.sciWorkSchManager) {{
                        window.sciWorkSchManager.loadCheckStandards({checkStandardsJson});
                        console.log('已載入查核標準資料（延遲）：', {checkStandardsJson});
                    }} else {{
                        console.log('sciWorkSchManager 未初始化，延遲重試...');
                        setTimeout(loadCheckStandardsData, 500);
                    }}
                }}

                // 延遲 1500ms 確保工作項目已完全載入
                setTimeout(loadCheckStandardsData, 1500);
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
            bool isProd = Request.Url.Host.Equals("projects.geosense.tw", StringComparison.OrdinalIgnoreCase);

            // 設定 ImageUrl
            
            DiagramFile.TemplatePath = ResolveUrl($"~/{DiagramFile.TemplatePath}");
            diagramPreview.ImageUrl = ResolveUrl($"~/{DiagramFile.TemplatePath}");
            // 顯示容器
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

        // 註解：起日欄位改為純文字顯示，不再從表單取得起日資料
        //// 處理開始日期：直接從民國年欄位值解析（PostBack 時會自動傳到後端）
        //if (!string.IsNullOrEmpty(startDate.Value))
        //{
        //    if (DateTimeHelper.TryParseMinguoDate(startDate.Value, out DateTime parsedStart))
        //    {
        //        startDateTime = parsedStart;
        //    }
        //}

        // 處理結束日期：直接從民國年欄位值解析（PostBack 時會自動傳到後端）
        if (!string.IsNullOrEmpty(endDate.Value))
        {
            if (DateTimeHelper.TryParseMinguoDate(endDate.Value, out DateTime parsedEnd))
            {
                endDateTime = parsedEnd;
            }
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

                    if (int.TryParse(item.startYear?.ToString(), out int startYear) && startYear > 0)
                    {
                        // 將民國年轉換為西元年
                        workItem.StartYear = DateTimeHelper.MinguoYearToGregorian(startYear);
                    }

                    if (int.TryParse(item.startMonth?.ToString(), out int startMonth) && startMonth > 0)
                    {
                        workItem.StartMonth = startMonth;
                    }

                    if (int.TryParse(item.endYear?.ToString(), out int endYear) && endYear > 0)
                    {
                        // 將民國年轉換為西元年
                        workItem.EndYear = DateTimeHelper.MinguoYearToGregorian(endYear);
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
                TemplatePath = relativePath
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
    /// 例外處理
    /// </summary>
    private void HandleException(Exception ex, string context)
    {
        System.Diagnostics.Debug.WriteLine($"{context}: {ex.Message}");
        // 可以在這裡加入記錄或通知邏輯
    }

    /// <summary>
    /// 顯示成功訊息
    /// </summary>
    /// <param name="message">訊息內容</param>
    /// <param name="redirectUrl">跳轉網址，如果為空則不跳轉</param>
    private void ShowSuccessMessage(string message, string redirectUrl = "")
    {
        string safeMessage = System.Web.HttpUtility.JavaScriptStringEncode(message);

        string script;

        if (!string.IsNullOrEmpty(redirectUrl))
        {
            // 有 URL：顯示 1 秒後自動跳轉
            string safeUrl = System.Web.HttpUtility.JavaScriptStringEncode(redirectUrl);
            script = $@"
                Swal.fire({{
                    title: '成功',
                    text: '{safeMessage}',
                    icon: 'success',
                    timer: 1000,
                    showConfirmButton: false,
                    customClass: {{
                        popup: 'animated fadeInDown'
                    }}
                }}).then(function() {{
                    window.location.href = '{safeUrl}';
                }});
            ";
        }
        else
        {
            // 沒有 URL：正常顯示訊息
            script = $@"
                Swal.fire({{
                    title: '成功',
                    text: '{safeMessage}',
                    icon: 'success',
                    confirmButtonText: '確定',
                    customClass: {{
                        popup: 'animated fadeInDown'
                    }}
                }});
            ";
        }

        Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowSuccessMessage" + Guid.NewGuid().ToString(), script, true);
    }

    /// <summary>
    /// 顯示錯誤訊息
    /// </summary>
    private void ShowErrorMessage(string message, string callback = "")
    {
        string safeMessage = System.Web.HttpUtility.JavaScriptStringEncode(message.Replace("\r\n", "<br>"));

        string script = $@"
            Swal.fire({{
                title: '錯誤',
                html: '{safeMessage}',
                icon: 'error',
                confirmButtonText: '確定',
                customClass: {{
                    popup: 'animated fadeInDown'
                }}
            }})";

        if (!string.IsNullOrEmpty(callback))
        {
            script += $".then(function() {{ {callback} }})";
        }

        script += ";";

        Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowErrorMessage" + Guid.NewGuid().ToString(), script, true);
    }

    /// <summary>
    /// 顯示警告訊息
    /// </summary>
    private void ShowWarningMessage(string message, string callback = "")
    {
        string safeMessage = System.Web.HttpUtility.JavaScriptStringEncode(message);

        string script = $@"
            Swal.fire({{
                title: '警告',
                text: '{safeMessage}',
                icon: 'warning',
                confirmButtonText: '確定',
                customClass: {{
                    popup: 'animated fadeInDown'
                }}
            }})";

        if (!string.IsNullOrEmpty(callback))
        {
            script += $".then(function() {{ {callback} }})";
        }

        script += ";";

        Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowWarningMessage" + Guid.NewGuid().ToString(), script, true);
    }

    /// <summary>
    /// 備份表單資料到 Session
    /// </summary>
    private void BackupFormDataToSession()
    {
        try
        {
            string sessionKey = $"SciWorkSch_FormData_{ProjectID}";

            // 從表單取得資料，參考 SaveData 方法的邏輯
            var projectSchedule = GetProjectScheduleFromForm();
            var workItems = GetWorkItemsFromForm();
            var checkStandards = GetCheckStandardsFromForm();
            
            var backupData = new
            {
                ProjectSchedule = new
                {
                    StartDate = projectSchedule.startDate,
                    EndDate = projectSchedule.endDate
                },
                WorkItems = workItems,
                CheckStandards = checkStandards,
            };

            var serializedData = JsonConvert.SerializeObject(backupData);
            Session[sessionKey] = serializedData;

            System.Diagnostics.Debug.WriteLine($"備份資料完成，Session Key: {sessionKey}");
            System.Diagnostics.Debug.WriteLine($"備份的 JSON 長度: {serializedData.Length}");
            System.Diagnostics.Debug.WriteLine($"備份的 JSON 內容 (前 500 字元): {serializedData.Substring(0, Math.Min(500, serializedData.Length))}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"備份表單資料失敗: {ex.Message}");
        }
    }

    /// <summary>
    /// 從 Session 還原表單資料
    /// </summary>
    private void RestoreFormDataFromSession()
    {
        try
        {
            string sessionKey = $"SciWorkSch_FormData_{ProjectID}";
            string backupJson = Session[sessionKey] as string;

            if (!string.IsNullOrEmpty(backupJson))
            {
                System.Diagnostics.Debug.WriteLine($"Session 中的完整 JSON: {backupJson}");

                // 使用 JObject 而非 dynamic 來避免存取問題
                var backupData = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(backupJson);

                // 檢查反序列化後的完整結構
                System.Diagnostics.Debug.WriteLine($"反序列化後的 backupData: {backupData}");

                // 還原計畫期程資料 - 參考 LoadProjectSchedule 方法的邏輯
                var projectSchedule = backupData["ProjectSchedule"];
                if (projectSchedule != null)
                {
                    // 註解：起日欄位改為純文字顯示，不再從 Session 還原起日
                    //var startDateToken = projectSchedule["StartDate"];
                    //if (startDateToken != null)
                    //{
                    //    if (DateTime.TryParse(startDateToken.ToString(), out DateTime startTime))
                    //    {
                    //        startDate.Value = startTime.ToMinguoDate();
                    //    }
                    //}

                    var endDateToken = projectSchedule["EndDate"];
                    if (endDateToken != null)
                    {
                        if (DateTime.TryParse(endDateToken.ToString(), out DateTime endTime))
                        {
                            endDate.Value = endTime.ToMinguoDate();
                        }
                    }
                }

                // 還原工作項目資料 - 參考 LoadWorkItems 方法的邏輯
                var workItemsToken = backupData["WorkItems"];
                if (workItemsToken != null)
                {
                    string workItemsJsonString = workItemsToken.ToString();
                    var workItemsData = JsonConvert.DeserializeObject<List<OFS_SCI_WorkSch_Main>>(workItemsJsonString);
                    WorkItemsData = workItemsData;

                    if (workItemsData != null && workItemsData.Any())
                    {
                        var workItemsList = new List<object>();
                        foreach (var w in workItemsData)
                        {
                            workItemsList.Add(new
                            {
                                projectId = w.ProjectID,
                                workItemId = w.WorkItem_id,
                                code = OFS_SciWorkSchHelper.ExtractItemCodeFromWorkItemId(w.WorkItem_id),
                                itemName = w.WorkName,
                                // 將西元年轉換為民國年供前端顯示
                                startYear = w.StartYear.HasValue ? DateTimeHelper.GregorianYearToMinguo(w.StartYear.Value) : (int?)null,
                                startMonth = w.StartMonth,
                                // 將西元年轉換為民國年供前端顯示
                                endYear = w.EndYear.HasValue ? DateTimeHelper.GregorianYearToMinguo(w.EndYear.Value) : (int?)null,
                                endMonth = w.EndMonth,
                                weight = w.Weighting,
                                personMonth = w.InvestMonth,
                                isOutsourced = w.IsOutsourced ?? false
                            });
                        }

                        var workItemsJson = JsonConvert.SerializeObject(workItemsList);
                        hiddenWorkItemsData.Value = workItemsJson;

                        string script = $@"
                            function restoreWorkItemsData() {{
                                if (window.sciWorkSchManager) {{
                                    window.sciWorkSchManager.loadWorkItems({workItemsJson});
                                    console.log('已還原工作項目資料');
                                }} else {{
                                    setTimeout(restoreWorkItemsData, 500);
                                }}
                            }}

                            if (document.readyState === 'loading') {{
                                document.addEventListener('DOMContentLoaded', restoreWorkItemsData);
                            }} else {{
                                restoreWorkItemsData();
                            }}
                        ";

                        Page.ClientScript.RegisterStartupScript(this.GetType(), "RestoreWorkItems", script, true);
                    }
                }

                // 還原查核標準資料 - 參考 LoadCheckStandards 方法的邏輯
                var checkStandardsToken = backupData["CheckStandards"];
                if (checkStandardsToken != null)
                {
                    string checkStandardsJsonString = checkStandardsToken.ToString();
                    var checkStandardsData = JsonConvert.DeserializeObject<List<OFS_SCI_WorkSch_CheckStandard>>(checkStandardsJsonString);
                    CheckStandardsData = checkStandardsData;

                    if (checkStandardsData != null && checkStandardsData.Any())
                    {
                        // HiddenField 儲存西元年格式（供後端解析使用）
                        var checkStandardsForHidden = new List<object>();
                        // JavaScript 顯示用的資料（民國年格式）
                        var checkStandardsForDisplay = new List<object>();

                        foreach (var c in checkStandardsData)
                        {
                            // 西元年版本（存入 HiddenField）
                            checkStandardsForHidden.Add(new
                            {
                                id = c.Id,
                                projectId = c.ProjectID,
                                workItem = c.WorkItem,
                                serialNumber = c.SerialNumber,
                                plannedFinishDate = c.PlannedFinishDate?.ToString("yyyy-MM-dd") ?? "",  // 西元年
                                description = c.CheckDescription
                            });

                            // 民國年版本（供前端顯示）
                            checkStandardsForDisplay.Add(new
                            {
                                id = c.Id,
                                projectId = c.ProjectID,
                                workItem = c.WorkItem,
                                serialNumber = c.SerialNumber,
                                plannedFinishDate = c.PlannedFinishDate?.ToMinguoDate() ?? "",  // 民國年
                                description = c.CheckDescription
                            });
                        }

                        var checkStandardsJsonForHidden = JsonConvert.SerializeObject(checkStandardsForHidden);
                        hiddenCheckStandardsData.Value = checkStandardsJsonForHidden;

                        var checkStandardsJson = JsonConvert.SerializeObject(checkStandardsForDisplay);

                        var script = $@"
                            function restoreCheckStandardsData() {{
                                if (window.sciWorkSchManager) {{
                                    window.sciWorkSchManager.loadCheckStandards({checkStandardsJson});
                                    console.log('已還原查核標準資料');
                                }} else {{
                                    setTimeout(restoreCheckStandardsData, 500);
                                }}
                            }}

                            // 延遲 1500ms 確保工作項目已完全載入
                            setTimeout(restoreCheckStandardsData, 1500);
                        ";

                        Page.ClientScript.RegisterStartupScript(this.GetType(), "RestoreCheckStandards", script, true);
                    }
                }

            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"還原表單資料失敗: {ex.Message}");
        }
    }

    /// <summary>
    /// 清除備份的 Session 資料
    /// </summary>
    private void ClearBackupSession()
    {
        try
        {
            string sessionKey = $"SciWorkSch_FormData_{ProjectID}";
            Session.Remove(sessionKey);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"清除備份 Session 失敗: {ex.Message}");
        }
    }

    /// <summary>
    /// 取得此 UserControl 對應的變更說明資料
    /// </summary>
    /// <returns>變更說明資料 (changeBefore, changeAfter)</returns>
    public (string changeBefore, string changeAfter) GetChangeDescriptionData()
    {
        try
        {
            if (!string.IsNullOrEmpty(ProjectID))
            {
                return tab2_ucChangeDescription.GetChangeDescriptionBySourcePage(ProjectID, "SciWorkSch");
            }
            return ("", "");
        }
        catch (Exception ex)
        {
            HandleException(ex, "取得變更說明資料時發生錯誤");
            return ("", "");
        }
    }

    #endregion
}
