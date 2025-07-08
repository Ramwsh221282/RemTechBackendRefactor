using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingNewParser.Async.Decorators;

public sealed class AsyncSqlSpeakingNewParser(ParsersSource parsers, IAsyncNewParser inner)
    : IAsyncNewParser
{
    private readonly IParsers _parsers = parsers;

    public async Task<Status<IParser>> Register(
        AsyncAddNewParser add,
        CancellationToken ct = default
    )
    {
        await using (_parsers)
        {
            Status<IParser> processed = await inner.Register(add, ct);
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
