using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

/// <summary>
/// mapElement2pic 的摘要描述
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
[System.Web.Script.Services.ScriptService]
public class mapElement2pic : System.Web.Services.WebService {

    string subPath = @"~/temp/";

    public mapElement2pic () {

        //如果使用設計的元件，請取消註解下列一行
        //InitializeComponent(); 
    }

    public string FixBase64ForImage(string Image)
    {
        System.Text.StringBuilder sbText = new System.Text.StringBuilder(Image, Image.Length);
        sbText.Replace("\r\n", String.Empty); sbText.Replace(" ", String.Empty);
        return sbText.ToString();
    }

    public static Bitmap Sharpen(Bitmap image)
    {
        Bitmap sharpenImage = (Bitmap)image.Clone();

        //int filterWidth = 5;
        //int filterHeight = 5;      
        int width = image.Width;
        int height = image.Height;

        const int filterWidth = 3;
        const int filterHeight = 3;

        double[,] filter = new double[filterWidth, filterHeight] {
            {  0, -2,  0, },  
            { -2, 11, -2, },  
            {  0, -2,  0, },
        };

        double factor = 0.328;
        double bias = 0.0;

        Color[,] result = new Color[image.Width, image.Height];

        // Lock image bits for read/write.
        BitmapData pbits = sharpenImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb); //Format24bppRgb

        // Declare an array to hold the bytes of the bitmap.
        int bytes = pbits.Stride * height;
        byte[] rgbValues = new byte[bytes];

        // Copy the RGB values into the array.
        System.Runtime.InteropServices.Marshal.Copy(pbits.Scan0, rgbValues, 0, bytes);

        int rgb;
        // Fill the color array with the new sharpened color values.
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                double red = 0.0, green = 0.0, blue = 0.0;

                for (int filterX = 0; filterX < filterWidth; filterX++)
                {
                    for (int filterY = 0; filterY < filterHeight; filterY++)
                    {
                        int imageX = (x - filterWidth / 2 + filterX + width) % width;
                        int imageY = (y - filterHeight / 2 + filterY + height) % height;

                        rgb = imageY * pbits.Stride + 3 * imageX;

                        red += rgbValues[rgb + 2] * filter[filterX, filterY];
                        green += rgbValues[rgb + 1] * filter[filterX, filterY];
                        blue += rgbValues[rgb + 0] * filter[filterX, filterY];
                    }
                    int r = Math.Min(Math.Max((int)(factor * red + bias), 0), 255);
                    int g = Math.Min(Math.Max((int)(factor * green + bias), 0), 255);
                    int b = Math.Min(Math.Max((int)(factor * blue + bias), 0), 255);

                    result[x, y] = Color.FromArgb(r, g, b);
                }
            }
        }

        // Update the image with the sharpened pixels.
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                rgb = y * pbits.Stride + 3 * x;

                rgbValues[rgb + 2] = result[x, y].R;
                rgbValues[rgb + 1] = result[x, y].G;
                rgbValues[rgb + 0] = result[x, y].B;
            }
        }

        // Copy the RGB values back to the bitmap.
        System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, pbits.Scan0, bytes);
        sharpenImage.UnlockBits(pbits);
        return sharpenImage;
    }

    /// <summary>
    /// 產製地圖元素圖片時，先刪除上一次產出的檔案
    /// </summary>
    [WebMethod]
    public void deleteLastFiles()
    {

        bool exists = System.IO.Directory.Exists(Server.MapPath(subPath));

        if (!exists)
            System.IO.Directory.CreateDirectory(Server.MapPath(subPath));

        System.IO.DirectoryInfo di = new DirectoryInfo(Server.MapPath(subPath));

        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete();
        }
    }

    /// <summary>
    /// 產製比例尺圖片
    /// </summary>
    /// <param name="ScaleLineImg"></param>
    [WebMethod]
    public void SaveScaleLineImage(string ScaleLineImg)
    {
        string path = Server.MapPath(subPath);
        string fileNameWitPath = path + "scaleline.png";
        Byte[] bitmapData = Convert.FromBase64String(FixBase64ForImage(ScaleLineImg));
        System.IO.MemoryStream streamBitmap = new System.IO.MemoryStream(bitmapData);
        Bitmap bitImage = new Bitmap((Bitmap)System.Drawing.Image.FromStream(streamBitmap));
        bitImage.SetResolution(96, 96);
        Bitmap outputImage = Sharpen(bitImage);
        outputImage.Save(fileNameWitPath, ImageFormat.Png);
    }

    /// <summary>
    /// 產製鷹眼圖圖片
    /// </summary>
    /// <param name="OveviewImage"></param>
    [WebMethod]
    public void SaveOveviewImage(string OveviewImage)
    {
        string path = Server.MapPath(subPath);
        string fileNameWitPath = path + "overviewmap.png";
        Byte[] bitmapData = Convert.FromBase64String(FixBase64ForImage(OveviewImage));
        System.IO.MemoryStream streamBitmap = new System.IO.MemoryStream(bitmapData);
        Bitmap bitImage = new Bitmap((Bitmap)System.Drawing.Image.FromStream(streamBitmap), new Size(190, 143));
        bitImage.SetResolution(96, 96);
        Bitmap outputImage = Sharpen(bitImage);
        outputImage.Save(fileNameWitPath, ImageFormat.Png);
    }

    /// <summary>
    /// 產製指北針圖片
    /// </summary>
    /// <param name="NortharrowImg"></param>
    [WebMethod]
    public void SaveNortharrowImage(string NortharrowImg)
    {
        string path = Server.MapPath(subPath);
        string fileNameWitPath = path + "northarrow.png";
        Byte[] bitmapData = Convert.FromBase64String(FixBase64ForImage(NortharrowImg));
        System.IO.MemoryStream streamBitmap = new System.IO.MemoryStream(bitmapData);
        Bitmap bitImage = new Bitmap((Bitmap)System.Drawing.Image.FromStream(streamBitmap), new Size(60, 68));     
        bitImage.SetResolution(96, 96);
        bitImage.Save(fileNameWitPath, ImageFormat.Png);
    }

    /// <summary>
    /// 產製圖例圖片
    /// </summary>
    /// <param name="LegendImg"></param>
    [WebMethod]
    public void SaveLegendImage(string LegendImg)
    {
        string path = Server.MapPath(subPath);
        string fileNameWitPath = path + "legend.png";
        Byte[] bitmapData = Convert.FromBase64String(FixBase64ForImage(LegendImg));
        System.IO.MemoryStream streamBitmap = new System.IO.MemoryStream(bitmapData);
        Bitmap bitImage = new Bitmap((Bitmap)System.Drawing.Image.FromStream(streamBitmap));
        bitImage.SetResolution(96, 96);
        bitImage.Save(fileNameWitPath, ImageFormat.Png);
    }

    /// <summary>
    /// 產製圖台截圖
    /// </summary>
    /// <param name="MapImg"></param>
    [WebMethod]
    public void SaveMapImage(string MapImg)
    {
        string path = Server.MapPath(subPath);
        string fileNameWitPath = path + "Map.png";
        Byte[] bitmapData = Convert.FromBase64String(FixBase64ForImage(MapImg));
        System.IO.MemoryStream streamBitmap = new System.IO.MemoryStream(bitmapData);
        Bitmap bitImage = new Bitmap((Bitmap)System.Drawing.Image.FromStream(streamBitmap));
        bitImage.SetResolution(96, 96);
        bitImage.Save(fileNameWitPath, ImageFormat.Png);
    }

    /// <summary>
    /// 將圖台截圖及地圖元素結合產出
    /// </summary>
    /// <param name="FinalImg"></param>
    /// <param name="Format"></param>
    [WebMethod]
    public void SaveFinalImage(string FinalImg, string Format)
    {
        ImageFormat ImgFormat;
        if (Format == "png")
        {
            ImgFormat = ImageFormat.Png;
        }
        else
        {
            ImgFormat = ImageFormat.Jpeg;
        }
        string path = Server.MapPath(subPath);
        string fileNameWitPath = path + "Final." + Format;
        Byte[] bitmapData = Convert.FromBase64String(FixBase64ForImage(FinalImg));
        System.IO.MemoryStream streamBitmap = new System.IO.MemoryStream(bitmapData);
        Bitmap bitImage = new Bitmap((Bitmap)System.Drawing.Image.FromStream(streamBitmap));
        bitImage.SetResolution(96, 96);
        bitImage.Save(fileNameWitPath, ImgFormat);
    }
}
