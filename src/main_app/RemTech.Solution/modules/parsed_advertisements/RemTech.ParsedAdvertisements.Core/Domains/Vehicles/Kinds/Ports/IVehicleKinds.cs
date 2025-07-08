namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Ports;

public interface IVehicleKinds
{
    Task<ParsedVehicleKind> AddOrGet(ParsedVehicleKind kind, CancellationToken ct = default);
}
