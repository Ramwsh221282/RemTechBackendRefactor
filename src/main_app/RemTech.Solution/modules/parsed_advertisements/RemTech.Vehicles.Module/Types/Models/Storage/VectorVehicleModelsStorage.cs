using System.Data.Common;
using Npgsql;
using Pgvector;
using RemTech.Vehicles.Module.Types.Models.ValueObjects;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace RemTech.Vehicles.Module.Types.Models.Storage;

internal sealed class VectorVehicleModelsStorage(
    NpgsqlDataSource dataSource,
    IEmbeddingGenerator generator
) : IVehicleModelsStorage
{
    public async Task<VehicleModel> Store(VehicleModel vehicleModel)
    {
        string sql = string.Intern(
            """
            SELECT text, id FROM parsed_advertisements_module.vehicle_models
            ORDER BY embedding <=> @embedding LIMIT 1;
            """
        );
        string name = vehicleModel.NameString();
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue("@embedding", new Vector(generator.Generate(name)));
        await using DbDataReader reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            throw new UnableToStoreVehicleModelException(
                "Не удается получить модель техники по векторному поиску.",
                name
            );
        Guid id = reader.GetGuid(reader.GetOrdinal("id"));
        string text = reader.GetString(reader.GetOrdinal("text"));
        VehicleModelIdentity identity = new VehicleModelIdentity(id);
        VehicleModelName modelName = new VehicleModelName(text);
        return vehicleModel.ChangeIdentity(identity).Rename(modelName);
    }
}
