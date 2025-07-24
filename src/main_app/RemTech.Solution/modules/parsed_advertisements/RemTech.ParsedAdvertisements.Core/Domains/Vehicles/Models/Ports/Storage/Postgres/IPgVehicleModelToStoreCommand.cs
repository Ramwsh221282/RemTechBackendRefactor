using Npgsql;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Ports.Storage.Postgres;

public interface IPgVehicleModelToStoreCommand
{
    Task<int> Execute(NpgsqlConnection connection, CancellationToken ct = default);
}