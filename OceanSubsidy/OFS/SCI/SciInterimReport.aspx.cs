using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using GS.App;
using GS.OCA_OceanSubsidy.Operation.OFS;
using GS.OCA_OceanSubsidy.Model.OFS.SCI;

public partial class OFS_SCI_SciInterimReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string action = Request.QueryString["action"];
        if (action == "download")
        {
            HandleFileDownload();
            return;
        }
        else if (action == "upload")
        {
            HandleFileUpload(null, null);
            return;
        }

        if (!IsPostBack)
        {
            InitializePage();
        }
    }

    private void InitializePage()
    {
        // 初始化頁面邏輯
    }

    /// <summary>
    /// 處理檔案下載
    /// </summary>
    private void HandleFileDownload()
    {
        string projectID = Request.QueryString["projectID"];
        string fileCode = Request.QueryString["fileCode"]; // 如果有fileCode代表要下載已上傳的檔案
        
        if (string.IsNullOrEmpty(projectID))
        {
            Response.StatusCode = 400;
            Response.Write("專案ID不可為空");
            Response.Flush();
            HttpContext.Current.ApplicationInstance.CompleteRequest();
            return;
        }

        string fileName;
        string physicalPath;

        if (!string.IsNullOrEmpty(fileCode))
        {
            // 下載已上傳的檔案
            var uploadedFiles = OFS_SciInterimReportHelper.GetUploadedFiles(projectID,fileCode);
            var targetFile = uploadedFiles.FirstOrDefault(f => f.FileCode == fileCode);
            
            if (targetFile == null)
            {
                Response.StatusCode = 404;
                Response.Write("找不到指定的檔案");
                Response.Flush();
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                return;
            }
            
            fileName = targetFile.FileName;
            physicalPath = Server.MapPath("~/" + targetFile.FilePath);
        }
        else
        {
            // 下載範本檔案
            fileName = "科專類_報告書_說明及範本.doc";
            physicalPath = Server.MapPath("~/Template/SCI/" + fileName);
        }

        if (!System.IO.File.Exists(physicalPath))
        {
            Response.StatusCode = 404;
            Response.Write("檔案不存在: " + fileName);
            Response.Flush();
            HttpContext.Current.ApplicationInstance.CompleteRequest();
            return;
        }

        // 下載檔案
        Response.Clear();
        Response.ClearContent();
        Response.ClearHeaders();
        Response.Buffer = true;
        
        string contentType = "application/octet-stream";
        string extension = Path.GetExtension(fileName).ToLower();
        switch (extension)
        {
            case ".doc":
                contentType = "application/msword";
                break;
            case ".docx":
                contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                break;
            case ".pdf":
                contentType = "application/pdf";
                break;
            case ".zip":
                contentType = "application/zip";
                break;
        }
        
        Response.ContentType = contentType;
        Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
        Response.AddHeader("Content-Length", new FileInfo(physicalPath).Length.ToString());
        Response.TransmitFile(physicalPath);
        Response.Flush();
        Response.End();
    }

    /// <summary>
    /// 處理檔案上傳
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void HandleFileUpload(object sender, EventArgs e)
    {
        try
        {
            string projectID = Request.QueryString["ProjectID"];
            string fileType = Request.Form["fileType"]; // 1=初版, 2=修正版
            string reportType = Request.Form["reportType"]; // 1=期中報告, 2=期末報告
            string customName = Request.Form["customName"]; // 修訂版的自訂名稱
            
            if (string.IsNullOrEmpty(projectID))
            {
                Response.Write("ERROR:專案ID不可為空");
                Response.End();
                return;
            }

            HttpFileCollection files = Request.Files;
            if (files.Count == 0 || files[0].ContentLength == 0)
            {
                Response.Write("ERROR:請選擇要上傳的檔案");
                Response.End();
                return;
            }

            HttpPostedFile uploadedFile = files[0];
            
            // 驗證檔案類型
            string fileExt = Path.GetExtension(uploadedFile.FileName).ToLower();
            if (fileExt != ".zip")
            {
                Response.Write("ERROR:請上傳 ZIP 格式的檔案");
                Response.End();
                return;
            }
            
            // 驗證檔案大小 (100MB)
            const int maxSize = 100 * 1024 * 1024;
            if (uploadedFile.ContentLength > maxSize)
            {
                Response.Write("ERROR:檔案大小不可超過 100MB");
                Response.End();
                return;
            }
            
            string fileCode;
            if (reportType == "1") // 期中報告
            {
                fileCode = fileType == "1" ? "MidExamFile" : "MidExamFile_revise";
            }
            else // 期末報告
            {
                fileCode = fileType == "1" ? "FinalExamFile" : "FinalExamFile_revise";
            }
            
            // 產生檔案路徑
            string relativePath = OFS_SciInterimReportHelper.GenerateFilePath(projectID, uploadedFile.FileName, fileCode, customName);
            string physicalPath = Server.MapPath("~/" + relativePath);
            
            // 確保目錄存在
            string directory = Path.GetDirectoryName(physicalPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            // 儲存檔案
            uploadedFile.SaveAs(physicalPath);
            
            // 儲存檔案記錄到資料庫
            string fileName = Path.GetFileName(relativePath);
            bool success = OFS_SciInterimReportHelper.SaveUploadedFile(projectID, fileCode, fileName, relativePath);
            
            if (success)
            {
                Response.Write("SUCCESS:" + fileName);
            }
            else
            {
                Response.Write("ERROR:儲存檔案記錄失敗");
            }
            
        }
        catch (Exception ex)
        {
            Response.Write("ERROR:" + ex.Message);
        }
        finally
        {
            Response.End();
        }
    }
    [System.Web.Services.WebMethod]
    public static object GetUploadedFiles(string projectID, int reportType = 1)
    {
        try
        {
            
            var files = OFS_SciInterimReportHelper.GetUploadedFilesList(projectID, reportType);
            return new { Success = true, Files = files };
        }
        catch (Exception ex)
        {
            return new { Success = false, Message = "系統錯誤: " + ex.Message };
        }
    }

    [System.Web.Services.WebMethod]
    public static object SubmitReport(string projectID, int stage)
    {
        try
        {
            if (string.IsNullOrEmpty(projectID))
            {
                return new { Success = false, Message = "專案ID不可為空" };
            }

            // 檢查是否有報告審核中
            if (OFS_SciInterimReportHelper.HasReportInReview(projectID))
            {
                return new { Success = false, Message = "目前有報告正在審核中，無法重複提送" };
            }

            string status = "審核中";
            string stageName = stage == 1 ? "期中報告" : "期末報告";
            string TaskNameEn = stage == 1 ? "MidReport" : "FinalReport";

            // 呼叫 Helper 方法處理資料庫操作
            OFS_SciInterimReportHelper.SubmitStageExam(projectID, stage, status);
            InprogressListHelper.UpdateTaskCompleted(projectID, TaskNameEn, true);
           
            // 寄信通知審核
            var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);
            var projectMain = OFS_SciApplicationHelper.getVersionByProjectID(projectID);

            if (applicationMain != null && projectMain != null)
            {
                string projectName = applicationMain.ProjectNameTw;
                string supervisoryAccount = projectMain.SupervisoryPersonAccount;

                // 根據承辦人帳號取得 UserID
                int? organizer = SysUserHelper.GetUserIDByAccount(supervisoryAccount);

                // 寄送通知信
                NotificationHelper.G2("科專", projectName, $"{stageName}", organizer);
            }
            return new { Success = true, Message = $"{stageName}提送成功" };
        }
        catch (Exception ex)
        {
            return new { Success = false, Message = "系統錯誤: " + ex.Message };
        }
    }


    /// <summary>
    /// 檢查使用者是否有審核權限
    /// </summary>
    /// <returns></returns>
    [System.Web.Services.WebMethod]
    public static object CheckReviewPermission()
    {
        try
        {
            var currentUser = GetCurrentUserInfo();
            bool hasPermission = CheckUserPermissionForReview(currentUser);
            
            return new { 
                Success = true, 
                HasPermission = hasPermission,
                UserName = currentUser?.UserName ?? "",
                Account = currentUser?.Account ?? ""
            };
        }
        catch (Exception ex)
        {
            return new { Success = false, Message = "系統錯誤: " + ex.Message };
        }
    }

    /// <summary>
    /// 取得階段審查狀態
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <param name="stage">階段</param>
    /// <returns></returns>
    [System.Web.Services.WebMethod]
    public static object GetStageExamStatus(string projectID, int stage)
    {
        try
        {
            var status = OFS_SciInterimReportHelper.GetStageExamStatus(projectID, stage);
            return new { Success = true, Data = status };
        }
        catch (Exception ex)
        {
            return new { Success = false, Message = "系統錯誤: " + ex.Message };
        }
    }

    /// <summary>
    /// 審核階段報告
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <param name="stage">階段</param>
    /// <param name="reviewMethod">審查方式</param>
    /// <param name="reviewResult">審查結果</param>
    /// <param name="reviewComment">審查意見</param>
    /// <returns></returns>
    [System.Web.Services.WebMethod]
    public static object ReviewStageExam(string projectID, int stage, string reviewMethod, string reviewResult, string reviewComment)
    {
        try
        {
            var currentUser = GetCurrentUserInfo();
            if (currentUser == null)
            {
                return new { Success = false, Message = "無法取得使用者資訊" };
            }
            
            // 檢查使用者權限
            if (!CheckUserPermissionForReview(currentUser))
            {
                return new { Success = false, Message = "您沒有審核權限" };
            }

            string status = reviewResult == "pass" ? "通過" : "不通過";
            string stageName = stage == 1 ? "期中" : "期末";
            string TaskNameEn = stage == 1 ? "MidReport" : "FinalReport";

            // 呼叫 Helper 方法處理審核
            OFS_SciInterimReportHelper.ReviewStageExam(projectID, stage, reviewMethod, status, reviewComment, currentUser.UserName, currentUser.Account);

            if (reviewResult == "pass")
            {
                InprogressListHelper.UpdateLastOperation(projectID, $"已通過{stageName}審查");
                if (stage == 2)
                {
                    // 開啟第二次請款待辦事項檢查
                    string UserAccount = OFS_SciApplicationHelper.GetApplicantAccountByProjectId(projectID);
                    var application = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);
                    InprogressListHelper.UpdateTaskTodo(projectID, "Payment2", true);
                    NotificationHelper.F12("科專", application.ProjectNameTw, "第二期請款", UserAccount);

                }
            }
            else
            {
                InprogressListHelper.UpdateTaskCompleted(projectID, TaskNameEn, false);
            }
            
            // 寄信通知結果
            var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);
            var projectMain = OFS_SciApplicationHelper.getVersionByProjectID(projectID);
            if (applicationMain != null && projectMain != null)
            {
                string projectName = applicationMain.ProjectNameTw;
                string UserAccount = projectMain.UserAccount;
                if (reviewResult == "pass")
                {

                    NotificationHelper.G5("科專", projectName, $"{stageName}報告階段", UserAccount);
                }
                else
                {
                    NotificationHelper.G3("科專", projectName, $"{stageName}報告階段", reviewComment,UserAccount);

                }
            }

            

            
            return new { Success = true, Message = $"{stageName}審核完成" };
        }
        catch (Exception ex)
        {
            return new { Success = false, Message = "系統錯誤: " + ex.Message };
        }
    }

    /// <summary>
    /// 檢查使用者是否有審核權限
    /// </summary>
    /// <param name="currentUser">使用者資訊</param>
    /// <returns></returns>
    private static bool CheckUserPermissionForReview(SessionHelper.UserInfoClass currentUser = null)
    {
        try
        {
            if (currentUser == null)
            {
                currentUser = GetCurrentUserInfo();
            }
            
            if (currentUser == null || currentUser.OFS_RoleName == null)
            {
                return false;
            }
            
            // 定義可以進行審核的角色
            var authorizedRoles = new[] { "主管單位人員", "主管單位窗口", "系統管理者" };
            
            foreach (string roleName in currentUser.OFS_RoleName)
            {
                if (!string.IsNullOrEmpty(roleName) && authorizedRoles.Contains(roleName))
                {
                    return true;
                }
            }
            
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"檢查使用者權限時發生錯誤: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 提送審查委員
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <param name="stage">階段</param>
    /// <param name="reviewers">審查委員清單</param>
    /// <returns></returns>
    [System.Web.Services.WebMethod]
    public static object SubmitReviewers(string projectID, int stage, List<ReviewerData> reviewers)
    {
        try
        {
            var currentUser = GetCurrentUserInfo();
            if (currentUser == null)
            {
                return new { Success = false, Message = "無法取得使用者資訊" };
            }
            
            // 檢查使用者權限
            if (!CheckUserPermissionForReview(currentUser))
            {
                return new { Success = false, Message = "您沒有提送審查委員的權限" };
            }

            if (string.IsNullOrEmpty(projectID))
            {
                return new { Success = false, Message = "專案ID不可為空" };
            }

            if (reviewers == null || reviewers.Count == 0)
            {
                return new { Success = false, Message = "請至少輸入一位審查委員" };
            }

            string stageName = stage == 1 ? "期中報告" : "期末報告";
            
            // 呼叫 Helper 方法處理審查委員資料
            OFS_SciInterimReportHelper.SubmitReviewers(projectID, stage, reviewers);
            
            return new { Success = true, Message = $"{stageName}審查委員提送成功，共 {reviewers.Count} 位" };
        }
        catch (Exception ex)
        {
            return new { Success = false, Message = "系統錯誤: " + ex.Message };
        }
    }

    /// <summary>
    /// 取得目前登入使用者資訊
    /// </summary>
    private static SessionHelper.UserInfoClass GetCurrentUserInfo()
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
    /// 取得審查意見檔案清單
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <param name="stage">階段</param>
    /// <param name="fileType">檔案類型 (1=初版, 2=修正版)</param>
    /// <returns></returns>
    [System.Web.Services.WebMethod]
    public static object GetReviewFiles(string projectID, int stage, int fileType)
    {
        try
        {
            if (string.IsNullOrEmpty(projectID))
            {
                return new { Success = false, Message = "專案ID不可為空" };
            }

            // 檢查使用者權限，決定是否顯示審查委員姓名
            bool includeReviewer = CanViewReviewerName();

            var files = OFS_SciInterimReportHelper.GetReviewFiles(projectID, stage, fileType, includeReviewer);
            return new { Success = true, Files = files };
        }
        catch (Exception ex)
        {
            return new { Success = false, Message = "系統錯誤: " + ex.Message };
        }
    }

    /// <summary>
    /// 檢查使用者是否有權限查看審查委員姓名
    /// </summary>
    /// <returns></returns>
    private static bool CanViewReviewerName()
    {
        try
        {
            var currentUser = GetCurrentUserInfo();

            if (currentUser == null || currentUser.OFS_RoleName == null)
            {
                return false;
            }

            // 定義可以查看審查委員姓名的角色
            var authorizedRoles = new[] { "主管單位人員", "主管單位窗口", "系統管理者" };

            foreach (string roleName in currentUser.OFS_RoleName)
            {
                if (!string.IsNullOrEmpty(roleName) && authorizedRoles.Contains(roleName))
                {
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"檢查審查委員姓名查看權限時發生錯誤: {ex.Message}");
            return false;
        }
    }
}