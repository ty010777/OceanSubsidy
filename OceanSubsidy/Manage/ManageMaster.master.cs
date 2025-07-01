using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class Manage_ManageMaster : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            var userInfo = SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo);
            if (userInfo != null)
            {
                lblUserName.Text = userInfo.UserName;
                lblUnit.Text = userInfo.UnitName;
            }

            HighlightCurrentMenu();
        }
    }

    /// <summary>
    /// 顯示一段訊息到共用的 messageModal
    /// </summary>
    /// <param name="message">要顯示的文字（可含 HTML）</param>
    public void ShowMessageModal(string message)
    {
        ltlMsgModalBody.Text = message;
        ScriptManager.RegisterStartupScript(
          Page, GetType(),
          "showMsg",
          "showModal('messageModal');",
          true);
    }


    private void HighlightCurrentMenu()
    {
        // 取得目前檔名
        var current = VirtualPathUtility
          .GetFileName(Request.AppRelativeCurrentExecutionFilePath)
          .ToLowerInvariant();

        // 找到 menubar 裡所有 li
        foreach (Control ctrl in menubar.Controls)
        {
            if (ctrl is HtmlGenericControl li && li.Attributes["data-page"] != null)
            {
                var page = li.Attributes["data-page"].ToLower();
                if (page == current)
                    li.Attributes["class"] = (li.Attributes["class"] + " active").Trim();
            }
        }
    }

}
