namespace Tickets.Core;

public sealed record TicketLifeCycleSnapshot(Guid TicketId, DateTime Created, DateTime? Closed, string Status);