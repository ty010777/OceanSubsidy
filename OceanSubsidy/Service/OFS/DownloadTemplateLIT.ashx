<%@ WebHandler Language="C#" Class="DownloadTemplateLIT" %>

using GS.OCA_OceanSubsidy.Operation.OSI.OpenXml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;

public class DownloadTemplateLIT : IHttpHandler, IRequiresSessionState
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
            string templatePath = context.Server.MapPath($"~/Template/LIT/{docName}");
            if (!File.Exists(templatePath))
            {
                context.Response.StatusCode = 404;
                context.Response.Write("Template not found.");
                return;
            }
            else
            {
                var literacyService = new LiteracyService();
                // 取得基本資料
                var data = literacyService.getApplication(new JObject { ["ID"] = id }, context);
                var jobj = JObject.Parse(JsonConvert.SerializeObject(data));
                // 取得其他補助項目
                data = literacyService.getFunding(new JObject { ["ID"] = id }, context);
                var jothers = JObject.Parse(JsonConvert.SerializeObject(data));

                var placeholder = new Dictionary<string, string>();
                placeholder.Add("{{A1}}", jobj["Project"]["ProjectID"]?.ToString() ?? "");
                placeholder.Add("{{A2}}", jobj["Project"]["ProjectName"]?.ToString() ?? "");
                placeholder.Add("{{A4}}", jobj["Project"]["OrgName"]?.ToString() ?? "");
                placeholder.Add("{{A6}}", jobj["Project"]["OrgLeader"]?.ToString() ?? "");
                placeholder.Add("{{A7}}", jobj["Project"]["Address"]?.ToString() ?? "");
                placeholder.Add("{{A8.1}}", jobj["Contacts"][0]["Name"]?.ToString() ?? "");
                placeholder.Add("{{A8.2}}", jobj["Contacts"][0]["JobTitle"]?.ToString() ?? "");
                placeholder.Add("{{A8.3}}", jobj["Contacts"][0]["Phone"]?.ToString() ?? "");
                placeholder.Add("{{A8.4}}", jobj["Contacts"][0]["MobilePhone"]?.ToString() ?? "");
                placeholder.Add("{{A8.5}}", jobj["Contacts"][0]["EMail"]?.ToString() ?? "");
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
                placeholder.Add("{{C0.1}}", (totalAmount / 10000).ToString());
                placeholder.Add("{{C0.2}}", (totalAmount % 10000).ToString());
                // PreviousStudies
                string[] A13_Keys = { "##A13.1##", "##A13.2##", "##A13.3##" };
                var previousStudies = new List<Dictionary<string, string>>();
                foreach (var obj in jobj["PreviousStudies"] as JArray)
                {
                    var dict = new Dictionary<string, string>();
                    dict[A13_Keys[0]] = obj["Title"]?.ToString() ?? "";
                    dict[A13_Keys[1]] = DateTime.TryParse(obj["TheDate"]?.ToString()?? "", out var dt) ? $"{dt.Year - 1911}/{dt.Month:D2}/{dt.Day:D2}" : "";
                    previousStudies.Add(dict);
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
                    helper.InsertSubTableRows(A13_Keys, previousStudies);
                    helper.CloseAsSave();
                }
            }
        }
        else if (templateType == "2")
        {
            docName = "2-計畫書.docx";
            string templatePath = context.Server.MapPath($"~/Template/LIT/{docName}");
            if (!File.Exists(templatePath))
            {
                context.Response.StatusCode = 404;
                context.Response.Write("Template not found.");
                return;
            }
            else
            {
                var literacyService = new LiteracyService();
                // 取得基本資料
                var data = literacyService.getApplication(new JObject { ["ID"] = id }, context);
                var jobj = JObject.Parse(JsonConvert.SerializeObject(data));

                var placeholder = new Dictionary<string, string>();
                placeholder.Add("{{A2}}", jobj["Project"]["ProjectName"]?.ToString() ?? "");
                // 計畫類別
                var baseService = new BaseService();
                var orgCategory = baseService.getZgsCodes(new JObject { ["CodeGroup"] = "LITField" }, context);
                foreach(var obj in JArray.Parse(JsonConvert.SerializeObject(orgCategory)))
                {
                    if (obj["Code"]?.ToString() == jobj["Project"]["Field"]?.ToString())
                    {
                        jobj["Project"]["FieldName"] = obj["Descname"]?.ToString() ?? "";
                    }
                }
                placeholder.Add("{{A3}}", jobj["Project"]["FieldName"]?.ToString() ?? "");
                placeholder.Add("{{A3.1}}", (jobj["Project"]["Field"]?.ToString() == "1") ? "○○縣/市海洋素養教師社群召集人○○○" : "");
                placeholder.Add("{{A4}}", jobj["Project"]["OrgName"]?.ToString() ?? "");
                placeholder.Add("{{A8.1}}", jobj["Contacts"][0]["Name"]?.ToString() ?? "");
                placeholder.Add("{{A10}}", jobj["Project"]["Target"]?.ToString() ?? "");

                // 取得工作進度
                data = literacyService.getWorkSchedule(new JObject { ["ID"] = id }, context);
                jobj = JObject.Parse(JsonConvert.SerializeObject(data));
                // Items
                string[] B2_1_Keys = { "##B2.2-1##", "##B2.5-1##" };
                var items = new List<Dictionary<string, string>>();
                foreach (var obj in jobj["Items"] as JArray)
                {
                    if (obj["Deleted"]?.ToObject<bool>() == true) continue;
                    var dict = new Dictionary<string, string>();
                    dict[B2_1_Keys[0]] = obj["Title"]?.ToString() ?? "";
                    dict[B2_1_Keys[1]] = obj["Content"]?.ToString() ?? "";
                    items.Add(dict);
                }

                // Progress
                string[] B2_2_Keys = { "##B2.2-2##", "##B2.4-2##" };
                var progress = new List<Dictionary<string, string>>();
                foreach (var obj in jobj["Schedules"] as JArray)
                {
                    if (obj["Deleted"]?.ToObject<bool>() == true) continue;
                    var dict = new Dictionary<string, string>();
                    var itemId = obj["ItemID"]?.ToString() ?? "";
                    var item = (jobj["Items"] as JArray).FirstOrDefault(x => x["ID"]?.ToString() == itemId);
                    dict[B2_2_Keys[0]] = item?["Title"]?.ToString() ?? "";
                    dict[B2_2_Keys[1]] = DateTime.TryParse(obj["Deadline"]?.ToString(), out var Deadline) ? $"{Deadline.Year - 1911} 年 {Deadline.Month:D2} 月 {Deadline.Day:D2} 日" : "";
                    progress.Add(dict);
                }

                // 預定時程及進度
                var scheduleArray = jobj["Schedules"] as JArray;

                string alignStyle = "text-align:center;vertical-align:middle;";
                string itemSchedules = $"<table><tr><th style='{alignStyle}'><br>工作項目 \\ 月份<br></th>";
                itemSchedules += string.Concat(Enumerable.Range(1, 12).Select(m => $"<th style='{alignStyle};width:6%'>{m}</th>"));
                itemSchedules += "</tr>";

                int itemIdx = 0;

                foreach (var obj in jobj["Items"] as JArray)
                {
                    if (obj["Deleted"]?.ToObject<bool>() == true) continue;
                    string itemId = obj["ID"]?.ToString() ?? "";
                    // 產生 A, B, C, D...
                    char label = (char)(65 + itemIdx);
                    // 每個 item 的進度編號從 1 開始
                    int i = 1;
                    itemSchedules += $"<tr><td>{label}.{obj["Title"]?.ToString() ?? ""}</td>";

                    for (int month = 1; month <= 12; month++)
                    {
                        // 取得該 item 的起訖月份
                        int beginMonth = obj["Begin"]?.ToObject<int>() ?? 0;
                        int endMonth = obj["End"]?.ToObject<int>() ?? 0;

                        // 判斷是否在範圍內
                        bool inRange = month >= beginMonth && month <= endMonth;

                        // 判斷是否有進度資料
                        bool hasSchedule = scheduleArray.Any(sch => sch["ItemID"]?.ToString() == itemId && DateTime.TryParse(sch["Deadline"]?.ToString(), out var dt) && dt.Month == month);

                        // 設定背景色
                        string bgStyle = inRange ? "background-color:#DEEAF6;" : "";

                        // 加入儲存格
                        itemSchedules += hasSchedule ? $"<td style='{alignStyle}{bgStyle}'>{label}{i++}</td>" : $"<td style='{alignStyle}{bgStyle}'></td>";
                    }
                    itemIdx++;
                    itemSchedules += "</tr>";
                }
                itemSchedules += "</table>";

                // 進度說明
                string schedules = $"<table><tr><th style='{alignStyle}width:20%'>完成百分比</th><th style='{alignStyle}width:20%'>查核點編號</th><th style='{alignStyle}width:15%'>預定完成日期</th><th style='{alignStyle}width:45%'>查核內容</th></tr>";

                char orderChar = 'A';
                string lastItemId = "";
                itemIdx = 1;

                foreach (var sch in scheduleArray)
                {
                    var itemId = sch["ItemID"]?.ToString() ?? "";
                    if (itemId != lastItemId)
                    {
                        orderChar = (char)('A' + (orderChar - 'A' + (lastItemId == "" ? 0 : 1)));
                        itemIdx = 1;
                        lastItemId = itemId;
                    }
                    sch["Order"] = $"{orderChar}{itemIdx}";
                    itemIdx++;
                }

                var groupTypes = new[] { 1, 2 };

                foreach (var type in groupTypes)
                {
                    var group = scheduleArray.Where(obj => (obj["Type"]?.ToObject<int>() ?? 0) == type && obj["Deleted"]?.ToObject<bool>() != true).ToList();

                    if (group.Count == 0) continue;

                    string percent = type == 1 ? "50%" : "100%";
                    bool isFirst = true;

                    foreach (var obj in group)
                    {
                        string deadline = DateTime.TryParse(obj["Deadline"]?.ToString(), out var Deadline) ? $"{Deadline.Year - 1911} 年 {Deadline.Month:D2} 月 {Deadline.Day:D2} 日" : "";
                        string content = obj["Content"]?.ToString() ?? "";
                        string order = obj["Order"]?.ToString() ?? "";

                        if (isFirst)
                        {
                            schedules += $"<tr><td rowspan='{group.Count}' style='{alignStyle}'>{percent}</td><td style='{alignStyle}'>{order}</td><td style='{alignStyle}'>{deadline}</td><td>{content}</td></tr>";
                            isFirst = false;
                        }
                        else
                        {
                            schedules += $"<tr><td style='{alignStyle}'>{order}</td><td style='{alignStyle}'>{deadline}</td><td>{content}</td></tr>";
                        }
                    }
                }
                schedules += "</table>";

                // context.Response.Write(schedules);
                // return;

                // BudgetPlans
                data = literacyService.getFunding(new JObject { ["ID"] = id }, context);
                jobj = JObject.Parse(JsonConvert.SerializeObject(data));
                string[] C5_Keys = { "##C5.0##", "##C5.1##", "##C5.2##", "##C5.3##", "##C5.4##", "##C5.5##" };
                var budgets = new List<Dictionary<string, string>>();
                int k = 1;
                int amtSum = 0;
                int oamtSum = 0;
                foreach (var obj in jobj["BudgetPlans"] as JArray)
                {
                    if (obj["Deleted"]?.ToObject<bool>() == true) continue;
                    var dict = new Dictionary<string, string>();
                    dict[C5_Keys[0]] = (k++).ToString();
                    dict[C5_Keys[1]] = obj["Title"]?.ToString() ?? "";
                    dict[C5_Keys[2]] = obj["Amount"]?.ToString() ?? "";
                    dict[C5_Keys[3]] = obj["OtherAmount"]?.ToString() ?? "";
                    int amt = int.TryParse(obj["Amount"]?.ToString(), out var a) ? a : 0;
                    int oamt = int.TryParse(obj["OtherAmount"]?.ToString(), out var b) ? b : 0;
                    dict[C5_Keys[4]] = (amt + oamt).ToString();
                    dict[C5_Keys[5]] = obj["Description"]?.ToString() ?? "";
                    amtSum += amt;
                    oamtSum += oamt;
                    budgets.Add(dict);
                }
                placeholder.Add("{{C5.2-sum}}", amtSum.ToString());
                placeholder.Add("{{C5.3-sum}}", oamtSum.ToString());
                placeholder.Add("{{C5.4-sum}}", (amtSum + oamtSum).ToString());

                // Benefits
                data = literacyService.getBenefit(new JObject { ["ID"] = id }, context);
                jobj = JObject.Parse(JsonConvert.SerializeObject(data));
                string[] D1_Keys = { "##D1.1##", "##D1.2##", "##D1.3##" };
                var benefit = new List<Dictionary<string, string>>();
                foreach (var obj in jobj["Benefits"] as JArray)
                {
                    if (obj["Deleted"]?.ToObject<bool>() == true) continue;
                    var dict = new Dictionary<string, string>();
                    dict[D1_Keys[0]] = obj["Title"]?.ToString() ?? "";
                    dict[D1_Keys[1]] = obj["Target"]?.ToString() ?? "";
                    dict[D1_Keys[2]] = obj["Description"]?.ToString() ?? "";
                    benefit.Add(dict);
                }
                placeholder.Add("{{D2}}", jobj["Project"]["Benefit"]?.ToString() ?? "");

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
                    helper.InsertSubTableRows(B2_1_Keys, items);
                    helper.InsertSubTableRows(B2_2_Keys, progress);
                    helper.InsertHtmlAsTable("{{B2_Table}}", itemSchedules);
                    helper.InsertHtmlAsTable("{{B3_Table}}", schedules);
                    helper.InsertSubTableRows(C5_Keys, budgets);
                    helper.InsertSubTableRows(D1_Keys, benefit);
                    helper.CloseAsSave();
                }
            }
        }
        else if (templateType == "3")
        {
            docName = "3-執行承諾書.docx";
            string templatePath = context.Server.MapPath($"~/Template/LIT/{docName}");
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
            docName = "4-未違反公職人員利益衝突迴避法切結書及事前揭露表.docx";
            string templatePath = context.Server.MapPath($"~/Template/LIT/{docName}");
            if (!File.Exists(templatePath))
            {
                context.Response.StatusCode = 404;
                context.Response.Write("Template not found.");
                return;
            }
            else
            {
                var literacyService = new LiteracyService();
                // 取得基本資料
                var data = literacyService.getWorkSchedule(new JObject { ["ID"] = id }, context);
                var jobj = JObject.Parse(JsonConvert.SerializeObject(data));

                var placeholder = new Dictionary<string, string>();
                placeholder.Add("{{A1}}", jobj["Project"]["ProjectID"]?.ToString() ?? "");
                placeholder.Add("{{A2}}", jobj["Project"]["ProjectName"]?.ToString() ?? "");
                placeholder.Add("{{A4}}", jobj["Project"]["OrgName"]?.ToString() ?? "");
                placeholder.Add("{{A6}}", jobj["Project"]["TaxID"]?.ToString() ?? "");
                placeholder.Add("{{A7}}", jobj["Project"]["Address"]?.ToString() ?? "");
                // 取得負責人姓名
                data = literacyService.getApplication(new JObject { ["ID"] = id }, context);
                var obj = JObject.Parse(JsonConvert.SerializeObject(data));
                placeholder.Add("{{A8}}", obj["Contacts"][0]["Name"]?.ToString() ?? "");
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
        else if (templateType == "5")
        {
            docName = "5-著作權授權同意書.docx";
            string templatePath = context.Server.MapPath($"~/Template/LIT/{docName}");
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
