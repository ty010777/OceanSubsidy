<%@ WebHandler Language="C#" Class="DownloadTemplateCUL" %>

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using GS.OCA_OceanSubsidy.Operation.OSI.OpenXml;

public class DownloadTemplateCUL : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        var templateCate = context.Request.QueryString["cate"];
        var templateType = context.Request.QueryString["type"];
        var id = context.Request.QueryString["id"];

        string tempFile = Path.GetTempFileName();
        string docName = string.Empty;

        if (string.IsNullOrEmpty(templateCate) || string.IsNullOrEmpty(templateType) || string.IsNullOrEmpty(id))
        {
            context.Response.StatusCode = 400;
            context.Response.Write("Missing parameters.");
            return;
        }
        else if (templateCate == "CUL")
        {
            if (templateType == "1")
            {
                docName = "1.申請表.docx";
                string templatePath = context.Server.MapPath($"~/Template/CUL/{docName}");
                if (!File.Exists(templatePath))
                {
                    context.Response.StatusCode = 404;
                    context.Response.Write("Template not found.");
                    return;
                }
                else
                {
                    var service = new CultureService();
                    var param = new JObject();
                    param["ID"] = id;

                    var data = service.getApplication(param, context);
                    var json = JsonConvert.SerializeObject(data);
                    var jobj = JObject.Parse(json);

                    var placeholder = new Dictionary<string, string>();
                    placeholder.Add("{{A1}}", jobj["Project"]["ProjectID"]?.ToString() ?? "");
                    placeholder.Add("{{A2}}", jobj["Project"]["ProjectName"]?.ToString() ?? "");
                    // placeholder.Add("{{A3}}", jobj["Project"]["OrgName"]?.ToString() ?? "");
                    placeholder.Add("{{A4}}", jobj["Project"]["OrgName"]?.ToString() ?? "");
                    placeholder.Add("{{A5}}", jobj["Project"]["RegisteredNum"]?.ToString() ?? "");
                    placeholder.Add("{{A6}}", jobj["Project"]["TaxID"]?.ToString() ?? "");
                    placeholder.Add("{{A7}}", jobj["Project"]["Address"]?.ToString() ?? "");
                    placeholder.Add("{{A8.1}}", jobj["Contacts"][0]["Name"]?.ToString() ?? "");
                    placeholder.Add("{{A8.2}}", jobj["Contacts"][0]["JobTitle"]?.ToString() ?? "");
                    placeholder.Add("{{A8.3}}", jobj["Contacts"][0]["Phone"]?.ToString() ?? "" + " #" + jobj["Contacts"][0]["Mobile"]?.ToString() ?? "");
                    placeholder.Add("{{A8.4}}", jobj["Contacts"][0]["MobilePhone"]?.ToString() ?? "");
                    placeholder.Add("{{A8.5}}", jobj["Contacts"][0]["EMail"]?.ToString() ?? "");
                    placeholder.Add("{{A9.1}}", jobj["Contacts"][1]["Name"]?.ToString() ?? "");
                    placeholder.Add("{{A9.2}}", jobj["Contacts"][1]["JobTitle"]?.ToString() ?? "");
                    placeholder.Add("{{A9.3}}", jobj["Contacts"][1]["Phone"]?.ToString() ?? "" + " #" + jobj["Contacts"][1]["Mobile"]?.ToString() ?? "");
                    placeholder.Add("{{A9.4}}", jobj["Contacts"][1]["MobilePhone"]?.ToString() ?? "");
                    placeholder.Add("{{A9.5}}", jobj["Contacts"][1]["EMail"]?.ToString() ?? "");
                    placeholder.Add("{{A10}}", jobj["Project"]["Target"]?.ToString() ?? "");
                    placeholder.Add("{{A11}}", jobj["Project"]["Summary"]?.ToString() ?? "");
                    placeholder.Add("{{A12.1}}", jobj["Project"]["Quantified"]?.ToString() ?? "");
                    placeholder.Add("{{A12.2}}", jobj["Project"]["Qualitative"]?.ToString() ?? "");
                    // placeholder.Add("{{A13}}", jobj["Project"]["Target"]?.ToString() ?? "");
                    // 期程
                    string start = jobj["Project"]["StartTime"]?.ToString();
                    string end = jobj["Project"]["EndTime"]?.ToString();
                    string startShort = DateTime.TryParse(start, out var sdt) ? sdt.ToString("yyyy/MM/dd") : "";
                    string endShort = DateTime.TryParse(end, out var edt) ? edt.ToString("yyyy/MM/dd") : "";
                    placeholder.Add("{{B1}}", $"{startShort} ~ {endShort}");
                    // 補助款
                    var applyAmountStr = jobj["Project"]["ApplyAmount"]?.ToString() ?? "0";
                    int applyAmount = int.TryParse(applyAmountStr, out var a1) ? a1 : 0;
                    placeholder.Add("{{C1.1}}", (applyAmount / 10000).ToString());
                    placeholder.Add("{{C1.2}}", (applyAmount % 10000).ToString());
                    // 自籌款
                    var selfAmountStr = jobj["Project"]["SelfAmount"]?.ToString() ?? "0";
                    int selfAmount = int.TryParse(selfAmountStr, out var a2) ? a2 : 0;
                    placeholder.Add("{{C2.1}}", (selfAmount / 10000).ToString());
                    placeholder.Add("{{C2.2}}", (selfAmount % 10000).ToString());
                    // 其他
                    var otherAmountStr = jobj["Project"]["OtherAmount"]?.ToString() ?? "0";
                    int otherAmount = int.TryParse(otherAmountStr, out var a3) ? a3 : 0;
                    placeholder.Add("{{C3.1}}", (otherAmount / 10000).ToString());
                    placeholder.Add("{{C3.2}}", (otherAmount % 10000).ToString());
                    // ReceivedSubsidies
                    var subsidies = jobj["ReceivedSubsidies"] as JArray;
                    for (int i = 0; i < 3; i++)
                    {
                        string nameKey = $"{{{{A13.{i * 3 + 1}}}}}";
                        string unitKey = $"{{{{A13.{i * 3 + 2}}}}}";
                        string amountKey = $"{{{{A13.{i * 3 + 3}}}}}";
                        if (subsidies != null && subsidies.Count > i)
                        {
                            var obj = subsidies[i];
                            placeholder[nameKey] = obj["Name"]?.ToString() ?? "";
                            placeholder[unitKey] = obj["Unit"]?.ToString() ?? "";
                            placeholder[amountKey] = obj["Amount"]?.ToString() ?? "";
                        }
                        else
                        {
                            placeholder[nameKey] = "";
                            placeholder[unitKey] = "";
                            placeholder[amountKey] = "";
                        }
                    }
                    // 年月日
                    var now = DateTime.Now;
                    int Year = now.Year - 1911;
                    int Month = now.Month;
                    int Day = now.Day;
                    placeholder.Add("{{Year}}", Year.ToString());
                    placeholder.Add("{{Month}}", Month.ToString());
                    placeholder.Add("{{Day}}", Day.ToString());

                    var repeatData = new List<Dictionary<string, string>>();

                    File.Copy(templatePath, tempFile, true);

                    using (var fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite))
                    {
                        var helper = new OpenXmlHelper(fs);
                        helper.GenerateWord(placeholder, repeatData);
                        helper.CloseAsSave();
                    }
                }
            }
        }

        context.Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        context.Response.AddHeader("Content-Disposition", $"attachment; filename={docName}");
        context.Response.WriteFile(tempFile);

        context.Response.Flush();
        context.Response.End();
        File.Delete(tempFile);
    }

    public bool IsReusable => false;
}
