using Identity.Domain.Users.Entities;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.DomainEvents;

namespace Identity.Domain.Users.Events;

public sealed record IdentityUserEmailChangedEvent(Guid UserId, string UserEmail) : IDomainEvent
{
    public IdentityUserEmailChangedEvent(UserId id, IdentityUserProfile profile)
        : this(id.Id, profile.Email.Email) { }
}
