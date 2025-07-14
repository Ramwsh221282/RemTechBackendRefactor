using RemTech.Core.Shared.Primitives;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Ports;

public sealed class ValidatingGeoLocations(IGeoLocations origin) : IGeoLocations
{
    public Status<GeoLocationEnvelope> Add(string? text, string? kind)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new ValidationError<GeoLocationEnvelope>("Название региона была пустым.");
        if (string.IsNullOrWhiteSpace(kind))
            return new ValidationError<GeoLocationEnvelope>("Название вида региона была пустым.");
        return origin.Add(text, kind);
    }

    public MaybeBag<GeoLocationEnvelope> GetByText(string? text) =>
        !new NotEmptyString(text) ? new MaybeBag<GeoLocationEnvelope>() : origin.GetByText(text);
}
