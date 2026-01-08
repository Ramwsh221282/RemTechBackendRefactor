using ParsersControl.Core.Features.AddParserLink;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Commands.AddParserLink;

public sealed class AddParserLinkCacheInvalidator(CachedParserArrayInvalidator invalidator) : ICacheInvalidator<AddParserLinkCommand, IEnumerable<SubscribedParserLink>>
{
    public async Task InvalidateCache(
        AddParserLinkCommand command, 
        IEnumerable<SubscribedParserLink> result, CancellationToken ct = default) =>
        await invalidator.Invalidate(ct);
}