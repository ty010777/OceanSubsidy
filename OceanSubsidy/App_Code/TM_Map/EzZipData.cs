using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;


public class EzZipData
{
    /// <summary>壓縮等級</summary>
    public enum CompressionLevel : int
    {
        /// <summary>快速壓縮</summary>
        FastCompression = 1,
        /// <summary>一般壓縮</summary>
        MiddleCompression = 5,
        /// <summary>最佳壓縮</summary>
        BestCompression = 9
    };


    /// <summary>壓縮資料</summary>
    /// <param name="oriData">來源資料</param>
    /// <param name="level">壓縮等級</param>
    /// <returns>已壓縮的資料</returns>
    public MemoryStream ZipData(ref byte[] oriData, CompressionLevel level)
    {
        MemoryStream mStream = new MemoryStream();

        //建立Zip資料流
        ZipOutputStream objZOS = new ZipOutputStream(mStream);
        //設定壓縮等級
        objZOS.SetLevel((int)level);

        //讀入資料
        ZipEntry entry = new ZipEntry(ZipEntry.CleanName("temp.dat"));
        entry.DateTime = DateTime.Now;
        entry.Size = oriData.Length;
        Crc32 crc = new Crc32();     //CRC檢查
        crc.Reset();
        crc.Update(oriData);
        entry.Crc = crc.Value;

        objZOS.PutNextEntry(entry);
        objZOS.Write(oriData, 0, oriData.Length);
        objZOS.Close();

        return mStream;
    }


    /// <summary>解壓縮資料</summary>
    /// <param name="zipData">已壓縮的資料</param>
    /// <returns>原始資料</returns>
    public byte[] UnZipData(ref MemoryStream zipData)
    {
        ZipInputStream objZOS = new ZipInputStream(zipData);
        byte[] buffer = null;
        ZipEntry entry = objZOS.GetNextEntry();

        if (!(entry == null))
        {
            if (entry.IsFile)
            {
                //從壓縮檔讀取資料
                buffer = new byte[entry.Size];
                objZOS.Read(buffer, 0, buffer.Length);
            }
        }
        objZOS.Close();

        return buffer;
    }


    /// <summary>解壓縮檔案</summary>
    /// <param name="oriName">來源壓縮檔名稱(含路徑)</param>
    /// <param name="desFolder">目的資料夾名稱(含路徑)</param>
    public void UnZipFile(string oriName, string desFolder)
    {
        ZipInputStream s = new ZipInputStream(File.OpenRead(oriName));
        byte[] buffer;
        FileStream stream;
        ZipEntry entry;
        //目的資料夾(含路徑)最後一個字元不為@"\"時，自動加上@"\"
        if (desFolder.Substring(desFolder.Length - 1, 1) != @"\")
        {
            desFolder += @"\";
        }
        //若目的資料夾不存在則建立一個資料夾
        if (!Directory.Exists(desFolder))
        {
            Directory.CreateDirectory(desFolder);
        }

        entry = s.GetNextEntry();
        while (!(entry == null))
        {
            //判斷該資料夾是否存在，若不存在則建立
            if (!Directory.Exists(desFolder + GetFolderName(entry.Name) + @"\"))
            {
                Directory.CreateDirectory(desFolder + GetFolderName(entry.Name) + @"\");
            }

            if (entry.IsFile)
            {
                //從壓縮檔讀取檔案
                buffer = new byte[entry.Size];
                s.Read(buffer, 0, buffer.Length);

                //將資料寫入
                stream = new FileStream(desFolder + entry.Name.Replace(@"/", @"\"), FileMode.Create, FileAccess.Write);
                stream.Write(buffer, 0, buffer.Length);
                stream.Close();
            }

            entry = s.GetNextEntry();
        }
        s.Close();
    }


    /// <summary>壓縮檔案</summary>
    /// <param name="oriName">來源檔案名稱(含路徑)</param>
    /// <param name="desName">目的壓縮檔名稱(含路徑)</param>
    /// <param name="level">壓縮等級</param>
    public void ZipFile(string oriName, string desName, CompressionLevel level)
    {
        Crc32 crc = new Crc32();
        //建立目的檔案
        ZipOutputStream objZOS = new ZipOutputStream(File.Create(desName));
        //設定壓縮等級
        objZOS.SetLevel((int)level);
        FileStream fs = File.OpenRead(oriName);
        //建立暫存區
        byte[] Buffer = new byte[fs.Length];
        //讀入檔案
        fs.Read(Buffer, 0, Buffer.Length);
        ZipEntry entry = new ZipEntry(ZipEntry.CleanName(GetFileName(oriName)));
        entry.DateTime = DateTime.Now;
        //entry.Comment = "test file";
        //entry.ZipFileIndex = 1;
        entry.Size = fs.Length;
        fs.Close();
        crc.Reset();
        crc.Update(Buffer);
        entry.Crc = crc.Value;
        objZOS.PutNextEntry(entry);
        objZOS.Write(Buffer, 0, Buffer.Length);
        objZOS.Finish();
        objZOS.Close();
    }


    /// <summary>壓縮資料夾</summary>
    /// <param name="oriFolderName">來源資料夾名稱(含路徑)</param>
    /// <param name="desName">目的壓縮檔名稱(含路徑)</param>
    /// <param name="level">壓縮等級</param>
    public void ZipFolder(string oriFolderName, string desName, CompressionLevel level)
    {
        ZipOutputStream objZOS = new ZipOutputStream(File.Create(desName));
        objZOS.SetLevel((int)level);
        if (oriFolderName.Substring(oriFolderName.Length - 1, 1) != @"\")
        {
            oriFolderName += @"\";
        }
        ZipFiles(ref oriFolderName, oriFolderName, ref objZOS);
        objZOS.Finish();
        objZOS.Close();
    }


    /// <summary></summary>
    /// <param name="oriFolderName"></param>
    /// <param name="FolderName1"></param>
    /// <param name="objZOS"></param>
    private void ZipFiles(ref string oriFolderName, string FolderName1, ref ZipOutputStream objZOS)
    {
        string[] SubDirectories = Directory.GetDirectories(FolderName1);
        string[] AllFiles = Directory.GetFiles(FolderName1);

        for (int i = 0; i < AllFiles.Length; i++)
        {
            Crc32 crc = new Crc32();
            FileStream fs = File.OpenRead(AllFiles[i]);
            byte[] Buffer = new byte[fs.Length];
            fs.Read(Buffer, 0, Buffer.Length);
            ZipEntry entry = new ZipEntry(ZipEntry.CleanName(AllFiles[i].Replace(oriFolderName, "")));
            entry.DateTime = DateTime.Now;
            //entry.ZipFileIndex = 1;
            entry.Size = fs.Length;
            fs.Close();
            crc.Reset();
            crc.Update(Buffer);
            entry.Crc = crc.Value;
            objZOS.PutNextEntry(entry);
            objZOS.Write(Buffer, 0, Buffer.Length);
        }
        for (int i = 0; i < SubDirectories.Length; i++)
        {
            ZipFiles(ref oriFolderName, SubDirectories[i], ref objZOS);
        }
    }


    /// <summary></summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private string GetFileName(string path)
    {
        path = path.Replace(@"/", @"\");
        string rPath = "";
        int Index = path.LastIndexOf(@"\");
        if (Index == -1)
        {
            rPath = path;
        }
        else
        {
            rPath = path.Substring(Index + 1);
        }
        return rPath;
    }


    /// <summary></summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private string GetFolderName(string path)
    {
        path = path.Replace(@"/", @"\");
        string rPath = "";
        int Index = path.LastIndexOf(@"\");
        if (Index == -1)
        {
            rPath = "";
        }
        else
        {
            rPath = path.Substring(0, Index + 1);
        }
        return rPath;
    }
}
