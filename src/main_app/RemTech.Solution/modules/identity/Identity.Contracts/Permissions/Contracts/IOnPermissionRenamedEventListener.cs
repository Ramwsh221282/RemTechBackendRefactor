using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Contracts.Permissions.Contracts;

public interface IOnPermissionRenamedEventListener
{
    Task<Result<Unit>> React(PermissionData data, CancellationToken ct = default);
}