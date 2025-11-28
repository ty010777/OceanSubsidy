<%@ WebHandler Language="C#" Class="DownloadTemplateEDC" %>

using GS.OCA_OceanSubsidy.Operation.OSI.OpenXml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;

public class DownloadTemplateEDC : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        var templateType = context.Request.QueryString["type"];
        var id = context.Request.QueryString["id"];

        string tempFile = Path.GetTempFileName();
        string docName = string.Empty;

        if (string.IsNullOrEmpty(templateType) || string.IsNullOrEmpty(id))
        {
            context.Response.StatusCode = 400;
            context.Response.Write("Missing parameters.");
            return;
        }
        else if (templateType == "1")
        {
            docName = "1-申請表.docx";
            string templatePath = context.Server.MapPath($"~/Template/EDC/{docName}");
            if (!File.Exists(templatePath))
            {
                context.Response.StatusCode = 404;
                context.Response.Write("Template not found.");
                return;
            }
            else
            {
                var educationService = new EducationService();

                // 取得基本資料
                var data = educationService.getApplication(new JObject { ["ID"] = id }, context);
                var jobj = JObject.Parse(JsonConvert.SerializeObject(data));
                // 取得申請補助單位
                var baseService = new BaseService();
                var edcOrgCategory = baseService.getZgsCodes(new JObject { ["CodeGroup"] = "EDCOrgCategory" }, context);
                foreach(var obj in JArray.Parse(JsonConvert.SerializeObject(edcOrgCategory)))
                {
                    if (obj["Code"]?.ToString() == jobj["Project"]["OrgCategory"]?.ToString())
                    {
                        jobj["Project"]["OrgCategoryName"] = obj["Descname"]?.ToString() ?? "";
                    }
                }
                var placeholder = new Dictionary<string, string>();
                placeholder.Add("{{A1}}", jobj["Project"]["ProjectName"]?.ToString() ?? "");
                placeholder.Add("{{A3}}", jobj["Project"]["OrgCategoryName"]?.ToString() ?? "");
                placeholder.Add("{{A4}}", jobj["Project"]["TaxID"]?.ToString() ?? "");
                placeholder.Add("{{A5}}", jobj["Project"]["RegisteredNum"]?.ToString() ?? "");
                placeholder.Add("{{A6}}", jobj["Project"]["Address"]?.ToString() ?? "");
                placeholder.Add("{{A7.1}}", jobj["Contacts"][0]["Name"]?.ToString() ?? "");
                placeholder.Add("{{A7.2}}", jobj["Contacts"][0]["JobTitle"]?.ToString() ?? "");
                placeholder.Add("{{A8.1}}", jobj["Contacts"][1]["Name"]?.ToString() ?? "");
                placeholder.Add("{{A8.2}}", jobj["Contacts"][1]["JobTitle"]?.ToString() ?? "");
                placeholder.Add("{{A8.3}}", jobj["Contacts"][1]["Phone"]?.ToString() ?? "");
                placeholder.Add("{{A8.4}}", jobj["Contacts"][1]["MobilePhone"]?.ToString() ?? "");
                // 期程
                string start = jobj["Project"]["StartTime"]?.ToString();
                string end = jobj["Project"]["EndTime"]?.ToString();
                string startShort = DateTime.TryParse(start, out var sdt) ? $"{sdt.Year - 1911}/{sdt.Month:D2}/{sdt.Day:D2}" : "";
                string endShort = DateTime.TryParse(end, out var edt) ? $"{edt.Year - 1911}/{edt.Month:D2}/{edt.Day:D2}" : "";
                placeholder.Add("{{A9}}", $"{startShort} ~ {endShort}");
                placeholder.Add("{{A11}}", jobj["Project"]["Summary"]?.ToString() ?? "");
                placeholder.Add("{{A12}}", jobj["Project"]["Quantified"]?.ToString() ?? "");

                // 申請本會分攤金額
                int applyAmount = jobj["Project"]["ApplyAmount"]?.ToObject<int>() ?? 0;
                placeholder.Add("{{A13.1}}", (applyAmount / 10000).ToString());
                placeholder.Add("{{A13.2}}", (applyAmount % 10000).ToString());

                // 單位自籌款
                int selfAmount = jobj["Project"]["SelfAmount"]?.ToObject<int>() ?? 0;
                placeholder.Add("{{A14.1}}", (selfAmount / 10000).ToString());
                placeholder.Add("{{A14.2}}", (selfAmount % 10000).ToString());

                // 其他政府機關補助款
                int otherGovAmount = jobj["Project"]["OtherGovAmount"]?.ToObject<int>() ?? 0;
                placeholder.Add("{{A15.1}}", (otherGovAmount / 10000).ToString());
                placeholder.Add("{{A15.2}}", (otherGovAmount % 10000).ToString());

                // 其他單位補助款
                int otherUnitAmount = jobj["Project"]["OtherUnitAmount"]?.ToObject<int>() ?? 0;
                placeholder.Add("{{A16.1}}", (otherUnitAmount / 10000).ToString());
                placeholder.Add("{{A16.2}}", (otherUnitAmount % 10000).ToString());

                // 計畫總經費
                int total = applyAmount + selfAmount + otherGovAmount + otherUnitAmount;
                placeholder.Add("{{A17.1}}", (total / 10000).ToString());
                placeholder.Add("{{A17.2}}", (total % 10000).ToString());

                // 近兩年補助
                string[] A18_Keys = { "##A18.1##", "##A18.2##" };
                var ReceivedSubsidies = new List<Dictionary<string, string>>();
                foreach (var obj in jobj["ReceivedSubsidies"] as JArray)
                {
                    var dict = new Dictionary<string, string>();
                    dict[A18_Keys[0]] = obj["Name"]?.ToString() ?? "";
                    dict[A18_Keys[1]] = obj["Amount"]?.ToString() ?? "";
                    ReceivedSubsidies.Add(dict);
                }

                // 年月日
                var now = DateTime.Now;
                placeholder.Add("{{Year}}", (now.Year - 1911).ToString());
                placeholder.Add("{{Month}}", now.Month.ToString());
                placeholder.Add("{{Day}}", now.Day.ToString());

                var repeatData = new List<Dictionary<string, string>>();

                File.Copy(templatePath, tempFile, true);

                using (var fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite))
                {
                    var helper = new OpenXmlHelper(fs);
                    helper.GenerateWord(placeholder, repeatData);
                    helper.InsertSubTableRows(A18_Keys, ReceivedSubsidies);
                    helper.CloseAsSave();
                }
            }
        }
        else if (templateType == "2")
        {
            docName = "2-申請計畫書.docx";
            string templatePath = context.Server.MapPath($"~/Template/EDC/{docName}");
            if (!File.Exists(templatePath))
            {
                context.Response.StatusCode = 404;
                context.Response.Write("Template not found.");
                return;
            }
            else
            {
                var educationService = new EducationService();

                // 取得基本資料
                var data = educationService.getApplication(new JObject { ["ID"] = id }, context);
                var jobj = JObject.Parse(JsonConvert.SerializeObject(data));
                // 取得申請補助單位
                var baseService = new BaseService();
                var edcOrgCategory = baseService.getZgsCodes(new JObject { ["CodeGroup"] = "EDCOrgCategory" }, context);
                foreach(var obj in JArray.Parse(JsonConvert.SerializeObject(edcOrgCategory)))
                {
                    if (obj["Code"]?.ToString() == jobj["Project"]["OrgCategory"]?.ToString())
                    {
                        jobj["Project"]["OrgCategory"] = obj["Descname"]?.ToString() ?? "";
                    }
                }
                var placeholder = new Dictionary<string, string>();
                placeholder.Add("{{A1}}", jobj["Project"]["ProjectName"]?.ToString() ?? "");
                placeholder.Add("{{A3}}", jobj["Project"]["OrgCategory"]?.ToString() ?? "");
                // 計畫時間日期
                string start = jobj["Project"]["StartTime"]?.ToString();
                string end = jobj["Project"]["EndTime"]?.ToString();
                string startShort = DateTime.TryParse(start, out var sdt) ? $"{sdt.Year - 1911}/{sdt.Month:D2}/{sdt.Day:D2}" : "";
                string endShort = DateTime.TryParse(end, out var edt) ? $"{edt.Year - 1911}/{edt.Month:D2}/{edt.Day:D2}" : "";
                placeholder.Add("{{A9}}", $"{startShort} ~ {endShort}");
                placeholder.Add("{{A12}}", jobj["Project"]["Quantified"]?.ToString() ?? "");

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
        else if (templateType == "3")
        {
            docName = "3-未違反公職人員利益衝突迴避法切結書及事前揭露表.docx";
            string templatePath = context.Server.MapPath($"~/Template/EDC/{docName}");
            if (!File.Exists(templatePath))
            {
                context.Response.StatusCode = 404;
                context.Response.Write("Template not found.");
                return;
            }
            else
            {
                var educationService = new EducationService();
                // 取得基本資料
                var data = educationService.getApplication(new JObject { ["ID"] = id }, context);
                var json = JsonConvert.SerializeObject(data);
                var jobj = JObject.Parse(json);

                var placeholder = new Dictionary<string, string>();
                placeholder.Add("{{A1}}", jobj["Project"]["ProjectID"]?.ToString() ?? "");
                placeholder.Add("{{A2}}", jobj["Project"]["ProjectName"]?.ToString() ?? "");
                placeholder.Add("{{A4}}", jobj["Project"]["OrgName"]?.ToString() ?? "");
                placeholder.Add("{{A6}}", jobj["Project"]["TaxID"]?.ToString() ?? "");
                placeholder.Add("{{A7}}", jobj["Project"]["Address"]?.ToString() ?? "");
                placeholder.Add("{{A8}}", jobj["Contacts"][0]["Name"]?.ToString() ?? "");
                // 年月日
                var now = DateTime.Now;
                placeholder.Add("{{Year}}", (now.Year - 1911).ToString());
                placeholder.Add("{{Month}}", now.Month.ToString());
                placeholder.Add("{{Day}}", now.Day.ToString());

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
        else
        {
            context.Response.StatusCode = 400;
            context.Response.Write("Invalid template type.");
            return;
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
