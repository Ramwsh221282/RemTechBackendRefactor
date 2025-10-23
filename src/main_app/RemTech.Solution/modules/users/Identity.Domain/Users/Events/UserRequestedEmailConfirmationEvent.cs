using Identity.Messaging.Port.EmailTickets;
using RemTech.Core.Shared.DomainEvents;

namespace Identity.Domain.Users.Events;

public record UserRequestedEmailConfirmationEvent(EmailConfirmationTicket Ticket) : IDomainEvent;
