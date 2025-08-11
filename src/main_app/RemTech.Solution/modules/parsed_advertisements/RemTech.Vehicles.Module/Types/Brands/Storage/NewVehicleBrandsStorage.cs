using Npgsql;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace RemTech.Vehicles.Module.Types.Brands.Storage;

internal sealed class NewVehicleBrandsStorage(
    NpgsqlDataSource dataSource,
    IEmbeddingGenerator generator
) : IVehicleBrandsStorage
{
    public async Task<VehicleBrand> Store(VehicleBrand brand)
    {
        string sql = string.Intern(
            """
            INSERT INTO parsed_advertisements_module.vehicle_brands(id, text, embedding)
            VALUES(@id, @text, @embedding)
            ON CONFLICT(text) DO NOTHING;
            """
        );
        Guid id = brand.Id();
        string text = brand.Name();
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<Guid>("@id", id));
        command.Parameters.Add(new NpgsqlParameter<string>("@text", text));
        command.Parameters.AddWithValue("@embedding", generator.Generate(text));
        int affected = await command.ExecuteNonQueryAsync();
        if (affected == 0)
            throw new UnableToStoreBrandException(
                $"Не удается сохранить бренд. Дубликат по имени.",
                text
            );
        return brand;
    }
}
