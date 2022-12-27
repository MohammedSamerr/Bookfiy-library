﻿using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Bookfiy_WepApp.settings
{
    public class EmailSender : IEmailSender
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly MailSettings _mailSettings;
        public EmailSender(IOptions<MailSettings> mailSettings, IWebHostEnvironment webHostEnvironment)
        {
            _mailSettings = mailSettings.Value;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            MailMessage message = new()
            {
                From = new MailAddress(_mailSettings.Mail!,_mailSettings.DisplayName),
                Body = htmlMessage,
                Subject = subject,
                IsBodyHtml= true
            };
            message.To.Add(_webHostEnvironment.IsDevelopment()? "mrm242879@gmail.com" : email);
            SmtpClient smtpClient = new(_mailSettings.Host)
            {
                Port = _mailSettings.Port,
                Credentials = new NetworkCredential(_mailSettings.Mail,_mailSettings.Password),
                EnableSsl= true
            };
            await smtpClient.SendMailAsync(message);
            smtpClient.Dispose();
        }
    }
}
