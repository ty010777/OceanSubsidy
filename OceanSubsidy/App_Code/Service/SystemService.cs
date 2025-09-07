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

    public object getEmptyNews(JObject param, HttpContext context)
    {
        return new
        {
            UserName = CurrentUser.UserName,
            UserOrg = CurrentUser.UnitName
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
