using System.Data;
using Dapper;
using Pgvector;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;
using Vehicles.Domain.Models;
using Vehicles.Domain.Models.Contracts;

namespace Vehicles.Infrastructure.Models.ModelPersisterImplementation;

/// <summary>
/// Реализация персистера моделей на основе NpgSql и векторных представлений.
/// </summary>
public sealed class NpgSqlModelPersister(NpgSqlSession session, EmbeddingsProvider embeddings) : IModelsPersister
{
	/// <summary>
	/// Сохраняет модель в базе данных.
	/// </summary>
	public async Task<Result<Model>> Save(Model model, CancellationToken ct = default)
    {
        const string sql = """
                            WITH vector_filtered AS (
                                SELECT 
                                    id as id, 
                                    name as name,
                                    1 - (embedding <=> @input_embedding) as score 
                                    FROM vehicles_module.models
                                WHERE 1 - (embedding <=> @input_embedding) >= 0.4
                                ORDER BY score DESC
                                LIMIT 15
                            ) 
                            SELECT
                                trgrm_filtered.id as Id,
                                trgrm_filtered.name as Name 
                            FROM vector_filtered
                            JOIN LATERAL (
                            SELECT 
                                vector_filtered.id as id,
                                vector_filtered.name as name,
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
                            ) trgrm_filtered ON TRUE
                            LIMIT 1
                            """;

		Vector vector = new(embeddings.Generate(model.Name.Value));
		DynamicParameters parameters = BuildParameters(model, vector);
		CommandDefinition command = session.FormCommand(sql, parameters, ct);
        NpgSqlSearchResult? result = await session.QueryMaybeRow<NpgSqlSearchResult?>(command);
		if (result is null)
		{
            return Error.NotFound("Unable to resolve model");
		}

		return new Model(ModelId.Create(result.Id), ModelName.Create(result.Name));
	}
    
	private static DynamicParameters BuildParameters(Model model, Vector vector)
	{
		DynamicParameters parameters = new();
		parameters.Add("@name", model.Name.Value, DbType.String);
		parameters.Add("@input_embedding", vector);
		return parameters;
	}

	private sealed class NpgSqlSearchResult
	{
		public required Guid Id { get; init; }
		public required string Name { get; init; }
	}
}
