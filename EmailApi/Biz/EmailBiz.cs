using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace EmailApi.Biz
{
    public class EmailBiz
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailBiz(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmailAsync(string to, string cc, string bcc, string subject, string body)
        {
            using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                Credentials = new NetworkCredential(_smtpSettings.UserName, _smtpSettings.Password),
                EnableSsl = _smtpSettings.EnableSsl
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.From, _smtpSettings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            if (!string.IsNullOrWhiteSpace(to))
            {
                foreach (var mail in to.Split(','))
                {
                    mailMessage.To.Add(mail);
                }
            }

            if (!string.IsNullOrWhiteSpace(cc))
            {
                foreach (var mail in cc.Split(','))
                {
                    mailMessage.CC.Add(mail);
                }
            }

            if (!string.IsNullOrWhiteSpace(bcc))
            {
                foreach (var mail in bcc.Split(','))
                {
                    mailMessage.Bcc.Add(mail);
                }
            }

            await client.SendMailAsync(mailMessage);
        }
    }

    public class SmtpSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string From { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
