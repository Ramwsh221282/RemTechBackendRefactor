using Identity.Application.Permissions;
using Identity.Contracts.Permissions;
using Identity.Contracts.Permissions.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Infrastructure.Permissions.EventListeners.OnCreate;

public sealed class NpgSqlPermissionCreatedEventListener(
    Serilog.ILogger logger, 
    NpgSqlPermissionsStorage storage) : 
    IOnPermissionCreatedEventListener
{
    private readonly Serilog.ILogger _logger = logger.ForContext<NpgSqlPermissionCreatedEventListener>();
    
    public async Task<Result<Unit>> React(PermissionData data, CancellationToken ct = default)
    {
        Permission? permission = await storage.Fetch(new PermissionQueryArgs(Name: data.Name), ct);
        if (permission != null)
        {
            _logger.Error("Permission with name: {Name} already exists.", data.Name);
            return Error.Conflict($"Разрешение с названием: {data.Name} уже существует.");
        }
        
        object[] logProperties = [data.Id, data.Name];
        await storage.Persist(new Permission(data), ct);
        _logger.Information("permission {Id} {Name} saved", logProperties);
        return Unit.Value;
    }
}