using Identity.Domain.Tickets;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Contracts;

public interface IAccountTicketsRepository
{
    Task Add(AccountTicket ticket, CancellationToken ct = default);
    Task<Result<AccountTicket>> Get(AccountTicketSpecification specification, CancellationToken ct = default);
}