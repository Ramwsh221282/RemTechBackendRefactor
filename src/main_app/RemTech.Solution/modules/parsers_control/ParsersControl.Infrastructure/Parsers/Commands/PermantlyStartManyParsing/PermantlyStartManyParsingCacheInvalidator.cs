using ParsersControl.Core.Features.PermantlyStartManyParsing;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace ParsersControl.Infrastructure.Parsers.Commands.PermantlyStartManyParsing;

public sealed class PermantlyStartManyParsingCacheInvalidator(
    CachedParserArrayInvalidator arrayInvalidator,
    ParserCacheRecordInvalidator recordInvalidator
) : ICacheInvalidator<PermantlyStartManyParsingCommand, IEnumerable<SubscribedParser>>
{
    public async Task InvalidateCache(
        PermantlyStartManyParsingCommand command,
        IEnumerable<SubscribedParser> result,
        CancellationToken ct = default
    )
    {
        IEnumerable<Task> recordInvalidationTasks =
        [
            arrayInvalidator.Invalidate(ct),
            .. result.Select(p => recordInvalidator.Invalidate(p, ct)),
        ];

        await Task.WhenAll(recordInvalidationTasks);
    }
}
