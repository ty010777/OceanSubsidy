using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;

public partial class Logout : System.Web.UI.Page
{
    private static readonly ILog log = LogManager.GetLogger(typeof(Logout));

    protected void Page_Load(object sender, EventArgs e)
    {
        // 取得使用者資訊並記錄登出日誌
        var userInfo = SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo);
        string account = userInfo?.Account ?? "未知使用者";
        string userId = userInfo?.UserID ?? "未知ID";
        string clientIP = GetClientIP();

        // 記錄登出日誌
        log.Info($"使用者登出 - 帳號: {account}, UserID: {userId}, IP: {clientIP}");

        // 清除所有 Session
        Session.Clear();
        Session.Abandon();

        // 清除認證 Cookie (如果有使用)
        if (Request.Cookies["ASP.NET_SessionId"] != null)
        {
            HttpCookie sessionCookie = new HttpCookie("ASP.NET_SessionId");
            sessionCookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(sessionCookie);
        }

        // 導向到登入頁面，並帶上登出成功的參數
        Response.Redirect("~/Login.aspx?msg=logout", false);
        Context.ApplicationInstance.CompleteRequest();
    }

    // 取得客戶端 IP
    private string GetClientIP()
    {
        string clientIP = "";

        // 嘗試從不同的 header 取得真實 IP（優先順序由高到低）
        string[] headers = {
            "HTTP_X_FORWARDED_FOR",
            "HTTP_X_REAL_IP",
            "HTTP_CLIENT_IP",
            "HTTP_X_CLUSTER_CLIENT_IP",
            "HTTP_FORWARDED_FOR",
            "HTTP_FORWARDED"
        };

        foreach (string header in headers)
        {
            string value = Request.ServerVariables[header];
            if (!string.IsNullOrEmpty(value))
            {
                // 處理多個 IP 的情況（以逗號分隔）
                string[] ips = value.Split(',');
                foreach (string ip in ips)
                {
                    string trimmedIP = ip.Trim();
                    if (!string.IsNullOrEmpty(trimmedIP) &&
                        !trimmedIP.StartsWith("10.") &&
                        !trimmedIP.StartsWith("192.168.") &&
                        !trimmedIP.StartsWith("172.") &&
                        trimmedIP != "127.0.0.1" &&
                        trimmedIP != "::1")
                    {
                        clientIP = trimmedIP;
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(clientIP))
                    break;
            }
        }

        // 如果沒有找到公網 IP，使用 UserHostAddress
        if (string.IsNullOrEmpty(clientIP))
        {
            clientIP = Request.UserHostAddress;
        }

        // 將 IPv6 localhost 轉換為 IPv4 格式
        if (clientIP == "::1")
        {
            clientIP = "127.0.0.1";
        }

        return clientIP ?? "";
    }
}