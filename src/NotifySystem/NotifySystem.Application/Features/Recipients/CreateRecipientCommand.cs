using Microsoft.Extensions.Logging;
using NotifySystem.Application.Cqs;
using NotifySystem.Application.Metrics;
using NotifySystem.Core.Domain.Entities;
using NotifySystem.Core.Domain.Repositories;
using NotifySystem.Core.Domain.SharedKernel.Result;
using NotifySystem.Core.Domain.ValueObjects;

namespace NotifySystem.Application.Features.Recipients;

public class CreateRecipientCommand : Command
{
    public string Name { get; set; }
    public ContactInfo ContactInfo { get; set; }
    public bool IsValid() => !string.IsNullOrWhiteSpace(Name)
                             && ContactInfo.HasAnyContact();
}

public class CreateRecipientCommandHandler : CommandHandler<CreateRecipientCommand>
{
    private readonly IRecipientRepository _recipientRepository;
    private readonly ILogger<CreateRecipientCommandHandler> _logger;

    public CreateRecipientCommandHandler(IRecipientRepository recipientRepository, ILogger<CreateRecipientCommandHandler> logger)
    {
        _recipientRepository = recipientRepository;
        _logger = logger;
    }
    public override async Task<Result> Handle(CreateRecipientCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!request.IsValid())
            {
                _logger.LogWarning("Invalid recipient creation request: Name={Name}, HasContacts={HasContacts}", 
                    request.Name, request.ContactInfo?.HasAnyContact());
                NotificationMetrics.RecipientOperation("create", "validation_error");
                return Error(new ValidationError("Recipient must have a least one contact"));
            }
            
            var recipient = new Recipient(request.Name, request.ContactInfo);
            
            await _recipientRepository.AddAsync(recipient, cancellationToken);
            await _recipientRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Recipient created successfully: Id={RecipientId}, Name={Name}", 
                recipient.Id, recipient.Name);
            
            NotificationMetrics.RecipientOperation("create", "success");
            return Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create recipient: Name={Name}, Error={Error}", 
                request.Name, ex.Message);
            
            NotificationMetrics.RecipientOperation("create", "error");
            return Error(new ValidationError($"Failed to create recipient: {ex.Message}"));
        }
    }
}