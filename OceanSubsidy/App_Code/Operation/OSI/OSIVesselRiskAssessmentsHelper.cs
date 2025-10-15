using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using GS.App;
using GS.Data;
using GS.Data.Sql;
using GS.Extension;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// OSIVesselRiskAssessmentsHelper 的摘要描述
/// </summary>
public class OSIVesselRiskAssessmentsHelper
{
    public OSIVesselRiskAssessmentsHelper()
    {
       
    }

    /// <summary>
    /// 查詢所有
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryAll()
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [AssessmentId]
    ,[Investigator]
    ,[Unit]
    ,[Title]
    ,[FormDate]
    ,[StartDate]
    ,[StartTime]
    ,[StartRemark]
    ,[EndDate]
    ,[EndTime]
    ,[EndRemark]
    ,[DurationDays]
    ,[SurveyAreaName]
    ,[VoyagePlanAndOperations]
    ,[Q_IsOperationInSensitiveArea]
    ,[Q_IsStayTimeMinimized]
    ,[Q_HasReducedOrRelocatedStation]
    ,[Q_KnowsReportingProcedure]
    ,[Q_HasStrongInterferenceContingencyPlan]
    ,[LastUpdated]
    ,[LastUpdatedBy]
    ,[IsValid]
FROM [OCA_OceanSubsidy].[dbo].[OSI_VesselRiskAssessments]
WHERE IsValid = 1
";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 查詢 By ID (Entity)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static OSI_VesselRiskAssessments QueryByID(int id)
    {
        var table = QueryByIDTable(id.ToString());
        if (table != null && table.Rows.Count > 0)
        {
            var row = table.Rows[0];
            return new OSI_VesselRiskAssessments
            {
                AssessmentId = Convert.ToInt32(row["AssessmentId"]),
                Investigator = row["Investigator"].ToString(),
                Unit = row["Unit"].ToString(),
                Title = row["Title"].ToString(),
                FormDate = Convert.ToDateTime(row["FormDate"]),
                StartDate = Convert.ToDateTime(row["StartDate"]),
                StartTime = row["StartTime"].ToString(),
                StartRemark = row["StartRemark"].ToString(),
                EndDate = Convert.ToDateTime(row["EndDate"]),
                EndTime = row["EndTime"].ToString(),
                EndRemark = row["EndRemark"].ToString(),
                DurationDays = Convert.ToInt32(row["DurationDays"]),
                SurveyAreaName = row["SurveyAreaName"].ToString(),
                VoyagePlanAndOperations = row["VoyagePlanAndOperations"].ToString(),
                Q_IsOperationInSensitiveArea = Convert.ToInt32(row["Q_IsOperationInSensitiveArea"]),
                Q_IsStayTimeMinimized = Convert.ToInt32(row["Q_IsStayTimeMinimized"]),
                Q_HasReducedOrRelocatedStation = Convert.ToInt32(row["Q_HasReducedOrRelocatedStation"]),
                Q_KnowsReportingProcedure = Convert.ToInt32(row["Q_KnowsReportingProcedure"]),
                Q_HasStrongInterferenceContingencyPlan = Convert.ToInt32(row["Q_HasStrongInterferenceContingencyPlan"]),
                LastUpdated = Convert.ToDateTime(row["LastUpdated"]),
                LastUpdatedBy = row["LastUpdatedBy"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["LastUpdatedBy"]),
                IsValid = Convert.ToBoolean(row["IsValid"])
            };
        }
        return null;
    }

    /// <summary>
    /// 查詢 By ID (Table)
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryByIDTable(string id)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [AssessmentId]
    ,[Investigator]
    ,[Unit]
    ,[Title]
    ,[FormDate]
    ,[StartDate]
    ,[StartTime]
    ,[StartRemark]
    ,[EndDate]
    ,[EndTime]
    ,[EndRemark]
    ,[DurationDays]
    ,[SurveyAreaName]
    ,[VoyagePlanAndOperations]
    ,[Q_IsOperationInSensitiveArea]
    ,[Q_IsStayTimeMinimized]
    ,[Q_HasReducedOrRelocatedStation]
    ,[Q_KnowsReportingProcedure]
    ,[Q_HasStrongInterferenceContingencyPlan]
    ,[LastUpdated]
    ,[LastUpdatedBy]
    ,[IsValid]
FROM [OCA_OceanSubsidy].[dbo].[OSI_VesselRiskAssessments]
WHERE IsValid = 1
AND AssessmentId = @AssessmentId
";
        db.Parameters.Clear();
        db.Parameters.Add("@AssessmentId", id);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢含篩選條件
    /// </summary>
    /// <param name="startYear">起始年度（民國年）</param>
    /// <param name="endYear">結束年度（民國年）</param>
    /// <param name="keyword">關鍵字（搜尋計畫名稱、申請單位）</param>
    /// <returns></returns>
    public static GisTable QueryWithFilter(int startYear, int endYear, string keyword)
    {
        DbHelper db = new DbHelper();

        // 將民國年轉換為西元年
        int startYearAD = startYear + 1911;
        int endYearAD = endYear + 1911;

        db.CommandText =
            @"
SELECT [AssessmentId]
    ,[Investigator]
    ,[Unit]
    ,[Title]
    ,[FormDate]
    ,[StartDate]
    ,[StartTime]
    ,[StartRemark]
    ,[EndDate]
    ,[EndTime]
    ,[EndRemark]
    ,[DurationDays]
    ,[SurveyAreaName]
    ,[VoyagePlanAndOperations]
    ,[Q_IsOperationInSensitiveArea]
    ,[Q_IsStayTimeMinimized]
    ,[Q_HasReducedOrRelocatedStation]
    ,[Q_KnowsReportingProcedure]
    ,[Q_HasStrongInterferenceContingencyPlan]
    ,[LastUpdated]
    ,[LastUpdatedBy]
    ,[IsValid]
FROM [OCA_OceanSubsidy].[dbo].[OSI_VesselRiskAssessments]
WHERE IsValid = 1
    -- 只要年份區間有重疊就符合條件
    AND YEAR(StartDate) <= @EndYearAD
    AND YEAR(EndDate) >= @StartYearAD
";

        // 如果有關鍵字，加入搜尋條件
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            db.CommandText += @"
    AND (Title LIKE @Keyword OR Unit LIKE @Keyword)
";
        }

        db.CommandText += @"
ORDER BY StartDate DESC, AssessmentId DESC
";

        db.Parameters.Clear();
        db.Parameters.Add("@StartYearAD", startYearAD);
        db.Parameters.Add("@EndYearAD", endYearAD);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            db.Parameters.Add("@Keyword", "%" + keyword + "%");
        }

        var tbl = db.GetTable();
        tbl.Columns.Add("StartDateDisplay", typeof(string));
        tbl.Columns.Add("EndDateDisplay", typeof(string));
        tbl.Columns.Add("LastUpdatedDisplay", typeof(string));
        foreach (DataRow row in tbl.Rows)
        {
            // 將日期欄位轉換為民國年格式
            if (row["StartDate"] != DBNull.Value)
            {
                DateTime startDate = Convert.ToDateTime(row["StartDate"]);
                row["StartDateDisplay"] = DateTimeHelper.ToMinguoDate(startDate);
            }

            if (row["EndDate"] != DBNull.Value)
            {
                DateTime endDate = Convert.ToDateTime(row["EndDate"]);
                row["EndDateDisplay"] = DateTimeHelper.ToMinguoDate(endDate);
            }

            if (row["LastUpdated"] != DBNull.Value)
            {
                DateTime lastUpdated = Convert.ToDateTime(row["LastUpdated"]);
                row["LastUpdatedDisplay"] = DateTimeHelper.ToMinguoDateTime(lastUpdated);
            }
        }

        return tbl;
    }

    /// <summary>
    /// 取得所有資料中的年份範圍
    /// </summary>
    /// <returns>包含 MinYear 和 MaxYear 欄位的 DataTable</returns>
    public static GisTable GetAllYears()
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT 
    MIN(YEAR(StartDate)) as MinYear,
    MAX(YEAR(EndDate)) as MaxYear
FROM [OCA_OceanSubsidy].[dbo].[OSI_VesselRiskAssessments]
WHERE IsValid = 1
    AND StartDate IS NOT NULL
    AND EndDate IS NOT NULL
";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 假刪除（將 IsValid 設為 0）
    /// </summary>
    /// <param name="assessmentId">要刪除的 AssessmentId</param>
    /// <param name="userId">執行刪除的使用者 ID</param>
    /// <returns>是否成功</returns>
    public static bool SoftDelete(int assessmentId, int userId)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
UPDATE [OCA_OceanSubsidy].[dbo].[OSI_VesselRiskAssessments]
SET IsValid = 0,
    LastUpdated = GETDATE(),
    LastUpdatedBy = @UserId
WHERE AssessmentId = @AssessmentId
";
        db.Parameters.Clear();
        db.Parameters.Add("@AssessmentId", assessmentId);
        db.Parameters.Add("@UserId", userId);

        try
        {
            db.ExecuteNonQuery();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 新增研究船風險檢核資料
    /// </summary>
    public static int InsertAssessment(OSI_VesselRiskAssessments assessment, List<int> selectedCategories, 
        List<OSI_VesselFiles> files, string wktData, int userId, string baseDir, string tempKey)
    {
        DbHelper db = new DbHelper();
        int newId = 0;
        
        db.BeginTrans();
        try
        {
            // 1. 新增主表
            db.CommandText = @"
INSERT INTO [OCA_OceanSubsidy].[dbo].[OSI_VesselRiskAssessments]
    ([Investigator], [Unit], [Title], [FormDate], [StartDate], [StartTime], [StartRemark],
     [EndDate], [EndTime], [EndRemark], [DurationDays], [SurveyAreaName], [VoyagePlanAndOperations],
     [Q_IsOperationInSensitiveArea], [Q_IsStayTimeMinimized], [Q_HasReducedOrRelocatedStation],
     [Q_KnowsReportingProcedure], [Q_HasStrongInterferenceContingencyPlan],
     [LastUpdated], [LastUpdatedBy], [IsValid])
VALUES
    (@Investigator, @Unit, @Title, @FormDate, @StartDate, @StartTime, @StartRemark,
     @EndDate, @EndTime, @EndRemark, @DurationDays, @SurveyAreaName, @VoyagePlanAndOperations,
     @Q_IsOperationInSensitiveArea, @Q_IsStayTimeMinimized, @Q_HasReducedOrRelocatedStation,
     @Q_KnowsReportingProcedure, @Q_HasStrongInterferenceContingencyPlan,
     GETDATE(), @LastUpdatedBy, 1);
SELECT SCOPE_IDENTITY();";
            
            db.Parameters.Clear();
            db.Parameters.Add("@Investigator", assessment.Investigator);
            db.Parameters.Add("@Unit", assessment.Unit);
            db.Parameters.Add("@Title", assessment.Title);
            db.Parameters.Add("@FormDate", assessment.FormDate);
            db.Parameters.Add("@StartDate", assessment.StartDate);
            db.Parameters.Add("@StartTime", assessment.StartTime);
            db.Parameters.Add("@StartRemark", assessment.StartRemark);
            db.Parameters.Add("@EndDate", assessment.EndDate);
            db.Parameters.Add("@EndTime", assessment.EndTime);
            db.Parameters.Add("@EndRemark", assessment.EndRemark);
            db.Parameters.Add("@DurationDays", assessment.DurationDays);
            db.Parameters.Add("@SurveyAreaName", assessment.SurveyAreaName);
            db.Parameters.Add("@VoyagePlanAndOperations", assessment.VoyagePlanAndOperations);
            db.Parameters.Add("@Q_IsOperationInSensitiveArea", assessment.Q_IsOperationInSensitiveArea);
            db.Parameters.Add("@Q_IsStayTimeMinimized", assessment.Q_IsStayTimeMinimized);
            db.Parameters.Add("@Q_HasReducedOrRelocatedStation", assessment.Q_HasReducedOrRelocatedStation);
            db.Parameters.Add("@Q_KnowsReportingProcedure", assessment.Q_KnowsReportingProcedure);
            db.Parameters.Add("@Q_HasStrongInterferenceContingencyPlan", assessment.Q_HasStrongInterferenceContingencyPlan);
            db.Parameters.Add("@LastUpdatedBy", userId);
            
            var result = db.GetTable();
            if (result != null && result.Rows.Count > 0)
            {
                newId = Convert.ToInt32(result.Rows[0][0]);
            }
            
            // 2. 新增類別關聯
            foreach (int categoryId in selectedCategories)
            {
                db.CommandText = @"
INSERT INTO [OCA_OceanSubsidy].[dbo].[OSI_VesselRiskAssessmentCategories]
    ([AssessmentId], [RiskCategoryId])
VALUES
    (@AssessmentId, @RiskCategoryId)";
                
                db.Parameters.Clear();
                db.Parameters.Add("@AssessmentId", newId);
                db.Parameters.Add("@RiskCategoryId", categoryId);
                db.ExecuteNonQuery();
            }
            
            // 3. 處理檔案（從暫存移到正式目錄）
            if (files != null && files.Count > 0)
            {
                string finalDir = Path.Combine(baseDir, newId.ToString());
                if (!Directory.Exists(finalDir))
                {
                    Directory.CreateDirectory(finalDir);
                }
                
                foreach (var file in files.Where(f => !f.IsDelete && f.AttachmentID == 0))
                {
                    string sourcePath = file.FilePath;
                    string destPath = Path.Combine(finalDir, file.FileName);
                    
                    if (File.Exists(sourcePath))
                    {
                        File.Move(sourcePath, destPath);
                        
                        // 新增到資料庫
                        db.CommandText = @"
INSERT INTO [OCA_OceanSubsidy].[dbo].[OSI_VesselFiles]
    ([AssessmentId], [FileName], [FilePath], [IsValid], [CreatedAt])
VALUES
    (@AssessmentId, @FileName, @FilePath, 1, GETDATE())";
                        
                        db.Parameters.Clear();
                        db.Parameters.Add("@AssessmentId", newId);
                        db.Parameters.Add("@FileName", file.FileName);
                        db.Parameters.Add("@FilePath", destPath);
                        db.ExecuteNonQuery();
                    }
                }
            }
            
            db.Commit();
            
            // 4. 儲存地理資料（在交易外處理）
            if (!string.IsNullOrEmpty(wktData))
            {
                try
                {
                    OSIVesselGeomHelper.SaveGeometries(newId, wktData, userId);
                }
                catch (Exception ex)
                {
                    // 記錄錯誤但不影響主要儲存流程
                    System.Diagnostics.Debug.WriteLine($"儲存地理資料失敗：{ex.Message}");
                }
            }
            
            return newId;
        }
        catch (Exception ex)
        {
            db.Rollback();
            throw new Exception($"新增資料失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// 更新研究船風險檢核資料
    /// </summary>
    public static bool UpdateAssessment(int assessmentId, OSI_VesselRiskAssessments assessment, 
        List<int> selectedCategories, List<OSI_VesselFiles> files, string wktData, int userId)
    {
        DbHelper db = new DbHelper();
        
        db.BeginTrans();
        try
        {
            // 1. 更新主表
            db.CommandText = @"
UPDATE [OCA_OceanSubsidy].[dbo].[OSI_VesselRiskAssessments]
SET [Investigator] = @Investigator,
    [Unit] = @Unit,
    [Title] = @Title,
    [FormDate] = @FormDate,
    [StartDate] = @StartDate,
    [StartTime] = @StartTime,
    [StartRemark] = @StartRemark,
    [EndDate] = @EndDate,
    [EndTime] = @EndTime,
    [EndRemark] = @EndRemark,
    [DurationDays] = @DurationDays,
    [SurveyAreaName] = @SurveyAreaName,
    [VoyagePlanAndOperations] = @VoyagePlanAndOperations,
    [Q_IsOperationInSensitiveArea] = @Q_IsOperationInSensitiveArea,
    [Q_IsStayTimeMinimized] = @Q_IsStayTimeMinimized,
    [Q_HasReducedOrRelocatedStation] = @Q_HasReducedOrRelocatedStation,
    [Q_KnowsReportingProcedure] = @Q_KnowsReportingProcedure,
    [Q_HasStrongInterferenceContingencyPlan] = @Q_HasStrongInterferenceContingencyPlan,
    [LastUpdated] = GETDATE(),
    [LastUpdatedBy] = @LastUpdatedBy
WHERE [AssessmentId] = @AssessmentId";
            
            db.Parameters.Clear();
            db.Parameters.Add("@AssessmentId", assessmentId);
            db.Parameters.Add("@Investigator", assessment.Investigator);
            db.Parameters.Add("@Unit", assessment.Unit);
            db.Parameters.Add("@Title", assessment.Title);
            db.Parameters.Add("@FormDate", assessment.FormDate);
            db.Parameters.Add("@StartDate", assessment.StartDate);
            db.Parameters.Add("@StartTime", assessment.StartTime);
            db.Parameters.Add("@StartRemark", assessment.StartRemark);
            db.Parameters.Add("@EndDate", assessment.EndDate);
            db.Parameters.Add("@EndTime", assessment.EndTime);
            db.Parameters.Add("@EndRemark", assessment.EndRemark);
            db.Parameters.Add("@DurationDays", assessment.DurationDays);
            db.Parameters.Add("@SurveyAreaName", assessment.SurveyAreaName);
            db.Parameters.Add("@VoyagePlanAndOperations", assessment.VoyagePlanAndOperations);
            db.Parameters.Add("@Q_IsOperationInSensitiveArea", assessment.Q_IsOperationInSensitiveArea);
            db.Parameters.Add("@Q_IsStayTimeMinimized", assessment.Q_IsStayTimeMinimized);
            db.Parameters.Add("@Q_HasReducedOrRelocatedStation", assessment.Q_HasReducedOrRelocatedStation);
            db.Parameters.Add("@Q_KnowsReportingProcedure", assessment.Q_KnowsReportingProcedure);
            db.Parameters.Add("@Q_HasStrongInterferenceContingencyPlan", assessment.Q_HasStrongInterferenceContingencyPlan);
            db.Parameters.Add("@LastUpdatedBy", userId);
            db.ExecuteNonQuery();
            
            // 2. 更新類別關聯（先刪除再新增）
            db.CommandText = @"
DELETE FROM [OCA_OceanSubsidy].[dbo].[OSI_VesselRiskAssessmentCategories]
WHERE AssessmentId = @AssessmentId";
            
            db.Parameters.Clear();
            db.Parameters.Add("@AssessmentId", assessmentId);
            db.ExecuteNonQuery();
            
            foreach (int categoryId in selectedCategories)
            {
                db.CommandText = @"
INSERT INTO [OCA_OceanSubsidy].[dbo].[OSI_VesselRiskAssessmentCategories]
    ([AssessmentId], [RiskCategoryId])
VALUES
    (@AssessmentId, @RiskCategoryId)";
                
                db.Parameters.Clear();
                db.Parameters.Add("@AssessmentId", assessmentId);
                db.Parameters.Add("@RiskCategoryId", categoryId);
                db.ExecuteNonQuery();
            }
            
            // 3. 處理檔案刪除
            foreach (var file in files.Where(f => f.IsDelete && f.AttachmentID > 0))
            {
                db.CommandText = @"
UPDATE [OCA_OceanSubsidy].[dbo].[OSI_VesselFiles]
SET IsValid = 0, DeletedAt = GETDATE(), DeletedBy = @DeletedBy
WHERE AttachmentID = @AttachmentID";
                
                db.Parameters.Clear();
                db.Parameters.Add("@AttachmentID", file.AttachmentID);
                db.Parameters.Add("@DeletedBy", userId.ToString());
                db.ExecuteNonQuery();
            }
            
            // 4. 處理新增檔案
            foreach (var file in files.Where(f => !f.IsDelete && f.AttachmentID == 0))
            {
                db.CommandText = @"
INSERT INTO [OCA_OceanSubsidy].[dbo].[OSI_VesselFiles]
    ([AssessmentId], [FileName], [FilePath], [IsValid], [CreatedAt])
VALUES
    (@AssessmentId, @FileName, @FilePath, 1, GETDATE())";
                
                db.Parameters.Clear();
                db.Parameters.Add("@AssessmentId", assessmentId);
                db.Parameters.Add("@FileName", file.FileName);
                db.Parameters.Add("@FilePath", file.FilePath);
                db.ExecuteNonQuery();
            }
            
            db.Commit();
            
            // 5. 儲存地理資料（在交易外處理）
            try
            {
                OSIVesselGeomHelper.SaveGeometries(assessmentId, wktData, userId);
            }
            catch (Exception ex)
            {
                // 記錄錯誤但不影響主要儲存流程
                System.Diagnostics.Debug.WriteLine($"儲存地理資料失敗：{ex.Message}");
            }
            
            return true;
        }
        catch (Exception ex)
        {
            db.Rollback();
            throw new Exception($"更新資料失敗：{ex.Message}");
        }
    }

}