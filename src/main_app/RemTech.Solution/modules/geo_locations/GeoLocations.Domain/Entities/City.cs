using GeoLocations.Domain.ValueObjects;

namespace GeoLocations.Domain.Entities;

public class City
{
    public CityId Id { get; }
    public LocationRegionId RegionId { get; }
    public CityName Name { get; }

    public City(LocationRegionId regionId, CityName name, CityId? id = null)
    {
        Id = id ?? new CityId();
        RegionId = regionId;
        Name = name;
    }
}
