using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using GS.App;

public partial class OFS_SCI_SciReimbursement : System.Web.UI.Page
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
        // 檢查使用者權限，控制審查結果視窗顯示
        if (!CheckUserPermissionForReimbursementReview())
        {
            // 如果沒有權限，隱藏審查結果視窗
            ClientScript.RegisterStartupScript(this.GetType(), "hideReviewPanel", 
                "$(document).ready(function() { $('.scroll-bottom-panel').hide(); });", true);
        }
    }

    /// <summary>
    /// 檢查使用者是否有請款審核/撥款權限
    /// </summary>
    /// <param name="currentUser">使用者資訊，若為null則會自動取得當前使用者</param>
    /// <returns>是否有權限</returns>
    private static bool CheckUserPermissionForReimbursementReview(SessionHelper.UserInfoClass currentUser = null)
    {
        try
        {
            // 如果沒有傳入使用者資訊，則取得當前使用者資訊
            if (currentUser == null)
            {
                currentUser = GetCurrentUserInfo();
            }
            
            if (currentUser == null || currentUser.OFS_RoleName == null)
            {
                return false;
            }
            
            // 定義可以進行撥款審核的角色：查核人員、主管單位人員、主管單位窗口、系統管理者
            var authorizedRoles = new[] { "查核人員", "主管單位人員", "主管單位窗口", "系統管理者" };
            
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
            var uploadedFiles = OFS_SciReimbursementHelper.GetUploadedFiles(projectID);
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
            string orgCategory = OFS_SciReimbursementHelper.GetOrgCategoryByProjectID(projectID);
            if (string.IsNullOrEmpty(orgCategory))
            {
                Response.StatusCode = 404;
                Response.Write("無法找到專案的組織類型資訊");
                Response.Flush();
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                return;
            }

            if (orgCategory == "OceanTech")
            {
                fileName = "科專類_經費支用明細表_業者(範例)(請自行調整).xlsx";
            }
            else if (orgCategory == "Academic" || orgCategory == "Legal")
            {
                fileName = "科專類_經費支用明細表_學研(範本).xlsx";
            }
            else
            {
                Response.StatusCode = 400;
                Response.Write("無法識別的組織類型: " + orgCategory);
                Response.Flush();
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                return;
            }

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
            case ".xlsx":
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                break;
            case ".xls":
                contentType = "application/vnd.ms-excel";
                break;
            case ".pdf":
                contentType = "application/pdf";
                break;
        }
        
        Response.ContentType = contentType;
        Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
        Response.AddHeader("Content-Length", new FileInfo(physicalPath).Length.ToString());
        Response.TransmitFile(physicalPath);
        Response.Flush();
        Response.End();
    }

    [WebMethod]
    public static object GetPhaseData(string projectID, int phaseOrder)
    {
        try
        {
            var result = OFS_SciReimbursementHelper.GetPhaseData(projectID, phaseOrder);
            return new { Success = true, Data = result };
        }
        catch (Exception ex)
        {
            return new { Success = false, Message = "系統錯誤: " + ex.Message };
        }
    }

    [WebMethod]
    public static object SubmitReimbursement(string projectID, int phaseOrder, decimal currentRequestAmount, decimal accumulatedAmount = 0, bool isDraft = false)
    {
        try
        {
            if (!isDraft)
            {
                // 只有提送時才更新專案狀態為審核中
                OFS_SciReimbursementHelper.SubmitReimbursement(projectID);
            }
            
            // 處理OFS_SCI_Payment資料
            string status = isDraft ? "請款中" : "審核中";
            OFS_SciReimbursementHelper.ProcessPaymentData(projectID, phaseOrder, currentRequestAmount, accumulatedAmount, status);
            
            
            // 寄信通知審核
            if (!isDraft)
            {
                var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);
                var projectMain = OFS_SciApplicationHelper.getVersionByProjectID(projectID);

                if (applicationMain != null && projectMain != null)
                {
                    string projectName = applicationMain.ProjectNameTw;
                    string supervisoryAccount = projectMain.SupervisoryPersonAccount;

                    // 根據承辦人帳號取得 UserID
                    int? organizer = SysUserHelper.GetUserIDByAccount(supervisoryAccount);

                    // 寄送通知信
                    NotificationHelper.G2("科專", projectName, $"第{phaseOrder}期請款", organizer);
                }
            }
            
            string message = isDraft ? "請款資料暫存成功" : "請款提送成功";
            return new { Success = true, Message = message };
        }
        catch (Exception ex)
        {
            return new { Success = false, Message = "系統錯誤: " + ex.Message };
        }
    }
    [WebMethod]
    public static object ReviewPayment(
        string projectID,
        string phaseOrder,
        decimal currentPayment,
        string reviewResult,
        string reviewComment)
    {
        try
        {
            // 取得當前使用者資訊
            var currentUser = GetCurrentUserInfo();
            if (currentUser == null)
            {
                return new { Success = false, Message = "無法取得使用者資訊" };
            }
            
            // 檢查使用者是否有撥款審核權限
            if (!CheckUserPermissionForReimbursementReview(currentUser))
            {
                return new { Success = false, Message = "您沒有撥款審核權限" };
            }

            string Status = reviewResult == "pass" ? "通過" : "請款中";
            currentPayment = reviewResult == "pass" ?  currentPayment: 0;
            OFS_SciReimbursementHelper.UpdatePayment(projectID,phaseOrder, currentPayment,Status,  reviewComment,currentUser.Account);
            if (reviewResult == "pass")
            {
                InprogressListHelper.UpdateLastOperation(projectID, $"已完成第{phaseOrder}期請款");
                InprogressListHelper.UpdateTaskCompleted(projectID, $"Payment{phaseOrder}", true);

                // 如果是第二期請款通過，更新專案狀態為「已結案」
                if (phaseOrder == "2")
                {
                    OFS_SciReimbursementHelper.UpdateProjectStatusToClosed(projectID);
                }
            }
            
            //根據是否通過寄信
            var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);
            var projectMain = OFS_SciApplicationHelper.getVersionByProjectID(projectID);

            if (applicationMain != null && projectMain != null)
            {
                string projectName = applicationMain.ProjectNameTw;
                string UserAccount = projectMain.UserAccount;
                if (reviewResult == "pass")
                {
                    // 寄送通知信
                    NotificationHelper.G6("科專", projectName, $"第{phaseOrder}期請款", currentPayment, reviewComment, UserAccount);
                }
                else
                {
                    NotificationHelper.G3("科專", projectName, $"第{phaseOrder}期請款", reviewComment, UserAccount);
                }
            
            }
            
            return new { Success = true, Message = "審查結果已成功儲存" };
        }
        catch (Exception ex)
        {
            return new { Success = false, Message = "系統錯誤: " + ex.Message };
        }
    }

    [WebMethod]
    public static object GetUploadedFiles(string projectID)
    {
        try
        {
            var files = OFS_SciReimbursementHelper.GetUploadedFiles(projectID);
            return new { Success = true, Files = files };
        }
        catch (Exception ex)
        {
            return new { Success = false, Message = "系統錯誤: " + ex.Message };
        }
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
            string fileType = Request.Form["fileType"]; // 1=經費支用表, 2=憑證
            
            if (string.IsNullOrEmpty(projectID))
            {
                Response.Write("專案ID不可為空");
                Response.End();
                return;
            }

            HttpFileCollection files = Request.Files;
            if (files.Count == 0 || files[0].ContentLength == 0)
            {
                Response.Write("請選擇要上傳的檔案");
                Response.End();
                return;
            }

            HttpPostedFile uploadedFile = files[0];
            string fileCode = fileType == "1" ? "REIMBURSE_EXPENSE" : "REIMBURSE_RECEIPT";
            
            // 產生檔案路徑
            string relativePath = OFS_SciReimbursementHelper.GenerateFilePath(projectID, uploadedFile.FileName, fileCode);
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
            bool success = OFS_SciReimbursementHelper.SaveUploadedFile(projectID, fileCode, fileName, relativePath);
            
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

}