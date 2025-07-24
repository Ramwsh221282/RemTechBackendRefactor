using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Ports.Storage.Postgres;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Adapters.Storage.Postgres;

public sealed class PgDuplicateResolvingVehicleModelsStorage(PgConnectionSource connectionSource) : IPgVehicleModelsStorage
{
    public async Task<VehicleModel> Get(VehicleModel model, CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await connectionSource.Connect(ct);
        return await model.FromStoreCommand().Fetch(connection, ct);
    }
}