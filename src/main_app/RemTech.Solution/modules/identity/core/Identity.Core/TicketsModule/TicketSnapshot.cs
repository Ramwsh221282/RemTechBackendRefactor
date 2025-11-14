namespace Identity.Core.TicketsModule;

public sealed record TicketSnapshot(
    Guid Id,
    Guid CreatorId,
    string Type,
    DateTime Created,
    DateTime? Closed,
    bool Active);