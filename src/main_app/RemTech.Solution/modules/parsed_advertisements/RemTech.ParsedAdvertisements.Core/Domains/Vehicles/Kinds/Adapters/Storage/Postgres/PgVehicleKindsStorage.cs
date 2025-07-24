using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Ports.Storage.Postgres;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Adapters.Storage.Postgres;

public sealed class PgVehicleKindsStorage(PgConnectionSource connectionSource) : IPgVehicleKindsStorage
{
    public async Task<VehicleKind> Read(VehicleKind kind, CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await connectionSource.Connect(ct);
        return await kind.ToStoreCommand().Execute(connection, ct) != 1
            ? throw new ArgumentException($"Duplicate vehicle kind with name: {kind.Identify().ReadText()}")
            : kind;
    }
}