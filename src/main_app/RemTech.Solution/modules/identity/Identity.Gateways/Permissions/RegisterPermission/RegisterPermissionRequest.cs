using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Permissions.RegisterPermission;

public sealed record RegisterPermissionRequest(string Name, CancellationToken Ct) : IRequest;