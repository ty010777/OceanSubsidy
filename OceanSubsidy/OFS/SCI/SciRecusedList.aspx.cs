using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Entity.Base;
using GS.OCA_OceanSubsidy.Operation.OFS;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.X509.SigI;
using Newtonsoft.Json;

public partial class OFS_SciAvoidList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // 設定顯示模式
            SetDisplayMode();
            
            if (!IsPostBack)
            {
                // 初始化頁面資料
                LoadData();
                
                // 檢查表單狀態並控制暫存按鈕顯示
                CheckFormStatusAndHideTempSaveButton();
            }
        }
        catch (Exception ex)
        {
            // 發生錯誤時記錄但不中斷頁面載入
            System.Diagnostics.Debug.WriteLine($"頁面載入錯誤：{ex.Message}");
        }
    }

    private void LoadData()
    {
        string ProjectID = Request.QueryString["ProjectID"] ?? "";

        if (!string.IsNullOrEmpty(ProjectID))
        {
           
            LoadExistingData(ProjectID);
        }
        
        // 載入 TRL 選項
        LoadTrlOptions();
        
        // 載入技術能力與技術關聯圖
        LoadTechDiagramFile();
    }
    
    private void LoadExistingData(string ProjectID)
    {
        try
        {
            // 載入委員迴避清單資料

            
            var recusedList = OFS_SciRecusedList.GetRecusedListByProjectID(ProjectID);
            var recusedData = recusedList.Select(x => new {
                committeeName = x.RecusedName,
                committeeUnit = x.EmploymentUnit,
                committeePosition = x.JobTitle,
                committeeReason = x.RecusedReason
            }).ToArray();
            
            // 載入技術成熟度資料
            var techList = OFS_SciRecusedList.GetTechReadinessListByProjectID(ProjectID);
            var techData = techList.Select(x => new {
                techItem = x.Name,
                trlPlanLevel = x.Bef_TRLevel,
                trlTrackLevel = x.Aft_TRLevel,
                techProcess = x.Description
            }).ToArray();
            
            // 將資料傳遞到前端
            var dataToSend = new {
                recusedData = recusedData,
                techData = techData
            };
            
            var dataJson = new JavaScriptSerializer().Serialize(dataToSend);
            ClientScript.RegisterStartupScript(this.GetType(), "existingData", 
                $"window.existingData = {dataJson};", true);
        }
        catch (Exception ex)
        {
            // 如果載入失敗，設定空資料
            ClientScript.RegisterStartupScript(this.GetType(), "existingData", 
                "window.existingData = { recusedData: [], techData: [] };", true);
        }
    }
    
    private void LoadTrlOptions()
    {
        try
        {
            var trlOptions = OFS_SciRecusedList.GetSysZgsCodeByCodeGroup("SCI_TRL");
            var optionsJson = new JavaScriptSerializer().Serialize(trlOptions.Select(x => new { 
                Code = x.Code, 
                Descname = x.Descname 
            }));
            
            ClientScript.RegisterStartupScript(this.GetType(), "trlOptions", 
                $"window.trlOptions = {optionsJson};", true);
        }
        catch (Exception ex)
        {
            // 如果載入失敗，使用預設選項
            ClientScript.RegisterStartupScript(this.GetType(), "trlOptions", 
                "window.trlOptions = [];", true);
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            // 取得 Checkbox 狀態
            bool noAvoidanceCommittee = chkNoAvoidance.Checked;

            string ProjectID = Request.QueryString["ProjectID"] ?? "";
           
            // 處理技術能力與技術關聯圖上傳（如果有的話）
            ProcessTechDiagramUpload(ProjectID);
            
            // // 驗證資料
            // if (!ValidateData(committeeData, techData, noAvoidanceCommittee, noAvoidanceTech))
            // {
            //     return;
            // }
            // 取得委員迴避清單資料
            var committeeData = GetCommitteeData(ProjectID);
            // 取得技術能力資料
            var techData = GetTechData(ProjectID);
            // TODO: 儲存到資料庫
            // 取得目前的申請案 ID

            OFS_SciRecusedList.ReplaceRecusedList(committeeData, ProjectID);
            OFS_SciRecusedList.ReplaceTechReadinessList(techData, ProjectID);

            // 更新版本狀態（暫存）
            UpdateVersionStatusBasedOnAction(ProjectID, false);

            // 重新載入資料以更新預覽
            LoadData();

            // 顯示成功訊息
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('儲存成功！');", true);
        }
        catch (Exception ex)
        {
            // 錯誤處理
            ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('儲存失敗：{ex.Message}');", true);
        }
    }

    protected void btnNext_Click(object sender, EventArgs e)
    {
        try
        {
            // 取得 Checkbox 狀態
            bool noAvoidanceCommittee = chkNoAvoidance.Checked;

            string ProjectID = Request.QueryString["ProjectID"] ?? "";

            // 處理技術能力與技術關聯圖上傳（如果有的話）
            ProcessTechDiagramUpload(ProjectID);
            
            // 取得委員迴避清單資料
            var committeeData = GetCommitteeData(ProjectID);
            // 取得技術能力資料
            var techData = GetTechData(ProjectID);

            // 儲存資料
            OFS_SciRecusedList.ReplaceRecusedList(committeeData, ProjectID);
            OFS_SciRecusedList.ReplaceTechReadinessList(techData, ProjectID);

            // 更新版本狀態（完成）
            UpdateVersionStatusBasedOnAction(ProjectID, true);

            // 顯示成功訊息（未來可能會導向下一頁）
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('儲存成功！本頁已完成。');", true);
            
            // 重新載入資料以更新預覽
            LoadData();
        }
        catch (Exception ex)
        {
            // 錯誤處理
            ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('儲存失敗：{ex.Message}');", true);
        }
    }

    private List<OFS_SCI_Other_Recused> GetCommitteeData(string projectID)
    {
        var data = new List<OFS_SCI_Other_Recused>();
        
        // 取得所有委員相關的 input 欄位
        string[] names = Request.Form.GetValues("committeeName");
        string[] units = Request.Form.GetValues("committeeUnit");
        string[] positions = Request.Form.GetValues("committeePosition");
        string[] reasons = Request.Form.GetValues("committeeReason");

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
                        ProjectID = projectID ?? "",
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

    private List<OFS_SCI_Other_TechReadiness> GetTechData(string projectID)
    {
        var data = new List<OFS_SCI_Other_TechReadiness>();
        
        // 取得所有技術相關的 input 欄位
        string[] techItems = Request.Form.GetValues("techItem");
        string[] trlPlanLevels = Request.Form.GetValues("trlPlanLevel");
        string[] trlTrackLevels = Request.Form.GetValues("trlTrackLevel");
        string[] techProcesses = Request.Form.GetValues("techProcess");

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
                        ProjectID= projectID ?? "",
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

    private bool ValidateData(List<OFS_SCI_Other_Recused> committeeData, List<OFS_SCI_Other_TechReadiness> techData, 
                             bool noAvoidanceCommittee)
    {
        // 如果沒有勾選「無需迴避」，則必須至少有一筆資料
        if (!noAvoidanceCommittee && committeeData.Count == 0)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", 
                "alert('請填寫委員迴避資料或勾選「無需迴避之審查委員」');", true);
            return false;
        }

        // 驗證必填欄位
        foreach (var item in committeeData)
        {
            if (string.IsNullOrWhiteSpace(item.RecusedName) || 
                string.IsNullOrWhiteSpace(item.EmploymentUnit) || 
                string.IsNullOrWhiteSpace(item.JobTitle) || 
                string.IsNullOrWhiteSpace(item.RecusedReason))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", 
                    "alert('委員迴避清單中有必填欄位未填寫');", true);
                return false;
            }
        }

        // 驗證技術資料
        // foreach (var item in techData)
        // {
        //     if (string.IsNullOrWhiteSpace(item.TechItem) || 
        //         string.IsNullOrWhiteSpace(item.TrlLevel) || 
        //         string.IsNullOrWhiteSpace(item.TechProcess))
        //     {
        //         ClientScript.RegisterStartupScript(this.GetType(), "alert", 
        //             "alert('技術能力資料中有必填欄位未填寫');", true);
        //         return false;
        //     }
        // }

        return true;
    }

    #region 技術能力與技術關聯圖處理

    /// <summary>
    /// 載入技術能力與技術關聯圖到前端
    /// </summary>
    private void LoadTechDiagramFile()
    {
        try
        {
            string ProjectID = Request.QueryString["ProjectID"] ?? "";
            if (string.IsNullOrEmpty(ProjectID)) return;

            var uploadFiles = OFS_SciWorkSchHelper.GetUploadFilesByProjectIDAndFileCode(ProjectID, "TechnologyDiagram");
            
            if (uploadFiles.Any())
            {
                var file = uploadFiles.First();
                var script = $@"
                    if (window.techDiagramManager) {{
                        window.techDiagramManager.loadTechDiagramFile('{file.TemplatePath}', '{file.FileName}');
                    }} else {{
                        // 如果 techDiagramManager 還沒初始化，延遲執行
                        setTimeout(function() {{
                            if (window.techDiagramManager) {{
                                window.techDiagramManager.loadTechDiagramFile('{file.TemplatePath}', '{file.FileName}');
                            }}
                        }}, 1000);
                    }}
                ";
                
                ClientScript.RegisterStartupScript(this.GetType(), "LoadTechDiagramFile", script, true);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"載入技術能力與技術關聯圖時發生錯誤: {ex.Message}");
        }
    }

    /// <summary>
    /// 處理技術能力與技術關聯圖上傳（在暫存時呼叫）
    /// </summary>
    private void ProcessTechDiagramUpload(string ProjectID)
    {
        try
        {
            // 檢查是否有檔案上傳
            var uploadedFile = Request.Files["fileUploadTechDiagram"];
            
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
                var uploadDir = Server.MapPath($"~/UploadFiles/OFS/SCI/{ProjectID}/");
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
                SaveTechDiagramRecord(ProjectID, fileName, relativePath);
               

            }
        }
        catch (Exception ex)
        {
            // 記錄錯誤
            var errorScript = $"console.error('技術能力與技術關聯圖上傳失敗：', '{ex.Message.Replace("'", "\\'")}');";
            ClientScript.RegisterStartupScript(this.GetType(), "LogTechDiagramError", errorScript, true);
            
            // 不拋出例外，讓其他資料繼續儲存
            Console.WriteLine($"技術能力與技術關聯圖上傳失敗: {ex.Message}");
        }
    }

    /// <summary>
    /// 儲存技術能力與技術關聯圖記錄到資料庫
    /// </summary>
    private void SaveTechDiagramRecord(string ProjectID, string fileName, string relativePath)
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
                    // 從資料庫刪除記錄
                    OFS_SciWorkSchHelper.DeleteUploadFile(existingFile.ProjectID, existingFile.FileCode);
                }
            }
            
            // 建立新的上傳檔案記錄
            var uploadFile = new OFS_SCI_UploadFile
            {
                ProjectID = ProjectID,
                FileCode = "TechnologyDiagram",
                FileName = fileName,
                TemplatePath = relativePath,
                Statuses = "Active"
            };
            
            // 儲存新記錄到資料庫
            OFS_SciWorkSchHelper.InsertUploadFile(uploadFile);
        }
        catch (Exception ex)
        {
            // 記錄錯誤但不中斷流程
            var errorScript = $"console.error('技術能力與技術關聯圖記錄儲存失敗：', '{ex.Message.Replace("'", "\\'")}');";
            ClientScript.RegisterStartupScript(this.GetType(), "LogTechDiagramRecordError", errorScript, true);
            
            Console.WriteLine($"儲存技術能力與技術關聯圖記錄失敗：{ex.Message}");
        }
    }

    #endregion

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
                // 1. Form5Status 設為 "完成" 
                // 2. 檢查 CurrentStep，如果 <= 5 則改成 6
                
                string currentStep = OFS_SciWorkSchHelper.GetCurrentStepByProjectID(ProjectID);
                int currentStepNum = 1;
                int.TryParse(currentStep, out currentStepNum);
                
                bool shouldUpdateCurrentStep = currentStepNum <= 5;
                string newCurrentStep = shouldUpdateCurrentStep ? "6" : currentStep;
                
                // 更新 Form5Status 為 "完成" 和 CurrentStep (如果需要)
                if (shouldUpdateCurrentStep)
                {
                    OFS_SciRecusedList.UpdateForm5StatusAndCurrentStep(ProjectID, "完成", newCurrentStep);
                }
                else
                {
                    OFS_SciRecusedList.UpdateForm5Status(ProjectID, "完成");
                }
            }
            else
            {
                // 點擊「暫存」按鈕
                // 只更新 Form5Status 為 "暫存"，CurrentStep 不變
                
                OFS_SciRecusedList.UpdateForm5Status(ProjectID, "暫存");
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
                    var formStatus = OFS_SciWorkSchHelper.GetFormStatusByProjectID(ProjectID, "Form5Status");
                    
                    if (formStatus == "完成")
                    {
                        // 隱藏暫存按鈕
                        btnTempSave.Style["display"] = "none";
                    }
                
            }
        }
        catch (Exception ex)
        {
            // 發生錯誤時不隱藏按鈕，讓用戶正常使用
            System.Diagnostics.Debug.WriteLine($"檢查表單狀態失敗: {ex.Message}");
        }
    }
    
    #region 顯示模式控制
    
    /// <summary>
    /// 設定顯示模式
    /// </summary>
    private void SetDisplayMode()
    {
        var master = (OFSApplicationMaster)this.Master;
        
        try
        {
            // 根據申請狀態決定模式
            if (ShouldShowInEditMode())
            {
                master.SetModeTo("編輯");
            }
            else
            {
                master.SetModeTo("檢視");
            }
        }
        catch (Exception ex)
        {
            // 發生錯誤時預設為檢視模式（安全考量）
            master.SetModeTo("檢視");
            System.Diagnostics.Debug.WriteLine($"設定顯示模式時發生錯誤：{ex.Message}");
        }
    }
    
    /// <summary>
    /// 判斷是否應該顯示為編輯模式
    /// </summary>
    /// <returns>true: 編輯模式, false: 檢視模式</returns>
    private bool ShouldShowInEditMode()
    {
        string ProjectID = Request.QueryString["ProjectID"];
        
        // 如果沒有 ProjectID，是新申請案件，可以編輯
        if (string.IsNullOrEmpty(ProjectID))
        {
            return true;
        }
        
        try
        {
            // 取得最新版本的狀態
            var ApplicationData = OFS_SciApplicationHelper.getVersionByProjectID(ProjectID);
            if (ApplicationData == null)
            {
                return true; // 沒有資料時允許編輯
            }
            
            // 只有這兩種狀態可以編輯
            string statuses = ApplicationData.Statuses ?? "";
            string statusesName = ApplicationData.StatusesName ?? "";
            
            return statuses == "尚未提送" || statusesName == "補正補件";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得申請狀態時發生錯誤：{ex.Message}");
            return false; // 發生錯誤時預設為檢視模式
        }
    }
    
    #endregion
}