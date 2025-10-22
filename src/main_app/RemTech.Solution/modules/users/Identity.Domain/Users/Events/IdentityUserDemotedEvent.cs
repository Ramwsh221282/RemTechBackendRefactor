using RemTech.Core.Shared.DomainEvents;

namespace Identity.Domain.Users.Events;

public sealed record IdentityUserDemotedEvent(Guid UserId, Guid RoleId) : IDomainEvent;