using System.Data.Common;
using Npgsql;
using RemTech.Core.Shared.Primitives;
using RemTech.Vehicles.Module.Types.Brands.Decorators.Postgres;
using RemTech.Vehicles.Module.Types.GeoLocations.ValueObjects;

namespace RemTech.Vehicles.Module.Types.GeoLocations.Storage;

internal sealed class UnableToStoreGeoLocationException : Exception
{
    public UnableToStoreGeoLocationException(string message)
        : base(message) { }
}

internal interface IGeoLocationsStorage
{
    Task<GeoLocation> Save(GeoLocation geoLocation);
}

internal sealed class VarietGeoLocationsStorage : IGeoLocationsStorage
{
    private readonly Queue<IGeoLocationsStorage> _storages = [];

    public VarietGeoLocationsStorage With(IGeoLocationsStorage storage)
    {
        _storages.Enqueue(storage);
        return this;
    }

    public async Task<GeoLocation> Save(GeoLocation geoLocation)
    {
        while (_storages.Count > 0)
        {
            IGeoLocationsStorage storage = _storages.Dequeue();
            try
            {
                return await storage.Save(geoLocation);
            }
            catch (UnableToStoreBrandException ex) { }
        }

        throw new UnableToStoreGeoLocationException("Unable to save geo location");
    }
}

internal sealed class PgTgrmGeoLocationsStorage(NpgsqlDataSource dataSource) : IGeoLocationsStorage
{
    public async Task<GeoLocation> Save(GeoLocation geoLocation)
    {
        string sql = string.Intern(
            """
            SELECT id, text, kind, similarity(@input, text) as sml
            FROM parsed_advertisements_module.geos
            WHERE similarity(@input, text) > 0.2
            ORDER BY sml DESC
            LIMIT 1;
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
                "Не удается найти гео локацию по pg tgrm запросу."
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

internal sealed class TsQueryGeoLocationsStorage(NpgsqlDataSource dataSource) : IGeoLocationsStorage
{
    public async Task<GeoLocation> Save(GeoLocation geoLocation)
    {
        string sql = string.Intern(
            """
            WITH input_words AS (
                SELECT
                    unnest(string_to_array(
                            lower(regexp_replace(@input, '[^a-zA-Z0-9А-Яа-я ]', '', 'g')),
                            ' '
                           )) AS input_word
            )
            SELECT
                COALESCE(e.text, 'Не найдено') AS text,
                COALESCE(max_sim.rank, 0) AS rank,
                e.id AS id
            FROM input_words iw
                     LEFT JOIN LATERAL (
                SELECT
                    id,
                    text,
                    kind,
                    ts_rank(geos.document_tsvector, to_tsquery('russian', lower(iw.input_word))) AS rank
                FROM parsed_advertisements_module.geos
                WHERE geos.document_tsvector @@ to_tsquery('russian', lower(iw.input_word))
                ORDER BY rank DESC
                LIMIT 1
                ) e ON true
                     LEFT JOIN LATERAL (
                SELECT ts_rank(geos.document_tsvector, to_tsquery('russian', lower(iw.input_word))) AS rank
                FROM parsed_advertisements_module.geos
                WHERE geos.document_tsvector @@ to_tsquery('russian', lower(iw.input_word))
                ORDER BY rank DESC
                LIMIT 1
                ) max_sim ON true
            WHERE iw.input_word != '' AND max_sim.rank > 0
            ORDER BY rank DESC
            LIMIT 1;
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
                "Не удается найти гео локацию по pg tgrm запросу."
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
                "Не удается найти гео локацию по pg tgrm запросу."
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

internal sealed class NewGeoLocationsStorage(NpgsqlDataSource dataSource) : IGeoLocationsStorage
{
    public async Task<GeoLocation> Save(GeoLocation geoLocation)
    {
        string sql = string.Intern(
            """
            INSERT INTO parsed_advertisements_module.geos(id, text, kind) VALUES (@id, @text, @kind)
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
        int affected = await command.ExecuteNonQueryAsync();
        return affected == 0
            ? throw new UnableToStoreBrandException(
                $"Невозможно сохранить локацию. Дубликат по имени {name}"
            )
            : geoLocation;
    }
}
