using NotifySystem.Core.Domain.Entities;

namespace NotifySystem.Core.Domain.Services;

public interface INotificationStatusUpdater
{
    Task MarkAsSuccessAsync(Notification notification, string channel);
    Task MarkForRetryAsync(Notification notification);
    Task MarkAsFailedAsync(Notification notification, string channel, string reason);
}