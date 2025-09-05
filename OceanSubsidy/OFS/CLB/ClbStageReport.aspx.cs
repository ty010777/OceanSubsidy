using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class OFS_CLB_ClbStageReport : System.Web.UI.Page
{
    /// <summary>
    /// 目前處理的計畫ID
    /// </summary>
    protected string ProjectID => Request.QueryString["ProjectID"];

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            InitializePage();
        }
    }
    
    /// <summary>
    /// 初始化頁面
    /// </summary>
    private void InitializePage()
    {
        try
        {
            // 設定 Master Page 的計畫資訊
            var master = Master as OFS_CLB_ClbInprogress;
            if (master != null && !string.IsNullOrEmpty(ProjectID))
            {
                master.ProjectInfoText = $"{ProjectID}/階段報告";
            }
            
            // 載入報告資料
            LoadReportData();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"初始化階段報告頁面時發生錯誤: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 載入報告資料
    /// </summary>
    private void LoadReportData()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return;
            }
            
            // TODO: 實作載入 CLB 階段報告資料的邏輯
            // 這裡先留空，等後續實作具體功能時再填入
            
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"載入報告資料時發生錯誤: {ex.Message}");
        }
    }
}