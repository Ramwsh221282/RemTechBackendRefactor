using Identity.Domain.Users.Entities;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.DomainEvents;

namespace Identity.Domain.Users.Events;

public sealed record IdentityUserPasswordChangedEvent(Guid UserId, string UserPassword)
    : IDomainEvent
{
    public IdentityUserPasswordChangedEvent(UserId id, IdentityUserProfile profile)
        : this(id.Id, profile.Password.Password) { }
}
