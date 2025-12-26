using RemTech.Core.Shared.DomainEvents;

namespace Identity.Domain.Users.Events;

public sealed record UserPromoted(Guid UserId, Guid RoleId) : IDomainEvent;
