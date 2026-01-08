using ParsersControl.Core.Features.DeleteLinkFromParser;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Commands.DeleteLinkFromParser;

public sealed class DeleteLinkFromParserCacheInvalidator(CachedParserArrayInvalidator invalidator) : ICacheInvalidator<DeleteLinkFromParserCommand, SubscribedParserLink>
{
    public async Task InvalidateCache(
        DeleteLinkFromParserCommand command, 
        SubscribedParserLink result,
        CancellationToken ct = default) =>
        await invalidator.Invalidate(ct);
}