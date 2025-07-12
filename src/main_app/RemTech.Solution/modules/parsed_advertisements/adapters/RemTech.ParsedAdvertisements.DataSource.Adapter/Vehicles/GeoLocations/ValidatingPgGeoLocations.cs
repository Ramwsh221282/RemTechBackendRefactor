using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.GeoLocations;

public sealed class ValidatingPgGeoLocations(IAsyncGeoLocations locations) : IAsyncGeoLocations
{
    public Task<Status<IGeoLocation>> Add(IGeoLocation location, CancellationToken ct = default)
    {
        if (!location.Identify().ReadId())
            return new ValidationError<IGeoLocation>("Некорректный ID объявления");
        return !location.Identify().ReadText()
            ? new ValidationError<IGeoLocation>("Некорректный текст объявления")
            : locations.Add(location, ct);
    }

    public Task<MaybeBag<IGeoLocation>> Find(
        GeoLocationIdentity identity,
        CancellationToken ct = default
    ) =>
        !identity.ReadText() && !identity.ReadId()
            ? Task.FromResult(new MaybeBag<IGeoLocation>())
            : locations.Find(identity, ct);

    public void Dispose() => locations.Dispose();

    public ValueTask DisposeAsync() => locations.DisposeAsync();
}
