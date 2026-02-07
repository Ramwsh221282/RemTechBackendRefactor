using Dapper;
using Pgvector;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;
using Vehicles.Domain.Locations;
using Vehicles.Domain.Locations.Contracts;

namespace Vehicles.Infrastructure.Locations.LocationsPersisterImplementation;

/// <summary>
/// Реализация персистера для локаций на PostgreSQL.
/// </summary>
public sealed class NpgSqlLocationsPersister(NpgSqlSession session, EmbeddingsProvider embeddings) : ILocationsPersister
{
	/// <summary>
	/// Сохраняет локацию.
	/// </summary>
	public async Task<Result<Location>> Save(Location location, CancellationToken ct = default)
    {
        const string sql = """
                           WITH vector_filtered AS (
                            SELECT 
                                r.id as id, 
                                (r.name || ' ' || r.kind) as location_text
                            FROM vehicles_module.regions r
                            WHERE 1 - (embedding <=> @input_embedding) >= 0.4
                            ORDER BY (embedding <=> @input_embedding)
                            LIMIT 20
                           )
                           SELECT 
                           vector_filtered.id as Id, 
                           vector_filtered.location_text as LocationText
                           FROM vector_filtered
                           JOIN LATERAL (
                                SELECT 
                                    vector_filtered.id as Id,
                                    vector_filtered.location_text as LocationText,
                                    word_similarity(vector_filtered.location_text, @location_text) as sml
                                FROM vector_filtered
                                WHERE 
                                    vector_filtered.location_text = @location_text
                                    OR word_similarity(vector_filtered.location_text, @location_text) > 0.8
                                ORDER BY
                                    CASE
                                        WHEN vector_filtered.location_text = @location_text THEN 0
                                        WHEN word_similarity(vector_filtered.location_text, @location_text) > 0.8 THEN 1
                                        ELSE 2
                                    END,
                                    sml DESC
                                LIMIT 1
                           ) trgm_filtered ON true
                           """;
        
        Vector vector = new(embeddings.Generate(location.Name.Value));
        DynamicParameters parameters = new();
        parameters.Add("input_embedding", vector);
        parameters.Add("location_text", location.Name.Value);
        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        NpgSqlSearchResult? result = await session.QueryMaybeRow<NpgSqlSearchResult?>(command);
        if (result is null)
        {
            return Error.NotFound("Unable to resolve location");
        }
        
        return new Location(LocationId.Create(result.Id), LocationName.Create(result.LocationText));
    }

	private sealed class NpgSqlSearchResult
	{
		public required Guid Id { get; init; }
		public required string LocationText { get; init; }
	}
}
