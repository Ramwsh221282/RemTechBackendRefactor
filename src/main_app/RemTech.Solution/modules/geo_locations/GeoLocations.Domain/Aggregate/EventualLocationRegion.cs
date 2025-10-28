using GeoLocations.Domain.Events;
using GeoLocations.Domain.ValueObjects;
using RemTech.Core.Shared.DomainEvents;

namespace GeoLocations.Domain.Aggregate;

public sealed class EventualLocationRegion : LocationRegion
{
    private readonly LocationRegion _origin;
    private readonly List<IDomainEvent> _events = [];

    public EventualLocationRegion(LocationRegion origin)
        : base(origin) => _origin = origin;

    public static EventualLocationRegion Create(
        LocationRegionName name,
        LocationRegionKind kind,
        LocationRegionId? id = null
    )
    {
        var region = new LocationRegion(name, kind, id);
        var eventual = new EventualLocationRegion(region);
        if (id == null)
            eventual._events.Add(new LocationRegionCreatedEvent(eventual));
        return eventual;
    }
}
