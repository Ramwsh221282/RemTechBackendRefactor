using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.UpdatingParser.Async.Decorators;

public sealed class AsyncSqlSpeakingUpdatedParser : IAsyncUpdatedParser
{
    private readonly IParsers _parsers;
    private readonly ITransactionalParsers _transactional;
    private readonly IAsyncUpdatedParser _inner;

    public AsyncSqlSpeakingUpdatedParser(ParsersSource source, IAsyncUpdatedParser inner)
    {
        _parsers = source;
        _transactional = source;
        _inner = inner;
    }

    public async Task<Status<IParser>> Update(
        AsyncUpdateParser update,
        CancellationToken ct = default
    )
    {
        Status<IParser> parser = await _parsers.Find(update.Take(), ct);
        if (parser.IsFailure)
            return parser.Error;
        await using ITransactionalParser transactional = await _transactional.Add(parser.Value, ct);
        update.Put(transactional);
        Status<IParser> updated = await _inner.Update(update, ct);
        if (updated.IsFailure)
            return updated.Error;
        Status saving = await transactional.Save(ct);
        return saving.IsSuccess ? updated : saving.Error;
    }
}
