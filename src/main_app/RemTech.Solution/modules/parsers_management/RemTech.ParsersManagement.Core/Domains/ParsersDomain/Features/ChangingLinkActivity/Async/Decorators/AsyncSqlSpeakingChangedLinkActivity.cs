using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.ChangingLinkActivity.Async.Decorators;

public sealed class AsyncSqlSpeakingChangedLinkActivity(
    ParsersSource source,
    IAsyncChangedLinkActivity inner
) : IAsyncChangedLinkActivity
{
    public async Task<Status<IParserLink>> AsyncChangedActivity(
        AsyncChangeLinkActivity change,
        CancellationToken ct = default
    ) =>
        await new TransactionOperatingParser<IParserLink>(source, source)
            .WithReceivingMethod(s => s.Find(change.OwnerId(), ct))
            .WithLogicMethod(() => inner.AsyncChangedActivity(change, ct))
            .WithPutting(change)
            .Process();
}
