using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;
using Serilog;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.ChangingLinkActivity.Async.Decorators;

public sealed class AsyncLoggingChangedLinkActivity(
    ILogger logger,
    IAsyncChangedLinkActivity inner
) : IAsyncChangedLinkActivity
{
    public async Task<Status<IParserLink>> AsyncChangedActivity(
        AsyncChangeLinkActivity change,
        CancellationToken ct = default)
    {
        logger.Information("Асинхронное изменение активности ссылки начато.");
        Status<IParserLink> link = await inner.AsyncChangedActivity(change, ct);
        logger.Information("Асинхронное добавление активности ссылки закончено.");
        return link;
    }
}
