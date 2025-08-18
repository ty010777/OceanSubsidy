using System;

namespace GS.OCA_OceanSubsidy.Operation.OSI.SpreadsheetReaders
{
    /// <summary>
    /// 試算表讀取器工廠類別
    /// </summary>
    public static class SpreadsheetReaderFactory
    {
        /// <summary>
        /// 根據副檔名取得對應的讀取器
        /// </summary>
        /// <param name="fileExtension">檔案副檔名（包含點號，例如 .xlsx）</param>
        /// <returns>對應的試算表讀取器實例</returns>
        public static ISpreadsheetReader GetReader(string fileExtension)
        {
            if (string.IsNullOrEmpty(fileExtension))
            {
                throw new ArgumentNullException(nameof(fileExtension), "檔案副檔名不可為空");
            }
            
            switch (fileExtension.ToLower())
            {
                case ".xlsx":
                    return new ExcelReader();
                    
                case ".csv":
                    return new CsvReader();
                    
                case ".ods":
                    return new OdsReader();
                    
                default:
                    throw new NotSupportedException($"不支援的檔案格式: {fileExtension}");
            }
        }
        
        /// <summary>
        /// 檢查是否為支援的檔案格式
        /// </summary>
        /// <param name="fileExtension">檔案副檔名</param>
        /// <returns>是否支援</returns>
        public static bool IsSupportedFormat(string fileExtension)
        {
            if (string.IsNullOrEmpty(fileExtension))
                return false;
                
            switch (fileExtension.ToLower())
            {
                case ".xlsx":
                case ".csv":
                case ".ods":
                    return true;
                default:
                    return false;
            }
        }
        
        /// <summary>
        /// 取得所有支援的檔案格式
        /// </summary>
        /// <returns>支援的副檔名陣列</returns>
        public static string[] GetSupportedFormats()
        {
            return new string[] { ".xlsx", ".csv", ".ods" };
        }
    }
}