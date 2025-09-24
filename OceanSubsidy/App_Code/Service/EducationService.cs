using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Model.OFS;
using GS.OCA_OceanSubsidy.Operation.OFS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class EducationService : BaseService
{
    public object applyChange(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id, new int[] {51}); //執行階段-審核中

        OFS_EdcProjectHelper.setProjectChanged(id, true);

        OFSProjectChangeRecordHelper.insert(new ProjectChangeRecord
        {
            Type = "EDC",
            Method = 1,
            DataID = data.ProjectID,
            Reason = param["Reason"].ToString()
        });

        return new {};
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
        var data = getProject(param, out JObject snapshot);

        if (snapshot != null)
        {
            return new
            {
                Project = snapshot["Project"],
                Contacts = snapshot["Contacts"],
                ReceivedSubsidies = snapshot["ReceivedSubsidies"]
            };
        }

        return new
        {
            Project = data,
            Contacts = OFS_EdcContactHelper.query(data.ID),
            ReceivedSubsidies = OFS_EdcReceivedSubsidyHelper.query(data.ID)
        };
    }

    public object getAttachment(JObject param, HttpContext context)
    {
        var data = getProject(param, out JObject snapshot);

        if (snapshot != null)
        {
            return new
            {
                Project = snapshot["Project"],
                Attachments = snapshot["Attachments"]
            };
        }

        return new
        {
            Project = data,
            Attachments = OFS_EdcAttachmentHelper.query(data.ID)
        };
    }

    public object getEmptyApplication(JObject param, HttpContext context)
    {
        var type = OFSGrantTypeHelper.getByCode("EDC");

        return new { Year = type.StartDate.Value.Year - 1911, SubsidyPlanType = $"{type.ShortName} ({type.FullName})" };
    }

    public object getPayment(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id);

        return new
        {
            Project = data,
            Payments = OFSPaymentHelper.query(data.ProjectID),
            Attachments = OFS_EdcAttachmentHelper.query(id, 2)
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
            Attachments = OFS_EdcAttachmentHelper.query(id, 1)
        };
    }

    public object reviewApplication(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());

        var project = getProject(id, new int[] {11}); //資格審查-審查中

        project.RejectReason = null;
        project.CorrectionDeadline = null;

        switch (int.Parse(param["Result"].ToString()))
        {
            case 2:
                project.Status = 13; //不通過
                project.RejectReason = param["Reason"].ToString();
                saveApplyReviewLog(project.ProjectID, "資格審查-不通過", project.RejectReason);
                break;
            case 3:
                project.Status = 14; //補正補件
                project.RejectReason = param["Reason"].ToString();
                project.CorrectionDeadline = DateTime.Parse(param["CorrectionDeadline"].ToString()); //補正期限
                saveApplyReviewLog(project.ProjectID, "資格審查-補正補件", project.RejectReason, project.CorrectionDeadline);
                break;
            default:
                project.Status = 12; //通過
                saveApplyReviewLog(project.ProjectID, "資格審查-通過");
                break;
        }

        OFS_EdcProjectHelper.reviewApplication(project);

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

                OFS_EdcProjectHelper.setProjectChanged(id, false);
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

            OFS_EdcProjectHelper.updateProgressStatus(data.ProjectID, 9); //結案
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
            var data = getProject(project.ID, new int[] {1,14}, true); //編輯中,補正補件 | 變更申請

            project.ProjectID = data.ProjectID;

            OFS_EdcProjectHelper.update(project);

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

        return new { ID = project.ProjectID };
    }

    public object saveAttachment(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id, new int[] {1,14}, true); //編輯中,補正補件 | 變更申請

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
                OFS_EdcProjectHelper.updateFormStep(data.ProjectID, 3);
                OFS_EdcProjectHelper.updateProgressStatus(data.ProjectID, 1); //資格審查

                saveApplyLog(data.ProjectID, "編輯中");
            }
            else if (data.Status == 14)
            {
                OFS_EdcProjectHelper.updateStatus(data.ProjectID, 11); //審查中

                saveApplyLog(data.ProjectID, "資格審查-補正補件");
            }

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
        var id = getID(param["ID"].ToString());

        getProject(id);

        OFS_EdcProjectHelper.updateOrganizer(id, int.Parse(param["Organizer"].ToString()));

        return new {};
    }

    public object savePayment(JObject param, HttpContext context)
    {
        var payment = param["Payment"].ToObject<OFS_SCI_Payment>();
        var id = getID(payment.ProjectID);
        var data = getProject(id, new int[] {51}); //執行階段-審核中

        //--

        var list = OFSPaymentHelper.query(data.ProjectID);

        if (payment.TotalSpentAmount <= 0)
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
        model.CurrentRequestAmount = payment.TotalSpentAmount;
        model.Status = bool.Parse(param["Submit"].ToString()) ? "審核中" : "請款中";

        OFSPaymentHelper.submit(model);

        //--

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
                item.Stage = 2; //請款

                OFS_EdcAttachmentHelper.insert(item);
            }
        }

        return new {};
    }

    public object saveReport(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id, new int[] {51}); //執行階段-審核中

        OFS_SciInterimReportHelper.SubmitStageExam(data.ProjectID, int.Parse(param["Stage"].ToString()), bool.Parse(param["Submit"].ToString()) ? "審核中" : "暫存");

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
                item.Stage = 1; //報告

                OFS_EdcAttachmentHelper.insert(item);
            }
        }

        return new {};
    }

    public object terminate(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());

        getProject(id);

        OFS_EdcProjectHelper.terminate(id, param["RejectReason"].ToString(), int.Parse(param["RecoveryAmount"].ToString()));

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
            return OFS_EdcProjectHelper.getID(value);
        }
    }

    private OFS_EdcProject getProject(int id, int[] statusList = null, bool changed = false)
    {
        var project = OFS_EdcProjectHelper.get(id);

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
                project.changeApply = OFSProjectChangeRecordHelper.getApplying("EDC", project.ProjectID);
            }

            return project;
        }

        throw new Exception("狀態錯誤");
    }

    private OFS_EdcProject getProject(JObject param, out JObject snapshot)
    {
        var project = OFS_EdcProjectHelper.get(getID(param["ID"].ToString()));

        snapshot = project.ProgressStatus >= 5 && bool.Parse(param["Apply"].ToString()) ? getSnapshot("EDC", project.ID) : null;

        return project;
    }

    private void snapshot(int id)
    {
        var project = getProject(id);

        OFSSnapshotHelper.insert(new Snapshot
        {
            Type = "EDC",
            DataID = id,
            Status = project.Status,
            Data = JsonConvert.SerializeObject(new
            {
                Project = project,
                Contacts = OFS_EdcContactHelper.query(id),
                ReceivedSubsidies = OFS_EdcReceivedSubsidyHelper.query(id),
                Attachments = OFS_EdcAttachmentHelper.query(id)
            })
        });
    }
}
