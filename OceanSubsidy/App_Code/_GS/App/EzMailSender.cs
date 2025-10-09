using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;


public class EzMailSender
{
    /// <summary>附件加入方式</summary>
    public enum EnumDispositionType : int
    {
        /// <summary>附加</summary>
        Attachment = 0,
        /// <summary>內嵌</summary>
        Inline = 1
    };
    public string TLSorSSL = "";

    /// <summary></summary>
    private MailMessage MailMessage1;

    /// <summary>建構函式</summary>
    public EzMailSender()
    {
        MailMessage1 = new MailMessage();
    }

    /// <summary>設定寄件者</summary>
    /// <param name="MailFromAddress">寄件者電子郵件地址</param>
    public void Set_FROM(string MailFromAddress)
    {
        MailMessage1.From = new MailAddress(MailFromAddress);
    }

    /// <summary>設定寄件者</summary>
    /// <param name="MailFromAddress">寄件者電子郵件地址</param>
    /// <param name="MailFromName">寄件者名稱</param>
    public void Set_FROM(string MailFromAddress, string MailFromName)
    {
        MailMessage1.From = new MailAddress(MailFromAddress, MailFromName);
    }


    /// <summary>設定收件者</summary>
    /// <param name="MailToAddress">收件者電子郵件地址</param>
    public void Add_TO(string MailToAddresses)
    {
        string[] MailToAddress = MailToAddresses.Split(';');
        if (MailToAddress != null)
        {
            for (int i = 0; i < MailToAddress.Length; i++)
            {
                if (!string.IsNullOrEmpty(MailToAddress[i].Trim()))
                {
                    MailMessage1.To.Add(new MailAddress(MailToAddress[i].Trim()));
                }
            }
        }
    }

    /// <summary>設定收件者</summary>
    /// <param name="MailToAddress">收件者電子郵件地址</param>
    /// <param name="MailToName">收件者名稱</param>
    public void Add_TO(string MailToAddress, string MailToName)
    {
        MailMessage1.To.Add(new MailAddress(MailToAddress, MailToName));
    }

    /// <summary>清除收件者</summary>
    public void Clear_TO()
    {
        MailMessage1.To.Clear();
    }

    /// <summary>設定副本</summary>
    /// <param name="MailCCAddress">副本電子郵件地址</param>
    public void Add_CC(string MailCCAddresses)
    {
        string[] MailCCAddress = MailCCAddresses.Split(';');
        if (MailCCAddress != null)
        {
            for (int i = 0; i < MailCCAddress.Length; i++)
            {
                if (!string.IsNullOrEmpty(MailCCAddress[i].Trim()))
                {
                    MailMessage1.CC.Add(new MailAddress(MailCCAddress[i].Trim()));
                }
            }
        }
    }

    /// <summary>設定副本</summary>
    /// <param name="MailCCAddress">副本電子郵件地址</param>
    /// <param name="MailCCName">副本名稱</param>
    public void Add_CC(string MailCCAddress, string MailCCName)
    {
        MailMessage1.CC.Add(new MailAddress(MailCCAddress, MailCCName));
    }

    /// <summary>清除副本</summary>
    public void Clear_CC()
    {
        MailMessage1.CC.Clear();
    }


    /// <summary>設定密件副本</summary>
    /// <param name="MailBCCAddress">密件副本電子郵件地址</param>
    public void Add_BCC(string MailBccAddresses)
    {
        string[] MailBccAddress = MailBccAddresses.Split(';');
        if (MailBccAddress != null)
        {
            for (int i = 0; i < MailBccAddress.Length; i++)
            {
                if (!string.IsNullOrEmpty(MailBccAddress[i].Trim()))
                {
                    MailMessage1.Bcc.Add(new MailAddress(MailBccAddress[i].Trim()));
                }
            }
        }
    }

    /// <summary>設定密件副本</summary>
    /// <param name="MailBCCAddress">密件副本電子郵件地址</param>
    /// <param name="MailBCCName">密件副本名稱</param>
    public void Add_BCC(string MailBCCAddress, string MailBCCName)
    {
        MailMessage1.Bcc.Add(new MailAddress(MailBCCAddress, MailBCCName));
    }

    /// <summary>清除密件副本</summary>
    public void Clear_BCC()
    {
        MailMessage1.Bcc.Clear();
    }

    /// <summary>設定主旨</summary>
    /// <param name="MailSubject">主旨文字</param>
    /// <param name="SubjectEncoding">主旨文字編碼方式</param>
    public void Set_SUBJECT(ref string MailSubject, Env.EnumEncoding SubjectEncoding)
    {
        MailMessage1.Subject = MailSubject;
        MailMessage1.SubjectEncoding = GetEncoding(ref SubjectEncoding);
    }

    /// <summary>設定內文</summary>
    /// <param name="MailBody">內文文字</param>
    /// <param name="BodyEncoding">內文文字編碼方式</param>
    public void Set_BODY(ref string MailBody, Env.EnumEncoding BodyEncoding)
    {
        MailMessage1.Body = MailBody;
        MailMessage1.BodyEncoding = GetEncoding(ref BodyEncoding);
        MailMessage1.IsBodyHtml = true;
    }

    /// <summary>設定內文</summary>
    /// <param name="MailBody">內文文字</param>
    /// <param name="BodyEncoding">內文文字編碼方式</param>
    /// <param name="IsBodyHtml">內文文字是否為HTML格式</param>
    public void Set_BODY(ref string MailBody, Env.EnumEncoding BodyEncoding, bool IsBodyHtml)
    {
        MailMessage1.Body = MailBody;
        MailMessage1.BodyEncoding = GetEncoding(ref BodyEncoding);
        MailMessage1.IsBodyHtml = IsBodyHtml;
    }

    /// <summary>加入附件</summary>
    /// <param name="FilePATH_and_FULLNAME">附件檔案路徑及檔名</param>
    /// <param name="DispositionType">附件加入方式</param>
    public void Add_Attachments(string FilePATH_and_FULLNAME, EnumDispositionType DispositionType)
    {
        Attachment File1 = new Attachment(FilePATH_and_FULLNAME, MediaTypeNames.Application.Octet);
        // Add time stamp information for the file.
        ContentDisposition disposition1 = File1.ContentDisposition;
        disposition1.CreationDate = System.IO.File.GetCreationTime(FilePATH_and_FULLNAME);
        disposition1.ModificationDate = System.IO.File.GetLastWriteTime(FilePATH_and_FULLNAME);
        disposition1.ReadDate = System.IO.File.GetLastAccessTime(FilePATH_and_FULLNAME);
        switch (DispositionType)
        {
            case EnumDispositionType.Attachment:
                disposition1.Inline = false;
                break;
            case EnumDispositionType.Inline:
                disposition1.Inline = true;
                break;
            default:
                disposition1.Inline = false;
                break;
        }
        // Add the file attachment to this e-mail message.
        MailMessage1.Attachments.Add(File1);
    }

    /// <summary>寄出郵件</summary>
    /// <param name="MailServerIP_or_Name">郵件伺服器IP或DNS</param>
    /// <param name="MailServerPort">郵件伺服器連接埠</param>
    /// <param name="UserID">登入郵件伺服器帳號</param>
    /// <param name="UserPasswd">登入郵件伺服器密碼</param>
    public string Send(string MailServerIP_or_Name, int MailServerPort, string UserID, string UserPasswd)
    {
        string OutPutMessage = "";
        SmtpClient Client1 = new SmtpClient(MailServerIP_or_Name, MailServerPort);

        if (TLSorSSL != "")
        {
            Client1.EnableSsl = true;
            // 這段一定要, 要寫這個才可以跳過 "根據驗證程序,遠端憑證是無效的" 的錯誤 
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);
            if (TLSorSSL == "TLS")
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            }
        }

        if (UserID != "")
        {
            try
            {
                Client1.UseDefaultCredentials = true;
                Client1.Credentials = new NetworkCredential(UserID, UserPasswd);
                Client1.Send(MailMessage1);
            }
            catch (Exception ex1)
            {
                OutPutMessage = "Failure:" + ex1.Message;
                try
                {
                    Client1.Credentials = new NetworkCredential(UserID, UserPasswd, MailServerIP_or_Name);
                    Client1.Send(MailMessage1);
                }
                catch (Exception ex2)
                {
                    OutPutMessage = "Failure:" + ex2.Message;
                }
            }
        }
        else
        {
            try
            {
                Client1.Send(MailMessage1);
            }
            catch (Exception ex3)
            {
                OutPutMessage = "Failure:" + ex3.Message;
            }
        }

        Client1 = null;
        return OutPutMessage;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="certificate"></param>
    /// <param name="chain"></param>
    /// <param name="sslPolicyErrors"></param>
    /// <returns></returns>
    public static bool ValidateServerCertificate(Object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

    /// <summary></summary>
    public void Dispose() // destructor
    {
        MailMessage1.Dispose();
    }

    /// <summary></summary>
    /// <param name="tmp_EnumEncoding"></param>
    /// <returns></returns>
    private System.Text.Encoding GetEncoding(ref Env.EnumEncoding tmp_EnumEncoding)
    {
        System.Text.Encoding tmp_Encoding;
        switch (tmp_EnumEncoding)
        {
            case Env.EnumEncoding.DefaultValue:
                tmp_Encoding = System.Text.Encoding.Default;
                break;
            case Env.EnumEncoding.UTF8:
                tmp_Encoding = System.Text.Encoding.UTF8;
                break;
            case Env.EnumEncoding.Unicode:
                tmp_Encoding = System.Text.Encoding.Unicode;
                break;
            case Env.EnumEncoding.UTF32:
                tmp_Encoding = System.Text.Encoding.UTF32;
                break;
            default:
                tmp_Encoding = System.Text.Encoding.Default;
                break;
        }
        return tmp_Encoding;
    }
}

