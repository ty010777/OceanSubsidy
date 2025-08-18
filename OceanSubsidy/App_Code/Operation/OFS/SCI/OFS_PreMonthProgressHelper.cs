using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Model.OFS;

/// <summary>
/// 預定分月進度資料處理 Helper
/// </summary>
public class OFS_PreMonthProgressHelper
{
    /// <summary>
    /// 根據 ProjectID 取得預定分月進度資料
    /// </summary>
    /// <param name="projectId">計畫ID</param>
    /// <returns>預定分月進度清單</returns>
    public static List<OFS_SCI_PreMonthProgress> GetPreMonthProgressByProjectId(string projectId)
    {
        List<OFS_SCI_PreMonthProgress> result = new List<OFS_SCI_PreMonthProgress>();
        
        if (string.IsNullOrEmpty(projectId))
            return result;

        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
                SELECT [ProjectID], [Month], [PreWorkAbstract], [ActWorkAbstract], [CheckDescription], [PreProgress], [ActProgress]
                FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_PreMonthProgress]
                WHERE [ProjectID] = @ProjectID
                ORDER BY [Month]";

            db.Parameters.Add("@ProjectID", projectId);

            DataTable dt = db.GetTable();
            
            foreach (DataRow row in dt.Rows)
            {
                var progress = new OFS_SCI_PreMonthProgress
                {
                    ProjectID = row["ProjectID"]?.ToString(),
                    Month = row["Month"]?.ToString(),
                    PreWorkAbstract = row["PreWorkAbstract"]?.ToString(),
                    ActWorkAbstract = row["ActWorkAbstract"]?.ToString(),
                    CheckDescription = row["CheckDescription"]?.ToString(),
                    PreProgress = row["PreProgress"] != DBNull.Value ? Convert.ToDecimal(row["PreProgress"]) : (decimal?)null,
                    ActProgress = row["ActProgress"] != DBNull.Value ? Convert.ToDecimal(row["ActProgress"]) : (decimal?)null
                };
                
                result.Add(progress);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得預定分月進度資料時發生錯誤：{ex.Message}");
            throw new Exception($"取得預定分月進度資料時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }

        return result;
    }

    /// <summary>
    /// 儲存預定分月進度資料 (使用 UPDATE/INSERT 模式)
    /// </summary>
    /// <param name="projectId">計畫ID</param>
    /// <param name="progressList">預定分月進度資料清單</param>
    /// <returns>是否成功</returns>
    public static bool SavePreMonthProgress(string projectId, List<OFS_SCI_PreMonthProgress> progressList)
    {
        if (string.IsNullOrEmpty(projectId) || progressList == null)
            return false;

        DbHelper db = new DbHelper();
        db.BeginTrans();

        try
        {
            // 逐筆處理資料，檢查是否存在後決定 UPDATE 或 INSERT
            foreach (var progress in progressList)
            {
                if (string.IsNullOrEmpty(progress.Month))
                    continue; // 跳過沒有月份的資料

                // 檢查是否已存在該月份資料
                db.CommandText = @"
                    SELECT COUNT(1) FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_PreMonthProgress] 
                    WHERE [ProjectID] = @ProjectID AND [Month] = @Month";
                    
                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", projectId);
                db.Parameters.Add("@Month", progress.Month);
                
                var countResult = db.GetTable();
                bool exists = countResult.Rows.Count > 0 && Convert.ToInt32(countResult.Rows[0][0]) > 0;
                
                if (exists)
                {
                    // 更新現有資料，只更新預定進度相關欄位，保留實際進度資料
                    db.CommandText = @"
                        UPDATE [OCA_OceanSubsidy].[dbo].[OFS_SCI_PreMonthProgress]
                        SET [PreWorkAbstract] = @PreWorkAbstract,
                            [CheckDescription] = @CheckDescription,
                            [PreProgress] = @PreProgress
                        WHERE [ProjectID] = @ProjectID AND [Month] = @Month";
                }
                else
                {
                    // 新增資料
                    db.CommandText = @"
                        INSERT INTO [OCA_OceanSubsidy].[dbo].[OFS_SCI_PreMonthProgress]
                        ([ProjectID], [Month], [PreWorkAbstract], [ActWorkAbstract], [CheckDescription], [PreProgress], [ActProgress], [MonthlySubsidy], [MonthlyCoop])
                        VALUES
                        (@ProjectID, @Month, @PreWorkAbstract, NULL, @CheckDescription, @PreProgress, NULL, NULL, NULL)";
                }

                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", projectId);
                db.Parameters.Add("@Month", progress.Month);
                db.Parameters.Add("@PreWorkAbstract", progress.PreWorkAbstract ?? "");
                db.Parameters.Add("@CheckDescription", progress.CheckDescription ?? "");
                db.Parameters.Add("@PreProgress", progress.PreProgress);

                db.ExecuteNonQuery();
            }

            db.Commit();
            return true;
        }
        catch (Exception ex)
        {
            db.Rollback();
            System.Diagnostics.Debug.WriteLine($"儲存預定分月進度資料時發生錯誤：{ex.Message}");
            throw new Exception($"儲存預定分月進度資料時發生錯誤：{ex.Message}", ex);
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 驗證預定分月進度資料
    /// </summary>
    /// <param name="progressList">預定分月進度資料清單</param>
    /// <returns>驗證結果</returns>
    public static ValidationResult ValidatePreMonthProgress(List<OFS_SCI_PreMonthProgress> progressList)
    {
        var result = new ValidationResult();

        if (progressList == null || progressList.Count == 0)
        {
            result.AddError("請填寫預定分月進度資料");
            return result;
        }

        foreach (var progress in progressList)
        {
            // 工作摘要為必填
            if (string.IsNullOrWhiteSpace(progress.PreWorkAbstract))
            {
                result.AddError($"月份「{progress.Month}」的工作摘要為必填項目");
            }

            // 預定進度需為 0-100 的整數
            if (progress.PreProgress.HasValue)
            {
                if (progress.PreProgress.Value < 0 || progress.PreProgress.Value > 100)
                {
                    result.AddError($"月份「{progress.Month}」的累計預定進度必須介於 0-100 之間");
                }
            }
        }

        return result;
    }
    
    /// <summary>
    /// 根據 ProjectID 和月份取得特定月份的預定分月進度資料
    /// </summary>
    /// <param name="projectId">計畫ID</param>
    /// <param name="month">月份 (格式: 114年5月)</param>
    /// <returns>該月份的預定分月進度資料</returns>
    public static OFS_SCI_PreMonthProgress GetPreMonthProgressByProjectIdAndMonth(string projectId, string month)
    {
        if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(month))
            return null;

        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
                SELECT * FROM [OFS_SCI_PreMonthProgress] 
                WHERE [ProjectID] = @ProjectID AND [Month] = @Month";
            
            db.Parameters.Add("@ProjectID", projectId);
            db.Parameters.Add("@Month", month);
            
            DataTable dt = db.GetTable();
            
            if (dt != null && dt.Rows.Count > 0)
            {
                var entity = new OFS_SCI_PreMonthProgress();
                entity.ProjectID = dt.Rows[0]["ProjectID"]?.ToString();
                entity.Month = dt.Rows[0]["Month"]?.ToString();
                entity.PreWorkAbstract = dt.Rows[0]["PreWorkAbstract"]?.ToString();
                entity.ActWorkAbstract = dt.Rows[0]["ActWorkAbstract"]?.ToString();
                entity.CheckDescription = dt.Rows[0]["CheckDescription"]?.ToString();
                
                // 處理 PreProgress 數值轉換 (decimal?)
                if (dt.Rows[0]["PreProgress"] != DBNull.Value && dt.Rows[0]["PreProgress"] != null)
                {
                    if (decimal.TryParse(dt.Rows[0]["PreProgress"].ToString(), out decimal preProgress))
                    {
                        entity.PreProgress = preProgress;
                    }
                }
                
                // 處理 ActProgress 數值轉換 (decimal?)
                if (dt.Rows[0]["ActProgress"] != DBNull.Value && dt.Rows[0]["ActProgress"] != null)
                {
                    if (decimal.TryParse(dt.Rows[0]["ActProgress"].ToString(), out decimal actProgress))
                    {
                        entity.ActProgress = actProgress;
                    }
                }
                // 處理 MonthlyCoop 數值轉換 (decimal?)
                if (dt.Rows[0]["MonthlyCoop"] != DBNull.Value && dt.Rows[0]["MonthlyCoop"] != null)
                {
                    if (decimal.TryParse(dt.Rows[0]["MonthlyCoop"].ToString(), out decimal monthlyCoop))
                    {
                        entity.MonthlyCoop = monthlyCoop;
                    }
                }

                // 處理 MonthlySubsidy 數值轉換 (decimal?)
                if (dt.Rows[0]["MonthlySubsidy"] != DBNull.Value && dt.Rows[0]["MonthlySubsidy"] != null)
                {
                    if (decimal.TryParse(dt.Rows[0]["MonthlySubsidy"].ToString(), out decimal monthlySubsidy))
                    {
                        entity.MonthlySubsidy = monthlySubsidy;
                    }
                }

                // 處理 MonthlyTotal 數值轉換 (decimal?)
                if (dt.Rows[0]["MonthlyTotal"] != DBNull.Value && dt.Rows[0]["MonthlyTotal"] != null)
                {
                    if (decimal.TryParse(dt.Rows[0]["MonthlyTotal"].ToString(), out decimal monthlyTotal))
                    {
                        entity.MonthlyTotal = monthlyTotal;
                    }
                }

                return entity;
            }
            
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetPreMonthProgressByProjectIdAndMonth 發生錯誤: {ex.Message}");
            return null;
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 檢查是否有現有資料
    /// </summary>
    /// <param name="projectId">計畫ID</param>
    /// <returns>是否存在資料</returns>
    public static bool HasExistingData(string projectId)
    {
        if (string.IsNullOrEmpty(projectId))
            return false;

        DbHelper db = new DbHelper();
        try
        {
            db.CommandText = @"
                SELECT COUNT(1)
                FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_PreMonthProgress]
                WHERE [ProjectID] = @ProjectID";

            db.Parameters.Add("@ProjectID", projectId);

            var result = db.GetTable();
            
            return result.Rows.Count > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"檢查現有資料時發生錯誤：{ex.Message}");
            return false;
        }
        finally
        {
            db.Dispose();
        }
    }
    
    /// <summary>
    /// 更新特定月份的預定分月進度資料
    /// </summary>
    /// <param name="projectId">計畫ID</param>
    /// <param name="month">月份</param>
    /// <param name="progress">進度資料</param>
    /// <returns>更新結果</returns>
    public static void UpdatePreMonthProgress(string projectId, string month, OFS_SCI_PreMonthProgress progress)
    {
        if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(month) || progress == null)
            return ;

        DbHelper db = new DbHelper();
        try
        {
            
            db.CommandText = @"
                UPDATE [OCA_OceanSubsidy].[dbo].[OFS_SCI_PreMonthProgress]
                SET [PreWorkAbstract] = @PreWorkAbstract,
                    [ActWorkAbstract] = @ActWorkAbstract,
                    [CheckDescription] = @CheckDescription,
                    [PreProgress] = @PreProgress,
                    [ActProgress] = @ActProgress
                WHERE [ProjectID] = @ProjectID AND [Month] = @Month";

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectId);
            db.Parameters.Add("@Month", month);
            db.Parameters.Add("@PreWorkAbstract", progress.PreWorkAbstract ?? "");
            db.Parameters.Add("@ActWorkAbstract", progress.ActWorkAbstract ?? "");
            db.Parameters.Add("@CheckDescription", progress.CheckDescription ?? "");
            db.Parameters.Add("@PreProgress", progress.PreProgress);
            db.Parameters.Add("@ActProgress", progress.ActProgress);

            db.ExecuteNonQuery();
            
            
        }
        catch (Exception ex)
        {
            db.Rollback();
        }
        finally
        {
            db.Dispose();
        }
    }
    
    /// <summary>
    /// 儲存或更新特定月份的預定分月進度資料 (如果不存在則新增，存在則更新)
    /// </summary>
    /// <param name="projectId">計畫ID</param>
    /// <param name="month">月份</param>
    /// <param name="progress">進度資料</param>
    /// <returns>儲存結果</returns>
    public static void SaveOrUpdatePreMonthProgress(string projectId, string month, OFS_SCI_PreMonthProgress progress)
    {
        if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(month) || progress == null)
            return ;

        DbHelper db = new DbHelper();
        try
        {
            
            // 檢查是否已存在該月份資料
            db.CommandText = @"
                SELECT COUNT(1) FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_PreMonthProgress] 
                WHERE [ProjectID] = @ProjectID AND [Month] = @Month";
                
            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectId);
            db.Parameters.Add("@Month", month);
            
            var countResult = db.GetTable();
            bool exists = countResult.Rows.Count > 0 && Convert.ToInt32(countResult.Rows[0][0]) > 0;
            
            if (exists)
            {
                // 更新
                db.CommandText = @"
                    UPDATE [OCA_OceanSubsidy].[dbo].[OFS_SCI_PreMonthProgress]
                    SET [PreWorkAbstract] = @PreWorkAbstract,
                        [ActWorkAbstract] = @ActWorkAbstract,
                        [CheckDescription] = @CheckDescription,
                        [PreProgress] = @PreProgress,
                        [ActProgress] = @ActProgress
                    WHERE [ProjectID] = @ProjectID AND [Month] = @Month";
            }
            else
            {
                // 新增
                db.CommandText = @"
                    INSERT INTO [OCA_OceanSubsidy].[dbo].[OFS_SCI_PreMonthProgress]
                    ([ProjectID], [Month], [PreWorkAbstract], [ActWorkAbstract], [CheckDescription], [PreProgress], [ActProgress])
                    VALUES
                    (@ProjectID, @Month, @PreWorkAbstract, @ActWorkAbstract, @CheckDescription, @PreProgress, @ActProgress)";
            }

            db.Parameters.Clear();
            db.Parameters.Add("@ProjectID", projectId);
            db.Parameters.Add("@Month", month);
            db.Parameters.Add("@PreWorkAbstract", progress.PreWorkAbstract ?? "");
            db.Parameters.Add("@ActWorkAbstract", progress.ActWorkAbstract ?? "");
            db.Parameters.Add("@CheckDescription", progress.CheckDescription ?? "");
            db.Parameters.Add("@PreProgress", progress.PreProgress);
            db.Parameters.Add("@ActProgress", progress.ActProgress);

            db.ExecuteNonQuery();
     
            
        }
        catch (Exception ex)
        {
            return ;
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 取得當月應顯示的累計執行經費
    /// 邏輯：當月有值用當月值，當月無值用上月累計值，第一月無值用0
    /// </summary>
    /// <param name="projectId">計畫ID</param>
    /// <param name="currentMonth">當前月份 (格式: 114年7月)</param>
    /// <returns>應顯示的累計補助金額、累計配合款、累計總額</returns>
    public static (decimal DisplaySubsidy, decimal DisplayCoop, decimal DisplayTotal) GetDisplayBudgetForMonth(string projectId, string currentMonth)
    {
        if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(currentMonth))
        {
            return (0, 0, 0);
        }

        using (DbHelper db = new DbHelper())
        {
            try
            {
                // 1. 先檢查當月是否有資料
                var currentMonthData = GetPreMonthProgressByProjectIdAndMonth(projectId, currentMonth);
                
                if (currentMonthData != null && 
                    currentMonthData.MonthlySubsidy.HasValue && 
                    currentMonthData.MonthlyCoop.HasValue)
                {
                    // 當月有資料，直接使用當月值
                    decimal subsidy = currentMonthData.MonthlySubsidy.Value;
                    decimal coop = currentMonthData.MonthlyCoop.Value;
                    decimal total = subsidy + coop;
                    
                    System.Diagnostics.Debug.WriteLine($"使用當月資料: {currentMonth} - 補助:{subsidy}, 配合款:{coop}, 總額:{total}");
                    return (subsidy, coop, total);
                }
                
                // 2. 當月沒有資料，找上一個月的累計值
                string previousMonth = GetPreviousMonth(projectId, currentMonth);
                
                if (!string.IsNullOrEmpty(previousMonth))
                {
                    // 遞迴取得上一個月的累計值
                    var (prevSubsidy, prevCoop, prevTotal) = GetDisplayBudgetForMonth(projectId, previousMonth);
                    
                    System.Diagnostics.Debug.WriteLine($"使用上月累計: {currentMonth} 繼承自 {previousMonth} - 補助:{prevSubsidy}, 配合款:{prevCoop}, 總額:{prevTotal}");
                    return (prevSubsidy, prevCoop, prevTotal);
                }
                
                // 3. 第一個月且沒有資料，預設為0
                System.Diagnostics.Debug.WriteLine($"第一月無資料，預設為0: {currentMonth}");
                return (0, 0, 0);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetDisplayBudgetForMonth 發生錯誤: {ex.Message}");
                return (0, 0, 0);
            }
        }
    }

    /// <summary>
    /// 取得指定月份的上一個月
    /// </summary>
    /// <param name="projectId">計畫ID</param>
    /// <param name="currentMonth">當前月份</param>
    /// <returns>上一個月份字串，如果沒有則回傳空字串</returns>
    public static string GetPreviousMonth(string projectId, string currentMonth)
    {
        using (DbHelper db = new DbHelper())
        {
            try
            {
                string sql = @"
                    WITH MonthlyData AS (
                        SELECT 
                            Month,
                            -- 將民國年月轉換為可排序的數字
                            (CAST(LEFT(Month, CHARINDEX('年', Month)-1) AS INT) * 10000 + 
                             CAST(REPLACE(REPLACE(SUBSTRING(Month, CHARINDEX('年', Month)+1, LEN(Month)), '月', ''), '年', '') AS INT) * 100) AS MonthSort
                        FROM OFS_SCI_PreMonthProgress 
                        WHERE ProjectID = @ProjectID
                        AND Month IS NOT NULL
                    ),
                    CurrentMonthData AS (
                        SELECT MonthSort as CurrentSort
                        FROM MonthlyData
                        WHERE Month = @CurrentMonth
                    )
                    SELECT TOP 1 Month
                    FROM MonthlyData, CurrentMonthData
                    WHERE MonthlyData.MonthSort < CurrentMonthData.CurrentSort
                    ORDER BY MonthlyData.MonthSort DESC";

                db.CommandText = sql;
                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", projectId);
                db.Parameters.Add("@CurrentMonth", currentMonth);
                
                DataTable dt = db.GetTable();
                
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0]["Month"]?.ToString() ?? "";
                }
                
                return "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetPreviousMonth 發生錯誤: {ex.Message}");
                return "";
            }
        }
    }

    /// <summary>
    /// 更新月份預算資料
    /// </summary>
    /// <param name="projectId">計畫ID</param>
    /// <param name="month">月份</param>
    /// <param name="monthlySubsidy">月份補助金額</param>
    /// <param name="monthlyCoop">月份配合款</param>
    /// <returns>是否成功</returns>
    public static bool UpdateMonthlyBudget(string projectId, string month, decimal? monthlySubsidy, decimal? monthlyCoop)
    {
        if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(month))
            return false;

        using (DbHelper db = new DbHelper())
        {
            try
            {
               
            // 更新現有資料
            db.CommandText = @"
                UPDATE [OFS_SCI_PreMonthProgress] 
                SET [MonthlySubsidy] = @MonthlySubsidy,
                    [MonthlyCoop] = @MonthlyCoop
                WHERE [ProjectID] = @ProjectID AND [Month] = @Month";
                

                db.Parameters.Clear();
                db.Parameters.Add("@ProjectID", projectId);
                db.Parameters.Add("@Month", month);
                db.Parameters.Add("@MonthlySubsidy", (object)monthlySubsidy ?? DBNull.Value);
                db.Parameters.Add("@MonthlyCoop", (object)monthlyCoop ?? DBNull.Value);

                db.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateMonthlyBudget 發生錯誤: {ex.Message}");
                return false;
            }
        }
    }
    public static bool UpdateActualProgress(string projectId, string month, string actWorkAbstract, decimal? actProgress, decimal? monthlySubsidy, decimal? monthlyCoop)
        {
            if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(month))
                return false;

            using (DbHelper db = new DbHelper())
            {
                try
                {
                    // 檢查是否已存在該月份資料
                    db.CommandText = @"
                        SELECT COUNT(1) FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_PreMonthProgress] 
                        WHERE [ProjectID] = @ProjectID AND [Month] = @Month";
                        
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", projectId);
                    db.Parameters.Add("@Month", month);
                    
                    var countResult = db.GetTable();
                    bool exists = countResult.Rows.Count > 0 && Convert.ToInt32(countResult.Rows[0][0]) > 0;
                    
                    if (exists)
                    {
                        // 更新實際進度欄位
                        db.CommandText = @"
                            UPDATE [OCA_OceanSubsidy].[dbo].[OFS_SCI_PreMonthProgress]
                            SET [ActWorkAbstract] = @ActWorkAbstract,
                                [ActProgress] = @ActProgress,
                                [MonthlySubsidy] = @MonthlySubsidy,
                                [MonthlyCoop] = @MonthlyCoop
                            WHERE [ProjectID] = @ProjectID AND [Month] = @Month";
                    }
                    else
                    {
                        // 如果不存在，則新增一筆記錄（預定欄位留空）
                        db.CommandText = @"
                            INSERT INTO [OCA_OceanSubsidy].[dbo].[OFS_SCI_PreMonthProgress]
                            ([ProjectID], [Month], [PreWorkAbstract], [ActWorkAbstract], [CheckDescription], [PreProgress], [ActProgress], [MonthlySubsidy], [MonthlyCoop])
                            VALUES
                            (@ProjectID, @Month, '', @ActWorkAbstract, '', NULL, @ActProgress, @MonthlySubsidy, @MonthlyCoop)";
                    }

                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", projectId);
                    db.Parameters.Add("@Month", month);
                    db.Parameters.Add("@ActWorkAbstract", actWorkAbstract ?? "");
                    db.Parameters.Add("@ActProgress", (object)actProgress ?? DBNull.Value);
                    db.Parameters.Add("@MonthlySubsidy", (object)monthlySubsidy ?? DBNull.Value);
                    db.Parameters.Add("@MonthlyCoop", (object)monthlyCoop ?? DBNull.Value);

                    db.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"UpdateActualProgress 發生錯誤: {ex.Message}");
                    return false;
                }
            }
        }
        
        /// <summary>
        /// 只更新預定進度資料（由 SciInprogress_PreProgress 頁面使用）
        /// </summary>
        /// <param name="projectId">計畫ID</param>
        /// <param name="month">月份</param>
        /// <param name="preWorkAbstract">預定工作摘要</param>
        /// <param name="checkDescription">查核內容</param>
        /// <param name="preProgress">累計預定進度</param>
        /// <returns>更新結果</returns>
        public static bool UpdatePlannedProgress(string projectId, string month, string preWorkAbstract, string checkDescription, decimal? preProgress)
        {
            if (string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(month))
                return false;

            using (DbHelper db = new DbHelper())
            {
                try
                {
                    // 檢查是否已存在該月份資料
                    db.CommandText = @"
                        SELECT COUNT(1) FROM [OCA_OceanSubsidy].[dbo].[OFS_SCI_PreMonthProgress] 
                        WHERE [ProjectID] = @ProjectID AND [Month] = @Month";
                        
                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", projectId);
                    db.Parameters.Add("@Month", month);
                    
                    var countResult = db.GetTable();
                    bool exists = countResult.Rows.Count > 0 && Convert.ToInt32(countResult.Rows[0][0]) > 0;
                    
                    if (exists)
                    {
                        // 更新預定進度欄位
                        db.CommandText = @"
                            UPDATE [OCA_OceanSubsidy].[dbo].[OFS_SCI_PreMonthProgress]
                            SET [PreWorkAbstract] = @PreWorkAbstract,
                                [CheckDescription] = @CheckDescription,
                                [PreProgress] = @PreProgress
                            WHERE [ProjectID] = @ProjectID AND [Month] = @Month";
                    }
                    else
                    {
                        // 如果不存在，則新增一筆記錄（實際欄位留空）
                        db.CommandText = @"
                            INSERT INTO [OCA_OceanSubsidy].[dbo].[OFS_SCI_PreMonthProgress]
                            ([ProjectID], [Month], [PreWorkAbstract], [ActWorkAbstract], [CheckDescription], [PreProgress], [ActProgress], [MonthlySubsidy], [MonthlyCoop])
                            VALUES
                            (@ProjectID, @Month, @PreWorkAbstract, '', @CheckDescription, @PreProgress, NULL, NULL, NULL)";
                    }

                    db.Parameters.Clear();
                    db.Parameters.Add("@ProjectID", projectId);
                    db.Parameters.Add("@Month", month);
                    db.Parameters.Add("@PreWorkAbstract", preWorkAbstract ?? "");
                    db.Parameters.Add("@CheckDescription", checkDescription ?? "");
                    db.Parameters.Add("@PreProgress", (object)preProgress ?? DBNull.Value);

                    db.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"UpdatePlannedProgress 發生錯誤: {ex.Message}");
                    return false;
                }
            }
        }

}