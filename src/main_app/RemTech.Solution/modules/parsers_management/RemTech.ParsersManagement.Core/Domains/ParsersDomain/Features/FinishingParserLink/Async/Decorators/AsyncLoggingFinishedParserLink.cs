using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;
using Serilog;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.FinishingParserLink.Async.Decorators;

public sealed class AsyncLoggingFinishedParserLink(
    ILogger logger,
    IAsyncFinishedParserLink inner
) : IAsyncFinishedParserLink
{
    public async Task<Status<IParserLink>> AsyncFinished(
        AsyncFinishParserLink finish,
        CancellationToken ct = default
    )
    {
        logger.Information("Асинхронная остановка ссылки парсера начата.");
        Status<IParserLink> finishing = await inner.AsyncFinished(finish, ct);
        logger.Information("Асинхронная остановка ссылки парсера закончена.");
        return finishing;
    }
}
