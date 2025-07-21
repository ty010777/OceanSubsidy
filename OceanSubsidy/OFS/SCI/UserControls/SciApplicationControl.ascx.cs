using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Operation.OFS;
using GS.App;

/// <summary>
/// 科專計畫申請表 UserControl
/// 可重複使用的申請表內容元件
/// </summary>
public partial class OFS_SCI_UserControls_SciApplicationControl : System.Web.UI.UserControl
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

    /// <summary>
    /// 申請資料
    /// </summary>
    public OFS_SCI_Application_Main ApplicationData { get; private set; }

    /// <summary>
    /// 人員資料
    /// </summary>
    public List<OFS_SCI_Application_Personnel> PersonnelData { get; private set; }

    /// <summary>
    /// 關鍵字資料
    /// </summary>
    public List<OFS_SCI_Application_KeyWord> KeywordsData { get; private set; }

    /// <summary>
    /// 需要設定的下拉選單值（延遲到 PreRender 設定）
    /// </summary>
    private Dictionary<DropDownList, string> _pendingDropDownValues = new Dictionary<DropDownList, string>();

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
            else
            {
                RestoreKeywordsAfterPostBack();
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "UserControl 載入時發生錯誤");
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        try
        {
            // 在 PreRender 階段設定下拉選單的選中值
            foreach (var kvp in _pendingDropDownValues)
            {
                var ddl = kvp.Key;
                var value = kvp.Value;
                
                if (!string.IsNullOrEmpty(value))
                {
                    var item = ddl.Items.FindByValue(value);
                    if (item != null)
                    {
                        ddl.SelectedValue = value;
                    }
                }
            }
            _pendingDropDownValues.Clear();
        }
        catch (Exception ex)
        {
            HandleException(ex, "設定下拉選單值時發生錯誤");
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

            // 確保下拉選單已初始化
            LoadDropDownLists();

            if (!string.IsNullOrEmpty(projectID))
            {
                LoadExistingData(projectID);
            }
            else
            {
                LoadDefaultData();
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
            // 檢查必填欄位
            if (string.IsNullOrWhiteSpace(txtProjectNameCh.Text))
            {
                result.AddError("請輸入計畫名稱(中文)");
            }

            if (ddlApplicationType.SelectedIndex <= 0)
            {
                result.AddError("請選擇申請類別");
            }

            if (ddlTopic.SelectedIndex <= 0)
            {
                result.AddError("請選擇主題");
            }

            if (ddlField.SelectedIndex <= 0)
            {
                result.AddError("請選擇領域");
            }

            // 檢查國家核心科技
            if (!IsNationalCoreTechSelected())
            {
                result.AddError("請選擇是否屬於國家核心科技");
            }

            if (string.IsNullOrWhiteSpace(txtOrgName.Text))
            {
                result.AddError("請輸入申請單位");
            }

            if (string.IsNullOrWhiteSpace(txtRegisteredAddress.Text))
            {
                result.AddError("請輸入登記地址");
            }

            if (string.IsNullOrWhiteSpace(txtCorrespondenceAddress.Text))
            {
                result.AddError("請輸入通訊地址");
            }

            // 檢查人員資料
            if (string.IsNullOrWhiteSpace(txtPIName.Text))
            {
                result.AddError("請輸入計畫主持人姓名");
            }

            if (string.IsNullOrWhiteSpace(txtPIJobTitle.Text))
            {
                result.AddError("請輸入計畫主持人職稱");
            }

            if (string.IsNullOrWhiteSpace(txtPIMobile.Text))
            {
                result.AddError("請輸入計畫主持人手機號碼");
            }

            if (string.IsNullOrWhiteSpace(txtContactName.Text))
            {
                result.AddError("請輸入計畫聯絡人姓名");
            }

            if (string.IsNullOrWhiteSpace(txtContactJobTitle.Text))
            {
                result.AddError("請輸入計畫聯絡人職稱");
            }

            if (string.IsNullOrWhiteSpace(txtContactMobile.Text))
            {
                result.AddError("請輸入計畫聯絡人手機號碼");
            }

            // 檢查計畫內容
            if (string.IsNullOrWhiteSpace(txtTarget.Text))
            {
                result.AddError("請輸入計畫目標");
            }

            if (string.IsNullOrWhiteSpace(txtSummary.Text))
            {
                result.AddError("請輸入計畫內容摘要");
            }

            if (string.IsNullOrWhiteSpace(txtInnovation.Text))
            {
                result.AddError("請輸入計畫創新重點");
            }

            // 檢查關鍵字
            var keywords = GetKeywordsFromForm();
            if (keywords.Count < 3)
            {
                result.AddError("請至少輸入3組關鍵字");
            }

            // 檢查聲明書
            if (!ChkAgreeTerms.Checked)
            {
                result.AddError("請閱讀並同意聲明書內容");
            }
        }
        catch (Exception ex)
        {
            result.AddError($"驗證過程發生錯誤：{ex.Message}");
        }

        return result;
    }

    /// <summary>
    /// 儲存表單資料
    /// </summary>
    /// <returns>儲存後的計畫ID</returns>
    public string SaveData(string ProjectID)
    {
        try
        {
            // 取得表單資料
            var applicationData = GetApplicationDataFromForm();
            var personnelData = GetPersonnelDataFromForm();
            var keywordsData = GetKeywordsFromForm();

            string resultProjectID = ProjectID;

            // 判斷是新增還是更新
            if (string.IsNullOrEmpty(ProjectID))
            {
                // 新增案件
                resultProjectID = CreateNewApplication(applicationData, personnelData, keywordsData);
            }
            else
            {
                // 更新現有案件
                UpdateExistingApplication(applicationData, personnelData, keywordsData);
                resultProjectID = ProjectID;
            }

            return resultProjectID;
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
        LoadDropDownLists();
        // 不在這裡初始化關鍵字，等 LoadData() 處理
    }

    /// <summary>
    /// 載入下拉選單資料
    /// </summary>
    private void LoadDropDownLists()
    {
        try
        {
            // 載入申請類別
            var applicationTypes = OFS_SciApplicationHelper.GetSysZgsCodeByCodeGroup("SCIOrgCategory");
            BindDropDownList(ddlApplicationType, applicationTypes, "請選擇申請類別");

            // 載入主題
            var topics = OFS_SciApplicationHelper.GetSysZgsCodeByCodeGroup("SCITopic");
            BindDropDownList(ddlTopic, topics, "請選擇主題");

            // 載入領域
            var fields = OFS_SciApplicationHelper.GetSysZgsCodeByCodeGroup("SCIField");
            BindDropDownList(ddlField, fields, "請選擇領域");
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入下拉選單資料時發生錯誤");
        }
    }

    /// <summary>
    /// 綁定下拉選單
    /// </summary>
    private void BindDropDownList(DropDownList ddl, List<Sys_ZgsCode> dataSource, string defaultText)
    {
        ddl.Items.Clear();
        ddl.Items.Add(new ListItem(defaultText, ""));

        foreach (var item in dataSource)
        {
            ddl.Items.Add(new ListItem(item.Descname, item.Code));
        }
    }

    /// <summary>
    /// 載入預設資料
    /// </summary>
    private void LoadDefaultData()
    {
        try
        {
            txtYear.Text = DateTime.Now.Year.ToString();
            txtSubsidyPlanType.Text = "科專（114年度補助學術機構、研究機關(構)及海洋科技業者執行海洋科技專案）";
            
            // 初始化空的關鍵字欄位
            KeywordsData = new List<OFS_SCI_Application_KeyWord>();
            PopulateKeywordsData(KeywordsData);
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入預設資料時發生錯誤");
        }
    }

    /// <summary>
    /// 載入現有資料
    /// </summary>
    private void LoadExistingData(string projectID)
    {
        try
        {
            // 載入申請主檔
            ApplicationData = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);
            if (ApplicationData != null)
            {
                PopulateApplicationData(ApplicationData);
            }

            // 載入人員資料
            PersonnelData = OFS_SciApplicationHelper.GetPersonnelByPersonID(ApplicationData?.PersonID);
            if (PersonnelData != null && PersonnelData.Count > 0)
            {
                PopulatePersonnelData(PersonnelData);
            }

            // 載入關鍵字資料
            KeywordsData = OFS_SciApplicationHelper.GetKeywordsByID(ApplicationData?.KeywordID);
            
            // 不管有沒有資料都要傳送關鍵字，讓前端顯示 3 個欄位
            if (KeywordsData == null)
            {
                KeywordsData = new List<OFS_SCI_Application_KeyWord>();
            }
            PopulateKeywordsData(KeywordsData);
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入現有資料時發生錯誤");
        }
    }

    /// <summary>
    /// 填入申請資料到表單
    /// </summary>
    private void PopulateApplicationData(OFS_SCI_Application_Main data)
    {
        txtProjectID.Text = data.ProjectID;
        txtPersonID.Text = data.PersonID;
        txtKeywordID.Text = data.KeywordID;
        txtYear.Text = data.Year.ToString();
        txtProjectNameCh.Text = data.ProjectNameTw;
        txtProjectNameEn.Text = data.ProjectNameEn;
        
        if (!string.IsNullOrEmpty(data.OrgCategory))
        {
            SetDropDownValue(ddlApplicationType, data.OrgCategory);
        }
        
        if (!string.IsNullOrEmpty(data.Topic))
        {
            SetDropDownValue(ddlTopic, data.Topic);
        }
        
        if (!string.IsNullOrEmpty(data.Field))
        {
            SetDropDownValue(ddlField, data.Field);
        }

        // 設定國家核心科技選項
        if (data.CountryTech_Underwater.HasValue)
        {
            rbUnderwaterYes.Checked = data.CountryTech_Underwater.Value;
            rbUnderwaterNo.Checked = !data.CountryTech_Underwater.Value;
        }

        if (data.CountryTech_Geology.HasValue)
        {
            rbMarineYes.Checked = data.CountryTech_Geology.Value;
            rbMarineNo.Checked = !data.CountryTech_Geology.Value;
        }

        if (data.CountryTech_Physics.HasValue)
        {
            rbPhysicsYes.Checked = data.CountryTech_Physics.Value;
            rbPhysicsNo.Checked = !data.CountryTech_Physics.Value;
        }

        txtOrgName.Text = data.OrgName;
        txtRegisteredAddress.Text = data.RegisteredAddress;
        txtCorrespondenceAddress.Text = data.CorrespondenceAddress;
        txtTarget.Text = data.Target;
        txtSummary.Text = data.Summary;
        txtInnovation.Text = data.Innovation;
        
        if (data.Declaration.HasValue)
        {
            ChkAgreeTerms.Checked = data.Declaration.Value;
        }
    }

    /// <summary>
    /// 填入人員資料到表單
    /// </summary>
    private void PopulatePersonnelData(List<OFS_SCI_Application_Personnel> personnelList)
    {
        foreach (var person in personnelList)
        {
            switch (person.Role)
            {
                case "計畫主持人":
                    txtPIIdx.Text = person.idx.ToString();
                    txtPIName.Text = person.Name;
                    txtPIJobTitle.Text = person.JobTitle;
                    txtPIPhone.Text = person.Phone;
                    txtPIPhoneExt.Text = person.PhoneExt;
                    txtPIMobile.Text = person.MobilePhone;
                    break;
                case "計畫聯絡人":
                    txtContactIdx.Text = person.idx.ToString();
                    txtContactName.Text = person.Name;
                    txtContactJobTitle.Text = person.JobTitle;
                    txtContactPhone.Text = person.Phone;
                    txtContactPhoneExt.Text = person.PhoneExt;
                    txtContactMobile.Text = person.MobilePhone;
                    break;
                case "會計聯絡人":
                    txtAccountIdx.Text = person.idx.ToString();
                    txtAccountName.Text = person.Name;
                    txtAccountJobTitle.Text = person.JobTitle;
                    txtAccountPhone.Text = person.Phone;
                    txtAccountPhoneExt.Text = person.PhoneExt;
                    txtAccountMobile.Text = person.MobilePhone;
                    break;
            }
        }
    }

    /// <summary>
    /// 填入關鍵字資料到表單
    /// </summary>
    private void PopulateKeywordsData(List<OFS_SCI_Application_KeyWord> keywordsList)
    {
        // 使用實際的關鍵字數量，但至少保持 3 筆
        int keywordCount = Math.Max(3, keywordsList.Count);
        
        var finalKeywords = new object[keywordCount];
        
        // 填入實際資料
        for (int i = 0; i < keywordCount; i++)
        {
            if (i < keywordsList.Count)
            {
                finalKeywords[i] = new { 
                    KeyWordTw = keywordsList[i].KeyWordTw ?? "", 
                    KeyWordEn = keywordsList[i].KeyWordEn ?? "" 
                };
            }
            else
            {
                // 空的關鍵字行
                finalKeywords[i] = new { KeyWordTw = "", KeyWordEn = "" };
            }
        }

        var keywordsJson = new JavaScriptSerializer().Serialize(finalKeywords);
        hiddenKeywordsData.Value = keywordsJson;

        string script = $@"
            if (window.KeywordManager) {{
                window.KeywordManager.loadExistingKeywords({keywordsJson});
            }} else {{
                setTimeout(function() {{
                    if (window.KeywordManager) {{
                        window.KeywordManager.loadExistingKeywords({keywordsJson});
                    }}
                }}, 500);
            }}
        ";

        Page.ClientScript.RegisterStartupScript(this.GetType(), "LoadKeywords", script, true);
    }

    /// <summary>
    /// 設定下拉選單值（延遲到 PreRender 階段）
    /// </summary>
    private void SetDropDownValue(DropDownList ddl, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        // 儲存到待處理字典，在 PreRender 階段設定
        _pendingDropDownValues[ddl] = value;
    }

    /// <summary>
    /// 從表單取得申請資料
    /// </summary>
    private OFS_SCI_Application_Main GetApplicationDataFromForm()
    {
        return new OFS_SCI_Application_Main
        {
            ProjectID = txtProjectID.Text,
            PersonID = txtPersonID.Text,
            KeywordID = txtKeywordID.Text,
            Year = int.TryParse(txtYear.Text, out int year) ? year : DateTime.Now.Year,
            SubsidyPlanType = txtSubsidyPlanType.Text,
            ProjectNameTw = txtProjectNameCh.Text.Trim(),
            ProjectNameEn = txtProjectNameEn.Text.Trim(),
            OrgCategory = ddlApplicationType.SelectedValue,
            Topic = ddlTopic.SelectedValue,
            Field = ddlField.SelectedValue,
            CountryTech_Underwater = GetRadioButtonValue("Underwater"),
            CountryTech_Geology = GetRadioButtonValue("Marine"),
            CountryTech_Physics = GetRadioButtonValue("Physics"),
            OrgName = txtOrgName.Text.Trim(),
            RegisteredAddress = txtRegisteredAddress.Text.Trim(),
            CorrespondenceAddress = txtCorrespondenceAddress.Text.Trim(),
            Target = txtTarget.Text.Trim(),
            Summary = txtSummary.Text.Trim(),
            Innovation = txtInnovation.Text.Trim(),
            Declaration = ChkAgreeTerms.Checked
        };
    }

    /// <summary>
    /// 從表單取得人員資料
    /// </summary>
    private List<OFS_SCI_Application_Personnel> GetPersonnelDataFromForm()
    {
        var personnelList = new List<OFS_SCI_Application_Personnel>();

        // 計畫主持人
        personnelList.Add(new OFS_SCI_Application_Personnel
        {
            idx = int.TryParse(txtPIIdx.Text, out int piIdx) ? piIdx : 0,
            PersonID = txtPersonID.Text,
            Role = "計畫主持人",
            Name = txtPIName.Text.Trim(),
            JobTitle = txtPIJobTitle.Text.Trim(),
            Phone = txtPIPhone.Text.Trim(),
            PhoneExt = txtPIPhoneExt.Text.Trim(),
            MobilePhone = txtPIMobile.Text.Trim()
        });

        // 計畫聯絡人
        personnelList.Add(new OFS_SCI_Application_Personnel
        {
            idx = int.TryParse(txtContactIdx.Text, out int contactIdx) ? contactIdx : 0,
            PersonID = txtPersonID.Text,
            Role = "計畫聯絡人",
            Name = txtContactName.Text.Trim(),
            JobTitle = txtContactJobTitle.Text.Trim(),
            Phone = txtContactPhone.Text.Trim(),
            PhoneExt = txtContactPhoneExt.Text.Trim(),
            MobilePhone = txtContactMobile.Text.Trim()
        });

        // 會計聯絡人
        if (!string.IsNullOrWhiteSpace(txtAccountName.Text))
        {
            personnelList.Add(new OFS_SCI_Application_Personnel
            {
                idx = int.TryParse(txtAccountIdx.Text, out int accountIdx) ? accountIdx : 0,
                PersonID = txtPersonID.Text,
                Role = "會計聯絡人",
                Name = txtAccountName.Text.Trim(),
                JobTitle = txtAccountJobTitle.Text.Trim(),
                Phone = txtAccountPhone.Text.Trim(),
                PhoneExt = txtAccountPhoneExt.Text.Trim(),
                MobilePhone = txtAccountMobile.Text.Trim()
            });
        }

        return personnelList;
    }

    /// <summary>
    /// 從表單取得關鍵字資料
    /// </summary>
    private List<OFS_SCI_Application_KeyWord> GetKeywordsFromForm()
    {
        var keywordsList = new List<OFS_SCI_Application_KeyWord>();

        try
        {
            if (!string.IsNullOrEmpty(hiddenKeywordsData.Value))
            {
                var serializer = new JavaScriptSerializer();
                var keywordsArray = serializer.Deserialize<dynamic[]>(hiddenKeywordsData.Value);

                foreach (var item in keywordsArray)
                {
                    if (item != null)
                    {
                        keywordsList.Add(new OFS_SCI_Application_KeyWord
                        {
                            KeywordID = txtKeywordID.Text,
                            KeyWordTw = item["KeyWordTw"]?.ToString() ?? "",
                            KeyWordEn = item["KeyWordEn"]?.ToString() ?? ""
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "解析關鍵字資料時發生錯誤");
        }

        return keywordsList;
    }

    /// <summary>
    /// 取得無線電按鈕值
    /// </summary>
    private bool? GetRadioButtonValue(string groupName)
    {
        switch (groupName)
        {
            case "Underwater":
                if (rbUnderwaterYes.Checked) return true;
                if (rbUnderwaterNo.Checked) return false;
                break;
            case "Marine":
                if (rbMarineYes.Checked) return true;
                if (rbMarineNo.Checked) return false;
                break;
            case "Physics":
                if (rbPhysicsYes.Checked) return true;
                if (rbPhysicsNo.Checked) return false;
                break;
        }
        return null;
    }

    /// <summary>
    /// 檢查是否已選擇國家核心科技
    /// </summary>
    private bool IsNationalCoreTechSelected()
    {
        return (rbUnderwaterYes.Checked || rbUnderwaterNo.Checked) &&
               (rbMarineYes.Checked || rbMarineNo.Checked) &&
               (rbPhysicsYes.Checked || rbPhysicsNo.Checked);
    }

    /// <summary>
    /// 建立新申請案
    /// </summary>
    private string CreateNewApplication(OFS_SCI_Application_Main applicationData, 
        List<OFS_SCI_Application_Personnel> personnelData, 
        List<OFS_SCI_Application_KeyWord> keywordsData)
    {
        // 產生新的ID
        string newProjectID = GenerateNewProjectID();
        string newPersonID = Guid.NewGuid().ToString();
        string newKeywordID = Guid.NewGuid().ToString();

        applicationData.ProjectID = newProjectID;
        applicationData.PersonID = newPersonID;
        applicationData.KeywordID = newKeywordID;

        // 更新人員資料的PersonID
        foreach (var person in personnelData)
        {
            person.PersonID = newPersonID;
        }

        // 更新關鍵字資料的KeywordID
        foreach (var keyword in keywordsData)
        {
            keyword.KeywordID = newKeywordID;
        }

        // 儲存到資料庫
        OFS_SciApplicationHelper.insertApplicationMain(applicationData);
        OFS_SciApplicationHelper.SavePersonnel(personnelData);
        OFS_SciApplicationHelper.SaveKeywordsToDatabase(newKeywordID, keywordsData);

        return newProjectID;
    }

    /// <summary>
    /// 更新現有申請案
    /// </summary>
    private void UpdateExistingApplication(OFS_SCI_Application_Main applicationData,
        List<OFS_SCI_Application_Personnel> personnelData,
        List<OFS_SCI_Application_KeyWord> keywordsData)
    {
        // 更新主檔
        OFS_SciApplicationHelper.updateApplicationMain(applicationData);

        // 更新人員資料
        OFS_SciApplicationHelper.SavePersonnel(personnelData);

        // 更新關鍵字資料
        OFS_SciApplicationHelper.SaveKeywordsToDatabase(applicationData.KeywordID, keywordsData);
    }

    /// <summary>
    /// 產生新的計畫編號
    /// </summary>
    private string GenerateNewProjectID()
    {
        try
        {
            int currentYear = DateTime.Now.Year;
            var latestApplication = OFS_SciApplicationHelper.getLatestApplicationMain(currentYear.ToString());

            int nextSerial = 1;
            if (latestApplication != null && !string.IsNullOrEmpty(latestApplication.Serial))
            {
                if (int.TryParse(latestApplication.Serial, out int currentSerial))
                {
                    nextSerial = currentSerial + 1;
                }
            }

            return $"{currentYear}{nextSerial:D4}";
        }
        catch (Exception ex)
        {
            throw new Exception($"產生計畫編號時發生錯誤：{ex.Message}", ex);
        }
    }

    /// <summary>
    /// PostBack後恢復關鍵字資料
    /// </summary>
    private void RestoreKeywordsAfterPostBack()
    {
        if (!string.IsNullOrEmpty(hiddenKeywordsData.Value))
        {
            string script = $@"
                if (window.SciApplicationKeywords) {{
                    window.SciApplicationKeywords.restoreFromHidden();
                }}
            ";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "RestoreKeywords", script, true);
        }
    }

    /// <summary>
    /// 套用檢視模式
    /// </summary>
    private void ApplyViewMode()
    {
        if (IsViewMode)
        {
            DisableAllControls(this);
        }
    }

    /// <summary>
    /// 禁用所有控制項
    /// </summary>
    private void DisableAllControls(Control parent)
    {
        foreach (Control control in parent.Controls)
        {
            if (control is TextBox textBox)
                textBox.ReadOnly = true;
            else if (control is DropDownList dropDown)
                dropDown.Enabled = false;
            else if (control is CheckBox checkBox)
                checkBox.Enabled = false;
            else if (control is Button button)
                button.Enabled = false;
            else if (control is RadioButton radioButton)
                radioButton.Enabled = false;
            else if (control is HtmlInputGenericControl htmlInput)
                htmlInput.Disabled = true;
       

            if (control.HasControls())
                DisableAllControls(control);
        }
    }

    /// <summary>
    /// 例外處理
    /// </summary>
    private void HandleException(Exception ex, string context)
    {
        System.Diagnostics.Debug.WriteLine($"{context}: {ex.Message}");
        // 可以在這裡加入記錄或通知邏輯
    }

    #endregion
}

/// <summary>
/// 驗證結果類別
/// </summary>
public class ValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<string> Errors { get; private set; } = new List<string>();

    public void AddError(string error)
    {
        Errors.Add(error);
    }

    public string GetErrorsAsString()
    {
        return string.Join("\\n", Errors);
    }
}