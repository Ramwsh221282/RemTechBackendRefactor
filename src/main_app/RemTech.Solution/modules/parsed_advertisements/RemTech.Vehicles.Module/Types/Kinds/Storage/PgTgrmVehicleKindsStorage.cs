using System.Data.Common;
using Npgsql;
using RemTech.Vehicles.Module.Types.Brands.Decorators.Postgres;
using RemTech.Vehicles.Module.Types.Kinds.ValueObjects;

namespace RemTech.Vehicles.Module.Types.Kinds.Storage;

internal sealed class PgTgrmVehicleKindsStorage(NpgsqlDataSource dataSource) : IVehicleKindsStorage
{
    public async Task<VehicleKind> Store(VehicleKind kind)
    {
        string sql = string.Intern(
            """
            SELECT id, text, similarity(@input, text) as sml FROM parsed_advertisements_module.vehicle_kinds
            WHERE similarity(@input, text) > 0.2
            ORDER BY sml DESC
            LIMIT 1;
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await using NpgsqlCommand command = connection.CreateCommand();
        string name = kind.Name();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@input", name));
        await using DbDataReader reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            throw new UnableToStoreBrandException(
                "Невозможно получить тип техники по tgrm запросу."
            );
        Guid id = reader.GetGuid(reader.GetOrdinal("id"));
        string text = reader.GetString(reader.GetOrdinal("text"));
        VehicleKindIdentity identity = new VehicleKindIdentity(
            new VehicleKindId(id),
            new VehicleKindText(text)
        );
        return kind.ChangeIdentity(identity);
    }
}
