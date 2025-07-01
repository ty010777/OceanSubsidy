<%@ WebHandler Language="C#" Class="OSI_GetPccAwardData" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using System.IO;
using System.Net;
using System.Text;
using System.Data;
using GS.Data.Sql;
using System.Runtime.Serialization;
using GS.Data;
using GS.Extension;
using GS.App;
using System.IO.Compression;
using GS.OCA_OceanSubsidy.Entity;
using System.Security.Authentication;

public class OSI_GetPccAwardData : IHttpHandler
{
    private string BaseUrl = "https://web.pcc.gov.tw";
    private Encoding UrlEncoding = Encoding.UTF8;

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        string isDo = context.Request.QueryString["isDo"];

        if (isDo == "y")
        {
            // 執行撈取資料
            GetPccData(context);
        }
        else
        {
            context.Response.ContentType = "application/json";
            context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, message = "Please set isDo=y to execute" }));
        }
    }

    protected void GetPccData(HttpContext context)
    {
        var today = DateTime.Today;

        //var url = new Uri("https://web.pcc.gov.tw/prkms/tender/common/agent/readTenderAgent");
        var url = new Uri("");

        List<KeyValuePair<string, string>> WebParams = new List<KeyValuePair<string, string>>();
        WebParams.Add(new KeyValuePair<string, string>("pageSize", "3000"));
        WebParams.Add(new KeyValuePair<string, string>("firstSearch", "true"));
        WebParams.Add(new KeyValuePair<string, string>("isQuery", ""));
        WebParams.Add(new KeyValuePair<string, string>("isBinding", "N"));
        WebParams.Add(new KeyValuePair<string, string>("isLogIn", "N"));
        WebParams.Add(new KeyValuePair<string, string>("orgName", ""));
        WebParams.Add(new KeyValuePair<string, string>("orgId", ""));
        WebParams.Add(new KeyValuePair<string, string>("tenderName", ""));
        WebParams.Add(new KeyValuePair<string, string>("tenderId", ""));
        WebParams.Add(new KeyValuePair<string, string>("tenderStatus", "TENDER_STATUS_1"));
        WebParams.Add(new KeyValuePair<string, string>("tenderWay", "TENDER_WAY_ALL_DECLARATION"));
        WebParams.Add(new KeyValuePair<string, string>("awardAnnounceStartDate", DateTime.Today.ToString("yyyy/MM/dd")));
        WebParams.Add(new KeyValuePair<string, string>("awardAnnounceEndDate", DateTime.Today.ToString("yyyy/MM/dd")));
        WebParams.Add(new KeyValuePair<string, string>("radProctrgCate", ""));
        WebParams.Add(new KeyValuePair<string, string>("tenderRange", "TENDER_RANGE_ALL"));
        WebParams.Add(new KeyValuePair<string, string>("minBudget", ""));
        WebParams.Add(new KeyValuePair<string, string>("maxBudget", ""));
        WebParams.Add(new KeyValuePair<string, string>("item", ""));
        WebParams.Add(new KeyValuePair<string, string>("gottenVendorName", ""));
        WebParams.Add(new KeyValuePair<string, string>("gottenVendorId", ""));
        WebParams.Add(new KeyValuePair<string, string>("submitVendorName", ""));
        WebParams.Add(new KeyValuePair<string, string>("submitVendorId", ""));
        WebParams.Add(new KeyValuePair<string, string>("execLocation", ""));
        WebParams.Add(new KeyValuePair<string, string>("priorityCate", ""));
        WebParams.Add(new KeyValuePair<string, string>("radReConstruct", ""));
        WebParams.Add(new KeyValuePair<string, string>("policyAdvocacy", ""));
        WebParams.Add(new KeyValuePair<string, string>("isCpp", ""));

        var QueryParams = GetParameter(WebParams, UrlEncoding);
        var responseData = RequestAction(new RequestOptions() { Uri = url, Method = "POST", XHRParams = QueryParams });

        // 直接顯示 HTML 內容到畫面
        //context.Response.ContentType = "text/html; charset=utf-8";
        //context.Response.Write(responseData);
        //return;

        HtmlDocument htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(responseData);

        List<string> oldDataUrl = OSIPccAwardDataHelper.QueryAllUrl();

        // 產出結果表格
        HtmlNode tblSearchResult = htmlDoc.DocumentNode.SelectNodes("//table").Where(d => d.Id == "atm").FirstOrDefault();
        if (tblSearchResult != null)
        {
            var data = new List<OSI_PccAwardData>();
            // 列出所有連結
            foreach (HtmlNode rowNode in tblSearchResult.SelectNodes(".//tbody//tr"))
            {
                HtmlNodeCollection tdNodes = rowNode.SelectNodes("td");
                if (tdNodes != null && tdNodes.Count >= 9)
                {
                    var meta = new OSI_PccAwardData();
                    meta.NewRecord();
                    meta.OrgName = tdNodes[1].InnerText.Trim();

                    // 解析標案案號和名稱
                    var tenderInfo = tdNodes[2].InnerHtml.Split(new string[] { "<br>" }, StringSplitOptions.None);
                    if (tenderInfo.Length > 0)
                        meta.AwardNo = System.Text.RegularExpressions.Regex.Replace(tenderInfo[0], "<.*?>", "").Trim();

                    // 取得標案名稱和連結
                    var nameLink = tdNodes[2].SelectSingleNode(".//a");
                    if (nameLink != null)
                    {
                        // 優先從 title 屬性取得標案名稱
                        string awardName = nameLink.Attributes["title"]?.Value;
                        if (string.IsNullOrEmpty(awardName))
                        {
                            // 嘗試從 span 標籤取得內容
                            var spanNode = nameLink.SelectSingleNode(".//span");
                            if (spanNode != null)
                            {
                                awardName = spanNode.InnerText.Trim();
                            }
                            else
                            {
                                // 最後使用 InnerText，但需要過濾掉可能的 JavaScript 程式碼
                                awardName = nameLink.InnerText.Trim();
                                // 移除可能的 JavaScript 程式碼（以 var 開頭的行）
                                awardName = System.Text.RegularExpressions.Regex.Replace(awardName, @"var\s+.*?;.*", "", System.Text.RegularExpressions.RegexOptions.Singleline).Trim();
                            }
                        }
                        meta.AwardName = awardName;
                        meta.AwardNameUrl = nameLink.Attributes["href"]?.Value;
                        if (!string.IsNullOrEmpty(meta.AwardNameUrl) && !meta.AwardNameUrl.StartsWith("http"))
                            meta.AwardNameUrl = BaseUrl + meta.AwardNameUrl;
                    }

                    meta.AwardWay = tdNodes[3].InnerText.Trim();
                    meta.ProctrgCate = tdNodes[4].InnerText.Trim();

                    // 轉換日期格式從民國年到西元年
                    string rocDate = tdNodes[5].InnerText.Trim();
                    if (!string.IsNullOrEmpty(rocDate))
                    {
                        try
                        {
                            meta.AwardDate = GS.App.DateTimeHelper.ParseMinguoDate(rocDate);
                        }
                        catch
                        {
                            // 日期解析失敗時保持為 null
                        }
                    }

                    meta.AwardPrice = tdNodes[6].InnerText.Trim();

                    // 解析決標公告編號和連結
                    var awardNo = tdNodes[7].SelectSingleNode(".//a");
                    if (awardNo != null)
                    {
                        meta.AwardNoticeNo = awardNo.InnerText.Trim();
                    }

                    // 檢視連結
                    var awardLink = tdNodes[9].SelectSingleNode(".//a");
                    if (awardLink != null)
                    {
                        meta.Url = awardLink.Attributes["href"]?.Value;
                        if (!string.IsNullOrEmpty(meta.Url) && !meta.Url.StartsWith("http"))
                            meta.Url = BaseUrl + meta.Url;
                    }

                    meta.QueryParams = QueryParams;
                    if (!oldDataUrl.Contains(meta.Url))
                        data.Add(meta);
                }
            }
            bool success = OSIPccAwardDataHelper.InsertList(data);

            // 回傳結果
            context.Response.ContentType = "application/json";
            context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new { success = true, message = success ? "" : "No save data", count = data.Count, data = data }));
        }
        else
        {
            context.Response.ContentType = "application/json";
            context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, message = "No data found" }));
        }
    }

    /// <summary>
    /// 發送HTTP請求
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    private string RequestAction(RequestOptions options)
    {
        string result = string.Empty;

        // 設定 TLS 1.2 以解決 SSL/TLS 錯誤
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

        var request = (HttpWebRequest)WebRequest.Create(options.Uri);
        request.Accept = options.Accept;
        // 在使用curl做POST的時候, 當要POST的數據大於1024字節的時候, curl並不會直接就發起POST請求, 而是會分爲倆步,
        // 發送一個請求, 包含一個Expect: 100 -continue, 詢問Server使用願意接受數據
        // 接收到Server返回的100 - continue應答以後, 才把數據POST給Server
        // 並不是所有的Server都會正確應答100 -continue, 比如lighttpd, 就會返回417 "Expectation Failed", 則會造成邏輯出錯.
        request.ServicePoint.Expect100Continue = false;
        request.ServicePoint.UseNagleAlgorithm = false; // 禁止Nagle算法加快載入速度
        if (!string.IsNullOrEmpty(options.XHRParams)) { request.AllowWriteStreamBuffering = true; } else { request.AllowWriteStreamBuffering = false; }
        ; // 禁止緩衝加快載入速度
        request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate"); // 定義gzip壓縮頁面支持
        request.ContentType = options.ContentType; // 定義文檔類型及編碼
        request.AllowAutoRedirect = options.AllowAutoRedirect; // 禁止自動跳轉
        request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.181 Safari/537.36"; // 設置User-Agent，僞裝成Google Chrome瀏覽器
        request.Timeout = options.Timeout; // 定義請求超時時間爲5秒
        request.KeepAlive = options.KeepAlive; // 啓用長連接
        if (!string.IsNullOrEmpty(options.Referer)) request.Referer = options.Referer; // 返回上一級歷史鏈接
        request.Method = options.Method; // 定義請求方式
        if (!string.IsNullOrEmpty(options.RequestCookies)) request.Headers[HttpRequestHeader.Cookie] = options.RequestCookies;
        request.ServicePoint.ConnectionLimit = options.ConnectionLimit; // 定義最大連接數
        if (options.WebHeader != null && options.WebHeader.Count > 0) request.Headers.Add(options.WebHeader); // 添加頭部信息

        if (!string.IsNullOrEmpty(options.XHRParams)) // 如果是POST請求，加入POST數據
        {
            byte[] buffer = Encoding.UTF8.GetBytes(options.XHRParams);
            if (buffer != null)
            {
                request.ContentLength = buffer.Length;
                request.GetRequestStream().Write(buffer, 0, buffer.Length);
            }
        }
        using (var response = (HttpWebResponse)request.GetResponse())
        {
            if (response.ContentEncoding.ToLower().Contains("gzip")) // 解壓
            {
                using (GZipStream stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }
            else if (response.ContentEncoding.ToLower().Contains("deflate")) // 解壓
            {
                using (DeflateStream stream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }
            else
            {
                using (Stream stream = response.GetResponseStream()) // 原始
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }
        }
        request.Abort();
        return result;
    }

    protected string GetParameter(List<KeyValuePair<string, string>> ParamColl, System.Text.Encoding e)
    {
        string ReturnValue = "";

        if (ParamColl.Count > 0)
        {
            foreach (KeyValuePair<string, string> param in ParamColl)
            {
                ReturnValue += (ReturnValue != "" ? "&" : "") + param.Key + "=" + HttpUtility.UrlEncode(param.Value, e);
            }
        }

        return ReturnValue;
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}

/// <summary>
/// Request參數
/// </summary>
public class RequestOptions
{
    /// <summary>
    /// 請求方式，GET或POST
    /// </summary>
    public string Method { get; set; }
    /// <summary>
    /// URL
    /// </summary>
    public Uri Uri { get; set; }
    /// <summary>
    /// 上一級歷史記錄鏈接
    /// </summary>
    public string Referer { get; set; }
    /// <summary>
    /// 超時時間（毫秒）
    /// </summary>
    public int Timeout = 15000;
    /// <summary>
    /// 啓用長連接
    /// </summary>
    public bool KeepAlive = true;
    /// <summary>
    /// 禁止自動跳轉
    /// </summary>
    public bool AllowAutoRedirect = false;
    /// <summary>
    /// 定義最大連接數
    /// </summary>
    public int ConnectionLimit = int.MaxValue;
    /// <summary>
    /// 請求次數
    /// </summary>
    public int RequestNum = 3;
    /// <summary>
    /// 可通過文件上傳提交的文件類型
    /// </summary>
    public string Accept = "*/*";
    /// <summary>
    /// 內容類型
    /// </summary>
    public string ContentType = "application/x-www-form-urlencoded";
    /// <summary>
    /// 實例化頭部信息
    /// </summary>
    private WebHeaderCollection header = new WebHeaderCollection();
    /// <summary>
    /// 頭部信息
    /// </summary>
    public WebHeaderCollection WebHeader
    {
        get { return header; }
        set { header = value; }
    }
    /// <summary>
    /// 定義請求Cookie字符串
    /// </summary>
    public string RequestCookies { get; set; }
    /// <summary>
    /// 異步參數數據
    /// </summary>
    public string XHRParams { get; set; }
}
