using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class OSI_VesselRiskDetail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // 檢查是否有 id 參數（編輯模式）
            string assessmentId = Request.QueryString["id"];
            if (!string.IsNullOrEmpty(assessmentId))
            {
                // 傳遞 ID 給 VesselRiskForm 控件
                VesselRiskForm.AssessmentId = Convert.ToInt32(assessmentId);
            }

            // 檢查是否從儲存後重新導向過
            if (Request.QueryString["saved"] == "1")
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    this.GetType(),
                    "saveSuccess",
                    "showGlobalMessage('儲存成功');",
                    true
                );
            }
        }
    }
}