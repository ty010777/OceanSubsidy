using System;
using System.Collections.Generic;
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
    public string ProjectID { get; set; }

    /// <summary>
    /// 是否為檢視模式
    /// </summary>
    public bool IsViewMode { get; set; } = false;
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
            }
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
    public void LoadData(string projectID, bool isViewMode = false)
    {
        try
        {
            this.ProjectID = projectID;
            this.IsViewMode = isViewMode;

            if (!string.IsNullOrEmpty(projectID))
            {
                LoadExistingData(projectID);
                LoadTechDiagramFile(projectID);
            }


            // 套用檢視模式
            if (isViewMode)
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
    /// 驗證表單資料
    /// </summary>
    /// <returns>驗證結果</returns>
    public ValidationResult ValidateForm()
    {
        var result = new ValidationResult();

        try
        {
            // 取得表單資料
            var committeeData = GetCommitteeDataFromForm();
            bool noAvoidanceCommittee = chkNoAvoidance.Checked;

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

            // 驗證技術能力資料
            var techData = GetTechDataFromForm();
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
        this.ProjectID = projectID;
        try
        {
            // 處理檔案上傳
            ProcessTechDiagramUpload();

            // 取得表單資料
            var committeeData = GetCommitteeDataFromForm();
            var techData = GetTechDataFromForm();
            bool isChkNoAvoidance = chkNoAvoidance.Checked;

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
            
            // 設定 checkbox 狀態
            chkNoAvoidance.Checked = isRecused;

            // 將資料傳遞到前端
            var dataToSend = new
            {
                recusedData = recusedData,
                techData = techData,
                isRecused = isRecused
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
                    }} else {{
                        console.log('loadExistingData 函數未找到');
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
                bool isProd = Request.Url.Host.Equals("projects.geosense", StringComparison.OrdinalIgnoreCase);

                // 設定 ImageUrl
                if (isProd)
                {
                    file.TemplatePath = ResolveUrl($"~/OceanSubsidy/{file.TemplatePath}");
                }
                else
                {
                    file.TemplatePath = ResolveUrl($"~/{file.TemplatePath}");
                }
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
    /// 處理技術能力與技術關聯圖上傳
    /// </summary>
    private void ProcessTechDiagramUpload()
    {
        try
        {
            // 檢查是否有檔案上傳
            var uploadedFile = HttpContext.Current.Request.Files["fileUploadTechDiagram"];

            if (uploadedFile != null && uploadedFile.ContentLength > 0)
            {
                // 驗證檔案類型
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png" };
                if (!allowedTypes.Contains(uploadedFile.ContentType.ToLower()))
                {
                    throw new Exception("只支援JPG和PNG格式的圖片");
                }

                // 驗證檔案大小（10MB）
                if (uploadedFile.ContentLength > 10 * 1024 * 1024)
                {
                    throw new Exception("檔案大小不能超過10MB");
                }

                // 建立檔案名稱
                var fileExtension = Path.GetExtension(uploadedFile.FileName).ToLower();
                var fileName = $"TechnologyDiagram{fileExtension}";

                // 建立完整檔案路徑（加上 ProjectID 資料夾）
                var uploadDir = HttpContext.Current.Server.MapPath($"~/UploadFiles/OFS/SCI/{ProjectID}/");
                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                var fullFilePath = Path.Combine(uploadDir, fileName);

                // 如果檔案已存在，先刪除舊檔案
                if (File.Exists(fullFilePath))
                {
                    File.Delete(fullFilePath);
                }

                // 儲存檔案
                uploadedFile.SaveAs(fullFilePath);

                // 相對路徑（用於資料庫儲存和顯示）
                var relativePath = $"UploadFiles/OFS/SCI/{ProjectID}/{fileName}";

                // 儲存到資料庫
                SaveTechDiagramRecord(fileName, relativePath);
            }
        }
        catch (Exception ex)
        {
            // 記錄錯誤但不中斷流程
            Page.ClientScript.RegisterStartupScript(this.GetType(), "TechDiagramUploadError",
                $"console.error('技術能力與技術關聯圖上傳失敗：{ex.Message.Replace("'", "\\'")}');", true);
        }
    }

    /// <summary>
    /// 儲存技術能力與技術關聯圖記錄到資料庫
    /// </summary>
    private void SaveTechDiagramRecord(string fileName, string relativePath)
    {
        try
        {
            // 先檢查是否已有該版本的技術能力與技術關聯圖記錄
            var existingFiles = OFS_SciWorkSchHelper.GetUploadFilesByProjectIDAndFileCode(ProjectID, "TechnologyDiagram");

            // 如果有舊記錄，先刪除
            if (existingFiles != null && existingFiles.Any())
            {
                foreach (var existingFile in existingFiles)
                {
                    OFS_SciWorkSchHelper.DeleteUploadFile(existingFile.ProjectID, existingFile.FileCode);
                }
            }

            // 建立新的上傳檔案記錄
            var uploadFile = new OFS_SCI_UploadFile
            {
                ProjectID = ProjectID,
                FileCode = "TechnologyDiagram",
                FileName = fileName,
                TemplatePath = relativePath
            };

            // 儲存新記錄到資料庫
            OFS_SciWorkSchHelper.InsertUploadFile(uploadFile);
        }
        catch (Exception ex)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "TechDiagramRecordError",
                $"console.error('技術能力與技術關聯圖記錄儲存失敗：{ex.Message.Replace("'", "\\'")}');", true);
        }
    }

    /// <summary>
    /// 套用檢視模式
    /// </summary>
    private void ApplyViewMode()
    {
        if (IsViewMode)
        {
            // 停用所有輸入控制項
            string script = @"
                setTimeout(function() {
                    // 停用所有輸入欄位
                    $('#committeeTable input, #techTable input, #techTable select, #techTable textarea').prop('disabled', true);
                    
                    // 停用檔案上傳
                    $('#fileUploadTechDiagram, #btnUploadTechDiagram, #btnDeleteTechDiagram').prop('disabled', true);
                    
                    // 停用新增和刪除按鈕
                    $('.add-row, .delete-row').prop('disabled', true).hide();
                    
                    // 停用 checkbox
                    $('#" + chkNoAvoidance.ClientID + @"').prop('disabled', true);
                }, 100);
            ";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "ApplyViewMode", script, true);
        }
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
}