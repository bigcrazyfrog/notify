using System.Linq.Expressions;
using NotifySystem.Core.Domain.SharedKernel.Base;

namespace NotifySystem.Core.Domain.SharedKernel.Storage.Implementation
{
    public abstract class Repository<TAggregateRoot>(IUnitOfWork unitOfWork) : ReadonlyRepository<TAggregateRoot>, IRepository<TAggregateRoot>
        where TAggregateRoot : class, IAggregateRoot
    {
        public abstract ValueTask<TAggregateRoot> AddAsync(TAggregateRoot entity, CancellationToken cancellationToken = default);
        public abstract Task DeleteAsync(TAggregateRoot entity, CancellationToken cancellationToken = default);
        public abstract Task UpdateAsync(TAggregateRoot entity, CancellationToken cancellationToken = default);

        public IUnitOfWork UnitOfWork
        {
            get
            {
                if (ReadOnly)
                {
                    throw new NotSupportedException("UnitOfWork is read-only.");
                }

                return unitOfWork;
            }
        }
    }
}
