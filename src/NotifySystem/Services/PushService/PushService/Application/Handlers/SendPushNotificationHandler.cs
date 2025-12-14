using NotifySystem.Application.Common.Logging;
using NotifySystem.Application.Metrics;
using NotifySystem.Core.Domain.Enums;
using NotifySystem.Core.Domain.Repositories;
using PushService.Application.Interfaces;
using PushService.Common;
using PushService.DTOs;
using PushService.Services;

namespace PushService.Application.Handlers
{
    public class SendPushNotificationHandler : ISendPushNotificationHandler
    {
        private readonly IPushNotificationSender _pushSender;
        private readonly IRecipientRepository _recipientRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationLogger _notificationLogger;
        private readonly ILogger<SendPushNotificationHandler> _logger;

        public SendPushNotificationHandler(
            IPushNotificationSender pushSender,
            IRecipientRepository recipientRepository,
            INotificationRepository notificationRepository,
            INotificationLogger notificationLogger,
            ILogger<SendPushNotificationHandler> logger)
        {
            _pushSender = pushSender;
            _recipientRepository = recipientRepository;
            _notificationRepository = notificationRepository;
            _notificationLogger = notificationLogger;
            _logger = logger;
        }

        public async Task<bool> HandleAsync(SendPushNotificationRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrEmpty(request.DeviceToken))
                {
                    request.DeviceToken = await GetDeviceTokenAsync(request, cancellationToken);
                }
                
                var title = !string.IsNullOrEmpty(request.Title) ? request.Title : NotificationMessages.DefaultTitle;
                var body = !string.IsNullOrEmpty(request.Body) ? request.Body : request.Message ?? string.Empty;

                request.Title = title;
                request.Body = body;

                var result = await _pushSender.SendAsync(request);

                if (result.IsSuccess)
                {
                    _notificationLogger.LogNotificationSent(request.NotificationId, "Push", request.DeviceToken);
                    
                    _logger.LogInformation("Push notification sent successfully for notification {NotificationId}. MessageId: {MessageId}", 
                        request.NotificationId, result.MessageId);

                    NotificationMetrics.Sent("Push", "success");
                    return true;
                }
                else
                {
                    _logger.LogWarning("Push failed for notification {Id}: {Err}", request.NotificationId, result.ErrorMessage);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _notificationLogger.LogNotificationFailed(request.NotificationId, "Push", request.DeviceToken ?? "Unknown", ex.Message);
                
                _logger.LogError(ex, "Failed to send push notification for notification {NotificationId}: {Error}",
                    request.NotificationId, ex.Message);

                NotificationMetrics.Failed("Push", ex.GetType().Name);
                return false;
            }
        }

        private async Task<string> GetDeviceTokenAsync(SendPushNotificationRequest request, CancellationToken cancellationToken)
        {
            var recipient = await _recipientRepository.GetByIdAsync(request.RecipientId, cancellationToken);
            if (recipient == null)
            {
                throw new InvalidOperationException($"Recipient with ID {request.RecipientId} not found");
            }

            var pushToken = recipient.GetContact(NotificationType.Push);
            
            if (string.IsNullOrEmpty(pushToken))
            {
                throw new InvalidOperationException($"Push token not found for recipient {request.RecipientId}");
            }

            return pushToken;
        }
    }
}