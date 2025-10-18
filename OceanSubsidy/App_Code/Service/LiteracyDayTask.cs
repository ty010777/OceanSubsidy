using System;
using System.Web;

public class LiteracyDayTask : IHttpHandler
{
    public bool IsReusable => false;

    public void ProcessRequest(HttpContext context)
    {
        var today = DateTime.Today;

        // 逾期未補
        var list1 = OFS_LitProjectHelper.query(new OFS_LitProject { Status = 14, CorrectionDeadline = today.AddDays(-1) });

        foreach (var data in list1)
        {
            OFS_LitProjectHelper.updateStatus(data.ProjectID, 15);
        }

        // 補正期限前2天提醒
        var list2 = OFS_LitProjectHelper.query(new OFS_LitProject { Status = 14, CorrectionDeadline = today.AddDays(2) });

        foreach (var data in list2)
        {
            NotificationHelper.B2("素養", data.ProjectName, data.Year.ToString(), data.RejectReason, BaseService.toTwDate(data.CorrectionDeadline.Value), data.UserAccount);
        }

        foreach (var grant in OFSGrantTypeHelper.query("LIT"))
        {
            // 成果報告
            if (grant.FinalOneMonth != null && grant.FinalOneMonth.Value)
            {
                // 計畫結束日
                var list3 = OFS_LitProjectHelper.query(new OFS_LitProject { Year = grant.Year.Value, EndTime = today, ProgressStatus = 5 });

                foreach (var data in list3)
                {
                    var task = OFS_TaskQueueHelper.Get(data.ProjectID, "Report");

                    if (task != null && !task.IsTodo.Value)
                    {
                        OFS_TaskQueueHelper.UpdateTaskStatus(data.ProjectID, "Report", 1, 0);

                        NotificationHelper.F11("素養", data.ProjectName, "成果報告", today.AddDays(30), data.UserAccount);
                    }
                }
            }
            else if (grant.FinalDeadline.HasValue)
            {
                // 報告繳交日一個月前提醒填報
                if (isSameDate(today, grant.FinalDeadline.Value.AddDays(-30)))
                {
                    var list4 = OFS_LitProjectHelper.query(new OFS_LitProject { Year = grant.Year.Value, ProgressStatus = 5 });

                    foreach (var data in list4)
                    {
                        var task = OFS_TaskQueueHelper.Get(data.ProjectID, "Report");

                        if (task != null && !task.IsTodo.Value)
                        {
                            OFS_TaskQueueHelper.UpdateTaskStatus(data.ProjectID, "Report", 1, 0);

                            NotificationHelper.F11("素養", data.ProjectName, "成果報告", grant.FinalDeadline.Value, data.UserAccount);
                        }
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
