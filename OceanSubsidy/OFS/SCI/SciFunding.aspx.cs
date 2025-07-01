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
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.X509.SigI;

public partial class OFS_SciFunding : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            
        }
        
        BindDropDown();
        
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
    [WebMethod]
    public static object SaveForm(SciFundingDataSave formData)
    {
        string projectId = formData.projectId;
        
        // 先查詢最新版本ID
        var lastVersion = OFS_SciApplicationHelper.getVersionLatestProjectID(projectId);
        string versionId = lastVersion.Version_ID; 
        
        try
        {
            // 處理人員資料
        
            OFS_SciFundingHelper.ReplacePersonFormList(formData.personnel, versionId);
            // 處理耗材資料
            OFS_SciFundingHelper.ReplaceMaterialList(formData.materials, versionId);
            // 處理研究經費
            OFS_SciFundingHelper.ReplaceResearchFees(formData.researchFees, versionId);
            // 處理差旅費
            OFS_SciFundingHelper.ReplaceTripForm(formData.travel, versionId);
            // 處理其他人事費
            OFS_SciFundingHelper.ReplaceOtherPersonFee(formData.otherFees, versionId);
            // 處理其他租金與勞務
            OFS_SciFundingHelper.ReplaceOtherObjectFee(formData.otherRent, versionId);
            // 處理經費總表
            if (formData.totalFees != null && formData.totalFees.Count > 0)
            {
                OFS_SciFundingHelper.ReplaceTotalFeeList(formData.totalFees, versionId);
            }

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
}
