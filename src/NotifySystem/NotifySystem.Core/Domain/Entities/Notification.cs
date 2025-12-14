using NotifySystem.Core.Domain.Enums;
using NotifySystem.Core.Domain.SharedKernel.Base;

namespace NotifySystem.Core.Domain.Entities;

public class Notification : Entity
{
    public long RecipientId { get; private set; } 
    public string Message { get; private set; }
    public NotificationType Type { get; private set; }
    public NotificationStatus Status { get; private set; }
    public DateTime? SentAt { get; private set; }
    public int RetryCount { get; private set; }
    
    private readonly List<NotificationHistory> _history = new();
    public IReadOnlyCollection<NotificationHistory> History => _history.AsReadOnly();

    public Notification(long recipientId, string message, NotificationType type)
    {
        RecipientId = recipientId;
        Message = message;
        Type = type;

        Status = NotificationStatus.Pending;
        RetryCount = 0;
    }

    public void MarkAsSent(string channel, string providerResponse)
    {
        if (Status == NotificationStatus.Sent)
            throw new InvalidOperationException("Already sent");
        
        Status = NotificationStatus.Sent;
        SentAt = DateTime.UtcNow;
        
        _history.Add(NotificationHistory.CreateSuccess(Id, channel, providerResponse));
    }

    public void MarkAsFailed(string channel, string error)
    {
        if (Status == NotificationStatus.Sent)
            throw new InvalidOperationException("Already sent");

        Status = NotificationStatus.Failed;

        _history.Add(NotificationHistory.CreateFailure(Id, channel, error));
    }

    public void MarkForRetry()
    {
        if (Status == NotificationStatus.Sent)
            throw new InvalidOperationException("Already sent");

        RetryCount++;
        Status = NotificationStatus.Retrying;
    }
}