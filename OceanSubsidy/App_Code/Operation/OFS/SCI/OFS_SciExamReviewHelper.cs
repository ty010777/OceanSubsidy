using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GS.Data;
using GS.OCA_OceanSubsidy.Entity;
using GS.App;
using System.IO;
using GS.Data.Sql;

namespace GS.OCA_OceanSubsidy.Operation.OFS
{
    /// <summary>
    /// 科專審查頁面相關操作
    /// </summary>
    public class OFS_SciExamReviewHelper
    {
        public OFS_SciExamReviewHelper()
        {
            //
            // TODO: 在這裡新增建構函式邏輯
            //
        }

        /// <summary>
        /// 根據Token取得審查資料
        /// </summary>
        /// <param name="token">審查Token</param>
        /// <returns>審查資料DataRow，若無資料則返回null</returns>
        public static DataRow GetExamDataByToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            DbHelper db = new DbHelper();
            db.CommandText = @"
                SELECT 
                    se.*,
                    srl.Reviewer,
                    srl.token,
                    am.ProjectNameTw AS ProjectName,
                    am.Field,
                    am.Year,
                    am.OrgName
                FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_StageExam] se
                LEFT JOIN OFS_SCI_StageExam_ReviewerList srl ON se.id = srl.ExamID
                LEFT JOIN OFS_SCI_Application_Main am ON se.ProjectID = am.ProjectID
                WHERE srl.token = @token";
            
            db.Parameters.Clear();
            db.Parameters.Add("@token", token);
            
            try
            {
                DataTable dt = db.GetTable();
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
            catch (Exception ex)
            {
                throw new Exception($"根據Token取得審查資料時發生錯誤: {ex.Message}", ex);
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// 根據ProjectID和Stage取得上傳的檔案清單
        /// </summary>
        /// <param name="projectID">專案ID</param>
        /// <param name="stage">階段 (1=期中, 2=期末)</param>
        /// <returns>報告檔案清單</returns>
        public static List<ReportFileInfo> GetReportFiles(string projectID, int stage)
        {
            var result = new List<ReportFileInfo>();

            if (string.IsNullOrEmpty(projectID))
                return result;

            string[] fileCodes;
            if (stage == 1)
            {
                fileCodes = new[] { "MidExamFile", "MidExamFile_revise" };
            }
            else
            {
                fileCodes = new[] { "FinalExamFile", "FinalExamFile_revise" };
            }

            DbHelper db = new DbHelper();
            string fileCodeList = "'" + string.Join("','", fileCodes) + "'";
            
            db.CommandText = $@"
                SELECT 
                    FileCode,
                    FileName,
                    TemplatePath
                FROM OFS_SCI_UploadFile 
                WHERE ProjectID = @projectID 
                AND FileCode IN ({fileCodeList})
                  ";
            
            db.Parameters.Clear();
            db.Parameters.Add("@projectID", projectID);
            
            try
            {
                DataTable dt = db.GetTable();
                
                foreach (DataRow row in dt.Rows)
                {
                    result.Add(new ReportFileInfo
                    {
                        FileCode = row["FileCode"]?.ToString() ?? "",
                        FileName = row["FileName"]?.ToString() ?? "",
                        FilePath = row["TemplatePath"]?.ToString() ?? "",
                        FileType = row["FileCode"].ToString().Contains("_revise") ? "修訂版" : "初版"
                    });
                }
                
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"取得報告檔案清單時發生錯誤: {ex.Message}");
                throw new Exception($"取得報告檔案清單時發生錯誤: {ex.Message}", ex);
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// 檢查Token是否有效
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns>Token是否有效</returns>
        public static bool IsValidToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            DbHelper db = new DbHelper();
            db.CommandText = "SELECT COUNT(*) FROM OFS_SCI_StageExam_ReviewerList WHERE token = @token";
            db.Parameters.Clear();
            db.Parameters.Add("@token", token);
            
            try
            {
                DataTable dt = db.GetTable();
                if (dt.Rows.Count > 0)
                {
                    int count = Convert.ToInt32(dt.Rows[0][0]);
                    return count > 0;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"檢查Token有效性時發生錯誤: {ex.Message}");
                return false;
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// 檢查審查是否已提交
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns>是否已提交</returns>
        public static bool IsReviewSubmitted(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            DbHelper db = new DbHelper();
            db.CommandText = "SELECT ISNULL(isSubmit, 0) FROM OFS_SCI_StageExam_ReviewerList WHERE token = @token";
            db.Parameters.Clear();
            db.Parameters.Add("@token", token);
            
            try
            {
                DataTable dt = db.GetTable();
                if (dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(dt.Rows[0][0]) == 1;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"檢查審查提交狀態時發生錯誤: {ex.Message}");
                return false;
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// 取得審查檔案路徑
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns>檔案路徑</returns>
        public static string GetReviewFilePath(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            DbHelper db = new DbHelper();
            db.CommandText = "SELECT ReviewFilePath FROM OFS_SCI_StageExam_ReviewerList WHERE token = @token";
            db.Parameters.Clear();
            db.Parameters.Add("@token", token);
            
            try
            {
                DataTable dt = db.GetTable();
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0]["ReviewFilePath"]?.ToString();
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"取得審查檔案路徑時發生錯誤: {ex.Message}");
                return null;
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// 儲存審查檔案
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="uploadedFile">上傳的檔案</param>
        /// <returns>儲存的檔案名稱</returns>
        public static string SaveReviewFile(string token, System.Web.HttpPostedFile uploadedFile)
        {
            if (string.IsNullOrEmpty(token) || uploadedFile == null)
                throw new Exception("參數不完整");

            // 取得審查資料
            DataRow examData = GetExamDataByToken(token);
            if (examData == null)
                throw new Exception("找不到對應的審查資料");

            string projectID = examData["ProjectID"]?.ToString();
            string reviewer = examData["Reviewer"]?.ToString() ?? "reviewer";
            
            // 產生檔案名稱：ProjectID_審查委員_審查意見.pdf
            string fileExtension = System.IO.Path.GetExtension(uploadedFile.FileName);
            string fileName = $"{projectID}_{reviewer}_審查意見{fileExtension}";
            
            // 產生儲存路徑
            string relativePath = $"UploadFiles/OFS/SCI/{projectID}/ReviewFiles/{fileName}";
            string physicalPath = System.Web.HttpContext.Current.Server.MapPath("~/" + relativePath);
            
            // 確保目錄存在
            string directory = System.IO.Path.GetDirectoryName(physicalPath);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            
            // 儲存檔案
            uploadedFile.SaveAs(physicalPath);
            
            // 更新資料庫記錄審查檔案路徑
            UpdateReviewerFilePath(token, relativePath);
            
            return fileName;
        }

        /// <summary>
        /// 更新審查委員的檔案路徑
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="filePath">檔案路徑</param>
        private static void UpdateReviewerFilePath(string token, string filePath)
        {
            DbHelper db = new DbHelper();
            db.CommandText = @"
                UPDATE OFS_SCI_StageExam_ReviewerList 
                SET ReviewFilePath = @filePath 
                WHERE token = @token";
            
            db.Parameters.Clear();
            db.Parameters.Add("@filePath", filePath);
            db.Parameters.Add("@token", token);
            
            try
            {
                db.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"更新審查檔案路徑時發生錯誤: {ex.Message}");
                throw new Exception($"更新審查檔案路徑時發生錯誤: {ex.Message}", ex);
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// 提交審查結果
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="reviewResult">審查結果</param>
        /// <param name="reviewComment">審查意見</param>
        /// <param name="reviewFilePath">審查檔案路徑</param>
        /// <returns>是否提交成功</returns>
        public static void SubmitReviewResult(string token)
        {
            

            DbHelper db = new DbHelper();
            db.CommandText = @"
                UPDATE OFS_SCI_StageExam_ReviewerList 
                SET 
                    isSubmit = 1
                WHERE token = @token";
            
            db.Parameters.Clear();
            db.Parameters.Add("@token", token);
            
            try
            {
                db.ExecuteNonQuery();
               
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"提交審查結果時發生錯誤: {ex.Message}");
                throw new Exception($"提交審查結果時發生錯誤: {ex.Message}", ex);
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// 取得檔案實際路徑用於下載
        /// </summary>
        /// <param name="projectID">專案ID</param>
        /// <param name="fileCode">檔案代碼</param>
        /// <returns>檔案路徑，若找不到則返回null</returns>
        public static string GetFilePhysicalPath(string projectID, string fileCode)
        {
            if (string.IsNullOrEmpty(projectID) || string.IsNullOrEmpty(fileCode))
                return null;

            DbHelper db = new DbHelper();
            db.CommandText = @"
                SELECT TemplatePath 
                FROM OFS_SCI_UploadFile 
                WHERE ProjectID = @projectID AND FileCode = @fileCode
                ";
            
            db.Parameters.Clear();
            db.Parameters.Add("@projectID", projectID);
            db.Parameters.Add("@fileCode", fileCode);
            
            try
            {
                DataTable dt = db.GetTable();
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0]["TemplatePath"]?.ToString();
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"取得檔案路徑時發生錯誤: {ex.Message}");
                throw new Exception($"取得檔案路徑時發生錯誤: {ex.Message}", ex);
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// 根據Token取得審查基本資訊 (不含檔案清單)
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns>審查基本資訊</returns>
        public static ExamBasicInfo GetExamBasicInfo(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            DataRow examData = GetExamDataByToken(token);
            
            if (examData == null)
                return null;

            try
            {
                return new ExamBasicInfo
                {
                    ProjectID = examData["ProjectID"]?.ToString() ?? "",
                    Stage = examData["Stage"] != DBNull.Value ? Convert.ToInt32(examData["Stage"]) : 0,
                    StageName = examData["Stage"] != DBNull.Value && Convert.ToInt32(examData["Stage"]) == 1 ? "期中報告" : "期末報告",
                    ProjectName = examData["ProjectName"]?.ToString() ?? "",
                    Year = examData["Year"]?.ToString() ?? "",
                    Field = examData["Field"]?.ToString() ?? "",
                    OrgName = examData["OrgName"]?.ToString() ?? "",
                    Reviewer = examData["Reviewer"]?.ToString() ?? "",
                    Status = examData["Status"]?.ToString() ?? "",
                    ReviewMethod = examData["ReviewMethod"]?.ToString() ?? ""
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"解析審查基本資訊時發生錯誤: {ex.Message}");
                throw new Exception($"解析審查基本資訊時發生錯誤: {ex.Message}", ex);
            }
        }

        #region 內部類別定義

        /// <summary>
        /// 報告檔案資訊類別
        /// </summary>
        public class ReportFileInfo
        {
            public string FileCode { get; set; } = "";
            public string FileName { get; set; } = "";
            public string FilePath { get; set; } = "";
            public DateTime? UploadTime { get; set; }
            public string FileType { get; set; } = "";
        }

        /// <summary>
        /// 審查基本資訊類別
        /// </summary>
        public class ExamBasicInfo
        {
            public string ProjectID { get; set; } = "";
            public int Stage { get; set; }
            public string StageName { get; set; } = "";
            public string ProjectName { get; set; } = "";
            public string Year { get; set; } = "";
            public string Field { get; set; } = "";
            public string OrgName { get; set; } = "";
            public string Reviewer { get; set; } = "";
            public string Status { get; set; } = "";
            public string ReviewMethod { get; set; } = "";
        }

        #endregion
    }
}