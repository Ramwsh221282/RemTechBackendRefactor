namespace RemTech.Vehicles.Module.Types.GeoLocations.Storage;

internal interface IGeoLocationsStorage
{
    Task<GeoLocation> Save(GeoLocation geoLocation);
}
