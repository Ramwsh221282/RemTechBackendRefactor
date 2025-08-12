using Npgsql;
using Pgvector;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace GeoLocations.Module.Features.Persisting;

internal sealed class CityToPersist(Guid id, string name)
{
    private const string Sql = """
        INSERT INTO locations_module.cities(id, region_id, name, embedding)
        VALUES(@id, @region_id, @name, @embedding)
        """;

    private const string TextFormat = "{0} {1} {2}";
    private const string IdParam = "@id";
    private const string RegionIdParam = "@region_id";
    private const string NameParam = "@name";
    private const string EmbeddingParam = "@embedding";

    public async Task Persist(
        NpgsqlConnection connection,
        IEmbeddingGenerator generator,
        string regionName,
        string regionType,
        Guid regionId
    )
    {
        string text = string.Format(TextFormat, regionName, regionType, name);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = Sql;
        command.Parameters.Add(new NpgsqlParameter<Guid>(IdParam, id));
        command.Parameters.Add(new NpgsqlParameter<Guid>(RegionIdParam, regionId));
        command.Parameters.Add(new NpgsqlParameter<string>(NameParam, name));
        command.Parameters.AddWithValue(EmbeddingParam, new Vector(generator.Generate(text)));
        await command.ExecuteNonQueryAsync();
    }
}
