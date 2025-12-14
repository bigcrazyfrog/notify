using NotifySystem.Core.Domain.Entities;

namespace NotifySystem.Core.Domain.Services;

public interface IRetryHandler
{
    Task<bool> ShouldRetryAsync(Notification notification);
    Task HandleRetryAsync(Notification notification);
    Task HandleMaxRetriesReachedAsync(Notification notification);
}