using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StartingParser.Async.Decorators;

public sealed class AsyncSqlSpeakingStartedParser(ParsersSource source, IAsyncStartedParser inner)
    : IAsyncStartedParser
{
    private readonly IParsers _parsers = source;
    private readonly ITransactionalParsers _transactionalParsers = source;

    public async Task<Status<IParser>> StartedAsync(
        AsyncStartParser start,
        CancellationToken ct = default
    ) =>
        await new TransactionOperatingParser(_parsers, _transactionalParsers)
            .WithLogicMethod(() => inner.StartedAsync(start, ct))
            .WithPutting(start)
            .WithReceivingMethod(p => p.Find(start.WhomStartId(), ct))
            .Process();
}
