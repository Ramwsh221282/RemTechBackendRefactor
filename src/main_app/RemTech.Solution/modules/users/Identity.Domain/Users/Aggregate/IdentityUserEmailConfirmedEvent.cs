using Identity.Domain.Users.Entities;
using Identity.Domain.Users.Events;
using Identity.Domain.Users.ValueObjects;

namespace Identity.Domain.Users.Aggregate;

public sealed record IdentityUserEmailConfirmedEvent(Guid UserId, string Email, Guid TokenId)
    : IdentityUserEvent
{
    public IdentityUserEmailConfirmedEvent(
        UserId userId,
        IdentityUserProfile profile,
        IdentityUserToken confirmation
    )
        : this(userId.Id, profile.Email.Email, confirmation.Id.Id) { }
}