using Pexita.Data.Entities.SMTP;
using Pexita.Services.Interfaces;
using System.Net;
using System.Net.Mail;
namespace Pexita.Services
{
    public class EmailService : IEmailService
    {
        private readonly SMTPSettings _smtpSettings;

        public EmailService(SMTPSettings smtpSettings)
        {
            _smtpSettings = smtpSettings;
        }

        public void SendEmail(string to, string subject, string body)
        {
            SmtpClient smtpClient = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port)
            {
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = true
            };

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false,
            };

            mailMessage.To.Add(to);
            smtpClient.Send(mailMessage);
        }
    }
}
