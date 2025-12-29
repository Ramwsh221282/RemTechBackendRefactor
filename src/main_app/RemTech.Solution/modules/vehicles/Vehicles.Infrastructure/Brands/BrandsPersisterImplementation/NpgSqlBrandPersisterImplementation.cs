using System.Data;
using Dapper;
using Pgvector;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;
using Vehicles.Domain.Brands;
using Vehicles.Domain.Brands.Contracts;

namespace Vehicles.Infrastructure.Brands.BrandsPersisterImplementation;

public sealed class NpgSqlBrandPersisterImplementation(EmbeddingsProvider embeddings, NpgSqlSession session) : IBrandPersister
{
    public async Task<Result<Brand>> Save(Brand brand, CancellationToken ct = default)
    {
        const string sql = """
                           WITH exact_match AS (
                            SELECT id, name FROM vehicles_module.brands WHERE name = @name
                            LIMIT 1
                           ),
                           embedding_match AS (
                            SELECT id, name, embedding <-> @input_embedding AS distance
                            FROM vehicles_module.brands
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
        
        Vector vector = new(embeddings.Generate(brand.Name.Name));
        DynamicParameters parameters = BuildParameters(brand, vector);
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        NpgSqlSearchResult result = (await session.QuerySingleUsingReader(command, MapFromReader))!;
        if (HasFromExactSearch(result)) return MapToBrandFromExactSearch(result);
        if (HasFromEmbeddingSearch(result)) return MapToBrandFromEmbeddingSearch(result);
        await SaveAsNewBrand(brand, vector, ct);
        return brand;
    }

    private async Task SaveAsNewBrand(Brand brand, Vector vector, CancellationToken ct)
    {
        const string sql = "INSERT INTO vehicles_module.brands (id, name, embedding) VALUES (@id, @name, @input_embedding)";
        DynamicParameters parameters = new();
        parameters.Add("@id", brand.Id.Id, DbType.Guid);
        parameters.Add("@name", brand.Name.Name, DbType.String);
        parameters.Add("@input_embedding", vector);
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        await session.Execute(command);
    }
    
    private DynamicParameters BuildParameters(Brand brand, Vector vector)
    {
        DynamicParameters parameters = new();
        parameters.Add("@name", brand.Name.Name, DbType.String);
        parameters.Add("@input_embedding", vector);
        parameters.Add("@max_distance", 0.3, DbType.Double);
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