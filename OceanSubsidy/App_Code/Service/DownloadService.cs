using System;
using System.IO;
using System.Web;
using System.Web.SessionState;
using GS.OCA_OceanSubsidy.Operation.OFS;

public class DownloadService : IHttpHandler, IRequiresSessionState
{
    public bool IsReusable => false;

    public void ProcessRequest(HttpContext context)
    {
        try
        {
            string action = context.Request["action"];
            
            // 新增 SciUploadAttachments 下載功能
            if (action == "download" || action == "preview")
            {
                HandleSciAttachmentDownload(context, action == "preview");
                return;
            }
            
            // 保留原有邏輯
            HandleLegacyDownload(context);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.Write($"Error: {ex.Message}");
        }
    }
    
    private void HandleSciAttachmentDownload(HttpContext context, bool isPreview)
    {
        string projectId = context.Request["projectId"];
        string fileCode = context.Request["fileCode"];
        string fileName = context.Request["fileName"];
        
        if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(fileCode) || string.IsNullOrEmpty(fileName))
        {
            context.Response.StatusCode = 400;
            context.Response.Write("Missing required parameters");
            return;
        }
        
        // 驗證檔案是否存在於資料庫
        var attachments = OFS_SciUploadAttachmentsHelper.GetAttachmentsByFileCodeAndProject(projectId, fileCode);
        var attachment = attachments.Find(a => a.FileName == fileName);
        
        if (attachment == null)
        {
            context.Response.StatusCode = 404;
            context.Response.Write("File not found in database");
            return;
        }
        
        // 建構檔案實體路徑
        string filePath = context.Server.MapPath($"~/{attachment.TemplatePath}");
        
        if (!File.Exists(filePath))
        {
            context.Response.StatusCode = 404;
            context.Response.Write("Physical file not found");
            return;
        }
        
        // 設定回應標頭
        context.Response.ContentType = "application/pdf";
        
        if (isPreview)
        {
            context.Response.AddHeader("Content-Disposition", $"inline; filename=\"{fileName}\"");
        }
        else
        {
            context.Response.AddHeader("Content-Disposition", $"attachment; filename=\"{fileName}\"");
        }
        
        // 傳送檔案
        context.Response.WriteFile(filePath);
    }
    
    private void HandleLegacyDownload(HttpContext context)
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
