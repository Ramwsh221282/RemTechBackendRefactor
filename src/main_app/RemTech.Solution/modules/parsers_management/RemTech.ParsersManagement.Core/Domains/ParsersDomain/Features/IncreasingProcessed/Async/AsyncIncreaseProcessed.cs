using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.IncreasingProcessed.Async;

public sealed class AsyncIncreaseProcessed(IIncreaseProcessed inner) : IAsyncIncreaseProcessed
{
    public Task<Status<ParserStatisticsIncreasement>> Increase(
        AsyncIncreaseProcess increase,
        CancellationToken ct = default
    ) =>
        Task.FromResult(
            inner.IncreaseProcessed(
                new IncreaseProcessed(increase.Take(), increase.WhomIncreaseId())
            )
        );
}