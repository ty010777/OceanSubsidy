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
    public bool LoadHistoryData(int historyId)
    {
        try
        {
            var data = GetHistoryData(historyId);
            if (data == null) return false;

            // 綁定基本資料到畫面
            lblActivityName.Text = data.ActivityName;
            lblDataPeriod.Text = data.DataPeriod;
            lblUnit.Text = data.Unit;
            lblNature.Text = data.Nature;
            lblNatureDetail.Text = data.NatureDetail;
            lblResearchCategory.Text = data.ResearchCategory;
            lblResItemNote.Text = data.ResItemNote;
            lblResInstruments.Text = data.ResInstruments;
            lblActivityOverview.Text = data.ActivityOverview;
            lblAuditAt.Text = data.AuditAt;

            // 標示修正說明
            trCorrectionNotes.Visible = data.ShowCorrectionNotes;
            if (data.ShowCorrectionNotes)
            {
                lblCorrectionNotes.Text = data.CorrectionNotes;
            }

            // 設定地圖按鈕的 ReportID
            btnOpenMap.Attributes["data-historyId"] = data.HistoryId;

            // 綁定關聯資料到畫面
            rptExecList.DataSource = data.ExecutorData;
            rptExecList.DataBind();

            rptResList.DataSource = data.PeriodData;
            rptResList.DataBind();

            rptCarrierList.DataSource = data.CarrierData;
            rptCarrierList.DataBind();

            rptActivityFile.DataSource = data.ActivityFiles;
            rptActivityFile.DataBind();

            rptScopeList.DataSource = data.SurveyScopes;
            rptScopeList.DataBind();

            rptScopeCountyList.DataSource = data.SurveyCounties;
            rptScopeCountyList.DataBind();

            // 進行資料比對並標記差異
            var historyIdList = OSIActivityReportsHistoryHelper.GetReportHistoryIdList(data.ReportId);
            var idx = historyIdList.FindIndex(id => id == historyId);
            if (idx != -1)
            {
                var followingData = new ReportFormHistoryData();
                if (idx > 0)
                {
                    followingData = GetHistoryData(historyIdList[idx - 1]);
                }
                else
                {
                    followingData = GetReportData(data.ReportId);
                }
                CompareHistoryData(data, followingData);
            }

            // 根據差異標記動態設定 th 元素的樣式
            ApplyDifferenceStyles(data);


        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("載入歷史資料錯誤：" + ex.Message);
            return false;
        }

        return true;
    }

    /// <summary>
    /// 取得歷史資料
    /// </summary>
    public ReportFormHistoryData GetHistoryData(int historyId)
    {
        var history = OSIActivityReportsHistoryHelper.GetHistoryById(historyId);
        if (history == null) return null;

        var data = new ReportFormHistoryData();

        try
        {
            data.ReportId = history.ReportID;
            data.ActivityName = history.ActivityName;
            data.DataPeriod = GetDataPeriodText(history.PeriodID);
            data.Unit = GetReportingUnitText(history.ReportingUnitID);
            data.Nature = GetActivityNatureText(history.NatureID);
            data.NatureDetail = !string.IsNullOrEmpty(history.NatureText) ?
                "補充說明：" + history.NatureText : "";
            data.ResearchCategory = GetResearchItemText(history.ResearchItemID);
            data.ResItemNote = !string.IsNullOrEmpty(history.ResearchItemNote) ?
                "補充說明：" + history.ResearchItemNote : "";
            data.ResInstruments = history.Instruments;
            data.ActivityOverview = history.ActivityOverview;
            data.CorrectionNotes = history.CorrectionNotes;
            data.ShowCorrectionNotes = !string.IsNullOrEmpty(history.CorrectionNotes);
            data.HistoryId = history.HistoryID.ToString();

            string unitName = SysUserHelper.QueryUnitNameByUserID(history.AuditBy);
            string userName = "";
            var tbl = SysUserHelper.QueryUserByID(history.AuditBy);
            if (tbl != null && tbl.Rows.Count > 0)
                userName = tbl.Rows[0]["Name"].ToString();
            data.AuditAt = DateTimeHelper.ToMinguoDateTime(history.AuditAt) + " " + unitName + " " + userName;

            data.ExecutorData = GetActivityExecutorsData(history.ReportID, history.AuditAt);
            data.PeriodData = GetResearchPeriodsData(history.ReportID, history.AuditAt);
            data.CarrierData = GetCarriersData(history.ReportID, history.AuditAt);
            data.ActivityFiles = GetActivityFilesData(history.ReportID, history.AuditAt);
            data.SurveyScopes = GetSurveyScopesData(history.ReportID, history.AuditAt);
            data.SurveyCounties = GetSurveyCountiesData(history.ReportID, history.AuditAt);
            data.SpatialData = GetSpatialData(history.ReportID, history.AuditAt);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("取得歷史資料錯誤：" + ex.Message);
            return null;
        }

        return data;
    }

    /// <summary>
    /// 取得最新報告資料
    /// </summary>
    public ReportFormHistoryData GetReportData(int reportId)
    {
        var report = OSIActivityReportsHelper.QueryByIDWithClass(reportId.ToString());
        if (report == null) return null;

        var data = new ReportFormHistoryData();

        try
        {
            data.ReportId = reportId;
            data.ActivityName = report.ActivityName;
            data.DataPeriod = GetDataPeriodText(report.PeriodID);
            data.Unit = GetReportingUnitText(report.ReportingUnitID);
            data.Nature = GetActivityNatureText(report.NatureID);
            data.NatureDetail = !string.IsNullOrEmpty(report.NatureText) ?
                "補充說明：" + report.NatureText : "";
            data.ResearchCategory = GetResearchItemText(report.ResearchItemID);
            data.ResItemNote = !string.IsNullOrEmpty(report.ResearchItemNote) ?
                "補充說明：" + report.ResearchItemNote : "";
            data.ResInstruments = report.Instruments;
            data.ActivityOverview = report.ActivityOverview;
            data.CorrectionNotes = "";
            data.ShowCorrectionNotes = false;
            data.HistoryId = "0";

            string unitName = SysUserHelper.QueryUnitNameByUserID(report.LastUpdatedBy.ToString());
            string userName = "";
            var tbl = SysUserHelper.QueryUserByID(report.LastUpdatedBy.ToString());
            if (tbl != null && tbl.Rows.Count > 0)
                userName = tbl.Rows[0]["Name"].ToString();
            data.AuditAt = DateTimeHelper.ToMinguoDateTime(report.LastUpdated) + " " + unitName + " " + userName;

            data.ExecutorData = GetCurrentActivityExecutorsData(reportId);
            data.PeriodData = GetCurrentResearchPeriodsData(reportId);
            data.CarrierData = GetCurrentCarriersData(reportId);
            data.ActivityFiles = GetCurrentActivityFilesData(reportId);
            data.SurveyScopes = GetCurrentSurveyScopesData(reportId);
            data.SurveyCounties = GetCurrentSurveyCountiesData(reportId);
            data.SpatialData = GetCurrentSpatialData(reportId);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("取得最新報告資料錯誤：" + ex.Message);
            return null;
        }

        return data;
    }

    /// <summary>
    /// 比對歷史資料差異並標記變更欄位
    /// </summary>
    private void CompareHistoryData(ReportFormHistoryData currentData, ReportFormHistoryData followingData)
    {
        if (followingData == null) return;

        // 比對基本欄位
        currentData.ActivityNameChanged = currentData.ActivityName != followingData.ActivityName;
        currentData.DataPeriodChanged = currentData.DataPeriod != followingData.DataPeriod;
        currentData.UnitChanged = currentData.Unit != followingData.Unit;
        currentData.NatureChanged = currentData.Nature != followingData.Nature;
        currentData.NatureDetailChanged = currentData.NatureDetail != followingData.NatureDetail;
        currentData.ResearchCategoryChanged = currentData.ResearchCategory != followingData.ResearchCategory;
        currentData.ResItemNoteChanged = currentData.ResItemNote != followingData.ResItemNote;
        currentData.ResInstrumentsChanged = currentData.ResInstruments != followingData.ResInstruments;
        currentData.ActivityOverviewChanged = currentData.ActivityOverview != followingData.ActivityOverview;

        // 比對複雜型別資料
        currentData.ExecutorDataChanged = !CompareExecutorData(currentData.ExecutorData, followingData.ExecutorData);
        currentData.PeriodDataChanged = !ComparePeriodData(currentData.PeriodData, followingData.PeriodData);
        currentData.CarrierDataChanged = !CompareCarrierData(currentData.CarrierData, followingData.CarrierData);
        currentData.ActivityFilesChanged = !CompareDataTable(currentData.ActivityFiles, followingData.ActivityFiles, new string[] { "FileName" });
        currentData.SurveyScopesChanged = !CompareDataTable(currentData.SurveyScopes, followingData.SurveyScopes, new string[] { "SurveyScope" });
        currentData.SurveyCountiesChanged = !CompareDataTable(currentData.SurveyCounties, followingData.SurveyCounties, new string[] { "CountyName" });
        currentData.SpatialDataChanged = !CompareDataTable(currentData.SpatialData, followingData.SpatialData, new string[] { "GeomName", "GeoData" });
    }

    /// <summary>
    /// 比對活動執行者資料
    /// </summary>
    private bool CompareExecutorData(List<object> current, List<object> following)
    {
        if (current == null && following == null) return true;
        if (current == null || following == null) return false;
        if (current.Count != following.Count) return false;

        for (int i = 0; i < current.Count; i++)
        {
            var currentItem = current[i];
            var previousItem = following[i];

            var currentCategoryName = GetPropertyValue(currentItem, "CategoryName");
            var currentExecutorName = GetPropertyValue(currentItem, "ExecutorName");
            var previousCategoryName = GetPropertyValue(previousItem, "CategoryName");
            var previousExecutorName = GetPropertyValue(previousItem, "ExecutorName");

            if (currentCategoryName != previousCategoryName || currentExecutorName != previousExecutorName)
                return false;
        }
        return true;
    }

    /// <summary>
    /// 比對研究調查日期資料
    /// </summary>
    private bool ComparePeriodData(List<object> current, List<object> following)
    {
        if (current == null && following == null) return true;
        if (current == null || following == null) return false;
        if (current.Count != following.Count) return false;

        for (int i = 0; i < current.Count; i++)
        {
            var currentItem = current[i];
            var previousItem = following[i];

            var currentStartDate = GetPropertyValue(currentItem, "StartDateRoc");
            var currentEndDate = GetPropertyValue(currentItem, "EndDateRoc");
            var currentPeriodLabel = GetPropertyValue(currentItem, "PeriodLabel");
            var previousStartDate = GetPropertyValue(previousItem, "StartDateRoc");
            var previousEndDate = GetPropertyValue(previousItem, "EndDateRoc");
            var previousPeriodLabel = GetPropertyValue(previousItem, "PeriodLabel");

            if (currentStartDate != previousStartDate || currentEndDate != previousEndDate || currentPeriodLabel != previousPeriodLabel)
                return false;
        }
        return true;
    }

    /// <summary>
    /// 比對載具資料
    /// </summary>
    private bool CompareCarrierData(List<object> current, List<object> following)
    {
        if (current == null && following == null) return true;
        if (current == null || following == null) return false;
        if (current.Count != following.Count) return false;

        for (int i = 0; i < current.Count; i++)
        {
            var currentItem = current[i];
            var previousItem = following[i];

            var currentCarrierTypeName = GetPropertyValue(currentItem, "CarrierTypeName");
            var currentCarrierDetail = GetPropertyValue(currentItem, "CarrierDetail");
            var currentCarrierNo = GetPropertyValue(currentItem, "CarrierNo");
            var previousCarrierTypeName = GetPropertyValue(previousItem, "CarrierTypeName");
            var previousCarrierDetail = GetPropertyValue(previousItem, "CarrierDetail");
            var previousCarrierNo = GetPropertyValue(previousItem, "CarrierNo");

            if (currentCarrierTypeName != previousCarrierTypeName ||
                currentCarrierDetail != previousCarrierDetail ||
                currentCarrierNo != previousCarrierNo)
                return false;
        }
        return true;
    }

    /// <summary>
    /// 比對 DataTable 資料
    /// </summary>
    private bool CompareDataTable(DataTable current, DataTable following, string[] columnNames = null)
    {
        if (current == null && following == null) return true;
        if (current == null || following == null) return false;
        if (current.Rows.Count != following.Rows.Count) return false;

        // 如果沒有指定欄位名稱，則比對所有欄位
        if (columnNames == null)
        {
            for (int i = 0; i < current.Rows.Count; i++)
            {
                for (int j = 0; j < current.Columns.Count; j++)
                {
                    var currentValue = current.Rows[i][j]?.ToString() ?? "";
                    var previousValue = following.Rows[i][j]?.ToString() ?? "";
                    if (currentValue != previousValue)
                        return false;
                }
            }
        }
        else
        {
            // 只比對指定的欄位
            for (int i = 0; i < current.Rows.Count; i++)
            {
                foreach (string columnName in columnNames)
                {
                    // 檢查欄位是否存在於兩個 DataTable 中
                    if (current.Columns.Contains(columnName) && following.Columns.Contains(columnName))
                    {
                        var currentValue = current.Rows[i][columnName]?.ToString() ?? "";
                        var previousValue = following.Rows[i][columnName]?.ToString() ?? "";
                        if (currentValue != previousValue)
                            return false;
                    }
                }
            }
        }
        return true;
    }

    /// <summary>
    /// 取得物件屬性值
    /// </summary>
    private string GetPropertyValue(object obj, string propertyName)
    {
        if (obj == null) return "";
        var property = obj.GetType().GetProperty(propertyName);
        return property?.GetValue(obj)?.ToString() ?? "";
    }

    /// <summary>
    /// 根據差異標記動態設定 th 元素樣式
    /// </summary>
    private void ApplyDifferenceStyles(ReportFormHistoryData data)
    {
        // 先清除所有 th 元素的 text-pink class
        RemoveTextPinkClass(thDataPeriod);
        RemoveTextPinkClass(thUnit);
        RemoveTextPinkClass(thActivityName);
        RemoveTextPinkClass(thNature);
        RemoveTextPinkClass(thExecutor);
        RemoveTextPinkClass(thPeriod);
        RemoveTextPinkClass(thCarrier);
        RemoveTextPinkClass(thResearchCategory);
        RemoveTextPinkClass(thResInstruments);
        RemoveTextPinkClass(thActivityOverview);
        RemoveTextPinkClass(thActivityFiles);
        RemoveTextPinkClass(thSurveyCounties);
        RemoveTextPinkClass(thSurveyScopes);
        RemoveTextPinkClass(thSpatialData);

        // 根據差異標記加上 text-pink class
        AddTextPinkClass(thDataPeriod, data.DataPeriodChanged);
        AddTextPinkClass(thUnit, data.UnitChanged);
        AddTextPinkClass(thActivityName, data.ActivityNameChanged);
        AddTextPinkClass(thNature, data.NatureChanged || data.NatureDetailChanged);
        AddTextPinkClass(thExecutor, data.ExecutorDataChanged);
        AddTextPinkClass(thPeriod, data.PeriodDataChanged);
        AddTextPinkClass(thCarrier, data.CarrierDataChanged);
        AddTextPinkClass(thResearchCategory, data.ResearchCategoryChanged || data.ResItemNoteChanged);
        AddTextPinkClass(thResInstruments, data.ResInstrumentsChanged);
        AddTextPinkClass(thActivityOverview, data.ActivityOverviewChanged);
        AddTextPinkClass(thActivityFiles, data.ActivityFilesChanged);
        AddTextPinkClass(thSurveyCounties, data.SurveyCountiesChanged);
        AddTextPinkClass(thSurveyScopes, data.SurveyScopesChanged);
        AddTextPinkClass(thSpatialData, data.SpatialDataChanged);
    }

    /// <summary>
    /// 為 th 元素加上或移除 text-pink class
    /// </summary>
    private void AddTextPinkClass(System.Web.UI.HtmlControls.HtmlTableCell thElement, bool shouldAdd)
    {
        if (thElement != null)
        {
            string existingClass = thElement.Attributes["class"] ?? "";
            if (!existingClass.Contains("text-pink") && shouldAdd)
            {
                thElement.Attributes["class"] = string.IsNullOrEmpty(existingClass)
                    ? "text-pink"
                    : existingClass + " text-pink";
            }
        }
    }

    /// <summary>
    /// 移除 th 元素的 text-pink class
    /// </summary>
    private void RemoveTextPinkClass(System.Web.UI.HtmlControls.HtmlTableCell thElement)
    {
        if (thElement != null)
        {
            string existingClass = thElement.Attributes["class"] ?? "";
            if (existingClass.Contains("text-pink"))
            {
                existingClass = existingClass.Replace("text-pink", "").Replace("  ", " ").Trim();
                thElement.Attributes["class"] = existingClass;
            }
        }
    }

    /// <summary>
    /// 取得資料時間文字
    /// </summary>
    private string GetDataPeriodText(int periodId)
    {
        try
        {
            var tbl = OSIDataPeriodsHelper.QueryByID(periodId.ToString());

            if (tbl != null && tbl.Rows.Count > 0)
            {
                var row = tbl.Rows[0];
                return string.Format("{0}年{1}", row["PeriodYear"].ToString(), row["PeriodQuarter"].ToString());
            }
        }
        catch { }
        return "";
    }

    /// <summary>
    /// 取得填報機關文字
    /// </summary>
    private string GetReportingUnitText(int? unitId)
    {
        if (!unitId.HasValue) return "";

        try
        {
            var unit = SysUnitHelper.GetUnitByID(unitId.Value.ToString());

            if (unit != null)
            {
                return unit.UnitName;
            }
        }
        catch { }
        return "";
    }

    /// <summary>
    /// 取得活動性質文字
    /// </summary>
    private string GetActivityNatureText(int natureId)
    {
        try
        {
            var nature = OSIActivityNaturesHelper.GetNatureByID(natureId.ToString());

            if (nature != null)
            {
                return nature.NatureName;
            }
        }
        catch { }
        return "";
    }

    /// <summary>
    /// 取得研究調查項目文字
    /// </summary>
    private string GetResearchItemText(int? itemId)
    {
        if (!itemId.HasValue) return "";

        try
        {
            var item = OSIResearchItemsHelper.GetItemByID(itemId.Value.ToString());

            if (item != null)
            {
                return item.ItemName;
            }
        }
        catch { }
        return "";
    }


    /// <summary>
    /// 取得活動執行者資料
    /// </summary>
    private List<object> GetActivityExecutorsData(int reportId, DateTime auditTime)
    {
        var executorData = new List<object>();
        try
        {
            var executorsTable = OSIActivityReportsHistoryHelper.GetRelatedExecutors(reportId, auditTime);

            if (executorsTable != null && executorsTable.Rows.Count > 0)
            {
                foreach (DataRow row in executorsTable.Rows)
                {
                    executorData.Add(new
                    {
                        CategoryName = row["CategoryName"] != null ? row["CategoryName"].ToString() : "",
                        ExecutorName = row["ExecutorName"] != null ? row["ExecutorName"].ToString() : ""
                    });
                }
            }
        }
        catch { }
        return executorData;
    }

    /// <summary>
    /// 取得研究調查日期資料
    /// </summary>
    private List<object> GetResearchPeriodsData(int reportId, DateTime auditTime)
    {
        var periodData = new List<object>();
        try
        {
            var periodsTable = OSIActivityReportsHistoryHelper.GetRelatedPeriods(reportId, auditTime);

            if (periodsTable != null && periodsTable.Rows.Count > 0)
            {
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
            }
        }
        catch { }
        return periodData;
    }

    /// <summary>
    /// 取得載具資料
    /// </summary>
    private List<object> GetCarriersData(int reportId, DateTime auditTime)
    {
        var carrierData = new List<object>();
        try
        {
            var carriersTable = OSIActivityReportsHistoryHelper.GetRelatedCarriers(reportId, auditTime);

            if (carriersTable != null && carriersTable.Rows.Count > 0)
            {
                foreach (DataRow row in carriersTable.Rows)
                {
                    carrierData.Add(new
                    {
                        CarrierTypeName = row["CarrierTypeName"] != null ? row["CarrierTypeName"].ToString() : "",
                        CarrierDetail = row["CarrierDetail"] != null ? row["CarrierDetail"].ToString() : "",
                        CarrierNo = row["CarrierNo"] != null ? row["CarrierNo"].ToString() : ""
                    });
                }
            }
        }
        catch { }
        return carrierData;
    }

    /// <summary>
    /// 取得最新活動執行者資料
    /// </summary>
    private List<object> GetCurrentActivityExecutorsData(int reportId)
    {
        var executorData = new List<object>();
        try
        {
            var actExecTbl = OSIActivityExecutorsHelper.QueryNameByReportID(reportId.ToString());
            foreach (DataRow row in actExecTbl.Rows)
            {
                executorData.Add(new
                {
                    CategoryName = row["CategoryName"] != null ? row["CategoryName"].ToString() : "",
                    ExecutorName = row["ExecutorName"] != null ? row["ExecutorName"].ToString() : ""
                });
            }
        }
        catch { }
        return executorData;
    }

    /// <summary>
    /// 取得最新研究調查日期資料
    /// </summary>
    private List<object> GetCurrentResearchPeriodsData(int reportId)
    {
        var periodData = new List<object>();
        try
        {
            var resPerTbl = OSIResearchPeriodsHelper.QueryByReportID(reportId.ToString());
            foreach (DataRow row in resPerTbl.Rows)
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
        }
        catch { }
        return periodData;
    }

    /// <summary>
    /// 取得最新載具資料
    /// </summary>
    private List<object> GetCurrentCarriersData(int reportId)
    {
        var carrierData = new List<object>();
        try
        {
            var carrierTbl = OSICarrierHelper.QueryNameByReportID(reportId.ToString());
            foreach (DataRow row in carrierTbl.Rows)
            {
                carrierData.Add(new
                {
                    CarrierTypeName = row["CarrierTypeName"] != null ? row["CarrierTypeName"].ToString() : "",
                    CarrierDetail = row["CarrierDetail"] != null ? row["CarrierDetail"].ToString() : "",
                    CarrierNo = row["CarrierNo"] != null ? row["CarrierNo"].ToString() : ""
                });
            }
        }
        catch { }
        return carrierData;
    }

    /// <summary>
    /// 取得最新活動附件資料
    /// </summary>
    private DataTable GetCurrentActivityFilesData(int reportId)
    {
        try
        {
            return OSIActivityFilesHelper.QueryByReportID(reportId.ToString());
        }
        catch { }
        return null;
    }

    /// <summary>
    /// 取得最新研究調查範圍(描述)資料
    /// </summary>
    private DataTable GetCurrentSurveyScopesData(int reportId)
    {
        try
        {
            var surveyScopes = OSISurveyScopesHelper.QueryByReportIDWithClass(reportId.ToString());
            
            // 將 List<OSI_SurveyScopes> 轉換為 DataTable
            var dataTable = new DataTable();
            dataTable.Columns.Add("ScopeID", typeof(string));
            dataTable.Columns.Add("SurveyScope", typeof(string));
            dataTable.Columns.Add("CreatedAt", typeof(string));
            
            foreach (var scope in surveyScopes)
            {
                DataRow row = dataTable.NewRow();
                row["ScopeID"] = scope.ScopeID;
                row["SurveyScope"] = scope.SurveyScope;
                row["CreatedAt"] = scope.CreatedAt;
                dataTable.Rows.Add(row);
            }
            
            return dataTable;
        }
        catch { }
        return null;
    }

    /// <summary>
    /// 取得最新研究調查範圍(縣市)資料
    /// </summary>
    private DataTable GetCurrentSurveyCountiesData(int reportId)
    {
        try
        {
            return OSISurveyCountiesHelper.QueryByReportID(reportId.ToString());
        }
        catch { }
        return null;
    }

    /// <summary>
    /// 取得活動附件資料
    /// </summary>
    private DataTable GetActivityFilesData(int reportId, DateTime auditTime)
    {
        try
        {
            return OSIActivityReportsHistoryHelper.GetRelatedFiles(reportId, auditTime);
        }
        catch { }
        return null;
    }

    /// <summary>
    /// 取得研究調查範圍(描述)資料
    /// </summary>
    private DataTable GetSurveyScopesData(int reportId, DateTime auditTime)
    {
        try
        {
            return OSIActivityReportsHistoryHelper.GetRelatedScopes(reportId, auditTime);
        }
        catch { }
        return null;
    }

    /// <summary>
    /// 取得研究調查範圍(縣市)資料
    /// </summary>
    private DataTable GetSurveyCountiesData(int reportId, DateTime auditTime)
    {
        try
        {
            return OSIActivityReportsHistoryHelper.GetRelatedCounties(reportId, auditTime);
        }
        catch { }
        return null;
    }

    /// <summary>
    /// 取得空間資料（歷史版本）
    /// </summary>
    private DataTable GetSpatialData(int reportId, DateTime auditTime)
    {
        try
        {
            return OSIActivityReportsHistoryHelper.GetRelatedGeoms(reportId, auditTime);
        }
        catch { }
        return null;
    }

    /// <summary>
    /// 取得最新空間資料
    /// </summary>
    private DataTable GetCurrentSpatialData(int reportId)
    {
        try
        {
            return OSIGeomHelper.QueryByReportID(reportId.ToString());
        }
        catch { }
        return null;
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

/// <summary>
/// 歷史表單資料類別
/// </summary>
public class ReportFormHistoryData
{
    public int ReportId { get; set; }
    public string ActivityName { get; set; }
    public string DataPeriod { get; set; }
    public string Unit { get; set; }
    public string Nature { get; set; }
    public string NatureDetail { get; set; }
    public string ResearchCategory { get; set; }
    public string ResItemNote { get; set; }
    public string ResInstruments { get; set; }
    public string ActivityOverview { get; set; }
    public string CorrectionNotes { get; set; }
    public bool ShowCorrectionNotes { get; set; }
    public string AuditAt { get; set; }
    public string HistoryId { get; set; }

    public List<object> ExecutorData { get; set; }
    public List<object> PeriodData { get; set; }
    public List<object> CarrierData { get; set; }
    public DataTable ActivityFiles { get; set; }
    public DataTable SurveyScopes { get; set; }
    public DataTable SurveyCounties { get; set; }
    public DataTable SpatialData { get; set; }

    // 欄位差異標記屬性
    public bool ActivityNameChanged { get; set; }
    public bool DataPeriodChanged { get; set; }
    public bool UnitChanged { get; set; }
    public bool NatureChanged { get; set; }
    public bool NatureDetailChanged { get; set; }
    public bool ResearchCategoryChanged { get; set; }
    public bool ResItemNoteChanged { get; set; }
    public bool ResInstrumentsChanged { get; set; }
    public bool ActivityOverviewChanged { get; set; }
    public bool ExecutorDataChanged { get; set; }
    public bool PeriodDataChanged { get; set; }
    public bool CarrierDataChanged { get; set; }
    public bool ActivityFilesChanged { get; set; }
    public bool SurveyScopesChanged { get; set; }
    public bool SurveyCountiesChanged { get; set; }
    public bool SpatialDataChanged { get; set; }
}

