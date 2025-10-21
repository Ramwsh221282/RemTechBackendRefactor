using Identity.Domain.Users.Entities;
using Identity.Domain.Users.ValueObjects;

namespace Identity.Domain.Users.Events;

public sealed record IdentityUserPasswordChangedEvent(Guid UserId, string UserPassword)
    : IdentityUserEvent
{
    public IdentityUserPasswordChangedEvent(UserId id, IdentityUserProfile profile)
        : this(id.Id, profile.Password.Password) { }
}
