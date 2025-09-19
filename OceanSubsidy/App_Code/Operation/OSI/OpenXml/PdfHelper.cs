using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace GS.OCA_OceanSubsidy.Operation.OSI.OpenXml
{
    /// <summary>
    /// PDF 處理輔助類別
    /// 提供 PDF 合併、分割等功能
    /// </summary>
    public class PdfHelper
    {
        /// <summary>
        /// 合併多個 PDF 檔案為一個 PDF
        /// </summary>
        /// <param name="pdfFilePaths">要合併的 PDF 檔案路徑列表</param>
        /// <param name="outputFilePath">輸出檔案路徑（可選，如果為空則輸出到記憶體流）</param>
        /// <returns>合併後的 PDF 位元組陣列</returns>
        public static byte[] MergePdfs(List<string> pdfFilePaths, string outputFilePath = null)
        {
            if (pdfFilePaths == null || pdfFilePaths.Count == 0)
            {
                throw new ArgumentException("PDF 檔案路徑列表不能為空");
            }

            try
            {
                // 建立新的輸出文檔
                PdfDocument outputDocument = new PdfDocument();
                outputDocument.Info.Title = "合併的PDF文檔";
                outputDocument.Info.Creator = "OceanSubsidy System";
                outputDocument.Info.CreationDate = DateTime.Now;

                // 逐一讀取並合併每個 PDF 檔案
                foreach (string filePath in pdfFilePaths)
                {
                    if (!File.Exists(filePath))
                    {
                        throw new FileNotFoundException($"找不到檔案: {filePath}");
                    }

                    // 開啟來源 PDF 文檔
                    PdfDocument inputDocument = PdfReader.Open(filePath, PdfDocumentOpenMode.Import);

                    // 複製所有頁面到輸出文檔
                    for (int pageIndex = 0; pageIndex < inputDocument.PageCount; pageIndex++)
                    {
                        PdfPage page = inputDocument.Pages[pageIndex];
                        outputDocument.AddPage(page);
                    }

                    // 關閉來源文檔
                    inputDocument.Close();
                }

                // 將結果寫入記憶體流或檔案
                byte[] pdfBytes;
                using (MemoryStream stream = new MemoryStream())
                {
                    outputDocument.Save(stream, false);
                    pdfBytes = stream.ToArray();
                }

                // 如果指定了輸出檔案路徑，同時儲存到檔案
                if (!string.IsNullOrEmpty(outputFilePath))
                {
                    File.WriteAllBytes(outputFilePath, pdfBytes);
                }

                // 關閉輸出文檔
                outputDocument.Close();

                return pdfBytes;
            }
            catch (Exception ex)
            {
                throw new Exception($"合併 PDF 檔案時發生錯誤: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 從檔案路徑列表合併 PDF 並直接輸出到 HTTP 回應
        /// </summary>
        /// <param name="pdfFilePaths">要合併的 PDF 檔案路徑列表</param>
        /// <param name="response">HTTP 回應物件</param>
        /// <param name="outputFileName">輸出檔案名稱</param>
        /// <param name="isDownload">是否為下載模式（true: 下載, false: 預覽）</param>
        public static void MergePdfsToResponse(List<string> pdfFilePaths, HttpResponse response, string outputFileName, bool isDownload = true)
        {
            try
            {
                byte[] mergedPdfBytes = MergePdfs(pdfFilePaths);

                // 設定回應標頭
                response.Clear();
                response.ContentType = "application/pdf";

                if (isDownload)
                {
                    response.AddHeader("Content-Disposition", $"attachment; filename=\"{outputFileName}\"");
                }
                else
                {
                    response.AddHeader("Content-Disposition", $"inline; filename=\"{outputFileName}\"");
                }

                response.AddHeader("Content-Length", mergedPdfBytes.Length.ToString());

                // 輸出 PDF 內容
                response.BinaryWrite(mergedPdfBytes);
                response.End();
            }
            catch (Exception ex)
            {
                throw new Exception($"輸出合併 PDF 時發生錯誤: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 從位元組陣列列表合併 PDF
        /// </summary>
        /// <param name="pdfBytesList">PDF 位元組陣列列表</param>
        /// <param name="outputFilePath">輸出檔案路徑（可選）</param>
        /// <returns>合併後的 PDF 位元組陣列</returns>
        public static byte[] MergePdfsFromBytes(List<byte[]> pdfBytesList, string outputFilePath = null)
        {
            if (pdfBytesList == null || pdfBytesList.Count == 0)
            {
                throw new ArgumentException("PDF 位元組陣列列表不能為空");
            }

            try
            {
                // 建立新的輸出文檔
                PdfDocument outputDocument = new PdfDocument();
                outputDocument.Info.Title = "合併的PDF文檔";
                outputDocument.Info.Creator = "OceanSubsidy System";
                outputDocument.Info.CreationDate = DateTime.Now;

                // 逐一處理每個 PDF 位元組陣列
                foreach (byte[] pdfBytes in pdfBytesList)
                {
                    if (pdfBytes == null || pdfBytes.Length == 0)
                    {
                        continue; // 跳過空的位元組陣列
                    }

                    // 從位元組陣列建立記憶體流
                    using (MemoryStream inputStream = new MemoryStream(pdfBytes))
                    {
                        // 開啟來源 PDF 文檔
                        PdfDocument inputDocument = PdfReader.Open(inputStream, PdfDocumentOpenMode.Import);

                        // 複製所有頁面到輸出文檔
                        for (int pageIndex = 0; pageIndex < inputDocument.PageCount; pageIndex++)
                        {
                            PdfPage page = inputDocument.Pages[pageIndex];
                            outputDocument.AddPage(page);
                        }

                        // 關閉來源文檔
                        inputDocument.Close();
                    }
                }

                // 將結果寫入記憶體流或檔案
                byte[] mergedPdfBytes;
                using (MemoryStream outputStream = new MemoryStream())
                {
                    outputDocument.Save(outputStream, false);
                    mergedPdfBytes = outputStream.ToArray();
                }

                // 如果指定了輸出檔案路徑，同時儲存到檔案
                if (!string.IsNullOrEmpty(outputFilePath))
                {
                    File.WriteAllBytes(outputFilePath, mergedPdfBytes);
                }

                // 關閉輸出文檔
                outputDocument.Close();

                return mergedPdfBytes;
            }
            catch (Exception ex)
            {
                throw new Exception($"從位元組陣列合併 PDF 檔案時發生錯誤: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 驗證檔案是否為有效的 PDF 檔案
        /// </summary>
        /// <param name="filePath">檔案路徑</param>
        /// <returns>是否為有效的 PDF</returns>
        public static bool IsValidPdf(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return false;
                }

                using (var document = PdfReader.Open(filePath, PdfDocumentOpenMode.ReadOnly))
                {
                    return document.PageCount > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 驗證位元組陣列是否為有效的 PDF
        /// </summary>
        /// <param name="pdfBytes">PDF 位元組陣列</param>
        /// <returns>是否為有效的 PDF</returns>
        public static bool IsValidPdf(byte[] pdfBytes)
        {
            try
            {
                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    return false;
                }

                using (var stream = new MemoryStream(pdfBytes))
                using (var document = PdfReader.Open(stream, PdfDocumentOpenMode.ReadOnly))
                {
                    return document.PageCount > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 取得 PDF 檔案的頁數
        /// </summary>
        /// <param name="filePath">檔案路徑</param>
        /// <returns>頁數</returns>
        public static int GetPdfPageCount(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return 0;
                }

                using (var document = PdfReader.Open(filePath, PdfDocumentOpenMode.ReadOnly))
                {
                    return document.PageCount;
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 取得 PDF 檔案資訊
        /// </summary>
        /// <param name="filePath">檔案路徑</param>
        /// <returns>PDF 資訊</returns>
        public static PdfInfo GetPdfInfo(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return null;
                }

                using (var document = PdfReader.Open(filePath, PdfDocumentOpenMode.ReadOnly))
                {
                    return new PdfInfo
                    {
                        PageCount = document.PageCount,
                        Title = document.Info.Title,
                        Author = document.Info.Author,
                        Subject = document.Info.Subject,
                        Creator = document.Info.Creator,
                        Producer = document.Info.Producer,
                        CreationDate = document.Info.CreationDate,
                        ModificationDate = document.Info.ModificationDate
                    };
                }
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// PDF 檔案資訊類別
    /// </summary>
    public class PdfInfo
    {
        public int PageCount { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Subject { get; set; }
        public string Creator { get; set; }
        public string Producer { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
    }
}