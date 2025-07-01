using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// 科專計畫申請表頁面
/// 提供申請表的填寫、儲存、提交功能
/// </summary>
public partial class OFS_SciApplication : System.Web.UI.Page
{
    #region 私有欄位
     
    private string ProjectID => Request.QueryString["ProjectID"];
    private const int MIN_KEYWORD_COUNT = 3;
     
    #endregion

    #region 頁面事件

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                InitializePage();
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "頁面載入時發生錯誤");
        }
    }

    #endregion

     #region 頁面初始化

     /// <summary>
     /// 初始化頁面
     /// </summary>
     private void InitializePage()
     {
         LoadDropDownLists();
         LoadFormData();
     }

     /// <summary>
     /// 載入下拉選單資料
     /// </summary>
     private void LoadDropDownLists()
     {
         try
         {
             // 申請類別
             LoadDropDownList(ddlApplicationType, "SCIOrgCategory");
             
             // 主題分類
             LoadDropDownList(ddlTopic, "SCITopic");
             
             // 領域分類
             LoadDropDownList(ddlField, "SCIField");
         }
         catch (Exception ex)
         {
             throw new Exception("載入下拉選單資料時發生錯誤", ex);
         }
     }

     /// <summary>
     /// 載入特定下拉選單
     /// </summary>
     private void LoadDropDownList(DropDownList ddl, string codeGroup)
     {
         ddl.Items.Clear();
         var items = OFS_SciApplicationHelper.GetSysZgsCodeByCodeGroup(codeGroup);
         
         foreach (var item in items)
         {
             ddl.Items.Add(new ListItem(item.Descname, item.Code));
         }
     }

     /// <summary>
     /// 載入表單資料
     /// </summary>
     private void LoadFormData()
     {
         if (string.IsNullOrEmpty(ProjectID))
         {
             // 新建專案，設定預設值
             SetDefaultValues();
             return;
         }

         try
         {
             var lastVersion = OFS_SciApplicationHelper.getVersionLatestProjectID(ProjectID);

             // 載入現有專案資料
             var applicationData = OFS_SciApplicationHelper.getApplicationMainByVersion_ID(lastVersion.Version_ID);
             if (applicationData != null)
             {
                 PopulateMainData(applicationData);
                 LoadPersonnelData(applicationData.PersonID);
                 LoadKeywordsData(applicationData.KeywordID);
             }
         }
         catch (Exception ex)
         {
             throw new Exception("載入表單資料時發生錯誤", ex);
         }
     }

     /// <summary>
     /// 設定預設值
     /// </summary>
     private void SetDefaultValues()
     {
         txtYear.Text = (Convert.ToInt32(DateTime.Now.Year)-1911).ToString();
         txtSubsidyPlanType.Text = $"科專（{txtYear.Text}年度補助學術機構、研究機關(構)及海洋科技業者執行海洋科技專案）";
     }

     #endregion

     #region 資料填充
        
     /// <summary>
     /// 填充主要資料
     /// </summary>
     private void PopulateMainData(OFS_SCI_Application_Main data)
     {
         if (data == null) return;

         // 基本資料
         txtProjectID.Text = data.Version_ID;
         txtPersonID.Text = data.PersonID;
         txtKeywordID.Text = data.KeywordID;
         txtYear.Text = data.Year.ToString();
         txtSubsidyPlanType.Text = data.SubsidyPlanType;

         // 計畫資訊
         txtProjectNameCh.Text = data.ProjectNameTw;
         txtProjectNameEn.Text = data.ProjectNameEn;
         SetSelectedValue(ddlApplicationType, data.OrgCategory);
         SetSelectedValue(ddlTopic, data.Topic);
         SetSelectedValue(ddlField, data.Field);

         // 核心技術領域
         SetRadioButtonValue(rbUnderwaterYes, rbUnderwaterNo, data.CountryTech_Underwater);
         SetRadioButtonValue(rbMarineYes, rbMarineNo, data.CountryTech_Geology);
         SetRadioButtonValue(rbPhysicsYes, rbPhysicsNo, data.CountryTech_Physics);

         // 申請單位資訊
         txtOrgName.Text = data.OrgName;
         txtRegisteredAddress.Text = data.RegisteredAddress;
         txtCorrespondenceAddress.Text = data.CorrespondenceAddress;

         // 計畫內容
         txtTarget.Text = data.Target;
         txtSummary.Text = data.Summary;
         txtInnovation.Text = data.Innovation;
     }
    #endregion
     /// <summary>
     /// 設定下拉選單選中值
     /// </summary>
     private void SetSelectedValue(DropDownList ddl, string value)
     {
         if (!string.IsNullOrEmpty(value) && ddl.Items.FindByValue(value) != null)
         {
             ddl.SelectedValue = value;
         }
     }

     /// <summary>
     /// 設定單選按鈕值
     /// </summary>
     private void SetRadioButtonValue(HtmlInputRadioButton  rbYes, HtmlInputRadioButton  rbNo, bool? value)
     {
         if (value.HasValue)
         {
             rbYes.Checked = value.Value;
             rbNo.Checked = !value.Value;
         }
     }

     /// <summary>
     /// 載入人員資料
     /// </summary>
     private void LoadPersonnelData(string personID)
     {
         if (string.IsNullOrEmpty(personID)) return;

         try
         {
             var personnelData = OFS_SciApplicationHelper.GetPersonnelByPersonID(personID);
             
             foreach (var person in personnelData)
             {
                 switch (person.Role)
                 {
                     case "計畫主持人":
                         PopulatePersonnelFields(person, txtPIIdx, txtPIName, txtPIJobTitle, 
                                               txtPIPhone, txtPIPhoneExt, txtPIMobile);
                         break;
                     case "計畫聯絡人":
                         PopulatePersonnelFields(person, txtContactIdx, txtContactName, txtContactJobTitle, 
                                               txtContactPhone, txtContactPhoneExt, txtContactMobile);
                         break;
                     case "會計聯絡人":
                         PopulatePersonnelFields(person, txtAccountIdx, txtAccountName, txtAccountJobTitle, 
                                               txtAccountPhone, txtAccountPhoneExt, txtAccountMobile);
                         break;
                 }
             }
         }
         catch (Exception ex)
         {
             throw new Exception("載入人員資料時發生錯誤", ex);
         }
     }

     /// <summary>
     /// 填充人員欄位
     /// </summary>
     private void PopulatePersonnelFields(OFS_SCI_Application_Personnel person, 
         Label idxLabel, TextBox nameBox, TextBox jobTitleBox, 
         TextBox phoneBox, TextBox extBox, TextBox mobileBox)
     {
         idxLabel.Text = person.idx.ToString();
         nameBox.Text = person.Name;
         jobTitleBox.Text = person.JobTitle;
         phoneBox.Text = person.Phone;
         extBox.Text = person.PhoneExt;
         mobileBox.Text = person.MobilePhone;
     }

     #region 關鍵字處理

     /// <summary>
     /// 載入關鍵字資料
     /// </summary>
     private void LoadKeywordsData(string keywordID)
     {
         if (string.IsNullOrEmpty(keywordID)) 
         {
             // 即使沒有 KeywordID，也要初始化空的關鍵字表格
             RegisterKeywordLoadScript(new List<OFS_SCI_Application_KeyWord>());
             return;
         }

         try
         {
             var keywords = OFS_SciApplicationHelper.GetKeywordsByID(keywordID);
             
             if (keywords?.Count > 0)
             {
                 RegisterKeywordLoadScript(keywords);
             }
             else
             {
                 // 沒有找到關鍵字時，也要初始化空的表格
                 RegisterKeywordLoadScript(new List<OFS_SCI_Application_KeyWord>());
             }
         }
         catch (Exception ex)
         {
             // 出錯時也要確保有基本的關鍵字表格
             RegisterKeywordLoadScript(new List<OFS_SCI_Application_KeyWord>());
             throw new Exception("載入關鍵字資料時發生錯誤", ex);
         }
     }

     /// <summary>
     /// 註冊關鍵字載入腳本
     /// </summary>
     private void RegisterKeywordLoadScript(List<OFS_SCI_Application_KeyWord> keywords)
     {
         // 轉換為前端期望的格式
         var frontendData = keywords.Select(k => new {
             chinese = k.KeyWordTw ?? "",
             english = k.KeyWordEn ?? ""
         }).ToList();

         var serializer = new JavaScriptSerializer();
         string keywordsJson = serializer.Serialize(frontendData);
         
         string script = $@"
             window.addEventListener('load', function() {{
                 setTimeout(function() {{
                     console.log('Loading keywords data:', {keywordsJson});
                     if (window.KeywordManager && typeof window.KeywordManager.loadFromData === 'function') {{
                         window.KeywordManager.loadFromData({keywordsJson});
                     }}
                 }}, 200);
             }});
         ";
         
         ClientScript.RegisterStartupScript(this.GetType(), "LoadKeywords", script, true);
     }


     #endregion

     #region 表單提交處理

     /// <summary>
     /// 儲存按鈕點擊事件
     /// </summary>
     protected void btnSave_Click(object sender, EventArgs e)
     {
         try
         {
             var button = (Button)sender;
             var actionType = button.ID == "btnTempSave" ? FormActionType.TempSave : FormActionType.Submit;
             
             if (actionType == FormActionType.Submit && !ValidateForm(actionType))
             {
                 return; // 驗證失敗，不繼續處理
             }

             SaveFormData(actionType);
             
             var message = actionType == FormActionType.TempSave ? "表單已暫時儲存" : "表單已成功提交";
             ShowMessage(message, MessageType.Success);
             
             // 重新導向到同一頁面以更新資料
             Response.Redirect($"SciApplication.aspx?ProjectID={txtProjectID.Text}", false);
             Context.ApplicationInstance.CompleteRequest();
         }
         catch (Exception ex)
         {
             HandleException(ex, "儲存表單時發生錯誤");
         }
     }

     #endregion

     #region 表單驗證

     /// <summary>
     /// 驗證整個表單
     /// </summary>
     private bool ValidateForm(FormActionType actionType)
     {
         var errors = new List<string>();

         // 基本資料驗證
         ValidateBasicFields(errors);
         
         // 人員資料驗證
         ValidatePersonnelFields(errors);
         
         // 內容資料驗證
         ValidateContentFields(errors);
         
         // 關鍵字驗證
         ValidateKeywords(errors);

         // 只有在正式提交時才檢查聲明書
         if (actionType == FormActionType.Submit)
         {
             ValidateAgreement(errors);
         }

         if (errors.Count > 0)
         {
             var errorMessage = "請修正以下錯誤：\n" + string.Join("\n", errors);
             ShowMessage(errorMessage, MessageType.Error);
             return false;
         }

         return true;
     }

     /// <summary>
     /// 驗證基本欄位
     /// </summary>
     private void ValidateBasicFields(List<string> errors)
     {
         if (string.IsNullOrWhiteSpace(txtProjectNameCh.Text))
             errors.Add("• 請輸入計畫中文名稱");

         if (string.IsNullOrWhiteSpace(txtOrgName.Text))
             errors.Add("• 請輸入申請單位");

         if (string.IsNullOrWhiteSpace(txtRegisteredAddress.Text))
             errors.Add("• 請輸入登記地址");

         if (string.IsNullOrWhiteSpace(txtCorrespondenceAddress.Text))
             errors.Add("• 請輸入通訊地址");
     }

     /// <summary>
     /// 驗證人員欄位
     /// </summary>
     private void ValidatePersonnelFields(List<string> errors)
     {
         // 驗證計畫主持人
         if (string.IsNullOrWhiteSpace(txtPIName.Text))
             errors.Add("• 請輸入計畫主持人姓名");
         if (string.IsNullOrWhiteSpace(txtPIJobTitle.Text))
             errors.Add("• 請輸入計畫主持人職稱");
         if (string.IsNullOrWhiteSpace(txtPIMobile.Text))
             errors.Add("• 請輸入計畫主持人手機號碼");
     
         // 驗證計畫聯絡人
         if (string.IsNullOrWhiteSpace(txtContactName.Text))
             errors.Add("• 請輸入計畫聯絡人姓名");
         if (string.IsNullOrWhiteSpace(txtContactJobTitle.Text))
             errors.Add("• 請輸入計畫聯絡人職稱");
         if (string.IsNullOrWhiteSpace(txtContactMobile.Text))
             errors.Add("• 請輸入計畫聯絡人手機號碼");
     }

     /// <summary>
     /// 驗證內容欄位
     /// </summary>
     private void ValidateContentFields(List<string> errors)
     {
         ValidateTextLength(txtTarget.Text, 500, "計畫目標", errors);
         ValidateTextLength(txtSummary.Text, 500, "計畫內容摘要", errors);
         ValidateTextLength(txtInnovation.Text, 250, "計畫創新重點", errors);
     }

     /// <summary>
     /// 驗證文字長度
     /// </summary>
     private void ValidateTextLength(string text, int maxLength, string fieldName, List<string> errors)
     {
         if (string.IsNullOrWhiteSpace(text))
             errors.Add($"• 請輸入{fieldName}");
         else if (text.Length > maxLength)
             errors.Add($"• {fieldName}不能超過{maxLength}字");
     }

     /// <summary>
     /// 驗證關鍵字
     /// </summary>
     private void ValidateKeywords(List<string> errors)
     {
         try
         {
             string keywordsJson = hiddenKeywordsData.Value;
             int validKeywordCount = 0;
             
             if (!string.IsNullOrEmpty(keywordsJson))
             {
                 var serializer = new JavaScriptSerializer();
                 var keywords = serializer.DeserializeObject(keywordsJson) as object[];
                 
                 if (keywords != null)
                 {
                     for (int i = 0; i < keywords.Length; i++)
                     {
                         var keyword = keywords[i] as Dictionary<string, object>;
                         if (keyword != null)
                         {
                             string chValue = keyword.ContainsKey("chinese") ? keyword["chinese"]?.ToString()?.Trim() ?? "" : "";
                             string enValue = keyword.ContainsKey("english") ? keyword["english"]?.ToString()?.Trim() ?? "" : "";

                             if (!string.IsNullOrEmpty(chValue) && !string.IsNullOrEmpty(enValue))
                             {
                                 validKeywordCount++;
                             }
                             else if (!string.IsNullOrEmpty(chValue) || !string.IsNullOrEmpty(enValue))
                             {
                                 errors.Add($"• 第{i + 1}個關鍵字需要同時填寫中文和英文");
                             }
                         }
                     }
                 }
             }

             if (validKeywordCount < MIN_KEYWORD_COUNT)
                 errors.Add($"• 至少需要輸入{MIN_KEYWORD_COUNT}個關鍵字（中英文都要填寫）");
         }
         catch (Exception ex)
         {
             errors.Add("• 關鍵字資料格式錯誤，請重新輸入");
         }
     }

     /// <summary>
     /// 驗證聲明書同意
     /// </summary>
     private void ValidateAgreement(List<string> errors)
     {
         if (!chkAgreeTerms.Checked)
         {
             errors.Add("• 請勾選「我已了解並同意」聲明書內容");
         }
     }

     #endregion

     #region 資料儲存

     /// <summary>
     /// 儲存表單資料
     /// </summary>
     private void SaveFormData(FormActionType actionType)
     {
         var versionData = OFS_SciApplicationHelper.getVersionLatestProjectID(txtProjectID.Text);
         
         if (versionData == null || string.IsNullOrEmpty(versionData.Version_ID))
         {
             CreateNewProject(actionType);
         }
         else
         {
             UpdateExistingProject(versionData, actionType);
         }
     }

     /// <summary>
     /// 建立新專案
     /// </summary>
     private void CreateNewProject(FormActionType actionType)
     {
         var latestData = OFS_SciApplicationHelper.getLatestApplicationMain(txtYear.Text);
         string newSerial = GenerateNewSerial(latestData);
         
         var applicationData = CreateApplicationMainData(newSerial);
         var versionData = CreateVersionData(applicationData, actionType);

         // 儲存資料
         OFS_SciApplicationHelper.insertApplicationMain(applicationData);
         OFS_SciApplicationHelper.InsertOFS_SCIVersion(versionData);
         
         txtProjectID.Text = applicationData.Version_ID;
         
         // 儲存子表單資料
         SaveSubFormData(applicationData.Version_ID, applicationData.PersonID, applicationData.KeywordID);
     }

     /// <summary>
     /// 更新現有專案
     /// </summary>
     private void UpdateExistingProject(OFS_SCI_Version versionData, FormActionType actionType)
     {
         var currentData = OFS_SciApplicationHelper.getApplicationMainByVersion_ID(versionData.Version_ID);
         UpdateApplicationMainData(currentData);
         UpdateVersionData(versionData, actionType);

         // 儲存資料
         OFS_SciApplicationHelper.updateApplicationMain(currentData);
         OFS_SciApplicationHelper.UpdateOFS_SCIVersion(versionData);
         
         // 儲存子表單資料
         SaveSubFormData(currentData.Version_ID, currentData.PersonID, currentData.KeywordID);
     }

     /// <summary>
     /// 產生新的序號
     /// </summary>
     private string GenerateNewSerial(OFS_SCI_Application_Main latestData)
     {
         if (latestData != null)
         {
             return (int.Parse(latestData.Serial) + 1).ToString("D4");
         }
         return "0001";
     }

     /// <summary>
     /// 建立申請主資料
     /// </summary>
     private OFS_SCI_Application_Main CreateApplicationMainData(string serial)
     {
         var yearStr = txtYear.Text;
         return new OFS_SCI_Application_Main
         {
             Version_ID = $"{yearStr}SCI{serial}",
             PersonID = $"P{yearStr}SCI{serial}",
             KeywordID = $"K{yearStr}SCI{serial}",
             Serial = serial,
             Year = int.Parse(yearStr),
             SubsidyPlanType = txtSubsidyPlanType.Text,
             ProjectNameTw = txtProjectNameCh.Text,
             ProjectNameEn = txtProjectNameEn.Text,
             OrgCategory = ddlApplicationType.SelectedValue,
             Topic = ddlTopic.SelectedValue,
             Field = ddlField.SelectedValue,
             CountryTech_Underwater = rbUnderwaterYes.Checked,
             CountryTech_Geology = rbMarineYes.Checked,
             CountryTech_Physics = rbPhysicsYes.Checked,
             OrgName = txtOrgName.Text,
             RegisteredAddress = txtRegisteredAddress.Text,
             CorrespondenceAddress = txtCorrespondenceAddress.Text,
             Target = txtTarget.Text,
             Summary = txtSummary.Text,
             Innovation = txtInnovation.Text,
             Declaration = true
         };
     }

     /// <summary>
     /// 更新申請主資料
     /// </summary>
     private void UpdateApplicationMainData(OFS_SCI_Application_Main data)
     {
         data.SubsidyPlanType = txtSubsidyPlanType.Text;
         data.ProjectNameTw = txtProjectNameCh.Text;
         data.ProjectNameEn = txtProjectNameEn.Text;
         data.OrgCategory = ddlApplicationType.SelectedValue;
         data.Topic = ddlTopic.SelectedValue;
         data.Field = ddlField.SelectedValue;
         data.CountryTech_Underwater = rbUnderwaterYes.Checked;
         data.CountryTech_Geology = rbMarineYes.Checked;
         data.CountryTech_Physics = rbPhysicsYes.Checked;
         data.OrgName = txtOrgName.Text;
         data.RegisteredAddress = txtRegisteredAddress.Text;
         data.CorrespondenceAddress = txtCorrespondenceAddress.Text;
         data.Target = txtTarget.Text;
         data.Summary = txtSummary.Text;
         data.Innovation = txtInnovation.Text;
         data.Declaration = true;
     }

     /// <summary>
     /// 建立版本資料
     /// </summary>
     private OFS_SCI_Version CreateVersionData(OFS_SCI_Application_Main applicationData, FormActionType actionType)
     {
         return new OFS_SCI_Version
         {
             Version_ID = applicationData.Version_ID,
             ProjectID = applicationData.Version_ID,
             VersionNum = 1,
             Statuses = "尚未提送",
             StatusesName = "編輯中",
             Form1Status = actionType == FormActionType.TempSave ? "暫存" : "送出",
             CurrentStep = actionType == FormActionType.TempSave ? "1" : "2",
             SupervisoryUnit = "",
             UserAccount = "",
             UserOrg = "",
             UserName = ""
         };
     }

     /// <summary>
     /// 更新版本資料
     /// </summary>
     private void UpdateVersionData(OFS_SCI_Version versionData, FormActionType actionType)
     {
         versionData.Form1Status = actionType == FormActionType.TempSave ? "暫存" : "送出";
         
         if (actionType == FormActionType.Submit && Convert.ToInt32(versionData.CurrentStep) <= 2)
         {
             versionData.CurrentStep = "2";
         }
         else if (actionType == FormActionType.TempSave)
         {
             versionData.CurrentStep = "1";
         }
     }

     /// <summary>
     /// 儲存子表單資料
     /// </summary>
     private void SaveSubFormData(string projectID, string personID, string keywordID)
     {
         SavePersonnelData(personID);
         SaveKeywordsData(keywordID);
     }

     /// <summary>
     /// 儲存人員資料
     /// </summary>
     private void SavePersonnelData(string personID)
     {
         var personnelList = new List<OFS_SCI_Application_Personnel>
         {
             CreatePersonnelData(personID, "計畫主持人", txtPIIdx, txtPIName, txtPIJobTitle, txtPIPhone, txtPIPhoneExt, txtPIMobile),
             CreatePersonnelData(personID, "計畫聯絡人", txtContactIdx, txtContactName, txtContactJobTitle, txtContactPhone, txtContactPhoneExt, txtContactMobile),
             CreatePersonnelData(personID, "會計聯絡人", txtAccountIdx, txtAccountName, txtAccountJobTitle, txtAccountPhone, txtAccountPhoneExt, txtAccountMobile)
         };

         OFS_SciApplicationHelper.SavePersonnel(personnelList);
     }

     /// <summary>
     /// 建立人員資料
     /// </summary>
     private OFS_SCI_Application_Personnel CreatePersonnelData(string personID, string role,
         Label idxLabel, TextBox nameBox, TextBox jobTitleBox, 
         TextBox phoneBox, TextBox extBox, TextBox mobileBox)
     {
         return new OFS_SCI_Application_Personnel
         {
             idx = int.Parse(idxLabel.Text),
             PersonID = personID,
             Role = role,
             Name = nameBox.Text,
             JobTitle = jobTitleBox.Text,
             Phone = phoneBox.Text,
             PhoneExt = extBox.Text,
             MobilePhone = mobileBox.Text
         };
     }

     /// <summary>
     /// 儲存關鍵字資料
     /// </summary>
     private void SaveKeywordsData(string keywordID)
     {
         var keywords = GetKeywordsFromForm(keywordID);
         
      
         OFS_SciApplicationHelper.SaveKeywordsToDatabase(keywordID,keywords);
         
     }

     /// <summary>
     /// 從表單取得關鍵字資料 - 適配動態欄位
     /// </summary>
     private List<OFS_SCI_Application_KeyWord> GetKeywordsFromForm(string keywordID)
     {
         var keywords = new List<OFS_SCI_Application_KeyWord>();

         try
         {
             // 直接從隱藏欄位取得關鍵字資料
             string keywordsJson = hiddenKeywordsData.Value;
             
             if (!string.IsNullOrEmpty(keywordsJson))
             {
                 var serializer = new JavaScriptSerializer();
                 var frontendKeywords = serializer.DeserializeObject(keywordsJson) as object[];
                 
                 if (frontendKeywords != null)
                 {
                     foreach (var item in frontendKeywords)
                     {
                         var keyword = item as Dictionary<string, object>;
                         if (keyword != null)
                         {
                             string chValue = keyword.ContainsKey("chinese") ? keyword["chinese"]?.ToString()?.Trim() ?? "" : "";
                             string enValue = keyword.ContainsKey("english") ? keyword["english"]?.ToString()?.Trim() ?? "" : "";

                             // 只有當兩個欄位都有值時才加入
                             if (!string.IsNullOrEmpty(chValue) && !string.IsNullOrEmpty(enValue))
                             {
                                 keywords.Add(new OFS_SCI_Application_KeyWord
                                 {
                                     KeywordID = keywordID,
                                     KeyWordTw = chValue,
                                     KeyWordEn = enValue
                                 });
                             }
                         }
                     }
                 }
             }
         }
         catch (Exception ex)
         {
             System.Diagnostics.Debug.WriteLine($"Error parsing keywords: {ex.Message}");
         }

         return keywords;
     }

     #endregion

     #region 訊息處理

     /// <summary>
     /// 顯示訊息
     /// </summary>
     private void ShowMessage(string message, MessageType type)
     {
         string icon = GetMessageIcon(type);
         string safeMessage = HttpUtility.JavaScriptStringEncode(message);
         safeMessage = safeMessage.Replace("\n", "\\n").Replace("\r", "");

         string script = $@"
             Swal.fire({{
                 title: '',
                 text: '{safeMessage}',
                 icon: '{icon}',
                 confirmButtonText: '確定',
                 customClass: {{
                     popup: 'animated fadeInDown'
                 }}
             }});
         ";

         ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), script, true);
     }

     /// <summary>
     /// 取得訊息圖示
     /// </summary>
     private string GetMessageIcon(MessageType type)
     {
         switch (type)
         {
             case MessageType.Success:
                 return "success";
             case MessageType.Error:
                 return "error";
             case MessageType.Warning:
                 return "warning";
             case MessageType.Info:
             default:
                 return "info";
         }
     }

     /// <summary>
     /// 處理例外
     /// </summary>
     private void HandleException(Exception ex, string userMessage)
     {
         // 顯示使用者友善的錯誤訊息
         ShowMessage($"{userMessage}: {ex.Message}", MessageType.Error);
     }

     #endregion

     #region 列舉和常數

     /// <summary>
     /// 表單動作類型
     /// </summary>
     private enum FormActionType
     {
         TempSave,   // 暫時儲存
         Submit      // 正式提交
     }

     /// <summary>
     /// 訊息類型
     /// </summary>
     private enum MessageType
     {
         Success,    // 成功
         Error,      // 錯誤
         Warning,    // 警告
         Info        // 資訊
     }

     #endregion
}