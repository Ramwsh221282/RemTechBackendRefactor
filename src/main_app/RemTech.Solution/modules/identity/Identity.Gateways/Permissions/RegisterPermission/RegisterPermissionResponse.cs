using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Permissions.RegisterPermission;

public sealed record RegisterPermissionResponse(Guid Id, string Name) : IResponse;