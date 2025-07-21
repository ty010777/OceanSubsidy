using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;
using GS.App;

public partial class OSI_ReportFormHistory : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    /// <summary>
    /// 載入歷史資料
    /// </summary>
    public void LoadHistoryData(OSI_ActivityReports_History history)
    {
        if (history == null) return;

        try
        {
            // 基本資料
            lblActivityName.Text = history.ActivityName;            

            // 資料時間
            LoadDataPeriod(history.PeriodID);

            // 填報機關
            LoadReportingUnit(history.ReportingUnitID);

            // 活動性質
            LoadActivityNature(history.NatureID);
            lblNatureDetail.Text = !string.IsNullOrEmpty(history.NatureText) ? 
                "補充說明：" + history.NatureText : "";

            // 研究調查項目
            LoadResearchItem(history.ResearchItemID);
            lblResItemNote.Text = !string.IsNullOrEmpty(history.ResearchItemNote) ? 
                "補充說明：" + history.ResearchItemNote : "";

            // 研究調查儀器
            lblResInstruments.Text = history.Instruments;

            // 活動內容概述
            lblActivityOverview.Text = history.ActivityOverview;

            // 標示修正說明
            if (!string.IsNullOrEmpty(history.CorrectionNotes))
            {
                trCorrectionNotes.Visible = true;
                lblCorrectionNotes.Text = history.CorrectionNotes;
            }
            else
            {
                trCorrectionNotes.Visible = false;
            }

            // 載入關聯資料（根據 AuditAt 時間判斷）
            LoadRelatedData(history.ReportID, history.AuditAt);

            // 設定地圖按鈕的 ReportID
            btnOpenMap.Attributes["data-historyId"] = history.HistoryID.ToString();

            // 更新資訊
            string unitName = SysUserHelper.QueryUnitNameByUserID(history.AuditBy);
            string userName = "";
            var tbl = SysUserHelper.QueryUserByID(history.AuditBy);
            if (tbl != null && tbl.Rows.Count > 0)
                userName = tbl.Rows[0]["Name"].ToString();
            lblAuditAt.Text = DateTimeHelper.ToMinguoDateTime(history.AuditAt) + " " + unitName + " " + userName;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("載入歷史資料錯誤：" + ex.Message);
        }
    }

    /// <summary>
    /// 載入資料時間
    /// </summary>
    private void LoadDataPeriod(int periodId)
    {
        try
        {
            var tbl = OSIDataPeriodsHelper.QueryByID(periodId.ToString());
            
            if (tbl != null && tbl.Rows.Count > 0)
            {
                var row = tbl.Rows[0];
                lblDataPeriod.Text = string.Format("{0}年{1}", row["PeriodYear"].ToString(), row["PeriodQuarter"].ToString());
            }
        }
        catch { }
    }

    /// <summary>
    /// 載入填報機關
    /// </summary>
    private void LoadReportingUnit(int? unitId)
    {
        if (!unitId.HasValue) return;

        try
        {
            var unit = SysUnitHelper.GetUnitByID(unitId.Value.ToString());
            
            if (unit != null)
            {
                lblUnit.Text = unit.UnitName;
            }
        }
        catch { }
    }

    /// <summary>
    /// 載入活動性質
    /// </summary>
    private void LoadActivityNature(int natureId)
    {
        try
        {
            var nature = OSIActivityNaturesHelper.GetNatureByID(natureId.ToString());
            
            if (nature != null)
            {
                lblNature.Text = nature.NatureName;
            }
        }
        catch { }
    }

    /// <summary>
    /// 載入研究調查項目
    /// </summary>
    private void LoadResearchItem(int? itemId)
    {
        if (!itemId.HasValue) return;

        try
        {
            var item = OSIResearchItemsHelper.GetItemByID(itemId.Value.ToString());
            
            if (item != null)
            {
                lblResearchCategory.Text = item.ItemName;
            }
        }
        catch { }
    }

    /// <summary>
    /// 載入關聯資料
    /// </summary>
    private void LoadRelatedData(int reportId, DateTime auditTime)
    {
        // 載入活動執行者
        LoadActivityExecutors(reportId, auditTime);

        // 載入研究調查日期
        LoadResearchPeriods(reportId, auditTime);

        // 載入載具資料
        LoadCarriers(reportId, auditTime);

        // 載入活動附件
        LoadActivityFiles(reportId, auditTime);

        // 載入研究調查範圍
        LoadSurveyScopes(reportId, auditTime);
    }

    /// <summary>
    /// 載入活動執行者
    /// </summary>
    private void LoadActivityExecutors(int reportId, DateTime auditTime)
    {
        try
        {
            var executorsTable = OSIActivityReportsHistoryHelper.GetRelatedExecutors(reportId, auditTime);

            if (executorsTable != null && executorsTable.Rows.Count > 0)
            {
                // 載入執行者類別資料
                var executorData = new List<object>();
                foreach (DataRow row in executorsTable.Rows)
                {
                    executorData.Add(new
                    {
                        CategoryName = row["CategoryName"] != null ? row["CategoryName"].ToString() : "",
                        ExecutorName = row["ExecutorName"] != null ? row["ExecutorName"].ToString() : ""
                    });
                }

                rptExecList.DataSource = executorData;
                rptExecList.DataBind();
            }
        }
        catch { }
    }

    /// <summary>
    /// 載入研究調查日期
    /// </summary>
    private void LoadResearchPeriods(int reportId, DateTime auditTime)
    {
        try
        {
            var periodsTable = OSIActivityReportsHistoryHelper.GetRelatedPeriods(reportId, auditTime);

            if (periodsTable != null && periodsTable.Rows.Count > 0)
            {
                var periodData = new List<object>();
                foreach (DataRow row in periodsTable.Rows)
                {
                    DateTime startDate = Convert.ToDateTime(row["StartDate"]);
                    DateTime endDate = Convert.ToDateTime(row["EndDate"]);
                    string note = row["PeriodLabel"] != null ? row["PeriodLabel"].ToString() : "";

                    periodData.Add(new
                    {
                        StartDateRoc = DateTimeHelper.ToMinguoDate(startDate),
                        EndDateRoc = DateTimeHelper.ToMinguoDate(endDate),
                        PeriodLabel = !string.IsNullOrEmpty(note) ? " (" + note + ")" : ""
                    });
                }

                rptResList.DataSource = periodData;
                rptResList.DataBind();
            }
        }
        catch { }
    }

    /// <summary>
    /// 載入載具資料
    /// </summary>
    private void LoadCarriers(int reportId, DateTime auditTime)
    {
        try
        {
            var carriersTable = OSIActivityReportsHistoryHelper.GetRelatedCarriers(reportId, auditTime);

            if (carriersTable != null && carriersTable.Rows.Count > 0)
            {
                var carrierData = new List<object>();
                foreach (DataRow row in carriersTable.Rows)
                {
                    carrierData.Add(new
                    {
                        CarrierTypeName = row["CarrierTypeName"] != null ? row["CarrierTypeName"].ToString() : "",
                        CarrierDetail = row["CarrierDetail"] != null ? row["CarrierDetail"].ToString() : "",
                        CarrierNo = row["CarrierNo"] != null ? row["CarrierNo"].ToString() : ""
                    });
                }

                rptCarrierList.DataSource = carrierData;
                rptCarrierList.DataBind();
            }
        }
        catch { }
    }

    /// <summary>
    /// 載入活動附件
    /// </summary>
    private void LoadActivityFiles(int reportId, DateTime auditTime)
    {
        try
        {
            var filesTable = OSIActivityReportsHistoryHelper.GetRelatedFiles(reportId, auditTime);

            if (filesTable != null && filesTable.Rows.Count > 0)
            {
                rptActivityFile.DataSource = filesTable;
                rptActivityFile.DataBind();
            }
        }
        catch { }
    }

    /// <summary>
    /// 載入研究調查範圍
    /// </summary>
    private void LoadSurveyScopes(int reportId, DateTime auditTime)
    {
        try
        {
            var scopesTable = OSIActivityReportsHistoryHelper.GetRelatedScopes(reportId, auditTime);

            if (scopesTable != null && scopesTable.Rows.Count > 0)
            {
                rptScopeList.DataSource = scopesTable;
                rptScopeList.DataBind();
            }
        }
        catch { }
    }

    /// <summary>
    /// 檔案下載
    /// </summary>
    protected void rptActivityFile_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Download")
        {
            string filePath = e.CommandArgument.ToString();
            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    string fullPath = Server.MapPath(filePath);
                    if (System.IO.File.Exists(fullPath))
                    {
                        string fileName = System.IO.Path.GetFileName(fullPath);
                        Response.ContentType = "application/octet-stream";
                        Response.AppendHeader("Content-Disposition", "attachment; filename=" + 
                            HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
                        Response.TransmitFile(fullPath);
                        Response.End();
                    }
                }
                catch { }
            }
        }
    }
}