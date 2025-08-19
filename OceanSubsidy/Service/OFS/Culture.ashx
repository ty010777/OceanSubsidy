<%@ WebHandler Language="C#" Class="CultureService" %>

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.SessionState;

public class CultureService : IHttpHandler, IRequiresSessionState
{
    public bool IsReusable => false;

    public void ProcessRequest(HttpContext context)
    {
        JObject input = null;

        try
        {
            input = JObject.Parse(new StreamReader(context.Request.InputStream).ReadToEnd());
        }
        catch
        {
            WriteJson(context, new { success = false, error = "Invalid JSON format" });

            return;
        }

        if (input == null || !input.TryGetValue("method", out JToken obj) || obj == null)
        {
            WriteJson(context, new { success = false, error = "Missing method" });

            return;
        }

        try
        {
            var name = obj.ToString();
            var method = typeof(CultureServiceHandler).GetMethod(name);

            if (method == null)
            {
                WriteJson(context, new { success = false, error = $"Method '{name}' not found" });

                return;
            }

            input.TryGetValue("param", out JToken param);

            WriteJson(context, new { success = true, data = method.Invoke(new CultureServiceHandler(), new object[] { (JObject) param, context }) });
        }
        catch (Exception ex)
        {
            WriteJson(context, new { success = false, error = "Exception: " + ex.ToString() });
        }
    }

    private void WriteJson(HttpContext context, object obj)
    {
        context.Response.ContentType = "application/json";
        context.Response.Write(JsonConvert.SerializeObject(obj));
    }
}
