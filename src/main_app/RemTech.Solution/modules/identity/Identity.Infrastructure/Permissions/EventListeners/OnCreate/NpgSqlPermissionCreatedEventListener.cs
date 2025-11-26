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
    private const string Context = nameof(NpgSqlPermissionCreatedEventListener);
    
    public async Task<Result<Unit>> React(PermissionData data, CancellationToken ct = default)
    {
        Permission? permission = await storage.Fetch(new PermissionQueryArgs(Name: data.Name), ct);
        if (permission != null)
        {
            logger.Error("{Context} permission with name: {Name} already exists.", Context, data.Name);
            return Error.Conflict($"Разрешение с названием: {data.Name} уже существует.");
        }
        
        object[] logProperties = [Context, data.Id, data.Name];
        await storage.Update(new Permission(data), ct);
        logger.Information("{Context} permission {Id} {Name}  saved", logProperties);
        return Unit.Value;
    }
}