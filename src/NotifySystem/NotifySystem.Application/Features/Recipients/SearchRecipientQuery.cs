using Microsoft.Extensions.Logging;
using NotifySystem.Application.Cqs;
using NotifySystem.Application.Metrics;
using NotifySystem.Core.Domain.Entities;
using NotifySystem.Core.Domain.Repositories.Specifications;
using NotifySystem.Core.Domain.SharedKernel.Result;
using NotifySystem.Core.Domain.SharedKernel.Specification;
using NotifySystem.Core.Domain.SharedKernel.Storage;
using ValidationError = NotifySystem.Core.Domain.SharedKernel.Result.ValidationError;

namespace NotifySystem.Application.Features.Recipients;

public class SearchRecipientQuery : Query<IReadOnlyList<Recipient>> 
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? DeviceToken { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public bool? HasEmail { get; set; }
    public bool? HasPhone { get; set; }
    public bool? HasDeviceToken { get; set; }
    public string? SearchTerm { get; set; } 
}

public class SearchRecipientQueryHandler : QueryHandler<SearchRecipientQuery, IReadOnlyList<Recipient>>
{
    private readonly IReadonlyRepository<Recipient> _recipientRepository;
    private readonly ILogger<SearchRecipientQueryHandler> _logger;
    
    public SearchRecipientQueryHandler(IReadonlyRepository<Recipient> recipientRepository, ILogger<SearchRecipientQueryHandler> logger)
    {
        _recipientRepository = recipientRepository;
        _logger = logger;
    }
    public override async Task<Result<IReadOnlyList<Recipient>>> Handle(SearchRecipientQuery request, CancellationToken cancellationToken)
    {
        using var timer = NotificationMetrics.RecipientSearchTimer("search");
        try
        {
            _logger.LogDebug("Searching recipients: SearchTerm={SearchTerm}, Name={Name}, Email={Email}", 
                request.SearchTerm, request.Name, request.Email);
            var recipientSpec = Specification<Recipient>.Empty();
            if (!string.IsNullOrEmpty(request.SearchTerm)) 
            {
                var searchSpec = RecipientSpecification.ByName(request.SearchTerm)
                    .Or(RecipientSpecification.ByEmail(request.SearchTerm))
                    .Or(RecipientSpecification.ByPhone(request.SearchTerm));
            
                recipientSpec = recipientSpec.And(searchSpec);
            }
            else 
            { 
                if (!string.IsNullOrEmpty(request.Name)) 
                { 
                    recipientSpec = recipientSpec.And(RecipientSpecification.ByName(request.Name)); 
                }
                
                if (!string.IsNullOrEmpty(request.Email)) 
                { 
                    recipientSpec = recipientSpec.And(RecipientSpecification.ByEmail(request.Email)); 
                }
                
                if (!string.IsNullOrEmpty(request.Phone)) 
                { 
                    recipientSpec = recipientSpec.And(RecipientSpecification.ByPhone(request.Phone)); 
                }
                
                if (!string.IsNullOrEmpty(request.DeviceToken)) 
                { 
                    recipientSpec = recipientSpec.And(RecipientSpecification.ByDeviceToken(request.DeviceToken)); 
                } 
            }
            if (request.IsActive.HasValue)
            {
                recipientSpec = recipientSpec.And(request.IsActive.Value 
                    ? RecipientSpecification.IsActive() 
                    : RecipientSpecification.IsInactive());
            }

            if (request.CreatedAfter.HasValue)
            {
                recipientSpec = recipientSpec.And(RecipientSpecification.CreatedAfter(request.CreatedAfter.Value));
            }

            if (request.CreatedBefore.HasValue)
            {
                recipientSpec = recipientSpec.And(RecipientSpecification.CreatedBefore(request.CreatedBefore.Value));
            }
        
            if (request.HasEmail.HasValue && request.HasEmail.Value)
            {
                recipientSpec = recipientSpec.And(RecipientSpecification.HasEmail());
            }

            if (request.HasPhone.HasValue && request.HasPhone.Value)
            {
                recipientSpec = recipientSpec.And(RecipientSpecification.HasPhone());
            }

            if (request.HasDeviceToken.HasValue && request.HasDeviceToken.Value)
            {
                recipientSpec = recipientSpec.And(RecipientSpecification.HasDeviceToken());
            }

            var recipients = await _recipientRepository.ListAsync(recipientSpec, cancellationToken);

            _logger.LogInformation("Found {Count} recipients matching search criteria", recipients.Count);
            NotificationMetrics.RecipientOperation("search", "success");
            return Success(recipients); 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search recipients: SearchTerm={SearchTerm}, Error={Error}", 
                request.SearchTerm, ex.Message);
            NotificationMetrics.RecipientOperation("search", "error");
            return Error(new ValidationError($"Failed to search recipients: {ex.Message}"));
        }
    }
}