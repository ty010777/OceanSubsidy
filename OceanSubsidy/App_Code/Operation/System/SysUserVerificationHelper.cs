using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GS.Data;
using GS.Data.Sql;

/// <summary>
/// SysUserVerificationHelper 的摘要描述
/// </summary>
public class SysUserVerificationHelper
{
    public SysUserVerificationHelper()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }

    /// <summary>
    /// 查詢BY Account
    /// </summary>
    /// <returns></returns>
    public static GisTable QueryByAccount(string account)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"SELECT TOP (1000) [ID]
    ,[Account]
    ,[VerificationCode]
    ,[UpdateTime]
FROM [OCA_OceanSubsidy].[dbo].[Sys_UserVerification]
WHERE Account = @Account";
        db.Parameters.Clear();
        db.Parameters.Add("@Account", account);

        return db.GetTable();
    }

    /// <summary>
    /// 新增或更新驗證碼
    /// </summary>
    /// <returns></returns>
    public static bool InsertOrUpdateCode(string account, string code)
    {
        var db = new DbHelper();
        db.BeginTrans();
        try
        {
            // 2. 更新該帳號的 PwdToken 欄位
            db.CommandText = @"
IF EXISTS (SELECT 1 
           FROM dbo.Sys_UserVerification 
           WHERE Account = @Account)
BEGIN
  UPDATE dbo.Sys_UserVerification
  SET 
    VerificationCode = @Code,
    UpdateTime       = GETDATE()
  WHERE Account = @Account;
END
ELSE
BEGIN
  INSERT INTO dbo.Sys_UserVerification
    (Account, VerificationCode, UpdateTime)
  VALUES
    (@Account, @Code, GETDATE());
END
            ";
            db.Parameters.Clear();
            db.Parameters.Add("@Account", account);
            db.Parameters.Add("@Code", code);
            db.GetTable();

            db.Commit();
            return true;
        }
        catch
        {
            db.Rollback();
            return false;
        }

    }

    /// <summary>
    /// 刪除BY Account
    /// </summary>
    /// <returns></returns>
    public static void DeleteByAccount(string account)
    {
        DbHelper db = new DbHelper();
        db.CommandText =
            @"UPDATE dbo.Sys_UserVerification
  SET
    VerificationCode = null,
    UpdateTime       = GETDATE()
  WHERE Account = @Account;";
        db.Parameters.Clear();
        db.Parameters.Add("@Account", account);
        db.GetTable();

        return;
    }
}