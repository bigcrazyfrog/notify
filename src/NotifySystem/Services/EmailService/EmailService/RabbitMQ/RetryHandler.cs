using Microsoft.Extensions.Options;
using NotifySystem.Core.Domain.Entities;
using NotifySystem.Core.Domain.Services;
using NotifySystem.Core.Domain.SharedKernel.Events;
using NotifySystem.Messaging.Configuration;
using NotifySystem.Messaging.Interfaces;
using EmailService.Common;

namespace EmailService.RabbitMQ
{
    public class RetryHandler : IRetryHandler
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly RabbitMqSettings _settings;
        private readonly ILogger<RetryHandler> _logger;

        public RetryHandler(
            IEventPublisher eventPublisher,
            IOptions<RabbitMqSettings> settings,
            ILogger<RetryHandler> logger)
        {
            _eventPublisher = eventPublisher;
            _settings = settings.Value;
            _logger = logger;
        }

        public Task<bool> ShouldRetryAsync(Notification notification)
        {
            var shouldRetry = notification.RetryCount < NotificationMessages.MaxRetries;
            
            _logger.LogInformation("Checking retry for notification {Id}. Current retry: {RetryCount}, Max: {MaxRetries}, Should retry: {ShouldRetry}",
                notification.Id, notification.RetryCount, NotificationMessages.MaxRetries, shouldRetry);
                
            return Task.FromResult(shouldRetry);
        }

        public async Task HandleRetryAsync(Notification notification)
        {
            _logger.LogWarning("Retry {Retry}/{MaxRetry} for notification {Id}",
                notification.RetryCount, NotificationMessages.MaxRetries, notification.Id);
            
            var delaySeconds = 3 * notification.RetryCount;
            _logger.LogInformation("Waiting {DelaySeconds} seconds before retry for notification {Id}", 
                delaySeconds, notification.Id);
            
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));

            await _eventPublisher.PublishAsync(
                new NotificationCreatedEvent(
                    notification.Id,
                    notification.RecipientId,
                    notification.Message,
                    notification.Type),
                _settings.QueueName);

            _logger.LogInformation("Retry message published to queue '{QueueName}' for notification {Id}", 
                _settings.QueueName, notification.Id);
        }

        public Task HandleMaxRetriesReachedAsync(Notification notification)
        {
            _logger.LogError("Notification {Id} has reached max retry attempts ({MaxRetries})", 
                notification.Id, NotificationMessages.MaxRetries);
                
            return Task.CompletedTask;
        }
    }
}