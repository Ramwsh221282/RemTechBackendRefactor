using RemTech.Core.Shared.Functional;
using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Ports;

public sealed class LoggingVehicleBrands(ICustomLogger logger, IVehicleBrands origin)
    : IVehicleBrands
{
    public Status<VehicleBrandEnvelope> Add(string? name)
    {
        logger.Info("Добавление бренда.");
        Status<VehicleBrandEnvelope> adding = origin.Add(name);
        return adding.IsSuccess
            ? new LoggingVehicleBrand(logger, adding.Value).Log()
            : new LoggingBadStatus<VehicleBrandEnvelope>(logger, adding).Logged();
    }

    public MaybeBag<VehicleBrandEnvelope> GetByName(string? name) =>
        new LoggingMaybeBag<VehicleBrandEnvelope>(
            logger,
            origin.GetByName(name),
            "Получение бренда по названию: {0}."
        ).Logged();
}
