using Microsoft.Extensions.DependencyInjection;
using EmailService.Application.Interfaces;
using EmailService.DTOs;
using NotifySystem.Messaging.Consumer;

namespace EmailService.RabbitMQ
{
    public class MessageProcessor : BaseMessageProcessor<SendEmailNotificationRequest>
    {
        public MessageProcessor(
            IServiceProvider serviceProvider,
            ILogger<MessageProcessor> logger)
            : base(serviceProvider, logger, "Email")
        {
        }

        protected override long GetNotificationId(SendEmailNotificationRequest request)
        {
            return request.NotificationId;
        }

        protected override async Task<bool> ProcessSpecificMessageAsync(IServiceScope scope, SendEmailNotificationRequest request)
        {
            var handler = scope.ServiceProvider.GetRequiredService<ISendEmailNotificationHandler>();
            return await handler.HandleAsync(request);
        }
    }
}