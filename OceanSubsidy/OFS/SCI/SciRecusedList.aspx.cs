using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.OCA_OceanSubsidy.Entity;
using GS.OCA_OceanSubsidy.Entity.Base;
using GS.OCA_OceanSubsidy.Operation.OFS;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.X509.SigI;
using Newtonsoft.Json;

public partial class OFS_SciAvoidList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // 設定顯示模式
            
            if (!IsPostBack)
            {
                // 載入 UserControl 資料
                string projectId = Request.QueryString["ProjectID"];
                // 載入 UserControl 資料
                var master = (OFSApplicationMaster)this.Master;
                sciRecusedListControl.IsViewMode = master.IsViewMode;
                if (!string.IsNullOrEmpty(projectId))
                {
                 
                    sciRecusedListControl.LoadData(projectId );
                }
            }
        }
        catch (Exception ex)
        {
            // 發生錯誤時記錄但不中斷頁面載入
            System.Diagnostics.Debug.WriteLine($"頁面載入錯誤：{ex.Message}");
        }
    }
}