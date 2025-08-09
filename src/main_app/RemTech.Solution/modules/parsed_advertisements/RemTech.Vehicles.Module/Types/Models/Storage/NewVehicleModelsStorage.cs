using Npgsql;
using RemTech.Vehicles.Module.Database.Embeddings;

namespace RemTech.Vehicles.Module.Types.Models.Storage;

internal sealed class NewVehicleModelsStorage(
    NpgsqlDataSource dataSource,
    IEmbeddingGenerator generator
) : IVehicleModelsStorage
{
    public async Task<VehicleModel> Store(VehicleModel vehicleModel)
    {
        string sql = string.Intern(
            """
            INSERT INTO parsed_advertisements_module.vehicle_models(id, text, embedding)
            VALUES(@id, @text, @embedding)
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
        command.Parameters.AddWithValue("@embedding", generator.Generate(text));
        int affected = await command.ExecuteNonQueryAsync();
        return affected == 0
            ? throw new UnableToStoreVehicleModelException(
                "Не удается сохранить модель техники. Дубликат по имени",
                text
            )
            : vehicleModel;
    }
}
