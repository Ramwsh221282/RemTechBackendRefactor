using System.Data.Common;
using Npgsql;
using Pgvector;
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

    private const string RegionsSql = """
        SELECT id, name, kind FROM locations_module.regions
        WHERE id = @id
        """;
    private const string IdParam = "@id";
    private const string IdColumn = "id";
    private const string NameColumn = "name";
    private const string KindColumn = "kind";

    private async Task<PersistedRegion> QueryRegionByCity(
        NpgsqlConnection connection,
        PersistedCity city,
        CancellationToken ct = default
    )
    {
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = RegionsSql;
        command.Parameters.Add(new NpgsqlParameter<Guid>(IdParam, city.RegionId));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            throw new ApplicationException("Region not found.");
        await reader.ReadAsync(ct);
        Guid id = reader.GetGuid(reader.GetOrdinal(IdColumn));
        string name = reader.GetString(reader.GetOrdinal(NameColumn));
        string kind = reader.GetString(reader.GetOrdinal(KindColumn));
        return new PersistedRegion(id, name, kind);
    }

    private const string CitiesSql = """
        SELECT id, region_id, name FROM locations_module.cities
        ORDER BY embedding <=> @embedding
        LIMIT 1;
        """;
    private const string EmbeddingParam = "@embedding";
    private const string RegionIdColumn = "region_id";

    private async Task<PersistedCity> QueryCity(
        NpgsqlConnection connection,
        string text,
        CancellationToken ct = default
    )
    {
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = CitiesSql;
        command.Parameters.AddWithValue(EmbeddingParam, new Vector(generator.Generate(text)));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            throw new ApplicationException("City not found.");
        await reader.ReadAsync(ct);
        Guid id = reader.GetGuid(reader.GetOrdinal(IdColumn));
        Guid regionId = reader.GetGuid(reader.GetOrdinal(RegionIdColumn));
        string name = reader.GetString(reader.GetOrdinal(NameColumn));
        return new PersistedCity(id, regionId, name);
    }
}
