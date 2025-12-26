using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Contracts.Permissions.Contracts;

public interface IOnPermissionCreatedEventListener
{
    Task<Result<Unit>> React(PermissionData data, CancellationToken ct = default);
}