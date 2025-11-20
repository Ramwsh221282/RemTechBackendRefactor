namespace Tickets.Core.Contracts;

public sealed record RegisterTicketArgs(
    Guid CreatorId, 
    Guid TicketId, 
    string Type, 
    string? ExtraInformationJson, 
    CancellationToken Ct);
public delegate Task<Result<Ticket>> RegisterTicket(RegisterTicketArgs args);

public sealed record ActivateTicketArgs(Guid CreatorId, Optional<Ticket> Ticket, CancellationToken Ct);
public delegate Task<Result<Ticket>> ActivateTicket(ActivateTicketArgs args);

public sealed record MarkPendingArgs(Guid TicketId, Optional<Ticket> Target, CancellationToken Ct);
public delegate Task<Result<Ticket>> MarkPending(MarkPendingArgs args);