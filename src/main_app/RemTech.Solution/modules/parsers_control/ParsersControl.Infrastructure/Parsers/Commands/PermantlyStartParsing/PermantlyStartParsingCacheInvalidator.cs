using ParsersControl.Core.Features.PermantlyStartParsing;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Commands.PermantlyStartParsing;

public sealed class PermantlyStartParsingCacheInvalidator(
    CachedParserArrayInvalidator arrayInvalidator, 
    ParserCacheRecordInvalidator recordInvalidator)
    : ICacheInvalidator<PermantlyStartParsingCommand, SubscribedParser>
{
    public async Task InvalidateCache(
        PermantlyStartParsingCommand command,
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