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

public partial class OFS_ReviewChecklist : System.Web.UI.Page
{
    private List<ReviewChecklistItem> SciMainList;
    private List<ReviewChecklistItem> OriginalSciMainList;
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadData();
            // 初始狀態顯示資格審查的資料
            hidSelectedStage.Value = "資格審查";
            FilterData();
            BindData();
            
            // 初始化標籤統計和 active 狀態
            UpdateTabStatisticsAndActiveState("資格審查");
        }
        else
        {
            // 從 ViewState 還原資料
            RestoreDataFromViewState();
            
            // PostBack 時也要更新標籤統計和狀態
            if (OriginalSciMainList != null && OriginalSciMainList.Count > 0)
            {
                string currentSelectedStage = hidSelectedStage.Value;
                if (string.IsNullOrEmpty(currentSelectedStage))
                {
                    currentSelectedStage = "資格審查";
                    hidSelectedStage.Value = currentSelectedStage;
                }
                UpdateTabStatisticsAndActiveState(currentSelectedStage);
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
            // 載入資格審查/內容審查資料
            OriginalSciMainList = ReviewCheckListHelper.GetLatestVersionPerSource();
            SciMainList = new List<ReviewChecklistItem>(OriginalSciMainList);
            // TODO: 還要請求 文化 和民間社團資料
            
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
            tableHtml.AppendLine($"    <td>{"科專" ?? ""}</td>");
            tableHtml.AppendLine($"    <td>{item.ProjectID ?? ""}</td>");
            tableHtml.AppendLine($"    <td><a href=\"#\" style=\"color: #007bff; text-decoration: none;\">{item.ProjectNameTw ?? ""}</a></td>");
            tableHtml.AppendLine($"    <td>{item.OrgName ?? ""}</td>");
            tableHtml.AppendLine($"    <td>{item.ApplicationAmount ?? "0"}</td>");
            tableHtml.AppendLine($"    <td><span class=\"status-tag {item.GetStatusCssClass()}\">{item.StatusesName ?? ""}</span></td>");
            tableHtml.AppendLine($"    <td>{item.GetFormattedExpirationDate()}</td>");
            tableHtml.AppendLine($"    <td>{item.SupervisoryPersonName ?? ""}</td>");
            tableHtml.AppendLine($"    <td>{item.GetActionButton()}</td>");
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
            string currentSelectedStage = hidSelectedStage.Value;
            if (string.IsNullOrEmpty(currentSelectedStage))
            {
                currentSelectedStage = "資格審查"; // 預設為資格審查
                hidSelectedStage.Value = currentSelectedStage;
            }
            
            UpdateTabStatisticsAndActiveState(currentSelectedStage);
        }
        catch (Exception ex)
        {
            ShowMessage($"搜尋時發生錯誤：{ex.Message}", false);
        }
    }
    
    // 批次通過按鈕事件處理
    protected void btnBatchPass_Click(object sender, EventArgs e)
    {
        try
        {
            // 從隱藏欄位取得選中的 ProjectID
            string selectedProjectIdsJson = hidSelectedProjectIds.Value;
            if (string.IsNullOrEmpty(selectedProjectIdsJson))
            {
                ShowMessage("未選擇任何案件", false);
                return;
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<string> selectedProjectIds = serializer.Deserialize<List<string>>(selectedProjectIdsJson);
            
            if (selectedProjectIds == null || selectedProjectIds.Count == 0)
            {
                ShowMessage("未選擇任何案件", false);
                return;
            }
            
            // 再次驗證選中案件的狀態（後端驗證）
            bool isValid = ValidateSelectedCases(selectedProjectIds);
            if (!isValid)
            {
                ShowMessage("選中的案件中包含非「通過」狀態或不在允許批次通過階段的案件，無法進行批次通過", false);
                return;
            }
            var versionIds = ConvertProjectIdsToVersionIds(selectedProjectIds);

            // 執行批次狀態更新
            UpdateCasesToNextStage(versionIds);
            
            ShowMessage($"成功案件批次通過，已進入下一階段", true);
            
            // 重新載入資料並維持當前選中的標籤狀態
            LoadData();
            
            string currentSelectedStage = hidSelectedStage.Value;
            if (string.IsNullOrEmpty(currentSelectedStage))
            {
                currentSelectedStage = "資格審查";
                hidSelectedStage.Value = currentSelectedStage;
            }
            
            FilterData();
            BindData();
            UpdateTabStatisticsAndActiveState(currentSelectedStage);
                
        }
        catch (Exception ex)
        {
            ShowMessage($"批次通過時發生錯誤：{ex.Message}", false);
        }
    }
    
    // 批次不通過按鈕事件處理
    protected void btnBatchReject_Click(object sender, EventArgs e)
    {
        try
        {
            // 從隱藏欄位取得選中的 ProjectID
            string selectedProjectIdsJson = hidSelectedProjectIds.Value;
            if (string.IsNullOrEmpty(selectedProjectIdsJson))
            {
                ShowMessage("未選擇任何案件", false);
                return;
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<string> selectedProjectIds = serializer.Deserialize<List<string>>(selectedProjectIdsJson);
            
            if (selectedProjectIds == null || selectedProjectIds.Count == 0)
            {
                ShowMessage("未選擇任何案件", false);
                return;
            }
            
            // 再次驗證選中案件的狀態（後端驗證）
            bool isValid = ValidateSelectedCasesForReject(selectedProjectIds);
            if (!isValid)
            {
                ShowMessage("選中的案件中包含非「不通過」或「逾期未補」狀態的案件，無法進行批次不通過", false);
                return;
            }
            var versionIds = ConvertProjectIdsToVersionIds(selectedProjectIds);

            // 執行批次不通過操作
            ReviewCheckListHelper.BatchRejectCases(versionIds);
            
            ShowMessage($"成功將 {selectedProjectIds.Count} 個案件批次不通過並結案", true);
            
            // 重新載入資料並維持當前選中的標籤狀態
            LoadData();
            
            string currentSelectedStage = hidSelectedStage.Value;
            if (string.IsNullOrEmpty(currentSelectedStage))
            {
                currentSelectedStage = "資格審查";
                hidSelectedStage.Value = currentSelectedStage;
            }
            
            FilterData();
            BindData();
            UpdateTabStatisticsAndActiveState(currentSelectedStage);
        }
        catch (Exception ex)
        {
            ShowMessage($"批次不通過時發生錯誤：{ex.Message}", false);
        }
    }
    
    // 驗證選中案件的狀態(針對批次不通過)
    private bool ValidateSelectedCasesForReject(List<string> projectIds)
    {
        try
        {
            // 轉換為 Version_ID 並驗證狀態
            var versionIds = ConvertProjectIdsToVersionIds(projectIds);
            
            if (versionIds.Count == 0)
            {
                return false;
            }
            
            // 使用新的驗證方法，檢查是否為不通過或逾期未補狀態
            return ReviewCheckListHelper.ValidateBatchRejectEligibility(versionIds);
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    // 驗證選中案件的狀態
    private bool ValidateSelectedCases(List<string> projectIds)
    {
        try
        {
            // 轉換為 Version_ID 並驗證狀態
            var versionIds = ConvertProjectIdsToVersionIds(projectIds);
            
            if (versionIds.Count == 0)
            {
                return false;
            }
            
            // 使用新的驗證方法，支援多種補助案類型
            return ReviewCheckListHelper.ValidateBatchPassEligibility(versionIds);
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    // 更新案件到下一階段
    private void UpdateCasesToNextStage(List<string> versionIds)
    {
        try
        {
            
            
            if (versionIds.Count > 0)
            {
                ReviewCheckListHelper.UpdateCasesToNextStage(versionIds);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"更新案件狀態時發生錯誤：{ex.Message}", ex);
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
        
        // 審查階段篩選（從標籤點擊）
        string selectedStage = hidSelectedStage.Value;
        if (!string.IsNullOrEmpty(selectedStage))
        {
            filteredList = filteredList.Where(x => x.Statuses == selectedStage);
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
        
        // 類別篩選
        string selectedCategory = ddlCategory.SelectedValue;
        if (!string.IsNullOrEmpty(selectedCategory))
        {
            filteredList = filteredList.Where(x => x.ProjectID != null && x.ProjectID.Contains(selectedCategory));
        }
        
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
            string selectedStage = hidSelectedStage.Value;
            UpdateTabStatisticsAndActiveState(selectedStage);
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
    private void UpdateTabStatisticsAndActiveState(string selectedStage)
    {
        if (OriginalSciMainList == null) return;
        
        try
        {
            // 統計各階段的數量
            var stageCounts = OriginalSciMainList
                .GroupBy(x => x.Statuses)
                .ToDictionary(g => g.Key, g => g.Count());
            
            // 建立 JSON 格式的統計資料
            var countsJson = new StringBuilder();
            countsJson.Append("{");
            
            var stages = new[] { "資格審查", "領域審查/初審", "技術審查/複審", "決審核定" };
            for (int i = 0; i < stages.Length; i++)
            {
                if (i > 0) countsJson.Append(",");
                var count = stageCounts.ContainsKey(stages[i]) ? stageCounts[stages[i]] : 0;
                countsJson.Append($"'{stages[i]}':{count}");
            }
            
            countsJson.Append("}");
            
            // 建立 JavaScript 語句
            var jsCode = $"updateTabCounts({countsJson.ToString()});";
            if (!string.IsNullOrEmpty(selectedStage))
            {
                jsCode += $" setActiveTab('{selectedStage}');";
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