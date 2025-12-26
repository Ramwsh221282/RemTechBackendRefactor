using GeoLocations.Domain.ValueObjects;

namespace GeoLocations.Domain.Aggregate;

public class LocationRegion
{
    public LocationRegionId Id { get; }
    public LocationRegionName Name { get; }
    public LocationRegionKind Kind { get; }

    public LocationRegion(
        LocationRegionName name,
        LocationRegionKind kind,
        LocationRegionId? id = null
    ) => (Name, Kind, Id) = (name, kind, id ?? new LocationRegionId());

    public LocationRegion(LocationRegion region)
        : this(region.Name, region.Kind, region.Id) { }
}
