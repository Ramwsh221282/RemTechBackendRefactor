using Identity.Domain.Users.Entities;
using Identity.Domain.Users.ValueObjects;

namespace Identity.Domain.Users.Events;

public sealed record IdentityUserLoginChangedEvent(Guid UserId, string UserLogin)
    : IdentityUserEvent
{
    public IdentityUserLoginChangedEvent(UserId id, IdentityUserProfile profile)
        : this(id.Id, profile.Login.Name) { }
}
