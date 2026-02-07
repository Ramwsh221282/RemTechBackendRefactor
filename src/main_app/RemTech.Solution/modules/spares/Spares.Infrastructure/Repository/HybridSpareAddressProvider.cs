using System.Data;
using System.Data.Common;
using Dapper;
using Npgsql;
using Pgvector;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;
using Spares.Domain.Contracts;
using Spares.Domain.Models;

namespace Spares.Infrastructure.Repository;

/// <summary>
/// Гибридный провайдер поиска региона по векторному представлению адреса и триграммам.
/// </summary>
/// <param name="session">Сессия базы данных PostgreSQL.</param>
/// <param name="embeddings">Провайдер векторных представлений.</param>
public sealed class HybridSpareAddressProvider(NpgSqlSession session, EmbeddingsProvider embeddings)
	: ISpareAddressProvider
{
	private const double MAX_DISTANCE = 0.3;

	/// <summary>
	/// Ищет идентификатор региона по адресу.
	/// </summary>
	public async Task<Result<Guid>> SearchRegionId(string address, CancellationToken ct = default)
	{
		const string sql = """
			SELECT id as region_id FROM vehicles_module.regions
			WHERE embedding <=> @embedding <= @max_distance
			LIMIT 1;
			""";
        
		DynamicParameters parameters = new();
		Vector embedding = new(embeddings.Generate(address));
		parameters.Add("@embedding", embedding);
		parameters.Add("@max_distance", MAX_DISTANCE, DbType.Double);
		CommandDefinition command = new(sql, parameters, transaction: session.Transaction);
        
		NpgsqlConnection connection = await session.GetConnection(ct);
		Guid? id = await connection.QueryFirstOrDefaultAsync<Guid?>(command);
        
		return id is null ? Error.NotFound("Unable to resolve region") : Result.Success(id.Value);
	}

    /// <summary>
    /// Ищет наиболее подходящие адреса для запчастей.
    /// </summary>
    public async Task<IReadOnlyList<Spare>> SearchAddressesForEachSpare(IEnumerable<Spare> spares,
        CancellationToken ct = default)
    {             // spare id
        Dictionary<Guid, Spare> source = spares.ToDictionary(s => s.Id.Value, s => s);
        
        string[] spareAddressTexts = [..spares.Select(s => $"{s.Details.Address.Value}")];
        Vector[] spareAddressVectors = [..embeddings.GenerateBatch(spareAddressTexts).Select(v => new Vector(v))];
        Guid[] spareIdentifiers = [..source.Keys];

        const string sql = """
                           WITH spare_input_values AS (
                               SELECT
                                   UNNEST(@input_ids) as original_spare_id,
                                   UNNEST(@input_texts) as original_spare_address_text,
                                   UNNEST(@input_embeddings) as original_spare_address_vector
                           )
                           SELECT
                               spare_input_values.original_spare_id as spare_id,
                               relative_regions_vector_search.region_id as region_id,
                               relative_regions_vector_search.region_text as region_text
                           FROM
                               spare_input_values
                           INNER JOIN LATERAL
                               (
                                   WITH regions_vector_filter AS (
                                       SELECT
                                           r.id as region_id,
                                           (r.name || ' ' || r.kind) as region_text
                                       FROM vehicles_module.regions r
                                       WHERE 1 - (r.embedding <=> spare_input_values.original_spare_address_vector) >= 0.4
                                       ORDER BY r.embedding <=> spare_input_values.original_spare_address_vector
                                       LIMIT 5
                                   )
                                   SELECT * FROM regions_vector_filter
                                   WHERE word_similarity(regions_vector_filter.region_text, spare_input_values.original_spare_address_text) >= 0.6
                                   LIMIT 1
                               ) relative_regions_vector_search ON TRUE
                           """;

        DynamicParameters parameters = new();
        parameters.Add("@input_ids", spareIdentifiers);
        parameters.Add("@input_texts", spareAddressTexts);
        parameters.Add("@input_embeddings", spareAddressVectors);

        CommandDefinition command = session.FormCommand(sql, parameters, ct);
        NpgsqlConnection connection = await session.GetConnection(ct);
        await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
        
        while (await reader.ReadAsync(ct))
        {
            Guid spareId = reader.GetGuid(reader.GetOrdinal("spare_id"));
            Guid regionId = reader.GetGuid(reader.GetOrdinal("region_id"));
            string locationText = reader.GetString(reader.GetOrdinal("region_text"));
            
            if (source.TryGetValue(spareId, out Spare spare))
            {
                Spare withAddress = spare.WithAddress(regionId, locationText);
                source[spareId] = withAddress;
            }
        }
        
        IReadOnlyList<Spare> results = source.Values.ToList();
        return results;
    }
}
