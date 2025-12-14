using NotifySystem.Application.Cqs.Interfaces;
using NotifySystem.Core.Domain.SharedKernel.Result;

namespace NotifySystem.Application.Cqs;

public abstract class QueryHandler<TQuery, TResult> : HandleBase<TQuery, Result<TResult>>, IQueryHandler<TQuery, Result<TResult>> 
    where TQuery : Query<TResult>
{
    protected Result Success() => Result.Success();
    protected Result<TResult> Success(TResult result) => Result<TResult>.Success(result);
    protected Result<TResult> Error(IError error) => Result<TResult>.Error(error);
}