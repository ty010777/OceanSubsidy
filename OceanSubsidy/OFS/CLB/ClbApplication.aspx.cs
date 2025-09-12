using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;

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
        // 檢查 URL 參數
        string projectID = Request.QueryString["ProjectID"];
        string step = Request.QueryString["step"] ?? "0"; // 預設為步驟 0
        
        // 將 ProjectID 傳遞給 UserControl，讓它自己處理載入邏輯
        ucClbApplication.ProjectID = projectID ?? "";
        
        // 判斷並設定檢視模式
        SetViewModeBasedOnStatus(projectID);
        
        // 根據 step 參數和 SetButtonVisibilityAndStepAccess 結果來控制顯示
        bool enableUploadStep = false;
        SetButtonVisibilityAndStepAccess(projectID, out enableUploadStep);
        
        // 根據 URL step 參數控制按鈕和區塊顯示
        int stepNumber = 0;
        if (int.TryParse(step, out stepNumber))
        {
            SetUIBasedOnStep(stepNumber, enableUploadStep);
        }
  
    }
    
    /// <summary>
    /// 根據步驟號碼設置 UI 顯示
    /// </summary>
    /// <param name="stepNumber">步驟號碼 (0=申請表, 1=上傳附件)</param>
    /// <param name="enableUploadStep">是否啟用上傳附件步驟</param>
    private void SetUIBasedOnStep(int stepNumber, bool enableUploadStep)
    {
        // 註冊 JavaScript 來控制前端顯示
        string script = "";
        
        if (stepNumber == 0)
        {
            // 步驟 0: 顯示申請表
            btnSaveAndNext.Style["display"] = "";  // 顯示「完成本頁，下一步」
            btnSubmitApplication.Style["display"] = "none";  // 隱藏「完成本頁，提送申請」
            
            script = @"
                $(document).ready(function() {
                    // 設置進度條狀態
                    $('.application-step .step-item').removeClass('active');
                    $('.application-step .step-item').eq(0).addClass('active');
                    
                    // 顯示申請表區塊，隱藏上傳附件區塊
                    $('#applicationFormSection').show();
                    $('#uploadAttachmentSection').hide();
                    
                    // 設置步驟狀態文字
                    var firstStep = $('.application-step .step-item').eq(0);
                    var secondStep = $('.application-step .step-item').eq(1);
                    
                    // 第一步：當前在編輯，顯示編輯中
                    firstStep.find('.step-status').remove();
                    firstStep.find('.step-content').append('<div class=""step-status edit"">編輯中</div>');
                    
                    // 第二步：根據是否啟用決定狀態
                    secondStep.find('.step-status').remove();
                    if (" + enableUploadStep.ToString().ToLower() + @") {
                        // 已啟用：顯示已完成狀態，但移除編輯中樣式
                        secondStep.find('.step-content').append('<div class=""step-status"">已完成</div>');
                        secondStep.removeClass('disabled').attr('aria-disabled', 'false').attr('tabindex', '0');
                    } else {
                        // 未啟用：不顯示任何狀態
                        secondStep.addClass('disabled').attr('aria-disabled', 'true').removeAttr('tabindex');
                    }
                });
            ";
        }
        else if (stepNumber == 1 && enableUploadStep)
        {
            // 步驟 1: 顯示上傳附件（只有在啟用時才顯示）
            btnSaveAndNext.Style["display"] = "none";  // 隱藏「完成本頁，下一步」
             btnSubmitApplication.Style["display"] = "";  // 顯示「完成本頁，提送申請」
            
            script = @"
                $(document).ready(function() {
                    // 設置進度條狀態
                    $('.application-step .step-item').removeClass('active');
                    $('.application-step .step-item').eq(1).addClass('active');
                    
                    // 隱藏申請表區塊，顯示上傳附件區塊
                    $('#applicationFormSection').hide();
                    $('#uploadAttachmentSection').show();
                    
                    // 設置步驟狀態文字
                    var firstStep = $('.application-step .step-item').eq(0);
                    var secondStep = $('.application-step .step-item').eq(1);
                    
                    // 第一步：已完成
                    firstStep.find('.step-status').remove();
                    firstStep.find('.step-content').append('<div class=""step-status"">已完成</div>');
                    
                    // 第二步：當前在編輯，顯示編輯中
                    secondStep.find('.step-status').remove();
                    secondStep.find('.step-content').append('<div class=""step-status edit"">編輯中</div>');
                    
                    // 確保兩個步驟都是啟用狀態
                    $('.application-step .step-item').removeClass('disabled')
                        .attr('aria-disabled', 'false').attr('tabindex', '0');
                });
            ";
        }
        else
        {
            // 無效步驟或未啟用的步驟，回到步驟 0
            btnSaveAndNext.Style["display"] = "";
            btnSubmitApplication.Style["display"] = "none";
            
            script = @"
                $(document).ready(function() {
                    // 設置進度條狀態
                    $('.application-step .step-item').removeClass('active');
                    $('.application-step .step-item').eq(0).addClass('active');
                    
                    // 顯示申請表區塊，隱藏上傳附件區塊
                    $('#applicationFormSection').show();
                    $('#uploadAttachmentSection').hide();
                    
                    // 設置步驟狀態文字
                    var firstStep = $('.application-step .step-item').eq(0);
                    var secondStep = $('.application-step .step-item').eq(1);
                    
                    // 第一步：當前在編輯，顯示編輯中
                    firstStep.find('.step-status').remove();
                    firstStep.find('.step-content').append('<div class=""step-status edit"">編輯中</div>');
                    
                    // 第二步：根據是否啟用決定狀態
                    secondStep.find('.step-status').remove();
                    if (" + enableUploadStep.ToString().ToLower() + @") {
                        secondStep.find('.step-content').append('<div class=""step-status"">已完成</div>');
                        secondStep.removeClass('disabled').attr('aria-disabled', 'false').attr('tabindex', '0');
                    } else {
                        secondStep.addClass('disabled').attr('aria-disabled', 'true').removeAttr('tabindex');
                    }
                });
            ";
        }
        
        // 註冊 JavaScript
        ClientScript.RegisterStartupScript(this.GetType(), "InitializePageUI", script, true);
    }

    /// <summary>
    /// 根據專案狀態設定檢視模式
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    private void SetViewModeBasedOnStatus(string projectID)
    {
        try
        {
            // 如果 projectID 為空或找不到，顯示編輯模式
            if (string.IsNullOrEmpty(projectID))
            {
                ucClbApplication.IsReadOnly = false;
            }
            else
            {
                // 從資料庫取得專案狀態資訊
                var projectMain = OFS_ClbApplicationHelper.GetProjectMainData(projectID);
                
                if (projectMain == null)
                {
                    // 找不到專案資料，顯示編輯模式
                    ucClbApplication.IsReadOnly = false;
                }
                else
                {
                    string statuses = projectMain.Statuses ?? "";
                    string statusesName = projectMain.StatusesName ?? "";

                    // 判斷狀態是否允許編輯
                    if ((statuses == "尚未提送" && statusesName == "編輯中") ||
                        (statuses == "決審核定" && statusesName == "計畫書修正中") ||
                        (statuses == "內容審查" && statusesName == "補正補件"))
                    {
                        // 可編輯狀態
                        ucClbApplication.IsReadOnly = false;
                    }
                    else
                    {
                        // 其他狀態都是檢視模式
                        ucClbApplication.IsReadOnly = true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // 發生錯誤時，預設為編輯模式
            ucClbApplication.IsReadOnly = false;
            System.Diagnostics.Debug.WriteLine($"設定檢視模式時發生錯誤: {ex.Message}");
        }
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

    protected void btnTempSave_Click(object sender, EventArgs e)
    {
        try
        {
            // 暫存邏輯
            TempSave();
        }
        catch (Exception ex)
        {
            // 錯誤處理
            Response.Write($"<script>alert('操作失敗：{ex.Message}');</script>");
        }
    }
    
    protected void btnSaveAndNext_Click(object sender, EventArgs e)
    {
        try
        {
            // 儲存並進入下一步
            SaveAndGoToUploadStep();
        }
        catch (Exception ex)
        {
            // 錯誤處理
            Response.Write($"<script>alert('操作失敗：{ex.Message}');</script>");
        }
    }
    
    protected void btnSubmitApplication_Click(object sender, EventArgs e)
    {
        try
        {
            // 提送申請
            SubmitApplication();
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
            // 取得最新歷史記錄的狀態
            string stageStatusBefore = ApplicationChecklistHelper.GetLatestStageStatus(projectID) ?? "";
            var lastProjectMain = OFS_ClbApplicationHelper.GetProjectMainData(projectID);
            
            // 更新計畫狀態
            if (lastProjectMain.StatusesName == "計畫書修正中")
            {
                OFS_ClbApplicationHelper.UpdateProjectStatus(projectID, "決審核定", "計畫書審核中", "3");
            }else{
                OFS_ClbApplicationHelper.UpdateProjectStatus(projectID, "內容審查", "審核中", "3");
            }
            var currentUser = GetCurrentUserInfo();
            var projectMain = OFS_ClbApplicationHelper.GetProjectMainData(projectID);
            var historyLog = new OFS_CaseHistoryLog
            {
                ProjectID = projectID,
                ChangeTime = DateTime.Now,
                UserName = currentUser?.UserName ?? "系統",
                StageStatusBefore = stageStatusBefore,
                StageStatusAfter =  projectMain.Statuses + projectMain.StatusesName,
                Description = $"提送至{projectMain.Statuses + projectMain.StatusesName}" 
                                 
            };
            ApplicationChecklistHelper.InsertCaseHistoryLog(historyLog);
           
            Response.Write("{\"success\":true,\"message\":\"申請已成功提送，狀態已更新為審核中\"}");
            
       
        }
        catch (Exception ex)
        {
            Response.Write($"{{\"success\":false,\"message\":\"提送失敗：{ex.Message}\"}}");
        }
        
        Response.End();
    }

    #endregion
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
            return null;
        }
    }
}