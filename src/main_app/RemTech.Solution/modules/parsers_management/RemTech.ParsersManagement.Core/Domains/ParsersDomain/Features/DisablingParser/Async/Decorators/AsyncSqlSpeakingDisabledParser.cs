using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.DisablingParser.Async.Decorators;

public sealed class AsyncSqlSpeakingDisabledParser : IAsyncDisabledParser
{
    private readonly IParsers _parsers;
    private readonly ITransactionalParsers _transactionalParsers;
    private readonly IAsyncDisabledParser _inner;

    public AsyncSqlSpeakingDisabledParser(ParsersSource parsers, IAsyncDisabledParser inner)
    {
        _parsers = parsers;
        _transactionalParsers = parsers;
        _inner = inner;
    }

    public async Task<Status<IParser>> Disable(
        AsyncDisableParser disable,
        CancellationToken ct = default
    )
    {
        IMaybeParserId maybeId = disable;
        Status<IParser> fromDatabase = await _parsers.Find(maybeId.Take(), ct);
        if (fromDatabase.IsFailure)
            return fromDatabase.Error;
        await using ITransactionalParser transactional = await _transactionalParsers.Add(
            fromDatabase.Value,
            ct
        );
        disable.Put(transactional);
        Status<IParser> disabled = await _inner.Disable(disable, ct);
        if (disabled.IsFailure)
            return disabled.Error;
        Status saving = await transactional.Save(ct);
        return saving.IsSuccess ? disabled : saving.Error;
    }
}
