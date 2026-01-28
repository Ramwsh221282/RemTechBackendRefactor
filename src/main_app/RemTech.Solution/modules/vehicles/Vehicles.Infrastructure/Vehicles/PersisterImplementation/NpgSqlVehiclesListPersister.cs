using System.Text.Json;
using Dapper;
using Npgsql;
using RemTech.SharedKernel.Infrastructure.Database;
using Vehicles.Domain.Vehicles.Contracts;

namespace Vehicles.Infrastructure.Vehicles.PersisterImplementation;

/// <summary>
/// Реализация персистера списка транспортных средств для PostgreSQL.
/// </summary>
/// <param name="session">Сессия для работы с базой данных PostgreSQL.</param>
public sealed class NpgSqlVehiclesListPersister(NpgSqlSession session) : IVehiclesListPersister
{
	/// <summary>
	/// Сохраняет список транспортных средств в базе данных.
	/// </summary>
	/// <param name="infos">Список информации о транспортных средствах для сохранения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Количество сохраненных транспортных средств.</returns>
	public async Task<int> Persist(IEnumerable<VehiclePersistInfo> infos, CancellationToken ct = default)
	{
		IEnumerable<Guid> existing = await GetExistingIdentifiers(infos, ct);
		IEnumerable<VehiclePersistInfo> filtered = FilterFromExisting(infos, existing);
		int saved = await SaveVehicles(filtered, ct);
		if (saved == 0)
			return saved;
		await PersistCharacteristics(session, filtered, ct);
		return saved;
	}

	private static IEnumerable<VehiclePersistInfo> FilterFromExisting(
		IEnumerable<VehiclePersistInfo> infos,
		IEnumerable<Guid> existing
	) => infos.Where(i => !existing.Contains(i.Vehicle.Id.Value));

	private static async Task PersistCharacteristics(
		NpgSqlSession session,
		IEnumerable<VehiclePersistInfo> insertedVehicles,
		CancellationToken ct
	)
	{
		const string sql = """
			INSERT INTO vehicles_module.vehicle_characteristics (vehicle_id, characteristic_id, value, name)
			VALUES (@vehicle_id, @characteristic_id, @value, @name)
			""";

		var parameters = insertedVehicles
			.SelectMany(i => i.Vehicle.Characteristics)
			.Select(c => new
			{
				vehicle_id = c.VehicleId.Value,
				characteristic_id = c.CharacteristicId.Value,
				value = c.Value.Value,
				name = c.Name.Value,
			});

		NpgsqlConnection connection = await session.GetConnection(ct);
		await connection.ExecuteAsync(sql, parameters, transaction: session.Transaction);
	}

	private async Task<IEnumerable<Guid>> GetExistingIdentifiers(
		IEnumerable<VehiclePersistInfo> infos,
		CancellationToken ct
	)
	{
		const string sql = "SELECT id FROM vehicles_module.vehicles WHERE id = ANY(@ids)";
		Guid[] ids = infos.Select(i => i.Vehicle.Id.Value).ToArray();
		DynamicParameters parameters = new();
		parameters.Add("@ids", ids);
		CommandDefinition command = session.FormCommand(sql, parameters, ct);
		NpgsqlConnection connection = await session.GetConnection(ct);
		return await connection.QueryAsync<Guid>(command);
	}

	private async Task<int> SaveVehicles(IEnumerable<VehiclePersistInfo> infos, CancellationToken ct)
	{
		const string sql = """
			INSERT INTO vehicles_module.vehicles 
			    (id, brand_id, category_id, model_id, region_id, source, price, is_nds, text, photos, characteristics)
			VALUES 
			    (@id, @brand_id, @category_id, @model_id, @region_id, @source, @price, @is_nds, @text, @photos::jsonb, @characteristics::jsonb)
			""";

		var parameters = infos.Select(i => new
		{
			id = i.Vehicle.Id.Value,
			brand_id = i.Vehicle.BrandId.Id,
			category_id = i.Vehicle.CategoryId.Id,
			model_id = i.Vehicle.ModelId.Value,
			region_id = i.Location.Id.Id,
			source = i.Vehicle.Source.Value,
			price = i.Vehicle.Price.Value,
			is_nds = i.Vehicle.Price.IsNds,
			text = i.Vehicle.Text.Value,
			photos = JsonSerializer.Serialize(i.Vehicle.Photos.Photos.Select(p => p.Path)),
			characteristics = JsonSerializer.Serialize(
				i.Vehicle.Characteristics.Select(c => new { Name = c.Name.Value, c.Value.Value })
			),
		});

		NpgsqlConnection connection = await session.GetConnection(ct);
		return await connection.ExecuteAsync(sql, parameters, transaction: session.Transaction);
	}
}
