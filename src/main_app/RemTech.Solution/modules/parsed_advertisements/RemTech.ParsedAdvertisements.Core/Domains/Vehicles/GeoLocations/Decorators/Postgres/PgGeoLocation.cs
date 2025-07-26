using Npgsql;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Decorators.Postgres;

public sealed class PgGeoLocation(PgConnectionSource connectionSource, GeoLocation origin) : GeoLocation(origin)
{
    public async Task<GeoLocation> SaveAsync(CancellationToken cancellationToken = default)
    {
        await using NpgsqlConnection connection = await connectionSource.Connect(cancellationToken);
        return await FromStoreCommand().Fetch(connection, cancellationToken);
    }
    
    public PgVehicleGeoFromStoreCommand FromStoreCommand() =>
        new(Identity.ReadText());
}