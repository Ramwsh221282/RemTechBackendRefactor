using System.Data;
using Dapper;
using Pgvector;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;
using Vehicles.Domain.Categories;
using Vehicles.Domain.Categories.Contracts;

namespace Vehicles.Infrastructure.Categories.CategoriesPersisterImplementation;

public sealed class NpgSqlCategoriesPersisterImplementation(NpgSqlSession session, EmbeddingsProvider embeddings) : ICategoryPersister
{
    public async Task<Result<Category>> Save(Category category, CancellationToken ct = default)
    {
        const string sql = """
                           WITH exact_match AS (
                            SELECT id, name FROM vehicles_module.categories WHERE name = @name
                            LIMIT 1
                           ),
                           embedding_match AS (
                            SELECT id, name, embedding <-> @input_embedding AS distance
                            FROM vehicles_module.categories
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

        Vector vector = new(embeddings.Generate(category.Name.Value));
        DynamicParameters parameters = BuildParameters(category, vector);
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        NpgSqlSearchResult result = (await session.QuerySingleUsingReader(command, MapFromReader))!;
        if (HasFromExactSearch(result)) return MapToCategoryFromExactSearch(result);
        if (HasFromEmbeddingSearch(result)) return MapToCategoryFromEmbeddingSearch(result);
        await SaveAsNewCategory(category, vector, ct);
        return category;
    }

    private async Task SaveAsNewCategory(Category category, Vector vector, CancellationToken ct)
    {
        const string sql = "INSERT INTO vehicles_module.categories (id, name, embedding) VALUES (@id, @name, @input_embedding)";
        DynamicParameters parameters = new();
        parameters.Add("@id", category.Id.Id, DbType.Guid);
        parameters.Add("@name", category.Name.Value, DbType.String);
        parameters.Add("@input_embedding", vector);
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        await session.Execute(command);
    }

    private DynamicParameters BuildParameters(Category category, Vector vector)
    {
        DynamicParameters parameters = new();
        parameters.Add("@name", category.Name.Value, DbType.String);
        parameters.Add("@input_embedding", vector);
        parameters.Add("@max_distance", 0.3, DbType.Double);
        return parameters;
    }

    private static bool HasFromEmbeddingSearch(NpgSqlSearchResult result)
    {
        return result.EmbeddingId.HasValue && result.EmbeddingName is not null;
    }

    private static Category MapToCategoryFromEmbeddingSearch(NpgSqlSearchResult result)
    {
        CategoryId id = CategoryId.Create(result.EmbeddingId!.Value);
        CategoryName name = CategoryName.Create(result.EmbeddingName!);
        return new Category(id, name);
    }

    private static bool HasFromExactSearch(NpgSqlSearchResult result)
    {
        return result.ExactId.HasValue && result.ExactName is not null;
    }

    private static Category MapToCategoryFromExactSearch(NpgSqlSearchResult result)
    {
        CategoryId id = CategoryId.Create(result.ExactId!.Value);
        CategoryName name = CategoryName.Create(result.ExactName!);
        return new Category(id, name);
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
    