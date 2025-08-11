using Npgsql;
using Pgvector;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace GeoLocations.Module.Features.Persisting;

internal sealed record RegionToPersist
{
    private readonly Guid _id;
    private readonly string _name;
    private readonly string _type;
    private readonly IEnumerable<CityToPersist> _cities;

    public RegionToPersist WithCity(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
            return this;
        Guid id = Guid.NewGuid();
        IEnumerable<CityToPersist> cities = [.. _cities, new CityToPersist(id, city)];
        return new RegionToPersist(_id, _name, _type, cities);
    }

    public float[] MakeEmbedding(IEmbeddingGenerator generator)
    {
        return generator.Generate($"{_name} {_type}");
    }

    public async Task Persist(NpgsqlDataSource dataSource, IEmbeddingGenerator generator)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await using NpgsqlCommand command = connection.CreateCommand();
        string sql = string.Intern(
            "INSERT INTO locations_module.regions(id, name, kind, embedding) VALUES(@id, @name, @kind, @embedding)"
        );
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<Guid>("@id", _id));
        command.Parameters.Add(new NpgsqlParameter<string>("@name", _name));
        command.Parameters.Add(new NpgsqlParameter<string>("@kind", _type));
        command.Parameters.AddWithValue("@embedding", new Vector(MakeEmbedding(generator)));
        await command.ExecuteNonQueryAsync();
        foreach (CityToPersist city in _cities)
            await city.Persist(connection, generator, _name, _type, _id);
    }

    public RegionToPersist(Guid id, string name, string type)
    {
        _id = id;
        _name = name;
        _type = type;
        _cities = [];
    }

    public RegionToPersist(Guid id, string name, string type, IEnumerable<CityToPersist> cities)
        : this(id, name, type) => _cities = cities;
}
