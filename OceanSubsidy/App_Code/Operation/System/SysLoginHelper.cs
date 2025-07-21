using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;
using static iTextSharp.text.pdf.AcroFields;


/// <summary>
/// SysLoginHelper 的摘要描述
/// </summary>
public class SysLoginHelper
{
    public SysLoginHelper()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
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
SELECT
    [LoginID]
    ,[UserID]
    ,[LoginIP]
    ,[LoginTime]
FROM [OCA_OceanSubsidy].[dbo].[Sys_Login]
ORDER BY LoginTime DESC";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 新增資料
    /// </summary>
    /// <returns></returns>
    public static bool Insert(Sys_Login login)
    {
        bool RtVal = true;
        DbHelper db = new DbHelper();
        db.BeginTrans();
        try
        {
            db.CommandText =
                @"
INSERT INTO [dbo].[Sys_Login]
    ([UserID],[LoginIP],[LoginTime])
VALUES (@UserID,@LoginIP,GETDATE())
";
            db.Parameters.Clear();
            db.Parameters.Add("@UserID", login.UserID);
            db.Parameters.Add("@LoginIP", login.LoginIP);

            db.ExecuteNonQuery();
            db.Commit();
            RtVal = true;
        }
        catch (Exception ex)
        {
            db.Rollback();
            RtVal = false;
        }

        return RtVal;
    }

    /// <summary>
    /// 查詢登入歷程（含使用者資料）
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryLoginHistory()
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT 
    L.LoginID,
    L.UserID,
    L.LoginIP,
    L.LoginTime,
    U.Account,
    U.Name,
    CASE 
        WHEN U.UnitID IS NOT NULL THEN 
            ISNULL((SELECT UnitName FROM Sys_Unit WHERE UnitID = U.UnitID), U.UnitName)
        ELSE 
            U.UnitName
    END AS UnitName
FROM [OCA_OceanSubsidy].[dbo].[Sys_Login] L
INNER JOIN [OCA_OceanSubsidy].[dbo].[Sys_User] U ON L.UserID = U.UserID
WHERE U.IsValid = 1
ORDER BY L.LoginTime DESC";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 查詢使用者最後登入時間
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryLastLoginTimeByUsers()
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT 
    UserID,
    MAX(LoginTime) AS LastLoginTime
FROM [OCA_OceanSubsidy].[dbo].[Sys_Login]
GROUP BY UserID";
        db.Parameters.Clear();

        return db.GetTable();
    }

}