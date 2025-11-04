using System;
using System.Web;

public class AccessibilityDayTask : IHttpHandler
{
    public bool IsReusable => false;

    public void ProcessRequest(HttpContext context)
    {
        var today = DateTime.Today;

        // 逾期未補
        var list1 = OFS_AccProjectHelper.query(new OFS_AccProject { Status = 14, CorrectionDeadline = today.AddDays(-1) });

        foreach (var data in list1)
        {
            OFS_AccProjectHelper.updateStatus(data.ProjectID, 15);
        }

        // 補正期限前2天提醒
        var list2 = OFS_AccProjectHelper.query(new OFS_AccProject { Status = 14, CorrectionDeadline = today.AddDays(2) });

        foreach (var data in list2)
        {
            NotificationHelper.B2("無障礙", data.ProjectName, data.Year.ToString(), data.RejectReason, BaseService.toTwDate(data.CorrectionDeadline.Value), data.UserAccount);
        }

        foreach (var grant in OFSGrantTypeHelper.query("ACC"))
        {
            // 期中報告
            if (grant.MidtermDeadline.HasValue)
            {
                // 報告繳交日一個月前提醒填報
                if (isSameDate(today, grant.MidtermDeadline.Value.AddDays(-30)))
                {
                    var list3 = OFS_AccProjectHelper.query(new OFS_AccProject { Year = grant.Year.Value, ProgressStatus = 5 });

                    foreach (var data in list3)
                    {
                        var task = OFS_TaskQueueHelper.Get(data.ProjectID, "MidReport");

                        if (task != null && !task.IsTodo.Value)
                        {
                            OFS_TaskQueueHelper.UpdateTaskStatus(data.ProjectID, "MidReport", 1, 0);

                            NotificationHelper.F11("無障礙", data.ProjectName, "期中報告", grant.MidtermDeadline.Value, data.UserAccount);
                        }
                    }
                }
            }

            // 成果報告
            if (grant.FinalOneMonth != null && grant.FinalOneMonth.Value)
            {
                // 計畫結束日
                var list4 = OFS_AccProjectHelper.query(new OFS_AccProject { Year = grant.Year.Value, EndTime = today, ProgressStatus = 5 });

                foreach (var data in list4)
                {
                    var task = OFS_TaskQueueHelper.Get(data.ProjectID, "FinalReport");

                    if (task != null && !task.IsTodo.Value)
                    {
                        OFS_TaskQueueHelper.UpdateTaskStatus(data.ProjectID, "FinalReport", 1, 0);

                        NotificationHelper.F11("無障礙", data.ProjectName, "成果報告", today.AddDays(30), data.UserAccount);
                    }
                }
            }
            else if (grant.FinalDeadline.HasValue)
            {
                // 報告繳交日一個月前提醒填報
                if (isSameDate(today, grant.FinalDeadline.Value.AddDays(-30)))
                {
                    var list5 = OFS_AccProjectHelper.query(new OFS_AccProject { Year = grant.Year.Value, ProgressStatus = 5 });

                    foreach (var data in list5)
                    {
                        var task = OFS_TaskQueueHelper.Get(data.ProjectID, "FinalReport");

                        if (task != null && !task.IsTodo.Value)
                        {
                            OFS_TaskQueueHelper.UpdateTaskStatus(data.ProjectID, "FinalReport", 1, 0);

                            NotificationHelper.F11("無障礙", data.ProjectName, "成果報告", grant.FinalDeadline.Value, data.UserAccount);
                        }
                    }
                }
            }

            if (grant.ApplyEndDate.HasValue)
            {
                // 逾期前1天提醒
                if (isSameDate(today, grant.ApplyEndDate.Value))
                {
                    var list6 = OFS_AccProjectHelper.query(new OFS_AccProject { Status = 1 });

                    foreach (var data in list6)
                    {
                        NotificationHelper.A0("無障礙", data.ProjectName, grant.ApplyEndDate.Value, data.UserAccount, "ACC");
                    }
                }

                // 逾期
                if (isSameDate(today, grant.ApplyEndDate.Value.AddDays(1)))
                {
                    var list7 = OFS_AccProjectHelper.query(new OFS_AccProject { Status = 1 });

                    foreach (var data in list7)
                    {
                        OFS_AccProjectHelper.updateStatus(data.ProjectID, 2);
                    }
                }
            }
        }
    }

    private bool isSameDate(DateTime date1, DateTime date2)
    {
        return date1.Year == date2.Year && date1.Month == date2.Month && date1.Day == date2.Day;
    }
}
