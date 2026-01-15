using ParsersControl.Core.Features.PermantlyDisableParsing;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace ParsersControl.Infrastructure.Parsers.Commands.PermantlyDisableParsing;

public sealed class PermantlyDisableParsingCacheInvalidator(
    CachedParserArrayInvalidator arrayInvalidator,
    ParserCacheRecordInvalidator recordInvalidator
) : ICacheInvalidator<PermantlyDisableParsingCommand, SubscribedParser>
{
    public async Task InvalidateCache(
        PermantlyDisableParsingCommand command,
        SubscribedParser result,
        CancellationToken ct = default
    )
    {
        Task[] tasks = [arrayInvalidator.Invalidate(ct), recordInvalidator.Invalidate(result, ct)];

        await Task.WhenAll(tasks);
    }
}
