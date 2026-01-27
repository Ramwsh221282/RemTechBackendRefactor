using System.Data;
using Dapper;
using Npgsql;
using Pgvector;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;

namespace Spares.Infrastructure.Repository;

/// <summary>
///     Провайдер поиска региона по векторному представлению адреса.
/// </summary>
/// <param name="session">Сессия базы данных PostgreSQL.</param>
/// <param name="embeddings">Провайдер векторных представлений.</param>
public sealed class EmbeddingSearchAddressProvider(NpgSqlSession session, EmbeddingsProvider embeddings)
	: ISpareAddressProvider
{
	private const double MAX_DISTANCE = 0.3;

	/// <summary>
	/// Ищет идентификатор региона по адресу.
	/// </summary>
	/// <param name="address">Адрес для поиска региона.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат с идентификатором региона или ошибкой.</returns>
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
		parameters.Add("@max_distance", MAX_DISTANCE, DbType.Double);
		CommandDefinition command = new(sql, parameters, transaction: session.Transaction);
		NpgsqlConnection connection = await session.GetConnection(ct);
		Guid? id = await connection.QueryFirstOrDefaultAsync<Guid?>(command);
		return id is null ? Error.NotFound("Unable to resolve region") : Result.Success(id.Value);
	}
}
