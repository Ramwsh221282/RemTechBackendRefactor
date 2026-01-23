using System.Data;
using Dapper;
using Npgsql;
using Pgvector;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;

namespace Spares.Infrastructure.Repository;

public sealed class EmbeddingSearchAddressProvider(NpgSqlSession session, EmbeddingsProvider embeddings)
	: ISpareAddressProvider
{
	private const double MaxDistance = 0.3;

	public async Task<Result<Guid>> SearchRegionId(string address, CancellationToken ct = default)
	{
		const string sql = """
			SELECT id as region_id FROM vehicles_module.regions
			WHERE embedding <=> @embedding < @max_distance
			LIMIT 1;
			""";
		DynamicParameters parameters = new();
		Vector embedding = new(embeddings.Generate(address));
		parameters.Add("@embedding", embedding);
		parameters.Add("@max_distance", MaxDistance, DbType.Double);
		CommandDefinition command = new(sql, parameters, transaction: session.Transaction);
		NpgsqlConnection connection = await session.GetConnection(ct);
		Guid? id = await connection.QueryFirstOrDefaultAsync<Guid?>(command);
		return id is null ? Error.NotFound("Unable to resolve region") : Result.Success(id.Value);
	}
}
