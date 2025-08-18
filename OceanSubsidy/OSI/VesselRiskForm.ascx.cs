using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.App;
using GS.Data;
using GS.Extension;
using GS.OCA_OceanSubsidy.Entity;

public partial class OSI_VesselRiskForm : System.Web.UI.UserControl
{
    #region 屬性
    private SessionHelper.UserInfoClass UserInfo
    {
        get => SessionHelper.Get<SessionHelper.UserInfoClass>(
                  SessionHelper.UserInfo)
                ?? new SessionHelper.UserInfoClass();
        set => SessionHelper.Set(SessionHelper.UserInfo, value);
    }

    // AssessmentId 屬性（從父頁面傳入）
    public int AssessmentId
    {
        get
        {
            if (ViewState["AssessmentId"] == null)
                return 0;
            return Convert.ToInt32(ViewState["AssessmentId"]);
        }
        set
        {
            ViewState["AssessmentId"] = value;
            hfAssessmentId.Value = value.ToString();
        }
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

    // 附件清單
    private List<VesselFile> FileList
    {
        get
        {
            var list = ViewState["VesselFileList"] as List<VesselFile>;
            if (list == null)
            {
                list = new List<VesselFile>();
                ViewState["VesselFileList"] = list;
            }
            return list;
        }
        set
        {
            ViewState["VesselFileList"] = value;
        }
    }

    // 新增/編輯模式
    private bool IsNew
    {
        get
        {
            return AssessmentId == 0;
        }
    }
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            InitControls();
            LoadCategories();

            if (!IsNew)
            {
                LoadData();
            }
            else
            {
                hfIsNew.Value = "1";
                txtFormDate.Text = DateTimeHelper.ToMinguoDate(DateTime.Now);
                lastUpdate.Visible = false;
            }
        }
    }

    #region 初始化
    private void InitControls()
    {
        // 設定圖台按鈕的 assessmentId
        btnOpenMap.Attributes["data-assessmentid"] = AssessmentId.ToString();
    }

    // 載入風險類別
    private void LoadCategories()
    {
        var categories = OSIVesselRiskCategoryHelper.QueryAll();
        if (categories != null && categories.Rows.Count > 0)
        {
            cblRiskCategories.DataSource = categories;
            cblRiskCategories.DataTextField = "CategoryName";
            cblRiskCategories.DataValueField = "RiskCategoryId";
            cblRiskCategories.DataBind();
        }
    }
    #endregion

    #region 載入資料
    private void LoadData()
    {
        var assessment = OSIVesselRiskAssessmentsHelper.QueryByID(AssessmentId);
        if (assessment == null || assessment.AssessmentId == 0)
        {
            Response.Redirect("~/OSI/VesselRiskAssessments.aspx");
            return;
        }

        // 基本資料
        txtInvestigator.Text = assessment.Investigator;
        txtUnit.Text = assessment.Unit;
        txtTitle.Text = assessment.Title;
        txtFormDate.Text = DateTimeHelper.ToMinguoDate(assessment.FormDate);

        // 日期時間
        txtStartDate.Text = DateTimeHelper.ToMinguoDate(assessment.StartDate);
        txtStartTime.Text = assessment.StartTime;
        txtStartRemark.Text = assessment.StartRemark;
        txtEndDate.Text = DateTimeHelper.ToMinguoDate(assessment.EndDate);
        txtEndTime.Text = assessment.EndTime;
        txtEndRemark.Text = assessment.EndRemark;
        txtDurationDays.Text = assessment.DurationDays.ToString();

        // 其他欄位
        txtSurveyAreaName.Text = assessment.SurveyAreaName;
        txtVoyagePlanAndOperations.Text = assessment.VoyagePlanAndOperations;

        // 風險評估問題
        SetRadioButtonValue("Q1", assessment.Q_IsOperationInSensitiveArea);
        SetRadioButtonValue("Q2", assessment.Q_IsStayTimeMinimized);
        SetRadioButtonValue("Q3", assessment.Q_HasReducedOrRelocatedStation);
        SetRadioButtonValue("Q4", assessment.Q_KnowsReportingProcedure);
        SetRadioButtonValue("Q5", assessment.Q_HasStrongInterferenceContingencyPlan);

        // 載入選中的類別
        LoadSelectedCategories();

        // 載入附件
        LoadFiles();

        // 載入地理資料
        LoadGeoData();

        // 顯示最後更新資訊
        if (assessment.LastUpdated != null)
        {
            string unitName = "";
            string userName = "";

            if (assessment.LastUpdatedBy.HasValue)
            {
                var user = SysUserHelper.QueryUserByIDWithClass(assessment.LastUpdatedBy.Value.toStr());
                if (user != null)
                {
                    userName = user.Name;
                    var unit = SysUnitHelper.QueryByID(user.UnitID.ToString());
                    if (unit != null && unit.Rows.Count > 0)
                    {
                        unitName = unit.Rows[0]["UnitName"]?.ToString() ?? "";
                    }
                }
            }

            lblLastUpdated.Text = DateTimeHelper.ToMinguoDateTime(assessment.LastUpdated) + " " + unitName + " " + userName;
        }
    }

    // 載入選中的類別
    private void LoadSelectedCategories()
    {
        var selectedCategories = OSIVesselRiskAssessmentCategoriesHelper.QueryByAssessmentId(AssessmentId);
        if (selectedCategories != null && selectedCategories.Rows.Count > 0)
        {
            foreach (DataRow row in selectedCategories.Rows)
            {
                string categoryId = row["RiskCategoryId"].ToString();
                ListItem item = cblRiskCategories.Items.FindByValue(categoryId);
                if (item != null)
                {
                    item.Selected = true;
                }
            }
        }
    }

    // 載入附件
    private void LoadFiles()
    {
        var files = OSIVesselFilesHelper.QueryByAssessmentId(AssessmentId);
        if (files != null && files.Rows.Count > 0)
        {
            FileList.Clear();
            foreach (DataRow row in files.Rows)
            {
                FileList.Add(new VesselFile
                {
                    AttachmentID = Convert.ToInt32(row["AttachmentID"]),
                    FileName = row["FileName"].ToString(),
                    FilePath = row["FilePath"].ToString(),
                    IsDelete = false
                });
            }
            BindFileList();
        }
    }

    // 載入地理資料
    private void LoadGeoData()
    {
        string geoData = OSIVesselGeomHelper.QueryGeoDataByAssessmentId(AssessmentId);
        if (!string.IsNullOrEmpty(geoData))
        {
            hdnGeo3826WKT.Value = geoData;
        }
    }

    // 設定 RadioButton 值
    private void SetRadioButtonValue(string groupName, int value)
    {
        RadioButton rbYes = this.FindControl($"rb{groupName}Yes") as RadioButton;
        RadioButton rbNo = this.FindControl($"rb{groupName}No") as RadioButton;
        RadioButton rbUnknown = this.FindControl($"rb{groupName}Unknown") as RadioButton;

        if (rbYes != null && rbNo != null && rbUnknown != null)
        {
            rbYes.Checked = (value == 1);
            rbNo.Checked = (value == 0);
            rbUnknown.Checked = (value == 2);
        }
    }

    // 取得 RadioButton 值
    private int GetRadioButtonValue(string groupName)
    {
        RadioButton rbYes = this.FindControl($"rb{groupName}Yes") as RadioButton;
        RadioButton rbNo = this.FindControl($"rb{groupName}No") as RadioButton;

        if (rbYes != null && rbYes.Checked) return 1;
        if (rbNo != null && rbNo.Checked) return 0;
        return 2; // 預設未知
    }
    #endregion

    #region 檔案處理
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        if (!fuVesselFile.HasFiles) return;

        // 允許的副檔名清單
        var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { ".doc", ".docx", ".xls", ".xlsx", ".odf", ".ods", ".pdf" };

        foreach (HttpPostedFile pf in fuVesselFile.PostedFiles)
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

                string savePath = GetFilePath(fileName, AssessmentId);
                pf.SaveAs(savePath);

                FileList.Add(new VesselFile
                {
                    AttachmentID = 0,
                    FileName = fileName,
                    FilePath = savePath,
                    IsDelete = false
                });
            }
        }

        BindFileList();
    }

    private string GetFilePath(string fileName, int assessmentId = 0)
    {
        var externalUpload = "";
        if (IsNew)
            externalUpload = Path.Combine(BaseDir, "Temp", $"{Today}_{TempKey}");
        else
            externalUpload = Path.Combine(BaseDir, $"VesselRisk_{AssessmentId}");

        if (!Directory.Exists(externalUpload))
            Directory.CreateDirectory(externalUpload);
        var savePath = Path.Combine(externalUpload, fileName);
        return savePath;
    }

    protected void btnDelFile_Click(object sender, EventArgs e)
    {
        if (int.TryParse(hfDelFileIndex.Value, out int rptIdx))
        {
            var visible = FileList.Where(a => !a.IsDelete).ToList();
            if (rptIdx >= 0 && rptIdx < visible.Count)
            {
                visible[rptIdx].IsDelete = true;
                if (visible[rptIdx].AttachmentID == 0)
                {
                    // 新檔案：刪實體檔，從列表移除
                    var phys = visible[rptIdx].FilePath;
                    if (File.Exists(phys))
                        File.Delete(phys);
                }
            }
            BindFileList();
        }
    }

    protected void rptVesselFile_ItemCommand(object source, RepeaterCommandEventArgs e)
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

    private void BindFileList()
    {
        rptVesselFile.DataSource = FileList.Where(f => !f.IsDelete).ToList();
        rptVesselFile.DataBind();
        upFileList.Update();
    }
    #endregion

    #region 儲存
    protected void btnSave_Click(object sender, EventArgs e)
    {
        // 停用儲存按鈕防止重複點擊
        LinkButton saveButton = sender as LinkButton;
        if (saveButton != null)
        {
            saveButton.Enabled = false;
            saveButton.Text = "儲存中...";
        }

        // 驗證 Main Group
        Page.Validate("Main");
        if (!Page.IsValid)
        {
            // 恢復按鈕狀態
            if (saveButton != null)
            {
                saveButton.Enabled = true;
                saveButton.Text = "儲存";
            }

            ScriptManager.RegisterStartupScript(
                this.Page,
                this.Page.GetType(),
                "SAVE_NotOK",
                "showGlobalMessage('必填欄位未填');",
                true);

            return;
        }

        try
        {
            // 準備主表資料
            OSI_VesselRiskAssessments assessment = IsNew ? new OSI_VesselRiskAssessments() : OSIVesselRiskAssessmentsHelper.QueryByID(AssessmentId);

            // 基本資料
            assessment.Investigator = txtInvestigator.Text.Trim();
            assessment.Unit = txtUnit.Text.Trim();
            assessment.Title = txtTitle.Text.Trim();
            assessment.FormDate = DateTimeHelper.ParseMinguoDate(txtFormDate.Text);

            // 日期時間
            assessment.StartDate = DateTimeHelper.ParseMinguoDate(txtStartDate.Text);
            assessment.StartTime = txtStartTime.Text.Trim();
            assessment.StartRemark = txtStartRemark.Text.Trim();
            assessment.EndDate = DateTimeHelper.ParseMinguoDate(txtEndDate.Text);
            assessment.EndTime = txtEndTime.Text.Trim();
            assessment.EndRemark = txtEndRemark.Text.Trim();

            // 計算天數
            if (!string.IsNullOrEmpty(txtDurationDays.Text))
            {
                assessment.DurationDays = Convert.ToInt32(txtDurationDays.Text);
            }

            // 其他欄位
            assessment.SurveyAreaName = txtSurveyAreaName.Text.Trim();
            assessment.VoyagePlanAndOperations = txtVoyagePlanAndOperations.Text.Trim();

            // 風險評估問題
            assessment.Q_IsOperationInSensitiveArea = GetRadioButtonValue("Q1");
            assessment.Q_IsStayTimeMinimized = GetRadioButtonValue("Q2");
            assessment.Q_HasReducedOrRelocatedStation = GetRadioButtonValue("Q3");
            assessment.Q_KnowsReportingProcedure = GetRadioButtonValue("Q4");
            assessment.Q_HasStrongInterferenceContingencyPlan = GetRadioButtonValue("Q5");

            // 更新資訊
            assessment.LastUpdated = DateTime.Now;
            assessment.LastUpdatedBy = UserInfo.UserID.toInt();
            assessment.IsValid = true;

            // 收集選中的類別
            List<int> selectedCategories = new List<int>();
            foreach (ListItem item in cblRiskCategories.Items)
            {
                if (item.Selected)
                {
                    selectedCategories.Add(Convert.ToInt32(item.Value));
                }
            }

            // 取得地理資料
            string wkt3826 = hdnGeo3826WKT.Value?.Trim();

            // 執行儲存
            bool success = false;
            int newAssessmentId = AssessmentId;

            // 轉換 FileList 為 OSI_VesselFiles
            var files = FileList.Select(f => new OSI_VesselFiles
            {
                AttachmentID = f.AttachmentID,
                FileName = f.FileName,
                FilePath = f.FilePath,
                IsDelete = f.IsDelete
            }).ToList();

            if (IsNew)
            {
                newAssessmentId = OSIVesselRiskAssessmentsHelper.InsertAssessment(
                    assessment, selectedCategories, files, wkt3826, UserInfo.UserID.toInt(), BaseDir, TempKey);
                success = newAssessmentId > 0;

                // 成功，檔案移動
                if (TempKey != null && success)
                {
                    AssessmentId = newAssessmentId;
                    var tempDir = Path.Combine(BaseDir, "Temp", $"{Today}_{TempKey}");
                    var realDir = Path.Combine(BaseDir, "VesselRisk_" + newAssessmentId);

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
            else
            {
                success = OSIVesselRiskAssessmentsHelper.UpdateAssessment(
                    AssessmentId, assessment, selectedCategories, files, wkt3826, UserInfo.UserID.toInt());
            }

            if (success)
            {
                // 構建重新導向的 URL（保留在同頁面）
                string currentPath = this.Page.Request.Path;
                var queryParams = HttpUtility.ParseQueryString(this.Page.Request.QueryString.ToString());

                // 更新或新增 id 參數
                if (IsNew)
                {
                    queryParams["id"] = newAssessmentId.ToString();
                }
                queryParams["saved"] = "1";

                // 組合完整 URL
                string redirectUrl = currentPath + "?" + queryParams.ToString();

                // 執行重新導向（成功時不需要恢復按鈕，因為會離開頁面）
                Response.Redirect(redirectUrl, false);
                Context.ApplicationInstance.CompleteRequest();
            }
            else
            {
                throw new Exception("儲存失敗");
            }
        }
        catch (Exception ex)
        {
            // 儲存失敗，恢復按鈕狀態
            if (saveButton != null)
            {
                saveButton.Enabled = true;
                saveButton.Text = "<i class=\"fas fa-check\"></i>儲存";
            }

            ScriptManager.RegisterStartupScript(
                this.Page,
                this.Page.GetType(),
                "SAVE_ERROR",
                $"showGlobalMessage('儲存失敗，請重新嘗試');",
                true);
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/OSI/VesselRiskAssessments.aspx");
    }
    #endregion

    #region 內部類別
    [Serializable]
    public class VesselFile
    {
        public int AttachmentID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public bool IsDelete { get; set; }
    }
    #endregion
}