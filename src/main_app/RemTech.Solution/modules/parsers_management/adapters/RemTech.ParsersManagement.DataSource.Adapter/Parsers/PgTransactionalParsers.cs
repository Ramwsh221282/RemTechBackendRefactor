using Npgsql;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers;

public sealed class PgTransactionalParsers(NpgsqlDataSource source, IParsers origin) : IParsers
{
    public void Dispose() => origin.Dispose();

    public ValueTask DisposeAsync() => origin.DisposeAsync();

    public async Task<Status<IParser>> Find(Name name, CancellationToken ct = default)
    {
        Status<IParser> inner = await origin.Find(name, ct);
        return inner.IsSuccess
            ? new PgParser(inner.Value, await source.OpenConnectionAsync(ct))
            : inner.Error;
    }

    public async Task<Status<IParser>> Find(NotEmptyGuid id, CancellationToken ct = default)
    {
        Status<IParser> inner = await origin.Find(id, ct);
        return inner.IsSuccess
            ? new PgParser(inner.Value, await source.OpenConnectionAsync(ct))
            : inner.Error;
    }

    public async Task<Status<IParser>> Find(
        ParsingType type,
        NotEmptyString domain,
        CancellationToken ct = default
    )
    {
        Status<IParser> inner = await origin.Find(type, domain, ct);
        return inner.IsSuccess
            ? new PgParser(inner.Value, await source.OpenConnectionAsync(ct))
            : inner.Error;
    }

    public Task<Status> Add(IParser parser, CancellationToken ct = default) =>
        origin.Add(parser, ct);
}
