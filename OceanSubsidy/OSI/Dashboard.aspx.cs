using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.Data;
using GS.OCA_OceanSubsidy.Entity;

public partial class OSI_Dashboard : System.Web.UI.Page
{
    // 在 aspx 中用來注入呼叫 renderCharts 的 JS
    protected string ChartDataScript = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindPeriodYears();
            BindPeriodQuarters();
            BindPieDropdown();
            
            // 初始載入時顯示圖表資料
            if (ddlPeriodQuarter.Items.Count > 0)
            {
                BindAllChartData();
                
                ScriptManager.RegisterStartupScript(
                    this,
                    this.GetType(),
                    "initialRenderCharts",
                    ChartDataScript,
                    true
                );
            }
        }
    }

    private void BindPeriodYears()
    {
        DataTable dtYears = OSIDataPeriodsHelper.QueryAllYears();

        ddlPeriodYear.Items.Clear();
        foreach (DataRow r in dtYears.Rows)
        {
            string year = r["PeriodYear"].ToString();
            ddlPeriodYear.Items.Add(new ListItem($"{year}年", year));
        }
        if (ddlPeriodYear.Items.Count > 0)
            ddlPeriodYear.SelectedIndex = 0;
    }

    protected void ddlPeriodYear_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindPeriodQuarters();
    }

    private void BindPeriodQuarters()
    {
        string oldText = ddlPeriodQuarter.SelectedItem?.Text;
        var year = ddlPeriodYear.SelectedValue;

        DataTable dtQuarters = OSIDataPeriodsHelper.QueryQuartersByYear(year);

        ddlPeriodQuarter.Items.Clear();
        foreach (DataRow r in dtQuarters.Rows)
        {
            string quarterText = r["PeriodQuarter"].ToString();
            string periodID = r["PeriodID"].ToString();
            ddlPeriodQuarter.Items.Add(new ListItem(quarterText, periodID));
        }

        ListItem found = ddlPeriodQuarter.Items
        .Cast<ListItem>()
        .FirstOrDefault(i => i.Text == oldText);

        if (found != null)
        {
            ddlPeriodQuarter.SelectedValue = found.Value;
        }
        else if (ddlPeriodQuarter.Items.Count > 0)
        {
            ddlPeriodQuarter.SelectedIndex = 0;
        }
    }

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        BindAllChartData();

        ScriptManager.RegisterStartupScript(
            this,
            this.GetType(),
            "renderCharts",
            ChartDataScript,
            true
        );
    }

    // 綁定「比例分析」下拉
    private void BindPieDropdown()
    {
        ddlPie.Items.Clear();
        foreach (PieCategory cat in Enum.GetValues(typeof(PieCategory)))
        {
            ddlPie.Items.Add(
                new ListItem(cat.ToString(), ((int)cat).ToString())
            );
        }
        ddlPie.SelectedIndex = 0;
    }


    protected void ddlPie_SelectedIndexChanged(object sender, EventArgs e)
    {
        // 只要 PostBack，Page_Load 會再呼叫 BindAllChartData()
    }

    // 撈四張圖的資料，並組成一段 JS 呼叫語法
    private void BindAllChartData()
    {
        var jss = new JavaScriptSerializer();

        var yearData = FetchYearData();
        var pieData = FetchPieData(ddlPeriodQuarter.SelectedValue, ddlPie.SelectedValue);
        var regionData = FetchRegionData(ddlPeriodQuarter.SelectedValue);
        var unitData = FetchUnitData(ddlPeriodQuarter.SelectedValue);

        //– 組成 renderCharts([...], [...], [...], [...]);
        var sb = new StringBuilder();
        sb.Append("renderCharts(")
          .Append(jss.Serialize(yearData)).Append(",")
          .Append(jss.Serialize(pieData)).Append(",")
          .Append(jss.Serialize(regionData)).Append(",")
          .Append(jss.Serialize(unitData)).Append(");");

        ChartDataScript = sb.ToString();
    }

    #region --- Fetch Data Methods ---
    // 1. 近三年季度案件數
    private List<QuarterData> FetchYearData()
    {
        var quarterList = new List<string> { "第一季", "第二季", "第三季", "第四季" };
        int thisYear = DateTime.Now.Year - 1911; // 民國年轉換
        var yearList = new List<int> { thisYear, thisYear - 1, thisYear - 2 };
        var datas = new List<QuarterData>();

        quarterList.ForEach(quarter =>
        {
            QuarterData data = new QuarterData();
            data.Quarter = quarter;
            List<int> counts = new List<int>();

            for (int i = 0; i < yearList.Count; i++)
            {
                var periodID = OSIDataPeriodsHelper.QueryIDByYearAndQuarter(yearList[i], quarter);
                counts.Add(OSIActivityReportsHelper.CountByPeriodIDWithOutCopy(periodID.ToString()));
            }
            data.ThisYear = counts[0];  // 今年該季案件數
            data.LastYear = counts[1];  // 去年該季案件數
            data.BeforeLastYear = counts[2];  // 前年該季案件數
            datas.Add(data);
        });

        return datas;
    }

    // 2. 比例分析
    private List<PieItem> FetchPieData(string periodID, string categoryID)
    {
        var datas = new List<PieItem>();
        GisTable tbl = new GisTable();
        if (categoryID == "0")
        {
            // 研究調查項目
            tbl = OSIActivityReportsHelper.QueryResearchDashboard(periodID);
        }
        else if (categoryID == "1")
        {
            // 填報單位
            tbl = OSIActivityReportsHelper.QueryUnitPercentDashboard(periodID);
        }
        else if (categoryID == "2")
        {
            // 活動性質
            tbl = OSIActivityReportsHelper.QueryNatureDashboard(periodID);
        }
        else if (categoryID == "3")
        {
            // 載具類別
            tbl = OSIActivityReportsHelper.QueryCarrierTypeDashboard(periodID);
        }

        foreach (DataRow row in tbl.Rows)
        {
            datas.Add(new PieItem
            {
                Name = row["Name"].ToString(),
                Value = Convert.ToInt32(row["Num"])
            });
        }

        if (tbl.Rows.Count == 0)
        {
            datas.Add(new PieItem { Name = "無資料", Value = 0 });
        }


        return datas;
    }

    [WebMethod]
    public static List<PieItem> GetPieChartData(string periodID, string categoryID)
    {
        var OSI_Dashboard = new OSI_Dashboard();
        return OSI_Dashboard.FetchPieData(periodID, categoryID);
    }

    // 3. 行政區堆疊長條（回傳 RegionName + 多個 Series）
    private List<RegionSeries> FetchRegionData(string periodID)
    {
        var regionSeriesList = new List<RegionSeries>();
        var tbl = OSIActivityReportsHelper.QueryRegionDashboard(periodID);
        
        // 按縣市分組
        var regionGroups = tbl.AsEnumerable()
            .GroupBy(row => row.Field<string>("RegionName"))
            .ToList();

        // 取得所有可能的系列名稱（單位名稱）
        var allSeriesNames = tbl.AsEnumerable()
            .Select(row => row.Field<string>("SeriesName"))
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        foreach (var regionGroup in regionGroups)
        {
            var regionSeries = new RegionSeries
            {
                RegionName = regionGroup.Key,
                Series = new List<SeriesItem>()
            };

            // 為每個系列名稱創建數據項，沒有數據的設為 0
            foreach (var seriesName in allSeriesNames)
            {
                var seriesData = regionGroup.FirstOrDefault(r => r.Field<string>("SeriesName") == seriesName);
                var value = seriesData != null ? Convert.ToInt32(seriesData["Value"]) : 0;
                
                if (value > 0) // 只加入有數據的項目
                {
                    regionSeries.Series.Add(new SeriesItem
                    {
                        Name = seriesName,
                        Value = value
                    });
                }
            }

            if (regionSeries.Series.Count > 0) // 只加入有數據的區域
            {
                regionSeriesList.Add(regionSeries);
            }
        }

        return regionSeriesList;
    }

    // 4. 單位填報情形（回傳 UnitName + 各縣市 Count）
    private List<UnitSeries> FetchUnitData(string periodID)
    {
        var unitSeriesList = new List<UnitSeries>();
        var tbl = OSIActivityReportsHelper.QueryUnitDashboard(periodID);
        
        // 按單位分組
        var unitGroups = tbl.AsEnumerable()
            .GroupBy(row => row.Field<string>("UnitName"))
            .ToList();

        // 取得所有可能的縣市名稱
        var allCounties = tbl.AsEnumerable()
            .Select(row => row.Field<string>("CountyName"))
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        foreach (var unitGroup in unitGroups)
        {
            var unitSeries = new UnitSeries
            {
                UnitName = unitGroup.Key,
                Series = new List<SeriesYear>()
            };

            // 為每個縣市創建數據項
            foreach (var county in allCounties)
            {
                var countyData = unitGroup.FirstOrDefault(r => r.Field<string>("CountyName") == county);
                var count = countyData != null ? Convert.ToInt32(countyData["ActivityCount"]) : 0;
                
                if (count > 0) // 只加入有數據的縣市
                {
                    unitSeries.Series.Add(new SeriesYear
                    {
                        Year = county, // 使用縣市名稱而非年份
                        Count = count
                    });
                }
            }

            if (unitSeries.Series.Count > 0) // 只加入有數據的單位
            {
                unitSeriesList.Add(unitSeries);
            }
        }

        return unitSeriesList;
    }
    #endregion

    private enum PieCategory
    {
        研究調查項目,
        填報單位,
        活動性質,
        載具類別
    }
}

#region --- DTO Classes ---
public class QuarterData
{
    public string Quarter { get; set; }
    public int ThisYear { get; set; }  // 今年該季案件數
    public int LastYear { get; set; }  // 去年該季案件數
    public int BeforeLastYear { get; set; }  // 前年該季案件數
}
public class PieItem
{
    public string Name;
    public int Value;
}
public class SeriesItem
{
    public string Name;
    public int Value;
}
public class RegionSeries
{
    public string RegionName;
    public List<SeriesItem> Series;
}
public class UnitSeries
{
    public string UnitName;
    public List<SeriesYear> Series;
}
public class SeriesYear
{
    public string Year;
    public int Count;
}
#endregion
