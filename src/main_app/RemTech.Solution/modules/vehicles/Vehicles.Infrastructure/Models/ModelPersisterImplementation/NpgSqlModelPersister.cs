using System.Data;
using Dapper;
using Pgvector;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;
using Vehicles.Domain.Models;
using Vehicles.Domain.Models.Contracts;

namespace Vehicles.Infrastructure.Models.ModelPersisterImplementation;

public sealed class NpgSqlModelPersister(NpgSqlSession session, EmbeddingsProvider embeddings) : IModelsPersister
{
    public async Task<Result<Model>> Save(Model model, CancellationToken ct = default)
    {
        const string sql = """
                           WITH exact_match AS (
                            SELECT id, name FROM vehicles_module.models WHERE name = @name
                            LIMIT 1
                           ),
                           embedding_match AS (
                            SELECT id, name, embedding <-> @input_embedding AS distance
                            FROM vehicles_module.models
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

        Vector vector = new(embeddings.Generate(model.Name.Value));
        DynamicParameters parameters = BuildParameters(model, vector);
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        NpgSqlSearchResult result = (await session.QuerySingleUsingReader(command, MapFromReader))!;
        if (HasFromExactSearch(result)) return MapToModelFromExactSearch(result);
        if (HasFromEmbeddingSearch(result)) return MapToModelFromEmbeddingSearch(result);
        await SaveAsNewModel(model, vector, ct);
        return model;
    }
    

    private async Task SaveAsNewModel(Model model, Vector vector, CancellationToken ct)
    {
        const string sql =
            "INSERT INTO vehicles_module.models (id, name, embedding) VALUES (@id, @name, @input_embedding)";
        DynamicParameters parameters = new();
        parameters.Add("@id", model.Id.Value, DbType.Guid);
        parameters.Add("@name", model.Name.Value, DbType.String);
        parameters.Add("@input_embedding", vector);
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        await session.Execute(command);
    }

    private DynamicParameters BuildParameters(Model model, Vector vector)
    {
        DynamicParameters parameters = new();
        parameters.Add("@name", model.Name.Value, DbType.String);
        parameters.Add("@input_embedding", vector);
        parameters.Add("@max_distance", 0.3, DbType.Double);
        return parameters;
    }

    private static bool HasFromEmbeddingSearch(NpgSqlSearchResult result)
    {
        return result.EmbeddingId.HasValue && result.EmbeddingName is not null;
    }

    private static Model MapToModelFromEmbeddingSearch(NpgSqlSearchResult result)
    {
        ModelId id = ModelId.Create(result.EmbeddingId!.Value);
        ModelName name = ModelName.Create(result.EmbeddingName!);
        return new Model(id, name);
    }

    private static bool HasFromExactSearch(NpgSqlSearchResult result)
    {
        return result.ExactId.HasValue && result.ExactName is not null;
    }

    private static Model MapToModelFromExactSearch(NpgSqlSearchResult result)
    {
        ModelId id = ModelId.Create(result.ExactId!.Value);
        ModelName name = ModelName.Create(result.ExactName!);
        return new Model(id, name);
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