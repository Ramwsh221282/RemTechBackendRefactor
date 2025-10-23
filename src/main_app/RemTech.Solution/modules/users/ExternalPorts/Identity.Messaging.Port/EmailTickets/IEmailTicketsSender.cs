namespace Identity.Messaging.Port.EmailTickets;

public interface IEmailTicketsSender
{
    Task<EmailTicketSendResult> Send(
        EmailConfirmationTicket confirmationTicket,
        CancellationToken ct = default
    );
}
