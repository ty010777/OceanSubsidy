using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class OFS_CLB_ClbApproved : System.Web.UI.Page
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
                master.ProjectInfoText = $"{ProjectID}/核定計畫";
            }
            
            // 載入計畫資料
            LoadProjectData();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"初始化核定計畫頁面時發生錯誤: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 載入計畫資料
    /// </summary>
    private void LoadProjectData()
    {
        try
        {
            if (string.IsNullOrEmpty(ProjectID))
            {
                return;
            }
            
            // TODO: 實作載入 CLB 計畫資料的邏輯
            // 這裡先留空，等後續實作具體功能時再填入
            
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"載入計畫資料時發生錯誤: {ex.Message}");
        }
    }
}