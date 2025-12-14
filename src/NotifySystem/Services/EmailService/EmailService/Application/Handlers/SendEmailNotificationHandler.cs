using EmailService.Application.Interfaces;
using EmailService.Common;
using EmailService.DTOs;
using EmailService.Services;
using NotifySystem.Application.Common.Logging;
using NotifySystem.Application.Metrics;
using NotifySystem.Core.Domain.Enums;
using NotifySystem.Core.Domain.Repositories;

namespace EmailService.Application.Handlers
{
    public class SendEmailNotificationHandler : ISendEmailNotificationHandler
    {
        private readonly IEmailSender _emailSender;
        private readonly IRecipientRepository _recipientRepository;
        private readonly INotificationLogger _notificationLogger;
        private readonly ILogger<SendEmailNotificationHandler> _logger;

        public SendEmailNotificationHandler(
            IEmailSender emailSender,
            IRecipientRepository recipientRepository,
            INotificationLogger notificationLogger,
            ILogger<SendEmailNotificationHandler> logger)
        {
            _emailSender = emailSender;
            _recipientRepository = recipientRepository;
            _notificationLogger = notificationLogger;
            _logger = logger;
        }

        public async Task<bool> HandleAsync(SendEmailNotificationRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrEmpty(request.RecipientEmail))
                {
                    request.RecipientEmail = await GetRecipientEmailAsync(request, cancellationToken);
                }
                
                var subject = !string.IsNullOrEmpty(request.Subject) ? request.Subject : NotificationMessages.DefaultSubject;
                var body = !string.IsNullOrEmpty(request.Body) ? request.Body : request.Message ?? string.Empty;

                request.Subject = subject;
                request.Body = body;

                var result = await _emailSender.SendEmailAsync(request);

                if (result.IsSuccess)
                {
                    _notificationLogger.LogNotificationSent(request.NotificationId, "Email", request.RecipientEmail);
                    
                    _logger.LogInformation("Email sent successfully for notification {NotificationId}. MessageId: {MessageId}", 
                        request.NotificationId, result.MessageId);

                    NotificationMetrics.Sent("Email", "success");
                    return true;
                }
                else
                {
                    _logger.LogWarning("Email failed for notification {Id}: {Err}", request.NotificationId, result.ErrorMessage);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _notificationLogger.LogNotificationFailed(request.NotificationId, "Email", request.RecipientEmail ?? "Unknown", ex.Message);
                
                _logger.LogError(ex, "Failed to send email for notification {NotificationId}: {Error}",
                    request.NotificationId, ex.Message);

                NotificationMetrics.Failed("Email", ex.GetType().Name);
                return false;
            }
        }

        private async Task<string> GetRecipientEmailAsync(SendEmailNotificationRequest request, CancellationToken cancellationToken)
        {
            var recipient = await _recipientRepository.GetByIdAsync(request.RecipientId, cancellationToken);
            if (recipient == null)
            {
                throw new InvalidOperationException($"Recipient with ID {request.RecipientId} not found");
            }

            var email = recipient.GetContact(NotificationType.Email);
            
            if (string.IsNullOrEmpty(email))
            {
                throw new InvalidOperationException($"Email not found for recipient {request.RecipientId}");
            }

            return email;
        }
    }
}
