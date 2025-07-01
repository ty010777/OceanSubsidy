using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// SysUserApprovedAppHelper 的摘要描述
/// </summary>
public class SysUserApprovedAppHelper
{
    public SysUserApprovedAppHelper()
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
SELECT [UserAppID]
    ,[UserID]
    ,[SystemID]
FROM [OCA_OceanSubsidy].[dbo].[Sys_UserApprovedApp]
";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 查詢 By UserID
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryByUserID(string userID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [UserAppID]
    ,[UserID]
    ,[SystemID]
FROM [OCA_OceanSubsidy].[dbo].[Sys_UserApprovedApp]
WHERE [UserID] = @UserID
";
        db.Parameters.Clear();
        db.Parameters.Add("@UserID", userID);

        return db.GetTable();
    }

    /// <summary>
    /// 新增
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public static bool InsertSysUserBasic(Sys_UserApprovedApp userApprovedApp)
    {
        bool RtVal = true;
        DbHelper db = new DbHelper();

        db.BeginTrans();
        try
        {
            db.CommandText = @"
INSERT INTO [dbo].[Sys_UserApprovedApp]
    ([UserID],[SystemID])
VALUES (@UserID,@SystemID)";

            db.Parameters.Clear();
            db.Parameters.Add("@UserID", userApprovedApp.UserID);
            db.Parameters.Add("@SystemID", userApprovedApp.SystemID);

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
}