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
    protected string ProjectID => Request.QueryString["ProjectID"];

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

    #region 按鈕事件

    /// <summary>
    /// 按鈕點擊事件處理
    /// </summary>
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            Button btnClicked = (Button)sender;
            bool isTemporarySave = btnClicked.ID == "tab1_btnTempSave";


            HandleSaveOperation(this.ProjectID, isTemporarySave);

        }
        catch (Exception ex)
        {
            HandleException(ex, "按鈕點擊事件處理時發生錯誤");
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
            
            // 確保下拉選單已初始化
            LoadDropDownLists();
            if (!string.IsNullOrEmpty(projectID))
            {
                // 檢查表單狀態並隱藏暫存按鈕（如果已完成）
                CheckFormStatusAndHideTempSaveButton();
                LoadExistingData(projectID);
                // 載入變更說明資料到輸入框
                SetViewMode();
                // 載入變更說明控制項
                tab1_ucChangeDescription.LoadData(ProjectID, IsViewMode);

            }
            else
            {
                LoadDefaultData();
            }
            

        }
        catch (Exception ex)
        {
            HandleException(ex, "載入資料時發生錯誤");
        }
    }

    private void SetViewMode()
    {
        if (IsViewMode)
        {
            string script = @"
            document.addEventListener('DOMContentLoaded', function() {
                // 只針對此 UserControl 內的元素進行鎖定
                // 找到 tab1 容器（申請表/聲明書）
                var userControl = document.querySelector('#tab1');

                if (!userControl) {
                    console.warn('找不到 UserControl 容器: #tab1');
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
            });
        ";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "AddViewModeStyles", script, true);
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
            var applicationData = GetApplicationDataFromForm(ProjectID);
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

    /// <summary>
    /// 隱藏暫存按鈕
    /// </summary>
    /// <summary>
    /// 檢查表單狀態並隱藏暫存按鈕（如果已完成）
    /// </summary>
    private void CheckFormStatusAndHideTempSaveButton()
    {
        try
        {
            if (!string.IsNullOrEmpty(ProjectID))
            {
                var formStatus = OFS_SciWorkSchHelper.GetFormStatusByProjectID(ProjectID, "Form1Status");

                if (formStatus == "完成")
                {
                    // 通知 UserControl 隱藏暫存按鈕
                    tab1_btnTempSave.Style["display"] = "none";

                }
            }
        }
        catch (Exception ex)
        {
            // 發生錯誤時不隱藏按鈕，讓用戶正常使用
            System.Diagnostics.Debug.WriteLine($"檢查表單狀態失敗: {ex.Message}");
        }
    }
    #endregion

    #region 私有方法


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
            //TODO 改成從GrantType取資料
            txtYear.Text = DateTimeHelper.GregorianYearToMinguo(DateTime.Now.Year).ToString();
            txtSubsidyPlanType.Text = "科專（114年度補助學術機構、研究機關(構)及海洋科技業者執行海洋科技專案）";
            
            // 初始化空的關鍵字欄位
            KeywordsData = new List<OFS_SCI_Application_KeyWord>();
            PopulateKeywordsData(KeywordsData);
            tab1_ucChangeDescription.Visible = false;
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
            KeywordsData = OFS_SciApplicationHelper.GetKeywordsByID(ApplicationData?.ProjectID);
            
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
        txtKeywordID.Text = data.ProjectID;
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
    private OFS_SCI_Application_Main GetApplicationDataFromForm(string ProjectID)
    {
        OFS_SCI_Application_Main applicationData;

        // 如果 ProjectID 不為空，先從資料庫取得現有資料
        if (!string.IsNullOrEmpty(ProjectID))
        {
            applicationData = OFS_SciApplicationHelper.getApplicationMainByProjectID(ProjectID);

            // 如果沒有找到現有資料，建立新的物件
            if (applicationData == null)
            {
                applicationData = new OFS_SCI_Application_Main();
            }
        }
        else
        {
            // ProjectID 為空，建立新的物件
            applicationData = new OFS_SCI_Application_Main();
        }

        // 將表單的值填入物件中
        applicationData.ProjectID = txtProjectID.Text;
        applicationData.PersonID = txtPersonID.Text;
        applicationData.Year = int.TryParse(txtYear.Text, out int year) ? year : DateTimeHelper.GregorianYearToMinguo(DateTime.Now.Year);
        applicationData.SubsidyPlanType = txtSubsidyPlanType.Text;
        applicationData.ProjectNameTw = txtProjectNameCh.Text.Trim();
        applicationData.ProjectNameEn = txtProjectNameEn.Text.Trim();
        applicationData.OrgCategory = ddlApplicationType.SelectedValue;
        applicationData.Topic = ddlTopic.SelectedValue;
        applicationData.Field = ddlField.SelectedValue;
        applicationData.CountryTech_Underwater = GetRadioButtonValue("Underwater");
        applicationData.CountryTech_Geology = GetRadioButtonValue("Marine");
        applicationData.CountryTech_Physics = GetRadioButtonValue("Physics");
        applicationData.OrgName = txtOrgName.Text.Trim();
        applicationData.RegisteredAddress = txtRegisteredAddress.Text.Trim();
        applicationData.CorrespondenceAddress = txtCorrespondenceAddress.Text.Trim();
        applicationData.Target = txtTarget.Text.Trim();
        applicationData.Summary = txtSummary.Text.Trim();
        applicationData.Innovation = txtInnovation.Text.Trim();
        applicationData.Declaration = ChkAgreeTerms.Checked;

        return applicationData;
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
                            KeywordID = txtProjectID.Text,
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
        string newProjectID = GenerateNewProjectID(applicationData);
        var user = SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo);
        OFS_SCI_Project_Main newProject = new OFS_SCI_Project_Main
        {
            ProjectID = newProjectID,
            Statuses = "尚未提送",
            StatusesName = "編輯中",
            UserAccount = user.Account,
            UserName = user.UserName,
            UserOrg = user.UnitName,
            
        };
        string newPersonID = "P"+newProjectID;
        string newKeywordID = newProjectID;

        applicationData.ProjectID = newProjectID;
        applicationData.PersonID = newPersonID;
        // KeywordID 不再需要設定，直接使用 ProjectID

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
        OFS_SciApplicationHelper.InsertOFS_SCIProjectMain(newProject);

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
        OFS_SciApplicationHelper.SaveKeywordsToDatabase(applicationData.ProjectID, keywordsData);
    }

    /// <summary>
    /// 產生新的計畫編號
    /// </summary>
    private string GenerateNewProjectID(OFS_SCI_Application_Main applicationData)
    {
        try
        {
            int currentYear = DateTimeHelper.GregorianYearToMinguo(DateTime.Now.Year);
            var latestApplication = OFS_SciApplicationHelper.getLatestApplicationMain(currentYear.ToString());

            int nextSerial = 1;
            if (latestApplication != null && !string.IsNullOrEmpty(latestApplication.Serial))
            {
                if (int.TryParse(latestApplication.Serial, out int currentSerial))
                {
                    nextSerial = currentSerial + 1;
                }
            }

            applicationData.Serial = nextSerial.ToString();
            return $"SCI{currentYear}{nextSerial:D4}";
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
            try
            {
                // 解析關鍵字資料
                var serializer = new JavaScriptSerializer();
                var keywordsArray = serializer.Deserialize<dynamic[]>(hiddenKeywordsData.Value);
                var keywordsJson = serializer.Serialize(keywordsArray);

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

                Page.ClientScript.RegisterStartupScript(this.GetType(), "RestoreKeywords", script, true);
            }
            catch (Exception ex)
            {
                HandleException(ex, "還原關鍵字資料時發生錯誤");
            }
        }
    }

    /// <summary>
    /// 處理儲存操作
    /// </summary>
    private void HandleSaveOperation(string projectID, bool isTemporarySave)
    {
        try
        {
            // 先將變更說明的值存到 Session，以便 PostBack 後還原
            SaveChangeDescriptionToSession();
            // 讓key PostBack 後還原
            RestoreKeywordsAfterPostBack();
            RestoreChangeDescriptionFromSession();

            // 如果是檢視模式，不允許儲存
            if (IsViewMode)
            {
                ShowWarningMessage("目前為檢視模式，無法執行儲存操作");
                return;
            }

            // 驗證表單資料
            if (!isTemporarySave)
            {
                var validationResult = ValidateForm();
                if (!validationResult.IsValid)
                {
                    ShowErrorMessage(validationResult.GetErrorsAsString());
                    return;
                }
            }

            // 儲存資料
            string resultProjectID = SaveData(projectID);

            // 儲存變更說明
            tab1_ucChangeDescription.SaveChangeDescription(resultProjectID);

            // 更新版本狀態
            UpdateVersionStatusBasedOnAction(resultProjectID, !isTemporarySave);

            if (isTemporarySave)
            {
                ShowSuccessMessage("資料已暫存");

                // 如果是新建案件，更新URL中的ProjectID
                if (string.IsNullOrEmpty(ProjectID) && !string.IsNullOrEmpty(resultProjectID))
                {
                    Response.Redirect($"SciApplication.aspx?ProjectID={resultProjectID}");
                }
            }
            else
            {
                // 判斷當前頁面是否為 SciInprogress_Approved.aspx
                string currentPage = System.IO.Path.GetFileName(Request.Url.AbsolutePath);
                string redirectUrl = currentPage != "SciInprogress_Approved.aspx"
                    ? $"SciWorkSch.aspx?ProjectID={resultProjectID}"
                    : "";

                // 顯示成功訊息（如果有 URL 則 1 秒後跳轉）
                ShowSuccessMessage("儲存成功", redirectUrl);
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"儲存失敗：{ex.Message}");
            HandleException(ex, "儲存時發生錯誤");
        }
    }

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

    /// <summary>
    /// 判斷是否應該顯示為編輯模式
    /// </summary>
    /// <returns>true: 編輯模式, false: 檢視模式</returns>
    private bool ShouldShowInEditMode()
    {
        // 如果沒有 ProjectID，是新申請案件，可以編輯
        if (string.IsNullOrEmpty(ProjectID))
        {
            return true;
        }
        
        try
        {
            // 取得最新版本的狀態
            var projectData = OFS_SciApplicationHelper.getVersionByProjectID(ProjectID);
            if (projectData == null)
            {
                return true; // 沒有資料時允許編輯
            }
            
            // 只有這些狀態可以編輯
            string statuses = projectData.Statuses ?? "";
            string statusesName = projectData.StatusesName ?? "";
            
            return statuses == "尚未提送" || 
                   statusesName == "補正補件" || 
                   statusesName == "計畫書修正中";
        }
        catch (Exception ex)
        {
            HandleException(ex, "取得申請狀態時發生錯誤");
            return false; // 發生錯誤時預設為檢視模式
        }
    }
 /// <summary>
    /// 根據動作類型更新版本狀態
    /// </summary>
    /// <param name="projectID">計畫ID</param>
    /// <param name="isComplete">是否為完成動作（下一步）</param>
    private void UpdateVersionStatusBasedOnAction(string projectID, bool isComplete)
    {
        try
        {
            
            if (isComplete)
            {
                // 點擊「完成本頁，下一步」按鈕
                // 1. Form1Status 設為 "完成" 
                // 2. 檢查 CurrentStep，如果 <= 1 則改成 2
                
                var projectData = OFS_SciApplicationHelper.getVersionByProjectID(projectID);
                if (projectData != null)
                {
                    int res;
                    int.TryParse(projectData.CurrentStep,out res); 
                    bool shouldUpdateCurrentStep =  res <= 1;
                    string newCurrentStep = shouldUpdateCurrentStep ? "2" : projectData.CurrentStep;
                    
                    // 更新專案版本狀態
                    var updateData = new OFS_SCI_Project_Main
                    {
                        ProjectID = projectID,
                        Form1Status = "完成",
                        CurrentStep = newCurrentStep,
                        updated_at = DateTime.Now
                    };
                    
                    OFS_SciApplicationHelper.UpdateOFS_SCIVersion(updateData);
                }
            }
            else
            {
                // 點擊「暫存」按鈕
                // 只更新 Form1Status 為 "暫存"，CurrentStep 不變
                var updateData = new OFS_SCI_Project_Main
                {
                    ProjectID = projectID,
                    Form1Status = "暫存",
                    CurrentStep = "1", // 保持在第一步
                    updated_at = DateTime.Now,
                    
                    
                };
                
                OFS_SciApplicationHelper.UpdateOFS_SCIVersion(updateData);
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "更新版本狀態時發生錯誤");
        }
    }
    // /// <summary>
    // /// 載入變更說明資料到輸入框
    // /// </summary>
    // private void LoadChangeDescriptionData()
    // {
    //     try
    //     {
    //         if (!string.IsNullOrEmpty(ProjectID))
    //         {
    //             // 從資料庫取得變更說明並設定到頁面元素
    //             var changeDescription = OFS_SciApplicationHelper.GetPageModifyNote(ProjectID, "SciApplication");
    //             if (changeDescription != null)
    //             {
    //                 string script = $@"
    //                     setTimeout(function() {{
    //                         const changeBeforeElement = document.getElementById('txtChangeBefore');
    //                         if (changeBeforeElement && '{changeDescription.ChangeBefore?.Replace("'", "\\'")}') {{
    //                             changeBeforeElement.textContent = '{changeDescription.ChangeBefore?.Replace("'", "\\'")}';
    //                         }}
    //                         
    //                         const changeAfterElement = document.getElementById('txtChangeAfter');
    //                         if (changeAfterElement && '{changeDescription.ChangeAfter?.Replace("'", "\\'")}') {{
    //                             changeAfterElement.textContent = '{changeDescription.ChangeAfter?.Replace("'", "\\'")}';
    //                         }}
    //                     }}, 100);
    //                 ";
    //                 Page.ClientScript.RegisterStartupScript(this.GetType(), "LoadChangeDescription", script, true);
    //             }
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         HandleException(ex, "載入變更說明資料時發生錯誤");
    //     }
    // }
    /// <summary>
    /// 例外處理
    /// </summary>
    private void HandleException(Exception ex, string context)
    {
        System.Diagnostics.Debug.WriteLine($"{context}: {ex.Message}");
        // 可以在這裡加入記錄或通知邏輯
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
                return tab1_ucChangeDescription.GetChangeDescriptionBySourcePage(ProjectID, "SciApplication");
            }
            return ("", "");
        }
        catch (Exception ex)
        {
            HandleException(ex, "取得變更說明資料時發生錯誤");
            return ("", "");
        }
    }

    /// <summary>
    /// 將變更說明的值存到 Session
    /// </summary>
    private void SaveChangeDescriptionToSession()
    {
        try
        {
            string sessionKey = $"ChangeDescription_tab1_{ProjectID}";
            var changeData = new Dictionary<string, string>
            {
                { "ChangeBefore", tab1_ucChangeDescription.ChangeBefore },
                { "ChangeAfter", tab1_ucChangeDescription.ChangeAfter }
            };
            Session[sessionKey] = changeData;
            System.Diagnostics.Debug.WriteLine($"已將 tab1 變更說明存入 Session: {sessionKey}");
        }
        catch (Exception ex)
        {
            HandleException(ex, "儲存變更說明到 Session 時發生錯誤");
        }
    }

    /// <summary>
    /// 從 Session 還原變更說明的值
    /// </summary>
    private void RestoreChangeDescriptionFromSession()
    {
        try
        {
            string sessionKey = $"ChangeDescription_tab1_{ProjectID}";
            if (Session[sessionKey] is Dictionary<string, string> changeData)
            {
                string changeBefore = changeData.ContainsKey("ChangeBefore") ? changeData["ChangeBefore"] : "";
                string changeAfter = changeData.ContainsKey("ChangeAfter") ? changeData["ChangeAfter"] : "";

                // 設定到 ChangeDescriptionControl
                tab1_ucChangeDescription.ChangeBefore = changeBefore;
                tab1_ucChangeDescription.ChangeAfter = changeAfter;

                System.Diagnostics.Debug.WriteLine($"已從 Session 還原 tab1 變更說明: {sessionKey}");
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "從 Session 還原變更說明時發生錯誤");
        }
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
        return string.Join("<br>", Errors);
    }

}
