using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens.Experimental;
using NotifySystem.Application.Cqs;
using NotifySystem.Application.Metrics;
using NotifySystem.Core.Domain.Entities;
using NotifySystem.Core.Domain.Repositories;
using NotifySystem.Core.Domain.SharedKernel.Result;
using NotifySystem.Core.Domain.SharedKernel.Storage;
using ValidationError = NotifySystem.Core.Domain.SharedKernel.Result.ValidationError;

namespace NotifySystem.Application.Features.Recipients;

public class GetRecipientQuery : Query<Recipient> 
{
    public long Id { get; set; }   
}

public class GetRecipientQueryHandler : QueryHandler<GetRecipientQuery, Recipient>
{
    private readonly IReadonlyRepository<Recipient> _recipientRepository;
    private readonly ILogger<GetRecipientQueryHandler> _logger;
    
    public GetRecipientQueryHandler(IReadonlyRepository<Recipient> recipientRepository, ILogger<GetRecipientQueryHandler> logger)
    {
        _recipientRepository = recipientRepository;
        _logger = logger;
    }
    public override async Task<Result<Recipient>> Handle(GetRecipientQuery request, CancellationToken cancellationToken)
    {
        using var timer = NotificationMetrics.RecipientSearchTimer("get");
        try
        {
            _logger.LogDebug("Getting recipient by Id: {RecipientId}", request.Id);
            
            var recipient = await _recipientRepository.FindAsync([request.Id], cancellationToken);

            if (recipient == null)
            {
                _logger.LogWarning("Recipient not found: Id={RecipientId}", request.Id);
                NotificationMetrics.RecipientOperation("get", "not_found");
                return Result<Recipient>.Error(ValidationError.Create("Recipient not found"));
            }
            
            _logger.LogDebug("Recipient found: Id={RecipientId}, Name={Name}", recipient.Id, recipient.Name);
            NotificationMetrics.RecipientOperation("get", "success");
            return Success(recipient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get recipient: Id={RecipientId}, Error={Error}", request.Id, ex.Message);
            NotificationMetrics.RecipientOperation("get", "error");
            return Result<Recipient>.Error(ValidationError.Create($"Failed to get recipient: {ex.Message}"));
        }
    }

}