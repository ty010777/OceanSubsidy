using GS.OCA_OceanSubsidy.Model.OFS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class LiteracyService : BaseService
{
    public object createItem(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        getProject(id, new int[] {1,3}); //申請中,退回補正

        var item = new OFS_LitItem {
            PID = id,
            Begin = 0,
            End = 0
        };

        OFS_LitItemHelper.insert(item);

        return item;
    }

    public object getApplication(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        return new
        {
            Project = OFS_LitProjectHelper.get(id),
            Contacts = OFS_LitContactHelper.query(id),
            PreviousStudies = OFS_LitPreviousStudyHelper.query(id)
        };
    }

    public object getAttachment(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        return new
        {
            Project = OFS_LitProjectHelper.get(id),
            Attachments = OFS_LitAttachmentHelper.query(id)
        };
    }

    public object getBenefit(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        return new
        {
            Project = OFS_LitProjectHelper.get(id),
            Benefits = OFS_LitBenefitHelper.query(id)
        };
    }

    public object getEmptyApplication(JObject param, HttpContext context)
    {
        var type = OFSGrantTypeHelper.getByCode("LIT");

        return new { Year = type.StartDate.Value.Year - 1911, SubsidyPlanType = $"{type.ShortName} ({type.FullName})" };
    }

    public object getFunding(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());
        var project = OFS_LitProjectHelper.get(id);

        return new
        {
            Project = project,
            OtherSubsidies = OFS_LitOtherSubsidyHelper.query(id),
            BudgetPlans = OFS_LitBudgetPlanHelper.query(id),
            GrantTargetSetting = OFSGrantTargetSettingHelper.getByTargetTypeID($"LIT{project.Field}")
        };
    }

    public object getWorkSchedule(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        return new
        {
            Project = OFS_LitProjectHelper.get(id),
            Items = OFS_LitItemHelper.query(id),
            Schedules = OFS_LitScheduleHelper.query(id),
            GrantType = OFSGrantTypeHelper.getByCode("LIT")
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

        OFS_LitProjectHelper.reviewApplication(project);

        return new {};
    }

    public object saveApplication(JObject param, HttpContext context)
    {
        var project = param["Project"].ToObject<OFS_LitProject>();

        if (project.ID == 0)
        {
            var info = SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo);
            var seq = OFS_LitProjectHelper.count(project.Year) + 1;

            project.ProjectID = $"LIT{project.Year}{seq:D4}";
            project.UserAccount = info.Account;
            project.UserName = info.UserName;
            project.UserOrg = info.UnitName;

            OFS_LitProjectHelper.insert(project);
        }
        else
        {
            var data = getProject(project.ID, new int[] {1,3}); //申請中,退回補正

            project.ProjectID = data.ProjectID;

            OFS_LitProjectHelper.update(project);
        }

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_LitProjectHelper.updateFormStep(project.ProjectID, 2);
        }

        var contacts = param["Contacts"].ToObject<List<OFS_LitContact>>();

        foreach (var item in contacts)
        {
            if (item.ID == 0)
            {
                item.PID = project.ID;

                OFS_LitContactHelper.insert(item);
            }
            else
            {
                OFS_LitContactHelper.update(item);
            }
        }

        var studies = param["PreviousStudies"].ToObject<List<OFS_LitPreviousStudy>>();

        foreach (var item in studies)
        {
            if (item.Deleted)
            {
                OFS_LitPreviousStudyHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = project.ID;

                OFS_LitPreviousStudyHelper.insert(item);
            }
            else
            {
                OFS_LitPreviousStudyHelper.update(item);
            }
        }

        return new { ID = project.ID };
    }

    public object saveAttachment(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());
        var data = getProject(id, new int[] {1,3}); //申請中,退回補正

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_LitProjectHelper.updateFormStep(data.ProjectID, 6);
            OFS_LitProjectHelper.updateStatus(data.ProjectID, 2);

            snapshot(id);
        }

        var attachments = param["Attachments"].ToObject<List<OFS_LitAttachment>>();

        foreach (var item in attachments)
        {
            if (item.Deleted)
            {
                OFS_LitAttachmentHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = id;

                OFS_LitAttachmentHelper.insert(item);
            }
        }

        return new {};
    }

    public object saveBenefit(JObject param, HttpContext context)
    {
        var project = param["Project"].ToObject<OFS_LitProject>();
        var data = getProject(project.ID, new int[] {1,3}); //申請中,退回補正

        OFS_LitProjectHelper.updateBenefit(project);

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_LitProjectHelper.updateFormStep(data.ProjectID, 5);
        }

        var benefits = param["Benefits"].ToObject<List<OFS_LitBenefit>>();

        foreach (var item in benefits)
        {
            if (item.Deleted)
            {
                OFS_LitBenefitHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = project.ID;

                OFS_LitBenefitHelper.insert(item);
            }
            else
            {
                OFS_LitBenefitHelper.update(item);
            }
        }

        return new {};
    }

    public object saveFunding(JObject param, HttpContext context)
    {
        var project = param["Project"].ToObject<OFS_LitProject>();
        var data = getProject(project.ID, new int[] {1,3}); //申請中,退回補正

        OFS_LitProjectHelper.updateFunding(project);

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_LitProjectHelper.updateFormStep(data.ProjectID, 4);
        }

        var others = param["OtherSubsidies"].ToObject<List<OFS_LitOtherSubsidy>>();

        foreach (var item in others)
        {
            if (item.Deleted)
            {
                OFS_LitOtherSubsidyHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = project.ID;

                OFS_LitOtherSubsidyHelper.insert(item);
            }
            else
            {
                OFS_LitOtherSubsidyHelper.update(item);
            }
        }

        var plans = param["BudgetPlans"].ToObject<List<OFS_LitBudgetPlan>>();

        foreach (var item in plans)
        {
            if (item.Deleted)
            {
                OFS_LitBudgetPlanHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = project.ID;

                OFS_LitBudgetPlanHelper.insert(item);
            }
            else
            {
                OFS_LitBudgetPlanHelper.update(item);
            }
        }

        return new {};
    }

    public object saveOrganizer(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());
        var data = getProject(id, new int[] {2}); //資格審查

        OFS_LitProjectHelper.updateOrganizer(data.ID, int.Parse(param["Organizer"].ToString()));

        return new {};
    }

    public object saveWorkSchedule(JObject param, HttpContext context)
    {
        var project = param["Project"].ToObject<OFS_LitProject>();
        var data = getProject(project.ID, new int[] {1,3}); //申請中,退回補正

        OFS_LitProjectHelper.updateSchedule(project);

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_LitProjectHelper.updateFormStep(data.ProjectID, 3);
        }

        var items = param["Items"].ToObject<List<OFS_LitItem>>();

        foreach (var item in items)
        {
            if (item.Deleted)
            {
                OFS_LitItemHelper.delete(item.ID);
            }
            else
            {
                OFS_LitItemHelper.update(item);
            }
        }

        var schedules = param["Schedules"].ToObject<List<OFS_LitSchedule>>();

        foreach (var item in schedules)
        {
            if (item.Deleted)
            {
                OFS_LitScheduleHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = project.ID;

                OFS_LitScheduleHelper.insert(item);
            }
            else
            {
                OFS_LitScheduleHelper.update(item);
            }
        }

        return new {};
    }

    public object terminate(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        getProject(id, new int[] {13}); //核定通過

        OFS_LitProjectHelper.terminate(id, param["RejectReason"].ToString(), int.Parse(param["RecoveryAmount"].ToString()));

        return new {};
    }

    private OFS_LitProject getProject(int id, int[] statusList = null)
    {
        var project = OFS_LitProjectHelper.get(id);

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

    private void snapshot(int id)
    {
    }
}
