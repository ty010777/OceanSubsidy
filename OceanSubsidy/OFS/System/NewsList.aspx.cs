using System;

public partial class Admin_NewsList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!CurrentUser.IsSupervisor && !CurrentUser.IsSysAdmin)
        {
            Response.Redirect(Page.ResolveUrl("~/OFS/Home.aspx"), false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
