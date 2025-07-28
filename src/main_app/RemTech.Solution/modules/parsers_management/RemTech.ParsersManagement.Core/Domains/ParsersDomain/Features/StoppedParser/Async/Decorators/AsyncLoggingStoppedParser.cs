using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;
using Serilog;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StoppedParser.Async.Decorators;

public sealed class AsyncLoggingStoppedParser(ILogger logger, IAsyncStoppedParser inner)
    : IAsyncStoppedParser
{
    public async Task<Status<IParser>> AsyncStopped(
        AsyncStopParser stop,
        CancellationToken ct = default
    )
    {
        logger.Information("Асинхронная остановка парсера начата.");
        Status<IParser> parser = await inner.AsyncStopped(stop, ct);
        logger.Information("Асинхронная остановка парсера закончена.");
        return parser;
    }
}
