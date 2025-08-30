using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class MultipleService : BaseService
{
    public object createItem(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        getProject(id, new int[] {1,3,10}); //申請中,退回補正,修正計畫書

        var item = new OFS_MulItem {
            PID = id,
            Begin = 0,
            End = 0
        };

        OFS_MulItemHelper.insert(item);

        return item;
    }

    public object getApplication(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        return new
        {
            Project = OFS_MulProjectHelper.get(id),
            Contacts = OFS_MulContactHelper.query(id),
            ReceivedSubsidies = OFS_MulReceivedSubsidyHelper.query(id)
        };
    }

    public object getAttachment(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        return new
        {
            Project = OFS_MulProjectHelper.get(id),
            Attachments = OFS_MulAttachmentHelper.query(id)
        };
    }

    public object getBenefit(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        return new
        {
            Project = OFS_MulProjectHelper.get(id),
            Benefits = OFS_MulBenefitHelper.query(id)
        };
    }

    public object getEmptyApplication(JObject param, HttpContext context)
    {
        var type = OFSGrantTypeHelper.getByCode("MUL");

        return new { Year = type.StartDate.Value.Year - 1911, SubsidyPlanType = $"{type.ShortName} ({type.FullName})" };
    }

    public object getFunding(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());
        var project = OFS_MulProjectHelper.get(id);

        return new
        {
            Project = project,
            OtherSubsidies = OFS_MulOtherSubsidyHelper.query(id),
            BudgetPlans = OFS_MulBudgetPlanHelper.query(id),
            GrantTargetSetting = OFSGrantTargetSettingHelper.getByTargetTypeID($"MUL{project.OrgCategory}")
        };
    }

    public object getWorkSchedule(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        return new
        {
            Project = OFS_MulProjectHelper.get(id),
            Items = OFS_MulItemHelper.query(id),
            Schedules = OFS_MulScheduleHelper.query(id),
            GrantType = OFSGrantTypeHelper.getByCode("MUL")
        };
    }

    public object reviewApplication(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        var project = getProject(id, new int[] {2}); //資格審查

        project.RejectReason = null;
        project.CorrectionDeadline = null;

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
                project.Status = 9; //核定補助經費
                break;
        }

        OFS_MulProjectHelper.reviewApplication(project);

        return new {};
    }

    public object saveApplication(JObject param, HttpContext context)
    {
        var project = param["Project"].ToObject<OFS_MulProject>();

        if (project.ID == 0)
        {
            var info = SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo);
            var seq = OFS_MulProjectHelper.count(project.Year) + 1;

            project.ProjectID = $"MUL{project.Year}{seq:D4}";
            project.UserAccount = info.Account;
            project.UserName = info.UserName;
            project.UserOrg = info.UnitName;

            OFS_MulProjectHelper.insert(project);
        }
        else
        {
            getProject(project.ID, new int[] {1,3}); //申請中,退回補正

            OFS_MulProjectHelper.update(project);
        }

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_MulProjectHelper.updateFormStep(project.ID, 2);
        }

        var contacts = param["Contacts"].ToObject<List<OFS_MulContact>>();

        foreach (var item in contacts)
        {
            if (item.ID == 0)
            {
                item.PID = project.ID;

                OFS_MulContactHelper.insert(item);
            }
            else
            {
                OFS_MulContactHelper.update(item);
            }
        }

        var receiveds = param["ReceivedSubsidies"].ToObject<List<OFS_MulReceivedSubsidy>>();

        foreach (var item in receiveds)
        {
            if (item.Deleted)
            {
                OFS_MulReceivedSubsidyHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = project.ID;

                OFS_MulReceivedSubsidyHelper.insert(item);
            }
            else
            {
                OFS_MulReceivedSubsidyHelper.update(item);
            }
        }

        return new { ID = project.ID };
    }

    public object saveAttachment(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        getProject(id, new int[] {1,3,10}); //申請中,退回補正,修正計畫書

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_MulProjectHelper.updateFormStep(id, 6);
            OFS_MulProjectHelper.updateStatus(id, 2);
        }

        var attachments = param["Attachments"].ToObject<List<OFS_MulAttachment>>();

        foreach (var item in attachments)
        {
            if (item.Deleted)
            {
                OFS_MulAttachmentHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = id;

                OFS_MulAttachmentHelper.insert(item);
            }
        }

        return new {};
    }

    public object saveBenefit(JObject param, HttpContext context)
    {
        var project = param["Project"].ToObject<OFS_MulProject>();

        getProject(project.ID, new int[] {1,3,10}); //申請中,退回補正,修正計畫書

        OFS_MulProjectHelper.updateBenefit(project);

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_MulProjectHelper.updateFormStep(project.ID, 5);
        }

        var benefits = param["Benefits"].ToObject<List<OFS_MulBenefit>>();

        foreach (var item in benefits)
        {
            if (item.Deleted)
            {
                OFS_MulBenefitHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = project.ID;

                OFS_MulBenefitHelper.insert(item);
            }
            else
            {
                OFS_MulBenefitHelper.update(item);
            }
        }

        return new {};
    }

    public object saveFunding(JObject param, HttpContext context)
    {
        var project = param["Project"].ToObject<OFS_MulProject>();

        getProject(project.ID, new int[] {1,3,10}); //申請中,退回補正,修正計畫書

        OFS_MulProjectHelper.updateFunding(project);

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_MulProjectHelper.updateFormStep(project.ID, 4);
        }

        var others = param["OtherSubsidies"].ToObject<List<OFS_MulOtherSubsidy>>();

        foreach (var item in others)
        {
            if (item.Deleted)
            {
                OFS_MulOtherSubsidyHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = project.ID;

                OFS_MulOtherSubsidyHelper.insert(item);
            }
            else
            {
                OFS_MulOtherSubsidyHelper.update(item);
            }
        }

        var plans = param["BudgetPlans"].ToObject<List<OFS_MulBudgetPlan>>();

        foreach (var item in plans)
        {
            if (item.Deleted)
            {
                OFS_MulBudgetPlanHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = project.ID;

                OFS_MulBudgetPlanHelper.insert(item);
            }
            else
            {
                OFS_MulBudgetPlanHelper.update(item);
            }
        }

        return new {};
    }

    public object saveOrganizer(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        getProject(id, new int[] {2}); //資格審查

        OFS_MulProjectHelper.updateOrganizer(id, int.Parse(param["Organizer"].ToString()));

        return new {};
    }

    public object saveWorkSchedule(JObject param, HttpContext context)
    {
        var project = param["Project"].ToObject<OFS_MulProject>();

        getProject(project.ID, new int[] {1,3,10}); //申請中,退回補正,修正計畫書

        OFS_MulProjectHelper.updateSchedule(project);

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_MulProjectHelper.updateFormStep(project.ID, 3);
        }

        var items = param["Items"].ToObject<List<OFS_MulItem>>();

        foreach (var item in items)
        {
            if (item.Deleted)
            {
                OFS_MulItemHelper.delete(item.ID);
            }
            else
            {
                OFS_MulItemHelper.update(item);
            }
        }

        var schedules = param["Schedules"].ToObject<List<OFS_MulSchedule>>();

        foreach (var item in schedules)
        {
            if (item.Deleted)
            {
                OFS_MulScheduleHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = project.ID;

                OFS_MulScheduleHelper.insert(item);
            }
            else
            {
                OFS_MulScheduleHelper.update(item);
            }
        }

        return new {};
    }

    private OFS_MulProject getProject(int id, int[] statusList = null)
    {
        var project = OFS_MulProjectHelper.get(id);

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
