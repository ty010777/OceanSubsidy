using Newtonsoft.Json;
using System;
using System.IO;
using System.Web;
using System.Web.SessionState;
using GS.OCA_OceanSubsidy.Operation.OFS;

public class UploadService : IHttpHandler, IRequiresSessionState
{
    public bool IsReusable => false;

    public void ProcessRequest(HttpContext context)
    {
        try
        {
            string action = context.Request["action"];

            // 處理 SciUploadAttachments 刪除功能
            if (action == "delete")
            {
                HandleSciAttachmentDelete(context);
                return;
            }

            // 保留原有的上傳邏輯
            HandleLegacyUpload(context);
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.Write(JsonConvert.SerializeObject(new { success = false, message = ex.Message }));
        }
    }

    private void HandleSciAttachmentDelete(HttpContext context)
    {
        string projectId = context.Request["projectId"];
        string fileCode = context.Request["fileCode"];

        if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(fileCode))
        {
            context.Response.ContentType = "application/json";
            context.Response.Write(JsonConvert.SerializeObject(new { success = false, message = "Missing required parameters" }));
            return;
        }

        try
        {
            // 使用 Helper 刪除檔案
            OFS_SciUploadAttachmentsHelper.DeleteAttachmentFile(projectId, fileCode);

            context.Response.ContentType = "application/json";
            context.Response.Write(JsonConvert.SerializeObject(new { success = true, message = "檔案刪除成功" }));
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.Write(JsonConvert.SerializeObject(new { success = false, message = $"刪除檔案失敗：{ex.Message}" }));
        }
    }

    private void HandleLegacyUpload(HttpContext context)
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

            OFSBaseFileHelper.insert(data);

            context.Response.ContentType = "application/json";
            context.Response.Write(JsonConvert.SerializeObject(new { success = true, data = data }));
        }
    }
}
