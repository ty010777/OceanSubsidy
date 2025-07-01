using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Map : System.Web.UI.Page
{
    protected string InitialWKT3826_JS = "null";
    protected string ppID = WebConfigurationManager.AppSettings["defaultPPID"];
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(Request["ppID"]))
        {
            ppID = HttpUtility.UrlEncode(Request["ppID"].Trim());
        }

        if (!IsPostBack)
        {
            // 取得地圖資料BY ReportID
            string reportIdStr = Request.QueryString["reportId"];
            if (!string.IsNullOrEmpty(reportIdStr))
            {
                string wkt = OSIActivityReportsHelper.QueryGeoDataByID(reportIdStr);
                InitialWKT3826_JS = "'" + wkt.Replace("'", "\\'").Replace("\r", "").Replace("\n", "") + "'";
            }
        }

    }
}