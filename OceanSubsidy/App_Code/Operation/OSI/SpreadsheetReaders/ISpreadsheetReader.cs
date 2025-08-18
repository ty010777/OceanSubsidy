using System.Data;
using System.IO;

namespace GS.OCA_OceanSubsidy.Operation.OSI.SpreadsheetReaders
{
    /// <summary>
    /// �պ��Ū��������
    /// </summary>
    public interface ISpreadsheetReader
    {
        /// <summary>
        /// �N�պ����Ū���� DataTable
        /// </summary>
        /// <param name="stream">�ɮ׸�Ƭy</param>
        /// <returns>�]�t�պ���ƪ� DataTable</returns>
        DataTable ReadToDataTable(Stream stream);
    }
}