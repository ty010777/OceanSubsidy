using System;
using System.IO;
using System.Web;

/// <summary>
/// MailContent 的摘要描述
/// </summary>
public class MailContent
{
    public MailContent()
    {
       
    }
    /// <summary>
    /// 信件通知
    /// </summary>
    public class OCA
    {
        public class PendingUser
        {
            public static string Subject = "海洋領域補助案網站 - 待審核帳號通知";
            public static string getMail(DateTime time)
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string templatePath = Path.Combine(baseDir, "App_Code", "_GS", "App", "MailContents", "PendingUser.html");
                return File.ReadAllText(templatePath)
                 .Replace("{{ApplyTime}}", time.ToString("yyyy/MM/dd HH:mm"));
            }
        }

        public class ResetPassword
        {
            public static string Subject = "海洋領域補助案網站 - 密碼重置通知";
            public static string getMail(string pwdToken)
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string templatePath = Path.Combine(baseDir, "App_Code", "_GS", "App", "MailContents", "ResetPassword.html");
                var request = HttpContext.Current.Request;
                string host = request.Url.GetLeftPart(UriPartial.Authority);
                string app = request.ApplicationPath.TrimEnd('/');
                string url = $"{host}{app}/Login.aspx?token={pwdToken}";
                return File.ReadAllText(templatePath)
                 .Replace("{{ResetTime}}", DateTime.Now.ToString("yyyy/MM/dd HH:mm"))
                 .Replace("{{ResetLink}}", url);
            }
        }

        public class SetPassword
        {
            public static string Subject = "海洋領域補助案網站 - 帳號申請結果通知";
            public static string getMail(string pwdToken)
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string templatePath = Path.Combine(baseDir, "App_Code", "_GS", "App", "MailContents", "SetPassword.html");
                var request = HttpContext.Current.Request;
                string host = request.Url.GetLeftPart(UriPartial.Authority);
                string app = request.ApplicationPath.TrimEnd('/');
                string url = $"{host}{app}/Login.aspx?token={pwdToken}";
                return File.ReadAllText(templatePath)
                 .Replace("{{ResetTime}}", DateTime.Now.ToString("yyyy/MM/dd HH:mm"))
                 .Replace("{{ResetLink}}", url);
            }
        }

        public class ApproveFail
        {
            public static string Subject = "海洋領域補助案網站 - 帳號申請結果通知";
            public static string getMail()
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string templatePath = Path.Combine(baseDir, "App_Code", "_GS", "App", "MailContents", "ApproveFail.html");
                return File.ReadAllText(templatePath)
                 .Replace("{{ResetTime}}", DateTime.Now.ToString("yyyy/MM/dd HH:mm"));
            }
        }

        public class UserVerification
        {
            public static string Subject = "海洋領域補助案網站 - 電子郵件信箱確認";
            public static string getMail(string code)
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string templatePath = Path.Combine(baseDir, "App_Code", "_GS", "App", "MailContents", "UserVerification.html");
                return File.ReadAllText(templatePath)
                 .Replace("{{ResetTime}}", DateTime.Now.ToString("yyyy/MM/dd HH:mm"))
                 .Replace("{{Code}}", code);
            }
        }

        // 啟用帳號，重設密碼
        public class ActiveAccount
        {
            public static string Subject = "海洋領域補助案網站 - 密碼設置通知信";
            public static string getMail(string account, string pwdToken)
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string templatePath = Path.Combine(baseDir, "App_Code", "_GS", "App", "MailContents", "ActiveAccount.html");
                var request = HttpContext.Current.Request;
                string host = request.Url.GetLeftPart(UriPartial.Authority);
                string app = request.ApplicationPath.TrimEnd('/');
                string url = $"{host}{app}/Login.aspx?token={pwdToken}";
                return File.ReadAllText(templatePath)
                 .Replace("{{ResetTime}}", DateTime.Now.ToString("yyyy/MM/dd HH:mm"))
                 .Replace("{{Account}}", account)
                 .Replace("{{ResetLink}}", url);
            }
        }

        // OSI 未填報通知
        public class OSI_UnFilledRemind
        {
            public static string Subject = "海洋領域補助案網站 - 未填報提醒通知";
            public static string getMail(string unitName, string year, string quarter)
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string templatePath = Path.Combine(baseDir, "App_Code", "_GS", "App", "MailContents", "OSI_UnFilledRemind.html");
                return File.ReadAllText(templatePath)
                 .Replace("{{UnitName}}", unitName)
                 .Replace("{{Year}}", year)
                 .Replace("{{Quarter}}", quarter);
            }
        }

        // OSI 開始填報通知
        public class OSI_StartFilledRemind
        {
            public static string Subject = "海洋領域補助案網站 - 開始填報提醒通知";
            public static string getMail(string unitName, string year, string quarter, string startDate, string endDate)
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string templatePath = Path.Combine(baseDir, "App_Code", "_GS", "App", "MailContents", "OSI_StartFilledRemind.html");
                return File.ReadAllText(templatePath)
                 .Replace("{{UnitName}}", unitName)
                 .Replace("{{Year}}", year)
                 .Replace("{{Quarter}}", quarter)
                 .Replace("{{StartDate}}", startDate)
                 .Replace("{{EndDate}}", endDate);
            }
        }


    }
}
