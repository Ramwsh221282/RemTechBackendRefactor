using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Ports;

public interface IVehicleKinds
{
    Status<VehicleKindEnvelope> Add(string? name);
    MaybeBag<VehicleKindEnvelope> GetByName(string? name);
}
