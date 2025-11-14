using System;
using System.IO;
using System.Web;
using System.Web.SessionState;
using GS.OCA_OceanSubsidy.Operation.OFS;

public class DownloadPdfService : IHttpHandler, IRequiresSessionState
{
    public bool IsReusable => false;

    public void ProcessRequest(HttpContext context)
    {
        try
        {
            var projectId = context.Request["ProjectID"].ToString();
            var version = context.Request["Version"]?.ToString();

            var path = Path.Combine(Path.GetFullPath(Path.Combine(context.Server.MapPath("~"), "..")), "UploadFiles", "OFS", projectId.Substring(0, 3), projectId);
            var file = "";

            if (version == "1")
            {
                file = Path.Combine(path, $"{projectId}_送審版.pdf");
            }
            else if (version == "2")
            {
                file = Path.Combine(path, $"{projectId}_計畫變更最新版.pdf");

                if (!File.Exists(file))
                {
                    file = Path.Combine(path, $"{projectId}_核定版.pdf");
                }
            }
            else
            {
                file = Path.Combine(path, $"{projectId}_評審版.pdf");
            }

            if (File.Exists(file))
            {
                var filename = Path.GetFileName(file);
                var encoded = HttpUtility.UrlPathEncode(filename);

                context.Response.ContentType = "application/pdf";
                context.Response.WriteFile(file);
                context.Response.AddHeader("Content-Disposition", $"attachment; filename=\"{encoded}\"; filename*=UTF-8''{Uri.EscapeDataString(filename)}");

                return;
            }

            context.Response.StatusCode = 404;
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.Write($"Error: {ex.Message}");
        }
    }
}
