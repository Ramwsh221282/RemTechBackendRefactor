using Identity.Contracts.AccountTickets;
using Identity.Contracts.AccountTickets.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Application.AccountTickets.Decorators;

public abstract class AccountTicketEnvelope(IAccountTicket ticket) : IAccountTicket
{
    public virtual Task<Result<Unit>> Register(IAccountTicketsStorage storage, CancellationToken ct) =>
        ticket.Register(storage, ct);

    public AccountTicketData Representation() =>
        ticket.Representation();
}