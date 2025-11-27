using Identity.Contracts.AccountPermissions;
using Identity.Contracts.AccountPermissions.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Infrastructure.AccountPermissions.EventListeners.OnRegistered;

public sealed class OnAccountPermissionRegisteredEventListenersPipeLine(
    IOnAccountPermissionCreatedEventListener listener,
    OnAccountPermissionRegisteredEventListenersPipeLine? next = null)
    : IOnAccountPermissionCreatedEventListener
{
    private readonly IOnAccountPermissionCreatedEventListener _current = listener;
    private readonly OnAccountPermissionRegisteredEventListenersPipeLine? _next = next;
    
    public async Task<Result<Unit>> React(AccountPermissionData data, CancellationToken ct = default)
    {
        Result<Unit> result = await _current.React(data, ct);
        if (result.IsFailure) return result;
        if (_next == null) return result;
        Result<Unit> nextResult = await _next.React(data, ct);
        return nextResult.IsFailure ? nextResult.Error : nextResult;
    }
}