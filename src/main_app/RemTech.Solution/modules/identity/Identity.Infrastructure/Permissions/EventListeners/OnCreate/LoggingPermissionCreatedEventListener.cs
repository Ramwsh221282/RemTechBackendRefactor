using Identity.Contracts.Permissions;
using Identity.Contracts.Permissions.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Infrastructure.Permissions.EventListeners.OnCreate;

public sealed class LoggingPermissionCreatedEventListener(Serilog.ILogger logger) : IOnPermissionCreatedEventListener
{
    private const string Context = nameof(LoggingPermissionCreatedEventListener);
    
    public Task<Result<Unit>> React(PermissionData data, CancellationToken ct = default)
    {
        object[] logProperties = [Context, data.Id, data.Name];
        logger.Information("{Context} Permission created {Id} {Name}", logProperties);
        return Task.FromResult(Result.Success(Unit.Value));
    }
}