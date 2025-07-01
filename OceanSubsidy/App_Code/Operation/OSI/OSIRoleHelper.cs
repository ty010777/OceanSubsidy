using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;

/// <summary>
/// OSIRoleHelper 的摘要描述
/// </summary>
public class OSIRoleHelper
{
    public OSIRoleHelper()
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
            @"SELECT [RoleID]
              ,[RoleName]
              ,[Sort]
              ,[IsValid]
            FROM OSI_Role
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
        var rtVal = string.Empty;
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT [RoleID]
            FROM OSI_Role
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
            FROM OSI_Role
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
    public static GisTable QueryByUserID(int userID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
 SELECT r.[RoleID]
        ,r.[RoleName]
        ,r.[Sort]
FROM OSI_Role r
JOIN Sys_User u ON u.OSI_RoleID = r.RoleID
WHERE r.IsValid = 1
AND u.IsValid = 1
AND u.UserID = @UserID";
        db.Parameters.Clear();
        db.Parameters.Add("@UserID", userID);

        return db.GetTable();
    }

}