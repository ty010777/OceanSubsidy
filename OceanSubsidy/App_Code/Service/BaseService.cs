using GS.OCA_OceanSubsidy.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.SessionState;

public class BaseService : IHttpHandler, IRequiresSessionState
{
    public static string toTwDate(DateTime date)
    {
        return $"{date.Year - 1911}/{date.Month:D2}/{date.Day:D2}";
    }

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

    public object getPaymentPhaseSettings(JObject param, HttpContext context)
    {
        return OFS_SciReimbursementHelper.GetPaymentPhaseSettings(param["TypeCode"].ToString());
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

    protected JObject getSnapshot(string type, int id)
    {
        var prev = OFSSnapshotHelper.get(type, id);

        return prev == null ? null : JObject.Parse(prev.Data);
    }

    protected void saveApplyLog(string pID, string before)
    {
        saveLog(pID, before, "資格審查-審核中", "完成附件上傳並提送申請");
    }

    protected void saveApplyReviewLog(string pID, string after, string note = null, DateTime? deadline = null)
    {
        var desc = string.IsNullOrWhiteSpace(note) ? "" : $"因{note}原因「{after}」";

        desc = (deadline == null) ? desc : $"{desc}，補正期限：{deadline}";

        saveLog(pID, "資格審查-審查中", after, desc);
    }

    protected void saveRevisionLog(string pID)
    {
        saveLog(pID, "決審核定-計畫書修正中", "決審核定-計畫書審核中", "完成計畫書修正並重新提送審核");
    }

    protected void saveRevisionReviewLog(string pID, string after, string note = null)
    {
        var desc = string.IsNullOrWhiteSpace(note) ? "" : $"因{note}原因「{after}」";

        saveLog(pID, "決審核定-計畫書審核中", after, desc);
    }

    private void saveLog(string pID, string before, string after, string desc)
    {
        ApplicationChecklistHelper.InsertCaseHistoryLog(new OFS_CaseHistoryLog
        {
            ProjectID = pID,
            ChangeTime = DateTime.Now,
            UserName = CurrentUser.UserName,
            StageStatusBefore = before,
            StageStatusAfter = after,
            Description = desc
        });
    }

    private void WriteJson(HttpContext context, object obj)
    {
        context.Response.ContentType = "application/json";
        context.Response.Write(JsonConvert.SerializeObject(obj));
    }
}
