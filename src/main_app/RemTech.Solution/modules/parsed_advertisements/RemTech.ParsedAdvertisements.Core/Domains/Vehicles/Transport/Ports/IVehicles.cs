using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Ports;

public interface IVehicles
{
    Status<VehicleEnvelope> Add(VehicleEnvelope vehicle);
    MaybeBag<VehicleEnvelope> Get(Func<VehicleEnvelope, bool> predicate);
}
