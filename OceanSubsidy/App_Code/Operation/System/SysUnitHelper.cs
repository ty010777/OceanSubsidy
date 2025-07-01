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
            ,[ParentUnitID]
            ,[IsValid]
            FROM Sys_Unit
            WHERE IsValid = 1";
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
                                      ([UnitName],[ParentUnitID],[IsValid])
                               VALUES (@UnitName,@ParentUnitID,@IsValid)";

            db.Parameters.Clear();
            db.Parameters.Add("@UnitName", unit.UnitName);
            db.Parameters.Add("@ParentUnitID", unit.ParentUnitID);
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
                   ParentUnitID = @ParentUnitID
             WHERE UnitID  = @UnitID

            SELECT CAST(@@ROWCOUNT AS INT);
            ";
            db.Parameters.Clear();
            db.Parameters.Add("@UnitName", unit.UnitName);
            db.Parameters.Add("@ParentUnitID", unit.ParentUnitID);
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
}