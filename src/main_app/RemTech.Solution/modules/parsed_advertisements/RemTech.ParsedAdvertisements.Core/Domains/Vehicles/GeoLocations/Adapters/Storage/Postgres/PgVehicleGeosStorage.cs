using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Ports.Storage.Postgres;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Adapters.Storage.Postgres;

public sealed class PgVehicleGeosStorage(PgConnectionSource connectionSource) : IPgVehicleGeosStorage
{
    public async Task<GeoLocation> Get(GeoLocation location, CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await connectionSource.Connect(ct);
        return await location.FromStoreCommand().Fetch(connection, ct);
    }
}