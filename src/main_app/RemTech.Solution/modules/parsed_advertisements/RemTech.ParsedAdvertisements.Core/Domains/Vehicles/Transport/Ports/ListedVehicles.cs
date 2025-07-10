using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Ports;

public sealed class ListedVehicles : IVehicles
{
    private readonly List<VehicleEnvelope> _vehicles = [];

    public Status<VehicleEnvelope> Add(VehicleEnvelope vehicle)
    {
        _vehicles.Add(vehicle);
        return vehicle;
    }

    public MaybeBag<VehicleEnvelope> Get(Func<VehicleEnvelope, bool> predicate)
    {
        VehicleEnvelope? vehicle = _vehicles.FirstOrDefault(predicate);
        return vehicle ?? new MaybeBag<VehicleEnvelope>();
    }
}
