using RemTech.Core.Shared.Functional;
using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Ports;

public sealed class LoggingGeoLocations(ICustomLogger logger, IGeoLocations origin) : IGeoLocations
{
    public Status<GeoLocationEnvelope> Add(string? text, string? kind)
    {
        logger.Info("Добавление геолокации.");
        Status<GeoLocationEnvelope> status = origin.Add(text, kind);
        return status.IsSuccess
            ? new LoggingGeoLocation(logger, status.Value).Log()
            : new LoggingBadStatus<GeoLocationEnvelope>(logger, status).Logged();
    }

    public MaybeBag<GeoLocationEnvelope> GetByText(string? text) =>
        new LoggingMaybeBag<GeoLocationEnvelope>(
            logger,
            origin.GetByText(text),
            $"Поиск геолокации по {text}."
        ).Logged();
}
