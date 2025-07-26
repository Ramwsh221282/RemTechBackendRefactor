using Npgsql;
using RemTech.Core.Shared.Exceptions;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Decorators.Postgres;

public sealed class PgVarietVehicleKind(PgConnectionSource connectionSource, VehicleKind kind) : VehicleKind(kind)
{
    private readonly Queue<Func<Task<VehicleKind>>> _strategies = [];

    public async Task<VehicleKind> SaveAsync(CancellationToken cancellationToken = default)
    {
        await using NpgsqlConnection connection = await connectionSource.Connect(cancellationToken);
        PgVehicleKind pgKind = new(connection, kind);
        _strategies.Enqueue(() => pgKind.SaveAsync(pgKind.ToStoreCommand(), cancellationToken));
        _strategies.Enqueue(() => pgKind.SaveAsync(pgKind.FromStoreCommand(), cancellationToken));
        while (_strategies.Count > 0)
        {
            try
            {
                VehicleKind kind = await _strategies.Dequeue()();
                return kind;
            }
            catch
            {
                // ignored
            }
        }

        throw new OperationException("Невозможно сохранить тип техники.");
    }
}