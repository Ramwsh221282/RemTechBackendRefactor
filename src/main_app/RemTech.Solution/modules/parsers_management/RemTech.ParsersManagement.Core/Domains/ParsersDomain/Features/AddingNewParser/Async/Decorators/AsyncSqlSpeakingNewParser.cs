using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingNewParser.Async.Decorators;

public sealed class AsyncSqlSpeakingNewParser : IAsyncNewParser
{
    private readonly IParsers _parsers;
    private readonly IAsyncNewParser _inner;

    public AsyncSqlSpeakingNewParser(ParsersSource parsers, IAsyncNewParser inner)
    {
        _parsers = parsers;
        _inner = inner;
    }

    public async Task<Status<IParser>> Register(
        AsyncAddNewParser add,
        CancellationToken ct = default
    )
    {
        await using (_parsers)
        {
            Status<IParser> processed = await _inner.Register(add, ct);
            if (processed.IsFailure)
                return processed;
            Status<IParser> existing = await _parsers.Find(
                processed.Value.Identification().ReadType(),
                processed.Value.Domain(),
                ct
            );
            if (existing.IsSuccess)
                return Error.Conflict(
                    $"Парсер с типом: {processed.Value.Identification().ReadType().Read().StringValue()} и доменом: {processed.Value.Domain().Read().NameString().StringValue()} уже существует"
                );
            Status adding = await _parsers.Add(processed.Value, ct);
            if (adding.IsFailure)
                return adding.Error;
            return processed;
        }
    }
}
