using RemTech.Core.Shared.DomainEvents;

namespace ParsedAdvertisements.Domain.VehicleContext.Events;

public sealed record VehicleCreatedEvent(
    string VehicleId,
    string ItemType,
    string SourceDomain,
    string SourceUrl,
    Guid SourceId) : IDomainEvent;