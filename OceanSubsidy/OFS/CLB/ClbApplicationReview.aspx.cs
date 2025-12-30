using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Entity.Base;
using GS.App;
using GS.Extension;

/// <summary>
/// 社團計畫申請表審核頁面
/// </summary>
public partial class OFS_CLB_Review_ClbApplicationReview : System.Web.UI.Page
{
    #region 屬性

    /// <summary>
    /// 目前審核的計畫ID
    /// </summary>
    protected string ProjectID => Request.QueryString["ProjectID"];

    /// <summary>
    /// 執行單位名稱（用於風險評估連結）
    /// </summary>
    protected string OrgName { get; set; } = "";

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

                // 檢查是否有移轉成功的訊息
                if (Session["TransferSuccess"] != null)
                {
                    string message = Session["TransferSuccess"].ToString();
                    Session.Remove("TransferSuccess"); // 移除 Session，避免重複顯示
                    ShowSweetAlert("成功", message, "success");
                }
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
    protected void btnConfirmReview_Click(object sender, EventArgs e)
    {
        try
        {
            // 檢查當前使用者是否為指派的審核承辦人員
            if (!ValidateReviewer())
            {
                ShowSweetAlert("錯誤", "您不是此案件的指派審核承辦人員，無法提交審查結果", "error");
                return;
            }

            // 檢查專案狀態是否為「內容審查」且「審核中」
            var projectMain = OFS_ClbApplicationHelper.GetProjectMainData(ProjectID);
            if (projectMain == null)
            {
                ShowSweetAlert("錯誤", "找不到計畫資料", "error");
                return;
            }

            if (projectMain.Statuses != "內容審查" || projectMain.StatusesName != "審核中")
            {
                ShowSweetAlert("錯誤", "此專案當前狀態不允許進行審核，請確認專案狀態是否為「內容審查-審核中」", "error");
                return;
            }

            // 取得審查結果
            string reviewResult = Request.Form["reviewResult"];
            string returnDate = Request.Form["returnDate"];
            string reviewNotes = Request.Form["reviewNotesHidden"];

            // 除錯：檢查取得的值
            System.Diagnostics.Debug.WriteLine($"reviewResult: {reviewResult}");
            System.Diagnostics.Debug.WriteLine($"returnDate: {returnDate}");
            System.Diagnostics.Debug.WriteLine($"reviewNotes: {reviewNotes}");

            if (string.IsNullOrEmpty(reviewResult))
            {
                ShowSweetAlert("錯誤", "請選擇審查結果", "error");
                return;
            }

            if (string.IsNullOrEmpty(ProjectID))
            {
                ShowSweetAlert("錯誤", "找不到計畫ID", "error");
                return;
            }

            // 根據審查結果設定狀態和期限
            string statusesName = "";
            DateTime? expirationDate = null;

            switch (reviewResult)
            {
                case "pass":
                    statusesName = "通過";
                    break;
                case "fail":
                    statusesName = "不通過";
                    break;
                case "return":
                    statusesName = "補正補件";
                    // 使用使用者設定的日期
                    if (!string.IsNullOrEmpty(returnDate))
                    {
                        if (DateTime.TryParse(returnDate, out DateTime parsedDate))
                        {
                            expirationDate = parsedDate;
                        }
                    }
                    break;
                default:
                    ShowSweetAlert("錯誤", "無效的審查結果", "error");
                    return;
            }

            // 取得最新歷史記錄的狀態
            string stageStatusBefore = ApplicationChecklistHelper.GetLatestStageStatus(ProjectID) ?? "";
            
            // 更新審查結果
            OFS_ClbApplicationHelper.UpdateReviewResult(ProjectID, statusesName, reviewNotes,expirationDate);
            
            // 新增案件歷史記錄
            var currentUser = GetCurrentUserInfo();
            var historyLog = new OFS_CaseHistoryLog
            {
                ProjectID = ProjectID,
                ChangeTime = DateTime.Now,
                UserName = currentUser?.UserName ?? "系統",
                StageStatusBefore = stageStatusBefore,
                StageStatusAfter = statusesName,
                Description = $"因{reviewNotes}原因「{statusesName}」" +
                              (reviewResult == "return" && !string.IsNullOrEmpty(returnDate)
                                  ? $"，補正期限：{returnDate}" 
                                  : "")
            };
            
            // 儲存歷史記錄
            ApplicationChecklistHelper.InsertCaseHistoryLog(historyLog);
            var projectBasic = OFS_ClbApplicationHelper.GetBasicData(ProjectID);
            //通過,不通過之寄信  在列表執行
            if (reviewResult == "return")
            {
                NotificationHelper.B2("學校社團", projectBasic.ProjectNameTw, projectBasic.Year.ToString(), reviewNotes, returnDate,
                    projectMain.UserAccount);
            }
           
            
            string script = $@"
                Swal.fire({{
                    title: '成功',
                    text: '審查結果已設定為：{statusesName}',
                    icon: 'success',
                    confirmButtonText: '確定'
                }}).then((result) => {{
                    if (result.isConfirmed) {{
                        window.location.href = '{ResolveUrl("~/OFS/ReviewChecklist.aspx")}';
                    }}
                }});
            ";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowSuccessAndRedirect", script, true);
            
        }
        catch (Exception ex)
        {
            HandleException(ex, "提交審查結果時發生錯誤");
            ShowSweetAlert("錯誤", "提交審查結果時發生錯誤，請稍後再試", "error");
        }
    }

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
            
            
            // 取得案件資料
            var projectMain = OFS_ClbApplicationHelper.GetProjectMainData(ProjectID);
            if (projectMain == null)
            {
                ShowSweetAlert("錯誤", "找不到案件資料", "error");
                return;
            }

            // 從下拉選單取得選中的部門名稱和承辦人員姓名
            string reviewerName = ddlReviewer.SelectedItem.Text;
            string departmentName = ddlDepartment.SelectedItem.Text;
            
            // 承辦人員姓名可能包含帳號，需要提取姓名部分
            if (reviewerName.Contains("(") && reviewerName.Contains(")"))
            {
                reviewerName = reviewerName.Split('(')[0].Trim();
            }
            
            // 只更新承辦人相關的三個欄位
            OFS_ClbApplicationHelper.UpdateProjectSupervisoryInfo(
                ProjectID,
                selectedReviewerAccount,
                reviewerName,
                departmentName
            );

            // 使用 Session 儲存成功訊息，避免重複顯示
            Session["TransferSuccess"] = "案件移轉完成";

            // 重新導向到當前頁面（使用 Response.Redirect 確保是全新的請求）
            Response.Redirect(Request.RawUrl, false);
            Context.ApplicationInstance.CompleteRequest();
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
            // 載入申請資料到 UserControl (檢視模式)
            LoadClbApplicationData();

            // 設定審核者資訊（從資料庫讀取）
            SetReviewerInfoFromDatabase();
            
            // 初始化審查結果面板
            InitializeReviewPanel();
            
            // 載入移轉案件的部門下拉選單
            LoadDepartmentDropDown();
        }
        catch (Exception ex)
        {
            HandleException(ex, "初始化頁面時發生錯誤");
        }
    }

    /// <summary>
    /// 載入社團申請資料
    /// </summary>
    private void LoadClbApplicationData()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return;
            }
            
            // 載入社團申請表資料 (設定 ProjectID 屬性)
            ucClbApplication.ProjectID = ProjectID;
            ucClbApplication.IsReadOnly = true;
            
            
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入社團申請資料時發生錯誤");
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
    /// 初始化審查結果面板
    /// </summary>
    private void InitializeReviewPanel()
    {
        try
        {
            // 取得計畫基本資料
            var projectData = OFS_ClbApplicationHelper.GetBasicData(ProjectID);
            if (projectData == null)
            {
                return;
            }

            string schoolName = projectData.SchoolName ?? "";
            string clubName = projectData.ClubName ?? "";
            int year = projectData.Year ?? 0;

            // 組合執行單位名稱供前端使用（用於風險評估連結）
            OrgName = (schoolName + clubName).Trim();

            // 同單位申請計畫數（使用 count 方法計算並扣掉當前計畫本身）
            int sameUnitCount = OFS_ClbApplicationHelper.count(year, schoolName, clubName);

            // 建立超連結到申請案件清單頁面，帶入年度和單位名稱篩選
            string appRootPath = ConfigurationManager.AppSettings["AppRootPath"] ?? "";
            string encodedOrgName = Server.UrlEncode(OrgName);
            string linkUrl = $"{appRootPath}/OFS/ApplicationChecklist.aspx?year={year}&OrgName={encodedOrgName}";
            lblSameUnitProjectCount.Text = $"<a href='{linkUrl}' class='link-teal fw-bold' target='_blank'>{sameUnitCount}</a>";

            // 使用 GetAuditRecordsByOrgName 取得風險評估記錄
            var auditRecords = AuditRecordsHelper.GetAuditRecordsByOrgName(OrgName);

            // 統計筆數
            int recordCount = auditRecords != null ? auditRecords.Count : 0;
            lblRiskRecordCount.Text = recordCount.ToString();

            // 計算最高風險等級
            string riskLevel = "無";
            int maxRiskLevel = 0;

            if (auditRecords != null && auditRecords.Count > 0)
            {
                foreach (var record in auditRecords)
                {
                    switch (record.Risk)
                    {
                        case "Low":
                            if (maxRiskLevel < 1) maxRiskLevel = 1;
                            break;
                        case "Medium":
                            if (maxRiskLevel < 2) maxRiskLevel = 2;
                            break;
                        case "High":
                            if (maxRiskLevel < 3) maxRiskLevel = 3;
                            break;
                    }
                }

                // 轉換風險等級顯示文字
                switch (maxRiskLevel)
                {
                    case 1:
                        riskLevel = "低風險";
                        break;
                    case 2:
                        riskLevel = "中風險";
                        break;
                    case 3:
                        riskLevel = "高風險";
                        break;
                    default:
                        riskLevel = "無";
                        break;
                }
            }

            lblRiskLevel.Text = riskLevel;

            // 風險評估 Modal 資料（保留原有 Modal 功能）
            lblExecutingUnit.Text = OrgName;
            lblModalRiskLevel.Text = riskLevel;
            lblProjectInfo.Text = $"{ProjectID} / {projectData.ProjectNameTw}";

            // 如果有風險評估記錄，顯示第一筆資料
            if (auditRecords != null && auditRecords.Count > 0)
            {
                var firstRecord = auditRecords[0];
                lblCheckDate.Text = firstRecord.CheckDate?.ToString("yyyy/MM/dd") ?? "";
                lblChecker.Text = firstRecord.ReviewerName ?? "";
                switch (firstRecord.Risk)
                {
                    case "Low":
                        lblTableRiskLevel.Text = "低風險";
                        break;
                    case "Medium":
                        lblTableRiskLevel.Text = "中風險";
                        break;
                    case "High":
                        lblTableRiskLevel.Text = "高風險";
                        break;
                    default:
                        lblTableRiskLevel.Text = "";
                        break;
                }
                lblCheckOpinion.Text = firstRecord.ReviewerComment ?? "";
                lblUnitReply.Text = firstRecord.ExecutorComment ?? "";
            }
            else
            {
                lblCheckDate.Text = "";
                lblChecker.Text = "";
                lblTableRiskLevel.Text = "";
                lblCheckOpinion.Text = "";
                lblUnitReply.Text = "";
            }
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

            // 取得當前使用者資訊
            var currentUser = GetCurrentUserInfo();
            string currentUserAccount = currentUser?.Account ?? "";

            if (projectMain != null && !string.IsNullOrEmpty(projectMain.SupervisoryPersonName))
            {
                lblReviewerName.Text = projectMain.SupervisoryPersonName;

                // 將指派的審核承辦人員帳號存到 HiddenField
                hdnAssignedReviewerAccount.Value = projectMain.SupervisoryPersonAccount ?? "";

                // 設定有指派審核人員的狀態
                ViewState["HasAssignedReviewer"] = true;

                // 判斷當前使用者是否為指派的審核人員
                bool isAssignedReviewer = !string.IsNullOrEmpty(projectMain.SupervisoryPersonAccount) &&
                                          string.Equals(currentUserAccount, projectMain.SupervisoryPersonAccount, StringComparison.OrdinalIgnoreCase);

                // 檢查專案狀態是否為「內容審查」且「審核中」
                bool isCorrectStatus = projectMain.Statuses == "內容審查" && projectMain.StatusesName == "審核中";

                // 只有當前使用者與審核人員相同，且狀態正確時才顯示 scrollBottomPanel
                scrollBottomPanel.Visible = isAssignedReviewer && isCorrectStatus;
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
}