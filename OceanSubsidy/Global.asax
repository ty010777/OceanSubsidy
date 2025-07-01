<%@ Application Language="C#" %>
<%@ Import Namespace="GS.Data.Sql" %>

<script RunAt="server">

    void Application_Start(object sender, EventArgs e)
    {
        // 應用程式啟動時執行的程式碼        

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

        // UserInfo Session過期自訂建立(開發若有需要可自行開啟)
        if (SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo) is null
            && true)
        {
            List<string> perms = SysUserHelper.GetOSIPermsByAccount("gregchen@geosense.tw");
            SessionHelper.Set(SessionHelper.UserPermissions, perms);

            var userInfoTemp = new SessionHelper.UserInfoClass();
            var tbl = SysUserHelper.QueryUserInfoByID(4);
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

            }
            SessionHelper.Set(SessionHelper.UserInfo, userInfoTemp);
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

        // 排除不需檢查的頁面(登入頁)
        if (path.EndsWith("/Login.aspx", StringComparison.OrdinalIgnoreCase))
            return;

        // 讀取 Session
        var perms = SessionHelper.Get<List<string>>(SessionHelper.UserPermissions);
        if (perms == null)
        {
            // Session 過期或尚未登入 → 導到登入
            ctx.Response.Redirect("~/Login.aspx");
            return;
        }

        // 權限不足，導到無權限頁面(若頁面無權限控管，則可以進入)
        string pageCode = SysPermissionHelper.QueryPermissionCodeByUrl(path);
        if (pageCode != null && !perms.Contains(pageCode))
        {
            if (path.StartsWith("/Manage"))
            {
                // 管理相關頁面，回到預設頁面
                ctx.Response.Redirect("~/Default.aspx");
            }
            else
            {
                ctx.Response.Redirect("~/Default.aspx");
            }
            return;
        }
    }

</script>
