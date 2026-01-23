using Identity.Domain.Permissions;

namespace WebHostApplication.Modules.identity.Responses;

public sealed record AccountPermissionsResponse(Guid Id, string Name, string Description)
{
	public static AccountPermissionsResponse ConvertFrom(Permission permission) =>
		new(permission.Id.Value, permission.Name.Value, permission.Description.Value);
}
