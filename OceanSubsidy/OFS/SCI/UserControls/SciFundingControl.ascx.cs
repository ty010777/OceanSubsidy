using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Model.OFS;
using GS.OCA_OceanSubsidy.Operation.OFS;
using Newtonsoft.Json;

/// <summary>
/// 科專計畫經費/人事 UserControl
/// 可重複使用的經費管理內容元件
/// </summary>
public partial class OFS_SCI_UserControls_SciFundingControl : System.Web.UI.UserControl
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
    /// 人事費資料
    /// </summary>
    public List<PersonRow> PersonnelData { get; private set; }

    /// <summary>
    /// 總費用資料
    /// </summary>
    public List<TotalFeeRow> TotalFeesData { get; private set; }

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
            
            BindDropDown();
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
            // 從隱藏欄位取得資料進行驗證
            var personnelData = GetPersonnelDataFromHidden();
            var totalFeesData = GetTotalFeesDataFromHidden();

            // 驗證人事費資料
            if (personnelData == null || personnelData.Count == 0)
            {
                result.AddError("請至少新增一筆人事費資料");
            }
            else
            {
                foreach (var person in personnelData)
                {
                    if (string.IsNullOrWhiteSpace(person.name))
                    {
                        result.AddError("請輸入人員姓名");
                        break;
                    }
                    if (string.IsNullOrWhiteSpace(person.title))
                    {
                        result.AddError("請選擇職稱");
                        break;
                    }
                    if (person.salary <= 0)
                    {
                        result.AddError("請輸入有效的平均月薪");
                        break;
                    }
                    if (person.months <= 0)
                    {
                        result.AddError("請輸入有效的參與人月");
                        break;
                    }
                }
            }

            // 驗證總費用資料
            if (totalFeesData == null || totalFeesData.Count == 0)
            {
                result.AddError("請輸入經費總表資料");
            }

            // 驗證行政管理費（可選）
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
    /// <returns>儲存是否成功</returns>
    public bool SaveData(string projectID)
    {
        this.ProjectID = projectID;
        try
        {
            // 取得表單資料
            var personnelData = GetPersonnelDataFromHidden();
            var materialData = GetMaterialDataFromHidden();
            var researchData = GetResearchDataFromHidden();
            var travelData = GetTravelDataFromHidden();
            var otherData = GetOtherDataFromHidden();
            var otherRentData = GetOtherRentDataFromHidden();
            var totalFeesData = GetTotalFeesDataFromHidden();

            // 儲存各類型資料
            if (personnelData != null && personnelData.Count > 0)
            {
                OFS_SciFundingHelper.ReplacePersonFormList(personnelData, ProjectID);
            }

            if (materialData != null && materialData.Count > 0)
            {
                OFS_SciFundingHelper.ReplaceMaterialList(materialData, ProjectID);
            }

            if (researchData != null && researchData.Count > 0)
            {
                OFS_SciFundingHelper.ReplaceResearchFees(researchData, ProjectID);
            }

            if (travelData != null && travelData.Count > 0)
            {
                OFS_SciFundingHelper.ReplaceTripForm(travelData, ProjectID);
            }

            if (otherData != null && otherData.Count > 0)
            {
                OFS_SciFundingHelper.ReplaceOtherPersonFee(otherData, ProjectID);
            }

            if (otherRentData != null && otherRentData.Count > 0)
            {
                OFS_SciFundingHelper.ReplaceOtherObjectFee(otherRentData, ProjectID);
            }

            if (totalFeesData != null && totalFeesData.Count > 0)
            {
                OFS_SciFundingHelper.ReplaceTotalFeeList(totalFeesData, ProjectID);
            }


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
        hdnPersonnelData.Value = "[]";
        hdnMaterialData.Value = "[]";
        hdnTravelData.Value = "[]";
        hdnOtherData.Value = "[]";
        hdnOtherRentData.Value = "[]";
        hdnTotalFeesData.Value = "[]";
    }

    /// <summary>
    /// 綁定下拉選單
    /// </summary>
    private void BindDropDown()
    {
        try
        {
            var personList = OFS_SciFundingHelper.GetSysZgsCodeByCodeGroup("SCIPersonAcademic");

            StringBuilder jsBuilder = new StringBuilder();
            jsBuilder.AppendLine("const ddlPersonOptions = [");

            foreach (var item in personList)
            {
                // 將 MaxPriceLimit 也輸出成 JS 物件屬性
                jsBuilder.AppendLine($"    {{ text: \"{item.Descname}\", value: \"{item.Code}\", maxLimit: {item.MaxPriceLimit} }},");
            }

            jsBuilder.AppendLine("];");

            // 材料單位下拉選單
            var materialUnitList = OFS_SciFundingHelper.GetSysZgsCodeByCodeGroup("SCIMaterialUnit");
            jsBuilder.AppendLine("const ddlMaterialOptions = [");
            foreach (var item in materialUnitList)
            {
                jsBuilder.AppendLine($"    {{ text: \"{item.Descname}\", value: \"{item.Code}\", maxLimit: {item.MaxPriceLimit} }},");
            }
            jsBuilder.AppendLine("];");

            // 其他業務費職稱下拉選單 - 修正參數名稱
            var otherJobList = OFS_SciFundingHelper.GetSysZgsCodeByCodeGroup("SCIOtherAcademic");
            jsBuilder.AppendLine("const ddlOtherOptions = [");
            foreach (var item in otherJobList)
            {
                jsBuilder.AppendLine($"    {{ text: \"{item.Descname}\", value: \"{item.Code}\", maxLimit: {item.MaxPriceLimit} }},");
            }
            jsBuilder.AppendLine("];");

            // 註冊到頁面
            Page.ClientScript.RegisterStartupScript(this.GetType(), "ddlOthersScript", jsBuilder.ToString(), true);
        }
        catch (Exception ex)
        {
            HandleException(ex, "綁定下拉選單時發生錯誤");
        }
    }

    /// <summary>
    /// 載入現有資料
    /// </summary>
    private void LoadExistingData(string projectID)
    {
        try
        {
            // 載入所有類型的資料
            var personnelData = OFS_SciFundingHelper.GetPersonFormList(projectID);
            var materialData = OFS_SciFundingHelper.GetMaterialList(projectID);
            var researchData = OFS_SciFundingHelper.GetResearchFeesList(projectID);
            var travelData = OFS_SciFundingHelper.GetTripFormList(projectID);
            var otherData = OFS_SciFundingHelper.GetOtherPersonFeeList(projectID);
            var otherRentData = OFS_SciFundingHelper.GetOtherObjectFeeList(projectID);
            var totalFeesData = OFS_SciFundingHelper.GetTotalFeeList(projectID);

            // 將資料轉換為 JSON 並輸出到前端
            var serializer = new JavaScriptSerializer();
            
            StringBuilder jsBuilder = new StringBuilder();
            jsBuilder.AppendLine("window.loadedData = {");
            jsBuilder.AppendLine($"    personnel: {serializer.Serialize(personnelData)},");
            jsBuilder.AppendLine($"    material: {serializer.Serialize(materialData)},");
            jsBuilder.AppendLine($"    research: {serializer.Serialize(researchData)},");
            jsBuilder.AppendLine($"    travel: {serializer.Serialize(travelData)},");
            jsBuilder.AppendLine($"    other: {serializer.Serialize(otherData)},");
            jsBuilder.AppendLine($"    otherRent: {serializer.Serialize(otherRentData)},");
            jsBuilder.AppendLine($"    totalFees: {serializer.Serialize(totalFeesData)}");
            jsBuilder.AppendLine("};");

            // 載入固定欄位資料到 ASP.NET 控件
            LoadFormControls(researchData, otherRentData, totalFeesData);

            // 確保載入資料後觸發前端載入函數
            jsBuilder.AppendLine();
            jsBuilder.AppendLine("// 確保載入資料到表單");
            jsBuilder.AppendLine("if (document.readyState === 'loading') {");
            jsBuilder.AppendLine("    document.addEventListener('DOMContentLoaded', function() {");
            jsBuilder.AppendLine("        setTimeout(function() {");
            jsBuilder.AppendLine("            if (typeof loadExistingDataToForm === 'function') {");
            jsBuilder.AppendLine("                loadExistingDataToForm();");
            jsBuilder.AppendLine("            }");
            jsBuilder.AppendLine("        }, 150);");
            jsBuilder.AppendLine("    });");
            jsBuilder.AppendLine("} else {");
            jsBuilder.AppendLine("    setTimeout(function() {");
            jsBuilder.AppendLine("        if (typeof loadExistingDataToForm === 'function') {");
            jsBuilder.AppendLine("            loadExistingDataToForm();");
            jsBuilder.AppendLine("        }");
            jsBuilder.AppendLine("    }, 150);");
            jsBuilder.AppendLine("}");

            Page.ClientScript.RegisterStartupScript(
                this.GetType(),
                "LoadedDataScript",
                jsBuilder.ToString(),
                true
            );
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入現有資料時發生錯誤");
            Page.ClientScript.RegisterStartupScript(
                this.GetType(),
                "LoadDataError",
                $"console.error('載入資料失敗: {ex.Message}');",
                true
            );
        }
    }

    /// <summary>
    /// 載入表單控制項資料
    /// </summary>
    private void LoadFormControls(List<ResearchFeeRow> researchData, List<OtherRent> otherRentData, List<TotalFeeRow> totalFeesData)
    {
        try
        {
            // 載入研究費資料到 ASP.NET 控件
            var techTransferData = researchData.FirstOrDefault(r => r.category == "技術移轉");
            if (techTransferData != null)
            {
                txtDate1Start.Text = techTransferData.dateStart;
                txtDate1End.Text = techTransferData.dateEnd;
                ResearchFeesName1.Text = techTransferData.projectName;
                ResearchFeesPersonName1.Text = techTransferData.targetPerson;
                ResearchFeesPrice1.Text = techTransferData.price.ToString();
            }

            var researchData2 = researchData.FirstOrDefault(r => r.category == "委託研究");
            if (researchData2 != null)
            {
                txtDate2Start.Text = researchData2.dateStart;
                txtDate2End.Text = researchData2.dateEnd;
                ResearchFeesName2.Text = researchData2.projectName;
                ResearchFeesPersonName2.Text = researchData2.targetPerson;
                ResearchFeesPrice2.Text = researchData2.price.ToString();
            }

            // 載入租金資料到 ASP.NET 控件
            var rentData = otherRentData.FirstOrDefault(r => r.item == "租金");
            if (rentData != null)
            {
                rentCash.Text = rentData.amount.ToString();
                rentDescription.Text = rentData.note;
            }

            // 載入行政管理費資料到 ASP.NET 控件
            var adminFeeData = totalFeesData.FirstOrDefault(t => t.accountingItem?.Contains("行政管理費") == true);
            if (adminFeeData != null)
            {
                AdminFeeSubsidy.Text = adminFeeData.subsidyAmount?.ToString() ?? "0";
                AdminFeeCoop.Text = adminFeeData.coopAmount?.ToString() ?? "0";
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入表單控制項資料時發生錯誤");
        }
    }


    /// <summary>
    /// 從隱藏欄位取得人事費資料
    /// </summary>
    private List<PersonRow> GetPersonnelDataFromHidden()
    {
        var personnelList = new List<PersonRow>();

        try
        {
            if (!string.IsNullOrEmpty(hdnPersonnelData.Value))
            {
                var serializer = new JavaScriptSerializer();
                var personnelArray = serializer.Deserialize<dynamic[]>(hdnPersonnelData.Value);

                foreach (var item in personnelArray)
                {
                    if (item != null)
                    {
                        var personnel = new PersonRow
                        {
                            name = item["name"]?.ToString() ?? "",
                            stay = bool.TryParse(item["stay"]?.ToString(), out bool stay) && stay,
                            title = item["title"]?.ToString() ?? "",
                            salary = decimal.TryParse(item["salary"]?.ToString(), out decimal salary) ? salary : 0,
                            months = decimal.TryParse(item["months"]?.ToString(), out decimal months) ? months : 0
                        };

                        personnelList.Add(personnel);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "解析人事費資料時發生錯誤");
        }

        return personnelList;
    }

    /// <summary>
    /// 從隱藏欄位取得總費用資料
    /// </summary>
    private List<TotalFeeRow> GetTotalFeesDataFromHidden()
    {
        var totalFeesList = new List<TotalFeeRow>();

        try
        {
            if (!string.IsNullOrEmpty(hdnTotalFeesData.Value))
            {
                var serializer = new JavaScriptSerializer();
                var totalFeesArray = serializer.Deserialize<dynamic[]>(hdnTotalFeesData.Value);

                foreach (var item in totalFeesArray)
                {
                    if (item != null)
                    {
                        var totalFee = new TotalFeeRow
                        {
                            accountingItem = item["accountingItem"]?.ToString() ?? "",
                            subsidyAmount = decimal.TryParse(item["subsidyAmount"]?.ToString(), out decimal subsidyAmount) ? subsidyAmount : 0,
                            coopAmount = decimal.TryParse(item["coopAmount"]?.ToString(), out decimal coopAmount) ? coopAmount : 0
                        };

                        totalFeesList.Add(totalFee);
                    }
                }
            }
            
            // 確保包含行政管理費資料（從表單控制項取得）
            if (!string.IsNullOrEmpty(AdminFeeSubsidy.Text) || !string.IsNullOrEmpty(AdminFeeCoop.Text))
            {
                var adminFee = new TotalFeeRow
                {
                    accountingItem = "行政管理費",
                    subsidyAmount = decimal.TryParse(AdminFeeSubsidy.Text, out decimal adminSub) ? adminSub : 0,
                    coopAmount = decimal.TryParse(AdminFeeCoop.Text, out decimal adminCoop) ? adminCoop : 0
                };

                // 如果已存在行政管理費記錄，則更新；否則新增
                var existingAdminFee = totalFeesList.FirstOrDefault(t => t.accountingItem?.Contains("行政管理費") == true);
                if (existingAdminFee != null)
                {
                    existingAdminFee.subsidyAmount = adminFee.subsidyAmount;
                    existingAdminFee.coopAmount = adminFee.coopAmount;
                }
                else
                {
                    totalFeesList.Add(adminFee);
                }
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "解析總費用資料時發生錯誤");
        }

        return totalFeesList;
    }

    /// <summary>
    /// 從隱藏欄位取得材料費資料
    /// </summary>
    private List<MaterialRow> GetMaterialDataFromHidden()
    {
        var materialList = new List<MaterialRow>();
        try
        {
            if (!string.IsNullOrEmpty(hdnMaterialData.Value))
            {
                var serializer = new JavaScriptSerializer();
                materialList = serializer.Deserialize<List<MaterialRow>>(hdnMaterialData.Value) ?? new List<MaterialRow>();
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "解析材料費資料時發生錯誤");
        }
        return materialList;
    }

    /// <summary>
    /// 從隱藏欄位取得研究費資料
    /// </summary>
    private List<ResearchFeeRow> GetResearchDataFromHidden()
    {
        var researchList = new List<ResearchFeeRow>();
        try
        {
            // 從表單控制項取得技術移轉和委託研究資料
            if (!string.IsNullOrEmpty(ResearchFeesPrice1.Text))
            {
                researchList.Add(new ResearchFeeRow
                {
                    category = "技術移轉",
                    dateStart = txtDate1Start.Text,
                    dateEnd = txtDate1End.Text,
                    projectName = ResearchFeesName1.Text,
                    targetPerson = ResearchFeesPersonName1.Text,
                    price = decimal.TryParse(ResearchFeesPrice1.Text, out decimal price1) ? price1 : 0
                });
            }

            if (!string.IsNullOrEmpty(ResearchFeesPrice2.Text))
            {
                researchList.Add(new ResearchFeeRow
                {
                    category = "委託研究",
                    dateStart = txtDate2Start.Text,
                    dateEnd = txtDate2End.Text,
                    projectName = ResearchFeesName2.Text,
                    targetPerson = ResearchFeesPersonName2.Text,
                    price = decimal.TryParse(ResearchFeesPrice2.Text, out decimal price2) ? price2 : 0
                });
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "解析研究費資料時發生錯誤");
        }
        return researchList;
    }

    /// <summary>
    /// 從隱藏欄位取得差旅費資料
    /// </summary>
    private List<TravelRow> GetTravelDataFromHidden()
    {
        var travelList = new List<TravelRow>();
        try
        {
            if (!string.IsNullOrEmpty(hdnTravelData.Value))
            {
                var serializer = new JavaScriptSerializer();
                travelList = serializer.Deserialize<List<TravelRow>>(hdnTravelData.Value) ?? new List<TravelRow>();
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "解析差旅費資料時發生錯誤");
        }
        return travelList;
    }

    /// <summary>
    /// 從隱藏欄位取得其他人事費資料
    /// </summary>
    private List<OtherFeeRow> GetOtherDataFromHidden()
    {
        var otherList = new List<OtherFeeRow>();
        try
        {
            if (!string.IsNullOrEmpty(hdnOtherData.Value))
            {
                var serializer = new JavaScriptSerializer();
                otherList = serializer.Deserialize<List<OtherFeeRow>>(hdnOtherData.Value) ?? new List<OtherFeeRow>();
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "解析其他人事費資料時發生錯誤");
        }
        return otherList;
    }

    /// <summary>
    /// 從隱藏欄位取得其他租金資料
    /// </summary>
    private List<OtherRent> GetOtherRentDataFromHidden()
    {
        var otherRentList = new List<OtherRent>();
        try
        {
            if (!string.IsNullOrEmpty(hdnOtherRentData.Value))
            {
                var serializer = new JavaScriptSerializer();
                otherRentList = serializer.Deserialize<List<OtherRent>>(hdnOtherRentData.Value) ?? new List<OtherRent>();
            }
            else
            {
                // 從表單控制項取得租金資料
                if (!string.IsNullOrEmpty(rentCash.Text))
                {
                    otherRentList.Add(new OtherRent
                    {
                        item = "租金",
                        amount = decimal.TryParse(rentCash.Text, out decimal amount) ? amount : 0,
                        note = rentDescription.Text
                    });
                }
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "解析其他租金資料時發生錯誤");
        }
        return otherRentList;
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