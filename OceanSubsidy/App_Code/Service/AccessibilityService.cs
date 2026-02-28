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

public class AccessibilityService : BaseService
{
    public object applyChange(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id, new int[] {51}); //執行階段-審核中

        checkPermission(data, true);

        OFS_AccProjectHelper.setProjectChanged(id, true);

        OFSProjectChangeRecordHelper.insert(new ProjectChangeRecord
        {
            Type = "ACC",
            Method = 1,
            DataID = data.ProjectID,
            Reason = param["Reason"].ToString()
        });

        OFS_TaskQueueHelper.UpdateTaskStatus(data.ProjectID, "Change", 1, 0);

        return new {};
    }

    public object createItem(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id, new int[] {1,14,42}, true); //編輯中,補正補件,計畫書修正中 | 變更申請

        checkPermission(data, true);

        var item = new OFS_AccItem {
            PID = id,
            Begin = 0,
            End = 0
        };

        OFS_AccItemHelper.insert(item);

        return item;
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
                ReceivedSubsidies = snapshot["ReceivedSubsidies"]
            };
        }

        return new
        {
            Project = data,
            Contacts = OFS_AccContactHelper.query(data.ID),
            ReceivedSubsidies = OFS_AccReceivedSubsidyHelper.query(data.ID)
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
            Attachments = OFS_AccAttachmentHelper.query(data.ID)
        };
    }

    public object getBenefit(JObject param, HttpContext context)
    {
        var data = getProject(param, out JObject snapshot);

        checkPermission(data);

        if (snapshot != null)
        {
            return new
            {
                Project = snapshot["Project"],
                Benefits = snapshot["Benefits"]
            };
        }

        return new
        {
            Project = data,
            Benefits = OFS_AccBenefitHelper.query(data.ID)
        };
    }

    public object getEmptyApplication(JObject param, HttpContext context)
    {
        var type = OFSGrantTypeHelper.getByCode("ACC");

        return new
        {
            Year = type.Year.HasValue ? type.Year.Value : (type.StartDate.Value.Year - 1911),
            SubsidyPlanType = $"{type.ShortName} ({type.FullName})"
        };
    }

    public object getFunding(JObject param, HttpContext context)
    {
        var data = getProject(param, out JObject snapshot);

        checkPermission(data);

        if (snapshot != null)
        {
            data = snapshot["Project"].ToObject<OFS_AccProject>();

            return new
            {
                Project = data,
                OtherSubsidies = snapshot["OtherSubsidies"],
                BudgetPlans = snapshot["BudgetPlans"],
                GrantTargetSetting = OFSGrantTargetSettingHelper.getByTargetTypeID($"ACC{data.OrgCategory}")
            };
        }

        return new
        {
            Project = data,
            OtherSubsidies = OFS_AccOtherSubsidyHelper.query(data.ID),
            BudgetPlans = OFS_AccBudgetPlanHelper.query(data.ID),
            GrantTargetSetting = OFSGrantTargetSettingHelper.getByTargetTypeID($"ACC{data.OrgCategory}")
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
            Attachments = OFS_AccAttachmentHelper.query(id, 2)
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
            Attachments = OFS_AccAttachmentHelper.query(id, 1)
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
            OtherCount = OFS_AccProjectHelper.count(data.Year, data.OrgName) ,
            AuditRecords = AuditRecordsHelper.GetAuditRecordsByOrgName(data.OrgName)
        };
    }

    public object getWorkSchedule(JObject param, HttpContext context)
    {
        var data = getProject(param, out JObject snapshot);

        checkPermission(data);

        if (snapshot != null)
        {
            return new
            {
                Project = snapshot["Project"],
                Items = snapshot["Items"],
                Schedules = snapshot["Schedules"],
                GrantType = OFSGrantTypeHelper.getByCode("ACC")
            };
        }

        return new
        {
            Project = data,
            Items = OFS_AccItemHelper.query(data.ID),
            Schedules = OFS_AccScheduleHelper.query(data.ID),
            GrantType = OFSGrantTypeHelper.getByCode("ACC")
        };
    }

    public object reviewApplication(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var project = getProject(id, new int[] {11,43}); //資格審查-審查中, 決審-計畫書審核中

        checkReviewPermission(project);

        project.RejectReason = null;
        project.CorrectionDeadline = null;

        if (project.Status == 11)
        {
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
                    NotificationHelper.B1("無障礙", project.ProjectName, project.Year.ToString(), project.RejectReason, toTwDate(project.CorrectionDeadline.Value), project.UserAccount);
                    break;
                default:
                    project.Status = 12; //通過
                    saveApplyReviewLog(project.ProjectID, "資格審查-通過");
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
                    saveRevisionReviewLog(project.ProjectID, "決審核定-不通過", project.RejectReason);
                    break;
                case 3:
                    project.Status = 42; //計畫書修正中
                    project.RejectReason = param["Reason"].ToString();
                    saveRevisionReviewLog(project.ProjectID, "決審核定-計畫書修正中", project.RejectReason);
                    break;
                default:
                    project.Status = 44; //計畫書已確認
                    saveRevisionReviewLog(project.ProjectID, "決審核定-計畫書已確認");
                    break;
            }

            var apply = project.changeApply;

            if (apply != null && apply.Status == 2) //待審核
            {
                if (int.Parse(param["Result"].ToString()) == 1)
                {
                    apply.Status = 3; //審核通過
                    apply.RejectReason = null;
                }
                else
                {
                    apply.Status = 4; //退回修改
                    apply.RejectReason = param["Reason"].ToString();
                }

                OFSProjectChangeRecordHelper.update(apply);
            }
        }

        OFS_AccProjectHelper.reviewApplication(project);

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

                NotificationHelper.G3("無障礙", data.ProjectName, "計畫變更申請", apply.RejectReason, data.UserAccount);
            }
            else
            {
                apply.Status = 3; //審核通過

                OFS_AccProjectHelper.setProjectChanged(id, false);
                OFS_AccProjectHelper.updateLastOperation(data.ProjectID, "已完成計畫變更");

                NotificationHelper.G4("無障礙", data.ProjectName, "計畫變更申請", data.UserAccount);

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

            if (payment.Stage == 2)
            {
                OFS_AccProjectHelper.updateProgressStatus(data.ProjectID, 9); //結案
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

        var setting = OFS_SciReimbursementHelper.GetPaymentPhaseSettings("ACC").FirstOrDefault(d => d.PhaseOrder == payment.Stage);

        if (result == 1)
        {
            NotificationHelper.G6("無障礙", data.ProjectName, setting.PhaseName, payment.CurrentActualPaidAmount, payment.ReviewerComment, data.UserAccount);

            OFS_TaskQueueHelper.UpdateTaskStatus(data.ProjectID, $"Payment{payment.Stage}", 1, 1);

            OFS_AccProjectHelper.updateLastOperation(data.ProjectID, $"已完成{setting.PhaseName}");
        }
        else
        {
            NotificationHelper.G3("無障礙", data.ProjectName, setting.PhaseName, payment.ReviewerComment, data.UserAccount);
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

        var eventName = stage == 1 ? "期中報告" : "成果報告";

        OFSReportHelper.ReviewStageExam(data.ProjectID, stage, eventName, status, comment, CurrentUser.UserName, CurrentUser.Account);

        if (result == 2)
        {
            NotificationHelper.G3("無障礙", data.ProjectName, eventName, comment, data.UserAccount);
        }
        else
        {
            OFS_TaskQueueHelper.UpdateTaskStatus(data.ProjectID, stage == 1 ? "MidReport" : "FinalReport", 1, 1);
            OFS_TaskQueueHelper.UpdateTaskStatus(data.ProjectID, $"Payment{stage}", 1, 0);

            OFS_AccProjectHelper.updateLastOperation(data.ProjectID, $"已完成{eventName}");

            NotificationHelper.G5("無障礙", data.ProjectName, eventName, data.UserAccount);

            var setting = OFS_SciReimbursementHelper.GetPaymentPhaseSettings("ACC").FirstOrDefault(d => d.PhaseOrder == stage);

            NotificationHelper.F12("無障礙", data.ProjectName, setting.PhaseName, data.UserAccount);
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

            while (OFS_AccProjectHelper.getID($"ACC{project.Year}{seq:D4}") > 0)
            {
                seq += 1;
            }

            project.ProjectID = $"ACC{project.Year}{seq:D4}";
            project.UserAccount = info.Account;
            project.UserName = info.UserName;
            project.UserOrg = info.UnitName;

            OFS_AccProjectHelper.insert(project);
        }
        else
        {
            var data = getProject(project.ID, new int[] {1,14}, true); //編輯中,補正補件 | 變更申請

            checkPermission(data, true);

            project.ProjectID = data.ProjectID;

            OFS_AccProjectHelper.update(project);

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
        var data = getProject(id, new int[] {1,14,42}, true); //編輯中,補正補件,計畫書修正中 | 變更申請

        checkPermission(data, true);

        bool submit = bool.Parse(param["Submit"].ToString());

        if (data.IsProjChanged || data.Status == 42)
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

                if (data.IsProjChanged && apply.Status == 2)
                {
                    NotificationHelper.G2("無障礙", data.ProjectName, "計畫變更申請", data.Organizer);
                }
            }
        }

        if (submit)
        {
            if (data.Status == 1)
            {
                OFS_AccProjectHelper.updateFormStep(data.ProjectID, 6);
                OFS_AccProjectHelper.updateProgressStatus(data.ProjectID, 1); //資格審查
                OFS_AccProjectHelper.updateApplyTime(data.ProjectID);

                saveApplyLog(data.ProjectID, "編輯中");
            }
            else if (data.Status == 14)
            {
                OFS_AccProjectHelper.updateStatus(data.ProjectID, 11); //審查中

                saveApplyLog(data.ProjectID, "資格審查-補正補件");
            }
            else if (data.Status == 42)
            {
                OFS_AccProjectHelper.updateStatus(data.ProjectID, 43); //計畫書審核中

                saveRevisionLog(data.ProjectID);
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

        if (submit)
        {
            if (data.Status == 1 || data.Status == 14)
            {
                mergePdfFiles(data, "送審版", context);
                mergePdfFiles(data, "核定版", context);

                NotificationHelper.A1("無障礙", data.ProjectName, "ACC");
                NotificationHelper.A3("無障礙", data.ProjectName, data.Status == 1 ? "申請送件" : "補正補件", DateTime.Now, data.UserAccount);
            }
            else if (data.Status == 42)
            {
                mergePdfFiles(data, "核定版", context);

                NotificationHelper.A2("無障礙", data.ProjectName, data.Organizer);
                NotificationHelper.A3("無障礙", data.ProjectName, "修正計畫書提送", DateTime.Now, data.UserAccount);
            }
            else if (data.IsProjChanged)
            {
                mergePdfFiles(data, "計畫變更最新版", context);
            }
        }

        return new {};
    }

    public object saveBenefit(JObject param, HttpContext context)
    {
        var project = param["Project"].ToObject<OFS_AccProject>();
        var data = getProject(project.ID, new int[] {1,14,42}, true); //編輯中,補正補件,計畫書修正中 | 變更申請

        checkPermission(data, true);

        if (data.IsProjChanged || data.Status == 42)
        {
            var apply = data.changeApply;

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
        var data = getProject(project.ID, new int[] {1,14,42}, true); //編輯中,補正補件,計畫書修正中 | 變更申請

        checkPermission(data, true);

        OFS_AccProjectHelper.updateFunding(project);

        if (data.IsProjChanged || data.Status == 42)
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
        if (!CurrentUser.IsOrganizer && !CurrentUser.IsSupervisor && !CurrentUser.IsSysAdmin)
        {
            throw new InvalidOperationException();
        }

        var id = getID(param["ID"].ToString());
        var data = getProject(id);

        OFS_AccProjectHelper.updateOrganizer(id, int.Parse(param["Organizer"].ToString()));

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
        var prev = list.Where(d => d.Stage < payment.Stage).Sum(d => d.CurrentRequestAmount);

        if (prev >= payment.TotalSpentAmount)
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
        model.CurrentRequestAmount = payment.TotalSpentAmount - prev;
        model.Status = submit ? "審核中" : "請款中";

        OFSPaymentHelper.submit(model);

        //--

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
                item.Stage = 2; //請款

                OFS_AccAttachmentHelper.insert(item);
            }
        }

        if (submit)
        {
            var setting = OFS_SciReimbursementHelper.GetPaymentPhaseSettings("ACC").FirstOrDefault(d => d.PhaseOrder == model.Stage);

            NotificationHelper.G2("無障礙", data.ProjectName, setting.PhaseName, data.Organizer);
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
                item.Stage = 1; //報告

                OFS_AccAttachmentHelper.insert(item);
            }
        }

        if (submit)
        {
            NotificationHelper.G2("無障礙", data.ProjectName, stage == 1 ? "期中報告" : "成果報告", data.Organizer);
        }

        return new {};
    }

    public object saveWorkSchedule(JObject param, HttpContext context)
    {
        var project = param["Project"].ToObject<OFS_AccProject>();
        var data = getProject(project.ID, new int[] {1,14,42}, true); //編輯中,補正補件,計畫書修正中 | 變更申請

        checkPermission(data, true);

        OFS_AccProjectHelper.updateSchedule(project);

        if (data.IsProjChanged || data.Status == 42)
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
        var data = getProject(id);

        checkReviewPermission(data);

        OFS_AccProjectHelper.terminate(id, param["RejectReason"].ToString(), int.Parse(param["RecoveryAmount"].ToString()));

        return new {};
    }

    private void checkPermission(OFS_AccProject data, bool forUpdate = false)
    {
        checkProjectPermission("ACC", data.Year, data.Organizer, data.UserAccount, forUpdate);
    }

    private void checkReviewPermission(OFS_AccProject data)
    {
        checkReviewPermission("ACC", data.Organizer);
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

    private OFS_AccProject getProject(int id, int[] statusList = null, bool changed = false)
    {
        var project = OFS_AccProjectHelper.get(id);

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
                project.changeApply = OFSProjectChangeRecordHelper.getApplying("ACC", 1, project.ProjectID);
            }
            else if (project.Status == 42 || project.Status == 43)
            {
                project.changeApply = OFSProjectChangeRecordHelper.getApplying("ACC", 2, project.ProjectID);
            }

            project.isOrganizer = getReviewPermission("ACC", project.Organizer);

            return project;
        }

        throw new Exception("狀態錯誤");
    }

    private OFS_AccProject getProject(JObject param, out JObject snapshot)
    {
        var project = getProject(getID(param["ID"].ToString()));

        snapshot = project.ProgressStatus >= 5 && bool.TryParse(param["Apply"]?.ToString(), out bool apply) && apply ? getSnapshot("ACC", project.ID) : null;

        return project;
    }

    private void mergePdfFiles(OFS_AccProject data, string version, HttpContext context)
    {
        var prefix = Path.GetFullPath(Path.Combine(context.Server.MapPath("~"), ".."));
        var paths = OFS_AccAttachmentHelper.query(data.ID).Select(d => Path.Combine(prefix, "UploadFiles", "files", d.Path)).ToList();

        string filename = $"{data.ProjectID}_{version}.pdf";
        string folder = Path.Combine(prefix, "UploadFiles", "OFS", "ACC", data.ProjectID);

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
            Type = "ACC",
            DataID = id,
            Status = project.Status,
            Data = JsonConvert.SerializeObject(new
            {
                Project = project,
                Contacts = OFS_AccContactHelper.query(id),
                ReceivedSubsidies = OFS_AccReceivedSubsidyHelper.query(id),
                Items = OFS_AccItemHelper.query(id),
                Schedules = OFS_AccScheduleHelper.query(id),
                OtherSubsidies = OFS_AccOtherSubsidyHelper.query(id),
                BudgetPlans = OFS_AccBudgetPlanHelper.query(id),
                Benefits = OFS_AccBenefitHelper.query(id),
                Attachments = OFS_AccAttachmentHelper.query(id)
            })
        });
    }
}
