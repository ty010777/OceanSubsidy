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

/// <summary>
/// 科專計畫申請表審核頁面
/// 展示如何重複使用 UserControl 來建立審核介面
/// </summary>
public partial class OFS_SCI_Review_SciApplicationReview : System.Web.UI.Page
{
    #region 屬性

    /// <summary>
    /// 目前審核的計畫ID
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
                Response.Redirect("~/OFS/ReviewChecklist.aspx");
                return;
            }

            // 檢查使用者權限
            if (!CheckReviewPermission())
            {
                ShowErrorAndRedirect("您沒有審核此計畫的權限");
                return;
            }

            if (!IsPostBack)
            {
                // 初始化頁面
                InitializePage();
            }
            else
            {
            // PostBack 時重新設定審核者資訊
                SetReviewerInfoFromDatabase();
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
    /// 確認審查結果
    /// </summary>
    // protected void btnConfirmReview_Click(object sender, EventArgs e)
    // {
    //     try
    //     {
    //         // 檢查當前使用者是否為指派的審核承辦人員
    //         if (!ValidateReviewer())
    //         {
    //             ShowSweetAlert("錯誤", "您不是此案件的指派審核承辦人員，無法提交審查結果", "error");
    //             return;
    //         }
    //
    //         // 取得審查結果
    //         string reviewResult = Request.Form["reviewResult"];
    //         string returnDate = Request.Form["returnDate"];
    //         string reviewNotes = Request.Form["reviewNotesHidden"];
    //
    //         if (string.IsNullOrEmpty(reviewResult))
    //         {
    //             ShowSweetAlert("錯誤", "請選擇審查結果", "error");
    //             return;
    //         }
    //
    //         if (string.IsNullOrEmpty(ProjectID))
    //         {
    //             ShowSweetAlert("錯誤", "找不到計畫ID", "error");
    //             return;
    //         }
    //
    //         // 建立更新物件
    //         var projectMain = new OFS_SCI_Project_Main
    //         {
    //             ProjectID = ProjectID,
    //             updated_at = DateTime.Now
    //         };
    //
    //         // 根據審查結果設定狀態
    //         switch (reviewResult)
    //         {
    //             case "pass":
    //                 projectMain.StatusesName = "通過";
    //                 break;
    //             case "fail":
    //                 projectMain.StatusesName = "不通過";
    //                 break;
    //             case "return":
    //                 projectMain.StatusesName = "補正補件";
    //                 // 使用使用者設定的日期
    //                 if (!string.IsNullOrEmpty(returnDate))
    //                 {
    //                     if (DateTime.TryParse(returnDate, out DateTime expirationDate))
    //                     {
    //                         projectMain.ExpirationDate = expirationDate;
    //                     }
    //                 }
    //                 break;
    //             default:
    //                 ShowSweetAlert("錯誤", "無效的審查結果", "error");
    //                 return;
    //         }
    //
    //         // 取得最新歷史記錄的狀態
    //         string stageStatusBefore = ApplicationChecklistHelper.GetLatestStageStatus(ProjectID) ?? "";
    //         
    //         // 更新資料庫
    //         OFS_SciApplicationHelper.UpdateOFS_SCIVersion(projectMain);
    //         
    //         // 新增案件歷史記錄
    //         var currentUser = GetCurrentUserInfo();
    //         var historyLog = new OFS_CaseHistoryLog
    //         {
    //             ProjectID = ProjectID,
    //             ChangeTime = DateTime.Now,
    //             UserName = currentUser?.UserName ?? "系統",
    //             StageStatusBefore = stageStatusBefore,
    //             StageStatusAfter =  projectMain.Statuses + projectMain.StatusesName,
    //             Description = $"因{reviewNotes}原因「{projectMain.StatusesName}」" +
    //                           (reviewResult == "return" && !string.IsNullOrEmpty(returnDate)
    //                               ? $"，補正期限：{returnDate}" 
    //                               : "")
    //         };
    //         
    //         // 儲存歷史記錄
    //         ApplicationChecklistHelper.InsertCaseHistoryLog(historyLog);
    //         
    //         // 送出成功後直接跳轉，避免 PostBack 問題
    //         string script = $@"
    //             Swal.fire({{
    //                 title: '成功',
    //                 text: '審查結果已設定為：{projectMain.StatusesName}',
    //                 icon: 'success',
    //                 confirmButtonText: '確定'
    //             }}).then((result) => {{
    //                 if (result.isConfirmed) {{
    //                     window.location.href = '{ResolveUrl("~/OFS/ReviewChecklist.aspx")}';
    //                 }}
    //             }});
    //         ";
    //         Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowSuccessAndRedirect", script, true);
    //     }
    //     catch (Exception ex)
    //     {
    //         HandleException(ex, "提交審查結果時發生錯誤");
    //         ShowSweetAlert("錯誤", "提交審查結果時發生錯誤，請稍後再試", "error");
    //     }
    // }

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
            
            // 更新頁面上的審核者資訊顯示
            UpdateReviewerInfoDisplay(reviewerName);
            
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
            
            SetReviewerInfoFromDatabase();
            LoadAllUserControlData();
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
            // 載入所有申請資料到 UserControl (檢視模式)
            LoadAllUserControlData();

            // 設定審核者資訊（從資料庫讀取）
            SetReviewerInfoFromDatabase();
            
            // 初始化審查結果面板
            InitializeReviewPanel();
            
            // 載入移轉案件的部門下拉選單
            LoadDepartmentDropDown();
            
            // 設定 Master Page 的進度條狀態
            InitializeReviewSteps();
        }
        catch (Exception ex)
        {
            HandleException(ex, "初始化頁面時發生錯誤");
        }
    }
    
    /// <summary>
    /// 初始化審核進度條
    /// </summary>
    private void InitializeReviewSteps()
    {
        try
        {
            // 取得 Master Page 參考
            var masterPage = this.Master as OFS_SCI_OFSReviewMaster;
            if (masterPage != null)
            {
                // 設定第一步為活躍狀態
                masterPage.SetActiveStep(1);
                
                // 可以根據審核進度設定其他步驟狀態
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "初始化審核進度條時發生錯誤");
        }
    }

    /// <summary>
    /// 載入所有 UserControl 資料
    /// </summary>
    private void LoadAllUserControlData()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return;
            }
            
            // 第一頁：申請表/聲明書
            ucSciApplication.LoadData(ProjectID, isViewMode: true);
            
            // 第二頁：期程及工作項目
            ucSciWorkSch.LoadData(ProjectID, isViewMode: true);
            
            // 第三頁：經費/人事費明細
            ucSciFunding.LoadData(ProjectID, isViewMode: true);
            
            // 第四頁：委員迴避清單
            ucSciRecusedList.LoadData(ProjectID, isViewMode: true);
            
            // 第五頁：上傳附件
            ucSciUploadAttachments.LoadData(ProjectID, isViewMode: true);
            
            // 註冊 JavaScript，在所有資料載入完成後重新應用檢視模式
            RegisterViewModeScript();
            
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入 UserControl 資料時發生錯誤");
        }
    }

    /// <summary>
    /// 註冊檢視模式 JavaScript，在所有動態內容載入後執行
    /// </summary>
    private void RegisterViewModeScript()
    {
        // 使用 JavaScript 添加檢視模式的 CSS 類別並禁用輸入控制項
        string script = @"
            document.addEventListener('DOMContentLoaded', function() {
                document.body.classList.add('app-mode-view');

                // 禁用所有表單元素，但跳過 UserInfo div 內的元素
                var formElements = document.querySelectorAll('input, textarea, select');
                formElements.forEach(function(element) {
                    // 檢查元素是否在 UserInfo div 內
                    if (!element.closest('#UserInfo, #FuncBtn,#btnTransferProject,#transferCaseModal,#BodyContent_MainContent_ApplicationContent_scrollBottomPanel')) {
                        element.disabled = true;
                    }
                });

                // 將所有有 view-mode class 的元件加上 d-none class
                var viewModeElements = document.querySelectorAll('.view-mode');
                viewModeElements.forEach(function(element) {
                    element.classList.add('d-none');
                });

                // 特別處理一些可能動態生成的元素
                setTimeout(function() {
                    var dynamicElements = document.querySelectorAll('input ,textarea, select');
                    dynamicElements.forEach(function(element) {
                        // 檢查元素是否在 UserInfo div 內且尚未被禁用
                        if (!element.disabled && !element.closest('#UserInfo, #FuncBtn,#btnTransferProject,#transferCaseModal,#BodyContent_MainContent_ApplicationContent_scrollBottomPanel')) {
                            element.disabled = true;
                        }
                    });

                    // 再次處理可能動態生成的 view-mode 元素
                    var dynamicViewModeElements = document.querySelectorAll('.view-mode');
                    dynamicViewModeElements.forEach(function(element) {
                        if (!element.classList.contains('d-none')) {
                            element.classList.add('d-none');
                        }
                    });
                }, 1000);
            });
        ";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "AddViewModeStyles", script, true);
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
            var reviewRoles = new[] { "主管單位人員", "主管單位窗口", "系統管理者", "審查委員" };
            
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
    /// 設定審核者資訊
    /// </summary>
    
    /// <summary>
    /// 初始化審查結果面板
    /// </summary>
    private void InitializeReviewPanel()
    {
        try
        {
            // TODO: 從資料庫載入實際資料
            
            // 同單位申請計畫數
            lblSameUnitProjectCount.Text = "0";
            
            // 風險評估資訊
            lblRiskLevel.Text = "低風險";
            lblRiskRecordCount.Text = "0";
            
            // 風險評估 Modal 資料
            lblExecutingUnit.Text = "海洋委員會科技文教處科技科";
            lblModalRiskLevel.Text = "低風險";
            lblProjectInfo.Text = $"{ProjectID} / 計畫名稱";
            lblCheckDate.Text = DateTime.Now.ToString("yyyy/MM/dd");
            lblChecker.Text = "審查人員";
            lblTableRiskLevel.Text = "低風險";
            lblCheckOpinion.Text = "";
            lblUnitReply.Text = "";
        }
        catch (Exception ex)
        {
            HandleException(ex, "初始化審查結果面板時發生錯誤");
        }
    }

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
            
            // 從資料庫載入部門資料
            var departmentData = SysUnitHelper.QueryReviewUnits();
            
            if (departmentData != null && departmentData.Rows.Count > 0)
            {
                for (int i = 0; i < departmentData.Rows.Count; i++)
                {
                    string unitID = departmentData.Rows[i]["UnitID"].ToString();
                    string unitName = departmentData.Rows[i]["UnitName"].ToString();
                    
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
            
            // 從 OFS_SCI_Project_Main 表中讀取承辦人員資訊
            var projectMain = OFS_SciApplicationHelper.getVersionByProjectID(ProjectID);
            
            
            if (projectMain != null && !string.IsNullOrEmpty(projectMain.SupervisoryPersonName))
            {
                lblReviewerName.Text = projectMain.SupervisoryPersonName;
                
                // 將指派的審核承辦人員帳號存到 HiddenField
                hdnAssignedReviewerAccount.Value = projectMain.SupervisoryPersonAccount ?? "";
                
                // 設定有指派審核人員的狀態
                ViewState["HasAssignedReviewer"] = true;
                scrollBottomPanel.Visible = true;
            }
            else
            {
                lblReviewerName.Text = "未分配承辦人員";
                h4TranProject.InnerHtml = "認領案件";
                btnTransferProject.InnerHtml = "認領案件";
                // 清空 HiddenField
                hdnAssignedReviewerAccount.Value = "";
                // 設定沒有指派審核人員的狀態
                ViewState["HasAssignedReviewer"] = false;
                scrollBottomPanel.Visible = false;
            }
            
        }
        catch (Exception ex)
        {
            HandleException(ex, "設定審核者資訊時發生錯誤");
            lblReviewerName.Text = "載入失敗";
        }
    }

    /// <summary>
    /// 更新審核者資訊顯示
    /// </summary>
    /// <param name="reviewerName">新的審核者姓名</param>
    private void UpdateReviewerInfoDisplay(string reviewerName)
    {
        try
        {
            lblReviewerName.Text = reviewerName ?? "未知使用者";
        }
        catch (Exception ex)
        {
            HandleException(ex, "更新審核者資訊時發生錯誤");
        }
    }

    /// <summary>
    /// 驗證當前使用者是否為指派的審核承辦人員
    /// </summary>
    /// <returns>是否為指派的審核承辦人員</returns>
    private bool ValidateReviewer()
    {
        try
        {
            // 取得當前使用者資訊
            var currentUser = GetCurrentUserInfo();
            if (currentUser == null || string.IsNullOrEmpty(currentUser.Account))
            {
                return false;
            }

            // 從 HiddenField 取得指派的審核承辦人員帳號
            string assignedReviewerAccount = hdnAssignedReviewerAccount.Value;
            if (string.IsNullOrEmpty(assignedReviewerAccount))
            {
                return false;
            }

            // 比較當前使用者帳號與指派的審核承辦人員帳號
            return string.Equals(currentUser.Account, assignedReviewerAccount, StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            HandleException(ex, "驗證審核權限時發生錯誤");
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
    /// 顯示錯誤訊息並跳轉
    /// </summary>
    private void ShowErrorAndRedirect(string message)
    {
        string script = $@"
            Swal.fire({{
                title: '存取權限：{message}',
                icon: 'error',
                confirmButtonText: '確定'
            }}).then((result) => {{
                if (result.isConfirmed) {{
                    window.location.href = '~/OFS/ReviewChecklist.aspx';
                }}
            }});
        ";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowErrorAndRedirect", script, true);
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
    /// AJAX 確認審查結果
    /// </summary>
    [System.Web.Services.WebMethod]
    public static object ConfirmReview(string reviewResult, string returnDate, string reviewNotes, string projectId)
    {
        try
        {
            // 檢查參數
            if (string.IsNullOrEmpty(reviewResult) || string.IsNullOrEmpty(projectId))
            {
                return new { success = false, message = "參數不完整" };
            }

            // 取得當前使用者資訊
            var currentUser = SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo);
            if (currentUser == null)
            {
                return new { success = false, message = "使用者未登入" };
            }

            // 驗證審核權限
            var projectMain = OFS_SciApplicationHelper.getVersionByProjectID(projectId);
            if (projectMain == null)
            {
                return new { success = false, message = "找不到計畫資料" };
            }

            // 檢查是否為指派的審核承辦人員
            if (!string.IsNullOrEmpty(projectMain.SupervisoryPersonAccount) &&
                !string.Equals(currentUser.Account, projectMain.SupervisoryPersonAccount, StringComparison.OrdinalIgnoreCase))
            {
                return new { success = false, message = "您不是此案件的指派審核承辦人員，無法提交審查結果" };
            }

            // 建立更新物件
            var updateProjectMain = new OFS_SCI_Project_Main
            {
                ProjectID = projectId,
                updated_at = DateTime.Now
            };

            // 根據審查結果設定狀態
            switch (reviewResult)
            {
                case "pass":
                    updateProjectMain.StatusesName = "通過";
                    updateProjectMain.QualReviewNotes = "";
                    break;
                case "fail":
                    updateProjectMain.StatusesName = "不通過";
                    updateProjectMain.QualReviewNotes = reviewNotes;
                    break;
                case "return":
                    updateProjectMain.StatusesName = "補正補件";
                    updateProjectMain.QualReviewNotes = reviewNotes;
                    // 使用使用者設定的日期
                    if (!string.IsNullOrEmpty(returnDate))
                    {
                        if (DateTime.TryParse(returnDate, out DateTime expirationDate))
                        {
                            updateProjectMain.ExpirationDate = expirationDate;
                        }
                    }
                    break;
                default:
                    return new { success = false, message = "無效的審查結果" };
            }

            // 取得最新歷史記錄的狀態
            string stageStatusBefore = ApplicationChecklistHelper.GetLatestStageStatus(projectId) ?? "";

            // 更新資料庫
            OFS_SciApplicationHelper.UpdateOFS_SCIVersion(updateProjectMain);

            // 新增案件歷史記錄
            var historyLog = new OFS_CaseHistoryLog
            {
                ProjectID = projectId,
                ChangeTime = DateTime.Now,
                UserName = currentUser?.UserName ?? "系統",
                StageStatusBefore = stageStatusBefore,
                StageStatusAfter = updateProjectMain.Statuses + updateProjectMain.StatusesName,
                Description = $"因{reviewNotes}原因「{updateProjectMain.StatusesName}」" +
                              (reviewResult == "return" && !string.IsNullOrEmpty(returnDate)
                                  ? $"，補正期限：{returnDate}"
                                  : "")
            };

            // 儲存歷史記錄
            ApplicationChecklistHelper.InsertCaseHistoryLog(historyLog);
            //TODO 寄信通知
            return new { success = true, message = $"審查結果已設定為：{updateProjectMain.StatusesName}" };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ConfirmReview WebMethod 錯誤: {ex.Message}");
            return new { success = false, message = "處理審查結果時發生錯誤，請稍後再試" };
        }
    }

    #endregion
}