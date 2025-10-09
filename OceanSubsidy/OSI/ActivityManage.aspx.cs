using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.App;
using GS.Data;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;

public partial class OSI_ActivityManage : System.Web.UI.Page
{
    public string YearFrom
    {
        get => hfYearFrom.Value == "" ? (DateTime.Now.Year - 1911).ToString() : hfYearFrom.Value;
        set => hfYearFrom.Value = value.ToString();
    }

    public string YearTo
    {
        get => hfYearTo.Value == "" ? (DateTime.Now.Year - 1911).ToString() : hfYearTo.Value;
        set => hfYearTo.Value = value.ToString();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            InitYearDropdowns();
            BindList();
        }
        if (IsPostBack)
        {
            // 取得使用者選擇的 Tab
            var active = hfActiveTab.Value; // e.g. "#adm" 或 "#setting"
            if (!String.IsNullOrEmpty(active))
            {
                // 註冊一段 Bootstrap5 的 Tab.show() 腳本
                string js = $@"
  var triggerEl = document.querySelector('[data-bs-target=""{active}""]') 
                  || document.querySelector('[href=""{active}""]');
  if (triggerEl) new bootstrap.Tab(triggerEl).show();
";
                ScriptManager.RegisterStartupScript(
                    this, this.GetType(),
                    "restoreTab", js, true
                );
            }
        }
    }

    private void InitYearDropdowns()
    {
        // 載入 ddlYearFrom
        string currentYear = DateTimeHelper
            .GregorianYearToMinguo(DateTime.Now.Year)
            .ToString();

        DataTable tbl = OSIDataPeriodsHelper.QueryAllYears();
        ddlYearFrom.Items.Clear();
        if (tbl != null)
        {
            var years = tbl.Rows
                          .Cast<DataRow>()
                          .Select(r => Convert.ToInt32(r["PeriodYear"]))
                          .Distinct()
                          .OrderBy(y => y)
                          .ToList();

            foreach (var y in years)
                ddlYearFrom.Items.Add(new ListItem(y.ToString() + "年", y.ToString()));

            // ddlYearFrom 預設
            if (ddlYearFrom.Items.FindByValue(currentYear) != null)
                ddlYearFrom.SelectedValue = currentYear;
            else
                ddlYearFrom.SelectedIndex = 0;
        }

        PopulateYearTo();
    }

    /// <summary>
    /// 根據 ddlYearFrom 選的起始年，載入所有 ≥ 起始年的結束年選項
    /// </summary>
    private void PopulateYearTo()
    {
        // 重新撈所有年度
        DataTable tbl = OSIDataPeriodsHelper.QueryAllYears();
        if (tbl == null || tbl.Rows.Count == 0)
        {
            ddlYearTo.Items.Clear();
            return;
        }

        // 取出 distinct、排序好的年度清單
        var years = tbl.Rows
                      .Cast<DataRow>()
                      .Select(r => Convert.ToInt32(r["PeriodYear"]))
                      .Distinct()
                      .OrderBy(y => y)
                      .ToList();

        // 起始年
        int fromYear = int.Parse(ddlYearFrom.SelectedValue);

        // 清空 & 加入所有 ≥ 起始年的年度
        var yearTo = ddlYearTo.SelectedValue;
        ddlYearTo.Items.Clear();
        foreach (var y in years.Where(y => y >= fromYear))
        {
            ddlYearTo.Items.Add(new ListItem(y.ToString() + "年", y.ToString()));
        }

        if (ddlYearTo.Items.FindByValue(yearTo) != null)
            ddlYearTo.SelectedValue = yearTo;
        else
            ddlYearTo.SelectedIndex = 0;

    }

    protected void ddlYearFrom_SelectedIndexChanged(object sender, EventArgs e)
    {
        PopulateYearTo();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        // 重設分頁到第一頁
        dpPeriods.SetPageProperties(0, dpPeriods.PageSize, false);

        YearFrom = ddlYearFrom.SelectedValue;
        YearTo = ddlYearTo.SelectedValue;
        lvPeriods.EditIndex = -1;
        BindList();
    }

    private void BindList()
    {
        DataTable dt = OSIDataPeriodsHelper.QueryByRange(YearFrom, YearTo);

        // 新增中央機關和縣市政府的欄位
        if (!dt.Columns.Contains("CentralFilledCount"))
            dt.Columns.Add("CentralFilledCount", typeof(int));
        if (!dt.Columns.Contains("CentralTotalUnit"))
            dt.Columns.Add("CentralTotalUnit", typeof(int));
        if (!dt.Columns.Contains("LocalFilledCount"))
            dt.Columns.Add("LocalFilledCount", typeof(int));
        if (!dt.Columns.Contains("LocalTotalUnit"))
            dt.Columns.Add("LocalTotalUnit", typeof(int));
        if (!dt.Columns.Contains("ReportCount"))
            dt.Columns.Add("ReportCount", typeof(int));

        // 分別計算中央機關和縣市政府的總數
        int centralTotalUnit = SysUnitHelper.QueryParentUnitsByGovType(1).Rows.Count;  // 中央機關總數
        int localTotalUnit = SysUnitHelper.QueryParentUnitsByGovType(2).Rows.Count;    // 縣市政府總數

        // 逐筆用另一個 helper 拿統計
        foreach (DataRow row in dt.Rows)
        {
            string periodID = row["PeriodID"].ToString();
            
            // 分別計算中央機關和縣市政府的已填報數
            var centralFilledCount = OSIDataPeriodsHelper.QueryUnitCountByIDAndGovType(periodID, 1);
            var localFilledCount = OSIDataPeriodsHelper.QueryUnitCountByIDAndGovType(periodID, 2);
            
            row["CentralFilledCount"] = centralFilledCount;
            row["CentralTotalUnit"] = centralTotalUnit;
            row["LocalFilledCount"] = localFilledCount;
            row["LocalTotalUnit"] = localTotalUnit;

            // 計算已填報的總數
            var reportCount = OSIActivityReportsHelper.QueryByPeriodID(periodID).Rows.Count;
            row["ReportCount"] = reportCount;
        }

        lvPeriods.DataSource = dt;
        lvPeriods.DataBind();
    }

    protected void dpPeriods_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
    {
        dpPeriods.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
        BindList();
    }

    protected void dpPeriods_PreRender(object sender, EventArgs e)
    {
        if (dpPeriods.Controls.Count < 2) return;

        var container = dpPeriods.Controls[1];
        foreach (Control c in container.Controls)
        {
            if (c is Button btn && btn.Text.Trim() == "...")
            {
                btn.CssClass = "pagination-item ellipsis";
            }
        }
    }

    // ListView ItemCommand 處理編輯與提醒
    protected void lvPeriods_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        string periodId = e.CommandArgument.ToString();
        if (e.CommandName == "Reminder")
        {
            SendReminderEmail(periodId);

            string safeMsg = HttpUtility.JavaScriptStringEncode("信件已寄送");
            string js = $"showGlobalMessage('{safeMsg}');";

            ScriptManager.RegisterStartupScript(
                this.Page,
                this.Page.GetType(),
                "showMsg",
                js,
                true
            );
        }
    }

    private void SendReminderEmail(string periodId)
    {
        OSIReminderHelper.SendReminderEmail(periodId);
    }

    // 點「編輯」
    protected void lvPeriods_ItemEditing(object sender, ListViewEditEventArgs e)
    {
        lvPeriods.EditIndex = e.NewEditIndex;
        BindList();
    }

    // 點「取消」
    protected void lvPeriods_ItemCanceling(object sender, ListViewCancelEventArgs e)
    {
        lvPeriods.EditIndex = -1;
        BindList();
    }

    // 點「儲存」
    protected void lvPeriods_ItemUpdating(object sender, ListViewUpdateEventArgs e)
    {
        // 找到正在編輯的那一筆
        var item = lvPeriods.Items[e.ItemIndex];
        int periodId = (int)lvPeriods.DataKeys[e.ItemIndex].Value;

        var txtStart = (TextBox)item.FindControl("txtEditStart");
        var txtEnd = (TextBox)item.FindControl("txtEditEnd");
        var txtColor = (TextBox)item.FindControl("txtEditColor");
        string newColor = txtColor.Text;

        // 解析使用者輸入
        if (!DateTimeHelper.TryParseMinguoDate(txtStart.Text, out var newStart)
            || !DateTimeHelper.TryParseMinguoDate(txtEnd.Text, out var newEnd))
        {
            return;
        }

        // 更新資料表
        OSIDataPeriodsHelper.UpdateRangeAndColerByID(periodId, newStart, newEnd, newColor);

        // 退出編輯模式
        lvPeriods.EditIndex = -1;
        BindList();
    }
}