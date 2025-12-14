namespace NotifySystem.Application.Features.Notifications.DTOs;

public class NotificationHistoryDto
{
    public long Id { get; set; }
    public long NotificationId { get; set; }
    public string Channel { get; set; } = string.Empty;
    public string? ProviderResponse { get; set; }
    public DateTime AttemptedAt { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public string NotificationMessage { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public long RecipientId { get; set; }
}
