namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Ports.Storage.Postgres;

public interface IPgVehicleKindsStorage
{
    Task<VehicleKind> Read(VehicleKind kind, CancellationToken ct = default);
}