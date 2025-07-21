using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using GS.OCA_OceanSubsidy.Operation.OFS;
using GS.App;

public partial class OFSApplicationMaster : System.Web.UI.MasterPage
{
    // 1. 模式枚舉
    public enum DisplayModeEnum
    {
        Edit,     // 編輯模式
        View      // 檢視模式
    }
    
    // 2. 模式屬性
    public DisplayModeEnum DisplayMode { get; set; } = DisplayModeEnum.Edit;
    
    // 3. 便利屬性
    public bool IsEditMode => DisplayMode == DisplayModeEnum.Edit;
    public bool IsViewMode => DisplayMode == DisplayModeEnum.View;
    
    // 4. 公開方法供內容頁面調用
    public void SetModeTo(string mode)
    {
        switch (mode.ToLower())
        {
            case "編輯":
                DisplayMode = DisplayModeEnum.Edit;
                break;
                
            case "檢視":
                DisplayMode = DisplayModeEnum.View;
                break;
                
            default:
                DisplayMode = DisplayModeEnum.View; // 預設為檢視模式（安全考量）
                break;
        }
        
        // 立即套用模式
        ApplyModeToPage();
    }
    
    private void ApplyModeToPage()
    {
        // 簡單的控制邏輯：禁用所有表單元素
        if (IsViewMode)
        {
            DisableAllControls(this.Page);
            AddViewModeStyles();
        }
    }
    
    private void DisableAllControls(Control parent)
    {
        foreach (Control control in parent.Controls)
        {
            // 只禁用按鈕類型的控制項
            if (control is System.Web.UI.WebControls.Button btn)
                btn.Enabled = false;
            else if (control is System.Web.UI.WebControls.LinkButton linkBtn)
                linkBtn.Enabled = false;
            else if (control is System.Web.UI.WebControls.ImageButton imgBtn)
                imgBtn.Enabled = false;
            else if (control is System.Web.UI.HtmlControls.HtmlInputButton htmlButton)
                htmlButton.Disabled = true;
            
            // 遞迴處理子控制項
            if (control.HasControls())
                DisableAllControls(control);
        }
    }
    
    private void AddViewModeStyles()
    {
        // 使用 JavaScript 添加檢視模式的 CSS 類別並禁用按鈕
        string script = @"
            document.addEventListener('DOMContentLoaded', function() {
                document.body.classList.add('app-mode-view');
                
                // 只禁用純 HTML 按鈕
                var buttons = document.querySelectorAll('button, input[type=""button""], input[type=""submit""], input[type=""reset""]');
                buttons.forEach(function(button) {
                    button.disabled = true;
                });
            });
        ";
        
        Page.ClientScript.RegisterStartupScript(this.GetType(), "AddViewModeStyles", script, true);
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        // 從URL參數中獲取ProjectID並設定到hidden欄位
        if (Request.QueryString["ProjectID"] != null)
        {
            hdnProjectID.Value = Request.QueryString["ProjectID"];
            
            // 檢查專案擁有權
            if (!ValidateProjectOwnership(Request.QueryString["ProjectID"]))
            {
                return; // 如果不是擁有者，已經導向到清單頁
            }
        }
        
        // 根據當前頁面設定進度條狀態
        SetStepStatus();
        
    }
    
    private void SetStepStatus()
    {
        string currentPage = System.IO.Path.GetFileName(Request.Path);
        string ProjectID = Request.QueryString["ProjectID"];
        
        // 建立頁面對應字典
        var pageMapping = new Dictionary<string, int>
        {
            { "SciApplication.aspx", 0 },     // 申請表/聲明書
            { "SciWorkSch.aspx", 1 },         // 期程／工作項目／查核
            { "SciFunding.aspx", 2 },         // 經費／人事
            { "SciRecusedList.aspx", 3 },     // 其他
            { "SciUploadAttachments.aspx", 4 } // 上傳附件/提送申請
        };
        
        var statusMapping = new Dictionary<int, string>
        {
            { 0, "Form1Status" },
            { 1, "Form2Status" },
            { 2, "Form3Status" },
            { 3, "Form4Status" },
            { 4, "Form5Status" }
        };
        
        string script = @"
            document.addEventListener('DOMContentLoaded', function() {
                var stepItems = document.querySelectorAll('.step-item');
                
                // 重設所有步驟狀態
                stepItems.forEach(function(item) {
                    item.classList.remove('active');
                    var statusElement = item.querySelector('.step-status');
                    if (statusElement) {
                        statusElement.remove();
                    }
                });
                
                function setStepActive(index, statusClass, statusText) {
                    if (stepItems[index]) {
                        stepItems[index].classList.add('active');
                        var stepContent = stepItems[index].querySelector('.step-content');
                        if (stepContent) {
                            var statusDiv = document.createElement('div');
                            statusDiv.className = 'step-status ' + statusClass;
                            statusDiv.textContent = statusText;
                            stepContent.appendChild(statusDiv);
                        }
                    }
                }
                
                function setStepActiveNoText(index) {
                    if (stepItems[index]) {
                        stepItems[index].classList.add('active');
                    }
                }
                
                function setStepLocked(index) {
                    if (stepItems[index]) {
                        stepItems[index].classList.add('locked');
                        stepItems[index].style.pointerEvents = 'none';
                        stepItems[index].style.cursor = 'not-allowed';
                        stepItems[index].style.opacity = '0.5';
                        
                        // 移除原有的 onclick 事件
                        stepItems[index].removeAttribute('onclick');
                        
                        // 添加新的點擊事件來顯示警告
                        stepItems[index].onclick = function(e) {
                            e.preventDefault();
                            e.stopPropagation();
                            alert('此步驟尚未開放，請依序完成前面的步驟。');
                            return false;
                        };
                    } else {
                    }
                }
        ";
        
        // 如果有 ProjectID，取得表單狀態並設定進度條
        if (!string.IsNullOrEmpty(ProjectID))
        {
            try
            {
                var ApplicationData = OFS_SciApplicationHelper.getVersionByProjectID(ProjectID);
                if (ApplicationData != null)
                {
                    // 取得 CurrentStep 值並轉換為數字
                    int currentStepNum = -1; // 預設為 -1 表示沒有設定
                    if (!string.IsNullOrEmpty(ApplicationData.CurrentStep) && int.TryParse(ApplicationData.CurrentStep, out currentStepNum))
                    {
                        // CurrentStep 從 1 開始，但陣列索引從 0 開始，所以要減 1
                        currentStepNum = currentStepNum - 1;
                    }
                    
                    
                    // 遍歷所有步驟 (根據 CurrentStep 最大值決定，目前支援 1~6)
                    int maxSteps = Math.Max(6, currentStepNum + 1); // 至少處理到 CurrentStep 或 6 步
                    for (int i = 0; i < maxSteps; i++)
                    {
                        string statusColumn = statusMapping.ContainsKey(i) ? statusMapping[i] : "";
                        string formStatus = "";
                        
                        // 只有在有對應的 statusColumn 時才查詢狀態
                        if (!string.IsNullOrEmpty(statusColumn))
                        {
                            formStatus = OFS_SciWorkSchHelper.GetFormStatusByProjectID(ProjectID, statusColumn);
                        }
                        
                        // 檢查是否為當前頁面
                        bool isCurrentPage = pageMapping.ContainsKey(currentPage) && pageMapping[currentPage] == i;
                        
                        // 檢查是否為 CurrentStep 對應的步驟
                        bool isCurrentStep = i == currentStepNum;
                        
                        // 檢查是否超過 CurrentStep（尚未開放的步驟）
                        bool isLocked = currentStepNum >= 0 && i > currentStepNum;
                        
                        if (isLocked && !isCurrentPage)
                        {
                            // 尚未開放的 步驟，鎖定不給使用
                            script += $"setStepLocked({i});";
                        }
                        else if (isCurrentPage)
                        {
                            // 使用者正在操作的頁面必定顯示「編輯中」
                            script += $"setStepActive({i}, 'edit', '編輯中');";
                        }
                        else if (isCurrentStep)
                        {
                            // CurrentStep 對應的欄位不顯示任何文字，div 隱藏
                            script += $"setStepActiveNoText({i});";
                        }
                        else if (formStatus == "完成")
                        {
                            // Form1Status ~ Form6Status 如果是「完成」則顯示已完成
                            script += $"setStepActive({i}, 'completed', '已完成');";
                        }
                        // 其他情況（空值、暫存等）不設定
                    }
                }
            }
            catch (Exception ex)
            {
                // 發生錯誤時使用預設邏輯
                System.Diagnostics.Debug.WriteLine($"取得表單狀態失敗: {ex.Message}");
                
                // 至少設定當前頁面顯示「編輯中」
                if (pageMapping.ContainsKey(currentPage))
                {
                    int currentIndex = pageMapping[currentPage];
                    script += $"setStepActive({currentIndex}, 'edit', '編輯中');";
                }
            }
        }
        else
        {
            // 沒有 ProjectID 時只開放第一個步驟，其他全部鎖定
            for (int i = 0; i < 6; i++)
            {
                bool isCurrentPage = pageMapping.ContainsKey(currentPage) && pageMapping[currentPage] == i;
                
                if (i == 0)
                {
                    // 第一個步驟永遠開放
                    if (isCurrentPage)
                    {
                        script += $"setStepActive({i}, 'edit', '編輯中');";
                    }
                    else
                    {
                        script += $"setStepActiveNoText({i});";
                    }
                }
                else
                {
                    // 其他步驟全部鎖定
                    script += $"setStepLocked({i});";
                }
            }
        }
        
        script += "});";
        
        Page.ClientScript.RegisterStartupScript(this.GetType(), "SetStepStatus", script, true);
    }
    
    /// <summary>
    /// 驗證專案擁有權 - 確認使用者是否為專案的擁有者
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <returns>true: 是擁有者或驗證通過, false: 不是擁有者</returns>
    private bool ValidateProjectOwnership(string projectID)
    {
        try
        {
            // 取得當前使用者資訊
            var currentUser = GetCurrentUserInfo();
            if (currentUser == null || string.IsNullOrEmpty(currentUser.Account))
            {
                // 沒有登入資訊，導向回清單頁
                RedirectToApplicationChecklist("未找到使用者登入資訊");
                return false;
            }
            
            // 取得專案資訊
            var projectData = OFS_SciApplicationHelper.getVersionByProjectID(projectID);
            if (projectData == null)
            {
                // 專案不存在，導向回清單頁
                RedirectToApplicationChecklist("專案不存在");
                return false;
            }
            
            // 檢查專案是否屬於當前使用者
            if (string.IsNullOrEmpty(projectData.UserAccount))
            {
                // 專案沒有設定擁有者，允許通過（可能是舊資料）
                return true;
            }
            
            // 比較使用者帳號，或檢查是否為主管單位人員
            if (projectData.UserAccount != currentUser.Account && !IsSupervisorRole(currentUser))
            {
                // 不是專案擁有者且不是主管單位人員，導向回清單頁
                RedirectToApplicationChecklist("無法檢視不是自己的專案");
                return false;
            }
            
            return true; // 驗證通過
        }
        catch (Exception ex)
        {
            // 發生錯誤時記錄並導向回清單頁
            System.Diagnostics.Debug.WriteLine($"驗證專案擁有權時發生錯誤: {ex.Message}");
            RedirectToApplicationChecklist("系統錯誤，請稍後再試");
            return false;
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
    /// 檢查使用者是否為主管單位人員（主管單位人員、主管單位窗口、系統管理者）
    /// </summary>
    /// <param name="userInfo">使用者資訊</param>
    /// <returns>true: 是主管單位人員, false: 不是主管單位人員</returns>
    private bool IsSupervisorRole(SessionHelper.UserInfoClass userInfo)
    {
        if (userInfo == null || userInfo.OFS_RoleName == null)
        {
            return false;
        }
        
        try
        {
            // 定義主管單位相關角色名稱
            var supervisorRoles = new[] { "主管單位人員", "主管單位窗口", "系統管理者" };
            
            // 檢查使用者的 OFS_RoleName 陣列中是否包含任一主管單位角色
            foreach (string roleName in userInfo.OFS_RoleName)
            {
                if (!string.IsNullOrEmpty(roleName) && supervisorRoles.Contains(roleName))
                {
                    return true;
                }
            }
            
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"檢查主管單位角色時發生錯誤: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 顯示警告訊息，使用者點選確認後導向到申請清單頁面
    /// </summary>
    /// <param name="message">要顯示的訊息</param>
    private void RedirectToApplicationChecklist(string message)
    {
        string safeMessage = System.Web.HttpUtility.JavaScriptStringEncode(message);
        string redirectUrl = Page.ResolveUrl("~/OFS/ApplicationChecklist.aspx");
        
        // 顯示警告訊息，等待使用者點選確認按鈕後再跳轉
        string script = $@"
            Swal.fire({{
                title: '存取權限',
                text: '{safeMessage}',
                icon: 'warning',
                confirmButtonText: '確定',
                allowOutsideClick: false,
                allowEscapeKey: false,
                customClass: {{
                    popup: 'animated fadeInDown'
                }}
            }}).then((result) => {{
                if (result.isConfirmed) {{
                    window.location.href = '{redirectUrl}';
                }}
            }});
        ";
        
        Page.ClientScript.RegisterStartupScript(this.GetType(), "RedirectWithConfirmation", script, true);
        
      
    }
    
    /// <summary>
    /// 顯示 SweetAlert2 訊息的通用方法
    /// </summary>
    /// <param name="message">訊息內容</param>
    /// <param name="title">標題</param>
    /// <param name="icon">圖示類型：success, error, warning, info</param>
    /// <param name="callback">點擊確定後的回調函式</param>
    public void ShowSweetAlert(string message, string title = "提示", string icon = "info", string callback = "")
    {
        string safeMessage = System.Web.HttpUtility.JavaScriptStringEncode(message);
        string safeTitle = System.Web.HttpUtility.JavaScriptStringEncode(title);
        
        string script = $@"
            Swal.fire({{
                title: '{safeTitle}',
                text: '{safeMessage}',
                icon: '{icon}',
                confirmButtonText: '確定',
                customClass: {{
                    popup: 'animated fadeInDown'
                }}
            }})";
        
        if (!string.IsNullOrEmpty(callback))
        {
            script += $".then(function() {{ {callback} }})";
        }
        
        script += ";";
        
        Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowSweetAlert" + Guid.NewGuid().ToString(), script, true);
    }
    
    /// <summary>
    /// 顯示成功訊息
    /// </summary>
    public void ShowSuccess(string message, string callback = "")
    {
        ShowSweetAlert(message, "成功", "success", callback);
    }
    
    /// <summary>
    /// 顯示錯誤訊息
    /// </summary>
    public void ShowError(string message, string callback = "")
    {
        ShowSweetAlert(message, "錯誤", "error", callback);
    }
    
    /// <summary>
    /// 顯示警告訊息
    /// </summary>
    public void ShowWarning(string message, string callback = "")
    {
        ShowSweetAlert(message, "警告", "warning", callback);
    }
    
    /// <summary>
    /// 顯示資訊訊息
    /// </summary>
    public void ShowInfo(string message, string callback = "")
    {
        ShowSweetAlert(message, "提示", "info", callback);
    }
    
}