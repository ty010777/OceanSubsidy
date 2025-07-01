<%@ WebHandler Language="C#" Class="proxy" %>

using System;
using System.Web;
using System.Net;
using System.IO;

public class proxy : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        var req = context.Request;
        if (String.IsNullOrEmpty(req["url"]))
        {
            return;
        }
        HttpWebRequest targetReq = (HttpWebRequest)HttpWebRequest.Create(req["url"]);
        byte[] buffer = new byte[1024];
        var fromResp = targetReq.GetResponse();
        var fromStream = fromResp.GetResponseStream();
        context.Response.ContentType = fromResp.ContentType;
        var iCount = 0;
        do
        {
            iCount = fromStream.Read(buffer, 0, 1024);
            context.Response.OutputStream.Write(buffer, 0, iCount);
        } while (iCount != 0);
        context.Response.OutputStream.Flush();
        fromStream.Dispose();
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}