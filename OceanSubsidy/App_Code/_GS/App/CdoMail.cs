using System;
using System.Collections.Generic;

public class CdoMail
{
    public string SecurityProtocol = "";
    private CDO.Message objEmail;
    private List<string> _TO;
    private List<string> _CC;
    private List<string> _BCC;


    /// <summary>
    /// 
    /// </summary>
    public CdoMail()
    {
        objEmail = new CDO.Message();
        _TO = new List<string>();
        _CC = new List<string>();
        _BCC = new List<string>();
    }


    /// <summary>設定寄件者</summary>
    /// <param name="MailFromAddress">寄件者電子郵件地址</param>
    /// <param name="MailFromName">寄件者名稱</param>
    public void Set_FROM(string MailFromAddress)
    {
        objEmail.From = MailFromAddress;
    }

    /// <summary>設定寄件者</summary>
    /// <param name="MailFromAddress">寄件者電子郵件地址</param>
    /// <param name="MailFromName">寄件者名稱</param>
    public void Set_FROM(string MailFromAddress, string MailFromName)
    {
        objEmail.From = string.Format("{0} <{1}>", MailFromName, MailFromAddress);
    }


    /// <summary>設定收件者</summary>
    /// <param name="MailToAddress">收件者電子郵件地址</param>
    public void Add_TO(string MailToAddresses)
    {
        string[] MailToAddressArray = MailToAddresses.Split(';');
        if (MailToAddressArray != null)
        {
            for (int i = 0; i < MailToAddressArray.Length; i++)
            {
                if (!string.IsNullOrEmpty(MailToAddressArray[i].Trim()))
                {
                    _TO.Add(MailToAddressArray[i].Trim());
                }
            }
        }
    }

    /// <summary>設定收件者</summary>
    /// <param name="MailToAddress">收件者電子郵件地址</param>
    /// <param name="MailToName">收件者名稱</param>
    public void Add_TO(string MailToAddress, string MailToName)
    {
        _TO.Add(string.Format("{0} <{1}>", MailToName, MailToAddress));
    }

    /// <summary>清除收件者</summary>
    public void Clear_TO()
    {
        _TO.Clear();
    }

    /// <summary>設定副本</summary>
    /// <param name="MailCCAddress">副本電子郵件地址</param>
    public void Add_CC(string MailCCAddress)
    {
        string[] MailCcAddressArray = MailCCAddress.Split(';');
        if (MailCcAddressArray != null)
        {
            for (int i = 0; i < MailCcAddressArray.Length; i++)
            {
                if (!string.IsNullOrEmpty(MailCcAddressArray[i].Trim()))
                {
                    _CC.Add(MailCcAddressArray[i].Trim());
                }
            }
        }
    }

    /// <summary>設定副本</summary>
    /// <param name="MailCCAddress">副本電子郵件地址</param>
    /// <param name="MailCCName">副本名稱</param>
    public void Add_CC(string MailCCAddress, string MailCCName)
    {
        _CC.Add(string.Format("{0} <{1}>", MailCCName, MailCCAddress));
    }

    /// <summary>清除副本</summary>
    public void Clear_CC()
    {
        _CC.Clear();
    }

    /// <summary>設定密件副本</summary>
    /// <param name="MailBCCAddress">密件副本電子郵件地址</param>
    public void Add_BCC(string MailBCCAddress)
    {
        string[] MailBccAddressArray = MailBCCAddress.Split(';');
        if (MailBccAddressArray != null)
        {
            for (int i = 0; i < MailBccAddressArray.Length; i++)
            {
                if (!string.IsNullOrEmpty(MailBccAddressArray[i].Trim()))
                {
                    _BCC.Add(MailBccAddressArray[i].Trim());
                }
            }
        }
    }

    /// <summary>設定密件副本</summary>
    /// <param name="MailBCCAddress">密件副本電子郵件地址</param>
    /// <param name="MailBCCName">密件副本名稱</param>
    public void Add_BCC(string MailBCCAddress, string MailBCCName)
    {
        _BCC.Add(string.Format("{0} <{1}>", MailBCCName, MailBCCAddress));
    }

    /// <summary>清除密件副本</summary>
    public void Clear_BCC()
    {
        _BCC.Clear();
    }

    /// <summary>設定主旨</summary>
    /// <param name="MailSubject">主旨文字</param>
    /// <param name="SubjectEncoding">主旨文字編碼方式</param>
    public void Set_SUBJECT(ref string MailSubject)
    {
        objEmail.Subject = MailSubject;
    }

    /// <summary>設定內文</summary>
    /// <param name="MailBody">內文文字</param>
    /// <param name="BodyCharset">內文文字編碼方式</param>
    /// <param name="IsBodyHtml"></param>
    public void Set_BODY(ref string MailBody, string BodyCharset = "utf-8", bool IsBodyHtml = true)
    {
        if (IsBodyHtml)
        {
            objEmail.TextBody = "";
            objEmail.HTMLBody = MailBody;
        }
        else
        {
            objEmail.HTMLBody = "";
            objEmail.TextBody = MailBody;
        }
        objEmail.BodyPart.Charset = BodyCharset;
    }

    /// <summary>加入附件</summary>
    /// <param name="FilePATH_and_FULLNAME">附件檔案路徑及檔名</param>
    /// <param name="DispositionType">附件加入方式</param>
    public void Add_Attachments(string FilePATH_and_FULLNAME, string Charset = "utf-8", string ContentMediaType = "text/plain")
    {
        CDO.IBodyPart iBody = objEmail.AddAttachment(FilePATH_and_FULLNAME);
        iBody.Charset = Charset;
        iBody.ContentMediaType = ContentMediaType;
    }

    /// <summary>寄出郵件</summary>
    /// <param name="MailServerIP_or_Name">郵件伺服器IP或DNS</param>
    public void Send(string MailServerIP_or_Name)
    {
        objEmail.To = GetAddresseeList(_TO);
        objEmail.CC = GetAddresseeList(_CC);
        objEmail.BCC = GetAddresseeList(_BCC);
        ADODB.Fields oFields = objEmail.Configuration.Fields;

        oFields["http://schemas.microsoft.com/cdo/configuration/sendusing"].Value = "2";
        oFields["http://schemas.microsoft.com/cdo/configuration/smtpserver"].Value = MailServerIP_or_Name;
        oFields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"].Value = "25";

        oFields.Update();
        objEmail.Send();
    }

    /// <summary>寄出郵件</summary>
    /// <param name="MailServerIP_or_Name">郵件伺服器IP或DNS</param>
    /// <param name="MailServerPort">郵件伺服器連接埠</param>
    /// <param name="UserID">登入郵件伺服器帳號</param>
    /// <param name="UserPasswd">登入郵件伺服器密碼</param>
    public void Send(string MailServerIP_or_Name, string MailServerPort, string UserID, string UserPasswd)
    {
        objEmail.To = GetAddresseeList(_TO);
        objEmail.CC = GetAddresseeList(_CC);
        objEmail.BCC = GetAddresseeList(_BCC);
        ADODB.Fields oFields = objEmail.Configuration.Fields;

        oFields["http://schemas.microsoft.com/cdo/configuration/sendusing"].Value = "2";
        oFields["http://schemas.microsoft.com/cdo/configuration/smtpserver"].Value = MailServerIP_or_Name;
        oFields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"].Value = MailServerPort;
        if (SecurityProtocol != "")
        {
            oFields["http://schemas.microsoft.com/cdo/configuration/smtpusessl"].Value = "1";
        }
        if (UserID != "")
        {
            oFields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"].Value = "1";
            oFields["http://schemas.microsoft.com/cdo/configuration/sendusername"].Value = UserID;
            oFields["http://schemas.microsoft.com/cdo/configuration/sendpassword"].Value = UserPasswd;
        }
        oFields.Update();
        objEmail.Send();
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

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_myList"></param>
    /// <returns></returns>
    public string GetAddresseeList(List<string> _myList)
    {
        string RtVal = "";
        for (int i = 0; i < _myList.Count; i++)
        {
            if (i > 0)
            {
                RtVal += ";";
            }
            RtVal += _myList[i];
        }
        return RtVal;
    }
}
