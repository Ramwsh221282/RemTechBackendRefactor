namespace Tickets.Core;

public sealed record TicketSnapshot(TicketLifeCycleSnapshot LifeCycle, TicketMetadataSnapshot Metadata);