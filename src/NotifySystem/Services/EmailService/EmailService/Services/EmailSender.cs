using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Logging;
using EmailService.Configuration;
using EmailService.DTOs;
using MailKit.Security;
using Microsoft.Extensions.Options;

namespace EmailService.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<EmailSettings> settings, ILogger<EmailSender> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }
        
        public async Task<EmailResult> SendEmailAsync(SendEmailNotificationRequest request)
        {
            if (!_settings.Enabled)
            {
                return EmailResult.Failure("Email service is disabled");
            }

            if (string.IsNullOrEmpty(request.RecipientEmail))
            {
                return EmailResult.Failure("Recipient email is required");
            }

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Notifications System", _settings.SenderEmail));
                message.To.Add(new MailboxAddress("", request.RecipientEmail));
                message.Subject = request.Subject ?? request.Message ?? "Notification";
                message.Body = new TextPart("html") { Text = request.Body ?? request.Message ?? "" };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_settings.SenderEmail, _settings.SenderPassword);
                    var response = await client.SendAsync(message);
                    await client.DisconnectAsync(true);

                    _logger.LogInformation("Email sent successfully to {RecipientEmail} for notification {NotificationId}", 
                        request.RecipientEmail, request.NotificationId);

                    return EmailResult.Success(response, $"Email sent successfully to {request.RecipientEmail}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {RecipientEmail} for notification {NotificationId}: {Message}", 
                    request.RecipientEmail, request.NotificationId, ex.Message);
                
                return EmailResult.Failure($"Email sending failed: {ex.Message}");
            }
        }
    }
}
