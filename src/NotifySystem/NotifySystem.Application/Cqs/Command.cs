using MediatR;
using NotifySystem.Application.Cqs.Interfaces;
using NotifySystem.Core.Domain.SharedKernel.Result;

namespace NotifySystem.Application.Cqs;

public abstract class Command : IRequest<Result>, ICommand
{
    
}