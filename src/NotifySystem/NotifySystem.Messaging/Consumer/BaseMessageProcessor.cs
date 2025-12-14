using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NotifySystem.Core.Domain.Entities;
using NotifySystem.Core.Domain.Repositories;
using NotifySystem.Core.Domain.Services;
using NotifySystem.Messaging.Interfaces;
using RabbitMQ.Client.Events;

namespace NotifySystem.Messaging.Consumer;

public abstract class BaseMessageProcessor<TRequest> : IMessageProcessor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;
    private readonly string _notificationType;

    protected BaseMessageProcessor(
        IServiceProvider serviceProvider,
        ILogger logger,
        string notificationType)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _notificationType = notificationType;
    }

    public async Task<bool> ProcessMessageAsync(BasicDeliverEventArgs eventArgs)
    {
        IServiceScope? scope = null;
        Notification? notification = null;
        
        try
        {
            var request = DeserializeMessage(eventArgs);
            if (request == null)
            {
                _logger.LogWarning("Invalid message format");
                return false;
            }

            var notificationId = GetNotificationId(request);
            _logger.LogInformation("Processing {NotificationType} notification: {NotificationId}", 
                _notificationType, notificationId);

            scope = _serviceProvider.CreateScope();
            
            notification = await GetNotificationAsync(scope, notificationId);
            if (notification == null)
            {
                _logger.LogWarning("Notification not found in DB: {Id}", notificationId);
                return false;
            }

            _logger.LogInformation("Processing notification {Id}, retry {Retry}", 
                notification.Id, notification.RetryCount);

            var success = await ProcessSpecificMessageAsync(scope, request);

            if (success)
            {
                var statusUpdater = scope.ServiceProvider.GetRequiredService<INotificationStatusUpdater>();
                await statusUpdater.MarkAsSuccessAsync(notification, _notificationType);
                return true;
            }

            await HandleFailedNotificationAsync(scope, notification);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message: {Message}", ex.Message);
            
            if (notification != null && scope != null)
            {
                try
                {
                    var statusUpdater = scope.ServiceProvider.GetRequiredService<INotificationStatusUpdater>();
                    await statusUpdater.MarkAsFailedAsync(notification, _notificationType, $"Processing error: {ex.Message}");
                    _logger.LogInformation("Marked notification {Id} as failed due to processing error", notification.Id);
                }
                catch (Exception markFailedEx)
                {
                    _logger.LogError(markFailedEx, "Failed to mark notification {Id} as failed", notification.Id);
                }
            }
            
            return false;
        }
        finally
        {
            scope?.Dispose();
        }
    }

    protected abstract long GetNotificationId(TRequest request);
    protected abstract Task<bool> ProcessSpecificMessageAsync(IServiceScope scope, TRequest request);

    private TRequest? DeserializeMessage(BasicDeliverEventArgs eventArgs)
    {
        try
        {
            var messageBody = eventArgs.Body.ToArray();
            var json = Encoding.UTF8.GetString(messageBody);
            return JsonSerializer.Deserialize<TRequest>(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize message");
            return default;
        }
    }

    private async Task<Notification?> GetNotificationAsync(IServiceScope scope, long notificationId)
    {
        var notificationRepo = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
        return await notificationRepo.GetByIdAsync(notificationId);
    }

    private async Task HandleFailedNotificationAsync(IServiceScope scope, Notification notification)
    {
        var statusUpdater = scope.ServiceProvider.GetRequiredService<INotificationStatusUpdater>();
        var retryHandler = scope.ServiceProvider.GetRequiredService<IRetryHandler>();
        
        if (await retryHandler.ShouldRetryAsync(notification))
        {
            await statusUpdater.MarkForRetryAsync(notification);
            await retryHandler.HandleRetryAsync(notification);
        }
        else
        {
            await retryHandler.HandleMaxRetriesReachedAsync(notification);
            await statusUpdater.MarkAsFailedAsync(notification, _notificationType, "Max retry attempts reached");
        }
    }
}