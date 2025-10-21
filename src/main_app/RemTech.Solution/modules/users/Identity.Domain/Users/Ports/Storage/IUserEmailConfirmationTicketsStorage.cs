using Identity.Domain.EmailTickets;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.Ports.Storage;

public interface IUserEmailConfirmationTicketsStorage
{
    Task<Status> Add(EmailConfirmationTicket ticket, CancellationToken ct = default);
}
