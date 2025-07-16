using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using GS.Data;
using GS.Extension;
using GS.OCA_OceanSubsidy.Entity;
using NPOI.Util;
using static GS.App.Utility;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // 一般先載入登入用的設定
            LoadRegisterSystems();
            LoadRegisterUnitType();
            LoadRegUnits(rblRegUnitType.SelectedValue);

            // 檢查是否為登出後導向
            var msg = Request.QueryString["msg"];
            if (msg == "logout")
            {
                ScriptManager.RegisterStartupScript(
                    this, GetType(),
                    "logoutSuccess",
                    "showGlobalMessage('您已成功登出');",
                    true);
            }

            // 若 QueryString 帶了 token，就先撈資料並開 Reset Modal
            var token = Request.QueryString["token"];
            if (!string.IsNullOrEmpty(token))
            {
                var dt = SysUserHelper.QueryPwdInfoByPwdToken(token);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    lblResetAccount.Text = row["Account"].ToString();
                    hfResetToken.Value = token;
                    hfResetSalt.Value = row["Salt"].ToString();

                    // 自動開 Modal
                    ScriptManager.RegisterStartupScript(
                        this, GetType(),
                        "showReset",
                        "showModal('resetModal');",
                        true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(
                    this, GetType(),
                    "pwdTokenExpiredAlert",
                    "showGlobalMessage('該連結已失效，請重新申請忘記密碼。');",
                    true);
                }
            }
        }
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        Page.Validate("Login");
        if (!Page.IsValid) return;

        bool ok = ValidateUser(txtEmail.Text.Trim(), password.Text, out int userId);
        if (ok)
        {
            // 呼叫共用登入方法
            PerformLogin(txtEmail.Text.Trim(), userId);
            return;
        }
        else
        {
            // 登入失敗，顯示錯誤訊息
            ScriptManager.RegisterStartupScript(
                this, GetType(),
                "loginError",
                "showGlobalMessage('帳號密碼有誤');",
                true);
        }
    }

    // 設定Session
    private void SetSession(string account, int userId)
    {
        List<string> perms = SysUserHelper.GetOSIPermsByAccount(account);
        SessionHelper.Set(SessionHelper.UserPermissions, perms);

        var userInfo = new SessionHelper.UserInfoClass();
        var tbl = SysUserHelper.QueryUserInfoByID(userId);
        if (tbl != null && tbl.Rows.Count > 0)
        {
            var data = tbl.Rows[0];
            userInfo.UserID = data["UserID"].ToString();
            userInfo.Account = data["Account"].ToString();
            userInfo.UserName = data["Name"].ToString();
            userInfo.UnitID = data["UnitID"].ToString();
            userInfo.UnitName= data["UnitName"].ToString();
            userInfo.UnitType = data["UnitType"].ToString();
            userInfo.UnitName = SysUserHelper.QueryUnitNameByUserID(userInfo.UserID);
            userInfo.OSI_RoleName = data["OSI_RoleName"].ToString();
            
            // 查詢使用者的 OFS 角色名稱
            var ofsRoleTable = OFSRoleHelper.QueryByUserID(userInfo.UserID);
            if (ofsRoleTable != null && ofsRoleTable.Rows.Count > 0)
            {
                userInfo.OFS_RoleName = ofsRoleTable.Rows.Cast<DataRow>()
                    .Select(row => row["RoleName"].ToString())
                    .ToArray();
            }
            else
            {
                userInfo.OFS_RoleName = new string[0];
            }
        }
        SessionHelper.Set(SessionHelper.UserInfo, userInfo);
    }

    // 執行登入動作（共用方法）
    private void PerformLogin(string account, int userId)
    {
        // 登入成功
        SysUserHelper.UpdateLastLoginTime(userId);
        SetSession(account, userId);
        Response.Redirect("~/Default.aspx");
    }

    protected void cvCaptcha_ServerValidate(object source, ServerValidateEventArgs args)
    {
        // 取得正確的圖形碼（Session 內存取）
        string expected = SessionHelper.Get<string>(SessionHelper.LoginValidate);
        // 使用者輸入
        string actual = args.Value.Trim();

        args.IsValid = string.Equals(expected, actual, StringComparison.OrdinalIgnoreCase);
    }

    private bool ValidateUser(string email, string password, out int userId)
    {
        userId = 0;
        // 1. 先查這個帳號
        var tbl = SysUserHelper.QueryUserByAccount(email);
        if (tbl == null || tbl.Rows.Count == 0)
            return false;

        var row = tbl.Rows[0];
        // 2. 檢查是否已經核可
        if (row["IsApproved"].ToString().ToLower() != "true")
            return false;

        // 3. 從資料庫拿出加密後的密文和對應的 salt
        string cipherText = row["Pwd"].ToString();
        string salt = row["Salt"].ToString();

        // 4. 解密（若解密失敗，DecryptText 會回傳錯誤字串，可視需要檢查）
        string decrypted;
        try
        {
            decrypted = Cryptography.AESGCM.DecryptText(cipherText, salt);
        }
        catch
        {
            // 解密失敗就判為不合法
            return false;
        }

        // 5. 明文比對使用者輸入的密碼
        userId = row.Field<int>("UserID");
        return decrypted == password;
    }

    // ========== 申請帳號 ==========
    private void LoadRegisterSystems()
    {
        GisTable sysApp = SysAppHelper.QuerySysApp();
        for (int i = 0; i <= sysApp.Rows.Count - 1; i++)
        {
            string text = sysApp.Rows[i]["SystemName"].ToString();
            string value = sysApp.Rows[i]["SystemID"].ToString();
            cblRegSystems.Items.Add(new ListItem(text, value));
        }

        if (cblRegSystems.Items.Count > 0)
            cblRegSystems.SelectedIndex = 0;
    }

    private void LoadRegisterUnitType()
    {
        var dt = SysUnitTypeHelper.QueryAll();

        rblRegUnitType.DataTextField = "TypeName";
        rblRegUnitType.DataValueField = "TypeID";
        rblRegUnitType.DataSource = dt;
        rblRegUnitType.DataBind();
        rblRegUnitType.SelectedIndex = 0;
    }

    protected void rblRegUnitType_SelectedIndexChanged(object s, EventArgs e)
    {
        string govID = SysUnitTypeHelper.QueryIDByTypeName("政府機關").ToString();
        var type = rblRegUnitType.SelectedValue;
        trGovUnit.Visible = (type == govID);
        trOtherUnit.Visible = (type != govID);
        rfvOtherUnit.Enabled = (type != govID);
    }

    private void LoadRegUnits(string type)
    {
        string govID = SysUnitTypeHelper.QueryIDByTypeName("政府機關").ToString();
        trGovUnit.Visible = (type == govID);
        txtOtherGovUnit.Visible = false;
        rfvOtherGovUnit.Enabled = false;
        trOtherUnit.Visible = (type != govID);
        rfvOtherUnit.Enabled = (type != govID);

        GisTable sysUnit = SysUnitHelper.QueryAllOrderByUnitID();
        ddlRegUnits.Items.Clear();
        for (int i = 0; i <= sysUnit.Rows.Count - 1; i++)
        {
            string text = sysUnit.Rows[i]["UnitName"].ToString();
            string value = sysUnit.Rows[i]["UnitID"].ToString();
            ddlRegUnits.Items.Add(new ListItem(text, value));
        }

        if (ddlRegUnits.Items.Count > 0)
            ddlRegUnits.SelectedIndex = 0;
    }

    protected void ddlRegUnits_SelectedIndexChanged(object sender, EventArgs e)
    {
        var tbl = SysUnitHelper.QueryByUnitName("其他");
        if (tbl != null && tbl.Rows.Count > 0)
        {
            var otherUnitID = tbl.Rows[0]["UnitID"].ToString();
            txtOtherGovUnit.Visible = (ddlRegUnits.SelectedValue == otherUnitID);
            rfvOtherGovUnit.Enabled = (ddlRegUnits.SelectedValue == otherUnitID);
        }
    }

    // 產生驗證碼並寄信
    protected void btnSendCode_Click(object sender, EventArgs e)
    {
        Page.Validate("Account");
        if (!Page.IsValid) return;

        string email = txtRegEmail.Text.Trim();

        // 檢查帳號是否存在
        bool exists = SysUserHelper.IsExistAccount(email);
        if (exists)
        {
            cvRegisterAccountExists.IsValid = false;
            return;
        }

        // 產生 4 碼隨機數字，範圍 0000~99999
        byte[] buf = new byte[2];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(buf);
        }
        // 把兩個 byte 轉成 UInt16，再 mod 100000，最後 padding 5 碼
        int num = BitConverter.ToUInt16(buf, 0) % 100000;
        string code = num.ToString("D5");

        // 存入資料庫
        SysUserVerificationHelper.InsertOrUpdateCode(email, code);

        // 寄信
        string mailBody = MailContent.OCA.UserVerification.getMail(code);
        GS.App.Utility.Mail.SendMail(email, "", MailContent.OCA.UserVerification.Subject, mailBody, out string ErrorMsg);

        // 寄信通知
        ScriptManager.RegisterStartupScript(
            this,
            this.GetType(),
            "sendMail",
            "showGlobalMessage('驗證碼已寄出。');",
            true);
    }
    // 驗證使用者輸入的驗證碼
    protected void cvRegCode_ServerValidate(
        object source, ServerValidateEventArgs args)
    {
        var tbl = SysUserVerificationHelper.QueryByAccount(txtRegEmail.Text.Trim());
        if (tbl == null || tbl.Rows.Count == 0)
        {
            args.IsValid = false;
            return;
        }
        string code = tbl.Rows[0]["VerificationCode"].ToString();
        args.IsValid = string.Equals(code, args.Value.Trim(),
                                      StringComparison.OrdinalIgnoreCase);
    }
    protected void btnRegister_Click(object sender, EventArgs e)
    {
        Page.Validate("Register");
        if (!Page.IsValid) return;

        // 存資料
        int govID = SysUnitTypeHelper.QueryIDByTypeName("政府機關");
        Sys_User user = new Sys_User
        {
            UnitType = rblRegUnitType.SelectedValue.toInt(),
            Account = txtRegEmail.Text.Trim(),
            Name = txtRegName.Text.Trim(),
            Tel = txtRegTel.Text.Trim(),
            ApprovedSource = "自行申請",
        };

        if (rblRegUnitType.SelectedValue.toInt() == govID)
        {
            user.UnitID = ddlRegUnits.SelectedValue.toInt();
            user.UnitName = null;
            var tbl = SysUnitHelper.QueryByUnitName("其他");
            if (tbl != null && tbl.Rows.Count > 0
                && ddlRegUnits.SelectedValue == tbl.Rows[0]["UnitID"].ToString())
            {
                user.UnitName = txtOtherGovUnit.Text.Trim();
            }
        }
        else
        {
            user.UnitID = null;
            user.UnitName = txtOtherUnit.Text.Trim();
        }

        List<int> ApprovedSysID = cblRegSystems.Items.Cast<ListItem>()
            .Where(item => item.Selected)
            .Select(item => int.Parse(item.Value))
            .ToList();

        bool ok = SysUserHelper.InsertSysUserBasic(user, ApprovedSysID);

        if (ok)
        {
            // 寄信
            if (ApprovedSysID.Contains(SysAppHelper.QueryIDBySystemName("海洋科學調查活動填報系統")))
            {
                // 海洋科學調查活動填報系統
                var tbl = SysUserHelper.QueryUserByOSIRoleName("系統管理者");
                if (tbl == null || tbl.Rows.Count == 0)
                    return;

                var email = tbl.Rows[0]["Account"].ToString();
                string ErrorMsg = "";
                string mailBody = MailContent.OCA.PendingUser.getMail(DateTime.Now);
                bool mailOk = GS.App.Utility.Mail.SendMail(email, "", MailContent.OCA.PendingUser.Subject, mailBody, out ErrorMsg);
            }

            if (ApprovedSysID.Contains(SysAppHelper.QueryIDBySystemName("海洋領域補助計畫管理資訊系統")))
            {
                // 海洋領域補助計畫管理資訊系統
                var tbl = SysUserHelper.QueryUserByOFSRoleName("系統管理者");
                if (tbl == null || tbl.Rows.Count == 0)
                    return;

                var email = tbl.Rows[0]["Account"].ToString();
                string ErrorMsg = "";
                string mailBody = MailContent.OCA.PendingUser.getMail(DateTime.Now);
                bool mailOk = GS.App.Utility.Mail.SendMail(email, "", MailContent.OCA.PendingUser.Subject, mailBody, out ErrorMsg);
            }

            // 寄信成功後，刪除驗證碼
            SysUserVerificationHelper.DeleteByAccount(txtRegEmail.Text.Trim());

            // 關閉 Modal
            ScriptManager.RegisterStartupScript(
                this, GetType(),
                "hideReg",
                "hideModal('registerModal'); showGlobalMessage('申請已送出，請等候審核');",
                true);
        }
        else
        {
            // 申請失敗
            ScriptManager.RegisterStartupScript(
                this,
                this.GetType(),
                "alertFail",
                "showGlobalMessage('申請失敗，請稍後再試或聯絡系統管理員。');",
                true);
        }
    }

    // ========== 忘記密碼 ==========
    protected void btnForgot_Click(object sender, EventArgs e)
    {
        Page.Validate("Forgot");
        if (!Page.IsValid) return;

        string email = txtForgotEmail.Text.Trim();
        // 檢查帳號是否存在或是否已經審核
        var tbl = SysUserHelper.QueryUserByAccount(email);
        if (tbl == null || tbl.Rows.Count == 0 || tbl.Rows[0]["IsApproved"].ToString().ToLower() != "true")
        {
            cvForgetAccountExists.IsValid = false;
            return;
        }

        // 設定PwdToken及寄信
        bool mailOk = SysUserHelper.SendResetPwdMail(email);

        if (mailOk)
        {
            ScriptManager.RegisterStartupScript(
                this, GetType(),
                "hideForgot",
                "hideModal('forgotModal'); showGlobalMessage('重置信已寄出');",
                true);
        }
        else
        {
            ScriptManager.RegisterStartupScript(
                this, GetType(), "fail",
                "showGlobalMessage('寄信失敗，請稍後再試或聯絡系統管理員');", true);
        }
    }

    // ========== 設定/重設密碼 ==========
    protected void btnReset_Click(object sender, EventArgs e)
    {
        Page.Validate("Reset");
        if (!Page.IsValid) return;

        string token = hfResetToken.Value;
        string salt = hfResetSalt.Value;
        string pwd = txtResetPwd.Text.Trim();

        bool ok = SysUserHelper.UpdatePwd(token, lblResetAccount.Text, salt, pwd);
        if (ok)
        {            
            ScriptManager.RegisterStartupScript(
                this, GetType(),
                "resetSuccess",
                "showGlobalMessage('您已成功重置密碼，將跳轉至登入頁。'); setTimeout(function() { window.location='Login.aspx'; }, 3000);",
                true);
        }
        else
        {
            ScriptManager.RegisterStartupScript(
                this, GetType(),
                "errReset",
                "showGlobalMessage('重置失敗，請聯絡管理員');",
                true);
        }
    }

}