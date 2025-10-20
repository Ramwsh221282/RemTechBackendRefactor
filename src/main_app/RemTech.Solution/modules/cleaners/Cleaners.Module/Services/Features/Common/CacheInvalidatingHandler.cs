using Cleaners.Module.Cache;
using Cleaners.Module.Domain;
using RemTech.Core.Shared.Cqrs;

namespace Cleaners.Module.Services.Features.Common;

internal sealed class CacheInvalidatingHandler<TCommand>(
    ICleanerCache cache,
    ICommandHandler<TCommand, ICleaner> inner
) : ICommandHandler<TCommand, ICleaner>
    where TCommand : ICommand
{
    public async Task<ICleaner> Handle(TCommand command, CancellationToken ct = default)
    {
        ICleaner cleaner = await inner.Handle(command, ct);
        await cache.Invalidate(cleaner);
        return cleaner;
    }
}
