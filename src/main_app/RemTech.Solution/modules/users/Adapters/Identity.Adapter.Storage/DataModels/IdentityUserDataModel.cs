namespace Identity.Adapter.Storage.DataModels;

public sealed class IdentityUserDataModel
{
    public required Guid Id { get; set; }
    public required string Login { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
    public required bool EmailConfirmed { get; set; }
}
