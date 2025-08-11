using Npgsql;
using Pgvector;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace GeoLocations.Module.Features.Persisting;

internal sealed class CityToPersist
{
    private readonly Guid _id;
    private readonly string _name;

    public CityToPersist(Guid id, string name)
    {
        _id = id;
        _name = name;
    }

    public async Task Persist(
        NpgsqlConnection connection,
        IEmbeddingGenerator generator,
        string regionName,
        string regionType,
        Guid regionId
    )
    {
        string text = $"{regionName} {regionType} {_name}";
        string sql = string.Intern(
            """
            INSERT INTO locations_module.cities(id, region_id, name, embedding)
            VALUES(@id, @region_id, @name, @embedding)
            """
        );
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<Guid>("@id", _id));
        command.Parameters.Add(new NpgsqlParameter<Guid>("@region_id", regionId));
        command.Parameters.Add(new NpgsqlParameter<string>("@name", _name));
        command.Parameters.AddWithValue("@embedding", new Vector(generator.Generate(text)));
        await command.ExecuteNonQueryAsync();
    }
}
