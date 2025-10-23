namespace Identity.Messaging.Port.EmailTickets;

public sealed record EmailConfirmationTicket(
    Guid Id,
    Guid UserId,
    DateTime Created,
    DateTime Expires,
    string Subject,
    string Message
);
