using Identity.Domain.Users.Entities;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.DomainEvents;

namespace Identity.Domain.Users.Events;

public sealed record IdentityUserLoginChangedEvent(Guid UserId, string UserLogin) : IDomainEvent
{
    public IdentityUserLoginChangedEvent(UserId id, IdentityUserProfile profile)
        : this(id.Id, profile.Login.Name) { }
}
