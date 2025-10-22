using Identity.Domain.Roles.Events;
using RemTech.Core.Shared.DomainEvents;

namespace Identity.Domain.Users.Events;

public sealed record IdentityUserCreatedEvent(
    Guid UserId,
    IdentityUserProfileEventArgs Profile,
    IEnumerable<RoleEventArgs> Roles
) : IDomainEvent;
