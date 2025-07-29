using Npgsql;

namespace RemTech.ParsedAdvertisements.Core.Types.Characteristics.Ports.Storage;

public interface IPgCharacteristicFromStoreCommand
{
    Task<Characteristic> Fetch(NpgsqlConnection connection, CancellationToken ct = default);
}
