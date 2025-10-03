using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Entity.Base;
using GS.OCA_OceanSubsidy.Operation.OFS;
using GS.OCA_OceanSubsidy.Model.OFS;
using GS.App;
using GS.Data;

public partial class OFS_SCI_SciInprogress_Approved : System.Web.UI.Page
{
    #region 屬性

    /// <summary>
    /// 目前檢視的計畫ID
    /// </summary>
    protected string ProjectID => Request.QueryString["ProjectID"];

    #endregion

    #region 頁面事件

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // 檢查是否有計畫ID
            if (string.IsNullOrEmpty(ProjectID))
            {
                Response.Redirect("~/OFS/inprogressList.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // 初始化頁面（只在第一次載入時執行）
                InitializePage();
            }
            else
            {
                // PostBack 時，重新載入「非當前操作」的 UserControl 資料
                // 避免因為 ViewState 問題導致其他 tab 的資料消失
                ReloadNonActiveUserControls();
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
    /// 部門下拉選單變更事件
    /// </summary>
    protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            string selectedUnitID = ddlDepartment.SelectedValue;
            LoadReviewerDropDown(selectedUnitID);
            
            // 更新 UpdatePanel
            upTransferCase.Update();
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入承辦人員時發生錯誤");
        }
    }

    /// <summary>
    /// 確認移轉案件
    /// </summary>
    protected void btnConfirmTransfer_Click(object sender, EventArgs e)
    {
        try
        {
            string selectedDepartmentID = ddlDepartment.SelectedValue;
            string selectedReviewerAccount = ddlReviewer.SelectedValue;
            
            // 驗證必填欄位
            if (string.IsNullOrEmpty(selectedDepartmentID))
            {
                ShowSweetAlert("錯誤", "請選擇部門", "error");
                return;
            }
            
            if (string.IsNullOrEmpty(selectedReviewerAccount))
            {
                ShowSweetAlert("錯誤", "請選擇承辦人員", "error");
                return;
            }
            
            if (string.IsNullOrEmpty(ProjectID))
            {
                ShowSweetAlert("錯誤", "找不到計畫ID", "error");
                return;
            }
            
            // 取得選中的承辦人員詳細資訊
            var reviewerData = SysUserHelper.QueryUserByAccount(selectedReviewerAccount);
            if (reviewerData == null || reviewerData.Rows.Count == 0)
            {
                ShowSweetAlert("錯誤", "找不到選擇的承辦人員資訊", "error");
                return;
            }
            
            // 取得部門名稱
            var departmentData = SysUnitHelper.QueryByID(selectedDepartmentID);
            if (departmentData == null || departmentData.Rows.Count == 0)
            {
                ShowSweetAlert("錯誤", "找不到選擇的部門資訊", "error");
                return;
            }
            
            string reviewerName = reviewerData.Rows[0]["Name"].ToString();
            string departmentName = departmentData.Rows[0]["UnitName"].ToString();
            
            // 建立更新物件
            var projectMain = new OFS_SCI_Project_Main
            {
                ProjectID = ProjectID,
                SupervisoryPersonAccount = selectedReviewerAccount,
                SupervisoryPersonName = reviewerName,
                SupervisoryUnit = departmentName,
                updated_at = DateTime.Now
            };
            
            // 更新資料庫
            OFS_SciApplicationHelper.UpdateOFS_SCIVersion(projectMain);
            
            // 更新頁面上的承辦人員資訊顯示
            LoadCurrentReviewerInfo();
            
            ShowSweetAlert("成功", "案件移轉完成", "success");
            
            // 清空選項並關閉 Modal
            ddlDepartment.SelectedIndex = 0;
            ddlReviewer.Items.Clear();
            ddlReviewer.Items.Add(new ListItem("請選擇承辦人員", ""));
            
            // 關閉 Modal
            string script = @"
                document.getElementById('transferCaseModal').querySelector('.btn-close').click();
            ";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "closeModal", script, true);
            LoadAllUserControlData(true);
        }
        catch (Exception ex)
        {
            HandleException(ex, "移轉案件時發生錯誤");
            ShowSweetAlert("錯誤", "移轉案件時發生錯誤，請稍後再試", "error");
        }
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 初始化頁面
    /// </summary>
    private void InitializePage()
    {
        try
        {
            // 檢查計畫是否處於變更狀態
            bool isViewMode = true;
            var projectMain = OFS_SciApplicationHelper.getVersionByProjectID(ProjectID);
            if (projectMain != null && projectMain.IsProjChanged == 1)
            {
                // 如果計畫處於變更狀態，則開啟編輯模式
                isViewMode = false;
            }

            // 載入所有申請資料到 UserControl
            LoadAllUserControlData(isViewMode);

            // 設定計畫資訊到 Master Page
            SetProjectInfoToMaster();

            // 載入移轉案件的部門下拉選單
            LoadDepartmentDropDown();

            // 載入目前的承辦人員資訊
            LoadCurrentReviewerInfo();

            // 檢查是否顯示計畫變更審查面板
            CheckAndShowReviewPanel();
        }
        catch (Exception ex)
        {
            HandleException(ex, "初始化頁面時發生錯誤");
        }
    }

    /// <summary>
    /// 載入所有 UserControl 資料
    /// </summary>
    /// <param name="isViewMode">是否為檢視模式</param>
    private void LoadAllUserControlData(bool isViewMode)
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return;
            }
            // 第一頁：申請表/聲明書
            ucSciApplication.IsViewMode= isViewMode;
            ucSciApplication.LoadData(ProjectID);

            // 第二頁：期程／工作項目／查核 (檢視模式)
            ucSciWorkSch.IsViewMode= isViewMode;
            ucSciWorkSch.LoadData(ProjectID);

            // 第三頁：經費／人事 (檢視模式)
            ucSciFunding.IsViewMode= isViewMode;
            ucSciFunding.LoadData(ProjectID);

            // 第四頁：其他 (檢視模式)
            ucSciRecusedList.IsViewMode= isViewMode;
            ucSciRecusedList.LoadData(ProjectID);

            // 第五頁：上傳附件/提送申請 (檢視模式)
            ucSciUploadAttachments.IsViewMode= isViewMode;
            ucSciUploadAttachments.LoadData(ProjectID);

            // 載入並渲染所有 UserControl 的變更說明
            // LoadAllChangeDescriptions();


        }
        catch (Exception ex)
        {
            HandleException(ex, "載入 UserControl 資料時發生錯誤");
        }
    }

    /// <summary>
    /// 設定計畫資訊到 Master Page
    /// </summary>
    private void SetProjectInfoToMaster()
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
            HandleException(ex, "設定計畫資訊時發生錯誤");
        }
    }


    /// <summary>
    /// 載入移轉案件的部門下拉選單
    /// </summary>
    private void LoadDepartmentDropDown()
    {
        try
        {
            // 載入指定的審核單位（用於移轉案件）
            var departmentData = SysUnitHelper.QueryReviewUnits();
            
            // 清空並重新綁定部門下拉選單
            ddlDepartment.Items.Clear();
            ddlDepartment.Items.Add(new ListItem("請選擇部門", ""));
            
            if (departmentData != null && departmentData.Rows.Count > 0)
            {
                foreach (System.Data.DataRow row in departmentData.Rows)
                {
                    string unitID = row["UnitID"].ToString();
                    string unitName = row["UnitName"].ToString();
                    ddlDepartment.Items.Add(new ListItem(unitName, unitID));
                }
            }
            
            // 初始化承辦人員下拉選單
            ddlReviewer.Items.Clear();
            ddlReviewer.Items.Add(new ListItem("請選擇承辦人員", ""));
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入部門下拉選單時發生錯誤");
        }
    }

    /// <summary>
    /// 載入指定部門的承辦人員下拉選單
    /// </summary>
    private void LoadReviewerDropDown(string unitID)
    {
        try
        {
            // 清空承辦人員下拉選單
            ddlReviewer.Items.Clear();
            ddlReviewer.Items.Add(new ListItem("請選擇承辦人員", ""));
            
            if (!string.IsNullOrEmpty(unitID))
            {
                // 根據部門ID載入使用者
                var userData = SysUserHelper.QueryUserByUnitID(unitID);
                
                if (userData != null && userData.Rows.Count > 0)
                {
                    foreach (System.Data.DataRow row in userData.Rows)
                    {
                        string account = row["Account"].ToString();
                        string name = row["Name"].ToString();
                        ddlReviewer.Items.Add(new ListItem(name, account));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入承辦人員下拉選單時發生錯誤");
        }
    }

    /// <summary>
    /// 載入目前的承辦人員資訊
    /// </summary>
    private void LoadCurrentReviewerInfo()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return;
            }

            // 從 OFS_SCI_Project_Main 表中讀取承辦人員資訊（參考 SciApplicationReview.aspx.cs）
            var projectMain = OFS_SciApplicationHelper.getVersionByProjectID(ProjectID);

            if (projectMain != null)
            {
                // 設定承辦人員顯示文字
                if (!string.IsNullOrEmpty(projectMain.SupervisoryPersonName))
                {
                    lblCurrentReviewer.Text = projectMain.SupervisoryPersonName;
                }
                else
                {
                    lblCurrentReviewer.Text = "未設定";
                }
            }
            else
            {
                lblCurrentReviewer.Text = "未設定";
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入承辦人員資訊時發生錯誤");
            lblCurrentReviewer.Text = "載入錯誤";
        }
    }

    /// <summary>
    /// 檢查是否顯示計畫變更審查面板
    /// 顯示條件：
    /// 1. OFS_SCI_Project_Main.IsProjChanged = 2 (計畫變更待審查)
    /// 2. 當前登入用戶帳號 == SupervisoryPersonAccount (承辦人員)
    /// </summary>
    private void CheckAndShowReviewPanel()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return;
            }

            // 取得當前專案資料
            var projectMain = OFS_SciApplicationHelper.getVersionByProjectID(ProjectID);
            if (projectMain == null)
            {
                return;
            }

            // 取得當前登入使用者帳號
            string currentUserAccount = CurrentUser.Account;
            if (string.IsNullOrEmpty(currentUserAccount))
            {
                return;
            }

            // 檢查條件
            bool shouldShowReviewPanel = projectMain.IsProjChanged == 2 &&
                                        !string.IsNullOrEmpty(projectMain.SupervisoryPersonAccount) &&
                                        projectMain.SupervisoryPersonAccount.Equals(currentUserAccount, StringComparison.OrdinalIgnoreCase);

            // 根據條件顯示或隱藏審查面板
            if (shouldShowReviewPanel)
            {
                reviewPanel.Style["display"] = "block";
            }
            else
            {
                reviewPanel.Style["display"] = "none";
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "檢查審查面板顯示條件時發生錯誤");
            reviewPanel.Style["display"] = "none";
        }
    }

    /// <summary>
    /// 顯示 SweetAlert 提示訊息
    /// </summary>
    private void ShowSweetAlert(string title, string text, string icon)
    {
        try
        {
            string script = $@"
                Swal.fire({{
                    title: '{title}',
                    text: '{text}',
                    icon: '{icon}',
                    confirmButtonText: '確定'
                }});
            ";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowAlert", script, true);
        }
        catch (Exception ex)
        {
            HandleException(ex, "顯示提示訊息時發生錯誤");
        }
    }

    /// <summary>
    /// 載入並渲染所有 UserControl 的變更說明
    /// </summary>
    private void LoadAllChangeDescriptions()
    {
        // try
        // {
        //     if (string.IsNullOrEmpty(ProjectID))
        //     {
        //         return;
        //     }
        //
        //     // 定義所有需要處理的 UserControl
        //     var userControls = new[]
        //     {
        //         (control: (Control)ucSciApplication, controlName: "ucSciApplication"),
        //         (control: (Control)ucSciWorkSch, controlName: "ucSciWorkSch"),
        //         (control: (Control)ucSciFunding, controlName: "ucSciFunding"),
        //         (control: (Control)ucSciRecusedList, controlName: "ucSciRecusedList"),
        //         (control: (Control)ucSciUploadAttachments, controlName: "ucSciUploadAttachments")
        //     };
        //
        //     // 遍歷所有 UserControl，取得變更說明並渲染
        //     foreach (var (control, controlName) in userControls)
        //     {
        //         try
        //         {
        //             // 使用反射調用 GetChangeDescriptionData 方法
        //             var (changeBefore, changeAfter) = GetChangeDescriptionData(control);
        //
        //             // 渲染到該 UserControl 的 ucChangeDescription 元件
        //             RenderChangeDescriptionToControl(control, changeBefore, changeAfter, controlName);
        //         }
        //         catch (Exception ex)
        //         {
        //             HandleException(ex, $"載入 {controlName} 的變更說明時發生錯誤");
        //         }
        //     }
        // }
        // catch (Exception ex)
        // {
        //     HandleException(ex, "載入所有變更說明時發生錯誤");
        // }
    }

    /// <summary>
    /// 從 UserControl 取得變更說明資料（透過反射調用 GetChangeDescriptionData 方法）
    /// </summary>
    // private (string changeBefore, string changeAfter) GetChangeDescriptionData(Control userControl)
    // {
    //     try
    //     {
    //         // 使用反射取得 UserControl 的 GetChangeDescriptionData 方法
    //         var method = userControl.GetType().GetMethod("GetChangeDescriptionData");
    //         if (method != null)
    //         {
    //             var result = method.Invoke(userControl, null);
    //             if (result is ValueTuple<string, string> tuple)
    //             {
    //                 return tuple;
    //             }
    //         }
    //         return ("", "");
    //     }
    //     catch (Exception ex)
    //     {
    //         HandleException(ex, $"取得 {userControl.GetType().Name} 變更說明資料時發生錯誤");
    //         return ("", "");
    //     }
    // }

    /// <summary>
    /// 渲染變更說明到 UserControl 的 ucChangeDescription 元件
    /// </summary>
    private void RenderChangeDescriptionToControl(Control userControl, string changeBefore, string changeAfter, string controlName)
    {
        try
        {
            // 根據 controlName 取得對應的 ChangeDescriptionControl ID
            string changeDescriptionControlID = GetChangeDescriptionControlID(controlName);

            // 在 UserControl 中尋找 ChangeDescriptionControl 元件
            Control changeDescriptionControl = userControl.FindControl(changeDescriptionControlID);

            if (changeDescriptionControl != null)
            {
                // 根據 controlName 設定對應的 SourcePage
                string sourcePage = GetSourcePageByControlName(controlName);

                // 使用反射設定 SourcePage 屬性
                var sourcePageProperty = changeDescriptionControl.GetType().GetProperty("SourcePage");
                if (sourcePageProperty != null)
                {
                    sourcePageProperty.SetValue(changeDescriptionControl, sourcePage);
                }

                // 使用反射設定 ChangeBefore 和 ChangeAfter 屬性
                var changeBeforeProperty = changeDescriptionControl.GetType().GetProperty("ChangeBefore");
                var changeAfterProperty = changeDescriptionControl.GetType().GetProperty("ChangeAfter");

                if (changeBeforeProperty != null && changeAfterProperty != null)
                {
                    changeBeforeProperty.SetValue(changeDescriptionControl, changeBefore ?? "");
                    changeAfterProperty.SetValue(changeDescriptionControl, changeAfter ?? "");

                    System.Diagnostics.Debug.WriteLine($"已渲染 {controlName} ({sourcePage}) 的變更說明: Before={changeBefore}, After={changeAfter}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"找不到 ChangeDescriptionControl: {changeDescriptionControlID}");
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, $"渲染 {controlName} 變更說明時發生錯誤");
        }
    }

    /// <summary>
    /// 根據 UserControl 名稱取得對應的 ChangeDescriptionControl ID
    /// </summary>
    private string GetChangeDescriptionControlID(string controlName)
    {
        switch (controlName)
        {
            case "ucSciApplication":
                return "tab1_ucChangeDescription";
            case "ucSciWorkSch":
                return "tab2_ucChangeDescription";
            case "ucSciFunding":
                return "tab3_ucChangeDescription";
            case "ucSciRecusedList":
                return "tab4_ucChangeDescription";
            case "ucSciUploadAttachments":
                return "tab5_ucChangeDescription";
            default:
                return "tab1_ucChangeDescription";
        }
    }

    /// <summary>
    /// 根據 UserControl 名稱取得對應的 SourcePage
    /// </summary>
    private string GetSourcePageByControlName(string controlName)
    {
        switch (controlName)
        {
            case "ucSciApplication":
                return "SciApplication";
            case "ucSciWorkSch":
                return "SciWorkSch";
            case "ucSciFunding":
                return "SciFunding";
            case "ucSciRecusedList":
                return "SciRecusedList";
            case "ucSciUploadAttachments":
                return "SciUploadAttachments";
            default:
                return "SciApplication";
        }
    }

    /// <summary>
    /// PostBack 時重新載入「非當前操作」的 UserControl
    /// </summary>
    private void ReloadNonActiveUserControls()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return;
            }

            // 判斷是哪個按鈕觸發的 PostBack
            // 檢查 Request.Form 中的按鈕 ID
            string activeTab = "";

            // 檢查 tab1 的按鈕
            if (Request.Form[ucSciApplication.FindControl("tab1_btnTempSave")?.UniqueID] != null ||
                Request.Form[ucSciApplication.FindControl("tab1_btnSubmit")?.UniqueID] != null)
            {
                activeTab = "tab1";
            }
            // 檢查 tab2 的按鈕
            else if (Request.Form[ucSciWorkSch.FindControl("tab2_btnTempSave")?.UniqueID] != null ||
                     Request.Form[ucSciWorkSch.FindControl("tab2_btnSaveAndNext")?.UniqueID] != null)
            {
                activeTab = "tab2";
            }
            // 檢查 tab3 的按鈕
            else if (Request.Form[ucSciFunding.FindControl("tab3_btnTempSave")?.UniqueID] != null ||
                     Request.Form[ucSciFunding.FindControl("tab3_btnSaveAndNext")?.UniqueID] != null)
            {
                activeTab = "tab3";
            }
            // 檢查 tab4 的按鈕
            else if (Request.Form[ucSciRecusedList.FindControl("tab4_btnTempSave")?.UniqueID] != null ||
                     Request.Form[ucSciRecusedList.FindControl("tab4_btnNext")?.UniqueID] != null)
            {
                activeTab = "tab4";
            }
            // 檢查 tab5 的按鈕
            else if (Request.Form[ucSciUploadAttachments.FindControl("tab5_btnSave")?.UniqueID] != null ||
                     Request.Form[ucSciUploadAttachments.FindControl("tab5_btnSubmit")?.UniqueID] != null)
            {
                activeTab = "tab5";
            }

            System.Diagnostics.Debug.WriteLine($"PostBack 觸發來源: {activeTab}");

            // 檢查計畫是否處於變更狀態
            bool isViewMode = true;
            var projectMain = OFS_SciApplicationHelper.getVersionByProjectID(ProjectID);
            if (projectMain != null && projectMain.IsProjChanged == 1)
            {
                isViewMode = false;
            }

            // 根據觸發來源，重新載入其他 UserControl
            if (activeTab != "tab1")
            {
                ucSciApplication.IsViewMode = isViewMode;
                ucSciApplication.LoadData(ProjectID);
            }

            if (activeTab != "tab2")
            {
                ucSciWorkSch.IsViewMode = isViewMode;
                ucSciWorkSch.LoadData(ProjectID);
            }

            if (activeTab != "tab3")
            {
                ucSciFunding.IsViewMode = isViewMode;
                ucSciFunding.LoadData(ProjectID);
            }

            if (activeTab != "tab4")
            {
                ucSciRecusedList.IsViewMode = isViewMode;
                ucSciRecusedList.LoadData(ProjectID);
            }

            if (activeTab != "tab5")
            {
                ucSciUploadAttachments.IsViewMode = isViewMode;
                ucSciUploadAttachments.LoadData(ProjectID);
            }

            // 重新載入變更說明
            // LoadAllChangeDescriptions();
        }
        catch (Exception ex)
        {
            HandleException(ex, "重新載入 UserControl 資料時發生錯誤");
        }
    }

    /// <summary>
    /// 例外處理
    /// </summary>
    private void HandleException(Exception ex, string context)
    {
        System.Diagnostics.Debug.WriteLine($"{context}: {ex.Message}");
        // 可以在這裡加入記錄或通知邏輯
    }

    #endregion

    #region WebMethod

    /// <summary>
    /// 提交計畫變更申請
    /// </summary>
    /// <param name="projectID">計畫ID</param>
    /// <param name="changeReason">變更原因</param>
    [WebMethod]
    public static object SubmitProjectChange(string projectID, string changeReason)
    {
        try
        {
            // 驗證參數
            if (string.IsNullOrEmpty(projectID))
            {
                return new { success = false, message = "計畫ID不能為空" };
            }

            if (string.IsNullOrEmpty(changeReason))
            {
                return new { success = false, message = "請填寫變更原因" };
            }

            // 1. 更新 OFS_SCI_Project_Main.IsProjChanged = 1
            OFS_SciApplicationHelper.UpdateIsProjChanged(projectID, 1);

            // 2. 新增 OFS_ProjectChangeRecord 記錄
            var changeRecord = new ProjectChangeRecord
            {
                Type = "SCI",
                Method = 1, // 1: 計畫變更
                DataID = projectID,
                Reason = changeReason,
                Status = 1 // 1: 變更中
            };
            OFSProjectChangeRecordHelper.insert(changeRecord);

            // 3. 更新 OFS_TaskQueue - 將 Change 任務設為待辦
            OFS_TaskQueueHelper.UpdateTaskStatus(projectID, "Change", 1,0);

            // 4. TODO: 進行快照 (未來實作)
            // 這裡需要呼叫快照功能

            return new { success = true, message = "計畫變更申請已提交" };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"提交計畫變更時發生錯誤: {ex.Message}");
            return new { success = false, message = $"系統發生錯誤: {ex.Message}" };
        }
    }

    /// <summary>
    /// 提交計畫變更審查
    /// </summary>
    /// <param name="projectID">計畫ID</param>
    /// <param name="isPass">是否通過</param>
    /// <param name="rejectReason">退回原因（不通過時必填）</param>
    [WebMethod]
    public static object SubmitChangeReview(string projectID, bool isPass, string rejectReason)
    {
        try
        {
            // 驗證參數
            if (string.IsNullOrEmpty(projectID))
            {
                return new { success = false, message = "計畫ID不能為空" };
            }

            if (!isPass && string.IsNullOrEmpty(rejectReason))
            {
                return new { success = false, message = "不通過時請輸入退回原因" };
            }

            if (isPass)
            {
                // === 通過的處理流程 ===
                // 1. 更新 OFS_SCI_Project_Main: IsProjChanged = 0, LastOperation = '已完成計畫變更'
                OFS_SciApplicationHelper.UpdateProjectChangeCompleted(projectID);

                // 2. 更新最新一筆 OFS_ProjectChangeRecord.Status = 3, RejectReason = ""
                OFS_SciApplicationHelper.UpdateProjectChangeRecordStatus(projectID, 3);

                // 3. 刪除 OFS_SCI_PreMonthProgress
                OFS_SciApplicationHelper.DeletePreMonthProgress(projectID);

                // 4. 更新 OFS_TaskQueue: IsCompleted = 1, isToDo = 0
                OFS_TaskQueueHelper.UpdateTaskStatus(projectID, "Change", 0, 1);

                return new { success = true, message = "計畫變更審查通過" };
            }
            else
            {
                // === 不通過的處理流程 ===
                // 1. 更新 OFS_SCI_Project_Main: IsProjChanged = 1
                OFS_SciApplicationHelper.UpdateProjectChangeRejected(projectID);

                // 2. 更新最新一筆 OFS_ProjectChangeRecord.Status = 1, RejectReason = 退回原因
                OFS_SciApplicationHelper.UpdateProjectChangeRecordStatus(projectID, 1, rejectReason);

                return new { success = true, message = "計畫變更審查不通過，已退回申請人修正" };
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"提交計畫變更審查時發生錯誤: {ex.Message}");
            return new { success = false, message = $"系統發生錯誤: {ex.Message}" };
        }
    }

    #endregion
}