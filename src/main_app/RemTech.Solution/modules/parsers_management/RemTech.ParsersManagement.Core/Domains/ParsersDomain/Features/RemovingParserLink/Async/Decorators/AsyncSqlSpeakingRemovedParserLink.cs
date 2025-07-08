using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.RemovingParserLink.Async.Decorators;

public sealed class AsyncSqlSpeakingRemovedParserLink(
    ParsersSource parsers,
    IAsyncRemovedParserLink inner
) : IAsyncRemovedParserLink
{
    public async Task<Status<IParserLink>> AsyncRemoved(
        AsyncRemoveParserLink remove,
        CancellationToken ct = default
    ) =>
        await new TransactionOperatingParser<IParserLink>(parsers, parsers)
            .WithReceivingMethod(s => s.Find(remove.TakeOwnerId(), ct))
            .WithLogicMethod(() => inner.AsyncRemoved(remove, ct))
            .WithPutting(remove)
            .Process();
}
