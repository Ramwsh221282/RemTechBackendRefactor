using Npgsql;
using RemTech.Core.Shared.Exceptions;
using RemTech.Postgres.Adapter.Library;
using IsolationLevel = System.Data.IsolationLevel;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators.Postgres;

public sealed class PgTransactionalVehicle(
    PgConnectionSource connectionSource,
    Vehicle origin) : Vehicle(origin)
{
    public async Task SaveAsync(CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await connectionSource.Connect(ct);
        await using NpgsqlTransaction txn = await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
        try
        {
            await new PgSavingVehicle(origin).SaveAsync(connection, ct);
            await new PgCharacteristicsSavingVehicle(origin).SaveAsync(connection, ct);
            await txn.CommitAsync(ct);
        }
        catch(OperationException ex)
        {
            await txn.RollbackAsync(ct);
            throw;
        }
    }
}