using System.Linq.Expressions;
using NotifySystem.Core.Domain.Entities;
using NotifySystem.Core.Domain.Enums;
using NotifySystem.Core.Domain.SharedKernel.Base;

namespace NotifySystem.Core.Domain.SharedKernel.Storage
{
    public interface IReadonlyRepository<TAggregateRoot> where TAggregateRoot : class, IAggregateRoot 
    {
        bool ReadOnly { get; set; }
        Task<TAggregateRoot?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TAggregateRoot>> ListAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TAggregateRoot>> ListAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken cancellationToken = default);
        Task<TAggregateRoot?> FindAsync(long[] ids, CancellationToken cancellationToken = default);
    }
}
