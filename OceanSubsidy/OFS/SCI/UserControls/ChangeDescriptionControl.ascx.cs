using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Model.OFS;
using GS.OCA_OceanSubsidy.Operation.OFS;

/// <summary>
/// 變更說明 UserControl
/// 只有在 StatusesName = '計畫書修正中' 時才會顯示
/// </summary>
public partial class OFS_SCI_UserControls_ChangeDescriptionControl : System.Web.UI.UserControl
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
    public string SourcePage { get; set; }

    /// <summary>
    /// 變更前內容
    /// </summary>
    public string ChangeBefore 
    { 
        get { return hdnChangeBefore.Value; }
        set { 
            hdnChangeBefore.Value = value;
            // 同時設定前端顯示
            SetChangeBeforeContent(value);
        }
    }

    /// <summary>
    /// 變更後內容
    /// </summary>
    public string ChangeAfter 
    { 
        get { return hdnChangeAfter.Value; }
        set { 
            hdnChangeAfter.Value = value;
            // 同時設定前端顯示
            SetChangeAfterContent(value);
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
        }
        catch (Exception ex)
        {
            HandleException(ex, "變更說明控制項載入時發生錯誤");
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

            // 檢查顯示條件
            bool shouldShow = CheckDisplayCondition(projectID);
            changeDescriptionSection.Visible = shouldShow;

            if (!string.IsNullOrEmpty(projectID))
            {
                // 載入已儲存的變更說明資料
                LoadExistingChangeDescription(projectID);
            }

            // 套用檢視模式
            if (isViewMode && shouldShow)
            {
                ApplyViewMode();
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入變更說明資料時發生錯誤");
        }
    }

    /// <summary>
    /// 驗證變更說明資料
    /// </summary>
    /// <returns>驗證結果</returns>
    public ValidationResult ValidateChangeDescription()
    {
        var result = new ValidationResult();

        try
        {
            // 如果區塊不可見，跳過驗證
            if (!changeDescriptionSection.Visible)
            {
                return result;
            }

            // 同步前端資料到隱藏欄位
            SyncContentFromFrontend();

            string changeBefore = hdnChangeBefore.Value?.Trim();
            string changeAfter = hdnChangeAfter.Value?.Trim();

            // 如果有填寫任何一個欄位，則兩個都必須填寫
            if (!string.IsNullOrEmpty(changeBefore) || !string.IsNullOrEmpty(changeAfter))
            {
                if (string.IsNullOrEmpty(changeBefore))
                {
                    result.AddError("請填寫變更前的內容");
                }

                if (string.IsNullOrEmpty(changeAfter))
                {
                    result.AddError("請填寫變更後的內容");
                }
            }
        }
        catch (Exception ex)
        {
            result.AddError($"驗證變更說明時發生錯誤：{ex.Message}");
        }

        return result;
    }

    /// <summary>
    /// 儲存變更說明資料
    /// </summary>
    /// <returns>儲存是否成功</returns>
    public bool SaveChangeDescription(string projectID)
    {
        this.ProjectID = projectID;
        try
        {
            // 如果區塊不可見，不需要儲存
            if (!changeDescriptionSection.Visible)
            {
                return true;
            }
            string pageName = SourcePage;
            // 同步前端資料到隱藏欄位
            SyncContentFromFrontend();
            
            string changeBefore = hdnChangeBefore.Value?.Trim();
            string changeAfter = hdnChangeAfter.Value?.Trim();
            
            // 儲存到資料庫（這裡需要根據實際的資料表結構來實作）
            SaveChangeDescriptionToDatabase(projectID,pageName, changeBefore, changeAfter);

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"儲存變更說明時發生錯誤：{ex.Message}", ex);
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
        hdnChangeBefore.Value = "";
        hdnChangeAfter.Value = "";
    }

    /// <summary>
    /// 檢查是否符合顯示條件
    /// </summary>
    /// <param name="projectID">計畫ID</param>
    /// <returns>是否應該顯示</returns>
    private bool CheckDisplayCondition(string projectID)
    {
        try
        {
            if (string.IsNullOrEmpty(projectID))
            {
                return false;
            }

            // 取得專案狀態資訊
            var project = OFS_SciApplicationHelper.getVersionByProjectID(projectID);
            
            // 取得當前頁面名稱和 SourcePage
            string currentPageName = GetCurrentPageName();
            string sourcePage = !string.IsNullOrEmpty(SourcePage) ? SourcePage : currentPageName;
            
            // 如果是在審核頁面中（SciFinalReview），則總是顯示
            if (currentPageName == "SciFinalReview")
            {
                return true; // 審核頁面總是顯示變更說明區塊，讓審核者查看
            }
            
            // 其他頁面只有在特定狀態下才顯示
            return project.StatusesName == "計畫書修正中" ||
                   project.StatusesName == "計畫書審查中";
            
        }
        catch (Exception ex)
        {
            HandleException(ex, "檢查顯示條件時發生錯誤");
            return false;
        }
    }
    

    /// <summary>
    /// 載入已存在的變更說明資料
    /// </summary>
    /// <param name="projectID">專案ID</param>
    private void LoadExistingChangeDescription(string projectID)
    {
        try
        {
            // 如果是 SciFinalReview 模式，載入所有 SourcePage 的變更說明
            if (SourcePage == "SciFinalReview")
            {
                LoadAllChangeDescriptionsForReview(projectID);
            }
            else
            {
                // 一般模式：從資料庫載入已儲存的變更說明
                var changeDescription = GetChangeDescriptionFromDatabase(projectID);
                
                if (changeDescription != null)
                {
                    ChangeBefore = changeDescription.ChangeBefore ?? "";
                    ChangeAfter = changeDescription.ChangeAfter ?? "";
                }
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入變更說明資料時發生錯誤");
        }
    }

    /// <summary>
    /// 從資料庫取得變更說明資料
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <returns>變更說明資料</returns>
    private OFS_SCI_PageModifyNote GetChangeDescriptionFromDatabase(string projectID)
    {
        try
        {
            // 優先使用設定的 SourcePage，否則使用當前頁面名稱
            string sourcePage = !string.IsNullOrEmpty(SourcePage) ? SourcePage : GetCurrentPageName();
            
            // 從資料庫載入變更說明
            return OFS_SciApplicationHelper.GetPageModifyNote(projectID, sourcePage);
        }
        catch (Exception ex)
        {
            HandleException(ex, "從資料庫取得變更說明時發生錯誤");
            return null;
        }
    }

    /// <summary>
    /// 儲存變更說明到資料庫
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <param name="pageName">頁面名稱</param>
    /// <param name="changeBefore">變更前內容</param>
    /// <param name="changeAfter">變更後內容</param>
    private void SaveChangeDescriptionToDatabase(string projectID, string pageName, string changeBefore, string changeAfter)
    {
        try
        {
            // 優先使用設定的 SourcePage，否則使用當前頁面名稱
            string sourcePage = !string.IsNullOrEmpty(SourcePage) ? SourcePage : GetCurrentPageName();
            
            // 儲存到資料庫
            bool success = OFS_SciApplicationHelper.SavePageModifyNote(projectID, sourcePage, changeBefore, changeAfter);
            
            if (!success)
            {
                throw new Exception("資料庫儲存操作失敗");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"儲存變更說明到資料庫時發生錯誤：{ex.Message}", ex);
        }
    }

    /// <summary>
    /// 同步前端內容到隱藏欄位
    /// </summary>
    private void SyncContentFromFrontend()
    {
        // 透過 JavaScript 同步，實際的同步會在前端進行
        string script = @"
            if (typeof syncChangeDescription === 'function') {
                syncChangeDescription();
            }
        ";
        
        Page.ClientScript.RegisterStartupScript(this.GetType(), "SyncChangeDescription", script, true);
    }

    /// <summary>
    /// 設定變更前內容到前端
    /// </summary>
    /// <param name="content">內容</param>
    private void SetChangeBeforeContent(string content)
    {
        if (!string.IsNullOrEmpty(content))
        {
            string script = $@"
                setTimeout(function() {{
                    const element = document.getElementById('txtChangeBefore');
                    if (element) {{
                        element.textContent = '{content.Replace("'", "\\'")}';
                    }}
                }}, 100);
            ";
            
            Page.ClientScript.RegisterStartupScript(this.GetType(), "SetChangeBeforeContent", script, true);
        }
    }

    /// <summary>
    /// 設定變更後內容到前端
    /// </summary>
    /// <param name="content">內容</param>
    private void SetChangeAfterContent(string content)
    {
        if (!string.IsNullOrEmpty(content))
        {
            string script = $@"
                setTimeout(function() {{
                    const element = document.getElementById('txtChangeAfter');
                    if (element) {{
                        element.textContent = '{content.Replace("'", "\\'")}';
                    }}
                }}, 100);
            ";
            
            Page.ClientScript.RegisterStartupScript(this.GetType(), "SetChangeAfterContent", script, true);
        }
    }

    /// <summary>
    /// 套用檢視模式
    /// </summary>
    private void ApplyViewMode()
    {
        if (IsViewMode)
        {
            string script = @"
                setTimeout(function() {
                    // 停用所有 contenteditable 元素
                    const editableElements = document.querySelectorAll('#changeDescriptionSection [contenteditable]');
                    editableElements.forEach(function(element) {
                        element.setAttribute('contenteditable', 'false');
                        element.style.backgroundColor = '#f8f9fa';
                        element.style.cursor = 'default';
                    });
                }, 100);
            ";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "ApplyChangeDescriptionViewMode", script, true);
        }
    }

    /// <summary>
    /// 取得目前頁面名稱
    /// </summary>
    /// <returns>頁面名稱</returns>
    private string GetCurrentPageName()
    {
        try
        {
            // 嘗試從 Parent.Page 取得頁面名稱
            if (Page != null)
            {
                string pageName = System.IO.Path.GetFileNameWithoutExtension(Page.Request.Url.AbsolutePath);
                
                // 根據頁面名稱對應到標準名稱
                switch (pageName.ToLower())
                {
                    case "sciapplication":
                        return "SciApplication";
                    case "sciworksch":
                        return "SciWorkSch";
                    case "scifunding":
                        return "SciFunding";
                    case "scioutcomes":
                        return "SciOutcomes";
                    case "sciuploadattachments":
                        return "SciUploadAttachments";
                    case "scirecusedlist":
                        return "SciRecusedList";
                    case "scifinalreview":
                        return "SciFinalReview";
                    default:
                        return pageName;
                }
            }
            
            return "Unknown";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得頁面名稱時發生錯誤：{ex.Message}");
            return "Unknown";
        }
    }

    /// <summary>
    /// 處理例外錯誤
    /// </summary>
    /// <param name="ex">例外物件</param>
    /// <param name="context">錯誤內容</param>
    private void HandleException(Exception ex, string context)
    {
        // 記錄錯誤
        System.Diagnostics.Debug.WriteLine($"{context}: {ex.Message}");

        // 可以在這裡加入更多錯誤處理邏輯，如記錄到日誌
    }

    #endregion

    #region 公用載入方法

    /// <summary>
    /// 載入變更說明資料到固定ID的輸入框中
    /// </summary>
    /// <param name="projectID">計畫ID</param>
    /// <param name="sourcePage">來源頁面名稱</param>
    public static void LoadChangeDescriptionToElements(string projectID, string sourcePage)
    {
        try
        {
            if (string.IsNullOrEmpty(projectID) || string.IsNullOrEmpty(sourcePage))
            {
                return;
            }

            // 從資料庫取得變更說明資料
            var changeDescription = OFS_SciApplicationHelper.GetPageModifyNote(projectID, sourcePage);
            
            if (changeDescription == null)
            {
                return;
            }

            // 產生 JavaScript 來設定輸入框的值
            string script = $@"
                setTimeout(function() {{
                    // 設定變更前的內容
                    const changeBeforeElement = document.getElementById('txtChangeBefore');
                    if (changeBeforeElement && '{changeDescription.ChangeBefore?.Replace("'", "\\'")}') {{
                        changeBeforeElement.textContent = '{changeDescription.ChangeBefore?.Replace("'", "\\'")}';
                    }}
                    
                    // 設定變更後的內容
                    const changeAfterElement = document.getElementById('txtChangeAfter');
                    if (changeAfterElement && '{changeDescription.ChangeAfter?.Replace("'", "\\'")}') {{
                        changeAfterElement.textContent = '{changeDescription.ChangeAfter?.Replace("'", "\\'")}';
                    }}
                }}, 100);
            ";

            // 註冊 JavaScript 到頁面
            if (HttpContext.Current?.Handler is Page page)
            {
                page.ClientScript.RegisterStartupScript(typeof(Page), "LoadChangeDescription", script, true);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"載入變更說明到輸入框時發生錯誤：{ex.Message}");
        }
    }

    /// <summary>
    /// 載入所有 SourcePage 的變更說明資料供審核頁面使用
    /// </summary>
    /// <param name="projectID">專案ID</param>
    private void LoadAllChangeDescriptionsForReview(string projectID)
    {
        try
        {
            var sourcePages = new[] { "SciApplication", "SciWorkSch", "SciFunding", "SciRecusedList", "SciUploadAttachments" };
            
            // 將所有 SourcePage 的變更說明資料寫入 JavaScript 變數
            string script = "window.allChangeDescriptions = {";
            
            for (int i = 0; i < sourcePages.Length; i++)
            {
                var sourcePage = sourcePages[i];
                var changeDescription = GetChangeDescriptionBySourcePage(projectID, sourcePage);
                
                string changeBefore = changeDescription?.ChangeBefore?.Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "") ?? "";
                string changeAfter = changeDescription?.ChangeAfter?.Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "") ?? "";
                
                script += $"'{sourcePage}': {{ 'ChangeBefore': '{changeBefore}', 'ChangeAfter': '{changeAfter}' }}";
                
                if (i < sourcePages.Length - 1)
                {
                    script += ",";
                }
            }
            
            script += "};";
            script += "console.log('已載入所有變更說明資料:', window.allChangeDescriptions);";
            
            // 註冊 JavaScript 到頁面
            if (HttpContext.Current?.Handler is Page page)
            {
                page.ClientScript.RegisterStartupScript(typeof(Page), "LoadAllChangeDescriptions", script, true);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"載入所有變更說明資料時發生錯誤：{ex.Message}");
        }
    }
    
    /// <summary>
    /// 根據 SourcePage 取得變更說明資料
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <param name="sourcePage">來源頁面</param>
    /// <returns>變更說明資料</returns>
    private OFS_SCI_PageModifyNote GetChangeDescriptionBySourcePage(string projectID, string sourcePage)
    {
        try
        {
            return OFS_SciApplicationHelper.GetPageModifyNote(projectID, sourcePage);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得 {sourcePage} 變更說明資料時發生錯誤：{ex.Message}");
            return null;
        }
    }

    #endregion
}