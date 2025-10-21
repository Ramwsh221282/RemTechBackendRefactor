namespace Identity.Domain.Users.Events;

public sealed record IdentityUserCreatedEvent(
    Guid UserId,
    IdentityUserProfileEventArgs Profile,
    IEnumerable<IdentityUserRoleEventArgs> Roles
) : IdentityUserEvent;
