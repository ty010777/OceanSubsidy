using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class OFSMaster : System.Web.UI.MasterPage
{
    // 公開屬性供 ASPX 頁面使用
    public string UserName { get; set; }
    public string UnitName { get; set; }
    public int ApplicationPendingCount { get; set; }
    public int InprogressPendingCount { get; set; }
    public int ApplicationReviewPendingCount { get; set; }
    public int InprogressReviewPendingCount { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // 取得 Session timeout 設定（分鐘）
            int sessionTimeoutMinutes = Session.Timeout;

            // 每次頁面載入都更新最後活動時間（這會重設 ASP.NET Session 的過期時間）
            Session["LastActivityTime"] = DateTime.Now;

            // 將 Session timeout 資訊傳遞到前端 JavaScript
            Page.ClientScript.RegisterStartupScript(this.GetType(), "sessionTimeout",
                $@"var sessionTimeoutMinutes = {sessionTimeoutMinutes};
                   var lastActivityTime = new Date().getTime();", true);
        }

        // 取得當前使用者資訊
        try
        {
            UserName = CurrentUser.UserName ?? "訪客";
            UnitName = CurrentUser.UnitName ?? "未設定單位";

            // 取得小鈴鐺提醒的數量
            string userAccount = CurrentUser.Account ?? "";
            ApplicationPendingCount = AlarmBellHelper.GetApplicationPendingCount(userAccount);
            InprogressPendingCount = AlarmBellHelper.GetInprogressPendingCount(userAccount);

            // 僅主管單位人員、主管單位窗口、系統管理者可看到審核數量
            if (CurrentUser.IsOrganizer || CurrentUser.IsSupervisor || CurrentUser.IsSysAdmin)
            {
                ApplicationReviewPendingCount = AlarmBellHelper.GetApplicationReviewPendingCount(
                    CurrentUser.IsOrganizer,
                    CurrentUser.IsSupervisor,
                    CurrentUser.IsSysAdmin,
                    userAccount,
                    CurrentUser.UnitName ?? ""
                );
                InprogressReviewPendingCount = AlarmBellHelper.GetInprogressReviewPendingCount(
                    CurrentUser.IsOrganizer,
                    CurrentUser.IsSupervisor,
                    CurrentUser.IsSysAdmin,
                    userAccount,
                    CurrentUser.UnitName ?? ""
                );
            }
        }
        catch
        {
            // 如果取得使用者資訊失敗，導向登入頁面
            Response.Redirect("~/Login.aspx");
        }
    }
}