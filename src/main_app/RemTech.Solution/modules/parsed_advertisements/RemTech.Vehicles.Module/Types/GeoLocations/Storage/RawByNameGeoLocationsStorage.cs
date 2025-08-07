using System.Data.Common;
using Npgsql;
using RemTech.Core.Shared.Primitives;
using RemTech.Vehicles.Module.Types.GeoLocations.ValueObjects;

namespace RemTech.Vehicles.Module.Types.GeoLocations.Storage;

internal sealed class RawByNameGeoLocationsStorage(NpgsqlDataSource dataSource)
    : IGeoLocationsStorage
{
    public async Task<GeoLocation> Save(GeoLocation geoLocation)
    {
        string sql = string.Intern(
            """
            SELECT id, text, kind FROM parsed_advertisements_module.geos
            WHERE text = @input
            """
        );
        string parameter = geoLocation.Name();
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@input", parameter));
        await using DbDataReader reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            throw new UnableToStoreGeoLocationException(
                "Не удается найти гео локацию по pg tgrm запросу.",
                geoLocation.Name()
            );
        Guid id = reader.GetGuid(reader.GetOrdinal("id"));
        string text = reader.GetString(reader.GetOrdinal("text"));
        string kind = reader.GetString(reader.GetOrdinal("kind"));
        GeoLocationIdentity identity = new(
            new GeoLocationId(new NotEmptyGuid(id)),
            new GeolocationText(text),
            new GeolocationText(kind)
        );
        return geoLocation.ChangeIdentity(identity);
    }
}
