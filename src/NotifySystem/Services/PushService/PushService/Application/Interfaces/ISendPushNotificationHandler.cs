using PushService.DTOs;

namespace PushService.Application.Interfaces;

public interface ISendPushNotificationHandler
{
    Task<bool> HandleAsync(SendPushNotificationRequest request, CancellationToken cancellationToken = default);
}