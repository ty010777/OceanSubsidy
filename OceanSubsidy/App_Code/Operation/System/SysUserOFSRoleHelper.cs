using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// SysUserOFSRoleHelper 的摘要描述
/// </summary>
public class SysUserOFSRoleHelper
{
    public SysUserOFSRoleHelper()
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
SELECT [UserOFSRoleID]
    ,[UserID]
    ,[RoleID]
FROM [OCA_OceanSubsidy].[dbo].[Sys_UserOFSRole]
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
SELECT [UserOFSRoleID]
    ,[UserID]
    ,[RoleID]
FROM [OCA_OceanSubsidy].[dbo].[Sys_UserOFSRole]
WHERE [UserID] = @UserID
";
        db.Parameters.Clear();
        db.Parameters.Add("@UserID", userID);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢 By UserID
    /// </summary>
    /// <returns></returns>
    public static List<Sys_UserOFSRole> QueryByUserIDWithClass(string userID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [UserOFSRoleID]
    ,[UserID]
    ,[RoleID]
FROM [OCA_OceanSubsidy].[dbo].[Sys_UserOFSRole]
WHERE [UserID] = @UserID
";
        db.Parameters.Clear();
        db.Parameters.Add("@UserID", userID);

        return db.GetList<Sys_UserOFSRole>();
    }    

    /// <summary>
    /// 新增
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public static bool InsertSysUserBasic(Sys_UserOFSRole userOFSRole)
    {
        bool RtVal = true;
        DbHelper db = new DbHelper();

        db.BeginTrans();
        try
        {
            db.CommandText = @"
INSERT INTO [dbo].[Sys_UserOFSRole]
    ([UserID],[RoleID])
VALUES (@UserID,@RoleID)";

            db.Parameters.Clear();
            db.Parameters.Add("@UserID", userOFSRole.UserID);
            db.Parameters.Add("@RoleID", userOFSRole.RoleID);

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