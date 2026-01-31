namespace Identity.Infrastructure.Permissions.GetPermissions;

public sealed class PermissionResponse
{
	public required Guid Id { get; set; }
	public required string Name { get; set; }
	public required string Description { get; set; }
}
