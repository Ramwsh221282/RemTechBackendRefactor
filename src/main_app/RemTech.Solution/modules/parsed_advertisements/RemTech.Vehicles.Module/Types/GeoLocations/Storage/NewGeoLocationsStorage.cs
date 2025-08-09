using Npgsql;
using Pgvector;
using RemTech.Vehicles.Module.Database.Embeddings;

namespace RemTech.Vehicles.Module.Types.GeoLocations.Storage;

internal sealed class NewGeoLocationsStorage(
    NpgsqlDataSource dataSource,
    IEmbeddingGenerator generator
) : IGeoLocationsStorage
{
    public async Task<GeoLocation> Save(GeoLocation geoLocation)
    {
        string sql = string.Intern(
            """
            INSERT INTO parsed_advertisements_module.geos(id, text, kind, embedding) VALUES (@id, @text, @kind, @embedding)
            ON CONFLICT(text) DO NOTHING;
            """
        );
        string name = geoLocation.Name();
        string kind = geoLocation.Kind();
        Guid id = geoLocation.Id();
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<Guid>("@id", id));
        command.Parameters.Add(new NpgsqlParameter<string>("@text", name));
        command.Parameters.Add(new NpgsqlParameter<string>("@kind", kind));
        command.Parameters.AddWithValue(
            "@embedding",
            new Vector(generator.Generate($"{name} {kind}"))
        );
        int affected = await command.ExecuteNonQueryAsync();
        return affected == 0
            ? throw new UnableToStoreGeoLocationException(
                "Невозможно сохранить локацию. Дубликат по имени",
                name
            )
            : geoLocation;
    }
}
