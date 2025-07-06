using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.ChangingLinkActivity.Async.Decorators;

public sealed class AsyncSqlSpeakingChangedLinkActivity(
    ParsersSource source,
    IAsyncChangedLinkActivity inner
) : IAsyncChangedLinkActivity
{
    private readonly IParsers _parsers = source;
    private readonly ITransactionalParsers _transactionalParsers = source;

    public async Task<Status<IParserLink>> AsyncChangedActivity(
        AsyncChangeLinkActivity change,
        CancellationToken ct = default
    )
    {
        await using (_parsers)
        {
            Status<IParser> fromDb = await _parsers.Find(change.OwnerId(), ct);
            if (fromDb.IsFailure)
                return fromDb.Error;
            ITransactionalParser transactional = await _transactionalParsers.Add(fromDb.Value, ct);
            change.Put(transactional);
            Status<IParserLink> status = await inner.AsyncChangedActivity(change, ct);
            if (status.IsFailure)
                return status.Error;
            Status commit = await transactional.Save(ct);
            return commit.IsFailure ? commit.Error : status;
        }
    }
}
