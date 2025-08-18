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
                // 計劃書修正中 -> 計劃書審核中
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
        string script = "window.SciUploadAttachments.showSubmitSuccess();";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowSweetAlertSuccess", script, true);
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
    /// 更新計劃書修正狀態 - 計劃書修正中 -> 計劃書審核中
    /// </summary>
    private void UpdateProjectStatusForPlanRevision()
    {
        try
        {
            // 使用ReviewCheckListHelper更新StatusesName
            ReviewCheckListHelper.UpdateProjectStatusName(CurrentProjectID, "計畫書審核中", GetCurrentUserInfo()?.Account ?? "系統");
        }
        catch (Exception ex)
        {
            throw new Exception($"更新計劃書修正狀態時發生錯誤：{ex.Message}");
        }
    }

    /// <summary>
    /// 記錄計劃書修正提送申請的操作歷程
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
                Description = "完成計劃書修正並重新提送審核"
            };

            // 儲存到資料庫
            bool success = ApplicationChecklistHelper.InsertCaseHistoryLog(caseHistoryLog);

            if (success)
            {
                System.Diagnostics.Debug.WriteLine($"計劃書修正提送歷程記錄已儲存：{CurrentProjectID}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"計劃書修正提送歷程記錄儲存失敗：{CurrentProjectID}");
            }
        }
        catch (Exception ex)
        {
            // 歷程記錄失敗不影響主要流程，只記錄錯誤
            System.Diagnostics.Debug.WriteLine($"記錄計劃書修正提送歷程失敗：{ex.Message}");
        }
    }

    #endregion
}