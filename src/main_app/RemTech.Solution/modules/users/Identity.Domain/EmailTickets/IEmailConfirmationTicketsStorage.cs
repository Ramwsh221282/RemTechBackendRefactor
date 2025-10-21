using RemTech.Core.Shared.Result;

namespace Identity.Domain.EmailTickets;

public interface IEmailConfirmationTicketsStorage
{
    Task<Status> Add(EmailConfirmationTicket ticket, CancellationToken ct = default);
    Task<EmailConfirmationTicket?> Get(Guid id, CancellationToken ct = default);

    Task Drop(EmailConfirmationTicket ticket, CancellationToken ct = default);
}
