using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class OFSMaster : System.Web.UI.MasterPage
{
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
    }
}