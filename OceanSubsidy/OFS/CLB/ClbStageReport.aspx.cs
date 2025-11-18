using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.Services;
using GS.OCA_OceanSubsidy.Entity;
using GS.Data;

public partial class OFS_CLB_ClbStageReport : System.Web.UI.Page
{
    /// <summary>
    /// 目前處理的計畫ID
    /// </summary>
    protected string ProjectID => Request.QueryString["ProjectID"];

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            InitializePage();
        }
    }
    
    /// <summary>
    /// 初始化頁面
    /// </summary>
    private void InitializePage()
    {
        try
        {
            // 設定 Master Page 的計畫資訊
            var master = Master as OFS_CLB_ClbInprogress;
            if (master != null && !string.IsNullOrEmpty(ProjectID))
            {
                master.ProjectInfoText = $"{ProjectID}/階段報告";
            }
            
            // 載入報告資料
            LoadReportData();
            
            // 載入已上傳檔案狀態
            LoadUploadedFileStatus();
            
            // 初始化頁面狀態（根據階段審查狀態控制UI顯示）
            InitializePageState();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"初始化階段報告頁面時發生錯誤: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 載入報告資料
    /// </summary>
    private void LoadReportData()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"載入報告資料時發生錯誤: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 載入已上傳檔案狀態
    /// </summary>
    private void LoadUploadedFileStatus()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return;
            }

            // 檢查檔案類型的上傳狀態
            string[] fileCodes = { "StageReport1" };
            string[] fileTypes = { "1" };

            string jsCode = @"
                document.addEventListener('DOMContentLoaded', function() {
            ";

            for (int i = 0; i < fileCodes.Length; i++)
            {
                var uploadedFile = OFS_ClbStageReportHelper.GetUploadedFile(ProjectID, fileCodes[i]);
                if (uploadedFile != null)
                {
                    jsCode += $@"
                        // 設定檔案 {fileTypes[i]} 的狀態
                        document.getElementById('uploadStatus{fileTypes[i]}').textContent = '已上傳';
                        document.getElementById('uploadStatus{fileTypes[i]}').className = 'text-success';
                        document.getElementById('fileName{fileTypes[i]}').textContent = '{uploadedFile.FileName.Replace("'", "\\'")}';
                        document.getElementById('uploadedFile{fileTypes[i]}').style.display = 'flex';
                        document.getElementById('uploadedFile{fileTypes[i]}').dataset.fileId = '{uploadedFile.ID}';
                    ";
                }
            }

            jsCode += @"
                });
            ";

            ClientScript.RegisterStartupScript(this.GetType(), "LoadUploadedFileStatus", jsCode, true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"載入已上傳檔案狀態時發生錯誤: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 初始化頁面狀態（根據階段審查狀態控制UI顯示）
    /// </summary>
    private void InitializePageState()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
                return;
                
            // 取得階段審查狀態
            var statusInfo = OFS_ClbStageReportHelper.GetStageExamStatus(ProjectID);
            
            // 產生 JavaScript 程式碼來控制 UI 顯示
            string jsCode = $@"
                document.addEventListener('DOMContentLoaded', function() {{
                    window.initializePageState({{
                        status: '{statusInfo.Status}',
                        canEdit: {statusInfo.CanEdit.ToString().ToLower()},
                        canReview: {statusInfo.CanReview.ToString().ToLower()},
                        showReviewPanel: {statusInfo.ShowReviewPanel.ToString().ToLower()}
                    }});
                }});
            ";
            
            ClientScript.RegisterStartupScript(this.GetType(), "InitializePageState", jsCode, true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"初始化頁面狀態時發生錯誤: {ex.Message}");
        }
    }

    #region WebMethods - 階段審查功能
    
    /// <summary>
    /// 取得頁面狀態資訊
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>頁面狀態資訊</returns>
    [WebMethod]
    public static object GetPageState(string projectID)
    {
        try
        {
            if (string.IsNullOrEmpty(projectID))
            {
                return new { success = false, message = "計畫編號不能為空" };
            }

            var statusInfo = OFS_ClbStageReportHelper.GetStageExamStatus(projectID);
            
            return new { 
                success = true,
                status = statusInfo.Status,
                canEdit = statusInfo.CanEdit,
                canReview = statusInfo.CanReview,
                showReviewPanel = statusInfo.ShowReviewPanel
            };
        }
        catch (Exception ex)
        {
            return new { 
                success = false, 
                message = $"取得頁面狀態失敗：{ex.Message}" 
            };
        }
    }

    #endregion

    #region WebMethods - 提送功能

    /// <summary>
    /// 提送階段報告 (暫存或提送)
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="isDraft">是否為暫存 (true=暫存, false=提送)</param>
    /// <returns>操作結果</returns>
    [WebMethod]
    public static object SubmitStageReport(string projectID, bool isDraft)
    {
        try
        {
            if (string.IsNullOrEmpty(projectID))
            {
                return new { success = false, message = "計畫編號不能為空" };
            }

            // 呼叫 Helper 處理資料庫操作
            OFS_ClbStageReportHelper.SaveStageExamData(projectID, isDraft);
            
            string actionText = isDraft ? "暫存" : "提送";
            return new { 
                success = true, 
                message = $"{actionText}成功！", 
                isDraft = isDraft 
            };
        }
        catch (Exception ex)
        {
            string actionText = isDraft ? "暫存" : "提送";
            return new { 
                success = false, 
                message = $"{actionText}失敗：{ex.Message}" 
            };
        }
    }
    
    /// <summary>
    /// 提交審查結果
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="reviewResult">審查結果 (pass/reject)</param>
    /// <param name="ReviewCommit">審查意見</param>
    /// <returns>操作結果</returns>
    [WebMethod]
    public static object SubmitReview(string projectID, string reviewResult, string ReviewCommit)
    {
        try
        {
            if (string.IsNullOrEmpty(projectID))
            {
                return new { success = false, message = "計畫編號不能為空" };
            }
            
            if (string.IsNullOrEmpty(reviewResult))
            {
                return new { success = false, message = "請選擇審查結果" };
            }

            // 呼叫 Helper 處理審查結果提交
            bool result = OFS_ClbStageReportHelper.SubmitReviewResult(projectID, reviewResult, ReviewCommit);
            
            if (result)
            {
                string resultText = reviewResult == "pass" ? "通過" : "不通過";
                return new { 
                    success = true, 
                    message = $"審查結果提交成功！結果：{resultText}", 
                    reviewResult = reviewResult 
                };
            }
            else
            {
                return new { 
                    success = false, 
                    message = "審查結果提交失敗" 
                };
            }
        }
        catch (Exception ex)
        {
            return new { 
                success = false, 
                message = $"提交審查結果失敗：{ex.Message}" 
            };
        }
    }

    #endregion

}