using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Model.OFS;
using GS.OCA_OceanSubsidy.Operation.OFS;

/// <summary>
/// 科專計畫建議迴避之審查委員清單與技術能力 UserControl
/// 可重複使用的委員迴避清單和技術能力管理元件
/// </summary>
public partial class OFS_SCI_UserControls_SciRecusedListControl : System.Web.UI.UserControl
{
    #region 屬性

    /// <summary>
    /// 目前的計畫ID
    /// </summary>
    protected string ProjectID => Request.QueryString["ProjectID"];

    /// <summary>
    /// 是否為檢視模式
    /// </summary>
    public bool IsViewMode { get; set; }
    public string SourcePage { get; set; }

    #endregion

    #region 頁面事件

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                InitializeControl();

                if (!string.IsNullOrEmpty(ProjectID))
                {
                    LoadData(ProjectID);
                    // 載入變更說明控制項
                    tab4_ucChangeDescription.LoadData(ProjectID);
                }
            }
     

            // 檢查表單狀態並控制暫存按鈕顯示
            CheckFormStatusAndHideTempSaveButton();
        }
        catch (Exception ex)
        {
            HandleException(ex, "UserControl 載入時發生錯誤");
        }
    }

    #endregion

    #region 公開方法

    /// <summary>
    /// 載入資料到控制項
    /// </summary>
    /// <param name="projectID">計畫ID</param>
    /// <param name="isViewMode">是否為檢視模式</param>
    public void LoadData(string projectID)
    {
        try
        {

            if (!string.IsNullOrEmpty(projectID))
            {
                LoadExistingData(projectID);
                LoadTechDiagramFile(projectID);
            }


            // 套用檢視模式
            if (IsViewMode)
            {
                ApplyViewMode();
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入資料時發生錯誤");
        }
    }

    /// <summary>
    /// 從快照資料載入（用於快照檢視頁面）
    /// </summary>
    /// <param name="snapshotData">快照的 JSON 資料物件</param>
    public void LoadFromSnapshot(dynamic snapshotData)
    {
        try
        {
            // 載入迴避委員清單資料
            if (snapshotData.OtherRecused != null)
            {
                var recusedList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OFS_SCI_Other_Recused>>(
                    snapshotData.OtherRecused.ToString()
                );
                if (recusedList != null && recusedList.Count > 0)
                {
                    hdnCommitteeData.Value = Newtonsoft.Json.JsonConvert.SerializeObject(recusedList);
                }
            }

            // 載入技術能力資料
            if (snapshotData.OtherTechReadiness != null)
            {
                var techReadinessList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OFS_SCI_Other_TechReadiness>>(
                    snapshotData.OtherTechReadiness.ToString()
                );
                if (techReadinessList != null && techReadinessList.Count > 0)
                {
                    hdnTechData.Value = Newtonsoft.Json.JsonConvert.SerializeObject(techReadinessList);
                }
            }

            // 載入技術架構圖檔案
            if (snapshotData.UploadFile != null)
            {
                List<OFS_SCI_UploadFile> uploadFiles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OFS_SCI_UploadFile>>(
                    snapshotData.UploadFile.ToString()
                );
                if (uploadFiles != null && uploadFiles.Count > 0)
                {
                    OFS_SCI_UploadFile techDiagramFile = uploadFiles.FirstOrDefault(f => f.FileCode == "F06");
                    if (techDiagramFile != null && !string.IsNullOrEmpty(techDiagramFile.TemplatePath))
                    {
                        // 將檔案資訊序列化到 hdnUploadedFile
                        var fileInfo = new
                        {
                            fileName = techDiagramFile.FileName,
                            filePath = techDiagramFile.TemplatePath
                        };
                        hdnUploadedFile.Value = Newtonsoft.Json.JsonConvert.SerializeObject(fileInfo);
                    }
                }
            }

            // 設定為檢視模式
            IsViewMode = true;
            ApplyViewMode();

            // 隱藏變更說明控制項（快照檢視不需要）
            if (tab4_ucChangeDescription != null)
            {
                tab4_ucChangeDescription.Visible = false;
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "從快照載入資料時發生錯誤");
        }
    }

    /// <summary>
    /// 驗證表單資料
    /// </summary>
    /// <returns>驗證結果</returns>
    public ValidationResult ValidateForm()
    {
        var result = new ValidationResult();

        try
        {
            // 取得 OrgCategory 判斷是否需要驗證技術能力
            string orgCategory = "";
            if (!string.IsNullOrEmpty(ProjectID))
            {
                var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(ProjectID);
                orgCategory = applicationMain?.OrgCategory ?? "";
            }
            bool isOceanTech = orgCategory == "OceanTech";

            // 取得表單資料
            var committeeData = GetCommitteeDataFromForm();

            // 從 Request.Form 讀取 checkbox 狀態
            bool noAvoidanceCommittee = false;
            string checkboxValue = HttpContext.Current.Request.Form[chkNoAvoidance.UniqueID];
            if (!string.IsNullOrEmpty(checkboxValue))
            {
                noAvoidanceCommittee = checkboxValue.ToLower() == "on" || checkboxValue.ToLower() == "true";
            }

            // 如果沒有勾選「無需迴避」，則必須至少有一筆資料
            if (!noAvoidanceCommittee && committeeData.Count == 0)
            {
                result.AddError("請填寫委員迴避資料或勾選「無需迴避之審查委員」");
            }

            // 驗證委員資料必填欄位
            foreach (var item in committeeData)
            {
                if (string.IsNullOrWhiteSpace(item.RecusedName) ||
                    string.IsNullOrWhiteSpace(item.EmploymentUnit) ||
                    string.IsNullOrWhiteSpace(item.JobTitle) ||
                    string.IsNullOrWhiteSpace(item.RecusedReason))
                {
                    result.AddError("委員迴避清單中有必填欄位未填寫");
                    break;
                }
            }

            // 當 OrgCategory != "OceanTech" 時才驗證技術能力資料
            if (!isOceanTech)
            {
                // 驗證技術能力資料
                var techData = GetTechDataFromForm();
                if (techData.Count == 0)
                {
                    result.AddError("請至少填寫一筆技術能力資料");
                }
                foreach (var item in techData)
                {
                    if (string.IsNullOrWhiteSpace(item.Name) ||
                        string.IsNullOrWhiteSpace(item.Bef_TRLevel) ||
                        string.IsNullOrWhiteSpace(item.Aft_TRLevel) ||
                        string.IsNullOrWhiteSpace(item.Description))
                    {
                        result.AddError("技術能力資料中有必填欄位未填寫");
                        break;
                    }
                }
            }

            // 驗證技術能力與技術關聯圖是否已上傳
            if (!string.IsNullOrEmpty(ProjectID))
            {
                var uploadedFiles = OFS_SciWorkSchHelper.GetUploadFilesByProjectIDAndFileCode(ProjectID, "TechnologyDiagram");
                if (uploadedFiles == null || !uploadedFiles.Any())
                {
                    result.AddError("請上傳技術能力與技術關聯圖");
                }
            }

        }
        catch (Exception ex)
        {
            result.AddError($"驗證過程發生錯誤：{ex.Message}");
        }

        return result;
    }

    /// <summary>
    /// 儲存資料
    /// </summary>
    /// <returns>儲存是否成功</returns>
    public bool SaveData(string projectID)
    {
        try
        {
            // 取得表單資料
            var committeeData = GetCommitteeDataFromForm();
            var techData = GetTechDataFromForm();

            // 從 Request.Form 讀取 checkbox 狀態
            bool isChkNoAvoidance = false;
            string checkboxValue = HttpContext.Current.Request.Form[chkNoAvoidance.UniqueID];
            if (!string.IsNullOrEmpty(checkboxValue))
            {
                isChkNoAvoidance = checkboxValue.ToLower() == "on" || checkboxValue.ToLower() == "true";
            }

            // 儲存委員迴避清單
            OFS_SciRecusedList.ReplaceRecusedList(committeeData, ProjectID,isChkNoAvoidance);
            OFS_SciRecusedList.UpdateIsRecused(ProjectID, isChkNoAvoidance);
            // 儲存技術能力資料
            OFS_SciRecusedList.ReplaceTechReadinessList(techData, ProjectID);

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"儲存資料時發生錯誤：{ex.Message}", ex);
        }
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 初始化控制項
    /// </summary>
    private void InitializeControl()
    {
        // 初始化隱藏欄位
        hdnCommitteeData.Value = "[]";
        hdnTechData.Value = "[]";
        hdnUploadedFile.Value = "";
    }

    /// <summary>
    /// 載入現有資料
    /// </summary>
    private void LoadExistingData(string projectID)
    {
        try
        {
            // 載入委員迴避清單資料
            var recusedList = OFS_SciRecusedList.GetRecusedListByProjectID(projectID);
            var recusedData = recusedList.Select(x => new
            {
                committeeName = x.RecusedName,
                committeeUnit = x.EmploymentUnit,
                committeePosition = x.JobTitle,
                committeeReason = x.RecusedReason
            }).ToArray();

            // 載入技術成熟度資料
            var techList = OFS_SciRecusedList.GetTechReadinessListByProjectID(projectID);
            var techData = techList.Select(x => new
            {
                techItem = x.Name,
                trlPlanLevel = x.Bef_TRLevel,
                trlTrackLevel = x.Aft_TRLevel,
                techProcess = x.Description
            }).ToArray();

            // 載入 IsRecused 狀態
            bool isRecused = OFS_SciRecusedList.GetIsRecusedByProjectID(projectID);

            // 取得 OrgCategory
            var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);
            string orgCategory = applicationMain?.OrgCategory ?? "";

            // 如果 OrgCategory 是 OceanTech，在伺服器端隱藏技術能力表格
            if (orgCategory == "OceanTech")
            {
                techTable.Attributes["class"] = (techTable.Attributes["class"] ?? "table-responsive mt-3 mb-0") + " d-none";
            }

            // 設定 checkbox 狀態
            chkNoAvoidance.Checked = isRecused;

            // 將資料傳遞到前端
            var dataToSend = new
            {
                recusedData = recusedData,
                techData = techData,
                isRecused = isRecused,
                orgCategory = orgCategory
            };

            var dataJson = new JavaScriptSerializer().Serialize(dataToSend);

            string script = $@"
                window.existingData = {dataJson};

                // 延遲載入資料，確保 JavaScript 已初始化
                setTimeout(function() {{
                    if (typeof loadExistingData === 'function') {{
                        loadExistingData();
                        console.log('已載入現有資料：', window.existingData);

                        // 如果 IsRecused 為 true，需要鎖定委員表格
                        if (window.existingData.isRecused && typeof clearAndLockCommitteeTable === 'function') {{
                            const $tbody = $('#committeeTableBody');
                            clearAndLockCommitteeTable($tbody);
                            console.log('已鎖定委員表格，因為 IsRecused 為 true');
                        }}
                    }}
                }}, 200);
            ";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "LoadExistingData", script, true);
        }
        catch (Exception ex)
        {
            // 如果載入失敗，設定空資料
            string fallbackScript = @"
                window.existingData = { recusedData: [], techData: [], isRecused: false };
                console.log('載入資料失敗，使用預設值');
            ";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "LoadExistingData", fallbackScript, true);
            HandleException(ex, "載入現有資料時發生錯誤");
        }
    }

    /// <summary>
    /// 載入技術能力與技術關聯圖
    /// </summary>
    private void LoadTechDiagramFile(string projectID)
    {
        try
        {
            var uploadFiles = OFS_SciWorkSchHelper.GetUploadFilesByProjectIDAndFileCode(projectID, "TechnologyDiagram");

            if (uploadFiles.Any())
            {
                var file = uploadFiles.First();
                string appRootPath = ConfigurationManager.AppSettings["AppRootPath"] ?? "";

                // 設定 ImageUrl
                // if (!string.IsNullOrEmpty(appRootPath))
                // {
                //     file.TemplatePath = ResolveUrl($"~/{appRootPath}/{file.TemplatePath}");
                // }
                // else
                // {
                //     file.TemplatePath = ResolveUrl($"~/{file.TemplatePath}");
                // }
                file.TemplatePath = ResolveUrl($"~/{file.TemplatePath}");

                var script = $@"
                    setTimeout(function() {{
                        if (window.techDiagramManager) {{
                            window.techDiagramManager.loadTechDiagramFile('{file.TemplatePath}', '{file.FileName}');
                        }} else {{
                            setTimeout(function() {{
                                if (window.techDiagramManager) {{
                                    window.techDiagramManager.loadTechDiagramFile('{file.TemplatePath}', '{file.FileName}');
                                }}
                            }}, 1000);
                        }}
                    }}, 100);
                ";

                Page.ClientScript.RegisterStartupScript(this.GetType(), "LoadTechDiagramFile", script, true);
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入技術能力與技術關聯圖時發生錯誤");
        }
    }

    /// <summary>
    /// 從表單取得委員資料
    /// </summary>
    private List<OFS_SCI_Other_Recused> GetCommitteeDataFromForm()
    {
        var data = new List<OFS_SCI_Other_Recused>();

        // 取得所有委員相關的 input 欄位
        string[] names = HttpContext.Current.Request.Form.GetValues("committeeName");
        string[] units = HttpContext.Current.Request.Form.GetValues("committeeUnit");
        string[] positions = HttpContext.Current.Request.Form.GetValues("committeePosition");
        string[] reasons = HttpContext.Current.Request.Form.GetValues("committeeReason");

        if (names != null)
        {
            for (int i = 0; i < names.Length; i++)
            {
                // 只處理有資料的行
                if (!string.IsNullOrWhiteSpace(names[i]) ||
                    !string.IsNullOrWhiteSpace(units?[i]) ||
                    !string.IsNullOrWhiteSpace(positions?[i]) ||
                    !string.IsNullOrWhiteSpace(reasons?[i]))
                {
                    data.Add(new OFS_SCI_Other_Recused
                    {
                        ProjectID = ProjectID ?? "",
                        RecusedName = names[i]?.Trim() ?? "",
                        EmploymentUnit = units?[i]?.Trim() ?? "",
                        JobTitle = positions?[i]?.Trim() ?? "",
                        RecusedReason = reasons?[i]?.Trim() ?? ""
                    });
                }
            }
        }

        return data;
    }

    /// <summary>
    /// 從表單取得技術資料
    /// </summary>
    private List<OFS_SCI_Other_TechReadiness> GetTechDataFromForm()
    {
        var data = new List<OFS_SCI_Other_TechReadiness>();

        // 取得所有技術相關的 input 欄位
        string[] techItems = HttpContext.Current.Request.Form.GetValues("techItem");
        string[] trlPlanLevels = HttpContext.Current.Request.Form.GetValues("trlPlanLevel");
        string[] trlTrackLevels = HttpContext.Current.Request.Form.GetValues("trlTrackLevel");
        string[] techProcesses = HttpContext.Current.Request.Form.GetValues("techProcess");

        if (techItems != null)
        {
            for (int i = 0; i < techItems.Length; i++)
            {
                // 只處理有資料的行
                if (!string.IsNullOrWhiteSpace(techItems[i]) ||
                    !string.IsNullOrWhiteSpace(trlPlanLevels?[i]) ||
                    !string.IsNullOrWhiteSpace(trlTrackLevels?[i]) ||
                    !string.IsNullOrWhiteSpace(techProcesses?[i]))
                {
                    data.Add(new OFS_SCI_Other_TechReadiness
                    {
                        ProjectID = ProjectID ?? "",
                        Name = techItems[i]?.Trim() ?? "",
                        Bef_TRLevel = trlPlanLevels?[i]?.Trim() ?? "",
                        Aft_TRLevel = trlTrackLevels?[i]?.Trim() ?? "",
                        Description = techProcesses?[i]?.Trim() ?? ""
                    });
                }
            }
        }

        return data;
    }


    /// <summary>
    /// 套用檢視模式
    /// </summary>
    private void ApplyViewMode()
    {
        if (IsViewMode)
        {
            string script = @"
            <script>
             $(document).ready(function () {
                // 只針對此 UserControl 內的元素進行鎖定
                // 找到 tab4 容器（委員迴避清單）
                var userControl = document.querySelector('#tab4');

                if (!userControl) {
                    console.warn('找不到 UserControl 容器: #tab4');
                    return;
                }

                // 禁用此 UserControl 內的所有表單元素
                var formElements = userControl.querySelectorAll('input, textarea, select, button');
                formElements.forEach(function(element) {
                    element.disabled = true;
                    element.readOnly = true;
                });

                // 將此 UserControl 內有 view-mode class 的元件加上 d-none class
                var viewModeElements = userControl.querySelectorAll('.view-mode');
                viewModeElements.forEach(function(element) {
                    element.classList.add('d-none');
                });

                // 特別處理一些可能動態生成的元素
                setTimeout(function() {
                    var dynamicElements = userControl.querySelectorAll('input, textarea, select, button');
                    dynamicElements.forEach(function(element) {
                        if (!element.disabled) {
                            element.disabled = true;
                            element.readOnly = true;
                        }
                    });

                    // 再次處理可能動態生成的 view-mode 元素
                    var dynamicViewModeElements = userControl.querySelectorAll('.view-mode');
                    dynamicViewModeElements.forEach(function(element) {
                        if (!element.classList.contains('d-none')) {
                            element.classList.add('d-none');
                        }
                    });
                }, 1000);

                // 處理表格欄位隱藏
                $('#tab4 #techTable, #tab4 #committeeTable').addClass('hide-col-last');
                });
            </script>";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "AddClassToTable", script);
        }
    }

    /// <summary>
    /// 儲存表單資料到 Session
    /// </summary>
    private void SaveFormDataToSession()
    {
        try
        {
            string sessionKey = $"SciRecusedList_FormData_{ProjectID}";

            // 取得表單資料
            var committeeData = GetCommitteeDataFromForm();
            var techData = GetTechDataFromForm();

            // 取得 checkbox 狀態
            bool isChkNoAvoidance = false;
            string checkboxValue = HttpContext.Current.Request.Form[chkNoAvoidance.UniqueID];
            if (!string.IsNullOrEmpty(checkboxValue))
            {
                isChkNoAvoidance = checkboxValue.ToLower() == "on" || checkboxValue.ToLower() == "true";
            }

            // 建立 Session 資料物件
            var sessionData = new
            {
                CommitteeData = committeeData,
                TechData = techData,
                IsChkNoAvoidance = isChkNoAvoidance,
                SaveTime = DateTime.Now
            };

            // 儲存到 Session
            Session[sessionKey] = sessionData;

            System.Diagnostics.Debug.WriteLine($"表單資料已儲存到 Session: {sessionKey}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"儲存表單資料到 Session 失敗: {ex.Message}");
        }
    }

    /// <summary>
    /// 從 Session 還原表單資料
    /// </summary>
    private void RestoreFormDataFromSession()
    {
        try
        {
            string sessionKey = $"SciRecusedList_FormData_{ProjectID}";

            if (Session[sessionKey] != null)
            {
                // 從 Session 取得資料
                dynamic sessionData = Session[sessionKey];

                // 將資料轉換為可序列化格式
                var committeeData = ((List<OFS_SCI_Other_Recused>)sessionData.CommitteeData)
                    .Select(x => new
                    {
                        committeeName = x.RecusedName,
                        committeeUnit = x.EmploymentUnit,
                        committeePosition = x.JobTitle,
                        committeeReason = x.RecusedReason
                    }).ToArray();

                var techData = ((List<OFS_SCI_Other_TechReadiness>)sessionData.TechData)
                    .Select(x => new
                    {
                        techItem = x.Name,
                        trlPlanLevel = x.Bef_TRLevel,
                        trlTrackLevel = x.Aft_TRLevel,
                        techProcess = x.Description
                    }).ToArray();

                bool isChkNoAvoidance = (bool)sessionData.IsChkNoAvoidance;

                // 設定 checkbox 狀態
                chkNoAvoidance.Checked = isChkNoAvoidance;

                // 將資料傳遞到前端
                var dataToSend = new
                {
                    recusedData = committeeData,
                    techData = techData,
                    isRecused = isChkNoAvoidance,
                    fromSession = true
                };

                var dataJson = new JavaScriptSerializer().Serialize(dataToSend);

                string script = $@"
                    window.existingData = {dataJson};

                    setTimeout(function() {{
                        if (typeof loadExistingData === 'function') {{
                            loadExistingData();
                            console.log('已從 Session 還原資料：', window.existingData);

                            // 如果 IsRecused 為 true，需要鎖定委員表格
                            if (window.existingData.isRecused && typeof clearAndLockCommitteeTable === 'function') {{
                                const $tbody = $('#committeeTableBody');
                                clearAndLockCommitteeTable($tbody);
                                console.log('已鎖定委員表格（從 Session）');
                            }}
                        }} else {{
                            console.log('loadExistingData 函數未找到');
                        }}
                    }}, 200);
                ";

                Page.ClientScript.RegisterStartupScript(this.GetType(), "RestoreFromSession", script, true);

                System.Diagnostics.Debug.WriteLine($"已從 Session 還原表單資料: {sessionKey}");
            }

            // 重新載入技術能力與技術關聯圖（從資料庫讀取）
            if (!string.IsNullOrEmpty(ProjectID))
            {
                LoadTechDiagramFileFromDatabase();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"從 Session 還原表單資料失敗: {ex.Message}");
        }
    }

    /// <summary>
    /// 從資料庫載入技術能力與技術關聯圖
    /// </summary>
    private void LoadTechDiagramFileFromDatabase()
    {
        try
        {
            var uploadFiles = OFS_SciWorkSchHelper.GetUploadFilesByProjectIDAndFileCode(ProjectID, "TechnologyDiagram");

            if (uploadFiles.Any())
            {
                var file = uploadFiles.First();
                string appRootPath = ConfigurationManager.AppSettings["AppRootPath"] ?? "";

                // 設定 ImageUrl
                if (!string.IsNullOrEmpty(appRootPath))
                {
                    file.TemplatePath = ResolveUrl($"~/{appRootPath}/{file.TemplatePath}");
                }
                else
                {
                    file.TemplatePath = ResolveUrl($"~/{file.TemplatePath}");
                }

                var script = $@"
                    setTimeout(function() {{
                        if (window.techDiagramManager) {{
                            window.techDiagramManager.loadTechDiagramFile('{file.TemplatePath}', '{file.FileName}');
                            console.log('已從資料庫重新載入技術能力與技術關聯圖');
                        }} else {{
                            setTimeout(function() {{
                                if (window.techDiagramManager) {{
                                    window.techDiagramManager.loadTechDiagramFile('{file.TemplatePath}', '{file.FileName}');
                                    console.log('已從資料庫重新載入技術能力與技術關聯圖（延遲）');
                                }}
                            }}, 1000);
                        }}
                    }}, 100);
                ";

                Page.ClientScript.RegisterStartupScript(this.GetType(), "LoadTechDiagramFileFromDB", script, true);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"從資料庫載入技術能力與技術關聯圖時發生錯誤: {ex.Message}");
        }
    }

    /// <summary>
    /// 清除表單資料 Session
    /// </summary>
    private void ClearFormDataSession()
    {
        try
        {
            string sessionKey = $"SciRecusedList_FormData_{ProjectID}";

            if (Session[sessionKey] != null)
            {
                Session.Remove(sessionKey);
                System.Diagnostics.Debug.WriteLine($"已清除 Session 資料: {sessionKey}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"清除 Session 資料失敗: {ex.Message}");
        }
    }

    /// <summary>
    /// 資料庫欄位名稱對應中文顯示名稱
    /// </summary>
    private static readonly System.Collections.Generic.Dictionary<string, string> _columnChineseNames =
        new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        // OFS_SCI_RecusedList
        { "RecusedName",    "迴避人員姓名" },
        { "EmploymentUnit", "任職機構" },
        { "JobTitle",       "職稱" },
        { "RecusedReason",  "迴避原因" },
        // OFS_SCI_Other_TechReadiness
        { "Name",           "名稱" },
        { "Bef_TRLevel",    "申請前TRL等級" },
        { "Aft_TRLevel",    "預期TRL等級" },
        { "Description",    "說明" },
    };

    /// <summary>
    /// 判斷是否為資料庫欄位長度超過上限的錯誤
    /// </summary>
    private bool IsDatabaseStringTruncationError(Exception ex)
    {
        return GetTruncatedColumnName(ex) != null;
    }

    /// <summary>
    /// 從例外訊息中解析被截斷的欄位名稱（含中文對應）
    /// 回傳欄位中文名稱；找到截斷錯誤但無法解析欄位時回傳 string.Empty；非截斷錯誤則回傳 null
    /// </summary>
    private string GetTruncatedColumnName(Exception ex)
    {
        Exception current = ex;
        while (current != null)
        {
            bool isTruncation =
                current.Message.IndexOf("String or binary data would be truncated", StringComparison.OrdinalIgnoreCase) >= 0 ||
                current.Message.IndexOf("字串或二進位資料將會截斷", StringComparison.OrdinalIgnoreCase) >= 0 ||
                current.Message.IndexOf("字串或二進位資料會被截斷", StringComparison.OrdinalIgnoreCase) >= 0;

            if (isTruncation)
            {
                var match = System.Text.RegularExpressions.Regex.Match(
                    current.Message,
                    @"(?:資料行|column)\s+'([^']+)'",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase
                );
                if (!match.Success) return string.Empty;

                string colName = match.Groups[1].Value;
                return _columnChineseNames.TryGetValue(colName, out string chineseName)
                    ? chineseName
                    : colName;
            }
            current = current.InnerException;
        }
        return null;
    }

    /// <summary>
    /// 處理例外錯誤
    /// </summary>
    private void HandleException(Exception ex, string context)
    {
        // 記錄錯誤
        System.Diagnostics.Debug.WriteLine($"{context}: {ex.Message}");

        // 可以在這裡加入更多錯誤處理邏輯，如記錄到日誌
    }

    #endregion

    #region 按鈕事件

    protected void btnSave_Click(object sender, EventArgs e)
    {
        string ProjectID = Request.QueryString["ProjectID"] ?? "";
        try
        {
            // 先儲存表單資料到 Session
            SaveFormDataToSession();

            // 儲存 UserControl 資料
            if (SaveData(ProjectID))
            {
                // 儲存變更說明
                tab4_ucChangeDescription.SaveChangeDescription(ProjectID);


                // 更新版本狀態（暫存）
                UpdateVersionStatusBasedOnAction(ProjectID, false);
                
                RestoreFormDataFromSession();
                // 清除 Session（儲存成功後）
                ClearFormDataSession();

                // 顯示成功訊息
                ShowSuccessMessage("儲存成功！");            }
        }
        catch (Exception ex)
        {
            // 錯誤處理（保留 Session 資料以便還原）
            string truncatedColumn = GetTruncatedColumnName(ex);
            if (truncatedColumn != null)
            {
                string columnHint = string.IsNullOrEmpty(truncatedColumn) ? "某欄位" : $"欄位 '{truncatedColumn}'";
                ShowErrorMessage($"{columnHint} 內容長度超過資料庫上限，請縮短輸入內容後重新儲存");
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('儲存失敗：{ex.Message}');", true);
            }
        }
    }

    protected void btnNext_Click(object sender, EventArgs e)
    {
        string ProjectID = Request.QueryString["ProjectID"] ?? "";
        try
        {
            // 先儲存表單資料到 Session
            SaveFormDataToSession();

            // 驗證 UserControl 資料
            var validationResult = ValidateForm();
            if (!validationResult.IsValid)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ValidationError",
                    $"alert('請修正以下錯誤：\\n{validationResult.GetErrorsAsString()}');", true);
                RestoreFormDataFromSession();
                return;
            }

            // 儲存 UserControl 資料
            if (SaveData(ProjectID))
            {
                // 儲存變更說明
                tab4_ucChangeDescription.SaveChangeDescription(ProjectID);

                // 更新版本狀態（完成）
                UpdateVersionStatusBasedOnAction(ProjectID, true);
                
                //postback 還原資料
                RestoreFormDataFromSession();
                // 清除 Session（儲存成功後）
                ClearFormDataSession();

                // 判斷當前頁面是否為 SciInprogress_Approved.aspx
                string currentPage = System.IO.Path.GetFileName(Request.Url.AbsolutePath);
                string redirectUrl = currentPage != "SciInprogress_Approved.aspx"
                    ? $"SciUploadAttachments.aspx?ProjectID={ProjectID}"
                    : "";

                // 顯示成功訊息（如果有 URL 則 1 秒後跳轉）
                ShowSuccessMessage("儲存成功！", redirectUrl);
            }
        }
        catch (Exception ex)
        {
            // 錯誤處理（保留 Session 資料以便還原）
            string truncatedColumn = GetTruncatedColumnName(ex);
            if (truncatedColumn != null)
            {
                string columnHint = string.IsNullOrEmpty(truncatedColumn) ? "某欄位" : $"欄位 '{truncatedColumn}'";
                ShowErrorMessage($"{columnHint} 內容長度超過資料庫上限，請縮短輸入內容後重新儲存");
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('儲存失敗：{ex.Message}');", true);
            }
        }
    }



    /// <summary>
    /// 根據動作類型更新版本狀態
    /// </summary>
    /// <param name="ProjectID">ProjectID</param>
    /// <param name="isComplete">是否為完成動作（下一步）</param>
    private void UpdateVersionStatusBasedOnAction(string ProjectID, bool isComplete)
    {
        try
        {
            if (isComplete)
            {
                // 點擊「完成本頁，下一步」按鈕
                // 1. Form4Status 設為 "完成" 
                // 2. 檢查 CurrentStep，如果 <= 4 則改成 5
                
                string currentStep = OFS_SciWorkSchHelper.GetCurrentStepByProjectID(ProjectID);
                int currentStepNum = 1;
                int.TryParse(currentStep, out currentStepNum);
                
                bool shouldUpdateCurrentStep = currentStepNum <= 4;
                string newCurrentStep = shouldUpdateCurrentStep ? "5" : currentStep;
                
                // 更新 Form4Status 為 "完成" 和 CurrentStep (如果需要)
                if (shouldUpdateCurrentStep)
                {
                    OFS_SciRecusedList.UpdateForm4StatusAndCurrentStep(ProjectID, "完成", newCurrentStep);
                }
                else
                {
                    OFS_SciRecusedList.UpdateForm4Status(ProjectID, "完成");
                }
            }
            else
            {
                // 點擊「暫存」按鈕
                // 只更新 Form5Status 為 "暫存"，CurrentStep 不變
                
                OFS_SciRecusedList.UpdateForm4Status(ProjectID, "暫存");
            }
        }
        catch (Exception ex)
        {
            
            // 記錄錯誤但不中斷流程
            System.Diagnostics.Debug.WriteLine($"更新版本狀態失敗: {ex.Message}");
        }
    }

    /// <summary>
    /// 檢查表單狀態並控制暫存按鈕顯示
    /// </summary>
    private void CheckFormStatusAndHideTempSaveButton()
    {
        try
        {
            string ProjectID = Request.QueryString["ProjectID"] ?? "";
            if (!string.IsNullOrEmpty(ProjectID))
            {
                    var formStatus = OFS_SciWorkSchHelper.GetFormStatusByProjectID(ProjectID, "Form4Status");

                    if (formStatus == "完成")
                    {
                        // 隱藏暫存按鈕
                        tab4_btnTempSave.Style["display"] = "none";
                    }

            }
        }
        catch (Exception ex)
        {
            // 發生錯誤時不隱藏按鈕，讓用戶正常使用
            System.Diagnostics.Debug.WriteLine($"檢查表單狀態失敗: {ex.Message}");
        }
    }

    /// <summary>
    /// 取得此 UserControl 對應的變更說明資料
    /// </summary>
    /// <returns>變更說明資料 (changeBefore, changeAfter)</returns>
    public (string changeBefore, string changeAfter) GetChangeDescriptionData()
    {
        try
        {
            if (!string.IsNullOrEmpty(ProjectID))
            {
                return tab4_ucChangeDescription.GetChangeDescriptionBySourcePage(ProjectID, "SciRecusedList");
            }
            return ("", "");
        }
        catch (Exception ex)
        {
            HandleException(ex, "取得變更說明資料時發生錯誤");
            return ("", "");
        }
    }

    #endregion

    /// <summary>
    /// 顯示成功訊息
    /// </summary>
    /// <param name="message">訊息內容</param>
    /// <param name="redirectUrl">跳轉網址，如果為空則不跳轉</param>
    private void ShowSuccessMessage(string message, string redirectUrl = "")
    {
        string safeMessage = System.Web.HttpUtility.JavaScriptStringEncode(message);

        string script;

        if (!string.IsNullOrEmpty(redirectUrl))
        {
            // 有 URL：顯示 1 秒後自動跳轉
            string safeUrl = System.Web.HttpUtility.JavaScriptStringEncode(redirectUrl);
            script = $@"
                Swal.fire({{
                    title: '成功',
                    text: '{safeMessage}',
                    icon: 'success',
                    timer: 1000,
                    showConfirmButton: false,
                    customClass: {{
                        popup: 'animated fadeInDown'
                    }}
                }}).then(function() {{
                    window.location.href = '{safeUrl}';
                }});
            ";
        }
        else
        {
            // 沒有 URL：正常顯示訊息
            script = $@"
                Swal.fire({{
                    title: '成功',
                    text: '{safeMessage}',
                    icon: 'success',
                    confirmButtonText: '確定',
                    customClass: {{
                        popup: 'animated fadeInDown'
                    }}
                }});
            ";
        }

        Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowSuccessMessage" + Guid.NewGuid().ToString(), script, true);
    }

    /// <summary>
    /// 顯示錯誤訊息
    /// </summary>
    private void ShowErrorMessage(string message, string callback = "")
    {
        string safeMessage = System.Web.HttpUtility.JavaScriptStringEncode(message.Replace("\r\n", "<br>"));

        string script = $@"
            Swal.fire({{
                title: '錯誤',
                html: '{safeMessage}',
                icon: 'error',
                confirmButtonText: '確定',
                customClass: {{
                    popup: 'animated fadeInDown'
                }}
            }})";

        if (!string.IsNullOrEmpty(callback))
        {
            script += $".then(function() {{ {callback} }})";
        }

        script += ";";

        Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowErrorMessage" + Guid.NewGuid().ToString(), script, true);
    }

    /// <summary>
    /// 顯示警告訊息
    /// </summary>
    private void ShowWarningMessage(string message, string callback = "")
    {
        string safeMessage = System.Web.HttpUtility.JavaScriptStringEncode(message);

        string script = $@"
            Swal.fire({{
                title: '警告',
                text: '{safeMessage}',
                icon: 'warning',
                confirmButtonText: '確定',
                customClass: {{
                    popup: 'animated fadeInDown'
                }}
            }})";

        if (!string.IsNullOrEmpty(callback))
        {
            script += $".then(function() {{ {callback} }})";
        }

        script += ";";

        Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowWarningMessage" + Guid.NewGuid().ToString(), script, true);
    }

}