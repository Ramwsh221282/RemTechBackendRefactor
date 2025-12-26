using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Permissions.AddPermission;

public sealed record AddPermissionRequest(string Name, CancellationToken Ct) : IRequest;