using System.Data;
using Dapper;
using Npgsql;
using Pgvector;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;
using Vehicles.Domain.Brands;
using Vehicles.Domain.Brands.Contracts;

namespace Vehicles.Infrastructure.Brands.BrandsPersisterImplementation;

public sealed class NpgSqlBrandPersisterImplementation(EmbeddingsProvider embeddings, NpgSqlSession session) : IBrandPersister
{
    private const double MaxDistance = 0.6;
    
    public async Task<Result<Brand>> Save(Brand brand, CancellationToken ct = default)
    {
        const string sql = """
                           WITH exact_match AS (
                            SELECT id, name FROM vehicles_module.brands WHERE name = @name
                            LIMIT 1
                           ),
                           embedding_match AS (
                            SELECT id, name, embedding <=> @input_embedding AS distance
                            FROM vehicles_module.brands
                            WHERE embedding <=> @input_embedding < @max_distance
                            ORDER BY distance
                            LIMIT 1
                           )
                           SELECT
                               e.id as exact_id, e.name as exact_name,
                               m.id as embedding_id, m.name as embedding_name
                           FROM (SELECT 1) dummy
                           LEFT JOIN exact_match e ON true
                           LEFT JOIN embedding_match m ON true;
                           """;
        
        Vector vector = new(embeddings.Generate(brand.Name.Name));
        DynamicParameters parameters = BuildParameters(brand, vector);
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        NpgSqlSearchResult[] result = (await session.QueryMultipleUsingReader(command, MapFromReader));
        NpgSqlSearchResult? found = result.FirstOrDefault();
        if (found is null) return Error.Conflict("Unable to resolve brand.");
        if (HasFromExactSearch(found)) return MapToBrandFromExactSearch(found);
        if (HasFromEmbeddingSearch(found)) return MapToBrandFromEmbeddingSearch(found);
        return Error.Conflict("Unable to resolve brand.");
    }
    
    private DynamicParameters BuildParameters(Brand brand, Vector vector)
    {
        DynamicParameters parameters = new();
        parameters.Add("@name", brand.Name.Name, DbType.String);
        parameters.Add("@input_embedding", vector);
        parameters.Add("@max_distance", MaxDistance, DbType.Double);
        return parameters;
    }
    
    private static bool HasFromEmbeddingSearch(NpgSqlSearchResult result)
    {
        return result.EmbeddingId.HasValue && result.EmbeddingName is not null;
    }
    
    private static Brand MapToBrandFromEmbeddingSearch(NpgSqlSearchResult result)
    {
        BrandId id = BrandId.Create(result.EmbeddingId!.Value);
        BrandName name = BrandName.Create(result.EmbeddingName!);
        return new Brand(id, name);
    }
    
    private static bool HasFromExactSearch(NpgSqlSearchResult result)
    {
        return result.ExactId.HasValue && result.ExactName is not null;
    }

    private static Brand MapToBrandFromExactSearch(NpgSqlSearchResult result)
    {
        BrandId id = BrandId.Create(result.ExactId!.Value);
        BrandName name = BrandName.Create(result.ExactName!);
        return new Brand(id, name);
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