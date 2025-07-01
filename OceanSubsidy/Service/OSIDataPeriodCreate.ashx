<%@ WebHandler Language="C#" Class="OSIDataPeriodCreate" %>

using System;
using System.Web;
using Newtonsoft.Json;

public class OSIDataPeriodCreate : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        try
        {           
            int affectedRows = OSIDataPeriodsHelper.InsertNextYear();

            var resp = new
            {
                success = true,
                insertedCount = affectedRows
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