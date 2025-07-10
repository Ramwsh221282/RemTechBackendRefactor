using RemTech.Core.Shared.Primitives;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Ports;

public sealed class ValidatingGeoLocations(IGeoLocations origin) : IGeoLocations
{
    public Status<GeoLocationEnvelope> Add(string? text)
    {
        NotEmptyString geoText = new(text);
        return !geoText
            ? new ValidationError<GeoLocationEnvelope>("Некорректное название геолокации.")
            : origin.Add(geoText);
    }

    public MaybeBag<GeoLocationEnvelope> GetByText(string? text) =>
        !new NotEmptyString(text) ? new MaybeBag<GeoLocationEnvelope>() : origin.GetByText(text);
}