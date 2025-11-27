using Identity.Contracts.AccountPermissions;
using Identity.Contracts.AccountPermissions.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Infrastructure.AccountPermissions.EventListeners.OnRemoved;

public sealed class OnAccountPermissionRemovedPipeLine(
    IOnAccountPermissionRemovedEventListener current,
    OnAccountPermissionRemovedPipeLine? next = null
) : IOnAccountPermissionRemovedEventListener
{
    private readonly IOnAccountPermissionRemovedEventListener _current = current;
    private readonly OnAccountPermissionRemovedPipeLine? _next = next;
    
    public async Task<Result<Unit>> React(AccountPermissionData data, CancellationToken ct = default)
    {
        Result<Unit> result = await _current.React(data, ct);
        if (result.IsFailure) return result.Error;
        if (_next == null) return result;
        Result<Unit> nextResult = await _next.React(data, ct);
        return nextResult.IsFailure ? nextResult.Error : nextResult;
    }
}