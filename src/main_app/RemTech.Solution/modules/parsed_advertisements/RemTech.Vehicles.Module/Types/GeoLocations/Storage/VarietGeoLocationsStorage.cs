using RemTech.Vehicles.Module.Types.Brands.Storage;

namespace RemTech.Vehicles.Module.Types.GeoLocations.Storage;

internal sealed class VarietGeoLocationsStorage : IGeoLocationsStorage
{
    private readonly Queue<IGeoLocationsStorage> _storages = [];

    public VarietGeoLocationsStorage With(IGeoLocationsStorage storage)
    {
        _storages.Enqueue(storage);
        return this;
    }

    public async Task<GeoLocation> Save(GeoLocation geoLocation)
    {
        while (_storages.Count > 0)
        {
            IGeoLocationsStorage storage = _storages.Dequeue();
            try
            {
                return await storage.Save(geoLocation);
            }
            catch
            {
                continue;
            }
        }

        throw new UnableToStoreGeoLocationException(
            "Unable to save geo location",
            geoLocation.Name()
        );
    }
}
