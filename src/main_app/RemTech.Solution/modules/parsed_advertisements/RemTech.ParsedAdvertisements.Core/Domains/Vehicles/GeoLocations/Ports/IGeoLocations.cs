namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Ports;

public interface IGeoLocations
{
    Task<ParsedGeoLocation> AddOrGet(ParsedGeoLocation geoLocation, CancellationToken ct = default);
}
