using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.Ports.Messaging;

public sealed record UserConfirmationEmailRequestedEvent(
    Guid UserId,
    Guid TicketId,
    DateTime Created,
    DateTime Expires,
    string Subject,
    string Body
);

public interface IUserConfirmationEmailRequestedEventSender
{
    Task<Status> Send(UserConfirmationEmailRequestedEvent @event, CancellationToken ct = default);
}
