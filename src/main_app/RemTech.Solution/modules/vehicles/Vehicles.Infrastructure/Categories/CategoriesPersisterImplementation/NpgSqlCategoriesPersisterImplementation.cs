using System.Data;
using Dapper;
using Pgvector;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;
using Vehicles.Domain.Categories;
using Vehicles.Domain.Categories.Contracts;

namespace Vehicles.Infrastructure.Categories.CategoriesPersisterImplementation;

/// <summary>
/// Реализация персистера категорий на основе NpgSql и векторных представлений.
/// </summary>
public sealed class NpgSqlCategoriesPersisterImplementation(NpgSqlSession session, EmbeddingsProvider embeddings)
	: ICategoryPersister
{
	/// <summary>
	/// Сохраняет категорию в базе данных.
	/// </summary>
	public async Task<Result<Category>> Save(Category category, CancellationToken ct = default)
    {
        const string sql = """
                           WITH vector_filtered AS (
                                SELECT 
                                    c.id as id, 
                                    c.name as name,
                                    1 - (c.embedding <=> @input_embedding) as score
                                FROM vehicles_module.categories c
                                WHERE
                                1 - (c.embedding <=> @input_embedding) >= 0.4
                                ORDER BY score DESC
                                LIMIT 20
                           )
                           SELECT
                           trgrm_filtered.id as id,
                           trgrm_filtered.name as name
                           FROM vector_filtered
                           JOIN LATERAL
                           (
                                SELECT 
                                    vector_filtered.id as Id,
                                    vector_filtered.name as Name,
                                    word_similarity(vector_filtered.name, @name) as sml
                                FROM vector_filtered
                                WHERE
                                    vector_filtered.name = @name
                                    OR word_similarity(vector_filtered.name, @name) >= 0.4
                                ORDER BY
                                    CASE
                                        WHEN vector_filtered.name = @name THEN 0
                                        WHEN word_similarity(vector_filtered.name, @name) >= 0.4 THEN 1
                                        ELSE 2
                                    END,
                                    sml DESC
                                    LIMIT 1
                           ) AS trgrm_filtered ON TRUE
                           LIMIT 1
                           """;

		Vector vector = new(embeddings.Generate(category.Name.Value));
		DynamicParameters parameters = BuildParameters(category, vector);
		CommandDefinition command = session.FormCommand(sql, parameters, ct);
        NpgSqlSearchResult? existing = await session.QueryMaybeRow<NpgSqlSearchResult?>(command);
        if (existing is null)
        {
            return Error.NotFound("Unable to resolve category.");
        }

        return new Category(CategoryId.Create(existing.Id), CategoryName.Create(existing.Name));
    }

	private static DynamicParameters BuildParameters(Category category, Vector vector)
	{
		DynamicParameters parameters = new();
		parameters.Add("@name", category.Name.Value, DbType.String);
		parameters.Add("@input_embedding", vector);
		return parameters;
	}

    private async Task SaveAsNewCategory(Category category, Vector vector, CancellationToken ct)
    {
        const string sql = """
                           INSERT INTO vehicles_module.categories 
                               (id, name, embedding) 
                           VALUES (@id, @name, @input_embedding) 
                           """;
        DynamicParameters parameters = new();
        parameters.Add("@id", category.Id.Id, DbType.Guid);
        parameters.Add("@name", category.Name.Value, DbType.String);
        parameters.Add("@input_embedding", vector);
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        await session.Execute(command);
    }

	private sealed class NpgSqlSearchResult
	{
		public required Guid Id { get; init; }
		public required string Name { get; init; }
	}
}
