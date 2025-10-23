using Identity.Domain.Users.Entities.Tickets;
using RemTech.Core.Shared.DomainEvents;

namespace Identity.Domain.Users.Events;

public sealed record UserCreatedTicket(
    Guid TicketId,
    Guid UserId,
    string Type,
    DateTime Created,
    DateTime Expired
) : IDomainEvent
{
    public UserCreatedTicket(UserTicket ticket)
        : this(
            ticket.Id.Id,
            ticket.IssuerId.Id,
            ticket.Type.Value,
            ticket.LifeTime.Created,
            ticket.LifeTime.Expires
        ) { }
}
