using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.IO;
using GS.Extension;
using GS.App;
using GS.Data;
using GS.OCA_OceanSubsidy.Entity;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

public partial class OFS_AuditRecords : System.Web.UI.Page
{
    protected string ProjectID { get; set; }
    protected AuditRecordsModel.UserPermissionType CurrentUserPermission { get; set; }
    protected AuditRecordsModel.ProjectBasicData ProjectData { get; set; }
    protected List<AuditRecordsModel.AuditRecordData> AuditRecords { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // 檢查是否有計畫ID
            // if (string.IsNullOrEmpty(Request.QueryString["ProjectID"]))
            // {
            //     Response.Redirect("~/OFS/inprogressList.aspx");
            //     return;
            // }

            // 檢查使用者是否有權限訪問此頁面
            // if (!CheckAuditPermission())
            // {
            //     // ShowErrorAndRedirect("您沒有訪問此頁面的權限");
            //     // return;
            // }

            if (!IsPostBack)
            {
                InitializePageData();
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "頁面載入時發生錯誤");
        }
    }

    private void InitializePageData()
    {
        // 從 URL 參數取得 ProjectID
        ProjectID = Request.QueryString["ProjectID"] ?? "";

        if (string.IsNullOrEmpty(ProjectID))
        {
            ShowErrorMessage("未指定計畫編號");
            return;
        }

        // 取得當前用戶權限
        CurrentUserPermission = GetCurrentUserPermission();
        // CurrentUserPermission = AuditRecordsModel.UserPermissionType.GeneralUser;

        // 載入計畫基本資料
        LoadProjectData();

        // 載入查核紀錄
        LoadAuditRecords();

        // 根據權限控制頁面元素顯示
        SetupPermissionControl();
    }

    /// <summary>
    /// 檢查查核權限
    /// </summary>
    /// <returns>是否有查核權限</returns>
    private bool CheckAuditPermission()
    {
        try
        {
            // 取得當前使用者資訊
            var currentUser = GetCurrentUserInfo();
            if (currentUser == null || currentUser.OFS_RoleName == null)
            {
                return false;
            }

            // 檢查是否有查核相關權限的角色
            // 主管單位和系統管理員可以進行查核作業
            // 一般用戶可以查看和回覆執行單位意見
            var allowedRoles = new[] { "主管單位人員", "主管單位窗口", "系統管理者", "申請單位人員", "執行單位人員" };

            foreach (string roleName in currentUser.OFS_RoleName)
            {
                if (!string.IsNullOrEmpty(roleName) && allowedRoles.Contains(roleName))
                {
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            HandleException(ex, "檢查查核權限時發生錯誤");
            return false;
        }
    }

    /// <summary>
    /// 取得當前使用者資訊
    /// </summary>
    /// <returns>使用者資訊</returns>
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


    private void LoadProjectData()
    {
        try
        {
            GisTable projectTable;
            if (ProjectID.Contains("SCI"))
            {
                projectTable = AuditRecordsHelper.SCI_GetProjectBasicData(ProjectID);
            }
            else if (ProjectID.Contains("CLB"))
            {
                projectTable = AuditRecordsHelper.CLB_GetProjectBasicData(ProjectID);
            }
            else
            {
                projectTable = AuditRecordsHelper.Other_GetProjectBasicData(ProjectID);
            }
            if (projectTable.Rows.Count == 0)
            {
                ShowErrorMessage("找不到指定的計畫資料");
                return;
            }

            // 將 GisTable 轉換為 ProjectBasicData
            var row = projectTable.Rows[0];
            ProjectData = new AuditRecordsModel.ProjectBasicData
            {
                ProjectID = row["ProjectID"]?.ToString() ?? "",
                ProjectNameTw = row["ProjectNameTw"]?.ToString() ?? "",
                OrgName = row["OrgName"]?.ToString() ?? "",
                StartTime = row["StartTime"] as DateTime?,
                EndTime = row["EndTime"] as DateTime?
            };

            // 綁定計畫基本資料到頁面
            BindProjectDataToPage();
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入計畫資料時發生錯誤");
        }
    }

    private void BindProjectDataToPage()
    {
        if (ProjectData == null) return;

        // 使用 Literal 控件來顯示基本資料
        var projectInfoHtml = new StringBuilder();

        projectInfoHtml.AppendLine($"<li><span class=\"text-gray fw-bold\">計畫編號：</span><span>{ProjectData.ProjectID}</span></li>");
        projectInfoHtml.AppendLine($"<li><span class=\"text-gray fw-bold\">計畫名稱：</span><span>{ProjectData.ProjectNameTw}</span></li>");
        projectInfoHtml.AppendLine($"<li><span class=\"text-gray fw-bold\">執行單位：</span><span>{ProjectData.OrgName}</span></li>");

        string periodText = "";
        if (ProjectData.StartTime.HasValue && ProjectData.EndTime.HasValue)
        {
            periodText = $"{ProjectData.StartTime.Value.ToMinguoDate()} - {ProjectData.EndTime.Value.ToMinguoDate()}";
        }
        projectInfoHtml.AppendLine($"<li><span class=\"text-gray fw-bold\">計畫期程：</span><span>{periodText}</span></li>");

        // 將基本資料放入 Literal 控件中
        litProjectInfo.Text = projectInfoHtml.ToString();
    }

    private void LoadAuditRecords()
    {
        try
        {
            var auditTable = AuditRecordsHelper.GetAuditRecordsByProjectID(ProjectID);

            AuditRecords = new List<AuditRecordsModel.AuditRecordData>();

            foreach (System.Data.DataRow row in auditTable.Rows)
            {
                var record = new AuditRecordsModel.AuditRecordData
                {
                    idx = Convert.ToInt32(row["idx"]),
                    ProjectID = row["ProjectID"]?.ToString() ?? "",
                    CheckDate = row["CheckDate"] as DateTime?,
                    ReviewerComment = row["ReviewerComment"]?.ToString() ?? "",
                    ExecutorComment = row["ExecutorComment"]?.ToString() ?? "",
                    create_at = row["create_at"] as DateTime?,
                    update_at = row["update_at"] as DateTime?
                };

                // 根據權限決定是否顯示查核人員和風險評估
                if (CurrentUserPermission == AuditRecordsModel.UserPermissionType.Administrator)
                {
                    record.ReviewerName = row["ReviewerName"]?.ToString() ?? "";
                    record.Risk = row["Risk"]?.ToString() ?? "";
                }
                else
                {
                    record.ReviewerName = "-";
                    record.Risk = "-";
                }

                // 判斷是否可編輯執行單位回覆
                // Administrator 權限：一律顯示純文字
                // GeneralUser 權限：空值或null才可編輯
                if (CurrentUserPermission == AuditRecordsModel.UserPermissionType.Administrator)
                {
                    record.CanEditExecutorComment = false; // Administrator 一律不可編輯，顯示純文字
                }
                else
                {
                    record.CanEditExecutorComment = string.IsNullOrEmpty(record.ExecutorComment); // GeneralUser 空值才可編輯
                }

                AuditRecords.Add(record);
            }

            BindAuditRecordsToRepeater();
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入查核紀錄時發生錯誤");
        }
    }

    private void BindAuditRecordsToRepeater()
    {
        rptAuditRecords.DataSource = AuditRecords;
        rptAuditRecords.DataBind();
    }

    private void SetupPermissionControl()
    {
        // 根據權限控制按鈕和欄位顯示
        if (CurrentUserPermission == AuditRecordsModel.UserPermissionType.Administrator)
        {
            // 管理員權限：顯示完整查核作業功能
            btnExportRecords.Visible = true;
            btnSubmitAuditResult.Visible = true;
            btnSubmitReply.Visible = false;

            // 顯示查核作業欄位
            txtAuditorName.Visible = true;
            txtAuditDate.Visible = true;
            ddlRiskAssessment.Visible = true;
            txtAuditComment.Visible = true;
        }
        else
        {
            // 一般用戶權限：只能回覆執行單位意見
            btnExportRecords.Visible = false;
            btnSubmitAuditResult.Visible = false;
            btnSubmitReply.Visible = true;

            // 隱藏查核作業欄位
            txtAuditorName.Visible = false;
            txtAuditDate.Visible = false;
            ddlRiskAssessment.Visible = false;
            txtAuditComment.Visible = false;

            // 添加 CSS 類別來隱藏 General-view 元素
            SetGeneralViewElementsHidden();

            // 為查核紀錄表格添加隱藏欄位的 CSS 類別
            SetRecordsTableColumnHidden();
        }
    }

    /// <summary>
    /// 為 General-view 元素添加 d-none 類別
    /// </summary>
    private void SetGeneralViewElementsHidden()
    {
        // 使用 ClientScript 來添加 CSS 類別
        string script = @"
            $(document).ready(function() {
                $('.General-view').addClass('d-none');
            });
        ";
        ClientScript.RegisterStartupScript(this.GetType(), "HideGeneralViewElements", script, true);
    }

    /// <summary>
    /// 為查核紀錄表格添加隱藏欄位的 CSS 類別
    /// </summary>
    private void SetRecordsTableColumnHidden()
    {
        // 使用 ClientScript 來添加 CSS 類別
        string script = @"
            $(document).ready(function() {
                $('#RecordsTable').addClass('hide-col-2 hide-col-3');
            });
        ";
        ClientScript.RegisterStartupScript(this.GetType(), "HideRecordsTableColumns", script, true);
    }

    protected void btnSubmitAuditResult_Click(object sender, EventArgs e)
    {
         string ProjectID=  Request.QueryString["ProjectID"];
        CurrentUserPermission = GetCurrentUserPermission();
        if (CurrentUserPermission != AuditRecordsModel.UserPermissionType.Administrator)
        {
            ShowErrorMessage("權限不足");
            return;
        }

        try
        {
            // 取得表單資料
            string auditorName = txtAuditorName.Text.Trim();
            string auditDateStr = Request.Form[txtAuditDate.UniqueID + "_gregorian"] ?? txtAuditDate.Attributes["data-gregorian-date"];
            string risk = ddlRiskAssessment.SelectedValue;
            string comment = txtAuditComment.Text.Trim();

            // 驗證必填欄位
            if (string.IsNullOrEmpty(auditorName) || string.IsNullOrEmpty(auditDateStr) ||
                string.IsNullOrEmpty(risk) || string.IsNullOrEmpty(comment))
            {
                ShowErrorMessage("請填寫所有必填欄位");
                return;
            }

            DateTime auditDate;
            if (!DateTime.TryParse(auditDateStr, out auditDate))
            {
                ShowErrorMessage("查核日期格式錯誤");
                return;
            }

            AuditRecordsHelper.InsertAuditRecord(ProjectID, auditorName, auditDate, risk, comment);

            // 寄送通知信
            SendAuditNotification(ProjectID);

            // 顯示成功訊息並延遲重新載入頁面
            string script = @"
                Swal.fire({
                    icon: 'success',
                    title: '成功',
                    text: '查核結果提送成功',
                    confirmButtonText: '確定',
                    timer: 2000,
                    timerProgressBar: true
                }).then(function() {
                    window.location.href = window.location.href;
                });";
            ClientScript.RegisterStartupScript(this.GetType(), "SuccessAndReload", script, true);

        }
        catch (Exception ex)
        {
            ShowErrorMessage("系統錯誤：" + ex.Message);
        }
    }

    /// <summary>
    /// 寄送查核通知信
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    private void SendAuditNotification(string projectID)
    {
        try
        {
            string category = "";
            string projectName = "";
            string supervisoryAccount = "";
            string applicantAccount = "";
            int? organizer = null;

            // 根據 ProjectID 判斷計畫類型並取得相關資訊
            if (projectID.Contains("SCI"))
            {
                category = "科專";
                var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);
                var projectMain = OFS_SciApplicationHelper.getVersionByProjectID(projectID);

                if (applicationMain != null)
                {
                    projectName = applicationMain.ProjectNameTw;
                    applicantAccount = projectMain.UserAccount;
                }

                if (projectMain != null)
                {
                    supervisoryAccount = projectMain.SupervisoryPersonAccount;
                    organizer = SysUserHelper.GetUserIDByAccount(supervisoryAccount);
                }
            }
            else if (projectID.Contains("CLB"))
            {
                category = "社團";
                var applicationBasic = OFS_ClbApplicationHelper.GetBasicData(projectID);
                var projectMain = OFS_ClbApplicationHelper.GetProjectMainData(projectID);

                if (applicationBasic != null)
                {
                    projectName = applicationBasic.ProjectNameTw;
                    applicantAccount = projectMain.UserAccount;
                }

                if (projectMain != null)
                {
                    supervisoryAccount = projectMain.SupervisoryPersonAccount;
                    organizer = SysUserHelper.GetUserIDByAccount(supervisoryAccount);
                }
            }
            else if (projectID.Contains("CUL"))
            {
                category = "文化";
                var ID = OFS_CulProjectHelper.getID(projectID);
                var project = OFS_CulProjectHelper.get(ID);

                if (project != null)
                {
                    projectName = project.ProjectName;
                    applicantAccount = project.UserAccount;
                    organizer = project.Organizer;
                }
            }
            else if (projectID.Contains("EDC"))
            {
                category = "民間";
                var ID = OFS_EdcProjectHelper.getID(projectID);
                var project = OFS_EdcProjectHelper.get(ID);

                if (project != null)
                {
                    projectName = project.ProjectName;
                    applicantAccount = project.UserAccount;
                    organizer = project.Organizer;
                }
            }
            else if (projectID.Contains("MUL"))
            {
                category = "多元";
                var ID = OFS_MulProjectHelper.getID(projectID);
                var project = OFS_MulProjectHelper.get(ID);

                if (project != null)
                {
                    projectName = project.ProjectName;
                    applicantAccount = project.UserAccount;
                    organizer = project.Organizer;
                }
            }
            else if (projectID.Contains("LIT"))
            {
                category = "素養";
                var ID = OFS_LitProjectHelper.getID(projectID);
                var project = OFS_LitProjectHelper.get(ID);

                if (project != null)
                {
                    projectName = project.ProjectName;
                    applicantAccount = project.UserAccount;
                    organizer = project.Organizer;
                }
            }
            else if (projectID.Contains("ACC"))
            {
                category = "無障礙";
                var ID = OFS_AccProjectHelper.getID(projectID);
                var project = OFS_AccProjectHelper.get(ID);

                if (project != null)
                {
                    projectName = project.ProjectName;
                    applicantAccount = project.UserAccount;
                    organizer = project.Organizer;
                }
            }

            // 寄送通知信給主管單位 (H11)
            if (!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(projectName) && organizer.HasValue)
            {
                NotificationHelper.H11(category, projectName, organizer);
            }

            // 寄送通知信給申請單位 (H12)
            if (!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(projectName) && !string.IsNullOrEmpty(applicantAccount))
            {
                NotificationHelper.H12(category, projectName, applicantAccount);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"寄送查核通知信時發生錯誤: {ex.Message}");
            // 寄信失敗不影響主流程，只記錄錯誤
        }
    }

    protected void btnSubmitReply_Click(object sender, EventArgs e)
    {
        string ProjectID = Request.QueryString["ProjectID"];

        try
        {
            bool hasUpdates = false;

            foreach (RepeaterItem item in rptAuditRecords.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    var hiddenIdx = item.FindControl("hiddenIdx") as HiddenField;
                    var txtExecutorReply = item.FindControl("txtExecutorReply") as TextBox;

                    if (hiddenIdx != null && txtExecutorReply != null && txtExecutorReply.Visible)
                    {
                        int idx = Convert.ToInt32(hiddenIdx.Value);
                        string reply = txtExecutorReply.Text.Trim();

                        if (!string.IsNullOrEmpty(reply))
                        {
                            AuditRecordsHelper.UpdateExecutorComment(idx, reply);
                            hasUpdates = true;
                        }
                    }
                }
            }

            if (hasUpdates)
            {
                // 寄送通知信給主管單位
                SendReplyNotification(ProjectID);

                // 顯示成功訊息並延遲重新載入頁面
                string script = @"
                    Swal.fire({
                        icon: 'success',
                        title: '成功',
                        text: '回覆提送成功',
                        confirmButtonText: '確定',
                        timer: 2000,
                        timerProgressBar: true
                    }).then(function() {
                        window.location.href = window.location.href;
                    });";
                ClientScript.RegisterStartupScript(this.GetType(), "ReplySuccessAndReload", script, true);
            }
            else
            {
                ShowErrorMessage("沒有找到需要更新的回覆內容");
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage("系統錯誤：" + ex.Message);
        }
    }

    /// <summary>
    /// 寄送查核回覆通知信給主管單位
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    private void SendReplyNotification(string projectID)
    {
        try
        {
            string category = "";
            string projectName = "";
            int? organizer = null;

            // 根據 ProjectID 判斷計畫類型並取得相關資訊
            if (projectID.Contains("SCI"))
            {
                category = "科專";
                var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);
                var projectMain = OFS_SciApplicationHelper.getVersionByProjectID(projectID);

                if (applicationMain != null)
                {
                    projectName = applicationMain.ProjectNameTw;
                }

                if (projectMain != null)
                {
                    string supervisoryAccount = projectMain.SupervisoryPersonAccount;
                    organizer = SysUserHelper.GetUserIDByAccount(supervisoryAccount);
                }
            }
            else if (projectID.Contains("CLB"))
            {
                category = "社團";
                var applicationBasic = OFS_ClbApplicationHelper.GetBasicData(projectID);
                var projectMain = OFS_ClbApplicationHelper.GetProjectMainData(projectID);

                if (applicationBasic != null)
                {
                    projectName = applicationBasic.ProjectNameTw;
                }

                if (projectMain != null)
                {
                    string supervisoryAccount = projectMain.SupervisoryPersonAccount;
                    organizer = SysUserHelper.GetUserIDByAccount(supervisoryAccount);
                }
            }
            else if (projectID.Contains("CUL"))
            {
                category = "文化";
                var ID = OFS_CulProjectHelper.getID(projectID);
                var project = OFS_CulProjectHelper.get(ID);

                if (project != null)
                {
                    projectName = project.ProjectName;
                    organizer = project.Organizer;
                }
            }
            else if (projectID.Contains("EDC"))
            {
                category = "民間";
                var ID = OFS_EdcProjectHelper.getID(projectID);
                var project = OFS_EdcProjectHelper.get(ID);

                if (project != null)
                {
                    projectName = project.ProjectName;
                    organizer = project.Organizer;
                }
            }
            else if (projectID.Contains("MUL"))
            {
                category = "多元";
                var ID = OFS_MulProjectHelper.getID(projectID);
                var project = OFS_MulProjectHelper.get(ID);

                if (project != null)
                {
                    projectName = project.ProjectName;
                    organizer = project.Organizer;
                }
            }
            else if (projectID.Contains("LIT"))
            {
                category = "素養";
                var ID = OFS_LitProjectHelper.getID(projectID);
                var project = OFS_LitProjectHelper.get(ID);

                if (project != null)
                {
                    projectName = project.ProjectName;
                    organizer = project.Organizer;
                }
            }
            else if (projectID.Contains("ACC"))
            {
                category = "無障礙";
                var ID = OFS_AccProjectHelper.getID(projectID);
                var project = OFS_AccProjectHelper.get(ID);

                if (project != null)
                {
                    projectName = project.ProjectName;
                    organizer = project.Organizer;
                }
            }

            // 寄送通知信給主管單位 (H2)
            if (!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(projectName) && organizer.HasValue)
            {
                NotificationHelper.H2(category, projectName, organizer);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"寄送查核回覆通知信時發生錯誤: {ex.Message}");
            // 寄信失敗不影響主流程，只記錄錯誤
        }
    }

    private void ClearAuditForm()
    {
        txtAuditorName.Text = "";
        txtAuditDate.Text = "";
        txtAuditDate.Attributes.Remove("data-gregorian-date");
        ddlRiskAssessment.SelectedIndex = 0;
        txtAuditComment.Text = "";
    }

    private void ShowSuccessMessage(string message)
    {
        string script = $@"
            Swal.fire({{
                icon: 'success',
                title: '成功',
                text: '{message}',
                confirmButtonText: '確定'
            }});";
        ClientScript.RegisterStartupScript(this.GetType(), "SuccessMessage", script, true);
    }

    private void ShowErrorMessage(string message)
    {
        string script = $@"
            Swal.fire({{
                icon: 'error',
                title: '錯誤',
                text: '{message}',
                confirmButtonText: '確定'
            }});";
        ClientScript.RegisterStartupScript(this.GetType(), "ErrorMessage", script, true);
    }

    protected string FormatCheckDate(object checkDate)
    {
        if (checkDate != null && checkDate is DateTime)
        {
            return ((DateTime)checkDate).ToMinguoDate();
        }
        return "";
    }

    protected string GetDisplayValue(string value)
    {
        return string.IsNullOrEmpty(value) ? "-" : value;
    }

    /// <summary>
    /// 將風險評估英文值轉換為中文顯示
    /// </summary>
    /// <param name="riskValue">風險評估英文值</param>
    /// <returns>中文風險評估</returns>
    protected string GetRiskDisplayValue(string riskValue)
    {
        if (string.IsNullOrEmpty(riskValue))
            return "-";

        switch (riskValue.ToUpper())
        {
            case "LOW":
                return "低風險";
            case "MEDIUM":
                return "中風險";
            case "HIGH":
                return "高風險";
            default:
                return riskValue; // 如果不是預期的值，直接返回原值
        }
    }

    /// <summary>
    /// 顯示錯誤訊息並重導向
    /// </summary>
    /// <param name="message">錯誤訊息</param>
    private void ShowErrorAndRedirect(string message)
    {
        string script = $@"
            Swal.fire({{
                icon: 'error',
                title: '錯誤',
                text: '{message}',
                confirmButtonText: '確定'
            }}).then(function() {{
                window.location.href = '{ResolveUrl("~/OFS/inprogressList.aspx")}';
            }});";
        ClientScript.RegisterStartupScript(this.GetType(), "ErrorRedirect", script, true);
    }

    /// <summary>
    /// 處理異常
    /// </summary>
    /// <param name="ex">異常物件</param>
    /// <param name="message">自定義訊息</param>
    private void HandleException(Exception ex, string message)
    {
        System.Diagnostics.Debug.WriteLine($"{message}: {ex.Message}");
        System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
        ShowErrorMessage($"{message}: {ex.Message}");
    }

    /// <summary>
    /// 判斷當前用戶權限類型
    /// </summary>
    /// <returns>用戶權限類型</returns>
    private AuditRecordsModel.UserPermissionType GetCurrentUserPermission()
    {
        try
        {
            // 取得當前使用者資訊
            var currentUser = GetCurrentUserInfo();
            if (currentUser == null || currentUser.OFS_RoleName == null)
            {
                return AuditRecordsModel.UserPermissionType.GeneralUser;
            }

            // 檢查是否為主管單位或系統管理員角色
            var adminRoles = new[] { "主管單位人員", "主管單位窗口", "系統管理者" };

            foreach (string roleName in currentUser.OFS_RoleName)
            {
                if (!string.IsNullOrEmpty(roleName) && adminRoles.Contains(roleName))
                {
                    return AuditRecordsModel.UserPermissionType.Administrator;
                }
            }

            return AuditRecordsModel.UserPermissionType.GeneralUser;
        }
        catch (Exception ex)
        {
            HandleException(ex, "取得用戶權限時發生錯誤");
            // 發生錯誤時回傳一般用戶權限，較為安全
            return AuditRecordsModel.UserPermissionType.GeneralUser;
        }
    }

    /// <summary>
    /// 匯出查核紀錄按鈕點擊事件
    /// </summary>
    protected void btnExportRecords_Click(object sender, EventArgs e)
    {
        string ProjectID = Request.QueryString["ProjectID"];

        try
        {
            // 權限檢查：只有主管單位窗口、系統管理員可以匯出
            CurrentUserPermission = GetCurrentUserPermission();
            if (CurrentUserPermission != AuditRecordsModel.UserPermissionType.Administrator)
            {
                ShowErrorMessage("您沒有匯出查核紀錄的權限");
                return;
            }

            // 取得匯出資料
            DataSet exportData = AuditRecordsHelper.GetAuditRecordsForExport(ProjectID);

            if (exportData.Tables["ProjectData"].Rows.Count == 0)
            {
                ShowErrorMessage("找不到計畫資料");
                return;
            }

            // 取得計畫基本資料
            DataRow projectRow = exportData.Tables["ProjectData"].Rows[0];
            string projectName = projectRow["ProjectNameTw"]?.ToString() ?? "";
            string orgName = projectRow["OrgName"]?.ToString() ?? "";
            DateTime? startTime = projectRow["StartTime"] as DateTime?;
            DateTime? endTime = projectRow["EndTime"] as DateTime?;

            // 計算計畫期程
            string projectPeriod = "";
            if (startTime.HasValue && endTime.HasValue)
            {
                projectPeriod = $"{startTime.Value.ToMinguoDate()} - {endTime.Value.ToMinguoDate()}";
            }

            // 讀取範本檔案
            string templatePath = Server.MapPath("~/Template/Shared/查核意見及回覆紀錄_匯出範本.xlsx");

            if (!File.Exists(templatePath))
            {
                ShowErrorMessage("找不到匯出範本檔案");
                return;
            }

            IWorkbook workbook;
            using (FileStream file = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(file);
            }

            ISheet sheet = workbook.GetSheetAt(0);

            // 填入計畫基本資料 (B1~B4)
            SetCellValue(sheet, 0, 1, ProjectID);          // B1: 計畫編號
            SetCellValue(sheet, 1, 1, projectName);        // B2: 計畫名稱
            SetCellValue(sheet, 2, 1, orgName);            // B3: 執行單位
            SetCellValue(sheet, 3, 1, projectPeriod);      // B4: 計畫期程

            // 填入查核紀錄資料 (從 A7 開始)
            int rowIndex = 6; // Excel 的第7行 (0-based index)
            foreach (DataRow auditRow in exportData.Tables["AuditRecords"].Rows)
            {
                DateTime? checkDate = auditRow["CheckDate"] as DateTime?;
                string reviewerName = auditRow["ReviewerName"]?.ToString() ?? "";
                string risk = auditRow["Risk"]?.ToString() ?? "";
                string reviewerComment = auditRow["ReviewerComment"]?.ToString() ?? "";
                string executorComment = auditRow["ExecutorComment"]?.ToString() ?? "";

                // 轉換風險評估為中文顯示
                string riskDisplayValue = GetRiskDisplayValue(risk);

                IRow row = sheet.GetRow(rowIndex);
                if (row == null)
                {
                    row = sheet.CreateRow(rowIndex);
                }

                SetCellValue(sheet, rowIndex, 0, checkDate.HasValue ? checkDate.Value.ToMinguoDate() : "");  // A: 查核日期
                SetCellValue(sheet, rowIndex, 1, reviewerName);          // B: 查核人員
                SetCellValue(sheet, rowIndex, 2, riskDisplayValue);      // C: 風險評估
                SetCellValue(sheet, rowIndex, 3, reviewerComment);       // D: 查核意見
                SetCellValue(sheet, rowIndex, 4, executorComment);       // E: 執行單位回覆

                rowIndex++;
            }

            // 輸出檔案
            string fileName = $"查核紀錄_{ProjectID}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("Content-Disposition", $"attachment; filename={HttpUtility.UrlEncode(fileName)}");
                Response.BinaryWrite(ms.ToArray());
                Response.End();
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "匯出查核紀錄時發生錯誤");
        }
    }

    /// <summary>
    /// 設定 Excel 儲存格的值
    /// </summary>
    private void SetCellValue(ISheet sheet, int rowIndex, int colIndex, string value)
    {
        IRow row = sheet.GetRow(rowIndex);
        if (row == null)
        {
            row = sheet.CreateRow(rowIndex);
        }

        ICell cell = row.GetCell(colIndex);
        if (cell == null)
        {
            cell = row.CreateCell(colIndex);
        }

        cell.SetCellValue(value ?? "");
    }
}
