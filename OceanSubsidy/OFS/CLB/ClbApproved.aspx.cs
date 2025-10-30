using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Entity.Base;
using GS.OCA_OceanSubsidy.Operation.OFS;
using GS.App;
using GS.OCA_OceanSubsidy.Model.OFS;

public partial class OFS_CLB_ClbApproved : System.Web.UI.Page
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
        else
        {
            // PostBack 時重新設定審核者資訊
            SetReviewerInfoFromDatabase();
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
                master.ProjectInfoText = $"{ProjectID}/核定計畫";
            }

            // 載入計畫資料
            LoadProjectData();

            // 設定審核者資訊（從資料庫讀取）
            SetReviewerInfoFromDatabase();

            // 載入移轉案件的部門下拉選單
            LoadDepartmentDropDown();

            // 檢查並顯示計畫變更審核面板
            CheckAndShowChangeReviewPanel();

            // 檢查計畫終止按鈕權限
            CheckPlanStopButtonPermission();

            // 檢查移轉案件按鈕權限
            CheckTransferCaseButtonPermission();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"初始化核定計畫頁面時發生錯誤: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 載入計畫資料
    /// </summary>
    private void LoadProjectData()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return;
            }

            // 設定 UserControl 的 ProjectID
            ucClbApplication.ProjectID = ProjectID;

            // 從資料庫檢查計畫變更狀態
            var projectMain = OFS_ClbApplicationHelper.GetProjectMainData(ProjectID);
            bool canEdit = false;

            if (projectMain != null)
            {
                // 只有當 IsProjChanged = 1 (計畫變更中) 時才可以編輯
                canEdit = projectMain.IsProjChanged == 1;
            }

            // 設定 UserControl 的編輯權限
            ucClbApplication.IsReadOnly = !canEdit;


        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"載入計畫資料時發生錯誤: {ex.Message}");
        }
    }
    
    #region 移轉案件功能
    
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
    /// 移轉案件 WebMethod (AJAX 呼叫)
    /// </summary>
    [System.Web.Services.WebMethod]
    public static object TransferProject(string ProjectID, string DepartmentID, string DepartmentName, string ReviewerAccount, string ReviewerName)
    {
        try
        {
            // 後端權限驗證：檢查當前使用者是否有移轉權限
            if (!CheckTransferPermissionStatic(ProjectID))
            {
                return new { success = false, message = "您沒有權限執行此操作" };
            }

            // 驗證必填欄位
            if (string.IsNullOrEmpty(DepartmentID))
            {
                return new { success = false, message = "請選擇部門" };
            }

            if (string.IsNullOrEmpty(ReviewerAccount))
            {
                return new { success = false, message = "請選擇承辦人員" };
            }

            if (string.IsNullOrEmpty(ProjectID))
            {
                return new { success = false, message = "找不到計畫ID" };
            }

            // 取得案件資料
            var projectMain = OFS_ClbApplicationHelper.GetProjectMainData(ProjectID);
            if (projectMain == null)
            {
                return new { success = false, message = "找不到案件資料" };
            }

            // 更新承辦人相關的三個欄位
            OFS_ClbApplicationHelper.UpdateProjectSupervisoryInfo(
                ProjectID,
                ReviewerAccount,
                ReviewerName,
                DepartmentName
            );

            return new { success = true, message = "案件移轉完成" };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"移轉案件時發生錯誤: {ex.Message}");
            return new { success = false, message = "移轉案件時發生錯誤，請稍後再試" };
        }
    }

    /// <summary>
    /// 檢查移轉權限（靜態方法，供 WebMethod 使用）
    /// </summary>
    private static bool CheckTransferPermissionStatic(string projectID)
    {
        try
        {
            if (string.IsNullOrEmpty(projectID))
            {
                return false;
            }

            string currentUserAccount = CurrentUser.Account;

            // 取得計畫資料
            var projectMain = OFS_ClbApplicationHelper.GetProjectMainData(projectID);
            if (projectMain == null)
            {
                return false;
            }

            string supervisorAccount = projectMain.SupervisoryPersonAccount;

            // 檢查 1：是否為承辦人本身
            bool isSupervisor = !string.IsNullOrEmpty(supervisorAccount) &&
                                supervisorAccount.Equals(currentUserAccount, StringComparison.OrdinalIgnoreCase);

            if (isSupervisor)
            {
                return true;
            }

            // 檢查 2：是否為同單位下具有「主管單位窗口」(RoleID = 6) 或「系統管理者」(RoleID = 7) 角色的使用者
            if (!string.IsNullOrEmpty(supervisorAccount))
            {
                List<string> authorizedAccounts = SysUserHelper.GetSameUnitUsersByRoles(
                    supervisorAccount,
                    new List<int> { 6, 7 }  // 主管單位窗口 + 系統管理者
                );

                if (authorizedAccounts != null && authorizedAccounts.Contains(currentUserAccount, StringComparer.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"檢查移轉權限時發生錯誤: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 處理計畫變更申請 WebMethod (AJAX 呼叫)
    /// </summary>
    [System.Web.Services.WebMethod]
    public static object ProcessPlanChange(string ProjectID, string ChangeReason)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrEmpty(ProjectID))
            {
                return new { success = false, message = "找不到計畫ID" };
            }

            if (string.IsNullOrEmpty(ChangeReason))
            {
                return new { success = false, message = "請輸入計畫變更原因" };
            }

            // 取得案件資料
            var projectMain = OFS_ClbApplicationHelper.GetProjectMainData(ProjectID);
            if (projectMain == null)
            {
                return new { success = false, message = "找不到案件資料" };
            }

            // 更新 IsProjChanged 為 1 (計畫變更中)
            OFS_ClbApplicationHelper.UpdateProjectChangeStatus(ProjectID, 1);

            // 插入變更記錄到資料庫
            try
            {
                OFSProjectChangeRecordHelper.insert(new ProjectChangeRecord
                {
                    Type = "CLB",
                    Method = 1,
                    DataID = ProjectID,
                    Reason = ChangeReason
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"插入變更記錄時發生錯誤: {ex.Message}");
                // 不因為插入變更記錄失敗而中斷主要流程，但記錄錯誤
            }

            return new { success = true, message = "計畫變更申請已通過" };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"處理計畫變更申請時發生錯誤: {ex.Message}");
            return new { success = false, message = "處理計畫變更申請時發生錯誤，請稍後再試" };
        }
    }

    #endregion

    #region 計畫變更審核功能

    /// <summary>
    /// 檢查並顯示計畫變更審核面板
    /// </summary>
    private void CheckAndShowChangeReviewPanel()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return;
            }

            // 檢查是否為承辦人員
            var projectMain = OFS_ClbApplicationHelper.GetProjectMainData(ProjectID);
            if (projectMain == null)
            {
                return;
            }

            string currentUserAccount = CurrentUser.Account;
            bool isProjectSupervisor = projectMain.SupervisoryPersonAccount == currentUserAccount;

            if (!isProjectSupervisor)
            {
                return; // 不是承辦人員，不顯示審核面板
            }

            // 檢查計畫變更記錄狀態
            var changeRecord = OFSProjectChangeRecordHelper.getApplying("CLB", 1, ProjectID);
            if (changeRecord == null || changeRecord.Status != 2)
            {
                return; // 沒有狀態為2的變更記錄，不顯示審核面板
            }

            // 顯示審核面板並載入變更原因
            changeReviewPanel.Visible = true;
            lblChangeReason.Text = changeRecord.Reason;
        }
        catch (Exception ex)
        {
            HandleException(ex, "檢查計畫變更審核面板時發生錯誤");
        }
    }

    /// <summary>
    /// 確認計畫變更審核按鈕點擊事件
    /// </summary>
    protected void btnConfirmChangeReview_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                ShowSweetAlert("錯誤", "找不到計畫ID", "error");
                return;
            }

            // 取得審核結果
            string reviewResult = Request.Form["changeReviewResult"];
            string reviewNotes = Request.Form["changeReviewNotesHidden"];

            if (string.IsNullOrEmpty(reviewResult))
            {
                ShowSweetAlert("錯誤", "請選擇審核結果", "error");
                return;
            }

            // 驗證退回修改時必須有審核意見
            if (reviewResult == "reject" && string.IsNullOrEmpty(reviewNotes))
            {
                ShowSweetAlert("錯誤", "退回修改時請輸入審核意見", "error");
                return;
            }

            // 取得計畫變更記錄
            var changeRecord = OFSProjectChangeRecordHelper.getApplying("CLB", 1, ProjectID);
            if (changeRecord == null)
            {
                ShowSweetAlert("錯誤", "找不到計畫變更記錄", "error");
                return;
            }
            var basicData = OFS_ClbApplicationHelper.GetBasicData(ProjectID);
            var projectMainData = OFS_ClbApplicationHelper.GetProjectMainData(ProjectID);

            // 根據審核結果更新狀態
            if (reviewResult == "approve")
            {
                // 通過：更新狀態為3 (已核准)
                changeRecord.Status = 3;
                changeRecord.RejectReason = reviewNotes; // 儲存審核意見

                // 同時更新專案主表的變更狀態為0 (變更完成)
                OFS_ClbApplicationHelper.UpdateProjectChangeStatus(ProjectID, 0);
            }
            else if (reviewResult == "reject")
            {
                // 退回修改：更新狀態為1 (已退回)
                changeRecord.Status = 1; // 保持為1 (可繼續修改)
                changeRecord.RejectReason = reviewNotes;

                // 專案主表的變更狀態保持1 (可繼續修改)
                OFS_ClbApplicationHelper.UpdateProjectChangeStatus(ProjectID, 1);
            }

            // 更新變更記錄
            OFSProjectChangeRecordHelper.update(changeRecord);
            
            // 根據審核結果寄送通知信
            if (reviewResult == "approve")
            {
                
                if (basicData != null && projectMainData != null)
                {
                    string projectName = basicData.ProjectNameTw;
                    string UserAccount = projectMainData.UserAccount;
                    
                    // 寄送通知信
                    NotificationHelper.G4("社團", projectName, "計畫變更申請", UserAccount);
                }
            }
            else if (reviewResult == "reject")
            {
                if (basicData != null && projectMainData != null)
                {
                    string projectName = basicData.ProjectNameTw;
                    string UserAccount = projectMainData.UserAccount;
                    // 寄送通知信
                    NotificationHelper.G3("社團", projectName, "計畫變更申請", reviewNotes,UserAccount);
                }
            }
            // 顯示成功訊息並重新載入頁面
            string successMessage = reviewResult == "approve" ? "計畫變更已通過審核" : "計畫變更已退回修改";
            ShowSweetAlert("成功", successMessage, "success");

            // 隱藏審核面板
            changeReviewPanel.Visible = false;
            string script = $@"
                       Swal.fire({{
                           title: '成功',
                           icon: 'success',
                           text: '{successMessage}',
                           confirmButtonText: '確定'
                       }}).then((result) => {{
                           if (result.isConfirmed) {{
                                // 審核完成後重新整理頁面
                                window.location.href = window.location.href;
                           }}
                       }});
                   ";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowSuccessAndReload", script, true);
        }
        catch (Exception ex)
        {
            HandleException(ex, "確認計畫變更審核時發生錯誤");
            ShowSweetAlert("錯誤", "審核處理時發生錯誤，請稍後再試", "error");
        }
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 載入移轉案件的部門下拉選單
    /// </summary>
    private void LoadDepartmentDropDown()
    {
        try
        {
            // 清空現有項目
            ddlDepartment.Items.Clear();
            
            // 加入預設選項
            ddlDepartment.Items.Add(new ListItem("請選擇部門", ""));
            // 載入指定的審核單位（用於移轉案件）
            var departmentData = SysUnitHelper.QueryReviewUnits();
            
            if (departmentData != null && departmentData.Rows.Count > 0)
            {
                foreach (System.Data.DataRow row in departmentData.Rows)
                {
                    string unitID = row["UnitID"].ToString();
                    string unitName = row["UnitName"].ToString();
                    ddlDepartment.Items.Add(new ListItem(unitName, unitID));
                }
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入部門下拉選單時發生錯誤");
        }
    }

    /// <summary>
    /// 載入承辦人員下拉選單
    /// </summary>
    /// <param name="unitID">部門ID</param>
    private void LoadReviewerDropDown(string unitID)
    {
        try
        {
            // 清空現有項目
            ddlReviewer.Items.Clear();
            
            // 加入預設選項
            ddlReviewer.Items.Add(new ListItem("請選擇承辦人員", ""));
            
            if (string.IsNullOrEmpty(unitID))
            {
                return;
            }
            
            // 從資料庫載入承辦人員資料
            var reviewerData = SysUserHelper.QueryReviewersByUnitID(unitID);
            
            if (reviewerData != null && reviewerData.Rows.Count > 0)
            {
                for (int i = 0; i < reviewerData.Rows.Count; i++)
                {
                    string account = reviewerData.Rows[i]["Account"].ToString();
                    string name = reviewerData.Rows[i]["Name"].ToString();
                    string displayText = $"{name} ({account})";
                    
                    ddlReviewer.Items.Add(new ListItem(displayText, account));
                }
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入承辦人員下拉選單時發生錯誤");
        }
    }
    
    /// <summary>
    /// 從資料庫設定審核者資訊
    /// </summary>
    private void SetReviewerInfoFromDatabase()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return;
            }
            
            // 從 OFS_CLB_Project_Main 表中讀取承辦人員資訊
            var projectMain = OFS_ClbApplicationHelper.GetProjectMainData(ProjectID);
            
            if (projectMain != null && !string.IsNullOrEmpty(projectMain.SupervisoryPersonName))
            {
                // 更新頁面顯示的承辦人員名稱
                lblReviewerName.Text = projectMain.SupervisoryPersonName;
            }
            else
            {
                lblReviewerName.Text = "未分配承辦人員";
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "設定審核者資訊時發生錯誤");
        }
    }
    
 
    
    /// <summary>
    /// 顯示 SweetAlert 訊息
    /// </summary>
    private void ShowSweetAlert(string title, string text, string icon)
    {
        string script = $@"
            Swal.fire({{
                title: '{title}',
                text: '{text}',
                icon: '{icon}',
                confirmButtonText: '確定'
            }});
        ";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowSweetAlert", script, true);
    }

    /// <summary>
    /// 例外處理
    /// </summary>
    private void HandleException(Exception ex, string context)
    {
        System.Diagnostics.Debug.WriteLine($"{context}: {ex.Message}");
        // 可以在這裡加入記錄或通知邏輯
    }

    /// <summary>
    /// 檢查計畫終止按鈕權限
    /// 規則：如果當前使用者不是主管機關，則隱藏計畫終止按鈕
    /// </summary>
    private void CheckPlanStopButtonPermission()
    {
        try
        {
            // 檢查是否為主管機關（主管單位人員、主管單位窗口、系統管理者）
            bool hasPermission = CheckReviewPermission();

            if (hasPermission)
            {
                // 有權限，顯示計畫終止按鈕
                btnPlanStop.Style["display"] = "inline-block";
            }
            else
            {
                // 無權限，隱藏計畫終止按鈕
                btnPlanStop.Style["display"] = "none";
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "檢查計畫終止按鈕權限時發生錯誤");
            // 發生錯誤時，預設隱藏按鈕以確保安全
            btnPlanStop.Style["display"] = "none";
        }
    }

    /// <summary>
    /// 檢查移轉案件按鈕權限
    /// 規則：只有承辦人本身、同單位下具有「主管單位窗口」(RoleID = 6) 角色的使用者、或系統管理者才能使用移轉案件功能
    /// </summary>
    private void CheckTransferCaseButtonPermission()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                // 無計畫ID，隱藏按鈕
                btnTransferProject.Attributes["class"] = btnTransferProject.Attributes["class"] + " d-none";
                return;
            }

            // 檢查權限
            bool hasPermission = CheckTransferPermission();

            if (hasPermission)
            {
                // 有權限，確保移除 d-none（防止 PostBack 時錯誤隱藏）
                string currentClass = btnTransferProject.Attributes["class"] ?? "";
                btnTransferProject.Attributes["class"] = currentClass.Replace("d-none", "").Trim();
            }
            else
            {
                // 無權限，加入 d-none
                string currentClass = btnTransferProject.Attributes["class"] ?? "";
                if (!currentClass.Contains("d-none"))
                {
                    btnTransferProject.Attributes["class"] = currentClass + " d-none";
                }
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "檢查移轉案件按鈕權限時發生錯誤");
            // 發生錯誤時，預設隱藏按鈕以確保安全
            btnTransferProject.Attributes["class"] = btnTransferProject.Attributes["class"] + " d-none";
        }
    }

    /// <summary>
    /// 檢查是否有移轉案件權限（供前端和後端共用）
    /// </summary>
    /// <returns>是否有移轉權限</returns>
    private bool CheckTransferPermission()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return false;
            }

            string currentUserAccount = CurrentUser.Account;

            // 取得計畫資料
            var projectMain = OFS_ClbApplicationHelper.GetProjectMainData(ProjectID);
            if (projectMain == null)
            {
                return false;
            }

            string supervisorAccount = projectMain.SupervisoryPersonAccount;

            // 檢查 1：是否為承辦人本身
            bool isSupervisor = !string.IsNullOrEmpty(supervisorAccount) &&
                                supervisorAccount.Equals(currentUserAccount, StringComparison.OrdinalIgnoreCase);

            if (isSupervisor)
            {
                return true;
            }

            // 檢查 2：是否為同單位下具有「主管單位窗口」(RoleID = 6) 或「系統管理者」(RoleID = 7) 角色的使用者
            if (!string.IsNullOrEmpty(supervisorAccount))
            {
                List<string> authorizedAccounts = SysUserHelper.GetSameUnitUsersByRoles(
                    supervisorAccount,
                    new List<int> { 6, 7 }  // 主管單位窗口 + 系統管理者
                );

                if (authorizedAccounts != null && authorizedAccounts.Contains(currentUserAccount, StringComparer.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            HandleException(ex, "檢查移轉權限時發生錯誤");
            return false;
        }
    }

    /// <summary>
    /// 檢查審核權限
    /// </summary>
    /// <returns>是否有審核權限</returns>
    private bool CheckReviewPermission()
    {
        try
        {
            // 取得當前使用者資訊
            var currentUser = GetCurrentUserInfo();
            if (currentUser == null || currentUser.OFS_RoleName == null)
            {
                return false;
            }

            // 檢查是否為審核相關角色
            var reviewRoles = new[] { "主管單位人員", "主管單位窗口", "系統管理者" };

            foreach (string roleName in currentUser.OFS_RoleName)
            {
                if (!string.IsNullOrEmpty(roleName) && reviewRoles.Contains(roleName))
                {
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            HandleException(ex, "檢查審核權限時發生錯誤");
            return false;
        }
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
            HandleException(ex, "取得使用者資訊時發生錯誤");
            return null;
        }
    }

    /// <summary>
    /// 取得計畫的已撥款金額
    /// </summary>
    /// <param name="projectID">計畫ID</param>
    [System.Web.Services.WebMethod]
    public static object GetPaidAmount(string projectID)
    {
        try
        {
            // 驗證參數
            if (string.IsNullOrEmpty(projectID))
            {
                return new { success = false, message = "計畫ID不能為空", paidAmount = 0 };
            }

            // 呼叫 Helper 查詢總撥款金額
            decimal totalPaidAmount = OFS_ClbPaymentHelper.GetTotalPaidAmount(projectID);

            return new { success = true, paidAmount = totalPaidAmount };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得已撥款金額時發生錯誤: {ex.Message}");
            return new { success = false, message = $"系統發生錯誤: {ex.Message}", paidAmount = 0 };
        }
    }

    /// <summary>
    /// 提交計畫終止
    /// </summary>
    /// <param name="projectID">計畫ID</param>
    /// <param name="stopReason">終止原因</param>
    /// <param name="recoveredAmount">已追回金額</param>
    [System.Web.Services.WebMethod]
    public static object SubmitPlanStop(string projectID, string stopReason, decimal recoveredAmount)
    {
        try
        {
            // 驗證參數
            if (string.IsNullOrEmpty(projectID))
            {
                return new { success = false, message = "計畫ID不能為空" };
            }

            if (string.IsNullOrEmpty(stopReason))
            {
                return new { success = false, message = "請填寫計畫終止原因" };
            }

            if (recoveredAmount < 0)
            {
                return new { success = false, message = "已追回金額不能為負數" };
            }

            // 更新計畫的追回金額到 OFS_CLB_Project_Main.RecoveryAmount
            OFS_ClbApplicationHelper.UpdateRecoveryAmount(projectID, recoveredAmount);

            return new { success = true, message = "計畫終止資料已儲存" };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"提交計畫終止時發生錯誤: {ex.Message}");
            return new { success = false, message = $"系統發生錯誤: {ex.Message}" };
        }
    }


    #endregion
}