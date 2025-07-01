<%@ WebHandler Language="C#" Class="TGOS_Geocoder" %>

using System;
using System.Web;
using System.Configuration;
using TGOS.QueryAddr;

public class TGOS_Geocoder : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "application/json";
        WSQueryAddrSoapClient client = new WSQueryAddrSoapClient("WSQueryAddrSoap12");
        context.Response.Write(
            client.QueryAddr(
                    ConfigurationManager.AppSettings["togs.queryAddr.APPID"],
                    ConfigurationManager.AppSettings["togs.queryAddr.APIKey"],
                    context.Request["addr"],
                    "EPSG:4326",
                    2,
                    "JSON",
                    0,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    false,
                    true,
                    true,
                    true,
                    10
                )
            );
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}