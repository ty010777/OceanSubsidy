using System;
using System.Globalization;
using System.Text;
using GS.Extension;
using log4net;
using Org.BouncyCastle.Crypto.Digests;

namespace GS.App
{
    public class Utility
    {
        /// <summary>
        /// 字串相關
        /// </summary>
        public class String
        {

            /// <summary>
            /// 取得以時間序的惟一值作為ID
            /// 年月日_時分秒_6碼隨機大寫英文+數字(20180617_061430_XYZ123)
            /// </summary>
            /// <returns></returns>
            public static string GetTimeUniqueId()
            {
                DateTime _DateTimeNow = DateTime.Now;
                string _yyyyMMdd = _DateTimeNow.ToString("yyyyMMdd");
                string _HHmmssfff = _DateTimeNow.ToString("HHmmssfff");
                return $"{_yyyyMMdd}_{_HHmmssfff}_{GetRandomString(6, 5)}";
            }

            private static readonly Random _RandomString = new Random();

            /// <summary>
            /// 取得隨機字串
            /// </summary>
            /// <param name="length">字串的長度，必須大於 0。</param>
            /// <param name="mod">
            /// 決定字串的格式：
            /// 1 - (A-Z)
            /// 2 - (a-z)
            /// 3 - (0-9)
            /// 4 - (A-Z)+(a-z)+(0-9)
            /// 5 - (A-Z)+(0-9)
            /// </param>
            /// <returns>指定格式與長度的隨機字串。</returns>
            /// <exception cref="ArgumentException">
            /// 當 <paramref name="length"/> 小於等於 0，或 <paramref name="mod"/> 不在 1 到 5 的範圍內時拋出。
            /// </exception>
            public static string GetRandomString(int length, int mod = 0)
            {
                if (length <= 0)
                {
                    throw new ArgumentException("取得隨機字串的長度(length)不可為零。");
                }

                string chars;
                switch (mod)
                {
                    case 1:
                        // 純大寫英文(A-Z)
                        chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                        break;
                    case 2:
                        // 純小寫英文(a-z)
                        chars = "abcdefghijklmnopqrstuvwxyz";
                        break;
                    case 3:
                        // 純數字(0-9)
                        chars = "0123456789";
                        break;
                    case 4:
                        // 混合型大小寫英文+數字(A-Z)+(a-z)+(0-9)
                        chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                        break;
                    case 5:
                        // 混合型大寫英文+數字(A-Z)+(0-9)
                        chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                        break;
                    default:
                        throw new ArgumentException("無效的 mod 值。有效值為 1 至 5。");
                }

                StringBuilder result = new StringBuilder(length);

                for (int i = 0; i < length; i++)
                {
                    result.Append(chars[_RandomString.Next(chars.Length)]);
                }

                return result.ToString();
            }
        }

        /// <summary>
        /// 加密相關
        /// </summary>
        public class Cryptography
        {
            /// <summary>
            /// SHA-256 雜湊加密
            /// </summary>
            public class HASH {

                /// <summary>
                /// 使用 SHA-256 進行雜湊加密
                /// </summary>
                /// <param name="input">要加密的字串</param>
                /// <returns>雜湊後的字串</returns>
                public static string Hash(string input)
                {
                    if (string.IsNullOrEmpty(input))
                        throw new ArgumentException("Input Required!", nameof(input));

                    var digest = new Sha256Digest();
                    var inputBytes = Encoding.UTF8.GetBytes(input);
                    digest.BlockUpdate(inputBytes, 0, inputBytes.Length);
                    var hashBytes = new byte[digest.GetDigestSize()];
                    digest.DoFinal(hashBytes, 0);

                    return Convert.ToBase64String(hashBytes);
                }

                /// <summary>
                /// 驗證輸入的字串是否與雜湊值匹配
                /// </summary>
                /// <param name="input">要驗證的字串</param>
                /// <param name="hash">雜湊值</param>
                /// <returns>是否匹配</returns>
                public static bool VerifyHash(string input, string hash)
                {
                    if (string.IsNullOrEmpty(input))
                        throw new ArgumentException("Input Required!", nameof(input));

                    if (string.IsNullOrEmpty(hash))
                        throw new ArgumentException("Hash Required!", nameof(hash));

                    var inputHash = Hash(input);
                    return inputHash == hash;
                }
            }

            /// <summary>
            /// AES-GCM 加解密
            /// </summary>
            public class AESGCM
            {
                public static string EncryptText(string plaintext, string salt = "")
                {
                    string key = null;
                    try
                    {
                        key = Env.C_AESGCM_SECRET_KEY;
                        return Core.AESGCM.Encrypt(plaintext, key, salt);
                    }
                    finally
                    {
                        key = null; // 協助 GC 清除 string 物件
                    }
                }


                public static string DecryptText(string ciphertext, string salt = "")
                {
                    string result = "解密失敗。Decrypt error";
                    string key = null;
                    try
                    {
                        key = Env.C_AESGCM_SECRET_KEY;
                        result = Core.AESGCM.Decrypt(ciphertext, key, salt);
                    }
                    catch (Exception ex)
                    {
                        //result = ex.Message;
                    }
                    finally
                    {
                        key = null; // 協助 GC 清除 string 物件
                    }
                    return result;
                }
            }

            /// <summary>
            /// Aes 加解密
            /// </summary>
            public class Aes
            {
                /// <summary>
                /// 基本資料加密
                /// </summary>
                /// <param name="plan_id"></param>
                /// <returns></returns>
                public static string EncryptText(string plaintext)
                {

                    return plaintext.AesEnCrypto(Env.C_ASE_CRYPTR_KEY, Env.C_ASE_CRYPTR_IV);
                }


                /// <summary>
                /// 基本資料解密
                /// </summary>
                /// <param name="ciphertext"></param>
                /// <returns></returns>
                public static string DecryptText(string ciphertext)
                {
                    return ciphertext.AesDeCrypto(Env.C_ASE_CRYPTR_KEY, Env.C_ASE_CRYPTR_IV);
                }
            }
        }

        /// <summary>
        /// 信件相關
        /// </summary>
        public class Mail
        {
            private static readonly ILog log = LogManager.GetLogger(typeof(Mail));

            /// <summary>
            /// 寄發EMAIL
            /// </summary>
            /// <param name="MailReceiverTo"></param>
            /// <param name="MailReceiverCc"></param>
            /// <param name="Subject"></param>
            /// <param name="Body"></param>
            /// <param name="ErrorMsg"></param>
            /// <returns></returns>
            public static bool SendMail(string MailReceiverTo, string MailReceiverCc, string Subject, string Body, out string ErrorMsg)
            {
                ErrorMsg = "";
                string MailSender = Env.MailServerLoginUser;

                log.Info($"[寄信開始] 收件者: {MailReceiverTo}, 副本: {MailReceiverCc}, 主旨: {Subject}");

                try
                {
                    if (Env.MailServerPort == "465" || Env.MailServerPort == "25")
                    {
                        //使用 CDO.Message
                        CdoMail CdoMail1 = new CdoMail();

                        // ------ 寄件者 ------
                        CdoMail1.Set_FROM(MailSender);

                        //------ 收件者/副本/密件副本 ------
                        CdoMail1.Add_TO(MailReceiverTo);
                        CdoMail1.Add_CC(MailReceiverCc);
                        CdoMail1.Add_BCC(Env.MailReceiverBcc);

                        //------ 主旨 ------ 主旨文字, 主旨內容所用的編碼方式
                        CdoMail1.Set_SUBJECT(ref Subject);

                        //------ 內文 ------ 內文文字, 內文內容所用的編碼方式, 內文是否為HTML格式
                        CdoMail1.Set_BODY(ref Body, "utf-8", true);

                        //------ 附件 ------

                        //------ 寄送 ------ MailServer的IP或名稱, MailServer的port, 登入MailServer的ID, 登入MailServer的Password
                        CdoMail1.SecurityProtocol = Env.SecurityProtocol;
                        CdoMail1.Send(Env.MailServerHost, Env.MailServerPort, Env.MailServerLoginUser, Env.MailServerLoginPwd);

                        log.Info($"[寄信成功] 收件者: {MailReceiverTo}, 主旨: {Subject}, 方式: CDO.Message");
                    }
                    else
                    {
                        //使用 MailMessage
                        EzMailSender MailSender1 = new EzMailSender();

                        // ------ 寄件者 ------
                        MailSender1.Set_FROM(MailSender);

                        //------ 收件者/副本/密件副本 ------
                        MailSender1.Add_TO(MailReceiverTo);
                        MailSender1.Add_CC(MailReceiverCc);
                        MailSender1.Add_BCC(Env.MailReceiverBcc);

                        //------ 主旨 ------ 主旨文字, 主旨內容所用的編碼方式
                        MailSender1.Set_SUBJECT(ref Subject, Env.EnumEncoding.UTF8);

                        //------ 內文 ------ 內文文字, 內文內容所用的編碼方式, 內文是否為HTML格式
                        MailSender1.Set_BODY(ref Body, Env.EnumEncoding.UTF8, true);

                        //------ 附件 ------ 檔案路徑及名稱, 郵件附件的配置類型(內嵌:Inline或附加:Attachment)
                        //MailSender1.Add_Attachments("C:\\TEMP\\test.zip", EasyMailSender.EnumDispositionType.Attachment);

                        //------ 寄送 ------ MailServer的IP或名稱, MailServer的port, 登入MailServer的ID, 登入MailServer的Password
                        int port = 25;
                        if (!int.TryParse(Env.MailServerPort, out port)) { port = 25; }
                        MailSender1.TLSorSSL = Env.SecurityProtocol;
                        string sendResult = MailSender1.Send(Env.MailServerHost, port, Env.MailServerLoginUser, Env.MailServerLoginPwd);

                        if (!string.IsNullOrEmpty(sendResult))
                        {
                            ErrorMsg = sendResult;
                            log.Warn($"[寄信警告] 收件者: {MailReceiverTo}, 主旨: {Subject}, 回傳訊息: {sendResult}");
                        }
                        else
                        {
                            log.Info($"[寄信成功] 收件者: {MailReceiverTo}, 主旨: {Subject}, 方式: MailMessage");
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    ErrorMsg = ex.Message;
                    log.Error($"[寄信失敗] 收件者: {MailReceiverTo}, 主旨: {Subject}, 錯誤: {ex.Message}", ex);
                    return false;
                }
            }
        }
    }
}