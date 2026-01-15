using ParsersControl.Core.Features.ChangeWaitDays;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

namespace ParsersControl.Infrastructure.Parsers.Commands.ChangeWaitDays;

public sealed class ChangeWaitDaysCacheInvalidator(
    CachedParserArrayInvalidator arrayInvalidator,
    ParserCacheRecordInvalidator recordInvalidator
) : ICacheInvalidator<ChangeWaitDaysCommand, SubscribedParser>
{
    public async Task InvalidateCache(
        ChangeWaitDaysCommand command,
        SubscribedParser result,
        CancellationToken ct = default
    )
    {
        Task[] tasks = [arrayInvalidator.Invalidate(ct), recordInvalidator.Invalidate(result, ct)];

        await Task.WhenAll(tasks);
    }
}
