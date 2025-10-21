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

public partial class OFS_SCI_SnapshotView : System.Web.UI.Page
{
    #region 屬性

    /// <summary>
    /// 快照ID
    /// </summary>
    protected string SnapshotID => Request.QueryString["SnapshotID"];

    /// <summary>
    /// 計畫ID（用於相容性）
    /// </summary>
    protected string ProjectID => Request.QueryString["ProjectID"];

    #endregion

    #region 頁面事件

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // 檢查是否有快照ID或計畫ID
            if (string.IsNullOrEmpty(SnapshotID) && string.IsNullOrEmpty(ProjectID))
            {
                Response.Redirect("~/OFS/inprogressList.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // 初始化頁面（只在第一次載入時執行）
                InitializePage();
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
    /// 初始化頁面
    /// </summary>
    private void InitializePage()
    {
        try
        {
            Snapshot snapshot = null;

            // 根據參數載入快照資料
            if (!string.IsNullOrEmpty(SnapshotID))
            {
                // 從 SnapshotID 載入
                snapshot = OFSSnapshotHelper.GetByID(int.Parse(SnapshotID));
            }
            else if (!string.IsNullOrEmpty(ProjectID))
            {
                // 從 ProjectID 載入最新快照
                int projectMainID = GetProjectMainID(ProjectID);
                if (projectMainID > 0)
                {
                    snapshot = OFSSnapshotHelper.get("SCI", projectMainID);
                }
            }

            if (snapshot == null)
            {
                ShowAlert("錯誤", "找不到快照資料", "error");
                return;
            }

            // 顯示快照資訊
            LoadSnapshotInfo(snapshot);

            // 解析快照資料並載入到 UserControl
            LoadSnapshotDataToControls(snapshot.Data);
        }
        catch (Exception ex)
        {
            HandleException(ex, "初始化頁面時發生錯誤");
            ShowAlert("錯誤", "載入快照資料時發生錯誤", "error");
        }
    }

    /// <summary>
    /// 顯示快照基本資訊
    /// </summary>
    private void LoadSnapshotInfo(Snapshot snapshot)
    {
        try
        {
            // 解析快照資料以取得計畫名稱
            var snapshotData = JsonConvert.DeserializeObject<dynamic>(snapshot.Data);

            // 設定補助案類型
            litGrantType.Text = snapshot.Type;

            // 設定計畫資訊
            if (snapshotData.ApplicationMain != null && snapshotData.ApplicationMain.ProjectNameTw != null)
            {
                string projectName = snapshotData.ApplicationMain.ProjectNameTw.ToString();
                string projectID = snapshotData.ProjectMain?.ProjectID?.ToString() ?? "";
                litProjectInfo.Text = $"{projectID} - {projectName}";
            }

            // 設定快照時間
            litSnapshotTime.Text = snapshot.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入快照資訊時發生錯誤");
        }
    }

    /// <summary>
    /// 載入快照資料到各個 UserControl
    /// </summary>
    private void LoadSnapshotDataToControls(string jsonData)
    {
        try
        {
            if (string.IsNullOrEmpty(jsonData))
            {
                return;
            }

            // 解析 JSON 資料
            var snapshotData = JsonConvert.DeserializeObject<dynamic>(jsonData);

            if (snapshotData == null)
            {
                throw new Exception("快照資料解析失敗");
            }

            // 呼叫各個 UserControl 的 LoadFromSnapshot 方法
            // 第一頁：申請表/聲明書
            ucSciApplication.LoadFromSnapshot(snapshotData);

            // 第二頁：期程／工作項目／查核
            ucSciWorkSch.LoadFromSnapshot(snapshotData);

            // 第三頁：經費／人事
            ucSciFunding.LoadFromSnapshot(snapshotData);

            // 第四頁：其他
            ucSciRecusedList.LoadFromSnapshot(snapshotData);

            // 第五頁：上傳附件/提送申請
            ucSciUploadAttachments.LoadFromSnapshot(snapshotData);
        }
        catch (Exception ex)
        {
            HandleException(ex, "載入快照資料時發生錯誤");
            throw;
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
    /// 例外處理
    /// </summary>
    private void HandleException(Exception ex, string context)
    {
        System.Diagnostics.Debug.WriteLine($"{context}: {ex.Message}");
        System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
    }

    #endregion
}
