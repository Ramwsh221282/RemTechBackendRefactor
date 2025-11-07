using System.Data;
using Mailing.Module.Domain.Models;
using Mailing.Module.Infrastructure.NpgSql.Adapters.SearchCriteria;
using Shared.Infrastructure.Module.Postgres;

namespace Mailing.Module.Infrastructure.NpgSql.Adapters.Storage;

internal sealed class PgPostmans(PostgresDatabase database) : IMailersStorage<PgMailerSearchCriteria>, IDisposable
{
    private IDbConnection? _connection;

    public async Task Add(IMailer postman, CancellationToken ct = default)
    {
        IDbConnection conn = await GetConnection(ct);
        await new PgMailer(conn, postman).Save(ct);
    }

    public async Task Remove(IMailer postman, CancellationToken ct = default)
    {
        IDbConnection conn = await GetConnection(ct);
        await new PgMailer(conn, postman).Delete(ct);
    }

    public async Task<IMailer> Find(PgMailerSearchCriteria criteria, CancellationToken ct = default)
    {
        criteria.AttachPostgres(database);
        return await criteria.Find(ct);
    }

    public void Dispose() => _connection?.Dispose();

    private async Task<IDbConnection> GetConnection(CancellationToken ct) =>
        _connection ??= await database.ProvideConnection(ct: ct);
}