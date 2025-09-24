using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;

public partial class OFS_CLB_ClbApplication : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // 初始化頁面
            InitializePage();
        }
    }

    private void InitializePage()
    {
        // 檢查 URL 參數
        string projectID = Request.QueryString["ProjectID"];

        // 將 ProjectID 傳遞給 UserControl，讓它自己處理載入邏輯
        ucClbApplication.ProjectID = projectID ?? "";

        // 判斷並設定檢視模式
        SetViewModeBasedOnStatus(projectID);


    }

  

    /// <summary>
    /// 根據專案狀態設定檢視模式
    /// </summary>
    /// <param name="projectID">計畫編號</param>
    private void SetViewModeBasedOnStatus(string projectID)
    {
        try
        {
            // 如果 projectID 為空或找不到，顯示編輯模式
            if (string.IsNullOrEmpty(projectID))
            {
                ucClbApplication.IsReadOnly = false;
            }
            else
            {
                // 從資料庫取得專案狀態資訊
                var projectMain = OFS_ClbApplicationHelper.GetProjectMainData(projectID);
                
                if (projectMain == null)
                {
                    // 找不到專案資料，顯示編輯模式
                    ucClbApplication.IsReadOnly = false;
                }
                else
                {
                    string statuses = projectMain.Statuses ?? "";
                    string statusesName = projectMain.StatusesName ?? "";

                    // 判斷狀態是否允許編輯
                    if ((statuses == "尚未提送" && statusesName == "編輯中") ||
                        (statuses == "決審核定" && statusesName == "計畫書修正中") ||
                        (statuses == "內容審查" && statusesName == "補正補件"))
                    {
                        // 可編輯狀態
                        ucClbApplication.IsReadOnly = false;
                    }
                    else
                    {
                        // 其他狀態都是檢視模式
                        ucClbApplication.IsReadOnly = true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // 發生錯誤時，預設為編輯模式
            ucClbApplication.IsReadOnly = false;
            System.Diagnostics.Debug.WriteLine($"設定檢視模式時發生錯誤: {ex.Message}");
        }
    }


}