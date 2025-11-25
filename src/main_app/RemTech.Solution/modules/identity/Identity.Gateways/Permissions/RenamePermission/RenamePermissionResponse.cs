using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Permissions.RenamePermission;

public sealed record RenamePermissionResponse(Guid Id, string NewName) : IResponse;