using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Operation.OFS;
using GS.App;
using GS.OCA_OceanSubsidy.Operation.OSI.OpenXml;

/// <summary>
/// 海洋科技專案計畫申請 - 上傳附件頁面
/// </summary>
public partial class OFS_SCI_SciUploadAttachments : System.Web.UI.Page
{
    /// <summary>
    /// 目前編輯的計畫ID
    /// </summary>

    protected string ProjectID => Request.QueryString["ProjectID"];
    

    /// <summary>
    /// 頁面載入事件
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            // 檢查是否有計畫ID
            if (string.IsNullOrEmpty(ProjectID))
            {
                Response.Redirect("~/OFS/ApplicationChecklist.aspx");
                return;
            }
            // 載入 UserControl 資料
            var master = (OFSApplicationMaster)this.Master;
            ucSciUploadAttachments.IsViewMode = master.IsViewMode;
            ucSciUploadAttachments.LoadData(ProjectID );

                 
                    
        }
    }
    

   
}