using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using GS.App;
using GS.Data;
using GS.Extension;
using GS.OCA_OceanSubsidy.Entity;
using Newtonsoft.Json.Linq;
using NPOI.Util;

public partial class Manage_Users : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            InitAppFilters();
            InitUnitFilters();
            BindUserList();
            BindPendingGrid();
            BindLoginHistory();
            BindOSIRoles();
            BindOFSRoles();
        }
        if (IsPostBack)
        {
            var active = hfActiveTab.Value;
            if (!string.IsNullOrEmpty(active))
            {
                // 註冊 client script，等頁面渲染完再用 bootstrap API 切回去
                string script = $@"
    var btn = document.querySelector('.tab-link[data-bs-target=""{active}""]');
    if (btn) new bootstrap.Tab(btn).show();";
                ScriptManager.RegisterStartupScript(this, GetType(), "restoreTab", script, true);
            }
        }
    }

    #region —– 初始與查詢 —–
    private void InitAppFilters()
    {
        var dt = SysAppHelper.QuerySysApp();

        ddlSearchApp.Items.Clear();
        ddlSearchApp.DataTextField = "SystemName";
        ddlSearchApp.DataValueField = "SystemID";
        ddlSearchApp.DataSource = dt;
        ddlSearchApp.DataBind();
        ddlSearchApp.Items.Insert(0, new ListItem("全部", "-1"));
    }

    private void InitUnitFilters()
    {
        var dt = SysUnitTypeHelper.QueryAll();

        ddlSearchUnitType.DataTextField = "TypeName";
        ddlSearchUnitType.DataValueField = "TypeID";
        ddlSearchUnitType.DataSource = dt;
        ddlSearchUnitType.DataBind();

        rblUserUnitType.DataTextField = "TypeName";
        rblUserUnitType.DataValueField = "TypeID";
        rblUserUnitType.DataSource = dt;
        rblUserUnitType.DataBind();

        rblApproveUnitType.DataTextField = "TypeName";
        rblApproveUnitType.DataValueField = "TypeID";
        rblApproveUnitType.DataSource = dt;
        rblApproveUnitType.DataBind();

        LoadUnits();
    }

    protected void btnSearchUser_Click(object s, EventArgs e)
    {
        BindUserList();
    }
    private void BindUserList()
    {
        DataTable dt;
        if (ddlSearchApp.SelectedValue == "-1")
        {
            dt = SysUserHelper.QueryByUnitType(ddlSearchUnitType.SelectedValue);
        }
        else if (ddlSearchApp.SelectedValue == SysAppHelper.QueryIDBySystemName("海洋科學調查活動填報系統").ToString())
        {
            dt = SysUserHelper.QueryByUnitTypeAndOSIUser(ddlSearchUnitType.SelectedValue);
        }
        else
        {
            dt = SysUserHelper.QueryByUnitTypeAndOFSUser(ddlSearchUnitType.SelectedValue);
        }

        var a = dt.Rows;
        if (dt == null || dt.Rows.Count == 0)
        {
            lvUsers.DataSource = null;
            lvUsers.DataBind();
            return;
        }

        var filtered = dt
            .AsEnumerable()
            .Where(row => row.Field<bool>("IsApproved") == true);

        if (!filtered.Any())
        {
            lvUsers.DataSource = null;
            lvUsers.DataBind();
            return;
        }
        dt = filtered.CopyToDataTable();
        dt.Columns.Add("OSIRoleName", typeof(string));
        dt.Columns.Add("OFSRoleName", typeof(string));
        dt.Columns.Add("LastLoginTime", typeof(DateTime));

        // 取得所有使用者的最後登入時間
        var loginTimeTable = SysLoginHelper.QueryLastLoginTimeByUsers();
        var loginTimeDict = new Dictionary<int, DateTime?>();
        foreach (DataRow loginRow in loginTimeTable.Rows)
        {
            loginTimeDict[Convert.ToInt32(loginRow["UserID"])] = loginRow["LastLoginTime"] as DateTime?;
        }

        foreach (DataRow row in dt.Rows)
        {
            row["OSIRoleName"] = "無需使用";
            row["OFSRoleName"] = "無需使用";

            string OSIRoleID = row["OSI_RoleID"].ToString();
            var OSIdt = OSIRoleHelper.QueryByID(OSIRoleID);
            if (OSIdt.Rows.Count > 0)
                row["OSIRoleName"] = OSIdt.Rows[0]["RoleName"].ToString();

            string userID = row["UserID"].ToString();
            var OFSdt = OFSRoleHelper.QueryByUserID(userID);
            if (OFSdt.Rows.Count > 0)
            {
                string result = string.Join("、",
                    OFSdt.AsEnumerable().Select(r => r["RoleName"].ToString()));
                row["OFSRoleName"] = result;
            }

            row["UnitName"] = SysUserHelper.QueryUnitNameByUserID(row["UserID"].ToString());
            
            // 從 Sys_Login 表更新最後登入時間
            int userId = Convert.ToInt32(row["UserID"]);
            if (loginTimeDict.ContainsKey(userId))
            {
                row["LastLoginTime"] = loginTimeDict[userId];
            }
            else
            {
                row["LastLoginTime"] = DBNull.Value;
            }
        }

        lvUsers.DataSource = dt;
        lvUsers.DataBind();
    }

    protected void dpUsers_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
    {
        dpUsers.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
        BindUserList();
    }

    private void BindPendingGrid()
    {
        var dt = SysUserHelper.QueryPendingUsers();
        if (!dt.Columns.Contains("SystemName"))
        {
            dt.Columns.Add("SystemName", typeof(string));
        }

        foreach (DataRow row in dt.Rows)
        {
            row["UnitName"] = SysUserHelper.QueryUnitNameByUserID(row["UserID"].ToString());
            row["SystemName"] = SysUserHelper.QueryApprovedSysByUserID(row["UserID"].ToString());
        }

        lvPendingUsers.DataSource = dt;
        lvPendingUsers.DataBind();
    }

    private void BindLoginHistory()
    {
        var dt = SysLoginHelper.QueryLoginHistory();
        lvLoginHistory.DataSource = dt;
        lvLoginHistory.DataBind();
    }

    private void BindOSIRoles()
    {
        var dt = OSIRoleHelper.QueryAll();

        rblUserOSIRole.DataTextField = "RoleName";
        rblUserOSIRole.DataValueField = "RoleID";
        rblUserOSIRole.DataSource = dt;
        rblUserOSIRole.DataBind();

        rblApproveOSIRole.DataTextField = "RoleName";
        rblApproveOSIRole.DataValueField = "RoleID";
        rblApproveOSIRole.DataSource = dt;
        rblApproveOSIRole.DataBind();
    }

    private void BindOFSRoles()
    {
        var dt = OFSRoleHelper.QueryAll();
        cblApproveOFSRoles.DataTextField = "RoleName";
        cblApproveOFSRoles.DataValueField = "RoleID";
        cblApproveOFSRoles.DataSource = dt;
        cblApproveOFSRoles.DataBind();

        cblUserOFSRoles.DataTextField = "RoleName";
        cblUserOFSRoles.DataValueField = "RoleID";
        cblUserOFSRoles.DataSource = dt;
        cblUserOFSRoles.DataBind();

    }

    protected void lvUsers_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        string id = e.CommandArgument.ToString();

        if (e.CommandName == "ToggleActive")
        {
            var dt = SysUserHelper.QueryUserInfoByID(id);
            if (dt.Rows.Count == 0) return;

            var user = dt.Rows[0];
            string account = user["Account"].ToString();
            bool currentActive = (bool)user["IsActive"];

            // 記錄到 HiddenField
            hfToggleUserID.Value = id;
            hfToggleUserAccount.Value = account;
            hfToggleNewStatus.Value = currentActive ? "0" : "1";

            // 動態決定顯示文字
            string message;

            if (currentActive)
            {
                message = @"
<h4 class=""text-blue-green fw-bold"">確認帳號停用?</h4>
<div class=""fs-18"">
    該使用者將無法進入系統，下次啟用帳號需進行密碼重置
</div>";
            }
            else
            {
                message = @"
<h4 class=""text-blue-green fw-bold"">確認帳號啟用?</h4>
<div class=""fs-18"">
    將寄發設置密碼通知信給予使用者
</div>";
            }

            ltlToggleConfirmMessage.Text = message;

            // 呼叫前端 Modal 顯示
            ScriptManager.RegisterStartupScript(
                this, GetType(), "showToggleModal",
                "showModal('toggleActiveModal');", true);
        }
        else if (e.CommandName == "EditUser")
        {
            string govID = SysUnitTypeHelper.QueryIDByTypeName("政府機關").ToString();
            DataTable dt = SysUserHelper.QueryUserInfoByID(id);
            if (dt.Rows.Count == 0) return;
            var r = dt.Rows[0];

            hfUserID.Value = r["UserID"].ToString();
            rblUserUnitType.SelectedValue = r["UnitType"].ToString();
            divGovUnit.Visible = (rblUserUnitType.SelectedValue == govID);
            divOtherUnit.Visible = (rblUserUnitType.SelectedValue != govID);
            rfvOtherUnit.Enabled = (rblUserUnitType.SelectedValue != govID);
            if (rblUserUnitType.SelectedValue == govID)
                ddlUserUnit.SelectedValue = r["UnitID"].ToString();
            var tbl = SysUnitHelper.QueryByUnitName("其他");
            if (tbl != null && tbl.Rows.Count > 0)
            {
                var otherUnitID = tbl.Rows[0]["UnitID"].ToString();
                txtOtherGovUnit.Visible = (ddlUserUnit.SelectedValue == otherUnitID);
                rfvOtherGovUnit.Enabled = (ddlUserUnit.SelectedValue == otherUnitID);
            }
            var unitName = SysUserHelper.QueryUnitNameByUserID(id);
            txtOtherGovUnit.Text = unitName;
            txtOtherUnit.Text = unitName;
            txtUserAccount.Text = r["Account"].ToString();
            txtUserName.Text = r["Name"].ToString();
            txtUserTel.Text = r["Tel"].ToString();
            rblUserOSIRole.SelectedValue = r["OSI_RoleID"].ToString();

            cblUserOFSRoles.ClearSelection();
            GisTable OFSdt = SysUserOFSRoleHelper.QueryByUserID(id);
            if (OFSdt == null || OFSdt.Rows.Count == 0)
            {
                rblUserNeedOFSRoles.SelectedValue = "0";
                pnlUserOFSRoleCheckboxes.Visible = false;
            }
            else
            {
                rblUserNeedOFSRoles.SelectedValue = "1";
                foreach (DataRow row in OFSdt.Rows)
                {
                    string roleID = row["RoleID"].ToString();
                    cblUserOFSRoles.Items.FindByValue(roleID).Selected = true;
                }
                pnlUserOFSRoleCheckboxes.Visible = true;
            }

            ScriptManager.RegisterStartupScript(
              this, GetType(), "showUser", "showModal('userModal');", true);
        }
        else if (e.CommandName == "DeleteUser")
        {
            hfDelUserID.Value = id;
            ScriptManager.RegisterStartupScript(
              this, GetType(), "showDelUser", "showModal('deleteUserModal');", true);
        }
    }

    protected void lvUsers_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType != ListViewItemType.DataItem)
            return;

        // 取出該列的資料
        var drv = (DataRowView)e.Item.DataItem;
        DateTime? lastLogin = drv["LastLoginTime"] as DateTime?;
        if (lastLogin.HasValue && lastLogin.Value < DateTime.Now.AddMonths(-4))
        {
            // 取得 <tr runat="server" id="itemRow">
            var row = (HtmlTableRow)e.Item.FindControl("itemRow");
            if (row != null)
                row.Attributes.Add("style",
                  "background-color:#f8d7da !important;");
        }
    }

    protected void dpUsers_PreRender(object sender, EventArgs e)
    {
        if (dpUsers.Controls.Count < 2) return;

        var container = dpUsers.Controls[1];
        foreach (Control c in container.Controls)
        {
            if (c is Button btn && btn.Text.Trim() == "...")
            {
                btn.CssClass = "pagination-item ellipsis";
            }
        }
    }

    // 啟用/停用 帳號
    protected void btnConfirmToggleActive_Click(object sender, EventArgs e)
    {
        string userID = hfToggleUserID.Value;
        string account = hfToggleUserAccount.Value;
        bool newStatus = hfToggleNewStatus.Value == "1";
        bool ok = SysUserHelper.SetUserActive(userID, newStatus);

        if (ok)
        {
            // 寄信
            if (hfToggleNewStatus.Value == "1")
            {
                bool mailOk = SysUserHelper.SendResetPwdMail(account);
            }

            BindUserList();
            ScriptManager.RegisterStartupScript(
              this, GetType(), "hideActiveModal", "hideModal('toggleActiveModal');", true);
            ((Manage_ManageMaster)Master).ShowMessageModal(
                newStatus ? "使用者已啟用。" : "使用者已停用。");
        }
        else
        {
            ((Manage_ManageMaster)Master).ShowMessageModal("操作失敗，請聯絡管理員。");
        }
    }

    #endregion

    #region —– Modal 下拉清單 —–
    protected void rblUserUnitType_SelectedIndexChanged(object s, EventArgs e)
    {
        string govID = SysUnitTypeHelper.QueryIDByTypeName("政府機關").ToString();
        var type = rblUserUnitType.SelectedValue;
        divGovUnit.Visible = (type == govID);
        divOtherUnit.Visible = (type != govID);
        rfvOtherUnit.Enabled = (type != govID);

    }
    private void LoadUnits()
    {
        var dt = SysUnitHelper.QueryAllOrderByUnitID();
        ddlUserUnit.Items.Clear();
        foreach (DataRow r in dt.Rows)
        {
            ddlUserUnit.Items.Add(new ListItem(
              r["UnitName"].ToString(),
              r["UnitID"].ToString()));

            ddlApproveUnit.Items.Add(new ListItem(
              r["UnitName"].ToString(),
              r["UnitID"].ToString()));
        }
    }
    protected void ddlUserUnit_SelectedIndexChanged(object sender, EventArgs e)
    {
        var tbl = SysUnitHelper.QueryByUnitName("其他");
        if (tbl != null && tbl.Rows.Count > 0)
        {
            var otherUnitID = tbl.Rows[0]["UnitID"].ToString();
            txtOtherGovUnit.Visible = (ddlUserUnit.SelectedValue == otherUnitID);
            rfvOtherGovUnit.Enabled = (ddlUserUnit.SelectedValue == otherUnitID);
        }
    }
    #endregion

    #region —– 新增/編輯 使用者 —–
    protected void btnAddUser_Click(object s, EventArgs e)
    {
        hfUserID.Value = "";
        rblUserUnitType.SelectedIndex = 0;
        ddlUserUnit.SelectedIndex = 0;
        txtUserAccount.Text = "";
        txtUserName.Text = "";
        txtUserTel.Text = "";
        rblUserOSIRole.SelectedIndex = 0;
        rblUserNeedOFSRoles.SelectedValue = "0";
        pnlUserOFSRoleCheckboxes.Visible = false;
        cblUserOFSRoles.ClearSelection();

        ScriptManager.RegisterStartupScript(
          this, GetType(), "showUser", "showModal('userModal');", true);
    }

    protected void btnSaveUser_Click(object s, EventArgs e)
    {
        Page.Validate("UserModal");
        if (!Page.IsValid) return;

        bool isNew = string.IsNullOrEmpty(hfUserID.Value);
        Sys_User user;

        if (isNew)
        {
            // 新增前準備：只有新增時才設
            user = new Sys_User
            {
                CreateTime = DateTime.Now,
                IsApproved = true,
                IsValid = true,
                ApprovedSource = "自行申請",
            };
        }
        else
        {
            // 編輯前先整筆讀出，保留庫存裡的其它欄位
            user = SysUserHelper.QueryUserByIDWithClass(hfUserID.Value);
        }

        // ── 共用欄位，一次設定 ──
        user.UpdateTime = DateTime.Now;
        user.UnitType = rblUserUnitType.SelectedValue.toInt();
        int govID = SysUnitTypeHelper.QueryIDByTypeName("政府機關");
        if (rblUserUnitType.SelectedValue.toInt() == govID)
        {
            user.UnitID = ddlUserUnit.SelectedValue.toInt();
            user.UnitName = null;
            var tbl = SysUnitHelper.QueryByUnitName("其他");
            if (tbl != null && tbl.Rows.Count > 0
                && ddlUserUnit.SelectedValue == tbl.Rows[0]["UnitID"].ToString())
            {
                user.UnitName = txtOtherGovUnit.Text.Trim();
            }
        }
        else
        {
            user.UnitID = null;
            user.UnitName = txtOtherUnit.Text.Trim();
        }
        user.Account = txtUserAccount.Text.Trim();
        user.Name = txtUserName.Text.Trim();
        user.Tel = txtUserTel.Text.Trim();
        user.OSI_RoleID = rblUserOSIRole.SelectedValue.toInt();
        user.IsReceiveMail = true;

        // OFS_Role
        List<Sys_UserOFSRole> ogOFSRoles = SysUserOFSRoleHelper.QueryByUserIDWithClass(user.UserID.ToString());
        var OFSRoles = new List<Sys_UserOFSRole>();
        if (rblUserNeedOFSRoles.SelectedValue == "1")
        {
            foreach (ListItem li in cblUserOFSRoles.Items)
            {
                if (!li.Selected)
                    continue;

                OFSRoles.Add(new Sys_UserOFSRole
                {
                    UserID = user.UserID,
                    RoleID = int.Parse(li.Value)
                });
            }
        }
        List<int> Intersection = ogOFSRoles
            .Select(ogr => ogr.RoleID)
            .Where(ogr => OFSRoles.Select(r => r.RoleID).Contains(ogr))
            .ToList();


        // 實際呼 Insert 或 Update
        bool ok = isNew
            ? SysUserHelper.InsertSysUser(user, OFSRoles)
            : SysUserHelper.UpdateUser(user,
                OFSRoles.Where(r => !Intersection.Contains(r.RoleID)).ToList(),
                ogOFSRoles.Where(r => !Intersection.Contains(r.RoleID)).ToList());

        if (ok)
        {
            BindUserList();
            ScriptManager.RegisterStartupScript(
              this, GetType(), "hideUser", "hideModal('userModal');", true);
            ((Manage_ManageMaster)Master)
              .ShowMessageModal("使用者資料已儲存。");

            // 寄信
            if (isNew)
                SysUserHelper.SendSetPwdMail(user.Account);
        }
        else
        {
            ((Manage_ManageMaster)Master)
              .ShowMessageModal("儲存失敗，請聯絡管理員。");
        }
    }
    #endregion

    #region —– 刪除 使用者 —–
    protected void btnConfirmDeleteUser_Click(object s, EventArgs e)
    {
        int id = int.Parse(hfDelUserID.Value);
        bool ok = SysUserHelper.DeleteUserByID(id);
        if (ok)
        {
            BindUserList();
            ScriptManager.RegisterStartupScript(
              this, GetType(), "hideDelUser", "hideModal('deleteUserModal');", true);
            ((Manage_ManageMaster)Master)
              .ShowMessageModal("使用者已刪除。");
        }
        else
        {
            ((Manage_ManageMaster)Master)
              .ShowMessageModal("刪除失敗，請聯絡管理員。");
        }
    }
    #endregion

    #region —– 待審核 帳號 —–
    protected void dpPendingUsers_PreRender(object sender, EventArgs e)
    {
        if (dpPendingUsers.Controls.Count < 2) return;

        var container = dpPendingUsers.Controls[1];
        foreach (Control c in container.Controls)
        {
            if (c is Button btn && btn.Text.Trim() == "...")
            {
                btn.CssClass = "pagination-item ellipsis";
            }
        }
    }

    protected void lvPendingUsers_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.CommandName != "ApproveUser") return;
        string id = e.CommandArgument.ToString();

        // 1. 取出該筆使用者
        var dt = SysUserHelper.QueryPendingUserByID(id);
        if (dt.Rows.Count == 0) return;
        var r = dt.Rows[0];

        // 2. 填入 Modal 裡的 Literal / RadioButtonList
        hfApproveUserID.Value = id;
        lblApproveSource.Text = r["ApprovedSource"].ToString();
        lblApproveSystems.Text = SysUserHelper.QueryApprovedSysByUserID(id);
        rblApproveUnitType.SelectedValue = r["UnitType"].ToString();
        string govID = SysUnitTypeHelper.QueryIDByTypeName("政府機關").ToString();
        divApproveGovUnit.Visible = (rblApproveUnitType.SelectedValue == govID);
        divApproveOtherUnit.Visible = (rblApproveUnitType.SelectedValue != govID);
        rfvApproveOtherUnit.Enabled = (rblApproveUnitType.SelectedValue != govID);
        if (rblApproveUnitType.SelectedValue == govID)
            ddlApproveUnit.SelectedValue = r["UnitID"].ToString();
        var tbl = SysUnitHelper.QueryByUnitName("其他");
        if (tbl != null && tbl.Rows.Count > 0)
        {
            var otherUnitID = tbl.Rows[0]["UnitID"].ToString();
            txtApproveOtherGovUnit.Visible = (ddlApproveUnit.SelectedValue == otherUnitID);
            rfvApproveOtherGovUnit.Enabled = (ddlApproveUnit.SelectedValue == otherUnitID);
        }
        var unitName = SysUserHelper.QueryUnitNameByUserID(id);
        txtApproveOtherGovUnit.Text = unitName;
        txtApproveOtherUnit.Text = unitName;
        txtApproveAccount.Text = r["Account"].ToString();
        txtApproveName.Text = r["Name"].ToString();
        txtApproveTel.Text = r["Tel"].ToString();
        rblApproveOSIRole.SelectedValue = OSIRoleHelper.QueryIDByRoleName("無需使用");
        if (lblApproveSystems.Text.Contains("海洋科學調查活動填報系統"))
        {
            rblApproveOSIRole.SelectedValue =
                (rblApproveUnitType.SelectedValue == govID)
                ? OSIRoleHelper.QueryIDByRoleName("機關填報端")
                : OSIRoleHelper.QueryIDByRoleName("計畫執行端");
        }

        rblNeedOFSRoles.SelectedValue = "0";
        pnlOFSRoleCheckboxes.Visible = false;
        cblApproveOFSRoles.ClearSelection();
        if (lblApproveSystems.Text.Contains("海洋領域補助計畫管理資訊系統"))
        {
            rblNeedOFSRoles.SelectedValue = "1";
            pnlOFSRoleCheckboxes.Visible = true;
            var selectedID = OFSRoleHelper.QueryIDByRoleName("申請者");
            cblApproveOFSRoles.Items.FindByValue(selectedID).Selected = true;
        }

        // 3. 呼叫 JS 顯示 Modal
        ScriptManager.RegisterStartupScript(
          this, GetType(), "showApprove",
          "showModal('approveModal');", true);
    }

    protected void lvPendingUsers_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
    {
        dpPendingUsers.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
        BindPendingGrid();
    }

    protected void lvLoginHistory_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
    {
        dpLoginHistory.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
        BindLoginHistory();
    }

    protected void dpLoginHistory_PreRender(object sender, EventArgs e)
    {
        if (dpLoginHistory.Controls.Count < 2) return;

        var container = dpLoginHistory.Controls[1];
        foreach (Control c in container.Controls)
        {
            if (c is Button btn && btn.Text.Trim() == "...")
            {
                btn.CssClass = "pagination-item ellipsis";
            }
        }
    }

    protected void rblNeedOFSRoles_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Value="1" 表需使用，"0" 表無需使用
        bool need = rblNeedOFSRoles.SelectedValue == "1";
        pnlOFSRoleCheckboxes.Visible = need;
    }

    protected void rblUserNeedOFSRoles_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Value="1" 表需使用，"0" 表無需使用
        bool need = rblUserNeedOFSRoles.SelectedValue == "1";
        pnlUserOFSRoleCheckboxes.Visible = need;
    }

    protected void cvApproveOFSRoles_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (rblNeedOFSRoles.SelectedValue == "0") return;
        // 檢查至少有一個選項被勾選
        bool anyChecked = cblApproveOFSRoles.Items
                            .Cast<ListItem>()
                            .Any(li => li.Selected);

        args.IsValid = anyChecked;
    }

    protected void cvUserOFSRoles_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (rblUserNeedOFSRoles.SelectedValue == "0") return;
        // 檢查至少有一個選項被勾選
        bool anyChecked = cblUserOFSRoles.Items
                            .Cast<ListItem>()
                            .Any(li => li.Selected);

        args.IsValid = anyChecked;
    }

    protected void rblApproveUnitType_SelectedIndexChanged(object s, EventArgs e)
    {
        string govID = SysUnitTypeHelper.QueryIDByTypeName("政府機關").ToString();
        var type = rblApproveUnitType.SelectedValue;
        divApproveGovUnit.Visible = (type == govID);
        divApproveOtherUnit.Visible = (type != govID);
    }

    protected void ddlApproveUnit_SelectedIndexChanged(object sender, EventArgs e)
    {
        var tbl = SysUnitHelper.QueryByUnitName("其他");
        if (tbl != null && tbl.Rows.Count > 0)
        {
            var otherUnitID = tbl.Rows[0]["UnitID"].ToString();
            txtApproveOtherGovUnit.Visible = (ddlApproveUnit.SelectedValue == otherUnitID);
            rfvApproveOtherGovUnit.Enabled = (ddlApproveUnit.SelectedValue == otherUnitID);
        }
    }

    protected void btnApprove_Click(object sender, EventArgs e)
    {
        // 強制前端驗證
        Page.Validate("ApproveGroup");
        if (!Page.IsValid) return;

        Sys_User user = SysUserHelper.QueryUserByIDWithClass(hfApproveUserID.Value);
        user.UpdateTime = DateTime.Now;
        user.IsApproved = true;
        user.UnitType = rblApproveUnitType.SelectedValue.toInt();
        int govID = SysUnitTypeHelper.QueryIDByTypeName("政府機關");
        if (rblApproveUnitType.SelectedValue.toInt() == govID)
        {
            user.UnitID = ddlApproveUnit.SelectedValue.toInt();
            user.UnitName = null;
            var tbl = SysUnitHelper.QueryByUnitName("其他");
            if (tbl != null && tbl.Rows.Count > 0
                && ddlApproveUnit.SelectedValue == tbl.Rows[0]["UnitID"].ToString())
            {
                user.UnitName = txtApproveOtherGovUnit.Text.Trim();
            }
        }
        else
        {
            user.UnitID = null;
            user.UnitName = txtApproveOtherUnit.Text.Trim();
        }
        user.Account = txtApproveAccount.Text.Trim();
        user.Name = txtApproveName.Text.Trim();
        user.Tel = txtApproveTel.Text.Trim();
        user.OSI_RoleID = int.Parse(rblApproveOSIRole.SelectedValue);
        user.IsReceiveMail = true;

        // OFS_Role
        var OFSRoles = new List<Sys_UserOFSRole>();
        if (rblNeedOFSRoles.SelectedValue == "1")
        {
            foreach (ListItem li in cblApproveOFSRoles.Items)
            {
                if (!li.Selected)
                    continue;

                OFSRoles.Add(new Sys_UserOFSRole
                {
                    UserID = user.UserID,
                    RoleID = int.Parse(li.Value)
                });
            }
        }

        // 呼叫 Helper 實作：設定通過狀態、角色、收稽催信
        bool ok = SysUserHelper.ApproveUser(user, OFSRoles);

        if (ok)
        {
            // 重新綁定待審核列表
            BindPendingGrid();

            // 隱藏 Modal
            ScriptManager.RegisterStartupScript(
              this, GetType(), "hideApprove",
              "hideModal('approveModal');", true);

            // 顯示共用訊息 Modal
            ((Manage_ManageMaster)Master)
              .ShowMessageModal("帳號已審核通過。");

            // 寄信
            SysUserHelper.SendSetPwdMail(user.Account);
        }
        else
        {
            ((Manage_ManageMaster)Master)
              .ShowMessageModal("審核操作失敗，請聯絡系統管理員。");
        }
    }
    // 「審核退回」— 刪除帳號
    protected void btnReject_Click(object s, EventArgs e)
    {
        int id = int.Parse(hfApproveUserID.Value);
        bool ok = SysUserHelper.DeleteUserByID(id);
        if (ok)
        {
            BindPendingGrid();
            ScriptManager.RegisterStartupScript(
              this, GetType(),
              "hideApprove",
              "hideModal('approveModal');",
              true);
            ((Manage_ManageMaster)Master)
              .ShowMessageModal("審核已退回。");

            // 寄信通知使用者帳號已退回
            string mailBody = MailContent.OCA.ApproveFail.getMail();
            GS.App.Utility.Mail.SendMail(txtApproveAccount.Text, "", MailContent.OCA.ApproveFail.Subject, mailBody, out string ErrorMsg);
        }
        else
        {
            ((Manage_ManageMaster)Master)
              .ShowMessageModal("退回失敗，請聯絡管理員。");
        }
    }
    #endregion

    #region —– 帳號重複驗證 —–
    protected void cvUserAccount_ServerValidate(
        object source, ServerValidateEventArgs args)
    {
        var tbl = SysUserHelper.QueryUserByAccount(args.Value);
        // 新增時 tbl.Rows.Count>0 就重複；編輯時若查到且 ID 不同也重複
        if (tbl.Rows.Count > 0 &&
           (string.IsNullOrEmpty(hfUserID.Value) ||
            tbl.Rows[0]["UserID"].ToString() != hfUserID.Value))
            args.IsValid = false;
    }
    #endregion
}
