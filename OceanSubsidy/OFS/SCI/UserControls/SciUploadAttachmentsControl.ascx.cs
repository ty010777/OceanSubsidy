using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Model.OFS;
using GS.OCA_OceanSubsidy.Operation.OFS;
using GS.OCA_OceanSubsidy.Operation.OSI.OpenXml;

/// <summary>
/// 海洋科技專案計畫申請 - 上傳附件 UserControl
/// 可重複使用的附件上傳內容元件
/// </summary>
public partial class OFS_SCI_UserControls_SciUploadAttachmentsControl : System.Web.UI.UserControl
{
    #region 屬性

    /// <summary>
    /// 目前的計畫ID
    /// </summary>
    protected string ProjectID => Request.QueryString["ProjectID"];
    
    /// <summary>
    /// 是否為檢視模式
    /// </summary>
    public bool IsViewMode { get; set; } = false;

    #endregion

    // Page_Load 已移除，所有初始化工作整合到 LoadData 方法中

    #region 公開方法

    /// <summary>
    /// 載入資料到控制項
    /// </summary>
    /// <param name="projectID">計畫ID</param>
    /// <param name="IsViewMode">是否為檢視模式</param>
    public void LoadData(string projectID)
    {
        try
        {
            if (!string.IsNullOrEmpty(projectID))
            {
                // 根據OrgCategory決定顯示哪個表單
                DetermineFormType(projectID);
                LoadExistingData(projectID);
                

                // 載入變更說明控制項
                tab5_ucChangeDescription.LoadData(projectID,IsViewMode );


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

    #endregion



    #region 受保護的方法

    /// <summary>
    /// 取得專案 ID，供前端 JavaScript 使用
    /// </summary>
    protected string GetProjectId()
    {
        return ProjectID ?? Session["ProjectID"]?.ToString() ?? Request.QueryString["ProjectID"] ?? "";
    }

    #endregion

    #region 私有方法



    /// <summary>
    /// 載入現有資料
    /// </summary>
    private void LoadExistingData(string projectID)
    {
        try
        {
            // 載入附件上傳狀態
            var attachmentList = OFS_SciUploadAttachmentsHelper.GetAttachmentsByProjectID(projectID);
            
            // 更新頁面上的附件狀態顯示
            UpdateAttachmentStatus(attachmentList);
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入現有資料時發生錯誤");
        }
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
                // 找到 tab5 容器（上傳附件）
                var userControl = document.querySelector('#tab5');

                if (!userControl) {
                    console.warn('找不到 UserControl 容器: #tab5');
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

                    // 處理表格欄位隱藏
                    $('#tab5 #academicTable, #tab5 #oceanTechTable').addClass('hide-col-3');
                }, 500);
            });
            </script>";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "AddClassToTable", script);
        }
    }
    
 

    /// <summary>
    /// 顯示訊息
    /// </summary>
    /// <param name="message">訊息內容</param>
    /// <param name="icon">圖示類型</param>
    /// <param name="redirectUrl">跳轉網址，如果為空則不跳轉</param>
    private void ShowMessage(string message, string icon = "info", string redirectUrl = "")
    {
        string escapedMessage = message.Replace("'", "\\'").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "");

        string script;

        if (!string.IsNullOrEmpty(redirectUrl))
        {
            // 有 URL：顯示 1 秒後自動跳轉
            string safeUrl = System.Web.HttpUtility.JavaScriptStringEncode(redirectUrl);
            script = $@"
                Swal.fire({{
                    icon: '{icon}',
                    title: '訊息',
                    text: '{escapedMessage}',
                    timer: 1000,
                    showConfirmButton: false
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
                    icon: '{icon}',
                    title: '訊息',
                    text: '{escapedMessage}',
                    confirmButtonText: '確定'
                }});
            ";
        }

        Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowSweetAlert" + Guid.NewGuid().ToString(), script, true);
    }

    /// <summary>
    /// 顯示成功訊息
    /// </summary>
    /// <param name="message">訊息內容</param>
    /// <param name="redirectUrl">跳轉網址，如果為空則不跳轉</param>
    private void ShowSuccessMessage(string message, string redirectUrl = "")
    {
        ShowMessage(message, "success", redirectUrl);
    }

    /// <summary>
    /// 顯示錯誤訊息
    /// </summary>
    private void ShowErrorMessage(string message)
    {
        ShowMessage(message, "error");
    }

    /// <summary>
    /// 顯示警告訊息
    /// </summary>
    private void ShowWarningMessage(string message)
    {
        ShowMessage(message, "warning");
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
    /// 根據ProjectID查詢OrgCategory並決定表單類型
    /// </summary>
    /// <param name="projectID">計畫ID</param>
    /// <returns>表單類型："OceanTech" 或 "Academic"</returns>
    private void DetermineFormType(string projectID)
    {
        try
        {
            var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);
            
            if (applicationMain != null && !string.IsNullOrEmpty(applicationMain.OrgCategory))
            {
                if (applicationMain.OrgCategory.Equals("OceanTech", StringComparison.OrdinalIgnoreCase))
                {
                    ShowOceanTechForm();
                }
                else
                {
                    ShowAcademicForm();
                }
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "查詢機構類別時發生錯誤");
        }
    }

    /// <summary>
    /// 顯示學研表單（Academic/Legal）
    /// </summary>
    private void ShowAcademicForm()
    {
        // 移除學研表單的隱藏 class
        string academicCssClass = academicForm.Attributes["class"] ?? "";
        if (academicCssClass.Contains("d-none"))
        {
            academicForm.Attributes["class"] = academicCssClass.Replace("d-none", "").Trim();
        }
        
        // 為業者表單添加隱藏 class
        string oceanTechCssClass = oceanTechForm.Attributes["class"] ?? "";
        if (!oceanTechCssClass.Contains("d-none"))
        {
            oceanTechForm.Attributes["class"] = (oceanTechCssClass + " d-none").Trim();
        }
        
        // 設定契約書名稱為學研版本
        lblContractName.Text = "海洋委員會補助科技專案計畫契約書";
    }

    /// <summary>
    /// 顯示業者表單（OceanTech）
    /// </summary>
    private void ShowOceanTechForm()
    {
        // 為學研表單添加隱藏 class
        string academicCssClass = academicForm.Attributes["class"] ?? "";
            if (!academicCssClass.Contains("d-none"))
        {
            academicForm.Attributes["class"] = (academicCssClass + " d-none").Trim();
        }
        
        // 移除業者表單的隱藏 class
        string oceanTechCssClass = oceanTechForm.Attributes["class"] ?? "";
        if (oceanTechCssClass.Contains("d-none"))
        {
            oceanTechForm.Attributes["class"] = oceanTechCssClass.Replace("d-none", "").Trim();
        }
        
        // 設定契約書名稱為業者版本（在 OceanTech 表單中已經是固定文字）
        // 注意：OceanTech 表單中的契約名稱已經直接寫在 HTML 中
    }

    /// <summary>
    /// 更新附件狀態顯示
    /// </summary>
    private void UpdateAttachmentStatus(List<OFS_SCI_UploadFile> attachmentList)
    {
        if (attachmentList == null || attachmentList.Count == 0)
        {
            System.Diagnostics.Debug.WriteLine("UpdateAttachmentStatus: 沒有找到任何附件記錄");
            return;
        }

        System.Diagnostics.Debug.WriteLine($"UpdateAttachmentStatus: 找到 {attachmentList.Count} 個附件記錄");

        // 建立FileCode對應字典
        var attachmentMapping = GetAttachmentMapping();
        System.Diagnostics.Debug.WriteLine($"UpdateAttachmentStatus: attachmentMapping 包含 {attachmentMapping.Count} 個對應");

        // 根據附件記錄更新狀態
        foreach (var attachment in attachmentList)
        {
            System.Diagnostics.Debug.WriteLine($"UpdateAttachmentStatus: 處理附件 FileCode={attachment.FileCode}, FileName={attachment.FileName}");
            
            if (attachmentMapping.ContainsKey(attachment.FileCode))
            {
                var mapping = attachmentMapping[attachment.FileCode];
                System.Diagnostics.Debug.WriteLine($"UpdateAttachmentStatus: 找到對應的控制項 FileCode={attachment.FileCode}");
                
                try
                {
                    // 更新狀態顯示
                    if (mapping.StatusLabel != null)
                    {
                        mapping.StatusLabel.Text = "已上傳";
                        mapping.StatusLabel.CssClass = "text-success fw-bold";
                        System.Diagnostics.Debug.WriteLine($"UpdateAttachmentStatus: 狀態標籤更新成功 FileCode={attachment.FileCode}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"UpdateAttachmentStatus: 狀態標籤是 null FileCode={attachment.FileCode}");
                    }
                    
                    // 顯示已上傳檔案
                    if (mapping.FilePanel != null)
                    {
                        ShowUploadedFile(mapping.FilePanel, attachment.FileName, attachment.FileCode);
                        System.Diagnostics.Debug.WriteLine($"UpdateAttachmentStatus: 檔案面板更新成功 FileCode={attachment.FileCode}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"UpdateAttachmentStatus: 檔案面板是 null FileCode={attachment.FileCode}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"UpdateAttachmentStatus: 更新附件狀態時發生錯誤 FileCode={attachment.FileCode}, Error={ex.Message}");
                    HandleException(ex, $"更新附件狀態時發生錯誤: {attachment.FileCode}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"UpdateAttachmentStatus: 找不到對應的控制項 FileCode={attachment.FileCode}");
                System.Diagnostics.Debug.WriteLine($"UpdateAttachmentStatus: 可用的 FileCode: {string.Join(", ", attachmentMapping.Keys)}");
            }
        }
    }

    /// <summary>
    /// 顯示已上傳的檔案
    /// </summary>
    private void ShowUploadedFile(Panel filePanel, string fileName, string fileCode)
    {
        filePanel.Visible = true;
        filePanel.Controls.Clear();

        var fileTag = new Literal
        {
            Text = $@"
                <span class='tag tag-green-light'>
                    <a class='tag-link' href='javascript:void(0)' onclick='window.SciUploadAttachments.downloadFile(""{ProjectID}"", ""{fileCode}"", ""{fileName}"")'>{fileName}</a>
                    <button type='button' class='tag-btn' onclick='window.SciUploadAttachments.deleteFile(""{ProjectID}"", ""{fileCode}"", this)'>
                        <i class='fa-solid fa-circle-xmark'></i>
                    </button>
                </span>"
        };

        filePanel.Controls.Add(fileTag);
    }

    /// <summary>
    /// 取得附件對應關係
    /// </summary>
    private Dictionary<string, AttachmentMapping> GetAttachmentMapping()
    {
        var mapping = new Dictionary<string, AttachmentMapping>();
        
        // 學研表單對應
        try
        {
            mapping["FILE_AC2"] = new AttachmentMapping { StatusLabel = FindControl("lblStatusAcademic2") as Label, FilePanel = FindControl("pnlFilesAcademic2") as Panel };
            mapping["FILE_AC3"] = new AttachmentMapping { StatusLabel = FindControl("lblStatusAcademic3") as Label, FilePanel = FindControl("pnlFilesAcademic3") as Panel };
            mapping["FILE_AC4"] = new AttachmentMapping { StatusLabel = FindControl("lblStatusAcademic4") as Label, FilePanel = FindControl("pnlFilesAcademic4") as Panel };
            mapping["FILE_AC5"] = new AttachmentMapping { StatusLabel = FindControl("lblStatusAcademic5") as Label, FilePanel = FindControl("pnlFilesAcademic5") as Panel };
            mapping["FILE_AC6"] = new AttachmentMapping { StatusLabel = FindControl("lblStatusAcademic6") as Label, FilePanel = FindControl("pnlFilesAcademic6") as Panel };
            mapping["FILE_AC7"] = new AttachmentMapping { StatusLabel = FindControl("lblStatusAcademic7") as Label, FilePanel = FindControl("pnlFilesAcademic7") as Panel };
            mapping["FILE_AC9"] = new AttachmentMapping { StatusLabel = FindControl("lblStatusAcademic9") as Label, FilePanel = FindControl("pnlFilesAcademic9") as Panel };
            mapping["FILE_AC11"] = new AttachmentMapping { StatusLabel = FindControl("lblStatusAcademic11") as Label, FilePanel = FindControl("pnlFilesAcademic11") as Panel };
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入學研表單控制項對應時發生錯誤");
        }
        
        // OTech 表單對應  
        try
        {
            mapping["FILE_OTech2"] = new AttachmentMapping { StatusLabel = FindControl("lblStatus_OTech2") as Label, FilePanel = FindControl("pnlFiles_OTech2") as Panel };
            mapping["FILE_OTech3"] = new AttachmentMapping { StatusLabel = FindControl("lblStatus_OTech3") as Label, FilePanel = FindControl("pnlFiles_OTech3") as Panel };
            mapping["FILE_OTech4"] = new AttachmentMapping { StatusLabel = FindControl("lblStatus_OTech4") as Label, FilePanel = FindControl("pnlFiles_OTech4") as Panel };
            mapping["FILE_OTech5"] = new AttachmentMapping { StatusLabel = FindControl("lblStatus_OTech5") as Label, FilePanel = FindControl("pnlFiles_OTech5") as Panel };
            mapping["FILE_OTech6"] = new AttachmentMapping { StatusLabel = FindControl("lblStatus6_OT") as Label, FilePanel = FindControl("pnlFiles_OTech6") as Panel };
            mapping["FILE_OTech8"] = new AttachmentMapping { StatusLabel = FindControl("lblStatusOTech8") as Label, FilePanel = FindControl("pnlFiles_OTech8") as Panel };
            // 向下相容：支援舊格式
            mapping["OTech2"] = new AttachmentMapping { StatusLabel = FindControl("lblStatus_OTech2") as Label, FilePanel = FindControl("pnlFiles_OTech2") as Panel };
            mapping["OTech3"] = new AttachmentMapping { StatusLabel = FindControl("lblStatus_OTech3") as Label, FilePanel = FindControl("pnlFiles_OTech3") as Panel };
            mapping["OTech4"] = new AttachmentMapping { StatusLabel = FindControl("lblStatus_OTech4") as Label, FilePanel = FindControl("pnlFiles_OTech4") as Panel };
            mapping["OTech5"] = new AttachmentMapping { StatusLabel = FindControl("lblStatus_OTech5") as Label, FilePanel = FindControl("pnlFiles_OTech5") as Panel };
            mapping["OTech6"] = new AttachmentMapping { StatusLabel = FindControl("lblStatus6_OT") as Label, FilePanel = FindControl("pnlFiles_OTech6") as Panel };
            mapping["OTech8"] = new AttachmentMapping { StatusLabel = FindControl("lblStatusOTech8") as Label, FilePanel = FindControl("pnlFiles_OTech8") as Panel };
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入OTech表單控制項對應時發生錯誤");
        }

        return mapping;
    }
 #region 頁面操作按鈕事件

    /// <summary>
    /// 暫存按鈕事件
    /// </summary>
    // protected void btnSave_Click(object sender, EventArgs e)
    // {
    //     try
    //     {
    //         // 檢查是否處於編輯模式
    //         if (IsViewMode)
    //         {
    //             ShowMessage("目前為檢視模式，無法執行暫存操作", false);
    //             return;
    //         }
    //
    //
    //         // 儲存變更說明
    //         tab5_ucChangeDescription.SaveChangeDescription(ProjectID);
    //         // 更新專案狀態為暫存
    //         UpdateProjectSaveStatus();
    //
    //         // 記錄操作歷程
    //         LogSaveHistory();
    //
    //         ShowMessage("資料已暫存", true);
    //     }
    //     catch (Exception ex)
    //     {
    //         ShowMessage($"暫存失敗：{ex.Message}", false);
    //     }
    // }

    /// <summary>
    /// 確認提送申請的實際處理
    /// </summary>
    protected void btnSubmitConfirmed_Click(object sender, EventArgs e)
    {
        try
        {
            // 檢查是否處於編輯模式
            if (IsViewMode)
            {
                ShowSweetAlertError("目前為檢視模式，無法執行提送申請操作");
                return;
            }

            // 使用 UserControl 驗證資料
            // var validationResult = ucSciUploadAttachments.ValidateForm();
            // if (!validationResult.IsValid)
            // {
            //     ShowSweetAlertError($"資料驗證失敗：{validationResult.ErrorMessage}");
            //     return;
            // }

            // 儲存資料
            // bool saveSuccess = ucSciUploadAttachments.SaveData(CurrentProjectID);


            // 檢查目前狀態
            var projectData = OFS_SciApplicationHelper.getVersionByProjectID(ProjectID);
            string currentStatusesName = projectData?.StatusesName ?? "";

            // 新增 PDF 合併邏輯
            try
            {
                var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(ProjectID);
                string ProjectName = applicationMain.ProjectNameTw ?? "";
                string orgCategory = applicationMain?.OrgCategory ?? "";
                // // 計畫書修正中 -> 計畫書審核中
                if (currentStatusesName == "計畫書修正中")
                {
                    tab5_ucChangeDescription.SaveChangeDescription(ProjectID);
                    // 驗證計畫變更記錄的 Form1~5 是否都有填寫
                    var validationResult = ValidateChangeRecordForms(ProjectID, 2);
                    if (!validationResult.IsValid)
                    {
                        ShowSweetAlertError($"請先完成變更說明填寫：<br>{validationResult.GetErrorsAsString()}");
                        return;
                    }

                    UpdateProjectStatusForPlanRevision();
                    LogPlanRevisionSubmissionHistory();
                    var changeRecord = OFSProjectChangeRecordHelper.getApplying("SCI", 2, ProjectID);
                    if (changeRecord != null)
                    {
                        changeRecord.Status = 2;
                        OFSProjectChangeRecordHelper.update(changeRecord);
                        System.Diagnostics.Debug.WriteLine($"專案 {ProjectID} 計畫變更記錄狀態更新成功：Status=2");
                    }
                    MergePdfFiles(ProjectID, orgCategory,ProjectName, "核定版");

                }else if (projectData?.IsProjChanged == 1)//計畫變更
                {
                    tab5_ucChangeDescription.SaveChangeDescription(ProjectID);
                    // 驗證計畫變更記錄的 Form1~5 是否都有填寫
                    var validationResult = ValidateChangeRecordForms(ProjectID, 1);
                    if (!validationResult.IsValid)
                    {
                        ShowSweetAlertError($"請先完成變更說明填寫：<br>{validationResult.GetErrorsAsString()}");
                        return;
                    }

                    // 儲存變更說明
                    // 1. 更新 IsProjChanged = 2
                    OFS_SciUploadAttachmentsHelper.UpdateProjectChangeCompleted(ProjectID);

                    // 2. 更新 OFS_ProjectChangeRecord Status = 2
                    var changeRecord = OFSProjectChangeRecordHelper.getApplying("SCI", 1, ProjectID);
                    if (changeRecord != null)
                    {
                        changeRecord.Status = 2;
                        OFSProjectChangeRecordHelper.update(changeRecord);
                        System.Diagnostics.Debug.WriteLine($"專案 {ProjectID} 計畫變更記錄狀態更新成功：Status=2");
                    }

                    // 3. 處理檔案版本控制
                    string uploadFolderPath = Server.MapPath($"~/UploadFiles/OFS/SCI/{ProjectID}/SciApplication");
                    string latestFileNameSuffix = "計畫變更最新版";
                    string latestFullFileName = $"{ProjectID}_科專_{ProjectName}_{latestFileNameSuffix}.pdf";
                    string latestFilePath = Path.Combine(uploadFolderPath, latestFullFileName);
                    
                    // 3.1 檢查「計畫變更最新版」是否存在
                    if (File.Exists(latestFilePath))
                    {
                        // 3.2 取得目前最新的版本號（計畫變更記錄筆數）
                        int latestVersion = OFS_SciUploadAttachmentsHelper.GetLatestChangeVersionNumber(ProjectID);

                        // 3.3 複製「最新版」並改名為 v{版本號+1}
                        int newVersion = latestVersion + 1;
                        string versionFileNameSuffix = $"計畫變更v{newVersion}";
                        string versionFullFileName = $"{ProjectID}_科專_{ProjectName}_{versionFileNameSuffix}.pdf";
                        string versionFilePath = Path.Combine(uploadFolderPath, versionFullFileName);

                        try
                        {
                            File.Copy(latestFilePath, versionFilePath, true);
                            System.Diagnostics.Debug.WriteLine($"已複製：{latestFullFileName} -> {versionFullFileName}");
                        }
                        catch (Exception copyEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"複製檔案時發生錯誤：{copyEx.Message}");
                        }
                    }

                    // 3.4 產生新的「計畫變更最新版」PDF
                    MergePdfFiles(ProjectID, orgCategory, ProjectName, latestFileNameSuffix);
                    
                    // 4. 寄信通知相關人員
                    var projectMain = OFS_SciApplicationHelper.getVersionByProjectID(ProjectID);

                    if (applicationMain != null && projectMain != null)
                    {
                        string projectName = applicationMain.ProjectNameTw;
                        string supervisoryAccount = projectMain.SupervisoryPersonAccount;

                        // 根據承辦人帳號取得 UserID
                        int? organizer = SysUserHelper.GetUserIDByAccount(supervisoryAccount);

                        // 寄送通知信
                        NotificationHelper.G2("科專", projectName, "計畫變更申請", organizer);
                    }

                    
                }
                else //申請階段
                {
                    // 其他狀態的正常流程
                    UpdateProjectStatus();
                    LogSubmissionHistory();

                    // 產生送審版與核定版 PDF
                    MergePdfFiles(ProjectID, orgCategory,ProjectName, "送審版");
                    MergePdfFiles(ProjectID, orgCategory,ProjectName, "核定版");
                }
                
                
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Debug.WriteLine($"{Ex.Message}");
            }

            // 判斷當前頁面是否為 SciInprogress_Approved.aspx
            string currentPage = System.IO.Path.GetFileName(Request.Url.AbsolutePath);
            string redirectUrl ="/OFS/ApplicationChecklist.aspx";

            // 顯示成功訊息（如果有 URL 則 1 秒後跳轉）
            ShowSuccessMessage("提送成功", redirectUrl);
        }
        catch (Exception ex)
        {
            ShowSweetAlertError($"提送申請失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 更新專案狀態為暫存 - 設定 Form5Status 為 '暫存'
    /// </summary>
    private void UpdateProjectSaveStatus()
    {
        try
        {
            OFS_SciUploadAttachmentsHelper.UpdateProjectSaveStatus(ProjectID);
        }
        catch (Exception ex)
        {
            throw new Exception($"更新專案暫存狀態時發生錯誤：{ex.Message}");
        }
    }

    /// <summary>
    /// 更新專案狀態 - 設定 Form5Status 為 '完成'，CurrentStep 為 6 
    /// </summary>
    private void UpdateProjectStatus()
    {
        try
        {
            OFS_SciUploadAttachmentsHelper.UpdateProjectSubmissionStatus(ProjectID);
        }
        catch (Exception ex)
        {
            throw new Exception($"更新專案狀態時發生錯誤：{ex.Message}");
        }
    }

    /// <summary>
    /// 記錄暫存的操作歷程
    /// </summary>
    private void LogSaveHistory()
    {
        try
        {
            var currentUser = GetCurrentUserInfo();
            string userName = currentUser?.UserName ?? "系統";

            // 建立案件歷程記錄
            var caseHistoryLog = new OFS_CaseHistoryLog
            {
                ProjectID = ProjectID,
                ChangeTime = DateTime.Now,
                UserName = userName,
                StageStatusBefore = "編輯中",
                StageStatusAfter = "暫存",
                Description = "暫存附件上傳頁面"
            };

            // 儲存到資料庫
            bool success = ApplicationChecklistHelper.InsertCaseHistoryLog(caseHistoryLog);

            if (success)
            {
                System.Diagnostics.Debug.WriteLine($"暫存歷程記錄已儲存：{ProjectID}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"暫存歷程記錄儲存失敗：{ProjectID}");
            }
        }
        catch (Exception ex)
        {
            // 歷程記錄失敗不影響主要流程，只記錄錯誤
            System.Diagnostics.Debug.WriteLine($"記錄暫存歷程失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 記錄提送申請的操作歷程
    /// </summary>
    private void LogSubmissionHistory()
    {
        try
        {
            var currentUser = GetCurrentUserInfo();
            string userName = currentUser?.UserName ?? "系統";

            // 建立案件歷程記錄
            var caseHistoryLog = new OFS_CaseHistoryLog
            {
                ProjectID = ProjectID,
                ChangeTime = DateTime.Now,
                UserName = userName,
                StageStatusBefore = "編輯中",
                StageStatusAfter = "資格審查 審核中",
                Description = "完成附件上傳並提送申請"
            };

            // 儲存到資料庫
            bool success = ApplicationChecklistHelper.InsertCaseHistoryLog(caseHistoryLog);

            if (success)
            {
                System.Diagnostics.Debug.WriteLine($"提送申請歷程記錄已儲存：{ProjectID}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"提送申請歷程記錄儲存失敗：{ProjectID}");
            }
        }
        catch (Exception ex)
        {
            // 歷程記錄失敗不影響主要流程，只記錄錯誤
            System.Diagnostics.Debug.WriteLine($"記錄提送申請歷程失敗：{ex.Message}");
        }
    }

    // 附件驗證功能已移至 UserControl

    #endregion
    

    #region 輔助方法

    /// <summary>
    /// 顯示訊息 (傳統 alert)
    /// </summary>
    private void ShowMessage(string message, bool isSuccess)
    {
        string alertType = isSuccess ? "success" : "error";
        string script = $"alert('{message}');";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowMessage", script, true);
    }


    /// <summary>
    /// 顯示 SweetAlert 錯誤訊息
    /// </summary>
    private void ShowSweetAlertError(string message)
    {
        string escapedMessage = message.Replace("'", "\\'").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "");
        string script = $@"
            Swal.fire({{
                icon: 'error',
                title: '錯誤',
                html: '{escapedMessage}',
                confirmButtonText: '確定'
            }});
        ";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowSweetAlertError", script, true);
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
    /// 檢查是否為決審核定+審核中狀態
    /// </summary>
    /// <returns>true: 決審核定+審核中, false: 其他狀態</returns>
    private bool IsDecisionReviewMode()
    {
        try
        {
            // 取得最新版本的狀態
            var projectData = OFS_SciApplicationHelper.getVersionByProjectID(ProjectID);
            if (projectData == null)
            {
                return false;
            }

            string statuses = projectData.Statuses ?? "";
            string statusesName = projectData.StatusesName ?? "";

            return statusesName == "計畫書修正中";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"檢查決審核定狀態時發生錯誤：{ex.Message}");
            return false;
        }
    }

  

   

    /// <summary>
    /// 更新計畫書修正狀態 - 計畫書修正中 -> 計畫書審核中
    /// </summary>
    private void UpdateProjectStatusForPlanRevision()
    {
        try
        {
            // 使用ReviewCheckListHelper更新StatusesName
            ReviewCheckListHelper.SCI_UpdateProjectStatusName(ProjectID, "計畫書審核中"
              );
        }
        catch (Exception ex)
        {
            throw new Exception($"更新計畫書修正狀態時發生錯誤：{ex.Message}");
        }
    }

    /// <summary>
    /// 記錄計畫書修正提送申請的操作歷程
    /// </summary>
    private void LogPlanRevisionSubmissionHistory()
    {
        try
        {
            var currentUser = GetCurrentUserInfo();
            string userName = currentUser?.UserName ?? "系統";

            // 建立案件歷程記錄
            var caseHistoryLog = new OFS_CaseHistoryLog
            {
                ProjectID = ProjectID,
                ChangeTime = DateTime.Now,
                UserName = userName,
                StageStatusBefore = "計畫書修正中",
                StageStatusAfter = "計畫書審核中",
                Description = "完成計畫書修正並重新提送審核"
            };

            // 儲存到資料庫
            bool success = ApplicationChecklistHelper.InsertCaseHistoryLog(caseHistoryLog);

            if (success)
            {
                System.Diagnostics.Debug.WriteLine($"計畫書修正提送歷程記錄已儲存：{ProjectID}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"計畫書修正提送歷程記錄儲存失敗：{ProjectID}");
            }
        }
        catch (Exception ex)
        {
            // 歷程記錄失敗不影響主要流程，只記錄錯誤
            System.Diagnostics.Debug.WriteLine($"記錄計畫書修正提送歷程失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 合併 PDF 檔案
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <param name="orgCategory">機構類別</param>
    /// <param name="version">版本（送審版或核定版）</param>
    private void MergePdfFiles(string projectId, string orgCategory,string ProjectName, string version)
    {
        try
        {
            // 建立檔案路徑清單
            var pdfFilePaths = new List<string>();

            // 根據 OrgCategory 決定要合併的檔案 Code
            var fileCodesToMerge = GetFileCodesToMerge(orgCategory);

            // 從資料庫取得檔案路徑並檢查檔案是否存在
            foreach (string fileCode in fileCodesToMerge)
            {
                var uploadedFiles = OFS_SciUploadAttachmentsHelper.GetAttachmentsByFileCodeAndProject(projectId, fileCode);

                if (uploadedFiles != null && uploadedFiles.Count > 0)
                {
                    var uploadedFile = uploadedFiles.First();
                    string fullPath = Server.MapPath($"~/{uploadedFile.TemplatePath}");

                    if (File.Exists(fullPath))
                    {
                        pdfFilePaths.Add(fullPath);
                        System.Diagnostics.Debug.WriteLine($"找到檔案 {fileCode}：{fullPath}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"檔案 {fileCode} 不存在：{fullPath}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"資料庫中找不到檔案記錄，FileCode：{fileCode}");
                }
            }

            // 如果沒有檔案可以合併，直接返回
            if (pdfFilePaths.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("沒有找到任何可合併的 PDF 檔案");
                return;
            }

            // 建立合併後的檔案名稱和路徑
            string mergedFileName = $"{projectId}_科專_{ProjectName}_{version}.pdf";
            string uploadFolderPath = Server.MapPath($"~/UploadFiles/OFS/SCI/{projectId}/SciApplication");
            string mergedFilePath = Path.Combine(uploadFolderPath, mergedFileName);

            // 確保目錄存在
            if (!Directory.Exists(uploadFolderPath))
            {
                Directory.CreateDirectory(uploadFolderPath);
            }

            // 使用 PdfHelper 合併 PDF
            byte[] mergedPdfBytes = PdfHelper.MergePdfs(pdfFilePaths, mergedFilePath);

            System.Diagnostics.Debug.WriteLine($"PDF 合併完成：{mergedFilePath}，合併了 {pdfFilePaths.Count} 個檔案");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"合併 PDF 檔案時發生錯誤：{ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// 根據機構類別取得要合併的檔案 Code 清單
    /// </summary>
    /// <param name="orgCategory">機構類別</param>
    /// <returns>檔案 Code 清單（依指定順序）</returns>
    private List<string> GetFileCodesToMerge(string orgCategory)
    {
        var fileCodes = new List<string>();

        if (orgCategory == "OceanTech")
        {
            // 海洋科技業者（OceanTech）- 6 個檔案（依順序）
            fileCodes.Add("FILE_OTech2");  // 海洋科技科專案計畫書
            fileCodes.Add("FILE_OTech3");  // 建議迴避之審查委員清單
            fileCodes.Add("FILE_OTech4");  // 未違反公職人員利益衝突迴避法切結書
            fileCodes.Add("FILE_OTech5");  // 蒐集個人資料告知事項暨個人資料提供同意書
            fileCodes.Add("FILE_OTech6");  // 申請人自我檢查表
            fileCodes.Add("FILE_OTech8");  // 海洋科技業者科專計畫補助契約書
        }
        else
        {
            // 學術機構/法人機構 - 8 個檔案（依順序）
            fileCodes.Add("FILE_AC2");     // 海洋科技科專案計畫書
            fileCodes.Add("FILE_AC3");     // 建議迴避之審查委員清單
            fileCodes.Add("FILE_AC4");     // 未違反公職人員利益衝突迴避法切結書
            fileCodes.Add("FILE_AC5");     // 蒐集個人資料告知事項暨個人資料提供同意書
            fileCodes.Add("FILE_AC6");     // 共同執行單位基本資料表
            fileCodes.Add("FILE_AC7");     // 申請人自我檢查表
            fileCodes.Add("FILE_AC9");     // 海洋委員會補助科技專案計畫契約書
            fileCodes.Add("FILE_AC11");    // 海洋科技專案成效追蹤自評表
        }

        return fileCodes;
    }

    #endregion
    /// <summary>
    /// 附件對應類別
    /// </summary>
    private class AttachmentMapping
    {
        public Label StatusLabel { get; set; }
        public Panel FilePanel { get; set; }
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
                return tab5_ucChangeDescription.GetChangeDescriptionBySourcePage(ProjectID, "SciUploadAttachments");
            }
            return ("", "");
        }
        catch (Exception ex)
        {
            HandleException(ex, "取得變更說明資料時發生錯誤");
            return ("", "");
        }
    }

    /// <summary>
    /// 檢查最新一筆計畫變更記錄的 Form1~5 Before/After 是否都有填寫
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <param name="method">方法類型（1=計畫變更, 2=計畫書修正）</param>
    /// <returns>ValidationResult 包含是否通過驗證及錯誤清單</returns>
    private ValidationResult ValidateChangeRecordForms(string projectID, int method)
    {
        var result = new ValidationResult();

        try
        {
            // 使用 OFSProjectChangeRecordHelper 取得最新一筆記錄
            var changeRecord = OFSProjectChangeRecordHelper.getApplying("SCI", method, projectID);

            if (changeRecord == null)
            {
                result.AddError("找不到計畫變更記錄");
                return result;
            }

            // 檢查 Form1~5 的 Before 和 After
            CheckFormField(result, 1, changeRecord.Form1Before, changeRecord.Form1After);
            CheckFormField(result, 2, changeRecord.Form2Before, changeRecord.Form2After);
            CheckFormField(result, 3, changeRecord.Form3Before, changeRecord.Form3After);
            CheckFormField(result, 4, changeRecord.Form4Before, changeRecord.Form4After);
            CheckFormField(result, 5, changeRecord.Form5Before, changeRecord.Form5After);

            if (result.IsValid)
            {
                System.Diagnostics.Debug.WriteLine($"專案 {projectID} 的計畫變更記錄驗證通過");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"專案 {projectID} 的計畫變更記錄驗證失敗：{result.GetErrorsAsString()}");
            }

            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"驗證計畫變更記錄時發生錯誤：{ex.Message}");
            result.AddError($"驗證計畫變更記錄時發生錯誤：{ex.Message}");
            return result;
        }
    }

    /// <summary>
    /// 檢查單一表單欄位是否已填寫
    /// </summary>
    private void CheckFormField(ValidationResult result, int formNumber, string beforeValue, string afterValue)
    {
        if (string.IsNullOrWhiteSpace(beforeValue))
        {
            result.AddError($"表單{formNumber}變更前尚未填寫");
        }

        if (string.IsNullOrWhiteSpace(afterValue))
        {
            result.AddError($"表單{formNumber}變更後尚未填寫");
        }
    }

    #endregion
}