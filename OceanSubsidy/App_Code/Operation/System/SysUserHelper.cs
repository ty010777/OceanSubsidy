using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Entity.Base;
using static GS.App.Utility.Cryptography;

/// <summary>
/// SysUserHelper 的摘要描述
/// </summary>
public class SysUserHelper
{
    public SysUserHelper()
    {
       
    }

    /// <summary>
    /// 查詢系統使用者
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryAllUser()
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT [UserID]
                ,[UnitType]
                ,[UnitID]
                ,[UnitName]
                ,[Account]
                ,[Pwd]
                ,[Name]
                ,[Tel]
                ,[OSI_RoleID]
                ,[IsReceiveMail]
                ,[IsApproved]
                ,[IsValid]
                ,[Salt]
                ,[CreateTime]
                ,[UpdateTime]
                ,[PwdToken]
                ,[IsActive]
            FROM Sys_User
            WHERE IsValid = 1";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 查詢系統使用者
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryByUnitType(string unitType)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT [UserID]
                ,[UnitType]
                ,[UnitID]
                ,[UnitName]
                ,[Account]
                ,[Pwd]
                ,[Name]
                ,[Tel]
                ,[OSI_RoleID]
                ,[IsReceiveMail]
                ,[IsApproved]
                ,[IsValid]
                ,[Salt]
                ,[CreateTime]
                ,[UpdateTime]
                ,[PwdToken]
                ,[IsActive]
            FROM Sys_User
            WHERE IsValid = 1
            AND UnitType = @UnitType";
        db.Parameters.Clear();
        db.Parameters.Add("@UnitType", unitType);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢系統使用者
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryByUnitTypeAndOSIUser(string unitType)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT [UserID]
                ,[UnitType]
                ,[UnitID]
                ,[UnitName]
                ,[Account]
                ,[Pwd]
                ,[Name]
                ,[Tel]
                ,[OSI_RoleID]
                ,[IsReceiveMail]
                ,[IsApproved]
                ,[IsValid]
                ,[Salt]
                ,[CreateTime]
                ,[UpdateTime]
                ,[PwdToken]
                ,[IsActive]
            FROM Sys_User
            WHERE IsValid = 1
            AND UnitType = @UnitType
            AND OSI_RoleID IS NOT NULL";
        db.Parameters.Clear();
        db.Parameters.Add("@UnitType", unitType);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢系統使用者
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryByUnitTypeAndOFSUser(string unitType)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT [UserID]
                ,[UnitType]
                ,[UnitID]
                ,[UnitName]
                ,[Account]
                ,[Pwd]
                ,[Name]
                ,[Tel]
                ,[OSI_RoleID]
                ,[IsReceiveMail]
                ,[IsApproved]
                ,[IsValid]
                ,[Salt]
                ,[CreateTime]
                ,[UpdateTime]
                ,[PwdToken]
                ,[IsActive]
            FROM Sys_User
            WHERE IsValid = 1
            AND UnitType = @UnitType
            AND UserID IN(
                SELECT DISTINCT UserID FROM Sys_UserOFSRole
            )";
        db.Parameters.Clear();
        db.Parameters.Add("@UnitType", unitType);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢海洋科學調查使用者
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryOSIUser()
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT [UserID]
                ,[UnitType]
                ,[UnitID]
                ,[Account]
                ,[Pwd]
                ,[Name]
                ,[Tel]
                ,[OSI_RoleID]
                ,[IsReceiveMail]
                ,[IsApproved]
                ,[IsValid]
                ,[Salt]
                ,[CreateTime]
                ,[UpdateTime]
                ,[PwdToken]
            FROM Sys_User
            WHERE IsValid = 1
            AND OSI_RoleID IS NOT NULL";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 查詢待審核使用者
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryPendingUsers()
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [UserID]
    ,[UnitID]
    ,[Account]
    ,[Pwd]
    ,[Name]
    ,[Tel]
    ,[OSI_RoleID]
    ,[IsReceiveMail]
    ,[IsApproved]
    ,[IsValid]
    ,[Salt]
    ,[CreateTime]
    ,[UpdateTime]
    ,[PwdToken]
    ,[IsActive]
    ,[UnitName]
    ,[UnitType]
    ,[ApprovedSource]
FROM [OCA_OceanSubsidy].[dbo].[Sys_User]
WHERE IsValid = 1
AND IsApproved = 0";
        db.Parameters.Clear();

        return db.GetTable();
    }

    /// <summary>
    /// 查詢系統使用者
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryUserInfoByID(int userID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT
u.UserID
,u.UnitType
,u.Account
,u.Name
,u.UnitID
,ut.UnitName
,osir.RoleName AS OSI_RoleName
FROM Sys_User u
LEFT JOIN Sys_Unit ut ON u.UnitID = ut.UnitID
LEFT JOIN OSI_Role osir ON u.OSI_RoleID = osir.RoleID
WHERE u.IsValid = 1
AND u.UserID = @UserID
";
        db.Parameters.Clear();
        db.Parameters.Add("@UserID", userID);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢待審核使用者 ByID
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryPendingUserByID(string userID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [UserID]
    ,[UnitID]
    ,[Account]
    ,[Pwd]
    ,[Name]
    ,[Tel]
    ,[OSI_RoleID]
    ,[IsReceiveMail]
    ,[IsApproved]
    ,[IsValid]
    ,[Salt]
    ,[CreateTime]
    ,[UpdateTime]
    ,[PwdToken]
    ,[IsActive]
    ,[UnitName]
    ,[UnitType]
    ,[ApprovedSource]
FROM [OCA_OceanSubsidy].[dbo].[Sys_User]
WHERE IsValid = 1
AND IsApproved = 0
AND UserID = @userID";
        db.Parameters.Clear();
        db.Parameters.Add("@userID", userID);

        return db.GetTable();
    }

    /// <summary>
    /// 欲申請系統BY USERID
    /// </summary>
    /// <returns></returns>
    public static string QueryApprovedSysByUserID(string userID)
    {
        string rtVal = "";
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT a.SystemName
FROM Sys_User u
JOIN Sys_UserApprovedApp uaa ON u.UserID = uaa.UserID
JOIN Sys_App a ON a.SystemID = uaa.SystemID
WHERE u.UserID = @userID";
        db.Parameters.Clear();
        db.Parameters.Add("@userID", userID);
        var tbl = db.GetTable();
        rtVal = string.Join("、", tbl.AsEnumerable().Select(row => row["SystemName"].ToString()));

        return rtVal;
    }

    /// <summary>
    /// 查詢系統使用者 By ID
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryUserInfoByID(string userID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT us.[UserID]
                ,us.[UnitType]
                ,us.[UnitID]
                ,us.[Account]
                ,us.[Pwd]
                ,us.[Name]
                ,us.[Tel]
                ,us.[OSI_RoleID]
                ,us.[IsReceiveMail]
                ,us.[IsApproved]
                ,us.[IsValid]
                ,us.[Salt]
                ,us.[CreateTime]
                ,us.[UpdateTime]
                ,us.[PwdToken]
                ,us.[IsActive]
                ,un.[UnitName]
                ,un.[ParentUnitID]
                ,r.[RoleName] AS OSIRoleName
            FROM Sys_User us
            LEFT JOIN Sys_Unit un ON un.UnitID = us.UnitID
            JOIN OSI_Role r ON us.OSI_RoleID = r.RoleID
            WHERE us.IsValid = 1
            AND us.UserID = @userID";
        db.Parameters.Clear();
        db.Parameters.Add("@userID", userID);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢系統使用者 By ID
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryUserByID(string userID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT [UserID]
    ,[UnitID]
    ,[Account]
    ,[Pwd]
    ,[Name]
    ,[Tel]
    ,[OSI_RoleID]
    ,[IsReceiveMail]
    ,[IsApproved]
    ,[IsValid]
    ,[Salt]
    ,[CreateTime]
    ,[UpdateTime]
    ,[PwdToken]
    ,[IsActive]
    ,[UnitName]
    ,[UnitType]
    ,[ApprovedSource]
FROM [OCA_OceanSubsidy].[dbo].[Sys_User]
WHERE IsValid = 1
AND UserID = @userID";
        db.Parameters.Clear();
        db.Parameters.Add("@userID", userID);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢使用者UnitName
    /// </summary>
    /// <returns></returns>
    public static string QueryUnitNameByUserID(string userID)
    {
        string rtVal = string.Empty;
        DbHelper db = new DbHelper();
        var tbl = SysUserHelper.QueryUserByID(userID);
        if (tbl != null && tbl.Rows.Count > 0)
        {
            string govID = SysUnitTypeHelper.QueryIDByTypeName("政府機關").ToString();
            string otherUnitID = "";
            var otherTbl = SysUnitHelper.QueryByUnitName("其他");
            if (otherTbl != null && otherTbl.Rows.Count > 0)
                otherUnitID = otherTbl.Rows[0]["UnitID"].ToString();

            var data = tbl.Rows[0];
            if (data["UnitType"].ToString() == govID && data["UnitID"].ToString() != otherUnitID)
            {
                var unitTbl = SysUnitHelper.QueryByID(data["UnitID"].ToString());
                if (unitTbl != null && unitTbl.Rows.Count > 0)
                    rtVal = unitTbl.Rows[0]["UnitName"].ToString();
            }
            else
            {
                rtVal = data["UnitName"].ToString();
            }
        }

        db.Parameters.Clear();

        return rtVal;
    }

    /// <summary>
    /// 查詢系統使用者 By ID With Sys_User
    /// </summary>
    /// <returns></returns>
    public static Sys_User QueryUserByIDWithClass(string userID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT [UserID]
    ,[UnitID]
    ,[Account]
    ,[Pwd]
    ,[Name]
    ,[Tel]
    ,[OSI_RoleID]
    ,[IsReceiveMail]
    ,[IsApproved]
    ,[IsValid]
    ,[Salt]
    ,[CreateTime]
    ,[UpdateTime]
    ,[PwdToken]
    ,[IsActive]
    ,[UnitName]
    ,[UnitType]
    ,[ApprovedSource]
FROM [OCA_OceanSubsidy].[dbo].[Sys_User]
WHERE IsValid = 1
AND UserID = @userID";
        db.Parameters.Clear();
        db.Parameters.Add("@userID", userID);

        return db.GetList<Sys_User>().FirstOrDefault();
    }

    /// <summary>
    /// 查詢系統使用者 By Acount
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryUserByAccount(string account)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT [UserID]
                ,[UnitType]
                ,[UnitID]
                ,[Account]
                ,[Pwd]
                ,[Name]
                ,[Tel]
                ,[OSI_RoleID]
                ,[IsReceiveMail]
                ,[IsApproved]
                ,[IsValid]
                ,[Salt]
                ,[CreateTime]
                ,[UpdateTime]
                ,[PwdToken]
            FROM Sys_User
            WHERE IsValid = 1
            AND Account = @account";
        db.Parameters.Clear();
        db.Parameters.Add("@account", account);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢系統使用者 By UnitID
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryUserByUnitID(string unitID)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT [UserID]
                ,[UnitType]
                ,[UnitID]
                ,[Account]
                ,[Pwd]
                ,[Name]
                ,[Tel]
                ,[OSI_RoleID]
                ,[IsReceiveMail]
                ,[IsApproved]
                ,[IsValid]
                ,[Salt]
                ,[CreateTime]
                ,[UpdateTime]
                ,[PwdToken]
            FROM Sys_User
            WHERE IsValid = 1
            AND UnitID = @unitID";
        db.Parameters.Clear();
        db.Parameters.Add("@unitID", unitID);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢 By OSIRoleName
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryUserByOSIRoleName(string roleName)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT 
                s.Account
            FROM Sys_User s
            JOIN OSI_Role r ON s.OSI_RoleID = r.RoleID
            WHERE s.IsValid = 1
            AND RoleName = @roleName";
        db.Parameters.Clear();
        db.Parameters.Add("@roleName", roleName);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢 By OFSRoleName
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryUserByOFSRoleName(string roleName)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT 
	s.Account
FROM Sys_User s
JOIN Sys_UserOFSRole uor ON uor.UserID = s.UserID
JOIN OFS_Role r ON uor.RoleID = r.RoleID
WHERE s.IsValid = 1
AND r.RoleName = @roleName";
        db.Parameters.Clear();
        db.Parameters.Add("@roleName", roleName);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢設定密碼資訊 By PwdToken
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryPwdInfoByPwdToken(string pwdToken)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT us.[Account],
                us.[Salt]
            FROM Sys_User us
            WHERE IsValid = 1
            AND IsApproved = 1
            AND pwdToken = @pwdToken";
        db.Parameters.Clear();
        db.Parameters.Add("@pwdToken", pwdToken);

        return db.GetTable();
    }

    /// <summary>
    /// 查詢寄送OSI提醒信 By unitID
    /// </summary>
    /// <returns></returns>
    public static List<string> GetOSIReminderUserByUnitID(string unitID)
    {
        var rtVal = new List<string>();
        DbHelper db = new DbHelper();
        db.CommandText =
            @"
SELECT u.Account
FROM Sys_User u
JOIN OSI_Role r ON u.OSI_RoleID = r.RoleID
WHERE (r.RoleName = N'機關填報端' 
OR r.RoleName = N'計畫執行端')
AND u.IsValid = 1
AND u.IsActive = 1
AND u.UnitID = @UnitID

";
        db.Parameters.Clear();
        db.Parameters.Add("@UnitID", unitID);
        var tbl = db.GetTable();
        if (tbl != null && tbl.Rows.Count > 0)
        {
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                rtVal.Add(tbl.Rows[i]["Account"].ToString());
            }
        }

        return rtVal;
    }

    /// <summary>
    /// 取得OSI使用者的權限
    /// </summary>
    /// <param name="userID"></param>
    /// <returns></returns>
    public static List<string> GetOSIPermsByAccount(string account)
    {
        DbHelper db = new DbHelper();
        List<string> permissions = new List<string>();
        db.CommandText =
            @"
WITH Assigned AS (
    SELECT rp.PermissionID
      FROM OSI_RolePermission rp
      JOIN Sys_User u
        ON rp.RoleID = u.OSI_RoleID
    WHERE u.Account = @account
    AND u.IsValid = 1
),
RecursivePerms AS (
    SELECT PermissionID
      FROM Assigned
    UNION ALL
    SELECT p.PermissionID
      FROM Sys_Permission p
      JOIN RecursivePerms r
        ON p.ParentID = r.PermissionID
)
SELECT DISTINCT p.PermissionCode
  FROM Sys_Permission p
  JOIN RecursivePerms r
    ON p.PermissionID = r.PermissionID;
";
        db.Parameters.Clear();
        db.Parameters.Add("@account", account);

        var tbl = db.GetTable();

        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            permissions.Add(tbl.Rows[i]["PermissionCode"].ToString());
        }

        return permissions;
    }

    /// <summary>
    /// 更新pwdToken
    /// </summary>
    /// <returns></returns>
    public static string UpdatePwdToken(string account)
    {
        // 1. 產生新的 Token 及到期時間
        string token = Guid.NewGuid().ToString();

        var db = new DbHelper();
        int rowsAffected = 0;
        db.BeginTrans();
        try
        {
            // 2. 更新該帳號的 PwdToken 欄位
            db.CommandText = @"
            UPDATE Sys_User
               SET PwdToken     = @token,
                   UpdateTime   = GETDATE()   
             WHERE IsValid = 1
            AND Account = @account;

             SELECT CAST(@@ROWCOUNT AS INT);
            ";
            db.Parameters.Clear();
            db.Parameters.Add("@token", token);
            db.Parameters.Add("@account", account);

            // 取得影響的行數
            object result = db.GetDataSet().Tables[0].Rows[0][0];
            rowsAffected = (result == null ? 0 : Convert.ToInt32(result));

            if (rowsAffected > 0)
            {
                db.Commit();
                return token;
            }
            else
            {
                db.Rollback();
                return null;
            }
        }
        catch
        {
            db.Rollback();
            return null;
        }

    }

    /// <summary>
    /// 寄密碼重置信
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public static bool SendResetPwdMail(string account)
    {
        bool rtVal = true;
        // 產生新的 Token
        var token = SysUserHelper.UpdatePwdToken(account);

        if (token == null)
            return false;

        // 帳號存在，呼叫 Helper 寄出重置信
        string mailBody = MailContent.OCA.ResetPassword.getMail(token);
        rtVal = GS.App.Utility.Mail.SendMail(account, "", MailContent.OCA.ResetPassword.Subject, mailBody, out string ErrorMsg);

        return rtVal;
    }

    /// <summary>
    /// 寄密碼設置信
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public static bool SendSetPwdMail(string account)
    {
        bool rtVal = true;
        // 產生新的 Token
        var token = SysUserHelper.UpdatePwdToken(account);

        if (token == null)
            return false;

        // 帳號存在，呼叫 Helper 寄出重置信
        string mailBody = MailContent.OCA.SetPassword.getMail(token);
        rtVal = GS.App.Utility.Mail.SendMail(account, "", MailContent.OCA.SetPassword.Subject, mailBody, out string ErrorMsg);

        return rtVal;
    }

    /// <summary>
    /// 寄啟用帳號信
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public static bool SendActiveAccountMail(string account)
    {
        bool rtVal = true;
        // 產生新的 Token
        var token = SysUserHelper.UpdatePwdToken(account);

        if (token == null)
            return false;

        // 帳號存在，呼叫 Helper 寄出重置信
        string mailBody = MailContent.OCA.ActiveAccount.getMail(account, token);
        rtVal = GS.App.Utility.Mail.SendMail(account, "", MailContent.OCA.ActiveAccount.Subject, mailBody, out string ErrorMsg);

        return rtVal;
    }

    /// <summary>
    /// 更新使用者
    /// </summary>
    /// <returns></returns>
    public static bool UpdateUser(Sys_User user, List<Sys_UserOFSRole> addOFS_Roles, List<Sys_UserOFSRole> deleteOFS_Roles)
    {
        // uniqueidentifier 處理
        if (user.PwdToken == "")
            user.PwdToken = null;
        if (user.Salt == "")
            user.Salt = null;

        var db = new DbHelper();
        int rowsAffected = 0;

        db.BeginTrans();
        try
        {
            db.CommandText = @"
            UPDATE Sys_User
               SET  UnitType = @UnitType,
                    UnitID = @UnitID,
                    Account = @Account,
                    Pwd = @Pwd,
                    Name = @Name,
                    Tel = @Tel,
                    OSI_RoleID = @OSI_RoleID,
                    IsReceiveMail = @IsReceiveMail,
                    IsApproved = @IsApproved,
                    IsValid = @IsValid,
                    Salt = @Salt,
                    CreateTime = @CreateTime,
                    UpdateTime = GETDATE(),
                    PwdToken = @PwdToken
            WHERE UserID = @UserID

            SELECT CAST(@@ROWCOUNT AS INT);";

            db.Parameters.Clear();
            db.Parameters.Add("@UnitType", user.UnitType);
            db.Parameters.Add("@UnitID", user.UnitID);
            db.Parameters.Add("@Account", user.Account);
            db.Parameters.Add("@Pwd", user.Pwd);
            db.Parameters.Add("@Name", user.Name);
            db.Parameters.Add("@Tel", user.Tel);
            db.Parameters.Add("@OSI_RoleID", user.OSI_RoleID);
            db.Parameters.Add("@IsReceiveMail", user.IsReceiveMail);
            db.Parameters.Add("@IsApproved", user.IsApproved);
            db.Parameters.Add("@IsValid", user.IsValid);
            db.Parameters.Add("@Salt", user.Salt);
            db.Parameters.Add("@CreateTime", user.CreateTime);
            db.Parameters.Add("@UpdateTime", user.UpdateTime);
            db.Parameters.Add("@PwdToken", user.PwdToken);
            db.Parameters.Add("@UserID", user.UserID);

            // 取得影響的行數
            object result = db.GetDataSet().Tables[0].Rows[0][0];
            rowsAffected = (result == null ? 0 : Convert.ToInt32(result));

            // 更新 Sys_UserOFSRole
            foreach (var item in addOFS_Roles)
            {
                db.CommandText = @"
INSERT INTO [dbo].[Sys_UserOFSRole]
    ([UserID],[RoleID])
VALUES (@UserID,@RoleID)";

                db.Parameters.Clear();
                db.Parameters.Add("@UserID", item.UserID);
                db.Parameters.Add("@RoleID", item.RoleID);

                db.ExecuteNonQuery();
            }

            foreach (var item in deleteOFS_Roles)
            {
                db.CommandText = @"
DELETE [Sys_UserOFSRole] WHERE UserId = @UserID AND RoleID = @RoleID";

                db.Parameters.Clear();
                db.Parameters.Add("@UserID", item.UserID);
                db.Parameters.Add("@RoleID", item.RoleID);

                db.ExecuteNonQuery();
            }

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
    /// 更新密碼
    /// </summary>
    /// <returns></returns>
    public static bool UpdatePwd(string pwdToken, string account, string salt, string newPwd)
    {
        // 用 AES-GCM 加密新密碼
        string encryptedPwd = AESGCM.EncryptText(newPwd, salt);

        var db = new DbHelper();
        int rowsAffected = 0;

        db.BeginTrans();
        try
        {
            db.CommandText = @"
            UPDATE Sys_User
               SET Pwd = @Pwd,
                   PwdToken  = NULL,
                   UpdateTime = GETDATE()
             WHERE Account  = @Account
               AND PwdToken = @PwdToken
               AND IsValid = 1

            SELECT CAST(@@ROWCOUNT AS INT);
            ";
            db.Parameters.Clear();
            db.Parameters.Add("@Pwd", encryptedPwd);
            db.Parameters.Add("@Account", account);
            db.Parameters.Add("@PwdToken", pwdToken);

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
    /// 更新停用狀態 By LastLoginTime
    /// </summary>
    /// <returns></returns>
    public static int UpdateActiveByLastLoginTime()
    {
        var db = new DbHelper();
        int rowsAffected = 0;

        db.BeginTrans();
        try
        {
            db.CommandText = @"
UPDATE Sys_User
SET IsActive = 0
WHERE UserID IN (
    SELECT u.UserID
    FROM Sys_User u
    LEFT JOIN (
        SELECT UserID, MAX(LoginTime) AS LastLoginTime
        FROM Sys_Login
        GROUP BY UserID
    ) l ON u.UserID = l.UserID
    WHERE u.IsActive = 1
    AND u.IsValid = 1
    AND (
        (l.LastLoginTime IS NULL AND u.UpdateTime < DATEADD(year, -1, GETDATE()))
        OR (l.LastLoginTime IS NOT NULL AND l.LastLoginTime < DATEADD(year, -1, GETDATE()))
    )
)
AND IsActive = 1;

SELECT CAST(@@ROWCOUNT AS INT);
            ";
            db.Parameters.Clear();

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

        return rowsAffected;
    }


    /// <summary>
    /// 審核通過更新
    /// </summary>
    /// <returns></returns>
    public static bool ApproveUser(Sys_User user, List<Sys_UserOFSRole> OFS_Roles)
    {
        var db = new DbHelper();
        int rowsAffected = 0;
        db.BeginTrans();
        try
        {
            // 2. 更新該帳號的 PwdToken 欄位
            db.CommandText = @"
            UPDATE Sys_User
                SET UnitType = @UnitType,
                    UnitID = @UnitID,
                    UnitName = @UnitName,
                    Account = @Account,
                    Name = @Name,
                    Tel = @Tel,
                    OSI_RoleID = @OSI_RoleID,
                    IsReceiveMail = @IsReceiveMail,
                    IsApproved = @IsApproved,
                    UpdateTime = GETDATE()   
             WHERE IsValid = 1
            AND UserID = @UserID;

             SELECT CAST(@@ROWCOUNT AS INT);
            ";
            db.Parameters.Clear();
            db.Parameters.Add("@UnitType", user.UnitType);
            db.Parameters.Add("@UnitID", user.UnitID);
            db.Parameters.Add("@UnitName", user.UnitName);
            db.Parameters.Add("@Account", user.Account);
            db.Parameters.Add("@Name", user.Name);
            db.Parameters.Add("@Tel", user.Tel);
            db.Parameters.Add("@OSI_RoleID", user.OSI_RoleID);
            db.Parameters.Add("@IsReceiveMail", user.IsReceiveMail);
            db.Parameters.Add("@IsApproved", user.IsApproved);
            db.Parameters.Add("@UserID", user.UserID);

            // 取得影響的行數
            object result = db.GetDataSet().Tables[0].Rows[0][0];
            rowsAffected = (result == null ? 0 : Convert.ToInt32(result));

            // 更新 Sys_UserOFSRole
            foreach (var item in OFS_Roles)
            {
                db.CommandText = @"
INSERT INTO [dbo].[Sys_UserOFSRole]
    ([UserID],[RoleID])
VALUES (@UserID,@RoleID)";

                db.Parameters.Clear();
                db.Parameters.Add("@UserID", item.UserID);
                db.Parameters.Add("@RoleID", item.RoleID);

                db.ExecuteNonQuery();
            }

            if (rowsAffected > 0)
            {
                db.Commit();
                return true;
            }
            else
            {
                db.Rollback();
                return false;
            }
        }
        catch
        {
            db.Rollback();
            return false;
        }

    }

    /// <summary>
    /// 查詢帳號是否存在
    /// </summary>
    /// <returns></returns>
    public static bool IsExistAccount(string account)
    {
        bool exists = false;

        var table = SysUserHelper.QueryAllUser();
        account = account.Trim();

        foreach (DataRow row in table.Rows)
        {
            if (string.Equals(
                    row["Account"].ToString(),
                    account,
                    StringComparison.OrdinalIgnoreCase))
            {
                exists = true;
                break;
            }
        }

        return exists;
    }

    /// <summary>
    /// 新增帳號(基本資料)
    /// </summary>
    /// <param name="user">使用者資料</param>
    /// <returns></returns>
    public static bool InsertSysUserBasic(Sys_User user, List<int> approvedSysID)
    {
        bool RtVal = true;
        int userID = 0;
        DbHelper db = new DbHelper();

        db.BeginTrans();
        try
        {
            db.CommandText = @"
INSERT INTO Sys_User
        ([UnitType],[UnitID],[UnitName],[Account],[Name],[Tel],[ApprovedSource])
VALUES (@UnitType,@UnitID,@UnitName,@Account,@Name,@Tel,@ApprovedSource)

 SELECT SCOPE_IDENTITY();
";

            db.Parameters.Clear();
            db.Parameters.Add("@UnitType", user.UnitType);
            db.Parameters.Add("@UnitID", user.UnitID);
            db.Parameters.Add("@UnitName", user.UnitName);
            db.Parameters.Add("@Account", user.Account);
            db.Parameters.Add("@Name", user.Name);
            db.Parameters.Add("@Tel", user.Tel);
            db.Parameters.Add("@ApprovedSource", user.ApprovedSource);

            object result = db.GetDataSet().Tables[0].Rows[0][0];
            if (result == null || result == DBNull.Value)
                throw new Exception("取得 UserID 失敗：結果為 null 或 DBNull");
            userID = Convert.ToInt32(result);

            foreach (int sysID in approvedSysID)
            {
                db.CommandText = @"
                INSERT INTO Sys_UserApprovedApp (UserID, SystemID)
                VALUES (@UserID, @SystemID)";
                db.Parameters.Clear();
                db.Parameters.Add("@UserID", userID);
                db.Parameters.Add("@SystemID", sysID);

                db.ExecuteNonQuery();
            }

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
    /// 新增帳號(全部資料)
    /// </summary>
    /// <param name="user">使用者資料</param>
    /// <returns></returns>
    public static bool InsertSysUser(Sys_User user, List<Sys_UserOFSRole> OFS_Roles)
    {
        bool RtVal = true;
        DbHelper db = new DbHelper();

        db.BeginTrans();
        try
        {
            db.CommandText = @"
INSERT INTO Sys_User
    ([UnitType],[UnitID],[UnitName],[Account],[Pwd],[Name],[Tel],[OSI_RoleID],[IsReceiveMail],[IsApproved],[IsValid],[CreateTime],[UpdateTime],[ApprovedSource])
VALUES (@UnitType,@UnitID,@UnitName,@Account,@Pwd,@Name,@Tel,@OSI_RoleID,@IsReceiveMail,@IsApproved,@IsValid,@CreateTime,@UpdateTime,@ApprovedSource)

 SELECT SCOPE_IDENTITY();
";

            db.Parameters.Clear();
            db.Parameters.Add("@UnitType", user.UnitType);
            db.Parameters.Add("@UnitID", user.UnitID);
            db.Parameters.Add("@Account", user.Account);
            db.Parameters.Add("@Pwd", user.Pwd);
            db.Parameters.Add("@Name", user.Name);
            db.Parameters.Add("@Tel", user.Tel);
            db.Parameters.Add("@OSI_RoleID", user.OSI_RoleID);
            db.Parameters.Add("@IsReceiveMail", user.IsReceiveMail);
            db.Parameters.Add("@IsApproved", user.IsApproved);
            db.Parameters.Add("@IsValid", user.IsValid);
            db.Parameters.Add("@CreateTime", user.CreateTime);
            db.Parameters.Add("@UpdateTime", user.UpdateTime);
            db.Parameters.Add("@PwdToken", user.PwdToken);
            db.Parameters.Add("@UnitName", user.UnitName);
            db.Parameters.Add("@ApprovedSource", user.ApprovedSource);

            object result = db.GetDataSet().Tables[0].Rows[0][0];
            if (result == null || result == DBNull.Value)
                throw new Exception("取得 UserID 失敗：結果為 null 或 DBNull");
            var userID = Convert.ToInt32(result);

            // 更新 Sys_UserOFSRole
            foreach (var item in OFS_Roles)
            {
                db.CommandText = @"
INSERT INTO [dbo].[Sys_UserOFSRole]
    ([UserID],[RoleID])
VALUES (@UserID,@RoleID)";

                db.Parameters.Clear();
                db.Parameters.Add("@UserID", userID);
                db.Parameters.Add("@RoleID", item.RoleID);

                db.ExecuteNonQuery();
            }

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
    /// 刪除使用者(假刪除)
    /// </summary>
    /// <returns></returns>
    public static bool SetUserActive(string userID, bool isActive)
    {
        var db = new DbHelper();
        int rowsAffected = 0;
        db.BeginTrans();
        try
        {
            // 2. 更新該帳號的 PwdToken 欄位
            db.CommandText = @"
            UPDATE Sys_User
               SET IsActive     = @isActive,
                   Pwd          = NULL,
                   UpdateTime   = GETDATE()   
             WHERE IsValid = 1
            AND UserID = @userID;

             SELECT CAST(@@ROWCOUNT AS INT);
            ";
            db.Parameters.Clear();
            db.Parameters.Add("@isActive", isActive);
            db.Parameters.Add("@userID", userID);

            // 取得影響的行數
            object result = db.GetDataSet().Tables[0].Rows[0][0];
            rowsAffected = (result == null ? 0 : Convert.ToInt32(result));

            if (rowsAffected > 0)
            {
                db.Commit();
                return true;
            }
            else
            {
                db.Rollback();
                return false;
            }
        }
        catch
        {
            db.Rollback();
            return false;
        }

    }

    /// <summary>
    /// 查詢指定部門的承辦人員（用於移轉案件）
    /// </summary>
    /// <param name="unitID">部門ID</param>
    /// <returns></returns>
    public static GisTable QueryReviewersByUnitID(string unitID)
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
            SELECT [UserID]
                ,[UnitID]
                ,[Account]
                ,[Name]
                ,[Tel]
                ,[IsReceiveMail]
                ,[IsApproved]
                ,[IsValid]
                ,[CreateTime]
                ,[UpdateTime]
                ,[IsActive]
                ,[UnitName]
                ,[UnitType]
                ,[ApprovedSource]
            FROM [Sys_User] 
            WHERE UnitID = @UnitID
            AND IsValid = 1
            AND IsApproved = 1
            ORDER BY Name";
        db.Parameters.Clear();
        db.Parameters.Add("@UnitID", unitID);

        return db.GetTable();
    }

    /// <summary>
    /// 刪除使用者(假刪除)
    /// </summary>
    /// <returns></returns>
    public static bool DeleteUserByID(int userID)
    {
        var db = new DbHelper();
        int rowsAffected = 0;
        db.BeginTrans();
        try
        {
            // 2. 更新該帳號的 PwdToken 欄位
            db.CommandText = @"
            UPDATE Sys_User
               SET IsValid     = 0,
                   UpdateTime   = GETDATE()   
             WHERE IsValid = 1
            AND UserID = @userID;

             SELECT CAST(@@ROWCOUNT AS INT);
            ";
            db.Parameters.Clear();
            db.Parameters.Add("@userID", userID);

            // 取得影響的行數
            object result = db.GetDataSet().Tables[0].Rows[0][0];
            rowsAffected = (result == null ? 0 : Convert.ToInt32(result));

            if (rowsAffected > 0)
            {
                db.Commit();
                return true;
            }
            else
            {
                db.Rollback();
                return false;
            }
        }
        catch
        {
            db.Rollback();
            return false;
        }

    }

    /// <summary>
    /// 根據帳號取得 UserID
    /// </summary>
    /// <param name="account">使用者帳號</param>
    /// <returns>UserID，若查無資料則回傳 null</returns>
    public static int? GetUserIDByAccount(string account)
    {
        if (string.IsNullOrEmpty(account))
        {
            return null;
        }

        DbHelper db = new DbHelper();
        db.CommandText = "SELECT UserID FROM [Sys_User] WHERE [Account] = @Account";
        db.Parameters.Add("@Account", account);

        try
        {
            DataTable dt = db.GetTable();
            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["UserID"] != DBNull.Value)
            {
                return Convert.ToInt32(dt.Rows[0]["UserID"]);
            }
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetUserIDByAccount 發生錯誤: {ex.Message}");
            return null;
        }
        finally
        {
            db.Dispose();
        }
    }

    /// <summary>
    /// 根據帳號查詢同單位下具有指定 OFS 角色的使用者帳號列表
    /// </summary>
    /// <param name="account">要搜尋的帳號</param>
    /// <param name="roleIDs">要查詢的 OFS 角色 ID 列表（可傳入多個）</param>
    /// <returns>符合條件的使用者帳號列表</returns>
    /// RoleID	RoleName
    // 1	申請者
    // 2	審查委員
    // 4	查核人員
    // 5	主管單位人員
    // 6	主管單位窗口
    // 7	系統管理者
    public static List<string> GetSameUnitUsersByRoles(string account, List<int> roleIDs)
    {
        if (string.IsNullOrEmpty(account) || roleIDs == null || roleIDs.Count == 0)
        {
            return new List<string>();
        }

        DbHelper db = new DbHelper();

        // 建立 IN 子句的參數
        string roleIDParams = string.Join(",", roleIDs.Select((id, index) => $"@RoleID{index}"));

        db.CommandText = $@"
            -- 查詢與指定帳號同單位，且具有指定 OFS 角色的使用者帳號
            SELECT DISTINCT u.Account
            FROM Sys_User u
            INNER JOIN Sys_UserOFSRole uor ON u.UserID = uor.UserID
            WHERE u.UnitID = (
                -- 取得指定帳號的 UnitID
                SELECT UnitID
                FROM Sys_User
                WHERE Account = @Account
                  AND IsValid = 1
            )
            AND uor.RoleID IN ({roleIDParams})
            AND u.IsValid = 1
            AND u.IsApproved = 1
            ORDER BY u.Account
        ";

        db.Parameters.Clear();
        db.Parameters.Add("@Account", account);

        // 動態新增 RoleID 參數
        for (int i = 0; i < roleIDs.Count; i++)
        {
            db.Parameters.Add($"@RoleID{i}", roleIDs[i]);
        }

        List<string> accounts = new List<string>();

        try
        {
            DataTable dt = db.GetTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    accounts.Add(row["Account"].ToString());
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetSameUnitUsersByRoles 發生錯誤: {ex.Message}");
        }
        finally
        {
            db.Dispose();
        }

        return accounts;
    }

    /// <summary>
    /// 根據帳號查詢同單位下具有指定 OFS 角色的使用者帳號列表（單一角色版本）
    /// </summary>
    /// <param name="account">要搜尋的帳號</param>
    /// <param name="roleID">要查詢的 OFS 角色 ID</param>
    /// <returns>符合條件的使用者帳號列表</returns>
    public static List<string> GetSameUnitUsersByRole(string account, int roleID)
    {
        return GetSameUnitUsersByRoles(account, new List<int> { roleID });
    }
}