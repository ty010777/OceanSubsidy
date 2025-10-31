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
            else
            {
                // PostBack 時，將 HiddenField 的值重新渲染到前端
                // 因為 contenteditable 不是標準表單控件，PostBack 後內容會消失
                RestoreContentFromHiddenFields();
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

            // PostBack 時不要重新載入資料,避免覆蓋使用者在前端輸入的值
            // (因為 LoadExistingChangeDescription 會設定 hdnChangeBefore.Value,覆蓋掉 LoadPostData 階段從 Request.Form 更新的值)
            if (!Page.IsPostBack && !string.IsNullOrEmpty(projectID))
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
    public ValidationResult ValidateChangeDescription(string changeBefore , string changeAfter)
    {
        var result = new ValidationResult();

        try
        {

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
            // 除錯:檢查隱藏欄位的值
            System.Diagnostics.Debug.WriteLine("=== SaveChangeDescription 開始 ===");
            System.Diagnostics.Debug.WriteLine($"hdnChangeBefore.Value: [{hdnChangeBefore.Value}]");
            System.Diagnostics.Debug.WriteLine($"hdnChangeAfter.Value: [{hdnChangeAfter.Value}]");

            // 1. 從隱藏欄位讀取值 (現在 PostBack 時不會被 LoadData 覆蓋了)
            string changeBefore = hdnChangeBefore.Value?.Trim();
            string changeAfter = hdnChangeAfter.Value?.Trim();

            System.Diagnostics.Debug.WriteLine($"最終 changeBefore: [{changeBefore}]");
            System.Diagnostics.Debug.WriteLine($"最終 changeAfter: [{changeAfter}]");
            System.Diagnostics.Debug.WriteLine("=== SaveChangeDescription 檢查完畢 ===");

            if (!changeDescriptionSection.Visible)
            { 
                return true;
            }
            // 2. 執行驗證並檢查結果
            var validationResult = ValidateChangeDescription(changeBefore, changeAfter);
            if (!validationResult.IsValid)
            {
                // 將錯誤訊息組合成字串並拋出例外
                string errorMessage = string.Join("; ", validationResult.Errors);
                throw new Exception($"驗證失敗：{errorMessage}");
            }
            // 3. 儲存到資料庫
            // 判斷 Method：計畫書修正 = 1, 計畫變更 = 2
            var project = OFS_SciApplicationHelper.getVersionByProjectID(projectID);
            int method = 2; // 預設為計畫書修正
            if (project.IsProjChanged == 1)
            {
                method = 1; // 計畫變更
            }
            try
            {
                string sourcePage = !string.IsNullOrEmpty(SourcePage) ? SourcePage : GetCurrentPageName();

                // 查詢是否已有現存的記錄
                var existingRecord = OFSProjectChangeRecordHelper.getApplying("SCI", method, projectID);

                if (existingRecord != null)
                {
                    // 更新現有記錄
                    HandleFormFields(existingRecord, sourcePage, changeBefore, changeAfter, isUpdate: true);
                    OFSProjectChangeRecordHelper.update(existingRecord);
                }
            }
            catch (Exception exDb)
            {
                throw new Exception($"儲存變更說明到資料庫時發生錯誤：{exDb.Message}", exDb);
            }

            return true;            

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
                   project.StatusesName == "計畫書審核中" ||
                     project.IsProjChanged == 1 || project.IsProjChanged == 2;
            
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
                var (changeBefore, changeAfter) = GetChangeDescriptionFromDatabase(projectID);

                ChangeBefore = changeBefore ?? "";
                ChangeAfter = changeAfter ?? "";
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
    private (string changeBefore, string changeAfter) GetChangeDescriptionFromDatabase(string projectID)
    {
        try
        {
            // 優先使用設定的 SourcePage，否則使用當前頁面名稱
            string sourcePage = !string.IsNullOrEmpty(SourcePage) ? SourcePage : GetCurrentPageName();
            // 取得專案狀態資訊來判斷是計畫變更還是計畫書修正
            var project = OFS_SciApplicationHelper.getVersionByProjectID(projectID);

            // 判斷 Method：計畫書修正 = 1, 計畫變更 = 2
            int method = 2; // 預設為計畫書修正
            if (project.IsProjChanged == 1 || project.IsProjChanged == 2)
            {
                method = 1; // 計畫變更
            }
            else if (project.StatusesName == "計畫書修正中")
            {
                method = 2; // 計畫書修正
            }
            // 從資料庫載入變更說明
            var changeRecord = OFSProjectChangeRecordHelper.getApplying("SCI",method, projectID);

            if (changeRecord == null)
            {
                return ("", "");
            }

            
            return  HandleFormFields(changeRecord, sourcePage);
        }
        catch (Exception ex)
        {
            HandleException(ex, "從資料庫取得變更說明時發生錯誤");
            return ("", "");
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
            // 使用 Predictable ClientID 而不是 Static ID
            string txtChangeBeforeID = txtChangeBefore.ClientID;

            string script = $@"
                setTimeout(function() {{
                    const element = document.getElementById('{txtChangeBeforeID}');
                    if (element) {{
                        element.textContent = '{EscapeForJavaScript(content)}';
                        console.log('已設定變更前內容:', '{txtChangeBeforeID}', '{EscapeForJavaScript(content)}');
                    }} else {{
                        console.warn('找不到變更前元素:', '{txtChangeBeforeID}');
                    }}
                }}, 100);
            ";

            Page.ClientScript.RegisterStartupScript(this.GetType(), $"SetChangeBeforeContent_{txtChangeBeforeID}", script, true);
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
            // 使用 Predictable ClientID 而不是 Static ID
            string txtChangeAfterID = txtChangeAfter.ClientID;

            string script = $@"
                setTimeout(function() {{
                    const element = document.getElementById('{txtChangeAfterID}');
                    if (element) {{
                        element.textContent = '{EscapeForJavaScript(content)}';
                        console.log('已設定變更後內容:', '{txtChangeAfterID}', '{EscapeForJavaScript(content)}');
                    }} else {{
                        console.warn('找不到變更後元素:', '{txtChangeAfterID}');
                    }}
                }}, 100);
            ";

            Page.ClientScript.RegisterStartupScript(this.GetType(), $"SetChangeAfterContent_{txtChangeAfterID}", script, true);
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
    document.addEventListener('DOMContentLoaded', function () {
        const editableElements = document.querySelectorAll('#changeDescriptionSection [contenteditable]');
        editableElements.forEach(function(element) {
            element.contentEditable = 'false';
            element.style.backgroundColor = '#e9ecef';
            element.style.cursor = 'default';
        });
    });
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
    /// 根據 SourcePage 取得對應的 Form 欄位操作
    /// </summary>
    /// <param name="record">ProjectChangeRecord 模型</param>
    /// <param name="sourcePage">來源頁面名稱</param>
    /// <param name="changeBefore">變更前內容（用於更新操作）</param>
    /// <param name="changeAfter">變更後內容（用於更新操作）</param>
    /// <param name="isUpdate">是否為更新操作</param>
    /// <returns>如果是讀取操作，回傳 (before, after)；如果是更新操作，回傳 ("", "")</returns>
    private (string before, string after) HandleFormFields(ProjectChangeRecord record, string sourcePage, string changeBefore = null, string changeAfter = null, bool isUpdate = false)
    {
        switch (sourcePage)
        {
            case "SciApplication":
                if (isUpdate)
                {
                    record.Form1Before = changeBefore;
                    record.Form1After = changeAfter;
                    return ("", "");
                }
                return (record.Form1Before ?? "", record.Form1After ?? "");

            case "SciWorkSch":
                if (isUpdate)
                {
                    record.Form2Before = changeBefore;
                    record.Form2After = changeAfter;
                    return ("", "");
                }
                return (record.Form2Before ?? "", record.Form2After ?? "");

            case "SciFunding":
                if (isUpdate)
                {
                    record.Form3Before = changeBefore;
                    record.Form3After = changeAfter;
                    return ("", "");
                }
                return (record.Form3Before ?? "", record.Form3After ?? "");

            case "SciRecusedList":
                if (isUpdate)
                {
                    record.Form4Before = changeBefore;
                    record.Form4After = changeAfter;
                    return ("", "");
                }
                return (record.Form4Before ?? "", record.Form4After ?? "");

            case "SciUploadAttachments":
                if (isUpdate)
                {
                    record.Form5Before = changeBefore;
                    record.Form5After = changeAfter;
                    return ("", "");
                }
                return (record.Form5Before ?? "", record.Form5After ?? "");

            default:
                if (isUpdate)
                {
                    record.Form1Before = changeBefore;
                    record.Form1After = changeAfter;
                    return ("", "");
                }
                return (record.Form1Before ?? "", record.Form1After ?? "");
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
                var (changeBefore, changeAfter) = GetChangeDescriptionBySourcePage(projectID, sourcePage);

                string changeBeforeEscaped = changeBefore?.Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "") ?? "";
                string changeAfterEscaped = changeAfter?.Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "") ?? "";

                script += $"'{sourcePage}': {{ 'ChangeBefore': '{changeBeforeEscaped}', 'ChangeAfter': '{changeAfterEscaped}' }}";

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
    public (string changeBefore, string changeAfter) GetChangeDescriptionBySourcePage(string projectID, string sourcePage)
    {
        try
        {
            var project = OFS_SciApplicationHelper.getVersionByProjectID(projectID);

            // 判斷 Method：計畫書修正 = 1, 計畫變更 = 2
            int method = 2; // 預設為計畫書修正
            if (project.IsProjChanged == 1)
            {
                method = 1; // 計畫變更
            }

            var changeRecord = OFSProjectChangeRecordHelper.getApplying("SCI",method, projectID);

            if (changeRecord == null)
            {
                return ("", "");
            }

            // 根據 SourcePage 取得對應的欄位內容

            return HandleFormFields(changeRecord, sourcePage);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得 {sourcePage} 變更說明資料時發生錯誤：{ex.Message}");
            return ("", "");
        }
    }

    /// <summary>
    /// 載入變更說明資料並直接渲染到控制項
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <param name="sourcePage">來源頁面</param>
    public void LoadChangeDescriptionAndRender(string projectID, string sourcePage)
    {
        try
        {
            var (changeBefore, changeAfter) = GetChangeDescriptionBySourcePage(projectID, sourcePage);

            // 使用 JavaScript 更新前端顯示
            string script = $@"
                (function() {{
                    var changeBeforeElement = document.getElementById('txtChangeBefore');
                    var changeAfterElement = document.getElementById('txtChangeAfter');

                    if (changeBeforeElement) {{
                        changeBeforeElement.textContent = {EscapeJavaScriptString(changeBefore)};
                    }}

                    if (changeAfterElement) {{
                        changeAfterElement.textContent = {EscapeJavaScriptString(changeAfter)};
                    }}

                    console.log('已渲染 {sourcePage} 的變更說明');
                }})();
            ";

            Page.ClientScript.RegisterStartupScript(this.GetType(), $"RenderChangeDescription_{sourcePage}_{Guid.NewGuid()}", script, true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"載入並渲染 {sourcePage} 變更說明時發生錯誤：{ex.Message}");
        }
    }

    /// <summary>
    /// 將字串轉換為安全的 JavaScript 字串格式
    /// </summary>
    private string EscapeJavaScriptString(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return "''";
        }

        // 使用 System.Web.HttpUtility.JavaScriptStringEncode 進行轉義
        string escaped = System.Web.HttpUtility.JavaScriptStringEncode(input);
        return $"'{escaped}'";
    }

    /// <summary>
    /// 根據 SourcePage 取得對應的 tab ID
    /// </summary>
    private string GetTabIdBySourcePage()
    {
        string sourcePage = !string.IsNullOrEmpty(SourcePage) ? SourcePage : GetCurrentPageName();

        switch (sourcePage)
        {
            case "SciApplication":
                return "tab1";
            case "SciWorkSch":
                return "tab2";
            case "SciFunding":
                return "tab3";
            case "SciRecusedList":
                return "tab4";
            case "SciUploadAttachments":
                return "tab5";
            default:
                return "tab1"; // 預設回傳第一個 tab
        }
    }

    /// <summary>
    /// 將字串轉義以便安全地嵌入 JavaScript 字串中
    /// </summary>
    private string EscapeForJavaScript(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return "";
        }

        return input
            .Replace("\\", "\\\\")
            .Replace("'", "\\'")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }

    /// <summary>
    /// 從 HiddenField 還原內容到前端 contenteditable 元素
    /// </summary>
    private void RestoreContentFromHiddenFields()
    {
        try
        {
            // 讀取 HiddenField 的值
            string changeBefore = hdnChangeBefore.Value ?? "";
            string changeAfter = hdnChangeAfter.Value ?? "";

            // 如果有值，則渲染到前端
            if (!string.IsNullOrEmpty(changeBefore))
            {
                SetChangeBeforeContent(changeBefore);
            }

            if (!string.IsNullOrEmpty(changeAfter))
            {
                SetChangeAfterContent(changeAfter);
            }

            System.Diagnostics.Debug.WriteLine($"PostBack 後還原變更說明: Before={changeBefore}, After={changeAfter}");
        }
        catch (Exception ex)
        {
            HandleException(ex, "從 HiddenField 還原內容時發生錯誤");
        }
    }

    #endregion
}