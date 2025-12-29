using System.Data;
using Dapper;
using Pgvector;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;
using Vehicles.Domain.Locations;
using Vehicles.Domain.Locations.Contracts;

namespace Vehicles.Infrastructure.Locations.LocationsPersisterImplementation;

public sealed class NpgSqlLocationsPersister(NpgSqlSession session, EmbeddingsProvider embeddings) : ILocationsPersister
{
    public async Task<Result<Location>> Save(Location location, CancellationToken ct = default)
    {
        const string sql = """
                           WITH exact_match AS (
                            SELECT id, name FROM vehicles_module.locations WHERE name = @name
                            LIMIT 1
                           ),
                           embedding_match AS (
                            SELECT id, name, embedding <-> @input_embedding AS distance
                            FROM vehicles_module.locations
                            ORDER BY distance
                            LIMIT 1
                            WHERE distance < @max_distance
                           )
                           SELECT exact_match.id as exact_id, exact_match.name as exact_name 
                           FROM exact_match 
                           UNION ALL 
                           SELECT embedding_match.id as embedding_id, embedding_match.name as embedding_name 
                           FROM embedding_match
                           """;

        Vector vector = new(embeddings.Generate(location.Name.Value));
        DynamicParameters parameters = BuildParameters(location, vector);
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        NpgSqlSearchResult result = (await session.QuerySingleUsingReader(command, MapFromReader))!;
        if (HasFromExactSearch(result)) return MapToLocationFromExactSearch(result);
        if (HasFromEmbeddingSearch(result)) return MapToLocationFromEmbeddingSearch(result);
        await SaveAsNewLocation(location, vector, ct);
        return location;
    }

    private DynamicParameters BuildParameters(Location location, Vector vector)
    {
        DynamicParameters parameters = new();
        parameters.Add("@name", location.Name.Value, DbType.String);
        parameters.Add("@input_embedding", vector);
        parameters.Add("@max_distance", 0.3, DbType.Double);
        return parameters;
    }

    private async Task SaveAsNewLocation(Location location, Vector vector, CancellationToken ct)
    {
        const string sql =
            "INSERT INTO vehicles_module.locations (id, name, embedding) VALUES (@id, @name, @input_embedding)";
        DynamicParameters parameters = new();
        parameters.Add("@id", location.Id.Id, DbType.Guid);
        parameters.Add("@name", location.Name.Value, DbType.String);
        parameters.Add("@input_embedding", vector);
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        await session.Execute(command);
    }

    private static bool HasFromEmbeddingSearch(NpgSqlSearchResult result)
    {
        return result.EmbeddingId.HasValue && result.EmbeddingName is not null;
    }

    private static Location MapToLocationFromEmbeddingSearch(NpgSqlSearchResult result)
    {
        LocationId id = LocationId.Create(result.EmbeddingId!.Value);
        LocationName name = LocationName.Create(result.EmbeddingName!);
        return new Location(id, name);
    }

    private static bool HasFromExactSearch(NpgSqlSearchResult result)
    {
        return result.ExactId.HasValue && result.ExactName is not null;
    }
    
    private static Location MapToLocationFromExactSearch(NpgSqlSearchResult result)
    {
        LocationId id = LocationId.Create(result.ExactId!.Value);
        LocationName name = LocationName.Create(result.ExactName!);
        return new Location(id, name);
    }
    
    private static NpgSqlSearchResult MapFromReader(IDataReader reader)
    {
        Guid? exactId = reader.GetNullable<Guid>("exact_id");
        string? exactName = reader.GetNullableReferenceType<string>("exact_name");
        Guid? embeddingId = reader.GetNullable<Guid>("embedding_id");
        string? embeddingName = reader.GetNullableReferenceType<string>("embedding_name");
        return new NpgSqlSearchResult
        {
            ExactId = exactId,
            ExactName = exactName,
            EmbeddingId = embeddingId,
            EmbeddingName = embeddingName,
        };
    }
    
    private sealed class NpgSqlSearchResult
    {
        public required Guid? ExactId { get; init; }
        public required string? ExactName { get; init; }
        public required Guid? EmbeddingId { get; init; }
        public required string? EmbeddingName { get; init; }
    }
}
    