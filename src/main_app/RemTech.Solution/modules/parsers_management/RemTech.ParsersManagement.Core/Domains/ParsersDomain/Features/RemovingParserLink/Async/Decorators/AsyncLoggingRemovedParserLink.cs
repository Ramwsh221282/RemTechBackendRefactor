using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;
using Serilog;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.RemovingParserLink.Async.Decorators;

public sealed class AsyncLoggingRemovedParserLink(
    ILogger logger,
    IAsyncRemovedParserLink inner
) : IAsyncRemovedParserLink
{
    public async Task<Status<IParserLink>> AsyncRemoved(
        AsyncRemoveParserLink remove,
        CancellationToken ct = default
    )
    {
        logger.Information("Асинхронное удаление ссылки у парсера начато.");
        Status<IParserLink> link = await inner.AsyncRemoved(remove, ct);
        logger.Information("Асинхронное удаление ссылки у парсера законечно.");
        return link;
    }
}
