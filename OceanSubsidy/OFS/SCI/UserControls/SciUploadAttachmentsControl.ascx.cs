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
    /// 驗證表單資料
    /// </summary>
    /// <returns>驗證結果</returns>
    // public ValidationResult ValidateForm()
    // {
    //     var result = new ValidationResult();
    //
    //     try
    //     {
    //         // 簡單驗證 - 檢查必要附件是否已上傳
    //         var attachmentList = OFS_SciUploadAttachmentsHelper.GetAttachmentsByProjectID(ProjectID);
    //         
    //         if (attachmentList == null || attachmentList.Count == 0)
    //         {
    //             result.AddMessage("提醒：尚未上傳任何附件", false);
    //         }
    //         else
    //         {
    //             result.AddMessage($"已上傳 {attachmentList.Count} 個附件", true);
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         result.AddError($"驗證過程發生錯誤：{ex.Message}");
    //     }
    //
    //     return result;
    // }

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

    #region 範本下載事件處理

    protected void btnDownloadTemplate2_Click(object sender, EventArgs e)
    {
        DownloadTemplate("海洋科技科專案計畫書範本.pdf");
    }

    protected void btnDownloadTemplate3_Click(object sender, EventArgs e)
    {
        DownloadTemplate("建議迴避之審查委員清單範本.pdf");
    }

    protected void btnDownloadTemplate4_Click(object sender, EventArgs e)
    {
        DownloadTemplate("未違反公職人員利益衝突迴避法切結書範本.pdf");
    }

    protected void btnDownloadTemplate5_Click(object sender, EventArgs e)
    {
        DownloadTemplate("蒐集個人資料告知事項暨個人資料提供同意書範本.pdf");
    }

    protected void btnDownloadTemplate6_Click(object sender, EventArgs e)
    {
        DownloadTemplate("共同執行單位基本資料表範本.pdf");
    }

    protected void btnDownloadTemplate7_Click(object sender, EventArgs e)
    {
        DownloadTemplate("申請人自我檢查表範本.pdf");
    }

    protected void btnDownloadTemplate9_Click(object sender, EventArgs e)
    {
        DownloadTemplate("海洋委員會補助科技專案計畫契約書範本.pdf");
    }

    protected void btnDownloadTemplate11_Click(object sender, EventArgs e)
    {
        DownloadTemplate("海洋科技專案成效追蹤自評表範本.pdf");
    }

    #endregion

    #region 檔案上傳事件處理

    protected void btnUpload2_Click(object sender, EventArgs e)
    {
        HandleFileUpload(fuAttachment2, lblStatus2, pnlFiles2, "FILE_AC_002");
    }

    protected void btnUpload3_Click(object sender, EventArgs e)
    {
        HandleFileUpload(fuAttachment3, lblStatus3, pnlFiles3, "FILE_AC_003");
    }

    protected void btnUpload4_Click(object sender, EventArgs e)
    {
        HandleFileUpload(fuAttachment4, lblStatus4, pnlFiles4, "FILE_AC_004");
    }

    protected void btnUpload5_Click(object sender, EventArgs e)
    {
        HandleFileUpload(fuAttachment5, lblStatus5, pnlFiles5, "FILE_AC_005");
    }

    protected void btnUpload6_Click(object sender, EventArgs e)
    {
        HandleFileUpload(fuAttachment6, lblStatus6, pnlFiles6, "FILE_AC_006");
    }

    protected void btnUpload7_Click(object sender, EventArgs e)
    {
        HandleFileUpload(fuAttachment7, lblStatus7, pnlFiles7, "FILE_AC_007");
    }

    protected void btnUpload9_Click(object sender, EventArgs e)
    {
        HandleFileUpload(fuAttachment9, lblStatus9, pnlFiles9, "FILE_AC_009");
    }

    protected void btnUpload11_Click(object sender, EventArgs e)
    {
        HandleFileUpload(fuAttachment11, lblStatus11, pnlFiles11, "FILE_AC_011");
    }

    // OceanTech 表單上傳事件處理 (重用既有邏輯，但用不同的FileCode)
    // 這些事件會被 OceanTech 表單的控制項呼叫，但使用不同的 FileCode 字首
    
    #endregion

    #region 私有方法

    /// <summary>
    /// 初始化控制項
    /// </summary>
    private void InitializeControl()
    {
        // 初始化隱藏欄位
        hdnAttachmentData.Value = "[]";
        // 初始化檔案上傳控制項
        InitializeFileUploadControls();
    }

    /// <summary>
    /// 初始化檔案上傳控制項
    /// </summary>
    private void InitializeFileUploadControls()
    {
        // 設定學研表單檔案上傳控制項的屬性
        SetFileUploadAttributes(fuAttachment2, "accept=\".pdf\"");
        SetFileUploadAttributes(fuAttachment3, "accept=\".pdf\"");
        SetFileUploadAttributes(fuAttachment4, "accept=\".pdf\"");
        SetFileUploadAttributes(fuAttachment5, "accept=\".pdf\"");
        SetFileUploadAttributes(fuAttachment6, "accept=\".pdf\"");
        SetFileUploadAttributes(fuAttachment7, "accept=\".pdf\"");
        SetFileUploadAttributes(fuAttachment9, "accept=\".pdf\"");
        SetFileUploadAttributes(fuAttachment11, "accept=\".pdf\"");
        
        // OceanTech 表單的檔案上傳控制項也需設定相同屬性
        try
        {
            SetFileUploadAttributes(fuAttachment2_OT, "accept=\".pdf\"");
            SetFileUploadAttributes(fuAttachment3_OT, "accept=\".pdf\"");
            SetFileUploadAttributes(fuAttachment4_OT, "accept=\".pdf\"");
            SetFileUploadAttributes(fuAttachment5_OT, "accept=\".pdf\"");
            SetFileUploadAttributes(fuAttachment6_OT, "accept=\".pdf\"");
            SetFileUploadAttributes(fuAttachment8_OT, "accept=\".pdf\"");
        }
        catch (Exception ex)
        {
            // 如果控制項不存在，忽略錯誤 (可能是在設計時時間)
            HandleException(ex, "初始化 OceanTech 表單控制項時發生錯誤");
        }
    }

    /// <summary>
    /// 設定檔案上傳控制項屬性
    /// </summary>
    private void SetFileUploadAttributes(FileUpload fileUpload, string attributes)
    {
        fileUpload.Attributes.Add("accept", ".pdf");
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
    /// 更新附件狀態顯示
    /// </summary>
    private void UpdateAttachmentStatus(List<OFS_SCI_UploadFile> attachmentList = null)
    {
        if (attachmentList == null || attachmentList.Count == 0)
        {
            // 沒有附件記錄，保持預設狀態
            return;
        }

        // 建立FileCode對應字典 (根據目前顯示的表單類型)
        var fileCodeMapping = new Dictionary<string, List<(Label statusLabel, Panel filePanel)>>();
        
        // 學研表單對應
        fileCodeMapping["FILE_AC_002"] = new List<(Label, Panel)> { (lblStatus2, pnlFiles2) };
        fileCodeMapping["FILE_AC_003"] = new List<(Label, Panel)> { (lblStatus3, pnlFiles3) };
        fileCodeMapping["FILE_AC_004"] = new List<(Label, Panel)> { (lblStatus4, pnlFiles4) };
        fileCodeMapping["FILE_AC_005"] = new List<(Label, Panel)> { (lblStatus5, pnlFiles5) };
        fileCodeMapping["FILE_AC_006"] = new List<(Label, Panel)> { (lblStatus6, pnlFiles6) };
        fileCodeMapping["FILE_AC_007"] = new List<(Label, Panel)> { (lblStatus7, pnlFiles7) };
        fileCodeMapping["FILE_AC_009"] = new List<(Label, Panel)> { (lblStatus9, pnlFiles9) };
        fileCodeMapping["FILE_AC_011"] = new List<(Label, Panel)> { (lblStatus11, pnlFiles11) };
        
        // 為 OceanTech 表單新增對應 (使用不同的 FileCode 前綴)
        try
        {
            fileCodeMapping["FILE_OT_002"] = new List<(Label, Panel)> { (lblStatus2_OT, pnlFiles2_OT) };
            fileCodeMapping["FILE_OT_003"] = new List<(Label, Panel)> { (lblStatus3_OT, pnlFiles3_OT) };
            fileCodeMapping["FILE_OT_004"] = new List<(Label, Panel)> { (lblStatus4_OT, pnlFiles4_OT) };
            fileCodeMapping["FILE_OT_005"] = new List<(Label, Panel)> { (lblStatus5_OT, pnlFiles5_OT) };
            fileCodeMapping["FILE_OT_006"] = new List<(Label, Panel)> { (lblStatus6_OT, pnlFiles6_OT) };
            fileCodeMapping["FILE_OT_008"] = new List<(Label, Panel)> { (lblStatus8_OT, pnlFiles8_OT) };
        }
        catch (Exception ex)
        {
            // OceanTech 表單控制項不存在時的錯誤處理
            HandleException(ex, "初始化 OceanTech 表單映射時發生錯誤");
        }

        // 根據附件記錄更新狀態 (更新所有對應的控制項)
        foreach (var attachment in attachmentList)
        {
            if (fileCodeMapping.ContainsKey(attachment.FileCode))
            {
                var controlPairs = fileCodeMapping[attachment.FileCode];
                
                // 更新所有對應的控制項 (學研表單和業者表單)
                foreach (var (statusLabel, filePanel) in controlPairs)
                {
                    try
                    {
                        // 更新狀態顯示
                        statusLabel.Text = "已上傳";
                        statusLabel.CssClass = "";
                        
                        // 顯示已上傳檔案
                        ShowUploadedFile(filePanel, attachment.FileName);
                    }
                    catch (Exception ex)
                    {
                        // 如果控制項不存在或無法存取，跳過
                        HandleException(ex, $"更新附件狀態時發生錯誤: {attachment.FileCode}");
                    }
                }
            }
        }
    }

    /// <summary>
    /// 下載範本檔案
    /// </summary>
    private void DownloadTemplate(string fileName)
    {
        try
        {
            string templatePath = Server.MapPath($"~/UploadFiles/Templates/{fileName}");
            
            if (File.Exists(templatePath))
            {
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", $"attachment; filename=\"{fileName}\"");
                Response.TransmitFile(templatePath);
                Response.End();
            }
            else
            {
                ShowMessage("範本檔案不存在，請聯繫系統管理員。");
            }
        }
        catch (Exception ex)
        {
            ShowMessage($"下載範本時發生錯誤：{ex.Message}");
        }
    }

    /// <summary>
    /// 處理檔案上傳
    /// </summary>
    private void HandleFileUpload(FileUpload fileUpload, Label statusLabel, Panel filePanel, string fileCode)
    {
        try
        {
            if (fileUpload.HasFile)
            {
                // 檢查檔案格式
                if (!IsValidFileFormat(fileUpload.FileName))
                {
                    ShowMessage("僅支援PDF格式檔案上傳。");
                    return;
                }

                // 檢查檔案大小（10MB = 10 * 1024 * 1024 bytes）
                if (fileUpload.PostedFile.ContentLength > 10 * 1024 * 1024)
                {
                    ShowMessage("檔案大小不能超過10MB。");
                    return;
                }

                // 儲存檔案
                string savedFileName = SaveUploadedFile(fileUpload, fileCode);
                
                if (!string.IsNullOrEmpty(savedFileName))
                {
                    // 更新狀態顯示
                    statusLabel.Text = "已上傳";
                    statusLabel.CssClass = "";
                    
                    // 顯示已上傳檔案
                    ShowUploadedFile(filePanel, savedFileName);
                    
                    // 儲存檔案資訊到資料庫
                    SaveAttachmentToDatabase(ProjectID, fileCode, savedFileName, fileUpload);
                    
                    ShowMessage("檔案上傳成功！");
                }
            }
            else
            {
                ShowMessage("請選擇要上傳的檔案。");
            }
        }
        catch (Exception ex)
        {
            ShowMessage($"檔案上傳失敗：{ex.Message}");
        }
    }


    /// <summary>
    /// 檢查檔案格式是否有效
    /// </summary>
    private bool IsValidFileFormat(string fileName)
    {
        return Path.GetExtension(fileName).ToLower() == ".pdf";
    }

    /// <summary>
    /// 儲存上傳的檔案
    /// </summary>
    private string SaveUploadedFile(FileUpload fileUpload, string fileCode)
    {
        try
        {
            string uploadDir = Server.MapPath($"~/UploadFiles/{ProjectID}/Attachments/");
            
            // 確保目錄存在
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            // 產生檔案名稱
            string fileName = $"{ProjectID}_{fileCode}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(fileUpload.FileName)}";
            string filePath = Path.Combine(uploadDir, fileName);

            // 儲存檔案
            fileUpload.SaveAs(filePath);

            return fileName;
        }
        catch (Exception ex)
        {
            throw new Exception($"儲存檔案時發生錯誤：{ex.Message}");
        }
    }

    /// <summary>
    /// 顯示已上傳的檔案
    /// </summary>
    private void ShowUploadedFile(Panel filePanel, string fileName)
    {
        filePanel.Visible = true;
        filePanel.Controls.Clear();

        var fileTag = new Literal
        {
            Text = $@"
                <span class='tag tag-green-light'>
                    <a class='tag-link' href='#' target='_blank'>{fileName}</a>
                    <button type='button' class='tag-btn' onclick='removeFile(this)'>
                        <i class='fa-solid fa-circle-xmark'></i>
                    </button>
                </span>"
        };

        filePanel.Controls.Add(fileTag);
    }

    /// <summary>
    /// 儲存附件資訊到資料庫
    /// </summary>
    private void SaveAttachmentToDatabase(string projectID, string fileCode, string fileName, FileUpload fileUpload)
    {
        try
        {
            string templatePath = $"~/UploadFiles/{projectID}/Attachments/{fileName}";

            OFS_SciUploadAttachmentsHelper.InsertAttachmentRecord(
                projectID, 
                fileCode, 
                fileName, 
                templatePath
            );
        }
        catch (Exception ex)
        {
            // 記錄錯誤但不影響主流程
            System.Diagnostics.Debug.WriteLine($"儲存附件資料庫記錄失敗：{ex.Message}");
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
            else if (control is FileUpload fileUpload)
                fileUpload.Enabled = false;

            if (control.HasControls())
                DisableAllControls(control);
        }
    }

    /// <summary>
    /// 顯示訊息給使用者
    /// </summary>
    private void ShowMessage(string message)
    {
        string script = $"alert('{message.Replace("'", "\\'")}');";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowMessage", script, true);
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

    #endregion
}