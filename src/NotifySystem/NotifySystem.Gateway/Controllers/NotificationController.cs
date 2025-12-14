using MediatR;
using Microsoft.AspNetCore.Mvc;
using NotifySystem.Application.Features.Notifications;
using NotifySystem.Application.Features.Notifications.DTOs;
using NotifySystem.Core.Domain.Entities;
using NotifySystem.Core.Domain.SharedKernel.Result;

namespace NotifySystem.Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("send")]
    public async Task<Result> Send(
        [FromBody] SendNotificationCommand request,
        CancellationToken cancellationToken)
    {
       return await _mediator.Send(request, cancellationToken);
    }

    [HttpGet("history")]
    public async Task<Result<List<NotificationHistoryDto>>> GetNotificationsHistory(
        [FromQuery] GetNotificationsHistoryQuery request,
        CancellationToken cancellationToken)
    {
        return await _mediator.Send(request, cancellationToken);
    }

    [HttpGet("{notificationId}/history")]
    public async Task<Result<List<NotificationHistoryDto>>> GetNotificationHistory(
        long notificationId,
        CancellationToken cancellationToken)
    {
        var request = new GetNotificationsHistoryQuery { NotificationId = notificationId };
        return await _mediator.Send(request, cancellationToken);
    }
}