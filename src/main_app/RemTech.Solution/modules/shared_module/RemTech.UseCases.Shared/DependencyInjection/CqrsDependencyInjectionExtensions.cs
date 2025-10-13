using Microsoft.Extensions.DependencyInjection;
using RemTech.UseCases.Shared.Cqrs;

namespace RemTech.UseCases.Shared.DependencyInjection;

public static class CqrsDependencyInjectionExtensions
{
    public static ICommandHandler<TCommand> GetCommandHandler<TCommand>(
        this AsyncServiceScope scope
    )
        where TCommand : ICommand =>
        scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand>>();

    public static ICommandHandler<TCommand, TResult> GetCommandHandler<TCommand, TResult>(
        this AsyncServiceScope scope
    )
        where TCommand : ICommand<TResult> =>
        scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
}
