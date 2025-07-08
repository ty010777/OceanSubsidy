using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using GS.Data.Sql;
using GS.Extension;
using GS.OCA_OceanSubsidy.Entity;
using System.Globalization;

namespace GS.OCA_OceanSubsidy.Operation.OFS
{
    public class OFS_SciWorkSchHelper
    {
        #region 計畫期程操作 (Application_Main)
        
        /// <summary>
        /// 更新計畫期程到 OFS_SCI_Application_Main
        /// </summary>
        /// <param name="versionId">版本ID</param>
        /// <param name="startTime">計畫開始時間</param>
        /// <param name="endTime">計畫結束時間</param>
        /// <returns>是否成功</returns>
        public static void UpdateProjectSchedule(string versionId, DateTime? startTime, DateTime? endTime)
        {

            using (DbHelper db = new DbHelper())
            {

                try
                {
                    db.CommandText = @"
                    UPDATE OFS_SCI_Application_Main 
                    SET StartTime = @StartTime, 
                        EndTime = @EndTime,
                        updated_at = @UpdatedAt
                    WHERE Version_ID = @VersionId";

                    
                    db.Parameters.Clear();
                    db.Parameters.Add("@StartTime", startTime);
                    db.Parameters.Add("@EndTime", endTime);
                    db.Parameters.Add("@UpdatedAt", DateTime.Now);
                    db.Parameters.Add("@VersionId", versionId);
                    db.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"UpdateProjectSchedule發生錯誤: {ex.Message}");
                    throw;
                }
            }
        }
        
        #endregion
        
        #region 工作項目操作 (單表設計)
        
        /// <summary>
        /// 儲存所有工作項目(主項+子項)
        /// </summary>
        /// <param name="versionId">版本ID</param>
        /// <param name="workItems">工作項目資料列表</param>
        /// <returns>成功儲存的筆數</returns>
        public static int SaveAllWorkItems(string versionId, List<OFS_SCI_WorkSch_Main> workItems)
        {
            using (DbHelper db = new DbHelper())
            {

                try
                {
                    // 先刪除該Version_ID的所有舊資料
                    string deleteSql = "DELETE FROM OFS_SCI_WorkSch_Main WHERE Version_ID = @Version_ID";
                    db.CommandText = deleteSql;
                    db.Parameters.Clear();
                    db.Parameters.Add("@Version_ID", versionId);
                    db.ExecuteNonQuery();

                    int savedCount = 0;

                    foreach (var item in workItems)
                    {
                        // 插入新的工作項目
                        string insertSql = @"
                            INSERT INTO OFS_SCI_WorkSch_Main 
                            (Version_ID, WorkItem_id, WorkName, StartMonth, EndMonth, Weighting, InvestMonth, IsOutsourced)
                            VALUES 
                            (@VersionId, @WorkItemId, @WorkName, @StartMonth, @EndMonth, @Weighting, @InvestMonth, @IsOutsourced)";
                        
                        db.CommandText = insertSql;
                        db.Parameters.Clear();
                        db.Parameters.Add("@VersionId", versionId);
                        db.Parameters.Add("@WorkItemId", item.WorkItem_id);
                        db.Parameters.Add("@WorkName", item.WorkName ?? "");
                        db.Parameters.Add("@StartMonth", (object)item.StartMonth ?? DBNull.Value);
                        db.Parameters.Add("@EndMonth", (object)item.EndMonth ?? DBNull.Value);
                        db.Parameters.Add("@Weighting", (object)item.Weighting ?? DBNull.Value);
                        db.Parameters.Add("@InvestMonth", (object)item.InvestMonth ?? DBNull.Value);
                        db.Parameters.Add("@IsOutsourced", item.IsOutsourced ?? false);
                        
                        db.ExecuteNonQuery();
                        savedCount++;
                    }
                    return savedCount;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"SaveAllWorkItems發生錯誤: {ex.Message}");
                    throw;
                }
            }
        }
        
        /// <summary>
        /// 產生WorkItem_id
        /// </summary>
        /// <param name="versionId">版本ID</param>
        /// <param name="itemCode">項目代碼 (A, A1, B, B1等)</param>
        /// <returns>完整的WorkItem_id</returns>
        public static string GenerateWorkItemId(string versionId, string itemCode)
        {
            return $"{versionId}_{itemCode}";
        }
        
        /// <summary>
        /// 從WorkItem_id中提取項目代碼
        /// </summary>
        /// <param name="workItemId">WorkItem_id (格式: {Version_ID}_A)</param>
        /// <returns>項目代碼 (A, A1, B等)</returns>
        public static string ExtractItemCodeFromWorkItemId(string workItemId)
        {
            if (string.IsNullOrEmpty(workItemId))
                return null;
                
            int lastUnderscoreIndex = workItemId.LastIndexOf('_');
            if (lastUnderscoreIndex >= 0 && lastUnderscoreIndex < workItemId.Length - 1)
            {
                return workItemId.Substring(lastUnderscoreIndex + 1);
            }
            
            return null;
        }
        
        /// <summary>
        /// 從WorkItem_id中提取Version_ID
        /// </summary>
        /// <param name="workItemId">WorkItem_id (格式: {Version_ID}_A)</param>
        /// <returns>Version_ID</returns>
        public static string ExtractVersionIdFromWorkItemId(string workItemId)
        {
            if (string.IsNullOrEmpty(workItemId))
                return null;
                
            int lastUnderscoreIndex = workItemId.LastIndexOf('_');
            if (lastUnderscoreIndex > 0)
            {
                return workItemId.Substring(0, lastUnderscoreIndex);
            }
            
            return null;
        }
        
        #endregion
        
        #region 查核標準操作
        
        /// <summary>
        /// 儲存查核標準資料
        /// </summary>
        /// <param name="checkStandards">查核標準資料列表</param>
        /// <returns>成功儲存的筆數</returns>
        public static int SaveCheckStandards(List<OFS_SCI_WorkSch_CheckStandard> checkStandards)
        {
            if (checkStandards == null || !checkStandards.Any())
            {
                return 0;
            }

            using (DbHelper db = new DbHelper())
            {
                try
                {
                    // 取得第一筆資料的 Version_ID 來刪除舊資料
                    string versionId = checkStandards.First().Version_ID;
                    
                    // 先刪除該Version_ID的所有舊資料
                    string deleteSql = "DELETE FROM OFS_SCI_WorkSch_CheckStandard WHERE Version_ID = @Version_ID";
                    db.CommandText = deleteSql;
                    db.Parameters.Clear();
                    db.Parameters.Add("@Version_ID", versionId);
                    db.ExecuteNonQuery();

                    int savedCount = 0;

                    foreach (var checkStandard in checkStandards)
                    {
                        // 插入新的查核標準資料
                        string insertSql = @"
                            INSERT INTO OFS_SCI_WorkSch_CheckStandard 
                            (Version_ID, WorkItem, SerialNumber, PlannedFinishDate, CheckDescription, CreatedAt, UpdatedAt)
                            VALUES 
                            (@VersionId, @WorkItem, @SerialNumber, @PlannedFinishDate, @CheckDescription, @CreatedAt, @UpdatedAt)";
                        
                        db.CommandText = insertSql;
                        db.Parameters.Clear();
                        db.Parameters.Add("@VersionId", checkStandard.Version_ID);
                        db.Parameters.Add("@WorkItem", checkStandard.WorkItem ?? "");
                        db.Parameters.Add("@SerialNumber", checkStandard.SerialNumber ?? "");
                        db.Parameters.Add("@PlannedFinishDate", (object)checkStandard.PlannedFinishDate ?? DBNull.Value);
                        db.Parameters.Add("@CheckDescription", checkStandard.CheckDescription ?? "");
                        db.Parameters.Add("@CreatedAt", DateTime.Now);
                        db.Parameters.Add("@UpdatedAt", DateTime.Now);
                        
                        db.ExecuteNonQuery();
                        savedCount++;
                    }
                    return savedCount;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"SaveCheckStandards發生錯誤: {ex.Message}");
                    throw;
                }
            }
        }
        
        #endregion
        
        #region 版本狀態更新操作
        
        /// <summary>
        /// 更新版本狀態
        /// </summary>
        /// <param name="versionId">版本ID</param>
        /// <param name="form2Status">Form2狀態（暫存或完成）</param>
        /// <param name="shouldUpdateCurrentStep">是否需要更新CurrentStep</param>
        /// <param name="newCurrentStep">新的CurrentStep值（如果shouldUpdateCurrentStep為true）</param>
        public static void UpdateVersionStatus(string versionId, string form2Status, bool shouldUpdateCurrentStep = false, string newCurrentStep = null)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    string sql;
                    
                    if (shouldUpdateCurrentStep && !string.IsNullOrEmpty(newCurrentStep))
                    {
                        // 同時更新 Form2Status 和 CurrentStep
                        sql = @"
                            UPDATE OFS_SCI_Version 
                            SET Form2Status = @Form2Status, 
                                CurrentStep = @CurrentStep,
                                updated_at = @UpdatedAt
                            WHERE Version_ID = @VersionId";
                        
                        db.CommandText = sql;
                        db.Parameters.Clear();
                        db.Parameters.Add("@Form2Status", form2Status);
                        db.Parameters.Add("@CurrentStep", newCurrentStep);
                        db.Parameters.Add("@UpdatedAt", DateTime.Now);
                        db.Parameters.Add("@VersionId", versionId);
                    }
                    else
                    {
                        // 只更新 Form2Status
                        sql = @"
                            UPDATE OFS_SCI_Version 
                            SET Form2Status = @Form2Status,
                                updated_at = @UpdatedAt
                            WHERE Version_ID = @VersionId";
                        
                        db.CommandText = sql;
                        db.Parameters.Clear();
                        db.Parameters.Add("@Form2Status", form2Status);
                        db.Parameters.Add("@UpdatedAt", DateTime.Now);
                        db.Parameters.Add("@VersionId", versionId);
                    }
                    
                    db.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"UpdateVersionStatus發生錯誤: {ex.Message}");
                    throw;
                }
            }
        }
        
        /// <summary>
        /// 取得當前版本的CurrentStep
        /// </summary>
        /// <param name="versionId">版本ID</param>
        /// <returns>當前的CurrentStep值</returns>
        public static string GetCurrentStepByVersionId(string versionId)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    string sql = "SELECT CurrentStep FROM OFS_SCI_Version WHERE Version_ID = @VersionId";
                    db.CommandText = sql;
                    db.Parameters.Clear();
                    db.Parameters.Add("@VersionId", versionId);
                    
                    var result = db.GetTable();
                    return result != null ? Convert.ToInt32(result).ToString() : "1";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"GetCurrentStepByVersionId發生錯誤: {ex.Message}");
                    return "1"; // 預設值
                }
            }
        }
        
        #endregion
        
        #region 資料讀取操作
        
        /// <summary>
        /// 根據 Version_ID 讀取計畫期程
        /// </summary>
        /// <param name="versionId">版本ID</param>
        /// <returns>計畫期程資料</returns>
        public static (DateTime? startTime, DateTime? endTime) GetProjectScheduleByVersionId(string versionId)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    string sql = @"
                        SELECT StartTime, EndTime 
                        FROM OFS_SCI_Application_Main 
                        WHERE Version_ID = @VersionId";
                    
                    db.CommandText = sql;
                    db.Parameters.Clear();
                    db.Parameters.Add("@VersionId", versionId);
                    
                    DataTable dt = db.GetTable();
                    if (dt.Rows.Count > 0)
                    {
                        var row = dt.Rows[0];
                        DateTime? startTime = row["StartTime"] != DBNull.Value ? Convert.ToDateTime(row["StartTime"]) : (DateTime?)null;
                        DateTime? endTime = row["EndTime"] != DBNull.Value ? Convert.ToDateTime(row["EndTime"]) : (DateTime?)null;
                        return (startTime, endTime);
                    }
                    
                    return (null, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"GetProjectScheduleByVersionId發生錯誤: {ex.Message}");
                    return (null, null);
                }
            }
        }
        
        /// <summary>
        /// 根據 Version_ID 讀取所有工作項目
        /// </summary>
        /// <param name="versionId">版本ID</param>
        /// <returns>工作項目列表</returns>
        public static List<OFS_SCI_WorkSch_Main> GetWorkItemsByVersionId(string versionId)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    string sql = @"
                        SELECT Version_ID, WorkItem_id, WorkName, StartMonth, EndMonth, 
                               Weighting, InvestMonth, IsOutsourced
                        FROM OFS_SCI_WorkSch_Main 
                        WHERE Version_ID = @VersionId
                        ORDER BY WorkItem_id";
                    
                    db.CommandText = sql;
                    db.Parameters.Clear();
                    db.Parameters.Add("@VersionId", versionId);
                    
                    DataTable dt = db.GetTable();
                    var result = new List<OFS_SCI_WorkSch_Main>();
                    
                    foreach (DataRow row in dt.Rows)
                    {
                        var workItem = new OFS_SCI_WorkSch_Main
                        {
                            Version_ID = row["Version_ID"].ToString(),
                            WorkItem_id = row["WorkItem_id"].ToString(),
                            WorkName = row["WorkName"].ToString(),
                            StartMonth = row["StartMonth"] != DBNull.Value ? Convert.ToInt32(row["StartMonth"]) : (int?)null,
                            EndMonth = row["EndMonth"] != DBNull.Value ? Convert.ToInt32(row["EndMonth"]) : (int?)null,
                            Weighting = row["Weighting"] != DBNull.Value ? Convert.ToDecimal(row["Weighting"]) : (decimal?)null,
                            InvestMonth = row["InvestMonth"] != DBNull.Value ? Convert.ToDecimal(row["InvestMonth"]) : (decimal?)null,
                            IsOutsourced = row["IsOutsourced"] != DBNull.Value ? Convert.ToBoolean(row["IsOutsourced"]) : (bool?)null
                        };
                        result.Add(workItem);
                    }
                    
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"GetWorkItemsByVersionId發生錯誤: {ex.Message}");
                    return new List<OFS_SCI_WorkSch_Main>();
                }
            }
        }
        
        /// <summary>
        /// 根據 Version_ID 讀取所有查核標準
        /// </summary>
        /// <param name="versionId">版本ID</param>
        /// <returns>查核標準列表</returns>
        public static List<OFS_SCI_WorkSch_CheckStandard> GetCheckStandardsByVersionId(string versionId)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    string sql = @"
                        SELECT Id, Version_ID, WorkItem, SerialNumber, PlannedFinishDate, 
                               CheckDescription, CreatedAt, UpdatedAt
                        FROM OFS_SCI_WorkSch_CheckStandard 
                        WHERE Version_ID = @VersionId
                        ORDER BY SerialNumber";
                    
                    db.CommandText = sql;
                    db.Parameters.Clear();
                    db.Parameters.Add("@VersionId", versionId);
                    
                    DataTable dt = db.GetTable();
                    var result = new List<OFS_SCI_WorkSch_CheckStandard>();
                    
                    foreach (DataRow row in dt.Rows)
                    {
                        var checkStandard = new OFS_SCI_WorkSch_CheckStandard
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            Version_ID = row["Version_ID"].ToString(),
                            WorkItem = row["WorkItem"].ToString(),
                            SerialNumber = row["SerialNumber"].ToString(),
                            PlannedFinishDate = row["PlannedFinishDate"] != DBNull.Value ? Convert.ToDateTime(row["PlannedFinishDate"]) : (DateTime?)null,
                            CheckDescription = row["CheckDescription"].ToString(),
                            CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                            UpdatedAt = row["UpdatedAt"] != DBNull.Value ? Convert.ToDateTime(row["UpdatedAt"]) : (DateTime?)null
                        };
                        result.Add(checkStandard);
                    }
                    
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"GetCheckStandardsByVersionId發生錯誤: {ex.Message}");
                    return new List<OFS_SCI_WorkSch_CheckStandard>();
                }
            }
        }
        
        #endregion
        
        #region 檔案上傳操作
        
        /// <summary>
        /// 根據 Version_ID 和 FileCode 查詢上傳檔案記錄
        /// </summary>
        /// <param name="versionId">版本ID</param>
        /// <param name="fileCode">檔案代碼</param>
        /// <returns>檔案記錄列表</returns>
        public static List<OFS_SCI_UploadFile> GetUploadFilesByVersionIdAndFileCode(string versionId, string fileCode)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    string sql = @"
                        SELECT ID, Version_ID, FileCode, FileName, TemplatePath, Statuses
                        FROM OFS_SCI_UploadFile 
                        WHERE Version_ID = @VersionId 
                        AND FileCode = @FileCode 
                        AND Statuses = 'Active'";
                    
                    db.CommandText = sql;
                    db.Parameters.Clear();
                    db.Parameters.Add("@VersionId", versionId);
                    db.Parameters.Add("@FileCode", fileCode);
                    
                    DataTable dt = db.GetTable();
                    var result = new List<OFS_SCI_UploadFile>();
                    
                    foreach (DataRow row in dt.Rows)
                    {
                        var uploadFile = new OFS_SCI_UploadFile
                        {
                            ID = Convert.ToInt32(row["ID"]),
                            Version_ID = row["Version_ID"].ToString(),
                            FileCode = row["FileCode"].ToString(),
                            FileName = row["FileName"].ToString(),
                            TemplatePath = row["TemplatePath"].ToString(),
                            Statuses = row["Statuses"].ToString()
                        };
                        result.Add(uploadFile);
                    }
                    
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"GetUploadFilesByVersionIdAndFileCode發生錯誤: {ex.Message}");
                    throw;
                }
            }
        }
        
        /// <summary>
        /// 新增檔案上傳記錄
        /// </summary>
        /// <param name="uploadFile">檔案記錄</param>
        /// <returns>新增的記錄數量</returns>
        public static void InsertUploadFile(OFS_SCI_UploadFile uploadFile)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    string sql = @"
                        INSERT INTO OFS_SCI_UploadFile 
                        (Version_ID, FileCode, FileName, TemplatePath, Statuses)
                        VALUES 
                        (@VersionId, @FileCode, @FileName, @TemplatePath, @Statuses)";
                    
                    db.CommandText = sql;
                    db.Parameters.Clear();
                    db.Parameters.Add("@VersionId", uploadFile.Version_ID);
                    db.Parameters.Add("@FileCode", uploadFile.FileCode);
                    db.Parameters.Add("@FileName", uploadFile.FileName);
                    db.Parameters.Add("@TemplatePath", uploadFile.TemplatePath);
                    db.Parameters.Add("@Statuses", uploadFile.Statuses);
                    db.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"InsertUploadFile發生錯誤: {ex.Message}");
                    throw;
                }
            }
        }
        
        /// <summary>
        /// 刪除檔案上傳記錄
        /// </summary>
        /// <param name="versionId">版本ID</param>
        /// <param name="fileCode">檔案代碼</param>
        /// <returns>刪除的記錄數量</returns>
        public static void DeleteUploadFile(string versionId, string fileCode)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    string sql = @"
                        UPDATE OFS_SCI_UploadFile 
                        SET Statuses = 'Deleted'
                        WHERE Version_ID = @VersionId 
                        AND FileCode = @FileCode";
                    
                    db.CommandText = sql;
                    db.Parameters.Clear();
                    db.Parameters.Add("@VersionId", versionId);
                    db.Parameters.Add("@FileCode", fileCode);
                    
                    db.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"DeleteUploadFile發生錯誤: {ex.Message}");
                    throw;
                }
            }
        }
        
        #endregion
        
    }
}