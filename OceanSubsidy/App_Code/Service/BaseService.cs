using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.SessionState;

public class BaseService : IHttpHandler, IRequiresSessionState
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
            var method = GetType().GetMethod(name);

            if (method == null)
            {
                WriteJson(context, new { success = false, error = $"Method '{name}' not found" });

                return;
            }

            input.TryGetValue("param", out JToken param);

            WriteJson(context, new { success = true, data = method.Invoke(this, new object[] { (JObject) param, context }) });
        }
        catch (Exception ex)
        {
            WriteJson(context, new { success = false, message = ex.InnerException?.Message, exception = ex.ToString() });
        }
    }

    public object getGrantTargetSetting(JObject param, HttpContext context)
    {
        return OFSGrantTargetSettingHelper.getByTargetTypeID(param["TargetTypeID"].ToString());
    }

    public object getZgsCodes(JObject param, HttpContext context)
    {
        return SysZgsCodeHelper.getZgsCodes(param["CodeGroup"].ToString());
    }

    public object queryReviewUnits(JObject param, HttpContext context)
    {
        return SysUnitHelper.QueryReviewUnits();
    }

    public object queryReviewersByUnit(JObject param, HttpContext context)
    {
        return SysUserHelper.QueryReviewersByUnitID(param["ID"].ToString());
    }

    private void WriteJson(HttpContext context, object obj)
    {
        context.Response.ContentType = "application/json";
        context.Response.Write(JsonConvert.SerializeObject(obj));
    }
}
