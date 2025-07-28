using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;
using Serilog;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.EnablingParser.Async.Decorators;

public sealed class AsyncLoggingEnabledParser(ILogger logger, IAsyncEnabledParser inner)
    : IAsyncEnabledParser
{
    public async Task<Status<IParser>> EnableAsync(
        AsyncEnableParser enable,
        CancellationToken ct = default)
    {
        logger.Information("Асинхронное включение парсера начато.");
        Status<IParser> parser = await inner.EnableAsync(enable, ct);
        logger.Information("Асинхронное включение парсера закончено.");
        return parser;
    }
}
