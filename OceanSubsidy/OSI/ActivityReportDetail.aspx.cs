using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.Extension;
using GS.OCA_OceanSubsidy.Entity;

public partial class OSI_ActivityReportDetail : System.Web.UI.Page
{
    private SessionHelper.UserInfoClass UserInfo
    {
        get => SessionHelper.Get<SessionHelper.UserInfoClass>(
                  SessionHelper.UserInfo)
                ?? new SessionHelper.UserInfoClass();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // 如果 URL 有 ?id=xx 就切到編輯模式
            if (int.TryParse(Request.QueryString["id"], out var id) && id > 0)
            {
                // 使用共用權限檢查類別進行權限驗證
                var permissionResult = OSIReportPermissionHelper.CheckReportPermission(id, UserInfo);
                
                if (!permissionResult.HasPermission)
                {
                    // 記錄未授權存取嘗試
                    OSIReportPermissionHelper.LogUnauthorizedAccess(
                        UserInfo, 
                        id.ToString(), 
                        "Report", 
                        Request.UserHostAddress
                    );
                    
                    // 處理無權限的情況
                    HandleUnauthorizedAccess(permissionResult.DeniedReason);
                    return;
                }
                
                ReportForm.ReportID = id;
            }
            else
            {
                ReportForm.IsNew = true;
            }
            
            // 檢查是否從儲存後重新導向過
            if (Request.QueryString["saved"] == "1")
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    this.GetType(),
                    "saveSuccess",
                    "showGlobalMessage('儲存成功');",
                    true
                );
            }
        }
    }

    /// <summary>
    /// 處理未授權存取的情況
    /// </summary>
    /// <param name="deniedReason">拒絕原因</param>
    private void HandleUnauthorizedAccess(string deniedReason)
    {
        try
        {
            // 顯示錯誤訊息並重新導向
            string safeReason = HttpUtility.JavaScriptStringEncode(deniedReason);
            string returnUrl = ResolveUrl(GetReturnUrl());
            string safeReturnUrl = HttpUtility.JavaScriptStringEncode(returnUrl);
            
            string script = $@"
                showGlobalMessage('{safeReason}');
                setTimeout(function() {{
                    window.location.href = '{safeReturnUrl}';
                }}, 2000);
            ";

            ScriptManager.RegisterStartupScript(
                this,
                this.GetType(),
                "unauthorizedAccess",
                script,
                true
            );
        }
        catch (Exception ex)
        {
            // 如果發生錯誤，直接重新導向
            System.Diagnostics.Debug.WriteLine($"HandleUnauthorizedAccess Error: {ex.Message}");
            Response.Redirect(GetReturnUrl(), false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }

    /// <summary>
    /// 決定返回的URL
    /// </summary>
    /// <returns>安全的返回URL</returns>
    private string GetReturnUrl()
    {
        // 1. 優先使用 QueryString 中的 returnUrl
        string returnUrl = Request.QueryString["returnUrl"];
        if (IsValidReturnUrl(returnUrl))
        {
            return returnUrl;
        }

        // 2. 預設返回報告列表頁面
        return "~/OSI/ActivityReports.aspx";
    }

    /// <summary>
    /// 驗證返回URL是否安全且有效
    /// </summary>
    /// <param name="url">要驗證的URL</param>
    /// <returns>安全且有效返回true</returns>
    private bool IsValidReturnUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
            return false;

        try
        {
            // 必須是相對路徑
            if (!url.StartsWith("~/"))
                return false;

            // 不能包含危險字符
            if (url.Contains("..") || url.Contains("//") || url.Contains(@"\\"))
                return false;

            // 必須是 .aspx 檔案
            if (!url.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
                return false;

            // 必須在 OSI 資料夾下
            if (!url.StartsWith("~/OSI/", StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
        catch
        {
            return false;
        }
    }
}