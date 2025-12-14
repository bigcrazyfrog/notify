namespace NotifySystem.Core.Domain.SharedKernel.Result;

public interface IResult<out T> : IResult
{
    T Value { get; }
}