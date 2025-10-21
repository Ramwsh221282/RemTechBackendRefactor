using Identity.Domain.Users.Entities;
using Identity.Domain.Users.ValueObjects;

namespace Identity.Domain.Users.Events;

public sealed record IdentityUserEmailChangedEvent(Guid UserId, string UserEmail)
    : IdentityUserEvent
{
    public IdentityUserEmailChangedEvent(UserId id, IdentityUserProfile profile)
        : this(id.Id, profile.Email.Email) { }
}
