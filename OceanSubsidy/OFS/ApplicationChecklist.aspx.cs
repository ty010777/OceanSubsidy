using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Model.OFS;
using GS.OCA_OceanSubsidy.Operation.OFS;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.X509.SigI;
using GS.App;
using GS.Data;

public partial class OFS_ApplicationChecklist : System.Web.UI.Page
{
    private List<ReviewChecklistItem> SciMainList;
    private List<ReviewChecklistItem> OriginalSciMainList;
    
    // 分頁相關屬性
    private int PageSize = 20; // 預設每頁20筆
    private int CurrentPage = 1; // 當前頁數
    private int TotalRecords = 0; // 總記錄數
    private int TotalPages = 1; // 總頁數
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadData();
            LoadAvailablePrograms(); // 載入可申請的計畫類別
            
            // 初始狀態顯示總申請的資料
            hidSelectedStage.Value = "總申請";
            FilterData();
            BindData();
            
            // 初始化標籤統計和 active 狀態
            UpdateTabStatisticsAndActiveState("總申請");
            
            // 綁定分頁按鈕事件
            btnPrevPage.Click += BtnPrevPage_Click;
            btnNextPage.Click += BtnNextPage_Click;
            ddlPageSize.SelectedIndexChanged += DdlPageSize_SelectedIndexChanged;
            ddlPageNumber.SelectedIndexChanged += DdlPageNumber_SelectedIndexChanged;
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
            // 計算分頁資訊
            CalculatePagination();
            
            // 取得當前頁的資料
            var pagedData = GetPagedData();
            
            // 更新記錄資訊
            UpdateRecordInfo();
            
            // 產生表格資料
            GenerateTableRows(pagedData);
            
            // 更新分頁控制項
            UpdatePaginationControls();
        }
        else
        {
            // 沒有資料時的處理
            GenerateEmptyTable();
            UpdatePaginationControls();
        }
    }
    
    private void UpdateRecordInfo()
    {
        if (SciMainList != null)
        {
            TotalRecords = SciMainList.Count;
            litRecordInfo.Text = $"<span class='text-teal'>{TotalRecords}</span> 筆資料";
        }
        else
        {
            TotalRecords = 0;
            litRecordInfo.Text = "<span class='text-teal'>0</span> 筆資料";
        }
    }
    
    private void GenerateTableRows(List<ReviewChecklistItem> dataList = null)
    {
        // 產生表格行的HTML
        StringBuilder tableHtml = new StringBuilder();
        var itemsToProcess = dataList ?? SciMainList;
        
        foreach (var item in itemsToProcess)
        {
            tableHtml.AppendLine("<tr>");
            tableHtml.AppendLine($"    <td data-th=\"年度:\">{item.Year ?? ""}</td>");
            tableHtml.AppendLine($"    <td data-th=\"計畫編號:\" style=\"text-align: left;\" nowrap>{item.ProjectID ?? ""}</td>");
            tableHtml.AppendLine($"    <td data-th=\"計畫名稱:\" style=\"text-align: left;\"><a href=\"#\" class=\"link-black\" target=\"_blank\">{item.ProjectNameTw ?? ""}</a></td>");
            tableHtml.AppendLine($"    <td data-th=\"申請單位:\" style=\"text-align: left;\">{item.OrgName ?? ""}</td>");
            tableHtml.AppendLine($"    <td data-th=\"類別:\">{item.GetProjectCategory()}</td>");
            tableHtml.AppendLine($"    <td data-th=\"申請補助金額:\">{FormatAmount(item.ApplicationAmount)}</td>");
            tableHtml.AppendLine($"    <td data-th=\"階段:\" nowrap><span class=\"\">{item.Statuses ?? ""}</span></td>");
            // 優先檢查撤案狀態
            string displayStatus = "";
            string statusClass = "";
            if (item.isWithdrawal == true)
            {
                displayStatus = "已撤案";
                statusClass = GetStatusColorClass("已撤案");
            }
            else
            {
                displayStatus = item.GetStatusWithDeadline();
                statusClass = GetStatusColorClass(item.StatusesName);
            }
            
            tableHtml.AppendLine($"    <td data-th=\"狀態:\" style=\"text-align: center;\"><span class=\"{statusClass}\">{displayStatus}</span></td>");
            tableHtml.AppendLine($"    <td data-th=\"功能:\"><div class=\"d-flex align-items-center justify-content-end gap-1\">{GenerateActionButtons(item)}</div></td>");
            tableHtml.AppendLine("</tr>");
        }
        
        // 將產生的HTML插入到頁面中
        // 這需要在前端頁面加入一個 Literal 控制項或使用 JavaScript 來更新
        Page.ClientScript.RegisterStartupScript(this.GetType(), "UpdateTable", 
            $"updateTableData(`{tableHtml.ToString().Replace("`", "\\`")}`);", true);
    }
    
    private void GenerateEmptyTable()
    {
        string emptyHtml = "<tr><td colspan=\"9\" style=\"text-align: center; padding: 20px;\">目前沒有資料</td></tr>";
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
                if (!string.IsNullOrEmpty(x.ProjectID))
                {
                    if (x.ProjectID.Contains("SCI")) category = "科專";
                    else if (x.ProjectID.Contains("CUL")) category = "文化";
                    else if (x.ProjectID.Contains("EDC")) category = "學校民間";
                    else if (x.ProjectID.Contains("CLB")) category = "學校社團";
                    else if (x.ProjectID.Contains("MUL")) category = "多元";
                    else if (x.ProjectID.Contains("LIT")) category = "素養";
                    else if (x.ProjectID.Contains("ACC")) category = "無障礙";
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
        string selectedDepartment = txtDepartment.Text.Trim();
        if (!string.IsNullOrEmpty(selectedDepartment))
        {
            filteredList = filteredList.Where(x => x.OrgName?.Contains(selectedDepartment) == true);
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
                ["科專"] = OriginalSciMainList.Count(x => x.ProjectID?.Contains("SCI") == true),
                ["文化"] = OriginalSciMainList.Count(x => x.ProjectID?.Contains("CUL") == true),
                ["學校民間"] = OriginalSciMainList.Count(x => x.ProjectID?.Contains("EDC") == true),
                ["學校社團"] = OriginalSciMainList.Count(x => x.ProjectID?.Contains("CLB") == true),
                ["多元"] = OriginalSciMainList.Count(x => x.ProjectID?.Contains("MUL") == true),
                ["素養"] = OriginalSciMainList.Count(x => x.ProjectID?.Contains("LIT") == true),
                ["無障礙"] = OriginalSciMainList.Count(x => x.ProjectID?.Contains("ACC") == true)
            };
            
            // 建立 JSON 格式的統計資料
            var countsJson = new StringBuilder();
            countsJson.Append("{");
            
            var categories = new[] { "總申請", "科專", "文化", "學校民間", "學校社團", "多元", "素養", "無障礙" };
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
    
    // 格式化金額顯示
    private string FormatAmount(string amount)
    {
        if (string.IsNullOrEmpty(amount) || amount == "0")
            return "0";
            
        if (decimal.TryParse(amount, out decimal value))
        {
            return value.ToString("#,##0");
        }
        
        return amount;
    }
    
    // 取得狀態顏色樣式
    private string GetStatusColorClass(string status)
    {
        if (string.IsNullOrEmpty(status)) return "";
        
        switch (status.Trim())
        {
            case "補正補件":
            case "待回覆":
            case "待修正":
                return "text-royal-blue";
            case "逾期未補":
            case "未通過":
            case "已撤案":
                return "text-pink";
            case "已核定":
                return "text-teal";
            case "尚未提送":
                return "text-royal-blue";
            default:
                return "";
        }
    }
    
    // 產生操作按鈕
    private string GenerateActionButtons(ReviewChecklistItem item)
    {
        StringBuilder buttons = new StringBuilder();
        
        // 根據狀態顯示不同的按鈕
        string StatusesName = item.StatusesName ?? "";
        string status = item.Statuses ?? "";
        bool isWithdrawn = item.isWithdrawal ?? false;
        
        // 編輯按鈕（只有「編輯中、補正補件」狀態可編輯）
        if (CanEdit(status,StatusesName))
        {
            string editUrl = GetEditUrl(item);
            string projectCategory = item.GetProjectCategory();
            
            if (editUrl != "#")
            {
                // 有對應的編輯頁面
                buttons.Append($"<a href=\"{ResolveUrl(editUrl)}\" class=\"btn btn-sm btn-teal-dark\" data-bs-toggle=\"tooltip\" data-bs-placement=\"top\" data-bs-title=\"編輯\"><i class=\"fa-solid fa-pen\"></i></a>");
            }
            
        }
        
        // 回覆按鈕（一直顯示）
        buttons.Append($"<button class=\"btn btn-sm btn-teal-dark\" type=\"button\" onclick=\"showReviewComments('{item.ProjectID}')\" data-bs-toggle=\"tooltip\" data-bs-placement=\"top\" data-bs-title=\"檢視審查意見\"><i class=\"fas fa-comment-dots\"></i></button>");
        
        // 歷程按鈕（所有項目都有）
        buttons.Append($"<button class=\"btn btn-sm btn-teal-dark\" type=\"button\" onclick=\"showHistory('{item.ProjectID}')\" data-bs-toggle=\"tooltip\" data-bs-placement=\"top\" data-bs-title=\"歷程\"><i class=\"fas fa-history\"></i></button>");
        
        // 更多操作選單
        buttons.Append("<div class=\"dropdown\"><button class=\"btn btn-sm btn-outline-teal\" type=\"button\" data-bs-toggle=\"dropdown\" aria-expanded=\"false\"><i class=\"fas fa-ellipsis-h\"></i></button>");
        buttons.Append("<ul class=\"dropdown-menu\" style=\"min-width: 120px;\">");
        
        // 撤案按鈕（只有在指定狀態下且未撤案時顯示）
        if (CanWithdraw(status) && !isWithdrawn)
        {
            buttons.Append($"<li><a class=\"dropdown-menu-item gap-1\" href=\"#\" onclick=\"handleWithdraw('{item.ProjectID}'); return false;\"><i class=\"fas fa-redo text-teal-dark\"></i>撤案</a></li>");
        }
        
        // 刪除按鈕（只有「尚未提送」狀態可刪除）
        if (CanDelete(status))
        {
            buttons.Append($"<li><a class=\"dropdown-menu-item gap-1\" href=\"#\" onclick=\"handleDelete('{item.ProjectID}'); return false;\" data-bs-toggle=\"modal\" data-bs-target=\"#planDeleteModal\"><i class=\"fas fa-times text-teal-dark\"></i>刪除</a></li>");
        }
        
        // 恢復案件按鈕（只有已撤案的案件顯示）
        if (isWithdrawn)
        {
            buttons.Append($"<li><a class=\"dropdown-menu-item gap-1\" href=\"#\" onclick=\"handleRestore('{item.ProjectID}')\"><i class=\"fas fa-undo text-teal-dark\"></i>恢復案件</a></li>");
        }
        
        buttons.Append("</ul></div>");
        
        return buttons.ToString();
    }
    
    // 檢查是否可編輯
    private bool CanEdit(string status ,string statusName)
    {
        if (status == "決審核定")
        {
            return statusName == "計畫書修正中";
            
        }else{
            return statusName == "編輯中" || statusName == "補正補件";
        }
        
    }
    
    // 檢查是否可撤案
    private bool CanWithdraw(string status)
    {
        var withdrawableStatuses = new[] { "資格審查", "內容審查", "領域審查", "初審", "技術審查", "複審", "決審核定" };
        return withdrawableStatuses.Contains(status);
    }
    
    // 檢查是否可刪除
    private bool CanDelete(string status)
    {
        return status == "尚未提送";
    }
    
    
    // 取得編輯頁面的網址
    private string GetEditUrl(ReviewChecklistItem item)
    {
        if (item == null || string.IsNullOrEmpty(item.ProjectID)) return "#";
        
        // 根據計畫類型決定編輯頁面
        string projectCategory = item.GetProjectCategory();
        
        switch (projectCategory)
        {
            case "科專":
                return $"~/OFS/SCI/SciApplication.aspx?ProjectID={item.ProjectID}";
            case "文化":
                // 尚未有這個網址，暫時返回空值
                return "#";
            case "學校社團":
                // 尚未有這個網址，暫時返回空值
                return "#";
            case "學校民間":
                // 尚未有這個網址，暫時返回空值
                return "#";
            case "多元":
                // 尚未有這個網址，暫時返回空值
                return "#";
            case "素養":
                // 尚未有這個網址，暫時返回空值
                return "#";
            case "無障礙":
                // 尚未有這個網址，暫時返回空值
                return "#";
            default:
                return "#";
        }
    }
    
    // 計算分頁資訊
    private void CalculatePagination()
    {
        // 取得每頁筆數設定
        if (int.TryParse(ddlPageSize.SelectedValue, out int pageSize))
        {
            PageSize = pageSize;
        }
        
        // 取得當前頁數
        if (int.TryParse(ddlPageNumber.SelectedValue, out int currentPage))
        {
            CurrentPage = currentPage;
        }
        
        // 計算總頁數
        TotalRecords = SciMainList?.Count ?? 0;
        TotalPages = TotalRecords > 0 ? (int)Math.Ceiling((double)TotalRecords / PageSize) : 1;
        
        // 確保當前頁數在有效範圍內
        if (CurrentPage > TotalPages) CurrentPage = TotalPages;
        if (CurrentPage < 1) CurrentPage = 1;
    }
    
    // 取得當前頁的資料
    private List<ReviewChecklistItem> GetPagedData()
    {
        if (SciMainList == null || SciMainList.Count == 0)
            return new List<ReviewChecklistItem>();
            
        int skipCount = (CurrentPage - 1) * PageSize;
        return SciMainList.Skip(skipCount).Take(PageSize).ToList();
    }
    
    // 更新分頁控制項
    private void UpdatePaginationControls()
    {
        // 更新頁數下拉選單
        ddlPageNumber.Items.Clear();
        for (int i = 1; i <= TotalPages; i++)
        {
            ddlPageNumber.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }
        ddlPageNumber.SelectedValue = CurrentPage.ToString();
        
        // 更新前端分頁顯示
        string jsCode = $"updatePagination({CurrentPage}, {TotalPages});";
        Page.ClientScript.RegisterStartupScript(this.GetType(), "UpdatePagination", jsCode, true);
    }
    
    // 上一頁按鈕事件
    protected void BtnPrevPage_Click(object sender, EventArgs e)
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            RestoreDataFromViewState();
            FilterData();
            BindData();
            
            // 維持當前選中的標籤狀態
            string currentSelectedCategory = hidSelectedStage.Value;
            if (string.IsNullOrEmpty(currentSelectedCategory))
                currentSelectedCategory = "總申請";
            UpdateTabStatisticsAndActiveState(currentSelectedCategory);
        }
    }
    
    // 下一頁按鈕事件
    protected void BtnNextPage_Click(object sender, EventArgs e)
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
            RestoreDataFromViewState();
            FilterData();
            BindData();
            
            // 維持當前選中的標籤狀態
            string currentSelectedCategory = hidSelectedStage.Value;
            if (string.IsNullOrEmpty(currentSelectedCategory))
                currentSelectedCategory = "總申請";
            UpdateTabStatisticsAndActiveState(currentSelectedCategory);
        }
    }
    
    // 每頁筆數變更事件
    protected void DdlPageSize_SelectedIndexChanged(object sender, EventArgs e)
    {
        CurrentPage = 1; // 重置到第一頁
        RestoreDataFromViewState();
        FilterData();
        BindData();
        
        // 維持當前選中的標籤狀態
        string currentSelectedCategory = hidSelectedStage.Value;
        if (string.IsNullOrEmpty(currentSelectedCategory))
            currentSelectedCategory = "總申請";
        UpdateTabStatisticsAndActiveState(currentSelectedCategory);
    }
    
    // 跳頁下拉選單變更事件
    protected void DdlPageNumber_SelectedIndexChanged(object sender, EventArgs e)
    {
        RestoreDataFromViewState();
        FilterData();
        BindData();
        
        // 維持當前選中的標籤狀態
        string currentSelectedCategory = hidSelectedStage.Value;
        if (string.IsNullOrEmpty(currentSelectedCategory))
            currentSelectedCategory = "總申請";
        UpdateTabStatisticsAndActiveState(currentSelectedCategory);
    }
    
    // 申請計畫按鈕事件
    protected void btnCreateApplication_Click(object sender, EventArgs e)
    {
        try
        {
            string selectedGrantTypeId = ddlModalYear.SelectedValue;
            
            if (string.IsNullOrEmpty(selectedGrantTypeId))
            {
                ShowMessage("請選擇申請補助計畫類別", false);
                return;
            }
            
            // 根據選擇的計畫類別重定向到相應的申請表單頁面
            // 這裡可以根據 GrantTypeID 判斷要導向哪個申請表單
            Response.Redirect($"~/OFS/SCI/SciApplication.aspx?GrantTypeID={selectedGrantTypeId}");
        }
        catch (Exception ex)
        {
            ShowMessage($"申請計畫時發生錯誤：{ex.Message}", false);
        }
    }
    
    // 載入可申請的計畫類別
    private void LoadAvailablePrograms()
    {
        try
        {
            // 使用統一的 Helper 方法載入計畫類別
            var grantTypes = ApplicationChecklistHelper.GetAvailableGrantTypes();
            
            ddlModalYear.Items.Clear();
            
            foreach (var item in grantTypes)
            {
                ddlModalYear.Items.Add(item);
            }
            
            // 如果沒有資料，添加預設選項
            if (ddlModalYear.Items.Count == 0)
            {
                ddlModalYear.Items.Add(new ListItem("目前沒有可申請的計畫", ""));
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"載入申請計畫類別時發生錯誤：{ex.Message}");
        }
    }
    
    /// <summary>
    /// 記錄案件操作到 OFS_CaseHistoryLog 資料表
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <param name="operationType">操作類型：撤案、恢復案件、刪除</param>
    /// <param name="description">操作描述或原因</param>
    /// <param name="stageStatusBefore">狀態變更前</param>
    /// <param name="stageStatusAfter">狀態變更後</param>
    private void LogCaseOperation(string projectId, string operationType, string description, string stageStatusBefore = "", string stageStatusAfter = "")
    {
        try
        {
            // 從Session取得使用者資訊
            var currentUser = GetCurrentUserInfo();
            string userName = currentUser?.UserName ?? "系統";
            
            // 建立案件歷程記錄
            var caseHistoryLog = new OFS_CaseHistoryLog
            {
                ProjectID = projectId,
                ChangeTime = DateTime.Now,
                UserName = userName,
                StageStatusBefore = stageStatusBefore,
                StageStatusAfter = stageStatusAfter,
                Description =description
            };
            
            // 儲存到資料庫
            bool success = ApplicationChecklistHelper.InsertCaseHistoryLog(caseHistoryLog);
            
            if (success)
            {
                System.Diagnostics.Debug.WriteLine($"案件操作記錄已儲存：{operationType} - {projectId}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"案件操作記錄儲存失敗：{operationType} - {projectId}");
            }
        }
        catch (Exception ex)
        {
            // 記錄失敗不影響主要操作，只記錄到Debug
            System.Diagnostics.Debug.WriteLine($"記錄案件操作失敗：{ex.Message}");
        }
    }
        
    
    /// <summary>
    /// 取得目前登入使用者資訊
    /// </summary>
    private SessionHelper.UserInfoClass GetCurrentUserInfo()
    {
        try
        {
            return SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得使用者資訊時發生錯誤: {ex.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 取得專案目前狀態
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <returns>目前狀態</returns>
    private string GetProjectCurrentStatus(string projectId)
    {
        try
        {
            // 從原始資料清單中找到對應的專案
            if (OriginalSciMainList != null)
            {
                var project = OriginalSciMainList.FirstOrDefault(p => p.ProjectID == projectId);
                if (project != null)
                {
                    // 如果是撤案狀態，優先顯示撤案
                    if (project.isWithdrawal == true)
                    {
                        return "已撤案";
                    }
                    
                    // 否則返回一般狀態
                    return project.Statuses+" "+project.StatusesName ?? "未知狀態";
                }
            }
            
            return "未知狀態";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得專案狀態時發生錯誤: {ex.Message}");
            return "未知狀態";
        }
    }
    
    // 處理撤案操作的 WebForm 方法
    protected void btnConfirmWithdraw_Click(object sender, EventArgs e)
    {
        try
        {
            string projectId = hdnWithdrawProjectId.Value;
            string reason = txtWithdrawReason.Text.Trim();
            
            if (string.IsNullOrEmpty(reason))
            {
                ShowMessage("請輸入撤案原因", false);
                return;
            }
            
            if (string.IsNullOrEmpty(projectId))
            {
                ShowMessage("系統錯誤：未找到專案資訊", false);
                return;
            }
            
            // 取得操作前的狀態
            string beforeStatus = GetProjectCurrentStatus(projectId);
            
            ApplicationChecklistHelper.UpdateWithdrawalStatus(projectId, true, reason);
            
            // 記錄操作到案件歷程
            LogCaseOperation(projectId, "撤案", reason, beforeStatus, "已撤案");
            
            // 清空輸入欄位
            txtWithdrawReason.Text = "";
            hdnWithdrawProjectId.Value = "";
            
            ShowMessage("撤案成功", true);
            
            // 重新載入資料
            LoadData();
            FilterData();
            BindData();
            UpdateTabStatisticsAndActiveState(hidSelectedStage.Value);
        }
        catch (Exception ex)
        {
            ShowMessage($"撤案時發生錯誤：{ex.Message}", false);
        }
    }
    
    // 處理撤案操作的 AJAX 方法 (保留以防其他地方使用)
    [System.Web.Services.WebMethod]
    public static string HandleWithdraw(string projectId, string reason)
    {
        try
        {
             ApplicationChecklistHelper.UpdateWithdrawalStatus(projectId, true, reason);
                return "success";
            
        }
        catch (Exception ex)
        {
            return $"撤案時發生錯誤：{ex.Message}";
        }
    }
    
    // 處理刪除操作的 WebForm 方法
    protected void btnConfirmDelete_Click(object sender, EventArgs e)
    {
        try
        {
            string projectId = hdnDeleteProjectId.Value;
            string reason = txtDeleteReason.Text.Trim();
            
            if (string.IsNullOrEmpty(reason))
            {
                ShowMessage("請輸入刪除原因", false);
                return;
            }
            
            if (string.IsNullOrEmpty(projectId))
            {
                ShowMessage("系統錯誤：未找到專案資訊", false);
                return;
            }
            
            // 取得操作前的狀態
            string beforeStatus = GetProjectCurrentStatus(projectId);
            
            ApplicationChecklistHelper.UpdateExistsStatus(projectId, false, reason);
            
            // 記錄操作到案件歷程
            LogCaseOperation(projectId, "刪除", reason, beforeStatus, "已刪除");
            
            // 清空輸入欄位
            txtDeleteReason.Text = "";
            hdnDeleteProjectId.Value = "";
            
            ShowMessage("刪除成功", true);
            
            // 重新載入資料
            LoadData();
            FilterData();
            BindData();
            UpdateTabStatisticsAndActiveState(hidSelectedStage.Value);
        }
        catch (Exception ex)
        {
            ShowMessage($"刪除時發生錯誤：{ex.Message}", false);
        }
    }
    
    // 處理刪除操作的 AJAX 方法 (保留以防其他地方使用)
    [System.Web.Services.WebMethod]
    public static string HandleDelete(string projectId, string reason)
    {
        try
        { 
            ApplicationChecklistHelper.UpdateExistsStatus(projectId, false, reason);
            return "success";
           
            
        }
        catch (Exception ex)
        {
            return $"刪除時發生錯誤：{ex.Message}";
        }
    }
    
    // 處理恢復案件操作的 WebForm 方法
    protected void btnConfirmRestore_Click(object sender, EventArgs e)
    {
        try
        {
            string projectId = hdnRestoreProjectId.Value;
            
            if (string.IsNullOrEmpty(projectId))
            {
                ShowMessage("系統錯誤：未找到專案資訊", false);
                return;
            }
            
            // 取得操作前的狀態
            string beforeStatus = GetProjectCurrentStatus(projectId);
            
            ApplicationChecklistHelper.UpdateWithdrawalStatus(projectId, false);
            
            // 重新載入資料以取得最新狀態
            LoadData();
            
            // 取得操作後的狀態（從重新載入的資料中取得）
            string afterStatus = GetProjectCurrentStatus(projectId);
            
            // 記錄操作到案件歷程
            LogCaseOperation(projectId, "恢復案件", "恢復已撤案的案件", beforeStatus, afterStatus);
            
            // 清空 HiddenField
            hdnRestoreProjectId.Value = "";
            
            ShowMessage("恢復案件成功", true);
            
            // 篩選和綁定資料
            FilterData();
            BindData();
            UpdateTabStatisticsAndActiveState(hidSelectedStage.Value);
        }
        catch (Exception ex)
        {
            ShowMessage($"恢復案件時發生錯誤：{ex.Message}", false);
        }
    }
    
    // 處理恢復案件操作的 AJAX 方法 (保留以防其他地方使用)
    [System.Web.Services.WebMethod]
    public static string HandleRestore(string projectId)
    {
        try
        {
            ApplicationChecklistHelper.UpdateWithdrawalStatus(projectId, false);
            
            return "success";
            
        }
        catch (Exception ex)
        {
            return $"恢復案件時發生錯誤：{ex.Message}";
        }
    }

    /// <summary>
    /// 取得案件歷程資料
    /// </summary>
    /// <param name="projectId">計畫ID</param>
    /// <returns>案件歷程資料</returns>
    [System.Web.Services.WebMethod]
    public static object GetCaseHistory(string projectId)
    {
        try
        {
            if (string.IsNullOrEmpty(projectId))
            {
                return new { success = false, message = "ProjectID 不能為空" };
            }

            var historyList = ApplicationChecklistHelper.GetCaseHistoryByProjectId(projectId);
            
            var result = historyList.Select(h => new
            {
                changeTime = h.ChangeTime.ToMinguoDateTime(),
                userName = h.UserName,
                stageChange = !string.IsNullOrEmpty(h.StageStatusBefore) && !string.IsNullOrEmpty(h.StageStatusAfter) 
                    ? $"{h.StageStatusBefore} → {h.StageStatusAfter}" 
                    : h.StageStatusAfter,
                description = h.Description
            }).ToList();

            return new { success = true, data = result };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得案件歷程時發生錯誤：{ex.Message}");
            return new { success = false, message = $"取得案件歷程時發生錯誤：{ex.Message}" };
        }
    }
    
    /// <summary>
    /// 取得審查意見回覆資料
    /// </summary>
    /// <param name="projectId">計畫ID</param>
    /// <returns>審查意見回覆資料</returns>
    [System.Web.Services.WebMethod]
    public static object GetReviewComments(string projectId)
    {
        try
        {
            if (string.IsNullOrEmpty(projectId))
            {
                return new { success = false, message = "ProjectID 不能為空" };
            }

            // 取得計畫基本資料
            var projectData = ApplicationChecklistHelper.GetProjectDataForReview(projectId);
            if (projectData == null)
            {
                return new { success = false, message = "找不到計畫資料" };
            }

            var reviewCommentsList = new List<object>();
            
            // 根據 ProjectID 判斷計畫類型並取得審查意見
            if (projectId.Contains("SCI"))
            {
                // 科專計畫處理
                string reviewStage = ApplicationChecklistHelper.GetCurrentReviewStage(projectId);
                var reviewCommentsTable = ReviewCheckListHelper.GetSciReviewComments(projectId, reviewStage);
                
                if (reviewCommentsTable != null && reviewCommentsTable.Rows.Count > 0)
                {
                    foreach (DataRow row in reviewCommentsTable.Rows)
                    {
                        reviewCommentsList.Add(new
                        {
                            
                            reviewerReviewID = row["ReviewID"]?.ToString() ?? "",
                            reviewerName = row["ReviewerName"]?.ToString() ?? "",
                            reviewComment = row["ReviewComment"]?.ToString() ?? "",
                            replyComment = row["ReplyComment"]?.ToString() ?? ""
                        });
                    }
                }
            }
            else if (projectId.Contains("CUL"))
            {
                // TODO: 文化計畫審查意見處理
                // TODO: 取得文化計畫的審查階段
                // TODO: 取得文化計畫的審查意見
            }
            else
            {
                return new { success = false, message = "不支援的計畫類型" };
            }

            var result = new
            {
                projectInfo = new
                {
                    year = projectData.Year,
                    ProjectID = projectData.ProjectID,
                    projectCategory = projectId.Contains("SCI") ? "科專" : projectId.Contains("CUL") ? "文化" : "其他",
                    reviewGroup = projectData.ReviewGroup,
                    projectName = projectData.ProjectName,
                    applicantUnit = projectData.ApplicantUnit
                },
                reviewComments = reviewCommentsList
            };
            
            return new { success = true, data = result };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得審查意見時發生錯誤：{ex.Message}");
            return new { success = false, message = $"取得審查意見時發生錯誤：{ex.Message}" };
        }
    }

    /// <summary>
    /// 更新審查意見回覆資料
    /// </summary>
    /// <param name="replies">回覆內容</param>
    /// <returns>審查意見回覆資料</returns>
    [System.Web.Services.WebMethod]
    public static object SubmitReply(List<ReplyItem> replies)
    {
        foreach (var item in replies)
        {
            string reviewId = item.reviewId;
            string content = item.replyContent;
            ApplicationChecklistHelper.UpdateReplyContent(reviewId, content);
        }
        
        return new { success = true, message = "已回覆"};
    }
}