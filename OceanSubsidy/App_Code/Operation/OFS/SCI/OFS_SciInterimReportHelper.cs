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
                    string fileCodeCondition;
                    if (reportType == 1)
                    {
                        fileCodeCondition = "('MidExamFile', 'MidExamFile_revise')";
                    }
                    else
                    {
                        fileCodeCondition = "('FinalExamFile', 'FinalExamFile_revise')";
                    }
                    
                    db.CommandText = $@"
                        SELECT [FileCode], [FileName], [TemplatePath]
                        FROM [OFS_SCI_UploadFile] 
                        WHERE [ProjectID] = @ProjectID
                        AND [FileCode] IN {fileCodeCondition}
                        ORDER BY [FileCode]";
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", projectID);
                    
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
        /// 提送階段審查報告 (新增或更新 OFS_SCI_StageExam 記錄)
        /// </summary>
        /// <param name="projectID">專案ID</param>
        /// <param name="stage">階段 (1=期中報告, 2=期末報告)</param>
        /// <param name="status">狀態 (暫存/審核中)</param>
        public static void SubmitStageExam(string projectID, int stage, string status)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    // 檢查是否已存在記錄
                    db.CommandText = "SELECT COUNT(*) FROM [OFS_SCI_StageExam] WHERE [ProjectID] = @ProjectID AND [Stage] = @Stage";
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", projectID);
                    db.Parameters.Add("@Stage", stage);
                    
                    bool exists = Convert.ToInt32(db.GetTable().Rows[0][0]) > 0;

                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", projectID);
                    db.Parameters.Add("@Stage", stage);
                    db.Parameters.Add("@Status", status);
                    db.Parameters.Add("@update_at", DateTime.Now);

                    if (exists)
                    {
                        // 更新現有記錄
                        db.CommandText = @"
                            UPDATE [OFS_SCI_StageExam] 
                            SET [Status] = @Status, 
                                [update_at] = @update_at
                            WHERE [ProjectID] = @ProjectID AND [Stage] = @Stage";
                    }
                    else
                    {
                        // 新增記錄
                        db.Parameters.Add("@ReviewMethod", DBNull.Value); // 審查方式為 null (供審查時填入)
                        db.Parameters.Add("@Reviewer", DBNull.Value); // 審核者為 null (供審查時填入)
                        db.Parameters.Add("@Account", DBNull.Value); // 審核者帳號為 null (供審查時填入)
                        db.Parameters.Add("@create_at", DateTime.Now);
                        
                        db.CommandText = @"
                            INSERT INTO [OFS_SCI_StageExam] 
                            ([ProjectID], [Stage], [ReviewMethod], [Status], [Reviewer], [Account], [create_at], [update_at]) 
                            VALUES (@ProjectID, @Stage, @ReviewMethod, @Status, @Reviewer, @Account, @create_at, @update_at)";
                    }

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
                    db.CommandText = @"
                        SELECT [Status], [ReviewMethod], [Reviewer], [Account]
                        FROM [OFS_SCI_StageExam] 
                        WHERE [ProjectID] = @ProjectID AND [Stage] = @Stage";
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", projectID);
                    db.Parameters.Add("@Stage", stage);
                    
                    DataTable dt = db.GetTable();
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
        /// 審核階段報告
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
                    db.CommandText = @"
                        UPDATE [OFS_SCI_StageExam] 
                        SET [ReviewMethod] = @ReviewMethod, 
                            [Status] = @Status,
                            [Reviewer] = @Reviewer,
                            [Account] = @Account,
                            [update_at] = @update_at
                        WHERE [ProjectID] = @ProjectID AND [Stage] = @Stage";
                    
                    db.Parameters.Clear();
                    db.Parameters.Add("@ReviewMethod", reviewMethod);
                    db.Parameters.Add("@Status", status);
                    db.Parameters.Add("@Reviewer", reviewer);
                    db.Parameters.Add("@Account", account);
                    db.Parameters.Add("@update_at", DateTime.Now);
                    db.Parameters.Add("@ProjectID", projectID);
                    db.Parameters.Add("@Stage", stage);

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
        /// 提送審查委員
        /// </summary>
        /// <param name="projectID">專案ID</param>
        /// <param name="stage">階段</param>
        /// <param name="reviewers">審查委員清單</param>
        public static void SubmitReviewers(string projectID, int stage, List<ReviewerData> reviewers)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    // 1. 取得 OFS_SCI_StageExam 的 ID
                    db.CommandText = @"
                        SELECT [ID] 
                        FROM [OFS_SCI_StageExam] 
                        WHERE [ProjectID] = @ProjectID AND [Stage] = @Stage";
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", projectID);
                    db.Parameters.Add("@Stage", stage);
                    
                    DataTable dt = db.GetTable();
                    if (dt.Rows.Count == 0)
                    {
                        throw new Exception("找不到對應的階段審查記錄");
                    }
                    
                    int examID = Convert.ToInt32(dt.Rows[0]["ID"]);
                    
                    // 2. 先刪除現有的審查委員記錄
                    db.CommandText = @"
                        DELETE FROM [OFS_SCI_StageExam_ReviewerList] 
                        WHERE [ExamID] = @ExamID";
                    db.Parameters.Clear();
                    db.Parameters.Add("@ExamID", examID);
                    db.ExecuteNonQuery();
                    
                    // 3. 批次寫入新的審查委員記錄
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
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"提送審查委員時發生錯誤: {ex.Message}");
                    throw;
                }
            }
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
    }
}