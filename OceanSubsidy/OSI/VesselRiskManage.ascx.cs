using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.App;
using GS.Data;
using GS.Extension;
using GS.OCA_OceanSubsidy.Entity;
using OfficeOpenXml;

public partial class OSI_VesselRiskManage : System.Web.UI.UserControl
{
    private SessionHelper.UserInfoClass UserInfo
    {
        get => SessionHelper.Get<SessionHelper.UserInfoClass>(
                  SessionHelper.UserInfo)
                ?? new SessionHelper.UserInfoClass();
        set => SessionHelper.Set(SessionHelper.UserInfo, value);
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindYears();
            BindVesselRiskData();
        }
    }

    // 載入年度下拉選單
    void BindYears()
    {
        ddlStartYear.Items.Clear();
        ddlEndYear.Items.Clear();

        // 取得資料庫中的年份範圍
        var yearData = OSIVesselRiskAssessmentsHelper.GetAllYears();
        
        int minYear = DateTime.Now.Year;
        int maxYear = DateTime.Now.Year;
        
        if (yearData != null && yearData.Rows.Count > 0)
        {
            var row = yearData.Rows[0];
            if (row["MinYear"] != DBNull.Value)
                minYear = Convert.ToInt32(row["MinYear"]);
            if (row["MaxYear"] != DBNull.Value)
                maxYear = Convert.ToInt32(row["MaxYear"]);
        }
        
        // 轉換為民國年
        int minYearROC = DateTimeHelper.GregorianYearToMinguo(minYear);
        int maxYearROC = DateTimeHelper.GregorianYearToMinguo(maxYear);
        int currentYearROC = DateTimeHelper.GregorianYearToMinguo(DateTime.Now.Year);

        // 建立年份選項（兩個下拉選單初始時有相同的選項）
        for (int year = minYearROC; year <= maxYearROC; year++)
        {
            string yearText = year + "年";
            ddlStartYear.Items.Add(new ListItem(yearText, year.ToString()));
            ddlEndYear.Items.Add(new ListItem(yearText, year.ToString()));
        }

        // 設定預設值
        if (ddlStartYear.Items.Count > 0)
        {
            // 如果今年在範圍內，預設選擇今年
            if (ddlStartYear.Items.FindByValue(currentYearROC.ToString()) != null)
            {
                ddlStartYear.SelectedValue = currentYearROC.ToString();
                ddlEndYear.SelectedValue = currentYearROC.ToString();
            }
            else
            {
                // 否則選擇第一個選項
                ddlStartYear.SelectedIndex = 0;
                ddlEndYear.SelectedIndex = 0;
            }
        }
    }

    // 起始年度變更事件
    protected void ddlStartYear_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(ddlStartYear.SelectedValue))
            return;

        int selectedStartYear = ddlStartYear.SelectedValue.toInt();
        string currentEndValue = ddlEndYear.SelectedValue;

        // 儲存所有原始選項
        List<ListItem> allItems = new List<ListItem>();
        foreach (ListItem item in ddlStartYear.Items)
        {
            allItems.Add(new ListItem(item.Text, item.Value));
        }

        // 重新綁定結束年度下拉選單，只顯示 >= 起始年度的選項
        ddlEndYear.Items.Clear();
        
        foreach (ListItem item in allItems)
        {
            int year = item.Value.toInt();
            if (year >= selectedStartYear)
            {
                ddlEndYear.Items.Add(new ListItem(item.Text, item.Value));
            }
        }

        // 設定結束年度的選擇
        if (ddlEndYear.Items.Count > 0)
        {
            // 如果原本的選擇仍在範圍內，保持選擇
            if (!string.IsNullOrEmpty(currentEndValue) && 
                ddlEndYear.Items.FindByValue(currentEndValue) != null)
            {
                ddlEndYear.SelectedValue = currentEndValue;
            }
            else
            {
                // 否則選擇第一個選項（即等於起始年度）
                ddlEndYear.SelectedIndex = 0;
            }
        }
    }

    // 查詢按鈕事件
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        // 重設分頁到第一頁
        dpVesselRisk.SetPageProperties(0, dpVesselRisk.PageSize, false);

        BindVesselRiskData();
    }

    // 綁定資料到 ListView
    private void BindVesselRiskData()
    {
        var tbl = OSIVesselRiskAssessmentsHelper.QueryWithFilter(
            ddlStartYear.SelectedValue.toInt(), 
            ddlEndYear.SelectedValue.toInt(),
            txtKeySearch.Text.Trim()
        );
        
        lvVesselRisk.DataSource = tbl;
        lvVesselRisk.DataBind();
    }

    protected void lvVesselRisk_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType != ListViewItemType.DataItem) return;

        var dataItem = (ListViewDataItem)e.Item;
        var drv = (DataRowView)dataItem.DataItem;
        var litProjectType = (Literal)dataItem.FindControl("litProjectType");
        
        // 設定計畫類別
        if (litProjectType != null && drv["AssessmentId"] != DBNull.Value)
        {
            int assessmentId = Convert.ToInt32(drv["AssessmentId"]);
            litProjectType.Text = OSIVesselRiskAssessmentCategoriesHelper.GetCategoryNamesByAssessmentId(assessmentId);
        }
    }

    protected void lvVesselRisk_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        string id = e.CommandArgument?.ToString();
        switch (e.CommandName)
        {
            case "EditRecord":
                int assessmentId = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"~/OSI/VesselRiskDetail.aspx?id={assessmentId}");
                break;
            case "AskDelete":
                hfDeleteID.Value = id;

                // 顯示刪除確認 Modal
                string modalId = deleteModal.ClientID;
                string script = $"showModal('{modalId}');";

                ScriptManager.RegisterStartupScript(
                    this.Page,
                    this.Page.GetType(),
                    "ASK_DELETE_MODAL",
                    script,
                    true
                );
                break;
        }
    }

    protected void lvVesselRisk_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
    {
        dpVesselRisk.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
        BindVesselRiskData();
    }

    protected void dpVesselRisk_PreRender(object sender, EventArgs e)
    {
        if (dpVesselRisk.Controls.Count < 2) return;

        var container = dpVesselRisk.Controls[1];
        foreach (Control c in container.Controls)
        {
            if (c is Button btn && btn.Text.Trim() == "...")
            {
                btn.Enabled = false;
                btn.CssClass = "pagination-item disabled";
            }
        }
    }

    // 新增按鈕事件
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/OSI/VesselRiskDetail.aspx");
    }

    // 確認刪除按鈕事件
    protected void btnConfirmDelete_Click(object sender, EventArgs e)
    {
        try
        {
            // 取得要刪除的 ID
            int assessmentId = hfDeleteID.Value.toInt();
            
            // 取得目前登入使用者的 ID
            int userId = UserInfo.UserID.toInt();
            
            // 執行假刪除
            bool success = OSIVesselRiskAssessmentsHelper.SoftDelete(assessmentId, userId);
            
            string modalId = deleteModal.ClientID;
            
            if (success)
            {
                // 刪除成功，關閉 Modal 並重新載入資料
                string script = $"hideModal('{modalId}');";
                ScriptManager.RegisterStartupScript(
                    this.Page,
                    this.Page.GetType(),
                    "DELETE_SUCCESS",
                    script,
                    true
                );
                
                // 重新綁定資料
                BindVesselRiskData();
            }
            else
            {
                // 刪除失敗
                string script = $"hideModal('{modalId}'); alert('刪除失敗，請稍後再試');";
                ScriptManager.RegisterStartupScript(
                    this.Page,
                    this.Page.GetType(),
                    "DELETE_FAILED",
                    script,
                    true
                );
            }
        }
        catch (Exception ex)
        {
            // 處理例外狀況
            string modalId = deleteModal.ClientID;
            string script = $"hideModal('{modalId}'); alert('發生錯誤：{ex.Message.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(
                this.Page,
                this.Page.GetType(),
                "DELETE_ERROR",
                script,
                true
            );
        }
    }
}