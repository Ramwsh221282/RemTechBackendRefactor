using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;
using Serilog;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.DisablingParser.Async.Decorators;

public sealed class AsyncLoggingDisabledParser(ILogger logger, IAsyncDisabledParser inner)
    : IAsyncDisabledParser
{
    public async Task<Status<IParser>> Disable(
        AsyncDisableParser disable,
        CancellationToken ct = default
    )
    {
        logger.Information("Асинхронное выключение парсера начато.");
        Status<IParser> parser = await inner.Disable(disable, ct);
        logger.Information("Асинхронное выключение парсера закончено.");
        return parser;
    }
}
