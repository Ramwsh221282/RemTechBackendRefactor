namespace Identity.Domain.Accounts.Features.GivePermissions;

/// <summary>
/// Параметры разрешений для выдачи пользователю.
/// </summary>
/// <param name="Id">Идентификатор разрешения.</param>
public sealed record GivePermissionsPermissionsPayload(Guid Id);
