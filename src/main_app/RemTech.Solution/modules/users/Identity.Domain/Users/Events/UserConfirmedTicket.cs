using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Entities.Tickets;
using RemTech.Core.Shared.DomainEvents;

namespace Identity.Domain.Users.Events;

public sealed record UserConfirmedTicket(Guid UserId, Guid TicketId) : IDomainEvent
{
    public UserConfirmedTicket(User user, UserTicket ticket)
        : this(user.Id.Id, ticket.Id.Id) { }
}
