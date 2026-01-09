using ParsersControl.Core.Features.EditLinkUrlInfo;
using ParsersControl.Core.ParserLinks.Models;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Commands.EditLinkUrlInfo;

public sealed class EditLinkUrlInfoCacheInvalidator(
    CachedParserArrayInvalidator arrayInvalidator,
    ParserCacheRecordInvalidator recordInvalidator)
    : ICacheInvalidator<EditLinkUrlInfoCommand, SubscribedParserLink>
{
    public async Task InvalidateCache(
        EditLinkUrlInfoCommand command,
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