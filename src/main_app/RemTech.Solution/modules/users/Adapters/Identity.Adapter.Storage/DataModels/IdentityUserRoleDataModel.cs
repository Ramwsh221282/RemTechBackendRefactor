namespace Identity.Adapter.Storage.DataModels;

public sealed class IdentityUserRoleDataModel
{
    public required Guid UserId { get; set; }
    public required Guid RoleId { get; set; }
}
