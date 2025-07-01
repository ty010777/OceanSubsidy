<%@ WebHandler Language="C#" Class="GetMapCaptureImg" %>

using System;
using System.Web;

public class GetMapCaptureImg : IHttpHandler {
    private static readonly string[] ALLOW_FILE_FORMATE = {"png", "jpg"};
    public void ProcessRequest (HttpContext context) {
        int statusCode = 200;
        var imgURL = context.Request["imgURL"];
        if (String.IsNullOrEmpty(imgURL)) statusCode = 404;
        var filePath = context.Server.MapPath(new Uri(imgURL).PathAndQuery);
        if (statusCode == 200)
        {
            String strFileFormat = System.IO.Path.GetExtension(filePath).Substring(1).ToLower();
            if (System.IO.File.Exists(filePath)
                && Array.IndexOf(ALLOW_FILE_FORMATE, strFileFormat) > -1)
            {
                if (filePath.EndsWith(".png"))
                {
                    context.Response.ContentType = "image/png";
                }
                else if (filePath.EndsWith(".jpg"))
                {
                    context.Response.ContentType = "image/jpeg";
                }
                context.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(System.IO.Path.GetFileName(filePath)));
                context.Response.WriteFile(filePath);
            }
            else
            {
                statusCode = 404;
            }
        }
        if (statusCode != 200)
        {
            context.Response.StatusCode = statusCode;
        }
        context.Response.End();
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}