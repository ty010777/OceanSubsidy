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
using GS.OCA_OceanSubsidy.Operation.OFS;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.X509.SigI;

public partial class OFS_SciFunding : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // 設定顯示模式
            SetDisplayMode();
            
            if (!IsPostBack)
            {
                LoadExistingData();
                CheckFormStatusAndHideTempSaveButton();
            }
            
            BindDropDown();
        }
        catch (Exception ex)
        {
            // 發生錯誤時記錄但不中斷頁面載入
            System.Diagnostics.Debug.WriteLine($"頁面載入錯誤：{ex.Message}");
        }
    }

    private void BindDropDown()
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
        
        
        var MList = OFS_SciFundingHelper.GetSysZgsCodeByCodeGroup("SCIMaterialUnit");
        jsBuilder.AppendLine("const ddlMaterialOptions = [");

        foreach (var item in MList)
        {
            // 將 MaxPriceLimit 也輸出成 JS 物件屬性
            jsBuilder.AppendLine($"    {{ text: \"{item.Descname}\", value: \"{item.Code}\", maxLimit: {item.MaxPriceLimit} }},");
        }

        jsBuilder.AppendLine("];");
        var OList = OFS_SciFundingHelper.GetSysZgsCodeByCodeGroup("SCIOtherAcademic");
        jsBuilder.AppendLine("const ddlOtherOptions = [");

        foreach (var item in OList)
        {
            // 將 MaxPriceLimit 也輸出成 JS 物件屬性
            jsBuilder.AppendLine($"    {{ text: \"{item.Descname}\", value: \"{item.Code}\", maxLimit: {item.MaxPriceLimit} }},");
        }

        jsBuilder.AppendLine("];");

        Page.ClientScript.RegisterStartupScript(
            this.GetType(),
            "ddlOthersScript",
            jsBuilder.ToString(),
            true
        );
    }
    
    private void LoadExistingData()
    {
        try
        {
            string projectId = Request.QueryString["ProjectID"];
            if (string.IsNullOrEmpty(projectId))
            {
                return; // 如果沒有 ProjectID，則不載入資料
            }
            
        
            // 載入各類型資料
            var personnelData = OFS_SciFundingHelper.GetPersonFormList(projectId);
            var materialData = OFS_SciFundingHelper.GetMaterialList(projectId);
            var researchData = OFS_SciFundingHelper.GetResearchFeesList(projectId);
            var travelData = OFS_SciFundingHelper.GetTripFormList(projectId);
            var otherData = OFS_SciFundingHelper.GetOtherPersonFeeList(projectId);
            var otherRentData = OFS_SciFundingHelper.GetOtherObjectFeeList(projectId);
            var totalFeesData = OFS_SciFundingHelper.GetTotalFeeList(projectId);
            
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
            
            // 注意：研究費資料由前端 loadResearchData() 處理，避免重複載入
            
            // 載入租金資料到 ASP.NET 控件（固定欄位，不會被前端清除）
            var rentData = otherRentData.FirstOrDefault(r => r.item == "租金");
            if (rentData != null)
            {
                rentCash.Text = rentData.amount.ToString();
                rentDescription.Text = rentData.note;
            }
            
            // 載入行政管理費資料到 ASP.NET 控件
            var adminFeeData = totalFeesData.FirstOrDefault(t => t.accountingItem.Contains("行政管理費"));
            if (adminFeeData != null)
            {
                AdminFeeSubsidy.Text = adminFeeData.subsidyAmount.ToString();
                AdminFeeCoop.Text = adminFeeData.coopAmount.ToString();
            }
            
            // 注意：人員、材料、差旅費、其他人事費等動態表格不需要預先載入資料到 ASP.NET 控件
            // 因為前端 JavaScript 會全部清除並重新建立所有行
            
            Page.ClientScript.RegisterStartupScript(
                this.GetType(),
                "LoadedDataScript",
                jsBuilder.ToString(),
                true
            );
        }
        catch (Exception ex)
        {
            // 載入資料失敗時不中斷頁面載入，只記錄錯誤
            Page.ClientScript.RegisterStartupScript(
                this.GetType(),
                "LoadDataError",
                $"console.error('載入資料失敗: {ex.Message}');",
                true
            );
        }
    }
    
    
    protected void btnTempSave_Click(object sender, EventArgs e)
    {
        try
        {
            SaveFormData();
            
            // 更新版本狀態（暫存）
            string projectId = Request.QueryString["ProjectID"];
            if (!string.IsNullOrEmpty(projectId))
            {
            
                UpdateVersionStatusBasedOnAction(projectId, false);
            }
            
            // 暫時儲存後重新載入資料
            LoadExistingData();
            // 可以顯示成功訊息但不跳轉
            Page.ClientScript.RegisterStartupScript(this.GetType(), "SaveSuccess", "alert('暫存成功！');", true);
        }
        catch (Exception ex)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "SaveError", $"alert('暫存失敗：{ex.Message}');", true);
        }
    }
    
    protected void btnSaveAndNext_Click(object sender, EventArgs e)
    {
        try
        {
            SaveFormData();
            
            // 更新版本狀態
            string ProjectID = Request.QueryString["ProjectID"];
            if (!string.IsNullOrEmpty(ProjectID))
            {
                
                    UpdateVersionStatusBasedOnAction(ProjectID, true);
            }
            
            
            // 儲存成功後跳轉到下一頁
            if (!string.IsNullOrEmpty(ProjectID))
            {
                Response.Redirect($"SciOutcomes.aspx?ProjectID={ProjectID}");
            }
            else
            {
                Response.Redirect("SciOutcomes.aspx");
            }
        }
        catch (Exception ex)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "SaveError", $"alert('儲存失敗：{ex.Message}');", true);
        }
    }
    
    private void SaveFormData()
    {
        string ProjectID = Request.QueryString["ProjectID"];
        
      
        
        // 收集人員資料
        var personnelData = CollectPersonnelData();
        if (personnelData.Count > 0)
        {
            OFS_SciFundingHelper.ReplacePersonFormList(personnelData, ProjectID);
        }
        
        // 收集材料資料
        var materialData = CollectMaterialData();
        if (materialData.Count > 0)
        {
            OFS_SciFundingHelper.ReplaceMaterialList(materialData, ProjectID);
        }
        
        // 收集研究費資料
        var researchData = CollectResearchData();
        if (researchData.Count > 0)
        {
            OFS_SciFundingHelper.ReplaceResearchFees(researchData, ProjectID);
        }
        
        // 收集差旅費資料
        var travelData = CollectTravelData();
        if (travelData.Count > 0)
        {
            OFS_SciFundingHelper.ReplaceTripForm(travelData, ProjectID);
        }
        
        // 收集其他人事費資料
        var otherData = CollectOtherData();
        if (otherData.Count > 0)
        {
            OFS_SciFundingHelper.ReplaceOtherPersonFee(otherData, ProjectID);
        }
        
        // 收集其他租金與勞務資料
        var otherRentData = CollectOtherRentData();
        if (otherRentData.Count > 0)
        {
            OFS_SciFundingHelper.ReplaceOtherObjectFee(otherRentData, ProjectID);
        }
        
        // 收集經費總表資料
        var totalFeesData = CollectTotalFeesData();
        if (totalFeesData.Count > 0)
        {
            OFS_SciFundingHelper.ReplaceTotalFeeList(totalFeesData, ProjectID);
        }
        
        
    }
    
    private List<PersonRow> CollectPersonnelData()
    {
        var result = new List<PersonRow>();
        
        try
        {
            // 從隱藏欄位讀取動態資料
            if (!string.IsNullOrEmpty(hdnPersonnelData.Value))
            {
                // 調試信息
                System.Diagnostics.Debug.WriteLine($"Personnel JSON: {hdnPersonnelData.Value}");
                
                var serializer = new JavaScriptSerializer();
                var dynamicData = serializer.Deserialize<List<PersonRow>>(hdnPersonnelData.Value);
                if (dynamicData != null)
                {
                    result.AddRange(dynamicData);
                    System.Diagnostics.Debug.WriteLine($"Personnel data count: {dynamicData.Count}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Personnel hidden field is empty");
            }
            
            // 如果沒有動態資料，則收集第一行的 ASP.NET 控件資料
            if (result.Count == 0 && !string.IsNullOrEmpty(personName1.Text))
            {
                result.Add(new PersonRow
                {
                    name = personName1.Text,
                    stay = stay1.Checked,
                    title = "", // 需要從前端 JavaScript 收集
                    salary = ParseDecimal(personSalary1.Text),
                    months = ParseDecimal(personMonths1.Text)
                });
            }
        }
        catch (Exception ex)
        {
            // 如果 JSON 解析失敗，記錄錯誤但不中斷程序
            // 可以考慮記錄到日誌
        }
        
        return result;
    }
    
    private List<MaterialRow> CollectMaterialData()
    {
        var result = new List<MaterialRow>();
        
        try
        {
            // 從隱藏欄位讀取動態資料
            if (!string.IsNullOrEmpty(hdnMaterialData.Value))
            {
                var serializer = new JavaScriptSerializer();
                var dynamicData = serializer.Deserialize<List<MaterialRow>>(hdnMaterialData.Value);
                if (dynamicData != null)
                {
                    result.AddRange(dynamicData);
                }
            }
            
            // 如果沒有動態資料，則收集第一行的 ASP.NET 控件資料
            if (result.Count == 0 && !string.IsNullOrEmpty(MaterialName1.Text))
            {
                result.Add(new MaterialRow
                {
                    name = MaterialName1.Text,
                    description = MaterialDescription1.Text,
                    unit = "", // 需要從前端 JavaScript 收集
                    quantity = ParseDecimal(MaterialNum1.Text),
                    unitPrice = ParseDecimal(MaterialUnitPrice1.Text)
                });
            }
        }
        catch (Exception ex)
        {
            // 如果 JSON 解析失敗，記錄錯誤但不中斷程序
        }
        
        return result;
    }
    
    private List<ResearchFeeRow> CollectResearchData()
    {
        var result = new List<ResearchFeeRow>();
        
        // 收集技術移轉資料
        if (!string.IsNullOrEmpty(ResearchFeesPrice1.Text))
        {
            result.Add(new ResearchFeeRow
            {
                category = "技術移轉",
                dateStart = txtDate1Start.Text,
                dateEnd = txtDate1End.Text,
                projectName = ResearchFeesName1.Text,
                targetPerson = ResearchFeesPersonName1.Text,
                price = ParseDecimal(ResearchFeesPrice1.Text)
            });
        }
        
        // 收集委託研究資料
        if (!string.IsNullOrEmpty(ResearchFeesPrice2.Text))
        {
            result.Add(new ResearchFeeRow
            {
                category = "委託研究",
                dateStart = txtDate2Start.Text,
                dateEnd = txtDate2End.Text,
                projectName = ResearchFeesName2.Text,
                targetPerson = ResearchFeesPersonName2.Text,
                price = ParseDecimal(ResearchFeesPrice2.Text)
            });
        }
        
        return result;
    }
    
    private List<TravelRow> CollectTravelData()
    {
        var result = new List<TravelRow>();
        
        try
        {
            // 從隱藏欄位讀取動態資料
            if (!string.IsNullOrEmpty(hdnTravelData.Value))
            {
                var serializer = new JavaScriptSerializer();
                var dynamicData = serializer.Deserialize<List<TravelRow>>(hdnTravelData.Value);
                if (dynamicData != null)
                {
                    result.AddRange(dynamicData);
                }
            }
            
            // 如果沒有動態資料，則收集第一行的 ASP.NET 控件資料
            if (result.Count == 0 && !string.IsNullOrEmpty(travelReason1.Text))
            {
                result.Add(new TravelRow
                {
                    reason = travelReason1.Text,
                    area = travelArea1.Text,
                    days = ParseDecimal(travelDays1.Text),
                    people = ParseDecimal(travelPeople1.Text),
                    price = ParseDecimal(travelPrice1.Text)
                });
            }
        }
        catch (Exception ex)
        {
            // 如果 JSON 解析失敗，記錄錯誤但不中斷程序
        }
        
        return result;
    }
    
    private List<OtherFeeRow> CollectOtherData()
    {
        var result = new List<OtherFeeRow>();
        
        try
        {
            // 從隱藏欄位讀取動態資料
            if (!string.IsNullOrEmpty(hdnOtherData.Value))
            {
                var serializer = new JavaScriptSerializer();
                var dynamicData = serializer.Deserialize<List<OtherFeeRow>>(hdnOtherData.Value);
                if (dynamicData != null)
                {
                    result.AddRange(dynamicData);
                }
            }
            
            // 如果沒有動態資料，則收集第一行的 ASP.NET 控件資料
            if (result.Count == 0 && !string.IsNullOrEmpty(otherAvgSalary1.Text))
            {
                result.Add(new OtherFeeRow
                {
                    title = "", // 需要從前端 JavaScript 收集
                    avgSalary = ParseDecimal(otherAvgSalary1.Text),
                    months = ParseDecimal(otherMonth1.Text),
                    people = ParseDecimal(otherPeople1.Text)
                });
            }
        }
        catch (Exception ex)
        {
            // 如果 JSON 解析失敗，記錄錯誤但不中斷程序
        }
        
        return result;
    }
    
    private List<OtherRent> CollectOtherRentData()
    {
        var result = new List<OtherRent>();
        
        try
        {
            // 從隱藏欄位讀取動態資料
            if (!string.IsNullOrEmpty(hdnOtherRentData.Value))
            {
                var serializer = new JavaScriptSerializer();
                var dynamicData = serializer.Deserialize<List<OtherRent>>(hdnOtherRentData.Value);
                if (dynamicData != null)
                {
                    result.AddRange(dynamicData);
                }
            }
            
            // 如果沒有動態資料，則收集 ASP.NET 控件資料
            if (result.Count == 0 && !string.IsNullOrEmpty(rentCash.Text))
            {
                result.Add(new OtherRent
                {
                    item = "租金",
                    amount = ParseDecimal(rentCash.Text),
                    note = rentDescription.Text
                });
            }
        }
        catch (Exception ex)
        {
            // 如果 JSON 解析失敗，記錄錯誤但不中斷程序
        }
        
        return result;
    }
    
    private List<TotalFeeRow> CollectTotalFeesData()
    {
        var result = new List<TotalFeeRow>();
        
        try
        {
            // 從隱藏欄位讀取動態資料
            if (!string.IsNullOrEmpty(hdnTotalFeesData.Value))
            {
                var serializer = new JavaScriptSerializer();
                var dynamicData = serializer.Deserialize<List<TotalFeeRow>>(hdnTotalFeesData.Value);
                if (dynamicData != null)
                {
                    result.AddRange(dynamicData);
                }
            }
        }
        catch (Exception ex)
        {
            // 如果 JSON 解析失敗，記錄錯誤但不中斷程序
        }
        
        return result;
    }
    
    private decimal ParseDecimal(string value)
    {
        if (string.IsNullOrEmpty(value))
            return 0;
            
        // 移除千分位符號再解析
        value = value.Replace(",", "");
        
        if (decimal.TryParse(value, out decimal result))
            return result;
            
        return 0;
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
    /// 檢查表單狀態並控制暫存按鈕顯示
    /// </summary>
    private void CheckFormStatusAndHideTempSaveButton()
    {
        try
        {
            string projectId = Request.QueryString["ProjectID"];
            if (!string.IsNullOrEmpty(projectId))
            {
                var lastVersion = OFS_SciApplicationHelper.getVersionByProjectID(projectId);
                if (lastVersion != null)
                {
                    var formStatus = OFS_SciWorkSchHelper.GetFormStatusByProjectID(lastVersion.ProjectID, "Form3Status");
                    
                    if (formStatus == "完成")
                    {
                        // 隱藏暫存按鈕
                        btnTempSave.Style["display"] = "none";
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

