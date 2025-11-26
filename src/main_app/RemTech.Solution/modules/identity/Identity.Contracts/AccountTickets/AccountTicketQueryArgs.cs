using Identity.Contracts.Shared.Contracts;

namespace Identity.Contracts.AccountTickets;

public sealed record AccountTicketQueryArgs(
    Guid? Id = null, 
    Guid? AccountId = null, 
    string? Type = null,
    bool WithLock = false
) : EntityFetchArgs;