using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Permissions.Features.AddPermissions;

public sealed record AddPermissionsCommand(IEnumerable<AddPermissionCommandPayload> Permissions) : ICommand;