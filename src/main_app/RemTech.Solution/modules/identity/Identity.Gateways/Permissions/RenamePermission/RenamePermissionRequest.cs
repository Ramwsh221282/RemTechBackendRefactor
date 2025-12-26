using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Permissions.RenamePermission;

public sealed record RenamePermissionRequest(Guid Id, string NewName, CancellationToken Ct) : IRequest;