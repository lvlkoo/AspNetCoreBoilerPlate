using System;
using System.Threading.Tasks;
using Boilerplate.Services.Abstractions;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;

namespace Boilerplate.Services.Implementations
{
    public class MailKitEmailService: IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MailKitEmailService> _logger;

        public MailKitEmailService(IConfiguration configuration, ILogger<MailKitEmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendMail(string to, string subject, string message)
        {
            try
            {
                var smtpHost = _configuration["Email:SMTPHost"];
                var smtpPort = int.Parse(_configuration["Email:SMTPPort"]);
                var smtpSsl = bool.Parse(_configuration["Email:SMTPSsl"]);
                var smtpLogin = _configuration["Email:SMTPLogin"];
                var smtpPassword = _configuration["Email:SMTPPassword"];
                var mailFrom = _configuration["Email:SMTPFrom"];

                var mailMessage = new MimeMessage
                {
                    Subject = subject,
                    Body = new TextPart(TextFormat.Plain)
                    {
                        Text = message
                    }
                };

                mailMessage.From.Add(new MailboxAddress(mailFrom, smtpLogin));
                mailMessage.To.Add(new MailboxAddress(to));

                using (var smtp = new SmtpClient())
                {
                    await smtp.ConnectAsync(smtpHost, smtpPort, smtpSsl);
                    smtp.AuthenticationMechanisms.Remove("XOAUTH2");
                    await smtp.AuthenticateAsync(smtpLogin, smtpPassword);
                    await smtp.SendAsync(mailMessage);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error sending email");
            }
        }
    }
}
