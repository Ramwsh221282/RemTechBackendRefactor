using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Contracts.AccountPermissions.Contracts;

public interface IOnAccountPermissionRemovedEventListener
{
    Task<Result<Unit>> React(AccountPermissionData data, CancellationToken ct = default);
}