using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Operation.OFS;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.X509.SigI;

public partial class OFS_SciFunding : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // 設定顯示模式
            SetDisplayMode();
            
            if (!IsPostBack)
            {
                // 載入 UserControl 資料
                string projectId = Request.QueryString["ProjectID"];
                string isViewMode = Request.QueryString["IsViewMode"];
                
                if (!string.IsNullOrEmpty(projectId))
                {
                    sciFundingControl.LoadData(projectId, isViewMode == "true");
                    
                    // 載入變更說明控制項
                    ucChangeDescription.LoadData(projectId, isViewMode == "true");
                }
                
                CheckFormStatusAndHideTempSaveButton();
                
                // 載入變更說明資料到輸入框
                LoadChangeDescriptionData();
            }
        }
        catch (Exception ex)
        {
            // 發生錯誤時記錄但不中斷頁面載入
            System.Diagnostics.Debug.WriteLine($"頁面載入錯誤：{ex.Message}");
        }
    }

    
    
    protected void btnTempSave_Click(object sender, EventArgs e)
    {
        try
        {
            string ProjectID = Request.QueryString["ProjectID"];

            // 驗證 UserControl 資料
            var validationResult = sciFundingControl.ValidateForm();
            if (!validationResult.IsValid)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ValidationError", 
                    $"alert('請修正以下錯誤：\\n{validationResult.GetErrorsAsString()}');", true);
                return;
            }

            // 儲存 UserControl 資料
            if (sciFundingControl.SaveData(ProjectID))
            {
                // 儲存變更說明
                ucChangeDescription.SaveChangeDescription(ProjectID);
                // 更新版本狀態（暫存）
                if (!string.IsNullOrEmpty(ProjectID))
                {
                    UpdateVersionStatusBasedOnAction(ProjectID, false);
                }
                
                // 重新載入資料
                string isViewMode = Request.QueryString["IsViewMode"];
                sciFundingControl.LoadData(ProjectID, isViewMode == "true");
                
                Page.ClientScript.RegisterStartupScript(this.GetType(), "SaveSuccess", "alert('暫存成功！');", true);
            }
        }
        catch (Exception ex)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "SaveError", $"alert('暫存失敗：{ex.Message}');", true);
        }
    }
    
    protected void btnSaveAndNext_Click(object sender, EventArgs e)
    {
        try
        {
            string ProjectID = Request.QueryString["ProjectID"];
            // 驗證 UserControl 資料
            var validationResult = sciFundingControl.ValidateForm();
            if (!validationResult.IsValid)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ValidationError", 
                    $"alert('請修正以下錯誤：\\n{validationResult.GetErrorsAsString()}');", true);
                return;
            }

            // 儲存 UserControl 資料
            if (sciFundingControl.SaveData(ProjectID))
            {
                // 儲存變更說明
                ucChangeDescription.SaveChangeDescription(ProjectID);
                // 更新版本狀態
                if (!string.IsNullOrEmpty(ProjectID))
                {
                    UpdateVersionStatusBasedOnAction(ProjectID, true);
                }
                
                // 儲存成功後跳轉到下一頁
                if (!string.IsNullOrEmpty(ProjectID))
                {
                    Response.Redirect($"SciRecusedList.aspx?ProjectID={ProjectID}");
                }
            }
        }
        catch (Exception ex)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "SaveError", $"alert('儲存失敗：{ex.Message}');", true);
        }
    }
    
    
    /// <summary>
    /// 根據動作類型更新版本狀態
    /// </summary>
    /// <param name="ProjectID">ProjectID</param>
    /// <param name="isComplete">是否為完成動作（下一步）</param>
    private void UpdateVersionStatusBasedOnAction(string ProjectID, bool isComplete)
    {
        try
        {
            if (isComplete)
            {
                // 點擊「完成本頁，下一步」按鈕
                // 1. Form3Status 設為 "完成" 
                // 2. 檢查 CurrentStep，如果 <= 3 則改成 4
                
                string currentStep = OFS_SciWorkSchHelper.GetCurrentStepByProjectID(ProjectID);
                int currentStepNum = 1;
                int.TryParse(currentStep, out currentStepNum);
                
                bool shouldUpdateCurrentStep = currentStepNum <= 3;
                string newCurrentStep = shouldUpdateCurrentStep ? "4" : currentStep;
                
                // 更新 Form3Status 為 "完成" 和 CurrentStep (如果需要)
                // 使用通用的版本狀態更新方法，針對 Form3
                if (shouldUpdateCurrentStep)
                {
                    OFS_SciFundingHelper.UpdateForm3StatusAndCurrentStep(ProjectID, "完成", newCurrentStep);
                }
                else
                {
                    OFS_SciFundingHelper.UpdateForm3Status(ProjectID, "完成");
                }
            }
            else
            {
                // 點擊「暫存」按鈕
                // 只更新 Form3Status 為 "暫存"，CurrentStep 不變
                
                OFS_SciFundingHelper.UpdateForm3Status(ProjectID, "暫存");
            }
        }
        catch (Exception ex)
        {
         
            // 記錄錯誤但不中斷流程
            System.Diagnostics.Debug.WriteLine($"更新狀態失敗: {ex.Message}");
        }
    }

    /// <summary>
    /// 檢查表單狀態並控制暫存按鈕顯示
    /// </summary>
    private void CheckFormStatusAndHideTempSaveButton()
    {
        try
        {
            string projectId = Request.QueryString["ProjectID"];
            if (!string.IsNullOrEmpty(projectId))
            {
                var lastVersion = OFS_SciApplicationHelper.getVersionByProjectID(projectId);
                if (lastVersion != null)
                {
                    var formStatus = OFS_SciWorkSchHelper.GetFormStatusByProjectID(lastVersion.ProjectID, "Form3Status");
                    
                    if (formStatus == "完成")
                    {
                        // 隱藏暫存按鈕
                        btnTempSave.Style["display"] = "none";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // 發生錯誤時不隱藏按鈕，讓用戶正常使用
            System.Diagnostics.Debug.WriteLine($"檢查表單狀態失敗: {ex.Message}");
        }
    }
    
    #region 顯示模式控制
    
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
            System.Diagnostics.Debug.WriteLine($"設定顯示模式時發生錯誤：{ex.Message}");
        }
    }
    
    /// <summary>
    /// 判斷是否應該顯示為編輯模式
    /// </summary>
    /// <returns>true: 編輯模式, false: 檢視模式</returns>
    private bool ShouldShowInEditMode()
    {
        string ProjectID = Request.QueryString["ProjectID"];
        
        // 如果沒有 ProjectID，是新申請案件，可以編輯
        if (string.IsNullOrEmpty(ProjectID))
        {
            return true;
        }
        
        try
        {
            // 取得最新版本的狀態
            var ApplicationData = OFS_SciApplicationHelper.getVersionByProjectID(ProjectID);
            if (ApplicationData == null)
            {
                return true; // 沒有資料時允許編輯
            }
            
            // 只有這些狀態可以編輯
            string statuses = ApplicationData.Statuses ?? "";
            string statusesName = ApplicationData.StatusesName ?? "";
            
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
    
    /// <summary>
    /// 載入變更說明資料到輸入框
    /// </summary>
    private void LoadChangeDescriptionData()
    {
        try
        {
            string projectId = Request.QueryString["ProjectID"];
            if (!string.IsNullOrEmpty(projectId))
            {
                // 從資料庫取得變更說明並設定到頁面元素
                var changeDescription = OFS_SciApplicationHelper.GetPageModifyNote(projectId, "SciFunding");
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
    
    #endregion
}

