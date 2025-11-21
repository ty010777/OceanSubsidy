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
        /// <param name="projectId">ProjectID</param>
        /// <param name="startTime">計畫開始時間</param>
        /// <param name="endTime">計畫結束時間</param>
        /// <returns>是否成功</returns>
        public static void UpdateProjectSchedule(string projectId, DateTime? startTime, DateTime? endTime)
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
                    WHERE ProjectID = @ProjectId";

                    
                    db.Parameters.Clear();
                    db.Parameters.Add("@StartTime", startTime);
                    db.Parameters.Add("@EndTime", endTime);
                    db.Parameters.Add("@UpdatedAt", DateTime.Now);
                    db.Parameters.Add("@ProjectId", projectId);
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
        /// <param name="projectId">ProjectID</param>
        /// <param name="workItems">工作項目資料列表</param>
        /// <returns>成功儲存的筆數</returns>
        public static int SaveAllWorkItems(string ProjectID, List<OFS_SCI_WorkSch_Main> workItems)
        {
            using (DbHelper db = new DbHelper())
            {

                try
                {
                    // 先刪除該ProjectID的所有舊資料
                    string deleteSql = "DELETE FROM OFS_SCI_WorkSch_Main WHERE ProjectID = @ProjectID";
                    db.CommandText = deleteSql;
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", ProjectID);
                    db.ExecuteNonQuery();

                    int savedCount = 0;

                    foreach (var item in workItems)
                    {
                        // 插入新的工作項目
                        string insertSql = @"
                            INSERT INTO OFS_SCI_WorkSch_Main
                            (ProjectID, WorkItem_id, WorkName, StartYear, StartMonth, EndYear, EndMonth, Weighting, InvestMonth, IsOutsourced)
                            VALUES
                            (@ProjectId, @WorkItemId, @WorkName, @StartYear, @StartMonth, @EndYear, @EndMonth, @Weighting, @InvestMonth, @IsOutsourced)";
                        
                        db.CommandText = insertSql;
                        db.Parameters.Clear();
                        db.Parameters.Add("@ProjectId", ProjectID);
                        db.Parameters.Add("@WorkItemId", item.WorkItem_id);
                        db.Parameters.Add("@WorkName", item.WorkName ?? "");
                        db.Parameters.Add("@StartYear", (object)item.StartYear ?? DBNull.Value);
                        db.Parameters.Add("@StartMonth", (object)item.StartMonth ?? DBNull.Value);
                        db.Parameters.Add("@EndYear", (object)item.EndYear ?? DBNull.Value);
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
        /// <param name="projectId">ProjectID</param>
        /// <param name="itemCode">項目代碼 (A, A1, B, B1等)</param>
        /// <returns>完整的WorkItem_id</returns>
        public static string GenerateWorkItemId(string projectId, string itemCode)
        {
            return $"{projectId}_{itemCode}";
        }
        
        /// <summary>
        /// 從WorkItem_id中提取項目代碼
        /// </summary>
        /// <param name="workItemId">WorkItem_id (格式: {ProjectID}_A)</param>
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
        /// 從WorkItem_id中提取ProjectID
        /// </summary>
        /// <param name="workItemId">WorkItem_id (格式: {ProjectID}_A)</param>
        /// <returns>ProjectID</returns>
        public static string ExtractProjectIDFromWorkItemId(string workItemId)
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
                    // 取得第一筆資料的 ProjectID 來刪除舊資料
                    string projectId = checkStandards.First().ProjectID;
                    
                    // 先刪除該ProjectID的所有舊資料
                    string deleteSql = "DELETE FROM OFS_SCI_WorkSch_CheckStandard WHERE ProjectID = @ProjectID";
                    db.CommandText = deleteSql;
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", projectId);
                    db.ExecuteNonQuery();

                    int savedCount = 0;

                    foreach (var checkStandard in checkStandards)
                    {
                        // 插入新的查核標準資料
                        string insertSql = @"
                            INSERT INTO OFS_SCI_WorkSch_CheckStandard 
                            (ProjectID, WorkItem, SerialNumber, PlannedFinishDate, CheckDescription, CreatedAt, UpdatedAt)
                            VALUES 
                            (@ProjectId, @WorkItem, @SerialNumber, @PlannedFinishDate, @CheckDescription, @CreatedAt, @UpdatedAt)";
                        
                        db.CommandText = insertSql;
                        db.Parameters.Clear();
                        db.Parameters.Add("@ProjectId", checkStandard.ProjectID);
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
        /// <param name="projectId">ProjectID</param>
        /// <param name="form2Status">Form2狀態（暫存或完成）</param>
        /// <param name="shouldUpdateCurrentStep">是否需要更新CurrentStep</param>
        /// <param name="newCurrentStep">新的CurrentStep值（如果shouldUpdateCurrentStep為true）</param>
        public static void UpdateVersionStatus(string projectId, string form2Status, bool shouldUpdateCurrentStep = false, string newCurrentStep = null)
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
                            UPDATE OFS_SCI_Project_Main 
                            SET Form2Status = @Form2Status, 
                                CurrentStep = @CurrentStep,
                                updated_at = @UpdatedAt
                            WHERE ProjectID = @ProjectId";
                        
                        db.CommandText = sql;
                        db.Parameters.Clear();
                        db.Parameters.Add("@Form2Status", form2Status);
                        db.Parameters.Add("@CurrentStep", newCurrentStep);
                        db.Parameters.Add("@UpdatedAt", DateTime.Now);
                        db.Parameters.Add("@ProjectId", projectId);
                    }
                    else
                    {
                        // 只更新 Form2Status
                        sql = @"
                            UPDATE OFS_SCI_Project_Main 
                            SET Form2Status = @Form2Status,
                                updated_at = @UpdatedAt
                            WHERE ProjectID = @ProjectId";
                        
                        db.CommandText = sql;
                        db.Parameters.Clear();
                        db.Parameters.Add("@Form2Status", form2Status);
                        db.Parameters.Add("@UpdatedAt", DateTime.Now);
                        db.Parameters.Add("@ProjectId", projectId);
                    }
                    
                    db.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"UpdateStatus發生錯誤: {ex.Message}");
                    throw;
                }
            }
        }
        
        /// <summary>
        /// 取得當前版本的CurrentStep
        /// </summary>
        /// <param name="projectId">ProjectID</param>
        /// <returns>當前的CurrentStep值</returns>
        public static string GetCurrentStepByProjectID(string projectId)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    string sql = "SELECT CurrentStep FROM OFS_SCI_Project_Main WHERE ProjectID = @ProjectId";
                    db.CommandText = sql;
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectId", projectId);
                    
                    var result = db.GetTable();
                    return result.Rows[0]["CurrentStep"].ToString();
                }
                catch (Exception ex)
                {
                    return "1"; // 預設值
                }
            }
        }
        
        #endregion
        
        #region 資料讀取操作
        
        /// <summary>
        /// 根據 ProjectID 讀取計畫期程
        /// </summary>
        /// <param name="projectId">ProjectID</param>
        /// <returns>計畫期程資料</returns>
        public static (DateTime? startTime, DateTime? endTime) GetProjectScheduleByProjectID(string projectId)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    string sql = @"
                        SELECT StartTime, EndTime 
                        FROM OFS_SCI_Application_Main 
                        WHERE ProjectID = @ProjectId";
                    
                    db.CommandText = sql;
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectId", projectId);
                    
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
                    return (null, null);
                }
            }
        }
        
        /// <summary>
        /// 根據 ProjectID 讀取所有工作項目
        /// </summary>
        /// <param name="projectId">ProjectID</param>
        /// <returns>工作項目列表</returns>
        public static List<OFS_SCI_WorkSch_Main> GetWorkItemsByProjectID(string projectId)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    string sql = @"
                        SELECT ProjectID, WorkItem_id, WorkName, StartYear, StartMonth, EndYear, EndMonth,
                               Weighting, InvestMonth, IsOutsourced
                        FROM OFS_SCI_WorkSch_Main
                        WHERE ProjectID = @ProjectId
                        ORDER BY WorkItem_id";
                    
                    db.CommandText = sql;
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectId", projectId);
                    
                    DataTable dt = db.GetTable();
                    var result = new List<OFS_SCI_WorkSch_Main>();
                    
                    foreach (DataRow row in dt.Rows)
                    {
                        var workItem = new OFS_SCI_WorkSch_Main
                        {
                            ProjectID = row["ProjectID"].ToString(),
                            WorkItem_id = row["WorkItem_id"].ToString(),
                            WorkName = row["WorkName"].ToString(),
                            StartYear = row["StartYear"] != DBNull.Value ? Convert.ToInt32(row["StartYear"]) : (int?)null,
                            StartMonth = row["StartMonth"] != DBNull.Value ? Convert.ToInt32(row["StartMonth"]) : (int?)null,
                            EndYear = row["EndYear"] != DBNull.Value ? Convert.ToInt32(row["EndYear"]) : (int?)null,
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
                    return new List<OFS_SCI_WorkSch_Main>();
                }
            }
        }
        
        /// <summary>
        /// 根據 ProjectID 讀取所有查核標準
        /// </summary>
        /// <param name="projectId">ProjectID</param>
        /// <returns>查核標準列表</returns>
        public static List<OFS_SCI_WorkSch_CheckStandard> GetCheckStandardsByProjectID(string projectId)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    string sql = @"
                        SELECT Id, ProjectID, WorkItem, SerialNumber, PlannedFinishDate, 
                               CheckDescription, CreatedAt, UpdatedAt
                        FROM OFS_SCI_WorkSch_CheckStandard 
                        WHERE ProjectID = @ProjectId
                        ORDER BY SerialNumber";
                    
                    db.CommandText = sql;
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectId", projectId);
                    
                    DataTable dt = db.GetTable();
                    var result = new List<OFS_SCI_WorkSch_CheckStandard>();
                    
                    foreach (DataRow row in dt.Rows)
                    {
                        var checkStandard = new OFS_SCI_WorkSch_CheckStandard
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            ProjectID = row["ProjectID"].ToString(),
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
                    return new List<OFS_SCI_WorkSch_CheckStandard>();
                }
            }
        }
        
        public static DataTable GetWorkScheduleWithCheckStandard(string projectId)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    string sql = @"
                SELECT 
                    m.ProjectID,
                    m.WorkItem_id,
                    m.WorkName,
                    m.StartYear,
                    m.StartMonth,
                    m.EndYear,
                    m.EndMonth,
                    m.Weighting,
                    m.InvestMonth,
                    m.IsOutsourced,
                    c.Id AS CheckStandardId,
                    c.WorkItem,
                    c.SerialNumber,
                    c.PlannedFinishDate,
                    c.CheckDescription,
                    c.ActFinishTime,
                    c.DelayReason,
                    c.ImprovedWay,
                    c.IsFinish,
                    c.CreatedAt,
                    c.UpdatedAt
                FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_WorkSch_Main] m
                INNER JOIN [OCA_OceanSubsidy].[dbo].[OFS_SCI_WorkSch_CheckStandard] c
                    ON m.ProjectID = c.ProjectID
                    AND CHARINDEX(c.WorkItem, m.WorkItem_id) > 0
                WHERE m.ProjectID = @ProjectId
                ORDER BY m.WorkItem_id, c.SerialNumber;
            ";

                    db.CommandText = sql;
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectId", projectId);

                    // 直接回傳 DataTable
                    DataTable dt = db.GetTable();
                    return dt;
                }
                catch (Exception ex)
                {
                    // 可視情況記錄 log
                    return new DataTable();
                }
            }
        }

        #endregion
        
        #region 檔案上傳操作
        
        /// <summary>
        /// 根據 ProjectID 和 FileCode 查詢上傳檔案記錄
        /// </summary>
        /// <param name="projectId">ProjectID</param>
        /// <param name="fileCode">檔案代碼</param>
        /// <returns>檔案記錄列表</returns>
        public static List<OFS_SCI_UploadFile> GetUploadFilesByProjectIDAndFileCode(string projectId, string fileCode)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    string sql = @"
                        SELECT ID, ProjectID, FileCode, FileName, TemplatePath
                        FROM OFS_SCI_UploadFile 
                        WHERE ProjectID = @ProjectId 
                        AND FileCode = @FileCode";
                    
                    db.CommandText = sql;
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectId", projectId);
                    db.Parameters.Add("@FileCode", fileCode);
                    
                    DataTable dt = db.GetTable();
                    var result = new List<OFS_SCI_UploadFile>();
                    
                    foreach (DataRow row in dt.Rows)
                    {
                        var uploadFile = new OFS_SCI_UploadFile
                        {
                            ID = Convert.ToInt32(row["ID"]),
                            ProjectID = row["ProjectID"].ToString(),
                            FileCode = row["FileCode"].ToString(),
                            FileName = row["FileName"].ToString(),
                            TemplatePath = row["TemplatePath"].ToString()
                        };
                        result.Add(uploadFile);
                    }
                    
                    return result;
                }
                catch (Exception ex)
                {
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
                        (ProjectID, FileCode, FileName, TemplatePath)
                        VALUES 
                        (@ProjectId, @FileCode, @FileName, @TemplatePath)";
                    
                    db.CommandText = sql;
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectId", uploadFile.ProjectID);
                    db.Parameters.Add("@FileCode", uploadFile.FileCode);
                    db.Parameters.Add("@FileName", uploadFile.FileName);
                    db.Parameters.Add("@TemplatePath", uploadFile.TemplatePath);
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
        /// <param name="projectId">ProjectID</param>
        /// <param name="fileCode">檔案代碼</param>
        /// <returns>刪除的記錄數量</returns>
        public static void DeleteUploadFile(string projectId, string fileCode)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    string sql = @"
                        DELETE FROM OFS_SCI_UploadFile 
                        WHERE ProjectID = @ProjectId 
                        AND FileCode = @FileCode";
                    
                    db.CommandText = sql;
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectId", projectId);
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
        
        /// <summary>
        /// 取得指定版本的表單狀態
        /// </summary>
        /// <param name="projectId">ProjectID</param>
        /// <param name="statusColumn">狀態欄位名稱 (Form1Status, Form2Status, etc.)</param>
        /// <returns>狀態值</returns>
        public static string GetFormStatusByProjectID(string projectId, string statusColumn)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    string sql = $@"
                        SELECT {statusColumn}
                        FROM OFS_SCI_Project_Main 
                        WHERE ProjectID = @ProjectId";
                    
                    db.CommandText = sql;
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectId", projectId);

                    DataTable dt = db.GetTable();
                    if (dt.Rows.Count > 0)
                    {
                        return dt.Rows[0][statusColumn]?.ToString() ?? "";
                    }
                    return "";
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
        }
        
        #endregion

        #region 補助案類型查詢操作

        /// <summary>
        /// 查詢目前有效的 SCI 補助案計畫結束日期
        /// </summary>
        /// <returns>計畫結束日期 (DateTime)，若無符合條件則回傳 null</returns>
        public static DateTime? GetCurrentSCIGrantTypePlanEndDate()
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    string sql = @"
                        SELECT TOP(1) PlanEndDate
                        FROM OFS_GrantType
                        WHERE TypeCode = @TypeCode
                        AND ApplyStartDate < @Today
                        AND ApplyEndDate > @Today
                        AND PlanEndDate  is not NULL
                        ORDER BY ApplyStartDate DESC";

                    db.CommandText = sql;
                    db.Parameters.Clear();
                    db.Parameters.Add("@TypeCode", "SCI");
                    db.Parameters.Add("@Today", DateTime.Today);

                    DataTable dt = db.GetTable();
                    if (dt.Rows.Count > 0 && dt.Rows[0]["PlanEndDate"] != DBNull.Value)
                    {
                        return Convert.ToDateTime(dt.Rows[0]["PlanEndDate"]);
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"GetCurrentSCIGrantTypePlanEndDate 發生錯誤: {ex.Message}");
                    return null;
                }
            }
        }

        /// <summary>
        /// 查詢專案狀態
        /// </summary>
        /// <param name="projectId">計畫ID</param>
        /// <returns>專案狀態 (例如: "尚未提送", "審查中"等)，若無資料則回傳空字串</returns>
        public static string GetProjectStatus(string projectId)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    string sql = @"
                        SELECT Statuses
                        FROM OFS_SCI_Project_Main
                        WHERE ProjectID = @ProjectId";

                    db.CommandText = sql;
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectId", projectId);

                    DataTable dt = db.GetTable();
                    if (dt.Rows.Count > 0 && dt.Rows[0]["Statuses"] != DBNull.Value)
                    {
                        return dt.Rows[0]["Statuses"].ToString();
                    }

                    return "";
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"GetProjectStatus 發生錯誤: {ex.Message}");
                    return "";
                }
            }
        }

        #endregion

        #region 查核點查詢操作
        
        /// <summary>
        /// 根據 ProjectID 和年月份查詢查核點資料
        /// </summary>
        /// <param name="projectId">計畫ID</param>
        /// <param name="yearMonth">年月份 (格式: 114年7月)</param>
        /// <returns>查核點列表</returns>
        public static List<OFS_SCI_WorkSch_CheckStandard> GetCheckStandardsByProjectIdAndMonth(string projectId, string yearMonth)
        {
            var result = new List<OFS_SCI_WorkSch_CheckStandard>();
            
            if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(yearMonth))
                return result;

            using (DbHelper db = new DbHelper())
            {
                try
                {
                    // 將年月份轉換為日期範圍 (例如: 114年7月 -> 2025-07-01 到 2025-07-31)
                    var (startDate, endDate) = ConvertYearMonthToDateRange(yearMonth);
                    
                    if (!startDate.HasValue || !endDate.HasValue)
                        return result;

                    string sql = @"
                        SELECT * FROM OFS_SCI_WorkSch_CheckStandard 
                        WHERE ProjectID = @ProjectID 
                        AND PlannedFinishDate >= @StartDate 
                        AND PlannedFinishDate <= @EndDate
                        ORDER BY PlannedFinishDate, SerialNumber";

                    db.CommandText = sql;
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", projectId);
                    db.Parameters.Add("@StartDate", startDate.Value);
                    db.Parameters.Add("@EndDate", endDate.Value);

                    DataTable dt = db.GetTable();

                    foreach (DataRow row in dt.Rows)
                    {
                        var checkStandard = new OFS_SCI_WorkSch_CheckStandard
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            ProjectID = row["ProjectID"]?.ToString(),
                            WorkItem = row["WorkItem"]?.ToString(),
                            SerialNumber = row["SerialNumber"]?.ToString(),
                            PlannedFinishDate = row["PlannedFinishDate"] != DBNull.Value ? 
                                Convert.ToDateTime(row["PlannedFinishDate"]) : (DateTime?)null,
                            CheckDescription = row["CheckDescription"]?.ToString(),
                            IsFinish = row["IsFinish"] != DBNull.Value ? 
                                Convert.ToInt32(row["IsFinish"]) : (int?)0,
                            DelayReason = row["DelayReason"]?.ToString(),
                            ImprovedWay = row["ImprovedWay"]?.ToString(),
                            ActFinishTime = row["ActFinishTime"] != DBNull.Value ? 
                                Convert.ToDateTime(row["ActFinishTime"]) : (DateTime?)null,
                            CreatedAt = row["CreatedAt"] != DBNull.Value ? 
                                Convert.ToDateTime(row["CreatedAt"]) : DateTime.Now,
                            UpdatedAt = row["UpdatedAt"] != DBNull.Value ? 
                                Convert.ToDateTime(row["UpdatedAt"]) : (DateTime?)null
                        };
                        
                        result.Add(checkStandard);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"GetCheckStandardsByProjectIdAndMonth 發生錯誤: {ex.Message}");
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// 將年月份字串轉換為日期範圍
        /// </summary>
        /// <param name="yearMonth">年月份 (格式: 114年7月)</param>
        /// <returns>開始日期和結束日期</returns>
        private static (DateTime? startDate, DateTime? endDate) ConvertYearMonthToDateRange(string yearMonth)
        {
            try
            {
                // 解析 "114年7月" 格式
                if (yearMonth.Contains("年") && yearMonth.Contains("月"))
                {
                    var parts = yearMonth.Replace("年", "/").Replace("月", "").Split('/');
                    if (parts.Length >= 2)
                    {
                        int minguoYear = int.Parse(parts[0]);
                        int month = int.Parse(parts[1]);
                        
                        // 民國年轉西元年
                        int westernYear = minguoYear + 1911;
                        
                        // 該月的第一天和最後一天
                        DateTime startDate = new DateTime(westernYear, month, 1);
                        DateTime endDate = startDate.AddMonths(1).AddDays(-1);
                        
                        return (startDate, endDate);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ConvertYearMonthToDateRange 發生錯誤: {ex.Message}");
            }
            
            return (null, null);
        }
        
        /// <summary>
        /// 計算專案年度目標達成率
        /// </summary>
        /// <param name="projectId">計畫ID</param>
        /// <returns>達成率百分比，四捨五入到小數點第二位</returns>
        public static decimal GetProjectAchievementRate(string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
                return 0;

            using (DbHelper db = new DbHelper())
            {
                try
                {
                    string sql = @"
                        SELECT 
                            COUNT(*) AS TotalCount,
                            SUM(CASE WHEN IsFinish = 3 THEN 1 ELSE 0 END) AS FinishCount
                        FROM OFS_SCI_WorkSch_CheckStandard
                        WHERE ProjectID = @ProjectID";

                    db.CommandText = sql;
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", projectId);

                    DataTable dt = db.GetTable();

                    if (dt.Rows.Count > 0)
                    {
                        var row = dt.Rows[0];
                        int totalCount = Convert.ToInt32(row["TotalCount"]);
                        int finishCount = Convert.ToInt32(row["FinishCount"]);

                        if (totalCount == 0)
                            return 0;

                        // 計算百分比並四捨五入到小數點第二位
                        decimal rate = (decimal)finishCount / totalCount * 100;
                        return Math.Round(rate, 2, MidpointRounding.AwayFromZero);
                    }

                    return 0;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"GetProjectAchievementRate 發生錯誤: {ex.Message}");
                    return 0;
                }
            }
        }
        
        #endregion
        
        #region 查核點狀態更新操作
        
        /// <summary>
        /// 更新查核點狀態
        /// </summary>
        /// <param name="checkStandardId">查核點ID</param>
        /// <param name="isFinish">完成狀態 (1:未完成, 2:部分完成, 3:完成)</param>
        /// <param name="delayReason">落後原因</param>
        /// <param name="improvedWay">改善措施</param>
        /// <param name="actFinishTime">實際完成時間</param>
        public static void UpdateCheckStandardStatus(int checkStandardId, int? isFinish, string delayReason, string improvedWay, DateTime? actFinishTime)
        {
            using (DbHelper db = new DbHelper())
            {
                try
                {
                    string sql = @"
                        UPDATE OFS_SCI_WorkSch_CheckStandard 
                        SET IsFinish = @IsFinish,
                            DelayReason = @DelayReason,
                            ImprovedWay = @ImprovedWay,
                            ActFinishTime = @ActFinishTime,
                            UpdatedAt = @UpdatedAt
                        WHERE Id = @Id";

                    db.CommandText = sql;
                    db.Parameters.Clear();
                    db.Parameters.Add("@IsFinish", isFinish);
                    db.Parameters.Add("@DelayReason", delayReason ?? "");
                    db.Parameters.Add("@ImprovedWay", improvedWay ?? "");
                    db.Parameters.Add("@ActFinishTime", (object)actFinishTime ?? DBNull.Value);
                    db.Parameters.Add("@UpdatedAt", DateTime.Now);
                    db.Parameters.Add("@Id", checkStandardId);

                    db.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"UpdateCheckStandardStatus 發生錯誤: {ex.Message}");
                    throw;
                }
            }
        }
        
        /// <summary>
        /// 批次更新查核點狀態
        /// </summary>
        /// <param name="checkStandardUpdates">查核點更新資料列表</param>
        public static void BatchUpdateCheckStandardStatus(List<OFS_SCI_WorkSch_CheckStandard> checkStandardUpdates)
        {
            if (checkStandardUpdates == null || !checkStandardUpdates.Any())
                return;

            using (DbHelper db = new DbHelper())
            {
                db.BeginTrans();
                try
                {
                    foreach (var update in checkStandardUpdates)
                    {
                        string sql = @"
                            UPDATE OFS_SCI_WorkSch_CheckStandard 
                            SET IsFinish = @IsFinish,
                                DelayReason = @DelayReason,
                                ImprovedWay = @ImprovedWay,
                                ActFinishTime = @ActFinishTime,
                                UpdatedAt = @UpdatedAt
                            WHERE Id = @Id";

                        db.CommandText = sql;
                        db.Parameters.Clear();

                        // 直接使用 Entity 的屬性，類型安全且清晰
                        db.Parameters.Add("@IsFinish", update.IsFinish ?? 0);
                        db.Parameters.Add("@DelayReason", update.DelayReason ?? "");
                        db.Parameters.Add("@ImprovedWay", update.ImprovedWay ?? "");
                        db.Parameters.Add("@ActFinishTime", (object)update.ActFinishTime ?? DBNull.Value);
                        db.Parameters.Add("@UpdatedAt", DateTime.Now);
                        db.Parameters.Add("@Id", update.Id);

                        db.ExecuteNonQuery();
                    }
                    
                    db.Commit();
                }
                catch (Exception ex)
                {
                    db.Rollback();
                    System.Diagnostics.Debug.WriteLine($"BatchUpdateCheckStandardStatus 發生錯誤: {ex.Message}");
                    throw;
                }
            }
        }
        
        #endregion
        
    }
}