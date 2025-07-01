<%@ WebHandler Language="C#" Class="UserActive" %>

using System;
using System.Web;
using Newtonsoft.Json;

public class UserActive : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        try
        {
            int affectedRows = SysUserHelper.UpdateActiveByLastLoginTime();

            var result = new
            {
                success = true,
                updatedCount = affectedRows
            };
            string json = JsonConvert.SerializeObject(result);
            context.Response.Write(json);
        }
        catch (Exception ex)
        {
            // 發生錯誤時回傳 500 + 錯誤訊息
            context.Response.StatusCode = 500;
            var err = new { success = false, message = ex.Message };
            context.Response.Write(
                JsonConvert.SerializeObject(err)
            );
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