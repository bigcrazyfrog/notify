namespace NotifySystem.Application.Common.Logging;

public interface INotificationLogger
{
    void LogNotificationSent(long notificationId, string type, string recipient);
    void LogNotificationFailed(long notificationId, string type, string recipient, string error);
    void LogNotificationRetry(long notificationId, string type, int attempt, string reason);
    void LogNotificationProcessing(long notificationId, string type, string status);
}