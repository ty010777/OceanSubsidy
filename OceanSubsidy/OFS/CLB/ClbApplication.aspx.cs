using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Model.OFS;
using Newtonsoft.Json;
using GS.App;

public partial class OFS_CLB_ClbApplication : System.Web.UI.Page
{
    #region 屬性

    /// <summary>
    /// 計畫ID
    /// </summary>
    protected string ProjectID => Request.QueryString["ProjectID"];

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        // 驗證專案擁有權
        if (!ValidateProjectOwnership(ProjectID))
        {
            return; // 如果不是擁有者，已經導向到清單頁
        }

        if (!IsPostBack)
        {
            // 初始化頁面
            InitializePage();
        }
    }

    private void InitializePage()
    {
        try
        {
            // 檢查 URL 參數
            string projectID = Request.QueryString["ProjectID"];

            // 取得專案狀態
            var projectMain = OFS_ClbApplicationHelper.GetProjectMainData(projectID);

            // 判斷是否為「計畫執行」狀態
            if (projectMain != null && projectMain.Statuses == "計畫執行")
            {
                // 載入快照資料
                LoadSnapshotData(projectID);
            }
            else
            {
                // 載入正常資料
                LoadNormalMode(projectID);
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "頁面初始化時發生錯誤");
            ShowAlert("錯誤", "頁面載入失敗，請稍後再試", "error");
        }
    }

    /// <summary>
    /// 一般模式：載入計畫資料
    /// </summary>
    private void LoadNormalMode(string projectID)
    {
        // 將 ProjectID 傳遞給 UserControl，讓它自己處理載入邏輯
        ucClbApplication.ProjectID = projectID ?? "";

        // 判斷並設定檢視模式
        SetViewModeBasedOnStatus(projectID);
    }

    /// <summary>
    /// 載入快照資料
    /// </summary>
    private void LoadSnapshotData(string projectID)
    {
        try
        {
            // 取得 Project_Main 的 ID
            int projectMainID = GetProjectMainID(projectID);
            if (projectMainID == 0)
            {
                ShowAlertAndRedirect("錯誤", "找不到計畫資料", "error");
                return;
            }

            // 從 ProjectID 載入最新快照 (Status = 11 表示送審版的快照)
            Snapshot snapshot = OFSSnapshotHelper.get("CLB", projectMainID, 11);

            if (snapshot == null)
            {
                ShowAlertAndRedirect("錯誤", "找不到快照資料，將返回申請列表", "error");
                return;
            }

            // 解析快照資料並載入到 UserControl
            var snapshotData = JsonConvert.DeserializeObject<dynamic>(snapshot.Data);
            if (snapshotData != null)
            {
                ucClbApplication.LoadFromSnapshot(snapshotData);
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入快照資料時發生錯誤");
            ShowAlertAndRedirect("錯誤", "載入快照資料時發生錯誤，將返回申請列表", "error");
        }
    }

    /// <summary>
    /// 取得 Project_Main 的 ID
    /// </summary>
    private int GetProjectMainID(string projectID)
    {
        try
        {
            var projectMain = OFS_ClbApplicationHelper.GetProjectMainData(projectID);
            return projectMain?.ID ?? 0;
        }
        catch (Exception ex)
        {
            HandleException(ex, "取得 ProjectMainID 時發生錯誤");
            return 0;
        }
    }

    /// <summary>
    /// 根據專案狀態設定檢視模式
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    private void SetViewModeBasedOnStatus(string projectID)
    {
        try
        {
            // 如果 projectID 為空或找不到，顯示編輯模式
            if (string.IsNullOrEmpty(projectID))
            {
                ucClbApplication.IsReadOnly = false;
            }
            else
            {
                // 從資料庫取得專案狀態資訊
                var projectMain = OFS_ClbApplicationHelper.GetProjectMainData(projectID);

                if (projectMain == null)
                {
                    // 找不到專案資料，顯示編輯模式
                    ucClbApplication.IsReadOnly = false;
                }
                else
                {
                    string statuses = projectMain.Statuses ?? "";
                    string statusesName = projectMain.StatusesName ?? "";

                    // 判斷狀態是否允許編輯
                    if ((statuses == "尚未提送" && statusesName == "編輯中") ||
                        (statuses == "決審核定" && statusesName == "計畫書修正中") ||
                        (statuses == "內容審查" && statusesName == "補正補件"))
                    {
                        // 可編輯狀態
                        ucClbApplication.IsReadOnly = false;
                    }
                    else
                    {
                        // 其他狀態都是檢視模式
                        ucClbApplication.IsReadOnly = true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // 發生錯誤時，預設為編輯模式
            ucClbApplication.IsReadOnly = false;
            HandleException(ex, "設定檢視模式時發生錯誤");
        }
    }

    /// <summary>
    /// 顯示 SweetAlert 提示訊息
    /// </summary>
    private void ShowAlert(string title, string text, string icon)
    {
        try
        {
            string script = $@"
                Swal.fire({{
                    title: '{title}',
                    text: '{text}',
                    icon: '{icon}',
                    confirmButtonText: '確定'
                }});
            ";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowAlert", script, true);
        }
        catch (Exception ex)
        {
            HandleException(ex, "顯示提示訊息時發生錯誤");
        }
    }

    /// <summary>
    /// 顯示 SweetAlert 提示訊息並重新導向到申請列表
    /// </summary>
    private void ShowAlertAndRedirect(string title, string text, string icon)
    {
        try
        {
            // 取得虛擬路徑
            string appRootPath = System.Configuration.ConfigurationManager.AppSettings["AppRootPath"] ?? "";

            string script = $@"
                window.AppRootPath = '{appRootPath}';
                Swal.fire({{
                    title: '{title}',
                    text: '{text}',
                    icon: '{icon}',
                    confirmButtonText: '確定'
                }}).then((result) => {{
                    // 使用者點擊確定後，重新導向到申請列表
                    window.location.href = window.AppRootPath + '/OFS/ApplicationChecklist.aspx';
                }});
            ";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowAlertAndRedirect", script, true);
        }
        catch (Exception ex)
        {
            HandleException(ex, "顯示提示訊息時發生錯誤");
            // 如果顯示訊息失敗，直接重新導向
            Response.Redirect("~/OFS/ApplicationChecklist.aspx");
        }
    }

    /// <summary>
    /// 例外處理
    /// </summary>
    private void HandleException(Exception ex, string context)
    {
        System.Diagnostics.Debug.WriteLine($"{context}: {ex.Message}");
        System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
    }

    #region 專案擁有權驗證

    /// <summary>
    /// 驗證專案擁有權 - 確認使用者是否為專案的擁有者
    /// </summary>
    /// <param name="projectID">專案ID</param>
    /// <returns>true: 是擁有者或驗證通過, false: 不是擁有者</returns>
    private bool ValidateProjectOwnership(string projectID)
    {
        try
        {
            // 如果沒有 projectID，允許新建專案
            if (string.IsNullOrEmpty(projectID))
            {
                return true; // ClbApplication.aspx 允許新建專案
            }

            // 取得當前使用者資訊
            var currentUser = GetCurrentUserInfo();
            if (currentUser == null || string.IsNullOrEmpty(currentUser.Account))
            {
                // 沒有登入資訊，導向回清單頁
                RedirectToApplicationChecklist("未找到使用者登入資訊");
                return false;
            }

            // 取得專案資訊
            var projectData = OFS_ClbApplicationHelper.GetProjectMainData(projectID);
            if (projectData == null)
            {
                // 專案不存在，導向回清單頁
                RedirectToApplicationChecklist("專案不存在");
                return false;
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
            HandleException(ex, "驗證專案擁有權時發生錯誤");
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
            HandleException(ex, "取得使用者資訊時發生錯誤");
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
            HandleException(ex, "檢查主管單位角色時發生錯誤");
            return false;
        }
    }

    /// <summary>
    /// 直接導向到申請清單頁面（不顯示訊息）
    /// </summary>
    /// <param name="message">錯誤訊息（記錄用，不顯示給使用者）</param>
    private void RedirectToApplicationChecklist(string message)
    {
        // 記錄錯誤訊息到 Debug
        System.Diagnostics.Debug.WriteLine($"RedirectToApplicationChecklist: {message}");

        // 直接跳轉到申請清單頁面
        string redirectUrl = Page.ResolveUrl("~/OFS/ApplicationChecklist.aspx");
        Response.Redirect(redirectUrl, false);
        Context.ApplicationInstance.CompleteRequest();
    }

    #endregion
}