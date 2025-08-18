using System;

namespace GS.OCA_OceanSubsidy.Operation.OSI.SpreadsheetReaders
{
    /// <summary>
    /// �պ��Ū�����u�t���O
    /// </summary>
    public static class SpreadsheetReaderFactory
    {
        /// <summary>
        /// �ھڰ��ɦW���o������Ū����
        /// </summary>
        /// <param name="fileExtension">�ɮװ��ɦW�]�]�t�I���A�Ҧp .xlsx�^</param>
        /// <returns>�������պ��Ū�������</returns>
        public static ISpreadsheetReader GetReader(string fileExtension)
        {
            if (string.IsNullOrEmpty(fileExtension))
            {
                throw new ArgumentNullException(nameof(fileExtension), "�ɮװ��ɦW���i����");
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
                    throw new NotSupportedException($"���䴩���ɮ׮榡: {fileExtension}");
            }
        }
        
        /// <summary>
        /// �ˬd�O�_���䴩���ɮ׮榡
        /// </summary>
        /// <param name="fileExtension">�ɮװ��ɦW</param>
        /// <returns>�O�_�䴩</returns>
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
        /// ���o�Ҧ��䴩���ɮ׮榡
        /// </summary>
        /// <returns>�䴩�����ɦW�}�C</returns>
        public static string[] GetSupportedFormats()
        {
            return new string[] { ".xlsx", ".csv", ".ods" };
        }
    }
}