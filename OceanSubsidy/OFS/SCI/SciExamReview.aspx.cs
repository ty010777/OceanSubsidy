using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.IO;
using System.Web;
using GS.OCA_OceanSubsidy.Operation.OFS;

/// <summary>
/// 領域審查頁面
/// </summary>
public partial class OFS_SCI_SciExamReview : System.Web.UI.Page
{
    private string ProjectID => Request["ProjectID"];
    private string Token => Request["Token"] ?? Request.QueryString["token"];

    protected void Page_Load(object sender, EventArgs e)
    {
        // 新增：處理檔案下載請求
        string action = Request.QueryString["action"];
        if (action == "download")
        {
            HandleFileDownload();
            return;
        }
        else if (action == "downloadReview")
        {
            HandleReviewFileDownload();
            return;
        }
        else if (action == "uploadReview")
        {
            HandleReviewFileUpload();
            return;
        }

        if (!IsPostBack)
        {
            // 檢查Token是否已提交，如已提交則重導向
            if (!string.IsNullOrEmpty(Token) && OFS_SciExamReviewHelper.IsReviewSubmitted(Token))
            {
                string script = @"
                    Swal.fire({
                        title: '此報告已審核',
                        text: '您已完成此報告的審核，無法再次進入。',
                        icon: 'info',
                        confirmButtonText: '確定'
                    }).then((result) => {
                        window.location.href = '../ReviewChecklist.aspx';
                    });
                ";
                ClientScript.RegisterStartupScript(this.GetType(), "AlreadySubmitted", script, true);
                return;
            }
            
            // 根據Request["ProjectID"] 得到ProjectID 並進行搜尋和初始化
            if (!string.IsNullOrEmpty(ProjectID))
            {
                LoadProjectData(ProjectID);
            }
            
            // 新增：如果有Token，則使用Token進行初始化
            if (!string.IsNullOrEmpty(Token))
            {
                LoadExamDataByToken(Token);
            }
        }
    }

    private void LoadProjectData(string projectID)
    {
        // 使用 Helper 取得計畫資料
        DataRow projectData = OFS_SciDomainReviewHelper.GetProjectData(projectID);
        
        if (projectData != null)
        {
            lblProjectNumber.Text = projectData["ProjectID"].ToString();
            lblYear.Text = projectData["Year"].ToString();
            lblProjectCategory.Text = "科專"; // 寫死為科專
            lblReviewGroup.Text = projectData["Field"].ToString();
            lblProjectName.Text = projectData["ProjectName"].ToString();
            lblApplicantUnit.Text = projectData["OrgName"].ToString();
        }
    }

    /// <summary>
    /// 新增：根據Token載入審查資料
    /// </summary>
    /// <param name="token">審查Token</param>
    private void LoadExamDataByToken(string token)
    {
        try
        {
            // 根據Token取得審查資料
            DataRow examData = OFS_SciExamReviewHelper.GetExamDataByToken(token);
            
            if (examData != null)
            {
                // 填入計畫基本資料
                lblProjectNumber.Text = examData["ProjectID"].ToString();
                lblYear.Text = examData["Year"].ToString();
                lblProjectCategory.Text = "科專";
                lblReviewGroup.Text = examData["Field"].ToString();
                lblProjectName.Text = examData["ProjectName"].ToString();
                lblApplicantUnit.Text = examData["OrgName"].ToString();
                
                // 儲存到ViewState供後續使用
                ViewState["ProjectID"] = examData["ProjectID"].ToString();
                ViewState["Stage"] = Convert.ToInt32(examData["Stage"]);
                ViewState["Token"] = token;
            }
            else
            {
                ShowMessage("找不到對應的審查資料，請確認Token是否正確", false);
            }
        }
        catch (Exception ex)
        {
            ShowMessage("載入審查資料時發生錯誤: " + ex.Message, false);
        }
    }

    protected void btnDownloadDocument_Click(object sender, EventArgs e)
    {
        // 保留原本的按鈕事件
    }

    /// <summary>
    /// 新增：處理檔案下載
    /// </summary>
    private void HandleFileDownload()
    {
        string projectID = Request.QueryString["projectID"];
        string fileCode = Request.QueryString["fileCode"];
        
        if (string.IsNullOrEmpty(projectID) || string.IsNullOrEmpty(fileCode))
        {
            Response.StatusCode = 400;
            Response.Write("參數不完整");
            Response.Flush();
            HttpContext.Current.ApplicationInstance.CompleteRequest();
            return;
        }

        try
        {
            string relativePath = OFS_SciExamReviewHelper.GetFilePhysicalPath(projectID, fileCode);
            
            if (string.IsNullOrEmpty(relativePath))
            {
                Response.StatusCode = 404;
                Response.Write("找不到指定的檔案");
                Response.Flush();
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                return;
            }
            
            string physicalPath = Server.MapPath("~/" + relativePath);
            
            if (!File.Exists(physicalPath))
            {
                Response.StatusCode = 404;
                Response.Write("檔案不存在於伺服器上");
                Response.Flush();
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                return;
            }

            string fileName = Path.GetFileName(physicalPath);
            string contentType = "application/octet-stream";
            string extension = Path.GetExtension(fileName).ToLower();
            
            switch (extension)
            {
                case ".zip":
                    contentType = "application/zip";
                    break;
                case ".pdf":
                    contentType = "application/pdf";
                    break;
                case ".doc":
                    contentType = "application/msword";
                    break;
                case ".docx":
                    contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
            }
            
            // 下載檔案
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Buffer = true;
            Response.ContentType = contentType;
            Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
            Response.AddHeader("Content-Length", new FileInfo(physicalPath).Length.ToString());
            Response.TransmitFile(physicalPath);
            Response.Flush();
            Response.End();
        }
        catch (Exception ex)
        
{
            Response.StatusCode = 500;
            Response.Write("下載檔案時發生錯誤: " + ex.Message);
            Response.Flush();
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }

    /// <summary>
    /// 新增：處理審查檔案下載
    /// </summary>
    private void HandleReviewFileDownload()
    {
        string token = Request.QueryString["token"];
        
        if (string.IsNullOrEmpty(token))
        {
            Response.StatusCode = 400;
            Response.Write("Token不可為空");
            Response.Flush();
            HttpContext.Current.ApplicationInstance.CompleteRequest();
            return;
        }

        try
        {
            string filePath = OFS_SciExamReviewHelper.GetReviewFilePath(token);
            
            if (string.IsNullOrEmpty(filePath))
            {
                Response.StatusCode = 404;
                Response.Write("找不到審查檔案");
                Response.Flush();
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                return;
            }
            
            string physicalPath = Server.MapPath("~/" + filePath);
            
            if (!File.Exists(physicalPath))
            {
                Response.StatusCode = 404;
                Response.Write("審查檔案不存在於伺服器上");
                Response.Flush();
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                return;
            }

            string fileName = Path.GetFileName(physicalPath);
            
            // 下載檔案
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
            Response.AddHeader("Content-Length", new FileInfo(physicalPath).Length.ToString());
            Response.TransmitFile(physicalPath);
            Response.Flush();
            Response.End();
        }
        catch (Exception ex)
        {
            Response.StatusCode = 500;
            Response.Write("下載審查檔案時發生錯誤: " + ex.Message);
            Response.Flush();
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }

    /// <summary>
    /// 新增：取得審查檔案清單
    /// </summary>
    /// <param name="token">Token</param>
    /// <returns></returns>
    [WebMethod]
    public static object GetExamData(string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                return new { Success = false, Message = "Token不可為空" };
            }

            DataRow examData = OFS_SciExamReviewHelper.GetExamDataByToken(token);
            
            if (examData == null)
            {
                return new { Success = false, Message = "找不到對應的審查資料" };
            }

            string projectID = examData["ProjectID"].ToString();
            int stage = Convert.ToInt32(examData["Stage"]);
            
            var files = OFS_SciExamReviewHelper.GetReportFiles(projectID, stage);
            
            return new
            {
                Success = true,
                Data = new
                {
                    ProjectID = projectID,
                    Stage = stage,
                    StageName = stage == 1 ? "期中報告" : "期末報告",
                    ProjectName = examData["ProjectName"].ToString(),
                    Year = examData["Year"].ToString(),
                    Field = examData["Field"].ToString(),
                    OrgName = examData["OrgName"].ToString(),
                    Reviewer = examData["Reviewer"].ToString(),
                    Account = examData["Account"]?.ToString() ?? "",
                    BankCode = examData["BankCode"]?.ToString() ?? "",
                    BankAccount = examData["BankAccount"]?.ToString() ?? "",
                    RegistrationAddress = examData["RegistrationAddress"]?.ToString() ?? "",
                    Files = files
                }
            };
        }
        catch (Exception ex)
        {
            return new { Success = false, Message = "系統錯誤: " + ex.Message };
        }
    }

    /// <summary>
    /// 新增：取得報告檔案清單
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <param name="stage">階段</param>
    /// <returns></returns>
    [WebMethod]
    public static object GetReportFiles(string projectID, int stage)
    {
        try
        {
            var files = OFS_SciExamReviewHelper.GetReportFiles(projectID, stage);
            return new { Success = true, Files = files };
        }
        catch (Exception ex)
        {
            return new { Success = false, Message = "系統錯誤: " + ex.Message };
        }
    }

    /// <summary>
    /// 新增：處理審查檔案上傳
    /// </summary>
    private void HandleReviewFileUpload()
    {
        try
        {
            string token = Request.Form["token"];
            
            if (string.IsNullOrEmpty(token))
            {
                Response.Write("{\"Success\": false, \"Message\": \"Token不可為空\"}");
                Response.End();
                return;
            }

            HttpFileCollection files = Request.Files;
            if (files.Count == 0 || files[0].ContentLength == 0)
            {
                Response.Write("{\"Success\": false, \"Message\": \"請選擇要上傳的檔案\"}");
                Response.End();
                return;
            }

            HttpPostedFile uploadedFile = files[0];
            
            // 驗證檔案類型
            string fileExt = Path.GetExtension(uploadedFile.FileName).ToLower();
            if (fileExt != ".pdf")
            {
                Response.Write("{\"Success\": false, \"Message\": \"請上傳 PDF 格式的檔案\"}");
                Response.End();
                return;
            }
            
            // 驗證檔案大小 (10MB)
            const int maxSize = 10 * 1024 * 1024;
            if (uploadedFile.ContentLength > maxSize)
            {
                Response.Write("{\"Success\": false, \"Message\": \"檔案大小不可超過 10MB\"}");
                Response.End();
                return;
            }
            
            // 使用Helper儲存檔案
            string result = OFS_SciExamReviewHelper.SaveReviewFile(token, uploadedFile);
            
            Response.Write($"{{\"Success\": true, \"Message\": \"檔案上傳成功\", \"FileName\": \"{result}\"}}");
            
        }
        catch (Exception ex)
        {
            Response.Write($"{{\"Success\": false, \"Message\": \"上傳檔案時發生錯誤: {ex.Message}\"}}");
        }
        finally
        {
            Response.End();
        }
    }

    /// <summary>
    /// 新增：上傳審查檔案 WebMethod
    /// </summary>
    /// <param name="token">Token</param>
    /// <returns></returns>
    [WebMethod]
    public static object UploadReviewFile(string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                return new { Success = false, Message = "Token不可為空" };
            }

            // 這個方法主要是為了支援AJAX調用，實際處理由HandleReviewFileUpload完成
            return new { Success = true, Message = "準備上傳" };
        }
        catch (Exception ex)
        {
            return new { Success = false, Message = "系統錯誤: " + ex.Message };
        }
    }

    /// <summary>
    /// 新增：檢查是否已有上傳的審查檔案
    /// </summary>
    /// <param name="token">Token</param>
    /// <returns></returns>
    [WebMethod]
    public static object CheckExistingReviewFile(string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                return new { Success = false, Message = "Token不可為空" };
            }

            string filePath = OFS_SciExamReviewHelper.GetReviewFilePath(token);
            bool isSubmitted = OFS_SciExamReviewHelper.IsReviewSubmitted(token);
            
            if (!string.IsNullOrEmpty(filePath))
            {
                string fileName = System.IO.Path.GetFileName(filePath);
                return new { 
                    Success = true, 
                    HasFile = true, 
                    FileName = fileName,
                    IsSubmitted = isSubmitted
                };
            }
            
            return new { Success = true, HasFile = false, IsSubmitted = isSubmitted };
        }
        catch (Exception ex)
        {
            return new { Success = false, Message = "系統錯誤: " + ex.Message };
        }
    }

    /// <summary>
    /// 新增：提交審查結果（包含儲存銀行資訊）
    /// </summary>
    /// <param name="token">Token</param>
    /// <param name="bankCode">銀行代碼</param>
    /// <param name="bankAccount">銀行帳號</param>
    /// <param name="registrationAddress">戶籍地址</param>
    /// <returns></returns>
    [WebMethod]
    public static object SubmitReviewResult(string token, string bankCode, string bankAccount, string registrationAddress)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                return new { Success = false, Message = "Token不可為空" };
            }

            // 先儲存銀行資訊
            OFS_SciExamReviewHelper.UpdateReviewerBankInfo(token, bankCode, bankAccount, registrationAddress);

            // 再提交審查結果
            OFS_SciExamReviewHelper.SubmitReviewResult(token);

            return new { Success = true, Message = "審查結果提交成功" };

        }
        catch (Exception ex)
        {
            return new { Success = false, Message = "系統錯誤: " + ex.Message };
        }
    }

    /// <summary>
    /// 取得銀行代碼清單
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public static object GetBankCodeList()
    {
        try
        {
            var bankCodes = OFS_SciExamReviewHelper.GetBankCodeList();
            return new { Success = true, Data = bankCodes };
        }
        catch (Exception ex)
        {
            return new { Success = false, Message = "系統錯誤: " + ex.Message };
        }
    }

    /// <summary>
    /// 儲存審查委員銀行資訊
    /// </summary>
    /// <param name="token">Token</param>
    /// <param name="bankCode">銀行代碼</param>
    /// <param name="bankAccount">銀行帳號</param>
    /// <param name="registrationAddress">戶籍地址</param>
    /// <returns></returns>
    [WebMethod]
    public static object SaveReviewerBankInfo(string token, string bankCode, string bankAccount, string registrationAddress)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                return new { Success = false, Message = "Token不可為空" };
            }

            OFS_SciExamReviewHelper.UpdateReviewerBankInfo(token, bankCode, bankAccount, registrationAddress);

            return new { Success = true, Message = "銀行資訊儲存成功" };
        }
        catch (Exception ex)
        {
            return new { Success = false, Message = "系統錯誤: " + ex.Message };
        }
    }

    /// <summary>
    /// 顯示訊息的輔助方法
    /// </summary>
    /// <param name="message">訊息內容</param>
    /// <param name="isSuccess">是否為成功訊息</param>
    private void ShowMessage(string message, bool isSuccess)
    {
        // 可以使用 JavaScript Alert 或其他方式顯示訊息
        string script = $"alert('{message}');";
        ClientScript.RegisterStartupScript(this.GetType(), "ShowMessage", script, true);
    }
}