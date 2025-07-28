using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;
using Serilog;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.IncreasingProcessed.Async.Decorators;

public sealed class AsyncLoggingIncreaseProcessed(
    ILogger logger,
    IAsyncIncreaseProcessed inner
) : IAsyncIncreaseProcessed
{
    public async Task<Status<ParserStatisticsIncreasement>> Increase(
        AsyncIncreaseProcess increase,
        CancellationToken ct = default
    )
    {
        logger.Information("Асинхронное увеличение количества обработанных данных начато.");
        Status<ParserStatisticsIncreasement> increasement = await inner.Increase(increase, ct);
        logger.Information("Асинхронное увеличение количества обработанных данных закончено.");
        return increasement;
    }
}
