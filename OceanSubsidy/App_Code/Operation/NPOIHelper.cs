using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.IO;

public class NPOIHelper
{
    public static MemoryStream CreateExcel(string name, List<string> headers, List<List<string>> rows)
    {
        IWorkbook workBook = new XSSFWorkbook();

        ISheet sheet = workBook.CreateSheet(name);

        int rowIndex = 0;

        if (headers.Count > 0)
        {
            IRow headerRow = sheet.CreateRow(rowIndex);

            for (int i = 0; i < headers.Count; i++)
            {
                headerRow.CreateCell(i).SetCellValue(headers[i]);
            }

            rowIndex++;
        }

        foreach (var row in rows)
        {
            IRow dataRow = sheet.CreateRow(rowIndex);

            for (int i = 0; i < row.Count; i++)
            {
                dataRow.CreateCell(i).SetCellValue(row[i]);
            }

            rowIndex++;
        }

        using (var ms = new MemoryStream())
        {
            workBook.Write(ms);

            return ms;
        }
    }
}
