using Identity.Domain.Users.Entities.Tickets;
using RemTech.Core.Shared.DomainEvents;

namespace Identity.Domain.Users.Events;

public sealed record TicketEventArgs(
    Guid TicketId,
    Guid UserId,
    string Type,
    DateTime Created,
    DateTime Expired
)
{
    public TicketEventArgs(UserTicket ticket)
        : this(
            ticket.Id.Id,
            ticket.IssuerId.Id,
            ticket.Type.Value,
            ticket.LifeTime.Created,
            ticket.LifeTime.Expires
        ) { }
}

public sealed record UserCreatedEmailConfirmTicket(TicketEventArgs EventArgs) : IDomainEvent
{
    public UserCreatedEmailConfirmTicket(UserTicket ticket)
        : this(new TicketEventArgs(ticket)) { }
}
