using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Operation.OFS;
using GS.OCA_OceanSubsidy.Model.OFS;
using GS.App;
using Newtonsoft.Json;

/// <summary>
/// 科專計畫申請表頁面 - 重構版本
/// 使用 UserControl 來提供申請表功能
/// </summary>
public partial class OFS_SciApplication : System.Web.UI.Page
{
    #region 屬性

    /// <summary>
    /// 目前編輯的計畫ID
    /// </summary>
    protected string ProjectID => Request.QueryString["ProjectID"];

    #endregion

    #region 頁面事件

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            var master = (OFSApplicationMaster)this.Master;
            ucSciApplication.IsViewMode = master.IsViewMode;

            if (!IsPostBack)
            {
                // 取得專案狀態
                    var projectMain = OFS_SciApplicationHelper.getVersionByProjectID(ProjectID);

                // 判斷是否為「計畫執行」狀態
                if (projectMain != null && projectMain.Statuses == "計畫執行")
                {
                    // 載入快照資料
                    LoadSnapshotData(ProjectID);
                }
                else
                {
                    // 載入正常資料
                    ucSciApplication.LoadData(ProjectID);
                }
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "頁面載入時發生錯誤");
        }
    }

    #endregion



    #region 私有方法

    /// <summary>
    /// 載入快照資料
    /// </summary>
    private void LoadSnapshotData(string projectID)
    {
        try
        {
            // 取得 Project_Main 的 ID
            int projectMainID = GetProjectMainID(projectID);
            if (projectMainID == 0)
            {
                ShowAlertAndRedirect("錯誤", "找不到計畫資料", "error");
                return;
            }

            // 從 ProjectID 載入最新快照 (Status = 11 表示計畫執行階段的快照)
            Snapshot snapshot = OFSSnapshotHelper.get("SCI", projectMainID, 11);

            if (snapshot == null)
            {
                ShowAlertAndRedirect("錯誤", "找不到快照資料，將返回申請列表", "error");
                return;
            }

            // 解析快照資料並載入到 UserControl
            var snapshotData = JsonConvert.DeserializeObject<dynamic>(snapshot.Data);
            if (snapshotData != null)
            {
                ucSciApplication.LoadFromSnapshot(snapshotData);
            }
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入快照資料時發生錯誤");
            ShowAlertAndRedirect("錯誤", "載入快照資料時發生錯誤，將返回申請列表", "error");
        }
    }

    /// <summary>
    /// 取得 Project_Main 的 ID
    /// </summary>
    private int GetProjectMainID(string projectID)
    {
        try
        {
            var projectMain = OFS_SciApplicationHelper.getVersionByProjectID(projectID);
            return projectMain?.ID ?? 0;
        }
        catch (Exception ex)
        {
            HandleException(ex, "取得 ProjectMainID 時發生錯誤");
            return 0;
        }
    }

    /// <summary>
    /// 顯示 SweetAlert 提示訊息
    /// </summary>
    private void ShowAlert(string title, string text, string icon)
    {
        try
        {
            string script = $@"
                Swal.fire({{
                    title: '{title}',
                    text: '{text}',
                    icon: '{icon}',
                    confirmButtonText: '確定'
                }});
            ";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowAlert", script, true);
        }
        catch (Exception ex)
        {
            HandleException(ex, "顯示提示訊息時發生錯誤");
        }
    }

    /// <summary>
    /// 顯示 SweetAlert 提示訊息並重新導向到申請列表
    /// </summary>
    private void ShowAlertAndRedirect(string title, string text, string icon)
    {
        try
        {
            // 取得虛擬路徑
            string appRootPath = System.Configuration.ConfigurationManager.AppSettings["AppRootPath"] ?? "";

            string script = $@"
                window.AppRootPath = '{appRootPath}';
                Swal.fire({{
                    title: '{title}',
                    text: '{text}',
                    icon: '{icon}',
                    confirmButtonText: '確定'
                }}).then((result) => {{
                    // 使用者點擊確定後，重新導向到申請列表
                    window.location.href = window.AppRootPath + '/OFS/ApplicationChecklist.aspx';
                }});
            ";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowAlertAndRedirect", script, true);
        }
        catch (Exception ex)
        {
            HandleException(ex, "顯示提示訊息時發生錯誤");
            // 如果顯示訊息失敗，直接重新導向
            Response.Redirect("~/OFS/ApplicationChecklist.aspx");
        }
    }

    /// <summary>
    /// 例外處理
    /// </summary>
    /// <param name="ex">例外物件</param>
    /// <param name="context">發生錯誤的上下文</param>
    private void HandleException(Exception ex, string context)
    {
        // 記錄錯誤
        System.Diagnostics.Debug.WriteLine($"{context}: {ex.Message}");

        // 可以在這裡加入更多的錯誤處理邏輯
        // 例如：記錄到日誌檔案、發送錯誤通知等
    }

    #endregion
}