<%@ WebHandler Language="C#" Class="DownloadTemplateCUL" %>

using GS.OCA_OceanSubsidy.Operation.OSI.OpenXml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

public class DownloadTemplateCUL : IHttpHandler
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
                var cultureService = new CultureService();

                // 取得基本資料
                var data = cultureService.getApplication(new JObject { ["ID"] = id }, context);
                var json = JsonConvert.SerializeObject(data);
                var jobj = JObject.Parse(json);

                // 取得其他補助項目
                data = cultureService.getFunding(new JObject { ["ID"] = id }, context);
                json = JsonConvert.SerializeObject(data);
                var jothers = JObject.Parse(json);

                var placeholder = new Dictionary<string, string>();
                placeholder.Add("{{A1}}", jobj["Project"]["ProjectID"]?.ToString() ?? "");
                placeholder.Add("{{A2}}", jobj["Project"]["ProjectName"]?.ToString() ?? "");

                // TODO: A3 有三個類別，每個類別還有子項目
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
                // 期程
                string start = jobj["Project"]["StartTime"]?.ToString();
                string end = jobj["Project"]["EndTime"]?.ToString();
                string startShort = DateTime.TryParse(start, out var sdt) ? $"{sdt.Year - 1911}/{sdt.Month:D2}/{sdt.Day:D2}" : "";
                string endShort = DateTime.TryParse(end, out var edt) ? $"{edt.Year - 1911}/{edt.Month:D2}/{edt.Day:D2}" : "";
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
                // 其他款
                var otherAmountStr = jobj["Project"]["OtherAmount"]?.ToString() ?? "0";
                int otherAmount = int.TryParse(otherAmountStr, out var a3) ? a3 : 0;
                placeholder.Add("{{C3.1}}", (otherAmount / 10000).ToString());
                placeholder.Add("{{C3.2}}", (otherAmount % 10000).ToString());
                // 其他補助項目
                string[] C4_Keys = { "##C4.1##", "##C4.2##" };
                var otherSubsidies = new List<Dictionary<string, string>>();
                foreach (var obj in jothers["OtherSubsidies"] as JArray)
                {
                    var dict = new Dictionary<string, string>();
                    dict[C4_Keys[0]] = obj["Unit"]?.ToString() ?? "";
                    dict[C4_Keys[1]] = obj["Amount"]?.ToString() ?? "";
                    otherSubsidies.Add(dict);
                }
                // 總經費
                int totalAmount = applyAmount + selfAmount + otherAmount;
                placeholder.Add("{{C5.1}}", (totalAmount / 10000).ToString());
                placeholder.Add("{{C5.2}}", (totalAmount % 10000).ToString());
                // ReceivedSubsidies
                string[] A13_Keys = { "##A13.1##", "##A13.2##", "##A13.3##" };
                var receivedSubsidies = new List<Dictionary<string, string>>();
                foreach (var obj in jobj["ReceivedSubsidies"] as JArray)
                {
                    var dict = new Dictionary<string, string>();
                    dict[A13_Keys[0]] = obj["Name"]?.ToString() ?? "";
                    dict[A13_Keys[1]] = obj["Unit"]?.ToString() ?? "";
                    dict[A13_Keys[2]] = obj["Amount"]?.ToString() ?? "";
                    receivedSubsidies.Add(dict);
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
                    helper.InsertSubTableRows(C4_Keys, otherSubsidies);
                    helper.InsertSubTableRows(A13_Keys, receivedSubsidies);
                    helper.CloseAsSave();
                }
            }
        }
        else if (templateType == "2")
        {
            docName = "2.計畫書.docx";
            string templatePath = context.Server.MapPath($"~/Template/CUL/{docName}");
            if (!File.Exists(templatePath))
            {
                context.Response.StatusCode = 404;
                context.Response.Write("Template not found.");
                return;
            }
            else
            {
                var cultureService = new CultureService();
                // 取得基本資料
                var data = cultureService.getWorkSchedule(new JObject { ["ID"] = id }, context);
                var jobj = JObject.Parse(JsonConvert.SerializeObject(data));

                var placeholder = new Dictionary<string, string>();
                placeholder.Add("{{A2}}", jobj["Project"]["ProjectName"]?.ToString() ?? "");
                placeholder.Add("{{A4}}", jobj["Project"]["OrgName"]?.ToString() ?? "");
                placeholder.Add("{{A10}}", jobj["Project"]["Target"]?.ToString() ?? "");










                // TODO: 尚未實作文化類的 2.計畫書.docx 的 7 個複雜的 Table










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
        else if (templateType == "3")
        {
            docName = "3.執行承諾書.docx";
            string templatePath = context.Server.MapPath($"~/Template/CUL/{docName}");
            if (!File.Exists(templatePath))
            {
                context.Response.StatusCode = 404;
                context.Response.Write("Template not found.");
                return;
            }
            else
            {
                // 年月日
                var now = DateTime.Now;
                var placeholder = new Dictionary<string, string>();
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
        else if (templateType == "4")
        {
            docName = "4.未違反公職人員利益衝突迴避法切結書及事前揭露表.docx";
            string templatePath = context.Server.MapPath($"~/Template/CUL/{docName}");
            if (!File.Exists(templatePath))
            {
                context.Response.StatusCode = 404;
                context.Response.Write("Template not found.");
                return;
            }
            else
            {
                var cultureService = new CultureService();
                // 取得基本資料
                var data = cultureService.getApplication(new JObject { ["ID"] = id }, context);
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
