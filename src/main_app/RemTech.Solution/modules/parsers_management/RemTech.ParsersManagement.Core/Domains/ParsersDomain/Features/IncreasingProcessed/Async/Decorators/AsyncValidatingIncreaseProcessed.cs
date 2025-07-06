using RemTech.ParsersManagement.Core.Common.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.IncreasingProcessed.Async.Decorators;

public sealed class AsyncValidatingIncreaseProcessed(IAsyncIncreaseProcessed inner)
    : IAsyncIncreaseProcessed
{
    public Task<Status<ParserStatisticsIncreasement>> Increase(
        AsyncIncreaseProcess increase,
        CancellationToken ct = default
    ) => new AsyncValidatingOperation(increase).Process(inner.Increase(increase, ct));
}
