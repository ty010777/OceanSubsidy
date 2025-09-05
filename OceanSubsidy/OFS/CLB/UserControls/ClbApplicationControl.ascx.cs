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

public partial class OFS_CLB_UserControls_ClbApplicationControl : System.Web.UI.UserControl
{
    public string ProjectID { get; set; }
    public bool IsReadOnly { get; set; } = false;


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            InitializeControl();
            
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

    private void InitializeControl()
    {
        // 設定年度
        
        lblYear.Text = DateTimeHelper.GregorianYearToMinguo(DateTime.Now.Year).ToString();
        hidYear.Value = DateTimeHelper.GregorianYearToMinguo(DateTime.Now.Year).ToString();
        
        // 如果是唯讀模式，設定控制項狀態
        if (IsReadOnly)
        {
            SetReadOnlyMode();
        }
    }



    private void SetReadOnlyMode()
    {
        // 設定所有控制項為唯讀
        txtProjectNameTw.ReadOnly = true;
        rbSubsidyTypeCreate.Enabled = false;
        rbSubsidyTypeOperation.Enabled = false;
        rbSubsidyTypeActivity.Enabled = false;
        txtSchoolName.ReadOnly = true;
        txtSchoolIDNumber.ReadOnly = true;
        txtClubName.ReadOnly = true;
        txtAddress.ReadOnly = true;
        txtCreationDate.ReadOnly = true;
        txtStartDate.ReadOnly = true;
        txtEndDate.ReadOnly = true;
        txtPurpose.ReadOnly = true;
        txtPlanContent.ReadOnly = true;
        txtPreBenefits.ReadOnly = true;
        txtPlanLocation.ReadOnly = true;
        txtEstimatedPeople.ReadOnly = true;
        txtEmergencyPlan.ReadOnly = true;
        txtSubsidyFunds.ReadOnly = true;
        txtSelfFunds.ReadOnly = true;
        txtOtherGovFunds.ReadOnly = true;
        txtOtherUnitFunds.ReadOnly = true;
        rbPreviouslySubsidizedYes.Enabled = false;
        rbPreviouslySubsidizedNo.Enabled = false;
        txtFundingDescription.ReadOnly = true;
        
        // 人員欄位
        txtTeacherName.ReadOnly = true;
        txtTeacherJobTitle.ReadOnly = true;
        txtTeacherPhone.ReadOnly = true;
        txtContactName.ReadOnly = true;
        txtContactJobTitle.ReadOnly = true;
        txtContactPhone.ReadOnly = true;
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
                Statuses = isTempSave ? "尚未提送" : "尚未提送", // 暫存和提送都是"尚未提送"
                StatusesName = isTempSave ? "編輯中" : "編輯中", // 暫存和提送都是"編輯中"
                UserAccount = userInfo?.Account ?? "",
                UserName = userInfo?.UserName ?? "",
                UserOrg = userInfo?.UnitName ?? "",
                CurrentStep = isTempSave ? "1" :"2",// 暫存 是 1 和提送 後是2
                isWithdrawal = false,
                isExist = true
            };

            // 呼叫 Helper 儲存 Project_Main 資訊
            OFS_ClbApplicationHelper.SaveProjectMainData(projectMainData);
        }
        catch (Exception ex)
        {
            throw new Exception($"儲存 Project_Main 資訊失敗：{ex.Message}");
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

 
    #region 文件上傳相關方法

    /// <summary>
    /// 處理文件上傳
    /// </summary>
    /// <param name="fileCode">文件代碼</param>
    /// <param name="uploadedFile">上傳的文件</param>
    public string HandleFileUpload(string projectID,string fileCode, HttpPostedFile uploadedFile)
    {
        try
        {
            // 驗證文件
            string validationResult = ValidateUploadedFile(uploadedFile);
            if (!string.IsNullOrEmpty(validationResult))
            {
                return validationResult;
            }

            // 確保有 ProjectID
            if (string.IsNullOrEmpty(projectID))
            {
                return "請先儲存申請表再上傳附件";
            }

            // 產生檔案名稱
            string attachmentName = GetAttachmentNameByFileCode(fileCode);
            string fileName = $"{projectID}_{attachmentName}.pdf";

            // 上傳檔案到指定路徑
            string relativePath = SaveUploadedFile(uploadedFile, fileName, fileCode);
            if (string.IsNullOrEmpty(relativePath))
            {
                return "檔案上傳失敗";
            }

            // 儲存到資料庫
            SaveFileToDatabase(projectID, fileCode, fileName, relativePath);

            // 更新UI顯示
            UpdateFileStatusUI(fileCode, fileName, relativePath);

            return ""; // 成功時返回空字符串
        }
        catch (Exception ex)
        {
            return $"上傳失敗：{ex.Message}";
        }
    }

    /// <summary>
    /// 驗證上傳的文件
    /// </summary>
    private string ValidateUploadedFile(HttpPostedFile file)
    {
        if (file == null || file.ContentLength == 0)
        {
            return "請選擇要上傳的檔案";
        }

        // 檢查檔案類型
        if (!file.FileName.ToLower().EndsWith(".pdf"))
        {
            return "僅支援PDF格式檔案";
        }

        // 檢查檔案大小 (10MB)
        int maxSize = 10 * 1024 * 1024; // 10MB
        if (file.ContentLength > maxSize)
        {
            return "檔案大小不能超過10MB";
        }

        return ""; // 驗證通過返回空字符串
    }

    /// <summary>
    /// 根據 FileCode 取得附件名稱
    /// </summary>
    private string GetAttachmentNameByFileCode(string fileCode)
    {
        switch (fileCode)
        {
            case "FILE_CLB1":
                return "申請表";
            case "FILE_CLB2":
                return "計畫書";
            case "FILE_CLB3":
                return "未違反公職人員利益衝突迴避法切結書及事前揭露表";
            case "FILE_CLB4":
                return "相關佐證資料";
            default:
                return "附件";
        }
    }

    /// <summary>
    /// 儲存上傳的檔案
    /// </summary>
    private string SaveUploadedFile(HttpPostedFile file, string fileName, string fileCode)
    {
        try
        {
            // 建立上傳目錄路徑
            string uploadDir = Server.MapPath("~/UploadFiles/CLB/");
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            // 完整檔案路徑
            string fullPath = Path.Combine(uploadDir, fileName);
            
            // 如果檔案已存在，先刪除
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            // 儲存檔案
            file.SaveAs(fullPath);

            // 回傳相對路徑
            return $"~/UploadFiles/CLB/{fileName}";
        }
        catch (Exception ex)
        {
            throw new Exception($"檔案儲存失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 儲存檔案資訊到資料庫
    /// </summary>
    private void SaveFileToDatabase(string projectID, string fileCode, string fileName, string relativePath)
    {
        try
        {
            var uploadFile = new OFS_CLB_UploadFile
            {
                ProjectID = projectID,
                FileCode = fileCode,
                FileName = fileName,
                TemplatePath = relativePath
            };

            // 檢查是否已存在同樣的記錄
            OFS_ClbApplicationHelper.DeleteUploadFile(projectID, fileCode);
            
            // 插入新記錄
            OFS_ClbApplicationHelper.InsertUploadFile(uploadFile);
        }
        catch (Exception ex)
        {
            throw new Exception($"儲存檔案資訊失敗：{ex.Message}");
        }
    }

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

    /// <summary>
    /// 刪除上傳的檔案
    /// </summary>
    public string DeleteUploadedFile(string fileCode)
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return "計畫編號不存在";
            }

            // 從資料庫刪除
            OFS_ClbApplicationHelper.DeleteUploadFile(ProjectID, fileCode);

            // 刪除實際檔案
            var fileInfo = OFS_ClbApplicationHelper.GetUploadedFile(ProjectID, fileCode);
            if (fileInfo != null && !string.IsNullOrEmpty(fileInfo.TemplatePath))
            {
                string physicalPath = Server.MapPath(fileInfo.TemplatePath);
                if (File.Exists(physicalPath))
                {
                    File.Delete(physicalPath);
                }
            }

            // 更新UI
            ResetFileStatusUI(fileCode);

            return ""; // 成功
        }
        catch (Exception ex)
        {
            return $"刪除失敗：{ex.Message}";
        }
    }

    /// <summary>
    /// 重置檔案狀態UI
    /// </summary>
    private void ResetFileStatusUI(string fileCode)
    {
        Label statusLabel = null;
        Panel filesPanel = null;

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
            statusLabel.Text = "未上傳";
            statusLabel.CssClass = "text-muted";
            filesPanel.Visible = false;
            filesPanel.Controls.Clear();
        }
    }

    #endregion
}