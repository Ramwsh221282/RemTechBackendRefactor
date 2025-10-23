using RemTech.Core.Shared.DomainEvents;

namespace Tickets.Domain.Tickets.Events;

public sealed record TicketCreatedEvent(Guid Id, DateTime Created, string Content) : IDomainEvent;
