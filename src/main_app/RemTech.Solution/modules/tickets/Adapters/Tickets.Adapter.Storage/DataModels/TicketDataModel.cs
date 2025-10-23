namespace Tickets.Adapter.Storage.DataModels;

public sealed class TicketDataModel
{
    public required Guid Id { get; init; }
    public required DateTime Created { get; init; }
    public required DateTime? Deleted { get; init; }
    public required string Content { get; init; } // json
}
