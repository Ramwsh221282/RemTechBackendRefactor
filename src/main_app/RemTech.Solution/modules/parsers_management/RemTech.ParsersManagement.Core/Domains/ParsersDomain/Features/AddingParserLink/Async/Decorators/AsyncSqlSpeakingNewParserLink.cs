using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingParserLink.Async.Decorators;

public sealed class AsyncSqlSpeakingNewParserLink(ParsersSource parsers, IAsyncNewParserLink inner)
    : IAsyncNewParserLink
{
    public async Task<Status<IParserLink>> AsyncNew(
        AsyncAddParserLink add,
        CancellationToken ct = default
    ) =>
        await new TransactionOperatingParser<IParserLink>(parsers, parsers)
            .WithReceivingMethod(s => s.Find(add.TakeOwnerId(), ct))
            .WithLogicMethod(() => inner.AsyncNew(add, ct))
            .WithPutting(add)
            .Process();
}
