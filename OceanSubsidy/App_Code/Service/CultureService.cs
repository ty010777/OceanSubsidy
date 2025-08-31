using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class CultureService : BaseService
{
    public object createGoalStep(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        getProject(id, new int[] {1,3,10}); //申請中,退回補正,修正計畫書

        var step = new OFS_CulGoalStep {
            PID = id,
            ItemID = int.Parse(param["ItemID"].ToString()),
            Begin = 0,
            End = 0
        };

        OFS_CulGoalStepHelper.insert(step);

        return step;
    }

    public object getApplication(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        return new
        {
            Project = OFS_CulProjectHelper.get(id),
            Contacts = OFS_CulContactHelper.query(id),
            ReceivedSubsidies = OFS_CulReceivedSubsidyHelper.query(id)
        };
    }

    public object getAttachment(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        return new
        {
            Project = OFS_CulProjectHelper.get(id),
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
        var id = int.Parse(param["ID"].ToString());
        var project = OFS_CulProjectHelper.get(id);

        return new
        {
            Project = project,
            OtherSubsidies = OFS_CulOtherSubsidyHelper.query(id),
            BudgetPlans = OFS_CulBudgetPlanHelper.query(id),
            Items = OFS_CulGoalItemHelper.query(id),
            GrantTargetSetting = OFSGrantTargetSettingHelper.getByTargetTypeID($"CUL{project.OrgCategory}")
        };
    }

    public object getRelatedProject(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        return new
        {
            Project = OFS_CulProjectHelper.get(id),
            Projects = OFS_CulRelatedProjectHelper.query(id)
        };
    }

    public object getWorkSchedule(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

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
            Project = OFS_CulProjectHelper.get(id),
            Goals = goals,
            GrantType = OFSGrantTypeHelper.getByCode("CUL")
        };
    }

    public object reviewApplication(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        var project = getProject(id, new int[] {2,11}); //資格審查,決審

        project.RejectReason = null;
        project.CorrectionDeadline = null;

        if (project.Status == 2)
        {
            switch (int.Parse(param["Result"].ToString()))
            {
                case 2:
                    project.Status = 4; //資格審查不通過
                    project.RejectReason = param["Reason"].ToString(); //原因
                    break;
                case 3:
                    project.Status = 3; //退回補正
                    project.RejectReason = param["Reason"].ToString(); //原因
                    project.CorrectionDeadline = DateTime.Parse(param["CorrectionDeadline"].ToString()); //補正期限
                    break;
                default:
                    project.Status = 5; //初審
                    break;
            }
        }
        else
        {
            switch (int.Parse(param["Result"].ToString()))
            {
                case 2:
                    project.Status = 12; //決審不通過
                    project.RejectReason = param["Reason"].ToString(); //原因
                    break;
                case 3:
                    project.Status = 10; //修正計畫書
                    project.RejectReason = param["Reason"].ToString(); //原因
                    break;
                default:
                    project.Status = 13; //核定通過
                    project.ProgressStatus = 1; //執行中
                    break;
            }
        }

        OFS_CulProjectHelper.reviewApplication(project);

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
            var data = getProject(project.ID, new int[] {1,3}); //申請中,退回補正

            project.ProjectID = data.ProjectID;

            OFS_CulProjectHelper.update(project);
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

        return new { ID = project.ID };
    }

    public object saveAttachment(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());
        var data = getProject(id, new int[] {1,3,10}); //申請中,退回補正,修正計畫書

        if (bool.Parse(param["Submit"].ToString()))
        {
            if (data.Status == 10)
            {
                OFS_CulProjectHelper.updateStatus(data.ProjectID, 11);
            }
            else
            {
                OFS_CulProjectHelper.updateFormStep(data.ProjectID, 6);
                OFS_CulProjectHelper.updateStatus(data.ProjectID, 2);
            }
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
        var data = getProject(project.ID, new int[] {1,3,10}); //申請中,退回補正,修正計畫書

        OFS_CulProjectHelper.updateFunding(project);

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

    public object saveOrganizer(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());
        var data = getProject(id, new int[] {2}); //資格審查

        OFS_CulProjectHelper.updateOrganizer(data.ID, int.Parse(param["Organizer"].ToString()));

        return new {};
    }

    public object saveRelatedProject(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());
        var data = getProject(id, new int[] {1,3}); //申請中,退回補正

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

    public object saveWorkSchedule(JObject param, HttpContext context)
    {
        var project = param["Project"].ToObject<OFS_CulProject>();
        var data = getProject(project.ID, new int[] {1,3,10}); //申請中,退回補正,修正計畫書

        OFS_CulProjectHelper.updateSchedule(project);

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

    private void deleteGoalItem(int id)
    {
        OFS_CulGoalStepHelper.deleteByItemID(id);
        OFS_CulGoalScheduleHelper.deleteByItemID(id);
        OFS_CulGoalItemHelper.delete(id);
    }

    private OFS_CulProject getProject(int id, int[] statusList = null)
    {
        var project = OFS_CulProjectHelper.get(id);

        if (project == null || !project.IsExists)
        {
            throw new Exception("查無資料");
        }

        if ((statusList != null && !statusList.Contains(project.Status)) || project.IsWithdrawal)
        {
            throw new Exception("狀態錯誤");
        }

        return project;
    }
}
