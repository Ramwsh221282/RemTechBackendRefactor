using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.UpdatingParser.Async.Decorators;

public sealed class AsyncSqlSpeakingUpdatedParser(ParsersSource source, IAsyncUpdatedParser inner)
    : IAsyncUpdatedParser
{
    public async Task<Status<IParser>> Update(
        AsyncUpdateParser update,
        CancellationToken ct = default
    ) =>
        await new TransactionOperatingParser<IParser>(source, source)
            .WithReceivingMethod(s => s.Find(update.Take(), ct))
            .WithLogicMethod(() => inner.Update(update, ct))
            .WithPutting(update)
            .Process();
}
