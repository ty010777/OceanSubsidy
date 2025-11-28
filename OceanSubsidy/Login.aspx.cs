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
using log4net;

public partial class Login : System.Web.UI.Page
{
    private static readonly ILog log = LogManager.GetLogger(typeof(Login));
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

        string account = txtEmail.Text.Trim();
        string clientIP = GetClientIP();

        bool ok = ValidateUser(account, password.Text, out int userId);
        if (ok)
        {
            // 記錄成功登入日誌
            log.Info($"使用者登入成功 - 帳號: {account}, UserID: {userId}, IP: {clientIP}");

            // ===== 新增：檢查密碼是否過期（90天） =====
            var user = SysUserHelper.QueryUserEntityByAccount(account);
            if (user != null && user.LastPwdChangeTime.HasValue)
            {
                TimeSpan passwordAge = DateTime.Now - user.LastPwdChangeTime.Value;
                // geo@geosense.tw 帳號不檢查密碼過期
                if (passwordAge.TotalDays > 90 && account != "geo@geosense.tw")
                {
                    // 密碼已過期，設定資料並開啟重設密碼 Modal
                    lblResetAccount.Text = account;
                    hfExpiredAccount.Value = account;
                    hfExpiredUserID.Value = userId.ToString();
                    hfExpiredSalt.Value = user.Salt;
                    hfIsExpiredChange.Value = "true";

                    ScriptManager.RegisterStartupScript(
                        this,
                        GetType(),
                        "showExpiredModal",
                        "showModal('resetModal'); $('#resetModalTitle').text('密碼已過期，請重設密碼');",
                        true);

                    return; // 中斷登入流程，要求先變更密碼
                }
            }

            // 呼叫共用登入方法
            PerformLogin(account, userId);
        }
        else
        {
            // 記錄登入失敗日誌（錯誤訊息已在 ValidateUser 或 HandleLoginFailure 中顯示）
            log.Warn($"使用者登入失敗 - 帳號: {account}, IP: {clientIP}");
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

    // 取得客戶端 IP
    private string GetClientIP()
    {
        string clientIP = "";
        
        // 嘗試從不同的 header 取得真實 IP（優先順序由高到低）
        string[] headers = {
            "HTTP_X_FORWARDED_FOR",
            "HTTP_X_REAL_IP",
            "HTTP_CLIENT_IP",
            "HTTP_X_CLUSTER_CLIENT_IP",
            "HTTP_FORWARDED_FOR",
            "HTTP_FORWARDED"
        };
        
        foreach (string header in headers)
        {
            string value = Request.ServerVariables[header];
            if (!string.IsNullOrEmpty(value))
            {
                // 處理多個 IP 的情況（以逗號分隔）
                string[] ips = value.Split(',');
                foreach (string ip in ips)
                {
                    string trimmedIP = ip.Trim();
                    if (!string.IsNullOrEmpty(trimmedIP) && 
                        !trimmedIP.StartsWith("10.") && 
                        !trimmedIP.StartsWith("192.168.") && 
                        !trimmedIP.StartsWith("172.") &&
                        trimmedIP != "127.0.0.1" &&
                        trimmedIP != "::1")
                    {
                        clientIP = trimmedIP;
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(clientIP))
                    break;
            }
        }
        
        // 如果沒有找到公網 IP，使用 UserHostAddress
        if (string.IsNullOrEmpty(clientIP))
        {
            clientIP = Request.UserHostAddress;
        }
        
        // 將 IPv6 localhost 轉換為 IPv4 格式
        if (clientIP == "::1")
        {
            clientIP = "127.0.0.1";
        }
        
        return clientIP ?? "";
    }

    // 執行登入動作（共用方法）
    private void PerformLogin(string account, int userId)
    {
        // 取得客戶端 IP
        string clientIP = GetClientIP();
        
        // 登入成功
        SysLoginHelper.Insert(new Sys_Login
        {
            UserID = userId,
            LoginIP = clientIP,
            LoginTime = DateTime.Now.ToString()
        });
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

        // 1. 使用新的查詢方法獲取完整的使用者實體（包含 LoginFailCount 和 LockoutTime）
        var user = SysUserHelper.QueryUserEntityByAccount(email);
        if (user == null)
        {
            // 帳號不存在（出於安全考量，顯示通用錯誤訊息）
            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "accountNotFound",
                "showGlobalMessage('帳號密碼有誤');",
                true);
            return false;
        }

        userId = user.UserID;

        // 2. 檢查帳號是否被鎖定
        if (user.LockoutTime.HasValue)
        {
            TimeSpan lockDuration = DateTime.Now - user.LockoutTime.Value;

            if (lockDuration.TotalMinutes < 15)
            {
                // 仍在鎖定期間
                int remainingMinutes = 15 - (int)lockDuration.TotalMinutes;

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "lockoutAlert",
                    $"showGlobalMessage('帳號已被鎖定，請於 {remainingMinutes} 分鐘後再試');",
                    true);

                return false;
            }
            else
            {
                // 鎖定時間已過，解除鎖定
                SysUserHelper.ResetLoginFailure(user.UserID);
            }
        }

        // 3. 檢查是否已經核可
        if (!user.IsApproved)
        {
            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "accountNotApproved",
                "showGlobalMessage('帳號尚未審核通過');",
                true);
            return false;
        }

        // 4. 解密密碼並驗證
        string decrypted;
        try
        {
            decrypted = Cryptography.AESGCM.DecryptText(user.Pwd, user.Salt);
        }
        catch
        {
            // 解密失敗，增加失敗次數
            HandleLoginFailure(user);
            return false;
        }

        // 5. 明文比對使用者輸入的密碼
        if (decrypted == password)
        {
            // 登入成功，重置失敗計數
            if (user.LoginFailCount > 0)
            {
                SysUserHelper.ResetLoginFailure(user.UserID);
            }
            return true;
        }
        else
        {
            // 密碼錯誤，處理登入失敗
            HandleLoginFailure(user);
            return false;
        }
    }

    /// <summary>
    /// 處理登入失敗（增加失敗次數或鎖定帳號）
    /// </summary>
    private void HandleLoginFailure(Sys_User user)
    {
        user.LoginFailCount++;

        if (user.LoginFailCount >= 3)
        {
            // 達到3次，鎖定帳號
            SysUserHelper.LockAccount(user.UserID);

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "lockAccountAlert",
                "showGlobalMessage('密碼錯誤次數過多，帳號已被鎖定 15 分鐘');",
                true);
        }
        else
        {
            // 未達3次，記錄失敗次數
            SysUserHelper.IncrementLoginFailCount(user.UserID);

            int remainingAttempts = 3 - user.LoginFailCount;
            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "failAlert",
                $"showGlobalMessage('密碼錯誤，還有 {remainingAttempts} 次機會');",
                true);
        }
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
            // 寄信通知所有系統管理者
            string mailBody = MailContent.OCA.PendingUser.getMail(DateTime.Now);

            if (ApprovedSysID.Contains(SysAppHelper.QueryIDBySystemName("海洋科學調查活動填報系統")))
            {
                // 海洋科學調查活動填報系統 - 寄給所有系統管理者
                var tbl = SysUserHelper.QueryUserByOSIRoleName("系統管理者");
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    foreach (DataRow row in tbl.Rows)
                    {
                        var email = row["Account"].ToString();
                        string ErrorMsg = "";
                        GS.App.Utility.Mail.SendMail(email, "", MailContent.OCA.PendingUser.Subject, mailBody, out ErrorMsg);
                    }
                }
            }

            if (ApprovedSysID.Contains(SysAppHelper.QueryIDBySystemName("海洋領域補助計畫管理資訊系統")))
            {
                // 海洋領域補助計畫管理資訊系統 - 寄給所有系統管理者
                var tbl = SysUserHelper.QueryUserByOFSRoleName("系統管理者");
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    foreach (DataRow row in tbl.Rows)
                    {
                        var email = row["Account"].ToString();
                        string ErrorMsg = "";
                        GS.App.Utility.Mail.SendMail(email, "", MailContent.OCA.PendingUser.Subject, mailBody, out ErrorMsg);
                    }
                }
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

        string newPwd = txtResetPwd.Text.Trim();
        string confirmPwd = txtResetPwdConfirm.Text.Trim();
        string isExpiredChange = hfIsExpiredChange.Value;

        // 檢查新密碼和確認密碼是否一致
        if (newPwd != confirmPwd)
        {
            ScriptManager.RegisterStartupScript(
                this, GetType(),
                "pwdMismatch",
                "showGlobalMessage('新密碼與確認密碼不一致');",
                true);
            return;
        }

        // 1. 驗證密碼複雜度
        string errorMsg;
        if (!GS.OCA_OceanSubsidy.Utility.PasswordValidator.ValidateComplexity(newPwd, out errorMsg))
        {
            ScriptManager.RegisterStartupScript(
                this, GetType(),
                "pwdComplexityError",
                $"showGlobalMessage('{errorMsg}');",
                true);
            return;
        }

        bool ok = false;

        try
        {
            if (isExpiredChange == "true")
            {
                // ===== 情境2：密碼過期變更 =====
                string account = hfExpiredAccount.Value;
                int userID = Convert.ToInt32(hfExpiredUserID.Value);
                string salt = hfExpiredSalt.Value;

                // 呼叫密碼過期變更方法（使用者已透過登入驗證，不需要再次驗證舊密碼）
                ok = SysUserHelper.UpdatePwdForExpired(userID, newPwd, salt);

                if (ok)
                {
                    // 變更成功，直接登入
                    SetSession(account, userID);

                    ScriptManager.RegisterStartupScript(
                        this, GetType(),
                        "expiredChangeSuccess",
                        "showGlobalMessage('密碼變更成功，正在登入系統...'); setTimeout(function() { window.location='Default.aspx'; }, 2000);",
                        true);
                }
            }
            else
            {
                // ===== 情境1：忘記密碼（原有邏輯） =====
                string token = hfResetToken.Value;
                string salt = hfResetSalt.Value;
                string account = lblResetAccount.Text;

                ok = SysUserHelper.UpdatePwd(token, account, salt, newPwd);

                if (ok)
                {
                    ScriptManager.RegisterStartupScript(
                        this, GetType(),
                        "resetSuccess",
                        "showGlobalMessage('您已成功重置密碼，將跳轉至登入頁。'); setTimeout(function() { window.location='Login.aspx'; }, 3000);",
                        true);
                }
            }

            if (!ok)
            {
                ScriptManager.RegisterStartupScript(
                    this, GetType(),
                    "errReset",
                    "showGlobalMessage('密碼變更失敗，請聯絡管理員');",
                    true);
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(
                this, GetType(),
                "exception",
                $"showGlobalMessage('{ex.Message}');",
                true);
        }
    }

}