using ParsersControl.Core.Features.DeleteLinkFromParser;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Commands.DeleteLinkFromParser;

public sealed class DeleteLinkFromParserCacheInvalidator(
    CachedParserArrayInvalidator arrayInvalidator,
    ParserCacheRecordInvalidator recordInvalidator)
    : ICacheInvalidator<DeleteLinkFromParserCommand, SubscribedParserLink>
{
    public async Task InvalidateCache(
        DeleteLinkFromParserCommand command,
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