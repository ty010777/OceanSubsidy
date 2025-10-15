using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GS.App;
using GS.Data;
using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Entity;

/// <summary>
/// OSI 提醒信件相關功能
/// </summary>
public class OSIReminderHelper
{
    public OSIReminderHelper()
    {
       
    }

    /// <summary>
    /// 發送提醒信件
    /// </summary>
    /// <param name="periodId">期間ID</param>
    public static void SendReminderEmail(string periodId)
    {
        GisTable periodTbl = OSIDataPeriodsHelper.QueryByID(periodId);
        GisTable unitTbl = SysUnitHelper.GetOSIReminderUnitByPeriodID(periodId);

        if (periodTbl == null || periodTbl.Rows.Count == 0)
            return;

        var year = periodTbl.Rows[0]["PeriodYear"].ToString();
        var quarter = periodTbl.Rows[0]["PeriodQuarter"].ToString();

        for (int i = 0; i < unitTbl.Rows.Count; i++)
        {
            var unitID = unitTbl.Rows[i]["UnitID"].ToString();
            var unitName = unitTbl.Rows[i]["UnitName"].ToString();
            List<string> users = SysUserHelper.GetOSIReminderUserByUnitID(unitID);

            users.ForEach(account =>
            {
                string mailBody = MailContent.OCA.OSI_UnFilledRemind.getMail(unitName, year, quarter);
                GS.App.Utility.Mail.SendMail(account, "", MailContent.OCA.OSI_UnFilledRemind.Subject, mailBody, out string ErrorMsg);
            });
        }
    }

    /// <summary>
    /// 發送昨天結束期間的提醒信件
    /// </summary>
    /// <returns>發送的期間數量</returns>
    public static int SendReminderEmailForYesterdayPeriods()
    {
        var endedPeriods = OSIDataPeriodsHelper.QueryEndedYesterday();
        
        if (endedPeriods == null || endedPeriods.Count == 0)
            return 0;

        foreach (var period in endedPeriods)
        {
            SendReminderEmail(period.PeriodID.ToString());
        }

        return endedPeriods.Count;
    }

    /// <summary>
    /// 發送開始填報提醒信件
    /// </summary>
    /// <param name="periodId">期間ID</param>
    public static void SendStartFilledReminderEmail(string periodId)
    {
        GisTable periodTbl = OSIDataPeriodsHelper.QueryByID(periodId);
        GisTable unitTbl = GetAllUnitsExceptOthers();

        if (periodTbl == null || periodTbl.Rows.Count == 0)
            return;

        var year = periodTbl.Rows[0]["PeriodYear"].ToString();
        var quarter = periodTbl.Rows[0]["PeriodQuarter"].ToString();
        var startDate = Convert.ToDateTime(periodTbl.Rows[0]["StartDate"]).ToMinguoDate();
        var endDate = Convert.ToDateTime(periodTbl.Rows[0]["EndDate"]).ToMinguoDate();

        for (int i = 0; i < unitTbl.Rows.Count; i++)
        {
            var unitID = unitTbl.Rows[i]["UnitID"].ToString();
            var unitName = unitTbl.Rows[i]["UnitName"].ToString();
            List<string> users = SysUserHelper.GetOSIReminderUserByUnitID(unitID);

            users.ForEach(account =>
            {
                string mailBody = MailContent.OCA.OSI_StartFilledRemind.getMail(unitName, year, quarter, startDate, endDate);
                GS.App.Utility.Mail.SendMail(account, "", MailContent.OCA.OSI_StartFilledRemind.Subject, mailBody, out string ErrorMsg);
            });
        }
    }

    /// <summary>
    /// 發送今天開始期間的開始填報提醒信件
    /// </summary>
    /// <returns>發送的期間數量</returns>
    public static int SendStartFilledReminderEmailForTodayPeriods()
    {
        var startingPeriods = OSIDataPeriodsHelper.QueryStartingToday();
        
        if (startingPeriods == null || startingPeriods.Count == 0)
            return 0;

        foreach (var period in startingPeriods)
        {
            SendStartFilledReminderEmail(period.PeriodID.ToString());
        }

        return startingPeriods.Count;
    }

    /// <summary>
    /// 取得所有單位（除了'其他'）
    /// </summary>
    /// <returns>單位列表</returns>
    private static GisTable GetAllUnitsExceptOthers()
    {
        DbHelper db = new DbHelper();
        db.CommandText = @"
SELECT UnitID, UnitName 
FROM Sys_Unit 
WHERE IsValid = 1
AND UnitName <> N'其他'";
        db.Parameters.Clear();
        return db.GetTable();
    }
}