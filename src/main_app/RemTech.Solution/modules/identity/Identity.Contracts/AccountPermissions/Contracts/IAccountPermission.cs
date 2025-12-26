using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Contracts.AccountPermissions.Contracts;

public interface IAccountPermission
{
    Task<Result<Unit>> Save(CancellationToken ct = default);
    Task<Result<Unit>> Delete(CancellationToken ct = default);
    public Result<Unit> Write(
        Action<Guid>? writePermissionId = null,
        Action<Guid>? writeAccountId = null,
        Action<string>? writeAccountName = null,
        Action<string>? writeAccountEmail = null,
        Action<string>? writePermissionName = null);
}