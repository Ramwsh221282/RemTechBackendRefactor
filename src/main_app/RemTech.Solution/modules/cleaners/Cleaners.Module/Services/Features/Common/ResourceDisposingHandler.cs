using Cleaners.Module.Domain;
using RemTech.Core.Shared.Cqrs;

namespace Cleaners.Module.Services.Features.Common;

internal sealed class ResourceDisposingHandler<TCommand>(ICommandHandler<TCommand, ICleaner> inner)
    : ICommandHandler<TCommand, ICleaner>
    where TCommand : ICommand
{
    private readonly List<IDisposable> _disposables = [];

    public ResourceDisposingHandler<TCommand> With(IDisposable disposable)
    {
        _disposables.Add(disposable);
        return this;
    }

    public async Task<ICleaner> Handle(TCommand command, CancellationToken ct = default)
    {
        try
        {
            ICleaner cleaner = await inner.Handle(command, ct);
            return cleaner;
        }
        finally
        {
            DisposeResources();
        }
    }

    private void DisposeResources()
    {
        foreach (var disposable in _disposables)
            disposable.Dispose();
    }
}
