using System.Configuration;

//環境變數
public class Env
{
    #region "列舉資料(Enum)"
    /// <summary>
    /// 編碼類型
    /// </summary>
    public enum EnumEncoding : int
    {
        /// <summary>預設</summary>
        DefaultValue = 0,
        /// <summary>UTF8</summary>
        UTF8 = 1,
        /// <summary>Unicode</summary>
        Unicode = 2,
        /// <summary>UTF32</summary>
        UTF32 = 3
    };


    #endregion


    #region 常數資料(Constants)
    /* 系統通用之常數資料 命名 : C_<Name> */

    /// <summary>ASE加解密的KEY 每個專案自行產生不同的key</summary>
    public const string C_ASE_CRYPTR_KEY = @"8QyHSpslbeNyixQPpjvAXtkDWLFqfok8V6H04CvWjw8=";

    /// <summary>ASE加解密的IV</summary>
    public const string C_ASE_CRYPTR_IV = @"P4YK1XHpwj5Ea/gJp1weKg==";

    /// <summary>AESGCM加解密</summary>
    public const string C_AESGCM_SECRET_KEY = @"orbD1OX2eJAJh2VDIv7cuquvzeAyNUZ5mXl5iZl4/bo=";


    #endregion

    #region Settings in Web.config 

    /// <summary>預設的連線字串</summary>
    public static string S_DefaultConnection
    {
        get { return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString; }
    }

    public static string IsOnline
    {
        get { return ConfigurationManager.AppSettings["IsOnline"]; }
    }

    /// <summary>
    /// 寄發EMAIL 
    /// </summary>
    public static string MailServerHost
    {
        get { return ConfigurationManager.AppSettings["MailServerHost"]; }
    }

    public static string SecurityProtocol
    {
        get { return ConfigurationManager.AppSettings["SecurityProtocol"]; }
    }
    public static string MailServerPort
    {
        get { return ConfigurationManager.AppSettings["MailServerPort"]; }
    }
    public static string MailServerLoginUser
    {
        get { return ConfigurationManager.AppSettings["MailServerLoginUser"]; }
    }
    public static string MailServerLoginPwd
    {
        get { return ConfigurationManager.AppSettings["MailServerLoginPwd"]; }
    }
    public static string MailReceiverBcc
    {
        get { return ConfigurationManager.AppSettings["MailReceiverBcc"]; }
    }

    public static log4net.ILog Log
    {
        get
        {
            return log4net.LogManager.GetLogger(new System.Diagnostics.StackFrame(1).GetMethod().DeclaringType);
        }
    }

    #endregion


}
