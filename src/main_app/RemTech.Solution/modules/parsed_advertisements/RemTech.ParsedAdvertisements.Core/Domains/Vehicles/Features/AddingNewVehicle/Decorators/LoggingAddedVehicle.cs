using RemTech.Core.Shared.Functional;
using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.AddingNewVehicle.Decorators;

public sealed class LoggingAddedVehicle(ICustomLogger logger, IAddedVehicle origin) : IAddedVehicle
{
    public Status<VehicleEnvelope> Added(AddVehicle add)
    {
        Status<VehicleEnvelope> result = origin.Added(add);
        return result.IsSuccess
            ? new LoggingVehicle(logger, result.Value).Log()
            : new LoggingBadStatus<VehicleEnvelope>(logger, result).Logged();
    }
}
