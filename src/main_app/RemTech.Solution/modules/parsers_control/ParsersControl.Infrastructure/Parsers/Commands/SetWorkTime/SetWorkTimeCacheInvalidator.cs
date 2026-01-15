using ParsersControl.Core.Features.SetWorkTime;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace ParsersControl.Infrastructure.Parsers.Commands.SetWorkTime;

public sealed class SetWorkTimeCacheInvalidator(
    CachedParserArrayInvalidator arrayInvalidator,
    ParserCacheRecordInvalidator recordInvalidator
) : ICacheInvalidator<SetWorkTimeCommand, SubscribedParser>
{
    public async Task InvalidateCache(
        SetWorkTimeCommand command,
        SubscribedParser result,
        CancellationToken ct = default
    )
    {
        Task[] tasks = [arrayInvalidator.Invalidate(ct), recordInvalidator.Invalidate(result, ct)];

        await Task.WhenAll(tasks);
    }
}
