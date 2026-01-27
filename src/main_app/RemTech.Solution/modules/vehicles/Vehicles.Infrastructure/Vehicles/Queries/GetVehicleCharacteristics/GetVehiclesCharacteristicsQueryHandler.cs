using System.Data;
using System.Data.Common;
using Dapper;
using Npgsql;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Vehicles.Infrastructure.Vehicles.Queries.GetVehicleCharacteristics;

/// <summary>
/// Обработчик запроса на получение характеристик транспортных средств по фильтрам.
/// </summary>
/// <param name="session">Сессия для работы с базой данных.</param>
public sealed class GetVehiclesCharacteristicsQueryHandler(NpgSqlSession session)
	: IQueryHandler<GetVehicleCharacteristicsQuery, GetVehicleCharacteristicsQueryResponse>
{
	/// <summary>
	/// Обрабатывает запрос на получение характеристик транспортных средств по фильтрам.
	/// </summary>
	/// <param name="query">Запрос с фильтрами для получения характеристик.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Ответ с характеристиками транспортных средств.</returns>
	public async Task<GetVehicleCharacteristicsQueryResponse> Handle(
		GetVehicleCharacteristicsQuery query,
		CancellationToken ct = default
	)
	{
		CommandDefinition command = FormSqlCommand(query, ct);
		await using NpgsqlConnection connection = await session.GetConnection(ct);
		await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
		return await FormResponseUsingReader(reader, ct);
	}

	private static async Task<GetVehicleCharacteristicsQueryResponse> FormResponseUsingReader(
		DbDataReader reader,
		CancellationToken ct
	)
	{
		GetVehicleCharacteristicsQueryResponse response = new();
		List<VehicleCharacteristicsResponse> characteristics = [];
		while (await reader.ReadAsync(ct))
		{
			characteristics.Add(
				new VehicleCharacteristicsResponse()
				{
					Id = reader.GetGuid(reader.GetOrdinal("id")),
					Name = reader.GetString(reader.GetOrdinal("name")),
					Values = (string[])reader.GetValue(reader.GetOrdinal("values")),
				}
			);
		}

		return response;
	}

	private static CommandDefinition FormSqlCommand(GetVehicleCharacteristicsQuery query, CancellationToken ct)
	{
		(DynamicParameters parameters, string filterSql) = FormFilters(query);
		string sql = $"""
			SELECT 
			    c.id as id,
			    c.name as name,
			    vals.values as values
			FROM vehicles_module.characteristics c
			LEFT JOIN LATERAL (
			    SELECT array_agg(DISTINCT vc.value) AS values
			FROM vehicles_module.vehicle_characteristics vc
			WHERE vc.characteristic_id = c.id
			    ) AS vals ON TRUE
			WHERE vals.values IS NOT NULL
			AND (SELECT EXISTS(
			    SELECT 1 FROM vehicles_module.vehicle_characteristics vc
			INNER JOIN vehicles_module.vehicles v ON vc.vehicle_id = v.id
			{filterSql}
			)) IS TRUE;
			""";
		return new(sql, parameters, cancellationToken: ct);
	}

	private static (DynamicParameters Parameters, string FilterSql) FormFilters(GetVehicleCharacteristicsQuery query)
	{
		DynamicParameters parameters = new();
		List<string> filters =
		[
			// this is a must, because we should get connected to vehicles list (only characteristics in such vehicles range)
			// and then filter by categories, models and brands (if provided).
			"vc.characteristic_id = c.id",
		];

		if (query.BrandId.HasValue)
		{
			filters.Add("v.brand_id=@brandId");
			parameters.Add("@brandId", query.BrandId.Value, DbType.Guid);
		}

		if (query.CategoryId.HasValue)
		{
			filters.Add("v.category_id=@categoryId");
			parameters.Add("@categoryId", query.CategoryId.Value, DbType.Guid);
		}

		if (query.ModelId.HasValue)
		{
			filters.Add("v.model_id=@modelId");
			parameters.Add("@modelId", query.ModelId.Value, DbType.Guid);
		}

		return (parameters, filters.Count == 0 ? string.Empty : $" WHERE {string.Join(" AND ", filters)}");
	}
}
