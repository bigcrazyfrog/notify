using Microsoft.Extensions.Logging;

namespace NotifySystem.Application.Common.Logging;

public class NotificationLogger : INotificationLogger
{
    private readonly ILogger<NotificationLogger> _logger;

    public NotificationLogger(ILogger<NotificationLogger> logger)
    {
        _logger = logger;
    }

    public void LogNotificationSent(long notificationId, string type, string recipient)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["NotificationId"] = notificationId,
            ["NotificationType"] = type,
            ["Recipient"] = recipient,
            ["Action"] = "NotificationSent",
            ["Status"] = "Success"
        });

        _logger.LogInformation(
            "Notification sent successfully. NotificationId: {NotificationId}, Type: {Type}, Recipient: {Recipient}",
            notificationId, type, recipient);
    }

    public void LogNotificationFailed(long notificationId, string type, string recipient, string error)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["NotificationId"] = notificationId,
            ["NotificationType"] = type,
            ["Recipient"] = recipient,
            ["Action"] = "NotificationFailed",
            ["Status"] = "Failed",
            ["ErrorMessage"] = error
        });

        _logger.LogError(
            "Notification failed. NotificationId: {NotificationId}, Type: {Type}, Recipient: {Recipient}, Error: {Error}",
            notificationId, type, recipient, error);
    }

    public void LogNotificationRetry(long notificationId, string type, int attempt, string reason)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["NotificationId"] = notificationId,
            ["NotificationType"] = type,
            ["Action"] = "NotificationRetry",
            ["RetryAttempt"] = attempt,
            ["RetryReason"] = reason
        });

        _logger.LogWarning(
            "Notification retry attempt {Attempt}. NotificationId: {NotificationId}, Type: {Type}, Reason: {Reason}",
            attempt, notificationId, type, reason);
    }

    public void LogNotificationProcessing(long notificationId, string type, string status)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["NotificationId"] = notificationId,
            ["NotificationType"] = type,
            ["Action"] = "NotificationProcessing",
            ["Status"] = status
        });

        _logger.LogInformation(
            "Notification processing. NotificationId: {NotificationId}, Type: {Type}, Status: {Status}",
            notificationId, type, status);
    }
}