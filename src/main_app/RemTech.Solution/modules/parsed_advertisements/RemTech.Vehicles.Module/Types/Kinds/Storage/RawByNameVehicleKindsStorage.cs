using System.Data.Common;
using Npgsql;
using RemTech.Vehicles.Module.Types.Kinds.ValueObjects;

namespace RemTech.Vehicles.Module.Types.Kinds.Storage;

internal sealed class RawByNameVehicleKindsStorage(NpgsqlDataSource dataSource)
    : IVehicleKindsStorage
{
    public async Task<VehicleKind> Store(VehicleKind kind)
    {
        string name = kind.Name();
        string sql = string.Intern(
            """
            SELECT id, text FROM parsed_advertisements_module.vehicle_kinds
            WHERE text = @input;
            """
        );
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@input", name));
        await using DbDataReader reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            throw new UnableToStoreVehicleKindException(
                $"Не удается найти тип техники с названием {name}"
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
