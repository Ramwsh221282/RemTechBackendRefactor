using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.AccountPermissions.DetachPermissionFromAccount;

public record DetachPermissionFromAccountRequest(
    Guid AccountId,
    Guid PermissionId,
    CancellationToken Ct
    ) : IRequest;