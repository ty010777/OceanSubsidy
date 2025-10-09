using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Model.OFS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;

public class SystemService : BaseService
{
    public object dashboard(JObject param, HttpContext context)
    {
        return new
        {
            GrantTypes = OFSGrantTypeHelper.query(true),
            Settings = OFSGrantTargetSettingHelper.query(),
            ApplyList = ReportHelper.queryApplyListByUser(CurrentUser.Account),
            StatList = ReportHelper.queryApplyStat()
        };
    }

    public object deleteNews(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        OFSNewsHelper.delete(id);

        foreach (var item in OFSNewsFileHelper.query(id))
        {
            OFSNewsFileHelper.delete(item.ID);
        }

        foreach (var item in OFSNewsImageHelper.query(id))
        {
            OFSNewsImageHelper.delete(item.ID);
        }

        foreach (var item in OFSNewsVideoHelper.query(id))
        {
            OFSNewsVideoHelper.delete(item.ID);
        }

        return new {};
    }

    public object downloadRecusedList(JObject param, HttpContext context)
    {
        var headers = new List<string>() { "年度", "計畫名稱", "計畫申請單位", "委員姓名", "任職單位", "職稱", "應迴避之具體理由及事證" };
        var rows = new List<List<string>>();

        var year = int.Parse(param["Year"].ToString());
        var keyword = param["Keyword"].ToString();
        var name = param["Name"].ToString();
        var org = param["Org"].ToString();

        foreach (var row in OFS_SciRecusedList.queryRecusedList(year, keyword, name, org))
        {
            rows.Add(new List<string>() { row.Year.ToString(), row.ProjectNameTw, row.OrgName, row.RecusedName, row.EmploymentUnit, row.JobTitle, row.RecusedReason });
        }

        var content = NPOIHelper.CreateExcel("迴避審查委員名單", headers, rows).ToArray();

        //--

        var date = DateTime.Now.ToString("yyMMdd");
        var folder = Path.Combine(Path.GetFullPath(Path.Combine(context.Server.MapPath("~"), "..")), "UploadFiles", "files", date);

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        var filename = Path.GetRandomFileName() + ".xlsx";

        File.WriteAllBytes(Path.Combine(folder, filename), content);

        OFSBaseFileHelper.insert(new BaseFile
        {
            Name = filename,
            Path = Path.Combine(date, filename),
            Size = content.Length,
            Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        });

        return new { filename = Path.Combine(date, filename) };
    }

    public object getApplyList(JObject param, HttpContext context)
    {
        return new
        {
            List = ReportHelper.queryApplyList()
        };
    }

    public object getApprovedList(JObject param, HttpContext context)
    {
        return new
        {
            List = ReportHelper.queryApplyList(1)
        };
    }

    public object getEmptyNews(JObject param, HttpContext context)
    {
        return new
        {
            UserName = CurrentUser.UserName,
            UserOrg = CurrentUser.UnitName
        };
    }

    public object getGrantTargetSettings(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());
        var data = OFSGrantTypeHelper.get(id);

        return new
        {
            List = OFSGrantTargetSettingHelper.query(data.TypeCode)
        };
    }

    public object getGrantType(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        return new
        {
            GrantType = OFSGrantTypeHelper.get(id)
        };
    }

    public object getGrantTypeContent(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());
        var data = OFSGrantTypeHelper.get(id);
        var content = OFSGrantTypeContentHelper.get(data.TypeID);

        if (content == null)
        {
            content = new GrantTypeContent { TypeID = data.TypeID, Status = 0 };

            OFSGrantTypeContentHelper.insert(content);
        }

        return new
        {
            GrantType = data,
            Content = content,
            Procedures = OFSGrantTypeProcedureHelper.query(id),
            Links = OFSGrantTypeOnlineLinkHelper.query(id)
        };
    }

    public object getGrantTypeList(JObject param, HttpContext context)
    {
        return new
        {
            List = OFSGrantTypeHelper.query()
        };
    }

    public object getInprogressList(JObject param, HttpContext context)
    {
        return new
        {
            List = ReportHelper.queryApplyList(2)
        };
    }

    public object getNews(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        return new
        {
            News = OFSNewsHelper.get(id),
            Files = OFSNewsFileHelper.query(id),
            Images = OFSNewsImageHelper.query(id),
            Videos = OFSNewsVideoHelper.query(id)
        };
    }

    public object getNewsList(JObject param, HttpContext context)
    {
        return new
        {
            List = OFSNewsHelper.query()
        };
    }

    public object getPublishedNewsList(JObject param, HttpContext context)
    {
        return new
        {
            List = OFSNewsHelper.query(true)
        };
    }

    public object getRecusedList(JObject param, HttpContext context)
    {
        var year = int.Parse(param["Year"].ToString());
        var keyword = param["Keyword"].ToString();
        var name = param["Name"].ToString();
        var org = param["Org"].ToString();

        return new
        {
            List = OFS_SciRecusedList.queryRecusedList(year, keyword, name, org)
        };
    }

    public object getReviewCommitteeList(JObject param, HttpContext context)
    {
        var id = param["ID"].ToString();

        return new
        {
            List = OFSReviewCommitteeHelper.query(id)
        };
    }

    public object getReviewGroups(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());
        var data = OFSGrantTypeHelper.get(id);

        switch (data.TypeCode)
        {
            case "CUL":
                return new { List = SysZgsCodeHelper.getCulReviewGroups() };
            case "SCI":
                return new { List = SysZgsCodeHelper.getSciReviewGroups() };
            default:
                return new {};
        }
    }

    public object saveGrantTargetSettings(JObject param, HttpContext context)
    {
        var settings = param["List"].ToObject<List<GrantTargetSetting>>();

        foreach (var item in settings)
        {
            OFSGrantTargetSettingHelper.updateLimit(item.ID, item.MatchingFund, item.GrantLimit);
        }

        return new {};
    }

    public object saveGrantType(JObject param, HttpContext context)
    {
        var data = param["GrantType"].ToObject<GrantType>();

        OFSGrantTypeHelper.update(data);

        return new {};
    }

    public object saveGrantTypeContent(JObject param, HttpContext context)
    {
        var data = param["GrantType"].ToObject<GrantType>();

        if (data.TypeID == 0)
        {
            OFSGrantTypeHelper.insert(data);
        }
        else
        {
            OFSGrantTypeHelper.updateContent(data);
        }

        var content = param["Content"].ToObject<GrantTypeContent>();

        content.TypeID = data.TypeID;

        OFSGrantTypeContentHelper.update(content);

        var procedures = param["Procedures"].ToObject<List<GrantTypeProcedure>>();

        foreach (var item in procedures)
        {
            if (item.Deleted)
            {
                OFSGrantTypeProcedureHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.TypeID = data.TypeID;

                OFSGrantTypeProcedureHelper.insert(item);
            }
            else
            {
                OFSGrantTypeProcedureHelper.update(item);
            }
        }

        var links = param["Links"].ToObject<List<GrantTypeOnlineLink>>();

        foreach (var item in links)
        {
            if (item.Deleted)
            {
                OFSGrantTypeOnlineLinkHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.TypeID = data.TypeID;

                OFSGrantTypeOnlineLinkHelper.insert(item);
            }
            else
            {
                OFSGrantTypeOnlineLinkHelper.update(item);
            }
        }

        if (bool.Parse(param["Submit"].ToString()))
        {
            return syncGrantTypeContent(data.TypeID);
        }

        return new { ID = data.TypeID, Success = true };
    }

    public object saveNews(JObject param, HttpContext context)
    {
        var news = param["News"].ToObject<News>();

        if (news.ID == 0)
        {
            news.UserName = CurrentUser.UserName;
            news.UserOrg = CurrentUser.UnitName;

            OFSNewsHelper.insert(news);
        }
        else
        {
            OFSNewsHelper.update(news);
        }

        var files = param["Files"].ToObject<List<NewsFile>>();

        foreach (var item in files)
        {
            if (item.ID == 0)
            {
                item.NewsID = news.ID;

                OFSNewsFileHelper.insert(item);
            }
            else if (item.Deleted)
            {
                OFSNewsFileHelper.delete(item.ID);
            }
        }

        var images = param["Images"].ToObject<List<NewsImage>>();

        foreach (var item in images)
        {
            if (item.ID == 0)
            {
                item.NewsID = news.ID;

                OFSNewsImageHelper.insert(item);
            }
            else if (item.Deleted)
            {
                OFSNewsImageHelper.delete(item.ID);
            }
        }

        var videos = param["Videos"].ToObject<List<NewsVideo>>();

        foreach (var item in videos)
        {
            if (item.Deleted)
            {
                OFSNewsVideoHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.NewsID = news.ID;

                OFSNewsVideoHelper.insert(item);
            }
            else
            {
                OFSNewsVideoHelper.update(item);
            }
        }

        return new {};
    }

    public object saveReviewCommitteeList(JObject param, HttpContext context)
    {
        var id = param["ID"].ToString();
        var list = param["List"].ToObject<List<ReviewCommittee>>();

        foreach (var item in list)
        {
            if (item.Deleted)
            {
                OFSReviewCommitteeHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.SubjectTypeID = id;

                OFSReviewCommitteeHelper.insert(item);
            }
            else
            {
                OFSReviewCommitteeHelper.update(item);
            }
        }

        return new {};
    }

    private object syncGrantTypeContent(int id)
    {
        var data = OFSGrantTypeHelper.get(id);
        var content = OFSGrantTypeContentHelper.get(id);
        var url = ConfigurationManager.AppSettings["Host"] + ConfigurationManager.AppSettings["AppRootPath"];

        switch (data.TypeCode)
        {
            case "SCI":
                url = $"{url}/OFS/SCI/SciApplication.aspx?GrantTypeID=SCI";
                break;
            case "CLB":
                url = $"{url}/OFS/CLB/ClbApplication.aspx?GrantTypeID=CLB";
                break;
            default:
                url = $"{url}/OFS/{data.TypeCode}/Application.aspx";
                break;
        }

        var identifier = ConfigurationManager.AppSettings["EGovIdentifier"] + "-" + id.ToString().PadLeft(6, '0');

        var payload = JsonConvert.SerializeObject(new List<object>() {
            new
            {
                oid = ConfigurationManager.AppSettings["EGovOid"],
                identifier = identifier,
                categorytheme = ConfigurationManager.AppSettings["EGovOid"],
                categorycake = ConfigurationManager.AppSettings["EGovOid"],
                categoryservice = ConfigurationManager.AppSettings["EGovOid"],
                keywords = content.Keywords,
                notificationemails = ConfigurationManager.AppSettings["EGovOid"],
                language = 0,
                title = data.FullName,
                statusreason = content.Status == 2 ? content.StatusReason : "",
                // replacecontent
                function = "1",
                servicecontent1 = content.ServiceContent,
                criteria1 = content.Criteria,
                procedure1 = string.Join("\n", OFSGrantTypeProcedureHelper.query(id).Select(d => d.Content).ToList()),
                documentary1 = content.Documentary,
                mydataresourceid1 = false,
                // workingdays1 = content.WorkingDays.ToString(),
                contactperson1 = $"{content.ContactPerson} ({content.ContactTel})",
                remarks1 = content.Remark,
                reference1 = OFSGrantTypeOnlineLinkHelper.query(id).Select(d => new { link = d.URL, linktitle = d.Title }).ToList(),
                onlinelink = url
            }
        });

        var api = ConfigurationManager.AppSettings["EGovAPI"];
        var method = content.Status == 2 ? HttpMethod.Delete : (OFSGrantTypeContentLogHelper.count(id) > 0 ? HttpMethod.Put : HttpMethod.Post);
        var token = ConfigurationManager.AppSettings["EGovToken"];

        if (method == HttpMethod.Delete)
        {
            api = $"{api}{identifier}/";
        }

        if (!string.IsNullOrEmpty(api) && !string.IsNullOrEmpty(token))
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(method, api);

                request.Headers.Add("Authorization", $"Bearer {token}");
                request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = client.SendAsync(request).GetAwaiter().GetResult();

                response.EnsureSuccessStatusCode();

                var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                JObject res = JObject.Parse(result);

                if (res != null && bool.Parse(res["Success"].ToString()))
                {
                    OFSGrantTypeContentLogHelper.insert(new GrantTypeContentLog
                    {
                        TypeID = id,
                        URL = api,
                        Method = method.ToString(),
                        Content = payload,
                        Result = result
                    });

                    res = (JObject) res["ResultData"];

                    return new { ID = id, Success = bool.Parse(res["Success"].ToString()), Message = res["Message"].ToString() };
                }
            }
        }

        return new { ID = id, Success = false };
    }
}
