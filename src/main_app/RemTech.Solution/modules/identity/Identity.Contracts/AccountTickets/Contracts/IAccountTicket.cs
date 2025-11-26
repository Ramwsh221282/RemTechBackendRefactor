using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Contracts.AccountTickets.Contracts;

public interface IAccountTicket
{
    Task<Result<Unit>> Register(
        IAccountTicketsStorage storage, 
        CancellationToken ct
        );

    AccountTicketData Representation();
}