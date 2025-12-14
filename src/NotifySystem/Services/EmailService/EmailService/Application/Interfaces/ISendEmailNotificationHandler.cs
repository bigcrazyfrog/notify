using EmailService.DTOs;

namespace EmailService.Application.Interfaces;

public interface ISendEmailNotificationHandler
{
    Task<bool> HandleAsync(SendEmailNotificationRequest request, CancellationToken cancellationToken = default);
}