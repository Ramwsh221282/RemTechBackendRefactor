namespace GeoLocations.Module.Features.Querying;

public interface IGeoLocationQueryService
{
    Task<GeoLocationInfo> VectorSearch(string text, CancellationToken ct = default);
}
