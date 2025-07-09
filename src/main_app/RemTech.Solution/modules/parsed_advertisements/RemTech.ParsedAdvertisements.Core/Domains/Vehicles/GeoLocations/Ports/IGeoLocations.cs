namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Ports;

public interface IGeoLocations
{
    Task<GeoLocation> Similar(GeoLocation geoLocation, CancellationToken ct = default);
}
