using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Contracts.AccountPermissions.Contracts;

public interface IOnAccountPermissionCreatedEventListener
{
    Task<Result<Unit>> React(AccountPermissionData data, CancellationToken ct = default);
}