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
    public string ProjectID { get; set; }

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
    /// <param name="isViewMode">是否為檢視模式</param>
    public void LoadData(string projectID, bool isViewMode = false)
    {
        try
        {
            // 執行基本初始化（原本在 Page_Load 中的工作）
            InitializeControl();
            
            this.ProjectID = projectID;
            this.IsViewMode = isViewMode;

            if (!string.IsNullOrEmpty(projectID))
            {
                // 根據OrgCategory決定顯示哪個表單
                DetermineFormType(projectID);
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
    /// 儲存表單資料
    /// </summary>
    /// <returns>儲存是否成功</returns>
    public bool SaveData(string projectID)
    {
        this.ProjectID = projectID;
        try
        {
            // 附件資料更新功能暫時移除，待實際需求確認後再實作
            
            
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"儲存資料時發生錯誤：{ex.Message}", ex);
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

    private void InitializeControl()
    {
        // 初始化隱藏欄位
        hdnAttachmentData.Value = "[]";
        // 由於現在使用原生 HTML input，不需要初始化 FileUpload 控制項
    }

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
            if (control is Button button)
                button.Enabled = false;
            // FileUpload 控制項已移除，不需要處理

            if (control.HasControls())
                DisableAllControls(control);
        }
    }

    /// <summary>
    /// 顯示訊息給使用者
    /// </summary>
    private void ShowMessage(string message, string icon = "info")
    {
        string escapedMessage = message.Replace("'", "\\'").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "");
        string script = $@"
            Swal.fire({{
                icon: '{icon}',
                title: '訊息',
                text: '{escapedMessage}',
                confirmButtonText: '確定'
            }});";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowSweetAlert", script, true);
    }

    /// <summary>
    /// 顯示成功訊息
    /// </summary>
    private void ShowSuccessMessage(string message)
    {
        ShowMessage(message, "success");
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
            mapping["FILE_AC9"] = new AttachmentMapping { StatusLabel = FindControl("lblStatus9") as Label, FilePanel = FindControl("pnlFiles9") as Panel };
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

    /// <summary>
    /// 附件對應類別
    /// </summary>
    private class AttachmentMapping
    {
        public Label StatusLabel { get; set; }
        public Panel FilePanel { get; set; }
    }

    #endregion
}