using System.Linq.Expressions;
using NotifySystem.Core.Domain.SharedKernel.Base;

namespace NotifySystem.Core.Domain.SharedKernel.Specification;

public interface ISpecification<TAggregateRoot> where TAggregateRoot : class, IAggregateRoot
{
    Expression<Func<TAggregateRoot, bool>> IsSatisfiedBy();
}