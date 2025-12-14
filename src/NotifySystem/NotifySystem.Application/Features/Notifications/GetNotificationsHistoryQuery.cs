using Microsoft.Extensions.Logging;
using NotifySystem.Application.Cqs;
using NotifySystem.Application.Features.Notifications.DTOs;
using NotifySystem.Application.Metrics;
using NotifySystem.Core.Domain.Entities;
using NotifySystem.Core.Domain.Repositories;
using NotifySystem.Core.Domain.SharedKernel.Result;

namespace NotifySystem.Application.Features.Notifications;

public class GetNotificationsHistoryQuery : Query<List<NotificationHistoryDto>>
{
    public long? NotificationId { get; set; }
    public long? RecipientId { get; set; }
    public string? Channel { get; set; }
    public bool? IsSuccess { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int PageSize { get; set; } = 50;
    public int PageNumber { get; set; } = 1;
}

public class GetNotificationsHistoryQueryHandler : QueryHandler<GetNotificationsHistoryQuery, List<NotificationHistoryDto>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<GetNotificationsHistoryQueryHandler> _logger;

    public GetNotificationsHistoryQueryHandler(INotificationRepository notificationRepository, ILogger<GetNotificationsHistoryQueryHandler> logger)
    {
        _notificationRepository = notificationRepository;
        _logger = logger;
    }

    public override async Task<Result<List<NotificationHistoryDto>>> Handle(GetNotificationsHistoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Getting notification history: NotificationId={NotificationId}, RecipientId={RecipientId}, Channel={Channel}, Page={Page}", 
                request.NotificationId, request.RecipientId, request.Channel, request.PageNumber);

            var historyWithNotifications = await _notificationRepository.GetHistoryWithNotificationsAsync(
                request.NotificationId,
                request.RecipientId,
                request.Channel,
                request.IsSuccess,
                request.FromDate,
                request.ToDate,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            var result = historyWithNotifications
                .Select(x => MapToDto(x.History, x.Notification))
                .ToList();

            _logger.LogInformation("Retrieved {Count} notification history records", result.Count);
            NotificationMetrics.Sent("History", "retrieved");
            
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get notification history: NotificationId={NotificationId}, RecipientId={RecipientId}, Error={Error}", 
                request.NotificationId, request.RecipientId, ex.Message);
            
            NotificationMetrics.Failed("History", ex.GetType().Name);
            return Error(new ValidationError($"Failed to get history: {ex.Message}"));
        }
    }

    private static NotificationHistoryDto MapToDto(NotificationHistory history, Notification notification)
    {
        return new NotificationHistoryDto
        {
            Id = history.Id,
            NotificationId = history.NotificationId,
            Channel = history.Channel,
            ProviderResponse = history.ProviderResponse,
            AttemptedAt = history.AttemptedAt,
            IsSuccess = history.IsSuccess,
            ErrorMessage = history.ErrorMessage,
            NotificationMessage = notification.Message,
            NotificationType = notification.Type.ToString(),
            RecipientId = notification.RecipientId
        };
    }
}