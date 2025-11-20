namespace Tickets.Persistence;

internal sealed record TableTicket(
    Guid Id,
    Guid CreatorId,
    string Type,
    DateTime Created,
    DateTime? Closed,
    string Status,
    string? ExtraInformation
);