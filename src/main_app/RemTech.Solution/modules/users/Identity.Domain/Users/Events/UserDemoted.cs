using RemTech.Core.Shared.DomainEvents;

namespace Identity.Domain.Users.Events;

public sealed record UserDemoted(Guid UserId, Guid RoleId) : IDomainEvent;
