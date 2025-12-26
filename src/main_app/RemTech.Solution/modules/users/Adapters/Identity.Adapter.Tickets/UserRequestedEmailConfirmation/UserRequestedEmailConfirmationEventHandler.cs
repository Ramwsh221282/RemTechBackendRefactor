using Identity.Domain.Users.Events;
using Identity.Messaging.Port.EmailTickets;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;

namespace Identity.Adapter.Tickets.UserRequestedEmailConfirmation;

public sealed class UserRequestedEmailConfirmationEventHandler(
    Serilog.ILogger logger,
    IEmailTicketsStorage storage,
    IEmailTicketsSender sender
) : IDomainEventHandler<UserRequestedEmailConfirmationEvent>
{
    private const string Context = nameof(UserRequestedEmailConfirmationEvent);

    public async Task<Status> Handle(
        UserRequestedEmailConfirmationEvent @event,
        CancellationToken ct = default
    )
    {
        try
        {
            EmailConfirmationTicket ticket = EventToTicket(@event);
            Status storing = await Store(ticket, ct);
            if (storing.IsFailure)
                return storing;

            Status sending = await Send(ticket, ct);
            if (sending.IsFailure)
                return sending;

            return Status.Success();
        }
        catch (Exception ex)
        {
            logger.Information("{Context}. Error: {ErrorMessage}.", Context, ex.Message);
            return Status.Internal("Ошибка при отправке письма подтверждения электронной почты");
        }
    }

    private async Task<Status> Store(EmailConfirmationTicket ticket, CancellationToken ct = default)
    {
        EmailTicketStoreResult result = await storage.Store(ticket, ct);
        return result.IsSuccess == false ? Status.Conflict(result.Error) : Status.Success();
    }

    private async Task<Status> Send(EmailConfirmationTicket ticket, CancellationToken ct = default)
    {
        EmailTicketSendResult result = await sender.Send(ticket, ct);
        return result.IsSuccess == false ? Status.Conflict(result.Message) : Status.Success();
    }

    private EmailConfirmationTicket EventToTicket(UserRequestedEmailConfirmationEvent @event) =>
        new(
            @event.Ticket.Id,
            @event.Ticket.UserId,
            @event.Ticket.Created,
            @event.Ticket.Expires,
            @event.Ticket.Subject,
            @event.Ticket.Message,
            EmailConfirmationTicket.PendingStatus
        );
}
