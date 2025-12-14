using NotifySystem.Core.Domain.Entities;
using NotifySystem.Core.Domain.Repositories;
using NotifySystem.Core.Domain.SharedKernel.Specification;

namespace NotifySystem.Infrastructure.Persistance.DataStorage.Repositories;

public class RecipientRepository : EFRepository<Recipient, NotifyDbContext>, IRecipientRepository
{
    private readonly NotifyDbContext _context;

    public RecipientRepository(NotifyDbContext context) : base(context)
    {
        _context = context;
    }
    
}