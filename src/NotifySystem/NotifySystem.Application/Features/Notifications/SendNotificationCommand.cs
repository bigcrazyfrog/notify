using NotifySystem.Application.Common.Logging;
using NotifySystem.Application.Cqs;
using NotifySystem.Application.Cqs.Interfaces;
using NotifySystem.Application.Metrics;
using NotifySystem.Core.Domain.Entities;
using NotifySystem.Core.Domain.Enums;
using NotifySystem.Core.Domain.Repositories;
using NotifySystem.Core.Domain.SharedKernel.Events;
using NotifySystem.Core.Domain.SharedKernel.Result;
using NotifySystem.Messaging.Interfaces;

namespace NotifySystem.Application.Features.Notifications;

public class SendNotificationCommand : Command
{
    public string Message { get; set; }
    public long RecipientId { get; set; }
    public NotificationType Type { get; set; }
}

public class SendNotificationCommandHandler : CommandHandler<SendNotificationCommand>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IEventPublisher _publisher;
    private readonly INotificationLogger _notificationLogger;

    public SendNotificationCommandHandler(INotificationRepository notificationRepository, IEventPublisher publisher, INotificationLogger notificationLogger)
    {
        _notificationRepository = notificationRepository;
        _publisher = publisher;
        _notificationLogger = notificationLogger;
    }
    
    public override async Task<Result> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var notification = new Notification(request.RecipientId, request.Message, request.Type);

            await _notificationRepository.AddAsync(notification, cancellationToken);
            await _notificationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            _notificationLogger.LogNotificationProcessing(notification.Id, request.Type.ToString(), "Created");

            var evt = new NotificationCreatedEvent(notification.Id, notification.RecipientId, notification.Message,
                notification.Type);
            
            var queueName = GetQueueName(request.Type);

            await _publisher.PublishAsync(evt, queueName, cancellationToken);

            _notificationLogger.LogNotificationProcessing(notification.Id, request.Type.ToString(), "Queued");
            
            NotificationMetrics.Sent(request.Type.ToString(), "success");
            return Success();
        }
        catch (Exception ex)
        {
            _notificationLogger.LogNotificationFailed(0, request.Type.ToString(), request.RecipientId.ToString(), ex.Message);
            NotificationMetrics.Failed(request.Type.ToString(), ex.Message);
            return Error(new ValidationError("Notification could not be sent"));
        }
    }

    private string GetQueueName(NotificationType type)
    {
        switch (type)
        {
            case NotificationType.Email: return "email";
            case NotificationType.Push: return "push";
            case NotificationType.SMS: return "telegram";
            default: throw new InvalidOperationException("Unknown type");
        }
    }
}