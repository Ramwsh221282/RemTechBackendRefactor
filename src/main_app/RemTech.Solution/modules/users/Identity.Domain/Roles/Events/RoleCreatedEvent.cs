using RemTech.Core.Shared.DomainEvents;

namespace Identity.Domain.Roles.Events;

public sealed record RoleCreatedEvent(Guid Id, string Name) : IDomainEvent;
