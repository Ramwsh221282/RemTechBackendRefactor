using System.Data.Common;
using Npgsql;
using Pgvector;
using RemTech.Core.Shared.Primitives;
using RemTech.Vehicles.Module.Database.Embeddings;
using RemTech.Vehicles.Module.Types.GeoLocations.ValueObjects;

namespace RemTech.Vehicles.Module.Types.GeoLocations.Storage;

internal sealed class VectorSearchGeoLocationStorage(
    NpgsqlDataSource source,
    IEmbeddingGenerator generator
) : IGeoLocationsStorage
{
    public async Task<GeoLocation> Save(GeoLocation geoLocation)
    {
        string sql = string.Intern(
            """
            SELECT id, text, kind FROM parsed_advertisements_module.geos ORDER BY embedding <=> @embedding LIMIT 1;
            """
        );
        await using NpgsqlConnection connection = await source.OpenConnectionAsync();
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        string text = geoLocation.Name();
        command.Parameters.AddWithValue("@embedding", new Vector(generator.Generate(text)));
        await using DbDataReader reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            throw new UnableToStoreGeoLocationException(
                "Не удается найти гео локацию по векторному запросу.",
                text
            );
        Guid id = reader.GetGuid(reader.GetOrdinal("id"));
        string name = reader.GetString(reader.GetOrdinal("text"));
        string kind = reader.GetString(reader.GetOrdinal("kind"));
        GeoLocationIdentity identity = new(
            new GeoLocationId(new NotEmptyGuid(id)),
            new GeolocationText(name),
            new GeolocationText(kind)
        );
        return geoLocation.ChangeIdentity(identity);
    }
}
