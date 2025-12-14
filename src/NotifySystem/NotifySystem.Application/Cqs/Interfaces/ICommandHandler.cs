using NotifySystem.Core.Domain.SharedKernel.Result;

namespace NotifySystem.Application.Cqs.Interfaces;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task<Result> Handle(TCommand command, CancellationToken cancellationToken);
}