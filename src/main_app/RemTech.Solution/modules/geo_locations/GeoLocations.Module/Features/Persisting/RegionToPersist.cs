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

    private const string Sql =
        "INSERT INTO locations_module.regions(id, name, kind, embedding) VALUES(@id, @name, @kind, @embedding)";
    private const string IdParam = "@id";
    private const string NameParam = "@name";
    private const string KindParam = "@kind";
    private const string EmbeddingParam = "@embedding";
    private const string TextFormat = "{0} {1}";

    public RegionToPersist WithCity(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
            return this;
        Guid id = Guid.NewGuid();
        IEnumerable<CityToPersist> cities = [.. _cities, new(id, city)];
        return new RegionToPersist(_id, _name, _type, cities);
    }

    private ReadOnlyMemory<float> MakeEmbedding(IEmbeddingGenerator generator)
    {
        string text = string.Format(TextFormat, _name, _type);
        return generator.Generate(text);
    }

    public async Task Persist(NpgsqlDataSource dataSource, IEmbeddingGenerator generator)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = Sql;
        command.Parameters.Add(new NpgsqlParameter<Guid>(IdParam, _id));
        command.Parameters.Add(new NpgsqlParameter<string>(NameParam, _name));
        command.Parameters.Add(new NpgsqlParameter<string>(KindParam, _type));
        command.Parameters.AddWithValue(EmbeddingParam, new Vector(MakeEmbedding(generator)));
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
