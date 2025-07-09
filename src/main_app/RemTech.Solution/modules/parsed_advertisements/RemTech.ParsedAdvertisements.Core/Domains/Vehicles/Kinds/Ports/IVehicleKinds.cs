namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Ports;

public interface IVehicleKinds
{
    Task<VehicleKind> Similar(VehicleKind kind, CancellationToken ct = default);
}
