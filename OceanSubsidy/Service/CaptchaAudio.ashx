<%@ WebHandler Language="C#" Class="CaptchaAudio" %>

using System;
using System.Web;
using System.Web.SessionState;
using System.Text;

public class CaptchaAudio : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

        try
        {
            string captchaCode = SessionHelper.Get<string>(SessionHelper.LoginValidate);
            
            if (string.IsNullOrEmpty(captchaCode))
            {
                context.Response.StatusCode = 404;
                context.Response.Write("{\"error\":\"Captcha code not found\"}");
                return;
            }

            StringBuilder speechText = new StringBuilder();
            for (int i = 0; i < captchaCode.Length; i++)
            {
                speechText.Append(captchaCode[i].ToString());
                if (i < captchaCode.Length - 1)
                {
                    speechText.Append(" ");
                }
            }

            string jsonResponse = "{\"text\":\"" + speechText.ToString() + "\",\"success\":true}";
            context.Response.Write(jsonResponse);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("CaptchaAudio Error: " + ex.Message);
            context.Response.StatusCode = 500;
            context.Response.Write("{\"error\":\"Unable to generate captcha audio\"}");
        }
    }

    public bool IsReusable
    {
        get { return false; }
    }
}