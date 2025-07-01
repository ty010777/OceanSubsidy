using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class OSI_ActivityReportDetail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // 如果 URL 有 ?id=xx 就切到編輯模式
            if (int.TryParse(Request.QueryString["id"], out var id) && id > 0)
            {
                ReportForm.ReportID = id;
            }
            else
            {
                ReportForm.IsNew = true;
            }
        }
    }

    
}