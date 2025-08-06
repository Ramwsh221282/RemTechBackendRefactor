using System.Data.Common;
using Npgsql;
using RemTech.Vehicles.Module.Types.Models.ValueObjects;

namespace RemTech.Vehicles.Module.Types.Models.Storage;

internal sealed class RawByNameVehicleModelsStorage(NpgsqlDataSource dataSource)
    : IVehicleModelsStorage
{
    public async Task<VehicleModel> Store(VehicleModel vehicleModel)
    {
        string sql = string.Intern(
            """
            SELECT id, text FROM parsed_advertisements_module.vehicle_models
            WHERE text = @input;
            """
        );
        string name = vehicleModel.NameString();
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@input", name));
        await using DbDataReader reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            throw new UnableToStoreVehicleModelException(
                "Не удается получить модель техники по названию."
            );
        Guid id = reader.GetGuid(reader.GetOrdinal("id"));
        string text = reader.GetString(reader.GetOrdinal("text"));
        VehicleModelIdentity identity = new VehicleModelIdentity(id);
        VehicleModelName modelName = new VehicleModelName(text);
        return vehicleModel.ChangeIdentity(identity).Rename(modelName);
    }
}
