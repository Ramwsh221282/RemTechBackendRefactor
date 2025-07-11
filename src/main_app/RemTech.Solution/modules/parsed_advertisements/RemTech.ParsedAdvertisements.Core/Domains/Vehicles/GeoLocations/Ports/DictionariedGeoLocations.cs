using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Ports;

public sealed class DictionariedGeoLocations : IGeoLocations
{
    private readonly Dictionary<NotEmptyString, GeoLocationEnvelope> _items = [];

    public Status<GeoLocationEnvelope> Add(string? text)
    {
        NotEmptyString geoText = new(text);
        if (_items.TryGetValue(geoText, out GeoLocationEnvelope? geo))
            return geo;
        NewGeoLocation created = new(geoText);
        _items.Add(geoText, created);
        return created;
    }

    public MaybeBag<GeoLocationEnvelope> GetByText(string? text)
    {
        NotEmptyString geoText = new(text);
        return _items.TryGetValue(geoText, out GeoLocationEnvelope? geo)
            ? geo
            : new MaybeBag<GeoLocationEnvelope>();
    }
}
