<%@ WebHandler Language="C#" Class="OSI_GetNSTCData" %>

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

public class OSI_GetNSTCData : IHttpHandler
{
    private string BaseUrl = "https://wsts.nstc.gov.tw";
    private Encoding UrlEncoding = Encoding.UTF8;
    private string sessionCookies = "";

    // 分頁處理相關欄位
    private string currentViewState = "";
    private string currentViewStateGenerator = "";
    private string currentEventValidation = "";
    private Uri currentUrl;
    private List<string> existingProjectNames;

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        string isDo = context.Request.QueryString["isDo"];

        if (isDo == "y")
        {
            // 執行撈取資料
            GetNSTCData(context);
        }
        else
        {
            context.Response.ContentType = "application/json";
            context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, message = "Please set isDo=y to execute" }));
        }
    }

    protected void GetNSTCData(HttpContext context)
    {
        var today = DateTime.Today;

        try
        {
            var url = new Uri("https://wsts.nstc.gov.tw/STSWeb/Award/AwardMultiQuery.aspx");

            // 步驟 1: GET 初始頁面取得 ViewState
            var initialResponse = RequestAction(new RequestOptions()
            {
                Uri = url,
                Method = "GET"
            });

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(initialResponse);

            // 取得 ViewState 等參數
            var viewStateNode = doc.DocumentNode.SelectSingleNode("//input[@id='__VIEWSTATE']");
            string viewState = viewStateNode != null ? viewStateNode.GetAttributeValue("value", "") : "";
            var viewStateGeneratorNode = doc.DocumentNode.SelectSingleNode("//input[@id='__VIEWSTATEGENERATOR']");
            string viewStateGenerator = viewStateGeneratorNode != null ? viewStateGeneratorNode.GetAttributeValue("value", "") : "";
            var eventValidationNode = doc.DocumentNode.SelectSingleNode("//input[@id='__EVENTVALIDATION']");
            string eventValidation = eventValidationNode != null ? eventValidationNode.GetAttributeValue("value", "") : "";

            if (string.IsNullOrEmpty(viewState) || string.IsNullOrEmpty(eventValidation))
            {
                context.Response.ContentType = "application/json";
                context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, message = "無法取得初始頁面的 ViewState 參數" }));
                return;
            }

            // 步驟 2: 模擬點擊「國家科學及技術委員會補助研究計畫」
            List<KeyValuePair<string, string>> clickParams = new List<KeyValuePair<string, string>>();

            // 設定點擊事件的參數
            clickParams.Add(new KeyValuePair<string, string>("__EVENTTARGET", "dtlItem$ctl00$btnItem"));
            clickParams.Add(new KeyValuePair<string, string>("__EVENTARGUMENT", ""));
            clickParams.Add(new KeyValuePair<string, string>("__LASTFOCUS", ""));
            clickParams.Add(new KeyValuePair<string, string>("__VIEWSTATE", viewState));
            clickParams.Add(new KeyValuePair<string, string>("__VIEWSTATEGENERATOR", viewStateGenerator));
            clickParams.Add(new KeyValuePair<string, string>("__EVENTVALIDATION", eventValidation));

            // 發送點擊請求
            var clickResponse = RequestAction(new RequestOptions()
            {
                Uri = url,
                Method = "POST",
                XHRParams = GetParameter(clickParams, UrlEncoding),
                Referer = url.ToString(),
                RequestCookies = sessionCookies
            });

            // 步驟 3: 解析點擊後的頁面，取得新的 ViewState
            doc.LoadHtml(clickResponse);
            viewStateNode = doc.DocumentNode.SelectSingleNode("//input[@id='__VIEWSTATE']");
            viewState = viewStateNode != null ? viewStateNode.GetAttributeValue("value", "") : "";
            viewStateGeneratorNode = doc.DocumentNode.SelectSingleNode("//input[@id='__VIEWSTATEGENERATOR']");
            viewStateGenerator = viewStateGeneratorNode != null ? viewStateGeneratorNode.GetAttributeValue("value", "") : "";
            eventValidationNode = doc.DocumentNode.SelectSingleNode("//input[@id='__EVENTVALIDATION']");
            eventValidation = eventValidationNode != null ? eventValidationNode.GetAttributeValue("value", "") : "";

            if (string.IsNullOrEmpty(viewState) || string.IsNullOrEmpty(eventValidation))
            {
                context.Response.ContentType = "application/json";
                context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, message = "無法取得點擊後頁面的 ViewState 參數" }));
                return;
            }

            // 儲存當前狀態
            currentViewState = viewState;
            currentViewStateGenerator = viewStateGenerator;
            currentEventValidation = eventValidation;
            currentUrl = url;
            existingProjectNames = OSINSTCDataHelper.QueryAllProjectNames();

            // 步驟 4: 取得 API 參數
            string pageSize = context.Request.QueryString["pageSize"] ?? "200"; // 預設每頁 200 筆
            int maxPages = int.Parse(context.Request.QueryString["maxPages"] ?? "100"); // 最大處理頁數
            int delaySeconds = int.Parse(context.Request.QueryString["delaySeconds"] ?? "2"); // 間隔秒數
            bool stopOnDuplicate = bool.Parse(context.Request.QueryString["stopOnDuplicate"] ?? "true"); // 遇到重複資料是否停止

            // 計算中華民國年度（西元年 - 1911）
            int currentRocYear = DateTime.Now.Year - 1911; // 今年的民國年
            int lastRocYear = currentRocYear - 1; // 去年的民國年

            string yearStart = context.Request.QueryString["yearStart"] ?? lastRocYear.ToString(); // 預設起始年度：去年
            string yearEnd = context.Request.QueryString["yearEnd"] ?? currentRocYear.ToString(); // 預設結束年度：今年

            // 執行初始查詢
            var searchResult = ExecuteSearch(pageSize, yearStart, yearEnd);
            if (!searchResult.Success)
            {
                context.Response.ContentType = "application/json";
                context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    success = false,
                    message = searchResult.ErrorMessage,
                    debugInfo = searchResult.DebugInfo
                }));
                return;
            }

            // 開始分頁處理迴圈
            var allParsedData = new List<OSI_NSTCData>();
            var allNewData = new List<OSI_NSTCData>();
            int totalParsedCount = 0;
            int totalInsertCount = 0;
            int processedPages = 0;
            int totalPages = 0;
            string stopReason = "";

            HtmlDocument pageDoc = new HtmlDocument();
            string currentResponse = searchResult.Response;

            // 處理第一頁和後續頁面
            while (processedPages < maxPages)
            {
                // 解析當前頁面
                var pageResult = ProcessPage(currentResponse, ref pageDoc);
                processedPages++;

                if (processedPages == 1)
                {
                    // 從第一頁取得總頁數資訊
                    totalPages = ExtractTotalPages(pageDoc);
                }

                if (!pageResult.Success)
                {
                    stopReason = "頁面解析失敗";
                    break;
                }

                // 累加資料
                allParsedData.AddRange(pageResult.ParsedData);
                allNewData.AddRange(pageResult.NewData);
                totalParsedCount += pageResult.ParsedCount;
                totalInsertCount += pageResult.NewDataCount;

                // 檢查結束條件
                if (stopOnDuplicate && pageResult.NewDataCount == 0 && pageResult.ParsedCount > 0)
                {
                    stopReason = "發現重複資料";
                    break;
                }

                if (IsLastPage(pageDoc))
                {
                    stopReason = "已到達最後一頁";
                    break;
                }

                if (processedPages >= maxPages)
                {
                    stopReason = string.Format("已達最大處理頁數 ({0})", maxPages);
                    break;
                }

                // 等待間隔時間
                System.Threading.Thread.Sleep(delaySeconds * 1000);

                // 切換到下一頁
                var nextPageResult = GoToNextPage();
                if (!nextPageResult.Success)
                {
                    stopReason = "無法切換到下一頁";
                    break;
                }

                currentResponse = nextPageResult.Response;
            }

            // 儲存所有新增資料
            bool saveSuccess = OSINSTCDataHelper.InsertList(allNewData);

            // 準備回傳訊息
            string message = string.Format("共處理 {0} 頁，解析 {1} 筆，新增 {2} 筆至資料庫。{3}",
                processedPages, totalParsedCount, totalInsertCount, stopReason);

            // 回傳結果
            context.Response.ContentType = "application/json";
            var responseObject = new
            {
                success = true,
                message = message,
                totalPages = totalPages,
                processedPages = processedPages,
                stopReason = stopReason,
                parsedCount = totalParsedCount,
                InsertCount = totalInsertCount,
                parsedData = allParsedData.Take(200).ToList(), // 最多只顯示 200 筆
                pageSize = pageSize,
                yearStart = yearStart,
                yearEnd = yearEnd
            };

            context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(responseObject));
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                success = false,
                message = "執行錯誤: " + ex.Message,
                stackTrace = ex.StackTrace
            }));
        }
    }

    /// <summary>
    /// 解析內容欄位（第4欄）
    /// </summary>
    /// <param name="contentCell"></param>
    /// <param name="project"></param>
    private void ParseContentCell(HtmlNode contentCell, OSI_NSTCData project)
    {
        try
        {
            // 解析計畫名稱
            var projectNameSpan = contentCell.SelectSingleNode(".//span[contains(@id, 'lblAWARD_PLAN_CHI_DESCc_')]");
            if (projectNameSpan != null)
            {
                project.tName = projectNameSpan.InnerText != null ? projectNameSpan.InnerText.Trim() : "";
            }

            // 解析執行起迄
            var executionPeriodSpan = contentCell.SelectSingleNode(".//span[contains(@id, 'lblAWARD_ST_ENDc_')]");
            if (executionPeriodSpan != null)
            {
                string executionPeriod = executionPeriodSpan.InnerText != null ? executionPeriodSpan.InnerText.Trim() : "";
                ParseExecutionPeriod(executionPeriod, project);
            }

            // 解析總核定金額
            var totalAmountSpan = contentCell.SelectSingleNode(".//span[contains(@id, 'lblAWARD_TOT_AUD_AMTc_')]");
            if (totalAmountSpan != null)
            {
                project.TotalApprovedAmount = totalAmountSpan.InnerText != null ? totalAmountSpan.InnerText.Trim() : "";
                project.TotalApprovedAmount = project.TotalApprovedAmount.Replace("元", "");
            }
        }
        catch (Exception ex)
        {
            // 內容解析失敗時忽略，但可以記錄錯誤
            System.Diagnostics.Debug.WriteLine(string.Format("ParseContentCell Error: {0}", ex.Message));
        }
    }

    /// <summary>
    /// 解析執行期間字串
    /// </summary>
    /// <param name="executionPeriod"></param>
    /// <param name="project"></param>
    private void ParseExecutionPeriod(string executionPeriod, OSI_NSTCData project)
    {
        if (string.IsNullOrEmpty(executionPeriod))
            return;

        try
        {
            // 嘗試解析執行期間，例如 "2025/08/01~2026/07/31"
            if (executionPeriod.Contains("~"))
            {
                string[] dates = executionPeriod.Split('~');
                if (dates.Length == 2)
                {
                    // 解析開始日期
                    if (DateTime.TryParse(dates[0].Trim(), out DateTime startDate))
                        project.ExecutionStart = startDate;

                    // 解析結束日期
                    if (DateTime.TryParse(dates[1].Trim(), out DateTime endDate))
                        project.ExecutionEnd = endDate;
                }
            }
        }
        catch
        {
            // 日期解析失敗時忽略
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
        request.ServicePoint.Expect100Continue = false;
        request.ServicePoint.UseNagleAlgorithm = false;
        if (!string.IsNullOrEmpty(options.XHRParams)) { request.AllowWriteStreamBuffering = true; } else { request.AllowWriteStreamBuffering = false; }
        request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
        request.ContentType = options.ContentType;
        request.AllowAutoRedirect = options.AllowAutoRedirect;
        request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36";
        request.Timeout = options.Timeout;
        request.KeepAlive = options.KeepAlive;
        if (!string.IsNullOrEmpty(options.Referer)) request.Referer = options.Referer;
        request.Method = options.Method;

        // Cookie 處理
        if (!string.IsNullOrEmpty(options.RequestCookies))
            request.Headers[HttpRequestHeader.Cookie] = options.RequestCookies;

        request.ServicePoint.ConnectionLimit = options.ConnectionLimit;
        if (options.WebHeader != null && options.WebHeader.Count > 0) request.Headers.Add(options.WebHeader);

        if (!string.IsNullOrEmpty(options.XHRParams))
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
            // 保存 Cookie
            if (response.Headers["Set-Cookie"] != null)
            {
                sessionCookies = response.Headers["Set-Cookie"];
            }

            if (response.ContentEncoding.ToLower().Contains("gzip"))
            {
                using (GZipStream stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }
            else if (response.ContentEncoding.ToLower().Contains("deflate"))
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
                using (Stream stream = response.GetResponseStream())
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

    /// <summary>
    /// 執行查詢請求
    /// </summary>
    private SearchResult ExecuteSearch(string pageSize, string yearStart, string yearEnd)
    {
        try
        {
            List<KeyValuePair<string, string>> searchParams = new List<KeyValuePair<string, string>>();

            // ASP.NET 必要參數
            searchParams.Add(new KeyValuePair<string, string>("__EVENTTARGET", ""));
            searchParams.Add(new KeyValuePair<string, string>("__EVENTARGUMENT", ""));
            searchParams.Add(new KeyValuePair<string, string>("__LASTFOCUS", ""));
            searchParams.Add(new KeyValuePair<string, string>("__VIEWSTATE", currentViewState));
            searchParams.Add(new KeyValuePair<string, string>("__VIEWSTATEGENERATOR", currentViewStateGenerator));
            searchParams.Add(new KeyValuePair<string, string>("__EVENTVALIDATION", currentEventValidation));

            // 查詢條件參數
            searchParams.Add(new KeyValuePair<string, string>("wUctlAwardQueryPage$ddlPageSize", pageSize));
            searchParams.Add(new KeyValuePair<string, string>("wUctlAwardQueryPage$repQuery$ctl01$ddlYRst", yearStart));
            searchParams.Add(new KeyValuePair<string, string>("wUctlAwardQueryPage$repQuery$ctl01$ddlYRend", yearEnd));
            searchParams.Add(new KeyValuePair<string, string>("wUctlAwardQueryPage$repQuery$ctl02$ddlD", ""));
            searchParams.Add(new KeyValuePair<string, string>("wUctlAwardQueryPage$repQuery$ctl03$ddlO1", ""));
            searchParams.Add(new KeyValuePair<string, string>("wUctlAwardQueryPage$repQuery$ctl04$txtT", ""));
            searchParams.Add(new KeyValuePair<string, string>("wUctlAwardQueryPage$repQuery$ctl05$txtT", ""));
            searchParams.Add(new KeyValuePair<string, string>("wUctlAwardQueryPage$repQuery$ctl06$ddlS1", ""));
            searchParams.Add(new KeyValuePair<string, string>("wUctlAwardQueryPage$repQuery$ctl07$txtT", ""));
            searchParams.Add(new KeyValuePair<string, string>("wUctlAwardQueryPage$repQuery$ctl08$rblR", "AWARD_YEAR"));
            searchParams.Add(new KeyValuePair<string, string>("wUctlAwardQueryPage$btnQuery", "查詢"));

            var response = RequestAction(new RequestOptions()
            {
                Uri = currentUrl,
                Method = "POST",
                XHRParams = GetParameter(searchParams, UrlEncoding),
                Referer = currentUrl.ToString(),
                RequestCookies = sessionCookies
            });

            // 檢查錯誤重導向
            if (response.Contains("Object moved") || response.Contains("GenericErrorPage"))
            {
                return new SearchResult
                {
                    Success = false,
                    ErrorMessage = "查詢請求被重導向到錯誤頁面",
                    DebugInfo = new
                    {
                        responsePreview = response.Length > 500 ? response.Substring(0, 500) : response
                    }
                };
            }

            return new SearchResult { Success = true, Response = response };
        }
        catch (Exception ex)
        {
            return new SearchResult
            {
                Success = false,
                ErrorMessage = "查詢執行錯誤: " + ex.Message
            };
        }
    }

    /// <summary>
    /// 處理單一頁面的資料解析
    /// </summary>
    private PageResult ProcessPage(string htmlContent, ref HtmlDocument doc)
    {
        try
        {
            doc.LoadHtml(htmlContent);

            var parsedData = new List<OSI_NSTCData>();
            var newData = new List<OSI_NSTCData>();

            // 更新當前的 ViewState 參數
            UpdateViewStateFromDoc(doc);

            // 查找結果表格
            var resultTable = doc.DocumentNode.SelectSingleNode("//table[@id='wUctlAwardQueryPage_grdResult']");
            if (resultTable == null)
            {
                var tableNodes = doc.DocumentNode.SelectNodes("//table");
                if (tableNodes != null)
                {
                    foreach (var table in tableNodes)
                    {
                        if (table.GetAttributeValue("class", "").Contains("Grid_Font"))
                        {
                            var trNodes = table.SelectNodes(".//tr");
                            if (trNodes != null && trNodes.Count > 1)
                            {
                                resultTable = table;
                                break;
                            }
                        }
                    }
                }
            }

            if (resultTable != null)
            {
                var rows = resultTable.SelectNodes(".//tr[position()>1]");
                if (rows != null)
                {
                    foreach (HtmlNode row in rows)
                    {
                        var cells = row.SelectNodes("td");
                        if (cells != null && cells.Count >= 4)
                        {
                            var project = new OSI_NSTCData();
                            project.NewRecord();

                            // 解析資料
                            if (cells.Count > 0)
                                project.Year = cells[0] != null && cells[0].InnerText != null ? cells[0].InnerText.Trim() : "";

                            if (cells.Count > 2)
                                project.Unit = cells[2] != null && cells[2].InnerText != null ? cells[2].InnerText.Trim() : "";

                            if (cells.Count > 3)
                            {
                                ParseContentCell(cells[3], project);
                            }

                            if (!string.IsNullOrEmpty(project.tName))
                            {
                                parsedData.Add(project);

                                if (!existingProjectNames.Contains(project.tName))
                                {
                                    newData.Add(project);
                                    existingProjectNames.Add(project.tName); // 避免重複加入
                                }
                            }
                        }
                    }
                }
            }

            return new PageResult
            {
                Success = true,
                ParsedData = parsedData,
                NewData = newData,
                ParsedCount = parsedData.Count,
                NewDataCount = newData.Count
            };
        }
        catch (Exception ex)
        {
            return new PageResult
            {
                Success = false,
                ErrorMessage = "頁面處理錯誤: " + ex.Message
            };
        }
    }

    /// <summary>
    /// 切換到下一頁
    /// </summary>
    private SearchResult GoToNextPage()
    {
        try
        {
            List<KeyValuePair<string, string>> nextPageParams = new List<KeyValuePair<string, string>>();

            // 限定一次200筆的頁面
            nextPageParams.Add(new KeyValuePair<string, string>("__EVENTTARGET", "wUctlAwardQueryPage$grdResult$ctl203$btnNext"));
            nextPageParams.Add(new KeyValuePair<string, string>("__EVENTARGUMENT", ""));
            nextPageParams.Add(new KeyValuePair<string, string>("__LASTFOCUS", ""));
            nextPageParams.Add(new KeyValuePair<string, string>("__VIEWSTATE", currentViewState));
            nextPageParams.Add(new KeyValuePair<string, string>("__VIEWSTATEGENERATOR", currentViewStateGenerator));
            nextPageParams.Add(new KeyValuePair<string, string>("__EVENTVALIDATION", currentEventValidation));

            var response = RequestAction(new RequestOptions()
            {
                Uri = currentUrl,
                Method = "POST",
                XHRParams = GetParameter(nextPageParams, UrlEncoding),
                Referer = currentUrl.ToString(),
                RequestCookies = sessionCookies
            });

            if (response.Contains("Object moved") || response.Contains("GenericErrorPage"))
            {
                return new SearchResult { Success = false, ErrorMessage = "下一頁請求失敗" };
            }

            return new SearchResult { Success = true, Response = response };
        }
        catch (Exception ex)
        {
            return new SearchResult { Success = false, ErrorMessage = "切換頁面錯誤: " + ex.Message };
        }
    }

    /// <summary>
    /// 檢查是否為最後一頁
    /// </summary>
    private bool IsLastPage(HtmlDocument doc)
    {
        try
        {
            // 檢查下一頁按鈕是否被禁用
            var nextButton = doc.DocumentNode.SelectSingleNode("//input[@id='wUctlAwardQueryPage_grdResult_btnNext']");
            if (nextButton == null)
            {
                // 如果找不到下一頁按鈕，可能是最後一頁
                return true;
            }

            // 檢查是否有disabled屬性或其他標示
            var disabled = nextButton.GetAttributeValue("disabled", "");
            if (!string.IsNullOrEmpty(disabled))
            {
                return true;
            }

            // 檢查按鈕是否變成圖片（disabled狀態）
            var nextImg = doc.DocumentNode.SelectSingleNode("//img[@id='wUctlAwardQueryPage_grdResult_imgNext']");
            if (nextImg != null)
            {
                return true; // 如果變成img標籤，表示被禁用
            }

            return false;
        }
        catch
        {
            return true; // 發生錯誤時假設是最後一頁
        }
    }

    /// <summary>
    /// 從文件中提取總頁數
    /// </summary>
    private int ExtractTotalPages(HtmlDocument doc)
    {
        try
        {
            var pageInfoSpan = doc.DocumentNode.SelectSingleNode("//span[@id='wUctlAwardQueryPage_grdResult_lblPage']");
            if (pageInfoSpan != null)
            {
                string text = pageInfoSpan.InnerText; // 例如: "共59頁(共11632筆)，目前在"
                if (text.Contains("共") && text.Contains("頁"))
                {
                    int startIndex = text.IndexOf("共") + 1;
                    int endIndex = text.IndexOf("頁");
                    if (startIndex < endIndex)
                    {
                        string pageCountStr = text.Substring(startIndex, endIndex - startIndex);
                        if (int.TryParse(pageCountStr, out int pageCount))
                        {
                            return pageCount;
                        }
                    }
                }
            }
            return 0;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// 更新 ViewState 參數
    /// </summary>
    private void UpdateViewStateFromDoc(HtmlDocument doc)
    {
        try
        {
            var viewStateNode = doc.DocumentNode.SelectSingleNode("//input[@id='__VIEWSTATE']");
            if (viewStateNode != null)
                currentViewState = viewStateNode.GetAttributeValue("value", "");

            var viewStateGeneratorNode = doc.DocumentNode.SelectSingleNode("//input[@id='__VIEWSTATEGENERATOR']");
            if (viewStateGeneratorNode != null)
                currentViewStateGenerator = viewStateGeneratorNode.GetAttributeValue("value", "");

            var eventValidationNode = doc.DocumentNode.SelectSingleNode("//input[@id='__EVENTVALIDATION']");
            if (eventValidationNode != null)
                currentEventValidation = eventValidationNode.GetAttributeValue("value", "");
        }
        catch
        {
            // 忽略更新錯誤
        }
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
    /// <summary>
    /// 查詢結果
    /// </summary>
    public class SearchResult
    {
        public bool Success { get; set; }
        public string Response { get; set; }
        public string ErrorMessage { get; set; }
        public object DebugInfo { get; set; }
    }

    /// <summary>
    /// 頁面處理結果
    /// </summary>
    public class PageResult
    {
        public bool Success { get; set; }
        public List<OSI_NSTCData> ParsedData { get; set; }
        public List<OSI_NSTCData> NewData { get; set; }
        public int ParsedCount { get; set; }
        public int NewDataCount { get; set; }
        public string ErrorMessage { get; set; }
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
        public int Timeout = 30000; // 增加到 30 秒
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

}

