using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Permissions.Features.AddPermissions;

/// <summary>
/// Команда добавления разрешений.
/// </summary>
/// <param name="Permissions">Коллекция полезных нагрузок команд добавления разрешений.</param>
public sealed record AddPermissionsCommand(IEnumerable<AddPermissionCommandPayload> Permissions) : ICommand;
