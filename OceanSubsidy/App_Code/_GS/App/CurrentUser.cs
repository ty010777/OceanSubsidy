public class CurrentUser
{
    public static string ID {
        get
        {
            return SessionHelper.Get<SessionHelper.UserInfoClass>(SessionHelper.UserInfo).UserID;
        }
    }
}
