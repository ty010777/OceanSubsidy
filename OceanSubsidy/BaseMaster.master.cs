using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class BaseMaster : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // 將 AppRootPath 設定值輸出到頁面中
        string appRootPath = ConfigurationManager.AppSettings["AppRootPath"] ?? "";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "AppRootPath", 
            $"window.AppRootPath = '{appRootPath}';", true);
    }
}
