﻿using Microsoft.Extensions.Options;
using Pexita.Data.Entities.SMTP;
using Pexita.Services.Interfaces;
using System.Net;
using System.Net.Mail;
namespace Pexita.Services
{
    public class EmailService : IEmailService
    {
        private readonly SMTPSettings _smtpSettings;

        public EmailService(IOptions<SMTPSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }
        /// <summary>
        /// Sends an Email using the SMTP settings provided in AppSettings.json.
        /// </summary>
        /// <param name="to">receiver of the email.</param>
        /// <param name="subject">Subject of the email.</param>
        /// <param name="body">Body of the email.</param>
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
