using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace RemTech.SharedKernel.Core.Handlers;

public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand
{
    Task<Result<TResult>> Execute(TCommand command, CancellationToken ct = default);
}