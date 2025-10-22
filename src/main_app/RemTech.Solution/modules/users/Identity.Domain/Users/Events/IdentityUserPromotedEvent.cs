using RemTech.Core.Shared.DomainEvents;

namespace Identity.Domain.Users.Events;

public sealed record IdentityUserPromotedEvent(Guid UserId, Guid RoleId) : IDomainEvent;
