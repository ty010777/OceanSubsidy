using GS.Data.Sql;
using System;
using System.Data;

/// <summary>
/// 計畫變更紀錄 Helper - 僅負責資料庫操作
/// </summary>
public class OFS_PlanChangeRecordsHelper
{
    /// <summary>
    /// 取得計畫變更紀錄清單
    /// </summary>
    /// <param name="projectType">計畫類型 (SCI, CUL, EDC, CLB, ACC, MUL, LIT)</param>
    /// <param name="dataID">計畫編號</param>
    /// <returns>變更紀錄資料表</returns>
    public static DataTable GetChangeRecords(string dataID)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [ID]
                  ,[Type]
                  ,[Method]
                  ,[DataID]
                  ,[Reason]
                  ,[Form1Before]
                  ,[Form1After]
                  ,[Form2Before]
                  ,[Form2After]
                  ,[Form3Before]
                  ,[Form3After]
                  ,[Form4Before]
                  ,[Form4After]
                  ,[Form5Before]
                  ,[Form5After]
                  ,[Status]
                  ,[RejectReason]
                  ,[CreateTime]
                  ,[CreateUser]
                  ,[UpdateTime]
                  ,[UpdateUser]
              FROM [OFS_ProjectChangeRecord]
             WHERE [DataID] = @DataID
               AND [Status] = 3
          ORDER BY [Method] DESC, [CreateTime] ASC
        ";

        
        db.Parameters.Add("@DataID", dataID);

        return db.GetTable();
    }

    /// <summary>
    /// 取得使用者名稱
    /// </summary>
    /// <param name="userID">使用者 ID</param>
    /// <returns>使用者姓名</returns>
    public static string GetUserName(int userID)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [Name]
            FROM [Sys_User]
            WHERE [UserID] = @UserID
        ";

        db.Parameters.Add("@UserID", userID);

        DataTable table = db.GetTable();

        return table.Rows.Count > 0 ? table.Rows[0].Field<string>("Name") : null;
    }
}
