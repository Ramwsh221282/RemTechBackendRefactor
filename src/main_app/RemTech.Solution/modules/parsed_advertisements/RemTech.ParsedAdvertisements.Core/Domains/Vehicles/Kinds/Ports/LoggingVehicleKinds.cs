using RemTech.Core.Shared.Functional;
using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Ports;

public sealed class LoggingVehicleKinds(ICustomLogger logger, IVehicleKinds origin) : IVehicleKinds
{
    public Status<VehicleKindEnvelope> Add(string? name)
    {
        logger.Info("Добавление типа техники.");
        Status<VehicleKindEnvelope> status = origin.Add(name);
        return status.IsSuccess
            ? new LoggingVehicleKind(logger, status.Value).Log()
            : new LoggingBadStatus<VehicleKindEnvelope>(logger, status).Logged();
    }

    public MaybeBag<VehicleKindEnvelope> GetByName(string? name) =>
        new LoggingMaybeBag<VehicleKindEnvelope>(
            logger,
            origin.GetByName(name),
            $"Получение типа техники по: {name}"
        ).Logged();
}
