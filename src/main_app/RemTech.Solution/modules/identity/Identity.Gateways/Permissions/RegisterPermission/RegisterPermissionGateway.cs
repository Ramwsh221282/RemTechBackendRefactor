using Identity.Core;
using Identity.Core.Permissions;
using Identity.Infrastructure.Logging;
using Identity.Infrastructure.NpgSql;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Permissions.RegisterPermission;

public sealed class RegisterPermissionGateway(
    NpgSqlPermissionsStorage storage,
    PermissionsLogger logger,
    PermissionRegisteredResponseWaiter responseWaiter,
    EventsStore events)
    : IGateway<RegisterPermissionRequest, RegisterPermissionResponse>
{
    private readonly NpgSqlPermissionsStorage _storage = storage;
    private readonly PermissionsLogger _logger = logger;
    private readonly PermissionRegisteredResponseWaiter _waiter = responseWaiter;
    private readonly EventsStore _events = events;
    
    public async Task<Result<RegisterPermissionResponse>> Execute(RegisterPermissionRequest request)
    {
        return await new AsyncOperation<RegisterPermissionResponse>(async () =>
        {
            Permission permission = new(Guid.NewGuid(), request.Name, _events);
            permission.Register();
            await _storage.ProcessDatabaseOperations(request.Ct);
            _logger.ProcessLogging();
            return _waiter.ReadResult();
        }).Process();
    }
}