using System.Linq;

public class CurrentUser
{
    public static string Account
    {
        get
        {
            return SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo).Account;
        }
    }

    public static string ID
    {
        get
        {
            return SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo).UserID;
        }
    }

    public static bool IsOrganizer
    {
        get
        {
            return SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo).OFS_RoleName.Contains("主管單位人員");
        }
    }

    public static bool IsSupervisor
    {
        get
        {
            return SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo).OFS_RoleName.Contains("主管單位窗口");
        }
    }

    public static bool IsSysAdmin
    {
        get
        {
            return SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo).OFS_RoleName.Contains("系統管理者");
        }
    }

    public static bool IsUser
    {
        get
        {
            return SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo).OFS_RoleName.Contains("申請者");
        }
    }

    public static string UnitName
    {
        get
        {
            return SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo).UnitName;
        }
    }

    public static string UserName
    {
        get
        {
            return SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo).UserName;
        }
    }
}
