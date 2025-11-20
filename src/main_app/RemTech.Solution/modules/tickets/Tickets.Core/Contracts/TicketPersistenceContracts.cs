namespace Tickets.Core.Contracts;

public sealed record QueryTicketArgs(
    Guid? TicketId = null, 
    Guid? CreatorId = null, 
    string? Type = null,
    bool WithLock = false);

public sealed record TicketsStorage(
    Insert Insert, 
    Delete Delete, 
    Update Update, 
    Find Find,
    HasAny HasAny,
    FindMany FindMany);

public delegate Task<Result<Unit>> Insert(Ticket ticket, CancellationToken ct);
public delegate Task<Result<Unit>> Delete(Ticket ticket, CancellationToken ct);
public delegate Task<Result<Unit>> Update(Ticket ticket, CancellationToken ct);
public delegate Task<Optional<Ticket>> Find(QueryTicketArgs args, CancellationToken ct);
public delegate Task<IEnumerable<Ticket>> FindMany(QueryTicketArgs args, CancellationToken ct);
public delegate Task<bool> HasAny(CancellationToken ct);