using System;
using System.Configuration;
using System.Net.Mail;

namespace Fildela.Business.Helpers
{
    public static class BusinessSMTPExtensions
    {
        private static readonly string EmailAddress = ConfigurationManager.AppSettings["EmailAddress"];
        private static readonly string EmailPassword = ConfigurationManager.AppSettings["EmailPassword"];
        private static readonly string SmtpClient = ConfigurationManager.AppSettings["SmtpClient"];
        private static readonly int SmtpClientPort = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpClientPort"]);

        public static void SendEmail(string subject, string receiver, string body, bool CC)
        {
            //Send to self if receiver is null
            if (String.IsNullOrEmpty(receiver))
                receiver = EmailAddress;

            MailMessage mailMessage = new MailMessage(EmailAddress, receiver.Trim());

            if (CC && !String.IsNullOrEmpty(receiver))
                mailMessage.CC.Add(new MailAddress(EmailAddress));

            mailMessage.IsBodyHtml = true;
            mailMessage.Subject = subject;
            mailMessage.Body = body;

            SmtpClient smtpClient = new SmtpClient(SmtpClient, SmtpClientPort);

            smtpClient.Credentials = new System.Net.NetworkCredential()
            {
                UserName = EmailAddress,
                Password = EmailPassword
            };

            smtpClient.EnableSsl = true;
            smtpClient.Send(mailMessage);
        }
    }
}