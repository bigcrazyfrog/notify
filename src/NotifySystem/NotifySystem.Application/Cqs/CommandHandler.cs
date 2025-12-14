using NotifySystem.Application.Cqs.Interfaces;
using NotifySystem.Core.Domain.SharedKernel.Result;

namespace NotifySystem.Application.Cqs;

public abstract class CommandHandler<TCommand> : HandleBase<TCommand, Result>, ICommandHandler<TCommand> 
    where TCommand : Command
{
    protected Result Success() => Result.Success();
    protected Result Error(IError error) => Result.Error(error);
}