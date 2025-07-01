using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using GS.OCA_OceanSubsidy.Entity;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.X509.SigI;

public partial class OFS_ApplicationChecklist : System.Web.UI.Page
{
    private List<ReviewChecklistItem> SciMainList;
    private List<ReviewChecklistItem> OriginalSciMainList;
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadData();
            // 初始狀態顯示總申請的資料
            hidSelectedStage.Value = "總申請";
            FilterData();
            BindData();
            
            // 初始化標籤統計和 active 狀態
            UpdateTabStatisticsAndActiveState("總申請");
        }
        else
        {
            // 從 ViewState 還原資料
            RestoreDataFromViewState();
            
            // PostBack 時也要更新標籤統計和狀態
            if (OriginalSciMainList != null && OriginalSciMainList.Count > 0)
            {
                string currentSelectedCategory = hidSelectedStage.Value;
                if (string.IsNullOrEmpty(currentSelectedCategory))
                {
                    currentSelectedCategory = "總申請";
                    hidSelectedStage.Value = currentSelectedCategory;
                }
                UpdateTabStatisticsAndActiveState(currentSelectedCategory);
            }
        }
    }
    
    private void RestoreDataFromViewState()
    {
        if (ViewState["OriginalSciMainList"] != null)
        {
            OriginalSciMainList = (List<ReviewChecklistItem>)ViewState["OriginalSciMainList"];
        }
    }
    
    private void SaveDataToViewState()
    {
        ViewState["OriginalSciMainList"] = OriginalSciMainList;
    }
    
    private void LoadData()
    {
        try
        {
            // 載入申請計畫清單資料
            OriginalSciMainList = ApplicationChecklistHelper.GetLatestApplicationChecklist();
            SciMainList = new List<ReviewChecklistItem>(OriginalSciMainList);
            
            // 儲存到 ViewState
            SaveDataToViewState();
        }
        catch (Exception ex)
        {
            // 記錄錯誤或顯示錯誤訊息
            Response.Write($"<script>alert('載入資料時發生錯誤: {ex.Message}');</script>");
            OriginalSciMainList = new List<ReviewChecklistItem>();
            SciMainList = new List<ReviewChecklistItem>();
        }
    }
    
    private void BindData()
    {
        if (SciMainList != null && SciMainList.Count > 0)
        {
            // 更新記錄資訊
            UpdateRecordInfo();
            
            // 產生表格資料
            GenerateTableRows();
        }
        else
        {
            // 沒有資料時的處理
            GenerateEmptyTable();
        }
    }
    
    private void UpdateRecordInfo()
    {
        if (SciMainList != null)
        {
            int totalRecords = SciMainList.Count;
            litRecordInfo.Text = $"共{totalRecords}筆資料，第1/1頁";
        }
        else
        {
            litRecordInfo.Text = "共0筆資料，第1/1頁";
        }
    }
    
    private void GenerateTableRows()
    {
        // 產生表格行的HTML
        StringBuilder tableHtml = new StringBuilder();
        
        foreach (var item in SciMainList)
        {
            tableHtml.AppendLine("<tr>");
            tableHtml.AppendLine($"    <td><input type=\"checkbox\" class=\"case-checkbox\" data-projectid=\"{item.ProjectID}\" data-status=\"{item.StatusesName}\" data-stage=\"{item.Statuses}\" /></td>");
            tableHtml.AppendLine($"    <td>{item.Year ?? ""}</td>");
            tableHtml.AppendLine($"    <td>{item.GetProjectCategory()}</td>");
            tableHtml.AppendLine($"    <td>{item.Version_ID ?? ""}</td>");
            tableHtml.AppendLine($"    <td><a href=\"#\" style=\"color: #007bff; text-decoration: none;\">{item.ProjectNameTw ?? ""}</a></td>");
            tableHtml.AppendLine($"    <td>{item.OrgName ?? ""}</td>");
            tableHtml.AppendLine($"    <td>{item.ApplicationAmount ?? "0"}</td>");
            tableHtml.AppendLine($"    <td>{item.Statuses ?? ""}</td>");
            tableHtml.AppendLine($"    <td><span class=\"status-tag {item.GetStatusCssClass()}\">{item.GetStatusWithDeadline()}</span></td>");
            tableHtml.AppendLine($"    <td>--</td>"); // 歷程欄位，暫時空白
            tableHtml.AppendLine($"    <td>--</td>"); // 操作欄位，暫時空白
            tableHtml.AppendLine("</tr>");
        }
        
        // 將產生的HTML插入到頁面中
        // 這需要在前端頁面加入一個 Literal 控制項或使用 JavaScript 來更新
        Page.ClientScript.RegisterStartupScript(this.GetType(), "UpdateTable", 
            $"updateTableData(`{tableHtml.ToString().Replace("`", "\\`")}`);", true);
    }
    
    private void GenerateEmptyTable()
    {
        string emptyHtml = "<tr><td colspan=\"11\" style=\"text-align: center; padding: 20px;\">目前沒有資料</td></tr>";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "UpdateTable", 
            $"updateTableData(`{emptyHtml}`);", true);
    }
    
    // 搜尋按鈕事件處理
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            // 根據搜尋條件篩選資料
            FilterData();
            BindData();
            
            // 維持當前選中的標籤狀態和更新統計
            string currentSelectedCategory = hidSelectedStage.Value;
            if (string.IsNullOrEmpty(currentSelectedCategory))
            {
                currentSelectedCategory = "總申請"; // 預設為總申請
                hidSelectedStage.Value = currentSelectedCategory;
            }
            
            UpdateTabStatisticsAndActiveState(currentSelectedCategory);
        }
        catch (Exception ex)
        {
            ShowMessage($"搜尋時發生錯誤：{ex.Message}", false);
        }
    }
    
    
    
    
    
    
    // 將 ProjectID 列表轉換為對應的最新 Version_ID 列表
    private List<string> ConvertProjectIdsToVersionIds(List<string> projectIds)
    {
        var versionIds = new List<string>();
        
        foreach (string projectId in projectIds)
        {
            var latestVersion = OFS_SciApplicationHelper.getVersionLatestProjectID(projectId);
            if (latestVersion != null && !string.IsNullOrEmpty(latestVersion.Version_ID))
            {
                versionIds.Add(latestVersion.Version_ID);
            }
        }
        
        return versionIds;
    }
    
    // 顯示訊息
    private void ShowMessage(string message, bool isSuccess)
    {
        string alertType = isSuccess ? "success" : "error";
        string script = $"alert('{message}');";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowMessage", script, true);
    }
    
    private void FilterData()
    {
        if (OriginalSciMainList == null) return;
        
        var filteredList = OriginalSciMainList.AsEnumerable();
        
        // 申請類別篩選（從標籤點擊）
        string selectedCategory = hidSelectedStage.Value;
        if (!string.IsNullOrEmpty(selectedCategory) && selectedCategory != "總申請")
        {
            filteredList = filteredList.Where(x => {
                var category = "";
                if (!string.IsNullOrEmpty(x.Version_ID))
                {
                    if (x.Version_ID.Contains("SCI")) category = "科專";
                    else if (x.Version_ID.Contains("CUL")) category = "文化";
                    else if (x.Version_ID.Contains("EDC")) category = "學校民間";
                    else if (x.Version_ID.Contains("CLB")) category = "學校社團";
                }
                return category == selectedCategory;
            });
        }
        
        // 關鍵字搜尋
        string searchText = txtSearch.Text.Trim();
        if (!string.IsNullOrEmpty(searchText))
        {
            filteredList = filteredList.Where(x => 
                (x.ProjectID?.Contains(searchText) == true) ||
                (x.ProjectNameTw?.Contains(searchText) == true));
        }
        
        // 年度篩選
        string selectedYear = ddlYear.SelectedValue;
        if (!string.IsNullOrEmpty(selectedYear))
        {
            filteredList = filteredList.Where(x => x.Year == selectedYear);
        }
        
        // // 類別篩選
        // string selectedCategory = ddlCategory.SelectedValue;
        // if (!string.IsNullOrEmpty(selectedCategory))
        // {
        //     filteredList = filteredList.Where(x => x.ProjectID != null && x.ProjectID.Contains(selectedCategory));
        // }
        
        // 狀態篩選
        string selectedStatus = ddlStatus.SelectedValue;
        if (!string.IsNullOrEmpty(selectedStatus))
        {
            filteredList = filteredList.Where(x => x.StatusesName == selectedStatus);
        }
        
        // 申請單位篩選
        string selectedDepartment = ddlDepartment.SelectedValue;
        if (!string.IsNullOrEmpty(selectedDepartment))
        {
            filteredList = filteredList.Where(x => x.OrgName == selectedDepartment);
        }
        
        // 審查員篩選
        string selectedReviewer = ddlReviewer.SelectedValue;
        if (!string.IsNullOrEmpty(selectedReviewer))
        {
            filteredList = filteredList.Where(x => x.SupervisoryPersonName == selectedReviewer);
        }
        
        SciMainList = filteredList.ToList();
    }
    
    // 標籤篩選事件處理
    protected void btnStageFilter_Click(object sender, EventArgs e)
    {
        try
        {
            // 進行篩選
            FilterData();
            BindData();
            
            // 更新標籤統計和選中狀態
            string selectedCategory = hidSelectedStage.Value;
            UpdateTabStatisticsAndActiveState(selectedCategory);
        }
        catch (Exception ex)
        {
            ShowMessage($"篩選時發生錯誤：{ex.Message}", false);
        }
    }
    
    // 更新標籤統計
    private void UpdateTabStatistics()
    {
        UpdateTabStatisticsAndActiveState(null);
    }
    
    // 更新標籤統計和選中狀態
    private void UpdateTabStatisticsAndActiveState(string selectedCategory)
    {
        if (OriginalSciMainList == null) return;
        
        try
        {
            // 統計各申請類別的數量
            var categoryCounts = new Dictionary<string, int>
            {
                ["總申請"] = OriginalSciMainList.Count,
                ["科專"] = OriginalSciMainList.Count(x => x.Version_ID?.Contains("SCI") == true),
                ["文化"] = OriginalSciMainList.Count(x => x.Version_ID?.Contains("CUL") == true),
                ["學校民間"] = OriginalSciMainList.Count(x => x.Version_ID?.Contains("EDC") == true),
                ["學校社團"] = OriginalSciMainList.Count(x => x.Version_ID?.Contains("CLB") == true)
            };
            
            // 建立 JSON 格式的統計資料
            var countsJson = new StringBuilder();
            countsJson.Append("{");
            
            var categories = new[] { "總申請", "科專", "文化", "學校民間", "學校社團" };
            for (int i = 0; i < categories.Length; i++)
            {
                if (i > 0) countsJson.Append(",");
                var count = categoryCounts[categories[i]];
                countsJson.Append($"'{categories[i]}':{count}");
            }
            
            countsJson.Append("}");
            
            // 建立 JavaScript 語句
            var jsCode = $"updateTabCounts({countsJson.ToString()});";
            if (!string.IsNullOrEmpty(selectedCategory))
            {
                jsCode += $" setActiveTab('{selectedCategory}');";
            }
            
            // 將統計資料和 active 狀態傳遞給前端
            Page.ClientScript.RegisterStartupScript(this.GetType(), "UpdateTabCountsAndActive", 
                jsCode, true);
        }
        catch (Exception ex)
        {
            // 記錄錯誤但不影響主功能
            System.Diagnostics.Debug.WriteLine($"更新標籤統計時發生錯誤：{ex.Message}");
        }
    }
}