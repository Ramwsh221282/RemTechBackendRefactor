using System.Data;
using System.Text.Json;
using Dapper;
using Npgsql;
using Pgvector;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;
using Vehicles.Domain.Locations;
using Vehicles.Domain.Vehicles;
using Vehicles.Domain.Vehicles.Contracts;

namespace Vehicles.Infrastructure.Vehicles.PersisterImplementation;

/// <summary>
/// Реализация персистера транспортных средств для PostgreSQL.
/// </summary>
/// <param name="session">Сессия для работы с базой данных PostgreSQL.</param>
/// <param name="embeddings">Провайдер для генерации векторных представлений текста.</param>
public sealed class NpgSqlVehiclesPersister(NpgSqlSession session, EmbeddingsProvider embeddings) : IVehiclesPersister
{
	/// <summary>
	/// Сохраняет транспортное средство в базе данных.
	/// </summary>
	/// <param name="info">Информация о транспортном средстве для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат операции сохранения.</returns>
	public async Task<Result<Unit>> Persist(VehiclePersistInfo info, CancellationToken ct = default)
	{
		const string sql = """
			INSERT INTO vehicles_module.vehicles 
			    (id, brand_id, category_id, model_id, region_id, source, price, is_nds, text, photos, characteristics, embedding)
			VALUES 
			    (@id, @brand_id, @category_id, @model_id, @region_id, @source, @price, @is_nds, @text, @photos::jsonb, @characteristics::jsonb, @embedding)
			ON CONFLICT (id) DO NOTHING;
			""";
		NpgsqlConnection connection = await session.GetConnection(ct);
		DynamicParameters parameters = CreateParameters(info, embeddings);
		CommandDefinition command = new(sql, parameters, transaction: session.Transaction, cancellationToken: ct);
		int affected = await connection.ExecuteAsync(command);
		if (affected == 0)
		{
			return Error.Conflict("Unable to save vehicle.");
		}

		await PersistCharacteristics(session, connection, info.Vehicle, ct);
		return Unit.Value;
	}

	private static DynamicParameters CreateParameters(VehiclePersistInfo info, EmbeddingsProvider embeddings)
	{
		Vehicle vehicle = info.Vehicle;
		Location location = info.Location;
		string jsonPhotos = JsonSerializer.Serialize(vehicle.Photos.Photos.Select(p => p.Path));
		string jsonCharacteristics = JsonSerializer.Serialize(
			vehicle.Characteristics.Select(c => new { Name = c.Name.Value, c.Value.Value })
		);
		Vector vector = new(embeddings.Generate(CreateTextForEmbedding(location, vehicle)));
		DynamicParameters parameters = new();
		parameters.Add("@id", vehicle.Id.Value, DbType.Guid);
		parameters.Add("@brand_id", vehicle.BrandId.Id, DbType.Guid);
		parameters.Add("@category_id", vehicle.CategoryId.Id, DbType.Guid);
		parameters.Add("@model_id", vehicle.ModelId.Value, DbType.Guid);
		parameters.Add("@region_id", vehicle.LocationId.Id, DbType.Guid);
		parameters.Add("@source", vehicle.Source.Value, DbType.String);
		parameters.Add("@price", vehicle.Price.Value, DbType.Int64);
		parameters.Add("@is_nds", vehicle.Price.IsNds, DbType.Boolean);
		parameters.Add("@text", vehicle.Text.Value, DbType.String);
		parameters.Add("@photos", jsonPhotos, DbType.String);
		parameters.Add("@characteristics", jsonCharacteristics, DbType.String);
		parameters.Add("@embedding", vector);
		return parameters;
	}

	private static string CreateTextForEmbedding(Location location, Vehicle vehicle)
	{
		return $"{vehicle.Text.Value.Trim()} {vehicle.Characteristics.Select(c => $"{c.Name.Value.Trim()} {c.Value.Value.Trim()}").Aggregate((a, b) => $"{a} {b}").Trim()} {location.Name.Value.Trim()}";
	}

	private static Task<int> PersistCharacteristics(
		NpgSqlSession session,
		NpgsqlConnection connection,
		Vehicle vehicle,
		CancellationToken ct
	)
	{
		List<string> insertClauses = [];
		DynamicParameters parameters = new();
		int index = 0;
		foreach (VehicleCharacteristic characteristic in vehicle.Characteristics)
		{
			string vehicleIdParam = $"vehicle_id_{index}";
			string characteristicIdParam = $"characteristic_id_{index}";
			string valueParam = $"value_{index}";
			string nameParam = $"name_{index}";
			insertClauses.Add($"(@{vehicleIdParam}, @{characteristicIdParam}, @{valueParam}, @{nameParam})");

			parameters.Add(vehicleIdParam, vehicle.Id.Value, DbType.Guid);
			parameters.Add(characteristicIdParam, characteristic.CharacteristicId.Value, DbType.Guid);
			parameters.Add(valueParam, characteristic.Value.Value, DbType.String);
			parameters.Add(nameParam, characteristic.Name.Value, DbType.String);
			index++;
		}

		string insertClause =
			$"INSERT INTO vehicles_module.vehicle_characteristics (vehicle_id, characteristic_id, value, name) VALUES {string.Join(",", insertClauses)} ON CONFLICT DO NOTHING;";
		CommandDefinition command = new(
			insertClause,
			parameters,
			transaction: session.Transaction,
			cancellationToken: ct
		);
		return connection.ExecuteAsync(command);
	}
}
