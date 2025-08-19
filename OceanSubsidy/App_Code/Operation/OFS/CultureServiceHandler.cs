using GS.Data.Sql;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class CultureServiceHandler
{
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
        return new { Year = 114, SubsidyPlanType = "114年度海洋文化領航計畫" };
    }

    public object getFunding(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        return new
        {
            Project = OFS_CulProjectHelper.get(id),
            OtherSubsidies = OFS_CulOtherSubsidyHelper.query(id),
            BudgetPlans = OFS_CulBudgetPlanHelper.query(id),
            Items = OFS_CulGoalItemHelper.query(id)
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
            Goals = goals
        };
    }

    public object getZgsCodes(JObject param, HttpContext context)
    {
        return SysZgsCodeHelper.getZgsCodes(param["CodeGroup"].ToString());
    }

    public object saveApplication(JObject param, HttpContext context)
    {
        var project = param["Project"].ToObject<OFS_CulProject>();

        if (project.ID == 0)
        {
            var seq = OFS_CulProjectHelper.count(project.Year) + 1;

            project.ProjectID = $"CUL{project.Year}{seq:D4}";

            OFS_CulProjectHelper.insert(project);
        }
        else
        {
            OFS_CulProjectHelper.update(project);
        }

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_CulProjectHelper.updateFormStep(project.ID, 2);
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

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_CulProjectHelper.updateFormStep(id, 6);
            OFS_CulProjectHelper.updateStatus(id, 2);
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

        OFS_CulProjectHelper.updateFunding(project);

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_CulProjectHelper.updateFormStep(project.ID, 4);
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

    public object saveRelatedProject(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_CulProjectHelper.updateFormStep(id, 5);
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

        OFS_CulProjectHelper.updateSchedule(project);

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_CulProjectHelper.updateFormStep(project.ID, 3);
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
                            else if (step.ID == 0)
                            {
                                step.PID = project.ID;
                                step.ItemID = item.ID;

                                OFS_CulGoalStepHelper.insert(step);
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
}
