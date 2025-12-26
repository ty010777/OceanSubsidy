using GS.Data.Sql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

public class NotificationHelper
{
    public static void A0(string category, string projectName, DateTime deadline, string account, string type)
    {
        toAll(
            "［海洋委員會］補助申請案 尚未提送，請於期限前完成！",
            $"您好：<br><br>" + $"您申請的{category}類計畫「{projectName}」，目前仍尚未完成提送！<br>" +
                $"為避免影響後續審查及您的申請權益，請務必於申請期限前完成提送。<br><br>" +
                $"申請截止日期：{deadline.Year - 1911}/{deadline.Month:D2}/{deadline.Day:D2}<br><br>" +
                $"如您確定無須申請，請登入系統後刪除此案，<br>" +
                $"感謝您的配合",
            account,
            type
        );
    }

    public static void A1(string category, string projectName, string type)
    {
        toSupervisor(
            "［海洋委員會］補助申請案 計畫提送通知",
            $"主管單位您好：<br><br>{category}計畫「{projectName}」已提送申請，敬請進行資格審查。",
            type
        );
    }

    public static void A2(string category, string projectName, int? organizer)
    {
        toOrganizer(
            "［海洋委員會］補助申請案 修正計畫書提送通知",
            $"主管單位您好：<br><br>{category}計畫「{projectName}」已提送修正計畫書，敬請至［計畫審查＞決審核定］進行修正計畫書確認。",
            organizer
        );
    }

    public static void A3(string category, string projectName, string kind, DateTime time, string account)
    {
        toUser(
            "［海洋委員會］補助申請案 計畫提送通知",
            $"您好：<br><br>{category}類計畫「{projectName}」已提送申請。<br><br>提送類型：{kind}<br>提送時間：{time.Year - 1911}/{time.Month:D2}/{time.Day:D2} {time.Hour:D2}:{time.Minute:D2}",
            account
        );
    }

    public static void B1(string category, string projectName, string year, string reason, string deadline, string account)
    {
        toUser(
            "［海洋委員會］補助申請案 資料須補正補件",
            $"您好：<br><br>有關貴單位所提本會{year}年{category}類計畫－「{projectName}」一案，<br>" +
            $"經資格審查後尚須補正或補件。<br>" +
            $"說明：{reason}<br>" +
            $"請於期限 {deadline}  內完成修正及提送。<br>",
            account
        );
    }

    public static void B2(string category, string projectName, string year, string reason, string deadline, string account)
    {
        toUser(
            "［海洋委員會］補助申請案 資料請盡速補正補件",
            $"您好：<br><br>有關貴單位所提本會{year}年{category}類計畫－「{projectName}」一案，<br>" +
            $"經資格審查後尚須補正或補件。<br>" +
            $"說明：{reason}<br>" +
            $"請於期限 {deadline}  內完成修正及提送。<br>",
            account
        );
    }

    public static void C1(string category, Dictionary<string, string> ProjectList, string eventName, string account)
    {
        string content =$"您好：<br><br>海洋委員會以下補助申請案，請協助{eventName}之評分及審查意見：<br>";
        int i = 1;
        foreach (var kvp in ProjectList)
        {
            string projectID = kvp.Key;
            string url = kvp.Value;
            content += $"{i}. <a href=\"{url}\">{projectID}</a><br>";
            i++;
        }
        toSomeOne(
            "［海洋委員會］補助申請案 請進行審查及評分",
            content
            ,
            account
        );
    }

    public static void C2(string category, string projectName, string account)
    {
        toUser(
            "［海洋委員會］補助申請案 審查意見通知",
            $"您好：<br><br>{category}類計畫「{projectName}」，<br>審查委員已提供審查意見，<br>請至［計畫申請］查看審查意見及回覆。",
            account
        );
    }

    public static void E1(string category, string projectName, string account)
    {
        toUser(
            "［海洋委員會］補助申請案 請修正計畫書",
            $"您好：<br><br>{category}類計畫「{projectName}」，已開放修正計畫書，<br>請至［計畫申請］修正申請資料及提出修正計畫書，感謝您！",
            account
        );
    }

    public static void E2(string category, int year, string projectName, string account)
    {
        toUser(
            "［海洋委員會］補助申請案 審查不通過通知",
            $"您好：<br><br> 有關貴單位所提本會{year}年{category}類計畫「{projectName}」一案，審查結果未通過，請查照。",
            account
        );
    }

    public static void E31(string category, int year, string projectName, string eventName, string account)
    {
        toUser(
            "［海洋委員會］補助申請案 審查通過通知",
            $"您好：<br><br> 有關貴單位所提本會{year}年{category}類計畫「{projectName}」一案，{eventName}結果<br>通過，相關配合事項本會另函通知，請查照。",
            account
        );
    }

    public static void E32(string category, string projectName, string eventName, string eventName2, string account)
    {
        toUser(
            "［海洋委員會］補助申請案 審查通過通知",
            $"您好：<br><br>{category}類計畫「{projectName}」，<br>" +
            $"{eventName}階段已通過，相關配合事項本會另函通知，請查照。<br>" +
            $"敬請提供{eventName2}所需簡報資料，請至［計畫申請］上傳簡報。",
            account
        );
    }

    public static void E5(int year, string projectName, string approveAmount, string account)
    {
        toUser(
            "［海洋委員會］補助申請案 計畫核定通知",
            $"您好：<br><br> 核定貴單位「{projectName}」計畫，{year}年度補助經費為新臺幣{approveAmount}元，<br>相關配合事項本會另函通知，請查照。" +
            $"貴單位申請經費補助已獲本會同意，請以紙本公文，後附相關附件正本<br>" +
            $"(其中申請書表及未違反公職人員利益衝突迴避法切結書及事前揭露表，需加蓋關防及負責人簽章，並需為正本)，<br>" +
            $"以郵寄方式寄送本會憑辦。",
            account
        );
    }

    public static void F2(string category, string projectName, string eventName, string account, int? organizer)
    {
        toAll(
            "［海洋委員會］執行計畫案 進度落後提醒",
            $"您好：<br><br>{category}類執行計畫「{projectName}」，<br>「{eventName}」已逾期未完成，請盡速至［計畫執行］內填報資料並完成提送。",
            account,
            organizer
        );
    }

    public static void F11(string category, string projectName, string eventName, DateTime deadline, string account)
    {
        toUser(
            "［海洋委員會］執行計畫案 資料填報提醒",
            $"您好：<br><br>{category}類執行計畫「{projectName}」，<br>提醒您於 {deadline.Year - 1911}/{deadline.Month:D2}/{deadline.Day:D2} 前至［計畫執行］填報「{eventName}」資料並提送，感謝配合。<br><br>（若已提送資料，請忽略此通知）",
            account
        );
    }

    public static void F12(string category, string projectName, string eventName, string account)
    {
        toUser(
            "［海洋委員會］執行計畫案 資料填報提醒",
            $"您好：<br><br>{category}類執行計畫「{projectName}」，<br>提醒您至［計畫執行］填報「{eventName}」資料並提送，感謝配合。<br><br>（若已提送資料，請忽略此通知）",
            account
        );
    }

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
            $"您好：<br><br>貴單位{category}類執行計畫「{projectName}」，<br>{eventName}已通過。<br><br>撥付金額：{amount}<br>審核備註：{remark}<br><br><br>" +
            $"貴單位申請經費補助核銷文件經本會審視，原則無意見。<br>" +
            $"請以紙本公文，後附相關核銷文件、成果報告等相關證明文件正本，以郵寄方式寄送本會憑辦。",
            account
        );
    }

    public static void H2(string category, string projectName, int? organizer)
    {
        toOrganizer(
            "［海洋委員會］查核意見已回覆",
            $"主管單位您好：<br><br>{category}類執行計畫「{projectName}」，<br>執行單位已回覆查核意見，可至［計畫執行>查核紀錄］查閱相關資料。",
            organizer
        );
    }

    public static void H11(string category, string projectName, int? organizer)
    {
        toOrganizer(
            "［海洋委員會］查核紀錄已登錄",
            $"主管單位您好：<br><br>{category}類執行計畫「{projectName}」，<br>已登錄查核紀錄，可至［計畫執行＞查核紀錄］查閱相關資料。",
            organizer
        );
    }

    public static void H12(string category, string projectName, string account)
    {
        toUser(
            "［海洋委員會］查核紀錄已登錄",
            $"您好：<br><br>{category}類執行計畫「{projectName}」，<br>已登錄查核意見，請至［計畫執行＞查核紀錄］查閱並進行改善及回覆。",
            account
        );
    }

    public static void J1( string projectName, string eventName, string url, string account)
    {
        toSomeOne(
            "［海洋委員會］執行計畫案 請委員進行審查",
            $"審查委員您好：<br><br> 海洋委員會以下執行計畫案，請協助「{eventName}」 之審查意見：<br> <a href=\"{url}\">{projectName}</a>",
            account
        );
    }

    public static void J2(string projectID, string projectName, string ReviewerName, string eventName, int? organizer)
    {
        toOrganizer(
            "［海洋委員會］執行計畫案 委員已提送審查意見",
            $"主管單位您好：<br><br> 以下執行計畫案，審查委員已提送審查意見。<br>" +
            $"計畫編號：{projectID}<br>" +
            $"計畫名稱：{projectName}<br>" +
            $"審查委員：{ReviewerName}<br>" +
            $"審查報告：{eventName}<br>",
            organizer
        );
    }

    public static void Z1(string category, string type, string projectName,  string reson, string account, int? organizer)
    {
        if (organizer == null || organizer == 0)
        {
            toSupervisor(
                "［海洋委員會］補助申請案 已撤案通知",
                $"您好：<br><br> {category}類計畫「{projectName}」已進行撤案申請。<br>撤案原因：{reson}",
                type
            );
        }
        else
        {
            toOrganizer(
                "［海洋委員會］補助申請案 已撤案通知",
                $"您好：<br><br> {category}類計畫「{projectName}」已進行撤案申請。<br>撤案原因：{reson}",
                organizer
            );
        }

        toSomeOne(
            "［海洋委員會］補助申請案 已撤案通知",
            $"您好：<br><br> {category}類計畫「{projectName}」已進行撤案申請。<br>撤案原因：{reson}",
            account
        );
    }

    public static void Z2(string category, string type, string projectName, string reson, string account, int? organizer)
    {
        if (organizer == null || organizer == 0)
        {
            toSupervisor(
                "［海洋委員會］補助申請案 撤案已恢復通知",
                $"您好：<br><br>原已撤案之 {category}類計畫「{projectName}」，目前已恢復案件至原審查階段。<br>恢復案件原因：{reson}",
                type
            );
        }
        else
        {
            toOrganizer(
                "［海洋委員會］補助申請案 撤案已恢復通知",
                $"您好：<br><br>原已撤案之 {category}類計畫「{projectName}」，目前已恢復案件至原審查階段。<br>恢復案件原因：{reson}",
                organizer
            );
        }

        toSomeOne(
            "［海洋委員會］補助申請案 撤案已恢復通知",
            $"您好：<br><br>原已撤案之 {category}類計畫「{projectName}」，目前已恢復案件至原審查階段。<br>恢復案件原因：{reson}",
            account
        );
    }
    public static void Z3(string category, string type, string projectName,  string reson, string account)
    {
        
        toSupervisor(
            "［海洋委員會］補助申請案 已刪除案件通知",
            $"您好：<br><br> {category}類計畫「{projectName}」申請案件已刪除。<br>刪除原因：{reson}",
            type
        );
        toSomeOne(
            "［海洋委員會］補助申請案 已刪除案件通知",
            $"您好：<br><br> {category}類計畫「{projectName}」申請案件已刪除。<br>刪除原因：{reson}",
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
        if (organizer == null || organizer == 0)
        {
            return string.Empty;
        }

        DbHelper db = new DbHelper();

        db.CommandText = @"
            -- 1. 取得指定 UserID 的帳號
            SELECT [Account]
              FROM [Sys_User]
             WHERE UserID = @UserID
               AND IsValid = 1
            UNION
            -- 2. 取得同單位下具有「主管單位窗口」(RoleID = 6) 角色的使用者帳號
            SELECT DISTINCT u.Account
              FROM [Sys_User] u
             INNER JOIN Sys_UserOFSRole uor ON u.UserID = uor.UserID
             WHERE u.UnitID IN (SELECT [UnitID] FROM [Sys_User] WHERE UserID = @UserID)
               AND uor.RoleID = 6
               AND u.IsValid = 1
               AND u.IsApproved = 1
        ";

        db.Parameters.Add("@UserID", organizer);

        return string.Join(",", db.GetTable().Rows.Cast<DataRow>().Select(row => row.Field<string>("Account")).ToList());
    }

    private static string querySupervisor(string Type)
    {
        DbHelper db = new DbHelper();

        db.CommandText = @"
        -- 取得目前有效 XX 補助類型對應單位下的特定角色使用者
       SELECT distinct Account
        FROM Sys_User AS u
        LEFT JOIN Sys_UserOFSRole AS ur
               ON u.UserID = ur.UserID
        WHERE
            -- 篩選角色 ID 5 或 6
            (ur.RoleID = '5' OR ur.RoleID = '6')
            AND
            -- 篩選使用者所屬單位
            u.UnitID = (
                SELECT s.UnitID
                FROM [OCA_OceanSubsidy].[dbo].[Sys_Unit] AS s
                WHERE s.UnitName = (
                    SELECT TOP(1) g.AdminUnit
                    FROM [OCA_OceanSubsidy].[dbo].[OFS_GrantType] AS g
                    WHERE g.TypeCode = @Type
                      AND GETDATE() >= g.ApplyStartDate
                      AND GETDATE() <= g.ApplyEndDate
                )
            );
;

        ";

        db.Parameters.Add("@Type", Type);

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

    private static void toAll(string subject, string content, string account, int? organizer)
    {
        EmailApiHelper.Send(account + "," + queryOrganizers(organizer), subject, content + getFooter(), querySysAdmin());
    }

    private static void toAll(string subject, string content, string account, string type)
    {
        EmailApiHelper.Send(account + "," + querySupervisor(type), subject, content + getFooter(), querySysAdmin());
    }

    private static void toOrganizer(string subject, string content, int? organizer)
    {
        EmailApiHelper.Send(queryOrganizers(organizer), subject, content + getFooter(), querySysAdmin());
    }

    private static void toUser(string subject, string content, string account)
    {
        EmailApiHelper.Send(account, subject, content + getFooter(), querySysAdmin());
    }

    private static void toSomeOne(string subject, string content, string account)
    {
        EmailApiHelper.Send(account, subject, content + getFooter());
    }

    private static void toSupervisor(string subject, string content, string type)
    {
        EmailApiHelper.Send(querySupervisor(type), subject, content + getFooter());
    }

    public static void ReviewCommittee(string token, string account)
    {
        var url = ConfigurationManager.AppSettings["Host"] + ConfigurationManager.AppSettings["AppRootPath"] + $"/OFS/ReviewCommitteeInfo.aspx?Token={token}";

        toUser(
            "［海洋委員會］補助申請案 請進行審查及評分",
            $"您好：<br><br>海洋委員會已邀請您參與計畫申請階段之審查。<br>為能讓後續作業順利進行，<br>請<a href=\"{url}\">進入此連結</a>提供您的銀行帳戶與戶籍地址資料，非常感謝！",
            account
        );
    }
}
