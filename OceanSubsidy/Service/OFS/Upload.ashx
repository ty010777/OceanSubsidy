<%@ WebHandler Language="C#" Class="UploadService" %>

using Newtonsoft.Json;
using System;
using System.IO;
using System.Web;
using System.Web.SessionState;

public class UploadService : IHttpHandler, IRequiresSessionState
{
    public bool IsReusable => false;

    public void ProcessRequest(HttpContext context)
    {
        if (context.Request.Files.Count > 0)
        {
            var date = DateTime.Now.ToString("yyMMdd");
            var folder = Path.Combine(Path.GetFullPath(Path.Combine(context.Server.MapPath("~"), "..")), "UploadFiles", "files", date);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var file = context.Request.Files[0];
            var name = Path.GetRandomFileName() + Path.GetExtension(file.FileName);

            file.SaveAs(Path.Combine(folder, name));

            var data = new BaseFile
            {
                Name = file.FileName,
                Path = Path.Combine(date, name),
                Size = file.ContentLength,
                Type = file.ContentType
            };

            BaseFileHelper.insert(data);

            context.Response.ContentType = "application/json";
            context.Response.Write(JsonConvert.SerializeObject(new { success = true, data = data }));
        }
    }
}
