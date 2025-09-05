using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class OFS_CLB_ClbApplication : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // 處理 AJAX 請求
        string action = Request.Form["action"];
        if (!string.IsNullOrEmpty(action))
        {
            switch (action)
            {
                case "uploadFile":
                    HandleFileUpload();
                    return;
                case "deleteFile":
                    HandleFileDelete();
                    return;
                case "submitApplication":
                    HandleApplicationSubmission();
                    return;
            }
        }

        if (!IsPostBack)
        {
            // 初始化頁面
            InitializePage();
        }
    }

    private void InitializePage()
    {
        // 檢查 URL 參數是否有 ProjectID
        string projectID = Request.QueryString["ProjectID"];
        string step = Request.QueryString["step"];
        
        // 將 ProjectID 傳遞給 UserControl，讓它自己處理載入邏輯
        ucClbApplication.ProjectID = projectID ?? "";
        
        // 整合查詢 CurrentStep 並控制按鈕顯示和步驟開放
        bool enableUploadStep = false;
        SetButtonVisibilityAndStepAccess(projectID, out enableUploadStep);
        
        // 根據 step 參數和權限調整頁面顯示
        if (!string.IsNullOrEmpty(step) && step == "1" && enableUploadStep)
        {
            ViewState["CurrentStep"] = step;
            
            // 修改按鈕文字
            btnSubmit.Text = "完成本頁，提送申請";
            
            // 添加 JavaScript 來切換到上傳附件介面並更新進度條狀態
            ClientScript.RegisterStartupScript(this.GetType(), "SwitchToUploadStep", @"
            $(document).ready(function() { 
                switchToStep(1); 
                // 更新第一步的狀態為已完成
                $('.application-step .step-item:eq(0)').removeClass('active').find('.step-status').text('已完成').removeClass('edit');
                // 設定第二步為編輯中
                $('.application-step .step-item:eq(1)').addClass('active').find('.step-content').append('<div class=""step-status edit"">編輯中</div>');
            });", true);
        }
        else if (!string.IsNullOrEmpty(step) && step == "1" && !enableUploadStep)
        {
            // 如果試圖進入上傳附件步驟但沒有權限，重導向回申請表步驟
            string redirectUrl = "ClbApplication.aspx";
            if (!string.IsNullOrEmpty(projectID))
            {
                redirectUrl += "?ProjectID=" + Server.UrlEncode(projectID);
            }
            Response.Redirect(redirectUrl);
        }
        else
        {
            ViewState["CurrentStep"] = "0"; // 預設為申請表步驟
        }
        
        // 設定上傳附件步驟的可用狀態
        string stepStateScript = $@"
        $(document).ready(function() {{ 
            if ({enableUploadStep.ToString().ToLower()}) {{
                $('.application-step .step-item:eq(1)')
                    .removeClass('disabled')
                    .removeAttr('tabindex')
                    .removeAttr('aria-disabled')
                    .attr('onclick', 'navigateToStep(1)');
            }} else {{
                $('.application-step .step-item:eq(1)')
                    .addClass('disabled')
                    .attr('tabindex', '-1')
                    .attr('aria-disabled', 'true')
                    .removeAttr('onclick');
            }}
        }});";
        
        ClientScript.RegisterStartupScript(this.GetType(), "SetStepState", stepStateScript, true);
    }


    /// <summary>
    /// 整合查詢 CurrentStep，同時控制暫存按鈕顯示和上傳附件步驟開放
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="enableUploadStep">是否開放上傳附件步驟</param>
    private void SetButtonVisibilityAndStepAccess(string projectID, out bool enableUploadStep)
    {
        // 預設值
        enableUploadStep = false;
        bool showTempSaveButton = true; // 預設顯示暫存按鈕（當找不到 projectID 時）
        
        try
        {
            // 如果有 ProjectID，查詢 CurrentStep
            if (!string.IsNullOrEmpty(projectID))
            {
                string currentStep = OFS_ClbApplicationHelper.GetProjectCurrentStep(projectID);
                
                if (!string.IsNullOrEmpty(currentStep) && int.TryParse(currentStep, out int stepNumber))
                {
                    // 控制暫存按鈕：CurrentStep > 1 時隱藏
                    showTempSaveButton = stepNumber <= 1;
                    
                    // 控制上傳附件步驟：CurrentStep >= 2 時開放
                    enableUploadStep = stepNumber >= 2;
                }
            }
            
            // 設定按鈕可見性
            btnTempSave.Visible = showTempSaveButton;
        }
        catch (Exception)
        {
            // 如果發生例外，使用預設值
            enableUploadStep = false;
            btnTempSave.Visible = true;
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        Button clickedButton = (Button)sender;
        
        try
        {
            if (clickedButton.ID == "btnTempSave")
            {
                // 暫存邏輯
                TempSave();
            }
            else if (clickedButton.ID == "btnSubmit")
            {
                // 儲存並進入下一步
                SaveAndNext();
            }
        }
        catch (Exception ex)
        {
            // 錯誤處理
            Response.Write($"<script>alert('操作失敗：{ex.Message}');</script>");
        }
    }

    private void TempSave()
    {
        try
        {
            // 直接引用 UserControl
            // 執行暫存（傳入 true 表示暫存）
            string projectID = ucClbApplication.SaveBasicData(true);
            
            if (!string.IsNullOrEmpty(projectID))
            {
                // 顯示成功訊息，包含 ProjectID，按下確定後導向
                string script = $@"
                    Swal.fire({{
                        icon: 'success',
                        title: '暫存成功！',
                        html: '計畫編號：<strong>{projectID}</strong><br>資料已成功暫存',
                        confirmButtonText: '確定'
                    }}).then((result) => {{
                        if (result.isConfirmed) {{
                            // 導向有 ProjectID 參數的頁面
                            window.location.href = 'ClbApplication.aspx?ProjectID={projectID}';
                        }}
                    }});";
                
                ClientScript.RegisterStartupScript(this.GetType(), "TempSaveSuccess", script, true);
            }
            else
            {
                throw new Exception("儲存失敗，請檢查必填欄位");
            }
        }
        catch (Exception ex)
        {
            // 錯誤處理
            string script = $@"
                Swal.fire({{
                    icon: 'error',
                    title: '暫存失敗',
                    text: '{ex.Message}',
                    confirmButtonText: '確定'
                }});";
            
            ClientScript.RegisterStartupScript(this.GetType(), "TempSaveError", script, true);
        }
    }

    private void SaveAndNext()
    {
        string currentStep = ViewState["CurrentStep"]?.ToString() ?? "0";
        
        if (currentStep == "0") // 申請表步驟
        {
            SaveAndGoToUploadStep();
        }
        else if (currentStep == "1") // 上傳附件/提送申請步驟
        {
            SubmitApplication();
        }
    }
    
    private void SaveAndGoToUploadStep()
    {
        try
        {
            // 直接引用 UserControl
            // 執行正式儲存（傳入 false 表示正式儲存）
            string projectID = ucClbApplication.SaveBasicData(false);
            
            if (!string.IsNullOrEmpty(projectID))
            {
                // 儲存成功，導向到上傳附件步驟
                string script = $@"
                    Swal.fire({{
                        icon: 'success',
                        title: '儲存成功！',
                        html: '計畫編號：<strong>{projectID}</strong><br>即將進入上傳附件步驟',
                        timer: 2000,
                        showConfirmButton: false
                    }}).then(() => {{
                        // 導向到上傳附件步驟
                        window.location.href = 'ClbApplication.aspx?ProjectID={projectID}&step=1';
                    }});";
                
                ClientScript.RegisterStartupScript(this.GetType(), "SaveAndNextSuccess", script, true);
            }
            else
            {
                throw new Exception("儲存失敗，請檢查必填欄位");
            }
        }
        catch (Exception ex)
        {
            // 錯誤處理
            string script = $@"
                Swal.fire({{
                    icon: 'error',
                    title: '儲存失敗',
                    text: '{ex.Message}',
                    confirmButtonText: '確定'
                }});";
            
            ClientScript.RegisterStartupScript(this.GetType(), "SaveAndNextError", script, true);
        }
    }
    
    private void SubmitApplication()
    {
        try
        {
            // 優先從 QueryString 取得 ProjectID，如果沒有再從 UserControl 取得
            string projectID = Request.QueryString["ProjectID"] ?? ucClbApplication.ProjectID;
            
            if (string.IsNullOrEmpty(projectID))
            {
                throw new Exception("計畫編號不存在，請確認 URL 參數或重新進入頁面");
            }
            
            // 顯示確認對話框
            string script = $@"
                Swal.fire({{
                    icon: 'warning',
                    title: '確認提送申請',
                    html: '是否要進行提送？<br><span style=""color: red;"">提送後將無法再編輯資料。</span>',
                    showCancelButton: true,
                    confirmButtonText: '是，提送申請',
                    cancelButtonText: '取消',
                    reverseButtons: true
                }}).then((result) => {{
                    if (result.isConfirmed) {{
                        // 使用者確認提送，執行提送邏輯
                        submitApplicationFinal('{projectID}');
                    }}
                }});";
            
            ClientScript.RegisterStartupScript(this.GetType(), "ConfirmSubmission", script, true);
        }
        catch (Exception ex)
        {
            // 錯誤處理
            string script = $@"
                Swal.fire({{
                    icon: 'error',
                    title: '提送失敗',
                    text: '{ex.Message}',
                    confirmButtonText: '確定'
                }});";
            
            ClientScript.RegisterStartupScript(this.GetType(), "SubmitError", script, true);
        }
    }

    #region 檔案上傳處理方法

    /// <summary>
    /// 處理檔案上傳請求
    /// </summary>
    private void HandleFileUpload()
    {
        Response.ContentType = "application/json";
        
        try
        {
            string fileCode = Request.Form["fileCode"];
            string projectID = Request.Form["projectID"];
            
            if (string.IsNullOrEmpty(fileCode))
            {
                Response.Write("{\"success\":false,\"message\":\"檔案代碼不能為空\"}");
                return;
            }

            if (Request.Files.Count == 0 || Request.Files["uploadedFile"] == null)
            {
                Response.Write("{\"success\":false,\"message\":\"請選擇要上傳的檔案\"}");
                return;
            }

            HttpPostedFile uploadedFile = Request.Files["uploadedFile"];
            
            // 呼叫 UserControl 的檔案上傳方法
            string result = ucClbApplication.HandleFileUpload(projectID,fileCode, uploadedFile);
            
            if (string.IsNullOrEmpty(result))
            {
                // 取得檔案資訊並回傳
                var uploadedFileInfo = OFS_ClbApplicationHelper.GetUploadedFile(projectID ?? ucClbApplication.ProjectID, fileCode);
                if (uploadedFileInfo != null)
                {
                    Response.Write($"{{\"success\":true,\"message\":\"檔案上傳成功\",\"fileName\":\"{uploadedFileInfo.FileName}\",\"relativePath\":\"{uploadedFileInfo.TemplatePath}\"}}");
                }
                else
                {
                    Response.Write("{\"success\":true,\"message\":\"檔案上傳成功\"}");
                }
            }
            else
            {
                Response.Write($"{{\"success\":false,\"message\":\"{result}\"}}");
            }
        }
        catch (Exception ex)
        {
            Response.Write($"{{\"success\":false,\"message\":\"檔案上傳失敗：{ex.Message}\"}}");
        }
        
        Response.End();
    }

    /// <summary>
    /// 處理檔案刪除請求
    /// </summary>
    private void HandleFileDelete()
    {
        Response.ContentType = "application/json";
        
        try
        {
            string fileCode = Request.Form["fileCode"];
            string projectID = Request.Form["projectID"];
            
            if (string.IsNullOrEmpty(fileCode))
            {
                Response.Write("{\"success\":false,\"message\":\"檔案代碼不能為空\"}");
                return;
            }

            if (string.IsNullOrEmpty(projectID))
            {
                Response.Write("{\"success\":false,\"message\":\"計畫編號不能為空\"}");
                return;
            }

            // 設定 UserControl 的 ProjectID
            ucClbApplication.ProjectID = projectID;
            
            // 呼叫 UserControl 的檔案刪除方法
            string result = ucClbApplication.DeleteUploadedFile(fileCode);
            
            if (string.IsNullOrEmpty(result))
            {
                Response.Write("{\"success\":true,\"message\":\"檔案刪除成功\"}");
            }
            else
            {
                Response.Write($"{{\"success\":false,\"message\":\"{result}\"}}");
            }
        }
        catch (Exception ex)
        {
            Response.Write($"{{\"success\":false,\"message\":\"檔案刪除失敗：{ex.Message}\"}}");
        }
        
        Response.End();
    }

    /// <summary>
    /// 處理申請提送請求
    /// </summary>
    private void HandleApplicationSubmission()
    {
        Response.ContentType = "application/json";
        
        try
        {
            string projectID = Request.Form["projectID"];
            
            // 如果 Form 中沒有 ProjectID，嘗試從 QueryString 獲取
            if (string.IsNullOrEmpty(projectID))
            {
                projectID = Request.QueryString["ProjectID"];
            }
            
            if (string.IsNullOrEmpty(projectID))
            {
                Response.Write("{\"success\":false,\"message\":\"計畫編號不能為空\"}");
                return;
            }

            // 更新計畫狀態
            OFS_ClbApplicationHelper.UpdateProjectStatus(projectID, "內容審查", "審核中", "3");
            
           
            Response.Write("{\"success\":true,\"message\":\"申請已成功提送，狀態已更新為審核中\"}");
            
       
        }
        catch (Exception ex)
        {
            Response.Write($"{{\"success\":false,\"message\":\"提送失敗：{ex.Message}\"}}");
        }
        
        Response.End();
    }

    #endregion
}