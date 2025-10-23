namespace Identity.Messaging.Port.EmailTickets;

public interface IEmailTicketsStorage
{
    Task<EmailTicketStoreResult> Store(
        EmailConfirmationTicket ticket,
        CancellationToken ct = default
    );
}