using Prometheus;

namespace NotifySystem.Application.Metrics;

public static class NotificationMetrics
{
    private static readonly Counter NotificationsSent = Prometheus.Metrics
        .CreateCounter("notifications_sent_total", "Total number of notifications sent", new[] { "type", "status" });

    private static readonly Counter NotificationsFailed = Prometheus.Metrics
        .CreateCounter("notifications_failed_total", "Total number of failed notifications", new[] { "type", "reason" });

    private static readonly Counter NotificationRetries = Prometheus.Metrics
        .CreateCounter("notification_retries_total", "Total number of notification retries", new[] { "type", "attempt" });

    private static readonly Counter RecipientOperations = Prometheus.Metrics
        .CreateCounter("recipient_operations_total", "Total number of recipient operations", new[] { "operation", "status" });

    private static readonly Histogram RecipientSearchDuration = Prometheus.Metrics
        .CreateHistogram("recipient_search_duration_seconds", "Duration of recipient search operations", new[] { "operation" });

    public static void Sent(string type, string status = "success") =>
        NotificationsSent.WithLabels(type, status).Inc();

    public static void Failed(string type, string reason) =>
        NotificationsFailed.WithLabels(type, reason).Inc();

    public static void Retried(string type, int attempt) =>
        NotificationRetries.WithLabels(type, attempt.ToString()).Inc();

    public static void RecipientOperation(string operation, string status = "success") =>
        RecipientOperations.WithLabels(operation, status).Inc();

    public static IDisposable RecipientSearchTimer(string operation) =>
        RecipientSearchDuration.WithLabels(operation).NewTimer();
}