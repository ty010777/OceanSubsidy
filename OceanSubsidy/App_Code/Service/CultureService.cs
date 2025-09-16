using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Model.OFS;
using GS.OCA_OceanSubsidy.Operation.OFS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class CultureService : BaseService
{
    public object applyChange(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());

        getProject(id, new int[] {51}); //執行階段-審核中

        OFS_CulProjectHelper.setProjectChanged(id, true);

        OFSProjectChangeRecordHelper.insert(new ProjectChangeRecord
        {
            Type = "CUL",
            DataID = id,
            Reason = param["Reason"].ToString()
        });

        return new {};
    }

    public object createGoal(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());

        getProject(id, new int[] {1,14,42}, true); //編輯中,補正補件,計畫書修正中 | 變更申請

        var goal = new OFS_CulGoal
        {
            PID = id
        };

        OFS_CulGoalHelper.insert(goal);

        return goal;
    }

    public object createGoalItem(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());

        getProject(id, new int[] {1,14,42}, true); //編輯中,補正補件,計畫書修正中 | 變更申請

        var item = new OFS_CulGoalItem
        {
            PID = id,
            GoalID = int.Parse(param["GoalID"].ToString())
        };

        OFS_CulGoalItemHelper.insert(item);

        return item;
    }

    public object createGoalSchedule(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());

        getProject(id, new int[] {1,14,42}, true); //編輯中,補正補件,計畫書修正中 | 變更申請

        var schedule = new OFS_CulGoalSchedule
        {
            PID = id,
            ItemID = int.Parse(param["ItemID"].ToString()),
            Type = int.Parse(param["Type"].ToString()),
            Month = 0,
            StepID = 0
        };

        OFS_CulGoalScheduleHelper.insert(schedule);

        return schedule;
    }

    public object createGoalStep(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());

        getProject(id, new int[] {1,14,42}, true); //編輯中,補正補件,計畫書修正中 | 變更申請

        var step = new OFS_CulGoalStep
        {
            PID = id,
            ItemID = int.Parse(param["ItemID"].ToString()),
            Begin = 0,
            End = 0
        };

        OFS_CulGoalStepHelper.insert(step);

        return step;
    }

    public object findApplication(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());

        return new
        {
            Project = getProject(id)
        };
    }

    public object getApplication(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());

        return new
        {
            Project = getProject(id),
            Contacts = OFS_CulContactHelper.query(id),
            ReceivedSubsidies = OFS_CulReceivedSubsidyHelper.query(id)
        };
    }

    public object getAttachment(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());

        return new
        {
            Project = getProject(id),
            Attachments = OFS_CulAttachmentHelper.query(id)
        };
    }

    public object getEmptyApplication(JObject param, HttpContext context)
    {
        var type = OFSGrantTypeHelper.getByCode("CUL");

        return new { Year = type.StartDate.Value.Year - 1911, SubsidyPlanType = $"{type.ShortName} ({type.FullName})" };
    }

    public object getFunding(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var project = getProject(id);

        return new
        {
            Project = project,
            OtherSubsidies = OFS_CulOtherSubsidyHelper.query(id),
            BudgetPlans = OFS_CulBudgetPlanHelper.query(id),
            Items = OFS_CulGoalItemHelper.query(id),
            GrantTargetSetting = OFSGrantTargetSettingHelper.getByTargetTypeID($"CUL{project.OrgCategory}")
        };
    }

    public object getMonthlyProgress(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var year = getID(param["Year"].ToString());
        var month = getID(param["Month"].ToString());

        var progress = OFS_CulMonthlyProgressHelper.get(id, year, month);

        if (progress == null) {
            progress = new OFS_CulMonthlyProgress
            {
                PID = id,
                Year = year,
                Month = month
            };

            OFS_CulMonthlyProgressHelper.insert(progress);
        }

        return new
        {
            Progress = progress,
            Logs = OFS_CulMonthlyProgressLogHelper.query(progress.ID),
            Items = OFS_CulGoalItemHelper.query(id),
            Steps = OFS_CulGoalStepHelper.query(id),
            Schedules = OFS_CulGoalScheduleHelper.query(id)
        };
    }

    public object getPayment(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id);

        return new
        {
            Project = data,
            Payments = OFSPaymentHelper.query(data.ProjectID),
            Attachments = OFS_CulAttachmentHelper.query(id, 2)
        };
    }

    public object getRelatedProject(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());

        return new
        {
            Project = getProject(id),
            Projects = OFS_CulRelatedProjectHelper.query(id)
        };
    }

    public object getReport(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id);
        var stage = int.Parse(param["Stage"].ToString());

        return new {
            Project = data,
            Report = OFS_SciInterimReportHelper.GetStageExamStatus(data.ProjectID, stage),
            Attachments = OFS_CulAttachmentHelper.query(id, 1)
        };
    }

    public object getWorkSchedule(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());

        var goals = OFS_CulGoalHelper.query(id);
        var items = OFS_CulGoalItemHelper.query(id);
        var steps = OFS_CulGoalStepHelper.query(id);
        var schedules = OFS_CulGoalScheduleHelper.query(id);

        foreach (var item in items)
        {
            item.Steps = steps.Where(d => d.ItemID == item.ID).ToList();
            item.Schedules = schedules.Where(d => d.ItemID == item.ID).ToList();
        }

        foreach (var goal in goals)
        {
            goal.Items = items.Where(d => d.GoalID == goal.ID).ToList();
        }

        return new
        {
            Project = getProject(id),
            Goals = goals,
            GrantType = OFSGrantTypeHelper.getByCode("CUL")
        };
    }

    public object reviewApplication(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());

        var project = getProject(id, new int[] {11,43}); //資格審查-審查中, 決審-計畫書審核中

        project.RejectReason = null;
        project.CorrectionDeadline = null;

        if (project.Status == 11)
        {
            switch (int.Parse(param["Result"].ToString()))
            {
                case 2:
                    project.Status = 13; //不通過
                    project.RejectReason = param["Reason"].ToString();
                    break;
                case 3:
                    project.Status = 14; //補正補件
                    project.RejectReason = param["Reason"].ToString();
                    project.CorrectionDeadline = DateTime.Parse(param["CorrectionDeadline"].ToString()); //補正期限
                    break;
                default:
                    project.Status = 12; //通過
                    break;
            }
        }
        else if (project.Status == 43)
        {
            switch (int.Parse(param["Result"].ToString()))
            {
                case 2:
                    project.Status = 46; //不通過
                    project.RejectReason = param["Reason"].ToString();
                    break;
                case 3:
                    project.Status = 42; //計畫書修正中
                    project.RejectReason = param["Reason"].ToString();
                    break;
                default:
                    project.Status = 44; //計畫書已確認
                    break;
            }
        }

        OFS_CulProjectHelper.reviewApplication(project);

        return new {};
    }

    public object reviewApplicationChange(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id);
        var apply = data.changeApply;

        if (apply != null && apply.Status == 2) //待審核
        {
            apply.RejectReason = null;

            if (int.Parse(param["Result"].ToString()) == 2)
            {
                apply.Status = 4; //退回修改
                apply.RejectReason = param["Reason"].ToString();
            }
            else
            {
                apply.Status = 3; //審核通過

                OFS_CulProjectHelper.setProjectChanged(id, false);
            }

            OFSProjectChangeRecordHelper.update(apply);
        }

        return new {};
    }

    public object reviewPayment(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id, new int[] {51}); //執行階段-審核中
        var stage = int.Parse(param["Stage"].ToString());

        var payment = OFSPaymentHelper.query(data.ProjectID).FirstOrDefault(d => d.Stage == stage && d.Status == "審核中");

        if (payment == null)
        {
            throw new Exception("查無資料");
        }

        if (int.Parse(param["Result"].ToString()) == 1)
        {
            payment.Status = "通過";
            payment.CurrentActualPaidAmount = int.Parse(param["Amount"].ToString());

            if (payment.Stage == 2)
            {
                OFS_CulProjectHelper.updateProgressStatus(data.ProjectID, 9); //結案
            }
        }
        else
        {
            payment.Status = "請款中";
        }

        payment.ReviewerComment = param["Reason"].ToString();
        payment.ReviewUser = CurrentUser.Account;
        payment.ReviewTime = DateTime.Now;

        OFSPaymentHelper.review(payment);

        return new {};
    }

    public object reviewReport(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id, new int[] {51}); //執行階段-審核中
        var stage = int.Parse(param["Stage"].ToString());
        var report = OFS_SciInterimReportHelper.GetStageExamStatus(data.ProjectID, stage);

        if (report.Status != "審核中")
        {
            throw new Exception("審核狀態錯誤");
        }

        var comment = "";
        var status = "通過";

        if (int.Parse(param["Result"].ToString()) == 2)
        {
            comment = param["Reason"].ToString();
            status = "退回修正";
        }

        OFS_SciInterimReportHelper.ReviewStageExam(data.ProjectID, stage, stage == 1 ? "期中報告" : "期末報告", status, comment, CurrentUser.UserName, CurrentUser.Account);

        return new {};
    }

    public object saveApplication(JObject param, HttpContext context)
    {
        var project = param["Project"].ToObject<OFS_CulProject>();

        if (project.ID == 0)
        {
            var info = SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo);
            var seq = OFS_CulProjectHelper.count(project.Year) + 1;

            project.ProjectID = $"CUL{project.Year}{seq:D4}";
            project.UserAccount = info.Account;
            project.UserName = info.UserName;
            project.UserOrg = info.UnitName;

            OFS_CulProjectHelper.insert(project);
        }
        else
        {
            var data = getProject(project.ID, new int[] {1,14}, true); //編輯中,補正補件 | 變更申請

            project.ProjectID = data.ProjectID;

            OFS_CulProjectHelper.update(project);

            if (data.IsProjChanged)
            {
                var apply = data.changeApply;

                if (apply != null)
                {
                    apply.Form1Before = param["Before"].ToString();
                    apply.Form1After = param["After"].ToString();

                    OFSProjectChangeRecordHelper.update(apply);
                }
            }
        }

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_CulProjectHelper.updateFormStep(project.ProjectID, 2);
        }

        var contacts = param["Contacts"].ToObject<List<OFS_CulContact>>();

        foreach (var item in contacts)
        {
            if (item.ID == 0)
            {
                item.PID = project.ID;

                OFS_CulContactHelper.insert(item);
            }
            else
            {
                OFS_CulContactHelper.update(item);
            }
        }

        var receiveds = param["ReceivedSubsidies"].ToObject<List<OFS_CulReceivedSubsidy>>();

        foreach (var item in receiveds)
        {
            if (item.Deleted)
            {
                OFS_CulReceivedSubsidyHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = project.ID;

                OFS_CulReceivedSubsidyHelper.insert(item);
            }
            else
            {
                OFS_CulReceivedSubsidyHelper.update(item);
            }
        }

        return new { ID = project.ProjectID };
    }

    public object saveAttachment(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id, new int[] {1,14,42}, true); //編輯中,補正補件,計畫書修正中 | 變更申請

        if (data.IsProjChanged)
        {
            var apply = data.changeApply;

            if (apply != null)
            {
                apply.Form5Before = param["Before"].ToString();
                apply.Form5After = param["After"].ToString();

                if (bool.Parse(param["Submit"].ToString()))
                {
                    apply.Status = 2; //待審核
                }

                OFSProjectChangeRecordHelper.update(apply);
            }
        }

        if (bool.Parse(param["Submit"].ToString()))
        {
            if (data.Status == 1)
            {
                OFS_CulProjectHelper.updateFormStep(data.ProjectID, 6);
                OFS_CulProjectHelper.updateProgressStatus(data.ProjectID, 1); //資格審查

                ApplicationChecklistHelper.InsertCaseHistoryLog(new OFS_CaseHistoryLog
                {
                    ProjectID = data.ProjectID,
                    ChangeTime = DateTime.Now,
                    UserName = CurrentUser.UserName,
                    StageStatusBefore = "編輯中",
                    StageStatusAfter = "資格審查-審核中",
                    Description = "完成附件上傳並提送申請"
                });
            }
            else if (data.Status == 14)
            {
                OFS_CulProjectHelper.updateStatus(data.ProjectID, 11); //審查中
            }
            else if (data.Status == 42)
            {
                OFS_CulProjectHelper.updateStatus(data.ProjectID, 43); //計畫書審核中
            }

            snapshot(id);
        }

        var attachments = param["Attachments"].ToObject<List<OFS_CulAttachment>>();

        foreach (var item in attachments)
        {
            if (item.Deleted)
            {
                OFS_CulAttachmentHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = id;

                OFS_CulAttachmentHelper.insert(item);
            }
        }

        return new {};
    }

    public object saveFunding(JObject param, HttpContext context)
    {
        var project = param["Project"].ToObject<OFS_CulProject>();
        var data = getProject(project.ID, new int[] {1,14,42}, true); //編輯中,補正補件,計畫書修正中 | 變更申請

        OFS_CulProjectHelper.updateFunding(project);

        if (data.IsProjChanged)
        {
            var apply = data.changeApply;

            if (apply != null)
            {
                apply.Form3Before = param["Before"].ToString();
                apply.Form3After = param["After"].ToString();

                OFSProjectChangeRecordHelper.update(apply);
            }
        }

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_CulProjectHelper.updateFormStep(data.ProjectID, 4);
        }

        var others = param["OtherSubsidies"].ToObject<List<OFS_CulOtherSubsidy>>();

        foreach (var item in others)
        {
            if (item.Deleted)
            {
                OFS_CulOtherSubsidyHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = project.ID;

                OFS_CulOtherSubsidyHelper.insert(item);
            }
            else
            {
                OFS_CulOtherSubsidyHelper.update(item);
            }
        }

        var plans = param["BudgetPlans"].ToObject<List<OFS_CulBudgetPlan>>();

        foreach (var item in plans)
        {
            if (item.Deleted)
            {
                OFS_CulBudgetPlanHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = project.ID;

                OFS_CulBudgetPlanHelper.insert(item);
            }
            else
            {
                OFS_CulBudgetPlanHelper.update(item);
            }
        }

        return new {};
    }

    public object saveMonthlyProgress(JObject param, HttpContext context)
    {
        var progress = OFS_CulMonthlyProgressHelper.get(int.Parse(param["ID"].ToString()));

        if (progress != null)
        {
            progress.Description = param["Description"].ToString();
            progress.Status = int.Parse(param["Status"].ToString());

            OFS_CulMonthlyProgressHelper.update(progress);

            //--

            foreach (var log in OFS_CulMonthlyProgressLogHelper.query(progress.ID))
            {
                OFS_CulMonthlyProgressLogHelper.delete(log.ID);
            }

            //--

            var logs = param["Logs"].ToObject<List<OFS_CulMonthlyProgressLog>>();

            foreach (var log in logs)
            {
                log.ID = 0;
                log.PID = progress.PID;
                log.MPID = progress.ID;

                OFS_CulMonthlyProgressLogHelper.insert(log);
                OFS_CulGoalScheduleHelper.updateStatus(log.ScheduleID, log.Status);
            }
        }

        return new {};
    }

    public object saveOrganizer(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());

        getProject(id);

        OFS_CulProjectHelper.updateOrganizer(id, int.Parse(param["Organizer"].ToString()));

        return new {};
    }

    public object savePayment(JObject param, HttpContext context)
    {
        var payment = param["Payment"].ToObject<OFS_SCI_Payment>();
        var id = getID(payment.ProjectID);
        var data = getProject(id, new int[] {51}); //執行階段-審核中

        //--

        var list = OFSPaymentHelper.query(data.ProjectID);
        var prev = list.Where(d => d.Stage < payment.Stage).Sum(d => d.CurrentRequestAmount);

        if (prev >= payment.TotalSpentAmount)
        {
            throw new Exception("金額錯誤");
        }

        var model = list.FirstOrDefault(d => d.Stage == payment.Stage);

        if (model == null)
        {
            model = new OFS_SCI_Payment();
            model.ProjectID = payment.ProjectID;
            model.Stage = payment.Stage;
        }

        model.ActDisbursementRatioPct = payment.ActDisbursementRatioPct;
        model.TotalSpentAmount = payment.TotalSpentAmount;
        model.CurrentRequestAmount = payment.TotalSpentAmount - prev;
        model.Status = bool.Parse(param["Submit"].ToString()) ? "審核中" : "請款中";

        OFSPaymentHelper.submit(model);

        //--

        var attachments = param["Attachments"].ToObject<List<OFS_CulAttachment>>();

        foreach (var item in attachments)
        {
            if (item.Deleted)
            {
                OFS_CulAttachmentHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = id;
                item.Stage = 2; //請款

                OFS_CulAttachmentHelper.insert(item);
            }
        }

        return new {};
    }

    public object saveRelatedProject(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id, new int[] {1,14}, true); //編輯中,補正補件 | 變更申請

        if (data.IsProjChanged)
        {
            var apply = data.changeApply;

            if (apply != null)
            {
                apply.Form4Before = param["Before"].ToString();
                apply.Form4After = param["After"].ToString();

                OFSProjectChangeRecordHelper.update(apply);
            }
        }

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_CulProjectHelper.updateFormStep(data.ProjectID, 5);
        }

        var projects = param["Projects"].ToObject<List<OFS_CulRelatedProject>>();

        foreach (var item in projects)
        {
            if (item.Deleted)
            {
                OFS_CulRelatedProjectHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = id;

                OFS_CulRelatedProjectHelper.insert(item);
            }
            else
            {
                OFS_CulRelatedProjectHelper.update(item);
            }
        }

        return new {};
    }

    public object saveReport(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id, new int[] {51}); //執行階段-審核中

        OFS_SciInterimReportHelper.SubmitStageExam(data.ProjectID, int.Parse(param["Stage"].ToString()), bool.Parse(param["Submit"].ToString()) ? "審核中" : "暫存");

        var attachments = param["Attachments"].ToObject<List<OFS_CulAttachment>>();

        foreach (var item in attachments)
        {
            if (item.Deleted)
            {
                OFS_CulAttachmentHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = id;
                item.Stage = 1; //報告

                OFS_CulAttachmentHelper.insert(item);
            }
        }

        return new {};
    }

    public object saveWorkSchedule(JObject param, HttpContext context)
    {
        var project = param["Project"].ToObject<OFS_CulProject>();
        var data = getProject(project.ID, new int[] {1,14,42}, true); //編輯中,補正補件,計畫書修正中 | 變更申請

        OFS_CulProjectHelper.updateSchedule(project);

        if (data.IsProjChanged)
        {
            var apply = data.changeApply;

            if (apply != null)
            {
                apply.Form2Before = param["Before"].ToString();
                apply.Form2After = param["After"].ToString();

                OFSProjectChangeRecordHelper.update(apply);
            }
        }

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_CulProjectHelper.updateFormStep(data.ProjectID, 3);
        }

        var goals = param["Goals"].ToObject<List<OFS_CulGoal>>();

        foreach (var goal in goals)
        {
            if (goal.Deleted)
            {
                var items = OFS_CulGoalItemHelper.query(project.ID);

                foreach (var item in items.Where(d => d.GoalID == goal.ID).ToList())
                {
                    deleteGoalItem(item.ID);
                }

                OFS_CulGoalHelper.delete(goal.ID);
            }
            else
            {
                if (goal.ID == 0)
                {
                    goal.PID = project.ID;

                    OFS_CulGoalHelper.insert(goal);
                }
                else
                {
                    OFS_CulGoalHelper.update(goal);
                }

                foreach (var item in goal.Items)
                {
                    if (item.Deleted)
                    {
                        deleteGoalItem(item.ID);
                    }
                    else
                    {
                        if (item.ID == 0)
                        {
                            item.PID = project.ID;
                            item.GoalID = goal.ID;

                            OFS_CulGoalItemHelper.insert(item);
                        }
                        else
                        {
                            OFS_CulGoalItemHelper.update(item);
                        }

                        foreach (var step in item.Steps)
                        {
                            if (step.Deleted)
                            {
                                OFS_CulGoalStepHelper.delete(step.ID);
                            }
                            else
                            {
                                OFS_CulGoalStepHelper.update(step);
                            }
                        }

                        foreach (var schedule in item.Schedules)
                        {
                            if (schedule.Deleted)
                            {
                                OFS_CulGoalScheduleHelper.delete(schedule.ID);
                            }
                            else if (schedule.ID == 0)
                            {
                                schedule.PID = project.ID;
                                schedule.ItemID = item.ID;

                                OFS_CulGoalScheduleHelper.insert(schedule);
                            }
                            else
                            {
                                OFS_CulGoalScheduleHelper.update(schedule);
                            }
                        }
                    }
                }
            }
        }

        return new {};
    }

    public object terminate(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());

        getProject(id);

        OFS_CulProjectHelper.terminate(id, param["RejectReason"].ToString(), int.Parse(param["RecoveryAmount"].ToString()));

        return new {};
    }

    private void deleteGoalItem(int id)
    {
        OFS_CulGoalStepHelper.deleteByItemID(id);
        OFS_CulGoalScheduleHelper.deleteByItemID(id);
        OFS_CulGoalItemHelper.delete(id);
    }

    private int getID(string value)
    {
        if (int.TryParse(value, out int id))
        {
            return id;
        }
        else
        {
            return OFS_CulProjectHelper.getID(value);
        }
    }

    private OFS_CulProject getProject(int id, int[] statusList = null, bool changed = false)
    {
        var project = OFS_CulProjectHelper.get(id);

        if (project == null || !project.IsExists)
        {
            throw new Exception("查無資料");
        }

        if (project.IsWithdrawal) //已撤銷
        {
            throw new Exception("狀態錯誤");
        }

        if ((changed && project.IsProjChanged) || statusList == null || statusList.Contains(project.Status))
        {
            if (project.IsProjChanged)
            {
                project.changeApply = OFSProjectChangeRecordHelper.getApplying("CUL", project.ID);
            }

            return project;
        }

        throw new Exception("狀態錯誤");
    }

    private void snapshot(int id)
    {
        var project = getProject(id);

        OFSSnapshotHelper.insert(new Snapshot
        {
            Type = "CUL",
            DataID = id,
            Status = project.Status,
            Data = JsonConvert.SerializeObject(new
            {
                Project = project,
                Contacts = OFS_CulContactHelper.query(id),
                ReceivedSubsidies = OFS_CulReceivedSubsidyHelper.query(id),
                Goals = OFS_CulGoalHelper.query(id),
                Items = OFS_CulGoalItemHelper.query(id),
                Steps = OFS_CulGoalStepHelper.query(id),
                Schedules = OFS_CulGoalScheduleHelper.query(id),
                OtherSubsidies = OFS_CulOtherSubsidyHelper.query(id),
                BudgetPlans = OFS_CulBudgetPlanHelper.query(id),
                Projects = OFS_CulRelatedProjectHelper.query(id),
                Attachments = OFS_CulAttachmentHelper.query(id)
            })
        });
    }
}
