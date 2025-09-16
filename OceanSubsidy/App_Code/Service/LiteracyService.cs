using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Model.OFS;
using GS.OCA_OceanSubsidy.Operation.OFS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class LiteracyService : BaseService
{
    public object applyChange(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());

        getProject(id, new int[] {51}); //執行階段-審核中

        OFS_LitProjectHelper.setProjectChanged(id, true);

        OFSProjectChangeRecordHelper.insert(new ProjectChangeRecord
        {
            Type = "LIT",
            DataID = id,
            Reason = param["Reason"].ToString()
        });

        return new {};
    }

    public object createItem(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());

        getProject(id, new int[] {1,14}, true); //編輯中,補正補件 | 變更申請

        var item = new OFS_LitItem {
            PID = id,
            Begin = 0,
            End = 0
        };

        OFS_LitItemHelper.insert(item);

        return item;
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
        var id = getID(param["ID"].ToString());

        return new
        {
            Project = getProject(id),
            Contacts = OFS_LitContactHelper.query(id),
            PreviousStudies = OFS_LitPreviousStudyHelper.query(id)
        };
    }

    public object getAttachment(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());

        return new
        {
            Project = getProject(id),
            Attachments = OFS_LitAttachmentHelper.query(id)
        };
    }

    public object getBenefit(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());

        return new
        {
            Project = getProject(id),
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
        var id = getID(param["ID"].ToString());
        var project = getProject(id);

        return new
        {
            Project = project,
            OtherSubsidies = OFS_LitOtherSubsidyHelper.query(id),
            BudgetPlans = OFS_LitBudgetPlanHelper.query(id),
            GrantTargetSetting = OFSGrantTargetSettingHelper.getByTargetTypeID($"LIT{project.Field}")
        };
    }

    public object getPayment(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id);

        return new
        {
            Project = data,
            Payments = OFSPaymentHelper.query(data.ProjectID),
            Attachments = OFS_LitAttachmentHelper.query(id, 2)
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
            Attachments = OFS_LitAttachmentHelper.query(id, 1)
        };
    }

    public object getWorkSchedule(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());

        return new
        {
            Project = getProject(id),
            Items = OFS_LitItemHelper.query(id),
            Schedules = OFS_LitScheduleHelper.query(id),
            GrantType = OFSGrantTypeHelper.getByCode("LIT")
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
                break;
            case 3:
                project.Status = 14; //補正補件
                project.RejectReason = param["Reason"].ToString();
                project.CorrectionDeadline = DateTime.Parse(param["CorrectionDeadline"].ToString()); //補正期限
                break;
            default:
                project.Status = 12; //通過
                break;
        }

        OFS_LitProjectHelper.reviewApplication(project);

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

                OFS_LitProjectHelper.setProjectChanged(id, false);
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

            if (payment.Stage == 2)
            {
                OFS_CulProjectHelper.updateProgressStatus(data.ProjectID, 9); //結案
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
            var data = getProject(project.ID, new int[] {1,14}, true); //編輯中,補正補件 | 變更申請

            project.ProjectID = data.ProjectID;

            OFS_LitProjectHelper.update(project);

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
                OFS_LitProjectHelper.updateFormStep(data.ProjectID, 6);
                OFS_LitProjectHelper.updateProgressStatus(data.ProjectID, 1); //資格審查

                ApplicationChecklistHelper.InsertCaseHistoryLog(new OFS_CaseHistoryLog
                {
                    ProjectID = data.ProjectID,
                    ChangeTime = DateTime.Now,
                    UserName = CurrentUser.UserName,
                    StageStatusBefore = "編輯中",
                    StageStatusAfter = "資格審查-審核中",
                    Description = "完成附件上傳並提送申請"
                });
            }
            else if (data.Status == 14)
            {
                OFS_LitProjectHelper.updateStatus(data.ProjectID, 11); //審查中
            }

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
        var data = getProject(project.ID, new int[] {1,14}, true); //編輯中,補正補件 | 變更申請

        if (data.IsProjChanged)
        {
            var apply = data.changeApply;

            if (apply != null)
            {
                apply.Form4Before = param["Before"].ToString();
                apply.Form4After = param["After"].ToString();

                OFSProjectChangeRecordHelper.update(apply);
            }
        }

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
        var data = getProject(project.ID, new int[] {1,14}, true); //編輯中,補正補件 | 變更申請

        OFS_LitProjectHelper.updateFunding(project);

        if (data.IsProjChanged)
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
        var id = getID(param["ID"].ToString());

        getProject(id);

        OFS_LitProjectHelper.updateOrganizer(id, int.Parse(param["Organizer"].ToString()));

        return new {};
    }

    public object savePayment(JObject param, HttpContext context)
    {
        var payment = param["Payment"].ToObject<OFS_SCI_Payment>();
        var id = getID(payment.ProjectID);
        var data = getProject(id, new int[] {51}); //執行階段-審核中

        //--

        var list = OFSPaymentHelper.query(data.ProjectID);
        var model = list.FirstOrDefault(d => d.Stage == payment.Stage);

        if (model == null)
        {
            model = new OFS_SCI_Payment();
            model.ProjectID = payment.ProjectID;
            model.Stage = payment.Stage;
        }

        model.ActDisbursementRatioPct = payment.ActDisbursementRatioPct;
        model.TotalSpentAmount = payment.TotalSpentAmount;
        model.CurrentRequestAmount = payment.CurrentRequestAmount;
        model.Status = bool.Parse(param["Submit"].ToString()) ? "審核中" : "請款中";

        OFSPaymentHelper.submit(model);

        //--

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
                item.Stage = 2; //請款

                OFS_LitAttachmentHelper.insert(item);
            }
        }

        return new {};
    }

    public object saveReport(JObject param, HttpContext context)
    {
        var id = getID(param["ID"].ToString());
        var data = getProject(id, new int[] {51}); //執行階段-審核中

        OFS_SciInterimReportHelper.SubmitStageExam(data.ProjectID, int.Parse(param["Stage"].ToString()), bool.Parse(param["Submit"].ToString()) ? "審核中" : "暫存");

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
                item.Stage = 1; //報告

                OFS_LitAttachmentHelper.insert(item);
            }
        }

        return new {};
    }

    public object saveWorkSchedule(JObject param, HttpContext context)
    {
        var project = param["Project"].ToObject<OFS_LitProject>();
        var data = getProject(project.ID, new int[] {1,14}, true); //編輯中,補正補件 | 變更申請

        OFS_LitProjectHelper.updateSchedule(project);

        if (data.IsProjChanged)
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
        var id = getID(param["ID"].ToString());

        getProject(id);

        OFS_LitProjectHelper.terminate(id, param["RejectReason"].ToString(), int.Parse(param["RecoveryAmount"].ToString()));

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
            return OFS_LitProjectHelper.getID(value);
        }
    }

    private OFS_LitProject getProject(int id, int[] statusList = null, bool changed = false)
    {
        var project = OFS_LitProjectHelper.get(id);

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
                project.changeApply = OFSProjectChangeRecordHelper.getApplying("LIT", project.ID);
            }

            return project;
        }

        throw new Exception("狀態錯誤");
    }

    private void snapshot(int id)
    {
        var project = getProject(id);

        OFSSnapshotHelper.insert(new Snapshot
        {
            Type = "LIT",
            DataID = id,
            Status = project.Status,
            Data = JsonConvert.SerializeObject(new
            {
                Project = project,
                Contacts = OFS_LitContactHelper.query(id),
                PreviousStudies = OFS_LitPreviousStudyHelper.query(id),
                Items = OFS_LitItemHelper.query(id),
                Schedules = OFS_LitScheduleHelper.query(id),
                OtherSubsidies = OFS_LitOtherSubsidyHelper.query(id),
                BudgetPlans = OFS_LitBudgetPlanHelper.query(id),
                Benefits = OFS_LitBenefitHelper.query(id),
                Attachments = OFS_LitAttachmentHelper.query(id)
            })
        });
    }
}
