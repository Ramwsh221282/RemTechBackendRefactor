namespace WebHostApplication.Modules.identity.Requests;

/// <summary>
/// Запрос на выдачу прав пользователю.
/// </summary>
/// <param name="PermissionIds">Идентификаторы прав, которые необходимо выдать пользователю.</param>
public sealed record GivePermissionsRequest(IEnumerable<Guid> PermissionIds);
