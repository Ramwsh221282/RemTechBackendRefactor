using Identity.Domain.Roles.Events;
using RemTech.Core.Shared.DomainEvents;

namespace Identity.Domain.Users.Events;

public sealed record UserCreated(
    Guid UserId,
    UserProfileEventArgs Profile,
    IEnumerable<RoleEventArgs> Roles
) : IDomainEvent;
