using Identity.Domain.Tickets;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Contracts.Persistence;

public interface IAccountTicketsRepository
{
	Task Add(AccountTicket ticket, CancellationToken ct = default);
	Task<Result<AccountTicket>> Find(AccountTicketSpecification specification, CancellationToken ct = default);
}
