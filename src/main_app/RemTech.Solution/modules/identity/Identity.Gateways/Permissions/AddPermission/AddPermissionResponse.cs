using Identity.Contracts.Permissions;
using Identity.Contracts.Permissions.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Permissions.AddPermission;

public sealed class AddPermissionResponse : IOnPermissionCreatedEventListener, IResponse
{
    public Guid Id { get; private set; } = Guid.Empty;
    
    public Task<Result<Unit>> React(PermissionData data, CancellationToken ct = default)
    {
        Id = data.Id;
        return Task.FromResult(Result.Success(Unit.Value));
    }
}