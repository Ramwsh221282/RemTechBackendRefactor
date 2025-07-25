using Npgsql;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Ports.Storage.Postgres;

public interface IPgVehicleKindFromStoreCommand
{
    Task<VehicleKind> Fetch(NpgsqlConnection connection, CancellationToken ct = default);
}