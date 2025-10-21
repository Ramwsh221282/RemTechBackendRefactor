using Identity.Domain.Users.Events;

namespace Identity.Domain.Roles.Events;

public sealed record RoleCreatedEvent(Guid Id, string Name) : IdentityUserEvent;
