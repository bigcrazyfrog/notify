using NotifySystem.Core.Domain.SharedKernel.Base;

namespace NotifySystem.Core.Domain.SharedKernel.Storage
{
    public interface IRepository<TAggregateRoot> : IReadonlyRepository<TAggregateRoot> 
        where TAggregateRoot : class, IAggregateRoot 
    {
        IUnitOfWork UnitOfWork { get; }

        ValueTask<TAggregateRoot> AddAsync(TAggregateRoot entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(TAggregateRoot entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(TAggregateRoot entity, CancellationToken cancellationToken = default);
    }
}
