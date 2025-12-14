using System.Linq.Expressions;
using NotifySystem.Core.Domain.SharedKernel.Base;

namespace NotifySystem.Core.Domain.SharedKernel.Storage.Implementation;

public abstract class ReadonlyRepository<TAggregateRoot> : IReadonlyRepository<TAggregateRoot>
    where TAggregateRoot : class, IAggregateRoot
{
    public virtual bool ReadOnly {get; set;}
    public abstract Task<IReadOnlyList<TAggregateRoot>> ListAsync(CancellationToken cancellationToken = default);
    public abstract Task<IReadOnlyList<TAggregateRoot>> ListAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken cancellationToken = default);
    public abstract Task<TAggregateRoot?> FindAsync(long[] ids, CancellationToken cancellationToken = default);
    public abstract Task<TAggregateRoot?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
}