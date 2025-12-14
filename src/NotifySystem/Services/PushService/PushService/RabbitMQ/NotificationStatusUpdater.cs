using NotifySystem.Core.Domain.Entities;
using NotifySystem.Core.Domain.Repositories;
using NotifySystem.Core.Domain.Services;

namespace PushService.RabbitMQ
{
    public class NotificationStatusUpdater : INotificationStatusUpdater
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<NotificationStatusUpdater> _logger;

        public NotificationStatusUpdater(
            INotificationRepository notificationRepository,
            ILogger<NotificationStatusUpdater> logger)
        {
            _notificationRepository = notificationRepository;
            _logger = logger;
        }

        public async Task MarkAsSuccessAsync(Notification notification, string channel)
        {
            notification.MarkAsSent(channel, "Sent successfully");
            await _notificationRepository.UnitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Notification {Id} marked as sent via {Channel}", 
                notification.Id, channel);
        }

        public async Task MarkForRetryAsync(Notification notification)
        {
            notification.MarkForRetry();
            await _notificationRepository.UnitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Notification {Id} marked for retry. Retry count: {RetryCount}", 
                notification.Id, notification.RetryCount);
        }

        public async Task MarkAsFailedAsync(Notification notification, string channel, string reason)
        {
            notification.MarkAsFailed(channel, reason);
            await _notificationRepository.UnitOfWork.SaveChangesAsync();
            
            _logger.LogError("Notification {Id} marked as failed via {Channel}. Reason: {Reason}", 
                notification.Id, channel, reason);
        }
    }
}