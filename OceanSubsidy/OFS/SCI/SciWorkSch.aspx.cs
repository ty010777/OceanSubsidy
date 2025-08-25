using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Operation.OFS;

/// <summary>
/// 科專計畫工作排程頁面
/// 提供期程/工作項目/查核標準的填寫、儲存功能
/// </summary>
public partial class OFS_SciWorkSch : System.Web.UI.Page
{
    private string ProjectID => Request.QueryString["ProjectID"];
    
    #region 頁面生命週期
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // 設定顯示模式
            SetDisplayMode();

            if (!IsPostBack)
            {
                InitializePage();
                
                // 載入資料到 UserControl
                var isViewMode = !ShouldShowInEditMode();
                sciWorkSchControl.LoadData(ProjectID, isViewMode);
                
                // 載入變更說明控制項
                ucChangeDescription.LoadData(ProjectID, isViewMode);
                
                // 載入變更說明資料到輸入框
                LoadChangeDescriptionData();
            }
        }
        catch (Exception ex)
        {
            Response.Write($"<script>alert('頁面載入錯誤：{ex.Message}');</script>");
        }
    }

    /// <summary>
    /// 初始化頁面
    /// </summary>
    private void InitializePage()
    {
        try
        {
            // 檢查表單狀態並控制暫存按鈕顯示
            CheckFormStatusAndHideTempSaveButton();
        }
        catch (Exception ex)
        {
            ShowMessage($"載入資料時發生錯誤：{ex.Message}", "error");
        }
    }
    #endregion

    #region 按鈕事件處理
    /// <summary>
    /// 暫存按鈕點擊事件
    /// </summary>
    protected void btnTempSave_Click(object sender, EventArgs e)
    {
        try
        {
            // 設定 ProjectID 到 UserControl
            sciWorkSchControl.ProjectID = ProjectID;
            
            // 儲存資料
            sciWorkSchControl.SaveData();
            
            // 儲存變更說明
            ucChangeDescription.SaveChangeDescription(ProjectID);
            
            // 更新版本狀態
            UpdateVersionStatusBasedOnAction(ProjectID, false);
            
            // 重新載入資料到 UserControl 並自動渲染到前端
            // LoadData 方法會自動調用 LoadWorkItems, LoadCheckStandards, LoadDiagramFile
            // 這些方法都會註冊 JavaScript 來重新渲染前端資料
            var isViewMode = !ShouldShowInEditMode();
            sciWorkSchControl.LoadData(ProjectID, isViewMode);
            
            ShowMessage("資料暫存成功！", "success");
        }
        catch (Exception ex)
        {
            ShowMessage($"暫存失敗：{ex.Message}", "error");
        }
    }

    /// <summary>
    /// 儲存並下一步按鈕點擊事件
    /// </summary>
    protected void btnSaveAndNext_Click(object sender, EventArgs e)
    {
        try
        {
            // 設定 ProjectID 到 UserControl（不重新載入資料）
            sciWorkSchControl.ProjectID = ProjectID;
            
            // 驗證資料
            // var validationResult = sciWorkSchControl.ValidateForm();
            if (true)
            {
                // 儲存資料
                sciWorkSchControl.SaveData();
                
                // 儲存變更說明
                ucChangeDescription.SaveChangeDescription(ProjectID);
                
                // 更新版本狀態
                UpdateVersionStatusBasedOnAction(ProjectID, true);
                
                // 跳轉到下一頁
                Response.Redirect($"SciFunding.aspx?ProjectID={ProjectID}");
            }
            else
            {
                // ShowMessage($"資料驗證失敗：\\n{validationResult.GetErrorsAsString()}", "error");
            }
        }
        catch (Exception ex)
        {
            ShowMessage($"儲存失敗：{ex.Message}", "error");
        }
    }

    #endregion

    #region 版本狀態處理

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
                // 1. Form2Status 設為 "完成" 
                // 2. 檢查 CurrentStep，如果 < 3則改成 3
                
                string currentStep = OFS_SciWorkSchHelper.GetCurrentStepByProjectID(ProjectID);
                int currentStepNum = 1;
                int.TryParse(currentStep, out currentStepNum);
                
                bool shouldUpdateCurrentStep = currentStepNum < 3;
                string newCurrentStep = shouldUpdateCurrentStep ? "3" : currentStep;
                
                OFS_SciWorkSchHelper.UpdateVersionStatus(ProjectID, "完成", shouldUpdateCurrentStep, newCurrentStep);
            }
            else
            {
                // 點擊「暫存」按鈕
                // 只更新 Form2Status 為 "暫存"，CurrentStep 不變
                
                OFS_SciWorkSchHelper.UpdateVersionStatus(ProjectID, "暫存");
                
            }
        }
        catch (Exception ex)
        {
            // 記錄錯誤但不中斷流程
            Console.WriteLine($"更新版本狀態失敗: {ex.Message}");
        }
    }



    /// <summary>
    /// 顯示訊息給使用者
    /// </summary>
    private void ShowMessage(string message, string type = "info")
    {
        var script = $"alert('{message.Replace("'", "\\'")}');";
        ClientScript.RegisterStartupScript(this.GetType(), "ShowMessage", script, true);
    }
    
    #endregion

    #region 工具方法
    /// <summary>
    /// 取得目前的 ProjectID
    /// </summary>
   
    
    /// <summary>
    /// 檢查表單狀態並控制暫存按鈕顯示
    /// </summary>
    private void CheckFormStatusAndHideTempSaveButton()
    {
        try
        {
            var ProjectID = Request.QueryString["ProjectID"];
            var formStatus = OFS_SciWorkSchHelper.GetFormStatusByProjectID(ProjectID, "Form2Status");
            
            if (formStatus == "完成")
            {
                // 隱藏暫存按鈕
                btnTempSave.Style["display"] = "none";
                
                // 也可以用 Visible 屬性
            }
        }
        catch (Exception ex)
        {
            // 發生錯誤時不隱藏按鈕，讓用戶正常使用
            System.Diagnostics.Debug.WriteLine($"檢查表單狀態失敗: {ex.Message}");
        }
    }
    #endregion


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
            if (!string.IsNullOrEmpty(ProjectID))
            {
                // 從資料庫取得變更說明並設定到頁面元素
                var changeDescription = OFS_SciApplicationHelper.GetPageModifyNote(ProjectID, "SciWorkSch");
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