using System.Linq.Expressions;
using NotifySystem.Core.Domain.SharedKernel.Base;

namespace NotifySystem.Core.Domain.SharedKernel.Specification;

public static class SpecificationExtensions
{
    public static ISpecification<TAggregateRoot> And<TAggregateRoot>(
        this ISpecification<TAggregateRoot> left,
        ISpecification<TAggregateRoot> right)
        where TAggregateRoot : class, IAggregateRoot
    {
        if (left == null) return right;

        var leftExpr = left.IsSatisfiedBy();
        var rightExpr = right.IsSatisfiedBy();
        
        var param = Expression.Parameter(typeof(TAggregateRoot));
        
        var leftBody = Expression.Invoke(leftExpr, param);
        var rightBody = Expression.Invoke(rightExpr, param);

        var body = Expression.AndAlso(leftBody, rightBody);

        var combinedExpr = Expression.Lambda<Func<TAggregateRoot, bool>>(body, param);

        return Specification<TAggregateRoot>.Create(combinedExpr);
    }

    public static ISpecification<TAggregateRoot> Or<TAggregateRoot>(
        this ISpecification<TAggregateRoot> left,
        ISpecification<TAggregateRoot> right)
        where TAggregateRoot : class, IAggregateRoot
    {
        if (left == null) return right;
        if (right == null) return left;

        var leftExpr = left.IsSatisfiedBy();
        var rightExpr = right.IsSatisfiedBy();

        var param = Expression.Parameter(typeof(TAggregateRoot));
        var leftBody = Expression.Invoke(leftExpr, param);
        var rightBody = Expression.Invoke(rightExpr, param);

        var body = Expression.OrElse(leftBody, rightBody);

        var combinedExpr = Expression.Lambda<Func<TAggregateRoot, bool>>(body, param);

        return Specification<TAggregateRoot>.Create(combinedExpr);
    }
    
    public static ISpecification<TAggregateRoot> Not<TAggregateRoot>(
        this ISpecification<TAggregateRoot> spec)
        where TAggregateRoot : class, IAggregateRoot
    {
        var expr = spec.IsSatisfiedBy();
        var param = Expression.Parameter(typeof(TAggregateRoot));
        var body = Expression.Not(Expression.Invoke(expr, param));
        var lambda = Expression.Lambda<Func<TAggregateRoot, bool>>(body, param);
        return Specification<TAggregateRoot>.Create(lambda);
    }
    
}