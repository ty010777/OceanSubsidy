<%@ Application Language="C#" %>
<%@ Import Namespace="GS.Data.Sql" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Linq" %>

<script RunAt="server">

    void Application_Start(object sender, EventArgs e)
    {
		// 啟用 TLS 1.2 和 TLS 1.3
		System.Net.ServicePointManager.SecurityProtocol = 
        System.Net.SecurityProtocolType.Tls12 | 
        System.Net.SecurityProtocolType.Tls13;
        // 應用程式啟動時執行的程式碼        

        // 設定 log4net 的全域變數
        log4net.GlobalContext.Properties["LogPath"] = System.Configuration.ConfigurationManager.AppSettings["LogPath"];

        // 初始化 log4net
        log4net.Config.XmlConfigurator.Configure();

        // 記錄 Log4net 啟動資訊
        Env.Log.Info("Log4net啟動");

        // 註冊DBConnectionString預設連線資訊
        Generic.DBConnectionString = Env.S_DefaultConnection;
    }

    void Application_End(object sender, EventArgs e)
    {
        //  應用程式啟動時執行的程式碼

    }

    void Application_Error(object sender, EventArgs e)
    {
        // 應用程式啟動時執行的程式碼

    }

    void Session_Start(object sender, EventArgs e)
    {
        // 在新的工作階段啟動時執行的程式碼

        // 開發環境自動登入 (讀取 dev.config)
        if (HttpContext.Current.IsDebuggingEnabled &&
            SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo) is null)
        {
            // 嘗試載入開發者設定檔
            var devConfigPath = HttpContext.Current.Server.MapPath("~/dev.config");
            if (System.IO.File.Exists(devConfigPath))
            {
                try
                {
                    var devConfig = new System.Configuration.ExeConfigurationFileMap();
                    devConfig.ExeConfigFilename = devConfigPath;
                    var config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(devConfig, System.Configuration.ConfigurationUserLevel.None);

                    var enabledSetting = config.AppSettings.Settings["DevAutoLogin.Enabled"];
                    if (enabledSetting != null && enabledSetting.Value == "true")
                    {
                        var accountSetting = config.AppSettings.Settings["DevAutoLogin.Account"];
                        var userIdSetting = config.AppSettings.Settings["DevAutoLogin.UserID"];

                        if (accountSetting != null && userIdSetting != null)
                        {
                            string devAccount = accountSetting.Value;
                            int devUserId = int.Parse(userIdSetting.Value);

                            // 設定權限
                            List<string> perms = SysUserHelper.GetOSIPermsByAccount(devAccount);
                            SessionHelper.Set(SessionHelper.UserPermissions, perms);

                            // 設定使用者資訊
                            var userInfoTemp = new SessionHelper.UserInfoClass();
                            var tbl = SysUserHelper.QueryUserInfoByID(devUserId);
                            if (tbl != null && tbl.Rows.Count > 0)
                            {
                                var data = tbl.Rows[0];
                                userInfoTemp.UserID = data["UserID"].ToString();
                                userInfoTemp.Account = data["Account"].ToString();
                                userInfoTemp.UserName = data["Name"].ToString();
                                userInfoTemp.UnitID = data["UnitID"].ToString();
                                userInfoTemp.UnitType = data["UnitType"].ToString();
                                userInfoTemp.UnitName = SysUserHelper.QueryUnitNameByUserID(userInfoTemp.UserID);
                                userInfoTemp.OSI_RoleName = data["OSI_RoleName"].ToString();

                                // 查詢使用者的 OFS 角色名稱
                                var ofsRoleTable = OFSRoleHelper.QueryByUserID(userInfoTemp.UserID);
                                if (ofsRoleTable != null && ofsRoleTable.Rows.Count > 0)
                                {
                                    userInfoTemp.OFS_RoleName = ofsRoleTable.Rows.Cast<DataRow>()
                                        .Select(row => row["RoleName"].ToString())
                                        .ToArray();
                                }
                                else
                                {
                                    userInfoTemp.OFS_RoleName = new string[0];
                                }
                            }
                            SessionHelper.Set(SessionHelper.UserInfo, userInfoTemp);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 開發者設定檔讀取失敗，記錄錯誤但不中斷程式
                    System.Diagnostics.Debug.WriteLine("讀取 dev.config 失敗: " + ex.Message);
                }
            }
        }

    }

    void Session_End(object sender, EventArgs e)
    {
        // 在工作階段結束時執行的程式碼
        // 注意: 只有在  Web.config 檔案中將 sessionstate 模式設定為 InProc 時，
        // 才會引起 Session_End 事件。如果將 session 模式設定為 StateServer 
        // 或 SQLServer，則不會引起該事件。

    }

    /// <summary>
    /// 每次 Request 在載入 Session 之後觸發，你可以在這裡檢查 Session["UserPermissions"] 權限
    /// </summary>
    void Application_AcquireRequestState(object sender, EventArgs e)
    {
        var ctx = HttpContext.Current;

        // 只檢查 .aspx 請求 (跳過 .css/.js/.png)
        var path = VirtualPathUtility.ToAppRelative(ctx.Request.Path);  // e.g. "~/Manage/Users.aspx"
        if (!path.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase)) return;

        // 排除不需檢查的頁面(登入頁、錯誤頁)
        if (path.EndsWith("/Login.aspx", StringComparison.OrdinalIgnoreCase) ||
            path.EndsWith("/Error.aspx", StringComparison.OrdinalIgnoreCase)||
            path.EndsWith("/SciDomainReview.aspx", StringComparison.OrdinalIgnoreCase)||
            path.EndsWith("/SciExamReview.aspx", StringComparison.OrdinalIgnoreCase))
            return;

        // 讀取 Session
        var perms = SessionHelper.Get<List<string>>(SessionHelper.UserPermissions);
        if (perms == null)
        {
            // Session 過期或尚未登入 → 導到登入
            Env.Log.Info($"Session 過期或尚未登入，導向登入頁面。Request Path: {path}");
            ctx.Response.Redirect("~/Login.aspx");
            return;
        }

        // 權限不足，導到無權限頁面(若頁面無權限控管，則可以進入)
        string pageCode = SysPermissionHelper.QueryPermissionCodeByUrl(path);
        if (pageCode != null && !perms.Contains(pageCode))
        {
            Env.Log.Info($"權限不足，導向預設頁面。Request Path: {path}, PageCode: {pageCode}");
            ctx.Response.Redirect("~/Default.aspx");
            return;
        }
    }

</script>
