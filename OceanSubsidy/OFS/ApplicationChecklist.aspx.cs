using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Model.OFS;
using GS.OCA_OceanSubsidy.Operation.OFS;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.X509.SigI;
using GS.App;
using GS.Data;

public partial class OFS_ApplicationChecklist : System.Web.UI.Page
{
    private List<ReviewChecklistItem> OriginalSciMainList;


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadData();
            LoadAvailablePrograms(); // 載入可申請的計畫類別

            // 設定年度預設值為今年
            int currentYear = DateTimeHelper.GregorianYearToMinguo(DateTime.Now.Year); // 民國年
            if (ddlYear.Items.FindByValue(currentYear.ToString()) != null)
            {
                ddlYear.SelectedValue = currentYear.ToString();
            }

            // 讀取 URL 參數並設定搜尋條件
            LoadSearchParametersFromUrl();

            // 初始狀態顯示總申請的資料
            hidSelectedStage.Value = "總申請";

            // 更新標籤統計
            UpdateTabStatisticsAndActiveState("總申請");

            // 初始載入時，觸發前端載入資料
            ClientScript.RegisterStartupScript(this.GetType(), "InitialLoad",
                "$(document).ready(function() { setTimeout(function() { loadFilteredData(); }, 200); });", true);
        }
        else
        {
            // 從 ViewState 還原資料
            RestoreDataFromViewState();

            // PostBack 時也要更新標籤統計
            if (OriginalSciMainList != null && OriginalSciMainList.Count > 0)
            {
                string currentSelectedCategory = hidSelectedStage.Value;
                if (string.IsNullOrEmpty(currentSelectedCategory))
                {
                    currentSelectedCategory = "總申請";
                    hidSelectedStage.Value = currentSelectedCategory;
                }

                UpdateTabStatisticsAndActiveState(currentSelectedCategory);
            }
        }
    }

    private void RestoreDataFromViewState()
    {
        if (ViewState["OriginalSciMainList"] != null)
        {
            OriginalSciMainList = (List<ReviewChecklistItem>)ViewState["OriginalSciMainList"];
        }
    }

    private void SaveDataToViewState()
    {
        ViewState["OriginalSciMainList"] = OriginalSciMainList;
    }


    private void LoadData()
    {
        try
        {
            // 判斷當前使用者是否為主管機關人員
            bool isSupervisoryUser = IsSupervisoryUser();
            string userAccount = "";

            // 如果不是主管機關人員，只載入自己的案件
            if (!isSupervisoryUser)
            {
                var currentUser = GetCurrentUserInfo();
                if (currentUser != null)
                {
                    userAccount = currentUser.Account;
                }
            }

            // 載入申請計畫清單資料
            OriginalSciMainList = ApplicationChecklistHelper.GetLatestApplicationChecklist(userAccount);

            // 儲存到 ViewState
            SaveDataToViewState();
        }
        catch (Exception ex)
        {
            // 記錄錯誤或顯示錯誤訊息
            System.Diagnostics.Debug.WriteLine($"載入資料時發生錯誤: {ex.Message}");
            Response.Write($"<script>alert('載入資料時發生錯誤: {ex.Message}');</script>");
        }

        // Debug 資訊
        System.Diagnostics.Debug.WriteLine($"LoadData completed: OriginalSciMainList.Count = {OriginalSciMainList?.Count ?? 0}");
    }

    // 搜尋按鈕事件處理 - 觸發前端載入篩選資料
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            // 觸發前端重新載入篩選資料
            ClientScript.RegisterStartupScript(this.GetType(), "TriggerSearch",
                "loadFilteredData();", true);
        }
        catch (Exception ex)
        {
            ShowMessage($"搜尋時發生錯誤：{ex.Message}", false);
        }
    }


    // 顯示訊息
    private void ShowMessage(string message, bool isSuccess)
    {
        string alertType = isSuccess ? "success" : "error";
        string script = $"alert('{message}');";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowMessage", script, true);
    }

    // 標籤篩選事件處理 - 觸發前端載入篩選資料
    protected void btnStageFilter_Click(object sender, EventArgs e)
    {
        try
        {
            // 觸發前端重新載入篩選資料
            ClientScript.RegisterStartupScript(this.GetType(), "TriggerStageFilter",
                "loadFilteredData();", true);
        }
        catch (Exception ex)
        {
            ShowMessage($"篩選時發生錯誤：{ex.Message}", false);
        }
    }

    // 更新標籤統計和選中狀態
    private void UpdateTabStatisticsAndActiveState(string selectedCategory)
    {
        if (OriginalSciMainList == null) return;

        try
        {
            // 統計各申請類別的數量
            var categoryCounts = new Dictionary<string, int>
            {
                ["總申請"] = OriginalSciMainList.Count,
                ["科專"] = OriginalSciMainList.Count(x => x.ProjectID?.Contains("SCI") == true),
                ["文化"] = OriginalSciMainList.Count(x => x.ProjectID?.Contains("CUL") == true),
                ["學校民間"] = OriginalSciMainList.Count(x => x.ProjectID?.Contains("EDC") == true),
                ["學校社團"] = OriginalSciMainList.Count(x => x.ProjectID?.Contains("CLB") == true),
                ["多元"] = OriginalSciMainList.Count(x => x.ProjectID?.Contains("MUL") == true),
                ["素養"] = OriginalSciMainList.Count(x => x.ProjectID?.Contains("LIT") == true),
                ["無障礙"] = OriginalSciMainList.Count(x => x.ProjectID?.Contains("ACC") == true)
            };

            // 建立 JSON 格式的統計資料
            var countsJson = new StringBuilder();
            countsJson.Append("{");

            var categories = new[] { "總申請", "科專", "文化", "學校民間", "學校社團", "多元", "素養", "無障礙" };
            for (int i = 0; i < categories.Length; i++)
            {
                if (i > 0) countsJson.Append(",");
                var count = categoryCounts[categories[i]];
                countsJson.Append($"'{categories[i]}':{count}");
            }

            countsJson.Append("}");

            // 建立 JavaScript 語句
            var jsCode = $"updateTabCounts({countsJson.ToString()});";
            if (!string.IsNullOrEmpty(selectedCategory))
            {
                jsCode += $" setActiveTab('{selectedCategory}');";
            }

            // 將統計資料和 active 狀態傳遞給前端
            Page.ClientScript.RegisterStartupScript(this.GetType(), "UpdateTabCountsAndActive",
                jsCode, true);
        }
        catch (Exception ex)
        {
            // 記錄錯誤但不影響主功能
            System.Diagnostics.Debug.WriteLine($"更新標籤統計時發生錯誤：{ex.Message}");
        }
    }
    // 檢查是否可編輯
    private bool CanEdit(string status, string statusName)
    {
        if (status == "決審核定")
        {
            return statusName == "計畫書修正中";
        }
        else
        {
            return statusName == "編輯中" || statusName == "補正補件";
        }
    }

    // 檢查是否可撤案
    private bool CanWithdraw(string status)
    {
        var withdrawableStatuses = new[] { "資格審查", "內容審查", "領域審查", "初審", "技術審查", "複審", "決審核定" };
        return withdrawableStatuses.Contains(status);
    }



    // 取得編輯頁面的網址
    // private string GetEditUrl(ReviewChecklistItem item)
    // {
    //     if (item == null || string.IsNullOrEmpty(item.ProjectID)) return "#";
    //
    //     // 根據計畫類型決定編輯頁面
    //     string projectCategory = item.GetProjectCategory();
    //     switch (projectCategory)
    //     {
    //         case "科專":
    //             return $"~/OFS/SCI/SciApplication.aspx?ProjectID={item.ProjectID}";
    //         case "文化":
    //             return $"~/OFS/CUL/Application.aspx?ID={item.ProjectID}";
    //         case "學校社團":
    //             // 尚未有這個網址，暫時返回空值
    //             return "#";
    //         case "學校民間":
    //             return $"~/OFS/EDC/Application.aspx?ID={item.ProjectID}";
    //         case "多元":
    //             return $"~/OFS/MUL/Application.aspx?ID={item.ProjectID}";
    //         case "素養":
    //             return $"~/OFS/LIT/Application.aspx?ID={item.ProjectID}";
    //         case "無障礙":
    //             return $"~/OFS/ACC/Application.aspx?ID={item.ProjectID}";
    //         default:
    //             return "#";
    //     }
    // }

    // 已改為純前端分頁架構，移除舊的分頁相關方法

    // 申請計畫按鈕事件
    protected void btnCreateApplication_Click(object sender, EventArgs e)
    {
        try
        {
            string selectedGrantTypeId = ddlModalYear.SelectedValue;

            if (string.IsNullOrEmpty(selectedGrantTypeId))
            {
                ShowMessage("請選擇申請補助計畫類別", false);
                return;
            }

            // 根據選擇的計畫類別重定向到相應的申請表單頁面
            // 這裡可以根據 GrantTypeID 判斷要導向哪個申請表單
            if (selectedGrantTypeId == "SCI")
            {
                Response.Redirect($"~/OFS/SCI/SciApplication.aspx?GrantTypeID={selectedGrantTypeId}");
            }
            else if (selectedGrantTypeId == "CUL")
            {
                Response.Redirect("~/OFS/CUL/Application.aspx");
            }
            else if (selectedGrantTypeId =="EDC")
            {
                Response.Redirect("~/OFS/EDC/Application.aspx");
            }
            else if (selectedGrantTypeId =="CLB")
            {
                Response.Redirect($"~/OFS/CLB/ClbApplication.aspx?GrantTypeID={selectedGrantTypeId}");
            }
            else if (selectedGrantTypeId =="MUL")
            {
                Response.Redirect("~/OFS/MUL/Application.aspx");
            }
            else if (selectedGrantTypeId =="LIT")
            {
                Response.Redirect("~/OFS/LIT/Application.aspx");
            }
            else if (selectedGrantTypeId =="ACC")
            {
                Response.Redirect("~/OFS/ACC/Application.aspx");
            }
            else
            {
                // 其他不符合的情況
            }


        }
        catch (Exception ex)
        {
            ShowMessage($"申請計畫時發生錯誤：{ex.Message}", false);
        }
    }

    // 載入可申請的計畫類別
    private void LoadAvailablePrograms()
    {
        try
        {
            // 使用統一的 Helper 方法載入計畫類別
            var grantTypes = ApplicationChecklistHelper.GetAvailableGrantTypes();

            ddlModalYear.Items.Clear();

            foreach (var item in grantTypes)
            {
                ddlModalYear.Items.Add(item);
            }

            // 如果沒有資料，添加預設選項
            if (ddlModalYear.Items.Count == 0)
            {
                ddlModalYear.Items.Add(new ListItem("目前沒有可申請的計畫", ""));
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"載入申請計畫類別時發生錯誤：{ex.Message}");
        }
    }

    /// <summary>
    /// 從 URL 參數讀取搜尋條件並設定到表單控制項
    /// </summary>
    private void LoadSearchParametersFromUrl()
    {
        try
        {
            // 讀取計畫編號或名稱關鍵字
            string searchText = Request.QueryString["search"];
            if (!string.IsNullOrEmpty(searchText))
            {
                txtSearch.Text = searchText;
            }

            // 讀取計畫內容關鍵字
            string contentKeyword = Request.QueryString["contentKeyword"];
            if (!string.IsNullOrEmpty(contentKeyword))
            {
                txtContentKeyword.Text = contentKeyword;
            }

            // 讀取年度
            string year = Request.QueryString["year"];
            if (!string.IsNullOrEmpty(year) && ddlYear.Items.FindByValue(year) != null)
            {
                ddlYear.SelectedValue = year;
            }

            // 讀取狀態
            string status = Request.QueryString["status"];
            if (!string.IsNullOrEmpty(status) && ddlStatus.Items.FindByValue(status) != null)
            {
                ddlStatus.SelectedValue = status;
            }

            // 讀取階段
            string stage = Request.QueryString["stage"];
            if (!string.IsNullOrEmpty(stage) && ddlStage.Items.FindByValue(stage) != null)
            {
                ddlStage.SelectedValue = stage;
            }

            // 讀取申請單位
            string orgName = Request.QueryString["OrgName"];
            if (!string.IsNullOrEmpty(orgName))
            {
                txtDepartment.Text = orgName;
            }

            // 讀取主管單位
            string reviewer = Request.QueryString["reviewer"];
            if (!string.IsNullOrEmpty(reviewer) && ddlReviewer.Items.FindByValue(reviewer) != null)
            {
                ddlReviewer.SelectedValue = reviewer;
            }

            // 讀取待回覆
            string waitingReply = Request.QueryString["waitingReply"];
            if (!string.IsNullOrEmpty(waitingReply))
            {
                waitingReply = waitingReply.ToLower();
                if (waitingReply == "true" || waitingReply == "1")
                {
                    this.waitingReply.Checked = true;
                }
            }

            System.Diagnostics.Debug.WriteLine("URL 參數已載入到搜尋表單");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"載入 URL 參數時發生錯誤：{ex.Message}");
        }
    }

    /// <summary>
    /// 記錄案件操作到 OFS_CaseHistoryLog 資料表
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <param name="operationType">操作類型：撤案、恢復案件、刪除</param>
    /// <param name="description">操作描述或原因</param>
    /// <param name="stageStatusBefore">狀態變更前</param>
    /// <param name="stageStatusAfter">狀態變更後</param>
    private void LogCaseOperation(string projectId, string operationType, string description,
        string stageStatusBefore = "", string stageStatusAfter = "")
    {
        try
        {
            // 從Session取得使用者資訊
            var currentUser = GetCurrentUserInfo();
            string userName = currentUser?.UserName ?? "系統";

            // 建立案件歷程記錄
            var caseHistoryLog = new OFS_CaseHistoryLog
            {
                ProjectID = projectId,
                ChangeTime = DateTime.Now,
                UserName = userName,
                StageStatusBefore = stageStatusBefore,
                StageStatusAfter = stageStatusAfter,
                Description = description
            };

            // 儲存到資料庫
            bool success = ApplicationChecklistHelper.InsertCaseHistoryLog(caseHistoryLog);

            if (success)
            {
                System.Diagnostics.Debug.WriteLine($"案件操作記錄已儲存：{operationType} - {projectId}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"案件操作記錄儲存失敗：{operationType} - {projectId}");
            }
        }
        catch (Exception ex)
        {
            // 記錄失敗不影響主要操作，只記錄到Debug
            System.Diagnostics.Debug.WriteLine($"記錄案件操作失敗：{ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"取得使用者資訊時發生錯誤: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 取得專案目前狀態
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <returns>目前狀態</returns>
    private string GetProjectCurrentStatus(string projectId)
    {
        try
        {
            // 從原始資料清單中找到對應的專案
            if (OriginalSciMainList != null)
            {
                var project = OriginalSciMainList.FirstOrDefault(p => p.ProjectID == projectId);
                if (project != null)
                {
                    // 如果是撤案狀態，優先顯示撤案
                    if (project.isWithdrawal == true)
                    {
                        return "已撤案";
                    }

                    // 否則返回一般狀態
                    return project.Statuses + " " + project.StatusesName ?? "未知狀態";
                }
            }

            return "未知狀態";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得專案狀態時發生錯誤: {ex.Message}");
            return "未知狀態";
        }
    }

    // 處理撤案操作的 WebForm 方法
    protected void btnConfirmWithdraw_Click(object sender, EventArgs e)
    {
        try
        {
            string projectId = hdnWithdrawProjectId.Value;
            string reason = txtWithdrawReason.Text.Trim();

            if (string.IsNullOrEmpty(reason))
            {
                ShowMessage("請輸入撤案原因", false);
                return;
            }

            if (string.IsNullOrEmpty(projectId))
            {
                ShowMessage("系統錯誤：未找到專案資訊", false);
                return;
            }
            else if (projectId.Contains("SCI"))
            {
                // 取得操作前的狀態
                string beforeStatus = GetProjectCurrentStatus(projectId);

                // 取得專案資料
                var projectMain = OFS_SciApplicationHelper.getVersionByProjectID(projectId);
                var projectBasic = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectId);

                // 取得承辦人的 UserID
                int? organizerUserId = null;
                if (!string.IsNullOrEmpty(projectMain.SupervisoryPersonAccount))
                {
                    var supervisoryUser = SysUserHelper.QueryUserByAccount(projectMain.SupervisoryPersonAccount);
                    if (supervisoryUser != null && supervisoryUser.Rows.Count > 0)
                    {
                        organizerUserId = Convert.ToInt32(supervisoryUser.Rows[0]["UserID"]);
                    }
                }

                // 更新撤案狀態
                ApplicationChecklistHelper.UpdateWithdrawalStatus(projectId, true, reason);

                // 記錄操作到案件歷程
                LogCaseOperation(projectId, "撤案", reason, beforeStatus, "已撤案");

                // 發送撤案通知
                NotificationHelper.Z1("科專", "SCI", projectBasic.ProjectNameTw, reason,
                    projectMain.UserAccount, organizerUserId);

            }
            else if (projectId.Contains("CLB"))
            {
                // 取得操作前的狀態
                string beforeStatus = GetProjectCurrentStatus(projectId);

                // 取得專案資料
                var projectMain = OFS_ClbApplicationHelper.GetProjectMainData(projectId);
                var projectBasic = OFS_ClbApplicationHelper.GetBasicData(projectId);

                // 取得承辦人的 UserID
                int? organizerUserId = null;
                if (!string.IsNullOrEmpty(projectMain.SupervisoryPersonAccount))
                {
                    var supervisoryUser = SysUserHelper.QueryUserByAccount(projectMain.SupervisoryPersonAccount);
                    if (supervisoryUser != null && supervisoryUser.Rows.Count > 0)
                    {
                        organizerUserId = Convert.ToInt32(supervisoryUser.Rows[0]["UserID"]);
                    }
                }

                // 更新撤案狀態
                ApplicationChecklistHelper.CLB_UpdateWithdrawalStatus(projectId, true, reason);

                // 記錄操作到案件歷程
                LogCaseOperation(projectId, "撤案", reason, beforeStatus, "已撤案");

                // 發送撤案通知
                NotificationHelper.Z1("社團", "CLB", projectBasic.ProjectNameTw, reason,
                    projectMain.UserAccount, organizerUserId);

            }
            else if (projectId.Contains("CUL"))
            {
                // 取得操作前的狀態
                string beforeStatus = GetProjectCurrentStatus(projectId);

                // 取得專案資料
                int projectIntId = OFS_CulProjectHelper.getID(projectId);
                var project = OFS_CulProjectHelper.get(projectIntId);

                // 取得承辦人的 UserID
                int? organizerUserId = null;
                if (project.Organizer.HasValue && project.Organizer.Value > 0)
                {
                    organizerUserId = project.Organizer.Value;
                }

                // 更新撤案狀態
                OFS_CulProjectHelper.updateWithdrawalStatus(projectId, true);

                // 記錄操作到案件歷程
                LogCaseOperation(projectId, "撤案", reason, beforeStatus, "已撤案");

                // 發送撤案通知
                NotificationHelper.Z1("文化", "CUL", project.ProjectName, reason,
                    project.UserAccount, organizerUserId);
            }
            else if (projectId.Contains("EDC"))
            {
                // 取得操作前的狀態
                string beforeStatus = GetProjectCurrentStatus(projectId);

                // 取得專案資料
                int projectIntId = OFS_EdcProjectHelper.getID(projectId);
                var project = OFS_EdcProjectHelper.get(projectIntId);

                // 取得承辦人的 UserID
                int? organizerUserId = null;
                if (project.Organizer.HasValue && project.Organizer.Value > 0)
                {
                    organizerUserId = project.Organizer.Value;
                }

                // 更新撤案狀態
                OFS_EdcProjectHelper.updateWithdrawalStatus(projectId, true);

                // 記錄操作到案件歷程
                LogCaseOperation(projectId, "撤案", reason, beforeStatus, "已撤案");

                // 發送撤案通知
                NotificationHelper.Z1("學校民間", "EDC", project.ProjectName, reason,
                    project.UserAccount, organizerUserId);
            }
            else if (projectId.Contains("MUL"))
            {
                // 取得操作前的狀態
                string beforeStatus = GetProjectCurrentStatus(projectId);

                // 取得專案資料
                int projectIntId = OFS_MulProjectHelper.getID(projectId);
                var project = OFS_MulProjectHelper.get(projectIntId);

                // 取得承辦人的 UserID
                int? organizerUserId = null;
                if (project.Organizer.HasValue && project.Organizer.Value > 0)
                {
                    organizerUserId = project.Organizer.Value;
                }

                // 更新撤案狀態
                OFS_MulProjectHelper.updateWithdrawalStatus(projectId, true);

                // 記錄操作到案件歷程
                LogCaseOperation(projectId, "撤案", reason, beforeStatus, "已撤案");

                // 發送撤案通知
                NotificationHelper.Z1("多元", "MUL", project.ProjectName, reason,
                    project.UserAccount, organizerUserId);
            }
            else if (projectId.Contains("LIT"))
            {
                // 取得操作前的狀態
                string beforeStatus = GetProjectCurrentStatus(projectId);

                // 取得專案資料
                int projectIntId = OFS_LitProjectHelper.getID(projectId);
                var project = OFS_LitProjectHelper.get(projectIntId);

                // 取得承辦人的 UserID
                int? organizerUserId = null;
                if (project.Organizer.HasValue && project.Organizer.Value > 0)
                {
                    organizerUserId = project.Organizer.Value;
                }

                // 更新撤案狀態
                OFS_LitProjectHelper.updateWithdrawalStatus(projectId, true);

                // 記錄操作到案件歷程
                LogCaseOperation(projectId, "撤案", reason, beforeStatus, "已撤案");

                // 發送撤案通知
                NotificationHelper.Z1("素養", "LIT", project.ProjectName, reason,
                    project.UserAccount, organizerUserId);
            }
            else if (projectId.Contains("ACC"))
            {
                // 取得操作前的狀態
                string beforeStatus = GetProjectCurrentStatus(projectId);

                // 取得專案資料
                int projectIntId = OFS_AccProjectHelper.getID(projectId);
                var project = OFS_AccProjectHelper.get(projectIntId);

                // 取得承辦人的 UserID
                int? organizerUserId = null;
                if (project.Organizer.HasValue && project.Organizer.Value > 0)
                {
                    organizerUserId = project.Organizer.Value;
                }

                // 更新撤案狀態
                OFS_AccProjectHelper.updateWithdrawalStatus(projectId, true);

                // 記錄操作到案件歷程
                LogCaseOperation(projectId, "撤案", reason, beforeStatus, "已撤案");

                // 發送撤案通知
                NotificationHelper.Z1("無障礙", "ACC", project.ProjectName, reason,
                    project.UserAccount, organizerUserId);
            }
            else
            {
                // 其他不符合的情況
            }

            // 清空輸入欄位
            txtWithdrawReason.Text = "";
            hdnWithdrawProjectId.Value = "";

            ShowMessage("撤案成功", true);

            // 重新載入資料並更新統計
            LoadData();
            UpdateTabStatisticsAndActiveState(hidSelectedStage.Value);
        }
        catch (Exception ex)
        {
            ShowMessage($"撤案時發生錯誤：{ex.Message}", false);
        }
    }

    // 處理撤案操作的 AJAX 方法 (保留以防其他地方使用)
    // [System.Web.Services.WebMethod]
    // public static string HandleWithdraw(string projectId, string reason)
    // {
    //     try
    //     {
    //         ApplicationChecklistHelper.UpdateWithdrawalStatus(projectId, true, reason);
    //         return "success";
    //     }
    //     catch (Exception ex)
    //     {
    //         return $"撤案時發生錯誤：{ex.Message}";
    //     }
    // }

    // 處理刪除操作的 WebForm 方法
    protected void btnConfirmDelete_Click(object sender, EventArgs e)
    {
        try
        {
            string projectId = hdnDeleteProjectId.Value;
            string reason = txtDeleteReason.Text.Trim();

            if (string.IsNullOrEmpty(reason))
            {
                ShowMessage("請輸入刪除原因", false);
                return;
            }

            if (string.IsNullOrEmpty(projectId))
            {
                ShowMessage("系統錯誤：未找到專案資訊", false);
                return;
            }else if (projectId.Contains("SCI"))
            {
                // 取得操作前的狀態
                string beforeStatus = GetProjectCurrentStatus(projectId);
                ApplicationChecklistHelper.UpdateExistsStatus(projectId, false);
                // 記錄操作到案件歷程
                LogCaseOperation(projectId, "刪除", reason, beforeStatus, "已刪除");
            }
            else if (projectId.Contains("CUL"))
            {
                string beforeStatus = GetProjectCurrentStatus(projectId);
                OFS_CulProjectHelper.updateExistsStatus(projectId, false);

                LogCaseOperation(projectId, "刪除", reason, beforeStatus, "已刪除");
            }
            else if (projectId.Contains("EDC"))
            {
                string beforeStatus = GetProjectCurrentStatus(projectId);
                OFS_EdcProjectHelper.updateExistsStatus(projectId, false);

                LogCaseOperation(projectId, "刪除", reason, beforeStatus, "已刪除");
            }
            else if (projectId.Contains("CLB"))
            {
                // 取得操作前的狀態
                string beforeStatus = GetProjectCurrentStatus(projectId);
                ApplicationChecklistHelper.CLB_UpdateExistsStatus(projectId, false, reason);
                // 記錄操作到案件歷程
                LogCaseOperation(projectId, "刪除", reason, beforeStatus, "已刪除");
            }
            else if (projectId.Contains("MUL"))
            {
                string beforeStatus = GetProjectCurrentStatus(projectId);
                OFS_MulProjectHelper.updateExistsStatus(projectId, false);

                LogCaseOperation(projectId, "刪除", reason, beforeStatus, "已刪除");
            }
            else if (projectId.Contains("LIT"))
            {
                string beforeStatus = GetProjectCurrentStatus(projectId);
                OFS_LitProjectHelper.updateExistsStatus(projectId, false);

                LogCaseOperation(projectId, "刪除", reason, beforeStatus, "已刪除");
            }
            else if (projectId.Contains("ACC"))
            {
                string beforeStatus = GetProjectCurrentStatus(projectId);
                OFS_AccProjectHelper.updateExistsStatus(projectId, false);

                LogCaseOperation(projectId, "刪除", reason, beforeStatus, "已刪除");
            }
            else
            {
                // 其他不符合的情況
            }



            // 清空輸入欄位
            txtDeleteReason.Text = "";
            hdnDeleteProjectId.Value = "";

            ShowMessage("刪除成功", true);

            // 重新載入資料並更新統計
            LoadData();
            UpdateTabStatisticsAndActiveState(hidSelectedStage.Value);
        }
        catch (Exception ex)
        {
            ShowMessage($"刪除時發生錯誤：{ex.Message}", false);
        }
    }

    // // 處理刪除操作的 AJAX 方法 (保留以防其他地方使用)
    // [System.Web.Services.WebMethod]
    // public static string HandleDelete(string projectId, string reason)
    // {
    //     try
    //     {
    //         ApplicationChecklistHelper.UpdateExistsStatus(projectId, false, reason);
    //         return "success";
    //     }
    //     catch (Exception ex)
    //     {
    //         return $"刪除時發生錯誤：{ex.Message}";
    //     }
    // }

    // 處理恢復案件操作的 WebForm 方法
    protected void btnConfirmRestore_Click(object sender, EventArgs e)
    {
        try
        {
            string projectId = hdnRestoreProjectId.Value;
            string reason = txtRestoreReason.Text.Trim();

            if (string.IsNullOrEmpty(reason))
            {
                ShowMessage("請輸入恢復案件原因", false);
                return;
            }

            if (string.IsNullOrEmpty(projectId))
            {
                ShowMessage("系統錯誤：未找到專案資訊", false);
                return;
            }
            if (projectId.Contains("SCI"))
            {
                // 取得操作前的狀態
                string beforeStatus = GetProjectCurrentStatus(projectId);

                // 取得專案資料
                var projectMain = OFS_SciApplicationHelper.getVersionByProjectID(projectId);
                var projectBasic = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectId);

                // 取得承辦人的 UserID
                int? organizerUserId = null;
                if (!string.IsNullOrEmpty(projectMain.SupervisoryPersonAccount))
                {
                    var supervisoryUser = SysUserHelper.QueryUserByAccount(projectMain.SupervisoryPersonAccount);
                    if (supervisoryUser != null && supervisoryUser.Rows.Count > 0)
                    {
                        organizerUserId = Convert.ToInt32(supervisoryUser.Rows[0]["UserID"]);
                    }
                }

                // 更新撤案狀態為 false（恢復案件）
                ApplicationChecklistHelper.UpdateWithdrawalStatus(projectId, false);

                // 取得操作後的狀態（從重新載入的資料中取得）
                string afterStatus = GetProjectCurrentStatus(projectId);

                // 記錄操作到案件歷程
                LogCaseOperation(projectId, "恢復案件", reason, beforeStatus, afterStatus);

                // 發送恢復案件通知
                NotificationHelper.Z2("科專", "SCI", projectBasic.ProjectNameTw, reason,
                    projectMain.UserAccount, organizerUserId);
            }
            else if (projectId.Contains("CLB"))
            {
                // 取得操作前的狀態
                string beforeStatus = GetProjectCurrentStatus(projectId);

                // 取得專案資料
                var projectMain = OFS_ClbApplicationHelper.GetProjectMainData(projectId);
                var projectBasic = OFS_ClbApplicationHelper.GetBasicData(projectId);

                // 取得承辦人的 UserID
                int? organizerUserId = null;
                if (!string.IsNullOrEmpty(projectMain.SupervisoryPersonAccount))
                {
                    var supervisoryUser = SysUserHelper.QueryUserByAccount(projectMain.SupervisoryPersonAccount);
                    if (supervisoryUser != null && supervisoryUser.Rows.Count > 0)
                    {
                        organizerUserId = Convert.ToInt32(supervisoryUser.Rows[0]["UserID"]);
                    }
                }

                // 更新撤案狀態為 false（恢復案件）
                ApplicationChecklistHelper.CLB_UpdateWithdrawalStatus(projectId, false);

                // 取得操作後的狀態（從重新載入的資料中取得）
                string afterStatus = GetProjectCurrentStatus(projectId);

                // 記錄操作到案件歷程
                LogCaseOperation(projectId, "恢復案件", reason, beforeStatus, afterStatus);

                // 發送恢復案件通知
                NotificationHelper.Z2("社團", "CLB", projectBasic.ProjectNameTw, reason,
                    projectMain.UserAccount, organizerUserId);
            }
            else if (projectId.Contains("CUL"))
            {
                // 取得操作前的狀態
                string beforeStatus = GetProjectCurrentStatus(projectId);

                // 取得專案資料
                int projectIntId = OFS_CulProjectHelper.getID(projectId);
                var project = OFS_CulProjectHelper.get(projectIntId);

                // 取得承辦人的 UserID
                int? organizerUserId = null;
                if (project.Organizer.HasValue && project.Organizer.Value > 0)
                {
                    organizerUserId = project.Organizer.Value;
                }

                // 更新撤案狀態為 false（恢復案件）
                OFS_CulProjectHelper.updateWithdrawalStatus(projectId, false);

                // 取得操作後的狀態
                string afterStatus = GetProjectCurrentStatus(projectId);

                // 記錄操作到案件歷程
                LogCaseOperation(projectId, "恢復案件", reason, beforeStatus, afterStatus);

                // 發送恢復案件通知
                NotificationHelper.Z2("文化", "CUL", project.ProjectName, reason,
                    project.UserAccount, organizerUserId);
            }
            else if (projectId.Contains("EDC"))
            {
                // 取得操作前的狀態
                string beforeStatus = GetProjectCurrentStatus(projectId);

                // 取得專案資料
                int projectIntId = OFS_EdcProjectHelper.getID(projectId);
                var project = OFS_EdcProjectHelper.get(projectIntId);

                // 取得承辦人的 UserID
                int? organizerUserId = null;
                if (project.Organizer.HasValue && project.Organizer.Value > 0)
                {
                    organizerUserId = project.Organizer.Value;
                }

                // 更新撤案狀態為 false（恢復案件）
                OFS_EdcProjectHelper.updateWithdrawalStatus(projectId, false);

                // 取得操作後的狀態
                string afterStatus = GetProjectCurrentStatus(projectId);

                // 記錄操作到案件歷程
                LogCaseOperation(projectId, "恢復案件", reason, beforeStatus, afterStatus);

                // 發送恢復案件通知
                NotificationHelper.Z2("學校民間", "EDC", project.ProjectName, reason,
                    project.UserAccount, organizerUserId);
            }
            else if (projectId.Contains("MUL"))
            {
                // 取得操作前的狀態
                string beforeStatus = GetProjectCurrentStatus(projectId);

                // 取得專案資料
                int projectIntId = OFS_MulProjectHelper.getID(projectId);
                var project = OFS_MulProjectHelper.get(projectIntId);

                // 取得承辦人的 UserID
                int? organizerUserId = null;
                if (project.Organizer.HasValue && project.Organizer.Value > 0)
                {
                    organizerUserId = project.Organizer.Value;
                }

                // 更新撤案狀態為 false（恢復案件）
                OFS_MulProjectHelper.updateWithdrawalStatus(projectId, false);

                // 取得操作後的狀態
                string afterStatus = GetProjectCurrentStatus(projectId);

                // 記錄操作到案件歷程
                LogCaseOperation(projectId, "恢復案件", reason, beforeStatus, afterStatus);

                // 發送恢復案件通知
                NotificationHelper.Z2("多元", "MUL", project.ProjectName, reason,
                    project.UserAccount, organizerUserId);
            }
            else if (projectId.Contains("LIT"))
            {
                // 取得操作前的狀態
                string beforeStatus = GetProjectCurrentStatus(projectId);

                // 取得專案資料
                int projectIntId = OFS_LitProjectHelper.getID(projectId);
                var project = OFS_LitProjectHelper.get(projectIntId);

                // 取得承辦人的 UserID
                int? organizerUserId = null;
                if (project.Organizer.HasValue && project.Organizer.Value > 0)
                {
                    organizerUserId = project.Organizer.Value;
                }

                // 更新撤案狀態為 false（恢復案件）
                OFS_LitProjectHelper.updateWithdrawalStatus(projectId, false);

                // 取得操作後的狀態
                string afterStatus = GetProjectCurrentStatus(projectId);

                // 記錄操作到案件歷程
                LogCaseOperation(projectId, "恢復案件", reason, beforeStatus, afterStatus);

                // 發送恢復案件通知
                NotificationHelper.Z2("素養", "LIT", project.ProjectName, reason,
                    project.UserAccount, organizerUserId);
            }
            else if (projectId.Contains("ACC"))
            {
                // 取得操作前的狀態
                string beforeStatus = GetProjectCurrentStatus(projectId);

                // 取得專案資料
                int projectIntId = OFS_AccProjectHelper.getID(projectId);
                var project = OFS_AccProjectHelper.get(projectIntId);

                // 取得承辦人的 UserID
                int? organizerUserId = null;
                if (project.Organizer.HasValue && project.Organizer.Value > 0)
                {
                    organizerUserId = project.Organizer.Value;
                }

                // 更新撤案狀態為 false（恢復案件）
                OFS_AccProjectHelper.updateWithdrawalStatus(projectId, false);

                // 取得操作後的狀態
                string afterStatus = GetProjectCurrentStatus(projectId);

                // 記錄操作到案件歷程
                LogCaseOperation(projectId, "恢復案件", reason, beforeStatus, afterStatus);

                // 發送恢復案件通知
                NotificationHelper.Z2("無障礙", "ACC", project.ProjectName, reason,
                    project.UserAccount, organizerUserId);
            }


            // 重新載入資料以取得最新狀態
            LoadData();

            // 清空輸入欄位
            txtRestoreReason.Text = "";
            hdnRestoreProjectId.Value = "";

            ShowMessage("恢復案件成功", true);

            // 更新標籤統計
            UpdateTabStatisticsAndActiveState(hidSelectedStage.Value);

        }
        catch (Exception ex)
        {
            ShowMessage($"恢復案件時發生錯誤：{ex.Message}", false);
        }
    }

    // 處理恢復案件操作的 AJAX 方法 (保留以防其他地方使用)
    // [System.Web.Services.WebMethod]
    // public static string HandleRestore(string projectId)
    // {
    //     try
    //     {
    //         ApplicationChecklistHelper.UpdateWithdrawalStatus(projectId, false);
    //
    //         return "success";
    //     }
    //     catch (Exception ex)
    //     {
    //         return $"恢復案件時發生錯誤：{ex.Message}";
    //     }
    // }

    /// <summary>
    /// 取得案件歷程資料
    /// </summary>
    /// <param name="projectId">計畫ID</param>
    /// <returns>案件歷程資料</returns>
    [System.Web.Services.WebMethod]
    public static object GetCaseHistory(string projectId)
    {
        try
        {
            if (string.IsNullOrEmpty(projectId))
            {
                return new { success = false, message = "ProjectID 不能為空" };
            }

            var historyList = ApplicationChecklistHelper.GetCaseHistoryByProjectId(projectId);

            var result = historyList.Select(h => new
            {
                changeTime = h.ChangeTime.ToMinguoDateTime(),
                userName = h.UserName,
                stageChange = !string.IsNullOrEmpty(h.StageStatusBefore) && !string.IsNullOrEmpty(h.StageStatusAfter)
                    ? $"{h.StageStatusBefore} → {h.StageStatusAfter}"
                    : h.StageStatusAfter,
                description = h.Description
            }).ToList();

            return new { success = true, data = result };
        }
        catch (Exception ex)
        {
            return new { success = false, message = ex.Message };
        }
    }

    /// <summary>
    /// 取得審查意見回覆資料
    /// </summary>
    /// <param name="projectId">計畫ID</param>
    /// <returns>審查意見回覆資料</returns>
    [System.Web.Services.WebMethod]
    public static object GetReviewComments(string projectId)
    {
        try
        {
            if (string.IsNullOrEmpty(projectId))
            {
                return new { success = false, message = "ProjectID 不能為空" };
            }

            // 判斷當前使用者是否為主管單位人員
            bool isSupervisoryUser = IsSupervisoryUser();

            // 取得計畫基本資料
            var projectData = ApplicationChecklistHelper.GetProjectDataForReview(projectId);
            if (projectData == null)
            {
                return new { success = false, message = "找不到計畫資料" };
            }

            var domainReviewComments = new List<object>();
            var technicalReviewComments = new List<object>();

            // 用於審查委員名字對應代稱（統一管理兩個階段的代稱）
            var reviewerAliasMap = new Dictionary<string, string>();
            int aliasCounter = 0;

            // 根據 ProjectID 判斷計畫類型並取得審查意見

            // 科專計畫處理 - 同時查詢領域審查和技術審查
            string Stage1 = "";
            string Stage2 = "";

            if (projectId.Contains("SCI"))
            {
                 Stage1 = "2";
                 Stage2 = "3";
                 // 查詢領域/初審 審查意見
                 var domainCommentsTable = ReviewCheckListHelper.GetSciReviewComments(projectId, Stage1);
                 if (domainCommentsTable != null && domainCommentsTable.Rows.Count > 0)
                 {
                     foreach (DataRow row in domainCommentsTable.Rows)
                     {
                         string reviewerName = row["ReviewerName"]?.ToString() ?? "";

                         // 如果不是主管單位人員，替換審查委員名字
                         if (!isSupervisoryUser && !string.IsNullOrEmpty(reviewerName))
                         {
                             reviewerName = GetReviewerAlias(reviewerName, reviewerAliasMap, ref aliasCounter);
                         }

                         domainReviewComments.Add(new
                         {
                             reviewerReviewID = row["ReviewID"]?.ToString() ?? "",
                             reviewerName = reviewerName,
                             reviewComment = row["ReviewComment"]?.ToString() ?? "",
                             replyComment = row["ReplyComment"]?.ToString() ?? ""
                         });
                     }
                 }

                 // 查詢技術審查/複審 意見
                 var technicalCommentsTable = ReviewCheckListHelper.GetSciReviewComments(projectId, Stage2);
                 if (technicalCommentsTable != null && technicalCommentsTable.Rows.Count > 0)
                 {
                     foreach (DataRow row in technicalCommentsTable.Rows)
                     {
                         string reviewerName = row["ReviewerName"]?.ToString() ?? "";

                         // 如果不是主管單位人員，替換審查委員名字
                         if (!isSupervisoryUser && !string.IsNullOrEmpty(reviewerName))
                         {
                             reviewerName = GetReviewerAlias(reviewerName, reviewerAliasMap, ref aliasCounter);
                         }

                         technicalReviewComments.Add(new
                         {
                             reviewerReviewID = row["ReviewID"]?.ToString() ?? "",
                             reviewerName = reviewerName,
                             reviewComment = row["ReviewComment"]?.ToString() ?? "",
                             replyComment = row["ReplyComment"]?.ToString() ?? ""
                         });
                     }
                 }

            }
            else if (projectId.Contains("CUL"))
            {
                Stage1 = "2";//初審
                Stage2 = "3";//複審
                // 查詢領域/初審 審查意見
                var domainCommentsTable = ReviewCheckListHelper.GetCulturalReviewComments(projectId, Stage1);
                if (domainCommentsTable != null && domainCommentsTable.Rows.Count > 0)
                {
                    foreach (DataRow row in domainCommentsTable.Rows)
                    {
                        string reviewerName = row["ReviewerName"]?.ToString() ?? "";

                        // 如果不是主管單位人員，替換審查委員名字
                        if (!isSupervisoryUser && !string.IsNullOrEmpty(reviewerName))
                        {
                            reviewerName = GetReviewerAlias(reviewerName, reviewerAliasMap, ref aliasCounter);
                        }

                        domainReviewComments.Add(new
                        {
                            reviewerReviewID = row["ReviewID"]?.ToString() ?? "",
                            reviewerName = reviewerName,
                            reviewComment = row["ReviewComment"]?.ToString() ?? "",
                            replyComment = row["ReplyComment"]?.ToString() ?? ""
                        });
                    }
                }

                // 查詢技術審查/複審 意見
                var technicalCommentsTable = ReviewCheckListHelper.GetCulturalReviewComments(projectId, Stage2);
                if (technicalCommentsTable != null && technicalCommentsTable.Rows.Count > 0)
                {
                    foreach (DataRow row in technicalCommentsTable.Rows)
                    {
                        string reviewerName = row["ReviewerName"]?.ToString() ?? "";

                        // 如果不是主管單位人員，替換審查委員名字
                        if (!isSupervisoryUser && !string.IsNullOrEmpty(reviewerName))
                        {
                            reviewerName = GetReviewerAlias(reviewerName, reviewerAliasMap, ref aliasCounter);
                        }

                        technicalReviewComments.Add(new
                        {
                            reviewerReviewID = row["ReviewID"]?.ToString() ?? "",
                            reviewerName = reviewerName,
                            reviewComment = row["ReviewComment"]?.ToString() ?? "",
                            replyComment = row["ReplyComment"]?.ToString() ?? ""
                        });
                    }
                }

            }

            // 即使沒有審查意見資料，仍然回傳計畫基本資料
            var result = new
            {
                projectInfo = new
                {
                    year = projectData.Year,
                    ProjectID = projectData.ProjectID,
                    projectCategory = projectId.Contains("SCI") ? "科專" : projectId.Contains("CUL") ? "文化" : "其他",
                    reviewGroup = projectData.ReviewGroup,
                    projectName = projectData.ProjectName,
                    applicantUnit = projectData.ApplicantUnit
                },
                domainReviewComments = domainReviewComments,
                technicalReviewComments = technicalReviewComments
            };

            return new { success = true, data = result };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得審查意見時發生錯誤：{ex.Message}");
            return new { success = false, message = $"取得審查意見時發生錯誤：{ex.Message}" };
        }
    }

    /// <summary>
    /// 更新審查意見回覆資料
    /// </summary>
    /// <param name="replies">回覆內容</param>
    /// <returns>審查意見回覆資料</returns>
    [System.Web.Services.WebMethod]
    public static object SubmitReply(List<ReplyItem> replies)
    {
        foreach (var item in replies)
        {
            string reviewId = item.reviewId;
            string content = item.replyContent;
            ApplicationChecklistHelper.UpdateReplyContent(reviewId, content);
        }

        return new { success = true, message = "已回覆" };
    }

    /// <summary>
    /// 取得篩選後的完整資料（純後端篩選，前端分頁）
    /// </summary>
    /// <param name="searchText">搜尋關鍵字</param>
    /// <param name="contentKeyword">內容關鍵字</param>
    /// <param name="year">年度</param>
    /// <param name="status">狀態</param>
    /// <param name="stage">階段</param>
    /// <param name="department">申請單位</param>
    /// <param name="reviewer">主管單位</param>
    /// <param name="waitingReply">待回覆</param>
    /// <param name="selectedStage">選中的標籤</param>
    /// <returns>篩選後的完整資料</returns>
    [System.Web.Services.WebMethod]
    public static object GetFilteredData(string searchText = "", string contentKeyword = "",
                                        string year = "", string status = "", string stage = "",
                                        string department = "", string reviewer = "",
                                        bool waitingReply = false, string selectedStage = "")
    {
        try
        {
            // 判斷當前使用者是否為主管機關人員
            bool isSupervisoryUser = IsSupervisoryUser();
            string userAccount = "";

            // 如果不是主管機關人員，只載入自己的案件
            if (!isSupervisoryUser)
            {
                var currentUser = SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo);
                if (currentUser != null)
                {
                    userAccount = currentUser.Account;
                }
            }

            // 載入原始資料
            var originalData = ApplicationChecklistHelper.GetLatestApplicationChecklist(userAccount);


            // 篩選資料
            var filteredData = FilterDataStatic(originalData, searchText, contentKeyword, year,
                                               status, stage, department, reviewer, waitingReply, selectedStage);

            // 將資料轉換為前端需要的格式
            var dataList = filteredData.Select(item => new {
                Year = item.Year ?? "",
                ProjectID = item.ProjectID ?? "",
                ProjectNameTw = item.ProjectNameTw ?? "",
                OrgName = item.OrgName ?? "",
                Category = GetProjectCategoryStatic(item.ProjectID),
                Req_SubsidyAmount = FormatAmountStatic(item.Req_SubsidyAmount),
                Statuses = item.Statuses ?? "",
                StatusesName = item.StatusesName ?? "",
                isWithdrawal = item.isWithdrawal ?? false,
                // 其他需要的欄位
                SupervisoryUnit = item.SupervisoryUnit ?? ""
            }).ToList();

            // 統計各類別數量（基於篩選後的資料，不含標籤類別篩選）
            // 先取得排除標籤類別篩選的資料
            var dataForCounting = FilterDataStatic(originalData, searchText, contentKeyword, year,
                                                   status, stage, department, reviewer, waitingReply, "");

            var categoryCounts = new Dictionary<string, int>
            {
                ["總申請"] = dataForCounting.Count,
                ["科專"] = dataForCounting.Count(x => x.ProjectID?.Contains("SCI") == true),
                ["文化"] = dataForCounting.Count(x => x.ProjectID?.Contains("CUL") == true),
                ["學校民間"] = dataForCounting.Count(x => x.ProjectID?.Contains("EDC") == true),
                ["學校社團"] = dataForCounting.Count(x => x.ProjectID?.Contains("CLB") == true),
                ["多元"] = dataForCounting.Count(x => x.ProjectID?.Contains("MUL") == true),
                ["素養"] = dataForCounting.Count(x => x.ProjectID?.Contains("LIT") == true),
                ["無障礙"] = dataForCounting.Count(x => x.ProjectID?.Contains("ACC") == true)
            };

            return new {
                success = true,
                data = dataList,
                totalRecords = filteredData.Count,
                categoryCounts = categoryCounts
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetFilteredData 錯誤：{ex.Message}");
            return new { success = false, message = $"載入資料時發生錯誤：{ex.Message}" };
        }
    }


    // 靜態方法：篩選資料
    private static List<ReviewChecklistItem> FilterDataStatic(List<ReviewChecklistItem> originalData,
        string searchText, string contentKeyword, string year, string status, string stage,
        string department, string reviewer, bool waitingReply, string selectedStage)
    {
        var filteredList = originalData.AsEnumerable();

        // 申請類別篩選（從標籤點擊）
        if (!string.IsNullOrEmpty(selectedStage) && selectedStage != "總申請")
        {
            filteredList = filteredList.Where(x =>
            {
                var category = "";
                if (!string.IsNullOrEmpty(x.ProjectID))
                {
                    if (x.ProjectID.Contains("SCI")) category = "科專";
                    else if (x.ProjectID.Contains("CUL")) category = "文化";
                    else if (x.ProjectID.Contains("EDC")) category = "學校民間";
                    else if (x.ProjectID.Contains("CLB")) category = "學校社團";
                    else if (x.ProjectID.Contains("MUL")) category = "多元";
                    else if (x.ProjectID.Contains("LIT")) category = "素養";
                    else if (x.ProjectID.Contains("ACC")) category = "無障礙";
                }
                return category == selectedStage;
            });
        }

        // 關鍵字搜尋
        if (!string.IsNullOrEmpty(searchText))
        {
            filteredList = filteredList.Where(x =>
                (x.ProjectID?.Contains(searchText) == true) ||
                (x.ProjectNameTw?.Contains(searchText) == true));
        }

        // 年度篩選
        if (!string.IsNullOrEmpty(year))
        {
            filteredList = filteredList.Where(x => x.Year == year);
        }

        // 狀態篩選
        if (!string.IsNullOrEmpty(status))
        {
            filteredList = filteredList.Where(x => x.StatusesName == status);
        }

        // 階段篩選
        if (!string.IsNullOrEmpty(stage))
        {
            filteredList = filteredList.Where(x => x.Statuses == stage);
        }

        // 申請單位篩選
        if (!string.IsNullOrEmpty(department))
        {
            filteredList = filteredList.Where(x => x.OrgName?.Contains(department) == true);
        }

        // 主管單位篩選
        if (!string.IsNullOrEmpty(reviewer))
        {
            filteredList = filteredList.Where(x => x.SupervisoryUnit?.Contains(reviewer) == true);
        }

        // 待回覆篩選 - 審查意見已提送但申請單位尚未回覆
        if (waitingReply)
        {
            // 取得需要待回覆的 ProjectID 清單
            var waitingReplyProjectIds = ApplicationChecklistHelper.GetWaitingReplyProjectIds();

            if (waitingReplyProjectIds != null && waitingReplyProjectIds.Count > 0)
            {
                filteredList = filteredList.Where(x =>
                    !string.IsNullOrEmpty(x.ProjectID) &&
                    waitingReplyProjectIds.Contains(x.ProjectID));
            }
            else
            {
                // 如果沒有待回覆的項目，返回空列表
                filteredList = filteredList.Where(x => false);
            }
        }

        return filteredList.ToList();
    }


    // 靜態輔助方法
    private static string GetProjectCategoryStatic(string projectId)
    {
        if (string.IsNullOrEmpty(projectId)) return "";

        if (projectId.Contains("SCI")) return "科專";
        else if (projectId.Contains("CUL")) return "文化";
        else if (projectId.Contains("EDC")) return "學校民間";
        else if (projectId.Contains("CLB")) return "學校社團";
        else if (projectId.Contains("MUL")) return "多元";
        else if (projectId.Contains("LIT")) return "素養";
        else if (projectId.Contains("ACC")) return "無障礙";
        return "";
    }

    private static string FormatAmountStatic(string amount)
    {
        if (string.IsNullOrEmpty(amount) || amount == "0")
            return "0";

        if (decimal.TryParse(amount, out decimal value))
        {
            return value.ToString("#,##0");
        }

        return amount;
    }

    /// <summary>
    /// 處理技術審查檔案上傳
    /// </summary>
    protected void btnUploadTechReview_Click(object sender, EventArgs e)
    {
        try
        {
            string projectId = hdnUploadProjectId.Value;

            if (string.IsNullOrEmpty(projectId))
            {
                ShowMessage("系統錯誤：未找到專案資訊", false);
                return;
            }

            if (!fileUploadTechReview.HasFile)
            {
                ShowMessage("請選擇要上傳的檔案", false);
                return;
            }

            var uploadedFile = fileUploadTechReview.PostedFile;

            // 檢查檔案類型
            string fileExtension = Path.GetExtension(uploadedFile.FileName).ToLower();
            if (fileExtension != ".ppt" && fileExtension != ".pptx")
            {
                ShowMessage("僅支援 PPT 或 PPTX 格式的檔案", false);
                return;
            }

            // 檢查檔案大小 (50MB = 52,428,800 bytes)
            if (uploadedFile.ContentLength > 52428800)
            {
                ShowMessage("檔案大小不能超過 50MB", false);
                return;
            }

            // 判斷專案類型
            string projectType = GetProjectTypeFromId(projectId);
            if (string.IsNullOrEmpty(projectType))
            {
                ShowMessage("無法識別專案類型", false);
                return;
            }

            // 建立上傳目錄：UploadFiles\OFS\{SCI或CUL}\{ProjectID}\TechReviewFiles
            string uploadFolder = Server.MapPath($"~/UploadFiles/OFS/{projectType}/{projectId}/TechReviewFiles/");
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            // 產生新的檔案名稱 (使用專案ID + 時間戳記 + 原始副檔名)
            string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string newFileName = $"{projectId}_TechReview_{timeStamp}{fileExtension}";
            string filePath = Path.Combine(uploadFolder, newFileName);

            // 如果已存在舊檔案，先刪除
            string[] existingFiles = Directory.GetFiles(uploadFolder, $"{projectId}_TechReview_*");
            foreach (string existingFile in existingFiles)
            {
                try
                {
                    File.Delete(existingFile);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"刪除舊檔案失敗：{ex.Message}");
                }
            }

            // 儲存新檔案
            uploadedFile.SaveAs(filePath);

            // 記錄到資料庫 (可選，如果需要的話)
            LogTechReviewFileUpload(projectId, uploadedFile.FileName, newFileName);

            // 清空上傳控制項和隱藏欄位
            hdnUploadProjectId.Value = "";

            ShowMessage("檔案上傳成功", true);

            // 觸發前端重新載入資料
            ClientScript.RegisterStartupScript(this.GetType(), "CloseUploadModal",
                "bootstrap.Modal.getInstance(document.getElementById('techReviewUploadModal')).hide();", true);
        }
        catch (Exception ex)
        {
            ShowMessage($"上傳檔案時發生錯誤：{ex.Message}", false);
        }
    }

    /// <summary>
    /// 記錄技術審查檔案上傳到案件歷程
    /// </summary>
    private void LogTechReviewFileUpload(string projectId, string originalFileName, string savedFileName)
    {
        try
        {
            string description = $"上傳技術審查檔案：{originalFileName}";
            LogCaseOperation(projectId, "上傳檔案", description);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"記錄檔案上傳失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 檢查專案是否已有技術審查檔案
    /// </summary>
    [System.Web.Services.WebMethod]
    public static object CheckTechReviewFile(string projectId)
    {
        try
        {
            if (string.IsNullOrEmpty(projectId))
            {
                return new { success = false, message = "ProjectID 不能為空" };
            }

            // 判斷專案類型
            string projectType = GetProjectTypeFromId(projectId);
            if (string.IsNullOrEmpty(projectType))
            {
                return new { success = false, message = "無法識別專案類型" };
            }

            string uploadFolder = HttpContext.Current.Server.MapPath($"~/UploadFiles/OFS/{projectType}/{projectId}/TechReviewFiles/");

            if (Directory.Exists(uploadFolder))
            {
                string[] files = Directory.GetFiles(uploadFolder, $"{projectId}_TechReview_*");

                if (files.Length > 0)
                {
                    string filePath = files[0]; // 取最新的檔案
                    string fileName = Path.GetFileName(filePath);

                    // 從檔案名稱中提取原始檔案資訊
                    string displayName = ExtractOriginalFileName(fileName);

                    return new {
                        success = true,
                        hasFile = true,
                        fileName = displayName,
                        savedFileName = fileName
                    };
                }
            }

            return new { success = true, hasFile = false };
        }
        catch (Exception ex)
        {
            return new { success = false, message = ex.Message };
        }
    }

    /// <summary>
    /// 從儲存的檔案名稱中提取顯示用的檔案名稱
    /// </summary>
    private static string ExtractOriginalFileName(string savedFileName)
    {
        // 儲存格式: {projectId}_TechReview_{timeStamp}{extension}
        // 這裡簡化處理，只返回副檔名資訊
        string extension = Path.GetExtension(savedFileName);
        return $"技術審查檔案{extension}";
    }



    /// <summary>
    /// 從專案ID判斷專案類型
    /// </summary>
    private static string GetProjectTypeFromId(string projectId)
    {
        if (string.IsNullOrEmpty(projectId))
            return null;

        if (projectId.Contains("SCI"))
            return "SCI";
        else if (projectId.Contains("CUL"))
            return "CUL";

        return null; // 目前只支援 SCI 和 CUL
    }

    /// <summary>
    /// 判斷當前使用者是否為主管單位人員
    /// </summary>
    /// <returns>true: 是主管單位人員, false: 不是主管單位人員</returns>
    private static bool IsSupervisoryUser()
    {
        try
        {
            var currentUser = SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo);

            // 檢查角色名稱是否包含主管單位相關角色
            var supervisoryRoles = new[] { "主管單位人員", "主管單位窗口", "系統管理者" };

            foreach (var role in supervisoryRoles)
            {
                if (currentUser.OFS_RoleName.Contains(role))
                {
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"判斷使用者角色時發生錯誤：{ex.Message}");
            return false; // 發生錯誤時預設為非主管單位人員
        }
    }

    /// <summary>
    /// 取得審查委員的代稱（委員A、委員B、委員C...）
    /// </summary>
    /// <param name="originalName">原始審查委員名字</param>
    /// <param name="aliasMap">名字對應代稱的字典</param>
    /// <param name="counter">代稱計數器</param>
    /// <returns>審查委員代稱</returns>
    private static string GetReviewerAlias(string originalName, Dictionary<string, string> aliasMap, ref int counter)
    {
        if (aliasMap.ContainsKey(originalName))
        {
            // 如果已經有對應的代稱，直接返回
            return aliasMap[originalName];
        }
        else
        {
            // 產生新的代稱：委員A、委員B、委員C...
            char aliasLetter = (char)('A' + counter);
            string alias = $"委員{aliasLetter}";

            aliasMap[originalName] = alias;
            counter++;

            return alias;
        }
    }

}

public class ReplyItem
{
    public string reviewId { get; set; }
    public string replyContent { get; set; }
}
