using System.Data;
using Dapper;
using Pgvector;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;
using Vehicles.Domain.Brands;
using Vehicles.Domain.Brands.Contracts;

namespace Vehicles.Infrastructure.Brands.BrandsPersisterImplementation;

/// <summary>
/// Реализация персистера брендов на основе NpgSql и векторных представлений.
/// </summary>
public sealed class NpgSqlBrandPersisterImplementation(EmbeddingsProvider embeddings, NpgSqlSession session)
	: IBrandPersister
{
	/// <summary>
	/// Сохраняет бренд в базе данных.
	/// </summary>
	public async Task<Result<Brand>> Save(Brand brand, CancellationToken ct = default)
    {
        const string sql = 
            """
            WITH vector_filtered AS (
                SELECT 
                    id as id, 
                    name as name,
                    1 - (b.embedding <=> @input_embedding) as score 
                FROM 
                    vehicles_module.brands b
                WHERE
                    1 - (b.embedding <=> @input_embedding) >= 0.5
                ORDER BY 
                    score DESC
                LIMIT 10                    
                )       
            SELECT
                trgrm_filtered.id as id,
                trgrm_filtered.name as name 
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
            ) AS trgrm_filtered ON TRUE
            LIMIT 1
            """;

		Vector vector = new(embeddings.Generate(brand.Name.Name));
		DynamicParameters parameters = BuildParameters(brand, vector);
		CommandDefinition command = session.FormCommand(sql, parameters, ct);
        NpgSqlSearchResult? existing = await session.QueryMaybeRow<NpgSqlSearchResult?>(command);
        if (existing is null)
        {
            return Error.NotFound("Unable to resolve brand.");
        }

        return new Brand(BrandId.Create(existing.Id), BrandName.Create(existing.Name));
    }

    private async Task SaveAsNew(Brand brand, Vector vector, CancellationToken ct)
    {
        const string sql = """
                           INSERT INTO vehicles_module.brands 
                               (id, name, embedding) 
                           VALUES 
                               (@id, @name, @input_embedding) 
                           """;
        DynamicParameters parameters = new();
        parameters.Add("@id", brand.Id.Id, DbType.Guid);
        parameters.Add("@name", brand.Name.Name, DbType.String);
        parameters.Add("@input_embedding", vector);
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        await session.Execute(command);
    }
    
	private static DynamicParameters BuildParameters(Brand brand, Vector vector)
	{
		DynamicParameters parameters = new();
		parameters.Add("@name", brand.Name.Name, DbType.String);
		parameters.Add("@input_embedding", vector);
		return parameters;
	}
    
	private sealed class NpgSqlSearchResult
	{
		public required Guid Id { get; init; }
		public required string Name { get; init; }
	}
}
