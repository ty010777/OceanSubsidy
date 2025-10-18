using System;
using System.Web;

public class CultureDayTask : IHttpHandler
{
    public bool IsReusable => false;

    public void ProcessRequest(HttpContext context)
    {
        var today = DateTime.Today;

        // 逾期未補
        var list1 = OFS_CulProjectHelper.query(new OFS_CulProject { Status = 14, CorrectionDeadline = today.AddDays(-1) });

        foreach (var data in list1)
        {
            OFS_CulProjectHelper.updateStatus(data.ProjectID, 15);
        }

        // 補正期限前2天提醒
        var list2 = OFS_CulProjectHelper.query(new OFS_CulProject { Status = 14, CorrectionDeadline = today.AddDays(2) });

        foreach (var data in list2)
        {
            NotificationHelper.B2("文化", data.ProjectName, data.Year.ToString(), data.RejectReason, BaseService.toTwDate(data.CorrectionDeadline.Value), data.UserAccount);
        }

        // 每月進度(每月20日)
        if (today.Day == 20)
        {
            var list3 = OFS_CulProjectHelper.query(new OFS_CulProject { ProgressStatus = 5 }, true, today);

            foreach (var data in list3)
            {
                var progress = OFS_CulMonthlyProgressHelper.get(data.ID, today.Year - 1911, today.Month);

                if (string.IsNullOrWhiteSpace(progress?.Description))
                {
                    OFS_TaskQueueHelper.UpdateTaskStatus(data.ProjectID, "MonthlyReport", 1, 0);

                    NotificationHelper.F11("文化", data.ProjectName, $"{today.Month}月進度回報", new DateTime(today.Year, today.Month + 1, 10), data.UserAccount);
                }
            }
        }

        // 每月進度逾期稽催(次月11日)
        if (today.Day == 11)
        {
            var last = today.AddMonths(-1);
            var list4 = OFS_CulProjectHelper.query(new OFS_CulProject { ProgressStatus = 5 }, true, last);

            foreach (var data in list4)
            {
                var progress = OFS_CulMonthlyProgressHelper.get(data.ID, last.Year - 1911, last.Month);

                if (string.IsNullOrWhiteSpace(progress?.Description))
                {
                    NotificationHelper.F2("文化", data.ProjectName, $"{last.Month}月進度回報", data.UserAccount, data.Organizer);
                }
            }
        }

        foreach (var grant in OFSGrantTypeHelper.query("CUL"))
        {
            // 期中報告
            if (grant.MidtermDeadline.HasValue)
            {
                // 報告繳交日一個月前提醒填報
                if (isSameDate(today, grant.MidtermDeadline.Value.AddDays(-30)))
                {
                    var list5 = OFS_CulProjectHelper.query(new OFS_CulProject { Year = grant.Year.Value, ProgressStatus = 5 });

                    foreach (var data in list5)
                    {
                        var task = OFS_TaskQueueHelper.Get(data.ProjectID, "MidReport");

                        if (task != null && !task.IsTodo.Value)
                        {
                            OFS_TaskQueueHelper.UpdateTaskStatus(data.ProjectID, "MidReport", 1, 0);

                            NotificationHelper.F11("文化", data.ProjectName, "期中報告", grant.MidtermDeadline.Value, data.UserAccount);
                        }
                    }
                }

                // 逾期稽催
                if (isSameDate(today, grant.MidtermDeadline.Value.AddDays(1)))
                {
                    var list6 = OFS_CulProjectHelper.query(new OFS_CulProject { Year = grant.Year.Value, ProgressStatus = 5 });

                    foreach (var data in list6)
                    {
                        var task = OFS_TaskQueueHelper.Get(data.ProjectID, "MidReport");

                        if (task != null && !task.IsCompleted.Value)
                        {
                            NotificationHelper.F2("文化", data.ProjectName, "期中報告", data.UserAccount, data.Organizer);
                        }
                    }
                }
            }

            // 期末報告
            if (grant.FinalOneMonth != null && grant.FinalOneMonth.Value)
            {
                // 計畫結束日
                var list7 = OFS_CulProjectHelper.query(new OFS_CulProject { Year = grant.Year.Value, EndTime = today, ProgressStatus = 5 });

                foreach (var data in list7)
                {
                    var task = OFS_TaskQueueHelper.Get(data.ProjectID, "FinalReport");

                    if (task != null && !task.IsTodo.Value)
                    {
                        OFS_TaskQueueHelper.UpdateTaskStatus(data.ProjectID, "FinalReport", 1, 0);

                        NotificationHelper.F11("文化", data.ProjectName, "期末報告", today.AddDays(30), data.UserAccount);
                    }
                }

                // 逾期稽催
                var list8 = OFS_CulProjectHelper.query(new OFS_CulProject { Year = grant.Year.Value, EndTime = today.AddDays(-31), ProgressStatus = 5 });

                foreach (var data in list8)
                {
                    var task = OFS_TaskQueueHelper.Get(data.ProjectID, "FinalReport");

                    if (task != null && !task.IsCompleted.Value)
                    {
                        NotificationHelper.F2("文化", data.ProjectName, "期末報告", data.UserAccount, data.Organizer);
                    }
                }
            }
            else if (grant.FinalDeadline.HasValue)
            {
                // 報告繳交日一個月前提醒填報
                if (isSameDate(today, grant.FinalDeadline.Value.AddDays(-30)))
                {
                    var list9 = OFS_CulProjectHelper.query(new OFS_CulProject { Year = grant.Year.Value, ProgressStatus = 5 });

                    foreach (var data in list9)
                    {
                        var task = OFS_TaskQueueHelper.Get(data.ProjectID, "FinalReport");

                        if (task != null && !task.IsTodo.Value)
                        {
                            OFS_TaskQueueHelper.UpdateTaskStatus(data.ProjectID, "FinalReport", 1, 0);

                            NotificationHelper.F11("文化", data.ProjectName, "期末報告", grant.FinalDeadline.Value, data.UserAccount);
                        }
                    }
                }

                // 逾期稽催
                if (isSameDate(today, grant.FinalDeadline.Value.AddDays(1)))
                {
                    var list10 = OFS_CulProjectHelper.query(new OFS_CulProject { Year = grant.Year.Value, ProgressStatus = 5 });

                    foreach (var data in list10)
                    {
                        var task = OFS_TaskQueueHelper.Get(data.ProjectID, "FinalReport");

                        if (task != null && !task.IsCompleted.Value)
                        {
                            NotificationHelper.F2("文化", data.ProjectName, "期末報告", data.UserAccount, data.Organizer);
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
