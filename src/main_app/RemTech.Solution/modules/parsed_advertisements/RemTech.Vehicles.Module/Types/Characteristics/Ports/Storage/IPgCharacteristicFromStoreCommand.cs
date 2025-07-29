using Npgsql;

namespace RemTech.Vehicles.Module.Types.Characteristics.Ports.Storage;

public interface IPgCharacteristicFromStoreCommand
{
    Task<Characteristic> Fetch(NpgsqlConnection connection, CancellationToken ct = default);
}
