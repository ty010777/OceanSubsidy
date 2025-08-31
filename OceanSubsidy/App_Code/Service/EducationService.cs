using GS.OCA_OceanSubsidy.Model.OFS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class EducationService : BaseService
{
    public object getApplication(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        return new
        {
            Project = OFS_EdcProjectHelper.get(id),
            Contacts = OFS_EdcContactHelper.query(id),
            ReceivedSubsidies = OFS_EdcReceivedSubsidyHelper.query(id)
        };
    }

    public object getAttachment(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        return new
        {
            Project = OFS_EdcProjectHelper.get(id),
            Attachments = OFS_EdcAttachmentHelper.query(id)
        };
    }

    public object getEmptyApplication(JObject param, HttpContext context)
    {
        var type = OFSGrantTypeHelper.getByCode("EDC");

        return new { Year = type.StartDate.Value.Year - 1911, SubsidyPlanType = $"{type.ShortName} ({type.FullName})" };
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

        OFS_EdcProjectHelper.reviewApplication(project);

        return new {};
    }

    public object saveApplication(JObject param, HttpContext context)
    {
        var project = param["Project"].ToObject<OFS_EdcProject>();

        if (project.ID == 0)
        {
            var info = SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo);
            var seq = OFS_EdcProjectHelper.count(project.Year) + 1;

            project.ProjectID = $"EDC{project.Year}{seq:D4}";
            project.UserAccount = info.Account;
            project.UserName = info.UserName;
            project.UserOrg = info.UnitName;

            OFS_EdcProjectHelper.insert(project);
        }
        else
        {
            var data = getProject(project.ID, new int[] {1,3}); //申請中,退回補正

            project.ProjectID = data.ProjectID;

            OFS_EdcProjectHelper.update(project);
        }

        if (bool.Parse(param["Submit"].ToString()))
        {
            OFS_EdcProjectHelper.updateFormStep(project.ProjectID, 2);
        }

        var contacts = param["Contacts"].ToObject<List<OFS_EdcContact>>();

        foreach (var item in contacts)
        {
            if (item.ID == 0)
            {
                item.PID = project.ID;

                OFS_EdcContactHelper.insert(item);
            }
            else
            {
                OFS_EdcContactHelper.update(item);
            }
        }

        var receiveds = param["ReceivedSubsidies"].ToObject<List<OFS_EdcReceivedSubsidy>>();

        foreach (var item in receiveds)
        {
            if (item.Deleted)
            {
                OFS_EdcReceivedSubsidyHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = project.ID;

                OFS_EdcReceivedSubsidyHelper.insert(item);
            }
            else
            {
                OFS_EdcReceivedSubsidyHelper.update(item);
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
            OFS_EdcProjectHelper.updateFormStep(data.ProjectID, 3);
            OFS_EdcProjectHelper.updateStatus(data.ProjectID, 2);

            snapshot(id);
        }

        var attachments = param["Attachments"].ToObject<List<OFS_EdcAttachment>>();

        foreach (var item in attachments)
        {
            if (item.Deleted)
            {
                OFS_EdcAttachmentHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = id;

                OFS_EdcAttachmentHelper.insert(item);
            }
        }

        return new {};
    }

    public object saveOrganizer(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());
        var data = getProject(id, new int[] {2}); //資格審查

        OFS_EdcProjectHelper.updateOrganizer(data.ID, int.Parse(param["Organizer"].ToString()));

        return new {};
    }

    public object terminate(JObject param, HttpContext context)
    {
        var id = int.Parse(param["ID"].ToString());

        getProject(id, new int[] {13}); //核定通過

        OFS_EdcProjectHelper.terminate(id, param["RejectReason"].ToString(), int.Parse(param["RecoveryAmount"].ToString()));

        return new {};
    }

    private OFS_EdcProject getProject(int id, int[] statusList = null)
    {
        var project = OFS_EdcProjectHelper.get(id);

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
