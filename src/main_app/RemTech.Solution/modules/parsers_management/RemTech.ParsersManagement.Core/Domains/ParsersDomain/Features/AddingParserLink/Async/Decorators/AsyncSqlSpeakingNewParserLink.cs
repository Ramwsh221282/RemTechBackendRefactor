using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingParserLink.Async.Decorators;

public sealed class AsyncSqlSpeakingNewParserLink(ParsersSource parsers, IAsyncNewParserLink inner)
    : IAsyncNewParserLink
{
    private readonly IParsers _parsers = parsers;
    private readonly ITransactionalParsers _transactionalParser = parsers;

    public async Task<Status<IParserLink>> AsyncNew(
        AsyncAddParserLink add,
        CancellationToken ct = default
    )
    {
        await using (_parsers)
        {
            Status<IParser> target = await _parsers.Find(add.TakeOwnerId(), ct);
            if (target.IsFailure)
                return target.Error;
            await using ITransactionalParser transactional = await _transactionalParser.Add(
                target.Value,
                ct
            );
            add.Put(transactional);
            Status<IParserLink> status = await inner.AsyncNew(add, ct);
            if (status.IsFailure)
                return status.Error;
            Status commit = await transactional.Save(ct);
            return commit.IsFailure ? commit.Error : status;
        }
    }
}
