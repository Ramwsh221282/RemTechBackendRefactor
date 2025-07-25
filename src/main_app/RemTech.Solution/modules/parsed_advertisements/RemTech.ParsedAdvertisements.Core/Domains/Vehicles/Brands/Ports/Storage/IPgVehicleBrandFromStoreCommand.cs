using Npgsql;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Ports.Storage;

public interface IPgVehicleBrandFromStoreCommand
{
    Task<VehicleBrand> Fetch(NpgsqlConnection connection, CancellationToken ct = default);
}