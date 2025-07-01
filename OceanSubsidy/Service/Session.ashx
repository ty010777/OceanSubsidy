<%@ WebHandler Language="C#" Class="Session" %>

using System;
using System.Web;
using System.Web.SessionState;
using System.Text.RegularExpressions;

public class Session : IHttpHandler, IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        string key = context.Request.QueryString["key"];
        // 從 Session 讀取資料，只接受英文或數字的key
        string sessionValue = (!string.IsNullOrWhiteSpace(key) && Regex.IsMatch(key, "^[a-zA-Z0-9]+$")) ? SessionHelper.Get<string>(key) : "";

        // 設定回傳的 ContentType 與資料格式
        context.Response.ContentType = "text/plain";
        context.Response.Write(sessionValue);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}