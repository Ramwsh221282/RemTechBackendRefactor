using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.AddingNewVehicle;

public interface IAddedVehicle
{
    Status<VehicleEnvelope> Added(AddVehicle add);
}