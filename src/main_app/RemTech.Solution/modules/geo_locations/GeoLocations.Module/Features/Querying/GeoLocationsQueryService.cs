using System.Data.Common;
using Npgsql;
using Pgvector;
using RemTech.Core.Shared.Exceptions;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace GeoLocations.Module.Features.Querying;

public sealed class GeoLocationsQueryService(
    NpgsqlDataSource dataSource,
    IEmbeddingGenerator generator
) : IGeoLocationQueryService
{
    public async Task<GeoLocationInfo> VectorSearch(string text, CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        PersistedCity city = await QueryCity(connection, text, ct);
        PersistedRegion region = await QueryRegionByCity(connection, city, ct);
        return new GeoLocationInfo(region, city);
    }

    private async Task<PersistedRegion> QueryRegionByCity(
        NpgsqlConnection connection,
        PersistedCity city,
        CancellationToken ct = default
    )
    {
        string sql = string.Intern(
            """
            SELECT id, name, kind FROM locations_module.regions
            WHERE id = @id
            """
        );
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<Guid>("@id", city.RegionId));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            throw new OperationException("Region not found.");
        await reader.ReadAsync(ct);
        Guid id = reader.GetGuid(reader.GetOrdinal("id"));
        string name = reader.GetString(reader.GetOrdinal("name"));
        string kind = reader.GetString(reader.GetOrdinal("kind"));
        return new PersistedRegion(id, name, kind);
    }

    private async Task<PersistedCity> QueryCity(
        NpgsqlConnection connection,
        string text,
        CancellationToken ct = default
    )
    {
        string sql = string.Intern(
            """
            SELECT id, region_id, name FROM locations_module.cities
            ORDER BY embedding <=> @embedding
            LIMIT 1;
            """
        );
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue("@embedding", new Vector(generator.Generate(text)));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            throw new OperationException("City not found.");
        await reader.ReadAsync(ct);
        Guid id = reader.GetGuid(reader.GetOrdinal("id"));
        Guid regionId = reader.GetGuid(reader.GetOrdinal("region_id"));
        string name = reader.GetString(reader.GetOrdinal("name"));
        return new PersistedCity(id, regionId, name);
    }
}
