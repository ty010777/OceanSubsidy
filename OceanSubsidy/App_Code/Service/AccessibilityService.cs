using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class AccessibilityService : BaseService
{
    public object createItem(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        getProject(id, new int[] {1});

        var item = new OFS_AccItem {
            PID = id,
            Begin = 0,
            End = 0
        };

        OFS_AccItemHelper.insert(item);

        return item;
    }

    public object getApplication(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        return new
        {
            Project = OFS_AccProjectHelper.get(id),
            Contacts = OFS_AccContactHelper.query(id),
            ReceivedSubsidies = OFS_AccReceivedSubsidyHelper.query(id)
        };
    }

    public object getAttachment(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        return new
        {
            Project = OFS_AccProjectHelper.get(id),
            Attachments = OFS_AccAttachmentHelper.query(id)
        };
    }

    public object getBenefit(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        return new
        {
            Project = OFS_AccProjectHelper.get(id),
            Benefits = OFS_AccBenefitHelper.query(id)
        };
    }

    public object getEmptyApplication(JObject param, HttpContext context)
    {
        var type = OFSGrantTypeHelper.getByCode("ACC");

        return new { Year = type.StartDate.Value.Year - 1911, SubsidyPlanType = $"{type.ShortName} ({type.FullName})" };
    }

    public object getFunding(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        return new
        {
            Project = OFS_AccProjectHelper.get(id),
            OtherSubsidies = OFS_AccOtherSubsidyHelper.query(id),
            BudgetPlans = OFS_AccBudgetPlanHelper.query(id)
        };
    }

    public object getWorkSchedule(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        return new
        {
            Project = OFS_AccProjectHelper.get(id),
            Items = OFS_AccItemHelper.query(id),
            Schedules = OFS_AccScheduleHelper.query(id)
        };
    }

    public object saveApplication(JObject param, HttpContext context)
    {
        var project = param["Project"].ToObject<OFS_AccProject>();

        if (project.ID == 0)
        {
            var info = SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo);
            var seq = OFS_AccProjectHelper.count(project.Year) + 1;

            project.ProjectID = $"ACC{project.Year}{seq:D4}";
            project.UserAccount = info.Account;
            project.UserName = info.UserName;
            project.UserOrg = info.UnitName;

            OFS_AccProjectHelper.insert(project);
        }
        else
        {
            getProject(project.ID, new int[] {1});

            OFS_AccProjectHelper.update(project);
        }

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_AccProjectHelper.updateFormStep(project.ID, 2);
        }

        var contacts = param["Contacts"].ToObject<List<OFS_AccContact>>();

        foreach (var item in contacts)
        {
            if (item.ID == 0)
            {
                item.PID = project.ID;

                OFS_AccContactHelper.insert(item);
            }
            else
            {
                OFS_AccContactHelper.update(item);
            }
        }

        var receiveds = param["ReceivedSubsidies"].ToObject<List<OFS_AccReceivedSubsidy>>();

        foreach (var item in receiveds)
        {
            if (item.Deleted)
            {
                OFS_AccReceivedSubsidyHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = project.ID;

                OFS_AccReceivedSubsidyHelper.insert(item);
            }
            else
            {
                OFS_AccReceivedSubsidyHelper.update(item);
            }
        }

        return new { ID = project.ID };
    }

    public object saveAttachment(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        getProject(id, new int[] {1});

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_AccProjectHelper.updateFormStep(id, 6);
            OFS_AccProjectHelper.updateStatus(id, 2);
        }

        var attachments = param["Attachments"].ToObject<List<OFS_AccAttachment>>();

        foreach (var item in attachments)
        {
            if (item.Deleted)
            {
                OFS_AccAttachmentHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = id;

                OFS_AccAttachmentHelper.insert(item);
            }
        }

        return new {};
    }

    public object saveBenefit(JObject param, HttpContext context)
    {
        var project = param["Project"].ToObject<OFS_AccProject>();

        getProject(project.ID, new int[] {1});

        OFS_AccProjectHelper.updateBenefit(project);

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_AccProjectHelper.updateFormStep(project.ID, 5);
        }

        var benefits = param["Benefits"].ToObject<List<OFS_AccBenefit>>();

        foreach (var item in benefits)
        {
            if (item.Deleted)
            {
                OFS_AccBenefitHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = project.ID;

                OFS_AccBenefitHelper.insert(item);
            }
            else
            {
                OFS_AccBenefitHelper.update(item);
            }
        }

        return new {};
    }

    public object saveFunding(JObject param, HttpContext context)
    {
        var project = param["Project"].ToObject<OFS_AccProject>();

        getProject(project.ID, new int[] {1});

        OFS_AccProjectHelper.updateFunding(project);

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_AccProjectHelper.updateFormStep(project.ID, 4);
        }

        var others = param["OtherSubsidies"].ToObject<List<OFS_AccOtherSubsidy>>();

        foreach (var item in others)
        {
            if (item.Deleted)
            {
                OFS_AccOtherSubsidyHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = project.ID;

                OFS_AccOtherSubsidyHelper.insert(item);
            }
            else
            {
                OFS_AccOtherSubsidyHelper.update(item);
            }
        }

        var plans = param["BudgetPlans"].ToObject<List<OFS_AccBudgetPlan>>();

        foreach (var item in plans)
        {
            if (item.Deleted)
            {
                OFS_AccBudgetPlanHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = project.ID;

                OFS_AccBudgetPlanHelper.insert(item);
            }
            else
            {
                OFS_AccBudgetPlanHelper.update(item);
            }
        }

        return new {};
    }

    public object saveWorkSchedule(JObject param, HttpContext context)
    {
        var project = param["Project"].ToObject<OFS_AccProject>();

        getProject(project.ID, new int[] {1});

        OFS_AccProjectHelper.updateSchedule(project);

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_AccProjectHelper.updateFormStep(project.ID, 3);
        }

        var items = param["Items"].ToObject<List<OFS_AccItem>>();

        foreach (var item in items)
        {
            if (item.Deleted)
            {
                OFS_AccItemHelper.delete(item.ID);
            }
            else
            {
                OFS_AccItemHelper.update(item);
            }
        }

        var schedules = param["Schedules"].ToObject<List<OFS_AccSchedule>>();

        foreach (var item in schedules)
        {
            if (item.Deleted)
            {
                OFS_AccScheduleHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = project.ID;

                OFS_AccScheduleHelper.insert(item);
            }
            else
            {
                OFS_AccScheduleHelper.update(item);
            }
        }

        return new {};
    }

    private OFS_AccProject getProject(int id, int[] statusList = null)
    {
        var project = OFS_AccProjectHelper.get(id);

        if (project == null)
        {
            throw new Exception("查無資料");
        }

        if (statusList != null && !statusList.Contains(project.Status))
        {
            throw new Exception("狀態錯誤");
        }

        return project;
    }
}
