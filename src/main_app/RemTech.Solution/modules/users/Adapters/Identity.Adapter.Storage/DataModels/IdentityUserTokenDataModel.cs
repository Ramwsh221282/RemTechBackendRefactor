namespace Identity.Adapter.Storage.DataModels;

public sealed class IdentityUserTokenDataModel
{
    public required Guid UserId { get; set; }
    public required Guid Id { get; set; }
    public required DateTime Created { get; set; }
    public required DateTime Deleted { get; set; }
    public required string Type { get; set; }
}
