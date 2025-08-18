using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using GS.App;
using GS.Data;
using GS.Extension;
using GS.OCA_OceanSubsidy.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public partial class OSI_ReportForm : System.Web.UI.UserControl
{
    #region 屬性
    private SessionHelper.UserInfoClass UserInfo
    {
        get => SessionHelper.Get<SessionHelper.UserInfoClass>(
                  SessionHelper.UserInfo)
                ?? new SessionHelper.UserInfoClass();
        set => SessionHelper.Set(SessionHelper.UserInfo, value);
    }
    // 這份唯一Key
    private string TempKey
    {
        get
        {
            var key = hfTempKey.Value;
            if (string.IsNullOrEmpty(key))
            {
                key = Guid.NewGuid().ToString("N");
                hfTempKey.Value = key;
            }
            return key;
        }
    }
    // 時間
    private string Today
    {
        get
        {
            var key = hfToday.Value;
            if (string.IsNullOrEmpty(key))
            {
                key = DateTime.Now.ToString("yyyyMMdd");
                hfToday.Value = key;
            }
            return key;
        }
    }
    // 活動執行者清單
    private List<ExecutorItem> ExecList
    {
        get
        {
            var list = ViewState["ExecList"] as List<ExecutorItem>;
            if (list == null)
            {
                list = new List<ExecutorItem>();
                ViewState["ExecList"] = list;
            }
            return list;
        }
        set
        {
            ViewState["ExecList"] = value;
        }
    }
    // 研究調查日期
    private List<PeriodItem> ResearchPeriodList
    {
        get
        {
            var list = ViewState["PeriodItem"] as List<PeriodItem>;
            if (list == null)
            {
                list = new List<PeriodItem>();
                ViewState["PeriodItem"] = list;
            }
            return list;
        }
        set
        {
            ViewState["PeriodItem"] = value;
        }
    }
    // 活動附件
    private List<ActivityFile> FileList
    {
        get
        {
            var list = ViewState["ActivityFile"] as List<ActivityFile>;
            if (list == null)
            {
                list = new List<ActivityFile>();
                ViewState["ActivityFile"] = list;
            }
            return list;
        }
        set => ViewState["ActivityFile"] = value;
    }
    // 研究調查範圍清單
    private List<SurveyScopeItem> SurveyScopeList
    {
        get
        {
            var list = ViewState["SurveyScopeList"] as List<SurveyScopeItem>;
            if (list == null)
            {
                list = new List<SurveyScopeItem>();
                ViewState["SurveyScopeList"] = list;
            }
            return list;
        }
        set
        {
            ViewState["SurveyScopeList"] = value;
        }
    }
    // 載具清單
    private List<CarrierItem> CarrierList
    {
        get
        {
            var list = ViewState["CarrierList"] as List<CarrierItem>;
            if (list == null)
            {
                list = new List<CarrierItem>();
                ViewState["CarrierList"] = list;
            }
            return list;
        }
        set
        {
            ViewState["CarrierList"] = value;
        }
    }
    // 研究調查範圍(縣市)清單
    private List<SurveyCountyItem> SurveyCountyList
    {
        get
        {
            var list = ViewState["SurveyCountyList"] as List<SurveyCountyItem>;
            if (list == null)
            {
                list = new List<SurveyCountyItem>();
                ViewState["SurveyCountyList"] = list;
            }
            return list;
        }
        set
        {
            ViewState["SurveyCountyList"] = value;
        }
    }
    public int ReportID
    {
        get => int.TryParse(hfReportID.Value, out var v) ? v : 0;
        set => hfReportID.Value = value.ToString();
    }
    public bool IsNew
    {
        get => bool.TryParse(hfIsNew.Value, out var v) ? v : false;
        set => hfIsNew.Value = value.ToString();
    }
    // 儲存用Report
    private GS.OCA_OceanSubsidy.Entity.OSI_ActivityReports reportSave
    {
        get
        {
            var list = ViewState["reportSave"] as GS.OCA_OceanSubsidy.Entity.OSI_ActivityReports;
            if (list == null)
            {
                list = new OSI_ActivityReports();
                ViewState["reportSave"] = list;
            }
            return list;
        }
        set => ViewState["reportSave"] = value;
    }
    // 檔案基本資料夾路徑
    private string BaseDir
    {
        get
        {
            var webRoot = Server.MapPath("~");
            var projectRoot = Path.GetFullPath(Path.Combine(webRoot, ".."));
            var baseDir = Path.Combine(projectRoot, "UploadFiles", "OSI");
            return baseDir;
        }
    }

    #endregion

    /// <summary>
    /// 取得單位顯示名稱（根據是否為中央機關決定顯示規則）
    /// </summary>
    /// <param name="unitID">單位ID</param>
    /// <returns>單位顯示名稱</returns>
    private string GetUnitDisplayName(int unitID)
    {
        // 查詢單位資訊
        GisTable unitTbl = SysUnitHelper.QueryByID(unitID.ToString());
        if (unitTbl == null || unitTbl.Rows.Count == 0)
            return "";

        DataRow unitRow = unitTbl.Rows[0];
        string unitName = unitRow["UnitName"]?.ToString() ?? "";
        int? govUnitTypeID = unitRow["GovUnitTypeID"] as int?;
        int? parentUnitID = unitRow["ParentUnitID"] as int?;

        // 查詢父單位名稱（如果有的話）
        string parentUnitName = "";
        if (parentUnitID.HasValue)
        {
            GisTable parentTbl = SysUnitHelper.QueryByID(parentUnitID.Value.ToString());
            if (parentTbl != null && parentTbl.Rows.Count > 0)
            {
                parentUnitName = parentTbl.Rows[0]["UnitName"]?.ToString() ?? "";
            }
        }

        // 查詢政府機關類型，判斷是否為中央機關
        bool isCentralGov = false;
        if (govUnitTypeID.HasValue)
        {
            GisTable govTypeTbl = SysGovUnitTypeHelper.QueryAll();
            foreach (DataRow row in govTypeTbl.Rows)
            {
                if (row["TypeID"].ToString().toInt() == govUnitTypeID.Value &&
                    row["TypeName"]?.ToString() == "中央機關")
                {
                    isCentralGov = true;
                    break;
                }
            }
        }

        // 根據是否為中央機關決定顯示規則
        if (isCentralGov)
        {
            // 中央機關：顯示 ParentUnitName + UnitName 或 UnitName（無 Parent 時）
            return string.IsNullOrEmpty(parentUnitName) ? unitName : parentUnitName + unitName;
        }
        else
        {
            // 非中央機關：顯示 ParentUnitName 或 UnitName（無 Parent 時）
            return string.IsNullOrEmpty(parentUnitName) ? unitName : parentUnitName;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BasicBind();
            LoadData();
            lastUpdate.Visible = !IsNew;
            // 中華民國月曆需要在頁面載入後才設定readonly
            //txtResFrom.Attributes.Add("readonly", "readonly");
            //txtResTo.Attributes.Add("readonly", "readonly");
        }
    }

    // 基本綁定
    private void BasicBind()
    {
        // 資料時間
        GisTable tbl = OSIDataPeriodsHelper.QueryByDateTime(DateTime.Now);
        if (tbl != null && tbl.Rows.Count > 0)
        {
            var r = tbl.Rows[0];
            lblDataPeriod.Text = r["PeriodYear"].ToString() + "年" + r["PeriodQuarter"].ToString();
            reportSave.PeriodID = r["PeriodID"].ToString().toInt();
        }
        // 填報機關
        ddlUnit.Items.Clear();
        if (string.IsNullOrEmpty(UserInfo.UnitID))
        {
            ddlUnit.Items.Add(new ListItem("非政府機關", "-1"));
        }
        else
        {
            if (UserInfo.OSI_RoleName == "系統管理者")
            {
                // 系統管理者
                GisTable unitTbl = SysUnitHelper.QueryAllOrderByUnitID();
                foreach (DataRow r in unitTbl.Rows)
                {
                    ddlUnit.Items.Add(new ListItem(
                            r["UnitName"].ToString(),
                            r["UnitID"].ToString()));
                }
            }
            else
            {
                // 非系統管理者
                GisTable dt = SysUnitHelper.QueryAllChildByID(UserInfo.UnitID.toInt());
                foreach (DataRow r in dt.Rows)
                {
                    string displayName = GetUnitDisplayName(r["UnitID"].ToString().toInt());
                    ddlUnit.Items.Add(new ListItem(displayName, r["UnitID"].ToString()));
                }
                ddlUnit.Enabled = false;
            }
            // 設定預設選取為UserInfo.UnitID
            if (ddlUnit.Items.FindByValue(UserInfo.UnitID) != null)
                ddlUnit.SelectedValue = UserInfo.UnitID;
        }
        // 活動名稱
        txtActivityName.Text = string.Empty;
        // 活動性質
        ddlNature.Items.Clear();
        foreach (DataRow r in OSIActivityNaturesHelper.QueryAll().Rows)
            ddlNature.Items.Add(new ListItem(r["NatureName"].ToString(), r["NatureID"].ToString()));
        // 活動性質-補充說明
        txtNatureDetail.Text = string.Empty;
        // 活動執行者
        ddlExecCategory.Items.Clear();
        foreach (DataRow r in OSIExecutorCategoriesHelper.QueryAll().Rows)
            ddlExecCategory.Items.Add(new ListItem(r["CategoryName"].ToString(), r["CategoryID"].ToString()));
        txtExecName.Text = string.Empty;
        // 載具種類
        ddlCarrierType.Items.Clear();
        ddlCarrierType.Items.Add(new ListItem("請選擇", "-1"));
        foreach (DataRow r in OSICarrierTypesHelper.QueryAll().Rows)
            ddlCarrierType.Items.Add(new ListItem(r["CarrierTypeName"].ToString(), r["CarrierTypeID"].ToString()));
        // 使用載具名稱、批准文號
        txtCarrierName.Text = string.Empty;
        txtCarrierApproval.Text = string.Empty;
        // 研究調查項目
        ddlResearchCategory.Items.Clear();
        ddlResearchCategory.Items.Add(new ListItem("請選擇", "-1"));
        foreach (DataRow r in OSIResearchItemsHelper.QueryAll().Rows)
            ddlResearchCategory.Items.Add(new ListItem(r["ItemName"].ToString(), r["ItemID"].ToString()));
        txtResItemNote.Text = string.Empty;
        txtResInstruments.Text = string.Empty;
        txtActivityOverview.Text = string.Empty;
        txtSurveyScope.Text = string.Empty;
        lblLastUpdated.Text = string.Empty;
        txtCorrectionNotes.Text = string.Empty;
        // 研究調查範圍(縣市) - 從資料庫載入
        ddlScopeCounty.Items.Clear();
        ddlScopeCounty.Items.Add(new ListItem("請選擇", ""));
        foreach (DataRow r in OSIMapCountyHelper.QueryAll().Rows)
            ddlScopeCounty.Items.Add(new ListItem(r["c_name"].ToString(), r["qgs_fid"].ToString()));
        // List初始化
        ExecList = new List<ExecutorItem>();
        ResearchPeriodList = new List<PeriodItem>();
        FileList = new List<ActivityFile>();
        SurveyScopeList = new List<SurveyScopeItem>();
        CarrierList = new List<CarrierItem>();
        SurveyCountyList = new List<SurveyCountyItem>();

        // 新增時隱藏歷程按鈕
        btnHistory.Visible = false;

        // 控制標示修正說明欄位的顯示
        SetCorrectionNotesVisibility();


    }

    /// <summary>
    /// 設定修正說明欄位的顯示狀態
    /// </summary>
    private void SetCorrectionNotesVisibility()
    {
        if (IsNew)
        {
            // 新增時不顯示修正說明
            trCorrectionNotes.Visible = false;
            rfvCorrectionNotes.Enabled = false;
        }
        else
        {
            // 編輯時，判斷是否為非填報區間
            bool isOutOfPeriod = !OSIDataPeriodsHelper.IsInPeriodByID(reportSave.PeriodID);
            trCorrectionNotes.Visible = isOutOfPeriod;
            rfvCorrectionNotes.Enabled = isOutOfPeriod;
        }
    }

    // 編輯資料綁定
    public void LoadData()
    {
        if (IsNew) return;
        OSI_ActivityReports report = OSIActivityReportsHelper.QueryByIDWithClass(ReportID.ToString());
        if (report == null) return;
        // 取得資料時間
        reportSave.PeriodID = report.PeriodID;
        GisTable periodTbl = OSIDataPeriodsHelper.QueryByID(report.PeriodID.ToString());
        if (periodTbl != null && periodTbl.Rows.Count > 0)
        {
            var r = periodTbl.Rows[0];
            string dataPeriod = r["PeriodYear"].ToString() + "年" + r["PeriodQuarter"].ToString();
            lblDataPeriod.Text = dataPeriod;
            lblCorrectionNotes.Text = "目前非「" + dataPeriod + "」填報期間，請針對本次修該填寫說明";
        }
        // 填報機關
        if (ddlUnit.Items.FindByValue(report.ReportingUnitID.ToString()) != null)
            ddlUnit.SelectedValue = report.ReportingUnitID.ToString();
        // 活動名稱
        txtActivityName.Text = report.ActivityName;
        // 活動性質
        if (ddlNature.Items.FindByValue(report.NatureID.ToString()) != null)
            ddlNature.SelectedValue = report.NatureID.ToString();
        // 活動性質-補充說明
        txtNatureDetail.Text = report.NatureText;
        var actExecTbl = OSIActivityExecutorsHelper.QueryNameByReportID(ReportID.ToString());
        foreach (DataRow row in actExecTbl.Rows)
        {
            ExecList.Add(new ExecutorItem
            {
                ExecutorID = row["ExecutorID"].ToString(),
                CategoryID = row["CategoryID"].ToString(),
                CategoryName = row["CategoryName"].ToString(),
                ExecutorName = row["ExecutorName"].ToString(),
                IsNew = false,
                IsDel = false,
            });
        }
        BindExecRepeater();
        // 研究調查日期
        var resPerTbl = OSIResearchPeriodsHelper.QueryByReportID(ReportID.ToString());
        foreach (DataRow row in resPerTbl.Rows)
        {
            var start = Convert.ToDateTime(row["StartDate"]);
            var end = Convert.ToDateTime(row["EndDate"]);
            ResearchPeriodList.Add(new PeriodItem
            {
                PeriodID = row["PeriodID"].ToString(),
                StartDate = start,
                EndDate = end,
                StartDateRoc = start.ToMinguoDate(),
                EndDateRoc = end.ToMinguoDate(),
                PeriodLabel = row["PeriodLabel"].ToString(),
                IsNew = false,
                IsDel = false
            });
        }
        BindResRepeater();
        // 載入載具資料
        var carrierTbl = OSICarrierHelper.QueryNameByReportID(ReportID.ToString());
        foreach (DataRow row in carrierTbl.Rows)
        {
            CarrierList.Add(new CarrierItem
            {
                CarrierID = row["CarrierID"].ToString(),
                CarrierTypeID = row["CarrierTypeID"].ToString(),
                CarrierTypeName = row["CarrierTypeName"]?.ToString() ?? "",
                CarrierDetail = row["CarrierDetail"].ToString(),
                CarrierNo = row["CarrierNo"].ToString(),
                IsNew = false,
                IsDel = false
            });
        }
        BindCarrierRepeater();
        // 研究調查項目
        if (ddlResearchCategory.Items.FindByValue(report.ResearchItemID.ToString()) != null)
            ddlResearchCategory.SelectedValue = report.ResearchItemID.ToString();
        txtResItemNote.Text = report.ResearchItemNote;
        txtResInstruments.Text = report.Instruments;
        txtActivityOverview.Text = report.ActivityOverview;
        // 載入研究調查範圍
        var surveyScopes = OSISurveyScopesHelper.QueryByReportIDWithClass(ReportID.ToString());
        foreach (var scope in surveyScopes)
        {
            SurveyScopeList.Add(new SurveyScopeItem
            {
                ScopeID = scope.ScopeID.ToString(),
                SurveyScope = scope.SurveyScope,
                IsNew = false,
                IsDel = false
            });
        }
        BindScopeRepeater();
        // 載入研究調查範圍(縣市)
        var surveyCountyTbl = OSISurveyCountiesHelper.QueryByReportID(ReportID.ToString());
        foreach (DataRow row in surveyCountyTbl.Rows)
        {
            SurveyCountyList.Add(new SurveyCountyItem
            {
                SurveyCountyID = row["SurveyCountyID"].ToString(),
                CountyID = row["CountyID"].ToString(),
                CountyName = row["CountyName"].ToString(),
                IsNew = false,
                IsDel = false
            });
        }
        BindScopeCountyRepeater();
        // 相關附件
        var fileTbl = OSIActivityFilesHelper.QueryByReportID(ReportID.ToString());
        foreach (DataRow row in fileTbl.Rows)
        {
            FileList.Add(new ActivityFile
            {
                AttachmentID = row["AttachmentID"].ToString(),
                FileName = row["FileName"].ToString(),
                FilePath = row["FilePath"].ToString(),
                IsNew = false,
                IsDel = false
            });
        }
        BindActivityFileRepeater();
        // 最後更新時間
        string unitName = SysUserHelper.QueryUnitNameByUserID(report.LastUpdatedBy.ToString());
        string userName = "";
        GisTable tbl = SysUserHelper.QueryUserByID(report.LastUpdatedBy.ToString());
        if (tbl != null && tbl.Rows.Count > 0)
            userName = tbl.Rows[0]["Name"].ToString();
        lblLastUpdated.Text = DateTimeHelper.ToMinguoDateTime(report.LastUpdated) + " " + unitName + " " + userName;
        // 圖台按鈕新增ReportID
        btnOpenMap.Attributes["data-reportid"] = ReportID.ToString();

        // 載入 GeoData (WKT 格式，SRID=3826)
        // 從 OSI_Geom 表的 GeoData 欄位載入
        string geoData = OSIActivityReportsHelper.QueryGeoDataByID(ReportID.ToString());
        if (!string.IsNullOrEmpty(geoData))
        {
            // 將 GeoData 存入隱藏欄位，供前端使用
            hdnGeo3826WKT.Value = geoData;
        }

        // 檢查是否有歷史記錄
        var reportIds = new List<int> { ReportID };
        var historyStatus = OSIActivityReportsHistoryHelper.CheckHistoryExists(reportIds);
        btnHistory.Visible = historyStatus.ContainsKey(ReportID) && historyStatus[ReportID];

        // 控制標示修正說明欄位的顯示
        SetCorrectionNotesVisibility();

    }
    private void BindExecRepeater()
    {
        rptExecList.DataSource = ExecList.Where(d => !d.IsDel);
        rptExecList.DataBind();
        upExecList.Update();
    }
    private void BindResRepeater()
    {
        rptResList.DataSource = ResearchPeriodList.Where(d => !d.IsDel);
        rptResList.DataBind();
        upResList.Update();
    }
    private void BindActivityFileRepeater()
    {
        rptActivityFile.DataSource = FileList.Where(d => !d.IsDel);
        rptActivityFile.DataBind();
        upFileList.Update();
    }
    private void BindScopeRepeater()
    {
        rptScopeList.DataSource = SurveyScopeList.Where(d => !d.IsDel);
        rptScopeList.DataBind();
        upScopeList.Update();
    }
    private void BindCarrierRepeater()
    {
        rptCarrierList.DataSource = CarrierList.Where(d => !d.IsDel);
        rptCarrierList.DataBind();
        upCarrierList.Update();
    }
    private void BindScopeCountyRepeater()
    {
        rptScopeCountyList.DataSource = SurveyCountyList.Where(d => !d.IsDel);
        rptScopeCountyList.DataBind();
        upScopeCounty.Update();
    }
    // 新增活動執行者
    protected void btnAddExec_Click(object sender, EventArgs e)
    {
        var catId = ddlExecCategory.SelectedValue;
        var cat = ddlExecCategory.SelectedItem.Text;
        var name = txtExecName.Text.Trim();
        if (!string.IsNullOrEmpty(name))
        {
            ExecList.Add(new ExecutorItem
            {
                CategoryID = catId,
                CategoryName = cat,
                ExecutorName = name,
                IsNew = true,
                IsDel = false,
            });
            BindExecRepeater();
            txtExecName.Text = "";
        }
    }

    protected void btnAddRes_Click(object sender, EventArgs e)
    {
        try
        {
            if (DateTimeHelper.TryParseMinguoDate(txtResFrom.Text, out var f)
             && DateTimeHelper.TryParseMinguoDate(txtResTo.Text, out var t))
            {
                if (f > t)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "dateError",
                        "showGlobalMessage('結束日期 不可大於 起始日期');", true);
                    return;
                }

                ResearchPeriodList.Add(new PeriodItem
                {
                    StartDate = f,
                    EndDate = t,
                    StartDateRoc = f.ToMinguoDate(),
                    EndDateRoc = t.ToMinguoDate(),
                    PeriodLabel = txtResRemark.Text.Trim(),
                    IsNew = true,
                    IsDel = false
                });
                BindResRepeater();
                txtResFrom.Text = "";
                txtResTo.Text = "";
                txtResRemark.Text = "";

                // 重新設定readonly屬性（因為UpdatePanel更新後會遺失）
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "setReadonly",
                    "$('#" + txtResFrom.ClientID + "').attr('readonly', 'readonly');" +
                    "$('#" + txtResTo.ClientID + "').attr('readonly', 'readonly');", true);
            }
        }
        catch
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "formatError",
                "showGlobalMessage('日期格式錯誤');", true);
        }
    }

    protected void btnAddScope_Click(object sender, EventArgs e)
    {
        var scope = txtSurveyScope.Text.Trim();
        if (!string.IsNullOrEmpty(scope))
        {
            SurveyScopeList.Add(new SurveyScopeItem
            {
                SurveyScope = scope,
                IsNew = true,
                IsDel = false,
            });
            BindScopeRepeater();
            txtSurveyScope.Text = "";
        }
    }

    // 新增載具
    protected void btnAddCarrier_Click(object sender, EventArgs e)
    {
        var typeId = ddlCarrierType.SelectedValue;
        var typeName = ddlCarrierType.SelectedItem.Text;
        var detail = txtCarrierName.Text.Trim();
        var no = txtCarrierApproval.Text.Trim();

        if (typeId != "-1" || !string.IsNullOrEmpty(detail) || !string.IsNullOrEmpty(no))
        {
            CarrierList.Add(new CarrierItem
            {
                CarrierTypeID = typeId,
                CarrierTypeName = typeId != "-1" ? typeName : "",
                CarrierDetail = detail,
                CarrierNo = no,
                IsNew = true,
                IsDel = false
            });
            BindCarrierRepeater();
            // 清空輸入欄位
            ddlCarrierType.SelectedValue = "-1";
            txtCarrierName.Text = "";
            txtCarrierApproval.Text = "";
        }
    }

    // 新增研究調查範圍(縣市)
    protected void btnAddScopeCounty_Click(object sender, EventArgs e)
    {
        var countyId = ddlScopeCounty.SelectedValue;
        var countyName = ddlScopeCounty.SelectedItem.Text;
        
        if (!string.IsNullOrEmpty(countyId))
        {
            // 檢查是否已經存在
            bool exists = SurveyCountyList.Any(x => x.CountyID == countyId && !x.IsDel);
            if (!exists)
            {
                SurveyCountyList.Add(new SurveyCountyItem
                {
                    CountyID = countyId,
                    CountyName = countyName,
                    IsNew = true,
                    IsDel = false
                });
                BindScopeCountyRepeater();
            }
            // 清空選擇
            ddlScopeCounty.SelectedValue = "";
        }
    }

    // 新的刪除事件處理函數
    protected void btnDelExec_Click(object sender, EventArgs e)
    {
        if (int.TryParse(hfDelExecIndex.Value, out int rptIdx))
        {
            var visible = ExecList.Where(x => !x.IsDel).ToList();
            if (rptIdx >= 0 && rptIdx < visible.Count)
                visible[rptIdx].IsDel = true;
            BindExecRepeater();
        }
    }

    protected void btnDelRes_Click(object sender, EventArgs e)
    {
        if (int.TryParse(hfDelResIndex.Value, out int rptIdx))
        {
            var visible = ResearchPeriodList.Where(d => !d.IsDel).ToList();
            if (rptIdx >= 0 && rptIdx < visible.Count)
                visible[rptIdx].IsDel = true;
            BindResRepeater();
        }
    }

    protected void btnDelFile_Click(object sender, EventArgs e)
    {
        if (int.TryParse(hfDelFileIndex.Value, out int rptIdx))
        {
            var visible = FileList.Where(a => !a.IsDel).ToList();
            if (rptIdx >= 0 && rptIdx < visible.Count)
            {
                visible[rptIdx].IsDel = true;
                if (visible[rptIdx].IsNew)
                {
                    // 新檔案：刪實體檔，從列表移除
                    var phys = visible[rptIdx].FilePath;
                    if (File.Exists(phys))
                        File.Delete(phys);
                }
            }
            BindActivityFileRepeater();
        }
    }

    protected void btnDelScope_Click(object sender, EventArgs e)
    {
        if (int.TryParse(hfDelScopeIndex.Value, out int rptIdx))
        {
            var visible = SurveyScopeList.Where(x => !x.IsDel).ToList();
            if (rptIdx >= 0 && rptIdx < visible.Count)
                visible[rptIdx].IsDel = true;
            BindScopeRepeater();
        }
    }

    protected void btnDelCarrier_Click(object sender, EventArgs e)
    {
        if (int.TryParse(hfDelCarrierIndex.Value, out int rptIdx))
        {
            var visible = CarrierList.Where(x => !x.IsDel).ToList();
            if (rptIdx >= 0 && rptIdx < visible.Count)
                visible[rptIdx].IsDel = true;
            BindCarrierRepeater();
        }
    }

    protected void btnDelScopeCounty_Click(object sender, EventArgs e)
    {
        if (int.TryParse(hfDelScopeCountyIndex.Value, out int rptIdx))
        {
            var visible = SurveyCountyList.Where(x => !x.IsDel).ToList();
            if (rptIdx >= 0 && rptIdx < visible.Count)
                visible[rptIdx].IsDel = true;
            BindScopeCountyRepeater();
        }
    }

    protected void btnUpload_Click(object sender, EventArgs e)
    {
        if (!fuActivityFile.HasFiles) return;

        // 允許的副檔名清單
        var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { ".doc", ".docx", ".xls", ".xlsx", ".odf", ".ods", ".pdf" };

        foreach (HttpPostedFile pf in fuActivityFile.PostedFiles)
        {
            var ext = Path.GetExtension(pf.FileName) ?? "";
            if (allowed.Contains(ext))
            {
                var baseName = Path.GetFileNameWithoutExtension(pf.FileName);
                var fileName = baseName + ext;
                int idx = 1;
                while (FileList.Select(f => f.FileName).Contains(fileName))
                {
                    fileName = $"{baseName}({idx}){ext}";
                    idx++;
                }

                string savePath = GetFilePath(fileName, ReportID);
                pf.SaveAs(savePath);

                FileList.Add(new ActivityFile
                {
                    FileName = fileName,
                    FilePath = savePath,
                    IsNew = true,
                    IsDel = false
                });
            }
        }

        BindActivityFileRepeater();
    }

    private string GetFilePath(string fileName, int reportID = 0)
    {
        var externalUpload = "";
        if (IsNew)
            externalUpload = Path.Combine(BaseDir, "Temp", $"{Today}_{TempKey}");
        else
            externalUpload = Path.Combine(BaseDir, $"Report_{ReportID}");

        if (!Directory.Exists(externalUpload))
            Directory.CreateDirectory(externalUpload);
        var savePath = Path.Combine(externalUpload, fileName);
        return savePath;
    }

    protected void rptActivityFile_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Download")
        {
            string fullPath = e.CommandArgument.ToString();
            string fileName = Path.GetFileName(fullPath);

            // 以動態 Response 送檔
            Response.Clear();
            Response.ContentType = MimeMapping.GetMimeMapping(fullPath);
            Response.AddHeader("Content-Disposition", $"attachment; filename=\"{fileName}\"");
            Response.TransmitFile(fullPath);
            Response.End();
        }
    }

    protected void cvExecList_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = ExecList.Any(x => !x.IsDel);
    }

    protected void cvResList_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = ResearchPeriodList.Any(x => !x.IsDel);
    }

    protected void cvScopeValidation_ServerValidate(object source, ServerValidateEventArgs args)
    {
        // 檢查是否至少有一個縣市或一個描述
        bool hasCounty = SurveyCountyList.Any(x => !x.IsDel);
        bool hasScope = SurveyScopeList.Any(x => !x.IsDel);
        args.IsValid = hasCounty || hasScope;
    }

    /// <summary>
    /// 儲存（Insert 或 Update）
    /// </summary>
    protected void btnSave_Click(object sender, EventArgs e)
    {
        // 停用儲存按鈕防止重複點擊
        Button saveButton = sender as Button;
        if (saveButton != null)
        {
            saveButton.Enabled = false;
            saveButton.Text = "儲存中...";
        }

        Page.Validate("Main");
        if (!Page.IsValid)
        {
            if (saveButton != null)
            {
                saveButton.Enabled = true;
                saveButton.Text = "儲存";
            }

            ScriptManager.RegisterStartupScript(
                this.Page,
                this.Page.GetType(),
                "saveNotOk",
                "showGlobalMessage('必填欄位未填');",
                true
            );

            return;
        }

        // 呼叫 UserControl 的 Save
        bool ok = Save();
        if (ok)
        {
            // 構建重新導向的 URL
            string currentPath = this.Page.Request.Path;
            var queryParams = HttpUtility.ParseQueryString(this.Page.Request.QueryString.ToString());
            
            // 更新或新增 id 參數
            queryParams["id"] = ReportID.ToString();
            queryParams["saved"] = "1";
            
            // 組合完整 URL
            string redirectUrl = currentPath + "?" + queryParams.ToString();
            
            // 執行重新導向（成功時不需要恢復按鈕，因為會離開頁面）
            Response.Redirect(redirectUrl, false);
            Context.ApplicationInstance.CompleteRequest();
        }
        else
        {
            // 儲存失敗，恢復按鈕狀態
            if (saveButton != null)
            {
                saveButton.Enabled = true;
                saveButton.Text = "儲存";
            }

            ScriptManager.RegisterStartupScript(
                this.Page,
                this.Page.GetType(),
                "saveNotOk",
                "showGlobalMessage('儲存失敗，請重新嘗試');",
                true
            );
        }
    }

    /// <summary>
    /// 查看歷程
    /// </summary>
    protected void btnHistory_Click(object sender, EventArgs e)
    {
        if (ReportID > 0)
        {
            Response.Redirect($"~/OSI/ActivityReportHistory.aspx?ReportID={ReportID}");
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        string targetUrl = DetermineReturnUrl();
        SafeRedirect(targetUrl);
    }

    /// <summary>
    /// 決定返回的 URL
    /// </summary>
    private string DetermineReturnUrl()
    {
        // 1. 優先使用 QueryString
        string returnUrl = Request.QueryString["returnUrl"];
        if (IsValidAndSafeUrl(returnUrl))
            return returnUrl;

        // 2. 其次使用 Referrer
        string referrer = Request.UrlReferrer?.AbsolutePath;
        if (!string.IsNullOrEmpty(referrer))
        {
            if (referrer.Contains("ActivityManage.aspx"))
                return "~/OSI/ActivityManage.aspx";
        }

        // 3. 預設值
        return "~/OSI/ActivityReports.aspx";
    }

    /// <summary>
    /// 驗證 URL 是否安全且有效
    /// </summary>
    private bool IsValidAndSafeUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
            return false;

        try
        {
            // 必須是相對路徑
            if (!url.StartsWith("~/"))
                return false;

            // 不能包含危險字符
            if (url.Contains("..") || url.Contains("//") || url.Contains(@"\\"))
                return false;

            // 必須是 .aspx 檔案
            if (!url.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
                return false;

            // 必須在 OSI 資料夾下
            if (!url.StartsWith("~/OSI/", StringComparison.OrdinalIgnoreCase))
                return false;

            // 檢查實體檔案是否存在
            string physicalPath = Server.MapPath(url);
            return File.Exists(physicalPath);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 安全的重定向
    /// </summary>
    private void SafeRedirect(string url)
    {
        try
        {
            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }
        catch (Exception ex)
        {
            // 記錄錯誤（如果有日誌系統）
            // LogError(ex);

            // 重定向到預設頁面
            try
            {
                Response.Redirect("~/OSI/ActivityReports.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch
            {
                // 最後的防護：顯示錯誤訊息
                Response.Clear();
                Response.Write("無法導向頁面，請手動返回。");
                Response.End();
            }
        }
    }

    public bool Save()
    {
        bool rtVal = false;
        Page.Validate("Main");
        if (!Page.IsValid) return false;

        // 填報資料
        reportSave.ReportID = ReportID;
        reportSave.ReportingUnitID = ddlUnit.SelectedValue.toInt();
        if (reportSave.ReportingUnitID == -1) reportSave.ReportingUnitID = null;
        reportSave.ActivityName = txtActivityName.Text;
        reportSave.NatureID = ddlNature.SelectedValue.toInt();
        reportSave.NatureText = txtNatureDetail.Text;
        reportSave.ResearchItemID = ddlResearchCategory.SelectedValue.toInt();
        if (reportSave.ResearchItemID == -1) reportSave.ResearchItemID = null;
        reportSave.ResearchItemNote = txtResItemNote.Text;
        reportSave.Instruments = txtResInstruments.Text;
        reportSave.ActivityOverview = txtActivityOverview.Text;
        reportSave.LastUpdated = DateTime.Now;
        reportSave.LastUpdatedBy = UserInfo.UserID.toInt();
        reportSave.IsValid = true;

        // 獲取地圖標記資料 (EPSG:3826 TWD97 / TM2 zone 121)
        string wkt3826 = hdnGeo3826WKT.Value?.Trim();

        // 活動執行者
        List<OSI_ActivityExecutors> executors = new List<OSI_ActivityExecutors>();
        List<int> delExecutors = new List<int>();
        ExecList.ForEach(e =>
        {
            if (e.IsNew && !e.IsDel)
            {
                executors.Add(new OSI_ActivityExecutors
                {
                    ExecutorID = e.ExecutorID.toInt(),
                    CategoryID = e.CategoryID.toInt(),
                    ExecutorName = e.ExecutorName,
                    IsValid = true,
                });
            }
            if (!e.IsNew && e.IsDel && !string.IsNullOrWhiteSpace(e.ExecutorID))
                delExecutors.Add(e.ExecutorID.toInt());
        });
        // 研究調查日期
        List<OSI_ResearchPeriods> resPeriods = new List<OSI_ResearchPeriods>();
        List<int> delResPeriods = new List<int>();
        ResearchPeriodList.ForEach(e =>
        {
            if (e.IsNew && !e.IsDel)
            {
                resPeriods.Add(new OSI_ResearchPeriods
                {
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    PeriodLabel = e.PeriodLabel,
                    IsValid = true,
                });
            }
            if (!e.IsNew && e.IsDel && !string.IsNullOrWhiteSpace(e.PeriodID))
                delResPeriods.Add(e.PeriodID.toInt());
        });
        // 活動檔案
        List<OSI_ActivityFiles> files = new List<OSI_ActivityFiles>();
        List<int> delFiles = new List<int>();
        FileList.ForEach(e =>
        {
            if (e.IsNew && !e.IsDel)
            {
                files.Add(new OSI_ActivityFiles
                {
                    FileName = e.FileName,
                    FilePath = e.FilePath,
                    IsValid = true,
                });
            }
            if (!e.IsNew && e.IsDel && !string.IsNullOrWhiteSpace(e.AttachmentID))
                delFiles.Add(e.AttachmentID.toInt());
        });
        // 研究調查範圍
        List<OSI_SurveyScopes> surveyScopes = new List<OSI_SurveyScopes>();
        List<int> delSurveyScopes = new List<int>();
        SurveyScopeList.ForEach(e =>
        {
            if (e.IsNew && !e.IsDel)
            {
                surveyScopes.Add(new OSI_SurveyScopes
                {
                    SurveyScope = e.SurveyScope,
                    IsValid = true,
                });
            }
            if (!e.IsNew && e.IsDel && !string.IsNullOrWhiteSpace(e.ScopeID))
                delSurveyScopes.Add(e.ScopeID.toInt());
        });
        // 載具
        List<OSI_Carrier> carriers = new List<OSI_Carrier>();
        List<int> delCarriers = new List<int>();
        CarrierList.ForEach(e =>
        {
            if (e.IsNew && !e.IsDel)
            {
                int? carrierTypeId = e.CarrierTypeID.toInt();
                if (carrierTypeId == -1) carrierTypeId = null; // 如果是-1則設定為null
                carriers.Add(new OSI_Carrier
                {
                    CarrierTypeID = carrierTypeId,
                    CarrierDetail = e.CarrierDetail,
                    CarrierNo = e.CarrierNo,
                    IsValid = true,
                });
            }
            if (!e.IsNew && e.IsDel && !string.IsNullOrWhiteSpace(e.CarrierID))
                delCarriers.Add(e.CarrierID.toInt());
        });
        // 研究調查範圍(縣市)
        List<OSI_SurveyCounties> surveyCounties = new List<OSI_SurveyCounties>();
        List<int> delSurveyCounties = new List<int>();
        SurveyCountyList.ForEach(e =>
        {
            if (e.IsNew && !e.IsDel)
            {
                surveyCounties.Add(new OSI_SurveyCounties
                {
                    CountyID = e.CountyID.toInt(),
                    IsValid = true,
                });
            }
            if (!e.IsNew && e.IsDel && !string.IsNullOrWhiteSpace(e.SurveyCountyID))
                delSurveyCounties.Add(e.SurveyCountyID.toInt());
        });

        // 新增
        if (IsNew)
        {
            try
            {
                var newReportID = OSIActivityReportsHelper.InsertReport(
                    reportSave, executors, resPeriods, files, surveyScopes, carriers, surveyCounties, delExecutors, delResPeriods, delFiles, delSurveyScopes, delCarriers, delSurveyCounties, BaseDir);
                rtVal = newReportID != 0;
                
                // 儲存圖徵資料到 OSI_Geom 表
                if (rtVal && newReportID > 0)
                {
                    try
                    {
                        OSIActivityReportsHelper.SaveGeometries(newReportID, wkt3826, UserInfo.UserID.toInt());
                    }
                    catch (Exception ex)
                    {
                        // 記錄錯誤但不影響主要儲存流程
                        System.Diagnostics.Debug.WriteLine($"儲存圖徵資料失敗：{ex.Message}");
                    }
                }
                
                // 成功，檔案移動
                if (TempKey != null && rtVal)
                {
                    ReportID = newReportID;
                    var tempDir = Path.Combine(BaseDir, "Temp", $"{Today}_{TempKey}");
                    var realDir = Path.Combine(BaseDir, "Report_" + newReportID);

                    if (Directory.Exists(tempDir))
                    {
                        // 如果目標已存在可先刪除或合併
                        if (Directory.Exists(realDir))
                            Directory.Delete(realDir, true);

                        Directory.Move(tempDir, realDir);
                        FileList.ForEach(f => f.FilePath = Path.Combine(realDir, f.FileName));
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        else
        // 更新
        {
            // 若在非填報區間內更新，會有歷史紀錄            
            if (!OSIDataPeriodsHelper.IsInPeriodByID(reportSave.PeriodID))
            {
                OSI_ActivityReports oldReport = OSIActivityReportsHelper.QueryByIDWithClass(ReportID.ToString());
                string correctionNotes = txtCorrectionNotes.Text.Trim();
                OSIActivityReportsHistoryHelper.InsertReport(oldReport, correctionNotes);
            }

            try
            {
                rtVal = OSIActivityReportsHelper.UpdateReport(
                    ReportID, reportSave, executors, resPeriods, files, surveyScopes, carriers, surveyCounties, delExecutors, delResPeriods, delFiles, delSurveyScopes, delCarriers, delSurveyCounties);
                
                // 儲存圖徵資料到 OSI_Geom 表
                if (rtVal)
                {
                    try
                    {
                        OSIActivityReportsHelper.SaveGeometries(ReportID, wkt3826, UserInfo.UserID.toInt());
                    }
                    catch (Exception ex)
                    {
                        // 記錄錯誤但不影響主要儲存流程
                        System.Diagnostics.Debug.WriteLine($"儲存圖徵資料失敗：{ex.Message}");
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        return rtVal;
    }
}

[Serializable]
public class ExecutorItem
{
    public string ExecutorID { get; set; }
    public string CategoryID { get; set; }
    public string CategoryName { get; set; }
    public string ExecutorName { get; set; }
    public bool IsNew { get; set; }
    public bool IsDel { get; set; }
}

[Serializable]
public class PeriodItem
{
    public string PeriodID { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string StartDateRoc { get; set; }
    public string EndDateRoc { get; set; }
    public string PeriodLabel { get; set; }
    public bool IsNew { get; set; }
    public bool IsDel { get; set; }
}

[Serializable]
public class ActivityFile
{
    public string AttachmentID { get; set; }
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public bool IsNew { get; set; }
    public bool IsDel { get; set; }
}

[Serializable]
public class SurveyScopeItem
{
    public string ScopeID { get; set; }
    public string SurveyScope { get; set; }
    public bool IsNew { get; set; }
    public bool IsDel { get; set; }
}

[Serializable]
public class CarrierItem
{
    public string CarrierID { get; set; }
    public string CarrierTypeID { get; set; }
    public string CarrierTypeName { get; set; }
    public string CarrierDetail { get; set; }
    public string CarrierNo { get; set; }
    public bool IsNew { get; set; }
    public bool IsDel { get; set; }
}

[Serializable]
public class SurveyCountyItem
{
    public string SurveyCountyID { get; set; }
    public string CountyID { get; set; }
    public string CountyName { get; set; }
    public bool IsNew { get; set; }
    public bool IsDel { get; set; }
}
