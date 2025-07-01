using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;
using static GS.App.Utility.Cryptography;
using System.Web.UI;
using System.Security.Policy;
using System.Data.SqlClient;


/// <summary>
/// SysPermissionHelper 的摘要描述
/// </summary>
public class SysPermissionHelper
{
    public SysPermissionHelper()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }

    /// <summary>
    /// 查詢所有
    /// </summary>
    /// <returns></returns>
    public static string QueryPermissionCodeByUrl(string url)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT PermissionCode 
                   FROM Sys_Permission 
                  WHERE Url = @Url";
        db.Parameters.Clear();
        db.Parameters.Add("@Url", url);

        var tbl = db.GetTable();
        if (tbl == null || tbl.Rows.Count == 0)
            return null;

        return tbl.Rows[0]["PermissionCode"].ToString();
    }

    /// <summary>
    /// 回傳使用者是否在某父 PermissionCode 底下，擁有任何子權限
    /// </summary>
    public static bool HasAnyChildPermsWithOSIByID(string userId, string parentCode)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
SELECT COUNT(*) AS NUM
FROM Sys_User u
JOIN OSI_RolePermission rp ON u.OSI_RoleID = rp.RoleID
JOIN Sys_Permission p      ON rp.PermissionID = p.PermissionID
JOIN Sys_Permission child ON p.PermissionID = child.ParentID
WHERE u.UserID = @UserID
AND p.PermissionCode = @ParentCode";
        db.Parameters.Clear();
        db.Parameters.Add("@UserID", userId);
        db.Parameters.Add("@ParentCode", parentCode);

        var tbl = db.GetTable();
        if (tbl == null || tbl.Rows.Count == 0)
            return false;

        return (int)tbl.Rows[0]["NUM"] > 0;
    }

    /// <summary>
    /// 回傳使用者是否在某父 PermissionCode 底下，擁有任何子權限
    /// </summary>
    public static bool HasAnyChildPermsWithOSIByAccount(string account, string parentCode)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
SELECT COUNT(*) AS NUM
FROM Sys_User u
JOIN OSI_RolePermission rp ON u.OSI_RoleID = rp.RoleID
JOIN Sys_Permission p      ON rp.PermissionID = p.PermissionID
JOIN Sys_Permission child ON p.PermissionID = child.ParentID
WHERE u.Account = @Account
AND p.PermissionCode = @ParentCode";
        db.Parameters.Clear();
        db.Parameters.Add("@Account", account);
        db.Parameters.Add("@ParentCode", parentCode);

        var tbl = db.GetTable();
        if (tbl == null || tbl.Rows.Count == 0)
            return false;

        return (int)tbl.Rows[0]["NUM"] > 0;
    }
}