using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 統一管理所有 Session 名稱，以及提供簡單的 Get/Set/Remove 方法。
/// </summary>
public class SessionHelper
{
    public SessionHelper()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }

    // ── 一、所有的 Key 常數 ──────────────────────────
    /// <summary>登入驗證碼</summary>
    public const string LoginValidate = "LoginValidate";

    /// <summary>儲存使用者的權限列表</summary>
    public const string UserPermissions = "UserPermissions";

    /// <summary>儲存使用者資訊</summary>
    public const string UserInfo = "UserInfo";


    // ── 二、簡單的 Get/Set/Remove 方法 ───────────────────

    public static void Set<T>(string key, T value)
    {
        System.Web.HttpContext.Current.Session[key] = value;
    }

    public static T Get<T>(string key)
    {
        var obj = System.Web.HttpContext.Current.Session[key];
        if (obj == null) return default;
        return (T)obj;
    }

    public static void Remove(string key)
    {
        System.Web.HttpContext.Current.Session.Remove(key);
    }

    public class UserInfoClass
    {
        public string UserID { get; set; }
        public string Account { get; set; }
        public string UserName { get; set; }
        public string UnitID { get; set; }
        public string UnitType { get; set; }
        public string UnitName { get; set; }
        public string OSI_RoleName { get; set; }
    }
}