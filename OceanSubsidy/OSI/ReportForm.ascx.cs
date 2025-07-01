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

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BasicBind();
            LoadData();
            lastUpdate.Visible = !IsNew;
            // 中華民國月曆需要在頁面載入後才設定readonly
            txtResFrom.Attributes.Add("readonly", "readonly");
            txtResTo.Attributes.Add("readonly", "readonly");
        }

        if (IsPostBack)
        {
            HandleResDeletion();
            HandleExecDeletion();
            HandleFileDeletion();
        }
    }
    // 刪除活動執行者
    private void HandleExecDeletion()
    {
        var target = Request["__EVENTTARGET"];
        var arg = Request["__EVENTARGUMENT"];

        if (target == rptExecList.UniqueID && arg?.StartsWith("DelExec$") == true)
        {
            var parts = arg.Split('$');
            if (parts.Length == 2 && int.TryParse(parts[1], out int rptIdx))
            {
                var visible = ExecList
                    .Where(x => !x.IsDel)
                    .ToList();

                if (rptIdx >= 0 && rptIdx < visible.Count)
                    visible[rptIdx].IsDel = true;
            }
            BindExecRepeater();
        }
    }
    // 刪除研究調查日期
    private void HandleResDeletion()
    {
        var target = Request["__EVENTTARGET"];
        var arg = Request["__EVENTARGUMENT"];

        if (target == rptResList.UniqueID && arg?.StartsWith("DelDate$") == true)
        {
            var parts = arg.Split('$');
            if (parts.Length == 2 && int.TryParse(parts[1], out int rptIdx))
            {
                var visible = ResearchPeriodList.Where(d => !d.IsDel).ToList();
                if (rptIdx >= 0 && rptIdx < visible.Count)
                {
                    visible[rptIdx].IsDel = true;
                }
            }
            BindResRepeater();
        }
    }
    // 刪除檔案
    private void HandleFileDeletion()
    {
        var target = Request["__EVENTTARGET"];
        var arg = Request["__EVENTARGUMENT"];
        if (target == rptActivityFile.UniqueID && arg?.StartsWith("DelAttach$") == true)
        {
            var parts = arg.Split('$');
            if (parts.Length == 2 && int.TryParse(parts[1], out int rptIdx))
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
            }
            BindActivityFileRepeater();
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
            GisTable unitTbl =
                (UserInfo.OSI_RoleName == "系統管理者") ?
                SysUnitHelper.QueryAll() :
                SysUnitHelper.QueryAllChildByID(UserInfo.UnitID.toInt());

            foreach (DataRow r in unitTbl.Rows)
                ddlUnit.Items.Add(new ListItem(r["UnitName"].ToString(), r["UnitID"].ToString()));
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
        txtResearchScope.Text = string.Empty;
        lblLastUpdated.Text = string.Empty;
        // List初始化
        ExecList = new List<ExecutorItem>();
        ResearchPeriodList = new List<PeriodItem>();
        FileList = new List<ActivityFile>();


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
            lblDataPeriod.Text = r["PeriodYear"].ToString() + "年" + r["PeriodQuarter"].ToString();
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
        // 載具種類
        if (ddlCarrierType.Items.FindByValue(report.CarrierTypeID.ToString()) != null)
            ddlCarrierType.SelectedValue = report.CarrierTypeID.ToString();
        // 使用載具名稱、批准文號
        txtCarrierName.Text = report.CarrierDetail;
        txtCarrierApproval.Text = report.CarrierNo;
        // 研究調查項目
        if (ddlResearchCategory.Items.FindByValue(report.ResearchItemID.ToString()) != null)
            ddlResearchCategory.SelectedValue = report.CarrierTypeID.ToString();
        txtResItemNote.Text = report.ResearchItemNote;
        txtResInstruments.Text = report.Instruments;
        txtActivityOverview.Text = report.ActivityOverview;
        txtResearchScope.Text = report.SurveyScope;
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
        lblLastUpdated.Text = report.LastUpdated.ToString("yyyy/MM/dd HH:mm") + " " + unitName + " " + userName;
        // 圖台按鈕新增ReportID
        btnOpenMap.Attributes["data-reportid"] = ReportID.ToString();
        
        // 載入 GeoData (WKT 格式，SRID=3826)
        if (!string.IsNullOrEmpty(report.GeoData))
        {
            // 將 GeoData 存入隱藏欄位，供前端使用
            hdnGeo3826WKT.Value = report.GeoData;
        }

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
        if (DateTimeHelper.TryParseMinguoDate(txtResFrom.Text, out var f)
         && DateTimeHelper.TryParseMinguoDate(txtResTo.Text, out var t)
         && f <= t)
        {
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
            ScriptManager.RegisterStartupScript(this, this.GetType(), "setReadonly", 
                "$('#" + txtResFrom.ClientID + "').attr('readonly', 'readonly');" +
                "$('#" + txtResTo.ClientID + "').attr('readonly', 'readonly');", true);
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

    /// <summary>
    /// 儲存（Insert 或 Update）
    /// </summary>

    protected void btnSave_Click(object sender, EventArgs e)
    {
        // 呼叫 UserControl 的 Save
        bool ok = Save();
        if (ok)
        {
            // 成功後可跳轉或顯示訊息
            ScriptManager.RegisterStartupScript(
                this,
                this.GetType(),
                "saveOk",
                "alert('儲存成功');window.location.href='ActivityReports.aspx';",
                true
            );
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
        reportSave.CarrierTypeID = ddlCarrierType.SelectedValue.toInt();
        if (reportSave.CarrierTypeID == -1) reportSave.CarrierTypeID = null;
        reportSave.CarrierDetail = txtCarrierName.Text;
        reportSave.CarrierNo = txtCarrierApproval.Text;
        reportSave.ResearchItemID = ddlResearchCategory.SelectedValue.toInt();
        if (reportSave.ResearchItemID == -1) reportSave.ResearchItemID = null;
        reportSave.ResearchItemNote = txtResItemNote.Text;
        reportSave.Instruments = txtResInstruments.Text;
        reportSave.ActivityOverview = txtActivityOverview.Text;
        reportSave.SurveyScope = txtResearchScope.Text;
        reportSave.LastUpdated = DateTime.Now;
        reportSave.LastUpdatedBy = UserInfo.UserID.toInt();
        reportSave.IsValid = true;

        // 獲取地圖標記資料 (EPSG:3826 TWD97 / TM2 zone 121)
        string wkt3826 = hdnGeo3826WKT.Value?.Trim();
        reportSave.GeoData = string.IsNullOrEmpty(wkt3826) ? null : wkt3826;

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

        // 新增
        if (IsNew)
        {
            try
            {
                var newReportID = OSIActivityReportsHelper.InsertReport(
                    reportSave, executors, resPeriods, files, delExecutors, delResPeriods, delFiles, BaseDir);
                rtVal = newReportID != 0;
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
                OSIActivityReportsHistoryHelper.InsertReport(oldReport);
            }

            try
            {
                rtVal = OSIActivityReportsHelper.UpdateReport(
                    ReportID, reportSave, executors, resPeriods, files, delExecutors, delResPeriods, delFiles);
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
