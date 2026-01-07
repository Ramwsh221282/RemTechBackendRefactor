namespace WebHostApplication.Modules.identity.Requests;

public sealed record GivePermissionsRequest(IEnumerable<Guid> PermissionIds);