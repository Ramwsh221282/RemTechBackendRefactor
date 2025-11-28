using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace Identity.Contracts.AccountTickets;

public sealed record AccountTicketQueryArgs(
    Guid? Id = null, 
    Guid? AccountId = null, 
    string? Type = null,
    bool WithLock = false
) : EntityFetchArgs;