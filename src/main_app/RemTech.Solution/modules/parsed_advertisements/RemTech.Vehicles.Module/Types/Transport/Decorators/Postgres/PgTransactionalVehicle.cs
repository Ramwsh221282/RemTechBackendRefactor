using Npgsql;
using RemTech.Core.Shared.Exceptions;
using Shared.Infrastructure.Module.Postgres.Embeddings;
using IsolationLevel = System.Data.IsolationLevel;

namespace RemTech.Vehicles.Module.Types.Transport.Decorators.Postgres;

internal sealed class PgTransactionalVehicle(
    NpgsqlDataSource connectionSource,
    IEmbeddingGenerator generator,
    Vehicle source
) : Vehicle(source)
{
    public async Task SaveAsync(CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await connectionSource.OpenConnectionAsync(ct);
        await using NpgsqlTransaction txn = await connection.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            ct
        );
        try
        {
            await new PgSavingVehicle(source, generator).SaveAsync(connection, ct);
            await txn.CommitAsync(ct);
        }
        catch (OperationException ex)
        {
            await txn.RollbackAsync(ct);
            throw;
        }
    }
}
