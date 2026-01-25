using Identity.Domain.Permissions;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Contracts.Persistence;

public interface IPermissionsRepository
{
    public Task<bool> Exists(PermissionSpecification specification, CancellationToken ct = default);
    public Task Add(Permission permission, CancellationToken ct = default);
    public Task Add(IEnumerable<Permission> permissions, CancellationToken ct = default);
    public Task<Result<Permission>> GetSingle(PermissionSpecification specification, CancellationToken ct = default);
    public Task<IEnumerable<Permission>> GetMany(
        IEnumerable<PermissionSpecification> specifications,
        CancellationToken ct = default
    );
}
