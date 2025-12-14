using Microsoft.Extensions.DependencyInjection;
using PushService.Application.Interfaces;
using PushService.DTOs;
using NotifySystem.Messaging.Consumer;

namespace PushService.RabbitMQ
{
    public class MessageProcessor : BaseMessageProcessor<SendPushNotificationRequest>
    {
        public MessageProcessor(
            IServiceProvider serviceProvider,
            ILogger<MessageProcessor> logger)
            : base(serviceProvider, logger, "Push")
        {
        }

        protected override long GetNotificationId(SendPushNotificationRequest request)
        {
            return request.NotificationId;
        }

        protected override async Task<bool> ProcessSpecificMessageAsync(IServiceScope scope, SendPushNotificationRequest request)
        {
            var handler = scope.ServiceProvider.GetRequiredService<ISendPushNotificationHandler>();
            return await handler.HandleAsync(request);
        }
    }
}