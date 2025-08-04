using Npgsql;

namespace RemTech.Vehicles.Module.Types.Brands.Decorators.Postgres;

internal sealed class NewVehicleBrandsStorage(NpgsqlDataSource dataSource) : IVehicleBrandsStorage
{
    public async Task<VehicleBrand> Store(VehicleBrand brand)
    {
        string _sql = string.Intern(
            """
            INSERT INTO parsed_advertisements_module.vehicle_brands(id, text)
            VALUES(@id, @text)
            ON CONFLICT(text) DO NOTHING;
            """
        );
        Guid id = brand.Id();
        string text = brand.Name();
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = _sql;
        command.Parameters.Add(new NpgsqlParameter<Guid>("@id", id));
        command.Parameters.Add(new NpgsqlParameter<string>("@text", text));
        int affected = await command.ExecuteNonQueryAsync();
        if (affected == 0)
            throw new UnableToStoreBrandException(
                $"Не удается сохранить бренд {text}. Дубликат по имени."
            );
        return brand;
    }
}
