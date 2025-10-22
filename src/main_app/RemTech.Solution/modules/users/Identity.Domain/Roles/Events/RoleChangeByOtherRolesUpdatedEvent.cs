using RemTech.Core.Shared.DomainEvents;

namespace Identity.Domain.Roles.Events;

public sealed record RoleChangeByOtherRolesUpdatedEvent(
    RoleEventArgs target,
    IEnumerable<RoleEventArgs> Changers
) : IDomainEvent;
