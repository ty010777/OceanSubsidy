using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.X509.SigI;

public partial class OFS_SciOutcomes : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // 檢查是否為 API 請求
        string action = Request.QueryString["action"];
        if (!string.IsNullOrEmpty(action))
        {
            HandleApiRequest(action);
        }
    }
    
    private void HandleApiRequest(string action)
    {
        Response.ContentType = "application/json";
        Response.Headers.Add("Access-Control-Allow-Origin", "*");
        Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
        
        try
        {
            switch (action.ToLower())
            {
                case "getdata":
                    HandleGetData();
                    break;
                case "savedata":
                    HandleSaveData();
                    break;
                default:
                    Response.StatusCode = 404;
                    WriteApiResponse(new { error = "Action not found" });
                    break;
            }
        }
        catch (Exception ex)
        {
            Response.StatusCode = 500;
            WriteApiResponse(new { error = ex.Message });
        }
    }
    
    private void HandleGetData()
    {
        string projectId = Request.QueryString["projectId"];
        var lastVersion = OFS_SciApplicationHelper.getVersionLatestProjectID(projectId);

        if (string.IsNullOrEmpty(projectId))
        {
            Response.StatusCode = 400;
            WriteApiResponse(new { error = "ProjectID is required" });
            return;
        }
        
        // 這裡可以實作載入資料的邏輯
        // 目前回傳空資料結構
        WriteApiResponse(new { 
            success = true, 
            data = CreateEmptyFormData(lastVersion.Version_ID) 
        });
    }
    
    private void HandleSaveData()
    {
        try
        {
            string jsonData = new System.IO.StreamReader(Request.InputStream).ReadToEnd();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dynamic requestData = serializer.DeserializeObject(jsonData);
            
            string ProjectID = requestData["projectId"];
            var formData = requestData["formData"];
            
            if (string.IsNullOrEmpty(ProjectID))
            {
                Response.StatusCode = 400;
                WriteApiResponse(new { success = false, error = "ProjectID is required" });
                return;
            }

            var lastVersion = OFS_SciApplicationHelper.getVersionLatestProjectID(ProjectID);
            // 轉換並儲存資料
            var entity = ConvertVueDataToEntity(lastVersion.Version_ID, formData);
            OFS_SciOutcomeHelper.SaveOutcomeData(entity);
            
            WriteApiResponse(new { success = true, message = "資料儲存成功" });
        }
        catch (Exception ex)
        {
            WriteApiResponse(new { success = false, error = ex.Message });
        }
    }
    
    private void WriteApiResponse(object data)
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        string json = serializer.Serialize(data);
        Response.Write(json);
        Response.End();
    }
    
    private object CreateEmptyFormData(string Version_ID)
    {
        return new
        {
            Version_ID = Version_ID,
            techTransfer = new { planCount = (int?)null, planPrice = (decimal?)null, trackCount = (int?)null, trackPrice = (decimal?)null, description = "" },
            patent = new { planApply = (int?)null, planGrant = (int?)null, trackApply = (int?)null, trackGrant = (int?)null, description = "" },
            talent = new { planPhd = (int?)null, planMaster = (int?)null, planOthers = (int?)null, trackPhd = (int?)null, trackMaster = (int?)null, trackOthers = (int?)null, description = "" },
            papers = new { plan = (int?)null, track = (int?)null, description = "" },
            industryCollab = new { planCount = (int?)null, planPrice = (decimal?)null, trackCount = (int?)null, trackPrice = (decimal?)null, description = "" },
            investment = new { planPrice = (decimal?)null, trackPrice = (decimal?)null, description = "" },
            products = new { planCount = (int?)null, planPrice = (decimal?)null, trackCount = (int?)null, trackPrice = (decimal?)null, description = "" },
            costReduction = new { planPrice = (decimal?)null, trackPrice = (decimal?)null, description = "" },
            promoEvents = new { plan = (int?)null, track = (int?)null, description = "" },
            techServices = new { planCount = (int?)null, planPrice = (decimal?)null, trackCount = (int?)null, trackPrice = (decimal?)null, description = "" },
            other = new { planDescription = "", trackDescription = "" }
        };
    }
    
    private OFS_SCI_Outcomes ConvertVueDataToEntity(string Version_ID, dynamic formData)
    {
        var entity = new OFS_SCI_Outcomes { Version_ID = Version_ID };
        
        if (formData["techTransfer"] != null)
        {
            var tech = formData["techTransfer"];
            entity.TechTransfer_Plan_Count = ParseNullableInt(tech["planCount"]);
            entity.TechTransfer_Plan_Price = ParseNullableDecimal(tech["planPrice"]);
            entity.TechTransfer_Track_Count = ParseNullableInt(tech["trackCount"]);
            entity.TechTransfer_Track_Price = ParseNullableDecimal(tech["trackPrice"]);
            entity.TechTransfer_Description = tech["description"]?.ToString() ?? "";
        }
        
        // 可以繼續添加其他欄位的轉換...
        
        return entity;
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

    [WebMethod]
    public static object SaveOutcomeData(OutcomeRequest formData)
    {
        try
        {
            // 先查詢最新版本ID
            var latestVersion = OFS_SciApplicationHelper.getVersionLatestProjectID(formData.ProjectID);
            
            var entityData = MapOutcomeRequestToEntity(formData, latestVersion.Version_ID);
            
            // 呼叫實際儲存邏輯
            OFS_SciOutcomeHelper.SaveOutcomeData(entityData);

            return new
            {
                success = true,
                message = "資料儲存成功"
            };
        }
        catch (Exception ex)
        {
            return new
            {
                success = false,
                message = "儲存失敗：" + ex.Message
            };
        }
    }
    public static OFS_SCI_Outcomes MapOutcomeRequestToEntity(OutcomeRequest request, string versionId)
    {
    var entity = new OFS_SCI_Outcomes
    {
        Version_ID = versionId
    };

    foreach (var item in request.outcomeData)
    {
        switch (item.item)
        {
            case "(1) 技術移轉":
                entity.TechTransfer_Plan_Count = ParseInt(item.values, "TechTransfer_Plan_Count");
                entity.TechTransfer_Plan_Price = ParseDecimal(item.values, "TechTransfer_Plan_Price");
                entity.TechTransfer_Track_Count = ParseInt(item.values, "TechTransfer_Track_Count");
                entity.TechTransfer_Track_Price = ParseDecimal(item.values, "TechTransfer_Track_Price");
                entity.TechTransfer_Description = item.description;
                break;
            case "(2) 專利":
                entity.Patent_Plan_Apply = ParseInt(item.values, "Patent_Plan_Apply");
                entity.Patent_Plan_Grant = ParseInt(item.values, "Patent_Plan_Grant");
                entity.Patent_Track_Apply = ParseInt(item.values, "Patent_Track_Apply");
                entity.Patent_Track_Grant = ParseInt(item.values, "Patent_Track_Grant");
                entity.Patent_Description = item.description;
                break;
            case "(3) 人才培育":
                entity.Talent_Plan_PhD = ParseInt(item.values, "Talent_Plan_PhD");
                entity.Talent_Plan_Master = ParseInt(item.values, "Talent_Plan_Master");
                entity.Talent_Plan_Others = ParseInt(item.values, "Talent_Plan_Others");
                entity.Talent_Track_PhD = ParseInt(item.values, "Talent_Track_PhD");
                entity.Talent_Track_Master = ParseInt(item.values, "Talent_Track_Master");
                entity.Talent_Track_Others = ParseInt(item.values, "Talent_Track_Others");
                entity.Talent_Description = item.description;
                break;
            case "(4) 論文":
                entity.Papers_Plan = ParseInt(item.values, "Papers_Plan");
                entity.Papers_Track = ParseInt(item.values, "Papers_Track");
                entity.Papers_Description = item.description;
                break;
            case "(5) 促成產學研合作":
                entity.IndustryCollab_Plan_Count = ParseInt(item.values, "IndustryCollab_Plan_Count");
                entity.IndustryCollab_Plan_Price = ParseDecimal(item.values, "IndustryCollab_Plan_Price");
                entity.IndustryCollab_Track_Count = ParseInt(item.values, "IndustryCollab_Track_Count");
                entity.IndustryCollab_Track_Price = ParseDecimal(item.values, "IndustryCollab_Track_Price");
                entity.IndustryCollab_Description = item.description;
                break;
            case "(6) 促成投資":
                entity.Investment_Plan_Price = ParseDecimal(item.values, "Investment_Plan_Price");
                entity.Investment_Track_Price = ParseDecimal(item.values, "Investment_Track_Price");
                entity.Investment_Description = item.description;
                break;
            case "(7) 衍生產品":
                entity.Products_Plan_Count = ParseInt(item.values, "Products_Plan_Count");
                entity.Products_Plan_Price = ParseDecimal(item.values, "Products_Plan_Price");
                entity.Products_Track_Count = ParseInt(item.values, "Products_Track_Count");
                entity.Products_Track_Price = ParseDecimal(item.values, "Products_Track_Price");
                entity.Products_Description = item.description;
                break;
            case "(8) 降低人力成本":
                entity.CostReduction_Plan_Price = ParseDecimal(item.values, "CostReduction_Plan_Price");
                entity.CostReduction_Track_Price = ParseDecimal(item.values, "CostReduction_Track_Price");
                entity.CostReduction_Description = item.description;
                break;
            case "(9) 技術推廣活動":
                entity.PromoEvents_Plan = ParseInt(item.values, "PromoEvents_Plan");
                entity.PromoEvents_Track = ParseInt(item.values, "PromoEvents_Track");
                entity.PromoEvents_Description = item.description;
                break;
            case "(10) 技術服務":
                entity.TechServices_Plan_Count = ParseInt(item.values, "TechServices_Plan_Count");
                entity.TechServices_Plan_Price = ParseDecimal(item.values, "TechServices_Plan_Price");
                entity.TechServices_Track_Count = ParseInt(item.values, "TechServices_Track_Count");
                entity.TechServices_Track_Price = ParseDecimal(item.values, "TechServices_Track_Price");
                entity.TechServices_Description = item.description;
                break;
            case "(11) 其他":
                entity.Other_Plan_Description = item.values.ContainsKey("Other_Plan_Description") ? item.values["Other_Plan_Description"] : "";
                entity.Other_Track_Description = item.description;
                break;
        }
    }

    return entity;
}

private static int? ParseInt(Dictionary<string, string> values, string key)
{
    return values.ContainsKey(key) && int.TryParse(values[key], out int result) ? result : (int?)null;
}

private static decimal? ParseDecimal(Dictionary<string, string> values, string key)
{
    return values.ContainsKey(key) && decimal.TryParse(values[key], out decimal result) ? result : (decimal?)null;
}

}