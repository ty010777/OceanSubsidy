using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Operation.OFS;
using GS.App;

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
                // 載入 UserControl 資料
                ucSciApplication.LoadData(ProjectID);
                
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