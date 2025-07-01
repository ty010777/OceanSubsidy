<%@ WebHandler Language="C#" Class="GenImageFromHtml" %>

using System;
using System.IO;
using System.Net;
using System.Web;
using System.Xml;
using System.Linq;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;
using Newtonsoft.Json;
using ImageMagick;

/// <summary>
/// 取得html中的img與canvas來產圖
/// 所有node必須使用position:absolute
/// canvas必須有ID
/// </summary>
public class GenImageFromHtml : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(GenImageFromHtml));

    Graphics graphics = null;
    WebClient client = new WebClient();
    Uri baseUri = null;
    Regex rgxNum = new Regex("[-]?[0-9]+");
    Bitmap bitmap = null;
    int counter = 0;
    HttpRequest Request = null;
    HttpServerUtility Server = null;
    int fullwidth, fullheight, imgWidth, imgHeight;
    float centerX, centerY;
    int startX, startY;
    HttpContext context = null;
    int canvansIdx = 0;
    string basePath = null;
    float rotation = 0;

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        this.context = context;
        Request = context.Request;
        Server = context.Server;

        #region 圖片輸出格式
        String outputFormat = Request["format"];
        ImageFormat imgFormat = ImageFormat.Png;
        if (outputFormat == null) outputFormat = "png";
        if (outputFormat.Equals("jpg", StringComparison.InvariantCultureIgnoreCase))
        {
            outputFormat = "jpg";
            imgFormat = ImageFormat.Jpeg;
        }
        else
        {
            outputFormat = "png";
        }
        #endregion

        var retObj = new RetObject();
        try
        {
            #region 圖片輸出路徑
            basePath = Server.MapPath(context.Request.ApplicationPath);
            basePath = Path.Combine(basePath, "temp\\map_capture");
            if (!Directory.Exists(basePath))
            {
                System.IO.Directory.CreateDirectory(basePath);
            }
            #endregion

            // 清除舊檔案
            clearOldFiles();

            string fileName = Guid.NewGuid().ToString() + "." + outputFormat;
            string fullFileName = System.IO.Path.Combine(basePath, fileName);
            string html = Request["content"];

            if (!string.IsNullOrEmpty(html))
            {
                // 圖台的大小 in pixel
                fullwidth = int.Parse(Request["fullwidth"]);
                fullheight = int.Parse(Request["fullheight"]);

                centerX = fullwidth / 2;
                centerY = fullheight / 2;

                // 輸出圖片大小
                imgWidth = int.Parse(Request["imgWidth"]);
                imgHeight = int.Parse(Request["imgHeight"]);

                // 截圖起始點 in coordinate
                startX = int.Parse(Request["startX"]);
                startY = int.Parse(Request["startY"]);

                // 圖台網址
                baseUri = new Uri(Request["baseUri"]);

                // 圖台範圍
                double minX = double.Parse(Request["minX"]);
                double minY = double.Parse(Request["minY"]);
                double maxX = double.Parse(Request["maxX"]);
                double maxY = double.Parse(Request["maxY"]);


                XmlDocument xDoc = new XmlDocument();

                #region 修正html的img使其符合xml
                Regex rgxImg = new Regex("<img .*?>");
                Regex rgxEndImg = new Regex("</\\s*img>");
                html = "<div xmlns:xlink=\"www.w3.org/1999/xlink\"" + html.Substring(4);
                var matches = rgxImg.Matches(html);
                for (var i = matches.Count - 1; i >= 0; i--)
                {
                    var match = matches[i];
                    if (matches[i].Value.EndsWith("/>"))
                        continue;
                    if (i + 1 < matches.Count && rgxEndImg.IsMatch(matches[i + 1].Value))
                        continue;
                    html = html.Substring(0, match.Index + match.Length - 1) + "/" + html.Substring(match.Index + match.Length - 1);
                }
                //log.Debug("html:" + html);
                #endregion

                xDoc.LoadXml(html);

                #region 圖台可視範圍圖
                log.Debug("繪製網頁可視範圍");
                bitmap = new Bitmap(fullwidth, fullheight);
                graphics = Graphics.FromImage(bitmap);
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //旋轉角度
                float.TryParse(Request["rotation"], out rotation);
                processNode(xDoc.FirstChild, 0, 0, 0);
                //graphics.RotateTransform(rotation);
                graphics.ResetTransform();
                graphics.Save();
                graphics.Dispose();
                //bitmap.Save(System.IO.Path.Combine(basePath, "tt.png"));
                //bitmap.Save(fullFileName.Replace(".png", "_ori.png"));
                #endregion

                #region 擷取所需範圍圖
                log.Debug("繪製所需範圍圖");
                var mainImage = new Bitmap(imgWidth, imgHeight);
                var mainGraphics = Graphics.FromImage(mainImage);
                mainGraphics.DrawImage(bitmap, startX * -1, startY * -1);
                mainGraphics.Save();
                mainGraphics.Dispose();
                bitmap.Dispose();
                #endregion

                mainImage.Save(fullFileName, imgFormat);
                mainImage.Dispose();

                retObj.imageURL = new Uri(Request.Url, Request.ApplicationPath) + "/temp/map_capture/" + fileName;
            }

        }
        catch (Exception e)
        {
            log.Error("A exception happend", e);
        }

        var jSerializer = Newtonsoft.Json.JsonSerializer.CreateDefault();
        var writer = new System.IO.StreamWriter(context.Response.OutputStream);
        jSerializer.Serialize(writer, retObj);
        writer.Flush();
        writer.Dispose();
    }

    /// <summary>
    /// 清除舊檔案
    /// </summary>
    private void clearOldFiles()
    {
        foreach (var file in Directory.GetFiles(basePath, "*.png"))
        {
            if (File.GetCreationTime(file).CompareTo(DateTime.Now.AddHours(-1)) < 0)
            {
                File.Delete(file);
            }
        }

        foreach (var file in Directory.GetFiles(basePath, "*.jpg"))
        {
            if (File.GetCreationTime(file).CompareTo(DateTime.Now.AddHours(-1)) < 0)
            {
                File.Delete(file);
            }
        }

    }

    private void processNode(XmlNode node, int parentX, int parentY, int parentZ)
    {
        CssStyle style;
        if (node.Attributes == null || node.Attributes["style"] == null)
        {
            style = new CssStyle();
        }
        else
        {
            style = parseCssText(node.Attributes["style"].Value);
        }
        if (style.display == "none") return;
        int x = parentX;
        int y = parentY;
        int z = parentZ;
        if (style.left.HasValue) x += style.left.Value;
        if (style.top.HasValue) y += style.top.Value;
        z += style.zIndex;
        if (node.HasChildNodes && node.LocalName != "svg")
        {
            var sortedChildNodes = node.ChildNodes.Cast<XmlNode>().OrderBy(r => r.Attributes == null || r.Attributes["style"] == null ? 0 : parseCssText(r.Attributes["style"].Value).zIndex);
            if (style.transform != null && rotation != 0)
            {
                var matrix = new System.Drawing.Drawing2D.Matrix();
                matrix.RotateAt(rotation, new PointF(style.transform[12], style.transform[13]));
                graphics.Transform = matrix;
            }
            foreach (XmlNode childNode in sortedChildNodes)
            {
                processNode(childNode, x, y, z);
            }
            if (style.transform != null && rotation != 0)
            {
                graphics.ResetTransform();
            }
            return;
        }
        Stream imgStream = null;
        if (node.LocalName == "img")
        {
            string src = node.Attributes["src"].Value;
            // image from URI
            try
            {
                log.Debug("圖檔讀取:" + src);
                log.Debug("[x,y] = " + "[" + x + "," + y + "]");
                imgStream = client.OpenRead(new Uri(baseUri, src));
            }
            catch (Exception ep)
            {
                log.Warn("圖檔讀取失敗: " + src);
            }
            //bitmap.Save(System.Web.HttpContext.Current.Server.MapPath("~/test"+counter+++".png"), ImageFormat.Png);
        }
        else if (node.LocalName == "canvas")
        {
            log.Debug("x:" + x + ", y:" + y);
            // image from base64
            string src = Request["canvasData[" + canvansIdx + "]"];
            //int slashIdx = src.IndexOf('/');
            //string imgType = src.Substring(slashIdx + 1, src.IndexOf(';') - slashIdx);
            byte[] data = Convert.FromBase64String(src.Substring(src.IndexOf(',') + 1));
            imgStream = new MemoryStream(data);
            data = null;
            canvansIdx++;
        }
        else if (node.LocalName == "svg")
        {
            XmlDocument xSvgDoc = new XmlDocument();
            xSvgDoc.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" + node.OuterXml);
            var svgNode = xSvgDoc.ChildNodes[1];
            //if (svgNode.Attributes == null) svgNode.Attributes = new XmlAttributeCollection();
            svgNode.Attributes.Append(xSvgDoc.CreateAttribute("version")).Value = "1.1";
            svgNode.Attributes.Append(xSvgDoc.CreateAttribute("xmlns")).Value = "http://www.w3.org/2000/svg";
            svgNode.Attributes.Append(xSvgDoc.CreateAttribute("xml:space")).Value = "preserve";
            svgNode.Attributes.Append(xSvgDoc.CreateAttribute("baseProfile")).Value = "basic";
            var imageNodes = svgNode.SelectNodes("//image");
            foreach (XmlNode imageNode in imageNodes)
            {
                var uri = new Uri(Request.Url, imageNode.Attributes["xlink:href"].Value);
                imageNode.Attributes["xlink:href"].Value = Server.MapPath(uri.AbsolutePath).Replace("\\", "/");
            }
            try
            {
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                log.Debug("SVG content:" + xSvgDoc.OuterXml);
                writer.Write(xSvgDoc.OuterXml);
                writer.Flush();
                stream.Position = 0;

                MagickReadSettings mrs = new MagickReadSettings();
                mrs = new MagickReadSettings();
                mrs.Format = MagickFormat.Svg;
                //mrs.ColorSpace = ColorSpace.Transparent; 
                MagickImage image = new MagickImage();
                image.BackgroundColor = MagickColor.FromRgba(0, 0, 0, 0);
                image.Read(stream, mrs);
                graphics.DrawImage(image.ToBitmap(), x, y);
                /*
                var svgDoc = Svg.SvgDocument.Open(stream);
                svgDoc.Width = width;
                svgDoc.Height = height;
                graphics.DrawImage(svgDoc.Draw(), x, y);
                 */

                stream.Dispose();
                writer.Dispose();
            }
            catch (Exception ep)
            {
                log.Error("A Exception happened", ep);
            }
        }
        if (imgStream != null)
        {
            graphics.DrawImage(Image.FromStream(imgStream), x, y);
            imgStream.Dispose();
        }


    }

    private CssStyle parseCssText(string cssText)
    {
        var cssItems = cssText.Split(';').Select(r => r.Split(':')).Where(r => r.Length > 1).ToDictionary(r => r[0].Trim(), r => r[1].Trim());
        var ret = new CssStyle();
        ret.left = cssItems.ContainsKey("left") ? int.Parse(rgxNum.Match(cssItems["left"]).Value) : (int?)null;
        ret.right = cssItems.ContainsKey("right") ? int.Parse(rgxNum.Match(cssItems["right"]).Value) : (int?)null;
        ret.top = cssItems.ContainsKey("top") ? int.Parse(rgxNum.Match(cssItems["top"]).Value) : (int?)null;
        ret.bottom = cssItems.ContainsKey("bottom") ? int.Parse(rgxNum.Match(cssItems["bottom"]).Value) : (int?)null;
        ret.height = cssItems.ContainsKey("height") ? int.Parse(rgxNum.Match(cssItems["height"]).Value) : (int?)null;
        ret.width = cssItems.ContainsKey("width") ? int.Parse(rgxNum.Match(cssItems["width"]).Value) : (int?)null;
        ret.zIndex = cssItems.ContainsKey("z-index") ? int.Parse(rgxNum.Match(cssItems["z-index"]).Value) : 0;
        ret.display = cssItems.ContainsKey("display") ? cssItems["display"] : null;
        if (cssItems.ContainsKey("transform") && cssItems["transform"].IndexOf("matrix3d") > -1)
        {
            string transform = cssItems["transform"];
            var startIdx = transform.IndexOf("(");
            transform = transform.Substring(startIdx + 1, transform.IndexOf(")") - startIdx - 1);
            var transValues = transform.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            ret.transform = transValues.Select(r => float.Parse(r)).ToArray();
            if (transValues.Length == 16)
            {
                ret.left = ret.left.HasValue ? ret.left : 0;
                ret.top = ret.top.HasValue ? ret.top : 0;
                ret.left = ret.left.Value + (int)ret.transform[12];
                ret.top = ret.top.Value + (int)ret.transform[13];
            }
            else if (transValues.Length == 6)
            {
                // for IE9
                ret.left = ret.left.HasValue ? ret.left : 0;
                ret.top = ret.top.HasValue ? ret.top : 0;
                ret.left = ret.left.Value + (int)double.Parse(transValues[4]);
                ret.top = ret.top.Value + (int)double.Parse(transValues[5]);
            }
        }
        return ret;
    }

    class CssStyle
    {
        public int? left;
        public int? right;
        public int? top;
        public int? bottom;
        public int? height;
        public int? width;
        public int zIndex;
        public string display;
        public float[] transform;
    }

    private class RetObject
    {
        public string imageURL { get; set; }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}