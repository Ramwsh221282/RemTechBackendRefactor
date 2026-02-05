using System.Data;
using System.Text.Json;
using Dapper;
using Npgsql;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;
using Spares.Domain.Contracts;
using Spares.Domain.Models;

namespace Spares.Infrastructure.Repository;

/// <summary>
/// Репозиторий для работы с запчастями.
/// </summary>
/// <param name="session">Сессия базы данных PostgreSQL.</param>
/// <param name="addressProvider">Провайдер адресов для запчастей.</param>
public sealed class SparesRepository(NpgSqlSession session, ISpareAddressProvider addressProvider) : ISparesRepository
{
	/// <summary>
	/// Сессия базы данных PostgreSQL.
	/// </summary>
	/// <param name="spares">Коллекция запчастей для добавления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Количество добавленных запчастей.</returns>
	public async Task<int> AddMany(IEnumerable<Spare> spares, CancellationToken ct = default)
	{
		DynamicParameters parameters = new();
		List<string> insertClauses = [];
		IEnumerable<Spare> filtered = await FilterFromExisting(spares, ct);
		await FillParameters(parameters, insertClauses, filtered, ct);
		CommandDefinition command = CreateInsertCommand(parameters, insertClauses, ct);
		return await ExecuteCommand(command, ct);
	}

	private async Task<int> ExecuteCommand(CommandDefinition command, CancellationToken ct)
	{
		NpgsqlConnection connection = await session.GetConnection(ct);
		return await connection.ExecuteAsync(command);
	}

	private async Task<IEnumerable<Spare>> FilterFromExisting(IEnumerable<Spare> spares, CancellationToken ct)
	{
		const string sql = """
			SELECT id FROM spares_module.spares
			WHERE id = ANY(@ids) OR url = ANY(@urls)
			""";

		DynamicParameters parameters = new();
		Spare[] array = [.. spares];
		Guid[] ids = [.. array.Select(s => s.Id.Value)];
		string[] urls = [.. array.Select(s => s.Source.Url)];
		parameters.Add("@ids", ids);
		parameters.Add("@urls", urls);
		CommandDefinition command = new(sql, parameters, transaction: session.Transaction, cancellationToken: ct);
		NpgsqlConnection connection = await session.GetConnection(ct);
		IEnumerable<Guid> existing = await connection.QueryAsync<Guid>(command);
		return spares.Where(s => !existing.Contains(s.Id.Value));
	}

	private CommandDefinition CreateInsertCommand(
		DynamicParameters parameters,
		List<string> insertClauses,
		CancellationToken ct
	)
	{
		string insertClause = string.Join(", ", insertClauses);
		string insertSql =
			$"INSERT INTO spares_module.spares (url, id, price, is_nds, oem_id, text, region_id, type_id, photos, ts_vector_field) VALUES {insertClause}";
		return new(insertSql, parameters, transaction: session.Transaction, cancellationToken: ct);
	}

	private async Task FillParameters(
		DynamicParameters parameters,
		List<string> insertClauses,
		IEnumerable<Spare> spares,
		CancellationToken ct
	)
	{
		int index = 0;
		foreach (Spare spare in spares)
		{
			Result<Guid> regionId = await addressProvider.SearchRegionId(spare.Details.Address.Value, ct);
			if (regionId.IsFailure)
			{
				continue;
			}

			Guid id = spare.Id.Value;
			Guid oemId = spare.Oem.Id.Value;
			Guid typeId = spare.Type.Id.Value;
			string url = spare.Source.Url;
			long price = spare.Details.Price.Value;
			bool isNds = spare.Details.Price.IsNds;
			string text = spare.Details.Text.Value;
			string? photos =
				spare.Details.Photos.Value.Count == 0 ? null : JsonSerializer.Serialize(spare.Details.Photos.Value);

			string urlParam = $"@url_{index}";
			string idParam = $"@id_{index}";
			string priceParam = $"@price_{index}";
			string isNdsParam = $"@is_nds_{index}";
			string oemParam = $"@oem_{index}";
			string textParam = $"@text_{index}";
			string regionIdParam = $"@region_id_{index}";
			string typeParam = $"@type_{index}";
			string photosParam = $"@photos_{index}";

			parameters.Add(urlParam, url, DbType.String);
			parameters.Add(idParam, id, DbType.Guid);
			parameters.Add(priceParam, price, DbType.Int64);
			parameters.Add(isNdsParam, isNds, DbType.Boolean);
			parameters.Add(oemParam, oemId, DbType.Guid);
			parameters.Add(textParam, text, DbType.String);
			parameters.Add(regionIdParam, regionId.Value, DbType.Guid);
			parameters.Add(typeParam, typeId, DbType.Guid);
			parameters.Add(photosParam, photos, DbType.String);

			string insertClause =
				$"({urlParam}, {idParam}, {priceParam}, {isNdsParam}, {oemParam}, {textParam}, {regionIdParam}, {typeParam}, {photosParam}::jsonb, plainto_tsquery('russian', {CreateTextForTsVector(spare)}))";
			insertClauses.Add(insertClause);
			index++;
		}
	}

	private static string CreateTextForTsVector(Spare spare)
	{
		return $"{spare.Type.Value} {spare.Oem.Value} {spare.Details.Text.Value}";
	}
}
