namespace Tickets.Core.Snapshots;

public sealed record TicketSnapshot(TicketLifeCycleSnapshot LifeCycle, TicketMetadataSnapshot Metadata);