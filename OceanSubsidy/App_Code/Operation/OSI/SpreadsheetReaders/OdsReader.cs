using System;
using System.Data;
using System.IO;
using System.Xml;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;

namespace GS.OCA_OceanSubsidy.Operation.OSI.SpreadsheetReaders
{
    /// <summary>
    /// ODS file reader (using SharpZipLib)
    /// </summary>
    public class OdsReader : ISpreadsheetReader
    {
        public DataTable ReadToDataTable(Stream stream)
        {
            var dt = new DataTable();
            
            try
            {
                // ODS files are actually ZIP files containing XML documents
                using (var zipStream = new ZipInputStream(stream))
                {
                    ZipEntry entry;
                    XmlDocument doc = null;
                    
                    // Find content.xml file
                    while ((entry = zipStream.GetNextEntry()) != null)
                    {
                        if (entry.Name == "content.xml")
                        {
                            doc = new XmlDocument();
                            doc.Load(zipStream);
                            break;
                        }
                    }
                    
                    if (doc == null)
                    {
                        throw new InvalidOperationException("Invalid ODS file format: content.xml not found");
                    }
                    
                    // Set namespace manager
                    var nsmgr = new XmlNamespaceManager(doc.NameTable);
                    nsmgr.AddNamespace("office", "urn:oasis:names:tc:opendocument:xmlns:office:1.0");
                    nsmgr.AddNamespace("table", "urn:oasis:names:tc:opendocument:xmlns:table:1.0");
                    nsmgr.AddNamespace("text", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
                    
                    // Get first table
                    var tables = doc.SelectNodes("//table:table", nsmgr);
                    if (tables == null || tables.Count == 0)
                    {
                        throw new InvalidOperationException("ODS file has no tables");
                    }
                    
                    var table = tables[0];
                    var rows = table.SelectNodes(".//table:table-row", nsmgr);
                    
                    if (rows == null || rows.Count == 0)
                    {
                        throw new InvalidOperationException("ODS file has no data");
                    }
                    
                    // Find maximum column count
                    int maxColumns = 0;
                    foreach (XmlNode row in rows)
                    {
                        int colCount = GetColumnCount(row, nsmgr);
                        if (colCount > maxColumns)
                            maxColumns = colCount;
                    }
                    
                    // Limit max columns to avoid too many empty columns
                    maxColumns = Math.Min(maxColumns, 100);
                    
                    // Create DataTable columns
                    for (int i = 0; i < maxColumns; i++)
                    {
                        dt.Columns.Add($"Column{i + 1}", typeof(string));
                    }
                    
                    // Read each row data
                    foreach (XmlNode row in rows)
                    {
                        var cells = row.SelectNodes(".//table:table-cell | .//table:covered-table-cell", nsmgr);
                        if (cells != null && cells.Count > 0)
                        {
                            var values = new string[maxColumns];
                            int colIndex = 0;
                            
                            foreach (XmlNode cell in cells)
                            {
                                if (colIndex >= maxColumns) break;
                                
                                // Get repeat count
                                var repeatAttr = cell.Attributes["table:number-columns-repeated"];
                                int repeatCount = 1;
                                
                                if (repeatAttr != null && !string.IsNullOrEmpty(repeatAttr.Value))
                                {
                                    if (int.TryParse(repeatAttr.Value, out int parsed))
                                    {
                                        repeatCount = Math.Min(parsed, maxColumns - colIndex);
                                    }
                                }
                                
                                // Get cell value
                                string cellValue = GetCellValue(cell, nsmgr);
                                
                                // Fill values (including repeated cells)
                                for (int i = 0; i < repeatCount && colIndex < maxColumns; i++)
                                {
                                    values[colIndex++] = cellValue;
                                }
                            }
                            
                            // Check if row is empty (all values are empty)
                            bool hasValue = false;
                            for (int i = 0; i < values.Length; i++)
                            {
                                if (!string.IsNullOrWhiteSpace(values[i]))
                                {
                                    hasValue = true;
                                    break;
                                }
                            }
                            
                            if (hasValue)
                            {
                                dt.Rows.Add(values);
                            }
                        }
                    }
                }
                
                if (dt.Rows.Count == 0)
                {
                    throw new InvalidOperationException("ODS file has no data");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to read ODS file: {ex.Message}", ex);
            }
            
            return dt;
        }
        
        private int GetColumnCount(XmlNode row, XmlNamespaceManager nsmgr)
        {
            var cells = row.SelectNodes(".//table:table-cell | .//table:covered-table-cell", nsmgr);
            if (cells == null) return 0;
            
            int count = 0;
            foreach (XmlNode cell in cells)
            {
                var repeatAttr = cell.Attributes["table:number-columns-repeated"];
                int repeatCount = 1;
                
                if (repeatAttr != null && !string.IsNullOrEmpty(repeatAttr.Value))
                {
                    if (int.TryParse(repeatAttr.Value, out int parsed))
                    {
                        // Limit repeat count to avoid excessive values
                        repeatCount = Math.Min(parsed, 1000);
                    }
                }
                
                count += repeatCount;
                
                // Stop counting if exceeds reasonable range
                if (count > 100) return 100;
            }
            
            return count;
        }
        
        private string GetCellValue(XmlNode cell, XmlNamespaceManager nsmgr)
        {
            // Get text content
            var textNodes = cell.SelectNodes(".//text:p", nsmgr);
            if (textNodes == null || textNodes.Count == 0)
                return string.Empty;
            
            var sb = new StringBuilder();
            foreach (XmlNode textNode in textNodes)
            {
                if (sb.Length > 0) sb.AppendLine();
                sb.Append(textNode.InnerText);
            }
            
            return sb.ToString().Trim();
        }
    }
}