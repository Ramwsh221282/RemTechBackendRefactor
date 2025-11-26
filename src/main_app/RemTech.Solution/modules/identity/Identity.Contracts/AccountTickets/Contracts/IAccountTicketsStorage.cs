using Identity.Contracts.Shared.Contracts;

namespace Identity.Contracts.AccountTickets.Contracts;

public interface IAccountTicketsStorage :
    IEntityPersister<IAccountTicket>,
    IEntityFetcher<IAccountTicket, AccountTicketQueryArgs>;