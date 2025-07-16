using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Logout : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // 清除所有 Session
        Session.Clear();
        Session.Abandon();

        // 清除認證 Cookie (如果有使用)
        if (Request.Cookies["ASP.NET_SessionId"] != null)
        {
            HttpCookie sessionCookie = new HttpCookie("ASP.NET_SessionId");
            sessionCookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(sessionCookie);
        }

        // 導向到登入頁面，並帶上登出成功的參數
        Response.Redirect("~/Login.aspx?msg=logout", false);
        Context.ApplicationInstance.CompleteRequest();
    }
}