namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Ports;

public interface IVehicles
{
    Task<Vehicle> Add(Vehicle transport, CancellationToken ct = default);
}
