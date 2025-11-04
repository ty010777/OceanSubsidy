using System;
using System.Web;

public class EducationDayTask : IHttpHandler
{
    public bool IsReusable => false;

    public void ProcessRequest(HttpContext context)
    {
        var today = DateTime.Today;

        // 逾期未補
        var list1 = OFS_EdcProjectHelper.query(new OFS_EdcProject { Status = 14, CorrectionDeadline = today.AddDays(-1) });

        foreach (var data in list1)
        {
            OFS_EdcProjectHelper.updateStatus(data.ProjectID, 15);
        }

        // 補正期限前2天提醒
        var list2 = OFS_EdcProjectHelper.query(new OFS_EdcProject { Status = 14, CorrectionDeadline = today.AddDays(2) });

        foreach (var data in list2)
        {
            NotificationHelper.B2("學校民間", data.ProjectName, data.Year.ToString(), data.RejectReason, BaseService.toTwDate(data.CorrectionDeadline.Value), data.UserAccount);
        }

        // 計畫結束日
        var list3 = OFS_EdcProjectHelper.query(new OFS_EdcProject { EndTime = today, ProgressStatus = 5 });

        foreach (var data in list3)
        {
            var task = OFS_TaskQueueHelper.Get(data.ProjectID, "Report");

            if (task != null && !task.IsTodo.Value)
            {
                OFS_TaskQueueHelper.UpdateTaskStatus(data.ProjectID, "Report", 1, 0);

                NotificationHelper.F11("學校民間", data.ProjectName, "成果報告", today.AddDays(30), data.UserAccount);
            }
        }

        foreach (var grant in OFSGrantTypeHelper.query("EDC"))
        {
            if (grant.ApplyEndDate.HasValue)
            {
                // 逾期前1天提醒
                if (isSameDate(today, grant.ApplyEndDate.Value))
                {
                    var list4 = OFS_EdcProjectHelper.query(new OFS_EdcProject { Status = 1 });

                    foreach (var data in list4)
                    {
                        NotificationHelper.A0("學校民間", data.ProjectName, grant.ApplyEndDate.Value, data.UserAccount, "EDC");
                    }
                }

                // 逾期
                if (isSameDate(today, grant.ApplyEndDate.Value.AddDays(1)))
                {
                    var list5 = OFS_EdcProjectHelper.query(new OFS_EdcProject { Status = 1 });

                    foreach (var data in list5)
                    {
                        OFS_EdcProjectHelper.updateStatus(data.ProjectID, 2);
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
