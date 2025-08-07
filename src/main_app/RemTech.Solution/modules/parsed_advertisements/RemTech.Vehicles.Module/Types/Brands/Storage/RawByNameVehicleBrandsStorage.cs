using System.Data.Common;
using Npgsql;
using RemTech.Vehicles.Module.Types.Brands.ValueObjects;

namespace RemTech.Vehicles.Module.Types.Brands.Storage;

internal sealed class RawByNameVehicleBrandsStorage(NpgsqlDataSource dataSource)
    : IVehicleBrandsStorage
{
    public async Task<VehicleBrand> Store(VehicleBrand brand)
    {
        string sql = string.Intern(
            """
            SELECT id, text FROM parsed_advertisements_module.vehicle_brands
            WHERE text = @name;
            """
        );
        string parameter = brand.Name();
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@name", parameter));
        await using DbDataReader reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            throw new UnableToStoreBrandException(
                "Не удается получить бренд по соответствию с именем.",
                brand.Name()
            );
        Guid id = reader.GetGuid(reader.GetOrdinal("id"));
        string text = reader.GetString(reader.GetOrdinal("text"));
        VehicleBrandIdentity otherIdentity = new VehicleBrandIdentity(
            new VehicleBrandId(id),
            new VehicleBrandText(text)
        );
        return brand.ChangeIdentity(otherIdentity);
    }
}
