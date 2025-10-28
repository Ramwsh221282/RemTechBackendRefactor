using RemTech.Core.Shared.DomainEvents;

namespace GeoLocations.Domain.Entities;

public sealed record CityCreatedEvent(Guid RegionId, string Name, Guid Id) : IDomainEvent
{
    public CityCreatedEvent(City city)
        : this(city.RegionId.Value, city.Name.Value, city.Id.Value) { }
}
