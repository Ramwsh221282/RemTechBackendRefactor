using Npgsql;

namespace RemTech.Vehicles.Module.Types.Characteristics.Ports.Storage;

internal interface IPgCharacteristicFromStoreCommand
{
    Task<Characteristic> Fetch(NpgsqlConnection connection, CancellationToken ct = default);
}
