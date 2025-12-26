namespace Identity.Adapter.Storage.DataModels;

public sealed class IdentityUserTicketDataModel
{
    public required Guid UserId { get; init; }
    public required Guid Id { get; init; }
    public required string Type { get; init; }
    public required DateTime Created { get; init; }
    public required DateTime Expired { get; init; }
    public required DateTime? Deleted { get; init; }
}
