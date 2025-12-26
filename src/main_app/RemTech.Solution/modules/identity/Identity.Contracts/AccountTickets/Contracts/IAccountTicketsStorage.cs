using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace Identity.Contracts.AccountTickets.Contracts;

public interface IAccountTicketsStorage :
    IEntityPersister<IAccountTicket>,
    IEntityFetcher<IAccountTicket, AccountTicketQueryArgs>;