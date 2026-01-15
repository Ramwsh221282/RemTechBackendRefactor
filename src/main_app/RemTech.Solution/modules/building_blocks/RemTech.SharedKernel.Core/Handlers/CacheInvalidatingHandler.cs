using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace RemTech.SharedKernel.Core.Handlers;

public sealed class CacheInvalidatingHandler<TCommand, TResult>
    : ICacheInvalidatingHandler<TCommand, TResult>
    where TCommand : ICommand
{
    private IEnumerable<ICacheInvalidator<TCommand, TResult>> Invalidators { get; }
    private ICommandHandler<TCommand, TResult> Inner { get; }

    public CacheInvalidatingHandler(
        IEnumerable<ICacheInvalidator<TCommand, TResult>> invalidators,
        ICommandHandler<TCommand, TResult> inner
    )
    {
        Invalidators = invalidators;
        Inner = inner;
    }

    public async Task<Result<TResult>> Execute(TCommand command, CancellationToken ct = default)
    {
        Result<TResult> result = await Inner.Execute(command, ct);
        if (result.IsFailure)
            return result.Error;

        foreach (ICacheInvalidator<TCommand, TResult> invalidator in Invalidators)
            await invalidator.InvalidateCache(command, result.Value, ct);
        return result;
    }
}
