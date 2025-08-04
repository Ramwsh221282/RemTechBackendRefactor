using Npgsql;

namespace RemTech.Vehicles.Module.Types.Models.Storage;

internal sealed class NewVehicleModelsStorage(NpgsqlDataSource dataSource) : IVehicleModelsStorage
{
    public async Task<VehicleModel> Store(VehicleModel vehicleModel)
    {
        string sql = string.Intern(
            """
            INSERT INTO parsed_advertisements_module.vehicle_models(id, text)
            VALUES(@id, @text)
            ON CONFLICT(text) DO NOTHING;
            """
        );
        Guid id = vehicleModel.Id();
        string text = vehicleModel.NameString();
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<Guid>("@id", id));
        command.Parameters.Add(new NpgsqlParameter<string>("@text", text));
        int affected = await command.ExecuteNonQueryAsync();
        return affected == 0
            ? throw new UnableToStoreVehicleModelException(
                $"Не удается сохранить модель техники. Дубликат по имени {text}"
            )
            : vehicleModel;
    }
}
