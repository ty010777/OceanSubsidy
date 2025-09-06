using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Operation.OFS;
using GS.App;
using GS.OCA_OceanSubsidy.Operation.OSI.OpenXml;

/// <summary>
/// 海洋科技專案計畫申請 - 上傳附件頁面
/// </summary>
public partial class OFS_SCI_SciUploadAttachments : System.Web.UI.Page
{
    /// <summary>
    /// 目前編輯的計畫ID
    /// </summary>
    protected string CurrentProjectID
    {
        get { return Request.QueryString["ProjectID"] ?? ""; }
    }

    /// <summary>
    /// 頁面載入事件
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // 處理上傳和下載請求
        string action = Request.QueryString["action"];
        if (!string.IsNullOrEmpty(action))
        {
            string projectId = Request.QueryString["ProjectID"] ?? Session["ProjectID"]?.ToString() ?? CurrentProjectID;
            string fileCode = Request.QueryString["fileCode"];
            
            switch (action)
            {
                case "upload":
                    HandleUpload(projectId);
                    return;
                case "downloadTemplate":
                    DownloadTemplate(projectId, fileCode);
                    return;
                case "downloadFile":
                    DownloadUploadedFile(projectId, fileCode);
                    return;
                case "deleteFile":
                    DeleteUploadedFile(projectId, fileCode);
                    return;
            }
        }
        
        if (!IsPostBack)
        {
            // 檢查是否有計畫ID
            if (string.IsNullOrEmpty(CurrentProjectID))
            {
                Response.Redirect("~/OFS/ApplicationChecklist.aspx");
                return;
            }

            // 初始化頁面
            InitializePage();
            
            // 載入 UserControl 資料
            ucSciUploadAttachments.LoadData(CurrentProjectID, !ShouldShowInEditMode());
            
            // 載入變更說明控制項
            ucChangeDescription.LoadData(CurrentProjectID, !ShouldShowInEditMode());
            
            // 檢查表單狀態並控制暫存按鈕顯示
            CheckFormStatusAndHideTempSaveButton();
            
            // 載入變更說明資料到輸入框
            LoadChangeDescriptionData();
        }
    }

    /// <summary>
    /// 初始化頁面
    /// </summary>
    private void InitializePage()
    {
        try
        {
            // Master Page 會自動從 QueryString 讀取 ProjectID，
            // 所以這裡不需要額外設定

            // 設定頁面顯示模式
            SetPageDisplayMode();
        }
        catch (Exception ex)
        {
            ShowMessage($"頁面初始化錯誤：{ex.Message}", false);
        }
    }

    /// <summary>
    /// 設定頁面顯示模式
    /// </summary>
    private void SetPageDisplayMode()
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
        if (string.IsNullOrEmpty(CurrentProjectID))
        {
            return true;
        }
        
        try
        {
            // 取得最新版本的狀態
            var projectData = OFS_SciApplicationHelper.getVersionByProjectID(CurrentProjectID);
            if (projectData == null)
            {
                return true; // 沒有資料時允許編輯
            }
            
            // 只有這些狀態可以編輯
            string statuses = projectData.Statuses ?? "";
            string statusesName = projectData.StatusesName ?? "";
            
            return statuses == "尚未提送" || 
                   statusesName == "補正補件" || 
                   statusesName == "計畫書修正中";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得申請狀態時發生錯誤：{ex.Message}");
            return false; // 發生錯誤時預設為檢視模式
        }
    }

    // 檔案上傳相關功能已移至 UserControl，這裡保留空白以維持程式結構
    // 如需要可在未來重新實作或移除

    #region 頁面操作按鈕事件

    /// <summary>
    /// 暫存按鈕事件
    /// </summary>
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            // 檢查是否處於編輯模式
            if (!ShouldShowInEditMode())
            {
                ShowMessage("目前為檢視模式，無法執行暫存操作", false);
                return;
            }

            // 使用 UserControl 儲存資料
            bool saveSuccess = ucSciUploadAttachments.SaveData(CurrentProjectID);
            
            if (saveSuccess)
            {
                // 儲存變更說明
                ucChangeDescription.SaveChangeDescription(CurrentProjectID);
            }
            
            if (saveSuccess)
            {
                // 更新專案狀態為暫存
                UpdateProjectSaveStatus();
                
                // 記錄操作歷程
                LogSaveHistory();

                ShowMessage("資料已暫存", true);
            }
            else
            {
                ShowMessage("暫存失敗", false);
            }
        }
        catch (Exception ex)
        {
            ShowMessage($"暫存失敗：{ex.Message}", false);
        }
    }

    /// <summary>
    /// 確認提送申請的實際處理
    /// </summary>
    protected void btnSubmitConfirmed_Click(object sender, EventArgs e)
    {
        try
        {
            // 檢查是否處於編輯模式
            if (!ShouldShowInEditMode())
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
            bool saveSuccess = ucSciUploadAttachments.SaveData(CurrentProjectID);
            if (saveSuccess)
            {
                // 儲存變更說明
                ucChangeDescription.SaveChangeDescription(CurrentProjectID);
            }
            
            if (!saveSuccess)
            {
                ShowSweetAlertError("資料儲存失敗，無法提送申請");
                return;
            }
            
            // 檢查目前狀態
            var projectData = OFS_SciApplicationHelper.getVersionByProjectID(CurrentProjectID);
            string currentStatusesName = projectData?.StatusesName ?? "";
            
            if (currentStatusesName == "計畫書修正中")
            {
                // 計畫書修正中 -> 計畫書審核中
                UpdateProjectStatusForPlanRevision();
                LogPlanRevisionSubmissionHistory();
            }
            else
            {
                // 其他狀態的正常流程
                UpdateProjectStatus();
                LogSubmissionHistory();
            }

            // 顯示成功訊息並跳轉
            ShowSweetAlertSuccess();
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
            OFS_SciUploadAttachmentsHelper.UpdateProjectSaveStatus(CurrentProjectID);
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
            OFS_SciUploadAttachmentsHelper.UpdateProjectSubmissionStatus(CurrentProjectID);
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
                ProjectID = CurrentProjectID,
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
                System.Diagnostics.Debug.WriteLine($"暫存歷程記錄已儲存：{CurrentProjectID}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"暫存歷程記錄儲存失敗：{CurrentProjectID}");
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
                ProjectID = CurrentProjectID,
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
                System.Diagnostics.Debug.WriteLine($"提送申請歷程記錄已儲存：{CurrentProjectID}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"提送申請歷程記錄儲存失敗：{CurrentProjectID}");
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

    #region 檔案上傳下載處理

    /// <summary>
    /// 處理檔案上傳
    /// </summary>
    private void HandleUpload(string projectId)
    {
        try
        {
            // 檢查是否有上傳的檔案
            if (Request.Files.Count == 0)
            {
                Response.Write("ERROR:沒有檔案被上傳");
                return;
            }
            
            var file = Request.Files[0];
            if (file == null || file.ContentLength == 0)
            {
                Response.Write("ERROR:檔案為空");
                return;
            }
            
            // 取得參數
            string attachmentNumber = Request.QueryString["attachmentNumber"];
            
            if (string.IsNullOrEmpty(attachmentNumber))
            {
                Response.Write("ERROR:附件編號不能為空");
                return;
            }
            
            if (string.IsNullOrEmpty(projectId))
            {
                Response.Write("ERROR:計畫編號不能為空");
                return;
            }
            
            // 驗證檔案格式
            string fileExt = Path.GetExtension(file.FileName).ToLower();
            if (fileExt != ".pdf")
            {
                Response.Write("ERROR:僅支援PDF格式檔案");
                return;
            }
            
            // 檢查檔案大小 (10MB)
            const int maxFileSize = 10 * 1024 * 1024;
            if (file.ContentLength > maxFileSize)
            {
                Response.Write("ERROR:檔案大小不能超過10MB");
                return;
            }
            
            // 生成檔案代碼和名稱
            string fileCode = attachmentNumber;
            string fileName = OFS_SciUploadAttachmentsHelper.GenerateFileName(projectId, fileCode);
            
            // 建構檔案路徑
            string relativePath = $"UploadFiles/OFS/SCI/{projectId}/SciApplication/{fileName}";
            string fullPath = Server.MapPath($"~/{relativePath}");
            
            // 確保目錄存在
            string directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            // 如果檔案已存在，先刪除舊檔案
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            
            // 儲存檔案到實體路徑
            file.SaveAs(fullPath);
            
            // 更新資料庫記錄 (使用現有的 UpdateAttachmentRecord 方法，該方法會自動處理新增或更新)
            OFS_SciUploadAttachmentsHelper.UpdateAttachmentRecord(
                projectId,
                fileCode,
                fileName,
                relativePath
            );
            
            Response.Write($"SUCCESS:{fileName}");
        }
        catch (Exception ex)
        {
            Response.Write($"ERROR:上傳失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 處理範本檔案下載
    /// </summary>
    private void DownloadTemplate(string projectId, string fileCode)
    {
        try
        {
            if (string.IsNullOrEmpty(fileCode))
            {
                Response.Clear();
                Response.Write("ERROR:缺少檔案代碼參數");
                Response.End();
                return;
            }

            string filePath = "";
            
            // OTech 業者範本檔案對應
            switch (fileCode)
            {
                case "FILE_OTech1":
                    filePath = Server.MapPath("~/Template/SCI/OTech/附件-01海洋委員會海洋科技專案補助作業要點.docx");
                    filePath = ApplyProjectDataToWord_FILE_OTech1(filePath, projectId);
                    break;
                case "FILE_OTech2":
                    filePath = Server.MapPath("~/Template/SCI/OTech/附件-02海洋科技科專案計畫書.zip");
                    break;
                case "FILE_OTech3":
                    filePath = Server.MapPath("~/Template/SCI/OTech/附件-03建議迴避之審查委員清單.docx");
                    break;
                case "FILE_OTech4":
                    filePath = Server.MapPath("~/Template/SCI/OTech/附件-04未違反公職人員利益衝突迴避法切結書.docx");
                    filePath = ApplyProjectDataToWord_FILE_OTech4(filePath, projectId);
                    
                    break;
                case "FILE_OTech5":
                    filePath = Server.MapPath("~/Template/SCI/OTech/附件-05蒐集個人資料告知事項暨個人資料提供同意書.docx");
                    break;
                case "FILE_OTech6":
                    filePath = Server.MapPath("~/Template/SCI/OTech/附件-06申請人自我檢查表.docx");
                    break;
                case "FILE_OTech7":
                    filePath = Server.MapPath("~/Template/SCI/OTech/附件-07簽約注意事項.docx");
                    break;
                case "FILE_OTech8":
                    filePath = Server.MapPath("~/Template/SCI/OTech/附件-08海洋科技業界科專計畫補助契約書.docx");
                    break;
                case "FILE_OTech9":
                    filePath = Server.MapPath("~/Template/SCI/OTech/附件-09研究紀錄簿使用原則.docx");
                    break;
                case "FILE_OTech10":
                    filePath = Server.MapPath("~/Template/SCI/OTech/附件-10海洋科技專案計畫會計科目編列與執行原則.docx");
                    break;
                case "FILE_OTech11":
                    filePath = Server.MapPath("~/Template/SCI/OTech/附件-11計畫書書脊（側邊）格式.docx");
                    break;
                    
                // 學研範本檔案對應
                case "FILE_AC1":
                    filePath = Server.MapPath("~/Template/SCI/Academic/附件-01海洋委員會海洋科技專案補助作業要點.docx");
                    filePath = ApplyProjectDataToWord_FILE_AC1(filePath, projectId);
                    break;
                case "FILE_AC2":
                    filePath = Server.MapPath("~/Template/SCI/Academic/附件-02海洋科技科專案計畫書.zip");
                    break;
                case "FILE_AC3":
                    filePath = Server.MapPath("~/Template/SCI/Academic/附件-03建議迴避之審查委員清單.docx");
                    break;
                case "FILE_AC4":
                    filePath = Server.MapPath("~/Template/SCI/Academic/附件-04未違反公職人員利益衝突迴避法切結書.docx");
                    break;
                case "FILE_AC5":
                    filePath = Server.MapPath("~/Template/SCI/Academic/附件-05蒐集個人資料告知事項暨個人資料提供同意書.docx");
                    break;
                case "FILE_AC6":
                    filePath = Server.MapPath("~/Template/SCI/Academic/附件-06共同執行單位基本資料表.docx");
                    break;
                case "FILE_AC7":
                    filePath = Server.MapPath("~/Template/SCI/Academic/附件-07申請人自我檢查表.docx");
                    break;
                case "FILE_AC8":
                    filePath = Server.MapPath("~/Template/SCI/Academic/附件-08簽約注意事項.docx");
                    break;
                case "FILE_AC9":
                    filePath = Server.MapPath("~/Template/SCI/Academic/附件-09海洋委員會補助科技專案計畫契約書.docx");
                    break;
                case "FILE_AC10":
                    filePath = Server.MapPath("~/Template/SCI/Academic/附件-10海洋科技專案計畫會計科目編列與執行原則.docx");
                    break;
                case "FILE_AC11":
                    filePath = Server.MapPath("~/Template/SCI/Academic/附件-11海洋科技專案成效追蹤自評表.docx");
                    break;
                case "FILE_AC12":
                    filePath = Server.MapPath("~/Template/SCI/Academic/附件-12研究紀錄簿使用原則.docx");
                    break;
                case "FILE_AC13":
                    filePath = Server.MapPath("~/Template/SCI/Academic/附件-13計畫書書脊（側邊）格式.docx");
                    break;
                    
                default:
                    Response.Clear();
                    Response.Write($"ERROR:不支援的檔案類型：{fileCode}");
                    Response.End();
                    return;
            }
            
            // 調試資訊
            System.Diagnostics.Debug.WriteLine($"Download Template - FileCode: {fileCode}, FilePath: {filePath}");
            
            if (File.Exists(filePath))
            {
                string fileName = Path.GetFileName(filePath);
                string fileExt = Path.GetExtension(filePath).ToLower();

                // 設定正確的 Content-Type
                string contentType = "application/octet-stream";
                switch (fileExt)
                {
                    case ".docx":
                        contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        break;
                    case ".zip":
                        contentType = "application/zip";
                        break;
                    case ".pdf":
                        contentType = "application/pdf";
                        break;
                    case ".xlsx":
                        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        break;
                }

                // 處理中文檔名編碼問題
                string encodedFileName = System.Web.HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8);
                
                Response.Clear();
                Response.ContentType = contentType;
                Response.AddHeader("Content-Disposition", $"attachment; filename*=UTF-8''{encodedFileName}");
                Response.AddHeader("Content-Length", new FileInfo(filePath).Length.ToString());
                Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                Response.AddHeader("Pragma", "no-cache");
                Response.AddHeader("Expires", "0");
                
                // 讀取檔案並寫入 Response
                byte[] fileData = File.ReadAllBytes(filePath);
                Response.BinaryWrite(fileData);
                Response.Flush();
                Response.End();
                
                
            }
            else
            {
                Response.Clear();
                Response.Write($"ERROR:檔案不存在：{filePath}");
                Response.End();
            }
        }
        catch (Exception ex)
        {
            Response.Clear();
            Response.Write($"ERROR:下載失敗：{ex.Message}");
            Response.End();
        }
    }

    #region  海洋委員會海洋科技專案補助作業要點

    

  
    private string ApplyProjectDataToWord_FILE_OTech1(string originalFilePath, string projectId)
    {
        try
        {
            if (!File.Exists(originalFilePath))
            {
                return originalFilePath; // 如果原檔案不存在，返回原路徑
            }

            // 建立暫存檔案路徑，保持原檔名
            string originalFileName = Path.GetFileName(originalFilePath);
            string tempFilePath = Path.Combine(Path.GetTempPath(), originalFileName);
            
            // 複製範本檔案到暫存資料夾
            File.Copy(originalFilePath, tempFilePath, true);
            
            // 使用 OpenXmlHelper 處理 Word 文件
            using (var fs = new FileStream(tempFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                
                var helper = new OpenXmlHelper(fs);
                // 取得當前年月日 (參考 DownloadTemplateCUL 的實作)
                DateTime currentDate = DateTime.Now;
                int year = currentDate.Year - 1911;  // 民國年
                int month = currentDate.Month;
                    
                // 建立替換字典
                var placeholder = new Dictionary<string, string>();
                placeholder.Add("{{Year}}", year.ToString());
                placeholder.Add("{{Month}}", month.ToString());
    
                    
                var repeatData = new List<Dictionary<string, string>>();
                    
                // 使用 GenerateWord 方法替換佔位符
                helper.GenerateWord(placeholder, repeatData);
                helper.CloseAsSave();
                
            }
            // 回傳處理後的檔案路徑
            return tempFilePath;
        }
        catch (Exception ex)
        {
            // 如果處理失敗，記錄錯誤並返回原檔案路徑
            System.Diagnostics.Debug.WriteLine($"ApplyProjectDataToWord_FILE_AC1 Error: {ex.Message}");
            return originalFilePath;
        }
    }
    private string ApplyProjectDataToWord_FILE_AC1(string originalFilePath, string projectId)
    {
        try
        {
            if (!File.Exists(originalFilePath))
            {
                return originalFilePath; // 如果原檔案不存在，返回原路徑
            }

            // 建立暫存檔案路徑，保持原檔名
            string originalFileName = Path.GetFileName(originalFilePath);
            string tempFilePath = Path.Combine(Path.GetTempPath(), originalFileName);
            
            // 複製範本檔案到暫存資料夾
            File.Copy(originalFilePath, tempFilePath, true);
            
            // 使用 OpenXmlHelper 處理 Word 文件
            using (var fs = new FileStream(tempFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                
                    var helper = new OpenXmlHelper(fs);
                    // 取得當前年月日 (參考 DownloadTemplateCUL 的實作)
                    DateTime currentDate = DateTime.Now;
                    int year = currentDate.Year - 1911;  // 民國年
                    int month = currentDate.Month;
                    
                    // 建立替換字典
                    var placeholder = new Dictionary<string, string>();
                    placeholder.Add("{{Year}}", year.ToString());
                    placeholder.Add("{{Month}}", month.ToString());
                    var repeatData = new List<Dictionary<string, string>>();
                    
                    // 使用 GenerateWord 方法替換佔位符
                    helper.GenerateWord(placeholder, repeatData);
                    helper.CloseAsSave();
                
            }
            // 回傳處理後的檔案路徑
            return tempFilePath;
        }
        catch (Exception ex)
        {
            // 如果處理失敗，記錄錯誤並返回原檔案路徑
            System.Diagnostics.Debug.WriteLine($"ApplyProjectDataToWord_FILE_AC1 Error: {ex.Message}");
            return originalFilePath;
        }
    }
    #endregion

    #region 未違反公職人員利益衝突迴避法切結書

    private string ApplyProjectDataToWord_FILE_OTech4(string originalFilePath, string projectId)
    {
        try
        {
            // 除錯：記錄 ProjectID
            System.Diagnostics.Debug.WriteLine($"ApplyProjectDataToWord_FILE_OTech4 - ProjectID: {projectId}");
            
            if (!File.Exists(originalFilePath))
            {
                return originalFilePath; // 如果原檔案不存在，返回原路徑
            }

            // 建立暫存檔案路徑，保持原檔名
            string originalFileName = Path.GetFileName(originalFilePath);
            string tempFilePath = Path.Combine(Path.GetTempPath(), originalFileName);
            
            // 複製範本檔案到暫存資料夾
            File.Copy(originalFilePath, tempFilePath, true);
            
            // 使用 OpenXmlHelper 處理 Word 文件
            using (var fs = new FileStream(tempFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                var helper = new OpenXmlHelper(fs);
                
                // 取得當前年月日 (參考 DownloadTemplateCUL 的實作)
                DateTime currentDate = DateTime.Now;
                int year = currentDate.Year - 1911;  // 民國年
                int month = currentDate.Month;
                int day = currentDate.Day;
                
                // 從資料庫取得申請主檔資料
                var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectId);
                
                // 建立替換字典
                var placeholder = new Dictionary<string, string>();
                placeholder.Add("year", year.ToString());
                placeholder.Add("month", month.ToString());
                placeholder.Add("day", day.ToString());
                
                // 加入申請資料
                placeholder.Add("{{A3}}", applicationMain.ProjectNameTw ?? "");
                placeholder.Add("{{A9}}", applicationMain.OrgName ?? "");
        
                
                var repeatData = new List<Dictionary<string, string>>();
                    
                // 使用 GenerateWord 方法替換佔位符
                helper.GenerateWord(placeholder, repeatData);
                helper.CloseAsSave();
            }
            // 回傳處理後的檔案路徑
            return tempFilePath;
        }
        catch (Exception ex)
        {
            // 如果處理失敗，記錄錯誤並返回原檔案路徑
            System.Diagnostics.Debug.WriteLine($"ApplyProjectDataToWord_FILE_AC1 Error: {ex.Message}");
            return originalFilePath;
        }
        
    }

    #endregion 
    /// <summary>
    /// 處理已上傳檔案的下載
    /// </summary>
    private void DownloadUploadedFile(string projectId, string fileCode)
    {
        try
        {
            if (string.IsNullOrEmpty(fileCode) || string.IsNullOrEmpty(projectId))
            {
                Response.Clear();
                Response.Write("ERROR:缺少必要參數");
                Response.End();
                return;
            }

            // 從資料庫取得檔案資訊
            var uploadedFiles = OFS_SciUploadAttachmentsHelper.GetAttachmentsByFileCodeAndProject(projectId, fileCode);
            if (uploadedFiles == null || uploadedFiles.Count == 0)
            {
                Response.Clear();
                Response.Write("ERROR:找不到檔案記錄");
                Response.End();
                return;
            }

            var uploadedFile = uploadedFiles.First(); // 取第一個記錄

            // 建構檔案完整路徑
            string fullPath = Server.MapPath($"~/{uploadedFile.TemplatePath}");
            
            if (File.Exists(fullPath))
            {
                string fileName = uploadedFile.FileName;
                string fileExt = Path.GetExtension(fileName).ToLower();

                // 設定正確的 Content-Type
                string contentType = "application/octet-stream";
                switch (fileExt)
                {
                    case ".pdf":
                        contentType = "application/pdf";
                        break;
                    case ".docx":
                        contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        break;
                    case ".xlsx":
                        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        break;
                }

                // 處理中文檔名編碼問題
                string encodedFileName = System.Web.HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8);
                
                Response.Clear();
                Response.ContentType = contentType;
                Response.AddHeader("Content-Disposition", $"attachment; filename*=UTF-8''{encodedFileName}");
                Response.AddHeader("Content-Length", new FileInfo(fullPath).Length.ToString());
                Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                Response.AddHeader("Pragma", "no-cache");
                Response.AddHeader("Expires", "0");
                
                // 讀取檔案並寫入 Response
                byte[] fileData = File.ReadAllBytes(fullPath);
                Response.BinaryWrite(fileData);
                Response.Flush();
                Response.End();
            }
            else
            {
                Response.Clear();
                Response.Write($"ERROR:檔案不存在：{fullPath}");
                Response.End();
            }
        }
        catch (Exception ex)
        {
            Response.Clear();
            Response.Write($"ERROR:下載失敗：{ex.Message}");
            Response.End();
        }
    }

    /// <summary>
    /// 處理已上傳檔案的刪除
    /// </summary>
    private void DeleteUploadedFile(string projectId, string fileCode)
    {
        try
        {
            if (string.IsNullOrEmpty(fileCode) || string.IsNullOrEmpty(projectId))
            {
                Response.Clear();
                Response.Write("ERROR:缺少必要參數");
                Response.End();
                return;
            }

            // 刪除附件檔案（包含實體檔案和資料庫記錄）
            OFS_SciUploadAttachmentsHelper.DeleteAttachmentFile(projectId, fileCode);
            Response.Clear();
            Response.ContentType = "text/plain";  // 設定為純文字
            Response.Write("SUCCESS");
            Response.Flush();
            Response.SuppressContent = true;       // 阻止後續 HTML
            HttpContext.Current.ApplicationInstance.CompleteRequest(); // 取代 Response.End()
        }
        catch (Exception ex)
        {
            Response.Clear();
            Response.Write($"ERROR:刪除失敗：{ex.Message}");
            Response.End();
        }
    }

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
    /// 顯示 SweetAlert 成功訊息並跳轉
    /// </summary>
    private void ShowSweetAlertSuccess()
    {
        Response.Redirect("~/OFS/ApplicationChecklist.aspx", false);
    }
    
    /// <summary>
    /// 顯示 SweetAlert 錯誤訊息
    /// </summary>
    private void ShowSweetAlertError(string message)
    {
        string escapedMessage = message.Replace("'", "\\'").Replace("\"", "\\\"");
        string script = $"window.SciUploadAttachments.showErrorMessage('{escapedMessage}');";
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
            var projectData = OFS_SciApplicationHelper.getVersionByProjectID(CurrentProjectID);
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
    /// 檢查表單狀態並控制暫存按鈕顯示
    /// </summary>
    private void CheckFormStatusAndHideTempSaveButton()
    {
        try
        {
            if (!string.IsNullOrEmpty(CurrentProjectID))
            {
                var formStatus = OFS_SciWorkSchHelper.GetFormStatusByProjectID(CurrentProjectID, "Form5Status");
                
                if (formStatus == "完成")
                {
                    // 隱藏暫存按鈕
                    btnSave.Style["display"] = "none";
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
    /// 載入變更說明資料到輸入框
    /// </summary>
    private void LoadChangeDescriptionData()
    {
        try
        {
            if (!string.IsNullOrEmpty(CurrentProjectID))
            {
                // 從資料庫取得變更說明並設定到頁面元素
                var changeDescription = OFS_SciApplicationHelper.GetPageModifyNote(CurrentProjectID, "SciUploadAttachments");
                if (changeDescription != null)
                {
                    string script = $@"
                        setTimeout(function() {{
                            const changeBeforeElement = document.getElementById('txtChangeBefore');
                            if (changeBeforeElement && '{changeDescription.ChangeBefore?.Replace("'", "\\'")}') {{
                                changeBeforeElement.textContent = '{changeDescription.ChangeBefore?.Replace("'", "\\'")}';
                            }}
                            
                            const changeAfterElement = document.getElementById('txtChangeAfter');
                            if (changeAfterElement && '{changeDescription.ChangeAfter?.Replace("'", "\\'")}') {{
                                changeAfterElement.textContent = '{changeDescription.ChangeAfter?.Replace("'", "\\'")}';
                            }}
                        }}, 100);
                    ";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "LoadChangeDescription", script, true);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"載入變更說明資料時發生錯誤：{ex.Message}");
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
            ReviewCheckListHelper.SCI_UpdateProjectStatusName(CurrentProjectID, "計畫書審核中", GetCurrentUserInfo()?.Account ?? "系統");
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
                ProjectID = CurrentProjectID,
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
                System.Diagnostics.Debug.WriteLine($"計畫書修正提送歷程記錄已儲存：{CurrentProjectID}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"計畫書修正提送歷程記錄儲存失敗：{CurrentProjectID}");
            }
        }
        catch (Exception ex)
        {
            // 歷程記錄失敗不影響主要流程，只記錄錯誤
            System.Diagnostics.Debug.WriteLine($"記錄計畫書修正提送歷程失敗：{ex.Message}");
        }
    }

    #endregion
}