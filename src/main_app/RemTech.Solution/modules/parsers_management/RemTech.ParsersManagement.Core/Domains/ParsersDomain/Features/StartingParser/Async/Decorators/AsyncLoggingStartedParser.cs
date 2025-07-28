using RemTech.Core.Shared.Functional;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;
using Serilog;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StartingParser.Async.Decorators;

public sealed class AsyncLoggingStartedParser(ILogger logger, IAsyncStartedParser inner)
    : IAsyncStartedParser
{
    public async Task<Status<IParser>> StartedAsync(
        AsyncStartParser start,
        CancellationToken ct = default
    )
    {
        logger.Information("Асинхронный запсук парсера начат.");
        Status<IParser> parser = await inner.StartedAsync(start, ct);
        logger.Information("Асинхронный запсук парсера закончен.");
        return parser;
    }
}
