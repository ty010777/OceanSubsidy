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
using GS.OCA_OceanSubsidy.Entity.Base;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.X509.SigI;

public partial class OFS_SciAvoidList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // 初始化頁面資料
            LoadData();
        }
    }

    private void LoadData()
    {
        string projectId = Request.QueryString["ProjectID"] ?? "";

        if (!string.IsNullOrEmpty(projectId))
        {
            var lastVersion = OFS_SciApplicationHelper.getVersionLatestProjectID(projectId);

            LoadExistingData(lastVersion.Version_ID);
        }
        
        // 載入 TRL 選項
        LoadTrlOptions();
    }
    
    private void LoadExistingData(string Version_ID)
    {
        try
        {
            // 載入委員迴避清單資料

            var recusedList = OFS_SciRecusedList.GetRecusedListByVersion_ID(Version_ID);
            var recusedData = recusedList.Select(x => new {
                committeeName = x.RecusedName,
                committeeUnit = x.EmploymentUnit,
                committeePosition = x.JobTitle,
                committeeReason = x.RecusedReason
            }).ToArray();
            
            // 載入技術成熟度資料
            var techList = OFS_SciRecusedList.GetTechReadinessListByVersion_ID(Version_ID);
            var techData = techList.Select(x => new {
                techItem = x.Name,
                trlPlanLevel = x.Bef_TRLevel,
                trlTrackLevel = x.Aft_TRLevel,
                techProcess = x.Description
            }).ToArray();
            
            // 將資料傳遞到前端
            var dataToSend = new {
                recusedData = recusedData,
                techData = techData
            };
            
            var dataJson = new JavaScriptSerializer().Serialize(dataToSend);
            ClientScript.RegisterStartupScript(this.GetType(), "existingData", 
                $"window.existingData = {dataJson};", true);
        }
        catch (Exception ex)
        {
            // 如果載入失敗，設定空資料
            ClientScript.RegisterStartupScript(this.GetType(), "existingData", 
                "window.existingData = { recusedData: [], techData: [] };", true);
        }
    }
    
    private void LoadTrlOptions()
    {
        try
        {
            var trlOptions = OFS_SciRecusedList.GetSysZgsCodeByCodeGroup("SCI_TRL");
            var optionsJson = new JavaScriptSerializer().Serialize(trlOptions.Select(x => new { 
                Code = x.Code, 
                Descname = x.Descname 
            }));
            
            ClientScript.RegisterStartupScript(this.GetType(), "trlOptions", 
                $"window.trlOptions = {optionsJson};", true);
        }
        catch (Exception ex)
        {
            // 如果載入失敗，使用預設選項
            ClientScript.RegisterStartupScript(this.GetType(), "trlOptions", 
                "window.trlOptions = [];", true);
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            // 取得 Checkbox 狀態
            bool noAvoidanceCommittee = chkNoAvoidance.Checked;

            string projectId = Request.QueryString["ProjectID"] ?? "";
            var lastVersion = OFS_SciApplicationHelper.getVersionLatestProjectID(projectId);

            // // 驗證資料
            // if (!ValidateData(committeeData, techData, noAvoidanceCommittee, noAvoidanceTech))
            // {
            //     return;
            // }
            // 取得委員迴避清單資料
            var committeeData = GetCommitteeData(lastVersion.Version_ID);
            // 取得技術能力資料
            var techData = GetTechData(lastVersion.Version_ID);
            // TODO: 儲存到資料庫
            // 取得目前的申請案 ID

            OFS_SciRecusedList.ReplaceRecusedList(committeeData, lastVersion.Version_ID);
            OFS_SciRecusedList.ReplaceTechReadinessList(techData, lastVersion.Version_ID);

            // 顯示成功訊息
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('儲存成功！');", true);
        }
        catch (Exception ex)
        {
            // 錯誤處理
            ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('儲存失敗：{ex.Message}');", true);
        }
    }

    // protected void btnAddCommittee_Click(object sender, EventArgs e)
    // {
    //     // 使用 JavaScript 新增委員行
    //     ClientScript.RegisterStartupScript(this.GetType(), "addRow", 
    //         "$('#committeeTableBody').append('" +
    //         "<tr>" +
    //         "<td><input type=\"text\" name=\"committeeName\" /></td>" +
    //         "<td><input type=\"text\" name=\"committeeUnit\" /></td>" +
    //         "<td><input type=\"text\" name=\"committeePosition\" /></td>" +
    //         "<td><input type=\"text\" name=\"committeeReason\" /></td>" +
    //         "<td><button type=\"button\" class=\"btn btn-danger\">🗑</button></td>" +
    //         "</tr>');", true);
    // }

    private List<OFS_SCI_Other_Recused> GetCommitteeData(string Version_ID)
    {
        var data = new List<OFS_SCI_Other_Recused>();
        
        // 取得所有委員相關的 input 欄位
        string[] names = Request.Form.GetValues("committeeName");
        string[] units = Request.Form.GetValues("committeeUnit");
        string[] positions = Request.Form.GetValues("committeePosition");
        string[] reasons = Request.Form.GetValues("committeeReason");

        if (names != null)
        {
            for (int i = 0; i < names.Length; i++)
            {
                // 只處理有資料的行
                if (!string.IsNullOrWhiteSpace(names[i]) || 
                    !string.IsNullOrWhiteSpace(units?[i]) || 
                    !string.IsNullOrWhiteSpace(positions?[i]) || 
                    !string.IsNullOrWhiteSpace(reasons?[i]))
                {
                    data.Add(new OFS_SCI_Other_Recused
                    {
                        Version_ID = Version_ID ?? "",
                        RecusedName = names[i]?.Trim() ?? "",
                        EmploymentUnit = units?[i]?.Trim() ?? "",
                        JobTitle = positions?[i]?.Trim() ?? "",
                        RecusedReason = reasons?[i]?.Trim() ?? ""
                    });
                }
            }
        }

        return data;
    }

    private List<OFS_SCI_Other_TechReadiness> GetTechData(string Version_ID)
    {
        var data = new List<OFS_SCI_Other_TechReadiness>();
        
        // 取得所有技術相關的 input 欄位
        string[] techItems = Request.Form.GetValues("techItem");
        string[] trlPlanLevels = Request.Form.GetValues("trlPlanLevel");
        string[] trlTrackLevels = Request.Form.GetValues("trlTrackLevel");
        string[] techProcesses = Request.Form.GetValues("techProcess");

        if (techItems != null)
        {
            for (int i = 0; i < techItems.Length; i++)
            {
                // 只處理有資料的行
                if (!string.IsNullOrWhiteSpace(techItems[i]) || 
                    !string.IsNullOrWhiteSpace(trlPlanLevels?[i]) || 
                    !string.IsNullOrWhiteSpace(trlTrackLevels?[i]) ||
                    !string.IsNullOrWhiteSpace(techProcesses?[i]))
                {
                    data.Add(new OFS_SCI_Other_TechReadiness
                    {
                        Version_ID= Version_ID ?? "",
                        Name = techItems[i]?.Trim() ?? "",
                        Bef_TRLevel = trlPlanLevels?[i]?.Trim() ?? "",
                        Aft_TRLevel = trlTrackLevels?[i]?.Trim() ?? "",
                        Description = techProcesses?[i]?.Trim() ?? ""
                    });
                }
            }
        }

        return data;
    }

    private bool ValidateData(List<OFS_SCI_Other_Recused> committeeData, List<OFS_SCI_Other_TechReadiness> techData, 
                             bool noAvoidanceCommittee)
    {
        // 如果沒有勾選「無需迴避」，則必須至少有一筆資料
        if (!noAvoidanceCommittee && committeeData.Count == 0)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", 
                "alert('請填寫委員迴避資料或勾選「無需迴避之審查委員」');", true);
            return false;
        }

        // 驗證必填欄位
        foreach (var item in committeeData)
        {
            if (string.IsNullOrWhiteSpace(item.RecusedName) || 
                string.IsNullOrWhiteSpace(item.EmploymentUnit) || 
                string.IsNullOrWhiteSpace(item.JobTitle) || 
                string.IsNullOrWhiteSpace(item.RecusedReason))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", 
                    "alert('委員迴避清單中有必填欄位未填寫');", true);
                return false;
            }
        }

        // 驗證技術資料
        // foreach (var item in techData)
        // {
        //     if (string.IsNullOrWhiteSpace(item.TechItem) || 
        //         string.IsNullOrWhiteSpace(item.TrlLevel) || 
        //         string.IsNullOrWhiteSpace(item.TechProcess))
        //     {
        //         ClientScript.RegisterStartupScript(this.GetType(), "alert", 
        //             "alert('技術能力資料中有必填欄位未填寫');", true);
        //         return false;
        //     }
        // }

        return true;
    }


    
}