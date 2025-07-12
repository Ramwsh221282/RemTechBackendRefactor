using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.GeoLocations;

public interface IAsyncGeoLocations : IDisposable, IAsyncDisposable
{
    Task<Status<IGeoLocation>> Add(IGeoLocation location, CancellationToken ct = default);

    Task<MaybeBag<IGeoLocation>> Find(GeoLocationIdentity identity, CancellationToken ct = default);
}
