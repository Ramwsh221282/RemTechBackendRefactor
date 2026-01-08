using ParsersControl.Core.Features.UpdateParserLinks;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Commands.UpdateParserLinks;

public sealed class UpdateParserLinksCacheInvalidator(CachedParserArrayInvalidator invalidator)
    : ICacheInvalidator<UpdateParserLinksCommand, IEnumerable<SubscribedParserLink>>
{
    public async Task InvalidateCache(
        UpdateParserLinksCommand command, 
        IEnumerable<SubscribedParserLink> result,
        CancellationToken ct = default) =>
        await invalidator.Invalidate(ct);    
}