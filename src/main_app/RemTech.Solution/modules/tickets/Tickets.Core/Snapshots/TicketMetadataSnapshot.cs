namespace Tickets.Core;

public sealed record TicketMetadataSnapshot(Guid CreatorId, Guid TicketId, string Type);