namespace NotifySystem.Core.Domain.SharedKernel.Result;

public class Result : IResult
{
    public bool IsSuccess => _errors.Count < 1;
    private readonly List<IError> _errors = [];
    
    protected void AddError(IError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        _errors.Add(error);
    }
    
    public IReadOnlyList<IError> GetErrors() => _errors;
    
    private static readonly Result _successResult = new Result();
    public static Result Success() => _successResult;

    public static Result Error(IError error)
    {
        var result = new Result();
        result.AddError(error);
        return result;
    }
}