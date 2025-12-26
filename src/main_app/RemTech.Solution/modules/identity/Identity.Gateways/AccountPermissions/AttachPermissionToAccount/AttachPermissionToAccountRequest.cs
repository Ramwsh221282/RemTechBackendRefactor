using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.AccountPermissions.AttachPermissionToAccount;

public sealed record AttachPermissionToAccountRequest(Guid AccountId, Guid PermissionId, CancellationToken Ct) : IRequest;