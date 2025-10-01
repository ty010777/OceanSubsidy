using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Operation.OFS;

/// <summary>
/// 科專計畫工作排程頁面
/// 提供期程/工作項目/查核標準的填寫、儲存功能
/// </summary>
public partial class OFS_SciWorkSch : System.Web.UI.Page
{
    private string ProjectID => Request.QueryString["ProjectID"];
    
    #region 頁面生命週期
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // 設定顯示模式
            var master = (OFSApplicationMaster)this.Master;
            sciWorkSchControl.IsViewMode = master.IsViewMode;

            if (!IsPostBack)
            {
                sciWorkSchControl.LoadData(ProjectID);
                
            }
        }
        catch (Exception ex)
        {
            Response.Write($"<script>alert('頁面載入錯誤：{ex.Message}');</script>");
        }
    }

    #endregion

    


    #region 顯示模式控制

    
    /// <summary>
    /// 判斷是否應該顯示為編輯模式
    /// </summary>
    /// <returns>true: 編輯模式, false: 檢視模式</returns>
    private bool ShouldShowInEditMode()
    {
        // 如果沒有 ProjectID，是新申請案件，可以編輯
        if (string.IsNullOrEmpty(ProjectID))
        {
            return true;
        }
        
        try
        {
            // 取得最新版本的狀態
            var ApplicationData = OFS_SciApplicationHelper.getVersionByProjectID(ProjectID);
            if (ApplicationData == null)
            {
                return true; // 沒有資料時允許編輯
            }
            
            // 只有這些狀態可以編輯
            string statuses = ApplicationData.Statuses ?? "";
            string statusesName = ApplicationData.StatusesName ?? "";
            
            return statuses == "尚未提送" || 
                   statusesName == "補正補件" || 
                   statusesName == "計畫書修正中";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取得申請狀態時發生錯誤：{ex.Message}");
            return false; // 發生錯誤時預設為檢視模式
        }
    }

   

    #endregion
}