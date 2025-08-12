using System.Data.Common;
using Npgsql;

namespace Shared.Infrastructure.Module.Postgres.PgCommandsBetter;

public sealed record NpgSqlCommandContainer(NpgsqlCommand Command) : IDisposable, IAsyncDisposable
{
    public void Dispose()
    {
        Command.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Command.DisposeAsync();
    }

    public async Task<NpgSqlReaderContainer> Reader(CancellationToken ct = default)
    {
        DbDataReader reader = await Command.ExecuteReaderAsync(ct);
        await DisposeAsync();
        return new NpgSqlReaderContainer(reader);
    }

    public ClosureNpgSqlReader ClosureReader(CancellationToken ct = default)
    {
        return new ClosureNpgSqlReader(async () => await Command.ExecuteReaderAsync(ct));
    }

    public async Task NonQuery(CancellationToken ct = default) =>
        await Command.ExecuteNonQueryAsync(ct);

    public async Task<T> Scalar<T>(CancellationToken ct = default)
    {
        object? scalar = await Command.ExecuteScalarAsync(ct);
        return scalar == null
            ? throw new ApplicationException(
                $"{nameof(NpgSqlCommandContainer)} scalar query resulted in null. Try to use other generic argument."
            )
            : (T)scalar;
    }
}
