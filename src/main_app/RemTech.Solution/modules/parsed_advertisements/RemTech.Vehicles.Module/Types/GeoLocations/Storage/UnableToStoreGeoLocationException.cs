namespace RemTech.Vehicles.Module.Types.GeoLocations.Storage;

internal sealed class UnableToStoreGeoLocationException : Exception
{
    public UnableToStoreGeoLocationException(string message, string geo)
        : base($"{message} {geo}") { }

    public UnableToStoreGeoLocationException(string message, string geo, Exception ex)
        : base($"{message} {geo}", ex) { }
}
