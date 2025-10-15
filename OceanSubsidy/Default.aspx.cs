using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.App;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // 是否顯示"帳號權限管理"
            string userId = SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo).UserID;
            manage.Visible = SysPermissionHelper.HasAnyChildPermsWithOSIByID(userId, "SYS_MANAGE");            
        }
    }

    protected void btnSystem1_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/OSI/ActivityReports.aspx");
    }

    protected void btnSystem2_Click(object sender, EventArgs e)
    {
          
    }

    protected void btnSystem3_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Manage/Users.aspx");
    }

}