namespace GeoLocations.Module.Features.Querying;

public sealed record PersistedCity(Guid Id, Guid RegionId, string Name);
