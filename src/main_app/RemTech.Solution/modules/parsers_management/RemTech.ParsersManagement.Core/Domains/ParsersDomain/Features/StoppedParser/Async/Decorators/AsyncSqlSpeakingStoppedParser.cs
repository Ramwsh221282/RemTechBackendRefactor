using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StoppedParser.Async.Decorators;

public sealed class AsyncSqlSpeakingStoppedParser(ParsersSource source, IAsyncStoppedParser inner)
    : IAsyncStoppedParser
{
    public async Task<Status<IParser>> AsyncStopped(
        AsyncStopParser stop,
        CancellationToken ct = default
    ) =>
        await new TransactionOperatingParser(source, source)
            .WithReceivingMethod(s => s.Find(stop.TakeWhomStopId(), ct))
            .WithLogicMethod(() => inner.AsyncStopped(stop, ct))
            .WithPutting(stop)
            .Process();
}
