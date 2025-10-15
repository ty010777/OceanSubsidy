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

    private void LoadProjectData()
    {
        try
        {
            // 使用 Helper 取得計畫基本資料
            string projectInfoHtml = OFS_PlanChangeRecordsHelper.GetProjectBasicData(ProjectID);
            litProjectInfo.Text = projectInfoHtml;
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入計畫資料時發生錯誤");
        }
    }

    private void LoadChangeRecords()
    {
        try
        {
            // 使用 Helper 取得變更紀錄
            var changeRecords = OFS_PlanChangeRecordsHelper.GetChangeRecordsList(ProjectID);

            if (changeRecords != null && changeRecords.Any())
            {
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

    protected void btnExportRecords_Click(object sender, EventArgs e)
    {
        try
        {
            // TODO: 實作匯出變更紀錄功能
            ShowErrorMessage("匯出功能尚未實作");
        }
        catch (Exception ex)
        {
            HandleException(ex, "匯出變更紀錄時發生錯誤");
        }
    }

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
}
