<%@ WebHandler Language="C#" Class="OSIActivityReportCopy" %>

using System;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using System.Data;
using System.Linq;


public class OSIActivityReportCopy : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        int rtDataPeriodsCount = 0;
        int rtActRptCount = 0;
        context.Response.ContentType = "application/json; charset=utf-8";
        try
        {
            var dataPeriods = (OSIDataPeriodsHelper.QueryByDateTimeWithClass(DateTime.Now)).Where(d => d.IsCopy == false).ToList();
            rtDataPeriodsCount += dataPeriods.Count;
            if (dataPeriods.Count > 0)
            {
                for (int i = 0; i < dataPeriods.Count; i++)
                {
                    string periodID = dataPeriods[i].PeriodID.ToString();
                    DateTime.TryParse(dataPeriods[i].QuarterStartDate.ToString(), out DateTime startDate);
                    DateTime.TryParse(dataPeriods[i].QuarterEndDate.ToString(), out DateTime endDate);
                    var reportIDs = OSIResearchPeriodsHelper.QueryReportIDByRange(startDate, endDate);

                    // 複製 活動填報資料
                    if (reportIDs.Count > 0)
                    {
                        rtActRptCount += reportIDs.Count;
                        reportIDs.ForEach(reportID =>
                        {
                            int newReportID = OSIActivityReportsHelper.CopyReportByID(reportID, periodID, out string sourcePath, out string destPath);
                            // 複製實體檔案
                            if (newReportID != 0 && sourcePath != "" && destPath != "")
                            {
                                try
                                {
                                    CopyDirectory(sourcePath, destPath);
                                    Console.WriteLine("資料夾複製完成！");
                                }
                                catch (Exception ex)
                                {
                                    Console.Error.WriteLine("複製失敗: " + ex.Message);
                                }
                            }
                        });
                    }
                }
            }

            var resp = new
            {
                success = true,
                dataPeriodsCount = rtDataPeriodsCount,
                ActivityReportCount = rtActRptCount,
            };
            context.Response.Write(JsonConvert.SerializeObject(resp));
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            var err = new { success = false, message = ex.Message };
            context.Response.Write(JsonConvert.SerializeObject(err));
        }
    }

    static void CopyDirectory(string sourceDir, string destDir)
    {
        // 1. 確認來源資料夾存在
        if (!Directory.Exists(sourceDir))
            throw new DirectoryNotFoundException($"來源資料夾不存在：{sourceDir}");

        // 2. 建立目的資料夾（若已存在則不會拋例外）
        Directory.CreateDirectory(destDir);

        // 3. 複製檔案
        foreach (var filePath in Directory.GetFiles(sourceDir))
        {
            string fileName = Path.GetFileName(filePath);
            string destFile = Path.Combine(destDir, fileName);

            // overwrite: true 表示如果目的檔已存在，會被覆蓋
            File.Copy(filePath, destFile, overwrite: true);
        }

        // 4. 遞迴複製子資料夾
        foreach (var subDir in Directory.GetDirectories(sourceDir))
        {
            string subDirName = Path.GetFileName(subDir);
            string destSubDir = Path.Combine(destDir, subDirName);

            CopyDirectory(subDir, destSubDir);
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}