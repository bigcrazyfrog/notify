namespace NotifySystem.Core.Domain.SharedKernel.Result;

public interface IResult
{
    bool IsSuccess { get; }
    IReadOnlyList<IError> GetErrors();
}