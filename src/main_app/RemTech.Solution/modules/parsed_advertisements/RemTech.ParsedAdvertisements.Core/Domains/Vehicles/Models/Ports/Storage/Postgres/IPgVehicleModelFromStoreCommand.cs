using Npgsql;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Ports.Storage.Postgres;

public interface IPgVehicleModelFromStoreCommand
{
    Task<VehicleModel> Fetch(NpgsqlConnection connection, CancellationToken ct = default);
}