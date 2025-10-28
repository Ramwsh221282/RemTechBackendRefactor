using GeoLocations.Domain.ValueObjects;
using RemTech.Core.Shared.DomainEvents;

namespace GeoLocations.Domain.Entities;

public class EventualCity : City
{
    private readonly List<IDomainEvent> _events = [];
    private readonly City _origin;

    public EventualCity(City origin)
        : base(origin.RegionId, origin.Name, origin.Id) => _origin = origin;

    public static EventualCity Create(LocationRegionId regionId, CityName name, CityId? id = null)
    {
        var city = new City(regionId, name, id);
        var eventual = new EventualCity(city);
        if (id == null)
            eventual._events.Add(new CityCreatedEvent(eventual));

        return eventual;
    }
}
