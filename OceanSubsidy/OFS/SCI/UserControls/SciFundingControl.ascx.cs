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
    protected string ProjectID => Request.QueryString["ProjectID"];

    /// <summary>
    /// 是否為檢視模式
    /// </summary>
    public bool IsViewMode { get; set; }

    /// <summary>
    /// 人事費資料
    /// </summary>
    public List<PersonRow> PersonnelData { get; private set; }

    /// <summary>
    /// 總費用資料
    /// </summary>
    public List<TotalFeeRow> TotalFeesData { get; private set; }

    /// <summary>
    /// 是否顯示國外差旅費（OceanTech 不顯示）
    /// </summary>
    protected bool ShowForeignTravel
    {
        get
        {
            if (string.IsNullOrEmpty(ProjectID)) return true;

            var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(ProjectID);
            string orgCategory = applicationMain?.OrgCategory ?? "";
            return !orgCategory.Equals("OceanTech", StringComparison.OrdinalIgnoreCase);
        }
    }

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
            }

            BindDropDown();

            // 取得並傳遞補助款上限和配合款比例到前端
            if (!string.IsNullOrEmpty(ProjectID))
            {
                LoadGrantLimitAndMatchingFund(ProjectID);
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
    public void LoadData(string projectID)
    {
        try
        {
            if (!string.IsNullOrEmpty(projectID))
            {
                LoadExistingData(projectID);
                // 載入變更說明控制項
                tab3_ucChangeDescription.LoadData(projectID);
                CheckFormStatusAndHideTempSaveButton();
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
            var serializer = new JavaScriptSerializer();
            var jsBuilder = new StringBuilder();

            // 解析各類經費資料（安全版）
            List<PersonRow> personnelData = snapshotData.PersonnelCostPersonForm != null
                ? snapshotData.PersonnelCostPersonForm.ToObject<List<PersonRow>>()
                : new List<PersonRow>();

            List<MaterialRow> materialData = snapshotData.PersonnelCostMaterial != null
                ? snapshotData.PersonnelCostMaterial.ToObject<List<MaterialRow>>()
                : new List<MaterialRow>();

            List<ResearchFeeRow> researchData = snapshotData.PersonnelCostResearchFees != null
                ? snapshotData.PersonnelCostResearchFees.ToObject<List<ResearchFeeRow>>()
                : new List<ResearchFeeRow>();

            List<TravelRow> travelData = snapshotData.PersonnelCostTripForm != null
                ? snapshotData.PersonnelCostTripForm.ToObject<List<TravelRow>>()
                : new List<TravelRow>();

            List<ForeignTravelRow> foreignTravelData = snapshotData.PersonnelCostAbroadTrip != null
                ? snapshotData.PersonnelCostAbroadTrip.ToObject<List<ForeignTravelRow>>()
                : new List<ForeignTravelRow>();

            List<OtherFeeRow> otherData = snapshotData.PersonnelCostOtherPersonFee != null
                ? snapshotData.PersonnelCostOtherPersonFee.ToObject<List<OtherFeeRow>>()
                : new List<OtherFeeRow>();

            List<OtherRent> otherRentData = snapshotData.PersonnelCostOtherObjectFee != null
                ? snapshotData.PersonnelCostOtherObjectFee.ToObject<List<OtherRent>>()
                : new List<OtherRent>();

            List<TotalFeeRow> totalFeesData = snapshotData.PersonnelCostTotalFee != null
                ? snapshotData.PersonnelCostTotalFee.ToObject<List<TotalFeeRow>>()
                : new List<TotalFeeRow>();

            // 取得 OrgCategory
            string orgCategory = "";
            if (snapshotData.ApplicationMain != null)
            {
                var applicationMain = snapshotData.ApplicationMain.ToObject<OFS_SCI_Application_Main>();
                orgCategory = applicationMain?.OrgCategory ?? "";
            }

            // 如果是 OceanTech，將 totalFeesData 中的行政管理費和國外差旅費歸零
            if (orgCategory.Equals("OceanTech", StringComparison.OrdinalIgnoreCase) && totalFeesData != null)
            {
                var adminFee = totalFeesData.FirstOrDefault(t => t.accountingItem?.Contains("行政管理費") == true);
                if (adminFee != null)
                {
                    adminFee.subsidyAmount = 0;
                    adminFee.coopAmount = 0;
                }

                var foreignTravelFee = totalFeesData.FirstOrDefault(t =>
                    t.accountingItem?.Contains("4-2") == true ||
                    t.accountingItem?.Contains("國外差旅費") == true);
                if (foreignTravelFee != null)
                {
                    foreignTravelFee.subsidyAmount = 0;
                    foreignTravelFee.coopAmount = 0;
                }
            }

            // 產生 JavaScript 載入資料
            jsBuilder.AppendLine("window.loadedData = {");
            jsBuilder.AppendLine($"    personnel: {serializer.Serialize(personnelData)},");
            jsBuilder.AppendLine($"    material: {serializer.Serialize(materialData)},");
            jsBuilder.AppendLine($"    research: {serializer.Serialize(researchData)},");
            jsBuilder.AppendLine($"    travel: {serializer.Serialize(travelData)},");
            jsBuilder.AppendLine($"    foreignTravel: {serializer.Serialize(foreignTravelData)},");
            jsBuilder.AppendLine($"    other: {serializer.Serialize(otherData)},");
            jsBuilder.AppendLine($"    otherRent: {serializer.Serialize(otherRentData)},");
            jsBuilder.AppendLine($"    totalFees: {serializer.Serialize(totalFeesData)},");
            jsBuilder.AppendLine($"    orgCategory: '{orgCategory}'");
            jsBuilder.AppendLine("};");

            // 載入固定欄位資料到 ASP.NET 控件
            LoadFormControls(researchData, otherRentData, totalFeesData);

            // 觸發前端載入函數
            jsBuilder.AppendLine();
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

            // 設定為檢視模式
            IsViewMode = true;
            ApplyViewMode();

            // 隱藏變更說明控制項（快照檢視不需要）
            if (tab3_ucChangeDescription != null)
            {
                tab3_ucChangeDescription.Visible = false;
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入快照資料時發生錯誤");
            Page.ClientScript.RegisterStartupScript(
                this.GetType(),
                "LoadDataError",
                $"console.error('載入快照資料失敗: {ex.Message}');",
                true
            );
        }
    }

    /// <summary>
    /// 檢查表單狀態並控制暫存按鈕顯示
    /// </summary>
    private void CheckFormStatusAndHideTempSaveButton()
    {
        try
        {
            if (!string.IsNullOrEmpty(ProjectID))
            {
                var lastVersion = OFS_SciApplicationHelper.getVersionByProjectID(ProjectID);
                if (lastVersion != null)
                {
                    var formStatus =
                        OFS_SciWorkSchHelper.GetFormStatusByProjectID(lastVersion.ProjectID, "Form3Status");

                    if (formStatus == "完成")
                    {
                        // 隱藏暫存按鈕
                        tab3_btnTempSave.Style["display"] = "none";
                    }
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
            var materialData = GetMaterialDataFromHidden();
            var researchData = GetResearchDataFromHidden();
            var travelData = GetTravelDataFromHidden();
            var foreignTravelData = GetForeignTravelDataFromHidden();
            var otherData = GetOtherDataFromHidden();
            var otherRentData = GetOtherRentDataFromHidden();
            var totalFeesData = GetTotalFeesDataFromHidden();

            // 取得 OrgCategory 判斷是否為 OceanTech
            var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(ProjectID);
            string orgCategory = applicationMain?.OrgCategory ?? "";
            bool isOceanTech = orgCategory.Equals("OceanTech", StringComparison.OrdinalIgnoreCase);

            // ===== 必填項目驗證 =====

            // 1. 驗證海洋科技研發人員人事費明細表（必填，至少一筆且必須完整填寫）
            if (personnelData == null || personnelData.Count == 0)
            {
                result.AddError("明細表「1.海洋科技研發人員人事費」：請至少新增一筆人事費資料");
            }
            else
            {
                List<string> missingFields = new List<string>();
                foreach (var person in personnelData)
                {
                    // 所有行都必須填寫完整，包含空白行
                    if (string.IsNullOrWhiteSpace(person.name))
                    {
                        if (!missingFields.Contains("人員姓名"))
                            missingFields.Add("「人員姓名」");
                    }

                    if (string.IsNullOrWhiteSpace(person.title))
                    {
                        if (!missingFields.Contains("職稱"))
                            missingFields.Add("「職稱」");
                    }

                    if (person.salary <= 0)
                    {
                        if (!missingFields.Contains("平均月薪"))
                            missingFields.Add("「平均月薪」");
                    }

                    if (person.months <= 0)
                    {
                        if (!missingFields.Contains("參與人月"))
                            missingFields.Add("「參與人月」");
                    }
                }

                if (missingFields.Count > 0)
                {
                    result.AddError($"明細表「1.海洋科技研發人員人事費」：請輸入{string.Join("、", missingFields)}");
                }
            }

            // 2. 驗證其他業務費（必填）
            // if (otherData == null || otherData.Count == 0)
            // {
            //     result.AddError("其他業務費：請至少新增一筆資料");
            // }
            // else
            // {
            //     foreach (var other in otherData)
            //     {
            //         if (string.IsNullOrWhiteSpace(other.title))
            //         {
            //             result.AddError("其他業務費：請選擇職稱");
            //             break;
            //         }
            //     }
            // }

            // ===== 選填項目驗證（如有填寫則檢查完整性，可接受金額/天數為0） =====

            // 3. 驗證消耗性器材及原材料費（選填，但有填就檢查）
            if (materialData != null && materialData.Count > 0)
            {
                List<string> missingFields = new List<string>();
                foreach (var material in materialData)
                {
                    // 所有行都必須填寫完整，包含空白行
                    if (string.IsNullOrWhiteSpace(material.name))
                    {
                        if (!missingFields.Contains("品名"))
                            missingFields.Add("「品名」");
                    }

                    if (string.IsNullOrWhiteSpace(material.unit))
                    {
                        if (!missingFields.Contains("單位"))
                            missingFields.Add("「單位」");
                    }

                    // description, quantity, unitPrice 可以為空或 0，不檢查
                }

                if (missingFields.Count > 0)
                {
                    result.AddError($"明細表「2.消耗性器材及原材料費」：請輸入{string.Join("、", missingFields)}");
                }
            }

            // 4. 驗證技術移轉、委託研究或驗證費（選填，但有填就檢查）
            if (researchData != null && researchData.Count > 0)
            {
                // 特殊處理：如果只有一行且完全空白，則跳過檢查（因為此項目為選填）
                if (researchData.Count == 1)
                {
                    var singleRow = researchData[0];
                    bool isCompletelyEmpty = string.IsNullOrWhiteSpace(singleRow.category) &&
                                            string.IsNullOrWhiteSpace(singleRow.dateStart) &&
                                            string.IsNullOrWhiteSpace(singleRow.dateEnd) &&
                                            string.IsNullOrWhiteSpace(singleRow.projectName) &&
                                            string.IsNullOrWhiteSpace(singleRow.targetPerson) &&
                                            singleRow.price <= 0;

                    if (isCompletelyEmpty)
                    {
                        // 跳過檢查，因為此項目為選填
                    }
                    else
                    {
                        // 有填寫任一欄位，則檢查必填欄位
                        List<string> missingFields = new List<string>();

                        if (string.IsNullOrWhiteSpace(singleRow.category))
                        {
                            if (!missingFields.Contains("類別"))
                                missingFields.Add("「類別」");
                        }

                        if (string.IsNullOrWhiteSpace(singleRow.projectName))
                        {
                            if (!missingFields.Contains("委託項目名稱"))
                                missingFields.Add("「委託項目名稱」");
                        }

                        if (string.IsNullOrWhiteSpace(singleRow.targetPerson))
                        {
                            if (!missingFields.Contains("委託對象"))
                                missingFields.Add("「委託對象」");
                        }

                        if (missingFields.Count > 0)
                        {
                            result.AddError($"明細表「3.技術移轉、委託研究或驗證費」：請輸入{string.Join("、", missingFields)}");
                        }
                    }
                }
                else
                {
                    // 多行時，每一行都必須填寫完整
                    List<string> missingFields = new List<string>();
                    foreach (var research in researchData)
                    {
                        if (string.IsNullOrWhiteSpace(research.category))
                        {
                            if (!missingFields.Contains("類別"))
                                missingFields.Add("「類別」");
                        }

                        if (string.IsNullOrWhiteSpace(research.projectName))
                        {
                            if (!missingFields.Contains("委託項目名稱"))
                                missingFields.Add("「委託項目名稱」");
                        }

                        if (string.IsNullOrWhiteSpace(research.targetPerson))
                        {
                            if (!missingFields.Contains("委託對象"))
                                missingFields.Add("「委託對象」");
                        }

                        // dateStart, dateEnd, price 可以為空或 0，不檢查
                    }

                    if (missingFields.Count > 0)
                    {
                        result.AddError($"明細表「3.技術移轉、委託研究或驗證費」：請輸入{string.Join("、", missingFields)}");
                    }
                }
            }

            // 5. 驗證國內差旅費（選填，但有填就檢查）
            if (travelData != null && travelData.Count > 0)
            {
                List<string> missingFields = new List<string>();
                foreach (var travel in travelData)
                {
                    // 所有行都必須填寫完整，包含空白行
                    if (string.IsNullOrWhiteSpace(travel.reason))
                    {
                        if (!missingFields.Contains("出差事由"))
                            missingFields.Add("「出差事由」");
                    }

                    if (string.IsNullOrWhiteSpace(travel.area))
                    {
                        if (!missingFields.Contains("地區"))
                            missingFields.Add("「地區」");
                    }

                    // days, people, price 可以為 0，不檢查
                }

                if (missingFields.Count > 0)
                {
                    result.AddError($"明細表「4-1.國內差旅費」：請輸入{string.Join("、", missingFields)}");
                }
            }

            // 6. 驗證國外差旅費（選填，但有填就檢查）
            if (foreignTravelData != null && foreignTravelData.Count > 0)
            {
                List<string> missingFields = new List<string>();
                foreach (var foreignTravel in foreignTravelData)
                {
                    // 所有行都必須填寫完整，包含空白行
                    if (string.IsNullOrWhiteSpace(foreignTravel.country))
                    {
                        if (!missingFields.Contains("擬前往國家或地區"))
                            missingFields.Add("「擬前往國家或地區」");
                    }

                    if (string.IsNullOrWhiteSpace(foreignTravel.topic))
                    {
                        if (!missingFields.Contains("主要會議議題"))
                            missingFields.Add("「主要會議議題」");
                    }

                    // days, people, transportFee, livingFee, conference 可以為空或 0，不檢查
                }

                if (missingFields.Count > 0)
                {
                    result.AddError($"明細表「4-2.國外差旅費」：請輸入{string.Join("、", missingFields)}");
                }
            }

            // ===== 經費總表驗證 =====

            // 驗證總費用資料是否存在
            if (totalFeesData == null || totalFeesData.Count == 0)
            {
                result.AddError("經費總表：請輸入有效的補助金額或合作金額");
            }
            else
            {
                // 檢查經費總表中的各項目
                foreach (var totalFee in totalFeesData)
                {
                    string itemName = totalFee.accountingItem ?? "";

                    // 如果是 OceanTech，跳過行政管理費和國外差旅費的檢查
                    if (isOceanTech)
                    {
                        if (itemName.Contains("行政管理費") ||
                            itemName.Contains("4-2") ||
                            itemName.Contains("國外差旅費"))
                        {
                            continue;
                        }
                    }

                    // 檢查補助金額和合作金額是否至少有一項有值
                    decimal subsidyAmt = totalFee.subsidyAmount ?? 0;
                    decimal coopAmt = totalFee.coopAmount ?? 0;

                    if (subsidyAmt < 0 && coopAmt < 0)
                    {
                        result.AddError($"經費總表：項目「{itemName}」的補助金額或合作金額至少需填寫一項");
                        break;
                    }
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
    /// 儲存表單資料
    /// </summary>
    /// <returns>儲存是否成功</returns>
    public bool SaveData()
    {
        try
        {
            // 取得表單資料
            var personnelData = GetPersonnelDataFromHidden();
            var materialData = GetMaterialDataFromHidden();
            var researchData = GetResearchDataFromHidden();
            var travelData = GetTravelDataFromHidden();
            var foreignTravelData = GetForeignTravelDataFromHidden();
            var otherData = GetOtherDataFromHidden();
            var otherRentData = GetOtherRentDataFromHidden();
            var totalFeesData = GetTotalFeesDataFromHidden();

            // 取得 OrgCategory 資訊
            var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(ProjectID);
            string orgCategory = applicationMain?.OrgCategory ?? "";
            bool isOceanTech = orgCategory.Equals("OceanTech", StringComparison.OrdinalIgnoreCase);

            // 如果是 OceanTech，將 totalFeesData 中的國外差旅費歸零
            if (isOceanTech && totalFeesData != null)
            {
                var foreignTravelFee = totalFeesData.FirstOrDefault(t =>
                    t.accountingItem?.Contains("4-2") == true ||
                    t.accountingItem?.Contains("國外差旅費") == true);
                if (foreignTravelFee != null)
                {
                    foreignTravelFee.subsidyAmount = 0;
                    foreignTravelFee.coopAmount = 0;
                }
            }

            // 儲存各類型資料
            if (personnelData != null && personnelData.Count > 0)
            {
                OFS_SciFundingHelper.ReplacePersonFormList(personnelData, ProjectID);
            }

            if (materialData != null && materialData.Count > 0)
            {
                OFS_SciFundingHelper.ReplaceMaterialList(materialData, ProjectID);
            }

            // 技術移轉、委託研究或驗證費：檢查是否只有一行且完全空白
            if (researchData != null && researchData.Count == 1)
            {
                var singleRow = researchData[0];
                bool isCompletelyEmpty = string.IsNullOrWhiteSpace(singleRow.category) &&
                                        string.IsNullOrWhiteSpace(singleRow.dateStart) &&
                                        string.IsNullOrWhiteSpace(singleRow.dateEnd) &&
                                        string.IsNullOrWhiteSpace(singleRow.projectName) &&
                                        string.IsNullOrWhiteSpace(singleRow.targetPerson) &&
                                        singleRow.price <= 0;

                if (isCompletelyEmpty)
                {
                    // 完全空白：傳空陣列，只執行 DELETE，不 INSERT
                    OFS_SciFundingHelper.ReplaceResearchFees(new List<ResearchFeeRow>(), ProjectID);
                }
                else
                {
                    // 有資料：正常儲存
                    OFS_SciFundingHelper.ReplaceResearchFees(researchData, ProjectID);
                }
            }
            else if (researchData != null && researchData.Count > 0)
            {
                // 多行：正常儲存
                OFS_SciFundingHelper.ReplaceResearchFees(researchData, ProjectID);
            }

            if (travelData != null && travelData.Count > 0)
            {
                OFS_SciFundingHelper.ReplaceTripForm(travelData, ProjectID);
            }

            // 國外差旅費：前端會根據 OrgCategory 決定是否收集資料
            // OceanTech 會傳空陣列，執行「先刪除」後不會插入，達到清除效果
            OFS_SciFundingHelper.ReplaceAbroadTripForm(foreignTravelData ?? new List<ForeignTravelRow>(), ProjectID);
            OFS_SciFundingHelper.ReplaceOtherPersonFee(otherData, ProjectID);
            

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
        hdnForeignTravelData.Value = "[]";
        hdnOtherData.Value = "[]";
        hdnOtherRentData.Value = "[]";
        hdnTotalFeesData.Value = "[]";
    }

    /// <summary>
    /// 取得並傳遞補助款上限和配合款比例到前端
    /// </summary>
    private void LoadGrantLimitAndMatchingFund(string projectID)
    {
        try
        {
            // 取得 OrgCategory
            var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);
            if (applicationMain == null) return;

            string orgCategory = applicationMain.OrgCategory ?? "";

            // 根據 OrgCategory 決定 TargetTypeID
            string targetTypeID = "";
            switch (orgCategory)
            {
                case "Academic":
                    targetTypeID = "SCI1";
                    break;
                case "Legal":
                    targetTypeID = "SCI2";
                    break;
                case "OceanTech":
                    targetTypeID = "SCI3";
                    break;
                default:
                    return; // 無法判斷，不傳遞資料
            }

            // 取得補助款上限和配合款比例
            var grantTargetSetting = OFSGrantTargetSettingHelper.getByTargetTypeID(targetTypeID);
            if (grantTargetSetting == null) return;

            decimal grantLimit = grantTargetSetting.GrantLimit ?? 0;
            decimal matchingFund = grantTargetSetting.MatchingFund ?? 0;

            // 將資料傳遞到前端 JavaScript
            StringBuilder jsBuilder = new StringBuilder();
            jsBuilder.AppendLine("window.grantLimitSettings = {");
            jsBuilder.AppendLine($"    grantLimit: {grantLimit},"); // 單位：萬元
            jsBuilder.AppendLine($"    matchingFund: {matchingFund}"); // 單位：百分比
            jsBuilder.AppendLine("};");

            Page.ClientScript.RegisterStartupScript(
                this.GetType(),
                "GrantLimitSettings",
                jsBuilder.ToString(),
                true
            );
        }
        catch (Exception ex)
        {
            HandleException(ex, "取得補助款上限和配合款比例時發生錯誤");
        }
    }

    /// <summary>
    /// 綁定下拉選單
    /// </summary>
    private void BindDropDown()
    {
        try
        {
            // 取得 OrgCategory 來判斷使用哪個 CodeGroup
            var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(ProjectID);
            string orgCategory = applicationMain?.OrgCategory ?? "";
            bool isOceanTech = orgCategory.Equals("OceanTech", StringComparison.OrdinalIgnoreCase);
            string PersonCodeGroup = isOceanTech ? "SCIPersonIndustry" : "SCIPersonAcademic";
            var personList = OFS_SciFundingHelper.GetSysZgsCodeByCodeGroup(PersonCodeGroup);

            StringBuilder jsBuilder = new StringBuilder();
            jsBuilder.AppendLine("const ddlPersonOptions = [");

            foreach (var item in personList)
            {
                // 將 MaxPriceLimit 也輸出成 JS 物件屬性
                jsBuilder.AppendLine(
                    $"    {{ text: \"{item.Descname}\", value: \"{item.Code}\", maxLimit: {item.MaxPriceLimit} }},");
            }

            jsBuilder.AppendLine("];");

            // 材料單位下拉選單
            var materialUnitList = OFS_SciFundingHelper.GetSysZgsCodeByCodeGroup("SCIMaterialUnit");
            jsBuilder.AppendLine("const ddlMaterialOptions = [");
            foreach (var item in materialUnitList)
            {
                jsBuilder.AppendLine(
                    $"    {{ text: \"{item.Descname}\", value: \"{item.Code}\", maxLimit: {item.MaxPriceLimit} }},");
            }

            jsBuilder.AppendLine("];");

            // 其他業務費職稱下拉選單 - 根據 OrgCategory 決定使用 Academic 或 Industry
            string otherCodeGroup = isOceanTech ? "SCIOtherIndustry" : "SCIOtherAcademic";
            var otherJobList = OFS_SciFundingHelper.GetSysZgsCodeByCodeGroup(otherCodeGroup);
            jsBuilder.AppendLine("const ddlOtherOptions = [");
            foreach (var item in otherJobList)
            {
                jsBuilder.AppendLine(
                    $"    {{ text: \"{item.Descname}\", value: \"{item.Code}\", maxLimit: {item.MaxPriceLimit} }},");
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
            var foreignTravelData = OFS_SciFundingHelper.GetAbroadTripFormList(projectID);
            var otherData = OFS_SciFundingHelper.GetOtherPersonFeeList(projectID);
            var otherRentData = OFS_SciFundingHelper.GetOtherObjectFeeList(projectID);
            var totalFeesData = OFS_SciFundingHelper.GetTotalFeeList(projectID);

            // 取得 OrgCategory 資訊
            var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);
            string orgCategory = applicationMain?.OrgCategory ?? "";
            bool isOceanTech = orgCategory.Equals("OceanTech", StringComparison.OrdinalIgnoreCase);

            // 如果是 OceanTech，將 totalFeesData 中的行政管理費和國外差旅費歸零
            if (isOceanTech && totalFeesData != null)
            {
                var adminFee = totalFeesData.FirstOrDefault(t => t.accountingItem?.Contains("行政管理費") == true);
                if (adminFee != null)
                {
                    adminFee.subsidyAmount = 0;
                    adminFee.coopAmount = 0;
                }

                var foreignTravelFee = totalFeesData.FirstOrDefault(t =>
                    t.accountingItem?.Contains("4-2") == true ||
                    t.accountingItem?.Contains("國外差旅費") == true);
                if (foreignTravelFee != null)
                {
                    foreignTravelFee.subsidyAmount = 0;
                    foreignTravelFee.coopAmount = 0;
                }
            }

            // 將資料轉換為 JSON 並輸出到前端
            var serializer = new JavaScriptSerializer();

            StringBuilder jsBuilder = new StringBuilder();
            jsBuilder.AppendLine("window.loadedData = {");
            jsBuilder.AppendLine($"    personnel: {serializer.Serialize(personnelData)},");
            jsBuilder.AppendLine($"    material: {serializer.Serialize(materialData)},");
            jsBuilder.AppendLine($"    research: {serializer.Serialize(researchData)},");
            jsBuilder.AppendLine($"    travel: {serializer.Serialize(travelData)},");
            jsBuilder.AppendLine($"    foreignTravel: {serializer.Serialize(foreignTravelData)},");
            jsBuilder.AppendLine($"    other: {serializer.Serialize(otherData)},");
            jsBuilder.AppendLine($"    otherRent: {serializer.Serialize(otherRentData)},");
            jsBuilder.AppendLine($"    totalFees: {serializer.Serialize(totalFeesData)},");
            jsBuilder.AppendLine($"    orgCategory: '{orgCategory}'");
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
    private void LoadFormControls(List<ResearchFeeRow> researchData, List<OtherRent> otherRentData,
        List<TotalFeeRow> totalFeesData)
    {
        try
        {
            // 研究費資料已改為動態行，由前端 JavaScript 處理，不需要在此載入

            // 載入租金資料到 ASP.NET 控件
            var rentData = otherRentData.FirstOrDefault(r => r.item == "租金");
            if (rentData != null)
            {
                rentCash.Text = rentData.amount.ToString();
                rentDescription.Text = rentData.note;
            }

            // 載入行政管理費資料到 ASP.NET 控件
            // 判斷 OrgCategory，如果是 OceanTech 則強制歸零
            var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(ProjectID);
            string orgCategory = applicationMain?.OrgCategory ?? "";
            bool isOceanTech = orgCategory.Equals("OceanTech", StringComparison.OrdinalIgnoreCase);

            var adminFeeData = totalFeesData.FirstOrDefault(t => t.accountingItem?.Contains("行政管理費") == true);
            if (isOceanTech)
            {
                // OceanTech 類型：強制歸零
                AdminFeeSubsidy.Text = "0";
                AdminFeeCoop.Text = "0";
            }
            else if (adminFeeData != null)
            {
                // 非 OceanTech：載入實際資料
                AdminFeeSubsidy.Text = adminFeeData.subsidyAmount?.ToString() ?? "0";
                AdminFeeCoop.Text = adminFeeData.coopAmount?.ToString() ?? "0";
            }
            else
            {
                // 沒有資料時預設為 0
                AdminFeeSubsidy.Text = "0";
                AdminFeeCoop.Text = "0";
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入表單控制項資料時發生錯誤");
        }
    }


    protected void btnTempSave_Click(object sender, EventArgs e)
    {
        try
        {
            // 暫存不需要檢核，直接儲存
            if (SaveData())
            {
                // 儲存變更說明
                // tab3_ucChangeDescription.SaveChangeDescription(ProjectID);
                // 更新版本狀態（暫存）
                if (!string.IsNullOrEmpty(ProjectID))
                {
                    UpdateVersionStatusBasedOnAction(ProjectID, false);
                }

                // 重新載入資料
                string isViewMode = Request.QueryString["IsViewMode"];
                LoadData(ProjectID);

                ShowSuccessMessage("暫存成功！");
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"暫存失敗：{ex.Message}");
        }
    }

    protected void btnSaveAndNext_Click(object sender, EventArgs e)
    {
        try
        {
            // PostBack 時，檢查是否是驗證按鈕觸發的 PostBack
            // 如果是「完成本頁，下一步」按鈕觸發，在按鈕事件執行前先保存當前資料
            SaveDataToSessionBeforeValidation();
            // 驗證 UserControl 資料
            var validationResult = ValidateForm();
            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.GetErrorsAsString();
                var fullMessage = $"請修正以下錯誤：<br>{errorMessages}<br><br>若無使用經費，請輸入「無」";
                    ShowErrorMessage(fullMessage);
                RestoreDataFromSession();
                return;
            }

            // 儲存 UserControl 資料
            if (SaveData())
            {
                UpdateVersionStatusBasedOnAction(ProjectID, true);
                // 儲存變更說明
                tab3_ucChangeDescription.SaveChangeDescription(ProjectID);
                RestoreDataFromSession();

                // 清除 Session 暫存資料（因為已成功儲存）
                ClearSessionData();

                // 判斷當前頁面是否為 SciInprogress_Approved.aspx
                string currentPage = System.IO.Path.GetFileName(Request.Url.AbsolutePath);
                string redirectUrl = currentPage != "SciInprogress_Approved.aspx"
                    ? $"SciRecusedList.aspx?ProjectID={ProjectID}"
                    : "";

                // 顯示成功訊息（如果有 URL 則 1 秒後跳轉）
                ShowSuccessMessage("儲存成功", redirectUrl);
            }
        }
        catch (Exception ex)
        {
            RestoreDataFromSession();
            ShowErrorMessage($"儲存失敗：{ex.Message}");
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
                            subsidyAmount =
                                decimal.TryParse(item["subsidyAmount"]?.ToString(), out decimal subsidyAmount)
                                    ? subsidyAmount
                                    : 0,
                            coopAmount = decimal.TryParse(item["coopAmount"]?.ToString(), out decimal coopAmount)
                                ? coopAmount
                                : 0
                        };

                        totalFeesList.Add(totalFee);
                    }
                }
            }

            // 取得 OrgCategory 來判斷是否需要強制設置行政管理費為 0
            var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(ProjectID);
            string orgCategory = applicationMain?.OrgCategory ?? "";
            bool isOceanTech = orgCategory.Equals("OceanTech", StringComparison.OrdinalIgnoreCase);

            // 確保包含行政管理費資料（從 totalFeesList 中取得，而非直接從表單控制項）
            // 當 OrgCategory 為 OceanTech 時，行政管理費金額設為 0
            decimal adminSubsidyAmount = 0;
            decimal adminCoopAmount = 0;

            // 先從 totalFeesList 中查找是否已有行政管理費資料
            var existingAdminFee = totalFeesList.FirstOrDefault(t => t.accountingItem?.Contains("行政管理費") == true);

            if (!isOceanTech)
            {
                // 非 OceanTech 時，從 totalFeesList 取得金額（來自前端隱藏欄位）
                if (existingAdminFee != null)
                {
                    adminSubsidyAmount = existingAdminFee.subsidyAmount ?? 0;
                    adminCoopAmount = existingAdminFee.coopAmount ?? 0;
                }
                
            }
            // 如果是 OceanTech，adminSubsidyAmount 和 adminCoopAmount 保持為 0

            // 更新或新增行政管理費記錄
            if (existingAdminFee != null)
            {
                existingAdminFee.subsidyAmount = adminSubsidyAmount;
                existingAdminFee.coopAmount = adminCoopAmount;
            }
            else
            {
                totalFeesList.Add(new TotalFeeRow
                {
                    accountingItem = "行政管理費",
                    subsidyAmount = adminSubsidyAmount,
                    coopAmount = adminCoopAmount
                });
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "解析總費用資料時發生錯誤");
        }

        return totalFeesList;
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
                // 1. Form3Status 設為 "完成" 
                // 2. 檢查 CurrentStep，如果 <= 3 則改成 4

                string currentStep = OFS_SciWorkSchHelper.GetCurrentStepByProjectID(ProjectID);
                int currentStepNum = 1;
                int.TryParse(currentStep, out currentStepNum);

                bool shouldUpdateCurrentStep = currentStepNum <= 3;
                string newCurrentStep = shouldUpdateCurrentStep ? "4" : currentStep;

                // 更新 Form3Status 為 "完成" 和 CurrentStep (如果需要)
                // 使用通用的版本狀態更新方法，針對 Form3
                if (shouldUpdateCurrentStep)
                {
                    OFS_SciFundingHelper.UpdateForm3StatusAndCurrentStep(ProjectID, "完成", newCurrentStep);
                }
                else
                {
                    OFS_SciFundingHelper.UpdateForm3Status(ProjectID, "完成");
                }
            }
            else
            {
                // 點擊「暫存」按鈕
                // 只更新 Form3Status 為 "暫存"，CurrentStep 不變

                OFS_SciFundingHelper.UpdateForm3Status(ProjectID, "暫存");
            }
        }
        catch (Exception ex)
        {
            // 記錄錯誤但不中斷流程
            System.Diagnostics.Debug.WriteLine($"更新狀態失敗: {ex.Message}");
        }
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
                materialList = serializer.Deserialize<List<MaterialRow>>(hdnMaterialData.Value) ??
                               new List<MaterialRow>();
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "解析材料費資料時發生錯誤");
        }

        return materialList;
    }

    /// <summary>
    /// 從隱藏欄位取得研究費資料（支援動態行數）
    /// </summary>
    private List<ResearchFeeRow> GetResearchDataFromHidden()
    {
        var researchList = new List<ResearchFeeRow>();
        try
        {
            // 從隱藏欄位讀取 JSON 資料
            if (!string.IsNullOrEmpty(hdnResearchData.Value))
            {
                var serializer = new JavaScriptSerializer();
                researchList = serializer.Deserialize<List<ResearchFeeRow>>(hdnResearchData.Value) ?? new List<ResearchFeeRow>();
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
    /// 從隱藏欄位取得國外差旅費資料
    /// </summary>
    private List<ForeignTravelRow> GetForeignTravelDataFromHidden()
    {
        var foreignTravelList = new List<ForeignTravelRow>();
        try
        {
            if (!string.IsNullOrEmpty(hdnForeignTravelData.Value))
            {
                var serializer = new JavaScriptSerializer();
                foreignTravelList = serializer.Deserialize<List<ForeignTravelRow>>(hdnForeignTravelData.Value) ?? new List<ForeignTravelRow>();
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "解析國外差旅費資料時發生錯誤");
        }

        return foreignTravelList;
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
                otherRentList = serializer.Deserialize<List<OtherRent>>(hdnOtherRentData.Value) ??
                                new List<OtherRent>();
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
            string script = @"
            <script>
             $(document).ready(function () {
                // 只針對此 UserControl 內的元素進行鎖定
                // 找到 tab3 容器（經費/人事費明細）
                var userControl = document.querySelector('#tab3');

                if (!userControl) {
                    console.warn('找不到 UserControl 容器: #tab3');
                    return;
                }

                // 創建一個函數來處理此 UserControl 內所有表單元素的禁用
                function disableAllFormElements() {
                    var allElements = userControl.querySelectorAll('input, textarea, select, button');
                    allElements.forEach(function(element) {
                        element.disabled = true;
                        element.readOnly = true;
                    });

                    // 處理 view-mode 元素
                    var viewModeElements = userControl.querySelectorAll('.view-mode');
                    viewModeElements.forEach(function(element) {
                        if (!element.classList.contains('d-none')) {
                            element.classList.add('d-none');
                        }
                    });
                }

                // 初始執行
                disableAllFormElements();

                // 特別處理一些可能動態生成的元素 - 多次檢查
                setTimeout(disableAllFormElements, 1000);
                setTimeout(disableAllFormElements, 2000);
                setTimeout(disableAllFormElements, 3000);

                // 使用 MutationObserver 監聽此 UserControl 內的 DOM 變化
                var observer = new MutationObserver(function(mutations) {
                    var shouldDisable = false;
                    mutations.forEach(function(mutation) {
                        if (mutation.type === 'childList' && mutation.addedNodes.length > 0) {
                            for (var i = 0; i < mutation.addedNodes.length; i++) {
                                var node = mutation.addedNodes[i];
                                if (node.nodeType === 1) { // Element node
                                    var hasFormElements = node.querySelectorAll &&
                                        node.querySelectorAll('input, textarea, select, button').length > 0;
                                    if (hasFormElements ||
                                        (node.tagName && ['INPUT', 'TEXTAREA', 'SELECT', 'BUTTON'].includes(node.tagName))) {
                                        shouldDisable = true;
                                        break;
                                    }
                                }
                            }
                        }
                    });

                    if (shouldDisable) {
                        setTimeout(disableAllFormElements, 100);
                    }
                });

                // 只監聽此 UserControl 的變化
                observer.observe(userControl, {
                    childList: true,
                    subtree: true
                });

                // 處理表格欄位隱藏
                $('#tab3 #point1Table, #tab3 #point2Table, #tab3 #travelTable, #tab3 #otherTable,#point3Table,#foreignTravelTable').addClass('hide-col-last');
                });
            </script>";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "AddClassToTable", script);
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

        Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowSuccessMessage" + Guid.NewGuid().ToString(),
            script, true);
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
                html: '<div style=""text-align: left;"">{safeMessage}</div>',
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

        Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowErrorMessage" + Guid.NewGuid().ToString(), script,
            true);
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

        Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowWarningMessage" + Guid.NewGuid().ToString(),
            script, true);
    }

    /// <summary>
    /// 在按鈕事件執行前保存當前表單資料到 Session（在 Page_Load 中呼叫）
    /// </summary>
    private void SaveDataToSessionBeforeValidation()
    {
        try
        {
            string sessionKey = $"SciFundingControl_Temp_{ProjectID}";

            // 取得表單資料（與 SaveData() 中相同的方法）
            var personnelData = GetPersonnelDataFromHidden();
            var materialData = GetMaterialDataFromHidden();
            var researchData = GetResearchDataFromHidden();
            var travelData = GetTravelDataFromHidden();
            var foreignTravelData = GetForeignTravelDataFromHidden();
            var otherData = GetOtherDataFromHidden();
            var otherRentData = GetOtherRentDataFromHidden();
            var totalFeesData = GetTotalFeesDataFromHidden();

            // 建立暫存資料物件
            var tempData = new
            {
                // 從前端收集到的實際資料物件
                PersonnelData = personnelData,
                MaterialData = materialData,
                ResearchData = researchData,
                TravelData = travelData,
                ForeignTravelData = foreignTravelData,
                OtherData = otherData,
                OtherRentData = otherRentData,
                TotalFeesData = totalFeesData,

                // ASP.NET 控制項資料（僅保留固定欄位）
                // 研究費資料已改為動態行，不需要在此保存 ASP.NET 控制項資料

                RentCash = rentCash.Text,
                RentDescription = rentDescription.Text,

                AdminFeeSubsidy = AdminFeeSubsidy.Text,
                AdminFeeCoop = AdminFeeCoop.Text
            };

            // 將資料序列化並保存到 Session
            Session[sessionKey] = JsonConvert.SerializeObject(tempData);
        }
        catch (Exception ex)
        {
            // 記錄錯誤但不中斷流程
            System.Diagnostics.Debug.WriteLine($"保存 Session 資料失敗: {ex.Message}");
        }
    }


    /// <summary>
    /// 從 Session 恢復表單資料（用於 PostBack 時恢復）
    /// </summary>
    private void RestoreDataFromSession()
    {
        try
        {
            string sessionKey = $"SciFundingControl_Temp_{ProjectID}";

            if (Session[sessionKey] != null)
            {
                string sessionDataJson = Session[sessionKey].ToString();

                // 解析暫存的資料
                dynamic tempDataDynamic = JsonConvert.DeserializeObject(sessionDataJson);

                if (tempDataDynamic != null)
                {
                    // 重建實際資料物件
                    var personnelData = (tempDataDynamic.PersonnelData != null)
                        ? JsonConvert.DeserializeObject<List<PersonRow>>(tempDataDynamic.PersonnelData.ToString())
                        : new List<PersonRow>();
                    var materialData = (tempDataDynamic.MaterialData != null)
                        ? JsonConvert.DeserializeObject<List<MaterialRow>>(tempDataDynamic.MaterialData.ToString())
                        : new List<MaterialRow>();
                    var researchData = (tempDataDynamic.ResearchData != null)
                        ? JsonConvert.DeserializeObject<List<ResearchFeeRow>>(tempDataDynamic.ResearchData.ToString())
                        : new List<ResearchFeeRow>();
                    var travelData = (tempDataDynamic.TravelData != null)
                        ? JsonConvert.DeserializeObject<List<TravelRow>>(tempDataDynamic.TravelData.ToString())
                        : new List<TravelRow>();
                    var foreignTravelData = (tempDataDynamic.ForeignTravelData != null)
                        ? JsonConvert.DeserializeObject<List<ForeignTravelRow>>(tempDataDynamic.ForeignTravelData.ToString())
                        : new List<ForeignTravelRow>();
                    var otherData = (tempDataDynamic.OtherData != null)
                        ? JsonConvert.DeserializeObject<List<OtherFeeRow>>(tempDataDynamic.OtherData.ToString())
                        : new List<OtherFeeRow>();
                    var otherRentData = (tempDataDynamic.OtherRentData != null)
                        ? JsonConvert.DeserializeObject<List<OtherRent>>(tempDataDynamic.OtherRentData.ToString())
                        : new List<OtherRent>();
                    var totalFeesData = (tempDataDynamic.TotalFeesData != null)
                        ? JsonConvert.DeserializeObject<List<TotalFeeRow>>(tempDataDynamic.TotalFeesData.ToString())
                        : new List<TotalFeeRow>();

                    // 使用與 LoadExistingData 相同的方式輸出前端資料
                    var serializer = new JavaScriptSerializer();

                    StringBuilder jsBuilder = new StringBuilder();
                    jsBuilder.AppendLine("window.restoredData = {");
                    jsBuilder.AppendLine($"    personnel: {serializer.Serialize(personnelData)},");
                    jsBuilder.AppendLine($"    material: {serializer.Serialize(materialData)},");
                    jsBuilder.AppendLine($"    research: {serializer.Serialize(researchData)},");
                    jsBuilder.AppendLine($"    travel: {serializer.Serialize(travelData)},");
                    jsBuilder.AppendLine($"    foreignTravel: {serializer.Serialize(foreignTravelData)},");
                    jsBuilder.AppendLine($"    other: {serializer.Serialize(otherData)},");
                    jsBuilder.AppendLine($"    otherRent: {serializer.Serialize(otherRentData)},");
                    jsBuilder.AppendLine($"    totalFees: {serializer.Serialize(totalFeesData)}");
                    jsBuilder.AppendLine("};");

                    // 使用與 LoadExistingData 相同的方式載入 ASP.NET 控制項
                    RestoreFormControls(researchData, otherRentData, totalFeesData, tempDataDynamic);

                    // 使用與 LoadExistingData 相同的前端觸發方式
                    jsBuilder.AppendLine();
                    jsBuilder.AppendLine("// 恢復資料到前端表單");
                    jsBuilder.AppendLine("if (document.readyState === 'loading') {");
                    jsBuilder.AppendLine("    document.addEventListener('DOMContentLoaded', function() {");
                    jsBuilder.AppendLine("        setTimeout(function() {");
                    jsBuilder.AppendLine("            if (typeof restoreDataFromPostback === 'function') {");
                    jsBuilder.AppendLine("                restoreDataFromPostback();");
                    jsBuilder.AppendLine("            } else if (typeof loadExistingDataToForm === 'function') {");
                    jsBuilder.AppendLine("                // 使用現有的載入函數，將 restoredData 設為 loadedData");
                    jsBuilder.AppendLine("                window.loadedData = window.restoredData;");
                    jsBuilder.AppendLine("                loadExistingDataToForm();");
                    jsBuilder.AppendLine("            }");
                    jsBuilder.AppendLine("        }, 100);");
                    jsBuilder.AppendLine("    });");
                    jsBuilder.AppendLine("} else {");
                    jsBuilder.AppendLine("    setTimeout(function() {");
                    jsBuilder.AppendLine("        if (typeof restoreDataFromPostback === 'function') {");
                    jsBuilder.AppendLine("            restoreDataFromPostback();");
                    jsBuilder.AppendLine("        } else if (typeof loadExistingDataToForm === 'function') {");
                    jsBuilder.AppendLine("            // 使用現有的載入函數，將 restoredData 設為 loadedData");
                    jsBuilder.AppendLine("            window.loadedData = window.restoredData;");
                    jsBuilder.AppendLine("            loadExistingDataToForm();");
                    jsBuilder.AppendLine("        }");
                    jsBuilder.AppendLine("    }, 100);");
                    jsBuilder.AppendLine("}");

                    Page.ClientScript.RegisterStartupScript(
                        this.GetType(),
                        "RestoreDataScript",
                        jsBuilder.ToString(),
                        true
                    );
                }
            }
        }
        catch (Exception ex)
        {
            // 記錄錯誤但不中斷流程
            System.Diagnostics.Debug.WriteLine($"從 Session 恢復資料失敗: {ex.Message}");
        }
    }

    /// <summary>
    /// 恢復 ASP.NET 控制項資料（仿照 LoadFormControls 的方式）
    /// </summary>
    private void RestoreFormControls(List<ResearchFeeRow> researchData, List<OtherRent> otherRentData,
        List<TotalFeeRow> totalFeesData, dynamic tempDataDynamic)
    {
        try
        {
            // 研究費資料已改為動態行，由前端 JavaScript 處理（透過 window.restoredData.research）

            // 載入租金資料到 ASP.NET 控件
            var rentData = otherRentData.FirstOrDefault(r => r.item == "租金");
            if (rentData != null)
            {
                rentCash.Text = rentData.amount.ToString();
                rentDescription.Text = rentData.note ?? "";
            }
            else
            {
                // 如果資料物件中沒有，從 tempDataDynamic 恢復
                if (tempDataDynamic.RentCash != null)
                    rentCash.Text = tempDataDynamic.RentCash.ToString();
                if (tempDataDynamic.RentDescription != null)
                    rentDescription.Text = tempDataDynamic.RentDescription.ToString();
            }

            // 載入行政管理費資料到 ASP.NET 控件
            var adminFeeData = totalFeesData.FirstOrDefault(t => t.accountingItem?.Contains("行政管理費") == true);
            if (adminFeeData != null)
            {
                AdminFeeSubsidy.Text = adminFeeData.subsidyAmount?.ToString() ?? "0";
                AdminFeeCoop.Text = adminFeeData.coopAmount?.ToString() ?? "0";
            }
            else
            {
                // 如果資料物件中沒有，從 tempDataDynamic 恢復
                if (tempDataDynamic.AdminFeeSubsidy != null)
                    AdminFeeSubsidy.Text = tempDataDynamic.AdminFeeSubsidy.ToString();
                if (tempDataDynamic.AdminFeeCoop != null)
                    AdminFeeCoop.Text = tempDataDynamic.AdminFeeCoop.ToString();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"恢復控制項資料失敗: {ex.Message}");
        }
    }

    /// <summary>
    /// 清除 Session 暫存資料
    /// </summary>
    private void ClearSessionData()
    {
        try
        {
            string sessionKey = $"SciFundingControl_Temp_{ProjectID}";
            Session.Remove(sessionKey);
        }
        catch (Exception ex)
        {
            // 記錄錯誤但不中斷流程
            System.Diagnostics.Debug.WriteLine($"清除 Session 資料失敗: {ex.Message}");
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
                return tab3_ucChangeDescription.GetChangeDescriptionBySourcePage(ProjectID, "SciFunding");
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
}