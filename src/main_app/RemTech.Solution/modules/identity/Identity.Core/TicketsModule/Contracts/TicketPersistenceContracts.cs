using RemTech.Functional.Extensions;

namespace Identity.Core.TicketsModule.Contracts;

public sealed record QueryTicketArgs(
    Guid? TicketId = null, 
    Guid? CreatorId = null, 
    string? Type = null,
    bool WithLock = false);

public delegate Task<Result<Unit>> InsertTicket(Ticket ticket, CancellationToken ct);
public delegate Task<Result<Unit>> DeleteTicket(Ticket ticket, CancellationToken ct);
public delegate Task<Result<Unit>> UpdateTicket(Ticket ticket, CancellationToken ct);
public delegate Task<Optional<Ticket>> GetTicket(QueryTicketArgs args, CancellationToken ct);