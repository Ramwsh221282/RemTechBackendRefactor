using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;
using Serilog;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.UpdatingParser.Async.Decorators;

public sealed class AsyncLoggingUpdatedParser(ILogger logger, IAsyncUpdatedParser inner)
    : IAsyncUpdatedParser
{
    public async Task<Status<IParser>> Update(
        AsyncUpdateParser update,
        CancellationToken ct = default
    )
    {
        logger.Information("Асинхронное обновление и(или) дней ожидания парсера начато.");
        Status<IParser> parser = await inner.Update(update, ct);
        logger.Information("Асинхронное обновление и(или) дней ожидания парсера закончено.");
        return parser;
    }
}
