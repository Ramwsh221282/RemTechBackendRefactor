using RemTech.Core.Shared.Functional;
using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Ports;

public sealed class LoggingVehicles(ICustomLogger logger, IVehicles origin) : IVehicles
{
    public Status<VehicleEnvelope> Add(VehicleEnvelope vehicle)
    {
        logger.Info("Добавление техники:");
        Status<VehicleEnvelope> adding = origin.Add(vehicle);
        return adding.IsSuccess
            ? new LoggingVehicle(logger, adding.Value).Log()
            : new LoggingBadStatus<VehicleEnvelope>(logger, adding).Logged();
    }

    public MaybeBag<VehicleEnvelope> Get(Func<VehicleEnvelope, bool> predicate) =>
        new LoggingMaybeBag<VehicleEnvelope>(
            logger,
            origin.Get(predicate),
            "Поиск техники."
        ).Logged();
}
