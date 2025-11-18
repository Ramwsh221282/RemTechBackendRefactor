namespace Tickets.Core.Snapshots;

public sealed record TicketMetadataSnapshot(Guid CreatorId, Guid TicketId, string Type);