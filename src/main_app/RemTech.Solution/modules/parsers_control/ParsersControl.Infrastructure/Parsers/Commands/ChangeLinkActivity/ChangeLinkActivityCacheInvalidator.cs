using ParsersControl.Core.Features.ChangeLinkActivity;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Commands.ChangeLinkActivity;

public sealed class ChangeLinkActivityCacheInvalidator(CachedParserArrayInvalidator invalidator) : ICacheInvalidator<ChangeLinkActivityCommand, SubscribedParserLink>
{
    public async Task InvalidateCache(
        ChangeLinkActivityCommand command, 
        SubscribedParserLink result,
        CancellationToken ct = default) => await invalidator.Invalidate(ct);
}