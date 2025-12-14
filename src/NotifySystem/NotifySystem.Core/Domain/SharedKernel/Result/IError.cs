namespace NotifySystem.Core.Domain.SharedKernel.Result;

public interface IError
{
    string Type { get; }
    Dictionary<string, object> Data { get; }
}