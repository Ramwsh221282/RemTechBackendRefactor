using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Common.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.IncreasingProcessed.Async.Decorators;

public sealed class AsyncLoggingIncreaseProcessed(
    ICustomLogger logger,
    IAsyncIncreaseProcessed inner
) : IAsyncIncreaseProcessed
{
    public async Task<Status<ParserStatisticsIncreasement>> Increase(
        AsyncIncreaseProcess increase,
        CancellationToken ct = default
    ) =>
        await new AsyncLoggingOperation<Status<ParserStatisticsIncreasement>>(
            logger,
            "Увеличение количества обработанных ссылок у парсера"
        ).Log(inner.Increase(increase, ct));
}
