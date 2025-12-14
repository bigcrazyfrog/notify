using MediatR;
using Microsoft.AspNetCore.Mvc;
using NotifySystem.Application.Features.Recipients;
using NotifySystem.Core.Domain.SharedKernel.Result;

namespace NotifySystem.Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipientController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public RecipientController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<Result> Create(
        [FromBody] CreateRecipientCommand request, 
        CancellationToken cancellationToken)
    {
        return await _mediator.Send(request, cancellationToken);
    }
}