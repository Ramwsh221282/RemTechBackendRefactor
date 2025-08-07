using System.Data.Common;
using Npgsql;
using RemTech.Vehicles.Module.Types.Brands.ValueObjects;

namespace RemTech.Vehicles.Module.Types.Brands.Storage;

internal sealed class PgTgrmVehicleBrandsStorage(NpgsqlDataSource dataSource)
    : IVehicleBrandsStorage
{
    public async Task<VehicleBrand> Store(VehicleBrand brand)
    {
        string sql = string.Intern(
            """
            SELECT id, text, similarity(@input, text) as sml
            FROM parsed_advertisements_module.vehicle_brands
            WHERE similarity(@input, text) > 0.2
            ORDER BY sml DESC
            LIMIT 1;
            """
        );
        string parameter = brand.Name();
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@input", parameter));
        await using DbDataReader reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            throw new UnableToStoreBrandException(
                "Не удается получить бренд по pg tgrm запросу.",
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
