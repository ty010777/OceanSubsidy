using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Operation;
using GS.OCA_OceanSubsidy.Model.OFS;
using GS.OCA_OceanSubsidy.Operation.OFS;

public partial class OFS_SciOutcomes : System.Web.UI.Page
{
    public string ProjectID { get; set; }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // 設定顯示模式
            SetDisplayMode();
            
            if (!IsPostBack)
            {
                // 設定 ProjectID
                ProjectID = Request.QueryString["ProjectID"];
                if (!string.IsNullOrEmpty(ProjectID))
                {
                    // 找到 Master Page 中的 HiddenField
                    HiddenField hdnProjectID = Master.FindControl("hdnProjectID") as HiddenField;
                    if (hdnProjectID != null)
                    {
                        hdnProjectID.Value = ProjectID;
                    }
                    
                    // 載入現有資料
                    LoadExistingData();
                    
                    // 檢查表單狀態並控制暫存按鈕顯示
                    CheckFormStatusAndHideTempSaveButton();
                }
            }
        }
        catch (Exception ex)
        {
            // 發生錯誤時記錄但不中斷頁面載入
            System.Diagnostics.Debug.WriteLine($"頁面載入錯誤：{ex.Message}");
        }
    }

    protected void btnTempSave_Click(object sender, EventArgs e)
    {
        try
        {
            // 確保有 ProjectID
            if (string.IsNullOrEmpty(ProjectID))
            {
                ProjectID = Request.QueryString["ProjectID"];
            }
            
            if (string.IsNullOrEmpty(ProjectID))
            {
                // 顯示錯誤訊息
                Response.Write("<script>alert('ProjectID 不能為空');</script>");
                return;
            }
            
            // 收集表單資料
            var entity = CollectFormData();
            
            // 儲存資料
            OFS_SciOutcomeHelper.SaveOutcomeData(entity);
            
            // 更新版本狀態（暫存）
            UpdateVersionStatusBasedOnAction(ProjectID, false);
        
            // 顯示成功訊息
            Response.Write("<script>alert('儲存成功');</script>");
            LoadExistingData();
        }
        catch (Exception ex)
        {
            // 顯示錯誤訊息
            Response.Write($"<script>alert('儲存失敗：{ex.Message}');</script>");
        }
    }

    protected void btnNext_Click(object sender, EventArgs e)
    {
        // 先儲存資料再導向下一步
        try
        {
            // 確保有 ProjectID
            if (string.IsNullOrEmpty(ProjectID))
            {
                ProjectID = Request.QueryString["ProjectID"];
            }
            
            if (string.IsNullOrEmpty(ProjectID))
            {
                Response.Write("<script>alert('ProjectID 不能為空');</script>");
                return;
            }
            
            // 收集表單資料
            var entity = CollectFormData();
            
            // 儲存資料
            OFS_SciOutcomeHelper.SaveOutcomeData(entity);
            
            // 更新版本狀態
          
            UpdateVersionStatusBasedOnAction(ProjectID, true);
            
        }
        catch (Exception ex)
        {
            // 如果儲存失敗，不要導向下一步
            Response.Write($"<script>alert('儲存失敗：{ex.Message}');</script>");
            return;
        }
        
        // 導向下一步
        if (!string.IsNullOrEmpty(ProjectID))
        {
            Response.Redirect("SciRecusedList.aspx?ProjectID=" + ProjectID);
        }
    }
    
    private void LoadExistingData()
    {
        try
        {
            // 載入現有的成果資料
            var existingData = OFS_SciOutcomeHelper.GetOutcomeData(ProjectID);
            if (existingData != null)
            {
                PopulateFormFields(existingData);
            }
            
        }
        catch (Exception ex)
        {
            // 載入失敗時不顯示錯誤，只是不填入資料
            System.Diagnostics.Debug.WriteLine($"載入資料失敗：{ex.Message}");
        }
    }
    
    private void PopulateFormFields(OFS_SCI_Outcomes data)
    {
        // 使用 JavaScript 在客戶端填入資料
        string script = $@"
        <script type='text/javascript'>
        $(document).ready(function() {{
            // 技術移轉
            $('input[name=""tech_transfer_plan_count""]').val('{data.TechTransfer_Plan_Count ?? 0}');
            $('input[name=""tech_transfer_plan_price""]').val('{data.TechTransfer_Plan_Price ?? 0}');
            $('input[name=""tech_transfer_track_count""]').val('{data.TechTransfer_Track_Count ?? 0}');
            $('input[name=""tech_transfer_track_price""]').val('{data.TechTransfer_Track_Price ?? 0}');
            $('textarea[name=""tech_transfer_description""]').val('{data.TechTransfer_Description?.Replace("'", "\\'")}');
            
            // 專利
            $('input[name=""patent_plan_apply""]').val('{data.Patent_Plan_Apply ?? 0}');
            $('input[name=""patent_plan_grant""]').val('{data.Patent_Plan_Grant ?? 0}');
            $('input[name=""patent_track_apply""]').val('{data.Patent_Track_Apply ?? 0}');
            $('input[name=""patent_track_grant""]').val('{data.Patent_Track_Grant ?? 0}');
            $('textarea[name=""patent_description""]').val('{data.Patent_Description?.Replace("'", "\\'")}');
            
            // 人才培育
            $('input[name=""talent_plan_phd""]').val('{data.Talent_Plan_PhD ?? 0}');
            $('input[name=""talent_plan_master""]').val('{data.Talent_Plan_Master ?? 0}');
            $('input[name=""talent_plan_others""]').val('{data.Talent_Plan_Others ?? 0}');
            $('input[name=""talent_track_phd""]').val('{data.Talent_Track_PhD ?? 0}');
            $('input[name=""talent_track_master""]').val('{data.Talent_Track_Master ?? 0}');
            $('input[name=""talent_track_others""]').val('{data.Talent_Track_Others ?? 0}');
            $('textarea[name=""talent_description""]').val('{data.Talent_Description?.Replace("'", "\\'")}');
            
            // 論文
            $('input[name=""papers_plan""]').val('{data.Papers_Plan ?? 0}');
            $('input[name=""papers_track""]').val('{data.Papers_Track ?? 0}');
            $('textarea[name=""papers_description""]').val('{data.Papers_Description?.Replace("'", "\\'")}');
            
            // 促成產學研合作
            $('input[name=""industry_collab_plan_count""]').val('{data.IndustryCollab_Plan_Count ?? 0}');
            $('input[name=""industry_collab_plan_price""]').val('{data.IndustryCollab_Plan_Price ?? 0}');
            $('input[name=""industry_collab_track_count""]').val('{data.IndustryCollab_Track_Count ?? 0}');
            $('input[name=""industry_collab_track_price""]').val('{data.IndustryCollab_Track_Price ?? 0}');
            $('textarea[name=""industry_collab_description""]').val('{data.IndustryCollab_Description?.Replace("'", "\\'")}');
            
            // 促成投資
            $('input[name=""investment_plan_price""]').val('{data.Investment_Plan_Price ?? 0}');
            $('input[name=""investment_track_price""]').val('{data.Investment_Track_Price ?? 0}');
            $('textarea[name=""investment_description""]').val('{data.Investment_Description?.Replace("'", "\\'")}');
            
            // 衍生產品
            $('input[name=""products_plan_count""]').val('{data.Products_Plan_Count ?? 0}');
            $('input[name=""products_plan_price""]').val('{data.Products_Plan_Price ?? 0}');
            $('input[name=""products_track_count""]').val('{data.Products_Track_Count ?? 0}');
            $('input[name=""products_track_price""]').val('{data.Products_Track_Price ?? 0}');
            $('textarea[name=""products_description""]').val('{data.Products_Description?.Replace("'", "\\'")}');
            
            // 降低人力成本
            $('input[name=""cost_reduction_plan_price""]').val('{data.CostReduction_Plan_Price ?? 0}');
            $('input[name=""cost_reduction_track_price""]').val('{data.CostReduction_Track_Price ?? 0}');
            $('textarea[name=""cost_reduction_description""]').val('{data.CostReduction_Description?.Replace("'", "\\'")}');
            
            // 技術推廣活動
            $('input[name=""promo_events_plan""]').val('{data.PromoEvents_Plan ?? 0}');
            $('input[name=""promo_events_track""]').val('{data.PromoEvents_Track ?? 0}');
            $('textarea[name=""promo_events_description""]').val('{data.PromoEvents_Description?.Replace("'", "\\'")}');
            
            // 技術服務
            $('input[name=""tech_services_plan_count""]').val('{data.TechServices_Plan_Count ?? 0}');
            $('input[name=""tech_services_plan_price""]').val('{data.TechServices_Plan_Price ?? 0}');
            $('input[name=""tech_services_track_count""]').val('{data.TechServices_Track_Count ?? 0}');
            $('input[name=""tech_services_track_price""]').val('{data.TechServices_Track_Price ?? 0}');
            $('textarea[name=""tech_services_description""]').val('{data.TechServices_Description?.Replace("'", "\\'")}');
            
            // 其他
            $('textarea[name=""other_plan_description""]').val('{data.Other_Plan_Description?.Replace("'", "\\'")}');
            $('textarea[name=""other_track_description""]').val('{data.Other_Track_Description?.Replace("'", "\\'")}');
            
            // 清除為0的數值欄位
            $('input[type=""text""]').each(function() {{
                if ($(this).val() === '0') {{
                    $(this).val('');
                }}
            }});
        }});
        </script>";
        
        ClientScript.RegisterStartupScript(this.GetType(), "PopulateData", script);
    }
    
    private int? ParseNullableInt(object value)
    {
        if (value == null) return null;
        if (int.TryParse(value.ToString(), out int result)) return result;
        return null;
    }
    
    private decimal? ParseNullableDecimal(object value)
    {
        if (value == null) return null;
        if (decimal.TryParse(value.ToString(), out decimal result)) return result;
        return null;
    }

    

    private OFS_SCI_Outcomes CollectFormData()
    {
        var entity = new OFS_SCI_Outcomes();
        
        // 取得 ProjectID
        if (!string.IsNullOrEmpty(ProjectID))
        {
            entity.ProjectID = ProjectID;
        }
        
        // 使用 Request.Form 收集表單資料
        // 技術移轉
        entity.TechTransfer_Plan_Count = ParseNullableInt(Request.Form["tech_transfer_plan_count"]);
        entity.TechTransfer_Plan_Price = ParseNullableDecimal(Request.Form["tech_transfer_plan_price"]);
        entity.TechTransfer_Track_Count = ParseNullableInt(Request.Form["tech_transfer_track_count"]);
        entity.TechTransfer_Track_Price = ParseNullableDecimal(Request.Form["tech_transfer_track_price"]);
        entity.TechTransfer_Description = Request.Form["tech_transfer_description"] ?? "";
        
        // 專利
        entity.Patent_Plan_Apply = ParseNullableInt(Request.Form["patent_plan_apply"]);
        entity.Patent_Plan_Grant = ParseNullableInt(Request.Form["patent_plan_grant"]);
        entity.Patent_Track_Apply = ParseNullableInt(Request.Form["patent_track_apply"]);
        entity.Patent_Track_Grant = ParseNullableInt(Request.Form["patent_track_grant"]);
        entity.Patent_Description = Request.Form["patent_description"] ?? "";
        
        // 人才培育
        entity.Talent_Plan_PhD = ParseNullableInt(Request.Form["talent_plan_phd"]);
        entity.Talent_Plan_Master = ParseNullableInt(Request.Form["talent_plan_master"]);
        entity.Talent_Plan_Others = ParseNullableInt(Request.Form["talent_plan_others"]);
        entity.Talent_Track_PhD = ParseNullableInt(Request.Form["talent_track_phd"]);
        entity.Talent_Track_Master = ParseNullableInt(Request.Form["talent_track_master"]);
        entity.Talent_Track_Others = ParseNullableInt(Request.Form["talent_track_others"]);
        entity.Talent_Description = Request.Form["talent_description"] ?? "";
        
        // 論文
        entity.Papers_Plan = ParseNullableInt(Request.Form["papers_plan"]);
        entity.Papers_Track = ParseNullableInt(Request.Form["papers_track"]);
        entity.Papers_Description = Request.Form["papers_description"] ?? "";
        
        // 促成產學研合作
        entity.IndustryCollab_Plan_Count = ParseNullableInt(Request.Form["industry_collab_plan_count"]);
        entity.IndustryCollab_Plan_Price = ParseNullableDecimal(Request.Form["industry_collab_plan_price"]);
        entity.IndustryCollab_Track_Count = ParseNullableInt(Request.Form["industry_collab_track_count"]);
        entity.IndustryCollab_Track_Price = ParseNullableDecimal(Request.Form["industry_collab_track_price"]);
        entity.IndustryCollab_Description = Request.Form["industry_collab_description"] ?? "";
        
        // 促成投資
        entity.Investment_Plan_Price = ParseNullableDecimal(Request.Form["investment_plan_price"]);
        entity.Investment_Track_Price = ParseNullableDecimal(Request.Form["investment_track_price"]);
        entity.Investment_Description = Request.Form["investment_description"] ?? "";
        
        // 衍生產品
        entity.Products_Plan_Count = ParseNullableInt(Request.Form["products_plan_count"]);
        entity.Products_Plan_Price = ParseNullableDecimal(Request.Form["products_plan_price"]);
        entity.Products_Track_Count = ParseNullableInt(Request.Form["products_track_count"]);
        entity.Products_Track_Price = ParseNullableDecimal(Request.Form["products_track_price"]);
        entity.Products_Description = Request.Form["products_description"] ?? "";
        
        // 降低人力成本
        entity.CostReduction_Plan_Price = ParseNullableDecimal(Request.Form["cost_reduction_plan_price"]);
        entity.CostReduction_Track_Price = ParseNullableDecimal(Request.Form["cost_reduction_track_price"]);
        entity.CostReduction_Description = Request.Form["cost_reduction_description"] ?? "";
        
        // 技術推廣活動
        entity.PromoEvents_Plan = ParseNullableInt(Request.Form["promo_events_plan"]);
        entity.PromoEvents_Track = ParseNullableInt(Request.Form["promo_events_track"]);
        entity.PromoEvents_Description = Request.Form["promo_events_description"] ?? "";
        
        // 技術服務
        entity.TechServices_Plan_Count = ParseNullableInt(Request.Form["tech_services_plan_count"]);
        entity.TechServices_Plan_Price = ParseNullableDecimal(Request.Form["tech_services_plan_price"]);
        entity.TechServices_Track_Count = ParseNullableInt(Request.Form["tech_services_track_count"]);
        entity.TechServices_Track_Price = ParseNullableDecimal(Request.Form["tech_services_track_price"]);
        entity.TechServices_Description = Request.Form["tech_services_description"] ?? "";
        
        // 其他
        entity.Other_Plan_Description = Request.Form["other_plan_description"] ?? "";
        entity.Other_Track_Description = Request.Form["other_track_description"] ?? "";
        
        return entity;
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
                // 點擊「下一步」按鈕
                // 1. Form4Status 設為 "完成" 
                // 2. 檢查 CurrentStep，如果 <= 4 則改成 5
                
                string currentStep = OFS_SciWorkSchHelper.GetCurrentStepByProjectID(ProjectID);
                int currentStepNum = 1;
                int.TryParse(currentStep, out currentStepNum);
                
                bool shouldUpdateCurrentStep = currentStepNum <= 4;
                string newCurrentStep = shouldUpdateCurrentStep ? "5" : currentStep;
                
                // 更新 Form4Status 為 "完成" 和 CurrentStep (如果需要)
                if (shouldUpdateCurrentStep)
                {
                    OFS_SciOutcomeHelper.UpdateForm4StatusAndCurrentStep(ProjectID, "完成", newCurrentStep);
                }
                else
                {
                    OFS_SciOutcomeHelper.UpdateForm4Status(ProjectID, "完成");
                }
            }
            else
            {
                // 點擊「暫存」按鈕
                // 只更新 Form4Status 為 "暫存"，CurrentStep 不變
                
                OFS_SciOutcomeHelper.UpdateForm4Status(ProjectID, "暫存");
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
            string projectId = Request.QueryString["ProjectID"];
            if (!string.IsNullOrEmpty(projectId))
            {
              
                var formStatus = OFS_SciWorkSchHelper.GetFormStatusByProjectID(ProjectID, "Form4Status");
                
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
        string projectId = Request.QueryString["ProjectID"] ?? ProjectID;
        
        // 如果沒有 ProjectID，是新申請案件，可以編輯
        if (string.IsNullOrEmpty(projectId))
        {
            return true;
        }
        
        try
        {
            // 確認狀態
            var   ApplicationData = OFS_SciApplicationHelper.getVersionByProjectID(projectId);
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

