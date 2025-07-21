using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// SysUnitHelper 的摘要描述
/// </summary>
public class SysUnitHelper
{
    public SysUnitHelper()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }

    /// <summary>
    /// 查詢全部
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryAll()
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT [UnitID]
            ,[UnitName]
            ,[GovUnitTypeID]
            ,[ParentUnitID]
            ,[IsValid]
            FROM Sys_Unit
            WHERE IsValid = 1";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 查詢全部 
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryAllOrderByUnitID()
    {
        // 使用核心查詢方法，查詢全部（govUnitTypeID = null），包含 GovUnitTypeName
        return QueryUnitsCore(null, true);
    }

    /// <summary>
    /// 查詢指定的審核單位（用於移轉案件）
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryReviewUnits()
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT [UnitID]
            ,[UnitName]
            ,[ParentUnitID]
            ,[IsValid]
            ,[GovUnitTypeID]
            FROM [Sys_Unit]
            WHERE UnitID IN (49, 50, 51)
            AND IsValid = 1
            ORDER BY UnitID";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 查詢系統單位ByParentUnitID
    /// </summary>
    /// <param name="parentUnitID"></param>
    /// <returns></returns>
    public static GisTable QueryByParentUnitID(string parentUnitID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT [UnitID]
            ,[UnitName]
            ,[GovUnitTypeID]
            ,[ParentUnitID]
            ,[IsValid]
            FROM Sys_Unit
            WHERE IsValid = 1
            AND ParentUnitID = @ParentUnitID";
        db.Parameters.Clear();
        db.Parameters.Add("@ParentUnitID", parentUnitID);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢系統單位By GovUnitTypeID (按照排序規則)
    /// </summary>
    /// <param name="govUnitTypeID"></param>
    /// <returns></returns>
    public static GisTable QueryByGovUnitTypeID(int govUnitTypeID, bool includeGovUnitTypeName = false)
    {
        // 使用核心查詢方法，查詢特定類型，不包含 GovUnitTypeName
        return QueryUnitsCore(govUnitTypeID, includeGovUnitTypeName);
    }

    /// <summary>
    /// 查詢系統單位ByParentUnitID
    /// </summary>
    /// <param name="unitName"></param>
    /// <returns></returns>
    public static GisTable QueryByUnitName(string unitName)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT [UnitID]
            ,[UnitName]
            ,[GovUnitTypeID]
            ,[ParentUnitID]
            ,[IsValid]
            FROM Sys_Unit
            WHERE IsValid = 1
            AND UnitName = @UnitName";
        db.Parameters.Clear();
        db.Parameters.Add("@UnitName", unitName);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢ID By Name
    /// </summary>
    /// <param name="unitName"></param>
    /// <returns></returns>
    public static int QueryIDByName(string name)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT [UnitID]
            FROM Sys_Unit
            WHERE IsValid = 1
            AND UnitName = @UnitName";
        db.Parameters.Clear();
        db.Parameters.Add("@UnitName", name);

        GisTable tbl = db.GetTable();
        if (tbl != null && tbl.Rows.Count > 0)
        {
            return Convert.ToInt32(tbl.Rows[0]["UnitID"]);
        }

        return 0;
    }

    /// <summary>
    /// 查詢系統單位By UnitID
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public static GisTable QueryByID(string unitID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT [UnitID]
            ,[UnitName]
            ,[GovUnitTypeID]
            ,[ParentUnitID]
            ,[IsValid]
            FROM Sys_Unit
            WHERE IsValid = 1
            AND UnitID = @unitID";
        db.Parameters.Clear();
        db.Parameters.Add("@unitID", unitID);

        return db.GetTable();
    }

    /// <summary>
    /// 根據ID查詢單筆單位資料
    /// </summary>
    /// <param name="unitID">單位ID</param>
    /// <returns></returns>
    public static Sys_Unit GetUnitByID(string unitID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT [UnitID]
            ,[UnitName]
            ,[GovUnitTypeID]
            ,[ParentUnitID]
            ,[IsValid]
            FROM Sys_Unit
            WHERE IsValid = 1
            AND UnitID = @unitID";
        db.Parameters.Clear();
        db.Parameters.Add("@unitID", unitID);

        GisTable tbl = db.GetTable();
        if (tbl != null && tbl.Rows.Count > 0)
        {
            var unit = new Sys_Unit();
            unit.UnitID = Convert.ToInt32(tbl.Rows[0]["UnitID"]);
            unit.UnitName = tbl.Rows[0]["UnitName"].ToString();
            unit.GovUnitTypeID = tbl.Rows[0]["GovUnitTypeID"] != DBNull.Value ? 
                Convert.ToInt32(tbl.Rows[0]["GovUnitTypeID"]) : (int?)null;
            unit.ParentUnitID = tbl.Rows[0]["ParentUnitID"] != DBNull.Value ? 
                Convert.ToInt32(tbl.Rows[0]["ParentUnitID"]) : (int?)null;
            unit.IsValid = Convert.ToBoolean(tbl.Rows[0]["IsValid"]);
            return unit;
        }

        return null;
    }

    /// <summary>
    /// 查詢寄送OSI提醒信單位 By PeriodID
    /// </summary>
    /// <returns></returns>
    public static GisTable GetOSIReminderUnitByPeriodID(string periodID)
    {
        var rtVal = new List<string>();
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT UnitID,UnitName FROM Sys_Unit 
WHERE IsValid = 1
AND UnitName <> N'其他'
AND UnitID NOT IN (
	SELECT DISTINCT ReportingUnitID FROM OSI_ActivityReports WHERE PeriodID = @PeriodID
)
";
        db.Parameters.Clear();
        db.Parameters.Add("@PeriodID", periodID);
        return db.GetTable();        
    }

    /// <summary>
    /// 查詢自己跟Child By UnitID
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public static GisTable QueryAllChildByID(int unitID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
WITH UnitHierarchy AS (
    SELECT 
        UnitID,
        UnitName,
        ParentUnitID,
        IsValid
    FROM Sys_Unit
    WHERE UnitID = @RootID
    UNION ALL
    SELECT
        u.UnitID,
        u.UnitName,
        u.ParentUnitID,
        u.IsValid
    FROM Sys_Unit u
    INNER JOIN UnitHierarchy h
        ON u.ParentUnitID = h.UnitID
)

SELECT [UnitID]
    ,[UnitName]
    ,[ParentUnitID]
    ,[IsValid]
FROM UnitHierarchy
WHERE IsValid = 1
ORDER BY UnitID;";
        db.Parameters.Clear();
        db.Parameters.Add("@RootID", unitID);

        return db.GetTable();
    }

    /// <summary>
    /// 判斷該機關名稱是否存在
    /// </summary>
    /// <returns></returns>
    public static bool IsExistByUnitName(string unitName)
    {
        bool rtVal = false;
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT 1
            FROM Sys_Unit
            WHERE IsValid = 1
            AND UnitName = @UnitName";
        db.Parameters.Clear();
        db.Parameters.Add("@UnitName", unitName);

        GisTable tbl = db.GetTable();
        if (tbl != null && tbl.Rows.Count > 0)
        {
            rtVal = true;
        }

        return rtVal;
    }

    /// <summary>
    /// 核心查詢方法，供 QueryAllOrderByUnitID 和 QueryByGovUnitTypeID 共用
    /// </summary>
    /// <param name="govUnitTypeID">政府單位類型ID，null 表示查詢全部</param>
    /// <param name="includeGovUnitTypeName">是否包含 GovUnitType 名稱</param>
    /// <returns></returns>
    private static GisTable QueryUnitsCore(int? govUnitTypeID, bool includeGovUnitTypeName)
    {
        DbHelper db = new DbHelper();
        
        // 建立基本查詢
        string baseSelect = @"SELECT 
            u.[UnitID]
            ,u.[UnitName]
            ,u.[GovUnitTypeID]";
        
        // 決定是否需要 JOIN 和額外欄位
        string joinClause = "";
        string additionalColumns = "";
        if (includeGovUnitTypeName)
        {
            additionalColumns = @"
            ,g.[TypeName] AS GovUnitTypeName";
            joinClause = @"
            JOIN Sys_GovUnitType g ON u.GovUnitTypeID = g.TypeID";
        }
        
        // 加入其他欄位
        string otherColumns = @"
            ,u.[ParentUnitID]
            ,u.[IsValid]";
        
        // WHERE 條件
        string whereClause = @"
            WHERE u.IsValid = 1";
        if (govUnitTypeID.HasValue)
        {
            whereClause += @"
            AND u.GovUnitTypeID = @GovUnitTypeID";
        }
        
        // ORDER BY 條件
        string orderByClause = @"
            ORDER BY 
                u.GovUnitTypeID,
                CASE WHEN u.ParentUnitID IS NULL THEN u.UnitID ELSE u.ParentUnitID END,
                CASE WHEN u.ParentUnitID IS NULL THEN 0 ELSE 1 END,
                u.UnitID";
        
        // 組合完整的 SQL 語句
        db.CommandText = baseSelect + additionalColumns + otherColumns + 
                        @"
            FROM Sys_Unit u" + joinClause + whereClause + orderByClause;
        
        db.Parameters.Clear();
        if (govUnitTypeID.HasValue)
        {
            db.Parameters.Add("@GovUnitTypeID", govUnitTypeID.Value);
        }
        
        GisTable table = db.GetTable();
        
        // 處理單位名稱顯示格式
        FormatUnitNames(table);
        
        return table;
    }

    /// <summary>
    /// 格式化單位名稱（子單位前加空格）
    /// </summary>
    /// <param name="table">要格式化的資料表</param>
    private static void FormatUnitNames(GisTable table)
    {
        if (table != null && table.Rows.Count > 0)
        {
            foreach (System.Data.DataRow r in table.Rows)
            {
                string unitName = r["UnitName"].ToString();
                if (r["ParentUnitID"] != DBNull.Value && r["ParentUnitID"] != null)
                {
                    unitName = "　" + unitName;
                }
                r["UnitName"] = unitName;
            }
        }
    }

    /// <summary>
    /// 新增
    /// </summary>
    /// <param name="unit">單位資料</param>
    /// <returns></returns>
    public static bool InsertSysUser(Sys_Unit unit)
    {
        bool RtVal = true;
        DbHelper db = new DbHelper();

        db.BeginTrans();
        try
        {
            db.CommandText = @"INSERT INTO Sys_Unit
                                      ([UnitName],[ParentUnitID],[GovUnitTypeID],[IsValid])
                               VALUES (@UnitName,@ParentUnitID,@GovUnitTypeID,@IsValid)";

            db.Parameters.Clear();
            db.Parameters.Add("@UnitName", unit.UnitName);
            db.Parameters.Add("@ParentUnitID", unit.ParentUnitID);
            db.Parameters.Add("@GovUnitTypeID", unit.GovUnitTypeID);
            db.Parameters.Add("@IsValid", true);

            GisTable Dt1 = db.GetTable();
            db.Commit();
        }
        catch (Exception ex)
        {
            db.Rollback();
            RtVal = false;
        }

        return RtVal;
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <returns></returns>
    public static bool UpdateByID(Sys_Unit unit)
    {
        var db = new DbHelper();
        int rowsAffected = 0;

        db.BeginTrans();
        try
        {
            db.CommandText = @"
            UPDATE Sys_Unit
               SET UnitName = @UnitName,
                   ParentUnitID = @ParentUnitID,
                   GovUnitTypeID = @GovUnitTypeID
             WHERE UnitID  = @UnitID

            SELECT CAST(@@ROWCOUNT AS INT);
            ";
            db.Parameters.Clear();
            db.Parameters.Add("@UnitName", unit.UnitName);
            db.Parameters.Add("@ParentUnitID", unit.ParentUnitID);
            db.Parameters.Add("@GovUnitTypeID", unit.GovUnitTypeID);
            db.Parameters.Add("@UnitID", unit.UnitID);

            // 取得影響的行數
            object result = db.GetDataSet().Tables[0].Rows[0][0];
            rowsAffected = (result == null ? 0 : Convert.ToInt32(result));

            if (rowsAffected > 0)
                db.Commit();
            else
                db.Rollback();
        }
        catch
        {
            db.Rollback();
            rowsAffected = 0;
        }

        return rowsAffected > 0;
    }

    /// <summary>
    /// 刪除 By ID
    /// </summary>
    /// <returns></returns>
    public static bool DeleteByID(int unitID)
    {
        var db = new DbHelper();
        int rowsAffected = 0;

        db.BeginTrans();
        try
        {
            db.CommandText = @"
            UPDATE Sys_Unit
               SET IsValid = 0
             WHERE UnitID  = @UnitID

            SELECT CAST(@@ROWCOUNT AS INT);
            ";
            db.Parameters.Clear();
            db.Parameters.Add("@UnitID", unitID);

            // 取得影響的行數
            object result = db.GetDataSet().Tables[0].Rows[0][0];
            rowsAffected = (result == null ? 0 : Convert.ToInt32(result));

            if (rowsAffected > 0)
                db.Commit();
            else
                db.Rollback();
        }
        catch
        {
            db.Rollback();
            rowsAffected = 0;
        }

        return rowsAffected > 0;
    }

    /// <summary>
    /// 查詢所有父單位（ParentUnitID 為 NULL 的單位）
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryParentUnits()
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT [UnitID]
            ,[UnitName]
            ,[GovUnitTypeID]
            ,[ParentUnitID]
            ,[IsValid]
            FROM Sys_Unit
            WHERE IsValid = 1
            AND ParentUnitID IS NULL
            AND UnitName <> N'其他'
            ORDER BY UnitID";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 查詢父單位 (Parent = null 的單位，排除"其他") 根據機關類型
    /// </summary>
    /// <param name="govUnitTypeID">機關類型ID (1=中央機關, 2=縣市政府)</param>
    /// <returns></returns>
    public static GisTable QueryParentUnitsByGovType(int govUnitTypeID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT [UnitID]
            ,[UnitName]
            ,[GovUnitTypeID]
            ,[ParentUnitID]
            ,[IsValid]
            FROM Sys_Unit
            WHERE IsValid = 1
            AND ParentUnitID IS NULL
            AND UnitName <> N'其他'
            AND GovUnitTypeID = @GovUnitTypeID
            ORDER BY UnitID";
        db.Parameters.Clear();
        db.Parameters.Add("@GovUnitTypeID", govUnitTypeID);

        return db.GetTable();
    }

}