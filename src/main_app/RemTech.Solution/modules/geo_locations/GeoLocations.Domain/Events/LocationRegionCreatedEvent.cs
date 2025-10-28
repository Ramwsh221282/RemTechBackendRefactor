using GeoLocations.Domain.Aggregate;
using RemTech.Core.Shared.DomainEvents;

namespace GeoLocations.Domain.Events;

public sealed record LocationRegionCreatedEvent(Guid Id, string Region, string Kind) : IDomainEvent
{
    public LocationRegionCreatedEvent(LocationRegion region)
        : this(region.Id.Value, region.Name.Value, region.Kind.Value) { }
}
