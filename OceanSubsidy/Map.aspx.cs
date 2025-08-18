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
                string wkt = GetReportGeoData(reportIdStr);
                InitialWKT3826_JS = "'" + wkt.Replace("'", "\\'").Replace("\r", "").Replace("\n", "") + "'";
            }

            string historyIdStr = Request.QueryString["historyId"];
            if (!string.IsNullOrEmpty(historyIdStr))
            {
                string wkt = OSIActivityReportsHistoryHelper.QueryGeoDataByID(historyIdStr);
                InitialWKT3826_JS = "'" + wkt.Replace("'", "\\'").Replace("\r", "").Replace("\n", "") + "'";
            }

            // 取得地圖資料BY AssessmentId
            string assessmentIdStr = Request.QueryString["assessmentId"];
            if (!string.IsNullOrEmpty(assessmentIdStr))
            {
                string wkt = GetAssessmentGeoData(assessmentIdStr);
                InitialWKT3826_JS = "'" + wkt.Replace("'", "\\'").Replace("\r", "").Replace("\n", "") + "'";
            }
        }

    }

    /// <summary>
    /// 取得報告的所有圖徵資料並組合成 JSON 格式
    /// </summary>
    private string GetReportGeoData(string reportId)
    {
        var geomsTable = OSIGeomHelper.QueryByReportID(reportId);
        
        if (geomsTable != null && geomsTable.Rows.Count > 0)
        {
            var result = new
            {
                type = "FeatureCollection",
                features = geomsTable.Rows.Cast<DataRow>().Select(row => new
                {
                    id = row["GeomID"]?.ToString() ?? "",
                    name = row["GeomName"]?.ToString() ?? "",
                    wkt = row["GeoData"]?.ToString() ?? ""
                }).ToArray()
            };
            
            return Newtonsoft.Json.JsonConvert.SerializeObject(result);
        }
        
        return "";
    }

    /// <summary>
    /// 取得研究船風險檢核的所有圖徵資料並組合成 JSON 格式
    /// </summary>
    private string GetAssessmentGeoData(string assessmentId)
    {
        if (int.TryParse(assessmentId, out int id))
        {
            var geomsTable = OSIVesselGeomHelper.QueryByAssessmentId(id);
            
            if (geomsTable != null && geomsTable.Rows.Count > 0)
            {
                var result = new
                {
                    type = "FeatureCollection",
                    features = geomsTable.Rows.Cast<DataRow>().Select(row => new
                    {
                        id = row["GeomID"]?.ToString() ?? "",
                        name = row["GeomName"]?.ToString() ?? "",
                        wkt = row["GeoData"]?.ToString() ?? ""
                    }).ToArray()
                };
                
                return Newtonsoft.Json.JsonConvert.SerializeObject(result);
            }
        }
        
        return "";
    }
}