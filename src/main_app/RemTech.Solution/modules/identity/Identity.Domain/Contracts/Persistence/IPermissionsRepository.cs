using Identity.Domain.Permissions;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Contracts.Persistence;

public interface IPermissionsRepository
{
	Task<bool> Exists(PermissionSpecification specification, CancellationToken ct = default);
	Task Add(Permission permission, CancellationToken ct = default);
	Task Add(IEnumerable<Permission> permissions, CancellationToken ct = default);
	Task<Result<Permission>> GetSingle(PermissionSpecification specification, CancellationToken ct = default);
	Task<IEnumerable<Permission>> GetMany(
		IEnumerable<PermissionSpecification> specifications,
		CancellationToken ct = default
	);
}
