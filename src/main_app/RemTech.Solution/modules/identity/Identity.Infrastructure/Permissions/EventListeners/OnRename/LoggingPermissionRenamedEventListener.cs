using Identity.Contracts.Permissions;
using Identity.Contracts.Permissions.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Infrastructure.Permissions.EventListeners.OnRename;

public sealed class LoggingPermissionRenamedEventListener(
    Serilog.ILogger logger
) : IOnPermissionRenamedEventListener
{
    public Task<Result<Unit>> React(PermissionData data, CancellationToken ct = default)
    {
        object[] logProperties = [data.Id, data.Name];
        logger.Information("Permission renamed: {Id} {Name}", logProperties);
        return Task.FromResult(Result.Success(Unit.Value));
    }
}