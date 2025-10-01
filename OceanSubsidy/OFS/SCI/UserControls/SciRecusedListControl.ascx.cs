using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Model.OFS;
using GS.OCA_OceanSubsidy.Operation.OFS;

/// <summary>
/// 科專計畫建議迴避之審查委員清單與技術能力 UserControl
/// 可重複使用的委員迴避清單和技術能力管理元件
/// </summary>
public partial class OFS_SCI_UserControls_SciRecusedListControl : System.Web.UI.UserControl
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
    public string SourcePage { get; set; }

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
            if (!string.IsNullOrEmpty(ProjectID))
            {
                
                LoadData(ProjectID);
                // 載入變更說明控制項
                ucChangeDescription.LoadData(ProjectID);
            }
            // 檢查表單狀態並控制暫存按鈕顯示
            CheckFormStatusAndHideTempSaveButton();
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
    public void LoadData(string projectID)
    {
        try
        {

            if (!string.IsNullOrEmpty(projectID))
            {
                LoadExistingData(projectID);
                LoadTechDiagramFile(projectID);
            }


            // 套用檢視模式
            if (IsViewMode)
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
            // 取得表單資料
            var committeeData = GetCommitteeDataFromForm();

            // 從 Request.Form 讀取 checkbox 狀態
            bool noAvoidanceCommittee = false;
            string checkboxValue = HttpContext.Current.Request.Form[chkNoAvoidance.UniqueID];
            if (!string.IsNullOrEmpty(checkboxValue))
            {
                noAvoidanceCommittee = checkboxValue.ToLower() == "on" || checkboxValue.ToLower() == "true";
            }

            // 如果沒有勾選「無需迴避」，則必須至少有一筆資料
            if (!noAvoidanceCommittee && committeeData.Count == 0)
            {
                result.AddError("請填寫委員迴避資料或勾選「無需迴避之審查委員」");
            }

            // 驗證委員資料必填欄位
            foreach (var item in committeeData)
            {
                if (string.IsNullOrWhiteSpace(item.RecusedName) ||
                    string.IsNullOrWhiteSpace(item.EmploymentUnit) ||
                    string.IsNullOrWhiteSpace(item.JobTitle) ||
                    string.IsNullOrWhiteSpace(item.RecusedReason))
                {
                    result.AddError("委員迴避清單中有必填欄位未填寫");
                    break;
                }
            }

            // 驗證技術能力資料
            var techData = GetTechDataFromForm();
            if (techData.Count == 0)
            {
                result.AddError("請至少填寫一筆技術能力資料");
            }
            foreach (var item in techData)
            {
                if (string.IsNullOrWhiteSpace(item.Name) ||
                    string.IsNullOrWhiteSpace(item.Bef_TRLevel) ||
                    string.IsNullOrWhiteSpace(item.Aft_TRLevel) ||
                    string.IsNullOrWhiteSpace(item.Description))
                {
                    result.AddError("技術能力資料中有必填欄位未填寫");
                    break;
                }
            }

            // 驗證技術能力與技術關聯圖是否已上傳
            if (!string.IsNullOrEmpty(ProjectID))
            {
                var uploadedFiles = OFS_SciWorkSchHelper.GetUploadFilesByProjectIDAndFileCode(ProjectID, "TechnologyDiagram");
                if (uploadedFiles == null || !uploadedFiles.Any())
                {
                    result.AddError("請上傳技術能力與技術關聯圖");
                }
            }

        }
        catch (Exception ex)
        {
            result.AddError($"驗證過程發生錯誤：{ex.Message}");
        }

        return result;
    }

    /// <summary>
    /// 儲存資料
    /// </summary>
    /// <returns>儲存是否成功</returns>
    public bool SaveData(string projectID)
    {
        try
        {
            // 取得表單資料
            var committeeData = GetCommitteeDataFromForm();
            var techData = GetTechDataFromForm();

            // 從 Request.Form 讀取 checkbox 狀態
            bool isChkNoAvoidance = false;
            string checkboxValue = HttpContext.Current.Request.Form[chkNoAvoidance.UniqueID];
            if (!string.IsNullOrEmpty(checkboxValue))
            {
                isChkNoAvoidance = checkboxValue.ToLower() == "on" || checkboxValue.ToLower() == "true";
            }

            // 儲存委員迴避清單
            OFS_SciRecusedList.ReplaceRecusedList(committeeData, ProjectID,isChkNoAvoidance);
            OFS_SciRecusedList.UpdateIsRecused(ProjectID, isChkNoAvoidance);
            // 儲存技術能力資料
            OFS_SciRecusedList.ReplaceTechReadinessList(techData, ProjectID);

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
        hdnCommitteeData.Value = "[]";
        hdnTechData.Value = "[]";
        hdnUploadedFile.Value = "";
    }

    /// <summary>
    /// 載入現有資料
    /// </summary>
    private void LoadExistingData(string projectID)
    {
        try
        {
            // 載入委員迴避清單資料
            var recusedList = OFS_SciRecusedList.GetRecusedListByProjectID(projectID);
            var recusedData = recusedList.Select(x => new
            {
                committeeName = x.RecusedName,
                committeeUnit = x.EmploymentUnit,
                committeePosition = x.JobTitle,
                committeeReason = x.RecusedReason
            }).ToArray();

            // 載入技術成熟度資料
            var techList = OFS_SciRecusedList.GetTechReadinessListByProjectID(projectID);
            var techData = techList.Select(x => new
            {
                techItem = x.Name,
                trlPlanLevel = x.Bef_TRLevel,
                trlTrackLevel = x.Aft_TRLevel,
                techProcess = x.Description
            }).ToArray();

            // 載入 IsRecused 狀態
            bool isRecused = OFS_SciRecusedList.GetIsRecusedByProjectID(projectID);
            
            // 設定 checkbox 狀態
            chkNoAvoidance.Checked = isRecused;

            // 將資料傳遞到前端
            var dataToSend = new
            {
                recusedData = recusedData,
                techData = techData,
                isRecused = isRecused
            };

            var dataJson = new JavaScriptSerializer().Serialize(dataToSend);
            
            string script = $@"
                window.existingData = {dataJson};
                
                // 延遲載入資料，確保 JavaScript 已初始化
                setTimeout(function() {{
                    if (typeof loadExistingData === 'function') {{
                        loadExistingData();
                        console.log('已載入現有資料：', window.existingData);
                        
                        // 如果 IsRecused 為 true，需要鎖定委員表格
                        if (window.existingData.isRecused && typeof clearAndLockCommitteeTable === 'function') {{
                            const $tbody = $('#committeeTableBody');
                            clearAndLockCommitteeTable($tbody);
                            console.log('已鎖定委員表格，因為 IsRecused 為 true');
                        }}
                    }} else {{
                        console.log('loadExistingData 函數未找到');
                    }}
                }}, 200);
            ";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "LoadExistingData", script, true);
        }
        catch (Exception ex)
        {
            // 如果載入失敗，設定空資料
            string fallbackScript = @"
                window.existingData = { recusedData: [], techData: [], isRecused: false };
                console.log('載入資料失敗，使用預設值');
            ";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "LoadExistingData", fallbackScript, true);
            HandleException(ex, "載入現有資料時發生錯誤");
        }
    }

    /// <summary>
    /// 載入技術能力與技術關聯圖
    /// </summary>
    private void LoadTechDiagramFile(string projectID)
    {
        try
        {
            var uploadFiles = OFS_SciWorkSchHelper.GetUploadFilesByProjectIDAndFileCode(projectID, "TechnologyDiagram");

            if (uploadFiles.Any())
            {
                var file = uploadFiles.First();
                bool isProd = Request.Url.Host.Equals("projects.geosense", StringComparison.OrdinalIgnoreCase);

                // 設定 ImageUrl
                if (isProd)
                {
                    file.TemplatePath = ResolveUrl($"~/OceanSubsidy/{file.TemplatePath}");
                }
                else
                {
                    file.TemplatePath = ResolveUrl($"~/{file.TemplatePath}");
                }
                var script = $@"
                    setTimeout(function() {{
                        if (window.techDiagramManager) {{
                            window.techDiagramManager.loadTechDiagramFile('{file.TemplatePath}', '{file.FileName}');
                        }} else {{
                            setTimeout(function() {{
                                if (window.techDiagramManager) {{
                                    window.techDiagramManager.loadTechDiagramFile('{file.TemplatePath}', '{file.FileName}');
                                }}
                            }}, 1000);
                        }}
                    }}, 100);
                ";

                Page.ClientScript.RegisterStartupScript(this.GetType(), "LoadTechDiagramFile", script, true);
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入技術能力與技術關聯圖時發生錯誤");
        }
    }

    /// <summary>
    /// 從表單取得委員資料
    /// </summary>
    private List<OFS_SCI_Other_Recused> GetCommitteeDataFromForm()
    {
        var data = new List<OFS_SCI_Other_Recused>();

        // 取得所有委員相關的 input 欄位
        string[] names = HttpContext.Current.Request.Form.GetValues("committeeName");
        string[] units = HttpContext.Current.Request.Form.GetValues("committeeUnit");
        string[] positions = HttpContext.Current.Request.Form.GetValues("committeePosition");
        string[] reasons = HttpContext.Current.Request.Form.GetValues("committeeReason");

        if (names != null)
        {
            for (int i = 0; i < names.Length; i++)
            {
                // 只處理有資料的行
                if (!string.IsNullOrWhiteSpace(names[i]) ||
                    !string.IsNullOrWhiteSpace(units?[i]) ||
                    !string.IsNullOrWhiteSpace(positions?[i]) ||
                    !string.IsNullOrWhiteSpace(reasons?[i]))
                {
                    data.Add(new OFS_SCI_Other_Recused
                    {
                        ProjectID = ProjectID ?? "",
                        RecusedName = names[i]?.Trim() ?? "",
                        EmploymentUnit = units?[i]?.Trim() ?? "",
                        JobTitle = positions?[i]?.Trim() ?? "",
                        RecusedReason = reasons?[i]?.Trim() ?? ""
                    });
                }
            }
        }

        return data;
    }

    /// <summary>
    /// 從表單取得技術資料
    /// </summary>
    private List<OFS_SCI_Other_TechReadiness> GetTechDataFromForm()
    {
        var data = new List<OFS_SCI_Other_TechReadiness>();

        // 取得所有技術相關的 input 欄位
        string[] techItems = HttpContext.Current.Request.Form.GetValues("techItem");
        string[] trlPlanLevels = HttpContext.Current.Request.Form.GetValues("trlPlanLevel");
        string[] trlTrackLevels = HttpContext.Current.Request.Form.GetValues("trlTrackLevel");
        string[] techProcesses = HttpContext.Current.Request.Form.GetValues("techProcess");

        if (techItems != null)
        {
            for (int i = 0; i < techItems.Length; i++)
            {
                // 只處理有資料的行
                if (!string.IsNullOrWhiteSpace(techItems[i]) ||
                    !string.IsNullOrWhiteSpace(trlPlanLevels?[i]) ||
                    !string.IsNullOrWhiteSpace(trlTrackLevels?[i]) ||
                    !string.IsNullOrWhiteSpace(techProcesses?[i]))
                {
                    data.Add(new OFS_SCI_Other_TechReadiness
                    {
                        ProjectID = ProjectID ?? "",
                        Name = techItems[i]?.Trim() ?? "",
                        Bef_TRLevel = trlPlanLevels?[i]?.Trim() ?? "",
                        Aft_TRLevel = trlTrackLevels?[i]?.Trim() ?? "",
                        Description = techProcesses?[i]?.Trim() ?? ""
                    });
                }
            }
        }

        return data;
    }


    /// <summary>
    /// 套用檢視模式
    /// </summary>
    private void ApplyViewMode()
    {
        if (IsViewMode)
        {
            string script = @"
            <script>
             $(document).ready(function () {
                // 只針對此 UserControl 內的元素進行鎖定
                // 找到 tab4 容器（委員迴避清單）
                var userControl = document.querySelector('#tab4');

                if (!userControl) {
                    console.warn('找不到 UserControl 容器: #tab4');
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
                $('#tab4 #techTable, #tab4 #committeeTable').addClass('hide-col-last');
                });
            </script>";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "AddClassToTable", script);
        }
    }

    /// <summary>
    /// 處理例外錯誤
    /// </summary>
    private void HandleException(Exception ex, string context)
    {
        // 記錄錯誤
        System.Diagnostics.Debug.WriteLine($"{context}: {ex.Message}");

        // 可以在這裡加入更多錯誤處理邏輯，如記錄到日誌
    }
 protected void btnSave_Click(object sender, EventArgs e)
    {
        string ProjectID = Request.QueryString["ProjectID"] ?? "";
        try
        {
            // 驗證 UserControl 資料
            var validationResult = ValidateForm();
            if (!validationResult.IsValid)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ValidationError", 
                    $"alert('請修正以下錯誤：\\n{validationResult.GetErrorsAsString()}');", true);
                return;
            }

            // 儲存 UserControl 資料
            if (SaveData(ProjectID))
            {
                // 儲存變更說明
                ucChangeDescription.SaveChangeDescription(ProjectID);
                

                // 更新版本狀態（暫存）
                UpdateVersionStatusBasedOnAction(ProjectID, false);

                // 重新載入資料
                LoadData(ProjectID);

                // 顯示成功訊息
                Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('儲存成功！');", true);
            }
        }
        catch (Exception ex)
        {
            // 錯誤處理
            Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('儲存失敗：{ex.Message}');", true);
        }
    }

    protected void btnNext_Click(object sender, EventArgs e)
    {
        string ProjectID = Request.QueryString["ProjectID"] ?? "";
        try
        {
            // 驗證 UserControl 資料
            var validationResult = ValidateForm();
            if (!validationResult.IsValid)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ValidationError", 
                    $"alert('請修正以下錯誤：\\n{validationResult.GetErrorsAsString()}');", true);
                return;
            }

            // 儲存 UserControl 資料
            if (SaveData(ProjectID))
            {
                // 儲存變更說明
                ucChangeDescription.SaveChangeDescription(ProjectID);

                // 更新版本狀態（完成）
                UpdateVersionStatusBasedOnAction(ProjectID, true);

                // 顯示成功訊息並導向下一頁
                string redirectScript = $@"
                    window.location.href = 'SciUploadAttachments.aspx?ProjectID={ProjectID}';
                ";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "redirect", redirectScript, true);
            }
        }
        catch (Exception ex)
        {
            // 錯誤處理
            Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('儲存失敗：{ex.Message}');", true);
        }
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
                // 1. Form4Status 設為 "完成" 
                // 2. 檢查 CurrentStep，如果 <= 4 則改成 5
                
                string currentStep = OFS_SciWorkSchHelper.GetCurrentStepByProjectID(ProjectID);
                int currentStepNum = 1;
                int.TryParse(currentStep, out currentStepNum);
                
                bool shouldUpdateCurrentStep = currentStepNum <= 4;
                string newCurrentStep = shouldUpdateCurrentStep ? "5" : currentStep;
                
                // 更新 Form4Status 為 "完成" 和 CurrentStep (如果需要)
                if (shouldUpdateCurrentStep)
                {
                    OFS_SciRecusedList.UpdateForm4StatusAndCurrentStep(ProjectID, "完成", newCurrentStep);
                }
                else
                {
                    OFS_SciRecusedList.UpdateForm4Status(ProjectID, "完成");
                }
            }
            else
            {
                // 點擊「暫存」按鈕
                // 只更新 Form5Status 為 "暫存"，CurrentStep 不變
                
                OFS_SciRecusedList.UpdateForm4Status(ProjectID, "暫存");
            }
        }
        catch (Exception ex)
        {
            
            // 記錄錯誤但不中斷流程
            System.Diagnostics.Debug.WriteLine($"更新版本狀態失敗: {ex.Message}");
        }
    }

    /// <summary>
    /// 檢查表單狀態並控制暫存按鈕顯示
    /// </summary>
    private void CheckFormStatusAndHideTempSaveButton()
    {
        try
        {
            string ProjectID = Request.QueryString["ProjectID"] ?? "";
            if (!string.IsNullOrEmpty(ProjectID))
            {
                    var formStatus = OFS_SciWorkSchHelper.GetFormStatusByProjectID(ProjectID, "Form4Status");

                    if (formStatus == "完成")
                    {
                        // 隱藏暫存按鈕
                        btnTempSave.Style["display"] = "none";
                    }

            }
        }
        catch (Exception ex)
        {
            // 發生錯誤時不隱藏按鈕，讓用戶正常使用
            System.Diagnostics.Debug.WriteLine($"檢查表單狀態失敗: {ex.Message}");
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
                return ucChangeDescription.GetChangeDescriptionBySourcePage(ProjectID, "SciRecusedList");
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