using NotifySystem.Core.Domain.Enums;

namespace NotifySystem.Core.Domain.SharedKernel.Events;

public class NotificationCreatedEvent
{
    public long NotificationId { get; set; }
    public long RecipientId { get; set; }
    public string Message { get; set; }
    public NotificationType NotificationType { get; set; }

    public NotificationCreatedEvent(long notificationId, long recipientId, string message,
        NotificationType notificationType)
    {
        NotificationId = notificationId;
        RecipientId = recipientId;
        Message = message;
        NotificationType = notificationType;
    }
}