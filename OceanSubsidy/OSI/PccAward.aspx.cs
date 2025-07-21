using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.Data;
using GS.OCA_OceanSubsidy.Entity;

public partial class OSI_PccAward : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // 中華民國月曆需要在頁面載入後才設定readonly
            txtAwardDateFrom.Attributes.Add("readonly", "readonly");
            txtAwardDateTo.Attributes.Add("readonly", "readonly");
            
            // 獲取並設定最新的決標日期
            SetLatestAwardDate();
        }
    }
    
    /// <summary>
    /// 設定最新的決標日期顯示
    /// </summary>
    private void SetLatestAwardDate()
    {
        DateTime? latestDate = OSIPccAwardDataHelper.GetLatestAwardDate();
        
        // 尋找 UpdatePanel 內的 Label 控制項
        Label lblLatestAwardDate = (Label)upList.ContentTemplateContainer.FindControl("lblLatestAwardDate");
        
        if (lblLatestAwardDate != null)
        {
            if (latestDate.HasValue)
            {
                // 轉換為民國年格式
                string rocDate = FormatRocDate(latestDate.Value);
                // 設定 Label 顯示文字
                lblLatestAwardDate.Text = rocDate;
            }
            else
            {
                lblLatestAwardDate.Text = "無資料";
            }
        }
    }

    /// <summary>
    /// 載入資料
    /// </summary>
    private void LoadData()
    {
        var data = OSIPccAwardDataHelper.QueryPccAwardData(
            null, null, null, null, null);
        
        // 綁定資料
        lvPccAward.DataSource = data;
        lvPccAward.DataBind();
    }

    /// <summary>
    /// 查詢按鈕點擊事件
    /// </summary>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        SearchData();
        // 更新最新日期顯示
        SetLatestAwardDate();
    }

    /// <summary>
    /// 執行查詢
    /// </summary>
    private void SearchData()
    {
        // 取得查詢條件
        DateTime? awardDateFrom = null;
        DateTime? awardDateTo = null;
        decimal? priceFrom = null;
        decimal? priceTo = null;

        // 處理日期（中華民國年轉西元年）
        if (!string.IsNullOrWhiteSpace(txtAwardDateFrom.Text))
        {
            var rocDate = txtAwardDateFrom.Text.Trim();
            awardDateFrom = ConvertRocToAD(rocDate);
        }
        if (!string.IsNullOrWhiteSpace(txtAwardDateTo.Text))
        {
            var rocDate = txtAwardDateTo.Text.Trim();
            awardDateTo = ConvertRocToAD(rocDate);
        }

        // 處理金額
        if (!string.IsNullOrWhiteSpace(txtPriceFrom.Text))
        {
            decimal.TryParse(txtPriceFrom.Text, out decimal tempPrice);
            if (tempPrice > 0) priceFrom = tempPrice;
        }
        if (!string.IsNullOrWhiteSpace(txtPriceTo.Text))
        {
            decimal.TryParse(txtPriceTo.Text, out decimal tempPrice);
            if (tempPrice > 0) priceTo = tempPrice;
        }

        // 取得關鍵字
        string keyword = txtKeyword.Text.Trim();

        // 查詢資料
        var data = OSIPccAwardDataHelper.QueryPccAwardData(
            awardDateFrom, awardDateTo, 
            priceFrom, priceTo, 
            keyword);

        // 綁定資料
        lvPccAward.DataSource = data;
        lvPccAward.DataBind();
    }

    /// <summary>
    /// 綁定資料
    /// </summary>
    private void BindData()
    {
        // 判斷是否有查詢條件
        if (string.IsNullOrWhiteSpace(txtKeyword.Text) && 
            string.IsNullOrWhiteSpace(txtAwardDateFrom.Text) && 
            string.IsNullOrWhiteSpace(txtAwardDateTo.Text) &&
            string.IsNullOrWhiteSpace(txtPriceFrom.Text) && 
            string.IsNullOrWhiteSpace(txtPriceTo.Text))
        {
            LoadData();
        }
        else
        {
            SearchData();
        }
    }

    /// <summary>
    /// ListView 命令事件
    /// </summary>
    protected void lvPccAward_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.CommandName == "ViewDetail")
        {
            string url = e.CommandArgument.ToString();
            if (!string.IsNullOrWhiteSpace(url))
            {
                // 在新視窗開啟政府採購網頁面
                ScriptManager.RegisterStartupScript(this, GetType(), 
                    "openWindow", $"window.open('{url}', '_blank');", true);
            }
        }
    }

    /// <summary>
    /// 分頁變更事件
    /// </summary>
    protected void lvPccAward_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
    {
        dpPccAward.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
        BindData();
        // 更新最新日期顯示
        SetLatestAwardDate();
    }

    /// <summary>
    /// DataPager PreRender 事件（處理分頁樣式）
    /// </summary>
    protected void dpPccAward_PreRender(object sender, EventArgs e)
    {
        // 處理分頁按鈕樣式
        if (dpPccAward.Controls.Count < 2) return;

        var container = dpPccAward.Controls[1];
        foreach (Control c in container.Controls)
        {
            if (c is Button btn && btn.Text.Trim() == "...")
            {
                btn.CssClass = "pagination-item ellipsis";
            }
        }
    }

    /// <summary>
    /// 格式化金額顯示
    /// </summary>
    protected string FormatPrice(object price)
    {
        if (price == null || string.IsNullOrWhiteSpace(price.ToString()))
            return "無資料";

        string priceStr = price.ToString();
        
        // 如果已經包含逗號，直接返回
        if (priceStr.Contains(","))
            return priceStr;

        // 嘗試解析為數字並格式化
        decimal priceDecimal;
        if (decimal.TryParse(priceStr.Replace("元", "").Replace(",", ""), out priceDecimal))
        {
            return priceDecimal.ToString("#,##0");
        }

        return priceStr;
    }

    /// <summary>
    /// 格式化民國年日期
    /// </summary>
    protected string FormatRocDate(object date)
    {
        if (date == null || !(date is DateTime))
            return "無資料";

        DateTime dt = (DateTime)date;
        int rocYear = dt.Year - 1911;
        return $"{rocYear:000}/{dt:MM/dd}";
    }

    /// <summary>
    /// 民國年轉西元年
    /// </summary>
    private DateTime? ConvertRocToAD(string rocDate)
    {
        if (string.IsNullOrWhiteSpace(rocDate))
            return null;

        try
        {
            // 預期格式：XXX/XX/XX 或 XXX-XX-XX
            string[] parts = rocDate.Replace("-", "/").Split('/');
            if (parts.Length == 3)
            {
                int rocYear = int.Parse(parts[0]);
                int month = int.Parse(parts[1]);
                int day = int.Parse(parts[2]);
                
                int adYear = rocYear + 1911;
                return new DateTime(adYear, month, day);
            }
        }
        catch
        {
            // 解析失敗
        }

        return null;
    }
}