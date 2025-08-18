<%@ WebHandler Language="C#" Class="OSI_StartFilledReminderEmail" %>

using System;
using System.Web;
using Newtonsoft.Json;

public class OSI_StartFilledReminderEmail : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        try
        {           
            int sentCount = OSIReminderHelper.SendStartFilledReminderEmailForTodayPeriods();

            var resp = new
            {
                success = true,
                sentPeriodCount = sentCount,
                message = sentCount > 0 ? $"已發送 {sentCount} 個期間的開始填報提醒信件" : "無需發送開始填報提醒信件"
            };
            context.Response.Write(JsonConvert.SerializeObject(resp));
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            var err = new { success = false, message = ex.Message };
            context.Response.Write(JsonConvert.SerializeObject(err));
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}