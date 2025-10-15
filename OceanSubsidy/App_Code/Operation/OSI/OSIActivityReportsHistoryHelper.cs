using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// OSIActivityReportsHistoryHelper 的摘要描述
/// </summary>
public class OSIActivityReportsHistoryHelper
{
    public OSIActivityReportsHistoryHelper()
    {
       
    }

    /// <summary>
    /// 新增一筆活動資料歷史紀錄
    /// </summary>
    public static bool InsertReport(OSI_ActivityReports report, string correctionNotes = "")
    {
        bool rtVal = false;
        DbHelper db = new DbHelper();

        db.BeginTrans();
        try
        {
            db.CommandText = @"
INSERT INTO [dbo].[OSI_ActivityReports_History]
    ([ReportID]
    ,[PeriodID]
    ,[ReportingUnitID]
    ,[ActivityName]
    ,[NatureID]
    ,[NatureText]
    ,[ResearchItemID]
    ,[ResearchItemNote]
    ,[Instruments]
    ,[ActivityOverview]
    ,[LastUpdated]
    ,[LastUpdatedBy]
    ,[IsValid]
    ,[CopyReportID]
    ,[CorrectionNotes]
    ,[AuditAt]
    ,[AuditBy])
VALUES
    (@ReportID
    ,@PeriodID
    ,@ReportingUnitID
    ,@ActivityName
    ,@NatureID
    ,@NatureText
    ,@ResearchItemID
    ,@ResearchItemNote
    ,@Instruments
    ,@ActivityOverview
    ,@LastUpdated
    ,@LastUpdatedBy
    ,@IsValid
    ,@CopyReportID
    ,@CorrectionNotes
    ,GETDATE()
    ,@AuditBy)
";

            db.Parameters.Clear();
            db.Parameters.Add("@ReportID", report.ReportID);
            db.Parameters.Add("@PeriodID", report.PeriodID);
            db.Parameters.Add("@ReportingUnitID", report.ReportingUnitID);
            db.Parameters.Add("@ActivityName", report.ActivityName);
            db.Parameters.Add("@NatureID", report.NatureID);
            db.Parameters.Add("@NatureText", report.NatureText);
            db.Parameters.Add("@ResearchItemID", report.ResearchItemID);
            db.Parameters.Add("@ResearchItemNote", report.ResearchItemNote);
            db.Parameters.Add("@Instruments", report.Instruments);
            db.Parameters.Add("@ActivityOverview", report.ActivityOverview);
            db.Parameters.Add("@LastUpdated", report.LastUpdated);
            db.Parameters.Add("@LastUpdatedBy", report.LastUpdatedBy);
            db.Parameters.Add("@CopyReportID", report.CopyReportID);
            db.Parameters.Add("@IsValid", report.IsValid);
            db.Parameters.Add("@CorrectionNotes", correctionNotes);
            db.Parameters.Add("@AuditBy", report.LastUpdatedBy);

            db.ExecuteNonQuery();
            db.Commit();
            rtVal = true;
        }
        catch (Exception ex)
        {
            db.Rollback();
            rtVal = false;
        }

        return rtVal;
    }

    /// <summary>
    /// 檢查報告是否有歷史記錄
    /// </summary>
    public static bool HasHistory(int reportId)
    {
        bool hasHistory = false;
        DbHelper db = new DbHelper();

        try
        {
            db.CommandText = @"
                SELECT COUNT(*) AS HistoryCount
                FROM OSI_ActivityReports_History
                WHERE ReportID = @ReportID AND IsValid = 1";

            db.Parameters.Clear();
            db.Parameters.Add("@ReportID", reportId);

            var tbl = db.GetTable();
            if (tbl != null && tbl.Rows.Count > 0)
            {
                int count = Convert.ToInt32(tbl.Rows[0]["HistoryCount"]);
                hasHistory = count > 0;
            }
        }
        catch (Exception ex)
        {
            // 記錄錯誤但不拋出
            System.Diagnostics.Debug.WriteLine("HasHistory Error: " + ex.Message);
        }

        return hasHistory;
    }

    /// <summary>
    /// 取得報告的所有歷史記錄
    /// </summary>
    public static GisTable GetReportHistory(int reportId)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT 
                h.HistoryID,
                h.ReportID,
                h.ActivityName,
                h.AuditAt,
                h.AuditBy,
                h.CorrectionNotes,
                u.UnitName AS ReportingUnit,
                n.NatureName,
                p.PeriodYear,
                p.PeriodQuarter
            FROM OSI_ActivityReports_History h
            LEFT JOIN Sys_Unit u ON h.ReportingUnitID = u.UnitID
            LEFT JOIN OSI_ActivityNatures n ON h.NatureID = n.NatureID
            LEFT JOIN OSI_DataPeriods p ON h.PeriodID = p.PeriodID
            WHERE h.ReportID = @ReportID AND h.IsValid = 1
            ORDER BY h.AuditAt DESC";

        db.Parameters.Clear();
        db.Parameters.Add("@ReportID", reportId);

        return db.GetTable();
    }

    /// <summary>
    /// 取得報告的所有歷史記錄
    /// </summary>
    public static List<int> GetReportHistoryIdList(int reportId)
    {
        var historyIds = new List<int>();

        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT
                HistoryID
            FROM OSI_ActivityReports_History
            WHERE ReportID = @ReportID AND IsValid = 1
            ORDER BY AuditAt DESC";

        db.Parameters.Clear();
        db.Parameters.Add("@ReportID", reportId);

        GisTable tbl = db.GetTable();
        if (tbl != null && tbl.Rows.Count > 0)
        {
            foreach (DataRow row in tbl.Rows)
            {
                if (row["HistoryID"] != DBNull.Value)
                {
                    historyIds.Add(Convert.ToInt32(row["HistoryID"]));
                }
            }
        }

        return historyIds;
    }

    /// <summary>
    /// 取得單筆歷史記錄
    /// </summary>
    public static OSI_ActivityReports_History GetHistoryById(int historyId)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [HistoryID]
              ,[ReportID]
              ,[PeriodID]
              ,[ReportingUnitID]
              ,[ActivityName]
              ,[NatureID]
              ,[NatureText]
              ,[ResearchItemID]
              ,[ResearchItemNote]
              ,[Instruments]
              ,[ActivityOverview]
              ,[LastUpdated]
              ,[IsValid]
              ,[AuditAt]
              ,[AuditBy]
              ,[CopyReportID]
              ,[LastUpdatedBy]
              ,[CorrectionNotes]
            FROM OSI_ActivityReports_History
            WHERE HistoryID = @HistoryID";

        db.Parameters.Clear();
        db.Parameters.Add("@HistoryID", historyId);

        return db.GetList<OSI_ActivityReports_History>().FirstOrDefault();
    }

    /// <summary>
    /// 取得歷史記錄的完整資料
    /// </summary>
    public static GisTable GetHistoryWithDetails(long historyId)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT 
                h.[HistoryID],
                h.[ReportID],
                h.[PeriodID],
                h.[ReportingUnitID],
                h.[ActivityName],
                h.[NatureID],
                h.[NatureText],
                h.[ResearchItemID],
                h.[ResearchItemNote],
                h.[Instruments],
                h.[ActivityOverview],
                h.[LastUpdated],
                h.[IsValid],
                h.[AuditAt],
                h.[AuditBy],
                h.[CopyReportID],
                h.[LastUpdatedBy],
                h.[CorrectionNotes],
                u.UnitName AS ReportingUnitName,
                n.NatureName,
                r.ItemName AS ResearchItemName,
                p.PeriodYear + '年第' + CAST(p.PeriodQuarter AS NVARCHAR) + '季' AS DataPeriod
            FROM OSI_ActivityReports_History h
            LEFT JOIN Sys_Unit u ON h.ReportingUnitID = u.UnitID
            LEFT JOIN OSI_ActivityNatures n ON h.NatureID = n.NatureID
            LEFT JOIN OSI_ResearchItems r ON h.ResearchItemID = r.ItemID
            LEFT JOIN OSI_DataPeriods p ON h.PeriodID = p.PeriodID
            WHERE h.HistoryID = @HistoryID";

        db.Parameters.Clear();
        db.Parameters.Add("@HistoryID", historyId);

        return db.GetTable();
    }

    /// <summary>
    /// 批次檢查多個報告是否有歷史記錄
    /// </summary>
    public static Dictionary<int, bool> CheckHistoryExists(List<int> reportIds)
    {
        var result = new Dictionary<int, bool>();

        if (reportIds == null || reportIds.Count == 0)
            return result;

        DbHelper db = new DbHelper();

        try
        {
            // 建立參數名稱
            var paramNames = new List<string>();
            for (int i = 0; i < reportIds.Count; i++)
            {
                paramNames.Add("@id" + i);
            }

            db.CommandText = string.Format(@"
                SELECT ReportID, COUNT(*) AS HistoryCount
                FROM OSI_ActivityReports_History
                WHERE ReportID IN ({0}) AND IsValid = 1
                GROUP BY ReportID", string.Join(", ", paramNames));

            db.Parameters.Clear();
            for (int i = 0; i < reportIds.Count; i++)
            {
                db.Parameters.Add(paramNames[i], reportIds[i]);
            }

            var tbl = db.GetTable();

            // 初始化所有報告為 false
            foreach (var id in reportIds)
            {
                result[id] = false;
            }

            // 更新有歷史記錄的報告
            if (tbl != null && tbl.Rows.Count > 0)
            {
                foreach (DataRow row in tbl.Rows)
                {
                    int reportId = Convert.ToInt32(row["ReportID"]);
                    int count = Convert.ToInt32(row["HistoryCount"]);
                    result[reportId] = count > 0;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("CheckHistoryExists Error: " + ex.Message);
        }

        return result;
    }

    /// <summary>
    /// 取得特定歷程時間點的活動執行者
    /// </summary>
    public static GisTable GetRelatedExecutors(int reportId, DateTime auditTime)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT 
                e.ExecutorID,
                e.CategoryID,
                c.CategoryName,
                e.ExecutorName,
                e.CreatedAt
            FROM OSI_ActivityExecutors e
            LEFT JOIN OSI_ExecutorCategories c ON e.CategoryID = c.CategoryID
            WHERE e.ReportID = @ReportID
                AND e.CreatedAt <= @AuditTime
                AND (e.DeletedAt IS NULL OR e.DeletedAt > @AuditTime)
            ORDER BY e.ExecutorID";

        db.Parameters.Clear();
        db.Parameters.Add("@ReportID", reportId);
        db.Parameters.Add("@AuditTime", auditTime);

        return db.GetTable();
    }

    /// <summary>
    /// 取得特定歷程時間點的研究調查日期
    /// </summary>
    public static GisTable GetRelatedPeriods(int reportId, DateTime auditTime)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT 
                PeriodID,
                StartDate,
                EndDate,
                PeriodLabel,
                CreatedAt
            FROM OSI_ResearchPeriods
            WHERE ReportID = @ReportID
                AND CreatedAt <= @AuditTime
                AND (DeletedAt IS NULL OR DeletedAt > @AuditTime)
            ORDER BY StartDate";

        db.Parameters.Clear();
        db.Parameters.Add("@ReportID", reportId);
        db.Parameters.Add("@AuditTime", auditTime);

        return db.GetTable();
    }

    /// <summary>
    /// 取得特定歷程時間點的載具資料
    /// </summary>
    public static GisTable GetRelatedCarriers(int reportId, DateTime auditTime)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT 
                c.CarrierID,
                c.CarrierTypeID,
                t.CarrierTypeName,
                c.CarrierDetail,
                c.CarrierNo,
                c.CreatedAt
            FROM OSI_Carrier c
            LEFT JOIN OSI_CarrierTypes t ON c.CarrierTypeID = t.CarrierTypeID
            WHERE c.ReportID = @ReportID
                AND c.CreatedAt <= @AuditTime
                AND (c.DeletedAt IS NULL OR c.DeletedAt > @AuditTime)
            ORDER BY c.CarrierID";

        db.Parameters.Clear();
        db.Parameters.Add("@ReportID", reportId);
        db.Parameters.Add("@AuditTime", auditTime);

        return db.GetTable();
    }

    /// <summary>
    /// 取得特定歷程時間點的附件
    /// </summary>
    public static GisTable GetRelatedFiles(int reportId, DateTime auditTime)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT 
                FileName,
                CreatedAt
            FROM OSI_ActivityFiles
            WHERE ReportID = @ReportID
                AND CreatedAt <= @AuditTime
                AND (DeletedAt IS NULL OR DeletedAt > @AuditTime)";

        db.Parameters.Clear();
        db.Parameters.Add("@ReportID", reportId);
        db.Parameters.Add("@AuditTime", auditTime);

        return db.GetTable();
    }

    /// <summary>
    /// 取得特定歷程時間點的調查範圍(描述)
    /// </summary>
    public static GisTable GetRelatedScopes(int reportId, DateTime auditTime)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT 
                ScopeID,
                SurveyScope,
                CreatedAt
            FROM OSI_SurveyScopes
            WHERE ReportID = @ReportID
                AND CreatedAt <= @AuditTime
                AND (DeletedAt IS NULL OR DeletedAt > @AuditTime)
            ORDER BY ScopeID";

        db.Parameters.Clear();
        db.Parameters.Add("@ReportID", reportId);
        db.Parameters.Add("@AuditTime", auditTime);

        return db.GetTable();
    }

    /// <summary>
    /// 取得特定歷程時間點的調查範圍(縣市)
    /// </summary>
    public static GisTable GetRelatedCounties(int reportId, DateTime auditTime)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT 
                SurveyCountyID,
                c.c_name as CountyName,
                CreatedAt
            FROM OSI_SurveyCounties sc
            JOIN OSI_MapCounty c ON sc.CountyID = c.qgs_fid
            WHERE ReportID = @ReportID
                AND CreatedAt <= @AuditTime
                AND (DeletedAt IS NULL OR DeletedAt > @AuditTime)
            ORDER BY sc.CountyID";

        db.Parameters.Clear();
        db.Parameters.Add("@ReportID", reportId);
        db.Parameters.Add("@AuditTime", auditTime);

        return db.GetTable();
    }

    /// <summary>
    /// 取得特定歷程時間點的圖徵資料
    /// </summary>
    public static GisTable GetRelatedGeoms(int reportId, DateTime auditTime)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT 
                GeomID,
                GeomName,
                GeoData.STAsText() AS GeoData,
                CreatedAt
            FROM OSI_Geom
            WHERE ReportID = @ReportID
                AND CreatedAt <= @AuditTime
                AND (DeletedAt IS NULL OR DeletedAt > @AuditTime)
                AND IsValid = 1
            ORDER BY GeomID";

        db.Parameters.Clear();
        db.Parameters.Add("@ReportID", reportId);
        db.Parameters.Add("@AuditTime", auditTime);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢地圖資訊 BY ID，回傳 JSON 格式
    /// </summary>
    /// <returns></returns>
    public static string QueryGeoDataByID(string historyID)
    {
        DbHelper db = new DbHelper();
        
        // 先查詢歷史記錄以取得 ReportID 和 AuditAt
        db.CommandText = @"
            SELECT ReportID, AuditAt
            FROM OSI_ActivityReports_History
            WHERE HistoryID = @HistoryID";
        
        db.Parameters.Clear();
        db.Parameters.Add("@HistoryID", historyID);
        
        var historyTable = db.GetTable();
        if (historyTable != null && historyTable.Rows.Count > 0)
        {
            int reportId = Convert.ToInt32(historyTable.Rows[0]["ReportID"]);
            DateTime auditTime = Convert.ToDateTime(historyTable.Rows[0]["AuditAt"]);
            
            // 查詢該時間點的所有圖徵（包含 ID 和名稱）
            db.CommandText = @"
                SELECT 
                    GeomID,
                    GeomName,
                    GeoData.STAsText() AS GeoData
                FROM OSI_Geom
                WHERE ReportID = @ReportID
                    AND CreatedAt <= @AuditTime
                    AND (DeletedAt IS NULL OR DeletedAt > @AuditTime)";
            
            db.Parameters.Clear();
            db.Parameters.Add("@ReportID", reportId);
            db.Parameters.Add("@AuditTime", auditTime);
            
            var geomsTable = db.GetTable();
            if (geomsTable != null && geomsTable.Rows.Count > 0)
            {
                // 組合成與 GetReportGeoData 相同的 JSON 格式
                var result = new
                {
                    type = "FeatureCollection",
                    features = geomsTable.Rows.Cast<DataRow>().Select(row => new
                    {
                        id = row["GeomID"]?.ToString() ?? "",
                        name = row["GeomName"]?.ToString() ?? "",
                        wkt = row["GeoData"]?.ToString() ?? ""
                    }).ToArray()
                };
                
                return Newtonsoft.Json.JsonConvert.SerializeObject(result);
            }
        }
        
        return "";
    }

}