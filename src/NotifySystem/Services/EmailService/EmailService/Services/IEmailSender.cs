using EmailService.DTOs;

namespace EmailService.Services
{
    public interface IEmailSender
    {
        Task<EmailResult> SendEmailAsync(SendEmailNotificationRequest request);
    }
}
