using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Model.OFS;
using GS.OCA_OceanSubsidy.Operation.OFS;
using GS.OCA_OceanSubsidy.Operation.OSI.OpenXml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

public class EducationService : BaseService
{
    public object applyChange(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id, new int[] {51}); //執行階段-審核中

        checkPermission(data, true);

        OFS_EdcProjectHelper.setProjectChanged(id, true);

        OFSProjectChangeRecordHelper.insert(new ProjectChangeRecord
        {
            Type = "EDC",
            Method = 1,
            DataID = data.ProjectID,
            Reason = param["Reason"].ToString()
        });

        OFS_TaskQueueHelper.UpdateTaskStatus(data.ProjectID, "Change", 1, 0);

        return new {};
    }

    public object findApplication(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id);

        checkPermission(data);

        return new { Project = data };
    }

    public object getApplication(JObject param, HttpContext context)
    {
        var data = getProject(param, out JObject snapshot);

        checkPermission(data);

        if (snapshot != null)
        {
            return new
            {
                Project = snapshot["Project"],
                Contacts = snapshot["Contacts"],
                OtherSubsidies = snapshot["OtherSubsidies"],
                BudgetPlans = snapshot["BudgetPlans"],
                ReceivedSubsidies = snapshot["ReceivedSubsidies"]
            };
        }

        return new
        {
            Project = data,
            Contacts = OFS_EdcContactHelper.query(data.ID),
            OtherSubsidies = OFS_EdcOtherSubsidyHelper.query(data.ID),
            BudgetPlans = OFS_EdcBudgetPlanHelper.query(data.ID),
            ReceivedSubsidies = OFS_EdcReceivedSubsidyHelper.query(data.ID)
        };
    }

    public object getAttachment(JObject param, HttpContext context)
    {
        var data = getProject(param, out JObject snapshot);

        checkPermission(data);

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

        return new
        {
            Year = type.Year.HasValue ? type.Year.Value : (type.StartDate.Value.Year - 1911),
            SubsidyPlanType = $"{type.ShortName} ({type.FullName})"
        };
    }

    public object getPayment(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id);

        checkPermission(data);

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

        checkPermission(data);

        var stage = int.Parse(param["Stage"].ToString());

        return new {
            Project = data,
            Report = OFSReportHelper.GetStageExamStatus(data.ProjectID, stage),
            Attachments = OFS_EdcAttachmentHelper.query(id, 1)
        };
    }

    public object getReviewInfo(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id);

        checkPermission(data);

        return new {
            Year = data.Year,
            OrgName = data.OrgName,
            OtherCount = OFS_EdcProjectHelper.count(data.Year, data.OrgName) ,
            AuditRecords = AuditRecordsHelper.GetAuditRecordsByOrgName(data.OrgName)
        };
    }

    public object reviewApplication(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var project = getProject(id, new int[] {11}); //資格審查-審查中

        checkReviewPermission(project);

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
                NotificationHelper.B1("學校民間", project.ProjectName, project.Year.ToString(), project.RejectReason, toTwDate(project.CorrectionDeadline.Value), project.UserAccount);
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

        checkReviewPermission(data);

        var apply = data.changeApply;

        if (apply != null && apply.Status == 2) //待審核
        {
            apply.RejectReason = null;

            if (int.Parse(param["Result"].ToString()) == 2)
            {
                apply.Status = 4; //退回修改
                apply.RejectReason = param["Reason"].ToString();

                NotificationHelper.G3("學校民間", data.ProjectName, "計畫變更申請", apply.RejectReason, data.UserAccount);
            }
            else
            {
                apply.Status = 3; //審核通過

                OFS_EdcProjectHelper.setProjectChanged(id, false);
                OFS_EdcProjectHelper.updateLastOperation(data.ProjectID, "已完成計畫變更");

                NotificationHelper.G4("學校民間", data.ProjectName, "計畫變更申請", data.UserAccount);

                OFS_TaskQueueHelper.UpdateTaskStatus(data.ProjectID, "Change", 1, 1);
            }

            OFSProjectChangeRecordHelper.update(apply);
        }

        return new {};
    }

    public object reviewPayment(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id, new int[] {51}); //執行階段-審核中

        checkReviewPermission(data);

        var stage = int.Parse(param["Stage"].ToString());

        var payment = OFSPaymentHelper.query(data.ProjectID).FirstOrDefault(d => d.Stage == stage && d.Status == "審核中");

        if (payment == null)
        {
            throw new Exception("查無資料");
        }

        var result = int.Parse(param["Result"].ToString());

        if (result == 1)
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

        var setting = OFS_SciReimbursementHelper.GetPaymentPhaseSettings("EDC").FirstOrDefault(d => d.PhaseOrder == payment.Stage);

        if (result == 1)
        {
            NotificationHelper.G6("學校民間", data.ProjectName, setting.PhaseName, payment.CurrentActualPaidAmount, payment.ReviewerComment, data.UserAccount);

            OFS_TaskQueueHelper.UpdateTaskStatus(data.ProjectID, "Payment", 1, 1);

            OFS_EdcProjectHelper.updateLastOperation(data.ProjectID, $"已完成{setting.PhaseName}");
        }
        else
        {
            NotificationHelper.G3("學校民間", data.ProjectName, setting.PhaseName, payment.ReviewerComment, data.UserAccount);
        }

        return new {};
    }

    public object reviewReport(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id, new int[] {51}); //執行階段-審核中

        checkReviewPermission(data);

        var stage = int.Parse(param["Stage"].ToString());
        var report = OFSReportHelper.GetStageExamStatus(data.ProjectID, stage);

        if (report.Status != "審核中")
        {
            throw new Exception("審核狀態錯誤");
        }

        var comment = "";
        var status = "通過";
        var result = int.Parse(param["Result"].ToString());

        if (result == 2)
        {
            comment = param["Reason"].ToString();
            status = "退回修正";
        }

        var eventName = "成果報告";

        OFSReportHelper.ReviewStageExam(data.ProjectID, stage, eventName, status, comment, CurrentUser.UserName, CurrentUser.Account);

        if (result == 2)
        {
            NotificationHelper.G3("學校民間", data.ProjectName, eventName, comment, data.UserAccount);
        }
        else
        {
            OFS_TaskQueueHelper.UpdateTaskStatus(data.ProjectID, "Report", 1, 1);
            OFS_TaskQueueHelper.UpdateTaskStatus(data.ProjectID, "Payment", 1, 0);

            OFS_EdcProjectHelper.updateLastOperation(data.ProjectID, $"已完成{eventName}");

            NotificationHelper.G5("學校民間", data.ProjectName, eventName, data.UserAccount);

            var setting = OFS_SciReimbursementHelper.GetPaymentPhaseSettings("EDC").FirstOrDefault(d => d.PhaseOrder == 1);

            NotificationHelper.F12("學校民間", data.ProjectName, setting.PhaseName, data.UserAccount);
        }

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

            checkPermission(data, true);

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

        var others = param["OtherSubsidies"].ToObject<List<OFS_EdcOtherSubsidy>>();

        foreach (var item in others)
        {
            if (item.Deleted)
            {
                OFS_EdcOtherSubsidyHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = project.ID;

                OFS_EdcOtherSubsidyHelper.insert(item);
            }
            else
            {
                OFS_EdcOtherSubsidyHelper.update(item);
            }
        }

        var plans = param["BudgetPlans"].ToObject<List<OFS_EdcBudgetPlan>>();

        foreach (var item in plans)
        {
            if (item.Deleted)
            {
                OFS_EdcBudgetPlanHelper.delete(item.ID);
            }
            else if (item.ID == 0)
            {
                item.PID = project.ID;

                OFS_EdcBudgetPlanHelper.insert(item);
            }
            else
            {
                OFS_EdcBudgetPlanHelper.update(item);
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

        checkPermission(data, true);

        bool submit = bool.Parse(param["Submit"].ToString());

        if (data.IsProjChanged)
        {
            var apply = data.changeApply;

            if (apply != null)
            {
                apply.Form5Before = param["Before"].ToString();
                apply.Form5After = param["After"].ToString();

                if (submit)
                {
                    apply.Status = 2; //待審核
                }

                OFSProjectChangeRecordHelper.update(apply);

                if (apply.Status == 2)
                {
                    NotificationHelper.G2("學校民間", data.ProjectName, "計畫變更申請", data.Organizer);
                }
            }
        }

        if (submit)
        {
            if (data.Status == 1)
            {
                OFS_EdcProjectHelper.updateFormStep(data.ProjectID, 3);
                OFS_EdcProjectHelper.updateProgressStatus(data.ProjectID, 1); //資格審查
                OFS_EdcProjectHelper.updateApplyTime(data.ProjectID);

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

        if (submit)
        {
            if (data.Status == 1 || data.Status == 14)
            {
                mergePdfFiles(data, "送審版", context);
                mergePdfFiles(data, "核定版", context);

                NotificationHelper.A1("學校民間", data.ProjectName, "EDC");
                NotificationHelper.A3("學校民間", data.ProjectName, data.Status == 1 ? "申請送件" : "補正補件", DateTime.Now, data.UserAccount);
            }
            else if (data.IsProjChanged)
            {
                mergePdfFiles(data, "計畫變更最新版", context);
            }
        }

        return new {};
    }

    public object saveOrganizer(JObject param, HttpContext context)
    {
        if (!CurrentUser.IsOrganizer && !CurrentUser.IsSupervisor && !CurrentUser.IsSysAdmin)
        {
            throw new InvalidOperationException();
        }

        var id = getID(param["ID"].ToString());
        var data = getProject(id);

        OFS_EdcProjectHelper.updateOrganizer(id, int.Parse(param["Organizer"].ToString()));

        return new {};
    }

    public object savePayment(JObject param, HttpContext context)
    {
        var payment = param["Payment"].ToObject<OFSPayment>();
        var id = getID(payment.ProjectID);
        var data = getProject(id, new int[] {51}); //執行階段-審核中

        checkPermission(data, true);

        //--

        var list = OFSPaymentHelper.query(data.ProjectID);

        if (payment.TotalSpentAmount <= 0)
        {
            throw new Exception("金額錯誤");
        }

        var model = list.FirstOrDefault(d => d.Stage == payment.Stage);

        if (model == null)
        {
            model = new OFSPayment();
            model.ProjectID = payment.ProjectID;
            model.Stage = payment.Stage;
        }

        var submit = bool.Parse(param["Submit"].ToString());

        model.ActDisbursementRatioPct = payment.ActDisbursementRatioPct;
        model.TotalSpentAmount = payment.TotalSpentAmount;
        model.CurrentRequestAmount = payment.TotalSpentAmount;
        model.Status = submit ? "審核中" : "請款中";

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

        if (submit)
        {
            var setting = OFS_SciReimbursementHelper.GetPaymentPhaseSettings("EDC").FirstOrDefault(d => d.PhaseOrder == model.Stage);

            NotificationHelper.G2("學校民間", data.ProjectName, setting.PhaseName, data.Organizer);
        }

        return new {};
    }

    public object saveReport(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id, new int[] {51}); //執行階段-審核中

        checkPermission(data, true);

        var stage = int.Parse(param["Stage"].ToString());
        var submit = bool.Parse(param["Submit"].ToString());

        OFSReportHelper.SubmitStageExam(data.ProjectID, stage, submit ? "審核中" : "暫存");

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

        if (submit)
        {
            NotificationHelper.G2("學校民間", data.ProjectName, "成果報告", data.Organizer);
        }

        return new {};
    }

    public object terminate(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id);

        checkReviewPermission(data);

        OFS_EdcProjectHelper.terminate(id, param["RejectReason"].ToString(), int.Parse(param["RecoveryAmount"].ToString()));

        return new {};
    }

    private void checkPermission(OFS_EdcProject data, bool forUpdate = false)
    {
        checkProjectPermission("EDC", data.Year, data.Organizer, data.UserAccount, forUpdate);
    }

    private void checkReviewPermission(OFS_EdcProject data)
    {
        checkReviewPermission("EDC", data.Organizer);
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
                project.changeApply = OFSProjectChangeRecordHelper.getApplying("EDC", 1, project.ProjectID);
            }
            else if (project.Status == 42 || project.Status == 43)
            {
                project.changeApply = OFSProjectChangeRecordHelper.getApplying("EDC", 2, project.ProjectID);
            }

            project.isOrganizer = getReviewPermission("EDC", project.Organizer);

            return project;
        }

        throw new Exception("狀態錯誤");
    }

    private OFS_EdcProject getProject(JObject param, out JObject snapshot)
    {
        var project = getProject(getID(param["ID"].ToString()));

        snapshot = project.ProgressStatus >= 5 && bool.TryParse(param["Apply"]?.ToString(), out bool apply) && apply ? getSnapshot("EDC", project.ID) : null;

        return project;
    }

    private void mergePdfFiles(OFS_EdcProject data, string version, HttpContext context)
    {
        var prefix = Path.GetFullPath(Path.Combine(context.Server.MapPath("~"), ".."));
        var paths = OFS_EdcAttachmentHelper.query(data.ID).Select(d => Path.Combine(prefix, "UploadFiles", "files", d.Path)).ToList();

        string filename = $"{data.ProjectID}_{version}.pdf";
        string folder = Path.Combine(prefix, "UploadFiles", "OFS", "EDC", data.ProjectID);

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        PdfHelper.MergePdfs(paths, Path.Combine(folder, filename));
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
                OtherSubsidies = OFS_EdcOtherSubsidyHelper.query(id),
                BudgetPlans = OFS_EdcBudgetPlanHelper.query(id),
                ReceivedSubsidies = OFS_EdcReceivedSubsidyHelper.query(id),
                Attachments = OFS_EdcAttachmentHelper.query(id)
            })
        });
    }
}
