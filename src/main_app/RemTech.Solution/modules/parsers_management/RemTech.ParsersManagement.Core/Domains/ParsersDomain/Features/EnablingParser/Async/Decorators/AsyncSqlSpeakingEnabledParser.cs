using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.EnablingParser.Async.Decorators;

public sealed class AsyncSqlSpeakingEnabledParser : IAsyncEnabledParser
{
    private readonly IParsers _parsers;
    private readonly ITransactionalParsers _transactionalParsers;
    private readonly IAsyncEnabledParser _inner;

    public AsyncSqlSpeakingEnabledParser(ParsersSource source, IAsyncEnabledParser inner)
    {
        _parsers = source;
        _transactionalParsers = source;
        _inner = inner;
    }

    public async Task<Status<IParser>> EnableAsync(
        AsyncEnableParser enable,
        CancellationToken ct = default
    )
    {
        Status<IParser> parser = await _parsers.Find(enable.WhomEnable(), ct);
        if (parser.IsFailure)
            return parser;
        await using ITransactionalParser transactional = await _transactionalParsers.Add(
            parser.Value,
            ct
        );
        IParser transactionalParser = transactional;
        enable.PutParser(transactionalParser);
        Status<IParser> inner = await _inner.EnableAsync(enable, ct);
        if (inner.IsFailure)
            return inner;
        Status commit = await transactional.Save(ct);
        return commit.IsFailure ? commit.Error : inner;
    }
}
