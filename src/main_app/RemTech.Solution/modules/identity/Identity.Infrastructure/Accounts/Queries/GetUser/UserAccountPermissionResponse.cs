using Identity.Domain.Permissions;

namespace Identity.Infrastructure.Accounts.Queries.GetUser;

public sealed record UserAccountPermissionResponse(Guid Id, string Name, string Description)
{
    public static UserAccountPermissionResponse Create(Permission permission) =>
        new(permission.Id.Value, permission.Name.Value, permission.Description.Value);
}
