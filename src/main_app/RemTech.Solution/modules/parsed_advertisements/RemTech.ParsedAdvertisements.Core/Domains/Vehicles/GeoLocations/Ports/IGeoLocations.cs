using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Ports;

public interface IGeoLocations
{
    Status<GeoLocationEnvelope> Add(string? text, string? kind);
    MaybeBag<GeoLocationEnvelope> GetByText(string? text);
}
