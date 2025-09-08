using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Model.OFS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class SystemService : BaseService
{
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
            content = new GrantTypeContent { TypeID = data.TypeID, IsValid = false };

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

        return new { ID = data.TypeID };
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
}
