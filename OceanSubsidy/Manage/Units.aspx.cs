using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GS.Data;
using GS.Extension;
using GS.OCA_OceanSubsidy.Entity;

public partial class Manage_Units : System.Web.UI.Page
{
    private Dictionary<int, string> unitDisplayNumbers = new Dictionary<int, string>();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindGovUnitType();
            BindSearchGovUnitType();
            BindList();
        }

    }

    void BindGovUnitType()
    {
        var dt = SysGovUnitTypeHelper.QueryAll();
        rblGovUnitType.DataSource = dt;
        rblGovUnitType.DataTextField = "TypeName";
        rblGovUnitType.DataValueField = "TypeID";
        rblGovUnitType.DataBind();

        // 預設選中第一筆
        if (rblGovUnitType.Items.Count > 0)
        {
            rblGovUnitType.SelectedIndex = 0;
        }
    }

    void BindSearchGovUnitType()
    {
        var dt = SysGovUnitTypeHelper.QueryAll();
        rblSearchGovUnitType.DataSource = dt;
        rblSearchGovUnitType.DataTextField = "TypeName";
        rblSearchGovUnitType.DataValueField = "TypeID";
        rblSearchGovUnitType.DataBind();

        // 預設選中第一筆
        if (rblSearchGovUnitType.Items.Count > 0)
        {
            rblSearchGovUnitType.SelectedIndex = 0;
        }
    }

    protected void rblGovUnitType_SelectedIndexChanged(object s, EventArgs e)
    {
        // 根據選中的政府機關類別載入單位
        if (!string.IsNullOrEmpty(rblGovUnitType.SelectedValue))
        {
            LoadModalParents(Convert.ToInt32(rblGovUnitType.SelectedValue));
        }
        else
        {
            LoadModalParents();
        }
    }

    protected void btnSearchUnit_Click(object sender, EventArgs e)
    {
        BindList();
    }


    void BindList()
    {
        DataTable dt;

        // 根據選擇的政府機關類別查詢
        if (!string.IsNullOrEmpty(rblSearchGovUnitType.SelectedValue))
        {
            int govUnitTypeID = Convert.ToInt32(rblSearchGovUnitType.SelectedValue);
            dt = SysUnitHelper.QueryByGovUnitTypeID(govUnitTypeID, true);
        }
        else
        {
            dt = SysUnitHelper.QueryAllOrderByUnitID();
        }

        // 排除 UnitName="其他" 的資料 (使用 LINQ)
        if (dt.Rows.Count > 0)
        {
            var filteredDt = dt.AsEnumerable()
                .Where(row => row.Field<string>("UnitName") != "其他")
                .CopyToDataTable();

            // 計算排序號碼
            CalculateDisplayNumbers(filteredDt);

            lvUnits.DataSource = filteredDt;
            lvUnits.DataBind();
        }
        else
        {
            // 如果沒有資料，清空列表
            lvUnits.DataSource = null;
            lvUnits.DataBind();
        }
    }

    private void CalculateDisplayNumbers(DataTable dt)
    {
        unitDisplayNumbers.Clear();
        Dictionary<int, int> parentCounts = new Dictionary<int, int>();
        Dictionary<int, int> childCounts = new Dictionary<int, int>();
        int parentOrder = 0;

        foreach (DataRow row in dt.Rows)
        {
            int unitId = Convert.ToInt32(row["UnitID"]);
            object parentUnitIdObj = row["ParentUnitID"];

            if (parentUnitIdObj == DBNull.Value || parentUnitIdObj == null)
            {
                // 父單位
                parentOrder++;
                parentCounts[unitId] = parentOrder;
                unitDisplayNumbers[unitId] = parentOrder.ToString();
            }
            else
            {
                // 子單位
                int parentUnitId = Convert.ToInt32(parentUnitIdObj);
                if (!childCounts.ContainsKey(parentUnitId))
                {
                    childCounts[parentUnitId] = 0;
                }
                childCounts[parentUnitId]++;

                if (parentCounts.ContainsKey(parentUnitId))
                {
                    unitDisplayNumbers[unitId] = $"{parentCounts[parentUnitId]}-{childCounts[parentUnitId]}";
                }
                else
                {
                    unitDisplayNumbers[unitId] = $"?-{childCounts[parentUnitId]}";
                }
            }
        }
    }

    public string GetDisplayNumber(object unitId)
    {
        if (unitId == null || unitId == DBNull.Value) return "";

        int id = Convert.ToInt32(unitId);
        return unitDisplayNumbers.ContainsKey(id) ? unitDisplayNumbers[id] : "";
    }

    protected void btnAdd_Click(object s, EventArgs e)
    {
        // 準備新增
        hfUnitID.Value = "";

        // 根據選中的政府機關類別載入單位
        if (!string.IsNullOrEmpty(rblGovUnitType.SelectedValue))
        {
            LoadModalParents(Convert.ToInt32(rblGovUnitType.SelectedValue));
        }
        else
        {
            LoadModalParents();
        }

        txtModalName.Text = "";

        // 顯示 Modal
        ScriptManager.RegisterStartupScript(
            this, GetType(), "showUnit",
            "showModal('unitModal');", true);
    }

    // ListView 的編輯/刪除按鈕
    protected void lvUnits_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        string id = e.CommandArgument.ToString();
        if (e.CommandName == "EditUnit")
        {
            var dt = SysUnitHelper.QueryByID(id);
            var dr = dt.Rows[0];

            hfUnitID.Value = dr["UnitID"].ToString();

            // 設定政府機關類別
            string govUnitTypeID = dr["GovUnitTypeID"]?.ToString() ?? "";
            if (!string.IsNullOrEmpty(govUnitTypeID) && rblGovUnitType.Items.FindByValue(govUnitTypeID) != null)
            {
                rblGovUnitType.SelectedValue = govUnitTypeID;
                LoadModalParents(Convert.ToInt32(govUnitTypeID));
            }
            else
            {
                LoadModalParents();
            }

            string p = dr["ParentUnitID"]?.ToString() ?? "";
            if (ddlModalParent.Items.FindByValue(p) != null)
                ddlModalParent.SelectedValue = p;
            else
                ddlModalParent.SelectedIndex = 0;

            txtModalName.Text = dr["UnitName"].ToString();

            ScriptManager.RegisterStartupScript(
                this, GetType(), "showUnit",
                "showModal('unitModal');", true);
        }
        else if (e.CommandName == "DeleteUnit")
        {
            hfDeleteID.Value = id;
            if (!IsValidDelete(id))
            {
                ((Manage_ManageMaster)Master)
                  .ShowMessageModal("該單位存在下層，無法刪除。");
                return;
            }
            ScriptManager.RegisterStartupScript(
                this, GetType(), "showDel",
                "showModal('deleteModal');", true);
        }
    }

    protected void lvUnits_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType != ListViewItemType.DataItem) return;

        var drv = (DataRowView)e.Item.DataItem;
        string id = drv["UnitID"].ToString();
        int unitId = Convert.ToInt32(id);

        var lb = (LinkButton)e.Item.FindControl("lkDeleteUnit");
        lb.Visible = IsValidDelete(id);

        // 設定排序號碼
        var displayNumberLabel = e.Item.FindControl("lblDisplayNumber") as Label;
        if (displayNumberLabel != null && unitDisplayNumbers.ContainsKey(unitId))
        {
            displayNumberLabel.Text = unitDisplayNumbers[unitId];
        }
    }

    private bool IsValidDelete(string id)
    {
        // 先查詢該單位是否有下層單位
        var tbl = SysUnitHelper.QueryByParentUnitID(id);
        if (tbl.Rows.Count > 0)
            return false;

        // 查詢該單位是否有使用者
        tbl = SysUserHelper.QueryUserByUnitID(id);
        if (tbl.Rows.Count > 0)
            return false;

        // 查詢該單位是否有活動報告
        if (OSIActivityReportsHelper.HasReportsByUnitID(id))
            return false;

        return true;
    }

    protected void lvUnits_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
    {
        dpUnits.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
        BindList();
    }

    protected void dpUnits_PreRender(object sender, EventArgs e)
    {
        if (dpUnits.Controls.Count < 2) return;

        var container = dpUnits.Controls[1];
        foreach (Control c in container.Controls)
        {
            if (c is Button btn && btn.Text.Trim() == "...")
            {
                btn.CssClass = "pagination-item ellipsis";
            }
        }
    }

    // 共用：載入下拉
    private void LoadModalParents(int? govUnitTypeID = null)
    {
        GisTable dt;
        if (govUnitTypeID.HasValue)
        {
            dt = SysUnitHelper.QueryByGovUnitTypeID(govUnitTypeID.Value);
        }
        else
        {
            dt = SysUnitHelper.QueryAllOrderByUnitID();
        }

        ddlModalParent.Items.Clear();
        ddlModalParent.Items.Add(new ListItem("請選擇", ""));  // 預設選項，Value 空字串

        // 篩選出 ParentUnitID 為 null 的記錄（非子單位的單位）
        foreach (DataRow row in dt.Rows)
        {
            if (row["ParentUnitID"] == DBNull.Value || row["ParentUnitID"] == null)
            {
                ddlModalParent.Items.Add(new ListItem(
                    row["UnitName"].ToString(),
                    row["UnitID"].ToString()
                ));
            }
        }
    }

    protected void cvUnitName_ServerValidate(object source, ServerValidateEventArgs args)
    {
        string newName = args.Value.Trim();
        args.IsValid = true;

        GisTable sysUnit = SysUnitHelper.QueryAllOrderByUnitID();
        for (int i = 0; i <= sysUnit.Rows.Count - 1; i++)
        {
            if (newName == sysUnit.Rows[i]["UnitName"].ToString())
            {
                args.IsValid = false;
                break;
            }
        }
    }

    protected void btnSave_Click(object s, EventArgs e)
    {
        Page.Validate("UnitModal");
        if (!Page.IsValid)
            return;

        string id = hfUnitID.Value;
        int? pid;
        var pidVal = ddlModalParent.SelectedValue;
        if (string.IsNullOrEmpty(pidVal))
            pid = null;
        else
            pid = Convert.ToInt32(pidVal);

        string name = txtModalName.Text.Trim();

        Sys_Unit unit = new Sys_Unit
        {
            ParentUnitID = pid,
            GovUnitTypeID = Convert.ToInt32(rblGovUnitType.SelectedValue),
            UnitName = name
        };

        bool ok = true;
        string modalMsg = "";
        if (string.IsNullOrEmpty(id))
        {
            Page.Validate("AddUnitModal");
            if (!Page.IsValid)
                return;

            // 新增
            ok = SysUnitHelper.InsertSysUser(unit);

            if (ok)
            {
                modalMsg = "資料已新增。";
            }
            else
            {
                modalMsg = "資料新增失敗。";
            }
        }
        else
        {
            // 編輯
            var orgUnit = SysUnitHelper.QueryByID(id);
            unit.UnitID = id.toInt();
            ok = SysUnitHelper.UpdateByID(unit);
            if (ok)
            {
                modalMsg = "資料已更新。";
            }
            else
            {
                modalMsg = "資料更新失敗。";
            }
        }

        if (ok)
        {
            ScriptManager.RegisterStartupScript(
                this, GetType(),
                "hideUnit",
                "hideModal('unitModal');", true);
        }

        BindList();
        ((Manage_ManageMaster)Master).ShowMessageModal(modalMsg);
    }

    protected void btnConfirmDelete_Click(object s, EventArgs e)
    {
        var ok = SysUnitHelper.DeleteByID(hfDeleteID.Value.toInt());
        string modalMsg = "";
        if (ok)
        {
            modalMsg = "資料已刪除。";
            ScriptManager.RegisterStartupScript(
              this, GetType(),
              "hideDel",
              "hideModal('deleteModal');", true);
        }
        else
        {
            modalMsg = "資料刪除失敗。";
        }

        BindList();
        ((Manage_ManageMaster)Master).ShowMessageModal(modalMsg);
    }
}
