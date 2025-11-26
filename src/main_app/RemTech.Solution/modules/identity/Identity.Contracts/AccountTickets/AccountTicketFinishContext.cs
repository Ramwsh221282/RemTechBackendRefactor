using Identity.Contracts.AccountTickets.Contracts;

namespace Identity.Contracts.AccountTickets;

public sealed record AccountTicketFinishContext(IAccountTicketsStorage Persister);