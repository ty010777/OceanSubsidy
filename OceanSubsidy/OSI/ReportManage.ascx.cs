using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.Data;
using GS.Extension;
using GS.OCA_OceanSubsidy.Entity;
using OfficeOpenXml;

public partial class OSI_ReportManage : System.Web.UI.UserControl
{
    private SessionHelper.UserInfoClass UserInfo
    {
        get => SessionHelper.Get<SessionHelper.UserInfoClass>(
                  SessionHelper.UserInfo)
                ?? new SessionHelper.UserInfoClass();
        set => SessionHelper.Set(SessionHelper.UserInfo, value);
    }
    private const string VS_REPORT_IDS = "VS_ReportIDs";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ToggleAddButtonAvailability();
            BindYears();
            SetDefaultPeriod();
            BindUnits();
            BindActivityNatures();
            BindCarrierTypes();
            BindResearchItems();
            BindReports();
        }
    }

    // 判斷是否可以新增
    private void ToggleAddButtonAvailability()
    {
        GisTable tbl = OSIDataPeriodsHelper.QueryByDateTime(DateTime.Now);

        bool inAnyPeriod = false;
        if (tbl != null && tbl.Rows.Count > 0)
            inAnyPeriod = true;

        btnAdd.Enabled = inAnyPeriod;
        if (!inAnyPeriod)
            btnAdd.CssClass += " disabled";
    }

    // 載入年度
    void BindYears()
    {
        var dt = OSIDataPeriodsHelper.QueryAllYearsWithFilter();

        ddlYear.Items.Clear();
        foreach (DataRow row in dt.Rows)
        {
            ddlYear.Items.Add(new ListItem(
                row["PeriodYear"].ToString() + "年",
                row["PeriodYear"].ToString()
            ));
        }
        if (ddlYear.Items.Count > 0)
            ddlYear.SelectedIndex = 0;
        ddlYear_SelectedIndexChanged(ddlYear, EventArgs.Empty);
    }

    // 年度改變 → 載入季別
    protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlYear.SelectedValue != null)
        {
            var dt = OSIDataPeriodsHelper.QueryQuartersByYearWithFilter(ddlYear.SelectedValue);

            ddlQuarter.Items.Clear();
            foreach (DataRow row in dt.Rows)
            {
                ddlQuarter.Items.Add(new ListItem(
                    row["PeriodQuarter"].ToString(),
                    row["PeriodID"].ToString()
                ));
            }
            ddlQuarter.Enabled = true;
        }
        else
        {
            ddlQuarter.Items.Clear();
            ddlQuarter.Enabled = false;
        }
    }

    void BindUnits()
    {
        ddlUnit.Items.Clear();

        GisTable dt;
        if (UserInfo.OSI_RoleName == "系統管理者")
        {
            dt = SysUnitHelper.QueryAllOrderByUnitID();

            ddlUnit.Items.Add(new ListItem("全部", "-99"));
            foreach (DataRow r in dt.Rows)
            {
                ddlUnit.Items.Add(new ListItem(
                        r["UnitName"].ToString(),
                        r["UnitID"].ToString()));
            }

            return;
        }

        if (!string.IsNullOrEmpty(UserInfo.UnitID))
        {
            dt = SysUnitHelper.QueryAllChildByID(UserInfo.UnitID.toInt());
            if (dt != null && dt.Rows.Count > 0)
            {
                // 一般政府機關
                foreach (DataRow row in dt.Rows)
                {
                    string unitName = row["UnitName"].ToString();
                    string unitID = row["UnitID"].ToString();

                    // 根據 ParentUnitID 判斷是否為根單位
                    if (!row.IsNull("ParentUnitID"))
                    {
                        unitName = "　" + unitName;
                    }

                    ddlUnit.Items.Add(new ListItem(unitName, unitID));
                }
                return;
            }
        }

        // 非政府機關
        ddlUnit.Items.Clear();
        ddlUnit.Items.Add(new ListItem("非政府機關", "-1"));
    }

    // 活動性質下拉選單
    void BindActivityNatures()
    {
        ddlActivityNatures.Items.Clear();

        DataTable dt;
        dt = OSIActivityNaturesHelper.QueryAll();
        ddlActivityNatures.DataTextField = "NatureName";
        ddlActivityNatures.DataValueField = "NatureID";
        ddlActivityNatures.DataSource = dt;
        ddlActivityNatures.DataBind();
        ddlActivityNatures.Items.Insert(0, new ListItem("全部", "-99"));
        return;
    }

    // 載具類別下拉選單
    void BindCarrierTypes()
    {
        ddlCarrierTypes.Items.Clear();

        DataTable dt;
        dt = OSICarrierTypesHelper.QueryAll();
        ddlCarrierTypes.DataTextField = "CarrierTypeName";
        ddlCarrierTypes.DataValueField = "CarrierTypeID";
        ddlCarrierTypes.DataSource = dt;
        ddlCarrierTypes.DataBind();
        ddlCarrierTypes.Items.Insert(0, new ListItem("全部", "-99"));
        return;
    }

    // 載具類別下拉選單
    void BindResearchItems()
    {
        ddlResearchItems.Items.Clear();

        DataTable dt;
        dt = OSIResearchItemsHelper.QueryAll();
        ddlResearchItems.DataTextField = "ItemName";
        ddlResearchItems.DataValueField = "ItemID";
        ddlResearchItems.DataSource = dt;
        ddlResearchItems.DataBind();
        ddlResearchItems.Items.Insert(0, new ListItem("全部", "-99"));
        return;
    }

    // 查詢列表
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        // 重設分頁到第一頁
        dpReports.SetPageProperties(0, dpReports.PageSize, false);        

        BindReports();
    }

    protected void lvReports_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType != ListViewItemType.DataItem) return;

        var dataItem = (ListViewDataItem)e.Item;
        var drv = (DataRowView)dataItem.DataItem;
        var ph = (PlaceHolder)dataItem.FindControl("phGeo");
        var litUnit = e.Item.FindControl("litReportingUnit") as Literal;
        // 填報機關
        if (litUnit != null && string.IsNullOrWhiteSpace(litUnit.Text))
            litUnit.Text = "非政府機關";

        // 活動空間範圍 - 透過 OSIGeomHelper 查詢新的資料表
        var reportId = drv["ReportID"].ToString();
        var geomData = OSIGeomHelper.QueryByReportID(reportId);
        
        if (geomData == null || geomData.Rows.Count == 0)
        {
            ph.Controls.Add(new Literal { Text = "未標定" });
        }
        else
        {
            string htmlBtn = $@"
            <button
                type=""button""
                class=""btn btn-sm btn-outline-green""
                data-bs-toggle=""modal""
                data-bs-target=""#mapModal""
                data-reportid=""{reportId}"">
                查看定位
            </button>";
            ph.Controls.Add(new Literal { Text = htmlBtn });
        }
    }

    private void BindReports()
    {
        var tbl = OSIActivityReportsHelper.QueryActReport(
            ddlQuarter.SelectedValue,
            ddlUnit.SelectedValue,
            ddlActivityNatures.SelectedValue,
            ddlCarrierTypes.SelectedValue,
            ddlResearchItems.SelectedValue,
            txtKeySearch.Text);
        lvReports.DataSource = tbl;
        lvReports.DataBind();

        // 存ReportIDs 到 ViewState
        var idList = tbl
            .AsEnumerable()
            .Select(r => r.Field<int>("ReportID").ToString());
        ViewState[VS_REPORT_IDS] = string.Join(",", idList);
    }

    protected void lvReports_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        string id = e.CommandArgument?.ToString();
        switch (e.CommandName)
        {
            case "EditReport":
                // 判斷當前頁面
                string currentPage = Request.Path.ToLower();
                string returnUrl = currentPage.Contains("activitymanage.aspx")
                    ? "~/OSI/ActivityManage.aspx"
                    : "~/OSI/ActivityReports.aspx";
                Response.Redirect($"~/OSI/ActivityReportDetail.aspx?id={id}&returnUrl={returnUrl}");
                break;
            case "AskDelete":
                hfDeleteID.Value = id;

                // 取出剛才改成 Static 的 deleteModal
                string modalId = deleteModal.ClientID;
                string script = $"showModal('{modalId}');";

                // 註冊到 Page 層級，才能命中 Modal 所在區塊
                ScriptManager.RegisterStartupScript(
                    this.Page,
                    this.Page.GetType(),
                    "ASK_DELETE_MODAL",
                    script,
                    true
                );

                BindReports();
                break;
            case "ViewMap":
                // 依 id 顯示地圖
                //Response.Redirect($"Map.aspx?reportID={id}");
                BindReports();
                break;
        }
    }

    protected void lvReports_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
    {
        dpReports.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
        BindReports();
    }

    protected void dpReports_PreRender(object sender, EventArgs e)
    {
        if (dpReports.Controls.Count < 2) return;

        var container = dpReports.Controls[1];
        foreach (Control c in container.Controls)
        {
            if (c is Button btn && btn.Text.Trim() == "...")
            {
                btn.CssClass = "pagination-item ellipsis";
            }
        }
    }

    // 確認刪除
    protected void btnConfirmDelete_Click(object sender, EventArgs e)
    {
        var ok = OSIActivityReportsHelper.DeleteByID(hfDeleteID.Value);
        string modalMsg = "";
        if (ok)
        {
            modalMsg = "資料已刪除。";

            string modalId = deleteModal.ClientID;
            string script = $"hideModal('{modalId}');";

            ScriptManager.RegisterStartupScript(
                this.Page,
                this.Page.GetType(),
                "HIDE_DELETE_MODAL",
                script,
                true
            );

            BindReports();
        }
        else
        {
            modalMsg = "資料刪除失敗。";
        }

        string safeMsg = HttpUtility.JavaScriptStringEncode(modalMsg);
        string js = $"showGlobalMessage('{safeMsg}');";

        ScriptManager.RegisterStartupScript(
            this.Page,
            this.Page.GetType(),
            "showMsg",
            js,
            true
        );
    }

    // 新增
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        // 判斷當前頁面
        string currentPage = Request.Path.ToLower();
        string returnUrl = currentPage.Contains("activitymanage.aspx")
            ? "~/OSI/ActivityManage.aspx"
            : "~/OSI/ActivityReports.aspx";
        Response.Redirect($"~/OSI/ActivityReportDetail.aspx?returnUrl={returnUrl}");
    }

    // 匯出列表功能
    protected void btnExport_Click(object sender, EventArgs e)
    {
        var idCsv = ViewState[VS_REPORT_IDS] as string;
        if (string.IsNullOrEmpty(idCsv))
            return;

        var ids = idCsv
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => int.Parse(s))
            .ToArray();

        // 1. 取得目前查詢過濾後的所有資料
        var dt = OSIActivityReportsHelper.QueryExcelActReport(ids);

        // 2. 設定
        ExcelPackage.License.SetNonCommercialOrganization("GIS.FCU");

        // 3. 建立 ExcelPackage 並加入工作表
        using (var package = new ExcelPackage())
        {
            var ws = package.Workbook.Worksheets.Add("匯出列表資料");

            // 4. 第一列：標題固定文字
            string[] headers = new[]
            {
                "填報期程",
                "填報機關",
                "活動名稱",
                "活動性質",
                "活動性質(描述)",
                "活動執行者",
                "研究調查日期",
                "使用載具名稱",
                "研究調查項目(類別)",
                "研究調查項目(描述)",
                "研究調查儀器",
                "研究調查活動內容概述",
                "研究調查範圍",
                "窗口",
                "電話",
                "電郵"
            };
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cells[1, i + 1].Value = headers[i];
            }

            // 5. 第二列起：資料列框架
            for (int r = 0; r < dt.Rows.Count; r++)
            {
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    ws.Cells[r + 2, c + 1].Value = dt.Rows[r][c]?.ToString();
                }
            }

            // 6. 自動調整欄寬
            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            // 7. 輸出到瀏覽器讓使用者下載
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", "attachment; filename=匯出列表資料.xlsx");
            Response.BinaryWrite(package.GetAsByteArray());
            Response.End();
        }
    }

    // 設定預設填報週期
    private void SetDefaultPeriod()
    {
        try
        {
            OSI_DataPeriods periodToSet = null;

            // 1. 先嘗試取得今天對應的填報期間
            var currentPeriods = OSIDataPeriodsHelper.QueryByDateTimeWithClass(DateTime.Now);

            if (currentPeriods != null && currentPeriods.Count > 0)
            {
                // 若有資料，取第一筆
                periodToSet = currentPeriods[0];
            }
            else
            {
                // 2. 若無資料，查詢最近已結束的期間
                periodToSet = OSIDataPeriodsHelper.QueryLatestEndedPeriod();
            }

            // 設定年度和季度
            if (periodToSet != null)
            {
                SetPeriodValues(periodToSet.PeriodYear, periodToSet.PeriodID.ToString());
            }
        }
        catch (Exception ex)
        {
            // 發生錯誤時不影響頁面載入，使用原本的預設值即可
            System.Diagnostics.Debug.WriteLine($"SetDefaultPeriod Error: {ex.Message}");
        }
    }

    // 設定年度和季度的值
    private void SetPeriodValues(string periodYear, string periodID)
    {
        // 設定年度
        var yearItem = ddlYear.Items.FindByValue(periodYear);
        if (yearItem != null)
        {
            ddlYear.SelectedValue = periodYear;

            // 觸發年度改變事件以載入季度
            ddlYear_SelectedIndexChanged(ddlYear, EventArgs.Empty);

            // 設定季度
            var quarterItem = ddlQuarter.Items.FindByValue(periodID);
            if (quarterItem != null)
            {
                ddlQuarter.SelectedValue = periodID;
            }
        }
    }

}