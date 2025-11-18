namespace Tickets.Core.Snapshots;

public sealed record TicketLifeCycleSnapshot(Guid TicketId, DateTime Created, DateTime? Closed, string Status);