using GS.OCA_OceanSubsidy.Model.OFS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class AccessibilityService : BaseService
{
    public object applyChange(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id, null, new int[] {1}); //執行中

        OFS_AccProjectHelper.updateProgressStatus(data.ProjectID, 2); //計畫變更

        OFSProjectChangeRecordHelper.insert(new ProjectChangeRecord
        {
            Type = "ACC",
            DataID = id,
            Reason = param["Reason"].ToString()
        });

        return new {};
    }

    public object createItem(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());

        getProject(id, new int[] {1,3,10}); //申請中,退回補正,修正計畫書

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
        var id = getID(param["ID"].ToString());

        return new
        {
            Project = getProject(id),
            Contacts = OFS_AccContactHelper.query(id),
            ReceivedSubsidies = OFS_AccReceivedSubsidyHelper.query(id)
        };
    }

    public object getAttachment(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());

        return new
        {
            Project = getProject(id),
            Attachments = OFS_AccAttachmentHelper.query(id)
        };
    }

    public object getBenefit(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());

        return new
        {
            Project = getProject(id),
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
        var id = getID(param["ID"].ToString());
        var project = getProject(id);

        return new
        {
            Project = project,
            OtherSubsidies = OFS_AccOtherSubsidyHelper.query(id),
            BudgetPlans = OFS_AccBudgetPlanHelper.query(id),
            GrantTargetSetting = OFSGrantTargetSettingHelper.getByTargetTypeID($"ACC{project.OrgCategory}")
        };
    }

    public object getWorkSchedule(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());

        return new
        {
            Project = getProject(id),
            Items = OFS_AccItemHelper.query(id),
            Schedules = OFS_AccScheduleHelper.query(id),
            GrantType = OFSGrantTypeHelper.getByCode("ACC")
        };
    }

    public object reviewApplication(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());

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
                    project.Status = 9; //核定補助經費
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

        OFS_AccProjectHelper.reviewApplication(project);

        return new {};
    }

    public object reviewApplicationChange(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id, null, new int[] {2}); //變更申請
        var apply = data.changeApply;

        if (apply != null && apply.Status == 2) //待審核
        {
            apply.RejectReason = null;

            if (int.Parse(param["Result"].ToString()) == 2)
            {
                apply.Status = 4; //退回修改
                apply.RejectReason = param["Reason"].ToString(); //原因
            }
            else
            {
                apply.Status = 3; //審核通過

                OFS_AccProjectHelper.updateProgressStatus(data.ProjectID, 1); //執行中
            }

            OFSProjectChangeRecordHelper.update(apply);
        }

        return new {};
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
            var data = getProject(project.ID, new int[] {1,3,13}, new int[] {2}); //申請中,退回補正 | 變更申請

            project.ProjectID = data.ProjectID;

            OFS_AccProjectHelper.update(project);

            if (data.ProgressStatus == 2)
            {
                var apply = OFSProjectChangeRecordHelper.getApplying("ACC", data.ID);

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
            OFS_AccProjectHelper.updateFormStep(project.ProjectID, 2);
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

        return new { ID = project.ProjectID };
    }

    public object saveAttachment(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id, new int[] {1,3,10,13}, new int[] {2}); //申請中,退回補正,修正計畫書 | 變更申請

        if (data.ProgressStatus == 2)
        {
            var apply = OFSProjectChangeRecordHelper.getApplying("ACC", data.ID);

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
            if (data.Status == 10)
            {
                OFS_AccProjectHelper.updateStatus(data.ProjectID, 11);
            }
            else if (data.Status == 1 || data.Status == 3)
            {
                OFS_AccProjectHelper.updateFormStep(data.ProjectID, 6);
                OFS_AccProjectHelper.updateStatus(data.ProjectID, 2);
            }

            snapshot(id);
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
        var data = getProject(project.ID, new int[] {1,3,10,13}, new int[] {2}); //申請中,退回補正,修正計畫書 | 變更申請

        if (data.ProgressStatus == 2)
        {
            var apply = OFSProjectChangeRecordHelper.getApplying("ACC", data.ID);

            if (apply != null)
            {
                apply.Form4Before = param["Before"].ToString();
                apply.Form4After = param["After"].ToString();

                OFSProjectChangeRecordHelper.update(apply);
            }
        }

        OFS_AccProjectHelper.updateBenefit(project);

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_AccProjectHelper.updateFormStep(data.ProjectID, 5);
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
        var data = getProject(project.ID, new int[] {1,3,10,13}, new int[] {2}); //申請中,退回補正,修正計畫書 | 變更申請

        OFS_AccProjectHelper.updateFunding(project);

        if (data.ProgressStatus == 2)
        {
            var apply = OFSProjectChangeRecordHelper.getApplying("ACC", data.ID);

            if (apply != null)
            {
                apply.Form3Before = param["Before"].ToString();
                apply.Form3After = param["After"].ToString();

                OFSProjectChangeRecordHelper.update(apply);
            }
        }

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_AccProjectHelper.updateFormStep(data.ProjectID, 4);
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

    public object saveOrganizer(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id, new int[] {2}); //資格審查

        OFS_AccProjectHelper.updateOrganizer(data.ID, int.Parse(param["Organizer"].ToString()));

        return new {};
    }

    public object saveWorkSchedule(JObject param, HttpContext context)
    {
        var project = param["Project"].ToObject<OFS_AccProject>();
        var data = getProject(project.ID, new int[] {1,3,10,13}, new int[] {2}); //申請中,退回補正,修正計畫書 | 變更申請

        OFS_AccProjectHelper.updateSchedule(project);

        if (data.ProgressStatus == 2)
        {
            var apply = OFSProjectChangeRecordHelper.getApplying("ACC", data.ID);

            if (apply != null)
            {
                apply.Form2Before = param["Before"].ToString();
                apply.Form2After = param["After"].ToString();

                OFSProjectChangeRecordHelper.update(apply);
            }
        }

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_AccProjectHelper.updateFormStep(data.ProjectID, 3);
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

    public object terminate(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());

        getProject(id, new int[] {13}); //核定通過

        OFS_AccProjectHelper.terminate(id, param["RejectReason"].ToString(), int.Parse(param["RecoveryAmount"].ToString()));

        return new {};
    }

    private int getID(string value)
    {
        if (int.TryParse(value, out int id))
        {
            return id;
        }
        else
        {
            return OFS_AccProjectHelper.getID(value);
        }
    }

    private OFS_AccProject getProject(int id, int[] statusList = null, int[] progressList = null)
    {
        var project = OFS_AccProjectHelper.get(id);

        if (project == null || !project.IsExists)
        {
            throw new Exception("查無資料");
        }

        if ((statusList != null && !statusList.Contains(project.Status)) || project.IsWithdrawal)
        {
            throw new Exception("狀態錯誤");
        }

        if (project.Status == 13 && progressList != null && !progressList.Contains(project.ProgressStatus))
        {
            throw new Exception("執行狀態錯誤");
        }

        if (project.ProgressStatus == 2)
        {
            project.changeApply = OFSProjectChangeRecordHelper.getApplying("ACC", project.ID);
        }

        return project;
    }

    private void snapshot(int id)
    {
    }
}
