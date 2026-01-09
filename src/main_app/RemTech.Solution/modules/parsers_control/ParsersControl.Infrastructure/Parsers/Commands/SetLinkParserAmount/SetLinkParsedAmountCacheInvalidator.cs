using ParsersControl.Core.Features.SetLinkParsedAmount;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Commands.SetLinkParserAmount;

public sealed class SetLinkParsedAmountCacheInvalidator(
    CachedParserArrayInvalidator arrayInvalidator,
    ParserCacheRecordInvalidator recordInvalidator)
    : ICacheInvalidator<SetLinkParsedAmountCommand, SubscribedParserLink>
{
    public async Task InvalidateCache(
        SetLinkParsedAmountCommand command,
        SubscribedParserLink result,
        CancellationToken ct = default)
    {
        Task[] tasks = 
            [
                arrayInvalidator.Invalidate(ct),
                recordInvalidator.Invalidate(command.ParserId, ct)
            ];
        
        await Task.WhenAll(tasks);
    }
}