using ParsersControl.Core.Features.DisableParser;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Commands.DisableParser;

public sealed class DisableParserCacheInvalidator(
    CachedParserArrayInvalidator arrayInvalidator, 
    ParserCacheRecordInvalidator recordInvalidator)
    : ICacheInvalidator<DisableParserCommand, SubscribedParser>
{
    public async Task InvalidateCache(
        DisableParserCommand command, 
        SubscribedParser result,
        CancellationToken ct = default)
    {
        Task[] tasks = 
            [
                arrayInvalidator.Invalidate(ct),
                recordInvalidator.Invalidate(result, ct)
            ];
        
        await Task.WhenAll(tasks);
    }
}