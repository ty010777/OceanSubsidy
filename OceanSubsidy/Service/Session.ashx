<%@ WebHandler Language="C#" Class="Session" %>

using System;
using System.Web;
using System.Web.SessionState;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;

public class Session : IHttpHandler, IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        // 檢查是否為 POST 請求（用於延長 Session）
        if (context.Request.HttpMethod == "POST")
        {
            HandleSessionRefresh(context);
            return;
        }

        // 原有的 GET 請求邏輯
        string key = context.Request.QueryString["key"];
        
        if (!string.IsNullOrWhiteSpace(key) && Regex.IsMatch(key, "^[a-zA-Z0-9]+$"))
        {
            if (key == "UserInfo")
            {
                // 特殊處理 UserInfo，檢查是否已登入
                var userInfo = SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo);
                string result = userInfo != null ? "logged_in" : "";
                context.Response.ContentType = "text/plain";
                context.Response.Write(result);
            }
            else
            {
                // 其他 Session 資料
                string sessionValue = SessionHelper.Get<string>(key);
                context.Response.ContentType = "text/plain";
                context.Response.Write(sessionValue ?? "");
            }
        }
        else
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("");
        }
    }

    private void HandleSessionRefresh(HttpContext context)
    {
        try
        {
            // 讀取 POST 資料
            string requestBody;
            using (var reader = new StreamReader(context.Request.InputStream))
            {
                requestBody = reader.ReadToEnd();
            }

            // 解析 JSON
            dynamic requestData = JsonConvert.DeserializeObject(requestBody);
            string action = requestData?.action;

            if (action == "refresh")
            {
                // 檢查用戶是否已登入
                var userInfo = SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo);
                if (userInfo != null)
                {
                    // 延長 Session - 存取 Session 就會自動延長，並更新最後活動時間
                    SessionHelper.Set(SessionHelper.UserInfo, userInfo);
                    context.Session["LastActivityTime"] = DateTime.Now;
                    
                    // 回傳成功訊息
                    context.Response.ContentType = "application/json";
                    context.Response.Write(JsonConvert.SerializeObject(new { success = true, message = "Session 已延長" }));
                }
                else
                {
                    // 用戶未登入
                    context.Response.ContentType = "application/json";
                    context.Response.Write(JsonConvert.SerializeObject(new { success = false, message = "用戶未登入" }));
                }
            }
            else
            {
                context.Response.ContentType = "application/json";
                context.Response.Write(JsonConvert.SerializeObject(new { success = false, message = "無效的操作" }));
            }
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.Write(JsonConvert.SerializeObject(new { success = false, message = "處理請求時發生錯誤: " + ex.Message }));
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}