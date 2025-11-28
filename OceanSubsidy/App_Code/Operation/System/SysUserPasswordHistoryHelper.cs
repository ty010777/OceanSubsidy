using System;
using System.Collections.Generic;
using System.Linq;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;
using static GS.App.Utility.Cryptography;

/// <summary>
/// 密碼歷史記錄管理類別
/// </summary>
public class SysUserPasswordHistoryHelper
{
    public SysUserPasswordHistoryHelper()
    {
    }

    /// <summary>
    /// 記錄密碼歷史
    /// </summary>
    /// <param name="userID">使用者ID</param>
    /// <param name="encryptedPwd">加密後的密碼</param>
    /// <param name="salt">Salt值（從 Sys_User 表取得）</param>
    public static void InsertPasswordHistory(int userID, string encryptedPwd, string salt)
    {
        try
        {
            DbHelper db = new DbHelper();
            db.CommandText = @"
                INSERT INTO [dbo].[Sys_UserPasswordHistory]
                    (UserID, Pwd, Salt, ChangeTime)
                VALUES
                    (@UserID, @Pwd, @Salt, @ChangeTime)";

            db.Parameters.Clear();
            db.Parameters.Add("@UserID", userID);
            db.Parameters.Add("@Pwd", encryptedPwd);
            db.Parameters.Add("@Salt", salt);
            db.Parameters.Add("@ChangeTime", DateTime.Now);

            db.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception("記錄密碼歷史失敗: " + ex.Message, ex);
        }
    }

    /// <summary>
    /// 檢查新密碼是否與前3次重複使用
    /// </summary>
    /// <param name="userID">使用者ID</param>
    /// <param name="newPassword">新密碼（明文）</param>
    /// <param name="salt">Salt值（從 Sys_User 表取得）</param>
    /// <returns>true: 密碼重複, false: 密碼未重複</returns>
    public static bool IsPasswordReused(int userID, string newPassword, string salt)
    {
        try
        {
            DbHelper db = new DbHelper();
            db.CommandText = @"
                SELECT TOP 3 Pwd, Salt
                FROM Sys_UserPasswordHistory
                WHERE UserID = @UserID
                ORDER BY ChangeTime DESC";

            db.Parameters.Clear();
            db.Parameters.Add("@UserID", userID);

            var histories = db.GetList<Sys_UserPasswordHistory>();

            // 因為 AES-GCM 每次加密結果都不同（Nonce 隨機）
            // 需要解密歷史密碼，比對明文
            foreach (var history in histories)
            {
                try
                {
                    // 使用儲存的 Salt 解密歷史密碼
                    // 注意：雖然我們選擇「Salt 固定不變」策略，但歷史表仍保存 Salt 以確保一致性
                    string useSalt = string.IsNullOrEmpty(history.Salt) ? salt : history.Salt;
                    string oldPassword = AESGCM.DecryptText(history.Pwd, useSalt);

                    if (oldPassword == newPassword)
                    {
                        return true; // 密碼與歷史記錄重複
                    }
                }
                catch
                {
                    // 解密失敗，可能是舊資料或 Salt 不匹配，跳過此筆記錄
                    continue;
                }
            }

            return false; // 密碼沒有重複
        }
        catch (Exception ex)
        {
            throw new Exception("檢查密碼歷史失敗: " + ex.Message, ex);
        }
    }

    /// <summary>
    /// 查詢使用者的密碼歷史記錄
    /// </summary>
    /// <param name="userID">使用者ID</param>
    /// <param name="topN">取得最近幾筆記錄（預設10筆）</param>
    /// <returns>密碼歷史記錄清單</returns>
    public static List<Sys_UserPasswordHistory> GetPasswordHistory(int userID, int topN = 10)
    {
        try
        {
            DbHelper db = new DbHelper();
            db.CommandText = $@"
                SELECT TOP {topN}
                    HistoryID, UserID, Pwd, Salt, ChangeTime
                FROM Sys_UserPasswordHistory
                WHERE UserID = @UserID
                ORDER BY ChangeTime DESC";

            db.Parameters.Clear();
            db.Parameters.Add("@UserID", userID);

            return db.GetList<Sys_UserPasswordHistory>();
        }
        catch (Exception ex)
        {
            throw new Exception("查詢密碼歷史失敗: " + ex.Message, ex);
        }
    }

    /// <summary>
    /// 刪除超過指定天數的密碼歷史記錄（資料維護用）
    /// </summary>
    /// <param name="days">保留天數（預設保留365天）</param>
    public static void DeleteOldPasswordHistory(int days = 365)
    {
        try
        {
            DbHelper db = new DbHelper();
            db.CommandText = @"
                DELETE FROM Sys_UserPasswordHistory
                WHERE ChangeTime < @CutoffDate";

            db.Parameters.Clear();
            db.Parameters.Add("@CutoffDate", DateTime.Now.AddDays(-days));

            db.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception("刪除舊密碼歷史失敗: " + ex.Message, ex);
        }
    }
}
