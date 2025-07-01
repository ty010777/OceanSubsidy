using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;

/// <summary>
/// OFSRoleHelper 的摘要描述
/// </summary>
public class OFSRoleHelper
{
    public OFSRoleHelper()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }

    public static GisTable QueryAll()
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT [RoleID]
              ,[RoleName]
              ,[Sort]
              ,[IsValid]
            FROM OFS_Role
            WHERE IsValid = 1
            ORDER BY Sort";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 查詢 By RoleName
    /// </summary>
    /// <returns></returns>
    public static string QueryIDByRoleName(string roleName)
    {
        var rtVal = "";
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT [RoleID]
            FROM OFS_Role
            WHERE IsValid = 1
            AND RoleName = @roleName";
        db.Parameters.Clear();
        db.Parameters.Add("@roleName", roleName);
        var tbl = db.GetTable();
        if (tbl != null)
            rtVal = tbl.Rows[0][0].ToString();

        return rtVal;
    }

    /// <summary>
    /// 查詢 By ID
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryByID(string roleID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT [RoleID]
              ,[RoleName]
              ,[Sort]
              ,[IsValid]
            FROM OFS_Role
            WHERE IsValid = 1
            AND RoleID = @RoleID";
        db.Parameters.Clear();
        db.Parameters.Add("@RoleID", roleID);

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
SELECT r.[RoleID]
    ,r.[RoleName]
    ,r.[Sort]
FROM Sys_User u
JOIN Sys_UserOFSRole uor ON u.UserID = uor.UserID
JOIN OFS_Role r ON r.RoleID = uor.RoleID
WHERE u.IsValid = 1
AND r.IsValid = 1
AND u.UserID = @UserID";
        db.Parameters.Clear();
        db.Parameters.Add("@UserID", userID);

        return db.GetTable();
    }
}