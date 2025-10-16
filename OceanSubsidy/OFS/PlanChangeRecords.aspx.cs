using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using GS.Extension;
using GS.App;
using GS.Data;

public partial class OFS_PlanChangeRecords : System.Web.UI.Page
{
    protected string ProjectID { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                InitializePageData();
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "頁面載入時發生錯誤");
        }
    }

    private void InitializePageData()
    {
        // 從 URL 參數取得 ProjectID
        ProjectID = Request.QueryString["ProjectID"] ?? "";

        if (string.IsNullOrEmpty(ProjectID))
        {
            ShowErrorMessage("未指定計畫編號");
            return;
        }

        // 載入計畫基本資料
        LoadProjectData();

        // 載入計畫變更紀錄
        LoadChangeRecords();
    }

    #region 計畫基本資料

    private void LoadProjectData()
    {
        try
        {
            string projectType = GetProjectType(ProjectID);
            string projectInfoHtml = GetProjectBasicData(ProjectID, projectType);
            litProjectInfo.Text = projectInfoHtml;
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入計畫資料時發生錯誤");
        }
    }

    /// <summary>
    /// 根據 ProjectID 判斷計畫類型
    /// </summary>
    private string GetProjectType(string projectID)
    {
        if (string.IsNullOrEmpty(projectID))
            return string.Empty;

        // ProjectID 格式: 類型代碼 + 年度 + 序號
        // 例如: SCI2024001, CUL1140001, CLB1140001
        if (projectID.Contains("SCI")) return "SCI";
        if (projectID.Contains("CUL")) return "CUL";
        if (projectID.Contains("EDC")) return "EDC";
        if (projectID.Contains("CLB")) return "CLB";
        if (projectID.Contains("ACC")) return "ACC";
        if (projectID.Contains("MUL")) return "MUL";
        if (projectID.Contains("LIT")) return "LIT";

        return string.Empty;
    }

    /// <summary>
    /// 取得計畫基本資料
    /// </summary>
    private string GetProjectBasicData(string projectID, string projectType)
    {
        if (string.IsNullOrEmpty(projectID))
            return string.Empty;

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

    private string GetSCIProjectData(string projectID)
    {
        var project = OFS_SciApplicationHelper.getApplicationMainByProjectID(projectID);
        if (project == null)
            return BuildErrorHtml(projectID, "找不到計畫資料");

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

    private string GetCULProjectData(string projectID)
    {
        int id = OFS_CulProjectHelper.getID(projectID);
        if (id == 0)
            return BuildErrorHtml(projectID, "找不到計畫資料");

        var project = OFS_CulProjectHelper.get(id);
        if (project == null)
            return BuildErrorHtml(projectID, "找不到計畫資料");

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

    private string GetEDCProjectData(string projectID)
    {
        int id = OFS_EdcProjectHelper.getID(projectID);
        if (id == 0)
            return BuildErrorHtml(projectID, "找不到計畫資料");

        var project = OFS_EdcProjectHelper.get(id);
        if (project == null)
            return BuildErrorHtml(projectID, "找不到計畫資料");

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

    private string GetCLBProjectData(string projectID)
    {
        var project = OFS_ClbApplicationHelper.GetBasicData(projectID);
        if (project == null)
            return BuildErrorHtml(projectID, "找不到計畫資料");

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

    private string GetACCProjectData(string projectID)
    {
        int id = OFS_AccProjectHelper.getID(projectID);
        if (id == 0)
            return BuildErrorHtml(projectID, "找不到計畫資料");

        var project = OFS_AccProjectHelper.get(id);
        if (project == null)
            return BuildErrorHtml(projectID, "找不到計畫資料");

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

    private string GetMULProjectData(string projectID)
    {
        int id = OFS_MulProjectHelper.getID(projectID);
        if (id == 0)
            return BuildErrorHtml(projectID, "找不到計畫資料");

        var project = OFS_MulProjectHelper.get(id);
        if (project == null)
            return BuildErrorHtml(projectID, "找不到計畫資料");

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

    private string GetLITProjectData(string projectID)
    {
        int id = OFS_LitProjectHelper.getID(projectID);
        if (id == 0)
            return BuildErrorHtml(projectID, "找不到計畫資料");

        var project = OFS_LitProjectHelper.get(id);
        if (project == null)
            return BuildErrorHtml(projectID, "找不到計畫資料");

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

    private string BuildErrorHtml(string projectID, string errorMessage)
    {
        return $"<li><span class=\"text-gray fw-bold\">計畫編號：</span><span>{projectID}</span></li>" +
               $"<li><span class=\"text-gray fw-bold\">錯誤：</span><span class=\"text-danger\">{errorMessage}</span></li>";
    }

    #endregion

    #region 計畫變更紀錄

    private void LoadChangeRecords()
    {
        try
        {
            string projectType = GetProjectType(ProjectID);
            if (string.IsNullOrEmpty(projectType))
            {
                pnlNoData.Visible = true;
                return;
            }

            // 從 Helper 取得原始資料
            DataTable table = OFS_PlanChangeRecordsHelper.GetChangeRecords(ProjectID);

            if (table.Rows.Count > 0)
            {
                var changeRecords = BuildChangeRecordsList(table);
                rptChangeRecords.DataSource = changeRecords;
                rptChangeRecords.DataBind();
                pnlNoData.Visible = false;
            }
            else
            {
                pnlNoData.Visible = true;
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入變更紀錄時發生錯誤");
        }
    }

    /// <summary>
    /// 建立變更紀錄清單
    /// </summary>
    private List<ChangeRecordItem> BuildChangeRecordsList(DataTable table)
    {
        var records = new List<ChangeRecordItem>();

        // 分組計算版次：先依 Method 分組
        int method2Count = 0;
        int method1Count = 0;

        foreach (DataRow row in table.Rows)
        {
            // 安全地取得欄位值
            int method = row["Method"] != DBNull.Value ? Convert.ToInt32(row["Method"]) : 0;
            DateTime? createTime = row["CreateTime"] != DBNull.Value ? Convert.ToDateTime(row["CreateTime"]) : (DateTime?)null;
            int? createUser = row["CreateUser"] != DBNull.Value ? Convert.ToInt32(row["CreateUser"]) : (int?)null;
            string reason = row["Reason"] != DBNull.Value ? row["Reason"].ToString() : "";

            // 計算版次
            string versionText = "";
            if (method == 2)
            {
                method2Count++;
                versionText = $"第{method2Count}次\n修正計畫書";
            }
            else if (method == 1)
            {
                method1Count++;
                versionText = $"第{method1Count}次\n計畫變更";
            }
            else
            {
                versionText = "未知類型";
            }

            var record = new ChangeRecordItem
            {
                Version = versionText,
                ChangeDate = FormatDateTime(createTime),
                ChangedBy = GetUserName(createUser),
                ChangeReason = reason,
                BeforeChange = BuildChangeContent(row, "Before"),
                AfterChange = BuildChangeContent(row, "After")
            };

            records.Add(record);
        }

        return records;
    }

    /// <summary>
    /// 建立變更內容字串
    /// </summary>
    private string BuildChangeContent(DataRow row, string suffix)
    {
        var sb = new StringBuilder();
        string projectType = GetProjectType(ProjectID);

        // 取得表單名稱對應
        var formNames = GetFormNames(projectType);

        for (int i = 1; i <= 5; i++)
        {
            string fieldName = $"Form{i}{suffix}";

            // 安全地取得欄位值
            string content = row[fieldName] != DBNull.Value ? row[fieldName].ToString() : "";

            if (!string.IsNullOrEmpty(content))
            {
                // 使用表單名稱作為標題
                string formTitle = formNames.ContainsKey(i) ? formNames[i] : $"表單{i}";

                sb.AppendLine($"【{formTitle}】");

                // 分行處理變更內容
                var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    sb.AppendLine(line);
                }
                sb.AppendLine(); // 空行分隔
            }
        }

        return sb.Length > 0 ? sb.ToString().TrimEnd() : "-";
    }

    /// <summary>
    /// 根據計畫類型取得表單名稱對應
    /// </summary>
    private Dictionary<int, string> GetFormNames(string projectType)
    {
        switch (projectType)
        {
            case "SCI": // 科專
                return new Dictionary<int, string>
                {
                    { 1, "申請表/聲明書" },
                    { 2, "期程／工作項目／查核" },
                    { 3, "經費／人事" },
                    { 4, "其他" },
                    { 5, "上傳附件/提送申請" }
                };

            case "CUL": // 文化
                return new Dictionary<int, string>
                {
                    { 1, "申請表／計畫書" },
                    { 2, "期程／工作項目／查核" },
                    { 3, "經費" },
                    { 4, "其他" },
                    { 5, "上傳附件／提送申請" }
                };

            case "EDC": // 學校民間
                return new Dictionary<int, string>
                {
                    { 1, "申請表／計畫書" },
                    { 5, "上傳附件／提送申請" }
                };

            case "CLB": // 社團
                return new Dictionary<int, string>
                {
                    { 1, "申請表" },
                    { 2, "上傳附件/提送申請" }
                };

            case "MUL": // 多元
                return new Dictionary<int, string>
                {
                    { 1, "申請書／計畫書" },
                    { 2, "期程／工作項目／查核" },
                    { 3, "經費" },
                    { 4, "成果效益" },
                    { 5, "上傳附件／提送申請" }
                };

            case "LIT": // 素養
                return new Dictionary<int, string>
                {
                    { 1, "申請書／計畫書" },
                    { 2, "期程／工作項目／查核" },
                    { 3, "經費" },
                    { 4, "成果效益" },
                    { 5, "上傳附件／提送申請" }
                };

            case "ACC": // 無障礙
                return new Dictionary<int, string>
                {
                    { 1, "申請書／計畫書" },
                    { 2, "期程／工作項目／查核" },
                    { 3, "經費" },
                    { 4, "成果效益" },
                    { 5, "上傳附件／提送申請" }
                };

            default:
                return new Dictionary<int, string>
                {
                    { 1, "表單1" },
                    { 2, "表單2" },
                    { 3, "表單3" },
                    { 4, "表單4" },
                    { 5, "表單5" }
                };
        }
    }

    /// <summary>
    /// 取得使用者名稱
    /// </summary>
    private string GetUserName(int? userId)
    {
        if (!userId.HasValue)
            return "-";

        try
        {
            string userName = OFS_PlanChangeRecordsHelper.GetUserName(userId.Value);
            return string.IsNullOrEmpty(userName) ? "-" : userName;
        }
        catch
        {
            return "-";
        }
    }

    #endregion

    #region 工具方法

    /// <summary>
    /// 格式化日期 (民國年)
    /// </summary>
    private string FormatDate(DateTime date)
    {
        int rocYear = date.Year - 1911;
        return $"{rocYear}/{date.Month:D2}/{date.Day:D2}";
    }

    /// <summary>
    /// 格式化日期時間 (民國年)
    /// </summary>
    private string FormatDateTime(DateTime? dateTime)
    {
        if (!dateTime.HasValue)
            return "-";
        
        return DateTimeHelper.ToMinguoDate(dateTime.Value);
    }

    #endregion

    #region 事件處理

    // 事件處理方法

    #endregion

    #region 訊息處理

    private void ShowSuccessMessage(string message)
    {
        string script = $@"
            Swal.fire({{
                icon: 'success',
                title: '成功',
                text: '{message}',
                confirmButtonText: '確定'
            }});";
        ClientScript.RegisterStartupScript(this.GetType(), "SuccessMessage", script, true);
    }

    private void ShowErrorMessage(string message)
    {
        string script = $@"
            Swal.fire({{
                icon: 'error',
                title: '錯誤',
                text: '{message}',
                confirmButtonText: '確定'
            }});";
        ClientScript.RegisterStartupScript(this.GetType(), "ErrorMessage", script, true);
    }

    private void HandleException(Exception ex, string message)
    {
        System.Diagnostics.Debug.WriteLine($"{message}: {ex.Message}");
        System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
        ShowErrorMessage($"{message}: {ex.Message}");
    }

    #endregion

    #region Model 類別

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

    #endregion
}
