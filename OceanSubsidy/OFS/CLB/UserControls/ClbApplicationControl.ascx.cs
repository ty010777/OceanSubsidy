using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using GS.App;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Model.OFS;
using GS.OCA_OceanSubsidy.Operation.OSI.OpenXml;
using Newtonsoft.Json;

public partial class OFS_CLB_UserControls_ClbApplicationControl : System.Web.UI.UserControl
{
    public string ProjectID { get; set; }
    public bool IsReadOnly { get; set; } = false;


    // 由於按鈕改為 HTML button，不再需要公開按鈕控制項


    protected void Page_Load(object sender, EventArgs e)
    {
        // 處理 AJAX 請求
        string action = Request.Form["action"];
        if (!string.IsNullOrEmpty(action))
        {
            HandleAjaxRequest(action);
            return;
        }

        if (!IsPostBack)
        {
            InitializeControl();

            // 根據專案狀態控制按鈕顯示和上傳附件步驟開放
            bool enableUploadStep = false;
            SetButtonVisibilityAndStepAccess(ProjectID, out enableUploadStep);
            // 初始化標籤系統 - 預設顯示申請表
            InitializeTabSystem(enableUploadStep, "application", IsReadOnly);
            // 檢查 URL 參數或屬性中的 ProjectID
            string projectIDToLoad = ProjectID;
            if (string.IsNullOrEmpty(projectIDToLoad))
            {
                projectIDToLoad = Page.Request.QueryString["ProjectID"];
            }


            if (!string.IsNullOrEmpty(projectIDToLoad))
            {
                try
                {
                    ProjectID = projectIDToLoad;
                    LoadBasicDataToForm(projectIDToLoad);

                    // 在 console 中輸出載入資訊，便於除錯
                    string script = $@"console.log('UserControl 載入計畫資料：{projectIDToLoad}');";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "LoadProjectInfo", script, true);
                }
                catch (Exception ex)
                {
                    // 錯誤處理
                    string script = $@"
                        Swal.fire({{
                            icon: 'error',
                            title: '載入資料失敗',
                            text: '{ex.Message}',
                            confirmButtonText: '確定'
                        }});";

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "LoadDataError", script, true);
                }
            }

            // 載入補助額度限制資料到前端
            LoadGrantLimitDataToFrontend();
        }
    }

    /// <summary>
    /// 處理 AJAX 請求
    /// </summary>
    /// <param name="action">動作類型</param>
    private void HandleAjaxRequest(string action)
    {
        Response.ContentType = "application/json";

        try
        {
            switch (action)
            {
                case "tempSave":
                    HandleTempSaveAjax();
                    break;
                case "saveAndNext":
                    HandleSaveAndNextAjax();
                    break;
                case "submitApplication":
                    HandleApplicationSubmission();
                    break;
                default:
                    Response.Write("{\"success\":false,\"message\":\"未知的動作類型\"}");
                    break;
            }
        }
        catch (Exception ex)
        {
            Response.Write($"{{\"success\":false,\"message\":\"處理失敗：{ex.Message}\"}}");
        }

        Response.End();
    }

    private void InitializeControl()
    {
        // 檢查是否為首次申請（沒有 ProjectID）
        string projectIDToLoad = ProjectID;
        if (string.IsNullOrEmpty(projectIDToLoad))
        {
            projectIDToLoad = Page.Request.QueryString["ProjectID"];
        }

        if (string.IsNullOrEmpty(projectIDToLoad))
        {
            // 首次申請：從 TypeID 取得年度和補助計畫類別
            LoadDefaultDataFromGrantType();
        }

        // 初始化變更說明控制項
        InitializeChangeDescriptionControl();

        // 如果是檢視模式，設定控制項狀態
        if (IsReadOnly)
        {
            SetReadOnlyMode();
        }
    }

    /// <summary>
    /// 首次申請：從 OFS_GrantType 載入預設資料
    /// </summary>
    private void LoadDefaultDataFromGrantType()
    {
        try
        {
            string typeId = Page.Request.QueryString["TypeID"];
            GrantTypeInfo grantTypeInfo = null;

            if (!string.IsNullOrEmpty(typeId))
            {
                // 從 OFS_GrantType 取得資料
                grantTypeInfo = OFSGrantTypeHelper.getByTypeID(typeId);
            }
            else
            {
                // 沒有 TypeID 時，以 TypeCode 和當前日期取得
                grantTypeInfo = OFSGrantTypeHelper.getByTypeCodeAndCurrentDate("CLB");
            }

            if (grantTypeInfo != null)
            {
                // 設定年度
                lblYear.Text = grantTypeInfo.Year?.ToString() ?? "";
                hidYear.Value = grantTypeInfo.Year?.ToString() ?? "";

                // 設定補助計畫類別
                lblSubsidyPlanType.Text = grantTypeInfo.FullName ?? "";
                hidSubsidyPlanType.Value = grantTypeInfo.FullName ?? "";
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"載入預設資料時發生錯誤：{ex.Message}");
            // 發生錯誤時留空白
        }
    }

    private void SetReadOnlyMode()
    {
        // 設定所有控制項為 disabled
        txtProjectNameTw.Enabled = false;
        rbSubsidyTypeCreate.Enabled = false;
        rbSubsidyTypeOperation.Enabled = false;
        rbSubsidyTypeActivity.Enabled = false;
        txtSchoolName.Enabled = false;
        txtSchoolIDNumber.Enabled = false;
        txtClubName.Enabled = false;
        txtAddress.Enabled = false;
        txtCreationDate.Enabled = false;
        txtStartDate.Enabled = false;
        txtEndDate.Enabled = false;
        txtPurpose.Enabled = false;
        txtPlanContent.Enabled = false;
        txtPreBenefits.Enabled = false;
        txtPlanLocation.Enabled = false;
        txtEstimatedPeople.Enabled = false;
        txtEmergencyPlan.Enabled = false;

        
        // 人員欄位
        txtTeacherName.Enabled = false;
        txtTeacherJobTitle.Enabled = false;
        txtTeacherPhone.Enabled = false;
        txtContactName.Enabled = false;
        txtContactJobTitle.Enabled = false;
        txtContactPhone.Enabled = false;

        // 變更說明欄位
        txtChangeBefore.Enabled = false;
        txtChangeAfter.Enabled = false;

        // 隱藏按鈕控制項（現在是 HTML button，使用 JavaScript 隱藏）

        // 使用 JavaScript 處理前端樣式
        string script = @"
            $(document).ready(function() {
                // 為 body 添加 app-mode-view class
                $('body').addClass('app-mode-view');

                // 為 FileTable 添加 hide-col-3 class
                $('#FileTable').addClass('hide-col-3');
                $('#OtherSubsidyTable, #BudgetPlanTable, #FundingDescriptionTable').addClass('hide-col-last');

                // 將 table 內的所有輸入框設為 disabled (包括 input, textarea, select, radio, checkbox)
                $('#FundingTable,#PreviouslySubsidizedTable, #OtherSubsidyTable, #BudgetPlanTable, #FundingDescriptionTable').find('input, textarea, select, button').prop('disabled', true);

                // 為所有有 view-mode class 的元素添加 d-none
                $('.view-mode').addClass('d-none');

                // 隱藏按鈕
                $('#btnTempSave, #btnSaveAndNext, #btnSubmitApplication').hide();
            });
        ";
        
        Page.ClientScript.RegisterStartupScript(this.GetType(), "SetViewModeStyles", script, true);
    }

    /// <summary>
    /// 儲存 Project_Main 資訊
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="formData">表單資料</param>
    /// <param name="isTempSave">是否為暫存</param>
    private void SaveProjectMainData(string projectID, Dictionary<string, object> formData, bool isTempSave)
    {
        try
        {
            // 取得使用者資訊
            var userInfo = SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo);
            
            var projectMainData = new OFS_CLB_Project_Main
            {
                ProjectID = projectID,
                Statuses = "尚未提送", // 暫存和提送都是"尚未提送"
                StatusesName = isTempSave ? "編輯中" : "編輯中", // 暫存和提送都是"編輯中"
                UserAccount = userInfo?.Account ?? "",
                UserName = userInfo?.UserName ?? "",
                UserOrg = userInfo?.UnitName ?? "",
                CurrentStep = isTempSave ? "1" :"2",// 暫存 是 1 和提送 後是2
                isWithdrawal = false,
                isExist = true,
                
            };
          
            // 檢查是否已存在
            bool isUpdate = OFS_ClbApplicationHelper.CheckProjectMainDataExists(projectMainData.ProjectID);
        
            if (isUpdate)
            {
                OFS_ClbApplicationHelper.UpdateProjectMainData(projectMainData);
            }
            else
            {
                OFS_ClbApplicationHelper.InsertProjectMainData(projectMainData);
            }
        
        }
        catch (Exception ex)
        {
            throw new Exception($"儲存 Project_Main 資訊失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 儲存變更說明記錄
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <param name="formData">表單資料</param>
    private void SaveProjectChangeRecord(string projectID, Dictionary<string, object> formData)
    {
        try
        {
            // 從 ASP.NET TextBox 控制項讀取資料
            string changeBefore = formData["txtChangeBefore"]?.ToString();
            string changeAfter = formData["txtChangeAfter"]?.ToString();


            // 只有當有變更說明時才儲存
            if (!string.IsNullOrEmpty(changeBefore) || !string.IsNullOrEmpty(changeAfter))
            {
                var ChangeRecord = OFSProjectChangeRecordHelper.getApplying("CLB", 1, projectID);
                ChangeRecord.Form1After = changeAfter;
                ChangeRecord.Form1Before = changeBefore;
                OFSProjectChangeRecordHelper.update(ChangeRecord);
            }

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"儲存變更說明記錄時發生錯誤: {ex.Message}");
            // 不中斷主要流程，只記錄錯誤
        }
    }

   

    public static string SaveBasicData(OFS_CLB_Application_Basic basicData)
    {
        try
        {
            // 如果 ProjectID 為空，則產生新的
            if (string.IsNullOrEmpty(basicData.ProjectID))
            {
                basicData.ProjectID = OFS_ClbApplicationHelper.GenerateNewProjectID(basicData.Year.Value);
                basicData.Serial = int.Parse(basicData.ProjectID.Substring(6)); // 取得序號部分
            }
            
            // 檢查是否已存在
            bool isUpdate = OFS_ClbApplicationHelper.CheckProjectIDExists(basicData.ProjectID);
            
            if (isUpdate)
            {
                OFS_ClbApplicationHelper.UpdateBasicData(basicData);
            }
            else
            {
                OFS_ClbApplicationHelper.InsertBasicData(basicData);
            }
            
            return basicData.ProjectID;
        }
        catch (Exception ex)
        {
            throw new Exception($"儲存基本資料失敗：{ex.Message}");
        }
    }
    /// <summary>
    /// 載入基本資料到表單
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    public void LoadBasicDataToForm(string projectID)
    {
        try
        {
            var basicData = OFS_ClbApplicationHelper.GetBasicData(projectID);
            if (basicData != null)
            {
                // 更新基本資料顯示
                lblProjectID.Text = basicData.ProjectID;
                hidProjectID.Value = basicData.ProjectID;
                txtProjectNameTw.Text = basicData.ProjectNameTw;

                // 載入年度和補助計畫類別
                lblYear.Text = basicData.Year?.ToString() ?? "";
                hidYear.Value = basicData.Year?.ToString() ?? "";
                lblSubsidyPlanType.Text = basicData.SubsidyPlanType ?? "";
                hidSubsidyPlanType.Value = basicData.SubsidyPlanType ?? "";

                // 設定申請補助類型
                switch (basicData.SubsidyType)
                {
                    case "Startup":
                        rbSubsidyTypeCreate.Checked = true;
                        break;
                    case "Admin":
                        rbSubsidyTypeOperation.Checked = true;
                        break;
                    case "Public":
                        rbSubsidyTypeActivity.Checked = true;
                        break;
                }
                
                txtSchoolName.Text = basicData.SchoolName;
                txtClubName.Text = basicData.ClubName;
                txtSchoolIDNumber.Text = basicData.School_IDNumber;
                txtAddress.Text = basicData.Address;
                
                // 設定成立日期 - 轉換為民國年顯示
                if (basicData.CreationDate.HasValue)
                {
                    txtCreationDate.Text = basicData.CreationDate.Value.ToMinguoDate();
                    // 設定 data attribute 供 JavaScript 使用
                    txtCreationDate.Attributes["data-gregorian-date"] = basicData.CreationDate.Value.ToString("yyyy/MM/dd");
                }
            }

            // 載入人員資料
            LoadPersonnelDataToForm(projectID);
            
            // 載入計畫資訊
            LoadPlanDataToForm(projectID);
            
            // 載入經費資訊
            LoadFundsDataToForm(projectID);
            
            // 載入 Project_Main 資訊
            LoadProjectMainDataToForm(projectID);
            
            // 載入檔案上傳狀態
            LoadFileStatus(projectID);
        }
        catch (Exception ex)
        {
            throw new Exception($"載入基本資料失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 載入人員資料到表單
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    private void LoadPersonnelDataToForm(string projectID)
    {
        try
        {
            var personnelList = OFS_ClbApplicationHelper.GetPersonnelData(projectID);
            
            foreach (var personnel in personnelList)
            {
                switch (personnel.Personnel)
                {
                    case "社團指導老師":
                        txtTeacherName.Text = personnel.Name;
                        txtTeacherJobTitle.Text = personnel.JobTitle;
                        txtTeacherPhone.Text = personnel.PhoneNum;
                        break;
                    case "社團業務聯絡人":
                        txtContactName.Text = personnel.Name;
                        txtContactJobTitle.Text = personnel.JobTitle;
                        txtContactPhone.Text = personnel.PhoneNum;
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"載入人員資料失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 載入計畫資訊到表單
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    private void LoadPlanDataToForm(string projectID)
    {
        try
        {
            var planData = OFS_ClbApplicationHelper.GetPlanData(projectID);
            
            if (planData != null)
            {
                // 設定執行期間 - 轉換為民國年顯示
                if (planData.StartDate.HasValue)
                {
                    txtStartDate.Text = planData.StartDate.Value.ToMinguoDate();
                    // 設定 data attribute 供 JavaScript 使用
                    txtStartDate.Attributes["data-gregorian-date"] = planData.StartDate.Value.ToString("yyyy/MM/dd");
                }
                if (planData.EndDate.HasValue)
                {
                    txtEndDate.Text = planData.EndDate.Value.ToMinguoDate();
                    // 設定 data attribute 供 JavaScript 使用
                    txtEndDate.Attributes["data-gregorian-date"] = planData.EndDate.Value.ToString("yyyy/MM/dd");
                }
                
                txtPurpose.Text = planData.Purpose;
                txtPlanContent.Text = planData.PlanContent;
                txtPreBenefits.Text = planData.PreBenefits;
                txtPlanLocation.Text = planData.PlanLocation;
                txtEstimatedPeople.Text = planData.EstimatedPeople;
                txtEmergencyPlan.Text = planData.EmergencyPlan;
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"載入計畫資訊失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 載入經費資訊到表單
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    private void LoadFundsDataToForm(string projectID)
    {
        try
        {
            // 1. 載入基本經費金額 (從 BasicData 取得)
            var basicData = OFS_ClbApplicationHelper.GetBasicData(projectID);
            if (basicData != null)
            {
                // 準備經費資料並通過 JavaScript 載入到前端
                string applyAmount = basicData.ApplyAmount?.ToString("N0") ?? "";
                string selfAmount = basicData.SelfAmount?.ToString("N0") ?? "";

                // 載入「最近兩年曾獲本會補助」的值
                bool isPreviouslySubsidized = basicData.IsPreviouslySubsidized ?? false;

                string script = $@"
                    if (typeof loadFundsData === 'function') {{
                        loadFundsData({{
                            applyAmount: '{applyAmount}',
                            selfAmount: '{selfAmount}',
                            isPreviouslySubsidized: {isPreviouslySubsidized.ToString().ToLower()}
                        }});
                    }}
                ";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "LoadFundsData", script, true);
            }

            // 2. 載入其他補助資料
            var otherSubsidyList = OFS_ClbOtherSubsidyHelper.GetByProjectID(projectID);
            if (otherSubsidyList != null && otherSubsidyList.Count > 0)
            {
                // 將其他補助資料轉換為 JSON 並傳遞給前端
                string otherSubsidyJson = JsonConvert.SerializeObject(otherSubsidyList.Select(s => new
                {
                    ID = s.ID,
                    Unit = s.Unit,
                    Amount = s.Amount,
                    Content = s.Content
                }).ToList());

                string script = $@"
                    if (typeof loadOtherSubsidyData === 'function') {{
                        loadOtherSubsidyData({otherSubsidyJson});
                    }}
                ";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "LoadOtherSubsidyData", script, true);
            }

            // 3. 載入經費預算規劃資料
            var budgetPlanList = OFS_ClbBudgetPlanHelper.GetByProjectID(projectID);
            if (budgetPlanList != null && budgetPlanList.Count > 0)
            {
                // 將經費預算規劃資料轉換為 JSON 並傳遞給前端
                string budgetPlanJson = JsonConvert.SerializeObject(budgetPlanList.Select(b => new
                {
                    ID = b.ID,
                    Title = b.Title,
                    Amount = b.Amount,
                    OtherAmount = b.OtherAmount,
                    Description = b.Description
                }).ToList());

                string script = $@"
                    if (typeof loadBudgetPlanData === 'function') {{
                        loadBudgetPlanData({budgetPlanJson});
                    }}
                ";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "LoadBudgetPlanData", script, true);
            }

            // 4. 載入經費說明資料 (已獲補助資料)
            var receivedSubsidyList = OFS_ClbReceivedSubsidyHelper.GetByProjectID(projectID);
            if (receivedSubsidyList != null && receivedSubsidyList.Count > 0)
            {
                // 將已獲補助資料轉換為 JSON 並傳遞給前端
                string receivedSubsidyJson = JsonConvert.SerializeObject(receivedSubsidyList.Select(r => new
                {
                    ID = r.ID,
                    ProjectName = r.Name,
                    Amount = r.Amount
                }).ToList());

                string script = $@"
                    if (typeof loadFundingDescriptionData === 'function') {{
                        loadFundingDescriptionData({receivedSubsidyJson});
                    }}
                ";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "LoadReceivedSubsidyData", script, true);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"載入經費資訊失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 載入 Project_Main 資訊到表單
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    private void LoadProjectMainDataToForm(string projectID)
    {
        try
        {
            var projectMainData = OFS_ClbApplicationHelper.GetProjectMainData(projectID);

            if (projectMainData != null)
            {
                // Project_Main 的資訊主要用於內部流程控制
                // 在申請表單頁面中不需要特別顯示這些資訊
                // 使用者資訊、狀態等都是由系統自動管理

                // 顯示申請送件時間（如果有的話）
                if (projectMainData.ApplyTime.HasValue)
                {
                    lblApplyTime.Text = projectMainData.ApplyTime.Value.ToMinguoDateTime();
                    divApplyTime.Visible = true;
                }
                else
                {
                    divApplyTime.Visible = false;
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"載入 Project_Main 資訊失敗：{ex.Message}");
        }
    }
    
    /// <summary>
    /// 更新檔案狀態UI（支援多檔）
    /// </summary>
    private void UpdateFileStatusUI(string fileCode, string fileName, string relativePath, int fileId, bool clearExisting = false)
    {
        Label statusLabel = null;
        Panel filesPanel = null;

        // 取得對應的控制項
        switch (fileCode)
        {
            case "FILE_CLB1":
                statusLabel = (Label)FindControl("lblStatusCLB1");
                filesPanel = (Panel)FindControl("pnlFilesCLB1");
                break;
            case "FILE_CLB2":
                statusLabel = (Label)FindControl("lblStatusCLB2");
                filesPanel = (Panel)FindControl("pnlFilesCLB2");
                break;
            case "FILE_CLB3":
                statusLabel = (Label)FindControl("lblStatusCLB3");
                filesPanel = (Panel)FindControl("pnlFilesCLB3");
                break;
            case "FILE_CLB4":
                statusLabel = (Label)FindControl("lblStatusCLB4");
                filesPanel = (Panel)FindControl("pnlFilesCLB4");
                break;
        }

        if (statusLabel != null && filesPanel != null)
        {
            // 更新狀態標籤
            statusLabel.Text = "已上傳";
            statusLabel.CssClass = "text-success";

            // 顯示檔案標籤
            filesPanel.Visible = true;

            // 只有在 clearExisting 為 true 或非 FILE_CLB4 時才清空（單檔模式）
            if (clearExisting || fileCode != "FILE_CLB4")
            {
                filesPanel.Controls.Clear();
            }

            // 建立主要的檔案標籤容器
            var fileTag = new System.Web.UI.HtmlControls.HtmlGenericControl("span");
            fileTag.Attributes["class"] = "tag tag-green-light";
            fileTag.Attributes["data-file-id"] = fileId.ToString();

            // 建立下載連結
            var downloadLink = new System.Web.UI.HtmlControls.HtmlGenericControl("a");
            downloadLink.Attributes["class"] = "tag-link";
            downloadLink.Attributes["href"] = "#";
            downloadLink.Attributes["onclick"] = $"downloadUploadedFileById({fileId}); return false;";
            downloadLink.InnerText = fileName;

            // 建立刪除按鈕
            var deleteButton = new System.Web.UI.HtmlControls.HtmlGenericControl("button");
            deleteButton.Attributes["type"] = "button";
            deleteButton.Attributes["class"] = "tag-btn";
            deleteButton.Attributes["onclick"] = $"deleteFileById({fileId}, '{fileCode}')";
            deleteButton.InnerHtml = "<i class=\"fa-solid fa-circle-xmark\"></i>";

            // 將元素加入到檔案標籤中
            fileTag.Controls.Add(downloadLink);
            fileTag.Controls.Add(deleteButton);

            filesPanel.Controls.Add(fileTag);
        }
    }

    /// <summary>
    /// 載入檔案狀態
    /// </summary>
    public void LoadFileStatus(string projectID)
    {
        try
        {
            if (string.IsNullOrEmpty(projectID)) return;

            var uploadedFiles = OFS_ClbApplicationHelper.GetUploadedFiles(projectID);

            // 按 FileCode 分組，以便判斷是否為第一筆（需要清空）
            var groupedFiles = uploadedFiles.GroupBy(f => f.FileCode);

            foreach (var group in groupedFiles)
            {
                bool isFirst = true;
                foreach (var file in group)
                {
                    // 第一筆清空面板，後續追加（支援 FILE_CLB4 多檔）
                    UpdateFileStatusUI(file.FileCode, file.FileName, file.TemplatePath, file.ID, isFirst);
                    isFirst = false;
                }
            }
        }
        catch (Exception ex)
        {
            // 記錄錯誤但不影響頁面載入
            System.Diagnostics.Debug.WriteLine($"載入檔案狀態失敗：{ex.Message}");
        }
    }
    

    #region AJAX 處理方法

    /// <summary>
    /// 處理暫存 AJAX 請求
    /// </summary>
    private void HandleTempSaveAjax()
    {
        string projectID = "";
        try
        {
            // 記錄開始處理暫存請求
            Env.Log.Info("===== 開始處理暫存請求 =====");

            // 取得表單資料
            var formData = GetFormDataFromRequest();
            projectID = formData.ContainsKey("ProjectID") ? formData["ProjectID"]?.ToString() : "";

            // 驗證字數是否超過限制
            List<string> charCountErrors = ValidateCharCount(formData);
            if (charCountErrors.Count > 0)
            {
                string errorMessage = string.Join("<br>", charCountErrors);
                Env.Log.Warn($"暫存失敗 - ProjectID: {projectID}, 原因: 字數超過限制");
                Response.Write($"{{\"success\":false,\"message\":\"{errorMessage}\"}}");
                return;
            }

            // 記錄關鍵欄位資料
            string projectName = formData.ContainsKey("ProjectNameTw") && formData["ProjectNameTw"] != null ? formData["ProjectNameTw"].ToString() : "";
            Env.Log.Info($"暫存請求 - ProjectID: {projectID}, 計畫名稱: {projectName}, 使用者: {GetCurrentUserAccount()}");

            // 執行暫存（傳入 true 表示暫存）
            string savedProjectID = SaveBasicDataAjax(formData, true);

            if (!string.IsNullOrEmpty(savedProjectID))
            {
                Env.Log.Info($"暫存成功 - ProjectID: {savedProjectID}");
                Response.Write($"{{\"success\":true,\"message\":\"暫存成功！\",\"projectID\":\"{savedProjectID}\"}}");
            }
            else
            {
                Env.Log.Warn($"暫存失敗 - ProjectID: {projectID}, 原因: SaveBasicDataAjax 回傳空值");
                Response.Write("{\"success\":false,\"message\":\"儲存失敗，請檢查必填欄位\"}");
            }
        }
        catch (Exception ex)
        {
            // 記錄完整的錯誤資訊
            Env.Log.Error($"暫存失敗 - ProjectID: {projectID}, 使用者: {GetCurrentUserAccount()}", ex);
            Env.Log.Error($"錯誤詳細資訊 - Message: {ex.Message}");
            Env.Log.Error($"錯誤堆疊追蹤: {ex.StackTrace}");

            // 如果有內部例外，也記錄下來
            if (ex.InnerException != null)
            {
                Env.Log.Error($"內部例外: {ex.InnerException.Message}");
                Env.Log.Error($"內部例外堆疊追蹤: {ex.InnerException.StackTrace}");
            }

            Response.Write($"{{\"success\":false,\"message\":\"暫存失敗：{ex.Message.Replace("\"", "\\\"").Replace("\r", "").Replace("\n", "")}\"}}");
        }
    }

    /// <summary>
    /// 處理儲存並下一步 AJAX 請求
    /// </summary>
    private void HandleSaveAndNextAjax()
    {
        try
        {
            // 取得表單資料
            var formData = GetFormDataFromRequest();

            // 驗證字數是否超過限制
            List<string> charCountErrors = ValidateCharCount(formData);
            if (charCountErrors.Count > 0)
            {
                string errorMessage = string.Join("<br>", charCountErrors);
                Response.Write($"{{\"success\":false,\"message\":\"{errorMessage}\"}}");
                return;
            }

            // 驗證必填欄位
            List<string> validationErrors = ValidateRequiredFields(formData);
            if (validationErrors.Count > 0)
            {
                string errorMessage = string.Join("<br>", validationErrors);
                Response.Write($"{{\"success\":false,\"message\":\"{errorMessage}\"}}");
                return;
            }

            // 檢查是否為計畫變更中狀態，如果是則驗證變更說明
            string projectID = formData["ProjectID"]?.ToString();
            if (!string.IsNullOrEmpty(projectID))
            {
                var projectMain = OFS_ClbApplicationHelper.GetProjectMainData(projectID);
                if (projectMain != null && projectMain.IsProjChanged == 1)
                {
                    // 驗證變更說明是否已填寫
                    string changeBefore = Request.Form["txtChangeBefore"] ?? "";
                    string changeAfter = Request.Form["txtChangeAfter"] ?? "";

                    if (string.IsNullOrEmpty(changeBefore) && string.IsNullOrEmpty(changeAfter))
                    {
                        Response.Write("{\"success\":false,\"message\":\"計畫變更中，請填寫變更說明後再進行儲存\"}");
                        return;
                    }
                }
            }

            // 執行正式儲存（傳入 false 表示正式儲存）
            string savedProjectID = SaveBasicDataAjax(formData, false);

            if (!string.IsNullOrEmpty(savedProjectID))
            {
                Response.Write($"{{\"success\":true,\"message\":\"儲存成功！\",\"projectID\":\"{savedProjectID}\",\"enableUpload\":true}}");
            }
            else
            {
                Response.Write("{\"success\":false,\"message\":\"儲存失敗，請檢查必填欄位\"}");
            }
        }
        catch (Exception ex)
        {
            Response.Write($"{{\"success\":false,\"message\":\"儲存失敗：{ex.Message}\"}}");
        }
    }

    /// <summary>
    /// 處理申請提送請求
    /// </summary>
    public void HandleApplicationSubmission()
    {
        try
        {
            string projectID = Request.Form["projectID"];

            // 如果 Form 中沒有 ProjectID，嘗試從 QueryString 獲取
            if (string.IsNullOrEmpty(projectID))
            {
                projectID = Request.QueryString["ProjectID"];
            }

            if (string.IsNullOrEmpty(projectID))
            {
                var errorResult = new { success = false, message = "計畫編號不能為空" };
                Response.Write(JsonConvert.SerializeObject(errorResult));
                return;
            }

            var lastProjectMain = OFS_ClbApplicationHelper.GetProjectMainData(projectID);

            // 檢查是否為計畫變更中狀態，如果是則驗證變更說明
            if (lastProjectMain != null && lastProjectMain.IsProjChanged == 1)
            {
                // 驗證變更說明是否已填寫
                string changeBefore = Request.Form["txtChangeBefore"] ?? "";
                string changeAfter = Request.Form["txtChangeAfter"] ?? "";

                if (string.IsNullOrEmpty(changeBefore) && string.IsNullOrEmpty(changeAfter))
                {
                    var changeErrorResult = new { success = false, message = "計畫變更中，請填寫變更說明後再提送申請" };
                    Response.Write(JsonConvert.SerializeObject(changeErrorResult));
                    return;
                }
            }
            var basicData = OFS_ClbApplicationHelper.GetBasicData(projectID);
            string ProjectName = basicData.ProjectNameTw;
            // 新增 PDF 合併邏輯


            try
            {
                if (lastProjectMain.StatusesName == "計畫書修正中")
                {
                   //  20251126理論上來說 社團僅有 補正補件 不會有計畫書修正 了。
                    // 決定審核 修正計畫書
                    // OFS_ClbApplicationHelper.UpdateProjectStatus(projectID, "決審核定", "計畫書審核中", "3");
                    //
                    // // 產生核定版 PDF
                    // MergePdfFiles(projectID, ProjectName,"核定版");

                }
                //這是計畫變更
                else if(lastProjectMain.IsProjChanged == 1)
                {
                    // 儲存變更說明記錄
                    string changeBefore  = Request.Form["txtChangeBefore"] ?? "";
                    string changeAfter = Request.Form["txtChangeAfter"] ?? "";

                    // 只有當有變更說明時才儲存
                    if (!string.IsNullOrEmpty(changeBefore) || !string.IsNullOrEmpty(changeAfter))
                    {
                        var ChangeRecord = OFSProjectChangeRecordHelper.getApplying("CLB", 1, projectID);
                        ChangeRecord.Form2After = changeAfter;
                        ChangeRecord.Form2Before = changeBefore;
                        ChangeRecord.Status = 2; // 變更審核中
                        OFSProjectChangeRecordHelper.update(ChangeRecord);

                    }
                    ExecuteProjectChange(projectID, ProjectName);

                    // 取得計畫資料並寄信
                    try
                    {
                        // 取得計畫名稱
                        // var basicData = OFS_ClbApplicationHelper.GetBasicData(projectID);
                        var projectMainData = OFS_ClbApplicationHelper.GetProjectMainData(projectID);

                        if (basicData != null && projectMainData != null)
                        {
                            string projectName = basicData.ProjectNameTw;
                            string supervisoryAccount = projectMainData.SupervisoryPersonAccount;

                            // 根據承辦人帳號取得 UserID
                            int? organizer = SysUserHelper.GetUserIDByAccount(supervisoryAccount);

                            // 寄送通知信
                            NotificationHelper.G2("學校社團", projectName, "計畫變更申請", organizer);
                        }
                    }
                    catch (Exception emailEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"寄送通知信時發生錯誤: {emailEx.Message}");
                        // 寄信失敗不影響主要流程
                    }
                }
                else
                {
                    // 申請計畫
                    OFS_ClbApplicationHelper.UpdateProjectStatus(projectID, "內容審查", "審核中", "3");

                    // 產生送審版與核定版 PDF
                    MergePdfFiles(projectID, ProjectName,"送審版");
                    MergePdfFiles(projectID, ProjectName,"核定版");
                    NotificationHelper.A1("學校社團", ProjectName,  "CLB");
                    //提送歷史紀錄
                    InsertClbHistory(projectID);
                }
                //快照 ， 送審版 status = 11 , 計劃書修正 = 43  , 計畫變更 51 
                // 核定版 如果 沒有計畫書修正 就是 11 有計畫書修正 就是 43 (最新) 
                OFS_ClbSnapshotHelper.CreateSnapshot(projectID);

                var successResult = new { success = true, message = "儲存成功！", projectID = projectID, enableUpload = true };
                Response.Write(JsonConvert.SerializeObject(successResult));
            }
            catch (Exception pdfEx)
            {
                // PDF 合併錯誤不影響主要流程，但會記錄錯誤
                var pdfErrorResult = new
                {
                    success = false,
                    message = $"PDF 合併失敗：{pdfEx.Message}",
                    projectID = projectID,
                    enableUpload = true
                };
                Response.Write(JsonConvert.SerializeObject(pdfErrorResult));
                HttpContext.Current.ApplicationInstance.CompleteRequest(); // 避免後面 ASPX 繼續輸出 HTML
                return;
            }
        }
        catch (Exception ex)
        {
            var mainErrorResult = new { success = false, message = $"提送失敗：{ex.Message}" };
            Response.Write(JsonConvert.SerializeObject(mainErrorResult));
        }
    }
    private void ExecuteProjectChange(string projectID, string projectName)
    {
        // 計畫執行：計畫變更 --> 計畫變更審核中
        OFS_ClbApplicationHelper.UpdateProjectChangeStatus(projectID, 2);
            
        //20251126 不需要計畫變更最新版了 僅需要送審版和核定版
        // 計畫變更版本管理 - 檢查是否存在計畫變更最新版並建立版本備份
        // CreatePlanChangeVersionBackup(projectID, projectName);

        // 產生並合併送審版與核定版 PDF
        // MergePdfFiles(projectID, projectName, "計畫變更最新版");
        
        
    }

    /// <summary>
    /// 插入 CLB 歷史紀錄
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    private void InsertClbHistory(string projectID)
    {
        // 取得最新歷史記錄的狀態
        string stageStatusBefore = ApplicationChecklistHelper.GetLatestStageStatus(projectID) ?? "";

        var currentUser = GetCurrentUserInfo();
        var projectMain = OFS_ClbApplicationHelper.GetProjectMainData(projectID);
        var historyLog = new OFS_CaseHistoryLog
        {
            ProjectID = projectID,
            ChangeTime = DateTime.Now,
            UserName = currentUser?.UserName ?? "系統",
            StageStatusBefore = stageStatusBefore,
            StageStatusAfter = projectMain.Statuses + projectMain.StatusesName,
            Description = $"提送至{projectMain.Statuses + projectMain.StatusesName}"
        };
        ApplicationChecklistHelper.InsertCaseHistoryLog(historyLog);

    }

    /// <summary>
    /// 取得目前登入使用者資訊
    /// </summary>
    private SessionHelper.UserInfoClass GetCurrentUserInfo()
    {
        try
        {
            return SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    /// <summary>
    /// 合併 CLB PDF 檔案
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <param name="ProjectName">計畫名稱</param>
    /// <param name="version">版本（送審版或核定版）</param>
    private void MergePdfFiles(string projectId, string ProjectName, string version)
    {
        try
        {
            Env.Log.Info($"[CLB] PDF 合併開始 - ProjectID: {projectId}, ProjectName: {ProjectName}, Version: {version}");

            // 建立檔案路徑清單
            var pdfFilePaths = new List<string>();

            // CLB 固定要合併的檔案 Code（依順序）
            var fileCodesToMerge = new List<string>
            {
                "FILE_CLB1",  // 申請表
                "FILE_CLB2",  // 計畫書
                "FILE_CLB3",  // 切結書
                "FILE_CLB4"   // 其他附件
            };

            // 從資料庫取得檔案路徑並檢查檔案是否存在
            foreach (string fileCode in fileCodesToMerge)
            {
                // FILE_CLB4 是複數檔案，其他是單一檔案
                if (fileCode == "FILE_CLB4")
                {
                    // 取得所有 FILE_CLB4 的檔案
                    var uploadedFiles = OFS_ClbApplicationHelper.GetUploadedFilesByCode(projectId, fileCode);

                    if (uploadedFiles != null && uploadedFiles.Count > 0)
                    {
                        foreach (var file in uploadedFiles)
                        {
                            string fullPath = Page.Server.MapPath($"~/{file.TemplatePath}");

                            if (File.Exists(fullPath))
                            {
                                pdfFilePaths.Add(fullPath);
                                System.Diagnostics.Debug.WriteLine($"找到檔案 {fileCode}：{fullPath}");
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"檔案 {fileCode} 不存在：{fullPath}");
                            }
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"資料庫中找不到檔案記錄，FileCode：{fileCode}");
                    }
                }
                else
                {
                    // 其他檔案只取第一筆
                    var uploadedFile = OFS_ClbApplicationHelper.GetUploadedFile(projectId, fileCode);

                    if (uploadedFile != null)
                    {
                        string fullPath = Page.Server.MapPath($"~/{uploadedFile.TemplatePath}");

                        if (File.Exists(fullPath))
                        {
                            pdfFilePaths.Add(fullPath);
                            System.Diagnostics.Debug.WriteLine($"找到檔案 {fileCode}：{fullPath}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"檔案 {fileCode} 不存在：{fullPath}");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"資料庫中找不到檔案記錄，FileCode：{fileCode}");
                    }
                }
            }

            // 如果沒有檔案可以合併，直接返回
            if (pdfFilePaths.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("沒有找到任何可合併的 PDF 檔案");
                return;
            }

            // 建立合併後的檔案名稱和路徑（加上時間戳記）
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            // 清除 ProjectName 中的不合法字符
            string safeProjectName = RemoveInvalidFileNameChars(ProjectName);

            string mergedFileName = $"{projectId}_社團_{safeProjectName}_{version}_{timestamp}.pdf";
            string uploadFolderPath = Page.Server.MapPath($"~/UploadFiles/OFS/CLB/{projectId}");
            string mergedFilePath = Path.Combine(uploadFolderPath, mergedFileName);

            // 確保目錄存在
            if (!Directory.Exists(uploadFolderPath))
            {
                Directory.CreateDirectory(uploadFolderPath);
            }

            // 使用 PdfHelper 合併 PDF
            byte[] mergedPdfBytes = PdfHelper.MergePdfs(pdfFilePaths, mergedFilePath);

            System.Diagnostics.Debug.WriteLine($"PDF 合併完成：{mergedFilePath}，合併了 {pdfFilePaths.Count} 個檔案");

            // 儲存合併後的 PDF 記錄到資料庫（僅送審版和核定版）
            if (version == "送審版" || version == "核定版")
            {
                string fileCodeForDb = GetMergedFileCodeForClb(version);
                string relativeTemplatePath = $"UploadFiles/OFS/CLB/{projectId}/{mergedFileName}";

                // 先刪除舊記錄（避免 FileCode 重複，確保資料乾淨）
                OFS_ClbApplicationHelper.DeleteUploadFile(projectId, fileCodeForDb);
                System.Diagnostics.Debug.WriteLine($"已刪除舊的合併 PDF 記錄：FileCode={fileCodeForDb}");

                // 新增新記錄
                var uploadFile = new OFS_CLB_UploadFile
                {
                    ProjectID = projectId,
                    FileCode = fileCodeForDb,
                    FileName = mergedFileName,
                    TemplatePath = relativeTemplatePath
                };
                OFS_ClbApplicationHelper.InsertUploadFile(uploadFile);
                System.Diagnostics.Debug.WriteLine($"合併 PDF 記錄已存入資料庫：FileCode={fileCodeForDb}, FileName={mergedFileName}");
            }

            Env.Log.Info($"[CLB] PDF 合併完成 - {mergedFileName}");
        }
        catch (Exception ex)
        {
            Env.Log.Error($"[CLB] PDF 合併錯誤 - ProjectID: {projectId}, ProjectName: {ProjectName}");
            Env.Log.Error($"錯誤訊息: {ex.Message}");
            Env.Log.Error($"堆疊追蹤: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Env.Log.Error($"內部錯誤: {ex.InnerException.Message}");
            }
            throw;
        }
    }

    /// <summary>
    /// 清除檔案名稱中的不合法字符
    /// </summary>
    /// <param name="fileName">原始檔案名稱</param>
    /// <returns>清理後的檔案名稱</returns>
    private string RemoveInvalidFileNameChars(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return fileName;

        // 取得 Windows 不合法的檔案名稱字符
        char[] invalidChars = Path.GetInvalidFileNameChars();

        // 將不合法字符替換為底線
        foreach (char c in invalidChars)
        {
            fileName = fileName.Replace(c, '_');
        }

        return fileName;
    }

    /// <summary>
    /// 建立計畫變更版本備份
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <param name="projectName">計畫名稱</param>
    private void CreatePlanChangeVersionBackup(string projectId, string projectName)
    {
        try
        {
            string uploadFolderPath = Page.Server.MapPath($"~/UploadFiles/OFS/CLB/{projectId}");
            string latestVersionFileName = $"{projectId}_社團_{projectName}_計畫變更最新版.pdf";
            string latestVersionFilePath = Path.Combine(uploadFolderPath, latestVersionFileName);

            // 檢查計畫變更最新版檔案是否存在
            if (File.Exists(latestVersionFilePath))
            {
                // 掃描現有版本號碼
                int nextVersion = GetNextVersionNumber(uploadFolderPath, projectId, projectName);

                // 建立新的版本檔案名稱
                string versionFileName = $"{projectId}_社團_{projectName}_計畫變更v{nextVersion}.pdf";
                string versionFilePath = Path.Combine(uploadFolderPath, versionFileName);

                // 複製檔案
                File.Copy(latestVersionFilePath, versionFilePath, true);

            }
      
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"建立計畫變更版本備份時發生錯誤：{ex.Message}");
            // 不拋出例外，避免影響主要流程
        }
    }

    /// <summary>
    /// 取得下一個版本號碼
    /// </summary>
    /// <param name="folderPath">資料夾路徑</param>
    /// <param name="projectId">專案ID</param>
    /// <param name="projectName">計畫名稱</param>
    /// <returns>下一個版本號碼</returns>
    private int GetNextVersionNumber(string folderPath, string projectId, string projectName)
    {
        try
        {
            if (!Directory.Exists(folderPath))
                return 1;

            // 建立搜尋模式，尋找所有計畫變更版本檔案
            string searchPattern = $"{projectId}_社團_{projectName}_計畫變更v*.pdf";
            var versionFiles = Directory.GetFiles(folderPath, searchPattern);

            int maxVersion = 0;

            foreach (string filePath in versionFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                // 從檔案名稱提取版本號碼，格式：{projectId}_社團_{projectName}_計畫變更v{版本號}
                string versionPart = fileName.Substring(fileName.LastIndexOf("_計畫變更v") + "_計畫變更v".Length);

                if (int.TryParse(versionPart, out int version))
                {
                    maxVersion = Math.Max(maxVersion, version);
                }
            }

            return maxVersion + 1;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得版本號碼時發生錯誤：{ex.Message}");
            return 1; // 發生錯誤時回傳版本 1
        }
    }

    /// <summary>
    /// 從 Request 取得表單資料（AJAX 版本）
    /// </summary>
    /// <returns>表單資料字典</returns>
    private Dictionary<string, object> GetFormDataFromRequest()
    {
        var data = new Dictionary<string, object>();

        // 基本資料
        data["ProjectID"] = Request.Form["projectID"] ?? Request.QueryString["ProjectID"] ?? "";
        data["Year"] = int.Parse(Request.Form["year"] ?? DateTime.Now.Year.ToString());
        data["SubsidyPlanType"] = Request.Form["subsidyPlanType"] ?? "";
        data["ProjectNameTw"] = Request.Form["projectNameTw"] ?? "";

        // 申請補助類型
        data["SubsidyType"] = Request.Form["subsidyType"] ?? "";

        data["SchoolName"] = Request.Form["schoolName"] ?? "";
        data["School_IDNumber"] = Request.Form["schoolIDNumber"] ?? "";
        data["ClubName"] = Request.Form["clubName"] ?? "";
        data["Address"] = Request.Form["address"] ?? "";

        // 成立日期
        string creationDate = Request.Form["creationDate"];
        if (!string.IsNullOrEmpty(creationDate) && DateTime.TryParse(creationDate, out DateTime creationDateTime))
            data["CreationDate"] = creationDateTime;

        // 計畫期間
        string startDate = Request.Form["startDate"];
        string endDate = Request.Form["endDate"];
        if (!string.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out DateTime startDateTime))
            data["StartDate"] = startDateTime;
        if (!string.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out DateTime endDateTime))
            data["EndDate"] = endDateTime;

        data["Purpose"] = Request.Form["purpose"] ?? "";
        data["PlanContent"] = Request.Form["planContent"] ?? "";
        data["PreBenefits"] = Request.Form["preBenefits"] ?? "";
        data["PlanLocation"] = Request.Form["planLocation"] ?? "";
        data["EstimatedPeople"] = Request.Form["estimatedPeople"] ?? "";
        data["EmergencyPlan"] = Request.Form["emergencyPlan"] ?? "";

        // 經費資料
        if (int.TryParse(Request.Form["applyAmount"], out int applyAmount))
            data["ApplyAmount"] = applyAmount;
        if (int.TryParse(Request.Form["selfAmount"], out int selfAmount))
            data["SelfAmount"] = selfAmount;
        if (int.TryParse(Request.Form["otherAmount"], out int otherAmount))
            data["OtherAmount"] = otherAmount;

        // 最近兩年曾獲本會補助
        if (bool.TryParse(Request.Form["isPreviouslySubsidized"], out bool isPreviouslySubsidized))
            data["IsPreviouslySubsidized"] = isPreviouslySubsidized;

        // 其他補助明細資料 (JSON 字串)
        data["OtherSubsidyData"] = Request.Form["otherSubsidyData"] ?? "";

        // 經費預算規劃資料 (JSON 字串)
        data["BudgetPlanData"] = Request.Form["budgetPlanData"] ?? "";

        // 經費說明資料 (JSON 字串)
        data["ReceivedSubsidyData"] = Request.Form["receivedSubsidyData"] ?? "";

        // 人員資料
        data["TeacherName"] = Request.Form["teacherName"] ?? "";
        data["TeacherJobTitle"] = Request.Form["teacherJobTitle"] ?? "";
        data["TeacherPhone"] = Request.Form["teacherPhone"] ?? "";
        data["ContactName"] = Request.Form["contactName"] ?? "";
        data["ContactJobTitle"] = Request.Form["contactJobTitle"] ?? "";
        data["ContactPhone"] = Request.Form["contactPhone"] ?? "";
        data["txtChangeBefore"] = Request.Form["txtChangeBefore"] ?? "";
        data["txtChangeAfter"] = Request.Form["txtChangeAfter"] ?? "";
        

        return data;
    }

    /// <summary>
    /// 儲存基本資料（AJAX 版本）
    /// </summary>
    /// <param name="formData">表單資料</param>
    /// <param name="isTempSave">是否為暫存</param>
    /// <returns>成功儲存的 ProjectID</returns>
    private string SaveBasicDataAjax(Dictionary<string, object> formData, bool isTempSave)
    {
        string projectID = formData.ContainsKey("ProjectID") ? formData["ProjectID"]?.ToString() : "";
        try
        {
            Env.Log.Info($"開始儲存基本資料 - ProjectID: {projectID}, 是否暫存: {isTempSave}");

            // 建立基本資料物件
            var basicData = new OFS_CLB_Application_Basic
            {
                ProjectID = formData["ProjectID"]?.ToString(), // 如果是新建會是空的，Helper會自動產生
                Year = (int)formData["Year"],
                SubsidyPlanType = formData["SubsidyPlanType"]?.ToString(),
                ProjectNameTw = formData["ProjectNameTw"]?.ToString(),
                SubsidyType = formData["SubsidyType"]?.ToString(),
                SchoolName = formData["SchoolName"]?.ToString(),
                ClubName = formData["ClubName"]?.ToString(),
                CreationDate = formData.ContainsKey("CreationDate") ? (DateTime?)formData["CreationDate"] : null,
                School_IDNumber = formData["School_IDNumber"]?.ToString(),
                Address = formData["Address"]?.ToString(),
                ApplyAmount = formData.ContainsKey("ApplyAmount") ? (int?)formData["ApplyAmount"] : null,
                SelfAmount = formData.ContainsKey("SelfAmount") ? (int?)formData["SelfAmount"] : null,
                OtherAmount = formData.ContainsKey("OtherAmount") ? (int?)formData["OtherAmount"] : null,
                IsPreviouslySubsidized = formData.ContainsKey("IsPreviouslySubsidized") ? (bool?)formData["IsPreviouslySubsidized"] : null
            };

            // 儲存資料並取得 ProjectID
            Env.Log.Info($"步驟1: 儲存基本資料 - ProjectID: {projectID}");
            string savedProjectID = SaveBasicData(basicData);
            Env.Log.Info($"步驟1完成 - 取得 ProjectID: {savedProjectID}");

            // 儲存人員表單
            Env.Log.Info($"步驟2: 儲存人員資料 - ProjectID: {savedProjectID}");
            SavePersonnelDataAjax(savedProjectID, formData);

            // 儲存計畫資訊
            Env.Log.Info($"步驟3: 儲存計畫資訊 - ProjectID: {savedProjectID}");
            SavePlanDataAjax(savedProjectID, formData);

            // 儲存經費資訊
            Env.Log.Info($"步驟4: 儲存經費資訊 - ProjectID: {savedProjectID}");
            SaveFundsDataAjax(savedProjectID, formData);

            // 儲存 Project_Main 資訊
            Env.Log.Info($"步驟5: 儲存 Project_Main 資訊 - ProjectID: {savedProjectID}");
            SaveProjectMainData(savedProjectID, formData, isTempSave);

            //儲存變更說明
            Env.Log.Info($"步驟6: 儲存變更說明 - ProjectID: {savedProjectID}");
            SaveProjectChangeRecord(savedProjectID, formData);

            Env.Log.Info($"所有資料儲存完成 - ProjectID: {savedProjectID}");
            return savedProjectID;
        }
        catch (Exception ex)
        {
            Env.Log.Error($"SaveBasicDataAjax 失敗 - ProjectID: {projectID}, 是否暫存: {isTempSave}", ex);
            throw new Exception($"儲存基本資料失敗：{ex.Message}", ex);
        }
    }

    /// <summary>
    /// 儲存人員資料（AJAX 版本）
    /// </summary>
    private void SavePersonnelDataAjax(string projectID, Dictionary<string, object> formData)
    {
        try
        {
            List<OFS_CLB_Application_Personnel> personnelList = new List<OFS_CLB_Application_Personnel>();

            // 社團指導老師
            if (!string.IsNullOrEmpty(formData["TeacherName"]?.ToString()))
            {
                personnelList.Add(new OFS_CLB_Application_Personnel
                {
                    ProjectID = projectID,
                    Personnel = "社團指導老師",
                    Name = formData["TeacherName"]?.ToString(),
                    JobTitle = formData["TeacherJobTitle"]?.ToString(),
                    PhoneNum = formData["TeacherPhone"]?.ToString()
                });
            }

            // 社團業務聯絡人
            if (!string.IsNullOrEmpty(formData["ContactName"]?.ToString()))
            {
                personnelList.Add(new OFS_CLB_Application_Personnel
                {
                    ProjectID = projectID,
                    Personnel = "社團業務聯絡人",
                    Name = formData["ContactName"]?.ToString(),
                    JobTitle = formData["ContactJobTitle"]?.ToString(),
                    PhoneNum = formData["ContactPhone"]?.ToString()
                });
            }

            // 呼叫 Helper 儲存人員資料
            OFS_ClbApplicationHelper.SavePersonnelData(projectID, personnelList);
            Env.Log.Info($"人員資料儲存成功 - ProjectID: {projectID}, 人員數量: {personnelList.Count}");
        }
        catch (Exception ex)
        {
            Env.Log.Error($"SavePersonnelDataAjax 失敗 - ProjectID: {projectID}", ex);
            throw new Exception($"儲存人員資料失敗：{ex.Message}", ex);
        }
    }

    /// <summary>
    /// 儲存計畫資訊（AJAX 版本）
    /// </summary>
    private void SavePlanDataAjax(string projectID, Dictionary<string, object> formData)
    {
        try
        {
            var planData = new OFS_CLB_Application_Plan
            {
                ProjectID = projectID,
                StartDate = formData.ContainsKey("StartDate") ? (DateTime?)formData["StartDate"] : null,
                EndDate = formData.ContainsKey("EndDate") ? (DateTime?)formData["EndDate"] : null,
                Purpose = formData["Purpose"]?.ToString(),
                PlanContent = formData["PlanContent"]?.ToString(),
                PreBenefits = formData["PreBenefits"]?.ToString(),
                PlanLocation = formData["PlanLocation"]?.ToString(),
                EstimatedPeople = formData["EstimatedPeople"]?.ToString(),
                EmergencyPlan = formData["EmergencyPlan"]?.ToString()
            };

            // 呼叫 Helper 儲存計畫資訊
            OFS_ClbApplicationHelper.SavePlanData(planData);
            Env.Log.Info($"計畫資訊儲存成功 - ProjectID: {projectID}");
        }
        catch (Exception ex)
        {
            Env.Log.Error($"SavePlanDataAjax 失敗 - ProjectID: {projectID}", ex);
            throw new Exception($"儲存計畫資訊失敗：{ex.Message}", ex);
        }
    }

    /// <summary>
    /// 儲存經費資訊（AJAX 版本）
    /// </summary>
    private void SaveFundsDataAjax(string projectID, Dictionary<string, object> formData)
    {
        try
        {
            // 儲存其他補助明細資料到 OFS_CLB_Other_Subsidy
            Env.Log.Info($"步驟4.1: 儲存其他補助明細 - ProjectID: {projectID}");
            SaveOtherSubsidyData(projectID, formData);

            // 儲存經費預算規劃資料到 OFS_CLB_Budget_Plan
            Env.Log.Info($"步驟4.2: 儲存經費預算規劃 - ProjectID: {projectID}");
            SaveBudgetPlanData(projectID, formData);

            // 儲存經費說明資料到 OFS_CLB_Received_Subsidy
            Env.Log.Info($"步驟4.3: 儲存經費說明 - ProjectID: {projectID}");
            SaveReceivedSubsidyData(projectID, formData);

            Env.Log.Info($"經費資訊儲存成功 - ProjectID: {projectID}");
        }
        catch (Exception ex)
        {
            Env.Log.Error($"SaveFundsDataAjax 失敗 - ProjectID: {projectID}", ex);
            throw new Exception($"儲存經費資訊失敗：{ex.Message}", ex);
        }
    }

    /// <summary>
    /// 儲存其他補助明細資料到 OFS_CLB_Other_Subsidy
    /// </summary>
    private void SaveOtherSubsidyData(string projectID, Dictionary<string, object> formData)
    {
        try
        {
            // 從 formData 取得其他補助明細 JSON 字串
            string otherSubsidyDataJson = formData.ContainsKey("OtherSubsidyData")
                ? formData["OtherSubsidyData"]?.ToString()
                : null;

            // 反序列化 JSON 為物件列表
            var subsidyDataList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(otherSubsidyDataJson);

            // 轉換為 Entity 列表
            List<OFS_CLB_Other_Subsidy> subsidyList = new List<OFS_CLB_Other_Subsidy>();

            foreach (var item in subsidyDataList)
            {
                subsidyList.Add(new OFS_CLB_Other_Subsidy
                {
                    ID = item.ContainsKey("ID") ? Convert.ToInt32(item["ID"]) : 0,
                    ProjectID = projectID,
                    Unit = item.ContainsKey("Unit") ? item["Unit"]?.ToString() : "",
                    Amount = item.ContainsKey("Amount") ? Convert.ToInt32(item["Amount"]) : (int?)null,
                    Content = item.ContainsKey("Content") ? item["Content"]?.ToString() : ""
                });
            }

            // 呼叫 Helper 儲存資料（先刪除後新增）
            OFS_ClbOtherSubsidyHelper.SaveOtherSubsidyData(projectID, subsidyList);
            Env.Log.Info($"其他補助明細儲存成功 - ProjectID: {projectID}, 筆數: {subsidyList.Count}");
        }
        catch (Exception ex)
        {
            string jsonData = formData.ContainsKey("OtherSubsidyData") && formData["OtherSubsidyData"] != null ? formData["OtherSubsidyData"].ToString() : "null";
            Env.Log.Error($"SaveOtherSubsidyData 失敗 - ProjectID: {projectID}, JSON: {jsonData}", ex);
            throw new Exception($"儲存其他補助明細資料失敗：{ex.Message}", ex);
        }
    }

    /// <summary>
    /// 儲存經費預算規劃資料到 OFS_CLB_Budget_Plan
    /// </summary>
    private void SaveBudgetPlanData(string projectID, Dictionary<string, object> formData)
    {
        try
        {
            // 從 formData 取得經費預算規劃 JSON 字串
            string budgetPlanDataJson = formData.ContainsKey("BudgetPlanData")
                ? formData["BudgetPlanData"]?.ToString()
                : null;

            // 如果沒有資料，則刪除所有舊資料
            if (string.IsNullOrEmpty(budgetPlanDataJson))
            {
                OFS_ClbBudgetPlanHelper.DeleteByProjectID(projectID);
                Env.Log.Info($"經費預算規劃資料為空，已刪除舊資料 - ProjectID: {projectID}");
                return;
            }

            // 反序列化 JSON 為物件列表
            var budgetPlanDataList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(budgetPlanDataJson);

            // 轉換為 Entity 列表
            List<OFS_CLB_Budget_Plan> budgetPlanList = new List<OFS_CLB_Budget_Plan>();

            foreach (var item in budgetPlanDataList)
            {
                budgetPlanList.Add(new OFS_CLB_Budget_Plan
                {
                    ID = item.ContainsKey("ID") ? Convert.ToInt32(item["ID"]) : 0,
                    ProjectID = projectID,
                    Title = item.ContainsKey("Title") ? item["Title"]?.ToString() : "",
                    Amount = item.ContainsKey("Amount") ? Convert.ToInt32(item["Amount"]) : (int?)null,
                    OtherAmount = item.ContainsKey("OtherAmount") ? Convert.ToInt32(item["OtherAmount"]) : (int?)null,
                    Description = item.ContainsKey("Description") ? item["Description"]?.ToString() : ""
                });
            }

            // 呼叫 Helper 儲存資料（先刪除後新增）
            OFS_ClbBudgetPlanHelper.SaveBudgetPlanData(projectID, budgetPlanList);
            Env.Log.Info($"經費預算規劃儲存成功 - ProjectID: {projectID}, 筆數: {budgetPlanList.Count}");
        }
        catch (Exception ex)
        {
            string jsonData = formData.ContainsKey("BudgetPlanData") && formData["BudgetPlanData"] != null ? formData["BudgetPlanData"].ToString() : "null";
            Env.Log.Error($"SaveBudgetPlanData 失敗 - ProjectID: {projectID}, JSON: {jsonData}", ex);
            throw new Exception($"儲存經費預算規劃資料失敗：{ex.Message}", ex);
        }
    }

    /// <summary>
    /// 儲存經費說明資料到 OFS_CLB_Received_Subsidy
    /// </summary>
    private void SaveReceivedSubsidyData(string projectID, Dictionary<string, object> formData)
    {
        try
        {
            // 從 formData 取得經費說明 JSON 字串
            string receivedSubsidyDataJson = formData.ContainsKey("ReceivedSubsidyData")
                ? formData["ReceivedSubsidyData"]?.ToString()
                : null;

            // 如果沒有資料，則刪除所有舊資料
            if (string.IsNullOrEmpty(receivedSubsidyDataJson))
            {
                OFS_ClbReceivedSubsidyHelper.DeleteByProjectID(projectID);
                Env.Log.Info($"經費說明資料為空，已刪除舊資料 - ProjectID: {projectID}");
                return;
            }

            // 反序列化 JSON 為物件列表
            var receivedSubsidyDataList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(receivedSubsidyDataJson);

            // 轉換為 Entity 列表
            List<OFS_CLB_Received_Subsidy> receivedSubsidyList = new List<OFS_CLB_Received_Subsidy>();

            foreach (var item in receivedSubsidyDataList)
            {
                receivedSubsidyList.Add(new OFS_CLB_Received_Subsidy
                {
                    ID = item.ContainsKey("ID") ? Convert.ToInt32(item["ID"]) : 0,
                    ProjectID = projectID,
                    Name = item.ContainsKey("ProjectName") ? item["ProjectName"]?.ToString() : "",
                    Amount = item.ContainsKey("Amount") ? Convert.ToInt32(item["Amount"]) : 0
                });
            }

            // 呼叫 Helper 儲存資料（先刪除後新增）
            OFS_ClbReceivedSubsidyHelper.SaveReceivedSubsidyData(projectID, receivedSubsidyList);
            Env.Log.Info($"經費說明儲存成功 - ProjectID: {projectID}, 筆數: {receivedSubsidyList.Count}");
        }
        catch (Exception ex)
        {
            string jsonData = formData.ContainsKey("ReceivedSubsidyData") && formData["ReceivedSubsidyData"] != null ? formData["ReceivedSubsidyData"].ToString() : "null";
            Env.Log.Error($"SaveReceivedSubsidyData 失敗 - ProjectID: {projectID}, JSON: {jsonData}", ex);
            throw new Exception($"儲存經費說明資料失敗：{ex.Message}", ex);
        }
    }

    /// <summary>
    /// 驗證必填欄位
    /// </summary>
    /// <param name="formData">表單資料</param>
    /// <returns>驗證錯誤訊息列表</returns>
    private List<string> ValidateRequiredFields(Dictionary<string, object> formData)
    {
        List<string> errors = new List<string>();

        // 取得申請補助類型
        string subsidyType = formData["SubsidyType"]?.ToString() ?? "";
        bool isPublicActivity = subsidyType == "Public";
        bool isOperation = subsidyType == "Admin";

        // 基本必填欄位（所有類型都需要）
        if (string.IsNullOrWhiteSpace(formData["ProjectNameTw"]?.ToString()))
            errors.Add("請輸入計畫名稱");

        if (string.IsNullOrWhiteSpace(subsidyType))
            errors.Add("請選擇申請補助類型");

        if (string.IsNullOrWhiteSpace(formData["SchoolName"]?.ToString()))
            errors.Add("請輸入學校名稱");

        if (string.IsNullOrWhiteSpace(formData["ClubName"]?.ToString()))
            errors.Add("請輸入社團全名");

        // affairs-view 欄位：成立日期（社務補助或公共活動費才需要）
        if (isOperation || isPublicActivity)
        {
            if (!formData.ContainsKey("CreationDate") || formData["CreationDate"] == null)
                errors.Add("請輸入成立日期");
        }

        // public-view 欄位（只有公共活動費才需要）
        if (isPublicActivity)
        {
            if (string.IsNullOrWhiteSpace(formData["School_IDNumber"]?.ToString()))
                errors.Add("請輸入學校統一編號");

            if (string.IsNullOrWhiteSpace(formData["Address"]?.ToString()))
                errors.Add("請輸入地址");
        }

        // 計畫執行期間（所有類型都需要）
        if (!formData.ContainsKey("StartDate") || formData["StartDate"] == null)
            errors.Add("請輸入計畫執行開始日期");

        if (!formData.ContainsKey("EndDate") || formData["EndDate"] == null)
            errors.Add("請輸入計畫執行結束日期");

        // 計畫資訊必填欄位（所有類型都需要）
        if (string.IsNullOrWhiteSpace(formData["Purpose"]?.ToString()))
            errors.Add("請輸入目的");

        if (string.IsNullOrWhiteSpace(formData["PlanContent"]?.ToString()))
            errors.Add("請輸入計畫內容");

        if (string.IsNullOrWhiteSpace(formData["PreBenefits"]?.ToString()))
            errors.Add("請輸入預期效益");

        // 人員必填欄位（所有類型都需要）
        if (string.IsNullOrWhiteSpace(formData["TeacherName"]?.ToString()))
            errors.Add("請輸入社團指導老師姓名");

        if (string.IsNullOrWhiteSpace(formData["TeacherJobTitle"]?.ToString()))
            errors.Add("請輸入社團指導老師職稱");

        if (string.IsNullOrWhiteSpace(formData["TeacherPhone"]?.ToString()))
            errors.Add("請輸入社團指導老師手機號碼");

        if (string.IsNullOrWhiteSpace(formData["ContactName"]?.ToString()))
            errors.Add("請輸入社團業務聯絡人姓名");

        if (string.IsNullOrWhiteSpace(formData["ContactJobTitle"]?.ToString()))
            errors.Add("請輸入社團業務聯絡人職稱");

        if (string.IsNullOrWhiteSpace(formData["ContactPhone"]?.ToString()))
            errors.Add("請輸入社團業務聯絡人手機號碼");

        // 經費必填欄位（所有類型都需要）
        if (!formData.ContainsKey("ApplyAmount") || formData["ApplyAmount"] == null)
            errors.Add("請輸入申請海委會補助／合作金額(元)");

        if (!formData.ContainsKey("SelfAmount") || formData["SelfAmount"] == null)
            errors.Add("請輸入申請單位自籌款(元)");

        return errors;
    }

    #endregion

    /// <summary>
    /// 整合查詢 CurrentStep，同時控制暫存按鈕顯示和上傳附件步驟開放
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="enableUploadStep">是否開放上傳附件步驟</param>
    private void SetButtonVisibilityAndStepAccess(string projectID, out bool enableUploadStep)
    {
        // 預設值
        enableUploadStep = false;
        bool showTempSaveButton = true; // 預設顯示暫存按鈕（當找不到 projectID 時）
        int currentStepNumber = 1; // 預設步驟為 1

        try
        {
            // 如果有 ProjectID，查詢 CurrentStep
            if (!string.IsNullOrEmpty(projectID))
            {
                string currentStep = OFS_ClbApplicationHelper.GetProjectCurrentStep(projectID);

                if (!string.IsNullOrEmpty(currentStep) && int.TryParse(currentStep, out int stepNumber))
                {
                    currentStepNumber = stepNumber;

                    // 控制暫存按鈕：CurrentStep > 1 時隱藏
                    showTempSaveButton = stepNumber <= 1;

                    // 控制上傳附件步驟：CurrentStep >= 2 時開放
                    enableUploadStep = stepNumber >= 2;
                }
            }

            // 設定按鈕可見性和步驟狀態（使用 JavaScript）
            string buttonScript = $@"
                $(document).ready(function() {{
                    // 設定暫存按鈕顯示
                    if ({showTempSaveButton.ToString().ToLower()}) {{
                        $('#btnTempSave').show();
                    }} else {{
                        $('#btnTempSave').hide();
                    }}

                    // 將 currentStep 傳遞給全域變數供 JavaScript 使用
                    window.currentStepNumber = {currentStepNumber};
                }});
            ";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "SetButtonVisibility", buttonScript, true);
        }
        catch (Exception)
        {
            // 如果發生例外，使用預設值
            enableUploadStep = false;
            string defaultButtonScript = @"
                $(document).ready(function() {
                    $('#btnTempSave').show();
                    window.currentStepNumber = 1;
                });
            ";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "SetDefaultButtonVisibility", defaultButtonScript, true);
        }
    }
    /// <summary>
    /// 初始化標籤系統
    /// </summary>
    /// <param name="enableUploadStep">是否啟用上傳附件步驟</param>
    /// <param name="initialTab">初始顯示的標籤</param>
    /// <param name="isReadOnly">是否為檢視模式</param>
    private void InitializeTabSystem(bool enableUploadStep, string initialTab, bool isReadOnly)
    {
        // 註冊 JavaScript 來初始化標籤系統
        string script = $@"
            $(document).ready(function() {{
                // 使用新的標籤系統初始化
                initializeTabSystem({enableUploadStep.ToString().ToLower()}, '{initialTab}', {isReadOnly.ToString().ToLower()});
            }});
        ";

        // 註冊 JavaScript
        Page.ClientScript.RegisterStartupScript(this.GetType(), "InitializeTabSystem", script, true);
    }
    
    

    #region 變更說明相關方法

    /// <summary>
    /// 初始化變更說明控制項
    /// </summary>
    private void InitializeChangeDescriptionControl()
    {
        try
        {
            if (!string.IsNullOrEmpty(ProjectID))
            {
                LoadChangeDescriptionData(ProjectID);
            }
            bool canEdit = CheckCanEditChangeDescription(ProjectID);
            bool shouldShow = CheckDisplayCondition(ProjectID);

            changeDescriptionSection.Visible = shouldShow;

            if (shouldShow && (IsReadOnly || !canEdit))
            {
                ApplyChangeDescriptionViewMode();
            }

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"初始化變更說明控制項時發生錯誤: {ex.Message}");
        }
    }

    /// <summary>
    /// 載入補助額度限制資料到前端
    /// </summary>
    private void LoadGrantLimitDataToFrontend()
    {
        try
        {
            // 查詢 CLB 補助類型的額度限制
            var grantLimits = OFSGrantTargetSettingHelper.query("CLB");

            if (grantLimits != null && grantLimits.Count > 0)
            {
                // 建立 JSON 物件
                var grantLimitData = new Dictionary<string, object>();

                foreach (var limit in grantLimits)
                {
                    grantLimitData[limit.TargetTypeID] = new
                    {
                        TargetName = limit.TargetName,
                        // 資料庫儲存單位為「萬」，需乘以 10000 轉換為「元」
                        GrantLimit = limit.GrantLimit.HasValue ? limit.GrantLimit.Value * 10000 : 0,
                        MatchingFund = limit.MatchingFund.HasValue ? limit.MatchingFund.Value * 10000 : 0
                    };
                }

                // 將資料序列化為 JSON
                string jsonData = JsonConvert.SerializeObject(grantLimitData);

                // 註冊到前端 JavaScript
                string script = $@"
                    window.grantLimitData = {jsonData};
                    console.log('補助額度限制資料已載入:', window.grantLimitData);
                ";

                Page.ClientScript.RegisterStartupScript(this.GetType(), "LoadGrantLimitData", script, true);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"載入補助額度限制資料時發生錯誤: {ex.Message}");
        }
    }

    /// <summary>
    /// 檢查是否符合顯示條件
    /// </summary>
    /// <param name="projectID">計畫ID</param>
    /// <returns>是否應該顯示</returns>
    private bool CheckDisplayCondition(string projectID)
    {
        try
        {
            if (string.IsNullOrEmpty(projectID))
            {
                return false;
            }

            // 取得專案狀態資料
            var projectData = OFS_ClbApplicationHelper.GetProjectMainData(projectID);
            if (projectData == null)
            {
                return false;
            }

            // 根據業務規則判斷是否顯示
            // 顯示條件：IsProjChanged = 1 或 2
            bool shouldShow = (projectData.IsProjChanged == 1 || projectData.IsProjChanged == 2);


            return shouldShow;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"檢查顯示條件時發生錯誤：{ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 檢查是否可以編輯
    /// </summary>
    /// <param name="projectID">計畫ID</param>
    /// <returns>是否可以編輯</returns>
    private bool CheckCanEditChangeDescription(string projectID)
    {
        try
        {
            if (string.IsNullOrEmpty(projectID))
            {
                return false;
            }

            // 取得專案狀態資料
            var projectData = OFS_ClbApplicationHelper.GetProjectMainData(projectID);
            if (projectData == null)
            {
                return false;
            }

            // 根據業務規則判斷是否可以編輯
            // 可以編輯：IsProjChanged = 1
            bool canEdit = projectData.IsProjChanged == 1;

            return canEdit;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"檢查編輯權限時發生錯誤：{ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 載入已存在的變更說明資料
    /// </summary>
    /// <param name="projectID">專案ID</param>
    private void LoadChangeDescriptionData(string projectID)
    {
        try
        {
            var changeDescription = OFSProjectChangeRecordHelper.getApplying("CLB", 1, projectID);

            // 將變更說明資料轉換為 JSON 格式並傳送到前端
            if (changeDescription != null)
            {
                ConvertChangeDescriptionToJson(changeDescription);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"載入變更說明資料時發生錯誤：{ex.Message}");
        }
    }

    /// <summary>
    /// 將變更說明資料轉換為 JSON 格式並傳送到前端
    /// </summary>
    /// <param name="changeDescription">變更說明資料</param>
    private void ConvertChangeDescriptionToJson(object changeDescription)
    {
        try
        {
            // 反序列化為動態物件以便讀取屬性
            dynamic data = changeDescription;

            // 載入到 ASP.NET TextBox 控制項
            if (data != null)
            {
                string form1Before = data.Form1Before?.ToString() ?? "";
                string form1After = data.Form1After?.ToString() ?? "";

                txtChangeBefore.Text = form1Before;
                txtChangeAfter.Text = form1After;
            }

            // 序列化整個 changeDescription 物件為 JSON（供前端切換時使用）
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(changeDescription);

            // 傳送 JSON 到前端並設定切換函數
            string script = $@"
                // 儲存變更說明資料到全域變數
                window.changeDescriptionData = {jsonString};
                console.log('變更說明資料已載入:', window.changeDescriptionData);

                // 載入當前表單資料的函數
                function loadCurrentFormData() {{
                    const currentForm = getCurrentFormType(); // 取得當前表單類型
                    loadFormChangeData(currentForm);
                }}

                // 取得當前表單類型的函數
                function getCurrentFormType() {{
                    // 檢查當前顯示的是哪個頁面
                    const applicationTab = document.getElementById('tab1');
                    const uploadTab = document.getElementById('uploadAttachmentSection');

                    if (applicationTab && applicationTab.style.display !== 'none') {{
                        return 'Form1'; // 申請表
                    }} else if (uploadTab && uploadTab.style.display !== 'none') {{
                        return 'Form2'; // 上傳附件
                    }}
                    return 'Form1'; // 預設為申請表
                }}

                // 根據表單類型載入對應的變更說明資料
                function loadFormChangeData(formType) {{
                    const txtChangeBefore = document.getElementById('{txtChangeBefore.ClientID}');
                    const txtChangeAfter = document.getElementById('{txtChangeAfter.ClientID}');

                    if (txtChangeBefore && txtChangeAfter && window.changeDescriptionData) {{
                        let beforeData = '';
                        let afterData = '';

                        if (formType === 'Form1') {{
                            // 申請表的變更說明
                            beforeData = window.changeDescriptionData.Form1Before || '';
                            afterData = window.changeDescriptionData.Form1After || '';
                        }} else if (formType === 'Form2') {{
                            // 上傳附件的變更說明
                            beforeData = window.changeDescriptionData.Form2Before || '';
                            afterData = window.changeDescriptionData.Form2After || '';
                        }}

                        txtChangeBefore.value = beforeData;
                        txtChangeAfter.value = afterData;
                    }}
                }}

                // 監聽頁面切換事件
                function setupFormSwitchListeners() {{
                    // 監聽申請表切換
                    const applicationTab = document.getElementById('applicationTab');
                    if (applicationTab) {{
                        applicationTab.addEventListener('click', function() {{
                            setTimeout(() => loadFormChangeData('Form1'), 100);
                        }});
                    }}

                    // 監聽上傳附件切換
                    const uploadTab = document.getElementById('uploadTab');
                    if (uploadTab) {{
                        uploadTab.addEventListener('click', function() {{
                            setTimeout(() => loadFormChangeData('Form2'), 100);
                        }});
                    }}
                }}

                // 頁面載入完成後執行
                if (document.readyState === 'complete') {{
                    loadCurrentFormData();
                    setupFormSwitchListeners();
                }} else {{
                    window.addEventListener('load', function() {{
                        loadCurrentFormData();
                        setupFormSwitchListeners();
                    }});
                }}
            ";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "LoadChangeDescriptionData", script, true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"轉換變更說明為 JSON 時發生錯誤：{ex.Message}");
        }
    }

    /// <summary>
    /// 套用檢視模式
    /// </summary>
    private void ApplyChangeDescriptionViewMode()
    {
        txtChangeBefore.Enabled = false;
        txtChangeAfter.Enabled = false;
    }
    

    #endregion

    #region 快照功能

    /// <summary>
    /// 從快照資料載入（用於快照檢視頁面）
    /// </summary>
    /// <param name="snapshotData">快照的 JSON 資料物件</param>
    public void LoadFromSnapshot(dynamic snapshotData)
    {
        try
        {
            // 載入基本資料
            if (snapshotData.ApplicationBasic != null)
            {
                var basicData = JsonConvert.DeserializeObject<OFS_CLB_Application_Basic>(
                    snapshotData.ApplicationBasic.ToString()
                );
                if (basicData != null)
                {
                    PopulateBasicData(basicData);
                }
            }

            // 載入人員資料
            if (snapshotData.ApplicationPersonnel != null)
            {
                var personnelList = JsonConvert.DeserializeObject<List<OFS_CLB_Application_Personnel>>(
                    snapshotData.ApplicationPersonnel.ToString()
                );
                if (personnelList != null && personnelList.Count > 0)
                {
                    PopulatePersonnelData(personnelList);
                }
            }

            // 載入計畫資訊
            if (snapshotData.ApplicationPlan != null)
            {
                var planData = JsonConvert.DeserializeObject<OFS_CLB_Application_Plan>(
                    snapshotData.ApplicationPlan.ToString()
                );
                if (planData != null)
                {
                    PopulatePlanData(planData);
                }
            }


            // 載入檔案上傳狀態
            if (snapshotData.UploadFile != null)
            {
                var uploadFiles = JsonConvert.DeserializeObject<List<OFS_CLB_UploadFile>>(
                    snapshotData.UploadFile.ToString()
                );
                if (uploadFiles != null && uploadFiles.Count > 0)
                {
                    PopulateUploadFileData(uploadFiles);
                }
            }

            // 載入其他補助資料
            if (snapshotData.OtherSubsidy != null)
            {
                var otherSubsidyList = JsonConvert.DeserializeObject<List<OFS_CLB_Other_Subsidy>>(
                    snapshotData.OtherSubsidy.ToString()
                );
                if (otherSubsidyList != null && otherSubsidyList.Count > 0)
                {
                    PopulateOtherSubsidyData(otherSubsidyList);
                }
            }

            // 載入經費預算規劃資料
            if (snapshotData.BudgetPlan != null)
            {
                var budgetPlanList = JsonConvert.DeserializeObject<List<OFS_CLB_Budget_Plan>>(
                    snapshotData.BudgetPlan.ToString()
                );
                if (budgetPlanList != null && budgetPlanList.Count > 0)
                {
                    PopulateBudgetPlanData(budgetPlanList);
                }
            }

            // 載入已獲補助資料
            if (snapshotData.ReceivedSubsidy != null)
            {
                var receivedSubsidyList = JsonConvert.DeserializeObject<List<OFS_CLB_Received_Subsidy>>(
                    snapshotData.ReceivedSubsidy.ToString()
                );
                if (receivedSubsidyList != null && receivedSubsidyList.Count > 0)
                {
                    PopulateReceivedSubsidyData(receivedSubsidyList);
                }
            }

            // 設定為檢視模式
            IsReadOnly = true;
            SetReadOnlyMode();
            bool enableUploadStep = true;
            InitializeTabSystem(enableUploadStep, "application", IsReadOnly);
            // 隱藏變更說明控制項（快照檢視不需要）
            if (changeDescriptionSection != null)
            {
                changeDescriptionSection.Visible = false;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"從快照載入資料時發生錯誤: {ex.Message}");
            throw new Exception($"從快照載入資料時發生錯誤：{ex.Message}");
        }
    }

    /// <summary>
    /// 填入基本資料到表單
    /// </summary>
    private void PopulateBasicData(OFS_CLB_Application_Basic basicData)
    {
        lblProjectID.Text = basicData.ProjectID;
        hidProjectID.Value = basicData.ProjectID;
        txtProjectNameTw.Text = basicData.ProjectNameTw;

        // 設定申請補助類型
        switch (basicData.SubsidyType)
        {
            case "Startup":
                rbSubsidyTypeCreate.Checked = true;
                break;
            case "Admin":
                rbSubsidyTypeOperation.Checked = true;
                break;
            case "Public":
                rbSubsidyTypeActivity.Checked = true;
                break;
        }

        txtSchoolName.Text = basicData.SchoolName;
        txtClubName.Text = basicData.ClubName;
        txtSchoolIDNumber.Text = basicData.School_IDNumber;
        txtAddress.Text = basicData.Address;

        // 設定成立日期
        if (basicData.CreationDate.HasValue)
        {
            txtCreationDate.Text = basicData.CreationDate.Value.ToMinguoDate();
            txtCreationDate.Attributes["data-gregorian-date"] = basicData.CreationDate.Value.ToString("yyyy/MM/dd");
        }

        // 載入經費資料到前端 (使用 JavaScript)
        string applyAmount = basicData.ApplyAmount?.ToString("N0") ?? "";
        string selfAmount = basicData.SelfAmount?.ToString("N0") ?? "";
        bool isPreviouslySubsidized = basicData.IsPreviouslySubsidized ?? false;

        string script = $@"
            if (typeof loadFundsData === 'function') {{
                loadFundsData({{
                    applyAmount: '{applyAmount}',
                    selfAmount: '{selfAmount}',
                    isPreviouslySubsidized: {isPreviouslySubsidized.ToString().ToLower()}
                }});
            }}
        ";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "PopulateBasicDataFunds", script, true);
    }

    /// <summary>
    /// 填入人員資料到表單
    /// </summary>
    private void PopulatePersonnelData(List<OFS_CLB_Application_Personnel> personnelList)
    {
        foreach (var personnel in personnelList)
        {
            switch (personnel.Personnel)
            {
                case "社團指導老師":
                    txtTeacherName.Text = personnel.Name;
                    txtTeacherJobTitle.Text = personnel.JobTitle;
                    txtTeacherPhone.Text = personnel.PhoneNum;
                    break;
                case "社團業務聯絡人":
                    txtContactName.Text = personnel.Name;
                    txtContactJobTitle.Text = personnel.JobTitle;
                    txtContactPhone.Text = personnel.PhoneNum;
                    break;
            }
        }
    }

    /// <summary>
    /// 填入計畫資訊到表單
    /// </summary>
    private void PopulatePlanData(OFS_CLB_Application_Plan planData)
    {
        // 設定執行期間
        if (planData.StartDate.HasValue)
        {
            txtStartDate.Text = planData.StartDate.Value.ToMinguoDate();
            txtStartDate.Attributes["data-gregorian-date"] = planData.StartDate.Value.ToString("yyyy/MM/dd");
        }
        if (planData.EndDate.HasValue)
        {
            txtEndDate.Text = planData.EndDate.Value.ToMinguoDate();
            txtEndDate.Attributes["data-gregorian-date"] = planData.EndDate.Value.ToString("yyyy/MM/dd");
        }

        txtPurpose.Text = planData.Purpose;
        txtPlanContent.Text = planData.PlanContent;
        txtPreBenefits.Text = planData.PreBenefits;
        txtPlanLocation.Text = planData.PlanLocation;
        txtEstimatedPeople.Text = planData.EstimatedPeople;
        txtEmergencyPlan.Text = planData.EmergencyPlan;
    }

   
    /// <summary>
    /// 填入上傳檔案資料到表單
    /// </summary>
    private void PopulateUploadFileData(List<OFS_CLB_UploadFile> uploadFiles)
    {
        // 按 FileCode 分組，以便判斷是否為第一筆（需要清空）
        var groupedFiles = uploadFiles.GroupBy(f => f.FileCode);

        foreach (var group in groupedFiles)
        {
            bool isFirst = true;
            foreach (var file in group)
            {
                // 第一筆清空面板，後續追加（支援 FILE_CLB4 多檔）
                UpdateFileStatusUI(file.FileCode, file.FileName, file.TemplatePath, file.ID, isFirst);
                isFirst = false;
            }
        }
    }

    /// <summary>
    /// 填入其他補助資料到表單
    /// </summary>
    private void PopulateOtherSubsidyData(List<OFS_CLB_Other_Subsidy> otherSubsidyList)
    {
        if (otherSubsidyList != null && otherSubsidyList.Count > 0)
        {
            // 將其他補助資料轉換為 JSON 並傳遞給前端
            string otherSubsidyJson = JsonConvert.SerializeObject(otherSubsidyList.Select(s => new
            {
                ID = s.ID,
                Unit = s.Unit,
                Amount = s.Amount,
                Content = s.Content
            }).ToList());

            string script = $@"
                if (typeof loadOtherSubsidyData === 'function') {{
                    loadOtherSubsidyData({otherSubsidyJson});
                }}
            ";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "PopulateOtherSubsidyData", script, true);
        }
    }

    /// <summary>
    /// 填入經費預算規劃資料到表單
    /// </summary>
    private void PopulateBudgetPlanData(List<OFS_CLB_Budget_Plan> budgetPlanList)
    {
        if (budgetPlanList != null && budgetPlanList.Count > 0)
        {
            // 將經費預算規劃資料轉換為 JSON 並傳遞給前端
            string budgetPlanJson = JsonConvert.SerializeObject(budgetPlanList.Select(b => new
            {
                ID = b.ID,
                Title = b.Title,
                Amount = b.Amount,
                OtherAmount = b.OtherAmount,
                Description = b.Description
            }).ToList());

            string script = $@"
                if (typeof loadBudgetPlanData === 'function') {{
                    loadBudgetPlanData({budgetPlanJson});
                }}
            ";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "PopulateBudgetPlanData", script, true);
        }
    }

    /// <summary>
    /// 填入已獲補助資料到表單
    /// </summary>
    private void PopulateReceivedSubsidyData(List<OFS_CLB_Received_Subsidy> receivedSubsidyList)
    {
        if (receivedSubsidyList != null && receivedSubsidyList.Count > 0)
        {
            // 將已獲補助資料轉換為 JSON 並傳遞給前端
            string receivedSubsidyJson = JsonConvert.SerializeObject(receivedSubsidyList.Select(r => new
            {
                ID = r.ID,
                ProjectName = r.Name,
                Amount = r.Amount
            }).ToList());

            string script = $@"
                if (typeof loadFundingDescriptionData === 'function') {{
                    loadFundingDescriptionData({receivedSubsidyJson});
                }}
            ";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "PopulateReceivedSubsidyData", script, true);
        }
    }

    /// <summary>
    /// 根據版本取得合併 PDF 的 FileCode（社團專用）
    /// </summary>
    /// <param name="version">版本名稱（送審版或核定版）</param>
    /// <returns>對應的 FileCode</returns>
    private string GetMergedFileCodeForClb(string version)
    {
        switch (version)
        {
            case "送審版":
                return "MERGED_REVIEW_VERSION";
            case "核定版":
                return "MERGED_APPROVED_VERSION";
            default:
                return "MERGED_UNKNOWN";
        }
    }

    /// <summary>
    /// 取得目前登入使用者的帳號
    /// </summary>
    /// <returns>使用者帳號，若無法取得則回傳 "未知使用者"</returns>
    private string GetCurrentUserAccount()
    {
        try
        {
            var userInfo = SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo);
            return userInfo?.Account ?? "未知使用者";
        }
        catch
        {
            return "未知使用者";
        }
    }

    /// <summary>
    /// 驗證字數是否超過限制
    /// </summary>
    /// <param name="formData">表單資料</param>
    /// <returns>錯誤訊息列表，如果沒有錯誤則回傳空列表</returns>
    private List<string> ValidateCharCount(Dictionary<string, object> formData)
    {
        List<string> errors = new List<string>();
        int maxLength = 1000;

        // 定義需要驗證的欄位
        var fieldsToValidate = new Dictionary<string, string>
        {
            { "Purpose", "目的" },
            { "PlanContent", "計畫內容" },
            { "PreBenefits", "預期效益" }
        };

        foreach (var field in fieldsToValidate)
        {
            if (formData.ContainsKey(field.Key) && formData[field.Key] != null)
            {
                string value = formData[field.Key].ToString();
                if (value.Length > maxLength)
                {
                    errors.Add($"{field.Value}欄位字數超過限制（{value.Length}/{maxLength}字）");
                }
            }
        }

        return errors;
    }

    #endregion
}