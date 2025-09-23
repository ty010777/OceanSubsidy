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
                // 取得文化類基本資料
                var data = cultureService.getWorkSchedule(new JObject { ["ID"] = id }, context);
                var jobj = JObject.Parse(JsonConvert.SerializeObject(data));

                var placeholder = new Dictionary<string, string>();
                placeholder.Add("{{A2}}", jobj["Project"]["ProjectName"]?.ToString() ?? "");
                placeholder.Add("{{A4}}", jobj["Project"]["OrgName"]?.ToString() ?? "");
                placeholder.Add("{{A10}}", jobj["Project"]["Target"]?.ToString() ?? "");

                var goals = jobj["Goals"] as JArray;

                // (六) 重要工作項目(條列式) 3-6-1
                string table3_6_1 = "<table><tr><td>計畫目標</td><td>重要工作項目</td><td>實施步驟</td></tr>";

                if (goals != null)
                {
                    foreach (var goal in goals)
                    {
                        var items = goal["Items"] as JArray;
                        if (items == null || items.Count == 0) continue;

                        int goalRowSpan = 0;
                        foreach (var item in items)
                        {
                            var steps = item["Steps"] as JArray;
                            goalRowSpan += (steps != null && steps.Count > 0) ? steps.Count : 1;
                        }

                        bool firstGoalRow = true;
                        foreach (var item in items)
                        {
                            var steps = item["Steps"] as JArray;
                            int itemRowSpan = (steps != null && steps.Count > 0) ? steps.Count : 1;
                            bool firstItemRow = true;

                            if (steps != null && steps.Count > 0)
                            {
                                foreach (var step in steps)
                                {
                                    table3_6_1 += "<tr>";
                                    if (firstGoalRow)
                                    {
                                        table3_6_1 += $"<td rowspan='{goalRowSpan}'>{goal["Title"]?.ToString() ?? ""}</td>";
                                        firstGoalRow = false;
                                    }
                                    if (firstItemRow)
                                    {
                                        table3_6_1 += $"<td rowspan='{itemRowSpan}'>{item["Title"]?.ToString() ?? ""}</td>";
                                        firstItemRow = false;
                                    }
                                    table3_6_1 += $"<td>{step["Title"]?.ToString() ?? ""}</td>";
                                    table3_6_1 += "</tr>";
                                }
                            }
                            else
                            {
                                // 沒有步驟時
                                table3_6_1 += "<tr>";
                                if (firstGoalRow)
                                {
                                    table3_6_1 += $"<td rowspan='{goalRowSpan}'>{goal["Title"]?.ToString() ?? ""}</td>";
                                    firstGoalRow = false;
                                }
                                table3_6_1 += $"<td rowspan='{itemRowSpan}'>{item["Title"]?.ToString() ?? ""}</td>";
                                table3_6_1 += "<td></td></tr>";
                            }
                        }
                    }
                }
                table3_6_1 += "</table>";

                // (六) 重要工作項目 3-6-2
                string table3_6_2 = "<table><tr><td>重要工作項目</td><td>實施步驟</td></tr>";

                if (goals != null)
                {
                    foreach (var goal in goals)
                    {
                        var items = goal["Items"] as JArray;
                        if (items == null || items.Count == 0) continue;

                        foreach (var item in items)
                        {
                            var steps = item["Steps"] as JArray;
                            int itemRowSpan = (steps != null && steps.Count > 0) ? steps.Count : 1;
                            bool firstItemRow = true;

                            if (steps != null && steps.Count > 0)
                            {
                                foreach (var step in steps)
                                {
                                    table3_6_2 += "<tr>";
                                    if (firstItemRow)
                                    {
                                        table3_6_2 += $"<td rowspan='{itemRowSpan}'>{item["Title"]?.ToString() ?? ""}</td>";
                                        firstItemRow = false;
                                    }
                                    table3_6_2 += $"<td>{step["Title"]?.ToString() ?? ""}</td>";
                                    table3_6_2 += "</tr>";
                                }
                            }
                            else
                            {
                                // 沒有步驟時
                                table3_6_2 += "<tr>";
                                table3_6_2 += $"<td rowspan='{itemRowSpan}'>{item["Title"]?.ToString() ?? ""}</td>";
                                table3_6_2 += "<td></td></tr>";
                            }
                        }
                    }
                }
                table3_6_2 += "</table>";

                // (七) 工作績效指標與工作進度
                string table3_7 = "<table><tr><td>重要工作項目</td><td>績效指標</td><td>工作進度達50%應檢核工作</td><td>工作進度達100%應檢核工作</td></tr>";

                if (goals != null)
                {
                    foreach (var goal in goals)
                    {
                        var items = goal["Items"] as JArray;
                        if (items == null || items.Count == 0) continue;

                        foreach (var item in items)
                        {
                            var steps = item["Steps"] as JArray;
                            var schedules = item["Schedules"] as JArray;
                            // 依 Type 分組
                            var type1 = schedules?.Where(s => (s["Type"]?.ToObject<int>() ?? 0) == 1).ToList() ?? new List<JToken>();
                            var type2 = schedules?.Where(s => (s["Type"]?.ToObject<int>() ?? 0) == 2).ToList() ?? new List<JToken>();
                            int maxCount = Math.Max(type1.Count, type2.Count);

                            bool firstItemRow = true;
                            for (int i = 0; i < maxCount; i++)
                            {
                                table3_7 += "<tr>";
                                if (firstItemRow)
                                {
                                    table3_7 += $"<td rowspan='{maxCount}'>{item["Title"]?.ToString() ?? ""}</td>";
                                    table3_7 += $"<td rowspan='{maxCount}'>{item["Indicator"]?.ToString() ?? ""}</td>";
                                    firstItemRow = false;
                                }
                                // Type=1
                                if (i < type1.Count)
                                {
                                    var sch = type1[i];
                                    var step = steps?.FirstOrDefault(st => st["ID"]?.ToString() == sch["StepID"]?.ToString());
                                    table3_7 += $"<td>{sch["Month"]}月 {step?["Title"]?.ToString() ?? ""}</td>";
                                }
                                else
                                {
                                    table3_7 += "<td></td>";
                                }
                                // Type=2
                                if (i < type2.Count)
                                {
                                    var sch = type2[i];
                                    var step = steps?.FirstOrDefault(st => st["ID"]?.ToString() == sch["StepID"]?.ToString());
                                    table3_7 += $"<td>{sch["Month"]}月 {step?["Title"]?.ToString() ?? ""}</td>";
                                }
                                else
                                {
                                    table3_7 += "<td></td>";
                                }
                                table3_7 += "</tr>";
                            }
                        }
                    }
                }
                table3_7 += "</table>";

                // (八) 實施步驟與期程
                var now = DateTime.Now;

                string bgStyle = "background-color:#DEEAF6;";
                string table3_8 = $"<table><tr><td rowspan='2'>工作 \\ 期程</td><td colspan='12' style='text-align:center;'>{(now.Year - 1911).ToString()} 年度</td></tr>";
                table3_8 += "<tr>";
                for (int m = 1; m <= 12; m++)
                {
                    table3_8 += $"<td style='width:7%'>{m}月</td>";
                }
                table3_8 += "</tr>";

                if (goals != null)
                {
                    foreach (var goal in goals)
                    {
                        var items = goal["Items"] as JArray;
                        if (items == null || items.Count == 0) continue;

                        foreach (var item in items)
                        {
                            // 工作項目標題列
                            table3_8 += $"<tr><td colspan='13'>{item["Title"]?.ToString() ?? ""}</td></tr>";

                            var steps = item["Steps"] as JArray;
                            if (steps == null || steps.Count == 0) continue;

                            foreach (var step in steps)
                            {
                                table3_8 += $"<tr><td>{step["Title"]?.ToString() ?? ""}</td>";
                                int begin = step["Begin"]?.ToObject<int>() ?? 0;
                                int end = step["End"]?.ToObject<int>() ?? 0;
                                for (int m = 1; m <= 12; m++)
                                {
                                    if (m >= begin && m <= end)
                                        table3_8 += $"<td style='{bgStyle}'></td>";
                                    else
                                        table3_8 += "<td></td>";
                                }
                                table3_8 += "</tr>";
                            }
                        }
                    }
                }
                table3_8 += "</table>";

                // 四、經費預算規劃
                data = cultureService.getFunding(new JObject { ["ID"] = id }, context);
                var json = JsonConvert.SerializeObject(data);
                jobj = JObject.Parse(json);

                var itemsDict = new Dictionary<string, string>();
                var itemsArr = jobj["Items"] as JArray;
                if (itemsArr != null)
                {
                    foreach (var item in itemsArr)
                    {
                        var itemId = item["ID"]?.ToString();
                        if (!string.IsNullOrEmpty(itemId) && !itemsDict.ContainsKey(itemId))
                            itemsDict[itemId] = item["Title"]?.ToString() ?? "";
                    }

                }

                var budgetPlans = jobj["BudgetPlans"] as JArray;
                var grouped = budgetPlans?.Where(p => p["Deleted"]?.ToObject<bool>() != true).GroupBy(p => p["ItemID"]?.ToString()).ToList();

                string table4 = "<table border='1' style='border-collapse:collapse;'><tr>"
                    + "<th style='padding:5px;'>重要工作項目</th>"
                    + "<th style='padding:5px;'>預算項目</th>"
                    + "<th style='padding:5px;'>預算金額海洋委員會經費</th>"
                    + "<th style='padding:5px;'>預算金額其他配合經費</th>"
                    + "<th style='padding:5px;'>預算金額小計</th>"
                    + "<th style='padding:5px;'>計算方式及說明</th></tr>";

                if (grouped != null)
                {
                    foreach (var group in grouped)
                    {
                        int rowspan = group.Count();
                        bool first = true;
                        foreach (var plan in group)
                        {
                            table4 += "<tr>";
                            if (first)
                            {
                                string itemTitle = itemsDict.ContainsKey(group.Key) ? itemsDict[group.Key] : "";
                                table4 += $"<td rowspan='{rowspan}' style='padding:5px;'>{itemTitle}</td>";
                                first = false;
                            }
                            table4 += $"<td style='padding:5px;'>{plan["Title"]?.ToString() ?? ""}</td>";
                            table4 += $"<td style='padding:5px;'>{plan["Amount"]?.ToString() ?? ""}</td>";
                            table4 += $"<td style='padding:5px;'>{plan["OtherAmount"]?.ToString() ?? ""}</td>";
                            int amt = int.TryParse(plan["Amount"]?.ToString(), out var a) ? a : 0;
                            int oamt = int.TryParse(plan["OtherAmount"]?.ToString(), out var b) ? b : 0;
                            table4 += $"<td style='padding:5px;'>{(amt + oamt).ToString()}</td>";
                            table4 += $"<td style='padding:5px;'>{plan["Description"]?.ToString() ?? ""}</td>";
                            table4 += "</tr>";
                        }
                    }
                }
                table4 += "</table>";

                // 五、預期成效
                string[] B_Keys = { "##B2##", "##B3##" };
                var b_data = new List<Dictionary<string, string>>();
                foreach (var goal in goals)
                {
                    if (goal["Deleted"]?.ToObject<bool>() == true) continue;
                    var dict = new Dictionary<string, string>();
                    dict[B_Keys[0]] = goal["Title"]?.ToString() ?? "";
                    dict[B_Keys[1]] = goal["Content"]?.ToString() ?? "";
                    b_data.Add(dict);
                }

                // 六、提案單位近三年執行相關計畫
                data = cultureService.getRelatedProject(new JObject { ["ID"] = id }, context);
                jobj = JObject.Parse(JsonConvert.SerializeObject(data));

                string[] D_Keys = { "##D1##", "##D2##", "##D3##", "##D4##", "##D5##", "##D6##" };
                var d_data = new List<Dictionary<string, string>>();
                foreach (var project in jobj["Projects"] as JArray)
                {
                    if (project["Deleted"]?.ToObject<bool>() == true) continue;
                    var dict = new Dictionary<string, string>();
                    dict[D_Keys[0]] = project["Title"]?.ToString() ?? "";
                    dict[D_Keys[1]] = project["Year"]?.ToString() ?? "";
                    dict[D_Keys[2]] = project["OrgName"]?.ToString() ?? "";
                    dict[D_Keys[3]] = project["Amount"]?.ToString() ?? "";
                    dict[D_Keys[4]] = project["Description"]?.ToString() ?? "";
                    dict[D_Keys[5]] = project["Benefit"]?.ToString() ?? "";
                    d_data.Add(dict);
                }





                // TODO: E1 插入多筆檔案目前尚未實作
                placeholder.Add("{{E1}}", "");





                // 年月日
                placeholder.Add("{{Year}}", (now.Year - 1911).ToString());
                placeholder.Add("{{Month}}", now.Month.ToString());
                placeholder.Add("{{Day}}", now.Day.ToString());

                var repeatData = new List<Dictionary<string, string>>();

                File.Copy(templatePath, tempFile, true);

                using (var fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite))
                {
                    var helper = new OpenXmlHelper(fs);
                    helper.GenerateWord(placeholder, repeatData);
                    helper.InsertHtmlAsTable("{{Table3-6-1}}", table3_6_1);
                    helper.InsertHtmlAsTable("{{Table3-6-2}}", table3_6_2);
                    helper.InsertHtmlAsTable("{{Table3-7}}", table3_7);
                    helper.InsertHtmlAsTable("{{Table3-8}}", table3_8);
                    helper.InsertHtmlAsTable("{{Table4}}", table4);
                    helper.InsertSubTableRows(B_Keys, b_data);
                    helper.InsertSubTableRows(D_Keys, d_data);
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
