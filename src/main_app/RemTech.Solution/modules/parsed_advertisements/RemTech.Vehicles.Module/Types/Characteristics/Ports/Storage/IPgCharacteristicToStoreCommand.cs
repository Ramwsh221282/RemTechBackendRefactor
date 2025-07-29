using Npgsql;

namespace RemTech.Vehicles.Module.Types.Characteristics.Ports.Storage;

public interface IPgCharacteristicToStoreCommand
{
    Task<int> Execute(NpgsqlConnection connection, CancellationToken ct = default);
}
