using System.Data;
using System.IO;

namespace GS.OCA_OceanSubsidy.Operation.OSI.SpreadsheetReaders
{
    /// <summary>
    /// 試算表讀取器介面
    /// </summary>
    public interface ISpreadsheetReader
    {
        /// <summary>
        /// 將試算表資料讀取為 DataTable
        /// </summary>
        /// <param name="stream">檔案資料流</param>
        /// <returns>包含試算表資料的 DataTable</returns>
        DataTable ReadToDataTable(Stream stream);
    }
}