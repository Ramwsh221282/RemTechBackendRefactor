namespace Identity.Domain.Permissions.Features.AddPermissions;

/// <summary>
/// Полезная нагрузка команды добавления разрешения.
/// </summary>
/// <param name="Name">Имя разрешения.</param>
/// <param name="Description">Описание разрешения.</param>
public sealed record AddPermissionCommandPayload(string Name, string Description);
