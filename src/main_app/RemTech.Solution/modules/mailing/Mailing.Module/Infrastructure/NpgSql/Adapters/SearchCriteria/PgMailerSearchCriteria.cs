using System.Data;
using Dapper;
using Mailing.Module.Domain.Models;
using Shared.Infrastructure.Module.Postgres;

namespace Mailing.Module.Infrastructure.NpgSql.Adapters.SearchCriteria;

internal abstract class PgMailerSearchCriteria : IMailersSearchCriteria
{
    private PostgresDatabase? _db;

    public void AttachPostgres(PostgresDatabase database) => _db = database;

    protected async Task<IMailer> Handle(CommandDefinition command, CancellationToken ct)
    {
        if (_db == null) return new EmptyMailer();
        using IDbConnection connection = await _db.ProvideConnection(ct: ct);
        TableMailer? postman = await connection.QueryFirstOrDefaultAsync<TableMailer>(command);
        return postman is null ? new EmptyMailer() : postman.ToMailer();
    }

    public abstract Task<IMailer> Find(CancellationToken ct = default);
}