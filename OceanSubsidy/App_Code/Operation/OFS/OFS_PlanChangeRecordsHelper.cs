using GS.Data.Sql;
using GS.OCA_OceanSubsidy.Model.OFS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

/// <summary>
/// 計畫變更紀錄 Helper
/// </summary>
public class OFS_PlanChangeRecordsHelper
{
    /// <summary>
    /// 根據 ProjectID 判斷計畫類型
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>計畫類型代碼 (SCI, CUL, EDC, CLB, ACC, MUL, LIT)</returns>
    public static string GetProjectType(string projectID)
    {
        if (string.IsNullOrEmpty(projectID))
            return string.Empty;

        // ProjectID 格式: 類型代碼 + 年度 + 序號
        // 例如: SCI2024001, CUL1140001, CLB1140001
        if (projectID.StartsWith("SCI"))
            return "SCI";
        else if (projectID.StartsWith("CUL"))
            return "CUL";
        else if (projectID.StartsWith("EDC"))
            return "EDC";
        else if (projectID.StartsWith("CLB"))
            return "CLB";
        else if (projectID.StartsWith("ACC"))
            return "ACC";
        else if (projectID.StartsWith("MUL"))
            return "MUL";
        else if (projectID.StartsWith("LIT"))
            return "LIT";

        return string.Empty;
    }

    /// <summary>
    /// 取得計畫基本資料
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>計畫基本資料的 HTML 字串</returns>
    public static string GetProjectBasicData(string projectID)
    {
        if (string.IsNullOrEmpty(projectID))
            return string.Empty;

        string projectType = GetProjectType(projectID);
        var sb = new StringBuilder();

        try
        {
            switch (projectType)
            {
                case "SCI":
                    return GetSCIProjectData(projectID);
                case "CUL":
                    return GetCULProjectData(projectID);
                case "EDC":
                    return GetEDCProjectData(projectID);
                case "CLB":
                    return GetCLBProjectData(projectID);
                case "ACC":
                    return GetACCProjectData(projectID);
                case "MUL":
                    return GetMULProjectData(projectID);
                case "LIT":
                    return GetLITProjectData(projectID);
                default:
                    return $"<li><span class=\"text-gray fw-bold\">計畫編號：</span><span>{projectID}</span></li>" +
                           $"<li><span class=\"text-gray fw-bold\">錯誤：</span><span class=\"text-danger\">無法識別的計畫類型</span></li>";
            }
        }
        catch (Exception ex)
        {
            return $"<li><span class=\"text-gray fw-bold\">計畫編號：</span><span>{projectID}</span></li>" +
                   $"<li><span class=\"text-gray fw-bold\">錯誤：</span><span class=\"text-danger\">{ex.Message}</span></li>";
        }
    }

    #region 各類型計畫資料取得方法

    private static string GetSCIProjectData(string projectID)
    {
        var project = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);
        if (project == null)
            return $"<li><span class=\"text-gray fw-bold\">計畫編號：</span><span>{projectID}</span></li>" +
                   $"<li><span class=\"text-gray fw-bold\">錯誤：</span><span class=\"text-danger\">找不到計畫資料</span></li>";

        var sb = new StringBuilder();
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">計畫編號：</span><span>{projectID}</span></li>");
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">計畫名稱：</span><span>{project.ProjectNameTw ?? ""}</span></li>");
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">執行單位：</span><span>{project.OrgName ?? ""}</span></li>");

        string period = "";
        if (project.StartTime.HasValue && project.EndTime.HasValue)
        {
            period = $"{FormatDate(project.StartTime.Value)} - {FormatDate(project.EndTime.Value)}";
        }
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">計畫期程：</span><span>{period}</span></li>");

        return sb.ToString();
    }

    private static string GetCULProjectData(string projectID)
    {
        int id = OFS_CulProjectHelper.getID(projectID);
        if (id == 0)
            return $"<li><span class=\"text-gray fw-bold\">計畫編號：</span><span>{projectID}</span></li>" +
                   $"<li><span class=\"text-gray fw-bold\">錯誤：</span><span class=\"text-danger\">找不到計畫資料</span></li>";

        var project = OFS_CulProjectHelper.get(id);
        if (project == null)
            return $"<li><span class=\"text-gray fw-bold\">計畫編號：</span><span>{projectID}</span></li>" +
                   $"<li><span class=\"text-gray fw-bold\">錯誤：</span><span class=\"text-danger\">找不到計畫資料</span></li>";

        var sb = new StringBuilder();
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">計畫編號：</span><span>{projectID}</span></li>");
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">計畫名稱：</span><span>{project.ProjectName ?? ""}</span></li>");
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">執行單位：</span><span>{project.OrgName ?? ""}</span></li>");

        string period = "";
        if (project.StartTime.HasValue && project.EndTime.HasValue)
        {
            period = $"{FormatDate(project.StartTime.Value)} - {FormatDate(project.EndTime.Value)}";
        }
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">計畫期程：</span><span>{period}</span></li>");

        return sb.ToString();
    }

    private static string GetEDCProjectData(string projectID)
    {
        int id = OFS_EdcProjectHelper.getID(projectID);
        if (id == 0)
            return $"<li><span class=\"text-gray fw-bold\">計畫編號：</span><span>{projectID}</span></li>" +
                   $"<li><span class=\"text-gray fw-bold\">錯誤：</span><span class=\"text-danger\">找不到計畫資料</span></li>";

        var project = OFS_EdcProjectHelper.get(id);
        if (project == null)
            return $"<li><span class=\"text-gray fw-bold\">計畫編號：</span><span>{projectID}</span></li>" +
                   $"<li><span class=\"text-gray fw-bold\">錯誤：</span><span class=\"text-danger\">找不到計畫資料</span></li>";

        var sb = new StringBuilder();
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">計畫編號：</span><span>{projectID}</span></li>");
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">計畫名稱：</span><span>{project.ProjectName ?? ""}</span></li>");
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">執行單位：</span><span>{project.OrgName ?? ""}</span></li>");

        string period = "";
        if (project.StartTime.HasValue && project.EndTime.HasValue)
        {
            period = $"{FormatDate(project.StartTime.Value)} - {FormatDate(project.EndTime.Value)}";
        }
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">計畫期程：</span><span>{period}</span></li>");

        return sb.ToString();
    }

    private static string GetCLBProjectData(string projectID)
    {
        var project = OFS_ClbApplicationHelper.GetBasicData(projectID);
        if (project == null)
            return $"<li><span class=\"text-gray fw-bold\">計畫編號：</span><span>{projectID}</span></li>" +
                   $"<li><span class=\"text-gray fw-bold\">錯誤：</span><span class=\"text-danger\">找不到計畫資料</span></li>";

        var planData = OFS_ClbApplicationHelper.GetPlanData(projectID);

        var sb = new StringBuilder();
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">計畫編號：</span><span>{projectID}</span></li>");
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">計畫名稱：</span><span>{project.ProjectNameTw ?? ""}</span></li>");
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">執行單位：</span><span>{project.SchoolName ?? ""} - {project.ClubName ?? ""}</span></li>");

        string period = "";
        if (planData != null && planData.StartDate.HasValue && planData.EndDate.HasValue)
        {
            period = $"{FormatDate(planData.StartDate.Value)} - {FormatDate(planData.EndDate.Value)}";
        }
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">計畫期程：</span><span>{period}</span></li>");

        return sb.ToString();
    }

    private static string GetACCProjectData(string projectID)
    {
        int id = OFS_AccProjectHelper.getID(projectID);
        if (id == 0)
            return $"<li><span class=\"text-gray fw-bold\">計畫編號：</span><span>{projectID}</span></li>" +
                   $"<li><span class=\"text-gray fw-bold\">錯誤：</span><span class=\"text-danger\">找不到計畫資料</span></li>";

        var project = OFS_AccProjectHelper.get(id);
        if (project == null)
            return $"<li><span class=\"text-gray fw-bold\">計畫編號：</span><span>{projectID}</span></li>" +
                   $"<li><span class=\"text-gray fw-bold\">錯誤：</span><span class=\"text-danger\">找不到計畫資料</span></li>";

        var sb = new StringBuilder();
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">計畫編號：</span><span>{projectID}</span></li>");
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">計畫名稱：</span><span>{project.ProjectName ?? ""}</span></li>");
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">執行單位：</span><span>{project.OrgName ?? ""}</span></li>");

        string period = "";
        if (project.StartTime.HasValue && project.EndTime.HasValue)
        {
            period = $"{FormatDate(project.StartTime.Value)} - {FormatDate(project.EndTime.Value)}";
        }
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">計畫期程：</span><span>{period}</span></li>");

        return sb.ToString();
    }

    private static string GetMULProjectData(string projectID)
    {
        int id = OFS_MulProjectHelper.getID(projectID);
        if (id == 0)
            return $"<li><span class=\"text-gray fw-bold\">計畫編號：</span><span>{projectID}</span></li>" +
                   $"<li><span class=\"text-gray fw-bold\">錯誤：</span><span class=\"text-danger\">找不到計畫資料</span></li>";

        var project = OFS_MulProjectHelper.get(id);
        if (project == null)
            return $"<li><span class=\"text-gray fw-bold\">計畫編號：</span><span>{projectID}</span></li>" +
                   $"<li><span class=\"text-gray fw-bold\">錯誤：</span><span class=\"text-danger\">找不到計畫資料</span></li>";

        var sb = new StringBuilder();
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">計畫編號：</span><span>{projectID}</span></li>");
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">計畫名稱：</span><span>{project.ProjectName ?? ""}</span></li>");
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">執行單位：</span><span>{project.OrgName ?? ""}</span></li>");

        string period = "";
        if (project.StartTime.HasValue && project.EndTime.HasValue)
        {
            period = $"{FormatDate(project.StartTime.Value)} - {FormatDate(project.EndTime.Value)}";
        }
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">計畫期程：</span><span>{period}</span></li>");

        return sb.ToString();
    }

    private static string GetLITProjectData(string projectID)
    {
        int id = OFS_LitProjectHelper.getID(projectID);
        if (id == 0)
            return $"<li><span class=\"text-gray fw-bold\">計畫編號：</span><span>{projectID}</span></li>" +
                   $"<li><span class=\"text-gray fw-bold\">錯誤：</span><span class=\"text-danger\">找不到計畫資料</span></li>";

        var project = OFS_LitProjectHelper.get(id);
        if (project == null)
            return $"<li><span class=\"text-gray fw-bold\">計畫編號：</span><span>{projectID}</span></li>" +
                   $"<li><span class=\"text-gray fw-bold\">錯誤：</span><span class=\"text-danger\">找不到計畫資料</span></li>";

        var sb = new StringBuilder();
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">計畫編號：</span><span>{projectID}</span></li>");
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">計畫名稱：</span><span>{project.ProjectName ?? ""}</span></li>");
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">執行單位：</span><span>{project.OrgName ?? ""}</span></li>");

        string period = "";
        if (project.StartTime.HasValue && project.EndTime.HasValue)
        {
            period = $"{FormatDate(project.StartTime.Value)} - {FormatDate(project.EndTime.Value)}";
        }
        sb.AppendLine($"<li><span class=\"text-gray fw-bold\">計畫期程：</span><span>{period}</span></li>");

        return sb.ToString();
    }

    #endregion

    /// <summary>
    /// 取得計畫變更紀錄清單
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    /// <returns>變更紀錄清單</returns>
    public static List<ChangeRecordItem> GetChangeRecordsList(string projectID)
    {
        var records = new List<ChangeRecordItem>();

        if (string.IsNullOrEmpty(projectID))
            return records;

        string projectType = GetProjectType(projectID);
        if (string.IsNullOrEmpty(projectType))
            return records;

        DbHelper db = new DbHelper();

        try
        {
            db.CommandText = @"
                SELECT [ID]
                      ,[Type]
                      ,[Method]
                      ,[DataID]
                      ,[Reason]
                      ,[Form1Before]
                      ,[Form1After]
                      ,[Form2Before]
                      ,[Form2After]
                      ,[Form3Before]
                      ,[Form3After]
                      ,[Form4Before]
                      ,[Form4After]
                      ,[Form5Before]
                      ,[Form5After]
                      ,[Status]
                      ,[RejectReason]
                      ,[CreateTime]
                      ,[CreateUser]
                      ,[UpdateTime]
                      ,[UpdateUser]
                  FROM [OFS_ProjectChangeRecord]
                 WHERE [Type] = @Type
                   AND [DataID] = @DataID
                   AND [Status] = 3
              ORDER BY [CreateTime] DESC
            ";

            db.Parameters.Add("@Type", projectType);
            db.Parameters.Add("@DataID", projectID);

            DataTable table = db.GetTable();

            int version = table.Rows.Count;
            foreach (DataRow row in table.Rows)
            {
                var record = new ChangeRecordItem
                {
                    Version = $"V{version}.0",
                    ChangeDate = FormatDateTime(row.Field<DateTime?>("CreateTime")),
                    ChangedBy = GetUserName(row.Field<int?>("CreateUser")),
                    ChangeReason = row.Field<string>("Reason") ?? "",
                    BeforeChange = BuildChangeContent(row, "Before"),
                    AfterChange = BuildChangeContent(row, "After")
                };

                records.Add(record);
                version--;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得變更紀錄時發生錯誤: {ex.Message}");
        }
        finally
        {
            db.Dispose();
        }

        return records;
    }

    /// <summary>
    /// 建立變更內容字串
    /// </summary>
    private static string BuildChangeContent(DataRow row, string suffix)
    {
        var contents = new List<string>();

        for (int i = 1; i <= 5; i++)
        {
            string fieldName = $"Form{i}{suffix}";
            string content = row.Field<string>(fieldName);

            if (!string.IsNullOrEmpty(content))
            {
                contents.Add($"表單{i}：{content}");
            }
        }

        return contents.Count > 0 ? string.Join("\n", contents) : "-";
    }

    /// <summary>
    /// 取得使用者名稱
    /// </summary>
    private static string GetUserName(int? userId)
    {
        if (!userId.HasValue)
            return "-";

        try
        {
            DbHelper db = new DbHelper();
            db.CommandText = "SELECT [Name] FROM [Sys_User] WHERE [UserID] = @UserID";
            db.Parameters.Add("@UserID", userId.Value);

            DataTable table = db.GetTable();
            if (table.Rows.Count > 0)
            {
                return table.Rows[0].Field<string>("Name") ?? "-";
            }
        }
        catch
        {
            // 忽略錯誤
        }

        return "-";
    }

    /// <summary>
    /// 格式化日期 (民國年)
    /// </summary>
    private static string FormatDate(DateTime date)
    {
        int rocYear = date.Year - 1911;
        return $"{rocYear}/{date.Month:D2}/{date.Day:D2}";
    }

    /// <summary>
    /// 格式化日期時間 (民國年)
    /// </summary>
    private static string FormatDateTime(DateTime? dateTime)
    {
        if (!dateTime.HasValue)
            return "-";

        int rocYear = dateTime.Value.Year - 1911;
        return $"{rocYear}/{dateTime.Value.Month:D2}/{dateTime.Value.Day:D2} {dateTime.Value.Hour:D2}:{dateTime.Value.Minute:D2}";
    }

    /// <summary>
    /// 變更紀錄項目類別
    /// </summary>
    public class ChangeRecordItem
    {
        public string Version { get; set; }
        public string ChangeDate { get; set; }
        public string ChangedBy { get; set; }
        public string ChangeReason { get; set; }
        public string BeforeChange { get; set; }
        public string AfterChange { get; set; }
    }
}
