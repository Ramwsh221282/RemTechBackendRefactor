using Npgsql;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Ports.Storage;

public interface IPgCharacteristicToStoreCommand
{
    Task<int> Execute(NpgsqlConnection connection, CancellationToken ct = default);
}