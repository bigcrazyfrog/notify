using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NotifySystem.Core.Domain.SharedKernel.Base;
using NotifySystem.Core.Domain.SharedKernel.Storage;
using NotifySystem.Core.Domain.SharedKernel.Storage.Implementation;

namespace NotifySystem.Infrastructure.Persistance.DataStorage
{
    public abstract class EFRepository<TAggregateRoot, TDbContext> : Repository<TAggregateRoot>
        where TAggregateRoot : class, IAggregateRoot
        where TDbContext : DbContext, IUnitOfWork
    {
        protected readonly TDbContext _context;
        private DbSet<TAggregateRoot> _items => _context.Set<TAggregateRoot>();
        protected virtual IQueryable<TAggregateRoot> Items => ReadOnly ? _items.AsNoTracking() : _items;

        public EFRepository(TDbContext context) : base(context)
        {
            _context = context;
        }

        public override async ValueTask<TAggregateRoot> AddAsync(TAggregateRoot entity, CancellationToken cancellationToken = default)
        {
            var entry = await _items.AddAsync(entity, cancellationToken);
            return entry.Entity;
        }

        public override Task DeleteAsync(TAggregateRoot entity, CancellationToken cancellationToken = default)
        {
            _items.Remove(entity);
            return Task.CompletedTask;
        }

        public override async Task<IReadOnlyList<TAggregateRoot>> ListAsync(CancellationToken cancellationToken = default)
        {
            return await Items.ToListAsync(cancellationToken);
        }
        
        public override async Task<IReadOnlyList<TAggregateRoot>> ListAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await Items.Where(predicate).ToListAsync(cancellationToken);
        }

        public override async Task<TAggregateRoot?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _items.FindAsync([id], cancellationToken);
        }

        public override Task UpdateAsync(TAggregateRoot entity, CancellationToken cancellationToken = default)
        {
            _items.Update(entity);
            return Task.CompletedTask;
        }
        
        public override async Task<TAggregateRoot?> FindAsync(long[] ids, CancellationToken cancellationToken = default)
        {
            if (ids == null || ids.Length == 0)
                return null;
            
            var id = ids[0];
            return await _items.FindAsync(new object[] { id }, cancellationToken);
        }

    }
}
