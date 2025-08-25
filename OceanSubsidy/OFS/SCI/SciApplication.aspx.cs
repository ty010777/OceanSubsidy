using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Operation.OFS;
using GS.App;

/// <summary>
/// 科專計畫申請表頁面 - 重構版本
/// 使用 UserControl 來提供申請表功能
/// </summary>
public partial class OFS_SciApplication : System.Web.UI.Page
{
    #region 屬性

    /// <summary>
    /// 目前編輯的計畫ID
    /// </summary>
    protected string ProjectID => Request.QueryString["ProjectID"];

    #endregion

    #region 頁面事件

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // 設定顯示模式
            SetDisplayMode();
            
            if (!IsPostBack)
            {
                // 載入 UserControl 資料
                LoadUserControlData();
                // 檢查表單狀態並隱藏暫存按鈕（如果已完成）
                CheckFormStatusAndHideTempSaveButton();
                // 載入變更說明資料到輸入框
                LoadChangeDescriptionData();
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "頁面載入時發生錯誤");
        }
    }

    #endregion

    #region 按鈕事件

    /// <summary>
    /// 儲存按鈕點擊事件
    /// </summary>
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            // 判斷是暫存還是下一步
            Button btnClicked = (Button)sender;
            bool isTemporarySave = btnClicked.ID == "btnTempSave";
            
            // 如果是檢視模式，不允許儲存
            if (!ShouldShowInEditMode())
            {
                ShowMessage("目前為檢視模式，無法執行儲存操作", false);
                return;
            }

            // // 驗證表單資料
            // if (!isTemporarySave)
            // {
            //     var validationResult = ucSciApplication.ValidateForm();
            //     if (!validationResult.IsValid)
            //     {
            //         ShowMessage(validationResult.GetErrorsAsString(), false);
            //         return;
            //     }
            // }

            // 儲存資料
            string resultProjectID = ucSciApplication.SaveData(ProjectID);
            
            // 儲存變更說明
            ucChangeDescription.SaveChangeDescription(resultProjectID);

            // 更新版本狀態
            UpdateVersionStatusBasedOnAction(resultProjectID, !isTemporarySave);

            if (isTemporarySave)
            {
                ShowMessage("資料已暫存", true);
                
                // 如果是新建案件，更新URL中的ProjectID
                if (string.IsNullOrEmpty(ProjectID) && !string.IsNullOrEmpty(resultProjectID))
                {
                    Response.Redirect($"SciApplication.aspx?ProjectID={resultProjectID}");
                }
            }
            else
            {
                // 完成並跳轉到下一頁
                ShowMessage("第一步已完成，即將跳轉到下一頁", true);
                Response.Redirect($"SciWorkSch.aspx?ProjectID={resultProjectID}");
            }
        }
        catch (Exception ex)
        {
            ShowMessage($"儲存失敗：{ex.Message}", false);
            HandleException(ex, "儲存時發生錯誤");
        }
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 載入 UserControl 資料
    /// </summary>
    private void LoadUserControlData()
    {
        try
        {
            bool isViewMode = ShouldShowInEditMode() ? false : true;
            ucSciApplication.LoadData(ProjectID, isViewMode);
            
            // 載入變更說明控制項
            ucChangeDescription.LoadData(ProjectID, isViewMode);
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入 UserControl 資料時發生錯誤");
        }
    }

    /// <summary>
    /// 設定顯示模式
    /// </summary>
    private void SetDisplayMode()
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
            HandleException(ex, "設定顯示模式時發生錯誤");
        }
    }
    
    /// <summary>
    /// 判斷是否應該顯示為編輯模式
    /// </summary>
    /// <returns>true: 編輯模式, false: 檢視模式</returns>
    private bool ShouldShowInEditMode()
    {
        // 如果沒有 ProjectID，是新申請案件，可以編輯
        if (string.IsNullOrEmpty(ProjectID))
        {
            return true;
        }
        
        try
        {
            // 取得最新版本的狀態
            var projectData = OFS_SciApplicationHelper.getVersionByProjectID(ProjectID);
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
            HandleException(ex, "取得申請狀態時發生錯誤");
            return false; // 發生錯誤時預設為檢視模式
        }
    }

    /// <summary>
    /// 根據動作類型更新版本狀態
    /// </summary>
    /// <param name="projectID">計畫ID</param>
    /// <param name="isComplete">是否為完成動作（下一步）</param>
    private void UpdateVersionStatusBasedOnAction(string projectID, bool isComplete)
    {
        try
        {
            
            if (isComplete)
            {
                // 點擊「完成本頁，下一步」按鈕
                // 1. Form1Status 設為 "完成" 
                // 2. 檢查 CurrentStep，如果 <= 1 則改成 2
                
                var projectData = OFS_SciApplicationHelper.getVersionByProjectID(projectID);
                if (projectData != null)
                {
                    int res;
                    int.TryParse(projectData.CurrentStep,out res); 
                    bool shouldUpdateCurrentStep =  res <= 1;
                    string newCurrentStep = shouldUpdateCurrentStep ? "2" : projectData.CurrentStep;
                    
                    // 更新專案版本狀態
                    var updateData = new OFS_SCI_Project_Main
                    {
                        ProjectID = projectID,
                        Form1Status = "完成",
                        CurrentStep = newCurrentStep,
                        updated_at = DateTime.Now
                    };
                    
                    OFS_SciApplicationHelper.UpdateOFS_SCIVersion(updateData);
                }
            }
            else
            {
                // 點擊「暫存」按鈕
                // 只更新 Form1Status 為 "暫存"，CurrentStep 不變
                var updateData = new OFS_SCI_Project_Main
                {
                    ProjectID = projectID,
                    Form1Status = "暫存",
                    CurrentStep = "1", // 保持在第一步
                    updated_at = DateTime.Now,
                    
                    
                };
                
                OFS_SciApplicationHelper.UpdateOFS_SCIVersion(updateData);
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "更新版本狀態時發生錯誤");
        }
    }

    /// <summary>
    /// 顯示訊息
    /// </summary>
    /// <param name="message">訊息內容</param>
    /// <param name="isSuccess">是否為成功訊息</param>
    private void ShowMessage(string message, bool isSuccess)
    {
        string script = $"alert('{message.Replace("'", "\\'")}');";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowMessage", script, true);
    }

    /// <summary>
    /// 例外處理
    /// </summary>
    /// <param name="ex">例外物件</param>
    /// <param name="context">發生錯誤的上下文</param>
    private void HandleException(Exception ex, string context)
    {
        // 記錄錯誤
        System.Diagnostics.Debug.WriteLine($"{context}: {ex.Message}");
        
        // 可以在這裡加入更多的錯誤處理邏輯
        // 例如：記錄到日誌檔案、發送錯誤通知等
    }

    /// <summary>
    /// 檢查表單狀態並隱藏暫存按鈕（如果已完成）
    /// </summary>
    private void CheckFormStatusAndHideTempSaveButton()
    {
        try
        {
            if (!string.IsNullOrEmpty(ProjectID))
            {
                var formStatus = OFS_SciWorkSchHelper.GetFormStatusByProjectID(ProjectID, "Form1Status");
                
                if (formStatus == "完成")
                {
                    // 隱藏暫存按鈕
                    btnTempSave.Style["display"] = "none";
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
            if (!string.IsNullOrEmpty(ProjectID))
            {
                // 從資料庫取得變更說明並設定到頁面元素
                var changeDescription = OFS_SciApplicationHelper.GetPageModifyNote(ProjectID, "SciApplication");
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
            HandleException(ex, "載入變更說明資料時發生錯誤");
        }
    }

    #endregion
}