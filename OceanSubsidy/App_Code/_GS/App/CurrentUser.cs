public class CurrentUser
{
    public static string Account {
        get
        {
            return SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo).Account;
        }
    }

    public static string ID {
        get
        {
            return SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo).UserID;
        }
    }

    public static string UnitName {
        get
        {
            return SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo).UnitName;
        }
    }

    public static string UserName {
        get
        {
            return SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo).UserName;
        }
    }
}
