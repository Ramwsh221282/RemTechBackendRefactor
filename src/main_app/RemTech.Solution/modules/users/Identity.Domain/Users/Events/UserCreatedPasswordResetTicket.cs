using Identity.Domain.Users.Entities.Tickets;
using RemTech.Core.Shared.DomainEvents;

namespace Identity.Domain.Users.Events;

public sealed record UserCreatedPasswordResetTicket(TicketEventArgs EventArgs) : IDomainEvent
{
    public UserCreatedPasswordResetTicket(UserTicket ticket)
        : this(new TicketEventArgs(ticket)) { }
}
