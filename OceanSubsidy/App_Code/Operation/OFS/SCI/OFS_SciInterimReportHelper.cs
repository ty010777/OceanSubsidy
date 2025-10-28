using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;
using GS.Data;
using GS.App;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Model.OFS.SCI;

namespace GS.OCA_OceanSubsidy.Operation.OFS
{
    /// <summary>
    /// SCI 期中報告相關資料處理 Helper
    /// </summary>
    public static class OFS_SciInterimReportHelper
    {
        /// <summary>
        /// 產生檔案儲存路徑
        /// </summary>
        /// <param name="projectID">專案ID</param>
        /// <param name="originalFileName">原始檔名</param>
        /// <param name="fileCode">檔案代碼</param>
        /// <param name="customName">自訂名稱 (修訂版使用)</param>
        /// <returns>檔案儲存路徑</returns>
        public static string GenerateFilePath(string projectID, string originalFileName, string fileCode, string customName = "")
        {
            string extension = Path.GetExtension(originalFileName);
            string fileName = "";
            
            if (fileCode == "MidExamFile")
            {
                fileName = $"{projectID}_期中報告書_初版{extension}";
            }
            else if (fileCode == "MidExamFile_revise")
            {
                fileName = string.IsNullOrEmpty(customName) 
                    ? $"{projectID}_期中報告書_修訂版{extension}" 
                    : $"{projectID}_{customName}{extension}";
            }
            else if (fileCode == "FinalExamFile")
            {
                fileName = $"{projectID}_期末報告書_初版{extension}";
            }
            else if (fileCode == "FinalExamFile_revise")
            {
                fileName = string.IsNullOrEmpty(customName) 
                    ? $"{projectID}_期末報告書_修訂版{extension}" 
                    : $"{projectID}_{customName}{extension}";
            }
            else
            {
                fileName = $"{projectID}_{fileCode}{extension}";
            }
            
            return $"UploadFiles/OFS/SCI/{projectID}/{fileName}";
        }

        /// <summary>
        /// 儲存上傳檔案記錄到資料庫
        /// </summary>
        /// <param name="projectID">專案ID</param>
        /// <param name="fileCode">檔案代碼</param>
        /// <param name="fileName">檔案名稱</param>
        /// <param name="filePath">檔案路徑</param>
        /// <returns>是否成功</returns>
        public static bool SaveUploadedFile(string projectID, string fileCode, string fileName, string filePath)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    // 檢查是否已存在相同檔案代碼的記錄
                    db.CommandText = "SELECT COUNT(*) FROM [OFS_SCI_UploadFile] WHERE [ProjectID] = @ProjectID AND [FileCode] = @FileCode";
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", projectID);
                    db.Parameters.Add("@FileCode", fileCode);
                    
                    bool exists = Convert.ToInt32(db.GetTable().Rows[0][0]) > 0;

                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", projectID);
                    db.Parameters.Add("@FileCode", fileCode);
                    db.Parameters.Add("@FileName", fileName);
                    db.Parameters.Add("@TemplatePath", filePath);

                    if (exists)
                    {
                        // 更新現有記錄 - 直接覆蓋
                        db.CommandText = @"
                            UPDATE [OFS_SCI_UploadFile] 
                            SET [FileName] = @FileName, 
                                [TemplatePath] = @TemplatePath
                            WHERE [ProjectID] = @ProjectID AND [FileCode] = @FileCode";
                    }
                    else
                    {
                        // 新增記錄
                        db.CommandText = @"
                            INSERT INTO [OFS_SCI_UploadFile] 
                            ([ProjectID], [FileCode], [FileName], [TemplatePath]) 
                            VALUES (@ProjectID, @FileCode, @FileName, @TemplatePath)";
                    }

                    db.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"儲存上傳檔案記錄時發生錯誤: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// 取得專案的已上傳檔案清單
        /// </summary>
        /// <param name="projectID">專案ID</param>
        /// <param name="reportType">報告類型 (1=期中報告, 2=期末報告)</param>
        /// <returns>已上傳檔案清單</returns>
        public static List<UploadedFileInfo> GetUploadedFilesList(string projectID, int reportType = 1)
        {
            var result = new List<UploadedFileInfo>();
            
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    // 根據報告類型決定要查詢的 FileCode
                    string[] fileCodes;
                    if (reportType == 1)
                    {
                        fileCodes = new[] { "MidExamFile", "MidExamFile_revise" };
                    }
                    else
                    {
                        fileCodes = new[] { "FinalExamFile", "FinalExamFile_revise" };
                    }
                    
                    // 建立參數化的 FileCode 參數
                    var fileCodeParams = new List<string>();
                    for (int i = 0; i < fileCodes.Length; i++)
                    {
                        fileCodeParams.Add("@fileCode" + i);
                    }
                    string fileCodeList = string.Join(",", fileCodeParams);
                    
                    db.CommandText = $@"
                        SELECT [FileCode], [FileName], [TemplatePath]
                        FROM [OFS_SCI_UploadFile] 
                        WHERE [ProjectID] = @ProjectID
                        AND [FileCode] IN ({fileCodeList})
                        ORDER BY [FileCode]";
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", projectID);
                    
                    // 新增每個 FileCode 參數
                    for (int i = 0; i < fileCodes.Length; i++)
                    {
                        db.Parameters.Add("@fileCode" + i, fileCodes[i]);
                    }
                    
                    DataTable dt = db.GetTable();
                    foreach (DataRow row in dt.Rows)
                    {
                        result.Add(new UploadedFileInfo
                        {
                            FileCode = row["FileCode"]?.ToString() ?? "",
                            FileName = row["FileName"]?.ToString() ?? "",
                            FilePath = row["TemplatePath"]?.ToString() ?? ""
                        });
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"取得已上傳檔案清單時發生錯誤: {ex.Message}");
                }
            }
            
            return result;
        }
        public static List<UploadedFileInfo> GetUploadedFiles(string projectID,string fileCode)
        {
            var result = new List<UploadedFileInfo>();
            
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    // 根據報告類型決定要查詢的 FileCode
                    
                    db.CommandText = $@"
                        SELECT [FileCode], [FileName], [TemplatePath]
                        FROM [OFS_SCI_UploadFile] 
                        WHERE [ProjectID] = @ProjectID
                        AND [FileCode] =@fileCode
                        ORDER BY [FileCode]";
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", projectID);
                    db.Parameters.Add("@fileCode", fileCode);
                    
                    DataTable dt = db.GetTable();
                    foreach (DataRow row in dt.Rows)
                    {
                        result.Add(new UploadedFileInfo
                        {
                            FileCode = row["FileCode"]?.ToString() ?? "",
                            FileName = row["FileName"]?.ToString() ?? "",
                            FilePath = row["TemplatePath"]?.ToString() ?? ""
                        });
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"取得已上傳檔案清單時發生錯誤: {ex.Message}");
                }
            }
            
            return result;
        }
        /// <summary>
        /// 取得下一個 ExamVersion 編號
        /// </summary>
        /// <param name="projectID">專案ID</param>
        /// <param name="stage">階段</param>
        /// <returns>下一個版本號</returns>
        private static int GetNextExamVersion(string projectID, int stage)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    db.CommandText = @"
                        SELECT MAX([ExamVersion])
                        FROM [OFS_SCI_StageExam]
                        WHERE [ProjectID] = @ProjectID AND [Stage] = @Stage";
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", projectID);
                    db.Parameters.Add("@Stage", stage);

                    DataTable dt = db.GetTable();
                    if (dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value)
                    {
                        return Convert.ToInt32(dt.Rows[0][0]) + 1;
                    }

                    return 0; // 第一次提送，版本號為 0
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"取得下一個版本號時發生錯誤: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// 提送階段審查報告 (每次都新增一筆記錄，透過 ExamVersion 區分版本)
        /// </summary>
        /// <param name="projectID">專案ID</param>
        /// <param name="stage">階段 (1=期中報告, 2=期末報告)</param>
        /// <param name="status">狀態 (審核中)</param>
        public static void SubmitStageExam(string projectID, int stage, string status)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    // 取得下一個版本號
                    int nextVersion = GetNextExamVersion(projectID, stage);

                    // 每次都新增記錄
                    db.CommandText = @"
                        INSERT INTO [OFS_SCI_StageExam]
                        ([ProjectID], [Stage], [ExamVersion], [ReviewMethod], [Status], [Reviewer], [Account], [create_at], [update_at])
                        VALUES (@ProjectID, @Stage, @ExamVersion, @ReviewMethod, @Status, @Reviewer, @Account, @create_at, @update_at)";

                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", projectID);
                    db.Parameters.Add("@Stage", stage);
                    db.Parameters.Add("@ExamVersion", nextVersion);
                    db.Parameters.Add("@ReviewMethod", DBNull.Value); // 審查方式為 null (供審查時填入)
                    db.Parameters.Add("@Status", status);
                    db.Parameters.Add("@Reviewer", DBNull.Value); // 審核者為 null (供審查時填入)
                    db.Parameters.Add("@Account", DBNull.Value); // 審核者帳號為 null (供審查時填入)
                    db.Parameters.Add("@create_at", DateTime.Now);
                    db.Parameters.Add("@update_at", DateTime.Now);

                    db.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"提送階段審查時發生錯誤: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// 取得階段審查狀態
        /// 邏輯：若有任何一筆記錄 Status = '通過'，則返回「通過」；否則返回最新版本的狀態
        /// </summary>
        /// <param name="projectID">專案ID</param>
        /// <param name="stage">階段</param>
        /// <returns></returns>
        public static StageExamStatus GetStageExamStatus(string projectID, int stage)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    // 1. 先檢查是否有任何一筆記錄的 Status = '通過'
                    db.CommandText = @"
                        SELECT TOP 1 [Status], [ReviewMethod], [Reviewer], [Account]
                        FROM [OFS_SCI_StageExam]
                        WHERE [ProjectID] = @ProjectID AND [Stage] = @Stage AND [Status] = N'通過'";
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", projectID);
                    db.Parameters.Add("@Stage", stage);

                    DataTable dt = db.GetTable();
                    if (dt.Rows.Count > 0)
                    {
                        // 找到至少一筆「通過」的記錄，直接返回
                        DataRow row = dt.Rows[0];
                        return new StageExamStatus
                        {
                            Status = "通過",
                            ReviewMethod = row["ReviewMethod"]?.ToString() ?? "",
                            Reviewer = row["Reviewer"]?.ToString() ?? "",
                            Account = row["Account"]?.ToString() ?? ""
                        };
                    }

                    // 2. 沒有「通過」記錄，查詢最新版本的審查狀態
                    db.CommandText = @"
                        SELECT TOP 1 [Status], [ReviewMethod], [Reviewer], [Account]
                        FROM [OFS_SCI_StageExam]
                        WHERE [ProjectID] = @ProjectID AND [Stage] = @Stage
                        ORDER BY [ExamVersion] DESC";
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", projectID);
                    db.Parameters.Add("@Stage", stage);

                    dt = db.GetTable();
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        return new StageExamStatus
                        {
                            Status = row["Status"]?.ToString() ?? "",
                            ReviewMethod = row["ReviewMethod"]?.ToString() ?? "",
                            Reviewer = row["Reviewer"]?.ToString() ?? "",
                            Account = row["Account"]?.ToString() ?? ""
                        };
                    }

                    return new StageExamStatus { Status = "", ReviewMethod = "", Reviewer = "", Account = "" };
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"取得階段審查狀態時發生錯誤: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// 審核階段報告 (更新最新版本)
        /// </summary>
        /// <param name="projectID">專案ID</param>
        /// <param name="stage">階段</param>
        /// <param name="reviewMethod">審查方式</param>
        /// <param name="status">狀態</param>
        /// <param name="reviewComment">審查意見</param>
        /// <param name="reviewer">審核者姓名</param>
        /// <param name="account">審核者帳號</param>
        public static void ReviewStageExam(string projectID, int stage, string reviewMethod, string status, string reviewComment, string reviewer, string account)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    // 先取得最新版本號
                    db.CommandText = @"
                        SELECT TOP 1 [ExamVersion]
                        FROM [OFS_SCI_StageExam]
                        WHERE [ProjectID] = @ProjectID AND [Stage] = @Stage
                        ORDER BY [ExamVersion] DESC";
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", projectID);
                    db.Parameters.Add("@Stage", stage);

                    DataTable dt = db.GetTable();
                    if (dt.Rows.Count == 0)
                    {
                        throw new Exception("找不到對應的階段審查記錄");
                    }

                    int latestVersion = Convert.ToInt32(dt.Rows[0]["ExamVersion"]);

                    // 更新最新版本的記錄
                    db.CommandText = @"
                        UPDATE [OFS_SCI_StageExam]
                        SET [ReviewMethod] = @ReviewMethod,
                            [Status] = @Status,
                            [Reviewer] = @Reviewer,
                            [Account] = @Account,
                            [update_at] = @update_at
                        WHERE [ProjectID] = @ProjectID AND [Stage] = @Stage AND [ExamVersion] = @ExamVersion";

                    db.Parameters.Clear();
                    db.Parameters.Add("@ReviewMethod", reviewMethod);
                    db.Parameters.Add("@Status", status);
                    db.Parameters.Add("@Reviewer", reviewer);
                    db.Parameters.Add("@Account", account);
                    db.Parameters.Add("@update_at", DateTime.Now);
                    db.Parameters.Add("@ProjectID", projectID);
                    db.Parameters.Add("@Stage", stage);
                    db.Parameters.Add("@ExamVersion", latestVersion);

                    db.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"審核階段報告時發生錯誤: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// 上傳檔案資訊類別
        /// </summary>
        public class UploadedFileInfo
        {
            public string FileCode { get; set; }
            public string FileName { get; set; }
            public string FilePath { get; set; }
        }

        /// <summary>
        /// 提送審查委員 (綁定最新版本)
        /// </summary>
        /// <param name="projectID">專案ID</param>
        /// <param name="stage">階段</param>
        /// <param name="reviewers">審查委員清單</param>
        /// <returns>審查委員 URL 資訊列表</returns>
        public static List<ReviewerUrlInfo> SubmitReviewers(string projectID, int stage, List<ReviewerData> reviewers)
        {
            var reviewerUrls = new List<ReviewerUrlInfo>();

            using (DbHelper db = new DbHelper())
            {
                try
                {
                    // 1. 取得最新版本的 OFS_SCI_StageExam ID
                    db.CommandText = @"
                        SELECT TOP 1 [ID]
                        FROM [OFS_SCI_StageExam]
                        WHERE [ProjectID] = @ProjectID AND [Stage] = @Stage
                        ORDER BY [ExamVersion] DESC";
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", projectID);
                    db.Parameters.Add("@Stage", stage);

                    DataTable dt = db.GetTable();
                    if (dt.Rows.Count == 0)
                    {
                        throw new Exception("找不到對應的階段審查記錄");
                    }

                    int examID = Convert.ToInt32(dt.Rows[0]["ID"]);

                    // // 2. 先刪除現有的審查委員記錄
                    // db.CommandText = @"
                    //     DELETE FROM [OFS_SCI_StageExam_ReviewerList]
                    //     WHERE [ExamID] = @ExamID";
                    // db.Parameters.Clear();
                    // db.Parameters.Add("@ExamID", examID);
                    // db.ExecuteNonQuery();

                    // 3. 批次寫入新的審查委員記錄並生成 URL
                    foreach (var reviewer in reviewers)
                    {
                        // 跳過姓名或Email為空的記錄
                        if (string.IsNullOrWhiteSpace(reviewer.name) || string.IsNullOrWhiteSpace(reviewer.email))
                        {
                            continue;
                        }

                        // 生成唯一的token
                        string token = GenerateReviewerToken();

                        db.CommandText = @"
                            INSERT INTO [OFS_SCI_StageExam_ReviewerList]
                            ([ExamID], [Account], [Reviewer], [token])
                            VALUES (@ExamID, @Account, @Reviewer, @token)";
                        db.Parameters.Clear();
                        db.Parameters.Add("@ExamID", examID);
                        db.Parameters.Add("@Account", reviewer.email); // Email存放在Account欄位
                        db.Parameters.Add("@Reviewer", reviewer.name);
                        db.Parameters.Add("@token", token);

                        db.ExecuteNonQuery();

                        // 生成審查 URL
                        string url = System.Configuration.ConfigurationManager.AppSettings["Host"] +
                                     System.Configuration.ConfigurationManager.AppSettings["AppRootPath"] +
                                     $"/OFS/SCI/SciExamReview.aspx?ProjectID={projectID}&token={token}";

                        // 添加到返回列表
                        reviewerUrls.Add(new ReviewerUrlInfo
                        {
                            Email = reviewer.email,
                            Name = reviewer.name,
                            Url = url
                        });
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"提送審查委員時發生錯誤: {ex.Message}");
                    throw;
                }
            }

            return reviewerUrls;
        }

        /// <summary>
        /// 生成審查委員Token
        /// </summary>
        /// <returns>唯一的Token字串</returns>
        private static string GenerateReviewerToken()
        {
            // 使用 GUID + 時間戳記 確保唯一性
            string guid = Guid.NewGuid().ToString("N"); // 移除破折號的32位字串
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            
            // 組合成唯一token (可以根據需求調整格式)
            return $"{guid}_{timestamp}";
        }

        /// <summary>
        /// 階段審查狀態類別
        /// </summary>
        public class StageExamStatus
        {
            public string Status { get; set; }
            public string ReviewMethod { get; set; }
            public string Reviewer { get; set; }
            public string Account { get; set; }
        }

        /// <summary>
        /// 檢查專案是否有審核中的階段報告
        /// </summary>
        /// <param name="projectID">專案ID</param>
        /// <returns>是否有審核中的報告</returns>
        public static bool HasReportInReview(string projectID)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    db.CommandText = @"
                        SELECT COUNT(*)
                        FROM [OFS_SCI_StageExam]
                        WHERE [ProjectID] = @ProjectID
                        AND [Status] = N'審核中'";

                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", projectID);

                    DataTable dt = db.GetTable();
                    if (dt.Rows.Count > 0)
                    {
                        return Convert.ToInt32(dt.Rows[0][0]) > 0;
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"檢查審核中報告時發生錯誤: {ex.Message}");
                    return false;
                }
            }
        }

        /// <summary>
        /// 取得階段審查意見檔案清單
        /// </summary>
        /// <param name="projectID">專案ID</param>
        /// <param name="stage">階段</param>
        /// <param name="fileType">檔案類型 (1=初版, 2=修正版)</param>
        /// <param name="includeReviewer">是否包含審查委員姓名</param>
        /// <returns>審查意見檔案清單</returns>
        public static List<ReviewFileInfo> GetReviewFiles(string projectID, int stage, int fileType, bool includeReviewer = true)
        {
            var result = new List<ReviewFileInfo>();

            using (DbHelper db = new DbHelper())
            {
                try
                {
                    string whereCondition = fileType == 1
                        ? "AND SE.[ExamVersion] = 0"
                        : "AND SE.[ExamVersion] > 0";

                    db.CommandText = $@"
                        SELECT SE.[ExamVersion], SR.[ReviewFilePath], SR.[Reviewer], ISNULL(SR.[isSubmit], 0) as IsSubmit
                        FROM [OFS_SCI_StageExam] SE
                        LEFT JOIN [OFS_SCI_StageExam_ReviewerList] SR ON SR.[ExamID] = SE.[ID]
                        WHERE SE.[ProjectID] = @ProjectID
                        AND SE.[Stage] = @Stage
                        {whereCondition}
                        AND SR.[Reviewer] IS NOT NULL
                        ORDER BY SE.[ExamVersion], SR.[ID]";

                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", projectID);
                    db.Parameters.Add("@Stage", stage);

                    DataTable dt = db.GetTable();
                    foreach (DataRow row in dt.Rows)
                    {
                        int examVersion = Convert.ToInt32(row["ExamVersion"]);
                        string filePath = row["ReviewFilePath"]?.ToString() ?? "";
                        string reviewer = includeReviewer ? (row["Reviewer"]?.ToString() ?? "") : "";
                        bool isSubmit = Convert.ToInt32(row["IsSubmit"]) == 1;

                        result.Add(new ReviewFileInfo
                        {
                            ExamVersion = examVersion,
                            ReviewFilePath = filePath,
                            Reviewer = reviewer,
                            IsSubmit = isSubmit
                        });
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"取得審查意見檔案清單時發生錯誤: {ex.Message}");
                }
            }

            return result;
        }

        /// <summary>
        /// 審查意見檔案資訊類別
        /// </summary>
        public class ReviewFileInfo
        {
            public int ExamVersion { get; set; }
            public string ReviewFilePath { get; set; }
            public string Reviewer { get; set; }
            public bool IsSubmit { get; set; }
        }

        /// <summary>
        /// 審查委員 URL 資訊類別
        /// </summary>
        public class ReviewerUrlInfo
        {
            public string Email { get; set; }
            public string Name { get; set; }
            public string Url { get; set; }
        }
    }
}