using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.IncreasingProcessed.Async.Decorators;

public sealed class AsyncSqlSpeakingIncreaseProcessed(
    ParsersSource parsers,
    IAsyncIncreaseProcessed inner
) : IAsyncIncreaseProcessed
{
    public async Task<Status<ParserStatisticsIncreasement>> Increase(
        AsyncIncreaseProcess increase,
        CancellationToken ct = default
    ) =>
        await new TransactionOperatingParser<ParserStatisticsIncreasement>(parsers, parsers)
            .WithReceivingMethod(s => s.Find(increase.TakeOwnerId(), ct))
            .WithLogicMethod(() => inner.Increase(increase, ct))
            .WithPutting(increase)
            .Process();
}
