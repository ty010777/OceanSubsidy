using GS.Data.Sql;
using System.Configuration;
using System.Data;
using System.Linq;

public class NotificationHelper
{
    public static void G1(string category, string projectName, string eventName, int? organizer)
    {
        toOrganizer(
            "［海洋委員會］執行計畫案 資料已提送",
            $"您好：<br><br>{category}類執行計畫「{projectName}」，<br>{eventName}資料已提送，請進入計畫確認資料。",
            organizer
        );
    }

    public static void G2(string category, string projectName, string eventName, int? organizer)
    {
        toOrganizer(
            "［海洋委員會］執行計畫案 請審核計畫執行資料",
            $"主管單位您好：<br><br>{category}類執行計畫「{projectName}」，<br>{eventName}資料已提送，請進行審核確認。",
            organizer
        );
    }

    public static void G3(string category, string projectName, string eventName, string reason, string account)
    {
        toUser(
            "［海洋委員會］執行計畫案 資料退回修正通知",
            $"您好：<br><br>貴單位{category}類執行計畫「{projectName}」，<br>{eventName}資料退回，請進行資料修正。<br><br>退回原因：{reason}",
            account
        );
    }

    public static void G4(string category, string projectName, string eventName, string account)
    {
        toUser(
            "［海洋委員會］執行計畫案 計畫變更已通過",
            $"您好：<br><br>貴單位{category}類執行計畫「{projectName}」，<br>{eventName}已審核通過。",
            account
        );
    }

    public static void G5(string category, string projectName, string eventName, string account)
    {
        toUser(
            "［海洋委員會］執行計畫案 階段報告已通過",
            $"您好：<br><br>貴單位{category}類執行計畫「{projectName}」，<br>{eventName}已審核通過。",
            account
        );
    }

    public static void G6(string category, string projectName, string eventName, decimal? amount, string remark, string account)
    {
        toUser(
            "［海洋委員會］執行計畫案 請款核銷已通過",
            $"您好：<br><br>貴單位{category}類執行計畫「{projectName}」，<br>{eventName}已通過。<br><br>撥付金額：{amount}<br>審核備註：{remark}",
            account
        );
    }

    private static string getFooter()
    {
        var url = ConfigurationManager.AppSettings["Host"] + ConfigurationManager.AppSettings["AppRootPath"];

        return $"<br><br><br><a href=\"{url}\">海洋委員會補助計畫管理系統</a>";
    }

    private static string queryOrganizers(int? organizer)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [Account]
              FROM [Sys_User]
             WHERE UserID = @UserID
            UNION
            SELECT [Account]
              FROM [Sys_User]
             WHERE [UnitID] IN (SELECT [UnitID] FROM [Sys_User] WHERE UserID = @UserID)
               AND [OSI_RoleID] = 6
        ";

        db.Parameters.Add("@UserID", organizer);

        return string.Join(",", db.GetTable().Rows.Cast<DataRow>().Select(row => row.Field<string>("Account")).ToList());
    }

    private static string querySysAdmin()
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
            SELECT [Account]
              FROM [Sys_User]
             WHERE [OSI_RoleID] = 7
        ";

        return string.Join(",", db.GetTable().Rows.Cast<DataRow>().Select(row => row.Field<string>("Account")).ToList());
    }

    private static void toOrganizer(string subject, string content, int? organizer)
    {
        EmailApiHelper.Send(queryOrganizers(organizer), subject, content + getFooter(), querySysAdmin());
    }

    private static void toUser(string subject, string content, string account)
    {
        EmailApiHelper.Send(account, subject, content + getFooter(), querySysAdmin());
    }
}
