using System.IO;
using System.Web;
using System.Web.SessionState;

public class DownloadService : IHttpHandler, IRequiresSessionState
{
    public bool IsReusable => false;

    public void ProcessRequest(HttpContext context)
    {
        var data = OFSBaseFileHelper.getByPath(context.Request["path"]);

        if (data != null)
        {
            var path = Path.Combine(Path.GetFullPath(Path.Combine(context.Server.MapPath("~"), "..")), "UploadFiles", "files", data.Path);

            if (File.Exists(path))
            {
                context.Response.ContentType = data.Type;
                context.Response.WriteFile(path);
                return;
            }
        }

        context.Response.StatusCode = 404;
    }
}
