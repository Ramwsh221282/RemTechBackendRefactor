using Identity.Domain.Users.Entities.Tickets;
using RemTech.Core.Shared.DomainEvents;

namespace Identity.Domain.Users.Events;

public sealed record UserCreaterPasswordResetTicket(TicketEventArgs EventArgs) : IDomainEvent
{
    public UserCreaterPasswordResetTicket(UserTicket ticket)
        : this(new TicketEventArgs(ticket)) { }
}
