using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Model.OFS;
using GS.OCA_OceanSubsidy.Operation.OSI.OpenXml;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

/// <summary>
/// OFSRoleHelper 的摘要描述
/// </summary>
public class ReviewCheckListHelper
{
    public ReviewCheckListHelper()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }

    #region 批次匯出簡報功能

    /// <summary>
    /// 批次匯出簡報檔案，產生臨時ZIP檔案
    /// </summary>
    /// <param name="projectIds">專案編號列表</param>
    /// <returns>包含ZIP檔案路徑和統計資訊的結果物件</returns>
    public static BatchPresentationExportResult ExportBatchPresentations(List<string> projectIds)
    {
        if (projectIds == null || projectIds.Count == 0)
        {
            throw new ArgumentException("專案編號列表不能為空");
        }

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            var foundFiles = new List<string>();
            var missingFiles = new List<string>();

            foreach (string projectId in projectIds)
            {
                if (string.IsNullOrEmpty(projectId)) continue;

                // 根據專案編號推斷補助類型
                string projectType = GetProjectTypeFromId(projectId);
                if (string.IsNullOrEmpty(projectType))
                {
                    missingFiles.Add($"{projectId} (無法識別專案類型)");
                    continue;
                }

                // 建構檔案路徑
                string baseDir = HttpContext.Current.Server.MapPath("~/UploadFiles/OFS");
                string projectDir = Path.Combine(baseDir, projectType, projectId, "TechReviewFiles");

                if (Directory.Exists(projectDir))
                {
                    // 搜尋 PPT 檔案 (支援 .ppt, .pptx)
                    var pptFiles = Directory.GetFiles(projectDir, "*.ppt*", SearchOption.AllDirectories);

                    foreach (string pptFile in pptFiles)
                    {
                        string fileName = Path.GetFileName(pptFile);
                        string destFileName = $"{projectId}_{fileName}";
                        string destPath = Path.Combine(tempDir, destFileName);

                        File.Copy(pptFile, destPath);
                        foundFiles.Add(destFileName);
                    }
                }

                if (!foundFiles.Any(f => f.StartsWith(projectId)))
                {
                    missingFiles.Add(projectId);
                }
            }

            // 如果沒有找到任何檔案，拋出例外
            if (foundFiles.Count == 0)
            {
                throw new FileNotFoundException($"未找到任何簡報檔案。缺少檔案的專案: {string.Join(", ", missingFiles)}");
            }

            // 建立 ZIP 檔案
            string zipPath = Path.Combine(Path.GetTempPath(), $"簡報匯出_{DateTime.Now:yyyyMMddHHmmss}.zip");

            // 如果有缺少的檔案，建立說明文件
            if (missingFiles.Count > 0)
            {
                string readmePath = Path.Combine(tempDir, "缺少檔案清單.txt");
                using (var writer = new StreamWriter(readmePath, false, System.Text.Encoding.UTF8))
                {
                    writer.WriteLine("以下專案未找到簡報檔案：");
                    writer.WriteLine();
                    foreach (string missing in missingFiles)
                    {
                        writer.WriteLine($"- {missing}");
                    }
                    writer.WriteLine();
                    writer.WriteLine($"成功匯出 {foundFiles.Count} 個檔案");
                    writer.WriteLine($"匯出時間：{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                }
            }

            // 使用 7-Zip 或 .NET 4.5+ 的方式創建 ZIP
            CreateZipFile(tempDir, zipPath);

            return new BatchPresentationExportResult
            {
                ZipFilePath = zipPath,
                TempDirectory = tempDir,
                FoundFileCount = foundFiles.Count,
                MissingFileCount = missingFiles.Count,
                MissingFiles = missingFiles,
                FoundFiles = foundFiles
            };
        }
        catch
        {
            // 發生例外時清理暫時目錄
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
            throw;
        }
    }

    /// <summary>
    /// 創建 ZIP 檔案 (.NET Framework 4.0 兼容版本)
    /// </summary>
    /// <param name="sourceDir">來源目錄</param>
    /// <param name="zipPath">ZIP 檔案路徑</param>
    private static void CreateZipFile(string sourceDir, string zipPath)
    {
        try
        {
            // 使用 Ionic.Zip 或其他第三方庫，如果不可用則使用 Shell 方式
            CreateZipUsingShell(sourceDir, zipPath);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"創建 ZIP 檔案失敗: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 使用 Shell 介面創建 ZIP 檔案
    /// </summary>
    /// <param name="sourceDir">來源目錄</param>
    /// <param name="zipPath">ZIP 檔案路徑</param>
    private static void CreateZipUsingShell(string sourceDir, string zipPath)
    {
        try
        {
            // 使用 Windows Shell 創建 ZIP 檔案
            var shell = new object();
            var shellType = Type.GetTypeFromProgID("Shell.Application");
            shell = Activator.CreateInstance(shellType);

            // 建立空的 ZIP 檔案
            byte[] emptyZip = new byte[] { 80, 75, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            File.WriteAllBytes(zipPath, emptyZip);

            // 等待檔案建立完成
            System.Threading.Thread.Sleep(100);

            // 取得 ZIP 資料夾物件
            var zipFolder = shellType.InvokeMember("NameSpace",
                System.Reflection.BindingFlags.InvokeMethod,
                null, shell, new object[] { zipPath });

            // 複製檔案到 ZIP
            var sourceFiles = Directory.GetFiles(sourceDir);
            foreach (string file in sourceFiles)
            {
                zipFolder.GetType().InvokeMember("CopyHere",
                    System.Reflection.BindingFlags.InvokeMethod,
                    null, zipFolder, new object[] { file, 4 });

                // 等待複製完成
                System.Threading.Thread.Sleep(100);
            }

            // 等待所有操作完成
            System.Threading.Thread.Sleep(500);
        }
        catch (Exception ex)
        {
            // 如果 Shell 方式失敗，嘗試使用 PowerShell
            CreateZipUsingPowerShell(sourceDir, zipPath);
        }
    }

    /// <summary>
    /// 使用 PowerShell 創建 ZIP 檔案
    /// </summary>
    /// <param name="sourceDir">來源目錄</param>
    /// <param name="zipPath">ZIP 檔案路徑</param>
    private static void CreateZipUsingPowerShell(string sourceDir, string zipPath)
    {
        try
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-Command \"Compress-Archive -Path '{sourceDir}\\*' -DestinationPath '{zipPath}' -Force\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var process = System.Diagnostics.Process.Start(startInfo))
            {
                process.WaitForExit(30000); // 等待最多30秒
                if (process.ExitCode != 0)
                {
                    string error = process.StandardError.ReadToEnd();
                    throw new InvalidOperationException($"PowerShell 創建 ZIP 失敗: {error}");
                }
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"PowerShell 創建 ZIP 檔案失敗: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 從專案編號取得專案類型
    /// </summary>
    /// <param name="projectId">專案編號</param>
    /// <returns>專案類型代碼 (僅支援 SCI 和 CUL)</returns>
    private static string GetProjectTypeFromId(string projectId)
    {
        if (string.IsNullOrEmpty(projectId)) return string.Empty;

        // 技術審查階段僅支援 SCI 和 CUL 兩種補助案
        if (projectId.ToUpper().Contains("SCI"))
        {
            return "SCI";
        }
        else if (projectId.ToUpper().Contains("CUL"))
        {
            return "CUL";
        }

        return string.Empty;
    }

    #endregion

    #region 資料轉換方法

    /// <summary>
    /// 將英文類別代碼轉換為中文類別名稱
    /// </summary>
    /// <param name="categoryCode">英文類別代碼</param>
    /// <returns>中文類別名稱</returns>
    public static string ConvertCategoryCodeToName(string categoryCode)
    {
        if (string.IsNullOrEmpty(categoryCode)) return "";

        switch (categoryCode.ToUpper())
        {
            case "SCI":
                return "科專";
            case "CUL":
                return "文化";
            case "EDC":
                return "學校民間";
            case "CLB":
                return "學校社團";
            case "MUL":
                return "多元";
            case "LIT":
                return "素養";
            case "ACC":
                return "無障礙";
            default:
                return categoryCode; // 如果找不到對應，返回原始值
        }
    }

    /// <summary>
    /// 將中文類別名稱轉換為英文類別代碼
    /// </summary>
    /// <param name="categoryName">中文類別名稱</param>
    /// <returns>英文類別代碼</returns>
    public static string ConvertCategoryNameToCode(string categoryName)
    {
        if (string.IsNullOrEmpty(categoryName)) return "";

        switch (categoryName)
        {
            case "科專":
                return "SCI";
            case "文化":
                return "CUL";
            case "學校民間":
                return "EDC";
            case "學校社團":
                return "CLB";
            case "多元":
                return "MUL";
            case "素養":
                return "LIT";
            case "無障礙":
                return "ACC";
            default:
                return categoryName; // 如果找不到對應，返回原始值
        }
    }

    #endregion

    #region 下拉選單資料載入方法

    /// <summary>
    /// 取得審查階段的狀態選項（固定選項）
    /// </summary>
    /// <returns>階段狀態清單</returns>
    public static List<DropdownItem> GetReviewStageStatusOptions()
    {
        // 固定的階段狀態選項
        List<DropdownItem> result = new List<DropdownItem>
        {
            new DropdownItem { Value = "", Text = "全部" },
            new DropdownItem { Value = "審核中", Text = "審核中" },
            new DropdownItem { Value = "通過", Text = "通過" },
            new DropdownItem { Value = "未通過", Text = "未通過" },
            new DropdownItem { Value = "補正補件", Text = "補正補件" },
            new DropdownItem { Value = "逾期未補", Text = "逾期未補" }
        };

        return result;
    }
    /// <summary>
    /// 取得指定資格審查的申請單位選項
    /// </summary>
    /// <returns>申請單位清單</returns>
    public static List<DropdownItem> GetType1OrgOptions()
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT　distinct OrgName
              FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type1]
              WHERE OrgName != '' AND OrgName IS NOT NULL
         ";


        try
        {
            db.Parameters.Clear();

            DataTable dt = db.GetTable();
            List<DropdownItem> result = new List<DropdownItem>();

            // 加入「全部申請單位」選項
            result.Add(new DropdownItem { Value = "", Text = "全部申請單位" });

            foreach (DataRow row in dt.Rows)
            {
                if (row["OrgName"] != DBNull.Value)
                {
                    string org = row["OrgName"].ToString();
                    result.Add(new DropdownItem { Value = org, Text = org });
                }
            }
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"取得申請單位選項時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }
    /// <summary>
    /// 取得指定資格審查的申請單位選項
    /// </summary>
    /// <returns>申請單位清單</returns>
    public static List<DropdownItem> GetType4OrgOptions()
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT　distinct OrgName
              FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type4]
              WHERE OrgName != '' AND OrgName IS NOT NULL
         ";


        try
        {
            db.Parameters.Clear();

            DataTable dt = db.GetTable();
            List<DropdownItem> result = new List<DropdownItem>();

            // 加入「全部申請單位」選項
            result.Add(new DropdownItem { Value = "", Text = "全部申請單位" });

            foreach (DataRow row in dt.Rows)
            {
                if (row["OrgName"] != DBNull.Value)
                {
                    string org = row["OrgName"].ToString();
                    result.Add(new DropdownItem { Value = org, Text = org });
                }
            }
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"取得申請單位選項時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }
    /// <summary>
    /// 取得指定審查階段的申請單位選項
    /// </summary>
    /// <param name="status">審查階段狀態</param>
    /// <returns>申請單位清單</returns>
    public static List<DropdownItem> GetReviewOrgOptions(string status = "")
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
         SELECT DISTINCT OrgName
            FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main] PM
	        LEFT JOIN OFS_SCI_Application_Main AM
	        on AM.ProjectID = PM.ProjectID
            WHERE OrgName != '' AND OrgName IS NOT NULL
         ";

        // 如果有指定審查階段，加入篩選條件
        if (!string.IsNullOrEmpty(status))
        {
            db.CommandText += " AND Statuses LIKE @status";
        }

        try
        {
            db.Parameters.Clear();
            if (!string.IsNullOrEmpty(status))
            {
                db.Parameters.Add("@status", $"%{status}%");
            }

            DataTable dt = db.GetTable();
            List<DropdownItem> result = new List<DropdownItem>();

            // 加入「全部申請單位」選項
            result.Add(new DropdownItem { Value = "", Text = "全部申請單位" });

            foreach (DataRow row in dt.Rows)
            {
                if (row["OrgName"] != DBNull.Value)
                {
                    string org = row["OrgName"].ToString();
                    result.Add(new DropdownItem { Value = org, Text = org });
                }
            }
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"取得申請單位選項時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 取得 Type6 執行計畫審核的申請單位選項
    /// </summary>
    /// <returns>申請單位清單</returns>
    public static List<DropdownItem> GetType6OrgOptions()
    {
        DbHelper db = new DbHelper();
        List<DropdownItem> result = new List<DropdownItem>();

        try
        {
            db.CommandText = @"
                SELECT distinct [OrgName]
                FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type6]
                WHERE [OrgName] IS NOT NULL AND [OrgName] != ''
                ORDER BY [OrgName]
            ";

            DataTable dt = db.GetTable();

            // 加入「全部申請單位」選項
            result.Add(new DropdownItem { Value = "", Text = "全部申請單位" });

            foreach (DataRow row in dt.Rows)
            {
                if (row["OrgName"] != DBNull.Value)
                {
                    string orgName = row["OrgName"].ToString().Trim();
                    if (!string.IsNullOrEmpty(orgName))
                    {
                        result.Add(new DropdownItem { Value = orgName, Text = orgName });
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Type6 申請單位選項載入完成，共 {result.Count - 1} 個單位");
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得 Type6 申請單位選項時發生錯誤：{ex.Message}");
            throw new Exception($"取得 Type6 申請單位選項時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 取得 Type6 執行計畫審核的主管單位選項
    /// </summary>
    /// <returns>主管單位清單</returns>
    public static List<DropdownItem> GetType6SupervisoryUnitOptions()
    {
        DbHelper db = new DbHelper();
        List<DropdownItem> result = new List<DropdownItem>();

        try
        {
            db.CommandText = @"
                SELECT distinct [SupervisoryUnit]
                FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type6]
                WHERE [SupervisoryUnit] IS NOT NULL AND [SupervisoryUnit] != ''
                ORDER BY [SupervisoryUnit]
            ";

            DataTable dt = db.GetTable();

            // 加入「全部主管單位」選項
            result.Add(new DropdownItem { Value = "", Text = "全部主管單位" });

            foreach (DataRow row in dt.Rows)
            {
                if (row["SupervisoryUnit"] != DBNull.Value)
                {
                    string supervisoryUnit = row["SupervisoryUnit"].ToString().Trim();
                    if (!string.IsNullOrEmpty(supervisoryUnit))
                    {
                        result.Add(new DropdownItem { Value = supervisoryUnit, Text = supervisoryUnit });
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Type6 主管單位選項載入完成，共 {result.Count - 1} 個單位");
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得 Type6 主管單位選項時發生錯誤：{ex.Message}");
            throw new Exception($"取得 Type6 主管單位選項時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 取得 Type5 計畫變更審核的申請單位選項
    /// </summary>
    /// <returns>申請單位清單</returns>
    public static List<DropdownItem> GetType5OrgOptions()
    {
        DbHelper db = new DbHelper();
        List<DropdownItem> result = new List<DropdownItem>();

        try
        {
            db.CommandText = @"
                SELECT distinct [OrgName]
                FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type5]
                WHERE [OrgName] IS NOT NULL AND [OrgName] != ''
                ORDER BY [OrgName]
            ";

            DataTable dt = db.GetTable();

            // 加入「全部申請單位」選項
            result.Add(new DropdownItem { Value = "", Text = "全部申請單位" });

            foreach (DataRow row in dt.Rows)
            {
                if (row["OrgName"] != DBNull.Value)
                {
                    string orgName = row["OrgName"].ToString().Trim();
                    if (!string.IsNullOrEmpty(orgName))
                    {
                        result.Add(new DropdownItem { Value = orgName, Text = orgName });
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Type5 申請單位選項載入完成，共 {result.Count - 1} 個單位");
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得 Type5 申請單位選項時發生錯誤：{ex.Message}");
            throw new Exception($"取得 Type5 申請單位選項時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 取得 Type5 計畫變更審核的主管單位選項
    /// </summary>
    /// <returns>主管單位清單</returns>
    public static List<DropdownItem> GetType5SupervisoryUnitOptions()
    {
        DbHelper db = new DbHelper();
        List<DropdownItem> result = new List<DropdownItem>();

        try
        {
            db.CommandText = @"
                SELECT distinct [SupervisoryUnit]
                FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type5]
                WHERE [SupervisoryUnit] IS NOT NULL AND [SupervisoryUnit] != ''
                ORDER BY [SupervisoryUnit]
            ";

            DataTable dt = db.GetTable();

            // 加入「全部主管單位」選項
            result.Add(new DropdownItem { Value = "", Text = "全部主管單位" });

            foreach (DataRow row in dt.Rows)
            {
                if (row["SupervisoryUnit"] != DBNull.Value)
                {
                    string supervisoryUnit = row["SupervisoryUnit"].ToString().Trim();
                    if (!string.IsNullOrEmpty(supervisoryUnit))
                    {
                        result.Add(new DropdownItem { Value = supervisoryUnit, Text = supervisoryUnit });
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Type5 主管單位選項載入完成，共 {result.Count - 1} 個單位");
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得 Type5 主管單位選項時發生錯誤：{ex.Message}");
            throw new Exception($"取得 Type5 主管單位選項時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 取得 Type1 資格審查的承辦人員選項
    /// </summary>
    /// <returns>承辦人員清單</returns>
    public static List<DropdownItem> GetType1SupervisorOptions()
    {
        DbHelper db = new DbHelper();
        List<DropdownItem> result = new List<DropdownItem>();

        try
        {
            db.CommandText = @"
                SELECT DISTINCT SupervisoryPersonAccount, SupervisoryPersonName
                FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type1]
                WHERE SupervisoryPersonAccount != ''
                  AND SupervisoryPersonName != ''
                  AND SupervisoryPersonAccount IS NOT NULL
                  AND SupervisoryPersonName IS NOT NULL
                ORDER BY SupervisoryPersonName
            ";

            DataTable dt = db.GetTable();

            // 加入「全部承辦人員」選項
            result.Add(new DropdownItem { Value = "", Text = "全部承辦人員" });

            foreach (DataRow row in dt.Rows)
            {
                if (row["SupervisoryPersonAccount"] != DBNull.Value && row["SupervisoryPersonName"] != DBNull.Value)
                {
                    string account = row["SupervisoryPersonAccount"].ToString().Trim();
                    string name = row["SupervisoryPersonName"].ToString().Trim();

                    if (!string.IsNullOrEmpty(account) && !string.IsNullOrEmpty(name))
                    {
                        result.Add(new DropdownItem { Value = account, Text = name });
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Type1 承辦人員選項載入完成，共 {result.Count - 1} 個人員");
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得 Type1 承辦人員選項時發生錯誤：{ex.Message}");
            throw new Exception($"取得 Type1 承辦人員選項時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 取得 Type4 決審核定的承辦人員選項
    /// </summary>
    /// <returns>承辦人員清單</returns>
    public static List<DropdownItem> GetType4SupervisorOptions()
    {
        DbHelper db = new DbHelper();
        List<DropdownItem> result = new List<DropdownItem>();

        try
        {
            db.CommandText = @"
                SELECT DISTINCT SupervisoryPersonAccount, SupervisoryPersonName
                FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type4]
                WHERE SupervisoryPersonAccount != ''
                  AND SupervisoryPersonName != ''
                  AND SupervisoryPersonAccount IS NOT NULL
                  AND SupervisoryPersonName IS NOT NULL
                ORDER BY SupervisoryPersonName
            ";

            DataTable dt = db.GetTable();

            // 加入「全部承辦人員」選項
            result.Add(new DropdownItem { Value = "", Text = "全部承辦人員" });

            foreach (DataRow row in dt.Rows)
            {
                if (row["SupervisoryPersonAccount"] != DBNull.Value && row["SupervisoryPersonName"] != DBNull.Value)
                {
                    string account = row["SupervisoryPersonAccount"].ToString().Trim();
                    string name = row["SupervisoryPersonName"].ToString().Trim();

                    if (!string.IsNullOrEmpty(account) && !string.IsNullOrEmpty(name))
                    {
                        result.Add(new DropdownItem { Value = account, Text = name });
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Type4 承辦人員選項載入完成，共 {result.Count - 1} 個人員");
            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得 Type4 承辦人員選項時發生錯誤：{ex.Message}");
            throw new Exception($"取得 Type4 承辦人員選項時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 取得指定審查階段的承辦人員選項
    /// </summary>
    /// <param name="status">審查階段狀態</param>
    /// <returns>承辦人員清單</returns>
    public static List<DropdownItem> GetReviewSupervisorOptions(string status = "")
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT DISTINCT SupervisoryPersonAccount, SupervisoryPersonName
            FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Project_Main]
            WHERE SupervisoryPersonAccount != '' AND SupervisoryPersonAccount IS NOT NULL
              AND SupervisoryPersonName != '' AND SupervisoryPersonName IS NOT NULL
        ";

        // 如果有指定審查階段，加入篩選條件
        if (!string.IsNullOrEmpty(status))
        {
            db.CommandText += " AND Statuses LIKE @status";
        }

        try
        {
            db.Parameters.Clear();
            if (!string.IsNullOrEmpty(status))
            {
                db.Parameters.Add("@status", $"%{status}%");
            }

            DataTable dt = db.GetTable();
            List<DropdownItem> result = new List<DropdownItem>();

            // 加入「全部」選項
            result.Add(new DropdownItem { Value = "", Text = "全部" });

            foreach (DataRow row in dt.Rows)
            {
                if (row["SupervisoryPersonName"] != DBNull.Value)
                {
                    string account = row["SupervisoryPersonAccount"].ToString();
                    string name = row["SupervisoryPersonName"].ToString();
                    result.Add(new DropdownItem { Value = account, Text = name });
                }
            }

            // TODO  目前只有科專，之後還要有文化、學校民間、學校社團等等的承辦人員 和併入到result。

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"取得承辦人員選項時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }


    /// <summary>
    /// 取得審查進度選項（固定選項）
    /// </summary>
    /// <returns>審查進度清單</returns>
    public static List<DropdownItem> GetReviewProgressOptions()
    {
        // 固定的審查進度選項
        List<DropdownItem> result = new List<DropdownItem>
        {
            new DropdownItem { Value = "", Text = "全部" },
            new DropdownItem { Value = "未完成", Text = "未完成" },
            new DropdownItem { Value = "完成", Text = "完成" }
        };

        return result;
    }

    /// <summary>
    /// 取得回覆狀態選項（固定選項）
    /// </summary>
    /// <returns>回覆狀態清單</returns>
    public static List<DropdownItem> GetReviewReplyStatusOptions()
    {
        // 固定的審查進度選項
        List<DropdownItem> result = new List<DropdownItem>
        {
            new DropdownItem { Value = "", Text = "全部" },
            new DropdownItem { Value = "未完成", Text = "未完成" },
            new DropdownItem { Value = "完成", Text = "完成" }
        };

        return result;
    }

    /// <summary>
    /// 取得科專審查組別選項
    /// </summary>
    /// <returns>科專審查組別清單</returns>
    public static List<DropdownItem> GetSciReviewGroupOptions()
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [Code], [Descname]
            FROM [Sys_ZgsCode]
            WHERE CodeGroup = 'SCIField'
            AND IsValid = 1
            ORDER BY OrderNo, Code
        ";

        List<DropdownItem> result = new List<DropdownItem>();

        try
        {
            db.Parameters.Clear();
            DataTable dt = db.GetTable();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    result.Add(new DropdownItem
                    {
                        Value = row["Code"].ToString(),
                        Text = row["Descname"].ToString()
                    });
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得科專審查組別選項時發生錯誤: {ex.Message}");
        }
        finally
        {
            db.Dispose();
        }

        return result;
    }

    /// <summary>
    /// 取得文化審查組別選項
    /// </summary>
    /// <returns>文化審查組別清單</returns>
    public static List<DropdownItem> GetCulReviewGroupOptions()
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT S.[Code],
                   M.[Descname] + '-' + S.[Descname] AS [Descname]
              FROM [Sys_ZgsCode] AS S
              JOIN [Sys_ZgsCode] AS M ON (M.CodeGroup = 'CULField' AND S.ParentCode = M.Code)
             WHERE S.CodeGroup = 'CULField'
               AND S.IsValid = 1
          ORDER BY S.Code
        ";

        List<DropdownItem> result = new List<DropdownItem>();

        try
        {
            db.Parameters.Clear();
            DataTable dt = db.GetTable();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    result.Add(new DropdownItem
                    {
                        Value = row["Code"].ToString(),
                        Text = row["Descname"].ToString()
                    });
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得文化審查組別選項時發生錯誤: {ex.Message}");
        }
        finally
        {
            db.Dispose();
        }

        return result;
    }

    #endregion

    #region type-4 Search 決審

    // /// <summary>
    // /// 查詢科專決審核定清單（支援分頁）
    // /// </summary>
    // /// <param name="year">年度</param>
    // /// <param name="orgName">申請單位</param>
    // /// <param name="supervisor">承辦人員</param>
    // /// <param name="keyword">關鍵字</param>
    // /// <param name="reviewGroupCode">審查組別代碼</param>
    // /// <param name="pageNumber">頁碼</param>
    // /// <param name="pageSize">每頁筆數</param>
    // /// <param name="totalRecords">總記錄數（輸出參數）</param>
    // /// <returns>分頁資料</returns>
    // public static PaginatedResult<ReviewChecklistItem> Search_SCI_Type4_Paged(
    //     out int totalRecords
    //     , string year = "",
    //     string orgName = "",
    //     string supervisor = "",
    //     string keyword = "",
    //     string reviewGroupCode = "",
    //     int pageNumber = 1,
    //     int pageSize = 10)
    // {
    //     var allData = Search_SCI_Type4(year, orgName, supervisor, keyword, reviewGroupCode);
    //     totalRecords = allData.Count;
    //
    //     var pagedData = allData
    //         .Skip((pageNumber - 1) * pageSize)
    //         .Take(pageSize)
    //         .ToList();
    //
    //     return new PaginatedResult<ReviewChecklistItem>
    //     {
    //         Data = pagedData,
    //         TotalRecords = totalRecords,
    //         PageNumber = pageNumber,
    //         PageSize = pageSize,
    //         TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
    //     };
    // }

    public static List<ReviewChecklistItem> Search_Type4(
        string year = "",
        string orgName = "",
        string supervisor = "",
        string keyword = "",
        string category="",
        string reviewGroupCode = "")
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
SELECT TOP (1000) [ProjectID]
      ,[Year]
      ,[ProjectNameTw]
      ,[OrgName]
      ,[Field]
      ,[SupervisoryPersonAccount]
      ,[TotalSubsidyPrice]
      ,[StatusesName]
      ,[ApprovedSubsidy]
      ,[FinalReviewNotes]
      ,[FinalReviewOrder]
      ,[TotalScore]
  FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type4]

";

        try
        {
            List<ReviewChecklistItem> checklist = new List<ReviewChecklistItem>();
            db.Parameters.Clear();

            // 添加篩選條件參數
            db.CommandText += "Where 1=1";
            if (!string.IsNullOrEmpty(year))
            {
                db.CommandText += " AND Year = @year";
                db.Parameters.Add("@year", year);
            }

            if (!string.IsNullOrEmpty(orgName))
            {
                db.CommandText += " AND OrgName LIKE @orgName";
                db.Parameters.Add("@orgName", $"%{orgName}%");
            }

            if (!string.IsNullOrEmpty(supervisor))
            {
                db.CommandText += " AND SupervisoryPersonAccount = @supervisor";
                db.Parameters.Add("@supervisor", supervisor);
            }

            if (!string.IsNullOrEmpty(category))
            {
                db.CommandText += " AND ProjectID LIKE @category";
                db.Parameters.Add("@category", $"%{category}%");
            }

            if (!string.IsNullOrEmpty(reviewGroupCode))
            {
                db.CommandText += " AND Field = @reviewGroupCode";
                db.Parameters.Add("@reviewGroupCode", reviewGroupCode);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                db.CommandText += " AND (ProjectID LIKE @keyword OR ProjectNameTw LIKE @keyword)";
                db.Parameters.Add("@keyword", $"%{keyword}%");
            }

            db.CommandText += " ORDER BY FinalReviewOrder ASC, TotalScore DESC";

            DataTable dt = db.GetTable();

            foreach (DataRow row in dt.Rows)
            {
                var item = new ReviewChecklistItem
                {
                    ProjectID = row["ProjectID"]?.ToString(),
                    ProjectNameTw = row["ProjectNameTw"]?.ToString(),
                    Year = row["Year"]?.ToString(),
                    UserOrg = row["OrgName"]?.ToString(),
                    OrgName = row["OrgName"]?.ToString(),
                    StatusesName = row["StatusesName"]?.ToString(),
                    SupervisoryPersonAccount = row["SupervisoryPersonAccount"]?.ToString(),
                    TopicField = row["Field"]?.ToString(),
                    Req_SubsidyAmount = row["TotalSubsidyPrice"]?.ToString() ?? "0",
                    ApprovedSubsidy = row["ApprovedSubsidy"]?.ToString() ?? "0",
                    FinalReviewNotes = row["FinalReviewNotes"]?.ToString(),
                    TotalScore = row["TotalScore"]?.ToString() ?? "0",
                    FinalReviewOrder = row["FinalReviewOrder"]?.ToString() ?? "0"
                };

                checklist.Add(item);
            }

            return checklist;
        }
        catch (Exception ex)
        {
            throw new Exception($"Search_SCI_Type4 查詢時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 排序模式專用查詢 - 取得決審核定階段的案件用於排序
    /// </summary>
    /// <param name="year">年度</param>
    /// <param name="category">計畫類別</param>
    /// <param name="reviewGroupCode">審查組別代碼</param>
    /// <returns>排序模式案件清單</returns>
    public static List<SortingModeItem> Search_ForSorting(
        string year = "",
        string category = "",
        string reviewGroupCode = "")
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
SELECT TOP (1000) [ProjectID]
      ,[Year]
      ,[ProjectNameTw]
      ,[OrgName]
      ,[FinalReviewNotes]
      ,[FinalReviewOrder]
      ,[TotalScore]
      ,[Field]
      ,[Category]
  FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type4_Sort]
    WHERE 1=1
";

        // 動態加入篩選條件
        if (!string.IsNullOrEmpty(year))
        {
            db.CommandText += " AND Year = @year";
        }

        if (!string.IsNullOrEmpty(reviewGroupCode))
        {
            db.CommandText += " AND Field = @reviewGroupCode";
        } 
        if (!string.IsNullOrEmpty(category))
        {
            db.CommandText += " AND Category = @category";
        }

        // 排序：優先依決審排序，再依總分降序
        db.CommandText += " ORDER BY FinalReviewOrder ASC, TotalScore DESC";

        try
        {
            db.Parameters.Clear();

            if (!string.IsNullOrEmpty(year))
            {
                db.Parameters.Add("@year", year);
            }

            if (!string.IsNullOrEmpty(reviewGroupCode))
            {
                db.Parameters.Add("@reviewGroupCode", reviewGroupCode);
            }
            if (!string.IsNullOrEmpty(category))
            {
                db.Parameters.Add("@category", category);
            }

            DataTable dt = db.GetTable();
            List<SortingModeItem> result = new List<SortingModeItem>();

            foreach (DataRow row in dt.Rows)
            {
                SortingModeItem item = new SortingModeItem
                {
                    ProjectID = row["ProjectID"].ToString(),
                    ProjectNameTw = row["ProjectNameTw"].ToString(),
                    OrgName = row["OrgName"].ToString(),
                    // SupervisoryPersonName = row["SupervisoryPersonName"].ToString(),
                    // ApprovedSubsidy = row["ApprovedSubsidy"] != DBNull.Value ? row["ApprovedSubsidy"].ToString() : "0",
                    FinalReviewNotes = row["FinalReviewNotes"] != DBNull.Value ? row["FinalReviewNotes"].ToString() : "",
                    FinalReviewOrder = row["FinalReviewOrder"] != DBNull.Value ? row["FinalReviewOrder"].ToString() : "999",
                    TotalScore = row["TotalScore"] != DBNull.Value ? row["TotalScore"].ToString() : "0",
                    Category = GetProjectCategoryFromId(row["ProjectID"].ToString())
                };

                result.Add(item);
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"Search_SCI_ForSorting 查詢時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 更新排序順序 - 純資料庫操作
    /// </summary>
    /// <param name="sortingItems">要更新的排序項目清單</param>
    public static void UpdateSortingOrder(List<SortingSaveItem> sortingItems)
    {
        DbHelper db = new DbHelper();

        try
        {

            foreach (var item in sortingItems)
            {
                // TODO 正文 根據 Category 決定要更新排序以及備註
                string tableName = GetTableNameByCategory(item.Category);

                db.CommandText = $@"
                    UPDATE {tableName}
                    SET FinalReviewOrder = @finalReviewOrder,
                        FinalReviewNotes = @finalReviewNotes,
                        updated_at = GETDATE()
                    WHERE ProjectID = @projectId
                ";

                db.Parameters.Clear();
                db.Parameters.Add("@finalReviewOrder", item.FinalReviewOrder);
                db.Parameters.Add("@finalReviewNotes", item.FinalReviewNotes ?? "");
                db.Parameters.Add("@projectId", item.ProjectID);

                db.ExecuteNonQuery();
            }

        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 更新核定資料 - 純資料庫操作
    /// </summary>
    /// <param name="tableName">資料表名稱</param>
    /// <param name="parameters">更新參數</param>
    public static void UpdateApprovalData(string tableName, Dictionary<string, object> parameters)
    {
        DbHelper db = new DbHelper();

        try
        {
            // 建立更新 SQL - 只更新核定經費和備註，不更新 FinalReviewOrder
            db.CommandText = $@"
                UPDATE {tableName}
                SET ApprovedSubsidy = @approvedSubsidy,
                    FinalReviewNotes = @finalReviewNotes,
                    updated_at = GETDATE()
                WHERE ProjectID = @projectId
            ";

            db.Parameters.Clear();
            foreach (var param in parameters)
            {
                db.Parameters.Add(param.Key, param.Value);
            }

            db.ExecuteNonQuery();
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 根據專案ID取得計畫類別
    /// </summary>
    /// <param name="projectId">專案ID</param>
    /// <returns>計畫類別</returns>
    private static string GetProjectCategoryFromId(string projectId)
    {
        if (string.IsNullOrEmpty(projectId)) return "其他";

        if (projectId.Contains("SCI"))
            return "科專";
        else if (projectId.Contains("CUL"))
            return "文化";
        else if (projectId.Contains("EDC"))
            return "學校民間";
        else if (projectId.Contains("CLB"))
            return "學校社團";
        else
            return "其他";
    }

    /// <summary>
    /// 根據計畫類別取得對應的資料表名稱
    /// </summary>
    /// <param name="category">計畫類別</param>
    /// <returns>資料表名稱</returns>
    private static string GetTableNameByCategory(string category)
    {
        switch (category)
        {
            // TODO 正文 請更新決審模式下，的 排序 和備註
            case "科專":
                return "OFS_SCI_Project_Main";
            case "文化":
                return "OFS_CUL_Project_Main";
            case "學校民間":
                return "OFS_EDC_Project_Main";
            case "學校社團":
                return "OFS_CLB_Project_Main";
            case "多元":
                return "OFS_MUL_Project_Main";
            case "素養":
                return "OFS_LIT_Project_Main";
            case "無障礙":
                return "OFS_ACC_Project_Main";
            default:
                return "OFS_SCI_Project_Main";
        }
    }

    #endregion

    #region type-1 Search 科專
    public static List<ReviewChecklistItem> Search_Type1(
        string year = "",
        string status = "",
        string orgName = "",
        string supervisor = "",
        string keyword = "",
        string category = "")
    {
        DbHelper db = new DbHelper();
        db.CommandText = $@"
         SELECT TOP (1000) [ProjectID]
              ,[ProjectNameTw]
              ,[OrgName]
              ,[StatusesName]
              ,[ExpirationDate]
              ,[SupervisoryPersonAccount]
              ,[SupervisoryPersonName]
              ,[SupervisoryUnit]
              ,[created_at]
              ,[Req_SubsidyAmount]
              ,[Year]
              ,[Category]
          FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type1]
          where 1 = 1 

";
        try
        {
            List<ReviewChecklistItem> checklist = new List<ReviewChecklistItem>();
            db.Parameters.Clear();

            // 添加篩選條件參數 
            if (!string.IsNullOrEmpty(year))
            {
                db.CommandText += " AND Year = @year";
                db.Parameters.Add("@year", year);
            }

            if (!string.IsNullOrEmpty(category))
            {
                db.CommandText += " AND Category = @category";
                db.Parameters.Add("@category", category);
            }

            if (!string.IsNullOrEmpty(status))
            {
                db.CommandText += " AND StatusesName LIKE @status";
                db.Parameters.Add("@status", $"%{status}%");
            }

            if (!string.IsNullOrEmpty(orgName))
            {
                db.CommandText += " AND OrgName LIKE @orgName";
                db.Parameters.Add("@orgName", $"%{orgName}%");
            }

            if (!string.IsNullOrEmpty(supervisor))
            {
                db.CommandText += " AND SupervisoryPersonAccount = @supervisor";
                db.Parameters.Add("@supervisor", supervisor);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                db.CommandText += " AND (ProjectID LIKE @keyword OR ProjectNameTw LIKE @keyword)";
                db.Parameters.Add("@keyword", $"%{keyword}%");
            }
       
            

            // 執行查詢
            DataTable dt = db.GetTable();

            // 將查詢結果轉換為 ReviewChecklistItem 物件
            foreach (DataRow row in dt.Rows)
            {
                var item = new ReviewChecklistItem
                {
                    ProjectID = row["ProjectID"]?.ToString(),
                    Statuses = "資格審查",
                    StatusesName = row["StatusesName"]?.ToString(),
                    ExpirationDate = row["ExpirationDate"] != DBNull.Value ? (DateTime?)row["ExpirationDate"] : null,
                    SupervisoryUnit = row["SupervisoryUnit"]?.ToString(),
                    SupervisoryPersonName = row["SupervisoryPersonName"]?.ToString(),
                    SupervisoryPersonAccount = row["SupervisoryPersonAccount"]?.ToString(),
                    OrgName = row["OrgName"]?.ToString(),
                    created_at = row["created_at"] != DBNull.Value ? (DateTime?)row["created_at"] : null,

                    // 從 SQL 結果設定的額外欄位
                    ProjectNameTw = row["ProjectNameTw"]?.ToString(),
                    Year = row["Year"]?.ToString(),
                    Req_SubsidyAmount = row["Req_SubsidyAmount"]?.ToString() ?? "0"
                };

                checklist.Add(item);
            }

            return checklist;
        }
        catch (Exception ex)
        {
            throw new Exception($"查詢審查清單時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    #endregion

    #region type-2 Search 科專
    // TODO 想刪除
    // /// <summary>
    // /// 查詢科專領域審查清單（支援分頁）
    // /// </summary>
    // /// <param name="year">年度</param>
    // /// <param name="orgName">申請單位</param>
    // /// <param name="supervisor">承辦人員</param>
    // /// <param name="keyword">關鍵字</param>
    // /// <param name="reviewProgress">審查進度</param>
    // /// <param name="replyProgress">回覆進度</param>
    // /// <param name="pageNumber">頁碼</param>
    // /// <param name="pageSize">每頁筆數</param>
    // /// <param name="totalRecords">總記錄數（輸出參數）</param>
    // /// <returns>分頁資料</returns>
    // public static PaginatedResult<ReviewChecklistItem> Search_SCI_Type2_Paged(out int totalRecords,
    //     string year = "",
    //     string orgName = "",
    //     string supervisor = "",
    //     string keyword = "",
    //     string reviewProgress = "",
    //     string replyProgress = "",
    //     int pageNumber = 1,
    //     int pageSize = 10
    //     )
    // {
    //     var allData = Search_SCI_Type2(year, orgName, supervisor, keyword, reviewProgress, replyProgress);
    //     totalRecords = allData.Count;
    //
    //     var pagedData = allData
    //         .Skip((pageNumber - 1) * pageSize)
    //         .Take(pageSize)
    //         .ToList();
    //
    //     return new PaginatedResult<ReviewChecklistItem>
    //     {
    //         Data = pagedData,
    //         TotalRecords = totalRecords,
    //         PageNumber = pageNumber,
    //         PageSize = pageSize,
    //         TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
    //     };
    // }


    /// <summary>
    /// 取得科專專案資料
    /// </summary>


    /// <summary>
    /// 取得科專基本資料
    /// </summary>
    public static List<ReviewChecklistItem> GetSciBasicData(

        string year,
        string orgName,
        string supervisor,
        string keyword,
        string status )
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT p.*,
                   m.ProjectNameTw,
                   m.OrgName,
                   YEAR(p.created_at) - 1911 AS 'Year',
                   m.SubsidyPlanType,
                   '科專' AS 'Category',
                   ISNULL((SELECT SUM(SubsidyAmount)
                           FROM OFS_SCI_PersonnelCost_TotalFee tf
                           WHERE tf.ProjectID = p.ProjectID), 0) AS 'Req_SubsidyAmount'
            FROM OFS_SCI_Project_Main p
            LEFT JOIN OFS_SCI_Application_Main m ON p.ProjectID = m.ProjectID
            WHERE p.Statuses LIKE @status
              AND (p.isExist = 1 OR p.isExist IS NULL)  AND isWithdrawal != 1
        ";

        // 添加篩選條件
        db.Parameters.Add("@status", $"%{status}%");

        if (!string.IsNullOrEmpty(year))
        {
            db.CommandText += " AND YEAR(p.created_at) - 1911 = @year";
            db.Parameters.Add("@year", year);
        }


        if (!string.IsNullOrEmpty(orgName))
        {
            db.CommandText += " AND p.UserOrg LIKE @orgName";
            db.Parameters.Add("@orgName", $"%{orgName}%");
        }

        if (!string.IsNullOrEmpty(supervisor))
        {
            db.CommandText += " AND p.SupervisoryPersonAccount = @supervisor";
            db.Parameters.Add("@supervisor", supervisor);
        }

        if (!string.IsNullOrEmpty(keyword))
        {
            db.CommandText += " AND (p.ProjectID LIKE @keyword OR m.ProjectNameTw LIKE @keyword)";
            db.Parameters.Add("@keyword", $"%{keyword}%");
        }

        db.CommandText += " ORDER BY p.updated_at DESC, p.ProjectID DESC";

        DataTable dt = db.GetTable();
        List<ReviewChecklistItem> result = new List<ReviewChecklistItem>();

        foreach (DataRow row in dt.Rows)
        {
            var item = new ReviewChecklistItem
            {
                ProjectID = row["ProjectID"]?.ToString(),
                Statuses = row["Statuses"]?.ToString(),
                StatusesName = row["StatusesName"]?.ToString(),
                ExpirationDate = row["ExpirationDate"] != DBNull.Value ? (DateTime?)row["ExpirationDate"] : null,
                SupervisoryUnit = row["SupervisoryUnit"]?.ToString(),
                SupervisoryPersonName = row["SupervisoryPersonName"]?.ToString(),
                SupervisoryPersonAccount = row["SupervisoryPersonAccount"]?.ToString(),
                UserAccount = row["UserAccount"]?.ToString(),
                UserOrg = row["UserOrg"]?.ToString(),
                UserName = row["UserName"]?.ToString(),
                Form1Status = row["Form1Status"]?.ToString(),
                Form2Status = row["Form2Status"]?.ToString(),
                Form3Status = row["Form3Status"]?.ToString(),
                Form4Status = row["Form4Status"]?.ToString(),
                Form5Status = row["Form5Status"]?.ToString(),
                CurrentStep = row["CurrentStep"]?.ToString(),
                created_at = row["created_at"] != DBNull.Value ? (DateTime?)row["created_at"] : null,
                updated_at = row["updated_at"] != DBNull.Value ? (DateTime?)row["updated_at"] : null,

                // Additional fields from Application Main
                ProjectNameTw = row["ProjectNameTw"]?.ToString(),
                OrgName = row["OrgName"]?.ToString(),
                Year = row["Year"]?.ToString(),
                SubsidyPlanType = row["SubsidyPlanType"]?.ToString(),
                Req_SubsidyAmount = row["Req_SubsidyAmount"]?.ToString() ?? "0"
            };

            result.Add(item);
        }

        return result;
    }

    /// <summary>
    /// 取得科專進度資料
    /// </summary>
    public static List<ProgressData> GetSciProgressData(List<string> projectIds , string status)
    {
        if (projectIds.Count == 0) return new List<ProgressData>();

        // 建立 IN 子句的參數
        var projectIdParams = projectIds.Select((id, index) => $"@projectId{index}").ToList();
        string inClause = "(" + string.Join(",", projectIdParams) + ")";
        DbHelper db = new DbHelper();

        db.CommandText = $@"
             SELECT
                RR.ProjectID,
                COUNT(RR.ProjectID) AS TotalCount,
                COUNT(RR.ReviewComment) AS CommentCount,
                COUNT(RR.ReplyComment) AS ReplyCount,
                CASE
                    WHEN COUNT(RR.ProjectID) > 0 AND COUNT(RR.ProjectID) = COUNT(RR.ReviewComment) THEN '完成'
                    ELSE '未完成'
                END AS ReviewProgress,
                CASE
                    WHEN COUNT(RR.ReviewComment) > 0 AND COUNT(RR.ProjectID) = COUNT(RR.ReplyComment) THEN '完成'
                    ELSE '未完成'
                END AS ReplyProgress
            FROM OFS_ReviewRecords RR
            WHERE RR.ReviewStage = @status
            GROUP BY RR.ProjectID
        ";

        // 添加 ProjectID 參數
        db.Parameters.Clear();
        for (int i = 0; i < projectIds.Count; i++)
        {
            db.Parameters.Add($"@projectId{i}", projectIds[i]);
        }
        db.Parameters.Add($"@status", status);
      

        DataTable dt = db.GetTable();
        List<ProgressData> result = new List<ProgressData>();

        foreach (DataRow row in dt.Rows)
        {
            var totalCount = Convert.ToInt32(row["TotalCount"]);
            var commentCount = Convert.ToInt32(row["CommentCount"]);
            var replyCount = Convert.ToInt32(row["ReplyCount"]);
            var reviewProgressValue = row["ReviewProgress"]?.ToString();
            var replyProgressValue = row["ReplyProgress"]?.ToString();

            var item = new ProgressData
            {
                ProjectID = row["ProjectID"]?.ToString(),
                TotalCount = totalCount,
                CommentCount = commentCount,
                ReplyCount = replyCount,
                ReviewProgress = reviewProgressValue,
                ReplyProgress = replyProgressValue,
                ReviewProgressDisplay = reviewProgressValue == "完成"
                    ? $"完成({commentCount}/{totalCount})"
                    : $"未完成({commentCount}/{totalCount})",
                ReplyProgressDisplay = replyProgressValue == "完成"
                    ? $"完成({replyCount}/{totalCount})"
                    : $"未完成({replyCount}/{totalCount})"
            };

            result.Add(item);
        }

        return result;
    }

    /// <summary>
    /// 取得科專審查組別資料
    /// </summary>
    public static List<ReviewGroupData> GetSciReviewGroupData(List<string> projectIds)
    {
        DbHelper db = new DbHelper();
        if (projectIds.Count == 0) return new List<ReviewGroupData>();

        // 建立 IN 子句的參數
        var projectIdParams = projectIds.Select((id, index) => $"@projectId{index}").ToList();
        string inClause = "(" + string.Join(",", projectIdParams) + ")";

        db.CommandText = $@"
            SELECT
                m.ProjectID,
                zg.Descname AS Field_Descname
            FROM OFS_SCI_Application_Main m
            LEFT JOIN Sys_ZgsCode zg ON m.Field = zg.Code
            WHERE m.ProjectID IN {inClause}
        ";

        // 添加 ProjectID 參數
        db.Parameters.Clear();
        for (int i = 0; i < projectIds.Count; i++)
        {
            db.Parameters.Add($"@projectId{i}", projectIds[i]);
        }

        DataTable dt = db.GetTable();
        List<ReviewGroupData> result = new List<ReviewGroupData>();

        foreach (DataRow row in dt.Rows)
        {
            var item = new ReviewGroupData
            {
                ProjectID = row["ProjectID"]?.ToString(),
                Field_Descname = row["Field_Descname"]?.ToString()
            };

            result.Add(item);
        }

        return result;
    }

    #endregion

    #region type-2 Search 文化

    /// <summary>
    /// 取得文化基本資料
    /// </summary>
    public static List<ReviewChecklistItem> GetCulBasicData(string year, string orgName, string supervisor, string keyword, string status)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT O.ProjectID,
                   P.Descname AS Statuses,
                   S.Descname AS StatusesName,
                   O.EndTime AS ExpirationDate,
                   U.UnitName AS SupervisoryUnit,
                   R.Name AS SupervisoryPersonName,
                   R.Account AS SupervisoryPersonAccount,
                   O.UserAccount,
                   O.UserOrg,
                   O.UserName,
                   O.CreateTime AS created_at,
                   O.UpdateTime AS updated_at,
                   O.ProjectName AS ProjectNameTw,
                   O.OrgName,
                   O.Year,
                   O.SubsidyPlanType,
                   O.ApplyAmount AS Req_SubsidyAmount
              FROM OFS_CUL_Project AS O
         LEFT JOIN Sys_User AS R ON (R.UserID = O.Organizer)
         LEFT JOIN Sys_Unit AS U ON (U.UnitID = R.UnitID)
              JOIN Sys_ZgsCode AS S ON (S.CodeGroup = 'ProjectStatus' AND S.Code = O.Status)
              JOIN Sys_ZgsCode AS P ON (P.CodeGroup = 'ProjectProgressStatus' AND P.Code = O.ProgressStatus)
            WHERE P.Descname LIKE @status
              AND O.IsExists = 1

              AND O.IsWithdrawal != 1
        ";

        // 添加篩選條件
        db.Parameters.Add("@status", $"%{status}%");

        if (!string.IsNullOrEmpty(year))
        {
            db.CommandText += " AND O.Year = @year";
            db.Parameters.Add("@year", year);
        }

        if (!string.IsNullOrEmpty(orgName))
        {
            db.CommandText += " AND O.UserOrg LIKE @orgName";
            db.Parameters.Add("@orgName", $"%{orgName}%");
        }

        if (!string.IsNullOrEmpty(supervisor))
        {
            db.CommandText += " AND R.Account = @supervisor";
            db.Parameters.Add("@supervisor", supervisor);
        }

        if (!string.IsNullOrEmpty(keyword))
        {
            db.CommandText += " AND (O.ProjectID LIKE @keyword OR O.ProjectName LIKE @keyword)";
            db.Parameters.Add("@keyword", $"%{keyword}%");
        }

        db.CommandText += " ORDER BY O.UpdateTime DESC, O.ProjectID DESC";

        DataTable dt = db.GetTable();

        var result = new List<ReviewChecklistItem>();

        foreach (DataRow row in dt.Rows)
        {
            result.Add(new ReviewChecklistItem
            {
                ProjectID = row["ProjectID"]?.ToString(),
                Statuses = row["Statuses"]?.ToString(),
                StatusesName = row["StatusesName"]?.ToString(),
                ExpirationDate = row["ExpirationDate"] != DBNull.Value ? (DateTime?)row["ExpirationDate"] : null,
                SupervisoryUnit = row["SupervisoryUnit"]?.ToString(),
                SupervisoryPersonName = row["SupervisoryPersonName"]?.ToString(),
                SupervisoryPersonAccount = row["SupervisoryPersonAccount"]?.ToString(),
                UserAccount = row["UserAccount"]?.ToString(),
                UserOrg = row["UserOrg"]?.ToString(),
                UserName = row["UserName"]?.ToString(),
                created_at = row["created_at"] != DBNull.Value ? (DateTime?)row["created_at"] : null,
                updated_at = row["updated_at"] != DBNull.Value ? (DateTime?)row["updated_at"] : null,
                ProjectNameTw = row["ProjectNameTw"]?.ToString(),
                OrgName = row["OrgName"]?.ToString(),
                Year = row["Year"]?.ToString(),
                SubsidyPlanType = row["SubsidyPlanType"]?.ToString(),
                Req_SubsidyAmount = row["Req_SubsidyAmount"]?.ToString() ?? "0"
            });
        }

        return result;
    }

    /// <summary>
    /// 取得文化進度資料
    /// </summary>
    public static List<ProgressData> GetCulProgressData(List<string> projectIds, string status)
    {
        // 建立 IN 子句的參數
        var projectIdParams = projectIds.Select((id, index) => $"@projectId{index}").ToList();
        string inClause = "(" + string.Join(",", projectIdParams) + ")";
      

        status = status == "初審" ? "2" : "3";
        DbHelper db = new DbHelper();

        db.CommandText = $@"
            SELECT ProjectID,
                   COUNT(ProjectID) AS TotalCount,
                   COUNT(ReviewComment) AS CommentCount,
                   COUNT(ReplyComment) AS ReplyCount,
                   CASE
                       WHEN COUNT(ProjectID) > 0 AND COUNT(ProjectID) = COUNT(ReviewComment) THEN '完成'
                       ELSE '未完成'
                   END AS ReviewProgress,
                   CASE
                       WHEN COUNT(ReviewComment) > 0 AND COUNT(ProjectID) = COUNT(ReplyComment) THEN '完成'
                       ELSE '未完成'
                   END AS ReplyProgress
              FROM OFS_ReviewRecords
             WHERE ReviewStage = @status
          GROUP BY ProjectID
        ";

        // 添加 ProjectID 參數
        db.Parameters.Clear();
        for (int i = 0; i < projectIds.Count; i++)
        {
            db.Parameters.Add($"@projectId{i}", projectIds[i]);
        }

        // 添加 status 參數
        db.Parameters.Add("@status", status);

        DataTable dt = db.GetTable();
        List<ProgressData> result = new List<ProgressData>();

        foreach (DataRow row in dt.Rows)
        {
            var totalCount = Convert.ToInt32(row["TotalCount"]);
            var commentCount = Convert.ToInt32(row["CommentCount"]);
            var replyCount = Convert.ToInt32(row["ReplyCount"]);
            var reviewProgressValue = row["ReviewProgress"]?.ToString();
            var replyProgressValue = row["ReplyProgress"]?.ToString();

            var item = new ProgressData
            {
                ProjectID = row["ProjectID"]?.ToString(),
                TotalCount = totalCount,
                CommentCount = commentCount,
                ReplyCount = replyCount,
                ReviewProgress = reviewProgressValue,
                ReplyProgress = replyProgressValue,
                ReviewProgressDisplay = reviewProgressValue == "完成"
                    ? $"完成({commentCount}/{totalCount})"
                    : $"未完成({commentCount}/{totalCount})",
                ReplyProgressDisplay = replyProgressValue == "完成"
                    ? $"完成({replyCount}/{totalCount})"
                    : $"未完成({replyCount}/{totalCount})"
            };

            result.Add(item);
        }

        return result;
    }

    /// <summary>
    /// 取得文化審查組別資料
    /// </summary>
    public static List<ReviewGroupData> GetCulReviewGroupData(List<string> projectIds)
    {
        DbHelper db = new DbHelper();

        // 建立 IN 子句的參數
        var projectIdParams = projectIds.Select((id, index) => $"@projectId{index}").ToList();
        string inClause = "(" + string.Join(",", projectIdParams) + ")";

        db.CommandText = $@"
            SELECT m.ProjectID,
                   zg.Descname AS Field_Descname
              FROM OFS_CUL_Project m
         LEFT JOIN Sys_ZgsCode zg ON (m.Field = zg.Code AND zg.CodeGroup = 'CULField')
             WHERE m.ProjectID IN {inClause}
        ";

        // 添加 ProjectID 參數
        db.Parameters.Clear();
        for (int i = 0; i < projectIds.Count; i++)
        {
            db.Parameters.Add($"@projectId{i}", projectIds[i]);
        }

        DataTable dt = db.GetTable();

        var result = new List<ReviewGroupData>();

        foreach (DataRow row in dt.Rows)
        {
            var item = new ReviewGroupData
            {
                ProjectID = row["ProjectID"]?.ToString(),
                Field_Descname = row["Field_Descname"]?.ToString()
            };

            result.Add(item);
        }

        return result;
    }

    #endregion

    #region type-3 Search 科專
    //TODO 想刪除
    // /// <summary>
    // /// 查詢科專技術審查清單（支援分頁）
    // /// </summary>
    // /// <param name="year">年度</param>
    // /// <param name="orgName">申請單位</param>
    // /// <param name="supervisor">承辦人員</param>
    // /// <param name="keyword">關鍵字</param>
    // /// <param name="reviewProgress">審查進度</param>
    // /// <param name="replyProgress">回覆進度</param>
    // /// <param name="pageNumber">頁碼</param>
    // /// <param name="pageSize">每頁筆數</param>
    // /// <param name="totalRecords">總記錄數（輸出參數）</param>
    // /// <returns>分頁資料</returns>
    // public static PaginatedResult<ReviewChecklistItem> Search_SCI_Type3_Paged(out int totalRecords,
    //     string year = "",
    //     string orgName = "",
    //     string supervisor = "",
    //     string keyword = "",
    //     string reviewProgress = "",
    //     string replyProgress = "",
    //     int pageNumber = 1,
    //     int pageSize = 10
    //     )
    // {
    //     var allData = Search_SCI_Type3(year, orgName, supervisor, keyword, reviewProgress, replyProgress);
    //     totalRecords = allData.Count;
    //
    //     var pagedData = allData
    //         .Skip((pageNumber - 1) * pageSize)
    //         .Take(pageSize)
    //         .ToList();
    //
    //     return new PaginatedResult<ReviewChecklistItem>
    //     {
    //         Data = pagedData,
    //         TotalRecords = totalRecords,
    //         PageNumber = pageNumber,
    //         PageSize = pageSize,
    //         TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
    //     };
    // }



    #endregion

    #region 批次處理方法


    /// <summary>
    /// 更新專案狀態到資料庫
    /// </summary>
    /// <param name="projectId">專案編號</param>
    /// <param name="newStatus">新狀態</param>
    /// <param name="userAccount">操作者帳號</param>
    /// <returns>是否更新成功</returns>
    public static void UpdateProjectStatusInDatabase(string projectId, string newStatus,
        string userAccount, string StatusesName)
    {
        string finalStatusesName = string.IsNullOrEmpty(StatusesName) ? "" : StatusesName;
        string LastOperation = newStatus == "計畫執行" ? "計畫已核定" : "";
        
        using (DbHelper db = new DbHelper())
        {
            db.CommandText = @"
                UPDATE OFS_SCI_Project_Main
                SET Statuses = @newStatus,
                    StatusesName = @finalStatusesName,
                    LastOperation = @LastOperation,
                    updated_at = GETDATE()
                WHERE ProjectID = @projectId";

            db.Parameters.Add("@projectId", projectId);
            db.Parameters.Add("@newStatus", newStatus);
            db.Parameters.Add("@finalStatusesName", finalStatusesName);
            db.Parameters.Add("@LastOperation", LastOperation);

            db.ExecuteNonQuery();
        }
    }
    /// <summary>
    /// 更新專案狀態到資料庫
    /// </summary>
    /// <param name="projectId">專案編號</param>
    /// <param name="newStatus">新狀態</param>
    /// <param name="userAccount">操作者帳號</param>
    /// <returns>是否更新成功</returns>
    public static void CLB_UpdateProjectStatusInDatabase(string projectId, string newStatus,
        string userAccount, string StatusesName)
    {
        string finalStatusesName = string.IsNullOrEmpty(StatusesName) ? "審核中" : StatusesName;
        string LastOperation = newStatus == "計畫執行" ? "計畫已核定" : "";
        
        using (DbHelper db = new DbHelper())
        {
            db.CommandText = @"
                UPDATE OFS_CLB_Project_Main
                SET Statuses = @newStatus,
                    StatusesName = @finalStatusesName,
                    LastOperation = @LastOperation,
                    updated_at = GETDATE()
                WHERE ProjectID = @projectId";

            db.Parameters.Add("@projectId", projectId);
            db.Parameters.Add("@newStatus", newStatus);
            db.Parameters.Add("@finalStatusesName", finalStatusesName);
            db.Parameters.Add("@LastOperation", LastOperation);

            db.ExecuteNonQuery();
        }
    }
    /// <summary>
    /// 插入審查歷程記錄 - 新版本支援完整狀態描述
    /// </summary>
    /// <param name="projectId">專案編號</param>
    /// <param name="fromStatusFull">完整的原始狀態描述 (例如: "資格審查審核中")</param>
    /// <param name="toStatusFull">完整的目標狀態描述 (例如: "資格審查通過")</param>
    /// <param name="actionDescription">操作描述 (例如: "批次通過，轉入下一階段")</param>
    /// <param name="userName">操作者帳號</param>
    public static void InsertReviewHistory(string projectId, string fromStatusFull, string toStatusFull,
        string actionDescription, string userName) {
        using (DbHelper db = new DbHelper())
        {
            db.CommandText = @"
                INSERT INTO OFS_CaseHistoryLog (
                    ProjectID,
                    ChangeTime,
                    UserName,
                    StageStatusBefore,
                    StageStatusAfter,
                    Description
                ) VALUES (
                    @projectId,
                    GETDATE(),
                    @userName,
                    @fromStatusFull,
                    @toStatusFull,
                    @actionDescription
                )";

            db.Parameters.Add("@projectId", projectId);
            db.Parameters.Add("@userName", userName);
            db.Parameters.Add("@fromStatusFull", fromStatusFull);
            db.Parameters.Add("@toStatusFull", toStatusFull);
            db.Parameters.Add("@actionDescription", actionDescription);

            db.ExecuteNonQuery();
        }
    }


    /// <summary>
    /// 更新專案狀態名稱（只更新StatusesName）
    /// </summary>
    /// <param name="projectId">專案編號</param>
    /// <param name="statusName">狀態名稱</param>
    /// <param name="userAccount">操作者帳號</param>
    public static void SCI_UpdateProjectStatusName(string projectId, string statusName, string userAccount)
    {
        using (DbHelper db = new DbHelper())
        {
            db.CommandText = @"
                UPDATE OFS_SCI_Project_Main
                SET StatusesName = @statusName,
                    updated_at = GETDATE()
                WHERE ProjectID = @projectId";

            db.Parameters.Add("@projectId", projectId);
            db.Parameters.Add("@statusName", statusName);
            
            db.ExecuteNonQuery();
        }
    }
    public static void CLB_UpdateProjectStatusName(string projectId, string statusName, string userAccount)
    {
        using (DbHelper db = new DbHelper())
        {
            db.CommandText = @"
                UPDATE OFS_CLB_Project_Main
                SET StatusesName = @statusName,
                    updated_at = GETDATE()
                WHERE ProjectID = @projectId";

            db.Parameters.Add("@projectId", projectId);
            db.Parameters.Add("@statusName", statusName);
            
            db.ExecuteNonQuery();
        }
    }

    #endregion

    #region 批次不通過處理

    /// <summary>
    /// 批次設定專案狀態為不通過
    /// </summary>
    /// <param name="projectIds">專案編號列表</param>
    /// <param name="userAccount">操作者帳號</param>
    /// <param name="actionType">操作類型</param>
    /// <returns>批次處理結果</returns>
    public static BatchApprovalResult BatchRejectProjectStatus(
        List<string> projectIds,
        string userAccount,
        string actionType,
        string reviewType)
    {
        var result = new BatchApprovalResult
        {
            ActionType = actionType,
            ProcessedAt = DateTime.Now
        };


        using (DbHelper db = new DbHelper())
        {
            try
            {
                int successCount = 0;
                var successIds = new List<string>();
                var errorMessages = new List<string>();

                foreach (string projectId in projectIds)
                {
                    try
                    {
                        int status = 19; 

                        switch (reviewType)
                        {
                            case "2": // 初審
                                status = 29;
                                break;
                            case "3": // 複審
                                status = 39;
                                break;
                            case "4": // 決審核定
                                status = 49;
                                break;
                        }

                        if (projectId.Contains("SCI"))
                        {
                            // 更新專案狀態為不通過
                            UpdateProjectRejectStatus(db, projectId);
                            // 記錄審查歷程
                            InsertRejectHistory(db, projectId, actionType, userAccount);
                            successCount++;
                            successIds.Add(projectId);
                        }
                        else if (projectId.Contains("CUL"))
                        {
                            OFS_CulProjectHelper.updateStatus(projectId, status);
                            InsertRejectHistory(db, projectId, actionType, userAccount);
                            successCount++;
                            successIds.Add(projectId);
                        }
                        else if (projectId.Contains("EDC"))
                        {
                            OFS_EdcProjectHelper.updateStatus(projectId, status);
                            InsertRejectHistory(db, projectId, actionType, userAccount);
                            successCount++;
                            successIds.Add(projectId);
                        }
                        else if (projectId.Contains("CLB"))
                        {
                            // 更新專案狀態為不通過
                            CLB_UpdateProjectRejectStatus(db, projectId);
                            // 記錄審查歷程
                            InsertRejectHistory(db, projectId, actionType, userAccount);
                            successCount++;
                            successIds.Add(projectId);
                        }
                        else if (projectId.Contains("MUL"))
                        {
                            OFS_MulProjectHelper.updateStatus(projectId, status);
                            InsertRejectHistory(db, projectId, actionType, userAccount);
                            successCount++;
                            successIds.Add(projectId);
                        }
                        else if (projectId.Contains("LIT"))
                        {
                            OFS_LitProjectHelper.updateStatus(projectId, status);
                            InsertRejectHistory(db, projectId, actionType, userAccount);
                            successCount++;
                            successIds.Add(projectId);
                        }
                        else if (projectId.Contains("ACC"))
                        {
                            OFS_AccProjectHelper.updateStatus(projectId, status);
                            InsertRejectHistory(db, projectId, actionType, userAccount);
                            successCount++;
                            successIds.Add(projectId);
                        }
                    }
                    catch (Exception ex)
                    {
                        errorMessages.Add($"處理專案 {projectId} 時發生錯誤: {ex.Message}");
                    }
                }

                // 設定結果
                if (successCount > 0)
                {
                    result.Success = true;
                    result.SuccessCount = successCount;
                    result.SuccessProjectIds = successIds;
                    result.Message = $"成功處理 {successCount} 件計畫";

                    if (errorMessages.Count > 0)
                    {
                        result.ErrorMessages = errorMessages;
                    }
                }
                else
                {
                    result.Success = false;
                    result.Message = "所有專案處理失敗";
                    result.ErrorMessages = errorMessages;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "批次處理時發生系統錯誤";
                result.ErrorMessages = new List<string> { ex.Message };
            }
        }

        return result;
    }

    /// <summary>
    /// 更新專案狀態為不通過
    /// </summary>
    /// <param name="db">資料庫連線</param>
    /// <param name="projectId">專案編號</param>
    /// <param name="userAccount">操作者帳號</param>
    private static void UpdateProjectRejectStatus(DbHelper db, string projectId)
    {
        db.CommandText = @"
            UPDATE OFS_SCI_Project_Main
            SET StatusesName = '結案(未通過)',
                isExist = 0,
                updated_at = GETDATE()
            WHERE ProjectID = @projectId";

        db.Parameters.Clear();
        db.Parameters.Add("@projectId", projectId);
        db.ExecuteNonQuery();
    }
    private static void CLB_UpdateProjectRejectStatus(DbHelper db, string projectId)
    {
        db.CommandText = @"
            UPDATE OFS_CLB_Project_Main
            SET StatusesName = '結案(未通過)',
                isExist = 0,
                updated_at = GETDATE()
            WHERE ProjectID = @projectId";

        db.Parameters.Clear();
        db.Parameters.Add("@projectId", projectId);
        db.ExecuteNonQuery();
    }
    /// <summary>
    /// 插入不通過審查歷程記錄
    /// </summary>
    /// <param name="db">資料庫連線</param>
    /// <param name="projectId">專案編號</param>
    /// <param name="actionType">操作類型</param>
    /// <param name="userAccount">操作者帳號</param>
    private static void InsertRejectHistory(DbHelper db, string projectId, string actionType, string userAccount)
    {
        db.CommandText = @"
            INSERT INTO OFS_CaseHistoryLog (
                ProjectID,
                ChangeTime,
                UserName,
                StageStatusBefore,
                StageStatusAfter,
                Description
            ) VALUES (
                @projectId,
                GETDATE(),
                @userAccount,
                '審核中',
                '結案(未通過)',
                @description
            )";

        db.Parameters.Clear();
        db.Parameters.Add("@projectId", projectId);
        db.Parameters.Add("@userAccount", userAccount);
        db.Parameters.Add("@description", $"批次{actionType}: 審核中 → 結案(未通過)");
        db.ExecuteNonQuery();
    }

    #endregion

    #region 科專批次審核後處理

    /// <summary>
    /// 處理科專批次審核後的特殊流程
    /// </summary>
    /// <param name="projectIds">專案編號列表</param>
    /// <param name="reviewStage">審查階段</param>
    /// <param name="actionType">操作類型</param>
    /// <param name="userAccount">操作者帳號</param>
    public static void ProcessSciPostApproval(List<string> projectIds, string reviewStage, string actionType, string userAccount)
    {
        DbHelper db = new DbHelper();

        foreach (string projectId in projectIds)
        {
            // 1. 從 OFS_SCI_Application_Main 取得 Field
            db.CommandText = "SELECT Field FROM OFS_SCI_Application_Main WHERE ProjectID = @ProjectID";
            db.Parameters.Add("@ProjectID", projectId);
            string field = db.GetTable().Rows[0]["Field"]?.ToString();
            db.Parameters.Clear();

            if (!string.IsNullOrEmpty(field))
            {
                // 2. 從 OFS_ReviewCommitteeList 取得對應審查組別的所有人員
                db.CommandText = @"
                        SELECT CommitteeUser, Email
                        FROM OFS_ReviewCommitteeList
                        WHERE SubjectTypeID = @SubjectTypeID";

                db.Parameters.Add("@SubjectTypeID", field);
                DataTable reviewers = db.GetTable();
                db.Parameters.Clear();

                // 3. 取得科專的審查範本
                db.CommandText = @"
                    SELECT TemplateName, TemplateWeight
                    FROM OFS_ReviewTemplate
                    WHERE SubsidyProjects = 'SCI'";

                DataTable templates = db.GetTable();

                // 4. 為每位審查委員建立完整的審核記錄
                foreach (DataRow reviewer in reviewers.Rows)
                {
                    string reviewerEmail = reviewer["Email"]?.ToString();
                    string reviewerName = reviewer["CommitteeUser"]?.ToString();

                    if (!string.IsNullOrEmpty(reviewerEmail) && !string.IsNullOrEmpty(reviewerName))
                    {
                        // 產生隨機 Token
                        string token = Guid.NewGuid().ToString();

                        // 4.1 新增記錄到 OFS_ReviewRecords
                        db.CommandText = @"
                            INSERT INTO OFS_ReviewRecords (
                                ProjectID,
                                ReviewStage,
                                Email,
                                ReviewerName,
                                Token
                            ) VALUES (
                                @projectId,
                                @reviewStage,
                                @reviewerEmail,
                                @reviewerName,
                                @token
                            );
                            SELECT SCOPE_IDENTITY() AS ReviewID;";

                        db.Parameters.Clear();
                        db.Parameters.Add("@projectId", projectId);
                        db.Parameters.Add("@reviewStage", reviewStage);
                        db.Parameters.Add("@reviewerEmail", reviewerEmail);
                        db.Parameters.Add("@reviewerName", reviewerName);
                        db.Parameters.Add("@token", token);

                        int reviewId = Convert.ToInt32(db.GetTable().Rows[0]["ReviewID"]);

                        // 4.2 為此審查委員建立所有評審項目記錄
                        foreach (DataRow template in templates.Rows)
                        {
                            string templateName = template["TemplateName"]?.ToString();
                            decimal templateWeight = template["TemplateWeight"] != DBNull.Value
                                ? Convert.ToDecimal(template["TemplateWeight"])
                                : 0;

                            if (!string.IsNullOrEmpty(templateName))
                            {
                                db.CommandText = @"
                                    INSERT INTO OFS_ReviewScores (
                                        ReviewID,
                                        ItemName,
                                        Weight,
                                        Score
                                    ) VALUES (
                                        @reviewId,
                                        @templateName,
                                        @templateWeight,
                                        NULL
                                    )";

                                db.Parameters.Clear();
                                db.Parameters.Add("@reviewId", reviewId);
                                db.Parameters.Add("@templateName", templateName);
                                db.Parameters.Add("@templateWeight", templateWeight);
                                db.ExecuteNonQuery();
                            }
                        }

                        System.Diagnostics.Debug.WriteLine($"已為專案 {projectId} 審查委員 {reviewerName} 建立完整審核記錄，包含 {templates.Rows.Count} 個評審項目");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"專案 {projectId} 領域 {field} 找到 {reviewers.Rows.Count} 位審查委員，已完成記錄新增");
            }
        }
    }

    #endregion

    /// <summary>
    /// 處理文化批次審核後的特殊流程
    /// </summary>
    public static void ProcessCulPostApproval(List<string> projectIds, string reviewStage, string actionType, string userAccount)
    {
        DbHelper db = new DbHelper();

        foreach (string projectId in projectIds)
        {
            // 1. 從 OFS_CUL_Project 取得 Field
            db.CommandText = "SELECT Field FROM OFS_CUL_Project WHERE ProjectID = @ProjectID";
            db.Parameters.Add("@ProjectID", projectId);
            string field = db.GetTable().Rows[0]["Field"]?.ToString();
            db.Parameters.Clear();
            int status = 5;
            switch (reviewStage)
            {
                case "初審":
                    reviewStage = "2";
                    break;
                case "複審":
                    reviewStage = "3";
                    break;
                
            }
            if (!string.IsNullOrEmpty(field))
            {
                // 2. 從 OFS_ReviewCommitteeList 取得對應審查組別的所有人員
                db.CommandText = @"
                    SELECT CommitteeUser, Email
                      FROM OFS_ReviewCommitteeList
                     WHERE SubjectTypeID = @SubjectTypeID
                ";

                db.Parameters.Add("@SubjectTypeID", field);
                DataTable reviewers = db.GetTable();
                db.Parameters.Clear();

                // 3. 取得文化的審查範本
                db.CommandText = $@"
                    SELECT TemplateName, TemplateWeight
                      FROM OFS_ReviewTemplate
                     WHERE SubsidyProjects = 'CUL'
                ";

                DataTable templates = db.GetTable();

                // 4. 為每位審查委員建立完整的審核記錄
                foreach (DataRow reviewer in reviewers.Rows)
                {
                    string reviewerEmail = reviewer["Email"]?.ToString();
                    string reviewerName = reviewer["CommitteeUser"]?.ToString();
                    
                    if (!string.IsNullOrEmpty(reviewerEmail) && !string.IsNullOrEmpty(reviewerName))
                    {
                        // 產生隨機 Token
                        string token = Guid.NewGuid().ToString();

                        // 4.1 新增記錄到 OFS_ReviewRecords
                        db.CommandText = @"
                            INSERT INTO OFS_ReviewRecords (
                                ProjectID,
                                ReviewStage,
                                Email,
                                ReviewerName,
                                Token
                            ) VALUES (
                                @projectId,
                                @reviewStage,
                                @reviewerEmail,
                                @reviewerName,
                                @token
                            );
                            SELECT SCOPE_IDENTITY() AS ReviewID;";

                        db.Parameters.Clear();
                        db.Parameters.Add("@projectId", projectId);
                        db.Parameters.Add("@reviewStage", reviewStage);
                        db.Parameters.Add("@reviewerEmail", reviewerEmail);
                        db.Parameters.Add("@reviewerName", reviewerName);
                        db.Parameters.Add("@token", token);

                        int reviewId = Convert.ToInt32(db.GetTable().Rows[0]["ReviewID"]);

                        // 4.2 為此審查委員建立所有評審項目記錄
                        foreach (DataRow template in templates.Rows)
                        {
                            string templateName = template["TemplateName"]?.ToString();
                            decimal templateWeight = template["TemplateWeight"] != DBNull.Value
                                ? Convert.ToDecimal(template["TemplateWeight"])
                                : 0;

                            if (!string.IsNullOrEmpty(templateName))
                            {
                                db.CommandText = @"
                                    INSERT INTO OFS_ReviewScores (
                                        ReviewID,
                                        ItemName,
                                        Weight,
                                        Score
                                    ) VALUES (
                                        @reviewId,
                                        @templateName,
                                        @templateWeight,
                                        NULL
                                    )";

                                db.Parameters.Clear();
                                db.Parameters.Add("@reviewId", reviewId);
                                db.Parameters.Add("@templateName", templateName);
                                db.Parameters.Add("@templateWeight", templateWeight);
                                db.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }
    }

    #region 計畫詳情查詢

    /// <summary>
    /// 取得科專計畫詳細資料
    /// </summary>
    /// <param name="projectId">專案編號</param>
    /// <returns>計畫詳細資料</returns>
    public static DataTable GetSciPlanDetail(string projectId)
    {
        using (var db = new DbHelper())
        {
            db.CommandText = @"
                SELECT TOP 1
                    [ProjectID],
                    [Year],
                    [SubsidyPlanType],
                    [ProjectNameTw],
                    (
                        SELECT Descname
                        FROM Sys_ZgsCode
                        WHERE Code = AM.[Topic]
                    ) + ' 、 ' +
                    (
                        SELECT Descname
                        FROM Sys_ZgsCode
                        WHERE Code = AM.[Field]
                    ) AS [TopicField],
                    [OrgName]
                FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_Application_Main] AM
                WHERE [ProjectID] = @projectId
            ";

            db.Parameters.Clear();
            db.Parameters.Add("@projectId", projectId);
            return db.GetTable();
        }
    }

    /// <summary>
    /// 取得文化計畫詳細資料
    /// </summary>
    /// <param name="projectId">專案編號</param>
    /// <returns>計畫詳細資料</returns>
    public static DataTable GetCulturalPlanDetail(string projectId)
    {
        using (var db = new DbHelper())
        {
            db.CommandText = @"
                SELECT [ProjectID],
                       [Year],
                       [SubsidyPlanType],
                       [ProjectName] AS [ProjectNameTw],
                       M.Descname + '-' + S.Descname AS [TopicField],
                       [OrgName]
                  FROM [OFS_CUL_Project] AS P
             LEFT JOIN [Sys_ZgsCode] AS S ON (S.CodeGroup = 'CULField' AND S.Code = P.Field)
             LEFT JOIN [Sys_ZgsCode] AS M ON (M.CodeGroup = 'CULField' AND M.Code = S.ParentCode)
                 WHERE [ProjectID] = @projectId
            ";

            db.Parameters.Clear();
            db.Parameters.Add("@projectId", projectId);
            return db.GetTable();
        }
    }

    /// <summary>
    /// 取得科專計畫評審意見
    /// </summary>
    /// <param name="projectId">專案編號</param>
    /// <param name="reviewStage">審查階段</param>
    /// <returns>評審意見資料</returns>
    public static DataTable GetSciReviewComments(string projectId, string reviewStage)
    {
        using (var db = new DbHelper())
        {
            db.CommandText = @"
                SELECT TOP (1000)
                    [ReviewID],
                    [ProjectID],
                    [ReviewStage],
                    [Email],
                    [ReviewerName],
                    [ReviewComment],
                    [ReplyComment],
                    [TotalScore],
                    [Token],
                    [CreateTime],
                    [IsSubmit]
                FROM [OCA_OceanSubsidy].[dbo].[OFS_ReviewRecords]
                WHERE ProjectID = @projectId AND ReviewStage = @reviewStage AND IsSubmit = 1
                ORDER BY ProjectID
            ";

            db.Parameters.Clear();
            db.Parameters.Add("@projectId", projectId);
            db.Parameters.Add("@reviewStage", reviewStage);
            return db.GetTable();
        }
    }

    /// <summary>
    /// 取得文化計畫評審意見
    /// </summary>
    /// <param name="projectId">專案編號</param>
    /// <param name="reviewStage">審查階段</param>
    /// <returns>評審意見資料</returns>
    public static DataTable GetCulturalReviewComments(string projectId, string reviewStage)
    {
        using (var db = new DbHelper())
        {
            db.CommandText = @"
                SELECT [ReviewID],
                       [ProjectID],
                       [ReviewStage],
                       [Email],
                       [ReviewerName],
                       [ReviewComment],
                       [ReplyComment],
                       [TotalScore],
                       [Token],
                       [CreateTime],
                       [IsSubmit]
                  FROM [OFS_ReviewRecords]
                 WHERE ProjectID = @projectId
                   AND ReviewStage = @reviewStage
                   AND IsSubmit = 1
              ORDER BY ProjectID
            ";

            db.Parameters.Clear();
            db.Parameters.Add("@projectId", projectId);
            db.Parameters.Add("@reviewStage", reviewStage);
            return db.GetTable();
        }
    }

    #endregion

    /// <summary>
    /// 取得待回覆的案件清單
    /// </summary>
    public static List<string> GetWaitReplyList()
    {
        List<string> projectIDs = new List<string>();

        DbHelper db = new DbHelper();

        try
        {
            db.CommandText = @"
                SELECT DISTINCT ProjectID
                FROM [OCA_OceanSubsidy].[dbo].[OFS_ReviewRecords]
                WHERE IsSubmit = 1
                AND (ReplyComment IS NULL OR ReplyComment = '')";

            DataTable dt = db.GetTable();

            foreach (DataRow row in dt.Rows)
            {
                string ProjectID = row["ProjectID"]?.ToString();
                if (!string.IsNullOrEmpty(ProjectID))
                {
                    projectIDs.Add(ProjectID);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"搜尋關鍵字時發生錯誤：{ex.Message}");
        }
        finally
        {
            db.Dispose();
        }

        return projectIDs;
    }

    #region Type=5 計畫變更審核查詢方法

    /// <summary>
    /// 查詢計畫變更審核清單 (Type=5)
    /// </summary>
    /// <param name="year">年度</param>
    /// <param name="category">類別</param>
    /// <param name="orgName">申請單位</param>
    /// <param name="supervisoryUnit">主管單位</param>
    /// <param name="keyword">計畫編號或名稱關鍵字</param>
    /// <returns>計畫變更審核項目清單</returns>
    public static List<PlanChangeReviewItem> Search_Type5_PlanChangeReview(
        string year = "",
        string category = "",
        string orgName = "",
        string supervisoryUnit = "",
        string keyword = "")
    {
        List<PlanChangeReviewItem> results = new List<PlanChangeReviewItem>();
        var db = new DbHelper();

        try
        {
            // 建立基礎查詢語句
            db.CommandText = @"
                SELECT
                    [Year],
                    [ProjectID],
                    [Category],
                    [ProjectNameTw],
                    [OrgName],
                    [SupervisoryUnit]
                FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type5]
                WHERE 1=1";

            // 清除參數
            db.Parameters.Clear();

            // 動態新增查詢條件
            if (!string.IsNullOrEmpty(year))
            {
                db.CommandText += " AND [Year] = @Year";
                db.Parameters.Add("@Year", year);
            }

            if (!string.IsNullOrEmpty(category))
            {
                db.CommandText += " AND [Category] = @Category";
                db.Parameters.Add("@Category", category);
            }

            if (!string.IsNullOrEmpty(orgName))
            {
                db.CommandText += " AND [OrgName] LIKE @OrgName";
                db.Parameters.Add("@OrgName", $"%{orgName}%");
            }

            if (!string.IsNullOrEmpty(supervisoryUnit))
            {
                db.CommandText += " AND [SupervisoryUnit] LIKE @SupervisoryUnit";
                db.Parameters.Add("@SupervisoryUnit", $"%{supervisoryUnit}%");
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                db.CommandText += " AND ([ProjectID] LIKE @Keyword OR [ProjectNameTw] LIKE @Keyword)";
                db.Parameters.Add("@Keyword", $"%{keyword}%");
            }

            // 排序
            db.CommandText += " ORDER BY [Year] DESC, [ProjectID] ASC";

            // 執行查詢
            DataTable dt = db.GetTable();

            // 處理查詢結果
            foreach (DataRow row in dt.Rows)
            {
                var item = new PlanChangeReviewItem
                {
                    Year = row["Year"]?.ToString() ?? "",
                    ProjectID = row["ProjectID"]?.ToString() ?? "",
                    Category = row["Category"]?.ToString() ?? "",
                    ProjectNameTw = row["ProjectNameTw"]?.ToString() ?? "",
                    OrgName = row["OrgName"]?.ToString() ?? "",
                    SupervisoryUnit = row["SupervisoryUnit"]?.ToString() ?? ""
                };

                results.Add(item);
            }

            System.Diagnostics.Debug.WriteLine($"Type5 查詢完成，共找到 {results.Count} 筆資料");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Search_Type5_PlanChangeReview 查詢時發生錯誤：{ex.Message}");
            throw new Exception($"Search_Type5_PlanChangeReview 查詢時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }

        return results;
    }

    /// <summary>
    /// 搜尋 Type=5 計畫變更審核資料（支援分頁）
    /// </summary>
    /// <param name="year">年度</param>
    /// <param name="category">類別</param>
    /// <param name="orgName">申請單位</param>
    /// <param name="supervisoryUnit">主管單位</param>
    /// <param name="keyword">關鍵字</param>
    /// <param name="pageNumber">頁碼</param>
    /// <param name="pageSize">每頁筆數</param>
    /// <returns>分頁結果</returns>
    public static PaginatedResult<PlanChangeReviewItem> Search_Type5_PlanChangeReview_Paged(
        string year = "",
        string category = "",
        string orgName = "",
        string supervisoryUnit = "",
        string keyword = "",
        int pageNumber = 1,
        int pageSize = 10)
    {
        // 取得所有資料
        var allData = Search_Type5_PlanChangeReview(year, category, orgName, supervisoryUnit, keyword);

        // 計算分頁資料
        int totalRecords = allData.Count;
        int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

        var pagedData = allData
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PaginatedResult<PlanChangeReviewItem>
        {
            Data = pagedData,
            TotalRecords = totalRecords,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }

    /// <summary>
    /// 搜尋 Type=6 執行計畫審核資料
    /// </summary>
    /// <param name="year">年度</param>
    /// <param name="category">類別</param>
    /// <param name="orgName">申請單位</param>
    /// <param name="supervisoryUnit">主管單位</param>
    /// <param name="keyword">關鍵字</param>
    /// <returns>執行計畫審核項目清單</returns>
    public static List<ExecutionPlanReviewItem> Search_Type6_ExecutionPlanReview(
        string year = "",
        string category = "",
        string orgName = "",
        string supervisoryUnit = "",
        string keyword = "")
    {
        List<ExecutionPlanReviewItem> results = new List<ExecutionPlanReviewItem>();
        var db = new DbHelper();

        try
        {
            // 建立基礎查詢語句
            db.CommandText = @"
                SELECT TOP (1000)
                    [Year],
                    [ProjectID],
                    [Category],
                    [Stage],
                    [ReviewTodo],
                    [SupervisoryPersonAccount],
                    [SupervisoryUnit],
                    [OrgName],
                    [ProjectNameTw]
                FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type6]
                WHERE 1=1";

            // 清除參數
            db.Parameters.Clear();

            // 動態新增查詢條件
            if (!string.IsNullOrEmpty(year))
            {
                db.CommandText += " AND [Year] = @Year";
                db.Parameters.Add("@Year", year);
            }

            if (!string.IsNullOrEmpty(category))
            {
                db.CommandText += " AND [Category] = @Category";
                db.Parameters.Add("@Category", category);
            }

            if (!string.IsNullOrEmpty(orgName))
            {
                db.CommandText += " AND [OrgName] LIKE @OrgName";
                db.Parameters.Add("@OrgName", "%" + orgName + "%");
            }

            if (!string.IsNullOrEmpty(supervisoryUnit))
            {
                db.CommandText += " AND [SupervisoryUnit] LIKE @SupervisoryUnit";
                db.Parameters.Add("@SupervisoryUnit", "%" + supervisoryUnit + "%");
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                db.CommandText += " AND ([ProjectID] LIKE @Keyword OR [ProjectNameTw] LIKE @Keyword OR [OrgName] LIKE @Keyword)";
                db.Parameters.Add("@Keyword", "%" + keyword + "%");
            }

            // 排序
            db.CommandText += " ORDER BY [Year] DESC, [ProjectID] ASC";

            // 執行查詢
            DataTable dt = db.GetTable();

            // 轉換資料
            foreach (DataRow row in dt.Rows)
            {
                var projectId = row["ProjectID"]?.ToString() ?? "";
                var rowCategory = row["Category"]?.ToString() ?? "";

                var item = new ExecutionPlanReviewItem
                {
                    Year = row["Year"]?.ToString() ?? "",
                    ProjectID = projectId,
                    Category = rowCategory,
                    Stage = row["Stage"]?.ToString() ?? "",
                    ReviewTodo = row["ReviewTodo"]?.ToString() ?? "",
                    SupervisoryPersonAccount = row["SupervisoryPersonAccount"]?.ToString() ?? "",
                    SupervisoryUnit = row["SupervisoryUnit"]?.ToString() ?? "",
                    OrgName = row["OrgName"]?.ToString() ?? "",
                    ProjectNameTw = row["ProjectNameTw"]?.ToString() ?? ""
                };

                // 僅針對科專專案加入審查委員進度
                if (rowCategory == "SCI")
                {
                    item.ReviewProgress = GetSciReviewProgress(projectId);
                }

                results.Add(item);
            }

            System.Diagnostics.Debug.WriteLine($"Search_Type6_ExecutionPlanReview 共查詢到 {results.Count} 筆資料");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Search_Type6_ExecutionPlanReview 查詢時發生錯誤：{ex.Message}");
            throw new Exception($"Search_Type6_ExecutionPlanReview 查詢時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }

        return results;
    }

    // /// <summary>
    // /// 搜尋 Type=6 執行計畫審核資料（支援分頁）
    // /// </summary>
    // /// <param name="year">年度</param>
    // /// <param name="category">類別</param>
    // /// <param name="orgName">申請單位</param>
    // /// <param name="supervisoryUnit">主管單位</param>
    // /// <param name="keyword">關鍵字</param>
    // /// <param name="pageNumber">頁碼</param>
    // /// <param name="pageSize">每頁筆數</param>
    // /// <returns>分頁結果</returns>
    // public static PaginatedResult<ExecutionPlanReviewItem> Search_Type6_ExecutionPlanReview_Paged(
    //     string year = "",
    //     string category = "",
    //     string orgName = "",
    //     string supervisoryUnit = "",
    //     string keyword = "",
    //     int pageNumber = 1,
    //     int pageSize = 10)
    // {
    //     // 取得所有資料
    //     var allData = Search_Type6_ExecutionPlanReview(year, category, orgName, supervisoryUnit, keyword);
    //
    //     // 計算分頁資料
    //     int totalRecords = allData.Count;
    //     int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
    //
    //     var pagedData = allData
    //         .Skip((pageNumber - 1) * pageSize)
    //         .Take(pageSize)
    //         .ToList();
    //
    //     return new PaginatedResult<ExecutionPlanReviewItem>
    //     {
    //         Data = pagedData,
    //         TotalRecords = totalRecords,
    //         PageNumber = pageNumber,
    //         PageSize = pageSize,
    //         TotalPages = totalPages
    //     };
    // }

    /// <summary>
    /// 取得科專專案的審查委員進度
    /// </summary>
    /// <param name="projectId">專案編號</param>
    /// <returns>審查委員進度字串，格式：已繳交人數/總人數 狀態</returns>
    public static string GetSciReviewProgress(string projectId)
    {
        if (string.IsNullOrEmpty(projectId) || !projectId.Contains("SCI"))
        {
            return null;
        }

        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
                SELECT
                    CAST(SUM(CASE WHEN SR.isSubmit = 1 THEN 1 ELSE 0 END) AS VARCHAR(10)) + '/' +
                    CAST(COUNT(*) AS VARCHAR(10)) + ' ' +
                    CASE
                        WHEN SUM(CASE WHEN SR.isSubmit = 1 THEN 1 ELSE 0 END) < COUNT(*)
                            THEN N'未完成'
                        ELSE N'完成'
                    END AS ReviewProgress
                FROM dbo.OFS_SCI_StageExam SE
                INNER JOIN dbo.OFS_SCI_StageExam_ReviewerList SR
                    ON SE.id = SR.ExamID
                WHERE SE.Status = '審核中'
                  AND SE.ProjectID = @ProjectID
                GROUP BY SE.ProjectID";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectId);

            DataTable dt = db.GetTable();

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["ReviewProgress"]?.ToString();
            }

            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得科專審查委員進度時發生錯誤：{ex.Message}");
            return null;
        }
        finally
        {
            db.Dispose();
        }
    }

    #endregion

    #region 待辦事項管理
   
    /// <summary>
    /// 為專案建立預設的待辦事項模板
    /// </summary>
    /// <param name="projectId">專案編號</param>
    /// <returns>是否成功建立</returns>
    public static void CreateTaskQueueTemplate(string projectId)
    {
        try
        {
            
            List<TaskTemplate> taskTemplates = new List<TaskTemplate>();

            if (projectId.Contains("SCI"))
            {
                taskTemplates = new List<TaskTemplate>
                {
                    new TaskTemplate("Contract", "簽訂契約資料", 1, true, false),
                    new TaskTemplate("Payment1", "第一次請款(預撥)", 2, true, false),
                    new TaskTemplate("Change", "計畫變更", 3, false, false),
                    new TaskTemplate("Schedule", "填寫預定進度", 4, true, false),
                    new TaskTemplate("MidReport", "填寫期中報告", 5, false, false),
                    new TaskTemplate("FinalReport", "填寫期末報告", 6, false, false),
                    new TaskTemplate("MonthlyReport", "填寫每月進度報告", 7, false, false),
                    new TaskTemplate("Payment2", "第二期請款", 8, false, false)
                };
            }
            else if (projectId.Contains("CLB"))
            {
                taskTemplates = new List<TaskTemplate>
                {
                    new TaskTemplate("Change", "計畫變更", 1, false, false),
                    new TaskTemplate("Report", "成果報告", 2, false, false),
                    new TaskTemplate("Payment", "請款", 3, false, false)
                };
            }


            // TODO 正文 設計待辦的項目

            // 批次插入待辦事項
            BatchInsertTaskQueue(projectId, taskTemplates);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"建立待辦事項模板時發生錯誤：{ex.Message}");
        }
    }



    /// <summary>
    /// 批次插入待辦事項
    /// </summary>
    /// <param name="projectId">專案編號</param>
    /// <param name="taskTemplates">待辦事項模板列表</param>
    /// <returns>是否成功插入</returns>
    private static void BatchInsertTaskQueue(string projectId, List<TaskTemplate> taskTemplates)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                foreach (var template in taskTemplates)
                {
                    // 使用參數化查詢預防 SQL 注入攻擊
                    db.Parameters.Clear();

                    db.CommandText = @"
                        INSERT INTO OFS_TaskQueue (ProjectID, TaskNameEn, TaskName, PriorityLevel, IsTodo, IsCompleted)
                        VALUES (@ProjectID, @TaskNameEn, @TaskName, @PriorityLevel, @IsTodo, @IsCompleted)";
                    
                    // 加入參數並防止 SQL 注入
                    db.Parameters.Add("@ProjectID", projectId);
                    db.Parameters.Add("@TaskNameEn", template.TaskNameEn);
                    db.Parameters.Add("@TaskName", template.TaskName);
                    db.Parameters.Add("@PriorityLevel", template.PriorityLevel);
                    db.Parameters.Add("@IsTodo", template.IsTodo ? 1 : 0);
                    db.Parameters.Add("@IsCompleted", template.IsCompleted ? 1 : 0);
                    db.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"批次插入待辦事項時發生錯誤：{ex.Message}");
            }
        }
    }

    /// <summary>
    /// 取得專案的待辦事項列表
    /// </summary>
    /// <param name="projectId">專案編號</param>
    /// <returns>待辦事項列表</returns>
    public static List<OFS_TaskQueue> GetTaskQueueByProjectId(string projectId)
    {
        var taskList = new List<OFS_TaskQueue>();

        using (DbHelper db = new DbHelper())
        {
            try
            {
                // 使用參數化查詢預防 SQL 注入攻擊
                db.CommandText = @"
                    SELECT ProjectID, TaskName, PriorityLevel, IsTodo, IsCompleted
                    FROM OFS_TaskQueue
                    WHERE ProjectID = @ProjectID
                    ORDER BY PriorityLevel";
                
                db.Parameters.Add("@ProjectID", projectId);

                DataTable dt = db.GetTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        var task = new OFS_TaskQueue
                        {
                            ProjectID = row["ProjectID"]?.ToString(),
                            TaskName = row["TaskName"]?.ToString(),
                            PriorityLevel = row["PriorityLevel"] as int?,
                            IsTodo = Convert.ToBoolean(row["IsTodo"]),
                            IsCompleted = Convert.ToBoolean(row["IsCompleted"])
                        };
                        taskList.Add(task);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"取得待辦事項時發生錯誤：{ex.Message}");
            }
        }

        return taskList;
    }

    /// <summary>
    /// 更新待辦事項狀態
    /// </summary>
    /// <param name="projectId">專案編號</param>
    /// <param name="taskName">任務名稱</param>
    /// <param name="isCompleted">是否完成</param>
    /// <returns>是否成功更新</returns>
    public static void UpdateTaskQueueStatus(string projectId, string taskName, bool isCompleted)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                db.CommandText = @"
                    UPDATE OFS_TaskQueue
                    SET IsCompleted = @isCompleted
                    WHERE ProjectID = @projectId AND TaskName = @taskName";

                db.Parameters.Clear();
                db.Parameters.Add("@isCompleted", isCompleted ? 1 : 0);
                db.Parameters.Add("@projectId", projectId);
                db.Parameters.Add("@taskName", taskName);
                db.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"更新待辦事項狀態時發生錯誤：{ex.Message}");
            }
        }
    }

    #endregion

    #region 審查排名功能

    /// <summary>
    /// 取得審查結果排名資料
    /// </summary>
    /// <param name="reviewType">審查類型 (2: 領域審查, 3: 技術審查)</param>
    /// <param name="reviewGroup">審查組別 (如: Information, Environment 等)</param>
    /// <returns>排名資料清單</returns>
    public static List<ReviewRankingItem> GetReviewRanking(string reviewType, string reviewGroup = null)
    {
        var results = new List<ReviewRankingItem>();
        
        try
        {
            // 驗證審查類型
            if (reviewType != "2" && reviewType != "3")
            {
                throw new ArgumentException("不支援的審查類型");
            }
            List<string> SciList = new List<string> { "Information", "Environment", "Material", "Mechanical" };
            string reviewStage = "";

            if (SciList.Contains(reviewGroup))
            {
                reviewStage = reviewType == "2" ? "領域審查" : "技術審查";

            }
            else
            {
                reviewStage = reviewType == "2" ? "2" : "3"; //初審、複審
            }
            // 根據審查類型設定審查階段
            using (var db = new DbHelper())
            {
                // 執行SQL查詢 - 使用參數化查詢防止SQL注入
                // 查詢特定審查組別
                if(reviewStage == "領域審查" || reviewStage == "技術審查")
                {
                    db.CommandText = @"
                    -- 先算每個專案的平均分與總分
                    WITH ProjectScores AS (
                        SELECT 
                            PM.ProjectID,
                            ProjectNameTw,
                            SUM(TotalScore) AS ProjectTotalScore,
                            AVG(TotalScore) AS AvgScore
                        FROM OFS_ReviewRecords RR
                        LEFT JOIN OFS_SCI_Application_Main AM ON RR.ProjectID = AM.ProjectID
                        LEFT JOIN OFS_SCI_Project_Main PM ON PM.ProjectID = AM.ProjectID
                        WHERE PM.Statuses = @ReviewStage 
                          AND RR.ReviewStage = @ReviewStage
                          AND IsSubmit = 1 AND PM.isExist != 0 
                          AND AM.Field = @ReviewGroup
                        GROUP BY PM.ProjectID, ProjectNameTw
                    ),
                    -- 每個委員對各專案的分數
                    ReviewerScores AS (
                        SELECT 
                            RR.ProjectID,
                            RR.ReviewerName,
                            RR.TotalScore
                            -- RANK() OVER (PARTITION BY RR.ReviewerName ORDER BY RR.TotalScore DESC) AS ReviewerRank
                        FROM OFS_ReviewRecords RR
                        LEFT JOIN OFS_SCI_Application_Main AM ON RR.ProjectID = AM.ProjectID
                        LEFT JOIN OFS_SCI_Project_Main PM ON PM.ProjectID = AM.ProjectID
                        WHERE PM.Statuses = @ReviewStage 
                          AND RR.ReviewStage = @ReviewStage
                          AND IsSubmit = 1 AND PM.isExist != 0 
                          AND AM.Field = @ReviewGroup
                    )
                    -- 最後組合成一張大表
                    SELECT 
                        PS.ProjectID,
                        PS.ProjectNameTw,
                        PS.ProjectTotalScore,
                        PS.AvgScore,
                        DENSE_RANK() OVER (ORDER BY PS.AvgScore DESC) AS DenseRankNo,
                        RS.ReviewerName,
                        RS.TotalScore
                        -- RS.ReviewerRank
                    FROM ProjectScores PS
                    JOIN ReviewerScores RS ON PS.ProjectID = RS.ProjectID
                    ORDER BY DenseRankNo, PS.ProjectID, RS.ReviewerName;
                ";
                  
                }
                else//初審、複審
                {
                    db.CommandText = @"
                WITH ProjectScores AS (

		            SELECT 
                            PM.ProjectID,
                            ProjectName ,
                            SUM(TotalScore) AS ProjectTotalScore,
                            AVG(TotalScore) AS AvgScore
                        FROM OFS_ReviewRecords RR
                        LEFT JOIN OFS_CUL_Project PM ON PM.ProjectID = RR.ProjectID
                        WHERE PM.ProgressStatus = @ReviewStage
                          AND RR.ReviewStage = @ReviewStage
                          AND IsSubmit = 1 AND PM.IsExists != 0 
                          AND PM.Field =@ReviewGroup
                        GROUP BY PM.ProjectID, ProjectName
                    ),
                    -- 每個委員對各專案的分數
                    ReviewerScores AS (
                        SELECT 
                            RR.ProjectID,
                            RR.ReviewerName,
                            RR.TotalScore
                        FROM OFS_ReviewRecords RR
                        LEFT JOIN OFS_CUL_Project PM ON PM.ProjectID = RR.ProjectID
                        WHERE PM.ProgressStatus = @ReviewStage 
                          AND RR.ReviewStage = @ReviewStage
                          AND IsSubmit = 1 AND PM.IsExists != 0 
                          AND PM.Field = @ReviewGroup
                    )
                    -- 最後組合成一張大表
                    SELECT 
                        PS.ProjectID,
                        PS.ProjectName as ProjectNameTw,
                        PS.ProjectTotalScore,
                        PS.AvgScore,
                        DENSE_RANK() OVER (ORDER BY PS.AvgScore DESC) AS DenseRankNo,
                        RS.ReviewerName,
                        RS.TotalScore
                        -- RS.ReviewerRank
                    FROM ProjectScores PS
                    JOIN ReviewerScores RS ON PS.ProjectID = RS.ProjectID
                    ORDER BY DenseRankNo, PS.ProjectID, RS.ReviewerName;";
                }
                

                db.Parameters.Add("@ReviewStage", reviewStage);
                db.Parameters.Add("@ReviewGroup", reviewGroup);
            
                
                var dataTable = db.GetTable();
                
                // 將查詢結果轉換為排名物件
                var rankingData = new Dictionary<string, ReviewRankingItem>();
                
                foreach (DataRow row in dataTable.Rows)
                {
                    string projectId = row["ProjectID"].ToString();
                    
                    if (!rankingData.ContainsKey(projectId))
                    {
                        rankingData[projectId] = new ReviewRankingItem
                        {
                            ProjectID = projectId,
                            ProjectNameTw = row["ProjectNameTw"].ToString(),
                            ProjectTotalScore = Convert.ToDecimal(row["ProjectTotalScore"]),
                            AvgScore = Convert.ToDecimal(row["AvgScore"]),
                            DenseRankNo = Convert.ToInt32(row["DenseRankNo"]),
                            ReviewerScores = new List<ReviewerScore>()
                        };
                    }
                    
                    rankingData[projectId].ReviewerScores.Add(new ReviewerScore
                    {
                        ReviewerName = row["ReviewerName"].ToString(),
                        TotalScore = Convert.ToDecimal(row["TotalScore"])
                    });
                }
                
                results = rankingData.Values.OrderBy(x => x.DenseRankNo).ToList();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得審查排名時發生錯誤: {ex.Message}");
            throw new Exception($"載入排名資料時發生錯誤: {ex.Message}");
        }
        
        return results;
    }

    /// <summary>
    /// 取得審查組別選項
    /// </summary>
    /// <param name="reviewType">審查類型</param>
    /// <returns>審查組別選項清單</returns>
    public static List<DropdownItem> GetReviewGroupOptions(string reviewType)
    {
        var options = new List<DropdownItem>();
        
        try
        {
            
            // 根據審查類型提供不同的組別選項
            if (reviewType == "2" || reviewType == "3")
            {
                // 領域審查和技術審查的組別
                options.Add(new DropdownItem { Value = "Information", Text = "資通訊" });
                options.Add(new DropdownItem { Value = "Environment", Text = "環境工程" });
                options.Add(new DropdownItem { Value = "Material", Text = "材料科技" });
                options.Add(new DropdownItem { Value = "Mechanical", Text = "機械與機電工程" });
       
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得審查組別選項時發生錯誤: {ex.Message}");
        }
        
        return options;
    }

    #endregion

    #region 匯出資料查詢方法

    /// <summary>
    /// 取得Type1審查中資料 (匯出用)
    /// </summary>
    /// <param name="year">年度</param>
    /// <param name="category">類別</param>
    /// <param name="status">狀態</param>
    /// <param name="orgName">申請單位</param>
    /// <param name="supervisor">承辦人員</param>
    /// <param name="keyword">關鍵字</param>
    /// <returns>審查中資料DataTable</returns>
    public static DataTable GetType1ReviewingData(string year, string category, string status, string orgName, string supervisor, string keyword)
    {
        try
        {
            using (var db = new DbHelper())
            {
                // 使用原始的SQL語句並加入篩選條件
                db.CommandText = @"
                    SELECT Year as '年度'
                          ,RC1.[ProjectID] as '計畫編號'
                          ,CASE
                               WHEN [Category] = 'SCI' THEN '科專'
                               WHEN [Category] = 'CUL' THEN '文化'
                               WHEN [Category] = 'EDC' THEN '學校/民間'
                               WHEN [Category] = 'CLB' THEN '學校社團'
                               WHEN [Category] = 'MUL' THEN '多元'
                               WHEN [Category] = 'LIT' THEN '素養'
                               WHEN [Category] = 'ACC' THEN '無障礙'
                               ELSE [Category]
                           END as '類別'
                          ,[ProjectNameTw] as '計畫名稱'
                          ,[OrgName] as '申請單位'
                          ,[Req_SubsidyAmount] as '申請經費'
                          ,[Req_TotalAmount] as '總經費'
                          ,[StatusesName] as '狀態'
                          ,EW.WorkNames as '工作項目(大項)'
                    FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type1] RC1
                    LEFT JOIN V_Excel_Worksch EW ON RC1.ProjectID = EW.ProjectID
                    WHERE 1=1";

                // 加入篩選條件
                if (!string.IsNullOrEmpty(year))
                {
                    db.CommandText += " AND Year = @Year";
                    db.Parameters.Add("@Year", year);
                }

                if (!string.IsNullOrEmpty(category))
                {
                    db.CommandText += " AND Category = @Category";
                    db.Parameters.Add("@Category", category);
                }

                if (!string.IsNullOrEmpty(status))
                {
                    db.CommandText += " AND StatusesName = @StatusesName";
                    db.Parameters.Add("@StatusesName", status);
                }

                if (!string.IsNullOrEmpty(orgName))
                {
                    db.CommandText += " AND OrgName = @OrgName";
                    db.Parameters.Add("@OrgName", orgName);
                }

                if (!string.IsNullOrEmpty(supervisor))
                {
                    db.CommandText += " AND SupervisoryPersonAccount = @SupervisoryPersonAccount";
                    db.Parameters.Add("@SupervisoryPersonAccount", supervisor);
                }

                if (!string.IsNullOrEmpty(keyword))
                {
                    db.CommandText += " AND (RC1.ProjectID LIKE @Keyword OR ProjectNameTw LIKE @Keyword)";
                    db.Parameters.Add("@Keyword", "%" + keyword + "%");
                }

                db.CommandText += " ORDER BY Year DESC, RC1.ProjectID";

                var dataTable = db.GetTable();
                db.Parameters.Clear();

                return dataTable;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得Type1審查中資料時發生錯誤: {ex.Message}");
            throw new Exception($"取得匯出資料時發生錯誤: {ex.Message}");
        }
    }

    /// <summary>
    /// 取得Type1審查結果資料 (匯出用) - 僅查詢通過和不通過的資料
    /// </summary>
    public static DataTable GetType1ReviewResultsData(string year, string category, string status, string orgName, string supervisor, string keyword)
    {
        try
        {
            using (var db = new DbHelper())
            {
                // 基於提供的SQL語句，查詢審查結果（通過和不通過）
                db.CommandText = @"
                    SELECT Year as '年度'
                          ,RC1.[ProjectID] as '計畫編號'
                          ,CASE
                               WHEN [Category] = 'SCI' THEN '科專'
                               WHEN [Category] = 'CUL' THEN '文化'
                               WHEN [Category] = 'EDC' THEN '學校/民間'
                               WHEN [Category] = 'CLB' THEN '學校社團'
                               WHEN [Category] = 'MUL' THEN '多元'
                               WHEN [Category] = 'LIT' THEN '素養'
                               WHEN [Category] = 'ACC' THEN '無障礙'
                               ELSE [Category]
                           END as '類別'
                          ,[ProjectNameTw] as '計畫名稱'
                          ,[OrgName] as '申請單位'
                          ,[Req_SubsidyAmount] as '申請經費'
                          ,[Req_TotalAmount] as '總經費'
                          ,[StatusesName] as '狀態'
                          ,QualReviewNotes as '審查意見'
                    FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type1] RC1
                    WHERE 1=1
                    AND (StatusesName LIKE @PassStatus OR StatusesName LIKE @FailStatus)";

                // 固定查詢通過和不通過的資料
                db.Parameters.Add("@PassStatus", "通過");
                db.Parameters.Add("@FailStatus", "不通過");

                // 加入其他篩選條件
                if (!string.IsNullOrEmpty(year))
                {
                    db.CommandText += " AND Year = @Year";
                    db.Parameters.Add("@Year", year);
                }

                if (!string.IsNullOrEmpty(category))
                {
                    db.CommandText += " AND Category = @Category";
                    db.Parameters.Add("@Category", category);
                }

                if (!string.IsNullOrEmpty(orgName))
                {
                    db.CommandText += " AND OrgName = @OrgName";
                    db.Parameters.Add("@OrgName", orgName);
                }

                if (!string.IsNullOrEmpty(supervisor))
                {
                    db.CommandText += " AND SupervisoryPersonAccount = @SupervisoryPersonAccount";
                    db.Parameters.Add("@SupervisoryPersonAccount", supervisor);
                }

                if (!string.IsNullOrEmpty(keyword))
                {
                    db.CommandText += " AND (RC1.ProjectID LIKE @Keyword OR ProjectNameTw LIKE @Keyword)";
                    db.Parameters.Add("@Keyword", "%" + keyword + "%");
                }

                db.CommandText += " ORDER BY Year DESC, RC1.ProjectID";

                var dataTable = db.GetTable();
                db.Parameters.Clear();

                return dataTable;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得Type1審查結果資料時發生錯誤: {ex.Message}");
            throw new Exception($"取得審查結果資料時發生錯誤: {ex.Message}");
        }
    }

    /// <summary>
    /// 取得Type2申請資料 (匯出用) - 回傳 ProjectID 和 PDF 路徑供 ZIP 匯出使用
    /// </summary>
    public static DataTable GetType2ApplicationData(string year, string category, string status, string orgName, string supervisor, string keyword, string progress = "", string replyStatus = "")
    {
        try
        {
            DataTable result = new DataTable();
            result.Columns.Add("ProjectID", typeof(string));
            result.Columns.Add("PdfPath", typeof(string));
            result.Columns.Add("Category", typeof(string));
            result.Columns.Add("ProjectName", typeof(string));

            // 根據類別執行不同的查詢
            if (category == "SCI" || string.IsNullOrEmpty(category))
            {
                // 取得 SCI 科專資料
                var sciResults = GetSciBasicData(year, orgName, supervisor, keyword, "領域審查");

                // 篩選符合進度和回覆狀態的 ProjectID
                var filteredSciResults = FilterByProgressAndReplyStatus(sciResults, progress, replyStatus, "SCI");

                foreach (var item in filteredSciResults)
                {
                    string projectId = item.ProjectID ?? "";
                    var applicationMain = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectId);
                    string ProjectName = applicationMain.ProjectNameTw ?? "";

                    if (!string.IsNullOrEmpty(projectId))
                    {
                        // 建構 SCI PDF 路徑
                        string pdfPath = HttpContext.Current.Server.MapPath($"~/UploadFiles/OFS/SCI/{projectId}/SciApplication/{projectId}_科專_{ProjectName}_送審版.pdf");

                        DataRow row = result.NewRow();
                        row["ProjectID"] = projectId;
                        row["PdfPath"] = pdfPath;
                        row["Category"] = "SCI";
                        row["ProjectName"] = item.ProjectNameTw ?? "";
                        result.Rows.Add(row);
                    }
                }
            }

            if (category == "CUL" || string.IsNullOrEmpty(category))
            {
                // 取得 CUL 文化資料
                var culResults = GetCulBasicData(year, orgName, supervisor, keyword, "初審");
                
                // 篩選符合進度和回覆狀態的 ProjectID
                var filteredCulResults = FilterByProgressAndReplyStatus(culResults, progress, replyStatus, "CUL");
            
                foreach (var item in filteredCulResults)
                {
                    string projectId = item.ProjectID ?? "";
                    string ProjectName = item.ProjectNameTw;
                    if (!string.IsNullOrEmpty(projectId))
                    {
                        string pdfPath = HttpContext.Current.Server.MapPath($"~/UploadFiles/OFS/CUL/{projectId}/TechReviewFiles/{projectId}_文化_{ProjectName}_送審版.pdf");

            
                        DataRow row = result.NewRow();
                        row["ProjectID"] = projectId;
                        row["PdfPath"] = pdfPath;
                        row["Category"] = "CUL";
                        row["ProjectName"] = item.ProjectNameTw ?? "";
                        result.Rows.Add(row);
                    }
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetType2ApplicationData 查詢時發生錯誤：{ex.Message}");
            throw new Exception($"GetType2ApplicationData 查詢時發生錯誤：{ex.Message}", ex);
        }
    }

    /// <summary>
    /// 根據審查進度和回覆狀態篩選專案
    /// </summary>
    private static List<ReviewChecklistItem> FilterByProgressAndReplyStatus(List<ReviewChecklistItem> items, string progress, string replyStatus, string category)
    {
        // 如果沒有指定篩選條件，直接回傳原始資料
        if (string.IsNullOrEmpty(progress) && string.IsNullOrEmpty(replyStatus))
        {
            return items;
        }

        // 取得所有 ProjectID
        var projectIds = items.Select(x => x.ProjectID).Where(x => !string.IsNullOrEmpty(x)).ToList();
        if (projectIds.Count == 0) return new List<ReviewChecklistItem>();

        // 取得進度資料
        List<ProgressData> progressDataList;
        if (category == "SCI")
        {
            progressDataList = GetSciProgressData(projectIds,"領域審查");
        }
        else if (category == "CUL")
        {
            progressDataList = GetCulProgressData(projectIds, "初審");
        }
        else
        {
            return items; // 其他類別暫不支援
        }

        // 建立 ProjectID 到進度資料的映射
        var progressDict = progressDataList.ToDictionary(x => x.ProjectID, x => x);

        // 篩選符合條件的專案
        var filteredItems = new List<ReviewChecklistItem>();

        foreach (var item in items)
        {
            bool shouldInclude = true;

            if (progressDict.TryGetValue(item.ProjectID, out var progressData))
            {
                // 篩選審查進度
                if (!string.IsNullOrEmpty(progress))
                {
                    if (progress == "完成" && progressData.ReviewProgress != "完成")
                    {
                        shouldInclude = false;
                    }
                    else if (progress == "未完成" && progressData.ReviewProgress != "未完成")
                    {
                        shouldInclude = false;
                    }
                }

                // 篩選回覆狀態
                if (!string.IsNullOrEmpty(replyStatus) && shouldInclude)
                {
                    if (replyStatus == "完成" && progressData.ReplyProgress != "完成")
                    {
                        shouldInclude = false;
                    }
                    else if (replyStatus == "未完成" && progressData.ReplyProgress != "未完成")
                    {
                        shouldInclude = false;
                    }
                }
            }
            else
            {
                // 如果找不到進度資料則不進入
                shouldInclude = false;
            }

            if (shouldInclude)
            {
                filteredItems.Add(item);
            }
        }

        return filteredItems;
    }

    /// <summary>
    /// 取得Type2審查結果資料 (匯出用) - 預留給未來使用
    /// </summary>
    public static DataTable GetType2ReviewResultsData(string year, string category, string status, string orgName, string supervisor, string keyword)
    {
        // TODO: 實作Type2審查結果資料查詢
        throw new NotImplementedException("Type2 審查結果匯出功能尚未實作");
    }

    /// <summary>
    /// 取得Type3申請資料 (匯出用) - 預留給未來使用
    /// </summary>
    public static DataTable GetType3ApplicationData(string year, string category, string status, string orgName, string supervisor, string keyword)
    {
        // TODO: 實作Type3申請資料查詢
        throw new NotImplementedException("Type3 申請資料匯出功能尚未實作");
    }

    /// <summary>
    /// 取得Type3審查結果資料 (匯出用) - 預留給未來使用
    /// </summary>
    public static DataTable GetType3ReviewResultsData(string year, string category, string status, string orgName, string supervisor, string keyword)
    {
        // TODO: 實作Type3審查結果資料查詢
        throw new NotImplementedException("Type3 審查結果匯出功能尚未實作");
    }

    /// <summary>
    /// Type4匯出專用查詢方法 - 根據提供的SQL查詢所需欄位
    /// </summary>
    /// <param name="year">年度</param>
    /// <param name="orgName">申請單位</param>
    /// <param name="supervisor">承辦人員</param>
    /// <param name="keyword">關鍵字</param>
    /// <param name="category">類別</param>
    /// <param name="reviewGroupCode">審查組別代碼</param>
    /// <returns>Type4匯出資料清單</returns>
    public static List<Type4ExportItem> SearchForExport_Type4(
        string year = "",
        string orgName = "",
        string supervisor = "",
        string keyword = "",
        string category = "",
        string reviewGroupCode = "")
    {
        DbHelper db = new DbHelper();

        // 根據用戶提供的SQL查詢
        db.CommandText = @"
SELECT [FinalReviewOrder] AS '排序'
      ,[Year] AS '年度'
      ,[ProjectNameTw] AS '計畫名稱'
      ,[OrgName] AS '申請單位'
      ,[TotalScore] AS '總分'
      ,[TotalSubsidyPrice] AS '申請經費'
      ,[ApprovedSubsidy] AS '核定經費'
      ,[FinalReviewNotes] AS '備註'
  FROM [OCA_OceanSubsidy].[dbo].[V_OFS_ReviewChecklist_type4]
  WHERE 1 = 1";

        try
        {
            db.Parameters.Clear();

            // 添加篩選條件
            if (!string.IsNullOrEmpty(year))
            {
                db.CommandText += " AND Year = @year";
                db.Parameters.Add("@year", year);
            }

            if (!string.IsNullOrEmpty(orgName))
            {
                db.CommandText += " AND OrgName = @orgName";
                db.Parameters.Add("@orgName", orgName);
            }

            if (!string.IsNullOrEmpty(supervisor))
            {
                db.CommandText += " AND SupervisoryPersonAccount = @supervisor";
                db.Parameters.Add("@supervisor", supervisor);
            }

            if (!string.IsNullOrEmpty(category))
            {
                db.CommandText += " AND ProjectID LIKE @category";
                db.Parameters.Add("@category", $"%{category}%");
            }

            if (!string.IsNullOrEmpty(reviewGroupCode))
            {
                db.CommandText += " AND Field = @reviewGroupCode";
                db.Parameters.Add("@reviewGroupCode", reviewGroupCode);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                db.CommandText += " AND (ProjectID LIKE @keyword OR ProjectNameTw LIKE @keyword)";
                db.Parameters.Add("@keyword", $"%{keyword}%");
            }

            db.CommandText += " ORDER BY [FinalReviewOrder] ASC";

            DataTable dt = db.GetTable();
            List<Type4ExportItem> results = new List<Type4ExportItem>();

            foreach (DataRow row in dt.Rows)
            {
                var item = new Type4ExportItem
                {
                    排序 = row["排序"]?.ToString() ?? "",
                    年度 = row["年度"]?.ToString() ?? "",
                    計畫名稱 = row["計畫名稱"]?.ToString() ?? "",
                    申請單位 = row["申請單位"]?.ToString() ?? "",
                    總分 = row["總分"]?.ToString() ?? "",
                    申請經費 = row["申請經費"]?.ToString() ?? "",
                    核定經費 = row["核定經費"]?.ToString() ?? "",
                    備註 = row["備註"]?.ToString() ?? ""
                };

                results.Add(item);
            }

            return results;
        }
        catch (Exception ex)
        {
            throw new Exception($"SearchForExport_Type4 查詢時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 生成Type4 Excel匯出檔案
    /// </summary>
    /// <param name="data">要匯出的資料</param>
    /// <returns>Excel檔案的byte陣列</returns>
    public static byte[] GenerateType4ExcelFile(List<Type4ExportItem> data)
    {
        try
        {
            // 創建臨時檔案
            string tempFile = Path.GetTempFileName() + ".xlsx";

            try
            {
                using (var excelHelper = ExcelHelper.CreateNew(tempFile))
                {
                    // 準備標題列和資料
                    string[] headers = { "排序", "年度", "計畫名稱", "申請單位", "總分", "申請經費", "核定經費", "備註" };
                    string sheetName = "決審核定列表";

                    // 重命名預設工作表
                    excelHelper.RenameWorksheet("工作表1", sheetName);

                    // 準備匯出資料
                    var exportData = new List<List<object>>();

                    // 加入標題列
                    exportData.Add(headers.Cast<object>().ToList());

                    // 加入資料列
                    foreach (var item in data)
                    {
                        var rowData = new List<object>
                        {
                            item.排序,
                            item.年度,
                            item.計畫名稱,
                            item.申請單位,
                            item.總分,
                            item.申請經費,
                            item.核定經費,
                            item.備註
                        };
                        exportData.Add(rowData);
                    }

                    // 寫入資料
                    excelHelper.WriteRange(sheetName, exportData, 1, 1);

                    // 自動調整欄寬
                    excelHelper.AutoSizeColumns(sheetName, headers.Length, minWidth: 8, maxWidth: 50);
                }

                // 讀取檔案內容
                return File.ReadAllBytes(tempFile);
            }
            finally
            {
                // 清理臨時檔案
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"生成Type4 Excel檔案時發生錯誤：{ex.Message}", ex);
        }
    }

    /// <summary>
    /// 根據欄位索引取得 Excel 欄位名稱 (A, B, C, ...)
    /// </summary>
    /// <param name="columnIndex">欄位索引 (0-based)</param>
    /// <returns>Excel 欄位名稱</returns>
    private static string GetColumnName(int columnIndex)
    {
        string columnName = "";
        while (columnIndex >= 0)
        {
            columnName = (char)('A' + (columnIndex % 26)) + columnName;
            columnIndex = (columnIndex / 26) - 1;
        }
        return columnName;
    }

    /// <summary>
    /// 設定 Excel 欄位寬度
    /// </summary>
    /// <param name="worksheetPart">工作表部分</param>
    /// <param name="headers">標題陣列</param>
    /// <param name="data">資料清單</param>
    private static void SetColumnWidths(WorksheetPart worksheetPart, string[] headers, List<Type4ExportItem> data)
    {
        // 建立欄位設定
        var columns = new Columns();

        // 計算每個欄位所需的寬度
        for (int i = 0; i < headers.Length; i++)
        {
            double maxWidth = headers[i].Length; // 從標題開始計算

            // 檢查該欄位中所有資料的最大長度
            foreach (var item in data)
            {
                string cellValue = "";
                switch (i)
                {
                    case 0: cellValue = item.排序; break;
                    case 1: cellValue = item.年度; break;
                    case 2: cellValue = item.計畫名稱; break;
                    case 3: cellValue = item.申請單位; break;
                    case 4: cellValue = item.總分; break;
                    case 5: cellValue = item.申請經費; break;
                    case 6: cellValue = item.核定經費; break;
                    case 7: cellValue = item.備註; break;
                }

                if (!string.IsNullOrEmpty(cellValue) && cellValue.Length > maxWidth)
                {
                    maxWidth = cellValue.Length;
                }
            }

            // 設定寬度限制：最小寬度8，最大寬度50
            // 中文字符需要較寬的空間，所以乘以1.2倍
            maxWidth = Math.Max(8, Math.Min(50, maxWidth * 1.2));

            var column = new Column()
            {
                Min = (uint)(i + 1),
                Max = (uint)(i + 1),
                Width = maxWidth,
                CustomWidth = true
            };

            columns.Append(column);
        }

        // 將欄位設定插入到工作表中
        worksheetPart.Worksheet.InsertBefore(columns, worksheetPart.Worksheet.GetFirstChild<SheetData>());
    }

    /// <summary>
    /// 取得Type4列表資料 (匯出用) - 預留給未來使用
    /// </summary>
    public static DataTable GetType4ListData(string year, string category, string status, string orgName, string supervisor, string keyword)
    {
        // TODO: 實作Type4列表資料查詢
        throw new NotImplementedException("Type4 列表資料匯出功能尚未實作");
    }

    #endregion
}
