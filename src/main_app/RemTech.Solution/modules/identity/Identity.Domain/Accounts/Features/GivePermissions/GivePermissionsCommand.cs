using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.GivePermissions;

/// <summary>
/// Команда для выдачи разрешений пользователю.
/// </summary>
/// <param name="AccountId">Идентификатор аккаунта пользователя.</param>
/// <param name="Permissions">Список разрешений для выдачи.</param>
public sealed record GivePermissionsCommand(Guid AccountId, IEnumerable<GivePermissionsPermissionsPayload> Permissions)
	: ICommand;
