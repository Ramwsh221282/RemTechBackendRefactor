using Npgsql;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers;

public sealed class PgTransactionalParsers : ITransactionalParsers
{
    private readonly PostgreSqlEngine _engine;

    public PgTransactionalParsers(PostgreSqlEngine engine) => _engine = engine;

    public async Task<ITransactionalParser> Add(IParser parser, CancellationToken ct = default)
    {
        NpgsqlConnection connection = await _engine.Connect(ct);
        return new PgTransactionalParser(
            parser,
            new PgTransactionalParserJournal(
                connection,
                await connection.BeginTransactionAsync(ct)
            ),
            ct
        );
    }

    public void Dispose() => _engine.Dispose();

    public async ValueTask DisposeAsync() => await _engine.DisposeAsync();
}
