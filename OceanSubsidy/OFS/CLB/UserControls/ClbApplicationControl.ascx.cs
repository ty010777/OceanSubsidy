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
        // 設定年度

        lblYear.Text = DateTimeHelper.GregorianYearToMinguo(DateTime.Now.Year).ToString();
        hidYear.Value = DateTimeHelper.GregorianYearToMinguo(DateTime.Now.Year).ToString();

        // 初始化變更說明控制項
        InitializeChangeDescriptionControl();

        // 如果是檢視模式，設定控制項狀態
        if (IsReadOnly)
        {
            SetReadOnlyMode();
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
        txtSubsidyFunds.Enabled = false;
        txtSelfFunds.Enabled = false;
        txtOtherGovFunds.Enabled = false;
        txtOtherUnitFunds.Enabled = false;
        rbPreviouslySubsidizedYes.Enabled = false;
        rbPreviouslySubsidizedNo.Enabled = false;
        txtFundingDescription.Enabled = false;
        
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

                // 為所有有 view-mode class 的元素添加 d-none
                $('.view-mode').addClass('d-none');

                // 隱藏按鈕
                $('#btnTempSave, #btnSaveAndNext, #btnSubmitApplication').hide();
            });
        ";
        
        Page.ClientScript.RegisterStartupScript(this.GetType(), "SetViewModeStyles", script, true);
    }

    public bool ValidateData()
    {
        List<string> errors = new List<string>();

        // 驗證必填欄位
        if (string.IsNullOrEmpty(txtProjectNameTw.Text.Trim()))
            errors.Add("請輸入計畫名稱");

        if (!rbSubsidyTypeCreate.Checked && !rbSubsidyTypeOperation.Checked && !rbSubsidyTypeActivity.Checked)
            errors.Add("請選擇申請補助類型");

        if (string.IsNullOrEmpty(txtSchoolName.Text.Trim()))
            errors.Add("請輸入學校名稱");

        if (string.IsNullOrEmpty(txtClubName.Text.Trim()))
            errors.Add("請輸入社團全名");

        if (string.IsNullOrEmpty(txtSchoolIDNumber.Text.Trim()))
            errors.Add("請輸入學校統一編號");

        if (string.IsNullOrEmpty(txtAddress.Text.Trim()))
            errors.Add("請輸入地址");

        if (string.IsNullOrEmpty(txtStartDate.Text.Trim()) || string.IsNullOrEmpty(txtEndDate.Text.Trim()))
            errors.Add("請輸入計畫執行期間");

        if (string.IsNullOrEmpty(txtPurpose.Text.Trim()))
            errors.Add("請輸入計畫目的");

        if (string.IsNullOrEmpty(txtPlanContent.Text.Trim()))
            errors.Add("請輸入計畫內容");

        if (string.IsNullOrEmpty(txtPreBenefits.Text.Trim()))
            errors.Add("請輸入預期效益");

        if (string.IsNullOrEmpty(txtSubsidyFunds.Text.Trim()))
            errors.Add("請輸入申請補助金額");

        // 人員必填驗證
        if (string.IsNullOrEmpty(txtTeacherName.Text.Trim()))
            errors.Add("請輸入社團指導老師姓名");
            
        if (string.IsNullOrEmpty(txtContactName.Text.Trim()))
            errors.Add("請輸入社團業務聯絡人姓名");

        if (errors.Count > 0)
        {
            string errorMessage = "以下欄位為必填：\\n" + string.Join("\\n", errors);
            Page.ClientScript.RegisterStartupScript(this.GetType(), "ValidationError", 
                $"alert('{errorMessage}');", true);
            return false;
        }

        return true;
    }

    public Dictionary<string, object> GetFormData()
    {
        var data = new Dictionary<string, object>();

        // 基本資料
        data["ProjectID"] = lblProjectID.Text;
        data["Year"] = int.Parse(hidYear.Value);
        data["SubsidyPlanType"] = hidSubsidyPlanType.Value;
        data["ProjectNameTw"] = txtProjectNameTw.Text.Trim();
        
        // 取得選中的申請補助類型
        string subsidyType = "";
        if (rbSubsidyTypeCreate.Checked) subsidyType = "Startup";
        else if (rbSubsidyTypeOperation.Checked) subsidyType = "Admin";
        else if (rbSubsidyTypeActivity.Checked) subsidyType = "Public";
        data["SubsidyType"] = subsidyType;
        
        data["SchoolName"] = txtSchoolName.Text.Trim();
        data["School_IDNumber"] = txtSchoolIDNumber.Text.Trim();
        data["ClubName"] = txtClubName.Text.Trim();
        data["Address"] = txtAddress.Text.Trim();
        
        // 成立日期 - 從 data attribute 取得西元日期
        string creationDateValue = Request.Form[txtCreationDate.UniqueID + "_gregorian"] ?? txtCreationDate.Attributes["data-gregorian-date"];
        if (!string.IsNullOrEmpty(creationDateValue))
            data["CreationDate"] = DateTime.Parse(creationDateValue);

        // 計畫資料 - 從 data attribute 取得西元日期
        string startDateValue = Request.Form[txtStartDate.UniqueID + "_gregorian"] ?? txtStartDate.Attributes["data-gregorian-date"];
        string endDateValue = Request.Form[txtEndDate.UniqueID + "_gregorian"] ?? txtEndDate.Attributes["data-gregorian-date"];
        
        if (!string.IsNullOrEmpty(startDateValue))
            data["StartDate"] = DateTime.Parse(startDateValue);
        if (!string.IsNullOrEmpty(endDateValue))
            data["EndDate"] = DateTime.Parse(endDateValue);
            
        data["Purpose"] = txtPurpose.Text.Trim();
        data["PlanContent"] = txtPlanContent.Text.Trim();
        data["PreBenefits"] = txtPreBenefits.Text.Trim();
        data["PlanLocation"] = txtPlanLocation.Text.Trim();
        data["EstimatedPeople"] = txtEstimatedPeople.Text.Trim();
        data["EmergencyPlan"] = txtEmergencyPlan.Text.Trim();

        // 經費資料
        if (!string.IsNullOrEmpty(txtSubsidyFunds.Text.Trim()))
            data["SubsidyFunds"] = decimal.Parse(txtSubsidyFunds.Text);
        if (!string.IsNullOrEmpty(txtSelfFunds.Text.Trim()))
            data["SelfFunds"] = decimal.Parse(txtSelfFunds.Text);
        if (!string.IsNullOrEmpty(txtOtherGovFunds.Text.Trim()))
            data["OtherGovFunds"] = decimal.Parse(txtOtherGovFunds.Text);
        if (!string.IsNullOrEmpty(txtOtherUnitFunds.Text.Trim()))
            data["OtherUnitFunds"] = decimal.Parse(txtOtherUnitFunds.Text);
            
        data["PreviouslySubsidized"] = rbPreviouslySubsidizedYes.Checked;
        data["FundingDescription"] = txtFundingDescription.Text.Trim();

        // 人員資料
        data["TeacherName"] = txtTeacherName.Text.Trim();
        data["TeacherJobTitle"] = txtTeacherJobTitle.Text.Trim();
        data["TeacherPhone"] = txtTeacherPhone.Text.Trim();
        data["ContactName"] = txtContactName.Text.Trim();
        data["ContactJobTitle"] = txtContactJobTitle.Text.Trim();
        data["ContactPhone"] = txtContactPhone.Text.Trim();

        return data;
    }

    /// <summary>
    /// 儲存基本資料
    /// </summary>
    /// <param name="isTempSave">是否為暫存（true=暫存，false=正式儲存）</param>
    /// <returns>成功儲存的 ProjectID</returns>
    public string SaveBasicData(bool isTempSave = false)
    {
        try
        {
            // 取得表單資料（不進行檢核）
            var formData = GetFormData();
            
            // 建立基本資料物件
            var basicData = new OFS_CLB_Application_Basic
            {
                ProjectID = lblProjectID.Text, // 如果是新建會是空的，Helper會自動產生
                Year = int.Parse(hidYear.Value),
                SubsidyPlanType = formData["SubsidyPlanType"]?.ToString(),
                ProjectNameTw = formData["ProjectNameTw"]?.ToString(),
                SubsidyType = formData["SubsidyType"]?.ToString(),
                SchoolName = formData["SchoolName"]?.ToString(),
                ClubName = formData["ClubName"]?.ToString(),
                CreationDate = formData.ContainsKey("CreationDate") ? (DateTime?)formData["CreationDate"] : null,
                School_IDNumber = formData["School_IDNumber"]?.ToString(),
                Address = formData["Address"]?.ToString()
            };

            // 儲存資料並取得 ProjectID
            string savedProjectID = SaveBasicData(basicData);
            
            // 儲存人員表單
            SavePersonnelData(savedProjectID, formData);
            
            // 儲存計畫資訊
            SavePlanData(savedProjectID, formData);
            
            // 儲存經費資訊
            SaveFundsData(savedProjectID, formData);
            
            // 儲存 Project_Main 資訊
            SaveProjectMainData(savedProjectID, formData, isTempSave);
            // 更新畫面上的 ProjectID 顯示
            lblProjectID.Text = savedProjectID;
            hidProjectID.Value = savedProjectID;
            ProjectID = savedProjectID;

            return savedProjectID;
        }
        catch (Exception ex)
        {
            // 錯誤處理
            throw new Exception($"儲存基本資料失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 儲存人員資料
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="formData">表單資料</param>
    private void SavePersonnelData(string projectID, Dictionary<string, object> formData)
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
        }
        catch (Exception ex)
        {
            throw new Exception($"儲存人員資料失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 儲存計畫資訊
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="formData">表單資料</param>
    private void SavePlanData(string projectID, Dictionary<string, object> formData)
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
        }
        catch (Exception ex)
        {
            throw new Exception($"儲存計畫資訊失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 儲存經費資訊
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <param name="formData">表單資料</param>
    private void SaveFundsData(string projectID, Dictionary<string, object> formData)
    {
        try
        {
            var fundsData = new OFS_CLB_Application_Funds
            {
                ProjectID = projectID,
                SubsidyFunds = formData.ContainsKey("SubsidyFunds") ? (decimal?)formData["SubsidyFunds"] : null,
                SelfFunds = formData.ContainsKey("SelfFunds") ? (decimal?)formData["SelfFunds"] : null,
                OtherGovFunds = formData.ContainsKey("OtherGovFunds") ? (decimal?)formData["OtherGovFunds"] : null,
                OtherUnitFunds = formData.ContainsKey("OtherUnitFunds") ? (decimal?)formData["OtherUnitFunds"] : null,
                PreviouslySubsidized = formData.ContainsKey("PreviouslySubsidized") ? (bool?)formData["PreviouslySubsidized"] : null,
                FundingDescription = formData["FundingDescription"]?.ToString()
            };

            // 總經費由前端 JavaScript calculateTotalFunds() 自動計算並顯示
            // 這裡直接從表單取得已計算好的總經費值
            if (!string.IsNullOrEmpty(lblTotalFunds.Text) && decimal.TryParse(lblTotalFunds.Text, out decimal calculatedTotal))
            {
                fundsData.TotalFunds = calculatedTotal;
            }

            // 呼叫 Helper 儲存經費資訊
            OFS_ClbApplicationHelper.SaveFundsData(fundsData);
        }
        catch (Exception ex)
        {
            throw new Exception($"儲存經費資訊失敗：{ex.Message}");
        }
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
            var fundsData = OFS_ClbApplicationHelper.GetFundsData(projectID);
            
            if (fundsData != null)
            {
                // 設定經費欄位
                txtSubsidyFunds.Text = fundsData.SubsidyFunds?.ToString("0");
                txtSelfFunds.Text = fundsData.SelfFunds?.ToString("0");
                txtOtherGovFunds.Text = fundsData.OtherGovFunds?.ToString("0");
                txtOtherUnitFunds.Text = fundsData.OtherUnitFunds?.ToString("0");
                
                // 設定計畫總經費
                lblTotalFunds.Text = fundsData.TotalFunds?.ToString("0") ?? "0";
                
                // 設定曾申請政府補助
                if (fundsData.PreviouslySubsidized.HasValue)
                {
                    rbPreviouslySubsidizedYes.Checked = fundsData.PreviouslySubsidized.Value;
                    rbPreviouslySubsidizedNo.Checked = !fundsData.PreviouslySubsidized.Value;
                }
                
                txtFundingDescription.Text = fundsData.FundingDescription;
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
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"載入 Project_Main 資訊失敗：{ex.Message}");
        }
    }

 
    // #region 文件上傳相關方法
    // /// <summary>
    // /// 儲存上傳的檔案
    // /// </summary>
    // private string SaveUploadedFile(HttpPostedFile file, string fileName, string fileCode, string projectID)
    // {
    //     try
    //     {
    //         // 建立上傳目錄路徑 (使用正確的 OFS/CLB/{projectID} 格式)
    //         string uploadDir = Server.MapPath($"~/UploadFiles/OFS/CLB/{projectID}");
    //         if (!Directory.Exists(uploadDir))
    //         {
    //             Directory.CreateDirectory(uploadDir);
    //         }
    //
    //         // 完整檔案路徑
    //         string fullPath = Path.Combine(uploadDir, fileName);
    //         
    //         // 如果檔案已存在，先刪除
    //         if (File.Exists(fullPath))
    //         {
    //             File.Delete(fullPath);
    //         }
    //
    //         // 儲存檔案
    //         file.SaveAs(fullPath);
    //
    //         // 回傳相對路徑
    //         return $"~/UploadFiles/OFS/CLB/{projectID}/{fileName}";
    //     }
    //     catch (Exception ex)
    //     {
    //         throw new Exception($"檔案儲存失敗：{ex.Message}");
    //     }
    // }

    // /// <summary>
    // /// 儲存檔案資訊到資料庫
    // /// </summary>
    // private void SaveFileToDatabase(string projectID, string fileCode, string fileName, string relativePath)
    // {
    //     try
    //     {
    //         var uploadFile = new OFS_CLB_UploadFile
    //         {
    //             ProjectID = projectID,
    //             FileCode = fileCode,
    //             FileName = fileName,
    //             TemplatePath = relativePath
    //         };
    //
    //         // 檢查是否已存在同樣的記錄
    //         OFS_ClbApplicationHelper.DeleteUploadFile(projectID, fileCode);
    //         
    //         // 插入新記錄
    //         OFS_ClbApplicationHelper.InsertUploadFile(uploadFile);
    //     }
    //     catch (Exception ex)
    //     {
    //         throw new Exception($"儲存檔案資訊失敗：{ex.Message}");
    //     }
    // }

    /// <summary>
    /// 更新檔案狀態UI
    /// </summary>
    private void UpdateFileStatusUI(string fileCode, string fileName, string relativePath)
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
            filesPanel.Controls.Clear();

            // 建立主要的檔案標籤容器
            var fileTag = new System.Web.UI.HtmlControls.HtmlGenericControl("span");
            fileTag.Attributes["class"] = "tag tag-green-light";

            // 建立下載連結
            var downloadLink = new System.Web.UI.HtmlControls.HtmlGenericControl("a");
            downloadLink.Attributes["class"] = "tag-link";
            downloadLink.Attributes["href"] = "#";
            downloadLink.Attributes["onclick"] = $"downloadUploadedFile('{fileCode}'); return false;";
            downloadLink.Attributes["target"] = "_blank";
            downloadLink.InnerText = fileName;

            // 建立刪除按鈕
            var deleteButton = new System.Web.UI.HtmlControls.HtmlGenericControl("button");
            deleteButton.Attributes["type"] = "button";
            deleteButton.Attributes["class"] = "tag-btn";
            deleteButton.Attributes["onclick"] = $"deleteFile('{fileCode}')";
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
            
            foreach (var file in uploadedFiles)
            {
                UpdateFileStatusUI(file.FileCode, file.FileName, file.TemplatePath);
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
        try
        {
            // 取得表單資料
            var formData = GetFormDataFromRequest();

            // 執行暫存（傳入 true 表示暫存）
            string projectID = SaveBasicDataAjax(formData, true);

            if (!string.IsNullOrEmpty(projectID))
            {
                Response.Write($"{{\"success\":true,\"message\":\"暫存成功！\",\"projectID\":\"{projectID}\"}}");
            }
            else
            {
                Response.Write("{\"success\":false,\"message\":\"儲存失敗，請檢查必填欄位\"}");
            }
        }
        catch (Exception ex)
        {
            Response.Write($"{{\"success\":false,\"message\":\"暫存失敗：{ex.Message}\"}}");
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
                    // 決定審核 修正計畫書
                    OFS_ClbApplicationHelper.UpdateProjectStatus(projectID, "決審核定", "計畫書審核中", "3");

                    // 產生核定版 PDF
                    MergePdfFiles(projectID, ProjectName,"核定版");

                }
                //這是計畫變更
                else if(lastProjectMain.Statuses== "計畫執行")
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
                            NotificationHelper.G2("社團", projectName, "計畫變更申請", organizer);
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


                }
                //提送歷史紀錄
                InsertClbHistory(projectID);

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

        // 計畫變更版本管理 - 檢查是否存在計畫變更最新版並建立版本備份
        CreatePlanChangeVersionBackup(projectID, projectName);

        // 產生並合併送審版與核定版 PDF
        MergePdfFiles(projectID, projectName, "計畫變更最新版");
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

            // 如果沒有檔案可以合併，直接返回
            if (pdfFilePaths.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("沒有找到任何可合併的 PDF 檔案");
                return;
            }

            // 建立合併後的檔案名稱和路徑
            string mergedFileName = $"{projectId}_社團_{ProjectName}_{version}.pdf";
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
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"合併 PDF 檔案時發生錯誤：{ex.Message}");
            throw;
        }
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
        data["SubsidyPlanType"] = Request.Form["subsidyPlanType"] ?? "學校社團";
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
        if (decimal.TryParse(Request.Form["subsidyFunds"], out decimal subsidyFunds))
            data["SubsidyFunds"] = subsidyFunds;
        if (decimal.TryParse(Request.Form["selfFunds"], out decimal selfFunds))
            data["SelfFunds"] = selfFunds;
        if (decimal.TryParse(Request.Form["otherGovFunds"], out decimal otherGovFunds))
            data["OtherGovFunds"] = otherGovFunds;
        if (decimal.TryParse(Request.Form["otherUnitFunds"], out decimal otherUnitFunds))
            data["OtherUnitFunds"] = otherUnitFunds;

        data["PreviouslySubsidized"] = Request.Form["previouslySubsidized"] == "true";
        data["FundingDescription"] = Request.Form["fundingDescription"] ?? "";

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
        try
        {
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
                Address = formData["Address"]?.ToString()
            };

            // 儲存資料並取得 ProjectID
            string savedProjectID = SaveBasicData(basicData);

            // 儲存人員表單
            SavePersonnelDataAjax(savedProjectID, formData);

            // 儲存計畫資訊
            SavePlanDataAjax(savedProjectID, formData);

            // 儲存經費資訊
            SaveFundsDataAjax(savedProjectID, formData);
            
            // 儲存 Project_Main 資訊
            
            SaveProjectMainData(savedProjectID, formData, isTempSave);
            
            //儲存變更說明
            SaveProjectChangeRecord(savedProjectID, formData);
            return savedProjectID;
        }
        catch (Exception ex)
        {
            throw new Exception($"儲存基本資料失敗：{ex.Message}");
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
        }
        catch (Exception ex)
        {
            throw new Exception($"儲存人員資料失敗：{ex.Message}");
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
        }
        catch (Exception ex)
        {
            throw new Exception($"儲存計畫資訊失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 儲存經費資訊（AJAX 版本）
    /// </summary>
    private void SaveFundsDataAjax(string projectID, Dictionary<string, object> formData)
    {
        try
        {
            var fundsData = new OFS_CLB_Application_Funds
            {
                ProjectID = projectID,
                SubsidyFunds = formData.ContainsKey("SubsidyFunds") ? (decimal?)formData["SubsidyFunds"] : null,
                SelfFunds = formData.ContainsKey("SelfFunds") ? (decimal?)formData["SelfFunds"] : null,
                OtherGovFunds = formData.ContainsKey("OtherGovFunds") ? (decimal?)formData["OtherGovFunds"] : null,
                OtherUnitFunds = formData.ContainsKey("OtherUnitFunds") ? (decimal?)formData["OtherUnitFunds"] : null,
                PreviouslySubsidized = formData.ContainsKey("PreviouslySubsidized") ? (bool?)formData["PreviouslySubsidized"] : null,
                FundingDescription = formData["FundingDescription"]?.ToString()
            };

            // 計算總經費
            decimal totalFunds = (fundsData.SubsidyFunds ?? 0) + (fundsData.SelfFunds ?? 0) +
                               (fundsData.OtherGovFunds ?? 0) + (fundsData.OtherUnitFunds ?? 0);
            fundsData.TotalFunds = totalFunds;

            // 呼叫 Helper 儲存經費資訊
            OFS_ClbApplicationHelper.SaveFundsData(fundsData);
        }
        catch (Exception ex)
        {
            throw new Exception($"儲存經費資訊失敗：{ex.Message}");
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

        // 經費必填欄位（所有類型都需要）
        if (!formData.ContainsKey("SubsidyFunds") || formData["SubsidyFunds"] == null)
            errors.Add("請輸入申請海委會補助金額");

        if (!formData.ContainsKey("SelfFunds") || formData["SelfFunds"] == null)
            errors.Add("請輸入自籌款金額");

        if (!formData.ContainsKey("OtherGovFunds") || formData["OtherGovFunds"] == null)
            errors.Add("請輸入其他政府補助金額");

        if (!formData.ContainsKey("OtherUnitFunds") || formData["OtherUnitFunds"] == null)
            errors.Add("請輸入其他單位補助金額");

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
        
        try
        {
            // 如果有 ProjectID，查詢 CurrentStep
            if (!string.IsNullOrEmpty(projectID))
            {
                string currentStep = OFS_ClbApplicationHelper.GetProjectCurrentStep(projectID);

                if (!string.IsNullOrEmpty(currentStep) && int.TryParse(currentStep, out int stepNumber))
                {
                    // 控制暫存按鈕：CurrentStep > 1 時隱藏
                    showTempSaveButton = stepNumber <= 1;

                    // 控制上傳附件步驟：CurrentStep >= 2 時開放
                    enableUploadStep = stepNumber >= 2;
                }
            }

            // 設定按鈕可見性（使用 JavaScript）
            string buttonScript = $@"
                $(document).ready(function() {{
                    if ({showTempSaveButton.ToString().ToLower()}) {{
                        $('#btnTempSave').show();
                    }} else {{
                        $('#btnTempSave').hide();
                    }}
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
    
    private void snapshot(string Project )
    {
        

        // OFSSnapshotHelper.insert(new Snapshot
        // {
        //     Type = "CLB",
        //     DataID = Project,
        //     Status = project.Status,
        //     Data = JsonConvert.SerializeObject(new
        //     {
        //         Project = project,
        //         Contacts = OFS_EdcContactHelper.query(id),
        //         ReceivedSubsidies = OFS_EdcReceivedSubsidyHelper.query(id),
        //         Attachments = OFS_EdcAttachmentHelper.query(id)
        //     })
        // });
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

    /// <summary>
    /// 驗證變更說明資料
    /// </summary>
    /// <returns>驗證結果</returns>
    public ValidationResult ValidateChangeDescription()
    {
        var result = new ValidationResult();

        try
        {
            if (!changeDescriptionSection.Visible)
            {
                return result;
            }

            string changeBefore = txtChangeBefore.Text?.Trim();
            string changeAfter = txtChangeAfter.Text?.Trim();

            if (!string.IsNullOrEmpty(changeBefore) || !string.IsNullOrEmpty(changeAfter))
            {
                if (string.IsNullOrEmpty(changeBefore))
                {
                    result.AddError("請填寫變更前的內容");
                }

                if (string.IsNullOrEmpty(changeAfter))
                {
                    result.AddError("請填寫變更後的內容");
                }
            }
        }
        catch (Exception ex)
        {
            result.AddError($"驗證變更說明時發生錯誤：{ex.Message}");
        }

        return result;
    }

    /// <summary>
    /// 插入變更記錄到資料庫
    /// </summary>
    /// <param name="projectID">計畫ID</param>
    /// <param name="reason">變更原因</param>
    /// <returns>是否成功</returns>
    public bool InsertChangeRecord(string projectID, string reason)
    {
        try
        {
            if (string.IsNullOrEmpty(projectID) || string.IsNullOrEmpty(reason))
            {
                return false;
            }

            // 呼叫 OFSProjectChangeRecordHelper.insert 插入變更記錄
            OFSProjectChangeRecordHelper.insert(new ProjectChangeRecord
            {
                Type = "CLB",
                Method = 1,
                DataID = projectID,
                Reason = reason
            });
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"插入變更記錄時發生錯誤：{ex.Message}");
            return false;
        }
    }

    #endregion
}