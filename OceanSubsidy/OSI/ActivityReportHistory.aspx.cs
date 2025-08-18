using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.App;
using GS.Extension;

public partial class OSI_ActivityReportHistory : System.Web.UI.Page
{
    private SessionHelper.UserInfoClass UserInfo
    {
        get => SessionHelper.Get<SessionHelper.UserInfoClass>(
                  SessionHelper.UserInfo)
                ?? new SessionHelper.UserInfoClass();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // 檢查是否有 ReportID 參數
            int reportId = 0;
            if (Request.QueryString["ReportID"] != null && int.TryParse(Request.QueryString["ReportID"], out reportId))
            {
                // 使用共用權限檢查類別進行權限驗證
                var permissionResult = OSIReportPermissionHelper.CheckReportPermission(reportId, UserInfo);

                if (!permissionResult.HasPermission)
                {
                    // 記錄未授權存取嘗試
                    OSIReportPermissionHelper.LogUnauthorizedAccess(
                        UserInfo,
                        reportId.ToString(),
                        "Report",
                        Request.UserHostAddress
                    );

                    // 處理無權限的情況
                    HandleUnauthorizedAccess(permissionResult.DeniedReason);
                    return;
                }

                LoadHistoryList(reportId);
            }
            else
            {
                Response.Redirect("~/OSI/ActivityReports.aspx");
            }
        }
    }

    /// <summary>
    /// 載入歷程清單
    /// </summary>
    private void LoadHistoryList(int reportId)
    {
        try
        {
            // 使用 Helper 查詢該報告的所有歷史記錄
            var historyTable = OSIActivityReportsHistoryHelper.GetReportHistory(reportId);

            if (historyTable != null && historyTable.Rows.Count > 0)
            {
                ddlHistory.Items.Clear();

                foreach (DataRow row in historyTable.Rows)
                {
                    long historyId = Convert.ToInt64(row["HistoryID"]);
                    DateTime auditAt = Convert.ToDateTime(row["AuditAt"]);
                    string correctionNotes = row["CorrectionNotes"]?.ToString() ?? "";

                    // 截斷過長的修正說明
                    if (correctionNotes.Length > 30)
                    {
                        correctionNotes = correctionNotes.Substring(0, 30) + "...";
                    }

                    // 組合顯示文字
                    string displayText = string.Format("{0}",
                        DateTimeHelper.ToMinguoDateTime(auditAt));

                    if (!string.IsNullOrEmpty(correctionNotes))
                    {
                        displayText += " - " + correctionNotes;
                    }

                    ddlHistory.Items.Add(new ListItem(displayText, historyId.ToString()));
                }

                // 儲存 ReportID 到 ViewState
                ViewState["ReportID"] = reportId;

                // 自動選擇並載入第一筆（最新）資料
                if (ddlHistory.Items.Count > 0)
                {
                    ddlHistory.SelectedIndex = 0;
                    LoadHistoryDetail(Convert.ToInt32(ddlHistory.SelectedValue));
                }
            }
            else
            {
                // 如果沒有歷史記錄，返回列表頁
                Response.Redirect("ActivityReports.aspx");
            }
        }
        catch (Exception ex)
        {
            // 記錄錯誤並返回列表頁
            System.Diagnostics.Debug.WriteLine("載入歷程清單錯誤：" + ex.Message);
            Response.Redirect("ActivityReports.aspx");
        }
    }

    /// <summary>
    /// 歷程選擇變更
    /// </summary>
    protected void ddlHistory_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(ddlHistory.SelectedValue))
        {
            int historyId;
            if (int.TryParse(ddlHistory.SelectedValue, out historyId))
            {
                LoadHistoryDetail(historyId);
            }
        }
        else
        {
            phHistoryContent.Visible = false;
        }
    }

    /// <summary>
    /// 載入歷程詳細資料
    /// </summary>
    private void LoadHistoryDetail(int historyId)
    {
        try
        {
            // 顯示歷程內容
            phHistoryContent.Visible = ReportFormHistory.LoadHistoryData(historyId);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("載入歷程詳細資料錯誤：" + ex.Message);
        }
    }

    /// <summary>
    /// 返回按鈕
    /// </summary>
    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("ActivityReports.aspx");
    }

    /// <summary>
    /// 處理未授權存取的情況
    /// </summary>
    /// <param name="deniedReason">拒絕原因</param>
    private void HandleUnauthorizedAccess(string deniedReason)
    {
        try
        {
            // 顯示錯誤訊息並重新導向
            string safeReason = HttpUtility.JavaScriptStringEncode(deniedReason);
            string returnUrl = ResolveUrl("~/OSI/ActivityReports.aspx");
            string safeReturnUrl = HttpUtility.JavaScriptStringEncode(returnUrl);

            string script = $@"
                showGlobalMessage('{safeReason}');
                setTimeout(function() {{
                    window.location.href = '{safeReturnUrl}';
                }}, 2000);
            ";

            ScriptManager.RegisterStartupScript(
                this,
                this.GetType(),
                "unauthorizedAccess",
                script,
                true
            );
        }
        catch (Exception ex)
        {
            // 如果發生錯誤，直接重新導向
            System.Diagnostics.Debug.WriteLine($"HandleUnauthorizedAccess Error: {ex.Message}");
            Response.Redirect("~/OSI/ActivityReports.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}