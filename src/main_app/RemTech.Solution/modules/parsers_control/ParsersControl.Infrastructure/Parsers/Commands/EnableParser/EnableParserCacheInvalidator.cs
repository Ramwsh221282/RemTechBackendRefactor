using ParsersControl.Core.Features.EnableParser;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace ParsersControl.Infrastructure.Parsers.Commands.EnableParser;

public sealed class EnableParserCacheInvalidator(
    CachedParserArrayInvalidator arrayInvalidator,
    ParserCacheRecordInvalidator recordInvalidator
) : ICacheInvalidator<EnableParserCommand, SubscribedParser>
{
    public async Task InvalidateCache(
        EnableParserCommand command,
        SubscribedParser result,
        CancellationToken ct = default
    )
    {
        Task[] tasks = [arrayInvalidator.Invalidate(ct), recordInvalidator.Invalidate(result, ct)];

        await Task.WhenAll(tasks);
    }
}
