using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Entities.Tickets;
using RemTech.Core.Shared.DomainEvents;

namespace Identity.Domain.Users.Events;

public sealed record UserConfirmedPasswordReset(Guid UserId, Guid TicketId, string OtherPassword)
    : IDomainEvent
{
    public UserConfirmedPasswordReset(User user, UserTicket ticket)
        : this(user.Id.Id, ticket.Id.Id, user.Profile.Password.Password) { }
}
