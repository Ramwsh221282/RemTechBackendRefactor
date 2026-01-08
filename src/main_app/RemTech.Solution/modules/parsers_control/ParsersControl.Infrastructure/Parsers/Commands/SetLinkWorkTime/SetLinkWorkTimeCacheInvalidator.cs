using ParsersControl.Core.Features.SetLinkWorkTime;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Commands.SetLinkWorkTime;

public sealed class SetLinkWorkTimeCacheInvalidator(CachedParserArrayInvalidator invalidator)
    : ICacheInvalidator<SetLinkWorkingTimeCommand, SubscribedParserLink>
{
    public async Task InvalidateCache(
        SetLinkWorkingTimeCommand command, 
        SubscribedParserLink result,
        CancellationToken ct = default) =>
        await invalidator.Invalidate(ct);    
}