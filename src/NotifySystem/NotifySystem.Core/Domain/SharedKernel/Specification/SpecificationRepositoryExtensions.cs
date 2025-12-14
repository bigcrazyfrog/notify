using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using NotifySystem.Core.Domain.SharedKernel.Base;
using NotifySystem.Core.Domain.SharedKernel.Storage;

namespace NotifySystem.Core.Domain.SharedKernel.Specification
{
    public static class SpecificationRepositoryExtensions
    {
        public static Task<IReadOnlyList<TEntity>> ListAsync<TEntity>(
            this IReadonlyRepository<TEntity> readonlyRepository, ISpecification<TEntity> specification,
            CancellationToken cancellationToken) where TEntity : Entity, IAggregateRoot
        {
            return readonlyRepository.ListAsync(specification.IsSatisfiedBy(), cancellationToken);
        }

        public static async Task<TEntity?> FirstOrDefaultAsync<TEntity>(
            this IReadonlyRepository<TEntity> readonlyRepository, ISpecification<TEntity> specification,
            CancellationToken cancellationToken) where TEntity : Entity, IAggregateRoot
        {
            var list = await readonlyRepository.ListAsync(specification.IsSatisfiedBy(), cancellationToken);
            return list.FirstOrDefault();
        }

        public static async Task<TEntity> FirstAsync<TEntity>(
            this IReadonlyRepository<TEntity> readonlyRepository, ISpecification<TEntity> specification,
            CancellationToken cancellationToken) where TEntity : Entity, IAggregateRoot
        {
            var list = await readonlyRepository.ListAsync(specification.IsSatisfiedBy(), cancellationToken);
            return list.First();
        }

        public static async Task<TEntity?> SingleOrDefaultAsync<TEntity>(
            this IReadonlyRepository<TEntity> readonlyRepository, ISpecification<TEntity> specification,
            CancellationToken cancellationToken) where TEntity : Entity, IAggregateRoot
        {
            var list = await readonlyRepository.ListAsync(specification.IsSatisfiedBy(), cancellationToken);
            return list.SingleOrDefault();
        }

        public static async Task<TEntity> SingleAsync<TEntity>(
            this IReadonlyRepository<TEntity> readonlyRepository, ISpecification<TEntity> specification,
            CancellationToken cancellationToken) where TEntity : Entity, IAggregateRoot
        {
            var list = await readonlyRepository.ListAsync(specification.IsSatisfiedBy(), cancellationToken);
            return list.Single();
        }

        public static async Task<int> CountAsync<TEntity>(
            this IReadonlyRepository<TEntity> readonlyRepository, ISpecification<TEntity> specification,
            CancellationToken cancellationToken) where TEntity : Entity, IAggregateRoot
        {
            var list = await readonlyRepository.ListAsync(specification.IsSatisfiedBy(), cancellationToken);
            return list.Count;
        }

        public static async Task<bool> AnyAsync<TEntity>(
            this IReadonlyRepository<TEntity> readonlyRepository, ISpecification<TEntity> specification,
            CancellationToken cancellationToken) where TEntity : Entity, IAggregateRoot
        {
            var list = await readonlyRepository.ListAsync(specification.IsSatisfiedBy(), cancellationToken);
            return list.Any();
        }

        public static async Task<bool> AllAsync<TEntity>(
            this IReadonlyRepository<TEntity> readonlyRepository, ISpecification<TEntity> specification,
            CancellationToken cancellationToken) where TEntity : Entity, IAggregateRoot
        {
            var list = await readonlyRepository.ListAsync(specification.IsSatisfiedBy(), cancellationToken);
            return list.All(specification.IsSatisfiedBy().Compile());
        }
    }
}