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
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindList();
        }

    }

    void BindList()
    {
        var dt = SysUnitHelper.QueryAll();
        lvUnits.DataSource = dt;
        lvUnits.DataBind();
    }

    protected void btnAdd_Click(object s, EventArgs e)
    {
        // 準備新增
        hfUnitID.Value = "";
        LoadModalParents();
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
            LoadModalParents();

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

        var lb = (LinkButton)e.Item.FindControl("lkDeleteUnit");
        lb.Visible = IsValidDelete(id);
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
    private void LoadModalParents()
    {
        var dt = SysUnitHelper.QueryAll();

        ddlModalParent.Items.Clear();
        ddlModalParent.Items.Add(new ListItem("請選擇", ""));  // 預設選項，Value 空字串

        foreach (DataRow row in dt.Rows)
        {
            ddlModalParent.Items.Add(new ListItem(
                row["UnitName"].ToString(),
                row["UnitID"].ToString()
            ));
        }
    }

    protected void cvUnitName_ServerValidate(object source, ServerValidateEventArgs args)
    {
        string newName = args.Value.Trim();
        args.IsValid = true;

        GisTable sysUnit = SysUnitHelper.QueryAll();
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
